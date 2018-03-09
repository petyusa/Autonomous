namespace Autonomous.Public
{
    /// <summary>
    /// State of the an object in the game (location, speed, etc)
    /// </summary>
    public class GameObjectState
    {
        /// <summary>
        /// [INTERNAL] Constructor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="boundingBox"></param>
        /// <param name="vx"></param>
        /// <param name="vy"></param>
        public GameObjectState(string id, GameObjectType type, RectangleF boundingBox, float vx, float vy, float damage)
        {
            Id = id;
            GameObjectType = type;
            BoundingBox = boundingBox;
            VX = vx;
            VY = vy;
            Damage = damage;
        }

        /// <summary>
        /// Speed in Y direction (along the road). [m/s]
        /// </summary>
        public float VY { get; private set; }

        /// <summary>
        /// Horizontal speed (left or right). [m/s]
        /// </summary>
        public float VX { get; private set; }

        /// <summary>
        /// Bounding box of the object. [m]
        /// </summary>
        public RectangleF BoundingBox { get; private set; }

        /// <summary>
        /// Type of the object.
        /// </summary>
        public GameObjectType GameObjectType { get; private set; }

        /// <summary>
        /// Damage (0-1).
        /// </summary>
        public float Damage { get; private set; }

        /// <summary>
        /// Id of the object.
        /// </summary>
        public string Id { get; private set; }

        public float MaximumAcceleration =>
            GameObjectType == GameObjectType.Car || GameObjectType == GameObjectType.Player
                ? GameConstants.PlayerAcceleration
                : 0;

        public float MaximumDeceleration =>
            GameObjectType == GameObjectType.Car || GameObjectType == GameObjectType.Player
                ? GameConstants.PlayerDeceleration
                : 0;

        // TODO Damage, score
    }
}