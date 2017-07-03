using System;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines method to unbind state groups
    /// </summary>
    public interface IStateBinder:IDisposable
    {
        /// <summary>
        /// Binds the state groups
        /// </summary>
        void Bind();

        /// <summary>
        /// Unbinds the state groups
        /// </summary>
        void Unbind();
    }
}