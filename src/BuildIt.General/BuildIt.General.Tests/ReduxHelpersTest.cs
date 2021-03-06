using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public ObservableCollection<T> DeepAdd<T>(ObservableCollection<T> source, T itemToAdd)
            where T : new()
        {
            ObservableCollection<T> result = ReduxHelpers.DeepAdd<T>(source, itemToAdd);
            return result;
            // TODO: add assertions to method ReduxHelpersTest.DeepAdd(ObservableCollection`1<!!0>, !!0)
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public T DeepEntityClone<T>(T source)
            where T : new()
        {
            T result = ReduxHelpers.DeepEntityClone<T>(source);
            return result;
            // TODO: add assertions to method ReduxHelpersTest.DeepEntityClone(!!0)
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public ObservableCollection<T> DeepInsert<T>(
            ObservableCollection<T> source,
            int index,
            T itemToAdd
        )
            where T : new()
        {
            ObservableCollection<T> result = ReduxHelpers.DeepInsert<T>(source, index, itemToAdd);
            return result;
            // TODO: add assertions to method ReduxHelpersTest.DeepInsert(ObservableCollection`1<!!0>, Int32, !!0)
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public ObservableCollection<T> DeepRemove<T>(ObservableCollection<T> source, T itemToRemove)
            where T : new()
        {
            ObservableCollection<T> result = ReduxHelpers.DeepRemove<T>(source, itemToRemove);
            return result;
            // TODO: add assertions to method ReduxHelpersTest.DeepRemove(ObservableCollection`1<!!0>, !!0)
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public ObservableCollection<T> DeepRemoveAt<T>(ObservableCollection<T> source, int index)
            where T : new()
        {
            ObservableCollection<T> result = ReduxHelpers.DeepRemoveAt<T>(source, index);
            return result;
            // TODO: add assertions to method ReduxHelpersTest.DeepRemoveAt(ObservableCollection`1<!!0>, Int32)
        }
    }
}