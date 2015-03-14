using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM64DSe
{
    // Based on http://stackoverflow.com/questions/268321/bidirectional-1-to-1-dictionary-in-c-sharp 
    // By user: Joel in Gö

    public class BiDictionaryOneToOne<TFirst, TSecond>
    {
        IDictionary<TFirst, TSecond> firstToSecond = new Dictionary<TFirst, TSecond>();
        IDictionary<TSecond, TFirst> secondToFirst = new Dictionary<TSecond, TFirst>();

        public BiDictionaryOneToOne() { }

        // Allows you to specify a custom IEqualityComparer for the first dictionary when 
        // retrieving by Key, eg. specify your own IEqualityComparer for using a byte[] as a key type
        public BiDictionaryOneToOne(IEqualityComparer<TFirst> comparator)
        {
            firstToSecond = new Dictionary<TFirst, TSecond>(comparator);
        }

        // Allows you to specify a custom IEqualityComparer for the second dictionary when 
        // retrieving by Key, eg. specify your own IEqualityComparer for using a byte[] as a key type
        public BiDictionaryOneToOne(IEqualityComparer<TSecond> comparator)
        {
            secondToFirst = new Dictionary<TSecond, TFirst>(comparator);
        }

        // Allows you to specify custom IEqualityComparers for both dictionaries when 
        // retrieving by Key, eg. specify your own IEqualityComparer for using a byte[] as a key type
        public BiDictionaryOneToOne(IEqualityComparer<TFirst> comparatorFirst, IEqualityComparer<TSecond> comparatorSecond)
        {
            firstToSecond = new Dictionary<TFirst, TSecond>(comparatorFirst);
            secondToFirst = new Dictionary<TSecond, TFirst>(comparatorSecond);
        }

        /// <summary>
        /// Tries to add the pair to the dictionary.
        /// Throws an exception if either element is already in the dictionary
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void Add(TFirst first, TSecond second)
        {
            if (firstToSecond.ContainsKey(first) || secondToFirst.ContainsKey(second))
                throw new ArgumentException("Duplicate first or second");

            firstToSecond.Add(first, second);
            secondToFirst.Add(second, first);
        }

        /// <summary>
        /// Find the TSecond corresponding to the TFirst first
        /// Throws an exception if first is not in the dictionary.
        /// </summary>
        /// <param name="first">the key to search for</param>
        /// <returns>the value corresponding to first</returns>
        public TSecond GetByFirst(TFirst first)
        {
            TSecond second;
            if (!firstToSecond.TryGetValue(first, out second))
                throw new ArgumentException("first");

            return second;
        }

        /// <summary>
        /// Find the TFirst corresponing to the Second second.
        /// Throws an exception if second is not in the dictionary.
        /// </summary>
        /// <param name="second">the key to search for</param>
        /// <returns>the value corresponding to second</returns>
        public TFirst GetBySecond(TSecond second)
        {
            TFirst first;
            if (!secondToFirst.TryGetValue(second, out first))
                throw new ArgumentException("second");

            return first;
        }


        /// <summary>
        /// Remove the record containing first.
        /// If first is not in the dictionary, throws an Exception.
        /// </summary>
        /// <param name="first">the key of the record to delete</param>
        public void RemoveByFirst(TFirst first)
        {
            TSecond second;
            if (!firstToSecond.TryGetValue(first, out second))
                throw new ArgumentException("first");

            firstToSecond.Remove(first);
            secondToFirst.Remove(second);
        }

        /// <summary>
        /// Remove the record containing second.
        /// If second is not in the dictionary, throws an Exception.
        /// </summary>
        /// <param name="second">the key of the record to delete</param>
        public void RemoveBySecond(TSecond second)
        {
            TFirst first;
            if (!secondToFirst.TryGetValue(second, out first))
                throw new ArgumentException("second");

            secondToFirst.Remove(second);
            firstToSecond.Remove(first);
        }

        /// <summary>
        /// Tries to add the pair to the dictionary.
        /// Returns false if either element is already in the dictionary        
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>true if successfully added, false if either element are already in the dictionary</returns>
        public Boolean TryAdd(TFirst first, TSecond second)
        {
            if (firstToSecond.ContainsKey(first) || secondToFirst.ContainsKey(second))
                return false;

            firstToSecond.Add(first, second);
            secondToFirst.Add(second, first);
            return true;
        }

        /// <summary>
        /// Find the TSecond corresponding to the TFirst first.
        /// Returns false if first is not in the dictionary.
        /// </summary>
        /// <param name="first">the key to search for</param>
        /// <param name="second">the corresponding value</param>
        /// <returns>true if first is in the dictionary, false otherwise</returns>
        public Boolean TryGetByFirst(TFirst first, out TSecond second)
        {
            return firstToSecond.TryGetValue(first, out second);
        }

        /// <summary>
        /// Find the TFirst corresponding to the TSecond second.
        /// Returns false if second is not in the dictionary.
        /// </summary>
        /// <param name="second">the key to search for</param>
        /// <param name="first">the corresponding value</param>
        /// <returns>true if second is in the dictionary, false otherwise</returns>
        public Boolean TryGetBySecond(TSecond second, out TFirst first)
        {
            return secondToFirst.TryGetValue(second, out first);
        }

        /// <summary>
        /// Remove the record containing first, if there is one.
        /// </summary>
        /// <param name="first"></param>
        /// <returns> If first is not in the dictionary, returns false, otherwise true</returns>
        public Boolean TryRemoveByFirst(TFirst first)
        {
            TSecond second;
            if (!firstToSecond.TryGetValue(first, out second))
                return false;

            firstToSecond.Remove(first);
            secondToFirst.Remove(second);
            return true;
        }

        /// <summary>
        /// Remove the record containing second, if there is one.
        /// </summary>
        /// <param name="second"></param>
        /// <returns> If second is not in the dictionary, returns false, otherwise true</returns>
        public Boolean TryRemoveBySecond(TSecond second)
        {
            TFirst first;
            if (!secondToFirst.TryGetValue(second, out first))
                return false;

            secondToFirst.Remove(second);
            firstToSecond.Remove(first);
            return true;
        }

        public IDictionary<TFirst, TSecond> GetFirstToSecond()
        {
            return firstToSecond;
        }

        public IDictionary<TSecond, TFirst> GetSecondToFirst()
        {
            return secondToFirst;
        }

        /// <summary>
        /// The number of pairs stored in the dictionary
        /// </summary>
        public Int32 Count
        {
            get { return firstToSecond.Count; }
        }

        /// <summary>
        /// Removes all items from the dictionary.
        /// </summary>
        public void Clear()
        {
            firstToSecond.Clear();
            secondToFirst.Clear();
        }
    }
}
