using System;

namespace BuildIt.Forms.Controls
{
    public class MediaFrameEventArgs : EventArgs
    {
        public MediaFrameEventArgs(MediaFrame mediaFrame)
        {
            MediaFrame = mediaFrame;
        }

        public MediaFrame MediaFrame { get; }
    }
}