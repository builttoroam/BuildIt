using MvvmCross.Platform.Logging;
using System;
using System.Diagnostics;

namespace BuildIt.MvvmCross.Forms.UWP
{
    public class EmptyVoidLogProvider : IMvxLogProvider
    {
        private readonly EmptyVoidLog voidLog;

        public EmptyVoidLogProvider()
        {
            voidLog = new EmptyVoidLog();
        }

        public virtual IMvxLog GetLogFor<T>()
        {
            return voidLog;
        }

        public virtual IMvxLog GetLogFor(string name)
        {
            return voidLog;
        }

        public virtual IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public virtual IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }

        public class EmptyVoidLog : IMvxLog
        {
            public virtual bool Log(MvxLogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
            {
                return true;
            }
        }
    }
}
