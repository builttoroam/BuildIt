using System;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

// ReSharper disable once CheckNamespace - Issue with Multi-targetting
namespace BuildIt.UI.Converters
{
    /// <summary>
    /// Returns a wrapper for a data property that will gracefully
    /// handle changes without forcing massive UI redraw
    /// </summary>
    public class ImmutableDataConverter : IValueConverter
    {
        /// <summary>
        /// Converts to wrapper
        /// </summary>
        /// <param name="value">The data entity to convert</param>
        /// <param name="targetType">The target type (not used)</param>
        /// <param name="parameter">converter parameter (not used)</param>
        /// <param name="language">language (not used)</param>
        /// <returns>The wrapped entity</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = value.GetType();
            var gtype = type.GetInterfaces().FirstOrDefault(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IHasImmutableData<>));
            if (gtype != null)
            {
                var appStateType = typeof(ImmutableDataWrapper<>).MakeGenericType(gtype.GetGenericArguments());
                return Activator.CreateInstance(appStateType, value, new UniversalUIContext());
            }

            return value;
        }

        /// <summary>
        /// Converts back - does nothing
        /// </summary>
        /// <param name="value">The data entity to convert</param>
        /// <param name="targetType">The target type (not used)</param>
        /// <param name="parameter">converter parameter (not used)</param>
        /// <param name="language">language (not used)</param>
        /// <returns>The original entity</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
