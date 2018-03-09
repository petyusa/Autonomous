namespace Autonomous.Impl.Viewports
{
    public class CameraSetup
    {
        public static CameraSetup CameraDefault = new CameraSetup
        {
            CameraPositionStartY = 0f,
            CameraPositionStartZ = 15f,
            CameraPositionEndY = 3f,
            CameraPositionEndZ = 15f
        };

        public  static CameraSetup CameraInside = new CameraSetup
        {
            CameraPositionStartY = 0f,
            CameraPositionStartZ = -2f,
            CameraPositionEndY = 1f,
            CameraPositionEndZ = -2f
        };

        public  static CameraSetup CameraRear = new CameraSetup
        {
            CameraPositionStartY = 0.4f,
            CameraPositionStartZ = 1f,
            CameraPositionEndY = 1.5f,
            CameraPositionEndZ = 8f
        };

        public float CameraPositionStartZ { get; set; }
        public float CameraPositionStartY { get; set; }
        public float CameraPositionEndZ { get; set; }
        public float CameraPositionEndY { get; set; }
    }
}
