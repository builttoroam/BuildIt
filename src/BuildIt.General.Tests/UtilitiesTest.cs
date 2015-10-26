// <copyright file="UtilitiesTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using System;
using BuildIt;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.Tests
{
    [TestClass]
    [PexClass(typeof(Utilities))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class UtilitiesTest
    {

        [PexMethod]
        [PexArguments("", typeof(string))]
        [PexArguments(null, typeof(int))]
        [PexArguments("\"invalid json", typeof(LogHelperTest.TestPersonEntity))]
        [PexArguments("\"test\"", typeof(string))]
        [PexArguments("test", typeof(int))]
        [PexArguments("5", typeof(int))]
        public object DecodeJson01(string jsonString, Type jsonType)
        {
            object result = Utilities.DecodeJson(jsonString, jsonType);
            return result;
            // TODO: add assertions to method UtilitiesTest.DecodeJson01(String, Type)
        }
    }
}
