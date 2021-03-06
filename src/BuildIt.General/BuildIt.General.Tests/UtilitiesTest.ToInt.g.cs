using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="UtilitiesTest.ToInt.g.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
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
public void ToInt60()
{
    int i;
    i = this.ToInt("-\0", 0);
    Assert.AreEqual<int>(0, i);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ToInt165()
{
    int i;
    i = this.ToInt("00", 0);
    Assert.AreEqual<int>(0, i);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ToInt188()
{
    int i;
    i = this.ToInt("\0\0", 0);
    Assert.AreEqual<int>(0, i);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ToInt238()
{
    int i;
    i = this.ToInt("9", 0);
    Assert.AreEqual<int>(9, i);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ToInt285()
{
    int i;
    i = this.ToInt("", 0);
    Assert.AreEqual<int>(0, i);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ToInt589()
{
    int i;
    i = this.ToInt("-", 0);
    Assert.AreEqual<int>(0, i);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ToInt614()
{
    int i;
    i = this.ToInt((string)null, 0);
    Assert.AreEqual<int>(0, i);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ToInt726()
{
    int i;
    i = this.ToInt("\0ĀĀ", 0);
    Assert.AreEqual<int>(0, i);
}
    }
}
