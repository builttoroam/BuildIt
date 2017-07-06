namespace BuildIt
{
    /// <summary>
    /// Event args with three strongly typed parameters
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter</typeparam>
    /// <typeparam name="T2">The type of the second parameter</typeparam>
    /// <typeparam name="T3">The type of the third parameter</typeparam>
    public class TripleParameterEventArgs<T1, T2, T3> : DualParameterEventArgs<T1, T2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TripleParameterEventArgs{T1, T2, T3}"/> class.
        /// Constructs new event args
        /// </summary>
        /// <param name="parameter1">First parameter</param>
        /// <param name="parameter2">Second parameter</param>
        /// <param name="parameter3">Third parameter</param>
        public TripleParameterEventArgs(T1 parameter1, T2 parameter2, T3 parameter3)
            : base(parameter1, parameter2)
        {
            Parameter3 = parameter3;
        }

        /// <summary>
        /// Gets or sets third parameter
        /// </summary>
        public T3 Parameter3 { get; set; }

        /// <summary>
        /// Converts the parameter into a ParameterEventArgs
        /// </summary>
        /// <param name="parameters">The parameter list</param>
        public static implicit operator TripleParameterEventArgs<T1, T2, T3>(object[] parameters)
        {
            if (parameters == null || parameters.Length != 3 || parameters[0] == null || parameters[1] == null || parameters[2] == null)
            {
                return null;
            }

            return new TripleParameterEventArgs<T1, T2, T3>((T1)parameters[0], (T2)parameters[1], (T3)parameters[2]);
        }
    }
}