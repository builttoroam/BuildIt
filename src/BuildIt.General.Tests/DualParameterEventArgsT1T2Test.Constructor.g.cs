using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuildIt;
// <copyright file="DualParameterEventArgsT1T2Test.Constructor.g.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
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
    public partial class DualParameterEventArgsT1T2Test
    {

[TestMethod]
[PexGeneratedBy(typeof(DualParameterEventArgsT1T2Test))]
public void Constructor860()
{
    DualParameterEventArgs<int, int> dualParameterEventArgs;
    dualParameterEventArgs = this.Constructor<int, int>(0, 0);
    Assert.IsNotNull((object)dualParameterEventArgs);
    Assert.AreEqual<int>(0, dualParameterEventArgs.Parameter2);
    Assert.AreEqual<int>
        (0, ((ParameterEventArgs<int>)dualParameterEventArgs).Parameter1);
}
    }
}
