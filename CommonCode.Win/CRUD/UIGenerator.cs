using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using ZidUtilities.CommonCode.Win.Controls;
using ZidUtilities.CommonCode.Win.Forms;

namespace ZidUtilities.CommonCode.Win.CRUD
{
    /// <summary>
    /// Facilitates Insert, Update, and Delete operations by generating dynamic forms
    /// with appropriate controls based on table structure.
    /// </summary>
    public class UIGenerator
    {
        #region Private Fields

        private readonly Dictionary<string, FieldMetadata> _fields;
        private readonly string _tableName;
        private ZidThemes _theme = ZidThemes.Default;
        private LayoutMode _layoutMode = LayoutMode.Auto;
        private int _columnCount = 2;
        private string _htmlLayout = null;
        private List<List<LayoutCell>> _parsedLayout = null;
        private int _formWidth = 500;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes UIGenerator with a SQL connection and table name.
        /// </summary>
        /// <param name="connection">SQL Server connection</param>
        /// <param name="tableName">Name of the table</param>
        public UIGenerator(SqlConnection connection, string tableName)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));

            _tableName = tableName;
            _fields = new Dictionary<string, FieldMetadata>();
            LoadSchemaFromSql(connection, tableName);
        }

        /// <summary>
        /// Initializes UIGenerator with a DataTable.
        /// </summary>
        /// <param name="dataTable">DataTable containing schema information</param>
        /// <param name="tableName">Name of the table (optional, uses DataTable.TableName if not provided)</param>
        public UIGenerator(DataTable dataTable, string tableName = null)
        {
            if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));

            _tableName = string.IsNullOrWhiteSpace(tableName) ? dataTable.TableName : tableName;
            _fields = new Dictionary<string, FieldMetadata>();
            LoadSchemaFromDataTable(dataTable);
        }

        /// <summary>
        /// Initializes UIGenerator with a dictionary of field names and their types.
        /// </summary>
        /// <param name="fields">Dictionary mapping field names to their default values (types inferred)</param>
        /// <param name="tableName">Name of the table</param>
        public UIGenerator(Dictionary<string, object> fields, string tableName)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));

            _tableName = tableName;
            _fields = new Dictionary<string, FieldMetadata>();
            LoadSchemaFromDictionary(fields);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the theme for generated dialogs.
        /// </summary>
        public ZidThemes Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        /// <summary>
        /// Gets or sets the width for generated dialogs. Default is 500.
        /// </summary>
        public int FormWidth
        {
            get { return _formWidth; }
            set
            {
                if (value < 100)
                    throw new ArgumentException("FormWidth must be at least 100 pixels.", nameof(value));
                _formWidth = value;
            }
        }

        #endregion

        #region Configuration Methods

        /// <summary>
        /// Sets an alias (display name) for a field.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="alias">Display alias</param>
        public void SetAlias(string fieldName, string alias)
        {
            if (!_fields.ContainsKey(fieldName))
                throw new ArgumentException($"Field '{fieldName}' not found.", nameof(fieldName));

            _fields[fieldName].Alias = alias;
        }

        /// <summary>
        /// Sets a mask configuration for a field.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="maskMethod">Function to mask the value for display</param>
        /// <param name="unmaskMethod">Function to unmask the value for storage</param>
        public void SetMask(string fieldName, Func<string, string> maskMethod, Func<string, string> unmaskMethod)
        {
            if (!_fields.ContainsKey(fieldName))
                throw new ArgumentException($"Field '{fieldName}' not found.", nameof(fieldName));

            _fields[fieldName].MaskInfo = new MaskInfo
            {
                MaskType = MaskType.Custom,
                MaskMethod = maskMethod,
                UnmaskMethod = unmaskMethod
            };
        }

        /// <summary>
        /// Sets a field as a password field (uses PasswordChar and requires confirmation).
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        public void SetPasswordField(string fieldName)
        {
            if (!_fields.ContainsKey(fieldName))
                throw new ArgumentException($"Field '{fieldName}' not found.", nameof(fieldName));

            _fields[fieldName].MaskInfo = new MaskInfo
            {
                MaskType = MaskType.Password
            };
        }

        /// <summary>
        /// Excludes a field from insert/update operations.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="defaultValue">Default value to use (null means exclude from SQL entirely)</param>
        public void SetExclusion(string fieldName, object defaultValue = null)
        {
            if (!_fields.ContainsKey(fieldName))
                throw new ArgumentException($"Field '{fieldName}' not found.", nameof(fieldName));

            _fields[fieldName].IsExcluded = true;
            _fields[fieldName].ExclusionDefaultValue = defaultValue;
        }

        /// <summary>
        /// Marks a field as a foreign key with lookup values.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="lookupValues">Dictionary mapping display values to key values</param>
        public void SetForeignKey(string fieldName, Dictionary<string, object> lookupValues)
        {
            if (!_fields.ContainsKey(fieldName))
                throw new ArgumentException($"Field '{fieldName}' not found.", nameof(fieldName));

            _fields[fieldName].ForeignKeyInfo = new ForeignKeyInfo
            {
                LookupValues = lookupValues ?? new Dictionary<string, object>()
            };
        }

        /// <summary>
        /// Marks a field as part of the primary key.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        public void SetPrimaryKey(string fieldName)
        {
            if (!_fields.ContainsKey(fieldName))
                throw new ArgumentException($"Field '{fieldName}' not found.", nameof(fieldName));

            _fields[fieldName].IsPrimaryKey = true;
        }

        /// <summary>
        /// Sets the format for a field.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="format">Field format type</param>
        /// <param name="formatConfig">Optional format configuration</param>
        public void SetFieldFormat(string fieldName, FieldFormat format, FormatConfig formatConfig = null)
        {
            if (!_fields.ContainsKey(fieldName))
                throw new ArgumentException($"Field '{fieldName}' not found.", nameof(fieldName));

            _fields[fieldName].Format = format;
            _fields[fieldName].FormatConfig = formatConfig;
        }

        /// <summary>
        /// Marks a field as required.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        public void SetRequired(string fieldName)
        {
            if (!_fields.ContainsKey(fieldName))
                throw new ArgumentException($"Field '{fieldName}' not found.", nameof(fieldName));

            _fields[fieldName].IsRequired = true;
        }

        /// <summary>
        /// Sets a custom validation method for a field.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="validationMethod">Validation method that returns null if valid, or an error message if invalid</param>
        public void SetValidation(string fieldName, Func<object, string> validationMethod)
        {
            if (!_fields.ContainsKey(fieldName))
                throw new ArgumentException($"Field '{fieldName}' not found.", nameof(fieldName));

            _fields[fieldName].ValidationMethod = validationMethod;
        }

        #endregion

        #region Getter Methods

        /// <summary>
        /// Gets all field names.
        /// </summary>
        public List<string> GetFieldNames()
        {
            return _fields.Keys.ToList();
        }

        /// <summary>
        /// Gets all primary key field names.
        /// </summary>
        public List<string> GetPrimaryKeyFields()
        {
            return _fields.Where(f => f.Value.IsPrimaryKey).Select(f => f.Key).ToList();
        }

        /// <summary>
        /// Gets all foreign key field names.
        /// </summary>
        public List<string> GetForeignKeyFields()
        {
            return _fields.Where(f => f.Value.ForeignKeyInfo != null).Select(f => f.Key).ToList();
        }

        /// <summary>
        /// Gets the alias for a field.
        /// </summary>
        public string GetAlias(string fieldName)
        {
            return _fields.ContainsKey(fieldName) ? _fields[fieldName].Alias : null;
        }

        /// <summary>
        /// Gets all field aliases.
        /// </summary>
        public Dictionary<string, string> GetAllAliases()
        {
            return _fields.Where(f => !string.IsNullOrEmpty(f.Value.Alias))
                         .ToDictionary(f => f.Key, f => f.Value.Alias);
        }

        /// <summary>
        /// Gets foreign key information for a field.
        /// </summary>
        public ForeignKeyInfo GetForeignKeyInfo(string fieldName)
        {
            return _fields.ContainsKey(fieldName) ? _fields[fieldName].ForeignKeyInfo : null;
        }

        /// <summary>
        /// Gets all foreign key information.
        /// </summary>
        public Dictionary<string, ForeignKeyInfo> GetAllForeignKeys()
        {
            return _fields.Where(f => f.Value.ForeignKeyInfo != null)
                         .ToDictionary(f => f.Key, f => f.Value.ForeignKeyInfo);
        }

        /// <summary>
        /// Gets excluded field names.
        /// </summary>
        public List<string> GetExcludedFields()
        {
            return _fields.Where(f => f.Value.IsExcluded).Select(f => f.Key).ToList();
        }

        /// <summary>
        /// Checks if a field is a primary key.
        /// </summary>
        public bool IsPrimaryKey(string fieldName)
        {
            return _fields.ContainsKey(fieldName) && _fields[fieldName].IsPrimaryKey;
        }

        /// <summary>
        /// Checks if a field is a foreign key.
        /// </summary>
        public bool IsForeignKey(string fieldName)
        {
            return _fields.ContainsKey(fieldName) && _fields[fieldName].ForeignKeyInfo != null;
        }

        /// <summary>
        /// Checks if a field is excluded.
        /// </summary>
        public bool IsExcluded(string fieldName)
        {
            return _fields.ContainsKey(fieldName) && _fields[fieldName].IsExcluded;
        }

        #endregion

        #region Clear Methods

        /// <summary>
        /// Clears the alias for a field.
        /// </summary>
        public void ClearAlias(string fieldName)
        {
            if (_fields.ContainsKey(fieldName))
                _fields[fieldName].Alias = null;
        }

        /// <summary>
        /// Clears all aliases.
        /// </summary>
        public void ClearAllAliases()
        {
            foreach (var field in _fields.Values)
                field.Alias = null;
        }

        /// <summary>
        /// Clears the mask for a field.
        /// </summary>
        public void ClearMask(string fieldName)
        {
            if (_fields.ContainsKey(fieldName))
                _fields[fieldName].MaskInfo = null;
        }

        /// <summary>
        /// Clears all masks.
        /// </summary>
        public void ClearAllMasks()
        {
            foreach (var field in _fields.Values)
                field.MaskInfo = null;
        }

        /// <summary>
        /// Clears the foreign key configuration for a field.
        /// </summary>
        public void ClearForeignKey(string fieldName)
        {
            if (_fields.ContainsKey(fieldName))
                _fields[fieldName].ForeignKeyInfo = null;
        }

        /// <summary>
        /// Clears all foreign key configurations (keeps auto-detected foreign keys).
        /// </summary>
        public void ClearAllForeignKeys()
        {
            foreach (var field in _fields.Values)
                field.ForeignKeyInfo = null;
        }

        /// <summary>
        /// Clears the primary key flag for a field.
        /// </summary>
        public void ClearPrimaryKey(string fieldName)
        {
            if (_fields.ContainsKey(fieldName))
                _fields[fieldName].IsPrimaryKey = false;
        }

        /// <summary>
        /// Clears all primary key flags.
        /// </summary>
        public void ClearAllPrimaryKeys()
        {
            foreach (var field in _fields.Values)
                field.IsPrimaryKey = false;
        }

        /// <summary>
        /// Clears the exclusion for a field.
        /// </summary>
        public void ClearExclusion(string fieldName)
        {
            if (_fields.ContainsKey(fieldName))
            {
                _fields[fieldName].IsExcluded = false;
                _fields[fieldName].ExclusionDefaultValue = null;
            }
        }

        /// <summary>
        /// Clears all exclusions.
        /// </summary>
        public void ClearAllExclusions()
        {
            foreach (var field in _fields.Values)
            {
                field.IsExcluded = false;
                field.ExclusionDefaultValue = null;
            }
        }

        /// <summary>
        /// Clears the field format for a field.
        /// </summary>
        public void ClearFieldFormat(string fieldName)
        {
            if (_fields.ContainsKey(fieldName))
            {
                _fields[fieldName].Format = FieldFormat.Default;
                _fields[fieldName].FormatConfig = null;
            }
        }

        /// <summary>
        /// Clears all field formats.
        /// </summary>
        public void ClearAllFieldFormats()
        {
            foreach (var field in _fields.Values)
            {
                field.Format = FieldFormat.Default;
                field.FormatConfig = null;
            }
        }

        /// <summary>
        /// Clears the required flag for a field.
        /// </summary>
        public void ClearRequired(string fieldName)
        {
            if (_fields.ContainsKey(fieldName))
                _fields[fieldName].IsRequired = false;
        }

        /// <summary>
        /// Clears all required flags.
        /// </summary>
        public void ClearAllRequired()
        {
            foreach (var field in _fields.Values)
                field.IsRequired = false;
        }

        /// <summary>
        /// Clears the validation method for a field.
        /// </summary>
        public void ClearValidation(string fieldName)
        {
            if (_fields.ContainsKey(fieldName))
                _fields[fieldName].ValidationMethod = null;
        }

        /// <summary>
        /// Clears all validation methods.
        /// </summary>
        public void ClearAllValidations()
        {
            foreach (var field in _fields.Values)
                field.ValidationMethod = null;
        }

        #endregion

        #region Layout Configuration Methods

        /// <summary>
        /// Sets the layout mode for form generation.
        /// </summary>
        public void SetLayoutByColumnCount(int columnCount)
        {
            if (columnCount < 1 || columnCount > 4)
                throw new ArgumentException("Column count must be between 1 and 4.", nameof(columnCount));

            _layoutMode = LayoutMode.ColumnCount;
            _columnCount = columnCount;
            _htmlLayout = null;
            _parsedLayout = null;
        }

        /// <summary>
        /// Sets the layout using an HTML table specification.
        /// </summary>
        /// <param name="htmlTable">HTML table defining the layout</param>
        public void SetLayoutByHtmlTable(string htmlTable)
        {
            if (string.IsNullOrWhiteSpace(htmlTable))
                throw new ArgumentNullException(nameof(htmlTable));

            _layoutMode = LayoutMode.HtmlTable;
            _htmlLayout = htmlTable;
            _parsedLayout = ParseHtmlTableLayout(htmlTable);
            _columnCount = _parsedLayout.Max(row => row.Sum(cell => cell.ColSpan));
        }

        /// <summary>
        /// Clears layout configuration and returns to auto layout.
        /// </summary>
        public void ClearLayout()
        {
            _layoutMode = LayoutMode.Auto;
            _columnCount = 2;
            _htmlLayout = null;
            _parsedLayout = null;
        }

        private List<List<LayoutCell>> ParseHtmlTableLayout(string htmlTable)
        {
            var layout = new List<List<LayoutCell>>();

            try
            {
                // Parse HTML table
                var doc = XDocument.Parse(htmlTable);
                var rows = doc.Descendants().Where(e => e.Name.LocalName.ToLower() == "tr");

                foreach (var row in rows)
                {
                    var layoutRow = new List<LayoutCell>();
                    var cells = row.Elements().Where(e => e.Name.LocalName.ToLower() == "td" || e.Name.LocalName.ToLower() == "th");

                    foreach (var cell in cells)
                    {
                        var colSpan = 1;
                        var rowSpan = 1;

                        var colSpanAttr = cell.Attribute("colspan");
                        if (colSpanAttr != null && int.TryParse(colSpanAttr.Value, out int cs))
                            colSpan = cs;

                        var rowSpanAttr = cell.Attribute("rowspan");
                        if (rowSpanAttr != null && int.TryParse(rowSpanAttr.Value, out int rs))
                            rowSpan = rs;

                        var fieldName = cell.Value.Trim();

                        layoutRow.Add(new LayoutCell
                        {
                            FieldName = fieldName,
                            ColSpan = colSpan,
                            RowSpan = rowSpan
                        });
                    }

                    if (layoutRow.Count > 0)
                        layout.Add(layoutRow);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid HTML table format: {ex.Message}", nameof(htmlTable), ex);
            }

            return layout;
        }

        #endregion

        #region Dialog Methods

        /// <summary>
        /// Shows an insert dialog for creating a new record.
        /// </summary>
        /// <returns>CRUDResult containing the values and SQL scripts, or null if cancelled</returns>
        public CRUDResult ShowInsertDialog()
        {
            using (var dialog = CreateCRUDForm("Insert Record", CRUDOperation.Insert, null))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return ExtractResult(dialog, CRUDOperation.Insert, null);
                }
            }
            return null;
        }

        /// <summary>
        /// Shows an update dialog for modifying an existing record.
        /// </summary>
        /// <param name="currentValues">Current values of the record (by field name)</param>
        /// <returns>CRUDResult containing the new values and SQL scripts, or null if cancelled</returns>
        public CRUDResult ShowUpdateDialog(Dictionary<string, object> currentValues)
        {
            if (currentValues == null) throw new ArgumentNullException(nameof(currentValues));

            using (var dialog = CreateCRUDForm("Update Record", CRUDOperation.Update, currentValues))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return ExtractResult(dialog, CRUDOperation.Update, currentValues);
                }
            }
            return null;
        }

        /// <summary>
        /// Shows an update dialog for modifying an existing record.
        /// </summary>
        /// <param name="dataRow">DataRow containing current values</param>
        /// <returns>CRUDResult containing the new values and SQL scripts, or null if cancelled</returns>
        public CRUDResult ShowUpdateDialog(DataRow dataRow)
        {
            if (dataRow == null) throw new ArgumentNullException(nameof(dataRow));
            return ShowUpdateDialog(ConvertDataRowToValues(dataRow));
        }

        /// <summary>
        /// Shows an update dialog for modifying an existing record with explicit column mapping.
        /// Use this when DataRow column names don't match field names, or when you need more control.
        /// </summary>
        /// <param name="dataRow">DataRow containing current values</param>
        /// <param name="columnMapping">Maps DataRow column names to field names (DataRow column → Field name)</param>
        /// <returns>CRUDResult containing the new values and SQL scripts, or null if cancelled</returns>
        public CRUDResult ShowUpdateDialog(DataRow dataRow, Dictionary<string, string> columnMapping)
        {
            if (dataRow == null) throw new ArgumentNullException(nameof(dataRow));
            if (columnMapping == null) throw new ArgumentNullException(nameof(columnMapping));
            return ShowUpdateDialog(ConvertDataRowToValues(dataRow, columnMapping));
        }

        /// <summary>
        /// Diagnostic method to see what values will be extracted from a DataRow.
        /// Use this to debug issues with ShowUpdateDialog or ShowDeleteDialog.
        /// </summary>
        /// <param name="dataRow">DataRow to analyze</param>
        /// <returns>Dictionary showing field names and their matched values</returns>
        public Dictionary<string, object> PreviewDataRowValues(DataRow dataRow)
        {
            if (dataRow == null) throw new ArgumentNullException(nameof(dataRow));
            return ConvertDataRowToValues(dataRow);
        }

        /// <summary>
        /// Shows a delete confirmation dialog with read-only field values.
        /// </summary>
        /// <param name="currentValues">Current values of the record to delete</param>
        /// <returns>CRUDResult containing the values and DELETE SQL scripts, or null if cancelled</returns>
        public CRUDResult ShowDeleteDialog(Dictionary<string, object> currentValues)
        {
            if (currentValues == null) throw new ArgumentNullException(nameof(currentValues));

            using (var dialog = CreateCRUDForm("Delete Record - Confirm", CRUDOperation.Delete, currentValues))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return ExtractResult(dialog, CRUDOperation.Delete, currentValues);
                }
            }
            return null;
        }

        /// <summary>
        /// Shows a delete confirmation dialog with read-only field values.
        /// </summary>
        /// <param name="dataRow">DataRow containing current values</param>
        /// <returns>CRUDResult containing the values and DELETE SQL scripts, or null if cancelled</returns>
        public CRUDResult ShowDeleteDialog(DataRow dataRow)
        {
            if (dataRow == null) throw new ArgumentNullException(nameof(dataRow));
            return ShowDeleteDialog(ConvertDataRowToValues(dataRow));
        }

        /// <summary>
        /// Shows a delete confirmation dialog with read-only field values and explicit column mapping.
        /// </summary>
        /// <param name="dataRow">DataRow containing current values</param>
        /// <param name="columnMapping">Maps DataRow column names to field names (DataRow column → Field name)</param>
        /// <returns>CRUDResult containing the values and DELETE SQL scripts, or null if cancelled</returns>
        public CRUDResult ShowDeleteDialog(DataRow dataRow, Dictionary<string, string> columnMapping)
        {
            if (dataRow == null) throw new ArgumentNullException(nameof(dataRow));
            if (columnMapping == null) throw new ArgumentNullException(nameof(columnMapping));
            return ShowDeleteDialog(ConvertDataRowToValues(dataRow, columnMapping));
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Converts a DataRow to a values dictionary with intelligent matching for foreign keys and list fields.
        /// Matches values by both key (value) and display text, making it easy to use DataRows directly.
        /// </summary>
        /// <param name="dataRow">DataRow to convert</param>
        /// <returns>Dictionary of field names to values</returns>
        private Dictionary<string, object> ConvertDataRowToValues(DataRow dataRow)
        {
            return ConvertDataRowToValues(dataRow, null);
        }

        /// <summary>
        /// Converts a DataRow to a values dictionary with intelligent matching for foreign keys and list fields.
        /// </summary>
        /// <param name="dataRow">DataRow to convert</param>
        /// <param name="columnMapping">Optional mapping of DataRow column names to field names</param>
        /// <returns>Dictionary of field names to values</returns>
        private Dictionary<string, object> ConvertDataRowToValues(DataRow dataRow, Dictionary<string, string> columnMapping)
        {
            var currentValues = new Dictionary<string, object>();

            // First pass: Add all values from DataRow columns that match field names
            foreach (DataColumn col in dataRow.Table.Columns)
            {
                string fieldName = col.ColumnName;

                // Check if there's a mapping for this column
                if (columnMapping != null && columnMapping.ContainsKey(col.ColumnName))
                {
                    fieldName = columnMapping[col.ColumnName];
                }

                // Add value if field exists
                if (_fields.ContainsKey(fieldName))
                {
                    currentValues[fieldName] = dataRow[col];
                }
            }

            // Second pass: Process foreign keys and list fields for intelligent matching
            foreach (var field in _fields.Values)
            {
                // Skip if we don't have a value for this field
                if (!currentValues.ContainsKey(field.FieldName))
                {
                    currentValues[field.FieldName] = DBNull.Value;
                    continue;
                }

                object value = currentValues[field.FieldName];

                // Skip if value is null or DBNull
                if (value == null || value == DBNull.Value)
                    continue;

                // Handle foreign key fields - try to match by both key and display value
                if (field.ForeignKeyInfo != null && field.ForeignKeyInfo.LookupValues != null)
                {
                    object matchedValue = MatchValueInLookup(value, field.ForeignKeyInfo.LookupValues);
                    currentValues[field.FieldName] = matchedValue;
                }
                // Handle list format fields - try to match by both key and display value
                else if (field.Format == FieldFormat.List &&
                         field.FormatConfig != null &&
                         field.FormatConfig.ListItems != null)
                {
                    object matchedValue = MatchValueInLookup(value, field.FormatConfig.ListItems);
                    currentValues[field.FieldName] = matchedValue;
                }
            }

            return currentValues;
        }

        /// <summary>
        /// Matches a value from a DataRow against a lookup dictionary (for foreign keys or lists).
        /// Tries to match by value first, then by display key.
        /// </summary>
        /// <param name="dataRowValue">Value from the DataRow</param>
        /// <param name="lookupValues">Lookup dictionary</param>
        /// <returns>Matched value or original value if no match found</returns>
        private object MatchValueInLookup(object dataRowValue, Dictionary<string, object> lookupValues)
        {
            if (dataRowValue == null || dataRowValue == DBNull.Value)
                return dataRowValue;

            // Try to match by value (the actual stored value)
            foreach (var kvp in lookupValues)
            {
                if (kvp.Value != null && kvp.Value != DBNull.Value)
                {
                    // Exact match
                    if (kvp.Value.Equals(dataRowValue))
                        return kvp.Value;

                    // Try numeric comparison for different numeric types (int vs long vs decimal etc)
                    if (IsNumericType(kvp.Value) && IsNumericType(dataRowValue))
                    {
                        try
                        {
                            if (Convert.ToDecimal(kvp.Value) == Convert.ToDecimal(dataRowValue))
                                return kvp.Value;
                        }
                        catch
                        {
                            // Conversion failed, continue to next comparison
                        }
                    }

                    // String comparison match
                    if (kvp.Value.ToString() == dataRowValue.ToString())
                        return kvp.Value;
                }
            }

            // Try to match by display key (in case DataRow contains display text)
            string dataRowValueStr = dataRowValue.ToString();
            foreach (var kvp in lookupValues)
            {
                // Match by display key
                if (kvp.Key.Equals(dataRowValueStr, StringComparison.OrdinalIgnoreCase))
                    return kvp.Value;
            }

            // No match found, return original value
            return dataRowValue;
        }

        /// <summary>
        /// Checks if an object is a numeric type.
        /// </summary>
        private bool IsNumericType(object value)
        {
            return value is sbyte || value is byte || value is short || value is ushort ||
                   value is int || value is uint || value is long || value is ulong ||
                   value is float || value is double || value is decimal;
        }

        #endregion

        #region Private Schema Loading Methods

        private void LoadSchemaFromSql(SqlConnection connection, string tableName)
        {
            var wasOpen = connection.State == ConnectionState.Open;
            if (!wasOpen) connection.Open();

            try
            {
                // Get column schema
                using (var cmd = new SqlCommand($"SELECT TOP 0 * FROM [{tableName}]", connection))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var schemaTable = reader.GetSchemaTable();
                    int order = 0;
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        var fieldName = row["ColumnName"].ToString();
                        var fieldMetadata = new FieldMetadata
                        {
                            FieldName = fieldName,
                            DataType = (Type)row["DataType"],
                            AllowNull = (bool)row["AllowDBNull"],
                            MaxLength = row["ColumnSize"] != DBNull.Value ? Convert.ToInt32(row["ColumnSize"]) : -1,
                            IsAutoIncrement = row["IsAutoIncrement"] != DBNull.Value && (bool)row["IsAutoIncrement"],
                            Order = order++
                        };

                        _fields[fieldName] = fieldMetadata;
                    }
                }

                // Get primary key information
                using (var cmd = new SqlCommand(@"
                    SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                    WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1
                    AND TABLE_NAME = @TableName", connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var columnName = reader.GetString(0);
                            if (_fields.ContainsKey(columnName))
                            {
                                _fields[columnName].IsPrimaryKey = true;
                            }
                        }
                    }
                }

                // Get foreign key information
                using (var cmd = new SqlCommand(@"
                    SELECT
                        fk.COLUMN_NAME,
                        pk.TABLE_NAME AS REFERENCED_TABLE,
                        pk.COLUMN_NAME AS REFERENCED_COLUMN
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
                    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE fk
                        ON rc.CONSTRAINT_NAME = fk.CONSTRAINT_NAME
                    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE pk
                        ON rc.UNIQUE_CONSTRAINT_NAME = pk.CONSTRAINT_NAME
                    WHERE fk.TABLE_NAME = @TableName", connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var columnName = reader.GetString(0);
                            var refTable = reader.GetString(1);
                            var refColumn = reader.GetString(2);

                            if (_fields.ContainsKey(columnName))
                            {
                                _fields[columnName].ForeignKeyInfo = new ForeignKeyInfo
                                {
                                    ReferencedTable = refTable,
                                    ReferencedColumn = refColumn,
                                    LookupValues = new Dictionary<string, object>()
                                };
                            }
                        }
                    }
                }
            }
            finally
            {
                if (!wasOpen) connection.Close();
            }
        }

        private void LoadSchemaFromDataTable(DataTable dataTable)
        {
            int order = 0;
            foreach (DataColumn col in dataTable.Columns)
            {
                var fieldMetadata = new FieldMetadata
                {
                    FieldName = col.ColumnName,
                    DataType = col.DataType,
                    AllowNull = col.AllowDBNull,
                    MaxLength = col.MaxLength,
                    IsAutoIncrement = col.AutoIncrement,
                    Order = order++
                };

                _fields[col.ColumnName] = fieldMetadata;

                if (dataTable.PrimaryKey != null && dataTable.PrimaryKey.Contains(col))
                {
                    fieldMetadata.IsPrimaryKey = true;
                }
            }
        }

        private void LoadSchemaFromDictionary(Dictionary<string, object> fields)
        {
            int order = 0;
            foreach (var kvp in fields)
            {
                var fieldMetadata = new FieldMetadata
                {
                    FieldName = kvp.Key,
                    DataType = kvp.Value?.GetType() ?? typeof(string),
                    AllowNull = true,
                    MaxLength = -1,
                    Order = order++
                };

                _fields[kvp.Key] = fieldMetadata;
            }
        }

        #endregion

        #region Private Form Generation Methods

        private Form CreateCRUDForm(string title, CRUDOperation operation, Dictionary<string, object> currentValues)
        {
            // Store currentValues for use in Load and Shown events
            var storedCurrentValues = currentValues;

            var form = new Form
            {
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ClientSize = new Size(_formWidth - 16, 100), // Set initial client size
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(10),
                MinimumSize = new Size(_formWidth, 100),
                MaximumSize = new Size(_formWidth, Screen.PrimaryScreen.WorkingArea.Height),
                AutoScroll = true
            };

            TableLayoutPanel mainPanel;

            // Generate layout based on mode
            // Pass null for currentValues during control creation - values will be set in Shown event
            switch (_layoutMode)
            {
                case LayoutMode.HtmlTable:
                    mainPanel = CreateHtmlTableLayout(operation, null);
                    break;
                case LayoutMode.ColumnCount:
                    mainPanel = CreateColumnCountLayout(operation, null);
                    break;
                case LayoutMode.Auto:
                default:
                    mainPanel = CreateAutoLayout(operation, null);
                    break;
            }

            // Add buttons
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Padding = new Padding(5)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Width = 80
            };

            var btnOK = new Button
            {
                Text = operation == CRUDOperation.Delete ? "Delete" : "OK",
                DialogResult = DialogResult.OK,
                Width = 80
            };

            btnOK.Click += (s, e) =>
            {
                // Validate password confirmations
                foreach (var field in _fields.Values)
                {
                    if (field.MaskInfo != null && field.MaskInfo.MaskType == MaskType.Password)
                    {
                        var mainControl = form.Controls.Find(field.FieldName, true).FirstOrDefault() as TextBox;
                        var confirmControl = form.Controls.Find(field.FieldName + "_Confirm", true).FirstOrDefault() as TextBox;

                        if (mainControl != null && confirmControl != null && mainControl.Text != confirmControl.Text)
                        {
                            MessageBoxDialog.Show($"Password confirmation for '{field.Alias ?? field.FieldName}' does not match.",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            form.DialogResult = DialogResult.None;
                            return;
                        }
                    }
                }

                // Validate required fields and custom validations
                foreach (var field in _fields.Values)
                {
                    // Skip excluded fields and auto-increment fields on insert
                    if (field.IsExcluded || (operation == CRUDOperation.Insert && field.IsAutoIncrement))
                        continue;

                    // Get the control value
                    object value = GetControlValue(form, field);

                    // Validate required fields
                    if (field.IsRequired && !field.IsPrimaryKey)
                    {
                        bool isEmpty = value == null || value == DBNull.Value ||
                                      (value is string str && string.IsNullOrWhiteSpace(str));

                        if (isEmpty)
                        {
                            MessageBoxDialog.Show($"Field '{field.Alias ?? field.FieldName}' is required.",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            form.DialogResult = DialogResult.None;
                            return;
                        }
                    }

                    // Validate custom validation method
                    if (field.ValidationMethod != null)
                    {
                        string validationError = field.ValidationMethod(value);
                        if (!string.IsNullOrEmpty(validationError))
                        {
                            MessageBoxDialog.Show($"Validation failed for '{field.Alias ?? field.FieldName}':\n{validationError}",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            form.DialogResult = DialogResult.None;
                            return;
                        }
                    }
                }
            };

            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnOK);

            form.Controls.Add(mainPanel);
            form.Controls.Add(buttonPanel);
            form.AcceptButton = btnOK;
            form.CancelButton = btnCancel;

            // Hook Load event to populate list controls (ComboBox, ListBox, etc.) with their possible values
            form.Load += (s, e) =>
            {
                PopulateListControls(form);
            };

            // Hook Shown event to set the actual values from currentValues
            form.Shown += (s, e) =>
            {
                if (storedCurrentValues != null)
                {
                    SetControlValues(form, storedCurrentValues);
                }
            };

            ApplyThemeToForm(form);

            return form;
        }

        /// <summary>
        /// Populates list controls (ComboBox, ListBox, etc.) with their possible values.
        /// Called in the Load event.
        /// </summary>
        private void PopulateListControls(Form form)
        {
            foreach (Control control in GetAllControls(form))
            {
                // Handle ComboBox controls
                if (control is ComboBox combo && combo.Tag is FieldMetadata field)
                {
                    // Foreign key ComboBox
                    if (field.ForeignKeyInfo != null)
                    {
                        var items = new List<KeyValuePair<string, object>>();
                        if (field.AllowNull)
                            items.Add(new KeyValuePair<string, object>("(NULL)", DBNull.Value));
                        items.AddRange(field.ForeignKeyInfo.LookupValues);
                        combo.DataSource = items;
                    }
                    // List format ComboBox
                    else if (field.Format == FieldFormat.List && field.FormatConfig != null && field.FormatConfig.ListItems != null)
                    {
                        var items = new List<KeyValuePair<string, object>>();
                        if (field.AllowNull)
                            items.Add(new KeyValuePair<string, object>("(NULL)", DBNull.Value));
                        items.AddRange(field.FormatConfig.ListItems);
                        combo.DataSource = items;
                    }
                }
            }
        }

        /// <summary>
        /// Sets control values from the currentValues dictionary.
        /// Called in the Shown event after all controls are populated.
        /// </summary>
        private void SetControlValues(Form form, Dictionary<string, object> currentValues)
        {
            foreach (var kvp in currentValues)
            {
                string fieldName = kvp.Key;
                object currentValue = kvp.Value;

                if (!_fields.ContainsKey(fieldName))
                    continue;

                var field = _fields[fieldName];

                // Find the control
                var controls = form.Controls.Find(fieldName, true);
                if (controls.Length == 0)
                {
                    // Try to find panel for formatted controls
                    controls = form.Controls.Find(fieldName + "_Panel", true);
                }
                if (controls.Length == 0)
                    continue;

                var control = controls[0];

                // Set value based on control type
                if (control is ComboBox combo)
                {
                    if (currentValue != null && currentValue != DBNull.Value)
                    {
                        // Set SelectedValue for data-bound ComboBox
                        combo.SelectedValue = currentValue;
                    }
                }
                else if (control is TextBox tb)
                {
                    if (field.MaskInfo != null && field.MaskInfo.MaskType == MaskType.Custom && field.MaskInfo.MaskMethod != null)
                    {
                        tb.Text = field.MaskInfo.MaskMethod(currentValue?.ToString() ?? "");
                    }
                    else
                    {
                        tb.Text = currentValue?.ToString() ?? "";
                    }
                }
                else if (control is CheckBox chk)
                {
                    chk.Checked = currentValue != null && currentValue != DBNull.Value &&
                                  (Convert.ToInt32(currentValue) == 1 || Convert.ToBoolean(currentValue));
                }
                else if (control is DateTimePicker dtp)
                {
                    if (currentValue != null && currentValue != DBNull.Value)
                    {
                        if (currentValue is DateTime dt)
                            dtp.Value = dt;
                        else if (DateTime.TryParse(currentValue.ToString(), out DateTime parsedDate))
                            dtp.Value = parsedDate;
                    }
                }
                else if (control is NumericUpDown num)
                {
                    if (currentValue != null && currentValue != DBNull.Value)
                        num.Value = Convert.ToDecimal(currentValue);
                }
                else if (control is Panel panel)
                {
                    // Handle formatted controls in panels
                    SetFormattedControlValue(panel, field, currentValue);
                }
            }
        }

        /// <summary>
        /// Sets value for formatted controls that are inside panels.
        /// </summary>
        private void SetFormattedControlValue(Panel panel, FieldMetadata field, object currentValue)
        {
            switch (field.Format)
            {
                case FieldFormat.File:
                case FieldFormat.Folder:
                    var textBox = panel.Controls.Find(field.FieldName + "_TextBox", false).FirstOrDefault() as TextBox;
                    if (textBox != null)
                        textBox.Text = currentValue?.ToString() ?? "";
                    break;

                case FieldFormat.DateTime:
                    var datePicker = panel.Controls.Find(field.FieldName + "_Date", false).FirstOrDefault() as DateTimePicker;
                    var timePicker = panel.Controls.Find(field.FieldName + "_Time", false).FirstOrDefault() as DateTimePicker;
                    if (datePicker != null && timePicker != null && currentValue != null && currentValue != DBNull.Value)
                    {
                        DateTime dt = DateTime.Now;
                        if (currentValue is DateTime dateTime)
                            dt = dateTime;
                        else if (DateTime.TryParse(currentValue.ToString(), out DateTime parsed))
                            dt = parsed;

                        datePicker.Value = dt;
                        timePicker.Value = dt;
                    }
                    break;
            }
        }

        /// <summary>
        /// Recursively gets all controls in a form, including nested controls.
        /// </summary>
        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;
                foreach (Control child in GetAllControls(control))
                {
                    yield return child;
                }
            }
        }

        private TableLayoutPanel CreateAutoLayout(CRUDOperation operation, Dictionary<string, object> currentValues)
        {
            // Auto layout is just a 2-column layout (same as column count layout with 1 column of label-control pairs)
            // For simplicity and consistency, just use the column count layout with 1 column
            int savedColumnCount = _columnCount;
            _columnCount = 1;
            var result = CreateColumnCountLayout(operation, currentValues);
            _columnCount = savedColumnCount;
            return result;
        }

        private TableLayoutPanel CreateColumnCountLayout(CRUDOperation operation, Dictionary<string, object> currentValues)
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = _columnCount * 2, // label + control for each column
                Padding = new Padding(5)
            };

            // Add column styles
            for (int i = 0; i < _columnCount; i++)
            {
                mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label
                mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / _columnCount)); // Control
            }

            int rowIndex = 0;
            int colIndex = 0;
            var sortedFields = _fields.Values.OrderBy(f => f.Order).ToList();

            foreach (var field in sortedFields)
            {
                if (!ShouldShowField(field, operation))
                    continue;

                AddFieldToPanel(mainPanel, field, operation, currentValues, ref rowIndex, colIndex * 2, 1, 1);

                colIndex++;
                if (colIndex >= _columnCount)
                {
                    colIndex = 0;
                    rowIndex++;
                }
            }

            return mainPanel;
        }

        private TableLayoutPanel CreateHtmlTableLayout(CRUDOperation operation, Dictionary<string, object> currentValues)
        {
            if (_parsedLayout == null || _parsedLayout.Count == 0)
                return CreateAutoLayout(operation, currentValues);

            // For HTML layout, we use _columnCount as the total number of columns
            // Each cell in the HTML can span multiple columns
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = _columnCount,
                Padding = new Padding(5)
            };

            // Add column styles - equal width for all columns
            for (int i = 0; i < _columnCount; i++)
            {
                mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / _columnCount));
            }

            int currentRow = 0;

            foreach (var layoutRow in _parsedLayout)
            {
                // Add row style
                mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                mainPanel.RowCount++;

                int currentCol = 0;
                foreach (var cell in layoutRow)
                {
                    if (!_fields.ContainsKey(cell.FieldName))
                    {
                        currentCol += cell.ColSpan;
                        continue;
                    }

                    var field = _fields[cell.FieldName];
                    if (!ShouldShowField(field, operation))
                    {
                        currentCol += cell.ColSpan;
                        continue;
                    }

                    // Create a panel to hold label and control vertically
                    var cellPanel = new TableLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        ColumnCount = 1,
                        RowCount = 2,
                        Margin = new Padding(3)
                    };

                    cellPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Label row
                    cellPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Control row
                    cellPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                    var label = new Label
                    {
                        Text = (field.Alias ?? field.FieldName) + ":",
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(3)
                    };

                    Control inputControl = CreateInputControl(field, operation, currentValues);
                    inputControl.Name = field.FieldName;
                    inputControl.Tag = field;
                    inputControl.Dock = DockStyle.Fill;
                    inputControl.Margin = new Padding(3);

                    cellPanel.Controls.Add(label, 0, 0);

                    // For password fields, add confirmation control
                    if (field.MaskInfo != null && field.MaskInfo.MaskType == MaskType.Password)
                    {
                        cellPanel.RowCount = 3;
                        cellPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Confirm control row

                        var confirmControl = new TextBox
                        {
                            Name = field.FieldName + "_Confirm",
                            PasswordChar = '\u2022',
                            Dock = DockStyle.Fill,
                            Margin = new Padding(3)
                        };

                        if (operation == CRUDOperation.Delete)
                            confirmControl.ReadOnly = true;

                        cellPanel.Controls.Add(inputControl, 0, 1);
                        cellPanel.Controls.Add(confirmControl, 0, 2);
                    }
                    else
                    {
                        cellPanel.Controls.Add(inputControl, 0, 1);
                    }

                    mainPanel.Controls.Add(cellPanel, currentCol, currentRow);
                    mainPanel.SetColumnSpan(cellPanel, cell.ColSpan);
                    mainPanel.SetRowSpan(cellPanel, cell.RowSpan);

                    currentCol += cell.ColSpan;
                }

                currentRow++;
            }

            return mainPanel;
        }

        private bool ShouldShowField(FieldMetadata field, CRUDOperation operation)
        {
            // Skip auto-increment fields on insert
            if (operation == CRUDOperation.Insert && field.IsAutoIncrement)
                return false;

            // Skip excluded fields unless they're primary keys (needed for update/delete)
            if (field.IsExcluded && !field.IsPrimaryKey)
                return false;

            return true;
        }

        private void AddFieldToPanel(TableLayoutPanel panel, FieldMetadata field, CRUDOperation operation,
            Dictionary<string, object> currentValues, ref int rowIndex, int colIndex, int colSpan, int rowSpan)
        {
            var label = new Label
            {
                Text = (field.Alias ?? field.FieldName) + ":",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(3, 6, 3, 3)
            };

            Control inputControl = CreateInputControl(field, operation, currentValues);
            inputControl.Name = field.FieldName;
            inputControl.Tag = field;
            inputControl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            inputControl.MinimumSize = new Size(150, 0);

            // Ensure row exists
            while (panel.RowCount <= rowIndex)
            {
                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                panel.RowCount++;
            }

            // For password fields, create confirmation field
            if (field.MaskInfo != null && field.MaskInfo.MaskType == MaskType.Password)
            {
                panel.Controls.Add(label, colIndex, rowIndex);
                panel.Controls.Add(inputControl, colIndex + 1, rowIndex);
                panel.SetColumnSpan(inputControl, colSpan);
                rowIndex++;

                var confirmLabel = new Label
                {
                    Text = "Confirm " + (field.Alias ?? field.FieldName) + ":",
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 6, 3, 3)
                };

                var confirmControl = new TextBox
                {
                    Name = field.FieldName + "_Confirm",
                    PasswordChar = '\u2022',
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    MinimumSize = new Size(150, 0)
                };

                if (operation == CRUDOperation.Delete)
                    confirmControl.ReadOnly = true;

                while (panel.RowCount <= rowIndex)
                {
                    panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    panel.RowCount++;
                }

                panel.Controls.Add(confirmLabel, colIndex, rowIndex);
                panel.Controls.Add(confirmControl, colIndex + 1, rowIndex);
                panel.SetColumnSpan(confirmControl, colSpan);
            }
            else
            {
                panel.Controls.Add(label, colIndex, rowIndex);
                panel.Controls.Add(inputControl, colIndex + 1, rowIndex);
                panel.SetColumnSpan(inputControl, colSpan);
            }
        }

        private Control CreateInputControl(FieldMetadata field, CRUDOperation operation, Dictionary<string, object> currentValues)
        {
            Control control = null;
            object currentValue = null;

            if (currentValues != null && currentValues.ContainsKey(field.FieldName))
                currentValue = currentValues[field.FieldName];

            // Check for custom format first
            if (field.Format != FieldFormat.Default)
            {
                control = CreateFormattedControl(field, currentValue);
            }
            // Foreign key - use ComboBox
            else if (field.ForeignKeyInfo != null)
            {
                var combo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DisplayMember = "Key",
                    ValueMember = "Value",
                    Name = field.FieldName,
                    Tag = field  // Store FieldMetadata for PopulateListControls to find
                };

                // Don't populate items here - will be done in Load event via PopulateListControls
                // Don't set value here - will be done in Shown event via SetControlValues

                control = combo;
            }
            // Boolean - use CheckBox
            else if (field.DataType == typeof(bool))
            {
                var checkbox = new CheckBox
                {
                    Name = field.FieldName
                };
                // Don't set Checked here - will be set in Shown event
                control = checkbox;
            }
            // DateTime - use DateTimePicker
            else if (field.DataType == typeof(DateTime))
            {
                var picker = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Name = field.FieldName
                };
                // Don't set Value here - will be set in Shown event
                control = picker;
            }
            // Numeric types - use NumericUpDown
            else if (field.DataType == typeof(int) || field.DataType == typeof(long) ||
                     field.DataType == typeof(short) || field.DataType == typeof(byte) ||
                     field.DataType == typeof(decimal) || field.DataType == typeof(double) ||
                     field.DataType == typeof(float))
            {
                var numeric = new NumericUpDown
                {
                    Maximum = decimal.MaxValue,
                    Minimum = decimal.MinValue,
                    DecimalPlaces = (field.DataType == typeof(decimal) || field.DataType == typeof(double) || field.DataType == typeof(float)) ? 2 : 0,
                    Name = field.FieldName
                };
                // Don't set Value here - will be set in Shown event
                control = numeric;
            }
            // Large text - use multiline TextBox
            else if (field.MaxLength > 500 || field.MaxLength == -1)
            {
                var textBox = new TextBox
                {
                    Multiline = true,
                    Height = 100,
                    ScrollBars = ScrollBars.Vertical,
                    Name = field.FieldName
                };
                // Don't set Text here - will be set in Shown event
                control = textBox;
            }
            // Default - use TextBox
            else
            {
                var textBox = new TextBox
                {
                    MaxLength = field.MaxLength > 0 ? field.MaxLength : 32767,
                    Name = field.FieldName
                };

                // Apply password masking
                if (field.MaskInfo != null && field.MaskInfo.MaskType == MaskType.Password)
                {
                    textBox.PasswordChar = '\u2022';
                }

                // Don't set Text here - will be set in Shown event
                control = textBox;
            }

            // Make control disabled for delete operation or primary key in update
            if (operation == CRUDOperation.Delete || (operation == CRUDOperation.Update && field.IsPrimaryKey))
            {
                if (control is TextBox tb)
                {
                    // For primary keys on update, disable instead of readonly for better visual indication
                    if (operation == CRUDOperation.Update && field.IsPrimaryKey)
                        tb.Enabled = false;
                    else
                        tb.ReadOnly = true;
                }
                else if (control is ComboBox cb)
                    cb.Enabled = false;
                else if (control is CheckBox chk)
                    chk.Enabled = false;
                else if (control is DateTimePicker dtp)
                    dtp.Enabled = false;
                else if (control is NumericUpDown num)
                    num.Enabled = false;
                else if (control is Panel pnl)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is TextBox txtBox)
                        {
                            // For primary keys on update, disable instead of readonly
                            if (operation == CRUDOperation.Update && field.IsPrimaryKey)
                                txtBox.Enabled = false;
                            else
                                txtBox.ReadOnly = true;
                        }
                        else if (c is Button btn)
                            btn.Enabled = false;
                        else if (c is DateTimePicker dtPicker)
                            dtPicker.Enabled = false;
                    }
                }
            }

            return control;
        }

        private Control CreateFormattedControl(FieldMetadata field, object currentValue)
        {
            switch (field.Format)
            {
                case FieldFormat.Date:
                    var datePicker = new DateTimePicker
                    {
                        Format = DateTimePickerFormat.Short
                    };
                    if (currentValue != null && currentValue != DBNull.Value)
                    {
                        if (currentValue is DateTime dt)
                            datePicker.Value = dt;
                        else if (DateTime.TryParse(currentValue.ToString(), out DateTime parsedDate))
                            datePicker.Value = parsedDate;
                    }
                    return datePicker;

                case FieldFormat.Time:
                    var timePicker = new DateTimePicker
                    {
                        Format = DateTimePickerFormat.Time,
                        ShowUpDown = true
                    };
                    if (currentValue != null && currentValue != DBNull.Value)
                    {
                        if (currentValue is DateTime dt)
                            timePicker.Value = dt;
                        else if (DateTime.TryParse(currentValue.ToString(), out DateTime parsedTime))
                            timePicker.Value = parsedTime;
                    }
                    return timePicker;

                case FieldFormat.DateTime:
                    var dateTimePanel = new Panel
                    {
                        Height = 25,
                        Name = field.FieldName + "_Panel"
                    };
                    var datePickerDT = new DateTimePicker
                    {
                        Format = DateTimePickerFormat.Short,
                        Name = field.FieldName + "_Date",
                        Width = 120,
                        Location = new Point(0, 0),
                        Anchor = AnchorStyles.Left | AnchorStyles.Top
                    };
                    var timePickerDT = new DateTimePicker
                    {
                        Format = DateTimePickerFormat.Time,
                        ShowUpDown = true,
                        Name = field.FieldName + "_Time",
                        Width = 100,
                        Location = new Point(125, 0),
                        Anchor = AnchorStyles.Left | AnchorStyles.Top
                    };
                    // Don't set Value here - will be set in Shown event
                    dateTimePanel.Controls.Add(datePickerDT);
                    dateTimePanel.Controls.Add(timePickerDT);
                    return dateTimePanel;

                case FieldFormat.File:
                    var filePanel = new Panel
                    {
                        Height = 25,
                        Name = field.FieldName + "_Panel"
                    };
                    var fileTextBox = new TextBox
                    {
                        Name = field.FieldName + "_TextBox",
                        ReadOnly = true,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                        Location = new Point(0, 0)
                        // Don't set Text here - will be set in Shown event
                    };
                    var fileBrowseButton = new Button
                    {
                        Text = "...",
                        Name = field.FieldName + "_Button",
                        Width = 30,
                        Anchor = AnchorStyles.Right | AnchorStyles.Top,
                        Location = new Point(0, -1)
                    };
                    fileBrowseButton.Click += (s, e) =>
                    {
                        using (var ofd = new OpenFileDialog())
                        {
                            if (field.FormatConfig != null && !string.IsNullOrEmpty(field.FormatConfig.FileFilter))
                                ofd.Filter = field.FormatConfig.FileFilter;
                            if (ofd.ShowDialog() == DialogResult.OK)
                                fileTextBox.Text = ofd.FileName;
                        }
                    };
                    filePanel.Controls.Add(fileBrowseButton);
                    filePanel.Controls.Add(fileTextBox);
                    // Position button on the right
                    fileBrowseButton.Left = filePanel.Width - fileBrowseButton.Width;
                    // Size textbox to leave room for button
                    fileTextBox.Width = filePanel.Width - fileBrowseButton.Width - 5;
                    return filePanel;

                case FieldFormat.Folder:
                    var folderPanel = new Panel
                    {
                        Height = 25,
                        Name = field.FieldName + "_Panel"
                    };
                    var folderTextBox = new TextBox
                    {
                        Name = field.FieldName + "_TextBox",
                        ReadOnly = true,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                        Location = new Point(0, 0)
                        // Don't set Text here - will be set in Shown event
                    };
                    var folderBrowseButton = new Button
                    {
                        Text = "...",
                        Name = field.FieldName + "_Button",
                        Width = 30,
                        Anchor = AnchorStyles.Right | AnchorStyles.Top,
                        Location = new Point(0, -1)
                    };
                    folderBrowseButton.Click += (s, e) =>
                    {
                        using (var fbd = new FolderBrowserDialog())
                        {
                            if (!string.IsNullOrEmpty(folderTextBox.Text))
                                fbd.SelectedPath = folderTextBox.Text;
                            if (fbd.ShowDialog() == DialogResult.OK)
                                folderTextBox.Text = fbd.SelectedPath;
                        }
                    };
                    folderPanel.Controls.Add(folderBrowseButton);
                    folderPanel.Controls.Add(folderTextBox);
                    // Position button on the right
                    folderBrowseButton.Left = folderPanel.Width - folderBrowseButton.Width;
                    // Size textbox to leave room for button
                    folderTextBox.Width = folderPanel.Width - folderBrowseButton.Width - 5;
                    return folderPanel;

                case FieldFormat.List:
                    var listCombo = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        DisplayMember = "Key",
                        ValueMember = "Value",
                        Name = field.FieldName,
                        Tag = field  // Store FieldMetadata for PopulateListControls to find
                    };

                    // Don't populate items here - will be done in Load event via PopulateListControls
                    // Don't set value here - will be done in Shown event via SetControlValues

                    return listCombo;

                case FieldFormat.Integer:
                    var intNumeric = new NumericUpDown
                    {
                        DecimalPlaces = 0,
                        Maximum = field.FormatConfig?.MaxValue ?? decimal.MaxValue,
                        Minimum = field.FormatConfig?.MinValue ?? decimal.MinValue,
                        Name = field.FieldName
                    };
                    // Don't set Value here - will be set in Shown event
                    return intNumeric;

                case FieldFormat.Check:
                    var checkBox = new CheckBox
                    {
                        Name = field.FieldName
                    };
                    // Don't set Checked here - will be set in Shown event
                    return checkBox;

                case FieldFormat.Float:
                    var floatNumeric = new NumericUpDown
                    {
                        DecimalPlaces = 2,
                        Maximum = field.FormatConfig?.MaxValue ?? decimal.MaxValue,
                        Minimum = field.FormatConfig?.MinValue ?? decimal.MinValue,
                        Name = field.FieldName
                    };
                    // Don't set Value here - will be set in Shown event
                    return floatNumeric;

                default:
                    return new TextBox { Name = field.FieldName }; // Don't set Text - will be set in Shown event
            }
        }

        private void ApplyThemeToForm(Form form)
        {
            if (_theme != ZidThemes.None && _theme != ZidThemes.Default)
            {
                // Use the ThemeManager extension method to apply theme
                form.ApplyTheme(_theme);
            }
        }

        private CRUDResult ExtractResult(Form dialog, CRUDOperation operation, Dictionary<string, object> originalValues)
        {
            var result = new CRUDResult(_tableName);

            foreach (var field in _fields.Values)
            {
                // Skip auto-increment fields on insert
                if (operation == CRUDOperation.Insert && field.IsAutoIncrement)
                    continue;

                var controls = dialog.Controls.Find(field.FieldName, true);
                if (controls.Length == 0)
                {
                    // Try to find panel for formatted controls
                    controls = dialog.Controls.Find(field.FieldName + "_Panel", true);
                }
                if (controls.Length == 0)
                    continue;

                var control = controls[0];
                object value = null;

                // Handle formatted controls
                if (control is Panel panel)
                {
                    value = ExtractValueFromFormattedControl(panel, field);
                }
                else if (control is TextBox tb)
                {
                    value = string.IsNullOrWhiteSpace(tb.Text) ? (object)DBNull.Value : tb.Text;

                    // Apply unmask if custom mask
                    if (field.MaskInfo != null && field.MaskInfo.MaskType == MaskType.Custom && field.MaskInfo.UnmaskMethod != null)
                    {
                        value = field.MaskInfo.UnmaskMethod(tb.Text);
                    }
                }
                else if (control is ComboBox cb)
                {
                    value = cb.SelectedValue ?? DBNull.Value;
                }
                else if (control is CheckBox chk)
                {
                    // For Check format, return 1 or 0
                    if (field.Format == FieldFormat.Check)
                        value = chk.Checked ? 1 : 0;
                    else
                        value = chk.Checked;
                }
                else if (control is DateTimePicker dtp)
                {
                    // Format based on field format
                    if (field.Format == FieldFormat.Date)
                        value = dtp.Value.ToString("yyyy-MM-dd");
                    else if (field.Format == FieldFormat.Time)
                        value = dtp.Value.ToString("HH:mm:ss");
                    else
                        value = dtp.Value;
                }
                else if (control is NumericUpDown num)
                {
                    value = num.Value;
                }

                result.SetValue(field.FieldName, value);

                // Store primary key values
                if (field.IsPrimaryKey)
                {
                    result.AddPrimaryKeyValue(field.FieldName, value);
                }
            }

            // Generate SQL scripts
            result.SqlServerScript = GenerateSqlServerScript(operation, result, originalValues);
            result.SqliteScript = GenerateSqliteScript(operation, result, originalValues);
            result.AccessScript = GenerateAccessScript(operation, result, originalValues);

            return result;
        }

        private object ExtractValueFromFormattedControl(Panel panel, FieldMetadata field)
        {
            switch (field.Format)
            {
                case FieldFormat.File:
                case FieldFormat.Folder:
                    var textBox = panel.Controls.Find(field.FieldName + "_TextBox", false).FirstOrDefault() as TextBox;
                    if (textBox != null)
                        return string.IsNullOrWhiteSpace(textBox.Text) ? (object)DBNull.Value : textBox.Text;
                    break;

                case FieldFormat.DateTime:
                    var datePicker = panel.Controls.Find(field.FieldName + "_Date", false).FirstOrDefault() as DateTimePicker;
                    var timePicker = panel.Controls.Find(field.FieldName + "_Time", false).FirstOrDefault() as DateTimePicker;
                    if (datePicker != null && timePicker != null)
                    {
                        var combinedDateTime = new DateTime(
                            datePicker.Value.Year,
                            datePicker.Value.Month,
                            datePicker.Value.Day,
                            timePicker.Value.Hour,
                            timePicker.Value.Minute,
                            timePicker.Value.Second
                        );
                        return combinedDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
                    }
                    break;
            }
            return DBNull.Value;
        }

        private object GetControlValue(Form form, FieldMetadata field)
        {
            var controls = form.Controls.Find(field.FieldName, true);
            if (controls.Length == 0)
            {
                // Try to find panel for formatted controls
                controls = form.Controls.Find(field.FieldName + "_Panel", true);
            }
            if (controls.Length == 0)
                return null;

            var control = controls[0];

            if (control is Panel panel)
            {
                return ExtractValueFromFormattedControl(panel, field);
            }
            else if (control is TextBox tb)
            {
                return string.IsNullOrWhiteSpace(tb.Text) ? (object)DBNull.Value : tb.Text;
            }
            else if (control is ComboBox cb)
            {
                return cb.SelectedValue ?? DBNull.Value;
            }
            else if (control is CheckBox chk)
            {
                return field.Format == FieldFormat.Check ? (chk.Checked ? 1 : 0) : (object)chk.Checked;
            }
            else if (control is DateTimePicker dtp)
            {
                if (field.Format == FieldFormat.Date)
                    return dtp.Value.ToString("yyyy-MM-dd");
                else if (field.Format == FieldFormat.Time)
                    return dtp.Value.ToString("HH:mm:ss");
                else
                    return dtp.Value;
            }
            else if (control is NumericUpDown num)
            {
                return num.Value;
            }

            return null;
        }

        #endregion

        #region SQL Generation Methods

        private string GenerateSqlServerScript(CRUDOperation operation, CRUDResult result, Dictionary<string, object> originalValues)
        {
            var sb = new StringBuilder();

            switch (operation)
            {
                case CRUDOperation.Insert:
                    sb.AppendLine($"INSERT INTO [{_tableName}] (");
                    var insertFields = result.GetFieldNames().Where(f => !_fields[f].IsExcluded || _fields[f].ExclusionDefaultValue != null).ToList();
                    sb.AppendLine("    " + string.Join(", ", insertFields.Select(f => $"[{f}]")));
                    sb.AppendLine(") VALUES (");
                    sb.AppendLine("    " + string.Join(", ", insertFields.Select(f => FormatValueForSqlServer(result[f]))));
                    sb.Append(");");
                    break;

                case CRUDOperation.Update:
                    sb.AppendLine($"UPDATE [{_tableName}] SET");
                    var updateFields = result.GetFieldNames().Where(f => !_fields[f].IsPrimaryKey && (!_fields[f].IsExcluded || _fields[f].ExclusionDefaultValue != null)).ToList();
                    sb.AppendLine("    " + string.Join(",\n    ", updateFields.Select(f => $"[{f}] = {FormatValueForSqlServer(result[f])}")));
                    sb.AppendLine("WHERE");
                    sb.Append("    " + string.Join(" AND ", result.PrimaryKeyValues.Select(kvp => $"[{kvp.Key}] = {FormatValueForSqlServer(kvp.Value)}")));
                    sb.Append(";");
                    break;

                case CRUDOperation.Delete:
                    sb.AppendLine($"DELETE FROM [{_tableName}]");
                    sb.AppendLine("WHERE");
                    sb.Append("    " + string.Join(" AND ", result.PrimaryKeyValues.Select(kvp => $"[{kvp.Key}] = {FormatValueForSqlServer(kvp.Value)}")));
                    sb.Append(";");
                    break;
            }

            return sb.ToString();
        }

        private string GenerateSqliteScript(CRUDOperation operation, CRUDResult result, Dictionary<string, object> originalValues)
        {
            var sb = new StringBuilder();

            switch (operation)
            {
                case CRUDOperation.Insert:
                    sb.AppendLine($"INSERT INTO `{_tableName}` (");
                    var insertFields = result.GetFieldNames().Where(f => !_fields[f].IsExcluded || _fields[f].ExclusionDefaultValue != null).ToList();
                    sb.AppendLine("    " + string.Join(", ", insertFields.Select(f => $"`{f}`")));
                    sb.AppendLine(") VALUES (");
                    sb.AppendLine("    " + string.Join(", ", insertFields.Select(f => FormatValueForSqlite(result[f]))));
                    sb.Append(");");
                    break;

                case CRUDOperation.Update:
                    sb.AppendLine($"UPDATE `{_tableName}` SET");
                    var updateFields = result.GetFieldNames().Where(f => !_fields[f].IsPrimaryKey && (!_fields[f].IsExcluded || _fields[f].ExclusionDefaultValue != null)).ToList();
                    sb.AppendLine("    " + string.Join(",\n    ", updateFields.Select(f => $"`{f}` = {FormatValueForSqlite(result[f])}")));
                    sb.AppendLine("WHERE");
                    sb.Append("    " + string.Join(" AND ", result.PrimaryKeyValues.Select(kvp => $"`{kvp.Key}` = {FormatValueForSqlite(kvp.Value)}")));
                    sb.Append(";");
                    break;

                case CRUDOperation.Delete:
                    sb.AppendLine($"DELETE FROM `{_tableName}`");
                    sb.AppendLine("WHERE");
                    sb.Append("    " + string.Join(" AND ", result.PrimaryKeyValues.Select(kvp => $"`{kvp.Key}` = {FormatValueForSqlite(kvp.Value)}")));
                    sb.Append(";");
                    break;
            }

            return sb.ToString();
        }

        private string GenerateAccessScript(CRUDOperation operation, CRUDResult result, Dictionary<string, object> originalValues)
        {
            var sb = new StringBuilder();

            switch (operation)
            {
                case CRUDOperation.Insert:
                    sb.AppendLine($"INSERT INTO [{_tableName}] (");
                    var insertFields = result.GetFieldNames().Where(f => !_fields[f].IsExcluded || _fields[f].ExclusionDefaultValue != null).ToList();
                    sb.AppendLine("    " + string.Join(", ", insertFields.Select(f => $"[{f}]")));
                    sb.AppendLine(") VALUES (");
                    sb.AppendLine("    " + string.Join(", ", insertFields.Select(f => FormatValueForAccess(result[f]))));
                    sb.Append(");");
                    break;

                case CRUDOperation.Update:
                    sb.AppendLine($"UPDATE [{_tableName}] SET");
                    var updateFields = result.GetFieldNames().Where(f => !_fields[f].IsPrimaryKey && (!_fields[f].IsExcluded || _fields[f].ExclusionDefaultValue != null)).ToList();
                    sb.AppendLine("    " + string.Join(",\n    ", updateFields.Select(f => $"[{f}] = {FormatValueForAccess(result[f])}")));
                    sb.AppendLine("WHERE");
                    sb.Append("    " + string.Join(" AND ", result.PrimaryKeyValues.Select(kvp => $"[{kvp.Key}] = {FormatValueForAccess(kvp.Value)}")));
                    sb.Append(";");
                    break;

                case CRUDOperation.Delete:
                    sb.AppendLine($"DELETE FROM [{_tableName}]");
                    sb.AppendLine("WHERE");
                    sb.Append("    " + string.Join(" AND ", result.PrimaryKeyValues.Select(kvp => $"[{kvp.Key}] = {FormatValueForAccess(kvp.Value)}")));
                    sb.Append(";");
                    break;
            }

            return sb.ToString();
        }

        private string FormatValueForSqlServer(object value)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";

            if (value is string || value is char)
                return $"N'{value.ToString().Replace("'", "''")}'";

            if (value is DateTime dt)
                return $"'{dt:yyyy-MM-dd HH:mm:ss}'";

            if (value is bool b)
                return b ? "1" : "0";

            return value.ToString();
        }

        private string FormatValueForSqlite(object value)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";

            if (value is string || value is char)
                return $"'{value.ToString().Replace("'", "''")}'";

            if (value is DateTime dt)
                return $"'{dt:yyyy-MM-dd HH:mm:ss}'";

            if (value is bool b)
                return b ? "1" : "0";

            return value.ToString();
        }

        private string FormatValueForAccess(object value)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";

            if (value is string || value is char)
                return $"'{value.ToString().Replace("'", "''")}'";

            if (value is DateTime dt)
                return $"#{dt:MM/dd/yyyy HH:mm:ss}#";

            if (value is bool b)
                return b ? "True" : "False";

            return value.ToString();
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Metadata for a database field.
    /// </summary>
    internal class FieldMetadata
    {
        public string FieldName { get; set; }
        public string Alias { get; set; }
        public Type DataType { get; set; }
        public bool AllowNull { get; set; }
        public int MaxLength { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsExcluded { get; set; }
        public object ExclusionDefaultValue { get; set; }
        public MaskInfo MaskInfo { get; set; }
        public ForeignKeyInfo ForeignKeyInfo { get; set; }
        public FieldFormat Format { get; set; }
        public FormatConfig FormatConfig { get; set; }
        public bool IsRequired { get; set; }
        public Func<object, string> ValidationMethod { get; set; }
        public int Order { get; set; }
    }

    /// <summary>
    /// Information about field masking.
    /// </summary>
    public class MaskInfo
    {
        public MaskType MaskType { get; set; }
        public Func<string, string> MaskMethod { get; set; }
        public Func<string, string> UnmaskMethod { get; set; }
    }

    /// <summary>
    /// Type of masking to apply.
    /// </summary>
    public enum MaskType
    {
        None,
        Password,
        Custom
    }

    /// <summary>
    /// Field format types.
    /// </summary>
    public enum FieldFormat
    {
        Default,
        Date,
        Time,
        DateTime,
        File,
        Folder,
        List,
        Integer,
        Check,
        Float
    }

    /// <summary>
    /// Configuration for field formats.
    /// </summary>
    public class FormatConfig
    {
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string FileFilter { get; set; }
        public Dictionary<string, object> ListItems { get; set; }
    }

    /// <summary>
    /// Information about foreign key relationships.
    /// </summary>
    public class ForeignKeyInfo
    {
        public string ReferencedTable { get; set; }
        public string ReferencedColumn { get; set; }
        public Dictionary<string, object> LookupValues { get; set; }
    }

    /// <summary>
    /// CRUD operation type.
    /// </summary>
    public enum CRUDOperation
    {
        Insert,
        Update,
        Delete
    }

    /// <summary>
    /// Layout mode for form generation.
    /// </summary>
    public enum LayoutMode
    {
        Auto,
        ColumnCount,
        HtmlTable
    }

    /// <summary>
    /// Represents a cell in the layout grid.
    /// </summary>
    public class LayoutCell
    {
        public string FieldName { get; set; }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
    }

    /// <summary>
    /// Result of a CRUD dialog operation.
    /// </summary>
    public class CRUDResult
    {
        private readonly Dictionary<string, object> _values;
        private readonly Dictionary<string, int> _fieldIndexes;
        private readonly Dictionary<string, object> _primaryKeyValues;

        internal CRUDResult(string tableName)
        {
            TableName = tableName;
            _values = new Dictionary<string, object>();
            _fieldIndexes = new Dictionary<string, int>();
            _primaryKeyValues = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets the SQL Server script for the operation.
        /// </summary>
        public string SqlServerScript { get; internal set; }

        /// <summary>
        /// Gets the SQLite script for the operation.
        /// </summary>
        public string SqliteScript { get; internal set; }

        /// <summary>
        /// Gets the MS Access script for the operation.
        /// </summary>
        public string AccessScript { get; internal set; }

        /// <summary>
        /// Gets primary key values.
        /// </summary>
        internal Dictionary<string, object> PrimaryKeyValues => _primaryKeyValues;

        /// <summary>
        /// Indexer to access field values by name.
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Field value</returns>
        public object this[string fieldName]
        {
            get
            {
                return _values.ContainsKey(fieldName) ? _values[fieldName] : null;
            }
        }

        /// <summary>
        /// Indexer to access field values by index.
        /// </summary>
        /// <param name="index">Field index</param>
        /// <returns>Field value</returns>
        public object this[int index]
        {
            get
            {
                var fieldName = _fieldIndexes.FirstOrDefault(kvp => kvp.Value == index).Key;
                return fieldName != null ? _values[fieldName] : null;
            }
        }

        /// <summary>
        /// Gets all field names.
        /// </summary>
        /// <returns>List of field names</returns>
        public List<string> GetFieldNames()
        {
            return _values.Keys.ToList();
        }

        internal void SetValue(string fieldName, object value)
        {
            if (!_fieldIndexes.ContainsKey(fieldName))
                _fieldIndexes[fieldName] = _fieldIndexes.Count;

            _values[fieldName] = value;
        }

        internal void AddPrimaryKeyValue(string fieldName, object value)
        {
            _primaryKeyValues[fieldName] = value;
        }
    }

    #endregion
}
