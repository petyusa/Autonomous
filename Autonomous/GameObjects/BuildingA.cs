using Microsoft.Xna.Framework.Graphics;

namespace Autonomous.Impl.GameObjects
{
    public class BuildingA : GameObject
    {
        public BuildingA(Model model, float x, float y, float rotation = 90, float width = 7f)
            : base(model, false)
        {
            X = x;
            Y = y;
            ModelRotate = rotation;
            Width = width;
        }
    }
}
