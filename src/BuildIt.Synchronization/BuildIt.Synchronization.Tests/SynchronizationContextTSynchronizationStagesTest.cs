// <copyright file="SynchronizationContextTSynchronizationStagesTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
using System;
using System.Threading.Tasks;
using BuildIt.Synchronization;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.Synchronization.Tests
{
    /// <summary>This class contains parameterized unit tests for SynchronizationContext`1</summary>
    [PexClass(typeof(SynchronizationContext<>))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class SynchronizationContextTSynchronizationStagesTest
    {
        /// <summary>Test stub for Synchronize(!0, Boolean, Boolean)</summary>
        [PexGenericArguments(typeof(ValueType))]
        [PexMethod]
        public Task SynchronizeTest<TSynchronizationStages>(
            [PexAssumeUnderTest]SynchronizationContext<TSynchronizationStages> target,
            TSynchronizationStages stagesToSynchronize,
            bool cancelExistingSynchronization,
            bool waitForSynchronizationToComplete
        )
            where TSynchronizationStages : struct
        {
            Task result = target.Synchronize(stagesToSynchronize, 
                                             cancelExistingSynchronization, waitForSynchronizationToComplete);
            return result;
            // TODO: add assertions to method SynchronizationContextTSynchronizationStagesTest.SynchronizeTest(SynchronizationContext`1<!!0>, !!0, Boolean, Boolean)
        }
    }
}
