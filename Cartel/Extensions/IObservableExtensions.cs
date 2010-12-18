using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Cartel.Extensions
{
    public static class IObservableExtensions
    {
        private static readonly object CONSTANT_OBJECT = new object();

        /// <summary>
        /// Removes items in an observable which have been encountered before
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable">the observable collection</param>
        /// <param name="keySelector">transforms an item into a key</param>
        /// <param name="previouslyEncountered">a set of items we have seen before (as keys)</param>
        /// <returns></returns>
        public static IObservable<T> Filter<T>(this IObservable<T> observable, Func<T, string> keySelector, ConcurrentDictionary<string, object> previouslyEncountered)
        {
            return observable
                .Select(a => new KeyValuePair<T, string>(a, keySelector(a)))
                .Where(a => !previouslyEncountered.ContainsKey(a.Value))
                .Do(a => previouslyEncountered.AddOrUpdate(a.Value, CONSTANT_OBJECT, (x, y) => CONSTANT_OBJECT))
                .Select(a => a.Key);
        }

        /// <summary>
        /// In a sequence of items, this removes subsequences of the same repeated item
        /// eg. [a,b,b,b,c,b] => [a,b,c,b]
        /// </summary>
        /// <typeparam name="T">Type of the observable</typeparam>
        /// <param name="observable">Observable collection</param>
        /// <param name="comparer">A comparer for items</param>
        /// <returns>An observable, with no subsequences of non distinct items</returns>
        public static IObservable<T> RemoveRepeatedSequences<T>(this IObservable<T> observable, Func<T, T, bool> comparer)
        {
            T lastItem = default(T);
            return observable.Where(a =>
                {
                    bool r = comparer(a, lastItem);
                    lastItem = a;
                    return !r;
                });
        }
    }
}
