using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Data.SQLite;

namespace ZidUtilities.TesterWin.DataAccess
{
    /// <summary>
    /// Delegate to notify start/finish of a query processing operation.
    /// </summary>
    /// <param name="Query">The SQL query being processed.</param>
    /// <param name="Time">The time of the event.</param>
    public delegate void ProcessingQuery(string Query, DateTime Time);

    /// <summary>
    /// Provides helper methods to execute SQLite commands using ADO.NET,
    /// offering sync and async execution, transaction support, and simple logging.
    /// </summary>
    public partial class SqliteConnector
    {
        #region Variables
        private DateTime _startTime, _endingTime;
        SQLiteCommand cmd = null;
        SQLiteDataAdapter da = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the number of rows affected by the last non-query command.
        /// </summary>
        public int RowsAffected { get; set; }

        /// <summary>
        /// Gets or sets the number of rows read by the last query execution.
        /// </summary>
        public int RowsRead { get; set; }

        /// <summary>
        /// Gets or sets the last operation message. "OK" indicates success; otherwise contains an error message.
        /// </summary>
        public string LastMessage { get; set; }

        /// <summary>
        /// Gets or sets the underlying SQLite connection used by commands.
        /// </summary>
        public SQLiteConnection Connection { get; set; }

        /// <summary>
        /// Gets a new SQLite connection instance using the current connection string.
        /// </summary>
        public SQLiteConnection NewConnection
        {
            get
            {
                return new SQLiteConnection(ConnectionString);
            }
        }

        /// <summary>
        /// Gets or sets the connection string used to establish SQLite connections.
        /// </summary>
        public string ConnectionString
        {
            get { return Connection.ConnectionString; }
            set { Connection.ConnectionString = value; }
        }

        /// <summary>
        /// Gets the time span representing the last execution duration.
        /// </summary>
        public TimeSpan ExecutionLapse
        {
            get
            {
                return _endingTime.Subtract(_startTime);
            }
        }

        /// <summary>
        /// Gets a value indicating whether any execution is currently in progress.
        /// </summary>
        public bool OnExecution { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating the execution state. Setting true starts timing; false stops timing.
        /// </summary>
        public bool Executing
        {
            get
            {
                return OnExecution;
            }
            set
            {
                if (value)
                {
                    _startTime = DateTime.Now;
                    OnExecution = true;
                }
                else
                {
                    _endingTime = DateTime.Now;
                    OnExecution = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the command timeout (in seconds). Zero means default.
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// Gets a value indicating whether the last operation produced an error.
        /// </summary>
        public bool Error
        {
            get
            {
                return LastMessage != "OK";
            }
        }

        /// <summary>
        /// Gets or sets the last thrown general exception.
        /// </summary>
        public Exception LastException { get; set; }

        /// <summary>
        /// Gets or sets the last thrown SQLite exception.
        /// </summary>
        public SQLiteException LastSqliteException { get; set; }

        /// <summary>
        /// Gets the SQLite database file path from the current connection.
        /// </summary>
        public string Server
        {
            get
            {
                if (Connection != null)
                    return Connection.DataSource;
                return "";
            }
        }

        /// <summary>
        /// Gets the current database name from the connection (for SQLite, this is the file path).
        /// </summary>
        public string DataBase
        {
            get
            {
                if (Connection != null)
                    return Connection.Database;
                return "";
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SqliteConnector with an empty connection.
        /// </summary>
        public SqliteConnector()
        {
            TimeOut = 0;
            Connection = new SQLiteConnection();

            LastMessage = "OK";
            cmd = new SQLiteCommand();
            da = new SQLiteDataAdapter();
        }

        /// <summary>
        /// Initializes a new instance of the SqliteConnector with the provided connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to use.</param>
        public SqliteConnector(string connectionString)
        {
            TimeOut = 0;
            Connection = new SQLiteConnection(connectionString);

            LastMessage = "OK";
            cmd = new SQLiteCommand();
            da = new SQLiteDataAdapter();
        }

        /// <summary>
        /// Initializes a new instance of the SqliteConnector with the provided SQLiteConnection.
        /// </summary>
        /// <param name="connection">The SQLite connection to use. If null, a new connection is created.</param>
        public SqliteConnector(SQLiteConnection connection)
        {
            TimeOut = 0;
            Connection = (connection == null ? new SQLiteConnection() : connection);

            LastMessage = "OK";
            cmd = new SQLiteCommand();
            da = new SQLiteDataAdapter();
        }
        #endregion

        /// <summary>
        /// Tests the ability to open and close the current connection.
        /// </summary>
        /// <returns>True if connection open/close succeeds; otherwise false.</returns>
        public bool TestConnection()
        {
            try
            {
                Connection.Open();
                LastMessage = "OK";
            }
            catch (Exception ex)
            {
                LastMessage = ex.Message;
                LastException = ex;
                return false;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
            LastMessage = "OK";
            return true;
        }

        /// <summary>
        /// Begins a transaction on the current connection and assigns it to the command.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when connection or command is invalid.</exception>
        private void BeginTransaction()
        {
            if (Connection != null && cmd != null)
            {
                cmd.Transaction = Connection.BeginTransaction();
            }
            else
            {
                throw new InvalidOperationException("There must be a valid connection before starting a transaction.");
            }
        }

        /// <summary>
        /// Commits the current transaction associated with the command.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no transaction is in progress.</exception>
        private void CommitTransaction()
        {
            if (cmd != null && cmd.Transaction != null)
            {
                try
                {
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new InvalidOperationException("A transaction must be in progress before commiting the changes.");
            }
        }

        /// <summary>
        /// Rolls back the current transaction associated with the command.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no transaction is in progress.</exception>
        private void RollbackTransaction()
        {

            if (cmd != null && cmd.Transaction != null)
            {
                try
                {
                    cmd.Transaction.Rollback();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new InvalidOperationException("A transaction must be in progress before rolling back the changes.");
            }
        }

        /// <summary>
        /// Executes a non-query SQL command (INSERT/UPDATE/DELETE).
        /// </summary>
        /// <param name="sql">The SQL text to execute.</param>
        /// <param name="autoTransact">If true, wraps execution in a transaction which is committed on success or rolled back on failure.</param>
        /// <returns>0 on success; -1 on failure.</returns>
        public SqliteResponse<int> ExecuteNonQuery(string sql, Dictionary<string, string> ps = null, bool autoTransact = false)
        {
            int result = 0;
            SqliteResponse<int> back = null;
            try
            {
                cmd.Connection = Connection;
                cmd.CommandText = sql;
                cmd.CommandTimeout = TimeOut;

                if (ps != null && ps.Count > 0)
                {
                    cmd.Parameters.Clear();
                    foreach (var p in ps)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                }

                Executing = true;
                cmd.Connection.Open();
                if (autoTransact)
                    BeginTransaction();

                cmd.CommandType = CommandType.Text;

                RowsAffected = cmd.ExecuteNonQuery();
                if (autoTransact)
                    CommitTransaction();

                back = SqliteResponse<int>.Successful(result);
                LastMessage = "OK";
            }
            catch (SQLiteException sqlex)
            {
                if (autoTransact)
                {
                    RollbackTransaction();
                }
                LastMessage = sqlex.Message;
                LastSqliteException = sqlex;
                result = -1;
                back = SqliteResponse<int>.Failure(sqlex);
            }
            catch (Exception ex)
            {
                if (autoTransact)
                {
                    RollbackTransaction();
                }
                LastMessage = ex.Message;
                LastException = ex;
                result = -1;
                back = SqliteResponse<int>.Failure(ex);
            }
            finally
            {
                if (cmd != null)
                {
                    if (cmd.Connection.State == ConnectionState.Open)
                        cmd.Connection.Close();
                }
                Executing = false;
            }
            return back;
        }

        /// <summary>
        /// Executes a SQL command and returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="sql">The SQL text to execute.</param>
        /// <param name="autoTransact">If true, wraps execution in a transaction which is committed on success or rolled back on failure.</param>
        /// <returns>The scalar value returned by the query; null if failed.</returns>
        public SqliteResponse<object> ExecuteScalar(string sql, Dictionary<string, string> ps = null, bool autoTransact = false)
        {
            object result = null;
            SqliteResponse<object> back = null;
            try
            {
                cmd.Connection = Connection;
                cmd.CommandText = sql;
                cmd.CommandTimeout = TimeOut;

                if (ps != null && ps.Count > 0)
                {
                    cmd.Parameters.Clear();
                    foreach (var p in ps)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                }

                Executing = true;
                cmd.Connection.Open();
                if (autoTransact)
                    BeginTransaction();

                cmd.CommandType = CommandType.Text;
                RowsRead = 0;
                result = cmd.ExecuteScalar();
                RowsRead++;
                if (autoTransact)
                    CommitTransaction();

                back = SqliteResponse<object>.Successful(result);

                LastMessage = "OK";
            }
            catch (SQLiteException sqlex)
            {
                if (autoTransact)
                    RollbackTransaction();

                LastMessage = sqlex.Message;
                LastSqliteException = sqlex;
                result = null;

                back = SqliteResponse<object>.Failure(sqlex);
            }
            catch (Exception ex)
            {
                if (autoTransact)
                    RollbackTransaction();


                LastMessage = ex.Message;
                LastException = ex;
                result = null;

                back = SqliteResponse<object>.Failure(ex);
            }
            finally
            {
                if (cmd != null)
                {
                    if (cmd.Connection.State == ConnectionState.Open)
                        cmd.Connection.Close();
                }
                Executing = false;
            }
            return back;
        }

        /// <summary>
        /// Executes a SQL query and returns a DataSet containing one or more result tables.
        /// </summary>
        /// <param name="sql">The SQL text to execute.</param>
        /// <param name="autoTransact">If true, wraps execution in a transaction which is committed on success or rolled back on failure.</param>
        /// <returns>A DataSet with the results; null if failed.</returns>
        public SqliteResponse<DataSet> ExecuteDataSet(string sql, Dictionary<string, string> ps = null, bool autoTransact = false)
        {
            DataSet result = new DataSet();
            SqliteResponse<DataSet> back = null;
            try
            {
                cmd.Connection = Connection;
                cmd.CommandText = sql;
                cmd.CommandTimeout = TimeOut;

                if (ps != null && ps.Count > 0)
                {
                    cmd.Parameters.Clear();
                    foreach (var p in ps)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                }

                da = new SQLiteDataAdapter(cmd);
                Executing = true;

                cmd.CommandType = CommandType.Text;

                da.SelectCommand.Connection.Open();
                if (autoTransact)
                    BeginTransaction();

                RowsRead = 0;
                da.Fill(result);
                foreach (DataTable tab in result.Tables)
                    if (tab != null && tab.Rows != null)
                        RowsRead = tab.Rows.Count;

                if (autoTransact)
                    CommitTransaction();

                back = SqliteResponse<DataSet>.Successful(result);

                LastMessage = "OK";
            }
            catch (SQLiteException sqlex)
            {
                if (autoTransact)
                    RollbackTransaction();

                LastMessage = sqlex.Message;
                LastSqliteException = sqlex;
                result = null;
                back = SqliteResponse<DataSet>.Failure(sqlex);
            }
            catch (Exception ex)
            {
                if (autoTransact)
                    RollbackTransaction();

                LastMessage = ex.Message;
                LastException = ex;
                result = null;
                back = SqliteResponse<DataSet>.Failure(ex);
            }
            finally
            {
                if (da.SelectCommand != null)
                {
                    if (da.SelectCommand.Connection.State == ConnectionState.Open)
                        da.SelectCommand.Connection.Close();
                }
                Executing = false;
            }
            return back;
        }

        /// <summary>
        /// Executes a SQL query and returns the first table in the result.
        /// </summary>
        /// <param name="sql">The SQL text to execute.</param>
        /// <param name="autoTransact">If true, wraps execution in a transaction which is committed on success or rolled back on failure.</param>
        /// <param name="tableName">Optional name to assign to the returned DataTable.</param>
        /// <returns>The first DataTable from the result; null if failed or no tables.</returns>
        public SqliteResponse<DataTable> ExecuteTable(string sql, Dictionary<string, string> ps = null, bool autoTransact = false, string tableName = "")
        {
            SqliteResponse<DataSet> tmp = null;
            SqliteResponse<DataTable> back = null;

            tmp = ExecuteDataSet(sql, ps, autoTransact);
            if (tmp.IsOK)
            {
                if (tmp?.Result != null && tmp.Result.Tables.Count > 0)
                {
                    DataTable table = tmp.Result.Tables[0];
                    if (!String.IsNullOrEmpty(tableName))
                        table.TableName = tableName; // Use 'table' instead of 'tmp.Result.Tables[0]'

                    back = SqliteResponse<DataTable>.Successful(table);
                }
                else
                {
                    back = SqliteResponse<DataTable>.Successful(new DataTable());
                }
                LastMessage = "OK";
            }
            else
            {
                back = new SqliteResponse<DataTable>();
                back.Errors.AddRange(tmp.Errors);
                LastMessage = "Errors occurred during execution.";
            }

            return back;
        }

        /// <summary>
        /// Executes a SQL query expected to return a single column and maps it to a list of strings.
        /// </summary>
        /// <param name="sql">The SQL text to execute.</param>
        /// <param name="autoTransact">If true, wraps execution in a transaction which is committed on success or rolled back on failure.</param>
        /// <returns>A list of string values from the first column; empty list on failure or no rows.</returns>
        public SqliteResponse<List<string>> ExecuteColumn(string sql, Dictionary<string, string> ps = null, bool autoTransact = false)
        {
            SqliteResponse<List<string>> back = new SqliteResponse<List<string>>();
            SqliteResponse<DataTable> aux = ExecuteTable(sql, ps, autoTransact);

            if (aux.IsOK)
            {
                if (aux?.Result != null)
                {
                    DataTable table = (DataTable)aux.Result;
                    List<string> list = new List<string>();
                    int rc = table.Rows.Count;

                    for (int i = 0; i < rc; i++)
                        list.Add(table.Rows[i][0].ToString());

                    back = SqliteResponse<List<string>>.Successful(list);
                }
                else
                {
                    back = SqliteResponse<List<string>>.Successful(new List<string>());
                }
                LastMessage = "OK";
            }
            else
            {
                back = new SqliteResponse<List<string>>();
                back.Errors.AddRange(aux.Errors);
                LastMessage = "Errors occurred during execution.";
            }

            return back;
        }

        /// <summary>
        /// Creates a table in SQLite based on the provided DataTable schema.
        /// </summary>
        /// <param name="table">The DataTable whose schema is used to generate a CREATE TABLE statement.</param>
        public void CreateTableInSQL(DataTable table)
        {
            string sql;
            sql = GetTableScript(table);
            ExecuteScalar(sql);
        }

        /// <summary>
        /// Builds a SQLite CREATE TABLE script from a DataTable schema,
        /// including default values and primary key constraints.
        /// </summary>
        /// <param name="table">The DataTable to inspect.</param>
        /// <returns>A string containing the SQLite CREATE TABLE statement.</returns>
        public static string GetTableScript(DataTable table)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("CREATE TABLE IF NOT EXISTS [{0}] (", table.TableName);

            for (int i = 0; i < table.Columns.Count; i++)
            {
                bool isNumeric = false;
                bool isPrimaryKey = table.PrimaryKey.Length > 0 && table.PrimaryKey.Contains(table.Columns[i]);

                sql.AppendFormat("\n\t[{0}]", table.Columns[i].ColumnName);

                // SQLite type mapping
                switch (table.Columns[i].DataType.ToString())
                {
                    case "System.Boolean":
                        sql.AppendFormat(" INTEGER"); // SQLite uses INTEGER for boolean (0/1)
                        break;
                    case "System.Byte":
                        sql.AppendFormat(" INTEGER");
                        isNumeric = true;
                        break;
                    case "System.Char":
                        sql.AppendFormat(" TEXT");
                        break;
                    case "System.Int16":
                        sql.Append(" INTEGER");
                        isNumeric = true;
                        break;
                    case "System.Int32":
                        sql.Append(" INTEGER");
                        isNumeric = true;
                        break;
                    case "System.Int64":
                        sql.Append(" INTEGER");
                        isNumeric = true;
                        break;
                    case "System.DateTime":
                        sql.Append(" TEXT"); // SQLite stores datetime as TEXT in ISO8601 format
                        break;
                    case "System.Single":
                        sql.Append(" REAL");
                        isNumeric = true;
                        break;
                    case "System.Double":
                        sql.Append(" REAL");
                        isNumeric = true;
                        break;
                    case "System.Decimal":
                        sql.AppendFormat(" NUMERIC"); // SQLite NUMERIC for decimal values
                        isNumeric = true;
                        break;
                    default:
                    case "System.String":
                        sql.AppendFormat(" TEXT");
                        break;
                }

                // Handle AutoIncrement - SQLite uses INTEGER PRIMARY KEY AUTOINCREMENT
                if (table.Columns[i].AutoIncrement)
                {
                    sql.Append(" PRIMARY KEY AUTOINCREMENT");
                }
                else
                {
                    // Handle default values - in SQLite, defaults must be in the CREATE TABLE statement
                    if (table.Columns[i].DefaultValue != null && !String.IsNullOrEmpty(table.Columns[i].DefaultValue.ToString()))
                    {
                        if (isNumeric)
                        {
                            sql.AppendFormat(" DEFAULT {0}", table.Columns[i].DefaultValue);
                        }
                        else if (table.Columns[i].DataType == typeof(DateTime))
                        {
                            // For DateTime, try to get SQL-compliant default from caption
                            try
                            {
                                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                                xml.LoadXml(table.Columns[i].Caption);
                                sql.AppendFormat(" DEFAULT {0}", xml.GetElementsByTagName("defaultValue")[0].InnerText);
                            }
                            catch
                            {
                                // If no XML caption, use CURRENT_TIMESTAMP for datetime
                                sql.Append(" DEFAULT CURRENT_TIMESTAMP");
                            }
                        }
                        else
                        {
                            sql.AppendFormat(" DEFAULT '{0}'", table.Columns[i].DefaultValue.ToString().Replace("'", "''"));
                        }
                    }
                }

                // Handle NOT NULL constraint
                if (!table.Columns[i].AllowDBNull)
                {
                    sql.Append(" NOT NULL");
                }

                sql.Append(",");
            }

            // Add PRIMARY KEY constraint for composite keys or non-autoincrement single keys
            if (table.PrimaryKey.Length > 0)
            {
                // Check if this is not an autoincrement column (already handled above)
                bool hasAutoIncrement = false;
                foreach (var pkCol in table.PrimaryKey)
                {
                    if (pkCol.AutoIncrement)
                    {
                        hasAutoIncrement = true;
                        break;
                    }
                }

                // Only add PRIMARY KEY constraint if not autoincrement or if composite key
                if (!hasAutoIncrement || table.PrimaryKey.Length > 1)
                {
                    StringBuilder primaryKeySql = new StringBuilder();
                    primaryKeySql.Append("\n\tPRIMARY KEY (");

                    for (int i = 0; i < table.PrimaryKey.Length; i++)
                    {
                        primaryKeySql.AppendFormat("[{0}]", table.PrimaryKey[i].ColumnName);
                        if (i < table.PrimaryKey.Length - 1)
                            primaryKeySql.Append(",");
                    }

                    primaryKeySql.Append(")");
                    sql.Append(primaryKeySql);
                }
                else
                {
                    // Remove the trailing comma
                    sql.Remove(sql.Length - 1, 1);
                }
            }
            else
            {
                // Remove the trailing comma
                sql.Remove(sql.Length - 1, 1);
            }

            sql.Append("\n);");

            return sql.ToString();
        }
    }

    /// <summary>
    /// Generic response container supporting result lists, single result, error collection, and status messages.
    /// </summary>
    /// <typeparam name="T">Type of the result.</typeparam>
    public class SqliteResponse<T>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the time the response was produced.
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Gets the execution time in a formatted string (yyyy-MM-dd HH:mm:ss.fff).
        /// </summary>
        public string ExecutionTimeString
        {
            get
            {
                return ExecutionTime.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        private List<T> _Results;

        /// <summary>
        /// Gets or sets the list of results.
        /// </summary>
        public List<T> Results { get { return _Results; } set { _Results = value; } }

        /// <summary>
        /// Gets or sets the first result item. Setting will replace the Results list with a single item.
        /// </summary>
        public T Result
        {
            get
            {// Return the first result or default if no results
                if (_Results == null || Results.Count == 0)
                    return default(T);
                return Results[0];
            }
            set
            {
                if (_Results != null)
                    Results.Clear();
                else if (_Results == null)
                    _Results = new List<T>();
                Results.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of errors associated with the response.
        /// </summary>
        public List<ErrorOnResponse> Errors { get; set; }

        private string _Message = String.Empty;

        /// <summary>
        /// Gets or sets the status message (e.g., "Success" or an error description).
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        /// <summary>
        /// Gets a value indicating whether an error occurred based on errors list and message.
        /// </summary>
        public bool ErrorOccured
        {
            get
            {
                if (Errors == null || Errors.Count == 0)
                    return false;
                return !Message.Equals("Success", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets a value indicating successful response (no error occurred).
        /// </summary>
        public bool IsOK { get { return !ErrorOccured; } }

        /// <summary>
        /// Gets a value indicating failure response (an error occurred).
        /// </summary>
        public bool IsFailure { get { return ErrorOccured; } }
        #endregion

        #region Class contructor
        /// <summary>
        /// Initializes a new instance of SqliteResponse with empty results and errors, and sets ExecutionTime.
        /// </summary>
        public SqliteResponse()
        {
            Errors = new List<ErrorOnResponse>();
            Results = new List<T>();
            ExecutionTime = DateTime.Now;
        }
        #endregion

        #region Indexers of the class, by field number(position on the table) and by field name(case insensitive)
        /// <summary>
        /// Gets a field by name ("results", "result", "message") in a case-insensitive manner.
        /// </summary>
        /// <param name="fieldName">The field name to access.</param>
        /// <returns>The value of the field or null if not found.</returns>
        public object this[string fieldName]
        {
            get
            {
                string aux = fieldName.ToLower();
                switch (aux)
                {
                    case "results":
                        return Results;
                    case "result":
                        return Result;
                    case "message":
                        return Message;
                }
                return null;
            }
            set
            {
                string aux = fieldName.ToLower();
                switch (aux)
                {
                    case "results":
                        if (value is List<T>)
                        {
                            Results = (List<T>)value;
                        }
                        else
                        {
                            try
                            {
                                Results = (List<T>)value;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Data type mismatch for the field: Result - index: Results", ex);
                            }
                        }
                        break;
                    case "result":
                        if (value is T)
                        {
                            Result = (T)value;
                        }
                        else
                        {
                            try
                            {
                                Result = (T)value;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Data type mismatch for the field: Result - index: Result", ex);
                            }
                        }
                        break;
                    case "message":
                        if (value is string)
                        {
                            Message = (string)value;
                        }
                        else
                        {
                            try
                            {
                                Message = (string)value;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Data type mismatch for the field: Message - index: Message", ex);
                            }
                        }
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Adds a single result to the Results list.
        /// </summary>
        /// <param name="result">The result to add.</param>
        public void AddResult(T result)
        {
            if (this.Results == null)
                this.Results = new List<T>();
            this.Results.Add(result);
            if (String.IsNullOrEmpty(this.Message))
                this.Message = "Success";
        }

        /// <summary>
        /// Adds a range of results to the Results list.
        /// </summary>
        /// <param name="results">The list of results to add.</param>
        public void AddResults(List<T> results)
        {
            if (this.Results == null)
                this.Results = new List<T>();
            this.Results.AddRange(results);
            if (String.IsNullOrEmpty(this.Message))
                this.Message = "Success";

        }

        /// <summary>
        /// Marks the response as failure and adds a default error using the current message.
        /// </summary>
        /// <returns>The same response instance for chaining.</returns>
        public SqliteResponse<T> Fail()
        {
            this.ExecutionTime = DateTime.Now;
            this.Message = "An error occurred while processing your request.";
            this.Errors.Add(new ErrorOnResponse(this.Message));
            return this;
        }

        /// <summary>
        /// Marks the response as failure with a specific message and optional exception.
        /// </summary>
        /// <param name="message">Failure message.</param>
        /// <param name="ex">Optional exception.</param>
        /// <returns>The same response instance for chaining.</returns>
        public SqliteResponse<T> Fail(string message, Exception ex = null)
        {
            this.ExecutionTime = DateTime.Now;
            this.Message = message;
            if (ex != null)
            {
                this.Errors.Add(new ErrorOnResponse(message, ex));
            }
            else
            {
                this.Errors.Add(new ErrorOnResponse(message));
            }
            return this;
        }

        /// <summary>
        /// Marks the response as failure using the provided exception.
        /// </summary>
        /// <param name="ex">Exception causing the failure.</param>
        /// <returns>The same response instance for chaining.</returns>
        public SqliteResponse<T> Fail(Exception ex)
        {
            this.ExecutionTime = DateTime.Now;
            this.Message = "An error occurred while processing your request.";
            if (ex == null)
            {
                Fail("An error occurred while processing your request.");
            }
            else
            {
                this.Errors.Add(new ErrorOnResponse(this.Message, ex));
            }
            return this;
        }

        /// <summary>
        /// Marks the response as successful, setting the message.
        /// </summary>
        /// <param name="message">Optional success message (default "Success").</param>
        /// <returns>The same response instance for chaining.</returns>
        public SqliteResponse<T> Success()
        {
            this.ExecutionTime = DateTime.Now;
            this.Message = "Success";
            return this;
        }

        /// <summary>
        /// Marks the response as successful with a single result and message.
        /// </summary>
        /// <param name="result">The result to store.</param>
        /// <param name="message">Optional success message.</param>
        /// <returns>The same response instance for chaining.</returns>
        public SqliteResponse<T> Success(T result)
        {
            this.ExecutionTime = DateTime.Now;
            this.Result = result;
            this.Message = "Success";
            return this;
        }

        /// <summary>
        /// Marks the response as successful with a list of results and message.
        /// </summary>
        /// <param name="results">The results to store.</param>
        /// <param name="message">Optional success message.</param>
        /// <returns>The same response instance for chaining.</returns>
        public SqliteResponse<T> Success(List<T> results)
        {
            this.ExecutionTime = DateTime.Now;
            this.Results = results;
            this.Message = "Success";
            return this;
        }

        /// <summary>
        /// Creates a failure response with specified message and optional exception.
        /// </summary>
        /// <param name="message">Failure message.</param>
        /// <param name="ex">Optional exception.</param>
        /// <returns>A new failure SqliteResponse instance.</returns>
        public static SqliteResponse<T> Failure(string message, Exception ex = null)
        {
            return new SqliteResponse<T>().Fail(message, ex);
        }

        /// <summary>
        /// Creates a failure response with optional exception.
        /// </summary>
        /// <param name="ex">Optional exception.</param>
        /// <returns>A new failure SqliteResponse instance.</returns>
        public static SqliteResponse<T> Failure(Exception ex = null)
        {
            return new SqliteResponse<T>().Fail(ex);
        }

        /// <summary>
        /// Creates a success response with a list of results and optional message.
        /// </summary>
        /// <param name="results">Results list.</param>
        /// <param name="message">Optional success message.</param>
        /// <returns>A new success SqliteResponse instance.</returns>
        public static SqliteResponse<T> Successful(List<T> results)
        {
            return new SqliteResponse<T>().Success(results);
        }

        /// <summary>
        /// Creates a success response with a single result and optional message.
        /// </summary>
        /// <param name="result">Single result.</param>
        /// <param name="message">Optional success message.</param>
        /// <returns>A new success SqliteResponse instance.</returns>
        public static SqliteResponse<T> Successful(T result)
        {
            return new SqliteResponse<T>().Success(result);
        }

        /// <summary>
        /// Creates a success response with no results and optional message.
        /// </summary>
        /// <param name="message">Optional success message.</param>
        /// <returns>A new success SqliteResponse instance.</returns>
        public static SqliteResponse<T> Successful()
        {
            return new SqliteResponse<T>().Success();
        }
    }

    /// <summary>
    /// Represents an error entry in a response, capturing execution time, message and exception details.
    /// </summary>
    public class ErrorOnResponse
    {
        /// <summary>
        /// Gets or sets the time when the error was recorded.
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the associated exception, if any.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Initializes a new ErrorOnResponse with a default message.
        /// </summary>
        public ErrorOnResponse()
        {
            this.ExecutionTime = DateTime.Now;
            this.Message = "An error occurred while processing your request.";
        }

        /// <summary>
        /// Initializes a new ErrorOnResponse with a specific message and optional exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="Ex">Optional exception associated with the error.</param>
        public ErrorOnResponse(string message, Exception Ex = null)
        {
            this.ExecutionTime = DateTime.Now;
            this.Message = message;
            this.Exception = Ex == null ? new Exception(this.Message) : Ex;
        }
    }
}
