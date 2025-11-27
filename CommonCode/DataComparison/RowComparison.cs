using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ZidUtilities.CommonCode.DataComparison
{
    /// <summary>
    /// Represents the comparison result for a single row between two data tables.
    /// Holds flags indicating presence in each table, the identifying row id, the two DataRow instances,
    /// any cell-level differences, and helper properties describing the comparison outcome.
    /// </summary>
    public class RowComparison
    {
        /// <summary>
        /// Gets or sets a value indicating whether the row exists in the first table.
        /// </summary>
        /// <value>True if the row is present in table 1; otherwise false.</value>
        public bool IsInTable1 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the row exists in the second table.
        /// </summary>
        /// <value>True if the row is present in table 2; otherwise false.</value>
        public bool IsInTable2 { get; set; }

        /// <summary>
        /// Gets a value indicating whether the row was found in both tables.
        /// </summary>
        /// <returns>True when <see cref="IsInTable1"/> and <see cref="IsInTable2"/> are both true; otherwise false.</returns>
        public bool IsFound { get { return IsInTable1 && IsInTable2; } }

        /// <summary>
        /// Gets a value indicating whether the row exists only in the first table.
        /// </summary>
        /// <returns>True when the row is present in table 1 and not in table 2.</returns>
        public bool OnlyTable1 { get { return IsInTable1 && !IsInTable2; } }

        /// <summary>
        /// Gets a value indicating whether the row exists only in the second table.
        /// </summary>
        /// <returns>True when the row is present in table 2 and not in table 1.</returns>
        public bool OnlyTable2 { get { return !IsInTable1 && IsInTable2; } }

        /// <summary>
        /// Gets or sets an identifier that represents the row (typically a concatenation of key column values).
        /// </summary>
        /// <value>A string uniquely identifying the row within the comparison context; may be null or empty if unknown.</value>
        public string RowId { get; set; }

        /// <summary>
        /// Gets or sets the list of cell-level differences detected for this row.
        /// </summary>
        /// <remarks>
        /// Each item in the list is a <see cref="CellDifference"/> describing the differing column and the two values.
        /// The list is initialized by the constructor and should not be assumed to be null.
        /// </remarks>
        public List<CellDifference> Differences { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataRow"/> instance from the first table corresponding to this comparison.
        /// </summary>
        /// <value>
        /// The <see cref="DataRow"/> from table 1 when available; otherwise null.
        /// </value>
        public DataRow Row1 { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataRow"/> instance from the second table corresponding to this comparison.
        /// </summary>
        /// <value>
        /// The <see cref="DataRow"/> from table 2 when available; otherwise null.
        /// </value>
        public DataRow Row2 { get; set; }

        /// <summary>
        /// Gets a value indicating whether the rows found in both tables match exactly (no cell differences).
        /// </summary>
        /// <returns>True if <see cref="Differences"/> contains zero items and the row was found in both tables; otherwise false.</returns>
        public bool ExactMatch { get { return Differences.Count == 0 && IsFound; } }

        /// <summary>
        /// Gets a human-readable comment that summarizes the comparison outcome for this row.
        /// </summary>
        /// <returns>
        /// - "Exact Match" when the row exists in both tables and there are no differences.
        /// - "Differences Found" when the row exists in both tables but some cells differ.
        /// - "Only Found in: Table1" or "Only Found in: Table2" when the row appears in only one table.
        /// </returns>
        public string RowComment 
        {
            get
            {
                if (IsFound)
                {
                    return ExactMatch ? "Exact Match" : "Differences Found";
                }
                else
                {
                    return "Only Found in: " + (IsInTable1 ? "Table1" : "Table2");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowComparison"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor ensures the <see cref="Differences"/> list is non-null and ready to receive <see cref="CellDifference"/> items.
        /// Other properties default to their CLR defaults (false for booleans, null for reference types).
        /// </remarks>
        public RowComparison()
        {
            Differences = new List<CellDifference>();
        }
    }
}
