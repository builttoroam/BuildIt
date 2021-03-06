// <copyright file="TripleParameterEventArgsT1T2T3Test.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BuildIt.Tests
{
    [TestClass]
    [PexClass(typeof(TripleParameterEventArgs<,,>))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class TripleParameterEventArgsT1T2T3Test
    {
        [PexGenericArguments(typeof(int), typeof(int), typeof(int))]
        [PexMethod]
        [PexAllowedException(typeof(InvalidCastException))]
        public TripleParameterEventArgs<T1, T2, T3> op_Implicit<T1, T2, T3>(object[] parameters)
        {
            TripleParameterEventArgs<T1, T2, T3> result = (TripleParameterEventArgs<T1, T2, T3>)parameters;
            return result;
            // TODO: add assertions to method TripleParameterEventArgsT1T2T3Test.op_Implicit(Object[])
        }

        [PexGenericArguments(typeof(int), typeof(int), typeof(int))]
        [PexMethod]
        public TripleParameterEventArgs<T1, T2, T3> Constructor<T1, T2, T3>(
            T1 parameter1,
            T2 parameter2,
            T3 parameter3
        )
        {
            TripleParameterEventArgs<T1, T2, T3> target
               = new TripleParameterEventArgs<T1, T2, T3>(parameter1, parameter2, parameter3);
            return target;
            // TODO: add assertions to method TripleParameterEventArgsT1T2T3Test.Constructor(!!0, !!1, !!2)
        }
    }
}