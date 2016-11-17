using Android.App;
using Android.Content;
using BuildIt.Bot.Client.Impl.Droid.Utilities;
using Gcm.Client;

namespace BuildIt.Bot.Client.Impl.Droid
{
    /// <summary>
    /// 
    /// </summary>
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class PushHandlerBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
    {
        /// <summary>
        /// 
        /// </summary>
        public static string GoogleApiConsoleAppProjectNumber { get; set; }
    }
}