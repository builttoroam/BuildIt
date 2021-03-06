using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// <copyright file="UtilitiesTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace BuildIt.Tests
{
    [TestClass]
    [PexClass(typeof(Utilities))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class UtilitiesTest
    {
        [PexMethod(MaxBranches = 40000)]
        // [PexArguments("", typeof(string))]
        // [PexArguments(null, typeof(int))]
        // [PexArguments("\"invalid json", typeof(LogHelperTest.TestPersonEntity))]
        // [PexArguments("\"test\"", typeof(string))]
        // [PexArguments("test", typeof(int))]
        // [PexArguments("5", typeof(int))]
        public object DecodeJson01(string jsonString, Type jsonType)
        {
            PexAssume.IsTrue(
                             (jsonString == "" && jsonType == typeof(string)) ||
                             (jsonString == null && jsonType == typeof(int)) ||
                             (jsonString == "\"invalid json" && jsonType == typeof(LogHelperTest.TestPersonEntity)) ||
                             (jsonString == "\"test\"" && jsonType == typeof(string)) ||
                             (jsonString == "test" && jsonType == typeof(int)) ||
                             (jsonString == "5" && jsonType == typeof(int))
                             );
            object result = Utilities.DecodeJson(jsonString, jsonType);

            return result;
            // TODO: add assertions to method UtilitiesTest.DecodeJson01(String, Type)
        }

        [PexMethod(MaxBranches = 40000)]
        [PexGenericArguments(typeof(int))]
        [PexGenericArguments(typeof(LogHelperTest.TestPersonEntity))]
        [PexGenericArguments(typeof(string))]
        public T DecodeJson<T>(string jsonString)
        {
            PexAssume.IsTrue(
                             (jsonString == "" && typeof(T) == typeof(string)) ||
                             (jsonString == null && typeof(T) == typeof(int)) ||
                             (jsonString == "\"invalid json" && typeof(T) == typeof(LogHelperTest.TestPersonEntity)) ||
                             (jsonString == "{\"Name\":\"test\"}" && typeof(T) == typeof(LogHelperTest.TestPersonEntity)) ||
                             (jsonString == "\"test\"" && typeof(T) == typeof(string)) ||
                             (jsonString == "test" && typeof(T) == typeof(int)) ||
                             (jsonString == "5" && typeof(T) == typeof(int))
                             );

            T result = Utilities.DecodeJson<T>(jsonString);
            return result;
            // TODO: add assertions to method UtilitiesTest.DecodeJson(String)
        }

        [PexMethod]
        public string EncodeJson(LogHelperTest.TestPersonEntity objectToJsonify)
        {
            PexAssume.IsNotNull(objectToJsonify);
            string result = Utilities.EncodeJson(objectToJsonify);
            return result;
            // TODO: add assertions to method UtilitiesTest.EncodeJson(!!0)
        }

        [PexMethod]
        public TestEnum EnumParse(string enumValue, bool ignoreCase)
        {
            TestEnum result = Utilities.EnumParse<TestEnum>(enumValue, ignoreCase);
            return result;
            // TODO: add assertions to method UtilitiesTest.EncodeJson(!!0)
        }

        [PexMethod(MaxRunsWithoutNewTests = 200)]
        [PexGenericArguments(typeof(List<int>), typeof(int))]
        public TList Replace<TList, T>(TList source, IEnumerable<T> newItems) where TList : IList<T>
        {
            return Utilities.Replace(source, newItems);
        }

        [PexMethod(MaxRunsWithoutNewTests = 200)]
        [PexGenericArguments(typeof(List<int>), typeof(int))]
        public TList Fill<TList, T>(TList source, IEnumerable<T> newItems) where TList : IList<T>
        {
            return Utilities.Fill(source, newItems);
        }

        [PexGenericArguments(typeof(LogHelperTest.TestPersonEntity))]
        [PexMethod]
        public string EncodeJson<T>(T objectToJsonify)
        {
            string result = Utilities.EncodeJson<T>(objectToJsonify);
            return result;
            // TODO: add assertions to method UtilitiesTest.EncodeJson(!!0)
        }

        [PexMethod]
        [PexAllowedException(typeof(NullReferenceException))]
        public StringBuilder AppendCommaIfNeeded(StringBuilder sb)
        {
            StringBuilder result = Utilities.AppendCommaIfNeeded(sb);
            return result;
            // TODO: add assertions to method UtilitiesTest.AppendCommaIfNeeded(StringBuilder)
        }

        [PexMethod(MaxRunsWithoutNewTests = 400)]
        public StringBuilder AppendIfNotNullOrEmpty(StringBuilder sb, string toAppend)
        {
            StringBuilder result = Utilities.AppendIfNotNullOrEmpty(sb, toAppend);
            return result;
            // TODO: add assertions to method UtilitiesTest.AppendIfNotNullOrEmpty(StringBuilder, String)
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod(MaxBranches = 20000)]
        public T ReadObject<T>(Stream stream)
        {
            T result = Utilities.ReadObject<T>(stream);
            return result;
            // TODO: add assertions to method UtilitiesTest.ReadObject(Stream)
        }

        [PexMethod(MaxRunsWithoutNewTests = 400)]
        public StringBuilder AppendOnCondition(
            StringBuilder sb,
            Func<bool> condition,
            string toAppend
        )
        {
            StringBuilder result = Utilities.AppendOnCondition(sb, condition, toAppend);
            return result;
            // TODO: add assertions to method UtilitiesTest.AppendOnCondition(StringBuilder, Func`1<Boolean>, String)
        }

        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod(MaxBranches = 40000, MaxConstraintSolverTime = 60, MaxRunsWithoutNewTests = 200, Timeout = 240)]
        public IDictionary<TKey, TValue> Combine<TKey, TValue>(IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            PexAssume.IsTrue((first is Dictionary<int, int> &&
                             second is Dictionary<int, int>) || first == null || second == null);

            IDictionary<TKey, TValue> result = Utilities.Combine<TKey, TValue>(first, second);
            return result;
        }

        [PexMethod]
        public DateTime ConvertToDateTimeFromUnixTimestamp(string unixTime)
        {
            DateTime result = Utilities.ConvertToDateTimeFromUnixTimestamp(unixTime);
            return result;
            // TODO: add assertions to method UtilitiesTest.ConvertToDateTimeFromUnixTimestamp(String)
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public bool DoForEach<T>(IEnumerable<T> source, Action<T> action)
        {
            bool result = Utilities.DoForEach<T>(source, action);
            return result;
            // TODO: add assertions to method UtilitiesTest.DoForEach(IEnumerable`1<!!0>, Action`1<!!0>)
        }

        [PexGenericArguments(typeof(object))]
        [PexMethod]
        public bool DoIfNotNull<T>(T item, Action<T> action)
            where T : class
        {
            bool result = Utilities.DoIfNotNull<T>(item, action);
            return result;
            // TODO: add assertions to method UtilitiesTest.DoIfNotNull(!!0, Action`1<!!0>)
        }

        [PexMethod]
        public bool ExecuteIfNotNull(object instance, Action method)
        {
            bool result = Utilities.ExecuteIfNotNull(instance, method);
            return result;
            // TODO: add assertions to method UtilitiesTest.ExecuteIfNotNull(Object, Action)
        }

        [PexMethod]
        public string FriendlyDateFormat(DateTime date)
        {
            /*var days = date.Date.Subtract(DateTime.Now.Date).TotalDays;
            PexAssume.IsTrue(
                date == DateTime.MinValue || date == DateTime.MaxValue || days < -365 * 2 || days < -365 || days < -31
                || days < -14 || days < -7 || days < -1 || date.Date < DateTime.Now.Date || days > 365 * 2 || days > 365
                || days > 31 || days > 14 || days > 7 || days > 1 || date.Date > DateTime.Now.Date);*/
            string result = Utilities.FriendlyDateFormat(date);
            return result;
            // TODO: add assertions to method UtilitiesTest.FriendlyDateFormat(DateTime), intellitest doesn't seem to create more unit tests even with PexAssume
        }

        [PexMethod]
        public string SafeAttributeValue(XElement element, XName name)
        {
            string result = Utilities.SafeAttributeValue(element, name);
            return result;
            // TODO: add assertions to method UtilitiesTest.SafeAttributeValue(XElement, XName)
        }

        [PexMethod]
        public string SafeAttributeValue01(XElement element, string name)
        {
            string result = Utilities.SafeAttributeValue(element, name);
            return result;
            // TODO: add assertions to method UtilitiesTest.SafeAttributeValue01(XElement, String)
        }

        [PexMethod]
        public string SafeDecendentValue(XElement element, string name)
        {
            string result = Utilities.SafeDescendentValue(element, name);
            return result;
            // TODO: add assertions to method UtilitiesTest.SafeDecendentValue(XElement, String)
        }

        [PexMethod]
        public string SafeDecendentValue01(XElement element, XName name)
        {
            string result = Utilities.SafeDescendentValue(element, name);
            return result;
            // TODO: add assertions to method UtilitiesTest.SafeDecendentValue01(XElement, XName)
        }

        [PexGenericArguments(typeof(int), typeof(int), typeof(int))]
        [PexMethod]
        public TReturn SafeDictionaryValue<TKey, TValue, TReturn>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TReturn result = Utilities.SafeValue<TKey, TValue, TReturn>(dictionary, key);
            return result;
            // TODO: add assertions to method UtilitiesTest.SafeDictionaryValue(IDictionary`2<!!0,!!1>, !!0)
        }

        [PexMethod]
        public string SafeElementValue(XElement element, string name)
        {
            string result = Utilities.SafeElementValue(element, name);
            return result;
            // TODO: add assertions to method UtilitiesTest.SafeElementValue(XElement, String)
        }

        [PexMethod]
        public string SafeElementValue01(XElement element, XName name)
        {
            string result = Utilities.SafeElementValue(element, name);
            return result;
            // TODO: add assertions to method UtilitiesTest.SafeElementValue01(XElement, XName)
        }

        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void SafeRaise01<TParameter1, TParameter2>(
            EventHandler<DualParameterEventArgs<TParameter1, TParameter2>> handler,
            object sender,
            TParameter1 arg1,
            TParameter2 arg2
        )
        {
            Utilities.SafeRaise<TParameter1, TParameter2>(handler, sender, arg1, arg2);
            // TODO: add assertions to method UtilitiesTest.SafeRaise01(EventHandler`1<DualParameterEventArgs`2<!!0,!!1>>, Object, !!0, !!1)
        }

        [PexGenericArguments(typeof(int), typeof(int), typeof(int))]
        [PexMethod]
        public void SafeRaise02<TParameter1, TParameter2, TParameter3>(
            EventHandler<TripleParameterEventArgs<TParameter1, TParameter2, TParameter3>> handler,
            object sender,
            TParameter1 arg1,
            TParameter2 arg2,
            TParameter3 arg3
        )
        {
            Utilities.SafeRaise<TParameter1, TParameter2, TParameter3>(handler, sender, arg1, arg2, arg3);
            // TODO: add assertions to method UtilitiesTest.SafeRaise02(EventHandler`1<TripleParameterEventArgs`3<!!0,!!1,!!2>>, Object, !!0, !!1, !!2)
        }

        [PexMethod]
        public void SafeRaise03(
            EventHandler handler,
            object sender,
            EventArgs args
        )
        {
            Utilities.SafeRaise(handler, sender, args);
            // TODO: add assertions to method UtilitiesTest.SafeRaise03(EventHandler, Object, EventArgs)
        }

        [PexGenericArguments(typeof(EventArgs))]
        [PexMethod]
        public void SafeRaise04<T>(
            EventHandler<T> handler,
            object sender,
            T args
        )
            where T : EventArgs
        {
            Utilities.SafeRaise<T>(handler, sender, args);
            // TODO: add assertions to method UtilitiesTest.SafeRaise04(EventHandler`1<!!0>, Object, !!0)
        }

        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public void SafeRaise<TParameter1>(
            EventHandler<ParameterEventArgs<TParameter1>> handler,
            object sender,
            TParameter1 args
        )
        {
            Utilities.SafeRaise<TParameter1>(handler, sender, args);
            // TODO: add assertions to method UtilitiesTest.SafeRaise(EventHandler`1<ParameterEventArgs`1<!!0>>, Object, !!0)
        }

        [PexMethod(MaxRunsWithoutNewTests = 200)]
        public DateTime ToDate(string doubleValue)
        {
            using (ShimsContext.Create())
            {
                System.Fakes.ShimDateTime.NowGet = () => new DateTime(2015, 10, 15, 11, 10, 40);
                DateTime result = Utilities.ToDate(doubleValue);
                return result;
            }

            // TODO: add assertions to method UtilitiesTest.ToDate(String)
        }

        [PexMethod]
        public double ToDouble(string doubleValue, double defaultValue)
        {
            using (ShimsContext.Create())
            {
                System.Fakes.ShimDateTime.NowGet = () => new DateTime(2015, 10, 15, 11, 10, 40);
                double result = Utilities.ToDouble(doubleValue, defaultValue);
                return result;
            }

            // TODO: add assertions to method UtilitiesTest.ToDouble(String, Double)
        }

        [PexMethod(MaxRunsWithoutNewTests = 200)]
        public int ToInt(string intValue, int defaultValue)
        {
            int result = Utilities.ToInt(intValue, defaultValue);
            return result;
            // TODO: add assertions to method UtilitiesTest.ToInt(String, Int32)
        }

        /// <summary>Test stub for ToQueryString(!!0, Boolean)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public string ToQueryStringTest<T>(T obj, bool applyLowerCaseParameters)
        {
            string result = Utilities.ToQueryString<T>(obj, applyLowerCaseParameters);
            return result;
            // TODO: add assertions to method UtilitiesTest.ToQueryStringTest(!!0, Boolean)
        }

        [TestMethod]
        public void ToQueryStringTests()
        {
            var testEntityA = new TestEntityA { Prop2 = 3 };
            var queryString = Utilities.ToQueryString(testEntityA);
            Assert.AreEqual(queryString, "prop1=0&prop2=3");
            queryString = Utilities.ToQueryString(testEntityA, false);
            Assert.AreEqual(queryString, "Prop1=0&Prop2=3");

            var testEntityB = new TestEntityB();
            queryString = Utilities.ToQueryString(testEntityB);
            Assert.AreEqual(queryString, string.Empty);
            queryString = Utilities.ToQueryString(testEntityB, false);
            Assert.AreEqual(queryString, string.Empty);
            testEntityB.Prop1 = 3;
            queryString = Utilities.ToQueryString(testEntityB);
            Assert.AreEqual(queryString, "prop1=3");
            queryString = Utilities.ToQueryString(testEntityB, false);
            Assert.AreEqual(queryString, "Prop1=3");

            var testEntityC = new TestEntityC();
            queryString = Utilities.ToQueryString(testEntityC);
            Assert.AreEqual(queryString, string.Empty);
            queryString = Utilities.ToQueryString(testEntityC, false);
            Assert.AreEqual(queryString, string.Empty);
            testEntityC.Prop1 = "hello";
            queryString = Utilities.ToQueryString(testEntityC);
            Assert.AreEqual(queryString, "prop1=hello");
            queryString = Utilities.ToQueryString(testEntityC, false);
            Assert.AreEqual(queryString, "Prop1=hello");
            testEntityC.Prop1 = "hello world";
            queryString = Utilities.ToQueryString(testEntityC, applyUrlEncoding: false);
            Assert.AreEqual(queryString, "prop1=hello world");
            queryString = Utilities.ToQueryString(testEntityC);
            Assert.AreEqual(queryString, "prop1=hello+world");
            testEntityC.Prop2 = "we meet again";
            queryString = Utilities.ToQueryString(testEntityC, applyUrlEncoding: false);
            Assert.AreEqual(queryString, "prop1=hello world&prop2=we meet again");
            queryString = Utilities.ToQueryString(testEntityC);
            Assert.AreEqual(queryString, "prop1=hello+world&prop2=we+meet+again");
        }
    }

    public static partial class PersonFactory

    {
        [PexFactoryMethod(typeof(LogHelperTest.TestPersonEntity))]
        internal static LogHelperTest.TestPersonEntity Create(string name)

        {
            return new LogHelperTest.TestPersonEntity { Name = name };
        }
    }

    public class TestEntityA
    {
        // private int privateField;

        public static int PublicStaticField;

        public int Prop1 { get; set; }

        public int Prop2 { get; set; }
    }

    public class TestEntityB
    {
        // private int privateField;

        public static int PublicStaticField;

        public int? Prop1 { get; set; }
    }

    public class TestEntityC
    {
        // private int privateField;

        public static int PublicStaticField;

        public string Prop1 { get; set; }
        public string Prop2 { get; set; }
    }

    public enum TestEnum
    {
        Base,
        One,
        Two,
    }
}