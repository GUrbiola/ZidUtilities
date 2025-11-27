using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZidUtilities.CommonCode.DataComparison
{
    /// <summary>
    /// Represents a difference between two cell values for a specific column.
    /// Holds the column identifier and the two differing values.
    /// </summary>
    public class CellDifference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CellDifference"/> class for the specified column.
        /// </summary>
        /// <param name="columnName">The name of the column where the difference was detected. Must not be null; use an empty string if unknown.</param>
        public CellDifference(string columnName)
        {
            ColumnName = columnName;
        }

        /// <summary>
        /// Gets or sets the zero-based column number/index where the difference occurred.
        /// </summary>
        /// <value>
        /// An integer representing the column index. Default is 0 unless set explicitly.
        /// </value>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Gets or sets the name of the column where the difference occurred.
        /// </summary>
        /// <value>
        /// A string containing the column name. This is set via the constructor and may be updated later.
        /// </value>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the first value observed for the column (e.g., from the first dataset).
        /// </summary>
        /// <value>
        /// A string representing the first observed cell value. May be null if the value is missing.
        /// </value>
        public string Value1 { get; set; }

        /// <summary>
        /// Gets or sets the second value observed for the column (e.g., from the second dataset).
        /// </summary>
        /// <value>
        /// A string representing the second observed cell value. May be null if the value is missing.
        /// </value>
        public string Value2 { get; set; }  
    }
}
