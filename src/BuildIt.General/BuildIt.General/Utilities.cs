using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BuildIt
{
    /// <summary>
    /// General helper methods
    /// </summary>
    public static partial class Utilities
    {
        private static DateTime Epoch => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Decodes a string to an entity
        /// </summary>
        /// <typeparam name="T">The type to deserialise to</typeparam>
        /// <param name="jsonString">The string to deserialise</param>
        /// <returns>The deserialised entity</returns>
        public static T DecodeJson<T>(this string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return default(T);
            }
        }

        /// <summary>
        /// Decodes json string to an entity
        /// </summary>
        /// <param name="jsonString">The string to deserialize</param>
        /// <param name="jsonType">The type of the entity to deserialize to</param>
        /// <returns>The deserialized entity</returns>
        public static object DecodeJson(this string jsonString, Type jsonType)
        {
            try
            {
                return JsonConvert.DeserializeObject(jsonString, jsonType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Serialize entity to json string
        /// </summary>
        /// <typeparam name="T">The type of entity to serialize</typeparam>
        /// <param name="objectToJsonify">The entity to serialize</param>
        /// <returns>The serialized string</returns>
        public static string EncodeJson<T>(this T objectToJsonify)
        {
            // ReSharper disable CompareNonConstrainedGenericWithNull
            return objectToJsonify == null ? null : JsonConvert.SerializeObject(objectToJsonify);
            // ReSharper restore CompareNonConstrainedGenericWithNull
        }

        /// <summary>
        /// Iterate through collection and perform action on each item
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="source">The collection of items</param>
        /// <param name="action">The action to perform</param>
        /// <returns>Successful execution</returns>
        public static bool DoForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null)
            {
                return false;
            }

            foreach (var item in source)
            {
                action(item);
            }

            return true;
        }

        /// <summary>
        /// Iterate through collection and perform async action on each item
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="source">The collection of items</param>
        /// <param name="action">The action to perform</param>
        /// <returns>Successful execution</returns>
        public static async Task<bool> DoForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
        {
            if (source == null || action == null)
            {
                return false;
            }

            foreach (var item in source)
            {
                await action(item);
            }

            return true;
        }

        /// <summary>
        /// Iterates over a collection and adds items to another collection
        /// </summary>
        /// <typeparam name="TList">The type of list to add items</typeparam>
        /// <typeparam name="T">The type of items in both lists</typeparam>
        /// <param name="source">The list to add items to</param>
        /// <param name="items">The list of items to add</param>
        /// <returns>The combined list</returns>
        public static TList Fill<TList, T>(this TList source, IEnumerable<T> items)
            where TList : IList<T>
        {
            if (source == null || items == null)
            {
                return source;
            }

            foreach (var item in items)
            {
                source.Add(item);
            }

            return source;
        }

        /// <summary>
        /// Replace all items in an existing list
        /// </summary>
        /// <typeparam name="TList">The type of the list to replace items</typeparam>
        /// <typeparam name="T">The type of items in both lists</typeparam>
        /// <param name="source">The list to replace items in</param>
        /// <param name="newItems">The list of items to add into the existing list</param>
        /// <returns>The final list</returns>
        public static TList Replace<TList, T>(this TList source, IEnumerable<T> newItems)
            where TList : IList<T>
        {
            if (source == null || newItems == null)
            {
                return source;
            }

            source.Clear();
            source.Fill(newItems);
            return source;
        }

        /// <summary>
        /// Append comma if length>0
        /// </summary>
        /// <param name="sb">The string builder to modify</param>
        /// <returns>The modified string builder</returns>
        public static StringBuilder AppendCommaIfNeeded(this StringBuilder sb)
        {
            if (sb != null && sb.Length > 0)
            {
                sb.Append(",");
            }

            return sb;
        }

        /// <summary>
        /// Append text if predicate/condition is true
        /// </summary>
        /// <param name="sb">The strinbuilder to add to</param>
        /// <param name="condition">The condition to invoke</param>
        /// <param name="toAppend">The text to add</param>
        /// <returns>The modified stringbuilder</returns>
        public static StringBuilder AppendOnCondition(this StringBuilder sb, Func<bool> condition, string toAppend)
        {
            if (sb != null && condition != null && condition())
            {
                sb.Append(toAppend);
            }

            return sb;
        }

        /// <summary>
        /// Append text if the stringbuilder if the text isn't null/empty
        /// </summary>
        /// <param name="sb">The stringbuilder to modify</param>
        /// <param name="toAppend">The text to add (if not null/empty)</param>
        /// <returns>The modified stringbuilder</returns>
        public static StringBuilder AppendIfNotNullOrEmpty(this StringBuilder sb, string toAppend)
        {
            if (sb != null && !string.IsNullOrEmpty(toAppend))
            {
                sb.Append(toAppend);
            }

            return sb;
        }

        /// <summary>
        /// Raises an event without throwing exception if not handlers
        /// </summary>
        /// <typeparam name="TParameter1">The type of the first parameter</typeparam>
        /// <param name="handler">The event to raise</param>
        /// <param name="sender">The sender entity</param>
        /// <param name="args">The first parameter</param>
        public static void SafeRaise<TParameter1>(this EventHandler<ParameterEventArgs<TParameter1>> handler, object sender, TParameter1 args)
        {
            handler.SafeRaise(sender, new ParameterEventArgs<TParameter1>(args));
        }

        /// <summary>
        /// Raises an event without throwing exception if not handlers
        /// </summary>
        /// <typeparam name="TParameter1">The type of the first parameter</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter</typeparam>
        /// <param name="handler">The event to raise</param>
        /// <param name="sender">The sender entity</param>
        /// <param name="arg1">The first parameter</param>
        /// <param name="arg2">The second parameter</param>
        public static void SafeRaise<TParameter1, TParameter2>(this EventHandler<DualParameterEventArgs<TParameter1, TParameter2>> handler, object sender, TParameter1 arg1, TParameter2 arg2)
        {
            handler.SafeRaise(sender, new DualParameterEventArgs<TParameter1, TParameter2>(arg1, arg2));
        }

        /// <summary>
        /// Raises an event without throwing exception if not handlers
        /// </summary>
        /// <typeparam name="TParameter1">The type of the first parameter</typeparam>
        /// <typeparam name="TParameter2">The type of the second parameter</typeparam>
        /// <typeparam name="TParameter3">The type of the third parameter</typeparam>
        /// <param name="handler">The event to raise</param>
        /// <param name="sender">The sender entity</param>
        /// <param name="arg1">The first parameter</param>
        /// <param name="arg2">The second parameter</param>
        /// <param name="arg3">The third parameter</param>
        public static void SafeRaise<TParameter1, TParameter2, TParameter3>(this EventHandler<TripleParameterEventArgs<TParameter1, TParameter2, TParameter3>> handler, object sender, TParameter1 arg1, TParameter2 arg2, TParameter3 arg3)
        {
            handler.SafeRaise(sender, new TripleParameterEventArgs<TParameter1, TParameter2, TParameter3>(arg1, arg2, arg3));
        }

        /// <summary>
        /// Raises an event without throwing exception if not handlers
        /// </summary>
        /// <param name="handler">The event to raise</param>
        /// <param name="sender">The sender entity</param>
        /// <param name="args">The parameters</param>
        public static void SafeRaise(this EventHandler handler, object sender, EventArgs args = null)
        {
            if (args == null)
            {
                args = EventArgs.Empty;
            }

            handler?.Invoke(sender, args);
        }

        /// <summary>
        /// Raises an event without throwing exception if not handlers
        /// </summary>
        /// <typeparam name="T">The type of the first parameter</typeparam>
        /// <param name="handler">The event to raise</param>
        /// <param name="sender">The sender entity</param>
        /// <param name="args">The parameter</param>
        public static void SafeRaise<T>(this EventHandler<T> handler, object sender, T args)
            where T : EventArgs
        {
            handler?.Invoke(sender, args);
        }

        /// <summary>
        /// Safetly retrieves a value from the dictionary. No exception thrown if key is null, or doesn't exist in dictionary
        /// </summary>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TValue">The type of the values</typeparam>
        /// <typeparam name="TReturn">The type to be returned</typeparam>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key to lookup</param>
        /// <returns>The value, or null</returns>
        public static TReturn SafeValue<TKey, TValue, TReturn>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null || key == null)
            {
                return default(TReturn);
            }

            TValue val;
            if (dictionary.TryGetValue(key, out val))
            {
                if (val is TReturn)
                {
                    return (TReturn)(object)val;
                }
            }

            return default(TReturn);
        }

        /// <summary>
        /// Safetly retrieves a value from the dictionary. No exception thrown if key is null, or doesn't exist in dictionary
        /// </summary>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TValue">The type of the values</typeparam>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key to lookup</param>
        /// <returns>The value, or null</returns>
        public static TValue SafeValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null || key == null)
            {
                return default(TValue);
            }

            return dictionary.TryGetValue(key, out var val) ? val : default(TValue);
        }

        /// <summary>
        /// Safetly retrieves a value from the dictionary. No exception thrown if key is null, or doesn't exist in dictionary
        /// </summary>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TValue">The type of the values</typeparam>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key to lookup</param>
        /// <returns>The value, or null</returns>
        public static TValue SafeValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null || key == null)
            {
                return default(TValue);
            }

            TValue val;
            return dictionary.TryGetValue(key, out val) ? val : default(TValue);
        }

        /// <summary>
        /// Does an action if both the item and the action are not null
        /// </summary>
        /// <typeparam name="T">The type of the entity to pass into the action</typeparam>
        /// <param name="item">The entity to pass into the action</param>
        /// <param name="action">The action</param>
        /// <returns>Successfully called action</returns>
        public static bool DoIfNotNull<T>(this T item, Action<T> action)
            where T : class
        {
            if (item == null || action == null)
            {
                return false;
            }

            action(item);
            return true;
        }

        /// <summary>
        /// Does an action if both the item and the action are not null
        /// </summary>
        /// <param name="instance">The entity to pass into the action</param>
        /// <param name="method">The action</param>
        /// <returns>Successfully called action</returns>
        public static bool ExecuteIfNotNull(this object instance, Action method)
        {
            if (instance == null || method == null)
            {
                return false;
            }

            method();
            return true;
        }

        /// <summary>
        /// Parses a string into an enum value
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="enumValue">The string to parse</param>
        /// <param name="ignoreCase">Whether to ignore case</param>
        /// <returns>The enum value, or the default (first) enum value</returns>
        public static T EnumParse<T>(this string enumValue, bool ignoreCase = true)
            where T : struct
        {
            try
            {
                if (string.IsNullOrEmpty(enumValue))
                {
                    return default(T);
                }

                return (T)Enum.Parse(typeof(T), enumValue, ignoreCase);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Converts a string to a double, or returns the default value
        /// </summary>
        /// <param name="doubleValue">The string to parse</param>
        /// <param name="defaultValue">The default value to return if string doesn't parse</param>
        /// <returns>The parsed (or default) value</returns>
        public static double ToDouble(this string doubleValue, double defaultValue = 0.0)
        {
            if (string.IsNullOrWhiteSpace(doubleValue))
            {
                return defaultValue;
            }

            return double.TryParse(doubleValue, out double retValue) ? retValue : defaultValue;
        }

        /// <summary>
        /// Converts a string to a date
        /// </summary>
        /// <param name="doubleValue">The string to parse</param>
        /// <returns>The parsed value</returns>
        public static DateTime ToDate(this string doubleValue)
        {
            if (string.IsNullOrWhiteSpace(doubleValue))
            {
                return DateTime.Now;
            }

            return DateTime.TryParse(doubleValue, out DateTime retValue) ? retValue : DateTime.Now;
        }

        /// <summary>
        /// Converts a string to a int, or returns the default value
        /// </summary>
        /// <param name="intValue">The string to parse</param>
        /// <param name="defaultValue">The default value to return if string doesn't parse</param>
        /// <returns>The parsed (or default) value</returns>
        public static int ToInt(this string intValue, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(intValue))
            {
                return defaultValue;
            }

            int retValue;
            if (int.TryParse(intValue, out retValue))
            {
                return retValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Reads an object from a stream into an entity (assumes json string)
        /// </summary>
        /// <typeparam name="T">The type of entity to read out</typeparam>
        /// <param name="stream">The stream to read from</param>
        /// <returns>The entity read from the stream</returns>
        public static T ReadObject<T>(this Stream stream)
        {
            try
            {
                using (var reader = new StreamReader(stream))
                using (var jreader = new JsonTextReader(reader))
                {
#if DEBUG
                    var txt = reader.ReadToEnd();
                    Debug.WriteLine(txt);
                    stream.Position = 0;
#endif
                    return JsonSerializer.CreateDefault().Deserialize<T>(jreader);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to read object from json stream - " + ex.Message);
                return default(T);
            }
        }

        /// <summary>
        /// Converts a date time to a friendly string
        /// </summary>
        /// <param name="date">The date to convert</param>
        /// <returns>The pretty date string</returns>
        public static string FriendlyDateFormat(this DateTime date)
        {
            if (date == DateTime.MinValue || date == DateTime.MaxValue)
            {
                return " ";
            }

            var days = date.Date.Subtract(DateTime.Now.Date).TotalDays;
            if (days < -365 * 2)
            {
                return (-(int)days / 365) + " years ago";
            }

            if (days < -365)
            {
                return "Last year";
            }

            if (days < -31)
            {
                return ((int)(-days / 31)) + " months ago";
            }

            if (days < -14)
            {
                return ((int)(-days / 7)) + " weeks ago";
            }

            if (days < -7)
            {
                return "Last week";
            }

            if (days < -1)
            {
                return (-days) + " days ago";
            }

            if (date.Date < DateTime.Now.Date)
            {
                return "Yesterday";
            }

            if (days > 365 * 2)
            {
                return ((int)days / 365) + " years";
            }

            if (days > 365)
            {
                return "Next year";
            }

            if (days > 31)
            {
                return ((int)(days / 31)) + " months";
            }

            if (days > 14)
            {
                return ((int)(days / 7)) + " weeks";
            }

            if (days > 7)
            {
                return "Next week";
            }

            if (days > 1)
            {
                return days + " days";
            }

            if (date.Date > DateTime.Now.Date)
            {
                return "Tomorrow";
            }

            return date.ToString(); // .ToShortTimeString();
        }

        /// <summary>
        /// Combines two dictionaries into a new dictionary - doesn't replace items in first dictionary
        /// </summary>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TValue">The type of the values</typeparam>
        /// <param name="first">The original dictionary</param>
        /// <param name="second">The dictionary with additionalvalues</param>
        /// <returns>Returns a new dictionary</returns>
        public static IDictionary<TKey, TValue> Combine<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            first = first ?? new Dictionary<TKey, TValue>();
            second = second ?? new Dictionary<TKey, TValue>();
            var dictionary = second.ToDictionary(current => current.Key, current => current.Value);
            foreach (var current in first)
            {
                if (!dictionary.ContainsKey(current.Key))
                {
                    dictionary.Add(current.Key, current.Value);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Serializes an entity into query string
        /// </summary>
        /// <typeparam name="T">The type of entity to serialize</typeparam>
        /// <param name="obj">The entity to serialize</param>
        /// <param name="applyLowerCaseFirstChar">Whether the first letter of each parameter should be lower case</param>
        /// <param name="applyUrlEncoding">Encodes each parameter</param>
        /// <returns>The query string</returns>
        public static string ToQueryString<T>(T obj, bool applyLowerCaseFirstChar = true, bool applyUrlEncoding = true)
        {
            var queryString = string.Empty;
            var properties = typeof(T).GetTypeInfo().DeclaredProperties.OrderBy(p => p.Name).ToList();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(obj);
                if (propertyValue == null)
                {
                    continue;
                }

                var propertyStringValue = propertyValue.ToString();
                if (propertyValue is string)
                {
                    if (string.IsNullOrWhiteSpace(propertyStringValue))
                    {
                        continue;
                    }

                    if (applyUrlEncoding)
                    {
                        propertyStringValue = WebUtility.UrlEncode(propertyStringValue);
                    }
                }

                var propertyName = property.Name;
                if (applyLowerCaseFirstChar)
                {
                    propertyName = $"{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}";
                }

                queryString = $"{queryString}{(!string.IsNullOrWhiteSpace(queryString) ? "&" : string.Empty)}{propertyName}={propertyStringValue}";
            }

            return queryString;
        }

        /// <summary>
        /// Converst a string value (ticks from Epoch) to a date time
        /// </summary>
        /// <param name="unixTime">Ticks from Epoch</param>
        /// <returns>The date time</returns>
        public static DateTime ConvertToDateTimeFromUnixTimestamp(this string unixTime)
        {
            double parsedTime;
            DateTime result;
            result = Epoch.AddSeconds(!double.TryParse(unixTime, out parsedTime) ? 0.0 : parsedTime);
            return result;
        }
    }
}