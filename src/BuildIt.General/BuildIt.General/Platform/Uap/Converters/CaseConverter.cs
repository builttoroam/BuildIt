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
    /// Converts to lower or upper case characters
    /// </summary>
    public class CaseConverter : IValueConverter
    {
        /// <summary>
        /// Converts to upper or to lower case based on the parameter value ("upper" or "lower" or null if no conversion)
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
            if (caseParameter == null || value == null)
            {
                return value;
            }

            switch (caseParameter.ToLower())
            {
                case "upper":
                    return value.ToString().ToUpper();
                case "lower":
                    return value.ToString().ToLower();
            }

            return value.ToString();
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
            throw new NotImplementedException();
        }
    }
}
