using System;
using System.CodeDom;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Autonomous.Impl.GameObjects;

namespace Autonomous.Impl
{
    class CourseObjectFactory
    {
        private Model treeModel;
        private Model barrierModel;
        private Model cityModel;
        private Model terrainModel;
        private Model buildingModelA;
        private Model buildingModelB;
        private readonly Dictionary<Type, float> lastObjectPositions = new Dictionary<Type, float>();

        public void LoadContent(ContentManager content)
        {
            treeModel = content.Load<Model>("Tree\\fir");
            barrierModel = content.Load<Model>("barrier");
            cityModel = content.Load<Model>("City/The City");
            terrainModel = content.Load<Model>("mountain/mountains");
            buildingModelA = content.Load<Model>("BuildingA");
            buildingModelB = content.Load<Model>("Buildings/BuildingC/Build");
        }

        public IEnumerable<GameObject> GenerateCourseArea(float positionY = 0)
        {
            var newObjects = new List<GameObject>();
            newObjects.AddRange(GenerateTrees(positionY));
            //newObjects.AddRange(GenerateBarriers(positionY));
            newObjects.AddRange(GenerateCity(positionY));
            newObjects.AddRange(GenerateTerrain(positionY));
            newObjects.AddRange(GenerateBuildings(positionY));
            return newObjects;
        }

        private IEnumerable<Tree> GenerateTrees(float areaPositionY = 0, int count = 10)
        {
            const float frequency = 40f;
            const float firstTreePositionY = 180f;
            const float cityStartPositionY = 300f;
            const float positionXNear = 8f;
            const float positionXFar = 9f;
            const float smallWidth = 3.2f;
            const float largeWidth = 4.5f;

            if (IsNewObjectGenerationRequried<Tree>(areaPositionY))
            {
                for (int i = 0; i < count; i++)
                {
                    float x = i % 4 == 0 ? positionXNear : positionXFar;
                    float widthLeft = i % 2 == 0 ? largeWidth : smallWidth;
                    float widthRight = i % 3 == 0 ? smallWidth : largeWidth;

                    var positionY = GetNextPosition<Tree>(frequency, firstTreePositionY);

                    if (positionY < cityStartPositionY)
                    {
                        widthLeft += 2f;
                        widthRight += 2f;
                    }

                    if (positionY >= firstTreePositionY)
                    {
                        yield return new Tree(treeModel, x, positionY + 10, widthLeft);
                        yield return new Tree(treeModel, -x, positionY + 10, widthRight);
                    }
                }
            }
        }

        private IEnumerable<Barrier> GenerateBarriers(float areaPositionY = 0, int count = 20)
        {
            float firtBarrierPositionY = 400;
            float lastBarrierPositionY = 900;
            float positionX = 6.3f;
            float frequency = 1.8f;

            if (lastBarrierPositionY > areaPositionY &&
                areaPositionY >= firtBarrierPositionY &&
                IsNewObjectGenerationRequried<Barrier>(areaPositionY))
            {
                for (int i = 0; i < count; i++)
                {
                    var positionY = GetNextPosition<Barrier>(frequency, firtBarrierPositionY);

                    yield return new Barrier(barrierModel, positionX, positionY);
                    yield return new Barrier(barrierModel, -positionX, positionY);
                }
            }
        }

        private IEnumerable<City> GenerateCity(float areaPositionY = 0, int count = 2)
        {
            const float cityStartPositionY = 600;
            const float frequency = 300f;

            if (cityStartPositionY < areaPositionY && IsNewObjectGenerationRequried<City>(areaPositionY))
            {
                for (int i = 0; i < count; i++)
                {
                    float positionY = GetNextPosition<City>(frequency, cityStartPositionY);
                    yield return new City(cityModel, 0f, positionY);
                }
            }
        }

        private IEnumerable<Terrain> GenerateTerrain(float areaPositionY = 0, int count = 4)
        {
            const float frequency = 180f;
            const float terrainPositionX = -10f;

            if (IsNewObjectGenerationRequried<Terrain>(areaPositionY))
            {
                for (int i = 0; i < count; i++)
                {
                    float positionY = GetNextPosition<Terrain>(frequency);
                    yield return new Terrain(terrainModel, terrainPositionX, positionY);
                }
            }
        }

        private IEnumerable<BuildingA> GenerateBuildings(float areaPositionY = 0, int count = 20)
        {
            const float buildingsStartPositionLeftY = 600f;
            const float buildingsStartPositionRightY = 420f;
            const float frequency = 30f;
            
            if (buildingsStartPositionRightY < areaPositionY && IsNewObjectGenerationRequried<BuildingA>(areaPositionY))
            {
                var random = new Random(1000);
                for (int i = 0; i < count; i++)
                {
                    float positionY = GetNextPosition<BuildingA>(frequency, buildingsStartPositionRightY);
                    float rotationLeft = i % 3 == 0 ? 90 : 180;
                    float rotationRight = i % 2 == 0 ? 90 : 180;
                    float x = i % 4 == 0 ? 11 : 13;
                    float widthLeft = i % 2 == 0 ? 6f : 7f;
                    float widthRight = i % 3 == 0 ? 7f : 6f;
                    
                    var model = random.Next(0, 200) % 2 == 0 ? buildingModelB : buildingModelA;

                    if (positionY >= buildingsStartPositionLeftY)
                        yield return new BuildingA(model, x, positionY, rotationLeft, widthLeft);

                    if (positionY >= buildingsStartPositionRightY)
                        yield return new BuildingA(model, -x, positionY, rotationRight, widthRight);

                }
            }
        }

        private bool IsNewObjectGenerationRequried<T>(float positionY = 0)
        {
            if (!lastObjectPositions.TryGetValue(typeof(T), out var lastPositionY))
            {
                return true;
            }

            return Math.Abs(positionY - lastPositionY) < 300f;
        }

        private float GetNextPosition<T>(float offsetY, float initialPosition = 0f) where T : GameObject
        {
            float newPosition = initialPosition;
            if (lastObjectPositions.TryGetValue(typeof(T), out var positionY))
            {
                newPosition = positionY + offsetY;
            }

            lastObjectPositions[typeof(T)] = newPosition;

            return newPosition;
        }
    }
}
