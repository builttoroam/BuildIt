using System.Collections.Generic;
using System;
using BuildIt;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.Tests
{
    [TestClass]
    [PexClass(typeof(FluentHelpers))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class FluentHelpersTest
    {

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        [PexAllowedException(typeof(NotSupportedException))]
        public IList<T> Insert<T>(
            IList<T> source,
            int index,
            T itemToInsert
        )
        {
            IList<T> result = FluentHelpers.Insert<T>(source, index, itemToInsert);
            return result;
            // TODO: add assertions to method FluentHelpersTest.Insert(IList`1<!!0>, Int32, !!0)
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        [PexAllowedException(typeof(NotSupportedException))]
        public IList<T> AddItem<T>(IList<T> source, T itemToAdd)
        {
            IList<T> result = FluentHelpers.AddItem<T>(source, itemToAdd);
            return result;
            // TODO: add assertions to method FluentHelpersTest.AddItem(IList`1<!!0>, !!0)
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        [PexAllowedException(typeof(NotSupportedException))]
        public IList<T> RemoveItem<T>(IList<T> source, T itemToRemove)
        {
            IList<T> result = FluentHelpers.RemoveItem<T>(source, itemToRemove);
            return result;
            // TODO: add assertions to method FluentHelpersTest.RemoveItem(IList`1<!!0>, !!0)
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        [PexAllowedException(typeof(NotSupportedException))]
        public IList<T> RemoveItemAt<T>(IList<T> source, int index)
        {
            IList<T> result = FluentHelpers.RemoveItemAt<T>(source, index);
            return result;
            // TODO: add assertions to method FluentHelpersTest.RemoveItemAt(IList`1<!!0>, Int32)
        }
    }
}
