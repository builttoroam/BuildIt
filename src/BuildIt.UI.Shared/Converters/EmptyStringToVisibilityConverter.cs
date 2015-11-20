using System;
using System.Globalization;
#if !NETFX_CORE
using System.Windows.Data;

#else
using Windows.UI.Xaml.Data;
#endif
namespace BuiltToRoam.UI.Converters
{
    public class EmptyStringToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture
#else
 string language
#endif
)
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

        public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture
#else
 string language
#endif
)
        {
            return string.Empty;
        }
    }
}
