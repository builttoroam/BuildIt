using System.Collections.Generic;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
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
    public partial class ReduxHelpersTest
    {

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepCloneTest793()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[0];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepCloneTest<int>(observableCollection);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepCloneTest760()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[1];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepCloneTest<int>(observableCollection);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepCloneTest543()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[2];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepCloneTest<int>(observableCollection);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepCloneTest707()
{
    ObservableCollection<int> observableCollection;
    observableCollection = this.DeepCloneTest<int>((ObservableCollection<int>)null);
    Assert.IsNull((object)observableCollection);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepCloneTest477()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[5];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepCloneTest<int>(observableCollection);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepCloneTest01()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[9];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepCloneTest<int>(observableCollection);
    Assert.IsNotNull((object)observableCollection1);
}
    }
}
