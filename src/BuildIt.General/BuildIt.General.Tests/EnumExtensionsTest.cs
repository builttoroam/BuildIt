// <copyright file="EnumExtensionsTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BuildIt.Tests
{
    /// <summary>This class contains parameterized unit tests for EnumExtensions</summary>
    [TestClass]
    public partial class EnumExtensionsTest
    {
        public enum TestEnum
        {
            Base,

            [Test]
            Value1,
        }

        private class TestAttribute : Attribute
        {
        }

        [TestMethod]
        public void GetAttributeTest()
        {
            const TestEnum value = TestEnum.Value1;

            var result = value.GetAttribute<TestAttribute>();
            Assert.IsNotNull(result);
        }
    }
}