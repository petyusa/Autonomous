using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous.Impl.Strategies
{
    public class ControlState
    {
        public ControlState()
        {
            Acceleration = 0f;
            HorizontalSpeed = 0f;
        }
        /// <summary>
        /// 1: Full throttle, -1: Full break
        /// </summary>
        public float Acceleration { get; set; }

        /// <summary>
        /// -1: Full speed left, 1: Full speed right
        /// </summary>
        public float HorizontalSpeed { get; set; }
    }
}
