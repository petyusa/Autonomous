namespace Autonomous.Public
{
    /// <summary>
    /// Type of the object the player interacts with,
    /// </summary>
    public enum GameObjectType
    {
        /// <summary>
        /// Another player.
        /// </summary>
        Player,
        /// <summary>
        /// Car
        /// </summary>
        Car,
        /// <summary>
        /// Roadblock
        /// </summary>
        Roadblock,
        /// <summary>
        /// Pedestrian.
        /// </summary>
        Pedestrian,
        /// <summary>
        /// Bus stop, outside of the road.
        /// </summary>
        BusStop,
        /// <summary>
        /// Line at the end of the track.
        /// </summary>
        FinishLine
    }
}