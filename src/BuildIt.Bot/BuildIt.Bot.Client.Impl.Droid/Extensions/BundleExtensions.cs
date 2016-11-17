using Android.OS;

namespace BuildIt.Bot.Client.Impl.Droid.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class BundleExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetStringForLowercaseKey(this Bundle bundle, string key)
        {
            var lowercaseKey = key?.ToLower();
            if (bundle.ContainsKey(lowercaseKey))
            {
                return bundle.GetString(lowercaseKey) ?? string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="key"></param>
        /// <returns></returns>
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