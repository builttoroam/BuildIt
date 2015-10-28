using System;
using System.Globalization;
using System.Windows;
#if !NETFX_CORE
using System.Windows.Data;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace BuiltToRoam.UI.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture
#else
 string language
#endif
)
        {
            var caseParameter = parameter as string;

            bool valueExists = value != null;

            if (caseParameter != null && caseParameter == "invert")
            {
                valueExists = !valueExists;
            }


            return valueExists ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture
#else
 string language
#endif
)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
