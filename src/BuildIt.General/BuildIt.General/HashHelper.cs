using System.Collections.Generic;

namespace BuildIt
{
    /// <summary>
    /// Helps generate hashcodes
    /// https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Generates a hash code from two arguments
        /// </summary>
        /// <typeparam name="T1">Type of first argument</typeparam>
        /// <typeparam name="T2">Type of second argument</typeparam>
        /// <param name="arg1">The first arg</param>
        /// <param name="arg2">The second arg</param>
        /// <returns>The generated hashcode</returns>
        public static int GetHashCode<T1, T2>(T1 arg1, T2 arg2)
        {
            unchecked
            {
                return arg1.GetHashCode().CombineHashCode(arg2.GetHashCode());
            }
        }

        /// <summary>
        /// Generates a hash code from three arguments
        /// </summary>
        /// <typeparam name="T1">Type of first argument</typeparam>
        /// <typeparam name="T2">Type of second argument</typeparam>
        /// <typeparam name="T3">Type of third argument</typeparam>
        /// <param name="arg1">The first arg</param>
        /// <param name="arg2">The second arg</param>
        /// <param name="arg3">The third arg</param>
        /// <returns>The generated hashcode</returns>
        public static int GetHashCode<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            unchecked
            {
                int hash = arg1.GetHashCode();
                hash = hash.CombineHashCode(arg2.GetHashCode());
                return hash.CombineHashCode(arg3.GetHashCode());
            }
        }

        /// <summary>
        /// Generates a hash code from four arguments
        /// </summary>
        /// <typeparam name="T1">Type of first argument</typeparam>
        /// <typeparam name="T2">Type of second argument</typeparam>
        /// <typeparam name="T3">Type of third argument</typeparam>
        /// <typeparam name="T4">Type of fourth argument</typeparam>
        /// <param name="arg1">The first arg</param>
        /// <param name="arg2">The second arg</param>
        /// <param name="arg3">The third arg</param>
        /// <param name="arg4">The fourth arg</param>
        /// <returns>The generated hashcode</returns>
        public static int GetHashCode<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            unchecked
            {
                int hash = arg1.GetHashCode();
                hash = hash.CombineHashCode(arg2.GetHashCode());
                hash = hash.CombineHashCode(arg3.GetHashCode());
                return hash.CombineHashCode(arg4.GetHashCode());
            }
        }

        /// <summary>
        /// Generates a hashcode for a list of items
        /// </summary>
        /// <typeparam name="T">The type of items in the list</typeparam>
        /// <param name="list">The list of items</param>
        /// <returns>The hashcode</returns>
        public static int GetHashCode<T>(T[] list)
        {
            unchecked
            {
                int hash = 0;
                foreach (var item in list)
                {
                    hash = hash.CombineHashCode(item.GetHashCode());
                }

                return hash;
            }
        }

        /// <summary>
        /// Generates a hashcode for a list of items
        /// </summary>
        /// <typeparam name="T">The type of items in the list</typeparam>
        /// <param name="list">The list of items</param>
        /// <returns>The hashcode</returns>
        public static int GetHashCode<T>(IEnumerable<T> list)
        {
            unchecked
            {
                int hash = 0;
                foreach (var item in list)
                {
                    hash = hash.CombineHashCode(item.GetHashCode());
                }

                return hash;
            }
        }

        /// <summary>
        /// Gets a hashcode for a collection for that the order of items
        /// does not matter.
        /// So {1, 2, 3} and {3, 2, 1} will get same hash code.
        /// </summary>
        /// <typeparam name="T">The type of item in list</typeparam>
        /// <param name="list">The list</param>
        /// <returns>The hashcode</returns>
        public static int GetHashCodeForOrderNoMatterCollection<T>(
            IEnumerable<T> list)
        {
            unchecked
            {
                int hash = 0;
                int count = 0;
                foreach (var item in list)
                {
                    hash += item.GetHashCode();
                    count++;
                }

                return hash.CombineHashCode(count.GetHashCode());
            }
        }

        /// <summary>
        /// Alternative way to get a hashcode is to use a fluent
        /// interface like this:<br />
        /// return 0.CombineHashCode(field1).CombineHashCode(field2).
        ///     CombineHashCode(field3);
        /// </summary>
        /// <typeparam name="T">The type of entity</typeparam>
        /// <param name="hashCode">The original hashcode</param>
        /// <param name="arg">The next item to hash</param>
        /// <returns>The new hashcode</returns>
        public static int CombineHashCode<T>(this int hashCode, T arg)
        {
            unchecked
            {
                return (31 * hashCode) + arg?.GetHashCode() ?? 0;
            }
        }
    }
}