using System.Diagnostics;
using Android.Media;
using Android.Net;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class ImageScanCompletedListener : Java.Lang.Object, MediaScannerConnection.IOnScanCompletedListener
    {
        public void OnScanCompleted(string path, Uri uri)
        {
            Debug.WriteLine($"[{nameof(ImageScanCompletedListener)}] Image scan completed. The scanned image path is {path}");
        }
    }
}
