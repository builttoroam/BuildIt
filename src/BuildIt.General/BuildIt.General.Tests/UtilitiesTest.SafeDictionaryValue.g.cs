using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
// <copyright file="UtilitiesTest.SafeDictionaryValue.g.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
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
public void SafeDictionaryValue640()
{
    int i;
    i = this.SafeDictionaryValue<int, int, int>((IDictionary<int, int>)null, 0);
    Assert.AreEqual<int>(0, i);
}
    }
}
