using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous.Public
{
    public static class GameConstants
    {
        /// <summary>
        /// Full width of the road. (meter)
        /// </summary>
        public const float RoadWidth = 12f;

        /// <summary>
        /// Width of a lane. (meter)
        /// </summary>
        public const float LaneWidth = 3f;

        /// <summary>
        /// Acceleration of the players' car. (m/s2)
        /// </summary>
        public const float PlayerAcceleration = 10f;

        /// <summary>
        /// Deceleration of the players' car. (m/s2)
        /// </summary>
        public const float PlayerDeceleration = 18f;

        /// <summary>
        /// Maximum speed of players' car. (m/s)
        /// </summary>
        public const float PlayerMaxSpeed = 180f / 3.6f;

        /// <summary>
        /// Speed of players' car in left/right direction. (m/s)
        /// </summary>
        public const float PlayerHoriztontalSpeed = 5f;

        /// <summary>
        /// Frequency of updating the course game objects
        /// </summary>
        public const int GameCourseUpdateFrequency = 500;

        /// <summary>
        /// Rendering area in Y axis
        /// </summary>
        public const float RenderingAreaY = 350f;

        /// <summary>
        /// Shadow rendering area in Y axis
        /// </summary>
        public const float ShadowRenderingAreaY = 180f;

        /// <summary>
        /// Player model width
        /// </summary>
        public const float PlayerWidth = 1.7f;
    }
}
