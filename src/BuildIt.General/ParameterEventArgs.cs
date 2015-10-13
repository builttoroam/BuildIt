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
        public T Parameter1 { get; set; }

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

    /// <summary>
    /// Event args with two strongly typed parameters
    /// </summary>
    public class DualParameterEventArgs<T1, T2> : ParameterEventArgs<T1>
    {
        public T2 Parameter2 { get; set; }

        public DualParameterEventArgs(T1 parameter1, T2 parameter2):base(parameter1)
        {
            Parameter2 = parameter2;
        }

        /// <summary>
        /// Converts the parameter into a ParameterEventArgs
        /// </summary>
        public static implicit operator DualParameterEventArgs<T1, T2>(object[] parameters)
        {
            if(parameters==null || parameters.Length!=2) return null;
            return new DualParameterEventArgs<T1, T2>((T1)parameters[0], (T2)parameters[1]);
        }
    }

    /// <summary>
    /// Event args with three strongly typed parameters
    /// </summary>
    public class TripleParameterEventArgs<T1, T2, T3> : DualParameterEventArgs<T1,T2>
    {
        public T3 Parameter3 { get; set; }

        public TripleParameterEventArgs(T1 parameter1, T2 parameter2, T3 parameter3):base(parameter1,parameter2)
        {
            Parameter3 = parameter3;
        }

        /// <summary>
        /// Converts the parameter into a ParameterEventArgs
        /// </summary>
        public static implicit operator TripleParameterEventArgs<T1, T2, T3>(object[] parameters)
        {
            if (parameters == null || parameters.Length != 3) return null;
            return new TripleParameterEventArgs<T1, T2, T3>((T1)parameters[0], (T2)parameters[1], (T3)parameters[2]);
        }
    }
}
