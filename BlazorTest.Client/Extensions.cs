using System;
using System.Collections.Generic;

namespace BlazorTest.Client
{
    internal static class Extensions
    {
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> seed, Func<T, IEnumerable<T>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (seed == null) yield break;

            var stack = new Stack<IEnumerator<T>>();
            var currentEnumerator = seed.GetEnumerator();

            while (true)
            {
                if (currentEnumerator.MoveNext())
                {
                    var current = currentEnumerator.Current;
                    yield return current;
                    var recursiveEnumerator = selector.Invoke(current)?.GetEnumerator();

                    if (recursiveEnumerator != null)
                    {
                        stack.Push(currentEnumerator);
                        currentEnumerator = recursiveEnumerator;
                    }
                }
                else
                {
                    currentEnumerator.Dispose();
                    if (stack.Count == 0) yield break;
                    currentEnumerator = stack.Pop();
                }
            }
        }
    }
}
