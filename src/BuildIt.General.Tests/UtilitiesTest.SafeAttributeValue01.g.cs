using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
// <copyright file="UtilitiesTest.SafeAttributeValue01.g.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
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
public void SafeAttributeValue01985()
{
    string s;
    s = this.SafeAttributeValue01((XElement)null, (string)null);
    Assert.AreEqual<string>("", s);
}
    }
}
