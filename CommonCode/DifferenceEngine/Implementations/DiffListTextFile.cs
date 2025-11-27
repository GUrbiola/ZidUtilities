using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using ZidUtilities.CommonCode.DifferenceEngine.Structure;

namespace ZidUtilities.CommonCode.DifferenceEngine.Implementations
{
    /// <summary>
    /// Represents a list of lines loaded from a text file for use by the difference engine.
    /// Each line from the file is stored as a <see cref="TextLine"/> inside an internal collection.
    /// </summary>
    public class DiffListTextFile : IDiffList
    {
        private const int MaxLineLength = 1024;
        private ArrayList _lines;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffListTextFile"/> class by reading
        /// the specified text file. Each line of the file is validated against a maximum line
        /// length and stored as a <see cref="TextLine"/> in the internal collection.
        /// </summary>
        /// <param name="fileName">The path to the text file to read.</param>
        /// <remarks>
        /// The constructor opens the file using <see cref="StreamReader"/> and reads it line-by-line.
        /// If any line exceeds <see cref="MaxLineLength"/>, an <see cref="InvalidOperationException"/> is thrown.
        /// IO-related exceptions thrown by <see cref="StreamReader"/> (for example, <see cref="FileNotFoundException"/>, 
        /// <see cref="UnauthorizedAccessException"/>, or <see cref="IOException"/>) can also propagate out of this constructor.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the file contains a line longer than <see cref="MaxLineLength"/> characters.
        /// </exception>
        public DiffListTextFile(string fileName)
        {
            _lines = new ArrayList();
            using (StreamReader sr = new StreamReader(fileName))
            {
                String line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > MaxLineLength)
                    {
                        throw new InvalidOperationException(
                            string.Format("File contains a line greater than {0} characters.",
                            MaxLineLength.ToString()));
                    }
                    _lines.Add(new TextLine(line));
                }
            }
        }
        #region IDiffList Members

        /// <summary>
        /// Gets the number of lines stored in this diff list.
        /// </summary>
        /// <returns>The number of lines (items) contained in the internal collection.</returns>
        public int Count()
        {
            return _lines.Count;
        }

        /// <summary>
        /// Retrieves the item at the specified index from the diff list.
        /// </summary>
        /// <param name="index">Zero-based index of the item to retrieve.</param>
        /// <returns>
        /// The item stored at the given index as an <see cref="IComparable"/> (typically a <see cref="TextLine"/>).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="index"/> is less than 0 or greater than or equal to the number of items.
        /// </exception>
        public IComparable GetByIndex(int index)
        {
            return (TextLine)_lines[index];
        }
        #endregion

    }
}