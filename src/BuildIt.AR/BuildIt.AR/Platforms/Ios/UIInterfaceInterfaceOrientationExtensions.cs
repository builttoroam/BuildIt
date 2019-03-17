using UIKit;

namespace BuildIt.AR.iOS
{
    public static class UIInterfaceInterfaceOrientationExtensions
    {
        public static Rotation ToRotationEnum(this UIInterfaceOrientation currentOrientation)
        {
            var rotation = Rotation.Rotation0;
            switch (currentOrientation)
            {
                case UIInterfaceOrientation.LandscapeLeft:
                    rotation = Rotation.Rotation90;
                    break;
                case UIInterfaceOrientation.LandscapeRight:
                    rotation = Rotation.Rotation270;
                    break;
                case UIInterfaceOrientation.Portrait:
                    rotation = Rotation.Rotation0;
                    break;
                case UIInterfaceOrientation.PortraitUpsideDown:
                    rotation = Rotation.Rotation180;
                    break;
                default:
                    rotation = Rotation.Rotation0;
                    break;
            }
            return rotation;
        }
    }
}