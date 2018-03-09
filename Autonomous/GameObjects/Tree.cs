using Microsoft.Xna.Framework.Graphics;

namespace Autonomous.Impl.GameObjects
{
    public class Tree : GameObject
    {
        public Tree(Model model, float x, float y, float width = 5)
            : base(model, false)
        {
            Width = width;
            X = x;
            Y = y;
        }
    }
}
