using System;
using System.Globalization;
#if !NETFX_CORE
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace BuildIt.General.UI.Converters

{
    public class CaseConverter : IValueConverter
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
            if (caseParameter == null || value == null) return value;
            switch (caseParameter.ToLower())
            {
                case "upper":
                    return value.ToString().ToUpper();
                case "lower":
                    return value.ToString().ToLower();
            }
            return value.ToString();
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


      
    }

}
