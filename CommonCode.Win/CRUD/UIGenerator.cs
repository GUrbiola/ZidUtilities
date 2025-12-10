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

            var currentValues = new Dictionary<string, object>();
            foreach (DataColumn col in dataRow.Table.Columns)
            {
                currentValues[col.ColumnName] = dataRow[col];
            }

            return ShowUpdateDialog(currentValues);
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

            var currentValues = new Dictionary<string, object>();
            foreach (DataColumn col in dataRow.Table.Columns)
            {
                currentValues[col.ColumnName] = dataRow[col];
            }

            return ShowDeleteDialog(currentValues);
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
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        var fieldName = row["ColumnName"].ToString();
                        var fieldMetadata = new FieldMetadata
                        {
                            FieldName = fieldName,
                            DataType = (Type)row["DataType"],
                            AllowNull = (bool)row["AllowDBNull"],
                            MaxLength = row["ColumnSize"] != DBNull.Value ? Convert.ToInt32(row["ColumnSize"]) : -1,
                            IsAutoIncrement = row["IsAutoIncrement"] != DBNull.Value && (bool)row["IsAutoIncrement"]
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
            foreach (DataColumn col in dataTable.Columns)
            {
                var fieldMetadata = new FieldMetadata
                {
                    FieldName = col.ColumnName,
                    DataType = col.DataType,
                    AllowNull = col.AllowDBNull,
                    MaxLength = col.MaxLength,
                    IsAutoIncrement = col.AutoIncrement
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
            foreach (var kvp in fields)
            {
                var fieldMetadata = new FieldMetadata
                {
                    FieldName = kvp.Key,
                    DataType = kvp.Value?.GetType() ?? typeof(string),
                    AllowNull = true,
                    MaxLength = -1
                };

                _fields[kvp.Key] = fieldMetadata;
            }
        }

        #endregion

        #region Private Form Generation Methods

        private Form CreateCRUDForm(string title, CRUDOperation operation, Dictionary<string, object> currentValues)
        {
            var form = new Form
            {
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Width = 500,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(10)
            };

            TableLayoutPanel mainPanel;

            // Generate layout based on mode
            switch (_layoutMode)
            {
                case LayoutMode.HtmlTable:
                    mainPanel = CreateHtmlTableLayout(operation, currentValues);
                    break;
                case LayoutMode.ColumnCount:
                    mainPanel = CreateColumnCountLayout(operation, currentValues);
                    break;
                case LayoutMode.Auto:
                default:
                    mainPanel = CreateAutoLayout(operation, currentValues);
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
                            MessageBox.Show($"Password confirmation for '{field.Alias ?? field.FieldName}' does not match.",
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

            ApplyThemeToForm(form);

            return form;
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
            var sortedFields = _fields.Values.OrderBy(f => f.FieldName).ToList();

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
            inputControl.Width = 300 * colSpan;

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
                    Width = 300 * colSpan
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

            // Foreign key - use ComboBox
            if (field.ForeignKeyInfo != null)
            {
                var combo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DisplayMember = "Key",
                    ValueMember = "Value"
                };

                var items = new List<KeyValuePair<string, object>>();
                if (field.AllowNull)
                    items.Add(new KeyValuePair<string, object>("(NULL)", DBNull.Value));

                items.AddRange(field.ForeignKeyInfo.LookupValues);
                combo.DataSource = items;

                if (currentValue != null && currentValue != DBNull.Value)
                {
                    var item = items.FirstOrDefault(i => i.Value?.ToString() == currentValue.ToString());
                    combo.SelectedItem = item;
                }

                control = combo;
            }
            // Boolean - use CheckBox
            else if (field.DataType == typeof(bool))
            {
                var checkbox = new CheckBox
                {
                    Checked = currentValue != null && currentValue != DBNull.Value && Convert.ToBoolean(currentValue)
                };
                control = checkbox;
            }
            // DateTime - use DateTimePicker
            else if (field.DataType == typeof(DateTime))
            {
                var picker = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short
                };

                if (currentValue != null && currentValue != DBNull.Value)
                    picker.Value = Convert.ToDateTime(currentValue);

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
                    DecimalPlaces = (field.DataType == typeof(decimal) || field.DataType == typeof(double) || field.DataType == typeof(float)) ? 2 : 0
                };

                if (currentValue != null && currentValue != DBNull.Value)
                    numeric.Value = Convert.ToDecimal(currentValue);

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
                    Text = currentValue?.ToString() ?? ""
                };

                control = textBox;
            }
            // Default - use TextBox
            else
            {
                var textBox = new TextBox
                {
                    MaxLength = field.MaxLength > 0 ? field.MaxLength : 32767
                };

                // Apply password masking
                if (field.MaskInfo != null && field.MaskInfo.MaskType == MaskType.Password)
                {
                    textBox.PasswordChar = '\u2022';
                }
                // Apply custom masking
                else if (field.MaskInfo != null && field.MaskInfo.MaskType == MaskType.Custom)
                {
                    if (currentValue != null && field.MaskInfo.MaskMethod != null)
                        textBox.Text = field.MaskInfo.MaskMethod(currentValue.ToString());
                }
                else
                {
                    textBox.Text = currentValue?.ToString() ?? "";
                }

                control = textBox;
            }

            // Make control read-only for delete operation or primary key in update
            if (operation == CRUDOperation.Delete || (operation == CRUDOperation.Update && field.IsPrimaryKey))
            {
                if (control is TextBox tb)
                    tb.ReadOnly = true;
                else if (control is ComboBox cb)
                    cb.Enabled = false;
                else if (control is CheckBox chk)
                    chk.Enabled = false;
                else if (control is DateTimePicker dtp)
                    dtp.Enabled = false;
                else if (control is NumericUpDown num)
                    num.Enabled = false;
            }

            return control;
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
                    continue;

                var control = controls[0];
                object value = null;

                if (control is TextBox tb)
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
                    value = chk.Checked;
                }
                else if (control is DateTimePicker dtp)
                {
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
