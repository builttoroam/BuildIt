// <copyright file="DualParameterEventArgsT1T2Test.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BuildIt.Tests
{
    [TestClass]
    [PexClass(typeof(DualParameterEventArgs<,>))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class DualParameterEventArgsT1T2Test
    {
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        [PexAllowedException(typeof(InvalidCastException))]
        public DualParameterEventArgs<T1, T2> op_Implicit<T1, T2>(object[] parameters)
        {
            DualParameterEventArgs<T1, T2> result = parameters;
            return result;
        }

        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public DualParameterEventArgs<T1, T2> Constructor<T1, T2>(T1 parameter1, T2 parameter2)
        {
            DualParameterEventArgs<T1, T2> target = new DualParameterEventArgs<T1, T2>(parameter1, parameter2);
            Assert.AreEqual(target.Parameter1, parameter1);
            Assert.AreEqual(target.Parameter2, parameter2);
            return target;
        }
    }
}