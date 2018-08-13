using Android.Content;
using Android.Media;
using Java.IO;
using Java.Lang;
using System.Threading.Tasks;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        private readonly CameraPreviewControlRenderer owner;
        private readonly TaskCompletionSource<string> savePhotoTaskCompletionSource;

        // Specified if a photo needs to be taken
        public string FilePath { get; set; }

        public bool SaveToPhotosLibrary { get; set; }

        public ImageAvailableListener(CameraPreviewControlRenderer cameraControlRenderer, TaskCompletionSource<string> savePhotoTaskCompletionSource)
        {
            this.savePhotoTaskCompletionSource = savePhotoTaskCompletionSource;
            if (cameraControlRenderer == null)
            {
                throw new System.ArgumentNullException("cameraControlRenderer");
            }

            owner = cameraControlRenderer;
        }

        public void OnImageAvailable(ImageReader reader)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                return;
            }

            var image = reader.AcquireNextImage();
            owner.backgroundHandler.Post(new ImageSaver(image, FilePath, SaveToPhotosLibrary, owner, savePhotoTaskCompletionSource));
        }

        // Saves a JPEG {@link Image} into the specified {@link File}.
        private class ImageSaver : Object, IRunnable
        {
            // The JPEG image
            private readonly Image image;
            private readonly string filePath;
            private readonly bool saveToPhotosLibrary;
            private readonly TaskCompletionSource<string> savePhotoTaskCompletionSource;
            private readonly CameraPreviewControlRenderer owner;

            public ImageSaver(Image image, string filePath, bool saveToPhotosLibrary, CameraPreviewControlRenderer owner, TaskCompletionSource<string> savePhotoTaskCompletionSource)
            {
                this.savePhotoTaskCompletionSource = savePhotoTaskCompletionSource;
                this.filePath = filePath;
                if (image == null)
                {
                    throw new System.ArgumentNullException("image");
                }

                this.image = image;
                this.saveToPhotosLibrary = saveToPhotosLibrary;
                this.owner = owner;
            }

            public void Run()
            {
                using (var file = new File(filePath))
                {
                    var buffer = image.GetPlanes()[0].Buffer;
                    var bytes = new byte[buffer.Remaining()];
                    buffer.Get(bytes);
                    using (var output = new FileOutputStream(file))
                    {
                        try
                        {
                            output.Write(bytes);
                        }
                        catch (IOException e)
                        {
                            e.PrintStackTrace();
                        }
                        finally
                        {
                            image.Close();
                            //if (saveToPhotosLibrary)
                            //{
                            //    owner.SaveToPhotosLibrary(file);
                            //}

                            savePhotoTaskCompletionSource.SetResult(filePath);
                        }
                    }
                }
            }
        }
    }
}