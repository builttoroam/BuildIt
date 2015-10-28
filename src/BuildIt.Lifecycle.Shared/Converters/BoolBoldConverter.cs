using System;
using System.Globalization;
using System.Windows;
#if !NETFX_CORE
using System.Windows.Data;

#else
using Windows.UI.Text;
using Windows.UI.Xaml.Data;
#endif

namespace BuiltToRoam.UI.Converters
{
    /// <summary>
    /// Sample converter that exchanges whether an item has been read or not
    /// to a font weight. Default is that if read, it will return bold. 
    /// If parameter is "invert" then if the item has not been read, it will 
    /// return bold
    /// </summary>
    public class BoolBoldConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture
#else
            string language
#endif
            )
        {
            if (value is bool)
            {
                var boolValue = (bool) value;

                // Invert if the parameter is "invert"
                bool invert = (parameter != null && parameter.ToString().ToLower() == "invert");
                boolValue = invert ? !boolValue : boolValue;

                // Determine whether to return bold or normal
                return boolValue ? FontWeights.Bold : FontWeights.Normal;
            }
            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture
#else
 string language
#endif
)
        {
            return false;
        }
    }
}
