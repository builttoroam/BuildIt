using Android.OS;

namespace BuildIt.Bot.Client.Impl.Droid.Extensions
{
    public static class BundleExtensions
    {
        public static string GetStringForLowercaseKey(this Bundle bundle, string key)
        {
            var lowercaseKey = key?.ToLower();
            if (bundle.ContainsKey(lowercaseKey))
            {
                return bundle.GetString(lowercaseKey) ?? string.Empty;
            }
            return string.Empty;
        }

        public static double? GetDoubleForLowercaseKey(this Bundle bundle, string key)
        {
            var lowercaseKey = key?.ToLower();
            if (bundle.ContainsKey(lowercaseKey))
            {
                return bundle.GetDouble(lowercaseKey);
            }
            return null;
        }
    }
}