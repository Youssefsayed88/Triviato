using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Assets.AdvancedMonoBehaviour.Scripts
{
    public static class AMTools
    {
        public static IEnumerator SetActiveFalseAfterCertainTime(float time, GameObject gameObject)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false);
        }
        
        public static string Reverse( string s )
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse( charArray );
            return new string( charArray );
        }
        
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            var buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
        
        public static List<T> ShuffleList<T>(this List<T> enumerable)
        {
            var r = new Random();
            return enumerable.OrderBy(x=>r.Next()).ToList();
        }
    }
}