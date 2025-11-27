using System;
using System.Collections;

namespace ZidUtilities.CommonCode.DifferenceEngine.Structure
{
    /// <summary>
    /// Represents a collection of <see cref="DiffState"/> instances indexed by destination index.
    /// Storage is lazily-initialized: each requested index will create a <see cref="DiffState"/> on demand.
    /// The backing storage strategy depends on the conditional compilation symbol <c>USE_HASH_TABLE</c>.
    /// </summary>
    internal class DiffStateList
    {
#if USE_HASH_TABLE
        private Hashtable _table;
#else
        private DiffState[] _array;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffStateList"/> class.
        /// </summary>
        /// <param name="destCount">
        /// The expected number of destination elements. This value is used to size the internal storage:
        /// - When using an array (default), an array of length <paramref name="destCount"/> is allocated.
        /// - When using a hash table (<c>USE_HASH_TABLE</c>), a hashtable is created with an initial capacity
        ///   derived from <paramref name="destCount"/>.
        /// </param>
        public DiffStateList(int destCount)
        {
#if USE_HASH_TABLE
            _table = new Hashtable(Math.Max(9, destCount / 10));
#else
            _array = new DiffState[destCount];
#endif
        }

        /// <summary>
        /// Gets the <see cref="DiffState"/> at the specified index, creating and storing a new instance if none exists.
        /// </summary>
        /// <param name="index">
        /// The destination index to retrieve. When the class is compiled without <c>USE_HASH_TABLE</c> this index
        /// must be within the bounds of the array allocated in the constructor (0 .. destCount - 1).
        /// When <c>USE_HASH_TABLE</c> is defined, any non-negative index is acceptable (the table is keyed by index).
        /// </param>
        /// <returns>
        /// The existing or newly created <see cref="DiffState"/> associated with <paramref name="index"/>.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when compiled without <c>USE_HASH_TABLE</c> and <paramref name="index"/> is outside the allocated array bounds.
        /// </exception>
        public DiffState GetByIndex(int index)
        {
#if USE_HASH_TABLE
            DiffState retval = (DiffState)_table[index];
            if (retval == null)
            {
                retval = new DiffState();
                _table.Add(index, retval);
            }
#else
            DiffState retval = _array[index];
            if (retval == null)
            {
                retval = new DiffState();
                _array[index] = retval;
            }
#endif
            return retval;
        }
    }
}
