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
    /// Converts between bool to visibility.
    /// </summary>
    public class BoolVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts bool to visibility.
        /// </summary>
        /// <param name="value">Incoming bool value.</param>
        /// <param name="targetType">N/A - type ignored.</param>
        /// <param name="parameter">N/A - parameter ignored.</param>
        /// <param name="language">The language to use for conversion.</param>
        /// <returns>Output visibility value.</returns>
#pragma warning disable SA1117 // Parameters must be on same line or separate lines

        public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture)
#else
            string language)
#endif
#pragma warning restore SA1117 // Parameters must be on same line or separate lines
        {
            if (value is bool)
            {
                if (parameter + string.Empty == "invert")
                {
                    value = !(bool)value;
                }

                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        /// <summary>
        /// Converts visibility to bool.
        /// </summary>
        /// <param name="value">Incoming Visibility value.</param>
        /// <param name="targetType">N/A - type ignored.</param>
        /// <param name="parameter">N/A - parameter ignored.</param>
        /// <param name="language">The language to use for conversion.</param>
        /// <returns>Output bool value.</returns>
#pragma warning disable SA1117 // Parameters must be on same line or separate lines

        public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture)
#else
            string language)
#endif
        {
            if (value is Visibility)
            {
                return ((Visibility)value) == Visibility.Visible ? true : false;
            }

            return false;
        }
    }
}