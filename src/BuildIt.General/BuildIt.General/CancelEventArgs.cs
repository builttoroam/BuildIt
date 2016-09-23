using System;

namespace BuildIt
{
    public class CancelEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}