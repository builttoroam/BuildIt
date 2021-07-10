using AVFoundation;
using CoreMedia;
using CoreVideo;
using System;
using System.Threading.Tasks;

namespace BuildIt.Media.Uno.WinUI
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