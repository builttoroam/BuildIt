using System;

namespace BuildIt
{
    public interface ILogService
    {
        void Debug(string message);

        void Exception(string message, Exception ex);
    }
}