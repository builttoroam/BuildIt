namespace BuildIt
{
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
            if (parameters == null || parameters.Length != 3 || parameters[0] == null || parameters[1] == null || parameters[2] == null) return null;
            return new TripleParameterEventArgs<T1, T2, T3>((T1)parameters[0], (T2)parameters[1], (T3)parameters[2]);
        }
    }
}