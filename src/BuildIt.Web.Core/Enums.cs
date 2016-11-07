using System;

// ReSharper disable InconsistentNaming

namespace BuildIt.Web.Core
{
    [Flags]
    public enum PushPlatform
    {
        APNS = 1,
        GCM = 2,
        WNS = 3
    }
}
