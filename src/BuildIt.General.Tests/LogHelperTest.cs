// <copyright file="LogHelperTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using System;
using BuildIt;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.Tests
{
    [TestClass]
    [PexClass(typeof(LogHelper))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class LogHelperTest
    {

        [PexMethod]
        public void LogException(
            Exception ex,
            string message,
            string caller
        )
        {
            LogHelper.LogException(ex, message, caller);
            // TODO: add assertions to method LogHelperTest.LogException(Exception, String, String)
        }

        public class TestPersonEntity
        {
            public string Name { get; set; }
        }


        [PexGenericArguments(typeof(int))]
        [PexGenericArguments(typeof(TestPersonEntity))]
        [PexMethod]
        public void Log<TEntity>(TEntity entity, string caller)
        {
            LogHelper.Log<TEntity>(entity, caller);
            // TODO: add assertions to method LogHelperTest.Log(!!0, String)
        }
    }
}
