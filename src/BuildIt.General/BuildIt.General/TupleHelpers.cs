using System;

namespace BuildIt
{
    /// <summary>
    /// Helpers for creating tuples
    /// </summary>
    public static class TupleHelpers
    {
        /// <summary>
        /// Create tuple with single item
        /// </summary>
        /// <typeparam name="T1">Type of the first item</typeparam>
        /// <param name="item1">The first item</param>
        /// <returns>Single tuple</returns>
        public static Tuple<T1> Build<T1>(T1 item1)
        {
            return new Tuple<T1>(item1);
        }

        /// <summary>
        /// Creates a tuple with two items
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <param name="item1">The first item</param>
        /// <param name="item2">The second item</param>
        /// <returns>2 item Tuple</returns>
        public static Tuple<T1, T2> Build<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }

        /// <summary>
        /// Extend 1 item tuple to 2 items
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <param name="t">The tuple to extend</param>
        /// <param name="item">The second item</param>
        /// <returns>A new 2 item tuple</returns>
        public static Tuple<T1, T2> Extend<T1, T2>(this Tuple<T1> t, T2 item)
        {
            return new Tuple<T1, T2>(t.Item1, item);
        }

        /// <summary>
        /// Remove item from a tuple
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <param name="t">The tuple to remove from</param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1> Remove<T1, T2>(this Tuple<T1, T2> t)
        {
            return new Tuple<T1>(t.Item1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2> Replace<T1, T2>(this Tuple<T1, T2> t, T1 replacement)
        {
            return new Tuple<T1, T2>(replacement, t.Item2);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2> Replace<T1, T2>(this Tuple<T1, T2> t, T2 replacement)
        {
            return new Tuple<T1, T2>(t.Item1, replacement);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="item"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3> Extend<T1, T2, T3>(this Tuple<T1, T2> t, T3 item)
        {
            return new Tuple<T1, T2, T3>(t.Item1, t.Item2, item);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2> Remove<T1, T2, T3>(this Tuple<T1, T2, T3> t)
        {
            return new Tuple<T1, T2>(t.Item1, t.Item2);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3> Replace<T1, T2, T3>(this Tuple<T1, T2, T3> t, T1 replacement)
        {
            return new Tuple<T1, T2, T3>(replacement, t.Item2, t.Item3);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3> Replace<T1, T2, T3>(this Tuple<T1, T2, T3> t, T2 replacement)
        {
            return new Tuple<T1, T2, T3>(t.Item1, replacement, t.Item3);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3> Replace<T1, T2, T3>(this Tuple<T1, T2, T3> t, T3 replacement)
        {
            return new Tuple<T1, T2, T3>(t.Item1, t.Item2, replacement);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="item"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4> Extend<T1, T2, T3, T4>(this Tuple<T1, T2, T3> t, T4 item)
        {
            return new Tuple<T1, T2, T3, T4>(t.Item1, t.Item2, t.Item3, item);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3> Remove<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t)
        {
            return new Tuple<T1, T2, T3>(t.Item1, t.Item2, t.Item3);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4> Replace<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t, T1 replacement)
        {
            return new Tuple<T1, T2, T3, T4>(replacement, t.Item2, t.Item3, t.Item4);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4> Replace<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t, T2 replacement)
        {
            return new Tuple<T1, T2, T3, T4>(t.Item1, replacement, t.Item3, t.Item4);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4> Replace<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t, T3 replacement)
        {
            return new Tuple<T1, T2, T3, T4>(t.Item1, t.Item2, replacement, t.Item4);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4> Replace<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t, T4 replacement)
        {
            return new Tuple<T1, T2, T3, T4>(t.Item1, t.Item2, t.Item3, replacement);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <typeparam name="T5">The type of the fifth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="item"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4, T5> Extend<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4> t, T5 item)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t.Item1, t.Item2, t.Item3, t.Item4, item);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <typeparam name="T5">The type of the fifth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4> Remove<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t)
        {
            return new Tuple<T1, T2, T3, T4>(t.Item1, t.Item2, t.Item3, t.Item4);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <typeparam name="T5">The type of the fifth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4, T5> Replace<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t, T1 replacement)
        {
            return new Tuple<T1, T2, T3, T4, T5>(replacement, t.Item2, t.Item3, t.Item4, t.Item5);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <typeparam name="T5">The type of the fifth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4, T5> Replace<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t, T2 replacement)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t.Item1, replacement, t.Item3, t.Item4, t.Item5);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <typeparam name="T5">The type of the fifth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4, T5> Replace<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t, T3 replacement)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t.Item1, t.Item2, replacement, t.Item4, t.Item5);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <typeparam name="T5">The type of the fifth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4, T5> Replace<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t, T4 replacement)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t.Item1, t.Item2, t.Item3, replacement, t.Item5);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <typeparam name="T3">The type of the third item</typeparam>
        /// <typeparam name="T4">The type of the fourth item</typeparam>
        /// <typeparam name="T5">The type of the fifth item</typeparam>
        /// <param name="t">The tuple to modify</param>
        /// <param name="replacement"></param>
        /// <returns>A new tuple</returns>
        public static Tuple<T1, T2, T3, T4, T5> Replace<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t, T5 replacement)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t.Item1, t.Item2, t.Item3, t.Item4, replacement);
        }
    }
}