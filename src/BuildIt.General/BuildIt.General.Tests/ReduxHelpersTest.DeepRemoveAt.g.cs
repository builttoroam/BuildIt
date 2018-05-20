using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Collections.Generic;
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
public void DeepRemoveAt219()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[0];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepRemoveAt<int>(observableCollection, 0);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepRemoveAt336()
{
    ObservableCollection<int> observableCollection;
    observableCollection =
      this.DeepRemoveAt<int>((ObservableCollection<int>)null, 0);
    Assert.IsNull((object)observableCollection);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepRemoveAt948()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[0];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 =
      this.DeepRemoveAt<int>(observableCollection, int.MinValue);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepRemoveAt378()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[1];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepRemoveAt<int>(observableCollection, 0);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepRemoveAt802()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[2];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepRemoveAt<int>(observableCollection, 0);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepRemoveAt855()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[6];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepRemoveAt<int>(observableCollection, 0);
    Assert.IsNotNull((object)observableCollection1);
}
    }
}
