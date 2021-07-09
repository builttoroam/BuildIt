using System;

namespace BuildIt.Media.Uno.WinUI
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