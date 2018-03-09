using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Autonomous.Public;
using Microsoft.Xna.Framework;

namespace Autonomous.Impl.GameObjects
{
    public class Car : GameObject
    {
        private readonly GameStateManager _gameStateManager;
        private TimeSpan _lastCollision;
        private int _collisions;
        public string PlayerName { get; }

        public Car(Model model, string playerId, string playerName, GameStateManager gameStateManager, Color color, float x = 0, float y = 0)
            : base(model, true)
        {
            _gameStateManager = gameStateManager;
            Color = color;
            Model = model;
            Width = GameConstants.PlayerWidth;
            ModelRotate = 180;
            X = x;
            Y = y;
            MaxVY = GameConstants.PlayerMaxSpeed;
            Id = playerId;
            Type = GameObjectType.Player;
            PlayerName = playerName;
            MaxX = 6f;
        }

        public Color Color { get; }
        public float Damage { get; private set; } = 0f;
        public TimeSpan? LastCollision => _lastCollision;

        public override void Update(GameTime gameTime)
        {
            if (Stopped) return;

            var command = _gameStateManager.GetPlayerCommand(Id);
            AccelerationY = 0;

            if (command.Acceleration > 0)
                AccelerationY = Math.Min(command.Acceleration, 1) * GameConstants.PlayerAcceleration;
            else
                AccelerationY = Math.Max(command.Acceleration, -1) * GameConstants.PlayerDeceleration;

            VX = 0;
            if (command.MoveLeft)
                VX -= GameConstants.PlayerHoriztontalSpeed;
            if (command.MoveRight)
                VX += GameConstants.PlayerHoriztontalSpeed;

            base.Update(gameTime);

            if (MaxX.HasValue && X - Width / 2 < -MaxX)
            {
                X = -MaxX.Value + Width / 2;
                HandleCollision(null, gameTime);
            }

            if (MaxX.HasValue && X + Width / 2 > MaxX)
            {
                X = MaxX.Value - Width / 2;
                HandleCollision(null, gameTime);
            }
        }

        public bool HadCollisionIn(GameTime gameTime, double interval)
        {
            return _collisions > 0 && (gameTime.TotalGameTime - _lastCollision).TotalMilliseconds < interval;
        }

        public override void HandleCollision(GameObject other, GameTime gameTime)
        {
            base.HandleCollision(other, gameTime);
            const double collisionPenalityInterval = 1000;
            if (!HadCollisionIn(gameTime, collisionPenalityInterval))
            {
                Damage = Math.Min((Damage + 0.1f), 1f);                
                const float speedLossFactor = 3f;
                var maxSpeed = GameConstants.PlayerMaxSpeed;
                float speedLoss = (maxSpeed * Damage / speedLossFactor);
                MaxVY = Math.Abs(Damage - 1f) < 0.0001f ? 0 : maxSpeed - speedLoss;

                if (other == null)
                {
                    VY = VY / 2;
                }

                VY = Math.Min(VY, MaxVY);

                ++_collisions;
                _lastCollision = gameTime.TotalGameTime;
            }
        }
    }
}
