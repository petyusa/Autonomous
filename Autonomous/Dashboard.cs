using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Autonomous.Impl.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Autonomous.Impl.Scoring;

namespace Autonomous.Impl
{
    internal class Dashboard
    {
        private readonly ScoreCalculator _scoreCalculator;
        private readonly ScoreFormatter _scoreFormatter;
        private SpriteFont _fontSmall;
        private SpriteFont _fontMedium;
        private SpriteFont _fontLarge;
        private SpriteBatch _spriteBatch;
        private Texture2D _rect;
        private Vector2 _coor;

        public Dashboard(ScoreCalculator scoreCalculator, ScoreFormatter scoreFormatter)
        {
            _scoreCalculator = scoreCalculator;
            _scoreFormatter = scoreFormatter;
        }

        public void LoadContent(ContentManager content)
        {
            _fontSmall = content.Load<SpriteFont>("fonts/FontDashboard.small");
            _fontMedium = content.Load<SpriteFont>("fonts/FontDashboard.medium");
            _fontLarge = content.Load<SpriteFont>("fonts/FontDashboard.large");
        }

        public void DrawText(GraphicsDevice graphics, string text, Color color)
        {
            if (_spriteBatch == null)
                _spriteBatch = new SpriteBatch(graphics);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_fontLarge, text, new Vector2(graphics.Viewport.Width / 2 - 50, graphics.Viewport.Height / 2),  color);
            _spriteBatch.End();

            graphics.BlendState = BlendState.Opaque;
            graphics.RasterizerState = RasterizerState.CullCounterClockwise;
            graphics.DepthStencilState = DepthStencilState.Default;
        }

        public void DrawStart(GraphicsDevice graphics)
        {
            DrawText(graphics, "START");
        }

        public void DrawEnd(GraphicsDevice graphics)
        {
            DrawText(graphics, "FINISH");
        }

        public void DrawText(GraphicsDevice graphics, string text)
        {
            DrawText(graphics, text, Color.White);
        }

        public void DrawPlayerName(GraphicsDevice graphics, Car player, int playerIndex)
        {
            if (_spriteBatch == null)
                _spriteBatch = new SpriteBatch(graphics);
                   
            _spriteBatch.Begin();

            string text = $"Camera: {player.PlayerName}";

            _spriteBatch.DrawString(_fontMedium, text, new Vector2(graphics.Viewport.Width / 2 - 50, 10), player.Color);
            _spriteBatch.DrawString(_fontMedium, text, new Vector2(graphics.Viewport.Width / 2 - 49, 11), Color.Black);
            _spriteBatch.End();

            graphics.BlendState = BlendState.Opaque;
            graphics.RasterizerState = RasterizerState.CullCounterClockwise;
            graphics.DepthStencilState = DepthStencilState.Default;

        }

        public void Draw(GraphicsDevice graphics, IList<Car> players, string fps, TimeSpan totalGameTime)
        {
            if (_spriteBatch == null)
                _spriteBatch = new SpriteBatch(graphics);

            _spriteBatch.Begin();

            const int width = 400;
            int height = players.Count() * 25 + 25;

            DrawRectangle(graphics, width, height, 0, 0);

            DrawScores(players, totalGameTime);

            DrawFps(positionY: 5 + players.Count() * 20, fps: fps);

            _spriteBatch.End();

            graphics.BlendState = BlendState.Opaque;
            graphics.RasterizerState = RasterizerState.CullCounterClockwise;
            graphics.DepthStencilState = DepthStencilState.Default;

        }

        private void DrawFps(float positionY, string fps)
        {
            _spriteBatch.DrawString(_fontSmall, fps, new Vector2(10, positionY), Color.Green);
        }

        private void DrawScores(IList<Car> players, TimeSpan totalGameTime)
        {
            if (players == null) return;

            var playerScores = _scoreCalculator.GetPlayerScores(players, totalGameTime);

            int line = 0;
            foreach (var score in playerScores)
            {
                var player = players.FirstOrDefault(p =>
                    p.PlayerName.Equals(score.PlayerName, StringComparison.InvariantCultureIgnoreCase));

                var color = player?.Color ?? Color.Azure;

                _spriteBatch.DrawString(_fontSmall,
                    _scoreFormatter.GetFormattedScore(score),
                    new Vector2(10, 5 + line * 20),
                    color);

                ++line;
            }
        }

        private void DrawRectangle(GraphicsDevice graphics, int width, int height, int x, int y)
        {
            if (_rect == null)
            {
                _rect = new Texture2D(graphics, width, height);
                Color[] data = new Color[width * height];

                _coor = new Vector2(x, y);

                for (int j = 0; j < data.Length; ++j)
                    data[j] = Color.FromNonPremultiplied(0, 0, 0, 100);

                _rect.SetData(data);
            }
            _spriteBatch.Draw(_rect, _coor, Color.Black);

        }

    }
}
