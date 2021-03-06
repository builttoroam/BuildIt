using Microsoft.Pex.Framework;

namespace System.Collections.Generic
{
    /// <summary>A factory for System.Collections.Generic.Dictionary`2[System.Int32,System.Int32] instances</summary>
    public static partial class DictionaryFactory
    {
        /// <summary>A factory for System.Collections.Generic.Dictionary`2[System.Int32,System.Int32] instances</summary>
        [PexFactoryMethod(typeof(Dictionary<int, int>))]
        public static Dictionary<int, int> Create(int capacity_i, int key_i1, int value_i2)
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>(capacity_i);
            dictionary[key_i1] = value_i2;
            return dictionary;

            // TODO: Edit factory method of Dictionary`2<Int32,Int32>
            // This method should be able to configure the object in all possible ways.
            // Add as many parameters as needed,
            // and assign their values to each field by using the API.
        }
    }

    public class CustomDictionary : Dictionary<int, int>, IDictionary<Int32, Int32>
    {
    }
}