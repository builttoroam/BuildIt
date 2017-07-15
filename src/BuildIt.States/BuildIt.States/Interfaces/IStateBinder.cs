using System;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines method to unbind state groups
    /// </summary>
    public interface IStateBinder : IDisposable
    {
        /// <summary>
        /// Binds the state groups
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Bind();

        /// <summary>
        /// Unbinds the state groups
        /// </summary>
        void Unbind();
    }
}