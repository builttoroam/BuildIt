using System;

// ReSharper disable InconsistentNaming

namespace BuildIt.Web.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]    
    public enum PushPlatform
    {
        /// <summary>
        /// 
        /// </summary>
        APNS = 1,
        /// <summary>
        /// 
        /// </summary>
        GCM = 2,
        /// <summary>
        /// 
        /// </summary>
        WNS = 3
    }
}
