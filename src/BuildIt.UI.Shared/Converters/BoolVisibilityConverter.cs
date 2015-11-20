/********************************************************************************************************
 * Copyright (c) 2011 Built to Roam
 * This code is available for use only within authorised applications. 
 * Do not redistribute or reuse in unauthorised applications
 ********************************************************************************************************/

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
    /// <summary>
    /// Converts between bool to visibility
    /// </summary>
    public class BoolVisibilityConverter:IValueConverter
    {
        /// <summary>
        /// Converts bool to visibility
        /// </summary>
        /// <param name="value">Incoming bool value</param>
        /// <param name="targetType">N/A</param>
        /// <param name="parameter">N/A</param>
        /// <param name="culture">N/A</param>
        /// <returns>Output visibility value</returns>
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
                if(parameter+""=="invert")
                {
                    value = !(bool)value;
                }
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Converts visibility to bool
        /// </summary>
        /// <param name="value">Incoming Visibility value</param>
        /// <param name="targetType">N/A</param>
        /// <param name="parameter">N/A</param>
        /// <param name="culture">N/A</param>
        /// <returns>Output bool value</returns>
        public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture
#else
 string language
#endif
)
        {
            if (value is Visibility)
            {
                return ((Visibility)value) == Visibility.Visible ? true : false;
            }
            return false;
        }
    }
}
