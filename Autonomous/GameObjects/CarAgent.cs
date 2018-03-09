using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Autonomous.Impl.Strategies;
using Autonomous.Public;

namespace Autonomous.Impl.GameObjects
{
    public class CarAgent : GameObject
    {
        private IGameStateProvider _gameStateProvider;
        private IControlStrategy _strategy;

        public CarAgent(Model model, float modelRotate, float width, float height, bool opposite, GameObjectType type,  IGameStateProvider gameStateProvider, IControlStrategy drivingStrategy)
            : base(model, true)
        {
            _gameStateProvider = gameStateProvider;
            Width = width;
            HardCodedHeight = height;
            ModelRotate = modelRotate;            
            OppositeDirection = opposite;
            if (drivingStrategy != null)
            {
                _strategy = drivingStrategy;
                _strategy.GameObject = this;
            }
            Type = type;
            Id = Guid.NewGuid().ToString();
        }

        public override void Update(GameTime gameTime)
        {
            if (_strategy != null)
            {
                var state = _strategy.Calculate();
                AccelerationY = state.Acceleration > 0
                    ? GameConstants.PlayerAcceleration * state.Acceleration
                    : GameConstants.PlayerDeceleration * state.Acceleration;

                VX = state.HorizontalSpeed;
            }

            base.Update(gameTime);
        }
    }
}
