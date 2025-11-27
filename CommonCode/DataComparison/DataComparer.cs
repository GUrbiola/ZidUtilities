using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Xml;
using ZidUtilities.CommonCode;
using ZidUtilities.CommonCode.DataComparison;

namespace ZidUtilities.CommonCode.DataComparison
{
    /// <summary>
    /// Performs a detailed comparison between two <see cref="DataTable"/> instances.
    /// </summary>
    /// <remarks>
    /// The comparer builds internal lookup dictionaries keyed by configured primary key fields to efficiently
    /// identify rows present in one table and not the other, and to compare rows that exist in both tables.
    /// It records per-row and per-cell differences in <see cref="RowComparison"/> and <see cref="CellDifference"/> objects,
    /// and produces a <see cref="FinalTable"/> that contains the original row values augmented with comparison metadata.
    /// Configuration options allow controlling case sensitivity, columns to always ignore, and custom column comparison rules.
    /// </remarks>
    public class DataComparer
    {
        /// <summary>
        /// Gets the total number of rows that exist only in the first input table after the last comparison run.
        /// </summary>
        public int AddCount { get; private set; }

        /// <summary>
        /// Gets or sets a human-friendly label used when emitting "add" entries in generated track files.
        /// </summary>
        public string AddLabel { get; set; }

        /// <summary>
        /// Gets the total number of rows that were detected as updates (present in both but with differences).
        /// </summary>
        public int UpdateCount { get; private set; }

        /// <summary>
        /// Gets or sets a human-friendly label used when emitting "update" entries in generated track files.
        /// </summary>
        public string UpdateLabel { get; set; }

        /// <summary>
        /// Gets the total number of rows that exist only in the second input table after the last comparison run.
        /// </summary>
        public int DeleteCount { get; private set; }

        /// <summary>
        /// Gets or sets a human-friendly label used when emitting "delete" entries in generated track files.
        /// </summary>
        public string DeleteLabel { get; set; }

        /// <summary>
        /// Gets the total number of rows that were found in both tables and had no detectable differences.
        /// </summary>
        public int NoChangeCount { get; private set; }

        /// <summary>
        /// Gets or sets a human-friendly label used when emitting "no change" entries in generated track files.
        /// </summary>
        public string NoChangeLabel { get; set; }

        /// <summary>
        /// Gets or sets an optional name for the entity being compared (used as the XML element name in track files).
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when a comparison run started.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when a comparison run ended.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the elapsed time span between <see cref="StartTime"/> and <see cref="EndTime"/>.
        /// </summary>
        public TimeSpan Lapse { get; set; }

        /// <summary>
        /// Gets or sets a collection of comments produced during comparison operations.
        /// </summary>
        public List<CommentRow> CommentRows { get; set; }

        /// <summary>
        /// Gets a human-friendly string representation of <see cref="Lapse"/>.
        /// </summary>
        /// <remarks>
        /// When <see cref="Lapse"/> is <see cref="TimeSpan.Zero"/> the method returns "00:00:00.00".
        /// Otherwise the TimeSpan is returned using the general ("g") format.
        /// </remarks>
        public string LapseString
        {
            get
            {
                if (Lapse != TimeSpan.Zero)
                {
                    return Lapse.ToString("g");
                }
                return "00:00:00.00";
            }
        }

        /// <summary>
        /// Internal lookup of rows from the first table keyed by composite primary key.
        /// </summary>
        private Dictionary<string, DataRow> Dictionary1;

        /// <summary>
        /// Internal lookup of rows from the second table keyed by composite primary key.
        /// </summary>
        private Dictionary<string, DataRow> Dictionary2;

        /// <summary>
        /// Gets or sets the list of per-row comparison results produced by the last comparison run.
        /// </summary>
        public List<RowComparison> ComparisonResult { get; set; }

        /// <summary>
        /// Gets or sets a label identifying the first input table (used in reports).
        /// </summary>
        public string Label1 { get; set; }

        /// <summary>
        /// Gets or sets a label identifying the second input table (used in reports).
        /// </summary>
        public string Label2 { get; set; }

        /// <summary>
        /// Gets or sets the zero-based column indexes that compose the composite primary key used to identify rows.
        /// </summary>
        /// <remarks>
        /// If empty, the comparer uses column 0 as a default primary key part.
        /// Multiple entries are concatenated with '|' when generating a composite key.
        /// </remarks>
        public List<int> PrimaryKeyFields { get; set; }

        /// <summary>
        /// Gets or sets the generated result <see cref="DataTable"/> produced after a comparison run.
        /// The table mirrors the source schema and adds comparison metadata columns.
        /// </summary>
        public DataTable FinalTable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether string comparisons should be case-sensitive.
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets a dictionary containing column-specific comparison rules.
        /// </summary>
        /// <remarks>
        /// Keys are column names and values are <see cref="ColumnComparisonDetail"/> instances describing
        /// how to compare values for that column (type, tolerance, case sensitivity, etc.).
        /// </remarks>
        public Dictionary<string, ColumnComparisonDetail> ColumnComparisonDetails;

        /// <summary>
        /// Gets or sets a list of column names that should always be ignored during comparisons.
        /// </summary>
        public List<string> AlwaysToIgnoreColumns;

        /// <summary>
        /// Gets or sets a flag indicating whether to use column names instead of column indexes when comparing cells.
        /// </summary>
        public bool UseColumnNamesForComparison { get; set; }

        /// <summary>
        /// Gets or sets the column name which, when matched with <see cref="IgEqualsTo"/>, will cause a conditional ignore of another column.
        /// </summary>
        public string IgWhenField { get; set; }

        /// <summary>
        /// Gets or sets the value used together with <see cref="IgWhenField"/> to determine if a row comparison should ignore a specific column.
        /// </summary>
        public string IgEqualsTo { get; set; }

        /// <summary>
        /// Gets or sets the column name that will be ignored when the conditional ignore triggered by <see cref="IgWhenField"/> is active.
        /// </summary>
        public string IgComparisonForField { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataComparer"/> class with default settings.
        /// </summary>
        /// <remarks>
        /// - Initializes collections used throughout the comparison process.
        /// - Sets default labels for <see cref="Label1"/> and <see cref="Label2"/>.
        /// - Sets <see cref="CaseSensitive"/> to false and <see cref="UseColumnNamesForComparison"/> to false.
        /// </remarks>
        public DataComparer()
        {
            CommentRows = new List<CommentRow>();
            Lapse = TimeSpan.Zero;
            AddCount = 0;
            UpdateCount = 0;
            DeleteCount = 0;
            NoChangeCount = 0;
            PrimaryKeyFields = new List<int>();
            FinalTable = new DataTable();
            CaseSensitive = false;
            Label1 = "Table1";
            Label2 = "Table2";
            ColumnComparisonDetails = new Dictionary<string, ColumnComparisonDetail>();
            AlwaysToIgnoreColumns = new List<string>();
            UseColumnNamesForComparison = false;
        }

        /// <summary>
        /// Compares two data tables row-by-row and cell-by-cell, producing a set of <see cref="RowComparison"/> results
        /// and populating <see cref="FinalTable"/> with comparison metadata.
        /// </summary>
        /// <param name="table1">The DataTable considered the primary/source dataset.</param>
        /// <param name="table2">The DataTable considered the secondary/previous dataset.</param>
        /// <remarks>
        /// This method:
        /// - Builds internal dictionaries for fast lookup by primary key.
        /// - Detects rows only in one table (add/remove).
        /// - Compares rows present in both tables and records cell differences using configured rules.
        /// - Fills <see cref="ComparisonResult"/> and <see cref="FinalTable"/> and sets summary counts and timing.
        /// </remarks>
        public void RunComparison(DataTable table1, DataTable table2)
        {
            Lapse = TimeSpan.Zero;
            StartTime = DateTime.Now;
            ComparisonResult = new List<RowComparison>();
            FinalTable = table1.Clone();
            DataColumn pk = new DataColumn("ComparisonPK") { DataType = typeof (String), AllowDBNull = false };
            FinalTable.Columns.Add(pk);
            FinalTable.PrimaryKey = new DataColumn[]{ pk };
            FinalTable.Columns.Add(new DataColumn("ComparisonComment") {DataType = typeof (String), AllowDBNull = false});
            List<int> ToIgnoreColumns = new List<int>();

            if (PrimaryKeyFields.Count == 0)
                PrimaryKeyFields.Add(0);


            #region Create the dictionaries for the comparison
            Dictionary1 = new Dictionary<string, DataRow>();
            if (table1 != null)
            {
                foreach (DataRow row in table1.Rows)
                {
                    string rowKey = GetKey(row);
                    if (!Dictionary1.ContainsKey(rowKey))
                        Dictionary1.Add(rowKey, row);
                }
            }
            Dictionary2 = new Dictionary<string, DataRow>();
            if (table2 != null)
            {
                foreach (DataRow row in table2.Rows)
                {
                    string rowKey = GetKey(row);
                    if (!Dictionary2.ContainsKey(rowKey))
                        Dictionary2.Add(rowKey, row);
                }
            }
            #endregion

            #region Find records only in table1
            foreach (var d1 in Dictionary1.Where( x => !Dictionary2.ContainsKey(x.Key)))
            {
                DataRow buff = FinalTable.NewRow();
                foreach (DataColumn col in table1.Columns)
                {
                    buff[col.ColumnName] = d1.Value[col.ColumnName];
                }
                buff["ComparisonPK"] = d1.Key;
                buff["ComparisonComment"] = "Only Found in: " + Label1;
                FinalTable.Rows.Add(buff);
                ComparisonResult.Add(new RowComparison(){ RowId = d1.Key, IsInTable1 = true, IsInTable2 = false, Row1 = d1.Value, Row2 = null });
            }
            #endregion

            #region Find records only in table2
            foreach (var d2 in Dictionary2.Where(x => !Dictionary1.ContainsKey(x.Key)))
            {
                DataRow buff = FinalTable.NewRow();
                foreach (DataColumn col in table2.Columns)
                {
                    buff[col.ColumnName] = d2.Value[col.ColumnName];
                }
                buff["ComparisonPK"] = d2.Key;
                buff["ComparisonComment"] = "Only Found in: " + Label2;
                FinalTable.Rows.Add(buff);
                ComparisonResult.Add(new RowComparison() { RowId = d2.Key, IsInTable2 = true, IsInTable1 = false, Row1 = null, Row2 = d2.Value });
            }
            #endregion

            #region Calculate which column numbers needs to be always ignored
            for (int i = 0; i < table1.Columns.Count; i++)
            {
                if (AlwaysToIgnoreColumns.InsensitiveListContains(table1.Columns[i].ColumnName))
                {
                    ToIgnoreColumns.Add(i);
                }
            }
            #endregion

            #region Do comparison cell by cell of records found in both tables
            foreach (var d1 in Dictionary1.Where(x => Dictionary2.ContainsKey(x.Key)))
            {
                DataRow rec1 = d1.Value;
                DataRow rec2 = Dictionary2[d1.Key];
                
                RowComparison rc = new RowComparison() { IsInTable1 = true, IsInTable2 = true, RowId = d1.Key, Row1 = d1.Value, Row2 = Dictionary2[d1.Key] };

                #region Check if the logic says a column should be ignored in the comparison
                bool ignoreComparison = false;
                for (int i = 0; i < table1.Columns.Count; i++)
                {
                    if (table1.Columns[i].ColumnName.Equals(IgWhenField, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (rec1[i].ToString().Equals(IgEqualsTo, StringComparison.CurrentCultureIgnoreCase))
                        {
                            ignoreComparison = true;
                            break;
                        }
                    }
                }
                #endregion

                for (int i = 0; i < table1.Columns.Count; i++)
                {
                    string curColumnName = table1.Columns[i].ColumnName;
                    if (ToIgnoreColumns.Contains(i) || (ignoreComparison && curColumnName.Equals(IgComparisonForField, StringComparison.CurrentCultureIgnoreCase)))
                        continue;//If this columns is set to be explicitly ignored or if by the logic should ignored, just skip

                    if (UseColumnNamesForComparison)
                    {
                        #region Do comparison of cell values by column names
                        if (rec1[curColumnName] == rec2[curColumnName]) 
                            continue;

                        bool emptyField1 = String.IsNullOrEmpty(rec1[curColumnName].ToString()) || rec1[curColumnName].ToString().Trim().Length == 0;
                        bool emptyField2 = String.IsNullOrEmpty(rec2[curColumnName].ToString()) || rec2[curColumnName].ToString().Trim().Length == 0;

                        if (emptyField1 && emptyField2)
                            continue;

                        if (ColumnComparisonDetails.ContainsKey(curColumnName))
                        {
                            if (!ColumnComparisonDetail.IsEqual(rec1[curColumnName], rec2[curColumnName], ColumnComparisonDetails[curColumnName]))
                            {
                                rc.Differences.Add(new CellDifference(curColumnName) { ColumnNumber = i, Value1 = rec1[curColumnName].ToString(), Value2 = rec2[curColumnName].ToString() });
                            }
                        }
                        else
                        {
                            if (CaseSensitive)
                            {
                                if (!rec1[curColumnName].ToString().Equals(rec2[curColumnName].ToString(), StringComparison.CurrentCulture))
                                {
                                    rc.Differences.Add(new CellDifference(curColumnName) { ColumnNumber = i, Value1 = rec1[curColumnName].ToString(), Value2 = rec2[curColumnName].ToString() });
                                }
                            }
                            else
                            {
                                if (!rec1[i].ToString().Equals(rec2[i].ToString(), StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rc.Differences.Add(new CellDifference(curColumnName) { ColumnNumber = i, Value1 = rec1[curColumnName].ToString(), Value2 = rec2[curColumnName].ToString() });
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region Do comparison of values by column indexes
                        if (rec1[i] == rec2[i]) 
                            continue;

                        bool emptyField1 = String.IsNullOrEmpty(rec1[i].ToString()) || rec1[i].ToString().Trim().Length == 0;
                        bool emptyField2 = String.IsNullOrEmpty(rec2[i].ToString()) || rec2[i].ToString().Trim().Length == 0;

                        if (emptyField1 && emptyField2)
                            continue;

                        if (ColumnComparisonDetails.ContainsKey(curColumnName))
                        {
                            if (!ColumnComparisonDetail.IsEqual(rec1[i], rec2[i], ColumnComparisonDetails[curColumnName]))
                            {
                                rc.Differences.Add(new CellDifference(curColumnName) { ColumnNumber = i, Value1 = rec1[i].ToString(), Value2 = rec2[i].ToString() });
                            }
                        }
                        else
                        {
                            if (CaseSensitive)
                            {
                                if (!rec1[i].ToString().Equals(rec2[i].ToString(), StringComparison.CurrentCulture))
                                {
                                    rc.Differences.Add(new CellDifference(curColumnName) { ColumnNumber = i, Value1 = rec1[i].ToString(), Value2 = rec2[i].ToString() });
                                }
                            }
                            else
                            {
                                if (!rec1[i].ToString().Equals(rec2[i].ToString(), StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rc.Differences.Add(new CellDifference(curColumnName) { ColumnNumber = i, Value1 = rec1[i].ToString(), Value2 = rec2[i].ToString() });
                                }
                            }
                        }
                        #endregion
                    }
                }

                #region Save the current record and the result of the comparison
                DataRow buff = FinalTable.NewRow();
                for (int i = 0; i < table1.Columns.Count; i++)
                {
                    buff[i] = d1.Value[i];
                }

                buff["ComparisonPK"] = d1.Key;
                buff["ComparisonComment"] = rc.RowComment;
                FinalTable.Rows.Add(buff);
                ComparisonResult.Add(rc);
                #endregion
            }
            #endregion

            AddCount = ComparisonResult.Count(x => x.OnlyTable1);
            UpdateCount = ComparisonResult.Count(x => x.IsFound && !x.ExactMatch);
            DeleteCount = ComparisonResult.Count(x => x.OnlyTable2);
            NoChangeCount = ComparisonResult.Count(x => x.IsFound && x.ExactMatch);

            EndTime = DateTime.Now;
            Lapse = EndTime.Subtract(StartTime);    
        }

        /// <summary>
        /// Improved and more readable version of RunComparison that delegates to helper methods.
        /// </summary>
        /// <param name="table1">The DataTable with the "current" data.</param>
        /// <param name="table2">The DataTable with the "previous" data.</param>
        /// <remarks>
        /// This method performs the same overall comparison as <see cref="RunComparison"/> but organizes the steps
        /// into private helper methods for readability and maintainability.
        /// </remarks>
        public void RunComparisonByClaude(DataTable table1, DataTable table2)
        {
            // Initialize timing and reset state
            StartTime = DateTime.Now;
            ComparisonResult = new List<RowComparison>();

            // Ensure we have at least one primary key field
            if (PrimaryKeyFields.Count == 0)
                PrimaryKeyFields.Add(0);

            // Step 1: Build lookup dictionaries for efficient row matching
            Dictionary1 = BuildRowDictionary(table1);
            Dictionary2 = BuildRowDictionary(table2);

            // Step 2: Prepare the final result table structure
            InitializeFinalTable(table1);

            // Step 3: Pre-calculate columns to ignore for performance
            HashSet<int> columnsToIgnore = BuildIgnoredColumnsSet(table1);

            // Step 4: Find and process rows only in table1 (additions)
            ProcessRowsOnlyInTable1(table1, columnsToIgnore);

            // Step 5: Find and process rows only in table2 (deletions)
            ProcessRowsOnlyInTable2(table2, columnsToIgnore);

            // Step 6: Compare rows that exist in both tables
            CompareCommonRows(table1, columnsToIgnore);

            // Step 7: Calculate summary statistics
            CalculateSummaryCounts();

            // Finalize timing
            EndTime = DateTime.Now;
            Lapse = EndTime.Subtract(StartTime);
        }

        #region Helper Methods for RunComparisonByClaude

        /// <summary>
        /// Builds a dictionary that maps composite primary key strings to the corresponding <see cref="DataRow"/>.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> to index. May be null.</param>
        /// <returns>A new dictionary. If <paramref name="table"/> is null, an empty dictionary is returned.</returns>
        private Dictionary<string, DataRow> BuildRowDictionary(DataTable table)
        {
            var dictionary = new Dictionary<string, DataRow>();

            if (table == null)
                return dictionary;

            foreach (DataRow row in table.Rows)
            {
                string rowKey = GetKey(row);
                if (!dictionary.ContainsKey(rowKey))
                    dictionary.Add(rowKey, row);
            }

            return dictionary;
        }

        /// <summary>
        /// Initializes the internal <see cref="FinalTable"/> by cloning the provided source schema and
        /// adding comparison metadata columns ("ComparisonPK" and "ComparisonComment").
        /// </summary>
        /// <param name="sourceTable">A DataTable whose schema will be cloned. Must not be null.</param>
        private void InitializeFinalTable(DataTable sourceTable)
        {
            FinalTable = sourceTable.Clone();

            var primaryKeyColumn = new DataColumn("ComparisonPK")
            {
                DataType = typeof(string),
                AllowDBNull = false
            };
            FinalTable.Columns.Add(primaryKeyColumn);
            FinalTable.PrimaryKey = new[] { primaryKeyColumn };

            FinalTable.Columns.Add(new DataColumn("ComparisonComment")
            {
                DataType = typeof(string),
                AllowDBNull = false
            });
        }

        /// <summary>
        /// Generates a set of zero-based column indices that should be skipped during comparisons based on <see cref="AlwaysToIgnoreColumns"/>.
        /// </summary>
        /// <param name="table">The table whose columns will be inspected.</param>
        /// <returns>A <see cref="HashSet{Int32}"/> containing the column indices to ignore.</returns>
        private HashSet<int> BuildIgnoredColumnsSet(DataTable table)
        {
            var columnsToIgnore = new HashSet<int>();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (AlwaysToIgnoreColumns.InsensitiveListContains(table.Columns[i].ColumnName))
                {
                    columnsToIgnore.Add(i);
                }
            }

            return columnsToIgnore;
        }

        /// <summary>
        /// Adds rows that appear only in the first table to <see cref="ComparisonResult"/> and <see cref="FinalTable"/>.
        /// </summary>
        /// <param name="table1">The first DataTable (source of values to include in FinalTable).</param>
        /// <param name="columnsToIgnore">A set of column indices to ignore (retained for signature parity; not used when adding rows).</param>
        private void ProcessRowsOnlyInTable1(DataTable table1, HashSet<int> columnsToIgnore)
        {
            foreach (var kvp in Dictionary1.Where(x => !Dictionary2.ContainsKey(x.Key)))
            {
                // Add to comparison result
                var rowComparison = new RowComparison
                {
                    RowId = kvp.Key,
                    IsInTable1 = true,
                    IsInTable2 = false,
                    Row1 = kvp.Value,
                    Row2 = null
                };
                ComparisonResult.Add(rowComparison);

                // Add to final table
                DataRow resultRow = FinalTable.NewRow();
                foreach (DataColumn col in table1.Columns)
                {
                    resultRow[col.ColumnName] = kvp.Value[col.ColumnName];
                }
                resultRow["ComparisonPK"] = kvp.Key;
                resultRow["ComparisonComment"] = "Only Found in: " + Label1;
                FinalTable.Rows.Add(resultRow);
            }
        }

        /// <summary>
        /// Adds rows that appear only in the second table to <see cref="ComparisonResult"/> and <see cref="FinalTable"/>.
        /// </summary>
        /// <param name="table2">The second DataTable (source of values to include in FinalTable for deletions).</param>
        /// <param name="columnsToIgnore">A set of column indices to ignore (retained for signature parity; not used when adding rows).</param>
        private void ProcessRowsOnlyInTable2(DataTable table2, HashSet<int> columnsToIgnore)
        {
            foreach (var kvp in Dictionary2.Where(x => !Dictionary1.ContainsKey(x.Key)))
            {
                // Add to comparison result
                var rowComparison = new RowComparison
                {
                    RowId = kvp.Key,
                    IsInTable1 = false,
                    IsInTable2 = true,
                    Row1 = null,
                    Row2 = kvp.Value
                };
                ComparisonResult.Add(rowComparison);

                // Add to final table
                DataRow resultRow = FinalTable.NewRow();
                foreach (DataColumn col in table2.Columns)
                {
                    resultRow[col.ColumnName] = kvp.Value[col.ColumnName];
                }
                resultRow["ComparisonPK"] = kvp.Key;
                resultRow["ComparisonComment"] = "Only Found in: " + Label2;
                FinalTable.Rows.Add(resultRow);
            }
        }

        /// <summary>
        /// Compares rows that exist in both tables to find cell-level differences.
        /// </summary>
        /// <param name="table1">The DataTable used as the reference schema and to iterate columns for comparison.</param>
        /// <param name="columnsToIgnore">A HashSet of column indices to skip during the comparison.</param>
        /// <remarks>
        /// For each row present in both dictionaries, the method applies optional conditional ignore logic, compares cells
        /// using either column names or column indices and records CellDifference entries on the RowComparison object.
        /// Results are appended to ComparisonResult and FinalTable.
        /// </remarks>
        private void CompareCommonRows(DataTable table1, HashSet<int> columnsToIgnore)
        {
            foreach (var kvp in Dictionary1.Where(x => Dictionary2.ContainsKey(x.Key)))
            {
                DataRow row1 = kvp.Value;
                DataRow row2 = Dictionary2[kvp.Key];

                var rowComparison = new RowComparison
                {
                    IsInTable1 = true,
                    IsInTable2 = true,
                    RowId = kvp.Key,
                    Row1 = row1,
                    Row2 = row2
                };

                // Check if conditional ignore logic applies
                bool shouldIgnoreComparison = ShouldIgnoreRowComparison(row1, table1);

                // Compare each column
                for (int columnIndex = 0; columnIndex < table1.Columns.Count; columnIndex++)
                {
                    string columnName = table1.Columns[columnIndex].ColumnName;

                    // Skip if column should be ignored
                    if (ShouldSkipColumn(columnIndex, columnName, columnsToIgnore, shouldIgnoreComparison))
                        continue;

                    // Perform cell comparison
                    CompareCells(row1, row2, columnIndex, columnName, rowComparison);
                }

                // Add to results
                ComparisonResult.Add(rowComparison);

                // Add to final table
                DataRow resultRow = FinalTable.NewRow();
                for (int i = 0; i < table1.Columns.Count; i++)
                {
                    resultRow[i] = row1[i];
                }
                resultRow["ComparisonPK"] = kvp.Key;
                resultRow["ComparisonComment"] = rowComparison.RowComment;
                FinalTable.Rows.Add(resultRow);
            }
        }

        /// <summary>
        /// Evaluates whether the provided row should be completely ignored for per-column comparison
        /// based on the configured conditional-ignore fields <see cref="IgWhenField"/> and <see cref="IgEqualsTo"/>.
        /// </summary>
        /// <param name="row">The row to evaluate.</param>
        /// <param name="table">The table schema used to resolve column names to indexes.</param>
        /// <returns>True when the row matches the configured ignore condition; otherwise false.</returns>
        private bool ShouldIgnoreRowComparison(DataRow row, DataTable table)
        {
            if (string.IsNullOrEmpty(IgWhenField))
                return false;

            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName.Equals(IgWhenField, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (row[i].ToString().Equals(IgEqualsTo, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether a column should be skipped during comparison, either because it is in the global ignore list
        /// or because the conditional-ignore logic applies to this specific column.
        /// </summary>
        /// <param name="columnIndex">Zero-based index of the column being inspected.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnsToIgnore">Set of globally-ignored column indices.</param>
        /// <param name="shouldIgnoreComparison">Flag indicating whether conditional ignore is active for the current row.</param>
        /// <returns>True if the column should be skipped; otherwise false.</returns>
        private bool ShouldSkipColumn(int columnIndex, string columnName, HashSet<int> columnsToIgnore, bool shouldIgnoreComparison)
        {
            // Skip if explicitly in ignore list
            if (columnsToIgnore.Contains(columnIndex))
                return true;

            // Skip if conditional ignore logic applies to this column
            if (shouldIgnoreComparison &&
                !string.IsNullOrEmpty(IgComparisonForField) &&
                columnName.Equals(IgComparisonForField, StringComparison.CurrentCultureIgnoreCase))
                return true;

            return false;
        }

        /// <summary>
        /// Compares two individual cell values according to configured column-specific rules or default string comparison rules,
        /// and appends a <see cref="CellDifference"/> entry to <paramref name="rowComparison"/> if a difference is detected.
        /// </summary>
        /// <param name="row1">DataRow from the first table.</param>
        /// <param name="row2">DataRow from the second table.</param>
        /// <param name="columnIndex">The zero-based column index to compare.</param>
        /// <param name="columnName">The column name (used when <see cref="UseColumnNamesForComparison"/> is true).</param>
        /// <param name="rowComparison">The <see cref="RowComparison"/> object to receive any detected <see cref="CellDifference"/>.</param>
        private void CompareCells(DataRow row1, DataRow row2, int columnIndex, string columnName, RowComparison rowComparison)
        {
            // Get cell values based on comparison mode
            object value1, value2;
            if (UseColumnNamesForComparison)
            {
                value1 = row1[columnName];
                value2 = row2[columnName];
            }
            else
            {
                value1 = row1[columnIndex];
                value2 = row2[columnIndex];
            }

            // Quick reference equality check
            if (value1 == value2)
                return;

            // Convert to strings for comparison
            string strValue1 = value1?.ToString() ?? string.Empty;
            string strValue2 = value2?.ToString() ?? string.Empty;

            // Check if both values are effectively empty
            bool isEmpty1 = string.IsNullOrWhiteSpace(strValue1);
            bool isEmpty2 = string.IsNullOrWhiteSpace(strValue2);

            if (isEmpty1 && isEmpty2)
                return;

            // Perform comparison based on column-specific rules or default rules
            bool areEqual;
            if (ColumnComparisonDetails.ContainsKey(columnName))
            {
                // Use custom comparison logic for this column
                areEqual = ColumnComparisonDetail.IsEqual(value1, value2, ColumnComparisonDetails[columnName]);
            }
            else
            {
                // Use default string comparison (case-sensitive or insensitive)
                StringComparison comparisonType = CaseSensitive
                    ? StringComparison.CurrentCulture
                    : StringComparison.CurrentCultureIgnoreCase;

                areEqual = strValue1.Equals(strValue2, comparisonType);
            }

            // Record difference if values are not equal
            if (!areEqual)
            {
                rowComparison.Differences.Add(new CellDifference(columnName)
                {
                    ColumnNumber = columnIndex,
                    Value1 = strValue1,
                    Value2 = strValue2
                });
            }
        }

        /// <summary>
        /// Computes the summary counters (AddCount, UpdateCount, DeleteCount, NoChangeCount) from <see cref="ComparisonResult"/>.
        /// </summary>
        private void CalculateSummaryCounts()
        {
            AddCount = ComparisonResult.Count(x => x.OnlyTable1);
            UpdateCount = ComparisonResult.Count(x => x.IsFound && !x.ExactMatch);
            DeleteCount = ComparisonResult.Count(x => x.OnlyTable2);
            NoChangeCount = ComparisonResult.Count(x => x.IsFound && x.ExactMatch);
        }

        #endregion

        /// <summary>
        /// Builds a composite string key for the provided <see cref="DataRow"/> using configured <see cref="PrimaryKeyFields"/>.
        /// </summary>
        /// <param name="row">The row from which key parts will be extracted.</param>
        /// <returns>A composite string key made by concatenating the configured primary key field values with '|'.</returns>
        private string GetKey(DataRow row)
        {
            string key = "";
            foreach (int keyIndex in PrimaryKeyFields)
            {
                if(String.IsNullOrEmpty(key))
                    key += row[keyIndex].ToString();
                else
                    key += "|" + row[keyIndex].ToString();
            }
            return key;
        }

        //public void GenerateTrackFile(string fileName, TrackFileType fileType, bool onlyChanges = true, bool includeSummary = false)
        //{
        //    switch (fileType)
        //    {
        //        case TrackFileType.Xlsx:
        //            GenerateXlsxTrackFile(fileName, onlyChanges, includeSummary);
        //            break;
        //        case TrackFileType.Html:
        //            break;
        //        default:
        //        case TrackFileType.Xml:
        //            GenerateXmlTrackFile(fileName, onlyChanges, includeSummary);
        //            break;
        //    }
        //}

        /// <summary>
        /// Generates an XML track file summarizing the comparison results and optionally including full record details.
        /// </summary>
        /// <param name="fileName">Destination path for the generated XML file.</param>
        /// <param name="rootName">Root element name for the XML (spaces will be replaced with underscores).</param>
        /// <param name="includeSummary">If true a <c>Summary</c> element describing counts and timing is emitted.</param>
        /// <param name="onlyRecordsWithChanges">When true, only records having differences are included in the non-add/del sections.</param>
        /// <param name="onlyFieldsWithChanges">When true, update sections include only fields that actually changed (old value is included as attribute).</param>
        /// <param name="includeAdds">When true, additions are included in the XML output.</param>
        /// <param name="includeDels">When true, deletions are included in the XML output.</param>
        /// <param name="includeUpdates">When true, updates are included in the XML output.</param>
        public void GenerateXmlTrackFile(string fileName, string rootName, bool includeSummary, bool onlyRecordsWithChanges, bool onlyFieldsWithChanges = false, bool includeAdds = true, bool includeDels = true, bool includeUpdates = true)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            XmlWriter scriba;

            if (ComparisonResult == null || ComparisonResult.Count == 0)
                return;

            scriba =  XmlWriter.Create(fileName, xmlSettings);
            scriba.WriteStartDocument();
            scriba.WriteStartElement(rootName.Replace(" ", "_"));//Lev 1

            if (includeSummary)
            {
                scriba.OpenNode(false, "Summary");//Lev 2

                    scriba.OpenNode(true, "Date", DateTime.Now.ToString("MM/dd/yyyy HH:mm tt"));//Lev 3
                    scriba.OpenNode(true, "Table1", Label1);//Lev 3
                    scriba.OpenNode(true, "Table2", Label2);//Lev 3

                    if(includeAdds)
                        scriba.OpenNode(true, String.Format("{0}_Count", String.IsNullOrEmpty(AddLabel) ? "Add" : AddLabel), AddCount);//Lev 3
                    if (includeUpdates)
                        scriba.OpenNode(true, String.Format("{0}_Count", String.IsNullOrEmpty(UpdateLabel) ? "Update" : UpdateLabel), UpdateCount);//Lev 3
                    if (includeDels)
                        scriba.OpenNode(true, String.Format("{0}_Count", String.IsNullOrEmpty(DeleteLabel) ? "Delete" : DeleteLabel), DeleteCount);//Lev 3

                    scriba.OpenNode(true, "NoChangeCount", NoChangeCount);//Lev 3
                    scriba.OpenNode(true, "Lapse", LapseString);//Lev 3

                scriba.CloseNode();//Lev 2
            }

            if (includeAdds)
            {
                scriba.OpenNode(false, String.IsNullOrEmpty(AddLabel) ? "Additions" : AddLabel);//Lev 2

                foreach (RowComparison rc in ComparisonResult.Where( x => x.OnlyTable1 ))
                {
                    WriteRecordWrapper(scriba, rc);//Lev 3

                    foreach (DataColumn col in rc.Row1.Table.Columns)
                        scriba.OpenNode(true, col.ColumnName, rc.Row1[col.ColumnName].ToString());//Lev 4

                    scriba.CloseNode();//Lev 3
                }

                scriba.CloseNode();//Lev 2
            }


            if (includeUpdates)
            {
                scriba.OpenNode(false, String.IsNullOrEmpty(UpdateLabel) ? "Updates" : UpdateLabel);//Lev 2

                foreach (RowComparison rc in ComparisonResult.Where(x => x.IsFound && !x.ExactMatch))
                {
                    WriteRecordWrapper(scriba, rc);//Lev 3

                    foreach (DataColumn col in rc.Row1.Table.Columns)
                    {
                        if (onlyFieldsWithChanges)
                        {
                            if (rc.Differences.Exists(x => x.ColumnName.Equals(col.ColumnName, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                scriba.OpenNode(true, col.ColumnName, rc.Row1[col.ColumnName].ToString(),
                                    "OldValue", rc.Row2[col.ColumnName].ToString()); //Lev 4                        
                            }
                        }
                        else
                        {
                            scriba.OpenNode(true, col.ColumnName, rc.Row1[col.ColumnName].ToString());//Lev 4                        
                        }
                    }

                    scriba.CloseNode();//Lev 3
                }

                scriba.CloseNode();//Lev 2
            }

            if (includeDels)
            {
                scriba.OpenNode(false, String.IsNullOrEmpty(DeleteLabel) ? "Deletions" : DeleteLabel);//Lev 2

                foreach (RowComparison rc in ComparisonResult.Where(x => x.OnlyTable2))
                {
                    WriteRecordWrapper(scriba, rc, true);//Lev 3

                    foreach (DataColumn col in rc.Row2.Table.Columns)
                        scriba.OpenNode(true, col.ColumnName, rc.Row2[col.ColumnName].ToString());//Lev 4

                    scriba.CloseNode();//Lev 3
                }

                scriba.CloseNode();//Lev 2
            }

            if (!onlyRecordsWithChanges)
            {
                scriba.OpenNode(false, String.IsNullOrEmpty(NoChangeLabel) ? "NoChanges" : NoChangeLabel);//Lev 2

                foreach (RowComparison rc in ComparisonResult.Where(x => x.ExactMatch))
                {
                    WriteRecordWrapper(scriba, rc);//Lev 3

                    foreach (DataColumn col in rc.Row1.Table.Columns)
                        scriba.OpenNode(true, col.ColumnName, rc.Row1[col.ColumnName].ToString());//Lev 4

                    scriba.CloseNode();//Lev 3
                }

                scriba.CloseNode();//Lev 2
            }

            scriba.WriteEndDocument();
            scriba.Close();
        }
        /// <summary>
        /// Writes an opening XML element for a record and injects primary key attributes based on configured <see cref="PrimaryKeyFields"/>.
        /// </summary>
        /// <param name="scriba">The <see cref="XmlWriter"/> used to write XML content.</param>
        /// <param name="rc">The <see cref="RowComparison"/> describing the record.</param>
        /// <param name="isDel">If true, the method uses <see cref="RowComparison.Row2"/> as the source; otherwise uses <see cref="RowComparison.Row1"/>.</param>
        /// <remarks>
        /// Supports up to four primary key fields; additional fields are ignored. If <see cref="EntityName"/> is specified it is used
        /// as the element name; otherwise a default element name of "Record" is used.
        /// </remarks>
        private void WriteRecordWrapper(XmlWriter scriba, RowComparison rc, bool isDel = false)
        {
            DataRow ptr = isDel ? rc.Row2 : rc.Row1;

            switch (PrimaryKeyFields.Count)
            {
                case 0:
                    scriba.OpenNode(false, String.IsNullOrEmpty(EntityName) ? "Record" : EntityName, null);
                    break;
                case 1:
                    scriba.OpenNode(false, String.IsNullOrEmpty(EntityName) ? "Record" : EntityName, null,
                        ptr.Table.Columns[PrimaryKeyFields[0]].ColumnName, ptr[PrimaryKeyFields[0]].ToString());
                    break;
                case 2:
                    scriba.OpenNode(false, String.IsNullOrEmpty(EntityName) ? "Record" : EntityName, null,
                        ptr.Table.Columns[PrimaryKeyFields[0]].ColumnName, ptr[PrimaryKeyFields[0]].ToString(),
                        ptr.Table.Columns[PrimaryKeyFields[1]].ColumnName, ptr[PrimaryKeyFields[1]].ToString()
                    );
                    break;
                case 3:
                    scriba.OpenNode(false, String.IsNullOrEmpty(EntityName) ? "Record" : EntityName, null,
                        ptr.Table.Columns[PrimaryKeyFields[0]].ColumnName, ptr[PrimaryKeyFields[0]].ToString(),
                        ptr.Table.Columns[PrimaryKeyFields[1]].ColumnName, ptr[PrimaryKeyFields[1]].ToString(),
                        ptr.Table.Columns[PrimaryKeyFields[2]].ColumnName, ptr[PrimaryKeyFields[2]].ToString()
                    );
                    break;
                case 4:
                default:
                    scriba.OpenNode(false, String.IsNullOrEmpty(EntityName) ? "Record" : EntityName, null,
                        ptr.Table.Columns[PrimaryKeyFields[0]].ColumnName, ptr[PrimaryKeyFields[0]].ToString(),
                        ptr.Table.Columns[PrimaryKeyFields[1]].ColumnName, ptr[PrimaryKeyFields[1]].ToString(),
                        ptr.Table.Columns[PrimaryKeyFields[2]].ColumnName, ptr[PrimaryKeyFields[2]].ToString(),
                        ptr.Table.Columns[PrimaryKeyFields[3]].ColumnName, ptr[PrimaryKeyFields[3]].ToString()
                    );
                    break;
            }
        }

        /// <summary>
        /// Generates an HTML representation of the comparison results using a template and writes it to the specified file.
        /// </summary>
        /// <param name="fileName">File path where the HTML output will be written.</param>
        /// <param name="onlyChanges">When true only rows with differences are included in the output table.</param>
        /// <param name="includeDifferenceColumnList">When true each row includes a comma-separated list of changed columns.</param>
        /// <remarks>
        /// The method looks for an HTML template file in the assembly location and the application base directory.
        /// If the template cannot be found an exception is thrown.
        /// </remarks>
        public void GenerateHtmlTrackFile(string fileName, bool onlyChanges, bool includeDifferenceColumnList)
        {
            if (ComparisonResult == null || ComparisonResult.Count == 0)
                return;

            RowComparison firstRow = ComparisonResult[0];
            DataRow firstDataRow = firstRow.IsInTable1 ? firstRow.Row1 : firstRow.Row2;
            int columncount;
            if (firstRow.IsInTable1)
                columncount = firstRow.Row1.Table.Columns.Count;
            else
                columncount = firstRow.Row2.Table.Columns.Count;

            // Read the HTML template, from either the executing assembly location or the base directory
            string templatePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DataComparison", "ComparisonReportTemplate.html");
            if (!System.IO.File.Exists(templatePath))
            {
                templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ComparisonReportTemplate.html");
            }

            string htmlTemplate = "";
            if(File.Exists(templatePath))
            {
                htmlTemplate = System.IO.File.ReadAllText(templatePath);
            }
            else
            {
                throw new Exception("HTML template file for comparison report not found.");
            }

            // Build dynamic content parts
            StringBuilder headerColumns = new StringBuilder();
            StringBuilder filterRow = new StringBuilder();
            StringBuilder tableRows = new StringBuilder();

            // Collect unique change types for dropdown
            List<string> changeTypes = new List<string>();
            foreach (RowComparison rowc in ComparisonResult)
            {
                string changeType;
                if (rowc.ExactMatch)
                    changeType = "No Change";
                else if (rowc.OnlyTable1)
                    changeType = $"Only on {Label1}";
                else if (rowc.OnlyTable2)
                    changeType = $"Only on {Label2}";
                else
                    changeType = "Differences Found";

                if (!changeTypes.Contains(changeType))
                    changeTypes.Add(changeType);
            }

            // Build header columns
            headerColumns.AppendLine("                            <th>Change Type</th>");
            if (includeDifferenceColumnList)
            {
                headerColumns.AppendLine("                            <th>Changed Columns</th>");
            }
            foreach (DataColumn col in firstDataRow.Table.Columns)
            {
                headerColumns.AppendLine($"                            <th>{System.Security.SecurityElement.Escape(col.ColumnName)}</th>");
            }

            // Build filter row
            filterRow.Append("                            <th><select class=\"filter-select\" data-column=\"0\">");
            filterRow.Append("<option value=\"\">All</option>");
            foreach (string ct in changeTypes)
            {
                filterRow.Append($"<option value=\"{System.Security.SecurityElement.Escape(ct)}\">{System.Security.SecurityElement.Escape(ct)}</option>");
            }
            filterRow.AppendLine("</select></th>");

            int filterColumnIndex = 1;
            if (includeDifferenceColumnList)
            {
                filterRow.AppendLine($"                            <th><input type=\"text\" class=\"filter-input\" data-column=\"{filterColumnIndex}\" placeholder=\"Filter...\"></th>");
                filterColumnIndex++;
            }

            for (int i = 0; i < columncount; i++)
            {
                filterRow.AppendLine($"                            <th><input type=\"text\" class=\"filter-input\" data-column=\"{filterColumnIndex}\" placeholder=\"Filter...\"></th>");
                filterColumnIndex++;
            }

            // Build table rows
            foreach (RowComparison rowc in ComparisonResult)
            {
                // Determine change type
                string changeType;
                string changeClass;

                if (rowc.ExactMatch)
                {
                    changeType = "No Change";
                    changeClass = "no-change";

                    if (onlyChanges)
                        continue;
                }
                else if (rowc.OnlyTable1)
                {
                    changeType = $"Only on {Label1}";
                    changeClass = "only-source";
                }
                else if (rowc.OnlyTable2)
                {
                    changeType = $"Only on {Label2}";
                    changeClass = "only-destination";
                }
                else
                {
                    changeType = "Differences Found";
                    changeClass = "differences";
                }

                tableRows.AppendLine("                        <tr>");
                tableRows.AppendLine($"                            <td><span class=\"change-type {changeClass}\">{changeType}</span></td>");

                // Add changed columns list if requested
                if (includeDifferenceColumnList)
                {
                    string changedColumns = string.Join(", ", rowc.Differences.Select(x => x.ColumnName));
                    tableRows.AppendLine($"                            <td><span class=\"changed-columns\">{System.Security.SecurityElement.Escape(changedColumns)}</span></td>");
                }

                // Add data columns
                for (int i = 0; i < columncount; i++)
                {
                    string cellValue = rowc.IsInTable1 ? rowc.Row1[i].ToString() : (rowc.IsInTable2 ? rowc.Row2[i].ToString() : "");
                    string escapedValue = System.Security.SecurityElement.Escape(cellValue);

                    // Check if this cell has a difference
                    CellDifference cellDiff = rowc.Differences.FirstOrDefault(x => x.ColumnNumber == i);

                    if (cellDiff != null)
                    {
                        // Cell has a difference - highlight it and add data attribute for tooltip
                        string tooltip = $"Value on {System.Security.SecurityElement.Escape(Label1)} was '{System.Security.SecurityElement.Escape(cellDiff.Value1)}' and on {System.Security.SecurityElement.Escape(Label2)} was '{System.Security.SecurityElement.Escape(cellDiff.Value2)}'";
                        tableRows.AppendLine($"                            <td class=\"cell-difference\" data-tooltip=\"{tooltip}\">{escapedValue}</td>");
                    }
                    else
                    {
                        // Normal cell
                        tableRows.AppendLine($"                            <td>{escapedValue}</td>");
                    }
                }

                tableRows.AppendLine("                        </tr>");
            }

            // Replace placeholders in template
            string finalHtml = htmlTemplate
                .Replace("{GENERATION_DATE}", DateTime.Now.ToString("MMMM dd, yyyy 'at' hh:mm tt"))
                .Replace("{LABEL1}", System.Security.SecurityElement.Escape(Label1))
                .Replace("{LABEL2}", System.Security.SecurityElement.Escape(Label2))
                .Replace("{INITIAL_ROW_COUNT}", (onlyChanges ? ComparisonResult.Count(x => !x.ExactMatch) : ComparisonResult.Count).ToString())
                .Replace("{HEADER_COLUMNS}", headerColumns.ToString())
                .Replace("{FILTER_ROW}", filterRow.ToString())
                .Replace("{TABLE_ROWS}", tableRows.ToString());

            // Write to file
            System.IO.File.WriteAllText(fileName, finalHtml);
        }
    }
}
