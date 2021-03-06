// <copyright file="DisplayNameAttributeTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BuildIt.Tests
{
    [TestClass]
    [PexClass(typeof(DisplayNameAttribute))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class DisplayNameAttributeTest
    {
        [PexMethod]
        public DisplayNameAttribute Constructor(string displayName)
        {
            var target = new DisplayNameAttribute(displayName);
            Assert.AreEqual(target.DisplayName, displayName);
            return target;
        }
    }
}