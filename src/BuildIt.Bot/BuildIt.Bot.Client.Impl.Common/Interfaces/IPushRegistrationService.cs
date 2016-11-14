using System;

namespace BuildIt.Bot.Client.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPushRegistrationService
    {
        /// <summary>
        /// 
        /// </summary>
        Func<string> RetrieveCurrentRegistrationId { set; }

        /// <summary>
        /// 
        /// </summary>
        Action<string> RegistrationSuccessful { set; }
        /// <summary>
        /// 
        /// </summary>
        Action<Exception> RegistrationFailure { set; }
    }
}
