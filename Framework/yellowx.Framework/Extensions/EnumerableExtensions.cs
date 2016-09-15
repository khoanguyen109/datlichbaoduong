using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace yellowx.Framework.Extensions
{
    public static class EnumerableExtensions
    {
        private static readonly Random _random = new Random();

        [DebuggerStepThrough]
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void ShuffleWell<T>(this IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            var n = list.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                var k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static IEnumerable<TSource> ConditionalWhere<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> filter)
        {
            if (condition)
                return source.Where(filter);

            return source;
        }

        public static IEnumerable<TSource> MaybeOrderBy<TSource, TKey>(this IEnumerable<TSource> source, bool condition, Func<TSource, TKey> orderBy)
        {
            if (condition)
                return source.OrderBy(orderBy);

            return source;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source)
        {
            T current = default(T);
            bool first = true;
            foreach (T x in source)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    yield return current;
                }
                current = x;
            }
        }

        /// <summary>
        /// Turn some sequence of stuff into a string.
        /// </summary>
        /// <param name="toString">How we turn some arbitrary element into a string.</param>
        /// <param name="toStringLast">How we turn the last element into a string.  If not specified uses toString</param>
        /// <param name="toStringFirst">How we turn the first element into a string.  If not specified uses toString</param>
        /// <returns></returns>
        public static string StringConcat<T>(this IEnumerable<T> source, Func<T, string> toString, Func<T, string> toStringLast = null, Func<T, string> toStringFirst = null)
        {
            toStringLast = toStringLast ?? toString;
            toStringFirst = toStringFirst ?? toString;

            var lsource = source.ToList();
            int total = lsource.Count;

            int ii = 0;
            return lsource.Aggregate(
                new StringBuilder(),
                (acc, member) =>
                {
                    ii++;
                    var ret = acc.Append(ii == total
                                             ? toStringLast(member)
                                             : ii == 1 ? toStringFirst(member) : toString(member));
                    return ret;
                },
                x => x.ToString());
        }

        /// <summary>
        /// Turn some elements into strings then concatenate.
        /// </summary>
        /// <param name="toString">Convert each element to a string.</param>
        /// <param name="delimiter">What to put between the strings.</param>
        /// <param name="trimEnd">Do we want to not put the delimiter at the end?  Defaults to true (i.e. remove trailing delimiter)</param>
        /// <param name="lastDelimiter">Do we want a different last delimiter.  This is with respect to trim end so will appear before last element
        /// if trimEnd; at the end otherwise.</param>
        /// <returns></returns>
        public static string StringConcat<T>(this IEnumerable<T> source, Func<T, string> toString, string delimiter,
                                             bool trimEnd = true, string lastDelimiter = null)
        {
            return source.Select(toString).StringConcat(delimiter, trimEnd, lastDelimiter);
        }

        /// <summary>
        /// Concatenates some strings.
        /// </summary>
        /// <param name="delimiter">What to put between the strings.</param>
        /// <param name="trimEnd">Do we want to not put the delimiter at the end?  Defaults to true (i.e. remove trailing delimiter)</param>
        /// <param name="lastDelimiter">Do we want a different last delimiter.  This is with respect to trim end so will appear before last element
        /// if trimEnd; at the end otherwise.</param>
        /// <returns></returns>
        public static string StringConcat(this IEnumerable<string> source, string delimiter, bool trimEnd = true, string lastDelimiter = null)
        {
            if (source == null)
                return string.Empty;

            //just deal with the special case of only one item with end trimmed as it was getting too hairy trying to 
            //work it out otherwise!
            if (source.Count() == 1 && trimEnd)
                return source.First();

            if (lastDelimiter == null)
            {
                return StringConcat(source, x => x + delimiter, trimEnd ? x => x : (Func<string, string>)null);
            }

            if (trimEnd)
            {
                return StringConcat(source, x => delimiter + x, x => lastDelimiter + x, x => x);
            }

            return StringConcat(source, x => x + delimiter, x => x + lastDelimiter);
        }

        public static IEnumerable<T> OrIfNoneThen<T>(this IEnumerable<T> enumerable, params T[] ifNone)
        {
            return enumerable.Any() ? enumerable : ifNone;
        }

        /// <summary>
        /// Method to partition an IEnumerable into chunks, courtesy of Jon Skeet
        /// http://stackoverflow.com/questions/438188/split-a-collection-into-n-parts-with-linq/438208#438208
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
        {
            T[] array = null;
            int count = 0;
            foreach (T item in source)
            {
                if (array == null)
                {
                    array = new T[size];
                }
                array[count] = item;
                count++;
                if (count == size)
                {
                    yield return new ReadOnlyCollection<T>(array);
                    array = null;
                    count = 0;
                }
            }
            if (array != null)
            {
                Array.Resize(ref array, count);
                yield return new ReadOnlyCollection<T>(array);
            }
        }

        public static int RemoveWhere<T>(this List<T> list, Func<T, bool> filter)
        {
            int removeCount = 0;
            foreach (var match in list.Where(filter).ToList())
            {
                removeCount++;
                list.Remove(match);
            }

            return removeCount;
        }

        public static Dictionary<TK, TV> FilterDict<TK, TV>(this Dictionary<TK, TV> dict, Func<KeyValuePair<TK, TV>, bool> where)
        {
            return dict.Where(where).ToDictionary(x => x.Key, x => x.Value);
        }

        public static IEnumerable<T> WrapInEnumerable<T>(this T obj, bool emptyIfNull = true)
        {
            if (emptyIfNull && obj == null)
                yield break;

            yield return obj;
        }

        public static IEnumerable<T> UnionParams<T>(this IEnumerable<T> sequence, params T[] extras)
        {
            return sequence.Union(extras.AsEnumerable());
        }

        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            if (sequence == null)
                return;

            foreach (var s in sequence)
                action(s);
        }

        [DebuggerStepThrough]
        public static IEnumerable<TResult> ForEach<T, TResult>(this IEnumerable<T> sequence, Func<T, TResult> func)
        {
            var results = new Collection<TResult>();
            if (sequence.IsNullOrEmpty())
                return results;
            sequence.ForEach(s =>
            {
                var result = func(s);
                result.IsNotNull(() => results.Add(result));
            });
            return results;
        }

        [DebuggerStepThrough]
        public static IDictionary<TKey, TValue> ForEach<T, TKey, TValue>(this IEnumerable<T> sequence, Func<T, TKey> keyFunc, Func<T, TValue> valueFunc)
        {
            var dict = new Dictionary<TKey, TValue>();
            sequence.ForEach(s =>
            {
                var tkey = keyFunc(s);
                var tvalue = valueFunc(s);
                if (!dict.ContainsKey(tkey))
                    dict.Add(tkey, tvalue);
            });
            return dict;
        }

        [DebuggerStepThrough]
        public static bool IsSubsetOf<T>(this IEnumerable<T> sequence, IEnumerable<T> target)
        {
            return !sequence.Except(target).Any();
        }

        /// <summary>
        /// Find out the root of a list of segments.
        /// A root segment will be the start of other segments.
        /// </summary>
        /// <param name="">Put into  6 segments: abcd, abc, abcde, xyzde, xyz, xy</param>
        /// <returns>abc, xy</returns>
        public static IEnumerable<T> FindRootSegments<T>(this IEnumerable<T> segments, Func<T, T, bool> segmentFinder, Func<T, T, T> segmentChooser)
        {
            var rootSegments = new Collection<T>();
            segments.ForEach(segment =>
            {
                var rootSegment = rootSegments.FirstOrDefault(rs => segmentFinder(segment, rs));
                rootSegments.Remove(rootSegment);
                rootSegments.Add(segmentChooser(rootSegment, segment));
            });
            return rootSegments;
        }
    }
}
