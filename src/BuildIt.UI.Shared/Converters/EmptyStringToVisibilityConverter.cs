using System;
using System.Globalization;
#if !NETFX_CORE
using System.Windows.Data;

#else
using Windows.UI.Xaml.Data;
#endif

namespace BuildIt.General.UI.Converters
{
    /// <summary>
    /// Converter to convert string to visibility (collapsed if string is empty or null)
    /// </summary>
    public class EmptyStringToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Returns visibile if value is not null and not empty
        /// </summary>
        /// <param name="value">value to convert back</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the conversion parameter</param>
        /// <param name="language">the conversion language</param>
        /// <returns>The converted value</returns>
#pragma warning disable SA1117 // Parameters must be on same line or separate lines
        public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture)
#else
            string language)
#endif
#pragma warning restore SA1117 // Parameters must be on same line or separate lines
        {
            var caseParameter = parameter as string;

            bool valueExists = string.IsNullOrWhiteSpace(value as string);

            if (caseParameter != null && caseParameter == "invert")
            {
                valueExists = !valueExists;
            }

            // Note: We take the reverse here because in most cases you want to display
            // the item if the value exists, rather than when it doesn't
            return (!valueExists).ToVisibility();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">value to convert back</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the conversion parameter</param>
        /// <param name="language">the conversion language</param>
        /// <returns>The converted value</returns>
#pragma warning disable SA1117 // Parameters must be on same line or separate lines
        public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture)
#else
            string language)
#endif
        {
            return string.Empty;
        }
    }
}
