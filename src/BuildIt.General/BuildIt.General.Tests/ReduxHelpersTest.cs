using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using BuildIt;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.Tests
{
    /// <summary>This class contains parameterized unit tests for ReduxHelpers</summary>
    [TestClass]
    [PexClass(typeof(ReduxHelpers))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ReduxHelpersTest
    {

        /// <summary>Test stub for DeepClone(ObservableCollection`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public ObservableCollection<T> DeepCloneTest<T>(ObservableCollection<T> source)
            where T : new()
        {
            ObservableCollection<T> result = ReduxHelpers.DeepClone<T>(source);
            return result;
            // TODO: add assertions to method ReduxHelpersTest.DeepCloneTest(ObservableCollection`1<!!0>)
        }

        /// <summary>Test stub for Map(!!0, Func`2&lt;!!1,!!1&gt;)</summary>
        [PexMethod]
        public TCollection MapTest<TCollection, TItem>(TCollection sourceCollection, Func<TItem, TItem> map)
            where TCollection : IList<TItem>, new()
        {
            TCollection result = ReduxHelpers.Map<TCollection, TItem>(sourceCollection, map);
            return result;
            // TODO: add assertions to method ReduxHelpersTest.MapTest(!!0, Func`2<!!1,!!1>)
        }
    }
}
