using System;

namespace Autonomous.Impl.Utilities
{
    public static class FloatExtensions
    {
        public static float SignedMax(this float value, float max)
        {
            var unsignedMax = Math.Abs(max);
            if (Math.Abs(value) > unsignedMax)
            {
                return value < 0 ? -unsignedMax : unsignedMax;
            }

            return value;
        }
    }
}
