namespace BuildIt
{
    /// <summary>
    /// Helper method for working with generics
    /// </summary>
    public static class TypeRef
    {
        // a type reference
        /// <summary>
        /// Generates an entity that references the generic type - useful for passing type information around as a generic
        /// instead of a type instance
        /// </summary>
        /// <typeparam name="TType">The type to be passed around</typeparam>
        // ReSharper disable once UnusedTypeParameter - This is used to generate a known generic from 
        public class TypeWrapper<TType>
        { }

        /// <summary>
        /// Creates a new instance of the type wrapper
        /// </summary>
        /// <typeparam name="TType">The type to wrap</typeparam>
        /// <returns>TypeWrapper instance</returns>
        public static TypeWrapper<TType> Get<TType>()
        {
            return new TypeWrapper<TType>();
        }

    }
}