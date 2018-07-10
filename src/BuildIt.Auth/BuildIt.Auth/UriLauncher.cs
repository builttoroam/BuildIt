using System;

namespace BuildIt.Auth
{
    public static class UriLauncher
    {
        private static Action<Uri> UriCallback { get; set; }

        public static void Register(Action<Uri> callback)
        {
            UriCallback = callback;
        }

        public static void Unregister()
        {
            UriCallback = null;
        }

        public static void HandleUri(Uri uri)
        {
            UriCallback?.Invoke(uri);
        }
    }
}