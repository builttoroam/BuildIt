using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;

namespace BuildIt
{
    public static class Utilities
    {

        public static T DecodeJson<T>(this string jsonString)
        {
            try
            {
                var decoded = InternalDecodeJson(jsonString, typeof(T));
                if (decoded == null) return default(T);
                return (T)decoded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return default(T);
            }
        }

        public static object DecodeJson(this string jsonString, Type jsonType)
        {
            try
            {

                return InternalDecodeJson(jsonString, jsonType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private static object InternalDecodeJson(string jsonString, Type jsonType)
        {
            if (string.IsNullOrWhiteSpace(jsonString)) return null;
            var builder = Encoding.UTF8.GetBytes(jsonString);
            using (var strm = new MemoryStream(builder))
            {
                var serializer = new DataContractJsonSerializer(jsonType);
                var obj = serializer.ReadObject(strm);
                return obj;
            }
        }

        public static string EncodeJson<T>(this T objectToJsonify)
        {
            // ReSharper disable CompareNonConstrainedGenericWithNull
            if (objectToJsonify == null) return null;
            // ReSharper restore CompareNonConstrainedGenericWithNull

            using (var strm = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(objectToJsonify.GetType());
                serializer.WriteObject(strm, objectToJsonify);
                strm.Flush();
                var bytes = strm.ToArray();
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
        }


     

        public static bool DoForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null) return false;
            foreach (var item in source)
            {
                action(item);
            }
            return true;
        }

        public static TList Fill<TList, T>(this TList source, IEnumerable<T> items) where TList : IList<T>
        {
            if (source == null || items == null) return source;
            if (items != null)
            {
                foreach (var item in items)
                {
                    source.Add(item);
                }
            }
            return source;
        }

        public static TList Replace<TList, T>(this TList source, IEnumerable<T> newItems) where TList : IList<T>
        {
            if (source == null || newItems == null) return source;
            source.Clear();
            source.Fill(newItems);
            return source;
        }



        public static StringBuilder AppendCommaIfNeeded(this StringBuilder sb)
        {
            if (sb!=null && sb.Length > 0) sb.Append(",");
            return sb;
        }
        public static StringBuilder AppendOnCondition(this StringBuilder sb, Func<bool> condition, string toAppend)
        {
            if (sb!=null && condition!=null && condition()) sb.Append(toAppend);
            return sb;
        }

        public static StringBuilder AppendIfNotNullOrEmpty(this StringBuilder sb, string toAppend)
        {
            if (sb!=null && !string.IsNullOrEmpty(toAppend)) sb.Append(toAppend);
            return sb;
        }



        public static bool CopyToStream(this Stream sourceStream, Stream destinationStream)
        {
            if (sourceStream == null || 
                !sourceStream.CanRead ||
                destinationStream == null ||
                !destinationStream.CanWrite 
                ) return false;
            try
            {
                var readWriteBuffer = new byte[1000];
                int cnt;
                while ((cnt = sourceStream.Read(readWriteBuffer, 0, readWriteBuffer.Length)) > 0)
                {
                    destinationStream.Write(readWriteBuffer, 0, cnt);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }


        public static void SafeRaise<TParameter1>(this EventHandler<ParameterEventArgs<TParameter1>> handler, object sender, TParameter1 args)
        {
            handler.SafeRaise(sender, new ParameterEventArgs<TParameter1>(args));
        }
        public static void SafeRaise<TParameter1, TParameter2>(this EventHandler<DualParameterEventArgs<TParameter1, TParameter2>> handler, object sender, TParameter1 arg1, TParameter2 arg2)
        {
            handler.SafeRaise(sender, new DualParameterEventArgs<TParameter1, TParameter2>(arg1, arg2));
        }
        public static void SafeRaise<TParameter1, TParameter2, TParameter3>(this EventHandler<TripleParameterEventArgs<TParameter1, TParameter2, TParameter3>> handler, object sender, TParameter1 arg1, TParameter2 arg2, TParameter3 arg3)
        {
            handler.SafeRaise(sender, new TripleParameterEventArgs<TParameter1, TParameter2, TParameter3>(arg1, arg2, arg3));
        }

        public static void SafeRaise(this EventHandler handler, object sender, EventArgs args = null)
        {
            if (args == null)
            {
                args = EventArgs.Empty;
            }
            handler?.Invoke(sender, args);
        }

        public static void SafeRaise<T>(this EventHandler<T> handler, object sender, T args)
            where T : EventArgs
        {
            handler?.Invoke(sender, args);
        }


        public const string PageStateKey = "Page";


        [Obsolete("Use SafeValue instead")]
        public static TReturn SafeDictionaryValue<TKey, TValue, TReturn>(this IDictionary<TKey, TValue> dictionary,
            TKey key)
        {
            return dictionary.SafeValue<TKey, TValue, TReturn>(key);
        }

        public static TReturn SafeValue<TKey, TValue, TReturn>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null) return default(TReturn);
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

        [Obsolete("Use SafeValue instead")]
        public static TValue SafeDictionaryValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return SafeValue(dictionary, key);
        }

        public static TValue SafeValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null) return default(TValue);
            TValue val;
            return dictionary.TryGetValue(key, out val) ? val : default(TValue);
        }



        public static string SafeDecendentValue(this XElement element, string name)
        {
            var dec = element?.Descendants(name).FirstOrDefault();
            return dec?.Value ?? string.Empty;
        }

        public static string SafeDecendentValue(this XElement element, XName name)
        {
            var dec = element?.Descendants(name).FirstOrDefault();
            return dec?.Value ?? string.Empty;
        }



        public static string SafeElementValue(this XElement element, string name)
        {
            if (element?.Element(name) == null) return string.Empty;
            // ReSharper disable PossibleNullReferenceException
            return element.Element(name).Value;
            // ReSharper restore PossibleNullReferenceException
        }

        public static string SafeElementValue(this XElement element, XName name)
        {
            if (element?.Element(name) == null) return string.Empty;
            // ReSharper disable PossibleNullReferenceException
            return element.Element(name).Value;
            // ReSharper restore PossibleNullReferenceException
        }

        public static string SafeAttributeValue(this XElement element, XName name)
        {
            if (element?.Attribute(name) == null) return string.Empty;
            // ReSharper disable PossibleNullReferenceException
            return element.Attribute(name).Value;
            // ReSharper restore PossibleNullReferenceException
        }

        public static string SafeAttributeValue(this XElement element, string name)
        {
            if (element?.Attribute(name) == null) return string.Empty;
            // ReSharper disable PossibleNullReferenceException
            return element.Attribute(name).Value;
            // ReSharper restore PossibleNullReferenceException
        }





        public static bool DoIfNotNull<T>(this T item, Action<T> action) where T : class
        {
            if (item == null || action==null) return false;
            action(item);
            return true;
        }

        public static bool ExecuteIfNotNull(this object instance, Action method)
        {
            if (instance == null || method==null) return false;
            method();
            return true;
        }





        public static T EnumParse<T>(this string enumValue, bool ignoreCase = true) where T : struct
        {
            try
            {
                if (string.IsNullOrEmpty(enumValue)) return default(T);
                return (T)Enum.Parse(typeof(T), enumValue, ignoreCase);
            }
            catch
            {
                return default(T);
            }
        }

        public static double ToDouble(this string doubleValue, double defaultValue = 0.0)
        {
            if (string.IsNullOrWhiteSpace(doubleValue)) return defaultValue;
            double retValue;
            if (double.TryParse(doubleValue, out retValue)) return retValue;
            return defaultValue;
        }

        public static DateTime ToDate(this string doubleValue)
        {
            if (string.IsNullOrWhiteSpace(doubleValue)) return DateTime.Now;
            DateTime retValue;
            if (DateTime.TryParse(doubleValue, out retValue)) return retValue;
            return DateTime.Now;
        }

        public static int ToInt(this string intValue, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(intValue)) return defaultValue;
            int retValue;
            if (int.TryParse(intValue, out retValue)) return retValue;
            return defaultValue;
        }

     
        public static T ReadObject<T>(this Stream stream)
        {
            try
            {
                using (var reader = new StreamReader(stream))
                {
#if DEBUG
                    var txt = reader.ReadToEnd();
                    Debug.WriteLine(txt);
                    stream.Position = 0;
#endif
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    var obj = serializer.ReadObject(stream);
                    return (T)obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to read object from json stream - " + ex.Message);
                return default(T);
            }
        }

        public static string FriendlyDateFormat(this DateTime date)
        {
            if (date == DateTime.MinValue || date == DateTime.MaxValue) return " ";
            var days = date.Date.Subtract(DateTime.Now.Date).TotalDays;
            if (days < -365 * 2) return (-(int)days / 365) + " years ago";
            if (days < -365) return "Last year";
            if (days < -31) return ((int)(-days / 31)) + " months ago";
            if (days < -14) return ((int)(-days / 7)) + " weeks ago";
            if (days < -7) return "Last week";
            if (days < -1) return (-days) + " days ago";
            if (date.Date < DateTime.Now.Date) return "Yesterday";
            if (days > 365 * 2) return ((int)days / 365) + " years";
            if (days > 365) return "Next year";
            if (days > 31) return ((int)(days / 31)) + " months";
            if (days > 14) return ((int)(days / 7)) + " weeks";
            if (days > 7) return "Next week";
            if (days > 1) return days + " days";
            if (date.Date > DateTime.Now.Date) return "Tomorrow";
            return date.ToString();//.ToShortTimeString();
        }

        public static IDictionary<TKey, TValue> Combine<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            first = (first ?? new Dictionary<TKey, TValue>());
            second = (second ?? new Dictionary<TKey, TValue>());
            var dictionary = second.ToDictionary(current => current.Key, current => current.Value);
            foreach (KeyValuePair<TKey, TValue> current in first)
            {
                if (!dictionary.ContainsKey(current.Key))
                {
                    dictionary.Add(current.Key, current.Value);
                }
            }
            return dictionary;
        }


      

        private static DateTime Epoch => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ConvertToDateTimeFromUnixTimestamp(this string unixTime)
        {
            double parsedTime;
            DateTime result;
            result = Epoch.AddSeconds(!double.TryParse(unixTime, out parsedTime) ? 0.0 : parsedTime);
            return result;
        }


    }
}