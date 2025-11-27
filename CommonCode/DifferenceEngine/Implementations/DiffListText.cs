using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using ZidUtilities.CommonCode.DifferenceEngine.Structure;

namespace ZidUtilities.CommonCode.DifferenceEngine.Implementations
{
    /// <summary>
    /// A text-backed implementation of <see cref="IDiffList"/> where each entry
    /// represents a line from a text source.
    /// </summary>
    public class DiffListText : IDiffList
    {
        /// <summary>
        /// Maximum allowed length for a single line when loading from a file.
        /// </summary>
        private const int MaxLineLength = 2048;

        /// <summary>
        /// Internal storage for the text lines.
        /// </summary>
        private List<string> Lines;

        /*
         * Plan (pseudocode):
         * - Add XML doc comments for the class, all constructors, and all methods.
         * - Each constructor should explain its purpose and parameters.
         * - The LoadFromFile method should document parameters, exceptions, and behavior.
         * - The Count method should describe the returned value.
         * - The GetByIndex method should describe parameter, return value and behavior for bounds.
         * - Do not modify existing logic or signatures; only add documentation comments.
         */

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffListText"/> class
        /// with an empty list of lines.
        /// </summary>
        public DiffListText()
        {
            Lines = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffListText"/> class
        /// by splitting the provided string into lines using the environment newline.
        /// </summary>
        /// <param name="str">
        /// The input string containing zero or more lines separated by
        /// <see cref="Environment.NewLine"/>. An empty string results in a single
        /// empty line in the list.
        /// </param>
        public DiffListText(string str)
        {
            Lines = new List<string>();
            foreach (string line in str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))    
            {
                Lines.Add(line);
            }
        }

        /// <summary>
        /// Loads lines from the specified file into this instance, replacing any
        /// existing lines.
        /// </summary>
        /// <param name="FileName">The path to the text file to read.</param>
        /// <remarks>
        /// Each line is read using <see cref="StreamReader.ReadLine"/>. If any
        /// line exceeds <see cref="MaxLineLength"/>, an <see cref="InvalidOperationException"/>
        /// is thrown. The file is read until EOF.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a line longer than <see cref="MaxLineLength"/> characters is encountered.
        /// </exception>
        public void LoadFromFile(string FileName)
        {
            Lines = new List<string>();
            using (StreamReader sr = new StreamReader(FileName))
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
                    Lines.Add(line);
                }
            }
        }

        /// <summary>
        /// Gets the number of lines currently stored in this instance.
        /// </summary>
        /// <returns>The count of lines as an <see cref="int"/>.</returns>
        public int Count()
        {
            return Lines.Count;
        }

        /// <summary>
        /// Retrieves the line at the given zero-based index as an <see cref="IComparable"/>.
        /// The returned object is a <see cref="TextLine"/> which includes a trailing
        /// <see cref="Environment.NewLine"/> for every line except the last one.
        /// </summary>
        /// <param name="index">Zero-based index of the line to retrieve.</param>
        /// <returns>
        /// A <see cref="TextLine"/> representing the requested line. If the index
        /// is out of range, an empty <see cref="TextLine"/> is returned.
        /// </returns>
        public IComparable GetByIndex(int index)
        {
            if (index >= 0 && index < Lines.Count)
            {
                if(index == Lines.Count -1)
                    return new TextLine(Lines[index]);
                else
                    return new TextLine(Lines[index] + Environment.NewLine);
            }
            return new TextLine("");
        }
    }
}