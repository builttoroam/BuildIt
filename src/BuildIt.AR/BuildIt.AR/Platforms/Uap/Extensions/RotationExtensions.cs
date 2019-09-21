using Windows.Media.Capture;

namespace BuildIt.AR.UWP.Extensions
{
    public static class RotationExtensions
    {
        public static VideoRotation ToVideoRotation(this Rotation rotation)
        {
            var videoRotation = VideoRotation.None;
            switch (rotation)
            {
                case Rotation.Rotation0:
                    videoRotation = VideoRotation.Clockwise90Degrees;
                    break;

                case Rotation.Rotation90:
                    break;

                case Rotation.Rotation180:
                    videoRotation = VideoRotation.Clockwise270Degrees;
                    break;

                case Rotation.Rotation270:
                    videoRotation = VideoRotation.Clockwise180Degrees;
                    break;
            }

            return videoRotation;
        }
    }
}