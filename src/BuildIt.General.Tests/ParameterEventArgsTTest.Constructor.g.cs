using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuildIt;
// <copyright file="ParameterEventArgsTTest.Constructor.g.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
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
    public partial class ParameterEventArgsTTest
    {

[TestMethod]
[PexGeneratedBy(typeof(ParameterEventArgsTTest))]
public void Constructor603()
{
    ParameterEventArgs<int> parameterEventArgs;
    parameterEventArgs = this.Constructor<int>(0);
    Assert.IsNotNull((object)parameterEventArgs);
    Assert.AreEqual<int>(0, parameterEventArgs.Parameter1);
}
    }
}
