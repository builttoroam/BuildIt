using Android.Media;
using Java.IO;
using Java.Lang;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class ImageAvailableListener : Object, ImageReader.IOnImageAvailableListener
    {
        private readonly CameraPreviewControlRenderer owner;

        private SemaphoreSlim frameProcessingSemaphore = new SemaphoreSlim(1);

        public ImageAvailableListener(CameraPreviewControlRenderer cameraControlRenderer)
        {
            this.SavePhotoTaskCompletionSource = SavePhotoTaskCompletionSource;
            owner = cameraControlRenderer;
        }

        public TaskCompletionSource<string> SavePhotoTaskCompletionSource { get; set; }

        // Specified if a photo needs to be taken
        public string FilePath { get; set; }

        public bool SaveToPhotosLibrary { get; set; }

        public async void OnImageAvailable(ImageReader reader)
        {
            try
            {
                await frameProcessingSemaphore.WaitAsync();
                using (var image = reader.AcquireNextImage())
                {
                    var buffer = image.GetPlanes()[0].Buffer;
                    var bytes = new byte[buffer.Remaining()];
                    buffer.Get(bytes);
                    await owner.OnMediaFrameArrived(bytes);
                    image.Close();
                    if (string.IsNullOrWhiteSpace(FilePath))
                    {
                        return;
                    }

                    owner.BackgroundHandler.Post(new ImageSaver(bytes, FilePath, SaveToPhotosLibrary, owner, SavePhotoTaskCompletionSource));
                }
            }
            finally
            {
                frameProcessingSemaphore.Release();
            }
        }

        // Saves a JPEG {@link Image} into the specified {@link File}.
        private class ImageSaver : Object, IRunnable
        {
            // The JPEG image
            private readonly byte[] image;
            private readonly string filePath;
            private readonly bool saveToPhotosLibrary;
            private readonly TaskCompletionSource<string> savePhotoTaskCompletionSource;
            private readonly CameraPreviewControlRenderer owner;

            public ImageSaver(byte[] image, string filePath, bool saveToPhotosLibrary, CameraPreviewControlRenderer owner, TaskCompletionSource<string> savePhotoTaskCompletionSource)
            {
                this.savePhotoTaskCompletionSource = savePhotoTaskCompletionSource;
                this.filePath = filePath;
                this.image = image;
                this.saveToPhotosLibrary = saveToPhotosLibrary;
                this.owner = owner;
            }

            public void Run()
            {
                using (var file = new File(filePath))
                {
                    using (var output = new FileOutputStream(file))
                    {
                        try
                        {
                            output.Write(image);
                        }
                        catch (IOException e)
                        {
                            e.PrintStackTrace();
                        }
                        finally
                        {
                            savePhotoTaskCompletionSource.SetResult(filePath);
                        }
                    }
                }
            }
        }
    }
}