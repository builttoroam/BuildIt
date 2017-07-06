namespace BuildIt
{
    /// <summary>
    /// Event args with two strongly typed parameters
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter</typeparam>
    /// <typeparam name="T2">The type of the second paramenter</typeparam>
    public class DualParameterEventArgs<T1, T2> : ParameterEventArgs<T1>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DualParameterEventArgs{T1, T2}"/> class.
        /// Constructs new event args with two typed parameters
        /// </summary>
        /// <param name="parameter1">First parameter</param>
        /// <param name="parameter2">Second parameter</param>
        public DualParameterEventArgs(T1 parameter1, T2 parameter2)
            : base(parameter1)
        {
            Parameter2 = parameter2;
        }

        /// <summary>
        /// Gets or sets gets/Sets the second parameter
        /// </summary>
        public T2 Parameter2 { get; set; }

        /// <summary>
        /// Converts the parameter into a ParameterEventArgs
        /// </summary>
        /// <param name="parameters">The parameter list</param>
        public static implicit operator DualParameterEventArgs<T1, T2>(object[] parameters)
        {
            if (parameters == null || parameters.Length != 2 || parameters[0] == null || parameters[1] == null)
            {
                return null;
            }

            return new DualParameterEventArgs<T1, T2>((T1)parameters[0], (T2)parameters[1]);
        }
    }
}