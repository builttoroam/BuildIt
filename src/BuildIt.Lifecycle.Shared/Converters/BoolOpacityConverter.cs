﻿/********************************************************************************************************
 * Copyright (c) 2011 Built to Roam
 * This code is available for use only within authorised applications. 
 * Do not redistribute or reuse in unauthorised applications
 ********************************************************************************************************/

using System;
using System.Globalization;
#if !NETFX_CORE
using System.Windows.Data;

#else
using Windows.UI.Xaml.Data;
#endif
namespace BuiltToRoam.UI.Converters
{
    /// <summary>
    /// Converts between a bool and opacity. If true, returns 1
    /// If false, returns 0, or the value specified as a parameter
    /// </summary>
    public class BoolOpacityConverter : IValueConverter
    {
        /// <summary>
        /// Converts from bool to opacity
        /// </summary>
        /// <param name="value">The bool value</param>
        /// <param name="targetType">The destination type</param>
        /// <param name="parameter">The low opacity value</param>
        /// <param name="culture"></param>
        /// <returns>Opacity value (low opacity value (default = 0) or 1.0</returns>
        public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture
#else
 string language
#endif
)
        {
            var lowopacity = 0.0;

            // Convert the parameter to the low opacity value
            if (parameter is double) lowopacity = (double)parameter;
            if (parameter is string)
            {
                double.TryParse((string)parameter, out lowopacity);
            }

            // Use the bool value to determine what the opacity value should be
            if (value is bool)
            {
                return ((bool)value) ? 1.0: lowopacity;
            }
            return 0.0;
        }

        /// <summary>
        /// Converts from opacity to bool
        /// </summary>
        /// <param name="value">The opacity value (double)</param>
        /// <param name="targetType">The destination type</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Bool value</returns>
        public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
            CultureInfo culture
#else
 string language
#endif
)
        {
            if (value is double)
            {
                return ((double)value) == 1.0 ? true : false;
            }
            return false;
        }
    }
}
