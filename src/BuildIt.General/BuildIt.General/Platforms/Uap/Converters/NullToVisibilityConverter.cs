using System;

#if !NETFX_CORE
using System.Windows.Data;

#else

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#endif

namespace BuildIt.UI.Converters
{
    /// <summary>
    /// Converts object to visbility based on whether it's null.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts from entity to visibility.
        /// </summary>
        /// <param name="value">value to convert back.</param>
        /// <param name="targetType">the target type.</param>
        /// <param name="parameter">the conversion parameter.</param>
        /// <param name="language">the conversion language.</param>
        /// <returns>The converted value.</returns>
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

            bool valueExists = value != null;

            if (caseParameter != null && caseParameter == "invert")
            {
                valueExists = !valueExists;
            }

            return valueExists ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">value to convert back.</param>
        /// <param name="targetType">the target type.</param>
        /// <param name="parameter">the conversion parameter.</param>
        /// <param name="language">the conversion language.</param>
        /// <returns>The converted value.</returns>
#pragma warning disable SA1117 // Parameters must be on same line or separate lines

        public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture)
#else
            string language)
#endif
        {
            throw new NotImplementedException();
        }
    }
}