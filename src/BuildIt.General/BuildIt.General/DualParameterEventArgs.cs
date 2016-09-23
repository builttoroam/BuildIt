namespace BuildIt
{
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
            if(parameters==null || parameters.Length!=2 || parameters[0]==null || parameters[1]==null) return null;
            return new DualParameterEventArgs<T1, T2>((T1)parameters[0], (T2)parameters[1]);
        }
    }
}