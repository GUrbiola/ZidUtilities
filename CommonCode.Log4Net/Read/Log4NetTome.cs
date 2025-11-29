using log4net.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZidUtilities.CommonCode;

namespace ZidUtilities.CommonCode.Log4Net.Read
{
    /// <summary>
    /// Represents a collection (tome) of parsed log4net records loaded from a file.
    /// </summary>
    public class Log4NetTome
    {
        public string File { get; set; }
        public string Pattern { get; set; }
        public List<Log4NetRecord> Logs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetTome"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor initializes the <see cref="Logs"/> collection to an empty list
        /// so callers can safely add records or call loading methods without needing to
        /// check for null.
        /// </remarks>
        public Log4NetTome()
        {
            Logs = new List<Log4NetRecord>();
        }

        /// <summary>
        /// Loads and parses a text log file formatted according to a log4net layout pattern.
        /// </summary>
        /// <remarks>
        /// This method reads all lines from the file specified in the <see cref="File"/> property,
        /// converts the configured <see cref="Pattern"/> into a regular expression, and attempts
        /// to parse each non-empty line. Successfully parsed lines are converted into
        /// <see cref="Log4NetRecord"/> instances and added to the <see cref="Logs"/> list.
        /// The method uses the instance properties <see cref="File"/> and <see cref="Pattern"/>
        /// rather than parameters.
        /// </remarks>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the <see cref="File"/> property is null/empty or the file does not exist.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the <see cref="Pattern"/> property is null or empty.
        /// </exception>
        /// <returns>None. The parsed records are appended to the <see cref="Logs"/> property.</returns>
        public void LoadFromFile()
        {
            if (string.IsNullOrEmpty(File) || !System.IO.File.Exists(File))
            {
                throw new FileNotFoundException("Log file not found", File);
            }

            if (string.IsNullOrEmpty(Pattern))
            {
                throw new ArgumentException("Pattern is required for parsing log file");
            }

            Logs.Clear();

            // Convert log4net pattern to regex
            var regex = PatternToRegex(Pattern, out var fieldNames);

            var lines = System.IO.File.ReadAllLines(File);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = regex.Match(line);
                if (match.Success)
                {
                    var record = new Log4NetRecord();

                    for (int i = 0; i < fieldNames.Count; i++)
                    {
                        var fieldName = fieldNames[i];
                        var value = match.Groups[i + 1].Value.Trim();

                        record[fieldName] = value;
                    }

                    Logs.Add(record);
                }
            }
        }

        /// <summary>
        /// Loads and parses a file where each line is a JSON object representing a log record.
        /// </summary>
        /// <remarks>
        /// The method reads all lines from the file specified in the <see cref="File"/> property,
        /// parses each JSON line into a <see cref="JObject"/>, maps known properties to a
        /// <see cref="Log4NetRecord"/>, collects unknown properties into the record's
        /// <c>custom</c> dictionary, and appends the record to <see cref="Logs"/>.
        /// Invalid JSON lines are skipped.
        /// </remarks>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the <see cref="File"/> property is null/empty or the file does not exist.
        /// </exception>
        /// <returns>None. Parsed records are appended to the <see cref="Logs"/> property.</returns>
        public void LoadFromJson()
        {
            if (string.IsNullOrEmpty(File) || !System.IO.File.Exists(File))
            {
                throw new FileNotFoundException("Log file not found", File);
            }

            Logs.Clear();

            var lines = System.IO.File.ReadAllLines(File);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    var jObject = JObject.Parse(line);
                    var record = new Log4NetRecord();

                    // Map JSON properties to Log4NetRecord
                    if (jObject["date"] != null)
                    {
                        if (DateTime.TryParse(jObject["date"].ToString(), out DateTime dateValue))
                        {
                            record.date = dateValue;
                        }
                    }

                    if (jObject["thread"] != null)
                        record.thread = jObject["thread"].ToString();

                    if (jObject["level"] != null)
                    {
                        if (Enum.TryParse<MessageLevel>(jObject["level"].ToString(), true, out MessageLevel levelValue))
                        {
                            record.level = levelValue;
                        }
                    }

                    if (jObject["logger"] != null)
                        record.logger = jObject["logger"].ToString();

                    if (jObject["message"] != null)
                        record.message = jObject["message"].ToString();

                    if (jObject["exception"] != null)
                        record.exception = jObject["exception"].ToString();

                    if (jObject["username"] != null)
                        record.username = jObject["username"].ToString();

                    if (jObject["identity"] != null)
                        record.identity = jObject["identity"].ToString();

                    if (jObject["location"] != null)
                        record.location = jObject["location"].ToString();

                    if (jObject["class"] != null)
                        record._class = jObject["class"].ToString();

                    if (jObject["file"] != null)
                        record.file = jObject["file"].ToString();

                    if (jObject["line"] != null)
                        record.line = jObject["line"].ToString();

                    // Add any other properties to custom dictionary
                    foreach (var prop in jObject.Properties())
                    {
                        var knownFields = new[] { "date", "thread", "level", "logger", "message", "exception",
                                                 "username", "identity", "location", "class", "file", "line", "method" };

                        if (!knownFields.Contains(prop.Name.ToLower()))
                        {
                            record.custom[prop.Name] = prop.Value.ToString();
                        }
                    }

                    Logs.Add(record);
                }
                catch (JsonException)
                {
                    // Skip invalid JSON lines
                    continue;
                }
            }
        }

        /// <summary>
        /// Converts a log4net layout pattern string into a regular expression that can be used
        /// to parse log lines and extract named fields in a fixed order.
        /// </summary>
        /// <param name="pattern">The log4net layout pattern (for example, "%date %-5level %logger - %message").</param>
        /// <param name="fieldNames">
        /// An output list that will be populated with the ordered field names corresponding to
        /// the capturing groups in the returned <see cref="Regex"/>. The list contains names
        /// such as "date", "level", "message", etc.
        /// </param>
        /// <returns>
        /// A <see cref="Regex"/> instance built from the provided pattern. The regex contains
        /// capturing groups in the same order as the entries in <paramref name="fieldNames"/>.
        /// </returns>
        private Regex PatternToRegex(string pattern, out List<string> fieldNames)
        {
            fieldNames = new List<string>();

            // Escape special regex characters except our pattern placeholders
            var regexPattern = Regex.Escape(pattern);

            // Define pattern mappings for log4net conversion specifiers
            var patternMappings = new Dictionary<string, (string regex, string fieldName)>
            {
                { @"%date", (@"(.+?)", "date") },
                { @"%d", (@"(.+?)", "date") },
                { @"%-?\d*level", (@"(\w+)", "level") },
                { @"%p", (@"(\w+)", "level") },
                { @"%message", (@"(.+)", "message") },
                { @"%m", (@"(.+)", "message") },
                { @"%thread", (@"(.+?)", "thread") },
                { @"%t", (@"(.+?)", "thread") },
                { @"%logger", (@"(.+?)", "logger") },
                { @"%c", (@"(.+?)", "logger") },
                { @"%exception", (@"(.+)", "exception") },
                { @"%username", (@"(.+?)", "username") },
                { @"%u", (@"(.+?)", "username") },
                { @"%identity", (@"(.+?)", "identity") },
                { @"%location", (@"(.+?)", "location") },
                { @"%l", (@"(.+?)", "location") },
                { @"%file", (@"(.+?)", "file") },
                { @"%F", (@"(.+?)", "file") },
                { @"%line", (@"(\d+)", "line") },
                { @"%L", (@"(\d+)", "line") },
                { @"%class", (@"(.+?)", "class") },
                { @"%C", (@"(.+?)", "class") },
                { @"%method", (@"(.+?)", "method") },
                { @"%M", (@"(.+?)", "method") },
                { @"%newline", (@"\r?\n?", "") },
                { @"%n", (@"\r?\n?", "") }
            };

            // Replace each pattern with its regex equivalent
            foreach (var mapping in patternMappings.OrderByDescending(x => x.Key.Length))
            {
                var escapedKey = Regex.Escape(mapping.Key);
                if (regexPattern.Contains(escapedKey))
                {
                    regexPattern = Regex.Replace(regexPattern, escapedKey, mapping.Value.regex);
                    if (!string.IsNullOrEmpty(mapping.Value.fieldName))
                    {
                        fieldNames.Add(mapping.Value.fieldName);
                    }
                }
            }

            return new Regex(regexPattern);
        }

        /// <summary>
        /// Convenience method that returns a DataTable with all standard fields (and optional custom columns).
        /// </summary>
        /// <param name="includeCustomColumns">If true, collect all distinct keys from <see cref="Log4NetRecord.custom"/> and add them as columns.</param>
        /// <param name="tableName">Optional table name; if null, the file name or "Logs" will be used.</param>
        /// <returns>
        /// A <see cref="System.Data.DataTable"/> populated with all standard fields and optional custom fields.
        /// The known fields included are: date, thread, level, logger, message, exception, username, identity, location, class, method, file, line.
        /// </returns>
        public System.Data.DataTable ToDataTableAllFields(bool includeCustomColumns = true, string tableName = null)
        {
            return ToDataTable(new string[]
            {
                "date", "thread", "level", "logger", "message", "exception",
                "username", "identity", "location", "class", "method", "file", "line"
            }, includeCustomColumns, tableName);
        }

        /// <summary>
        /// Converts the current `Logs` collection into a `System.Data.DataTable`.
        /// Known fields are added as typed columns; optional custom properties are added as string columns.
        /// </summary>
        /// <param name="columns">A list of field names to include in the resulting table. Known fields include: date, thread, level, logger, message, exception, username, identity, location, class, method, file, line.</param>
        /// <param name="includeCustomColumns">If true, collect all distinct keys from <see cref="Log4NetRecord.custom"/> and add them as columns.</param>
        /// <param name="tableName">Optional table name; if null, the file name or "Logs" will be used.</param>
        /// <returns>A populated <see cref="System.Data.DataTable"/> representing the logs. The <c>date</c> column (if included) has type <see cref="DateTime"/>; all other columns are strings.</returns>
        public System.Data.DataTable ToDataTable(string[] columns, bool includeCustomColumns = true, string tableName = null)
        {
            if (columns == null || columns.Length == 0)
                throw new ArgumentException("List of fields to include is required", nameof(columns));

            var columnsAvailable = new HashSet<string>(new[]
            {
                "date", "thread", "level", "logger", "message", "exception",
                "username", "identity", "location", "class", "method", "file", "line"
            }, StringComparer.OrdinalIgnoreCase);

            var dtName = !string.IsNullOrEmpty(tableName) ? tableName :
                         (!string.IsNullOrEmpty(File) ? System.IO.Path.GetFileNameWithoutExtension(File) : "Logs");
            var table = new System.Data.DataTable(dtName);

            // Track added columns to avoid duplicates (case-insensitive)
            var addedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Known columns
            foreach (var col in columns)
            {
                if (!columnsAvailable.Contains(col))
                    continue;

                if (addedColumns.Contains(col))
                    continue;

                if (col.Equals("date", StringComparison.OrdinalIgnoreCase))
                    table.Columns.Add("date", typeof(DateTime));
                else
                    table.Columns.Add(col, typeof(string));

                addedColumns.Add(col);
            }

            // Collect custom keys if requested (case-insensitive)
            HashSet<string> customKeysSet = null;
            if (includeCustomColumns)
            {
                var customKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (Logs != null)
                {
                    foreach (var rec in Logs)
                    {
                        if (rec?.custom == null) continue;
                        foreach (var k in rec.custom.Keys)
                        {
                            if (!string.IsNullOrEmpty(k))
                                customKeys.Add(k);
                        }
                    }
                }

                customKeysSet = customKeys;

                foreach (var key in customKeysSet)
                {
                    if (!table.Columns.Contains(key) && !addedColumns.Contains(key))
                        table.Columns.Add(key, typeof(string));
                }
            }

            if (Logs == null || Logs.Count == 0)
                return table;

            var tableFields = table.Columns.Cast<System.Data.DataColumn>().Select(c => c.ColumnName).ToList();

            // Populate rows
            foreach (var rec in Logs)
            {
                var row = table.NewRow();
                foreach (var field in tableFields)
                {
                    if (field.Equals("date", StringComparison.OrdinalIgnoreCase))
                    {
                        if (rec == null || rec.date == DateTime.MinValue)
                            row[field] = DBNull.Value;
                        else
                            row[field] = rec.date;
                    }
                    else if (includeCustomColumns && customKeysSet != null && customKeysSet.Contains(field))
                    {
                        object value = DBNull.Value;
                        if (rec?.custom != null && rec.custom.TryGetValue(field, out var v))
                        {
                            // keep empty string if present; use DBNull only when missing
                            value = v ?? (object)string.Empty;
                        }
                        row[field] = value ?? DBNull.Value;
                    }
                    else
                    {
                        string val = rec[field];
                        if (val == null)
                            row[field] = DBNull.Value;
                        else
                            row[field] = val; // Update here to use 'val' instead of 'rec[field]'
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }

    }
}
