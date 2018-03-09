namespace Autonomous.CleverPlayer
{
    public static class FloatExtension
    {
        public static bool IsBetween(this float number, float left, float right)
        {
            return number >= left && number < right;
        }
    }
}