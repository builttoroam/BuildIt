using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <auto-generated>
// This file contains automatically generated tests.
// Do not modify this file manually.
// 
// If the contents of this file becomes outdated, you can delete it.
// For example, if it no longer compiles.
// </auto-generated>
using System;

namespace BuildIt.Tests
{
    public partial class UtilitiesTest
    {

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void EnumParse363()
{
    TestEnum i;
    i = this.EnumParse("\0", false);
    Assert.AreEqual<TestEnum>(TestEnum.Base, i);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void EnumParse592()
{
    TestEnum i;
    i = this.EnumParse((string)null, false);
    Assert.AreEqual<TestEnum>(TestEnum.Base, i);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void EnumParse928()
{
    TestEnum i;
    i = this.EnumParse("Base", false);
    Assert.AreEqual<TestEnum>(TestEnum.Base, i);
}
    }
}
