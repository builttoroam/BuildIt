using System;
using System.Globalization;
#if !NETFX_CORE
using System.Windows.Data;

#else
using Windows.UI.Xaml.Data;
#endif

namespace BuildIt.UI.Converters
{
    /// <summary>
    /// Converts a value to a string using a format string.
    /// </summary>
    public class StringFormatConverter : IValueConverter
    {
#pragma warning disable SA1615 // Element return value must be documented
        /// <summary>
        /// Converts a value to a string by formatting it.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type of the conversion.</param>
        /// <param name="parameter">The format string.</param>
        /// <param name="language">The language to use for conversion.</param>
        /// <returns>The formatted string.</returns>
#pragma warning disable SA1117 // Parameters must be on same line or separate lines
        public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture)
#else
            string language)
#endif
#pragma warning restore SA1117 // Parameters must be on same line or separate lines
#pragma warning restore SA1615 // Element return value must be documented
        {
            if (value == null)
            {
                return string.Empty;
            }

            return string.Format(CultureInfo.CurrentCulture, (parameter as string) ?? "{0}", value);
        }

        /// <summary>
        /// Converts a value from a string to a target type.
        /// </summary>
        /// <param name="value">The value to convert to a string.</param>
        /// <param name="targetType">The target type of the conversion.</param>
        /// <param name="parameter">A parameter used during the conversion
        /// process.</param>
        /// <param name="language">The language to use for conversion.</param>
        /// <returns>The converted object.</returns>
#pragma warning disable SA1117 // Parameters must be on same line or separate lines
        public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture)
#else
            string language)
#endif
        {
            throw new NotSupportedException();
        }
    }
}