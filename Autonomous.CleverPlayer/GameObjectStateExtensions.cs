using Autonomous.Public;

namespace Autonomous.CleverPlayer
{
    public static class GameObjectStateExtensions
    {
        public static float GetLane(this GameObjectState thisObject)
        {
            var objectPosition = thisObject.BoundingBox.CenterX;

            if (objectPosition.IsBetween(-7, -3))
                return Lane.LeftLeft;
            if (objectPosition.IsBetween(-3, 0))
                return Lane.Left;
            if (objectPosition.IsBetween(0, 3))
                return Lane.Right;
            if (objectPosition.IsBetween(3, 7))
                return Lane.RightRight;
            return Lane.Unknown;
        }
    }
}