using System;
using System.Collections;

namespace ZidUtilities.CommonCode.DifferenceEngine.Structure
{
    /// <summary>
    /// Represents an ordered collection of comparable items used by the diff engine.
    /// Implementations provide access to the number of elements and to individual elements by index.
    /// </summary>
    public interface IDiffList
    {
        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>
        /// An <see cref="int"/> representing the total count of items in the list.
        /// </returns>
        int Count();

        /// <summary>
        /// Retrieves the element at the specified zero-based index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to retrieve.
        /// </param>
        /// <returns>
        /// The element at the specified index as an <see cref="IComparable"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count()"/>.
        /// Implementations may also throw <see cref="IndexOutOfRangeException"/> depending on internal storage.
        /// </exception>
        IComparable GetByIndex(int index);
    }
}
