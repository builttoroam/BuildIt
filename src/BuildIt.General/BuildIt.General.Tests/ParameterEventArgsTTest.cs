// <copyright file="ParameterEventArgsTTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BuildIt.Tests
{
    [TestClass]
    [PexClass(typeof(ParameterEventArgs<>))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ParameterEventArgsTTest
    {
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public ParameterEventArgs<T> Constructor<T>(T parameter)
        {
            ParameterEventArgs<T> target = new ParameterEventArgs<T>(parameter);
            Assert.AreEqual(target.Parameter1, parameter);
            return target;
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public ParameterEventArgs<T> ImplicitConstructor<T>(T parameter)
        {
            ParameterEventArgs<T> target = parameter;
            Assert.AreEqual(target.Parameter1, parameter);
            return target;
        }
    }
}