using AVFoundation;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Ios;
using CoreMedia;
using CoreVideo;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Controls.Platforms.Ios
{
    internal class FrameExtractor : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        private readonly Func<MediaFrame, Task> frameArrivedAction;

        public FrameExtractor(Func<MediaFrame, Task> frameArrivedAction)
        {
            this.frameArrivedAction = frameArrivedAction;
        }

        public override async void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            using (sampleBuffer)
            {
                using (var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer)
                {
                    await frameArrivedAction(new MediaFrame(pixelBuffer));
                }
            }
        }
    }
}