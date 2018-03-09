using Autonomous.Public;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Autonomous.Impl.GameObjects;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Xna.Framework;

namespace Autonomous.Impl
{
    public class PlayerFactory
    {
        [ImportMany]
        IEnumerable<Lazy<IPlayer, IPlayerData>> _players;

        private CompositionContainer _container;
        private Model _porsheModel;
        private Model _lamborginiModel;
        private Model _porshe911Model;
        private Model _mazdaModel;
        private IList<Model> _carModels;
        private int _carModelIndex = 0;
        private readonly List<Color> _colors;

        public PlayerFactory()
        {
            _colors = new List<Color>
            {   Color.LightCyan,
                Color.Orange,
                Color.LightBlue,
                Color.LightGreen
            };
        }

        public IEnumerable<IPlayerData> PlayersInfo
        {
            get
            {
                foreach (Lazy<IPlayer, IPlayerData> player in _players)
                {
                    yield return player.Metadata;
                }
            }
        }

        public int HumanPlayerIndex
        {
            get
            {
                int index = 0;
                foreach (var item in PlayersInfo)
                {
                    if (item.PlayerName.Equals("Human", StringComparison.InvariantCultureIgnoreCase))
                        return index;

                    ++index;
                }

                return -1;
            }
        }

        public void LoadContent(ContentManager content)
        {
            _porsheModel = content.Load<Model>("Cars/Porshe/carrgt");
            _lamborginiModel = content.Load<Model>("Lambo/Lamborghini_Aventador");
            _porshe911Model = content.Load<Model>("Cars/Porshe911/Porsche_911_GT2");
            _mazdaModel = content.Load<Model>("Cars/mazda/Mazda_RX-8-By_Decan");

            _carModels = new List<Model>
            {
                _lamborginiModel,
                _porshe911Model,
                _porsheModel,
                _mazdaModel
            };
        }

        public IEnumerable<Car> LoadPlayers(GameStateManager gameStateManager)
        {
            //An aggregate catalog that combines multiple catalogs  
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class  
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(PlayerFactory).Assembly));

            catalog.Catalogs.Add(new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory));

            //Create the CompositionContainer with the parts in the catalog  
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object  
            this._container.ComposeParts(this);

            int playerIndex = 0;
            foreach (var player in ShuffledPlayers)
            {
                var model = GetNextCar();
                var x = GameConstants.LaneWidth * GameConstants.PlayerWidth - playerIndex * GameConstants.LaneWidth;
                var y = playerIndex * 2f;
                string id = Guid.NewGuid().ToString();
                PlayerGameLoop.StartGameLoop(player.Value, id, gameStateManager);
                var color = GetColorByIndex(playerIndex);

                playerIndex++;
                yield return new Car(model, id, player.Metadata.PlayerName, gameStateManager, color, x, y);                
            }
        }

        private IEnumerable<Lazy<IPlayer, IPlayerData>> ShuffledPlayers
        {
            get
            {
                var random = new Random();
                return _players.OrderBy(x => random.Next()).ToArray();
            }
        }

        private Model GetNextCar()
        {
            if (_carModels == null)
                return null;
            var model = _carModels[_carModelIndex];
            _carModelIndex = (++_carModelIndex) % _carModels.Count;
            return model;
        }

        private Color GetColorByIndex(int index)
        {
            return _colors[Math.Min(index, _colors.Count - 1)];
        }

    }
}
