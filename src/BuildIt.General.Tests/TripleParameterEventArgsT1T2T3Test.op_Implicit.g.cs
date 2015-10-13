using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuildIt;
// <copyright file="TripleParameterEventArgsT1T2T3Test.op_Implicit.g.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
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
    public partial class TripleParameterEventArgsT1T2T3Test
    {

[TestMethod]
[PexGeneratedBy(typeof(TripleParameterEventArgsT1T2T3Test))]
public void op_Implicit675()
{
    TripleParameterEventArgs<int, int, int> tripleParameterEventArgs;
    tripleParameterEventArgs = this.op_Implicit<int, int, int>((object[])null);
    Assert.IsNull((object)tripleParameterEventArgs);
}

[TestMethod]
[PexGeneratedBy(typeof(TripleParameterEventArgsT1T2T3Test))]
public void op_Implicit697()
{
    TripleParameterEventArgs<int, int, int> tripleParameterEventArgs;
    object[] os = new object[0];
    tripleParameterEventArgs = this.op_Implicit<int, int, int>(os);
    Assert.IsNull((object)tripleParameterEventArgs);
}

[TestMethod]
[PexGeneratedBy(typeof(TripleParameterEventArgsT1T2T3Test))]
public void op_Implicit554()
{
    TripleParameterEventArgs<int, int, int> tripleParameterEventArgs;
    object[] os = new object[3];
    tripleParameterEventArgs = this.op_Implicit<int, int, int>(os);
    Assert.IsNull((object)tripleParameterEventArgs);
}

[TestMethod]
[PexGeneratedBy(typeof(TripleParameterEventArgsT1T2T3Test))]
public void op_Implicit386()
{
    TripleParameterEventArgs<int, int, int> tripleParameterEventArgs;
    object[] os = new object[3];
    object s0 = new object();
    os[0] = s0;
    object s1 = new object();
    os[2] = s1;
    tripleParameterEventArgs = this.op_Implicit<int, int, int>(os);
    Assert.IsNull((object)tripleParameterEventArgs);
}

[TestMethod]
[PexGeneratedBy(typeof(TripleParameterEventArgsT1T2T3Test))]
public void op_Implicit573()
{
    TripleParameterEventArgs<int, int, int> tripleParameterEventArgs;
    object[] os = new object[3];
    object boxi = (object)(default(int));
    os[0] = boxi;
    object boxi1 = (object)(default(int));
    os[1] = boxi1;
    object boxi2 = (object)(default(int));
    os[2] = boxi2;
    tripleParameterEventArgs = this.op_Implicit<int, int, int>(os);
    Assert.IsNotNull((object)tripleParameterEventArgs);
    Assert.AreEqual<int>(0, tripleParameterEventArgs.Parameter3);
    Assert.AreEqual<int>
        (0, ((DualParameterEventArgs<int, int>)tripleParameterEventArgs).Parameter2);
    Assert.AreEqual<int>
        (0, ((ParameterEventArgs<int>)tripleParameterEventArgs).Parameter1);
}

[TestMethod]
[PexGeneratedBy(typeof(TripleParameterEventArgsT1T2T3Test))]
[ExpectedException(typeof(InvalidCastException))]
public void op_ImplicitThrowsInvalidCastException77701()
{
    TripleParameterEventArgs<int, int, int> tripleParameterEventArgs;
    object[] os = new object[3];
    object s0 = new object();
    os[0] = s0;
    object s1 = new object();
    os[1] = s1;
    object s2 = new object();
    os[2] = s2;
    tripleParameterEventArgs = this.op_Implicit<int, int, int>(os);
}
    }
}
