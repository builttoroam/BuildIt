/********************************************************************************************************
 * Copyright (c) 2015 Built to Roam
 * This code is available for use only within authorised applications. 
 * Do not redistribute or reuse in unauthorised applications
 ********************************************************************************************************/

using System;

namespace BuildIt
{
    /// <summary>
    /// Event args with one strongly typed parameter
    /// </summary>
    public class ParameterEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The first parameter
        /// </summary>
        public T Parameter1 { get; set; }

        /// <summary>
        /// Constructs the event args
        /// </summary>
        /// <param name="parameter">The first parameter</param>
        public ParameterEventArgs(T parameter)
        {
            Parameter1 = parameter;
        }

        /// <summary>
        /// Converts the parameter into a ParameterEventArgs
        /// </summary>
        public static implicit operator ParameterEventArgs<T>(T parameter)
        {
            return new ParameterEventArgs<T>(parameter);
        }
    }
}
