// <copyright file="SyncStepWrapperTStageTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
using System;
using BuildIt.Synchronization;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.Synchronization.Tests
{
    /// <summary>This class contains parameterized unit tests for SyncStepWrapper`1</summary>
    [PexClass(typeof(SyncStepWrapper<>))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class SyncStepWrapperTStageTest
    {
    }
}
