using System;
using System.Globalization;
using System.Windows;
#if !NETFX_CORE
using System.Windows.Data;

#else
using Windows.UI.Text;
using Windows.UI.Xaml.Data;
#endif

namespace BuildIt.UI.Converters
{
    /// <summary>
    /// Sample converter that exchanges whether an item has been read or not
    /// to a font weight. Default is that if read, it will return bold.
    /// If parameter is "invert" then if the item has not been read, it will
    /// return bold
    /// </summary>
    public class BoolBoldConverter : IValueConverter
    {
        /// <summary>
        /// Converts from bool to bold fontweight (if true)
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="targetType">The target type</param>
        /// <param name="parameter">The conversion parameter</param>
        /// <param name="language">The language to convert</param>
        /// <returns>FontWeight</returns>
#pragma warning disable SA1117 // Parameters must be on same line or separate lines
        public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture)
#else
            string language)
#endif
#pragma warning restore SA1117 // Parameters must be on same line or separate lines
        {
            if (value is bool boolValue)
            {
                // Invert if the parameter is "invert"
                bool invert = parameter != null && parameter.ToString().ToLower() == "invert";
                boolValue = invert ? !boolValue : boolValue;

                // Determine whether to return bold or normal
                return boolValue ? FontWeights.Bold : FontWeights.Normal;
            }

            return FontWeights.Normal;
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
#pragma warning restore SA1117 // Parameters must be on same line or separate lines
        {
            return false;
        }
    }
}
