namespace BuildIt
{
    public class TypeRef
    {
        // ReSharper disable once UnusedTypeParameter - This is used to generate a known generic from 
        // a type reference
        public class TypeWrapper<TType>
        { }

        public static TypeWrapper<TType> Get<TType>()
        {
            return new TypeWrapper<TType>();
        }

    }
}