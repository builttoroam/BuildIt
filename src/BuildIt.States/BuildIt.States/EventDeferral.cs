using System;
using System.Threading;

namespace BuildIt.States
{
    /// <summary>
    /// Deferral entity to allow event handlers to perform async operations
    /// </summary>
    public class EventDeferral : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventDeferral"/> class.
        /// </summary>
        /// <param name="deferral">The semaphore lock to use in the deferral</param>
        public EventDeferral(SemaphoreSlim deferral)
        {
            Deferral = deferral;
            Deferral.Wait();
        }

        /// <summary>
        /// Gets the deferral lock
        /// </summary>
        public SemaphoreSlim Deferral { get; }

       /// <summary>
       /// Disposes and ends the deferral
       /// </summary>
        public void Dispose()
        {
            Deferral?.Release();
        }
    }
}