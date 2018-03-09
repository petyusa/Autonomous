using System;
using System.Collections.Generic;
using Autonomous.Public;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Autonomous.Impl.GameObjects;
using Autonomous.Impl.Strategies;

namespace Autonomous.Impl
{
    class AgentFactory
    {
        private Model _vanModel;
        private Model _lamboModel;
        private Model _busModel;
        private Model _barrierModel;
        private Model _prideModel;
        private Model _peugeotModel;
        private Model _porsheModel;
        private Model _porshe911Model;
        private Model _taxiModel;
        private Model _policeModel;
        private Model _pickupModel;
        private Model _cabrioModel;
        private Model _sedanModel;
        private Model _ambulanceModel;
        private Model _busStopModel;
        private IGameStateProvider _gameStateProvider;
        private Random random = new Random();
        private List<Func<bool, float, CarAgent>> _agentCreators = new List<Func<bool, float, CarAgent>>();

        public AgentFactory(IGameStateProvider stateProvider)
        {
            _gameStateProvider = stateProvider;

            _agentCreators.Add(CreateBarrier);
            _agentCreators.Add(CreateVan);
            _agentCreators.Add(CreateBus);
            _agentCreators.Add(CreatePeugeot);
            _agentCreators.Add(CreatePride);
            _agentCreators.Add(CreateBusStop);
            _agentCreators.Add(CreateSedan);
            _agentCreators.Add(CreateTaxi);
            _agentCreators.Add(CreatePolice);
            _agentCreators.Add(CreatePickup);
            _agentCreators.Add(CreateCabrio);
            _agentCreators.Add(CreateAmbulance);
        }

        public void LoadContent(ContentManager content)
        {
            _vanModel = content.Load<Model>("kendo");
            _lamboModel = content.Load<Model>("Lambo\\Lamborghini_Aventador");
            _busModel = content.Load<Model>("bus");
            _barrierModel = content.Load<Model>("barrier");
            _prideModel = content.Load<Model>("cars\\pride\\pride_400");
            _peugeotModel = content.Load<Model>("cars\\glx_400\\glx_400");
            _busStopModel = content.Load<Model>("busstop\\bus_stop");
            _porsheModel = content.Load<Model>("Cars/Porshe/carrgt");
            _porshe911Model = content.Load<Model>("Cars/Porshe911/Porsche_911_GT2");
            _taxiModel = content.Load<Model>("Cars/taxi/cars");
            _policeModel = content.Load<Model>("Cars/police/cars");
            _pickupModel = content.Load<Model>("Cars/pickup/cars");
            _cabrioModel = content.Load<Model>("Cars/cabrio/cars");
            _sedanModel = content.Load<Model>("Cars/blacksedan/cars");
            _ambulanceModel = content.Load<Model>("Cars/ambulance/cars");
        }

        public IEnumerable<GameObject> GenerateInitialCarAgents(float agentDensity)
        {

            float y = 0;
            for (int i = 0; i < 10; i++)
            {
                y += GetRandomDistance(agentDensity);
                var index = random.Next(_agentCreators.Count);
                yield return _agentCreators[index](false, y);
            }

            y = 0;
            for (int i = 0; i < 10; i++)
            {
                y += GetRandomDistance(agentDensity);
                var index = random.Next(_agentCreators.Count);
                yield return _agentCreators[index](true, y);
            }

        }

        private float GetRandomDistance(float agentDensity)
        {
            float minDist = 20 + (1 - agentDensity) * 20;
            float maxDist = 60 + (1 - agentDensity) * 60;
            return (float)random.NextDouble() * (maxDist - minDist) + minDist;
        }

        public GameObject GenerateRandomAgent(float miny, bool opposite, float agentDensity)
        {
            var y = GetRandomDistance(agentDensity) + miny;
            var index = random.Next(_agentCreators.Count);
            return _agentCreators[index](opposite, y);
        }

        const float laneWidth = GameConstants.LaneWidth;
        public CarAgent CreateBarrier(bool opposite, float y)
        {
            int lane = random.NextDouble() < 0.7 ? 0 : 1;
            var barrier = new CarAgent(_barrierModel, 90, laneWidth * 0.8f, 0.79f, opposite, GameObjectType.Roadblock, _gameStateProvider, null)
            {
                VY = 0,
                MaxVY = 0,
                X = GetLaneCenter(lane, opposite),
                Y = y
            };
            return barrier;
        }

        public CarAgent CreateBusStop(bool opposite, float y)
        {
            int lane = 0;
            float width = 2f;
            const float height = 2.77f;
            float x = GameConstants.RoadWidth / 2 + width / 2 + 0.1f;
            var barrier = new CarAgent(_busStopModel, 270, width, height, opposite, GameObjectType.BusStop, _gameStateProvider, null)
            {
                VY = 0,
                MaxVY = 0,
                X = opposite ? -x : x,
                Y = y
            };
            return barrier;
        }

        public CarAgent CreateVan(bool opposite, float y)
        {
            int lane = 0;
            const float vanWidth = 2.1f;
            const float height = 3.97f;

            float v = ((float)random.NextDouble() * 20 + 70) / 3.6f;
            return CreateVehicleAgent(_vanModel, lane, opposite, y, vanWidth, height, v, 90);
        }

        public CarAgent CreateTaxi(bool opposite, float y)
        {
            int lane = 1;
            const float width = 1.81f;
            const float height = 4.32f;

            float v = ((float)random.NextDouble() * 20 + 70) / 3.6f;
            return CreateVehicleAgent(_taxiModel, lane, opposite, y, width, height, v, 180);
        }

        public CarAgent CreatePolice(bool opposite, float y)
        {
            int lane = 1;
            const float width = 1.82f;
            const float height = 4.4f;

            float v = ((float)random.NextDouble() * 20 + 70) / 3.6f;
            return CreateVehicleAgent(_policeModel, lane, opposite, y, width, height, v, 180);
        }

        public CarAgent CreatePickup(bool opposite, float y)
        {
            int lane = 0;
            const float width = 1.83f;
            const float height = 4.16f;

            float v = ((float)random.NextDouble() * 20 + 70) / 3.6f;
            return CreateVehicleAgent(_pickupModel, lane, opposite, y, width, height, v, 180);
        }

        public CarAgent CreateCabrio(bool opposite, float y)
        {
            int lane = 0;
            const float width = 1.79f;
            const float height = 3.79f;

            float v = ((float)random.NextDouble() * 20 + 70) / 3.6f;
            return CreateVehicleAgent(_cabrioModel, lane, opposite, y, width, height, v, 180);
        }

        public CarAgent CreateSedan(bool opposite, float y)
        {
            int lane = 0;
            const float width = 1.8f;
            const float height = 4.27f;

            float v = ((float)random.NextDouble() * 20 + 70) / 3.6f;
            return CreateVehicleAgent(_sedanModel, lane, opposite, y, width, height, v, 180);
        }

        public CarAgent CreateAmbulance(bool opposite, float y)
        {
            int lane = 0;
            const float width = 2.11f;
            const float height = 6.13f;

            float v = ((float)random.NextDouble() * 20 + 70) / 3.6f;
            return CreateVehicleAgent(_ambulanceModel, lane, opposite, y, width, height, v, 180);
        }

        public CarAgent CreatePride(bool opposite, float y)
        {
            int lane = 0;
            const float width = 1.45f;
            const float height = 3.06f;
            float v = ((float)random.NextDouble() * 20 + 70) / 3.6f;
            return CreateVehicleAgent(_prideModel, lane, opposite, y, width, height, v, 180);
        }

        public CarAgent CreatePeugeot(bool opposite, float y)
        {
            int lane = 1;
            const float width = 1.65f;
            const float height = 3.41f;

            float v = ((float)random.NextDouble() * 30 + 70) / 3.6f;
            return CreateVehicleAgent(_peugeotModel, lane, opposite, y, width, height, v, 0);
        }

        public CarAgent CreateLambo(bool opposite, float y)
        {
            int lane = 1;
            const float width = 2f;
            const float height = 4.25f;
            float v = ((float)random.NextDouble() * 20 + 90) / 3.6f;
            return CreateVehicleAgent(_lamboModel, lane, opposite, y, width, height, v, 180);
        }

        public CarAgent CreatePorshe911(bool opposite, float y)
        {
            int lane = 1;
            const float width = 2f;
            const float height = 4.25f;
            float v = ((float)random.NextDouble() * 20 + 90) / 3.6f;
            return CreateVehicleAgent(_porshe911Model, lane, opposite, y, width, height, v, 180);
        }

        public CarAgent CreatePorshe(bool opposite, float y)
        {
            int lane = 1;
            const float width = 2f;
            const float height = 4.25f;
            float v = ((float)random.NextDouble() * 20 + 90) / 3.6f;
            return CreateVehicleAgent(_porsheModel, lane, opposite, y, width, height, v, 180);
        }


        public CarAgent CreateBus(bool opposite, float y)
        {
            int lane = 0;
            const float width = 2.6f;
            const float height = 9.35f;
            float v = ((float)random.NextDouble() * 20 + 50) / 3.6f;

            var drivingStrategy = new BusStrategy(v, lane, _gameStateProvider);
            return CreateVehicleAgent(_busModel, lane, opposite, y, width, height, v, 180, drivingStrategy);
        }

        private CarAgent CreateVehicleAgent(Model model, int lane, bool opposite, float y, float vanWidth, float height, float v, float rotation, IControlStrategy drivingStrategy = null)
        {
            if (drivingStrategy == null)
                drivingStrategy = new OvertakingStrategy(v, lane, _gameStateProvider);

            var van = new CarAgent(model, rotation, vanWidth, height, opposite, GameObjectType.Car, _gameStateProvider, drivingStrategy)
            {
                VY = v,
                MaxVY = 100f / 3.6f,
                X = GetLaneCenter(lane, opposite),
                Y = y
            };
            return van;
        }

        private float GetLaneCenter(int lane, bool opposite)
        {
            float x = 0;
            if (lane == 0)
                x = GameConstants.LaneWidth * 1.45f;
            else
                x = GameConstants.LaneWidth / 2;

            if (opposite)
                x = -x;

            return x;
        }
    }
}
