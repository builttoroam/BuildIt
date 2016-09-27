using System;

namespace BuildIt
{
    public static class TupleHelpers
    {
        public static Tuple<T1> Build<T1>(T1 item1)
        {
            return new Tuple<T1>(item1);
        }

        public static Tuple<T1, T2> Build<T1,T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1,T2>(item1,item2);
        }

        public static Tuple<T1, T2> Extend<T1, T2>(this Tuple<T1> t, T2 item)
        {
            return new Tuple<T1, T2>(t.Item1,item);
        }

        public static Tuple<T1> Remove<T1, T2>(this Tuple<T1, T2> t)
        {
            return new Tuple<T1>(t.Item1);
        }

        public static Tuple<T1, T2> Replace<T1, T2>(this Tuple<T1, T2> t, T1 replacement)
        {
            return new Tuple<T1, T2>(replacement, t.Item2);
        }

        public static Tuple<T1, T2> Replace<T1, T2>(this Tuple<T1, T2> t, T2 replacement)
        {
            return new Tuple<T1, T2>(t.Item1,replacement);
        }




        public static Tuple<T1, T2,T3> Extend<T1, T2, T3>(this Tuple<T1, T2> t, T3 item)
        {
            return new Tuple<T1, T2, T3>(t.Item1,t.Item2, item);
        }

        public static Tuple<T1, T2> Remove<T1, T2, T3>(this Tuple<T1, T2, T3> t)
        {
            return new Tuple<T1, T2>(t.Item1,t.Item2);
        }

        public static Tuple<T1, T2, T3> Replace<T1, T2,T3>(this Tuple<T1, T2, T3> t, T1 replacement)
        {
            return new Tuple<T1, T2, T3>(replacement,t.Item2,t.Item3);
        }
        public static Tuple<T1, T2, T3> Replace<T1, T2, T3>(this Tuple<T1, T2, T3> t, T2 replacement)
        {
            return new Tuple<T1, T2, T3>(t.Item1, replacement, t.Item3);
        }
        public static Tuple<T1, T2, T3> Replace<T1, T2, T3>(this Tuple<T1, T2, T3> t, T3 replacement)
        {
            return new Tuple<T1, T2, T3>(t.Item1, t.Item2, replacement);
        }



        public static Tuple<T1, T2, T3,T4> Extend<T1, T2, T3, T4>(this Tuple<T1, T2, T3> t, T4 item)
        {
            return new Tuple<T1, T2, T3, T4>(t.Item1, t.Item2,t.Item3, item);
        }

        public static Tuple<T1, T2, T3> Remove<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t)
        {
            return new Tuple<T1, T2, T3>(t.Item1, t.Item2,t.Item3);
        }

        public static Tuple<T1, T2, T3, T4> Replace<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t, T1 replacement)
        {
            return new Tuple<T1, T2, T3, T4>(replacement, t.Item2,t.Item3,t.Item4);
        }
        public static Tuple<T1, T2, T3, T4> Replace<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t, T2 replacement)
        {
            return new Tuple<T1, T2, T3, T4>(t.Item1, replacement, t.Item3, t.Item4);
        }
        public static Tuple<T1, T2, T3, T4> Replace<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t, T3 replacement)
        {
            return new Tuple<T1, T2, T3, T4>(t.Item1, t.Item2, replacement, t.Item4);
        }
        public static Tuple<T1, T2, T3, T4> Replace<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t, T4 replacement)
        {
            return new Tuple<T1, T2, T3, T4>(t.Item1, t.Item2, t.Item3, replacement);
        }




        public static Tuple<T1, T2, T3, T4,T5> Extend<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4> t, T5 item)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t.Item1, t.Item2, t.Item3,t.Item4, item);
        }

        public static Tuple<T1, T2, T3, T4> Remove<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t)
        {
            return new Tuple<T1, T2, T3, T4>(t.Item1, t.Item2, t.Item3,t.Item4);
        }

        public static Tuple<T1, T2, T3, T4, T5> Replace<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t, T1 replacement)
        {
            return new Tuple<T1, T2, T3, T4, T5>(replacement, t.Item2, t.Item3, t.Item4,t.Item5);
        }
        public static Tuple<T1, T2, T3, T4, T5> Replace<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t, T2 replacement)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t.Item1, replacement, t.Item3, t.Item4, t.Item5);
        }
        public static Tuple<T1, T2, T3, T4, T5> Replace<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t, T3 replacement)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t.Item1, t.Item2, replacement, t.Item4, t.Item5);
        }
        public static Tuple<T1, T2, T3, T4, T5> Replace<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t, T4 replacement)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t.Item1, t.Item2, t.Item3, replacement, t.Item5);
        }
        public static Tuple<T1, T2, T3, T4, T5> Replace<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> t, T5 replacement)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t.Item1, t.Item2, t.Item3, t.Item4, replacement);
        }
    }
}