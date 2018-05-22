using BuildIt.UI;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace BuildIt.Forms.Converters
{
    /// <summary>
    /// Returns a wrapper for a data property that will gracefully
    /// handle changes without forcing massive UI redraw
    /// </summary>
    public class FormsImmutableDataConverter : IValueConverter
    {
        /// <summary>
        /// Converts to wrapper
        /// </summary>
        /// <param name="value">The data entity to convert</param>
        /// <param name="targetType">The target type (not used)</param>
        /// <param name="parameter">converter parameter (not used)</param>
        /// <param name="culture">culture (not used)</param>
        /// <returns>The wrapped entity</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value?.GetType().GetTypeInfo();
            var gtype = type?.ImplementedInterfaces.FirstOrDefault(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IHasImmutableData<>));
            if (gtype != null)
            {
                var appStateType = typeof(ImmutableDataWrapper<>).MakeGenericType(gtype.GenericTypeArguments);
                return Activator.CreateInstance(appStateType, value, new PlatformUIContext());
            }

            return value;
        }

        /// <summary>
        /// Converts back - does nothing
        /// </summary>
        /// <param name="value">The data entity to convert</param>
        /// <param name="targetType">The target type (not used)</param>
        /// <param name="parameter">converter parameter (not used)</param>
        /// <param name="culture">culture (not used)</param>
        /// <returns>The original entity</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
