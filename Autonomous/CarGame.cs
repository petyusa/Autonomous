using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autonomous.Impl.Commands;
using Autonomous.Public;
using Autonomous.Impl.Viewports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Autonomous.Impl.GameObjects;
using Autonomous.Impl.Scoring;

namespace Autonomous.Impl
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class CarGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly ViewportManager _viewportManager;
        private List<GameObject> _gameObjects = new List<GameObject>();
        private Road _road;
        private FinishLine _finishline;
        private readonly AgentFactory _agentFactory;
        private readonly CourseObjectFactory _courseObjectFactory;
        private readonly PlayerFactory _playerFactory;
        private readonly Dashboard _dashboard;
        private readonly GameStateManager _gameStateManager = new GameStateManager();
        private List<Car> _players;
        private TimeSpan _lastUpdate;
        private readonly float _length;
        private readonly float _agentDensity;
        private readonly List<IGameCommand> _gameCommands = new List<IGameCommand>();
        private bool _slowdown;
        private Texture2D _background;
        private readonly FrameCounter _frameCounter = new FrameCounter();
        private readonly ScoreCsvExporter _scoreCsvExporter;
        private readonly ScoreCalculator _scoreCalculator;
        private readonly bool _playerCollision;
        private TimeSpan failedAt;

        public CarGame(float length, float agentDensity, bool playerCollision)
        {
            _agentDensity = agentDensity;
            _length = length;
            _playerCollision = playerCollision;

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
            };
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            _agentFactory = new AgentFactory(_gameStateManager);
            _courseObjectFactory = new CourseObjectFactory();
            _playerFactory = new PlayerFactory();
            _scoreCalculator = new ScoreCalculator();
            _dashboard = new Dashboard(_scoreCalculator, new ScoreFormatter());
            _viewportManager = new ViewportManager(new ViewportFactory(_graphics));
            _scoreCsvExporter = new ScoreCsvExporter("results.csv");
        }

        public bool Stopped { get; private set; }
        public bool Failed { get; private set; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _background = Content.Load<Texture2D>("background");

            Road.LoadContent(Content, _graphics);
            FinishLine.LoadContent(Content, _graphics);

            _agentFactory.LoadContent(Content);
            _courseObjectFactory.LoadContent(Content);
            _playerFactory.LoadContent(Content);
            _dashboard.LoadContent(Content);
            // TEMP
            InitializeModel();

            _viewportManager.SetViewports(_players);
        }

        public void InitializeModel()
        {
            _road = new Road();
            _finishline = new FinishLine() { Y = _length };
            _players = _playerFactory.LoadPlayers(_gameStateManager).ToList();
            _gameObjects = new List<GameObject>(_players) { _road, _finishline };
            _gameObjects.AddRange(_courseObjectFactory.GenerateCourseArea());
            _gameObjects.AddRange(_agentFactory.GenerateInitialCarAgents(_agentDensity));
            _gameObjects.ForEach(go => go.Initialize());

            InitializeCommands();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {            
            HandleCommands(gameTime);
            UpdateModel(gameTime);
            _viewportManager.Viewports.ForEach(vp => vp.Update());

            base.Update(gameTime);
        }

        public void UpdateModel(GameTime gameTime)
        {
            // Hold cars at the start line for 2 sec
            if (gameTime.TotalGameTime.TotalMilliseconds < 2000) return;

            var agentObjects = this._gameObjects
                .Where(go => go.GetType() == typeof(Car) || go.GetType() == typeof(CarAgent) || go.GetType() == typeof(FinishLine)).OrderBy(g => g.Y);
            var internalState = new GameStateInternal() { GameObjects = agentObjects.ToList(), Stopped = Stopped, PlayerCollision = _playerCollision};
            CheckIfGameFinished(internalState, gameTime.TotalGameTime);
            if (Stopped)
                internalState.Stopped = Stopped;

            _gameStateManager.GameStateInternal = internalState;
            _gameStateManager.GameState = GameStateMapper.GameStateToPublic(internalState);
            _gameStateManager.GameStateCounter++;


            if (_slowdown)
                gameTime.ElapsedGameTime = TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds / 5);

            _gameObjects.ForEach(go => go.Update(gameTime));
            UpdateGameCourse(gameTime);
            CheckCollision(gameTime, _playerCollision);
        }

        private void CheckIfGameFinished(GameStateInternal internalState, TimeSpan gameTimeTotalGameTime)
        {
            float firstPlayerFront = internalState.FirstPlayer.BoundingBox.Bottom;
            if (firstPlayerFront >= _finishline.Y - 10)
            {
                _slowdown = true;
                _viewportManager.SetViewports(new List<GameObject>() { _finishline });
            }

            if (firstPlayerFront >= _finishline.Y)
            {
                ExportPlayerScores(gameTimeTotalGameTime);
                Stopped = true;
            }

            if (internalState.GameObjects.OfType<Car>().All(car => car.Stopped))
            {
                ExportPlayerScores(gameTimeTotalGameTime);
                if (!Failed)
                {
                    failedAt = gameTimeTotalGameTime;
                }
                Stopped = Failed = true;
            }

            if ((firstPlayerFront >= _finishline.Y + 20)
                || (Failed && (gameTimeTotalGameTime - failedAt).TotalMilliseconds > 2000))
            {
                Exit();
            }
        }

        private void ExportPlayerScores(TimeSpan totalGameTime)
        {
            try
            {
                var scores = _scoreCalculator.GetPlayerScores(_players, totalGameTime);
                _scoreCsvExporter.ExportScoresToCsv(scores);
            }
            catch (IOException error)
            {
                Console.WriteLine($"Cannot export results to CSV: {error.Message}");
            }
        }

        private void InitializeCommands()
        {
            _gameCommands.Add(new ExitCommand(Exit));
            _gameCommands.Add(new AutoViewportSelectionCommand(_viewportManager, _players, _playerFactory.HumanPlayerIndex));
            _gameCommands.Add(new ManualViewportSelectionCommand(_viewportManager, _players));
        }

        private void HandleCommands(GameTime gameTime)
        {
            _gameCommands.ForEach(command => command.Handle(gameTime));
        }

        private void CheckCollision(GameTime gameTime, bool handlePlayerCollision)
        {
            foreach (var player in _players)
            {
                var agentsInCollision = _gameObjects
                    .OfType<CarAgent>()
                    .Where(x => CollisionDetector.IsCollision(x, player));

                foreach (var agent in agentsInCollision)
                {
                    agent.HandleCollision(player, gameTime);
                    player.HandleCollision(agent, gameTime);
                }

                if (handlePlayerCollision)
                {
                    var playersInCollision = _gameObjects
                        .OfType<Car>()
                        .Where(x => !x.Id.Equals(player.Id) 
                                && CollisionDetector.IsCollision(x, player));

                    foreach (var other in playersInCollision)
                    {
                        other.HandleCollision(player, gameTime);
                        player.HandleCollision(other, gameTime);
                    }
                }
            }
        }

        private void UpdateGameCourse(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime - _lastUpdate).TotalMilliseconds < GameConstants.GameCourseUpdateFrequency)
                return;

            var newObjects = _courseObjectFactory
                .GenerateCourseArea(_gameStateManager.GameStateInternal.FirstPlayer.Y)
                .ToList();

            int firstPlayerIndex = _gameStateManager.GameStateInternal.FirstPlayerIndex;
            newObjects.AddRange(GenerateAgents(firstPlayerIndex));
            _gameObjects.AddRange(newObjects);
            newObjects.ForEach(go => go.Initialize());

            var lastCarPosition = _gameStateManager.GameStateInternal.LastPlayer.Y;

            const float dinstanceToRemove = 50f;

            var objectsToRemove = _gameObjects
                .Where(go => !(go.GetType() == typeof(CarAgent) && go.VY >= 0) &&
                             go.GetType() != typeof(Road) &&
                             go.GetType() != typeof(Car))
                .Where(go =>
                    go.BoundingBox.Top <= lastCarPosition &&
                    Math.Abs(go.BoundingBox.Top - lastCarPosition) > dinstanceToRemove
                ).ToList();

            objectsToRemove.ForEach(go => _gameObjects.Remove(go));

            _lastUpdate = gameTime.ElapsedGameTime;
        }

        private IEnumerable<GameObject> GenerateAgents(int firstPlayerIndex)
        {
            var objects = _gameStateManager.GameStateInternal.GameObjects;
            float? sameDirY = null;
            float? oppositeY = null;


            float firstPlayerPosition = objects[firstPlayerIndex].Y;

            int closeCount = 0;
            for (int i = objects.Count - 1; i > firstPlayerIndex; i--)
            {
                if (objects[i].OppositeDirection && oppositeY == null)
                    oppositeY = objects[i].Y;

                if (!objects[i].OppositeDirection && sameDirY == null)
                {
                    sameDirY = objects[i].Y;
                }

                if (!objects[i].OppositeDirection && objects[i].Y - firstPlayerPosition < 500)
                {
                    closeCount++;
                }
            }

            if (sameDirY - firstPlayerPosition < 500)
                yield return _agentFactory.GenerateRandomAgent(sameDirY.GetValueOrDefault(firstPlayerPosition), false, _agentDensity);

            if (oppositeY - firstPlayerPosition < 500)
                yield return _agentFactory.GenerateRandomAgent(oppositeY.GetValueOrDefault(firstPlayerPosition), true, _agentDensity);

            if (closeCount < 2 + 10 * _agentDensity)
                yield return _agentFactory.GenerateRandomAgent(firstPlayerPosition + 200, false, _agentDensity);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(78, 55, 38, 255));

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _frameCounter.Update(deltaTime);

            var fps = $"FPS: {_frameCounter.AverageFramesPerSecond}";

            Viewport original = _graphics.GraphicsDevice.Viewport;

            int viewPortIndex = 0;
            foreach (var viewport in _viewportManager.Viewports)
            {
                _graphics.GraphicsDevice.Viewport = viewport.Viewport;

                if (!(viewport is BirdsEyeViewport))
                    DrawBackground();
                Draw(gameTime, viewport);

                if (!(viewport is BirdsEyeViewport))
                {
                    if (viewport.GameObject is Car player)
                    {
                        DrawPlayerInformation(gameTime, viewPortIndex, player);
                    }
                    DrawDashboard(gameTime);

                    ++viewPortIndex;
                }
            } 

            _graphics.GraphicsDevice.Viewport = original;

            _dashboard.Draw(_graphics.GraphicsDevice, _players, fps, gameTime.TotalGameTime);

            base.Draw(gameTime);
        }

        private void DrawPlayerInformation(GameTime gameTime, int viewPortIndex, Car player)
        {
            _dashboard.DrawPlayerName(_graphics.GraphicsDevice, player, viewPortIndex);
            if (player.HadCollisionIn(gameTime, 2000))
            {
                _dashboard.DrawText(_graphics.GraphicsDevice, $"BANGGGG!", Color.Red);
            }
        }

        private void DrawDashboard(GameTime gameTime)
        {
            if (!Stopped && gameTime.TotalGameTime.TotalSeconds < 3)
                _dashboard.DrawStart(_graphics.GraphicsDevice);

            if (Stopped && !Failed)
                _dashboard.DrawEnd(_graphics.GraphicsDevice);

            if (Stopped && Failed)
                _dashboard.DrawText(_graphics.GraphicsDevice, "GAME OVER!");

        }

        private void DrawBackground()
        {
            var backgroundSpriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundSpriteBatch.Begin();

            backgroundSpriteBatch.Draw(_background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2), Color.White);

            backgroundSpriteBatch.End();
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        private void Draw(GameTime gameTime, ViewportWrapper viewport)
        {
            _gameObjects.ForEach(go => go.Draw(gameTime.ElapsedGameTime, viewport, GraphicsDevice));
        }
    }
}
