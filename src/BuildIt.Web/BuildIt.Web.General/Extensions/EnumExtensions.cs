using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildIt.Web.Core.Extensions
{
    public static class EnumExtensions
    {
        //MK TODO: After upgarding BuildIt.General libraries remove this code and use the one in general libraries
        public static IEnumerable<T> GetFlags<T>(this Enum input)
        {
            return Enum.GetValues(input.GetType())
                       .Cast<Enum>()
                       .Where(value => input.HasFlag(value))
                       .Cast<T>();
        }
    }
}
