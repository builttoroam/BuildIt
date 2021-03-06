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
public void DeepInsert85()
{
    ObservableCollection<int> observableCollection;
    observableCollection =
      this.DeepInsert<int>((ObservableCollection<int>)null, 0, 0);
    Assert.IsNull((object)observableCollection);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepInsert27()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[0];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepInsert<int>(observableCollection, 0, 0);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepInsert422()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[0];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 =
      this.DeepInsert<int>(observableCollection, int.MinValue, 0);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepInsert324()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[1];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepInsert<int>(observableCollection, 0, 0);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepInsert54()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[2];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepInsert<int>(observableCollection, 0, 0);
    Assert.IsNotNull((object)observableCollection1);
}

[TestMethod]
[PexGeneratedBy(typeof(ReduxHelpersTest))]
public void DeepInsert731()
{
    List<int> list;
    ObservableCollection<int> observableCollection;
    ObservableCollection<int> observableCollection1;
    int[] ints = new int[6];
    list = new List<int>((IEnumerable<int>)ints);
    observableCollection = new ObservableCollection<int>(list);
    observableCollection1 = this.DeepInsert<int>(observableCollection, 0, 0);
    Assert.IsNotNull((object)observableCollection1);
}
    }
}
