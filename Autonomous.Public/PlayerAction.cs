namespace Autonomous.Public
{
    /// <summary>
    /// Action what the player performs in each loop.
    /// </summary>
    public class PlayerAction
    {
        /// <summary>
        /// Set true to move left.
        /// </summary>
        public bool MoveLeft { get; set; }
        /// <summary>
        /// Set true to move right.
        /// </summary>
        public bool MoveRight { get; set; }

        /// <summary>
        /// Acceleration in range (-1, 1). 1: Full acceleration, -1 Full deceleration. 
        /// </summary>
        public float Acceleration { get; set; }
    }
}