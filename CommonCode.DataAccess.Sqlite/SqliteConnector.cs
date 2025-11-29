using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Threading;

namespace CommonCode.DataAccess.Sqlite
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
        /// Gets or sets the current transaction associated with the command.
        /// </summary>
        public SQLiteTransaction Transaction
        {
            get { return cmd == null ? null : cmd.Transaction; }
            set { cmd.Transaction = value; }
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
        public int ExecuteNonQuery(string sql, bool autoTransact = false)
        {
            int result = 0;
            try
            {
                cmd.Connection = Connection;
                cmd.CommandText = sql;
                cmd.CommandTimeout = TimeOut;
                Executing = true;
                cmd.Connection.Open();
                if (autoTransact)
                {
                    BeginTransaction();
                }
                cmd.CommandType = CommandType.Text;

                RowsAffected = cmd.ExecuteNonQuery();
                if (autoTransact)
                {
                    CommitTransaction();
                }
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
            return result;
        }

        /// <summary>
        /// Executes a SQL command and returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="sql">The SQL text to execute.</param>
        /// <param name="autoTransact">If true, wraps execution in a transaction which is committed on success or rolled back on failure.</param>
        /// <returns>The scalar value returned by the query; null if failed.</returns>
        public object ExecuteScalar(string sql, bool autoTransact = false)
        {
            object result = null;
            try
            {
                cmd.Connection = Connection;
                cmd.CommandText = sql;
                cmd.CommandTimeout = TimeOut;
                Executing = true;
                cmd.Connection.Open();
                if (autoTransact)
                {
                    BeginTransaction();
                }

                cmd.CommandType = CommandType.Text;
                RowsRead = 0;
                result = cmd.ExecuteScalar();
                RowsRead++;
                if (autoTransact)
                {
                    CommitTransaction();
                }
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
                result = null;
            }
            catch (Exception ex)
            {
                if (autoTransact)
                {
                    RollbackTransaction();
                }
                LastMessage = ex.Message;
                LastException = ex;
                result = null;
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
            return result;
        }

        /// <summary>
        /// Executes a SQL query and returns a DataSet containing one or more result tables.
        /// </summary>
        /// <param name="sql">The SQL text to execute.</param>
        /// <param name="autoTransact">If true, wraps execution in a transaction which is committed on success or rolled back on failure.</param>
        /// <returns>A DataSet with the results; null if failed.</returns>
        public DataSet ExecuteDataSet(string sql, bool autoTransact = false)
        {
            DataSet result = new DataSet();
            try
            {
                cmd.Connection = Connection;
                cmd.CommandText = sql;
                cmd.CommandTimeout = TimeOut;
                da = new SQLiteDataAdapter(cmd);
                Executing = true;

                cmd.CommandType = CommandType.Text;

                da.SelectCommand.Connection.Open();
                if (autoTransact)
                {
                    BeginTransaction();
                }
                RowsRead = 0;
                da.Fill(result);
                foreach (DataTable tab in result.Tables)
                    if (tab != null && tab.Rows != null)
                        RowsRead = tab.Rows.Count;

                if (autoTransact)
                {
                    CommitTransaction();
                }
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
                result = null;
            }
            catch (Exception ex)
            {
                if (autoTransact)
                {
                    RollbackTransaction();
                }
                LastMessage = ex.Message;
                LastException = ex;
                result = null;
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
            return result;
        }

        /// <summary>
        /// Executes a SQL query and returns the first table in the result.
        /// </summary>
        /// <param name="sql">The SQL text to execute.</param>
        /// <param name="autoTransact">If true, wraps execution in a transaction which is committed on success or rolled back on failure.</param>
        /// <param name="tableName">Optional name to assign to the returned DataTable.</param>
        /// <returns>The first DataTable from the result; null if failed or no tables.</returns>
        public DataTable ExecuteTable(string sql, bool autoTransact = false, string tableName = "")
        {
            DataTable aux = null;
            DataSet info;

            info = ExecuteDataSet(sql, autoTransact);
            if (info != null && info.Tables.Count > 0)
            {
                if (!String.IsNullOrEmpty(tableName))
                {
                    info.Tables[0].TableName = tableName;
                }
                aux = info.Tables[0];
            }

            return aux;
        }

        /// <summary>
        /// Executes a SQL query expected to return a single column and maps it to a list of strings.
        /// </summary>
        /// <param name="sql">The SQL text to execute.</param>
        /// <param name="autoTransact">If true, wraps execution in a transaction which is committed on success or rolled back on failure.</param>
        /// <returns>A list of string values from the first column; empty list on failure or no rows.</returns>
        public List<string> ExecuteColumn(string sql, bool autoTransact = false)
        {
            List<string> back = new List<string>();
            DataTable aux;
            aux = ExecuteTable(sql, autoTransact);
            if (!Error && aux != null && aux.Rows.Count > 0)
            {
                int rc = aux.Rows.Count;
                for (int i = 0; i < rc; i++)
                    back.Add(aux.Rows[i][0].ToString());
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

        #region Async execution properties and methods
        private Thread Executor;
        private string curquery;
        public int AsyncResult;
        private bool _CancelExecution;

        /// <summary>
        /// Occurs when an async query execution starts.
        /// </summary>
        public event ProcessingQuery StartExecution;

        /// <summary>
        /// Occurs when an async query execution finishes.
        /// </summary>
        public event ProcessingQuery FinishExecution;

        /// <summary>
        /// Gets or sets the command used during async operations.
        /// </summary>
        public SQLiteCommand AsyncCmd = null;

        private DataSet _Results;

        /// <summary>
        /// Gets the results DataSet from the last async execution.
        /// </summary>
        public DataSet Results { get { return _Results; } }

        /// <summary>
        /// Starts asynchronous execution of the provided SQL query, building a DataSet incrementally.
        /// </summary>
        /// <param name="Query">The SQL query text to execute asynchronously.</param>
        public void AsyncExecuteDataSet(string Query)
        {
            if (!OnExecution)
            {
                Executor = new Thread(AsyncExecQuery);
                curquery = Query;
                LastException = null;
                LastSqliteException = null;
                Executing = true;
                _CancelExecution = false;
                if (StartExecution != null)
                    StartExecution(Query, DateTime.Now);
                Executor.Start();
            }
            else
            {
                LastMessage = "There is already another async query on execution, must wait until its completion to execute a new one.";
            }
        }

        /// <summary>
        /// Requests cancellation of the current async execution loop.
        /// </summary>
        public void CancelExecute()
        {
            _CancelExecution = true;
        }

        /// <summary>
        /// Attempts to stop the async execution immediately and clean up resources.
        /// </summary>
        public void ExtremeStop()
        {
            if (Executor.IsAlive)
            {
                if (AsyncCmd != null)
                {
                    try
                    {
                        AsyncCmd.Cancel();
                        AsyncCmd.Dispose();
                    }
                    catch (Exception)
                    {
                        ;
                    }
                }
                AsyncResult = 0;
                Executor.Join();
                Executing = false;
                if (FinishExecution != null)
                    FinishExecution("", DateTime.Now);
            }
        }

        /// <summary>
        /// Internal thread method that executes the current query asynchronously and populates the Results DataSet.
        /// </summary>
        private void AsyncExecQuery()
        {
            DateTime LastCheck;
            _Results = new DataSet();
            int indexxx;
            try
            {

                AsyncCmd = new SQLiteCommand(curquery, Connection);
                AsyncCmd.Connection.Open();
                using (SQLiteDataReader AsyncReader = AsyncCmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    indexxx = 0;
                    LastCheck = DateTime.Now;
                    do
                    {
                        indexxx++;
                        // Create new data table
                        DataTable schemaTable = AsyncReader.GetSchemaTable();
                        DataTable dataTable = new DataTable();
                        if (schemaTable != null)
                        {// A query returning records was executed
                            for (int i = 0; i < schemaTable.Rows.Count; i++)
                            {
                                DataRow dataRow = schemaTable.Rows[i];
                                // Create a column name that is unique in the data table
                                string columnName = (string)dataRow["ColumnName"]; //+ "<C" + i + "/>";
                                if (dataTable.Columns.Contains(columnName))
                                {
                                    int index = 1;
                                    foreach (DataColumn Col in dataTable.Columns)
                                        if (Col.ColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                                            index++;
                                    columnName += index.ToString();
                                }
                                // Add the column definition to the data table
                                DataColumn column = new DataColumn(columnName, (Type)dataRow["DataType"]);
                                dataTable.Columns.Add(column);
                            }
                            _Results.Tables.Add(dataTable);
                            // Fill the data table we just created
                            while (AsyncReader.Read())
                            {
                                DataRow dataRow = dataTable.NewRow();
                                for (int i = 0; i < AsyncReader.FieldCount; i++)
                                    dataRow[i] = AsyncReader.GetValue(i);
                                dataTable.Rows.Add(dataRow);
                                if (DateTime.Now.Subtract(LastCheck) > new TimeSpan(0, 0, 1))
                                {
                                    if (_CancelExecution)
                                    {
                                        AsyncReader.Close();
                                        LastMessage = "OK";
                                        AsyncResult = 0;
                                        if (FinishExecution != null)
                                            FinishExecution(curquery, DateTime.Now);
                                        break;
                                    }
                                    else
                                    {
                                        LastCheck = DateTime.Now;
                                    }
                                }
                            }

                            DataTable NonQ1 = new DataTable("NonQuery" + indexxx.ToString());
                            NonQ1.Columns.Add(new DataColumn("RowsAffected"));
                            DataRow DRx1 = NonQ1.NewRow();
                            DRx1[0] = Math.Max(AsyncReader.RecordsAffected, 0);
                            NonQ1.Rows.Add(DRx1);
                            Results.Tables.Add(NonQ1);
                        }
                        else
                        {
                            // No records were returned
                            DataTable NonQ2 = new DataTable("NonQuery" + indexxx.ToString());
                            NonQ2.Columns.Add(new DataColumn("RowsAffected"));
                            DataRow DRx2 = NonQ2.NewRow();
                            DRx2[0] = Math.Max(AsyncReader.RecordsAffected, 0);
                            NonQ2.Rows.Add(DRx2);
                            Results.Tables.Add(NonQ2);
                        }
                    } while (AsyncReader.NextResult());
                    AsyncReader.Close();
                    LastMessage = "OK";
                }
            }
            catch (SQLiteException sqlex)
            {
                AsyncResult = -1;
                LastMessage = sqlex.Message;
                LastSqliteException = sqlex;
            }
            catch (Exception ex)
            {
                AsyncResult = -1;
                LastMessage = ex.Message;
                LastException = ex;
            }
            finally
            {
                Executing = false;
                if (AsyncCmd != null)
                    AsyncCmd.Dispose();
            }

            Executing = false;
            if (FinishExecution != null)
                FinishExecution(curquery, DateTime.Now);

            AsyncResult = 1;
        }
        #endregion

        /// <summary>
        /// Creates database tables required for logging (SystemLog and SystemExceptions) if they do not exist.
        /// </summary>
        public void CreateLogTables()
        {
            string sql = @"
CREATE TABLE IF NOT EXISTS SystemLog
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Comment TEXT NOT NULL,
    ClassName TEXT NULL,
    MethodName TEXT NULL,
    Executor TEXT NULL,
    ExecutionTime TEXT NOT NULL,
    LogLevel TEXT NOT NULL,
    ProcessType TEXT NULL,
    Exception INTEGER NULL
);

CREATE TABLE IF NOT EXISTS SystemExceptions
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Message TEXT NULL,
    StackTrace TEXT NOT NULL,
    Source TEXT NULL,
    ExecutionTime TEXT NOT NULL
);
";
            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// Registers an exception in the SystemExceptions table.
        /// </summary>
        /// <param name="Ex">The exception to register.</param>
        /// <returns>A response containing the inserted exception ID or failure details.</returns>
        private SqliteResponse<int> RegisterException(Exception Ex)
        {
            return RegisterException(Ex.Message, Ex.StackTrace, Ex.Source);
        }

        /// <summary>
        /// Registers an exception in the SystemExceptions table using explicit fields.
        /// </summary>
        /// <param name="msg">Exception message.</param>
        /// <param name="stackTrace">Exception stack trace.</param>
        /// <param name="source">Exception source.</param>
        /// <returns>A response containing the inserted exception ID or failure details.</returns>
        private SqliteResponse<int> RegisterException(string msg, string stackTrace, string source)
        {
            string sql = @"
INSERT INTO SystemExceptions (Message, StackTrace, Source, ExecutionTime)
VALUES (@Message, @StackTrace, @Source, datetime('now'));

SELECT last_insert_rowid() AS Result;
";

            sql = sql.Replace("@Message", "'" + (msg ?? "").Replace("'", "''") + "'")
                     .Replace("@StackTrace", "'" + (stackTrace ?? "").Replace("'", "''") + "'")
                     .Replace("@Source", "'" + (source ?? "").Replace("'", "''") + "'");
            int back = -1;

            var buff = ExecuteScalar(sql);
            back = buff == null ? -1 : Convert.ToInt32(buff);
            if (back <= 0)
            {
                return SqliteResponse<int>.Failure("Error while registering exception on DB.", LastException ?? (Exception)LastSqliteException);
            }
            return SqliteResponse<int>.Successful(back);
        }

        /// <summary>
        /// Registers a DEBUG level log into SystemLog.
        /// </summary>
        /// <param name="comment">Log comment.</param>
        /// <param name="className">Origin class name.</param>
        /// <param name="methodName">Origin method name.</param>
        /// <param name="executor">Executor identity.</param>
        /// <param name="processType">Process type descriptor.</param>
        /// <returns>Response with inserted log ID or failure.</returns>
        public SqliteResponse<int> Debug(string comment, string className, string methodName, string executor, string processType)
        {
            return RegisterLog(comment, className, methodName, executor, "DEBUG", processType);
        }

        /// <summary>
        /// Registers an INFO level log into SystemLog.
        /// </summary>
        /// <param name="comment">Log comment.</param>
        /// <param name="className">Origin class name.</param>
        /// <param name="methodName">Origin method name.</param>
        /// <param name="executor">Executor identity.</param>
        /// <param name="processType">Process type descriptor.</param>
        /// <returns>Response with inserted log ID or failure.</returns>
        public SqliteResponse<int> Info(string comment, string className, string methodName, string executor, string processType)
        {
            return RegisterLog(comment, className, methodName, executor, "INFO", processType);
        }

        /// <summary>
        /// Registers a WARN level log into SystemLog, optionally linking an exception.
        /// </summary>
        /// <param name="comment">Log comment.</param>
        /// <param name="className">Origin class name.</param>
        /// <param name="methodName">Origin method name.</param>
        /// <param name="executor">Executor identity.</param>
        /// <param name="processType">Process type descriptor.</param>
        /// <param name="Ex">Optional exception to register.</param>
        /// <returns>Response with inserted log ID or failure.</returns>
        public SqliteResponse<int> Warning(string comment, string className, string methodName, string executor, string processType, Exception Ex = null)
        {
            return RegisterLog(comment, className, methodName, executor, "WARN", processType, Ex);
        }

        /// <summary>
        /// Registers an ERROR level log into SystemLog, optionally linking an exception.
        /// </summary>
        /// <param name="comment">Log comment.</param>
        /// <param name="className">Origin class name.</param>
        /// <param name="methodName">Origin method name.</param>
        /// <param name="executor">Executor identity.</param>
        /// <param name="processType">Process type descriptor.</param>
        /// <param name="Ex">Optional exception to register.</param>
        /// <returns>Response with inserted log ID or failure.</returns>
        public SqliteResponse<int> Exception(string comment, string className, string methodName, string executor, string processType, Exception Ex = null)
        {
            return RegisterLog(comment, className, methodName, executor, "ERROR", processType, Ex);
        }

        /// <summary>
        /// Registers a log record into SystemLog with the specified level, optionally linking an exception.
        /// </summary>
        /// <param name="comment">Log comment.</param>
        /// <param name="className">Origin class name.</param>
        /// <param name="methodName">Origin method name.</param>
        /// <param name="executor">Executor identity.</param>
        /// <param name="logLevel">Log level text (e.g., DEBUG, INFO, WARN, ERROR).</param>
        /// <param name="processType">Process type descriptor.</param>
        /// <param name="Ex">Optional exception to register.</param>
        /// <returns>Response with inserted log ID or failure.</returns>
        public SqliteResponse<int> RegisterLog(string comment, string className, string methodName, string executor, string logLevel, string processType, Exception Ex = null)
        {
            string sql = @"
INSERT INTO SystemLog (Comment, ClassName, MethodName, Executor, ExecutionTime, LogLevel, ProcessType, Exception)
VALUES (@Comment, @ClassName, @MethodName, @Executor, datetime('now'), @LogLevel, @ProcessType, @Exception);

SELECT last_insert_rowid() AS Result;
";
            int exceptionId = -1, back;

            if (Ex != null)
            {
                var response = RegisterException(Ex);
                if (response.IsFailure)
                {
                    return SqliteResponse<int>.Failure("Error while registering exception on DB.", response.Errors.FirstOrDefault()?.Exception);
                }
                exceptionId = response.Result;
            }

            sql = sql.Replace("@Comment", "'" + (comment ?? "").Replace("'", "''") + "'")
                     .Replace("@ClassName", "'" + (className ?? "").Replace("'", "''") + "'")
                     .Replace("@MethodName", "'" + (methodName ?? "").Replace("'", "''") + "'")
                     .Replace("@Executor", "'" + (executor ?? "").Replace("'", "''") + "'")
                     .Replace("@LogLevel", "'" + (logLevel ?? "").Replace("'", "''") + "'")
                     .Replace("@ProcessType", "'" + (processType ?? "").Replace("'", "''") + "'")
                     .Replace("@Exception", Ex == null ? "NULL" : exceptionId.ToString());

            var buff = ExecuteScalar(sql);
            back = buff == null ? -1 : Convert.ToInt32(buff);
            if (back <= 0)
            {
                return SqliteResponse<int>.Failure("Error while registering log on DB.", LastException ?? (Exception)LastSqliteException);
            }
            return SqliteResponse<int>.Successful(back);
        }
    }
}
