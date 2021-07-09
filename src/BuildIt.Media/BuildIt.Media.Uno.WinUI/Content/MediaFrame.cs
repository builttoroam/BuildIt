namespace BuildIt.Media.Uno.WinUI
{
    public class MediaFrame
    {
        public MediaFrame(object nativeFrame)
        {
            NativeFrame = nativeFrame;
        }

        public object NativeFrame { get; }
    }
}