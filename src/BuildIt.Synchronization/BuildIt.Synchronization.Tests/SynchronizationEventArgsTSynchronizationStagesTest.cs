// <copyright file="SynchronizationEventArgsTSynchronizationStagesTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
using System;
using BuildIt.Synchronization;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.Synchronization.Tests
{
    /// <summary>This class contains parameterized unit tests for SynchronizationEventArgs`1</summary>
    [PexClass(typeof(SynchronizationEventArgs<>))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class SynchronizationEventArgsTSynchronizationStagesTest
    {
    }
}
