using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuildIt;
// <copyright file="UtilitiesTest.SafeRaise01.g.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
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
public void SafeRaise01191()
{
    this.SafeRaise01<int, int>
        ((EventHandler<DualParameterEventArgs<int, int>>)null, (object)null, 0, 0);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void SafeRaise0184()
{
    this.SafeRaise01<int, int>
        (PexChoose.CreateDelegate<EventHandler<DualParameterEventArgs<int, int>>>(), 
         (object)null, 0, 0);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void SafeRaise348()
{
    this.SafeRaise<int>
        ((EventHandler<ParameterEventArgs<int>>)null, (object)null, 0);
}
    }
}
