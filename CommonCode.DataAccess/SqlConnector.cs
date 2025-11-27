using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;



namespace CommonCode.DataAccess
{
    public delegate void ProcessingQuery(string Query, DateTime Time);
    public partial class SqlConnector
    {
        #region Variables
        private DateTime _startTime, _endingTime;
        SqlCommand cmd = null;
        SqlDataAdapter da = null;
        #endregion

        #region Properties
        public int RowsAffected { get; set; }
        public int RowsRead { get; set; }
        public string LastMessage { get; set; }
        public SqlConnection Connection { get; set; }
        public SqlConnection NewConnection
        {
            get
            {
                return new SqlConnection(ConnectionString);
            }
        }
        public string ConnectionString
        {
            get { return Connection.ConnectionString; }
            set { Connection.ConnectionString = value; }
        }
        public SqlTransaction Transaction
        {
            get { return cmd == null ? null : cmd.Transaction; }
            set { cmd.Transaction = value; }
        }
        public TimeSpan ExecutionLapse
        {
            get
            {
                return _endingTime.Subtract(_startTime);
            }
        }
        public bool OnExecution { get; private set; }
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
        public int TimeOut { get; set; }
        public bool Error
        {
            get
            {
                return LastMessage != "OK";
            }
        }
        public Exception LastException { get; set; }
        public SqlException LastSqlException { get; set; }
        public string Server
        {
            get
            {
                if (Connection != null)
                    return Connection.DataSource;
                return "";
            }
        }
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
        public SqlConnector()
        {
            TimeOut = 0;
            Connection = new SqlConnection();

            LastMessage = "OK";
            cmd = new SqlCommand();
            da = new SqlDataAdapter();
        }
        public SqlConnector(string connectionString)
        {
            TimeOut = 0;
            Connection = new SqlConnection(connectionString);

            LastMessage = "OK";
            cmd = new SqlCommand();
            da = new SqlDataAdapter();
        }
        public SqlConnector(SqlConnection connection)
        {
            TimeOut = 0;
            Connection = (connection == null ? new SqlConnection() : Connection);

            LastMessage = "OK";
            cmd = new SqlCommand();
            da = new SqlDataAdapter();
        }
        #endregion

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
            catch (SqlException sqlex)
            {
                if (autoTransact)
                {
                    RollbackTransaction();
                }
                LastMessage = sqlex.Errors[0].Message;
                LastSqlException = sqlex;
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
            catch (SqlException sqlex)
            {
                if (autoTransact)
                {
                    RollbackTransaction();
                }
                LastMessage = sqlex.Errors[0].Message;
                LastSqlException = sqlex;
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
        public DataSet ExecuteDataSet(string sql, bool autoTransact = false)
        {
            DataSet result = new DataSet();
            try
            {
                cmd.Connection = Connection;
                cmd.CommandText = sql;
                cmd.CommandTimeout = TimeOut;
                da = new SqlDataAdapter(cmd);
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
            catch (SqlException sqlex)
            {
                if (autoTransact)
                {
                    RollbackTransaction();
                }
                LastMessage = sqlex.Errors[0].Message;
                LastSqlException = sqlex;
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

        public void CreateTableInSQL(DataTable table)
        {
            string sql;
            sql = GetTableScript(table);
            ExecuteScalar(sql);
        }

        /// <summary>
        /// Inspects a DataTable and return a SQL string that can be used to CREATE a TABLE in SQL Server.
        /// </summary>
        /// <param name="table">System.Data.DataTable object to be inspected for building the SQL CREATE TABLE statement.</param>
        /// <returns>String of SQL</returns>
        public static string GetTableScript(DataTable table)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder alterSql = new StringBuilder();

            sql.AppendFormat("CREATE TABLE [{0}] (", table.TableName);

            for (int i = 0; i < table.Columns.Count; i++)
            {
                bool isNumeric = false;
                bool usesColumnDefault = true;

                sql.AppendFormat("\n\t[{0}]", table.Columns[i].ColumnName);

                switch (table.Columns[i].DataType.ToString())
                {
                    case "System.Boolean":
                        sql.AppendFormat(" bit");
                        break;
                    case "System.Byte":
                        sql.AppendFormat(" smallint");
                        break;
                    case "System.Char":
                        sql.AppendFormat(" nvarchar(5)");
                        break;
                    case "System.Int16":
                        sql.Append(" smallint");
                        isNumeric = true;
                        break;
                    case "System.Int32":
                        sql.Append(" int");
                        isNumeric = true;
                        break;
                    case "System.Int64":
                        sql.Append(" bigint");
                        isNumeric = true;
                        break;
                    case "System.DateTime":
                        sql.Append(" datetime");
                        usesColumnDefault = false;
                        break;
                    case "System.Single":
                        sql.Append(" single");
                        isNumeric = true;
                        break;
                    case "System.Double":
                        sql.Append(" double");
                        isNumeric = true;
                        break;
                    case "System.Decimal":
                        sql.AppendFormat(" decimal(18, 6)");
                        isNumeric = true;
                        break;
                    default:
                    case "System.String":
                        if (table.Columns[i].MaxLength == -1)
                            sql.AppendFormat(" nvarchar(500)");
                        else
                            sql.AppendFormat(" nvarchar({0})", table.Columns[i].MaxLength);
                        break;
                }


                /*
                switch (table.Columns[i].DataType.ToString().ToUpper())
                {
                    case "SYSTEM.INT16":
                        sql.Append(" smallint");
                        isNumeric = true;
                        break;
                    case "SYSTEM.INT32":
                        sql.Append(" int");
                        isNumeric = true;
                        break;
                    case "SYSTEM.INT64":
                        sql.Append(" bigint");
                        isNumeric = true;
                        break;
                    case "SYSTEM.DATETIME":
                        sql.Append(" datetime");
                        usesColumnDefault = false;
                        break;
                    case "SYSTEM.STRING":
                        if(table.Columns[i].MaxLength == -1)
                            sql.AppendFormat(" nvarchar(500)");
                        else
                            sql.AppendFormat(" nvarchar({0})", table.Columns[i].MaxLength);
                        break;
                    case "SYSTEM.SINGLE":
                        sql.Append(" single");
                        isNumeric = true;
                        break;
                    case "SYSTEM.DOUBLE":
                        sql.Append(" double");
                        isNumeric = true;
                        break;
                    case "SYSTEM.DECIMAL":
                        sql.AppendFormat(" decimal(18, 6)");
                        isNumeric = true;
                        break;
                    default:
                        sql.AppendFormat(" nvarchar({0})", table.Columns[i].MaxLength);
                        break;
                }*/

                if (table.Columns[i].AutoIncrement)
                {
                    sql.AppendFormat(" IDENTITY({0},{1})",
                        table.Columns[i].AutoIncrementSeed,
                        table.Columns[i].AutoIncrementStep);
                }
                else
                {
                    // DataColumns will add a blank DefaultValue for any AutoIncrement column. 
                    // We only want to create an ALTER statement for those columns that are not set to AutoIncrement. 
                    if (table.Columns[i].DefaultValue != null && !String.IsNullOrEmpty(table.Columns[i].DefaultValue.ToString()))
                    {
                        if (usesColumnDefault)
                        {
                            if (isNumeric)
                            {
                                alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ({2}) FOR [{1}];",
                                    table.TableName,
                                    table.Columns[i].ColumnName,
                                    table.Columns[i].DefaultValue);
                            }
                            else
                            {
                                alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ('{2}') FOR [{1}];",
                                    table.TableName,
                                    table.Columns[i].ColumnName,
                                    table.Columns[i].DefaultValue);
                            }
                        }
                        else
                        {
                            // Default values on Date columns, e.g., "DateTime.Now" will not translate to SQL.
                            // This inspects the caption for a simple XML string to see if there is a SQL compliant default value, e.g., "GETDATE()".
                            try
                            {
                                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();

                                xml.LoadXml(table.Columns[i].Caption);

                                alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ({2}) FOR [{1}];",
                                    table.TableName,
                                    table.Columns[i].ColumnName,
                                    xml.GetElementsByTagName("defaultValue")[0].InnerText);
                            }
                            catch
                            {
                                // Handle
                            }
                        }
                    }
                }

                if (!table.Columns[i].AllowDBNull)
                {
                    sql.Append(" NOT NULL");
                }

                sql.Append(",");
            }

            if (table.PrimaryKey.Length > 0)
            {
                StringBuilder primaryKeySql = new StringBuilder();

                primaryKeySql.AppendFormat("\n\tCONSTRAINT PK_{0} PRIMARY KEY (", table.TableName);

                for (int i = 0; i < table.PrimaryKey.Length; i++)
                {
                    primaryKeySql.AppendFormat("{0},", table.PrimaryKey[i].ColumnName);
                }

                primaryKeySql.Remove(primaryKeySql.Length - 1, 1);
                primaryKeySql.Append(")");

                sql.Append(primaryKeySql);
            }
            else
            {
                sql.Remove(sql.Length - 1, 1);
            }

            sql.AppendFormat("\n);\n{0}", alterSql.ToString());

            return sql.ToString();
        }

        #region Async execution properties and methods
        private Thread Executor;
        private string curquery;
        public int AsyncResult;
        private bool _CancelExecution;
        public event ProcessingQuery StartExecution;
        public event ProcessingQuery FinishExecution;

        public SqlCommand AsyncCmd = null;

        private DataSet _Results;
        public DataSet Results { get { return _Results; } }


        public void AsyncExecuteDataSet(string Query)
        {
            if (!OnExecution)
            {
                Executor = new Thread(AsyncExecQuery);
                curquery = Query;
                LastException = null;
                LastSqlException = null;
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
        public void CancelExecute()
        {
            _CancelExecution = true;
        }
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

        private void AsyncExecQuery()
        {
            DateTime LastCheck;
            _Results = new DataSet();
            int indexxx;
            try
            {

                AsyncCmd = new SqlCommand(curquery, Connection);
                AsyncCmd.Connection.Open();
                using (SqlDataReader AsyncReader = AsyncCmd.ExecuteReader(CommandBehavior.CloseConnection))
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
            catch (SqlException sqlex)
            {
                AsyncResult = -1;
                LastMessage = sqlex.Message;
                LastSqlException = sqlex;
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

        public void CreateLogTables()
        {
            string sql = @"
IF OBJECT_ID('dbo.SystemLog', 'U') IS NULL 
BEGIN
	CREATE TABLE dbo.SystemLog
	(
		Id INT NOT NULL  identity( 1, 1 )
		,Comment NVARCHAR(100) NOT NULL 
		,ClassName NVARCHAR(100) NULL
		,MethodName NVARCHAR(100) NULL
		,Executor NVARCHAR(50) NULL 
		,ExecutionTime DATETIME NOT NULL 
		,LogLevel  NVARCHAR(10) NOT NULL
		,ProcessType NVARCHAR(50) NULL
		,Exception INT NULL
		,CONSTRAINT PK_SystemLog PRIMARY KEY(Id)
	)
END

IF OBJECT_ID('dbo.SystemExceptions', 'U') IS NULL 
BEGIN
	CREATE TABLE dbo.SystemExceptions
	(
		Id INT NOT NULL  identity( 1, 1 )
		,Message NVARCHAR(500) NULL 
		,StackTrace NVARCHAR(MAX) NOT NULL 
		,Source NVARCHAR(150) NULL 
		,ExecutionTime DATETIME NOT NULL 
		,CONSTRAINT PK_SystemExceptions PRIMARY KEY(Id)
	)
END
";
            ExecuteNonQuery(sql);
        }

        private SpResponse<int> RegisterException(Exception Ex)
        {
            return RegisterException(Ex.Message, Ex.StackTrace, Ex.Source);
        }
        private SpResponse<int> RegisterException(string msg, string stackTrace, string source)
        {
            string sql = @"
    SET NOCOUNT ON
	DECLARE 
	
	SELECT 
		@Message = '@Message@', 
		@StackTrace = '@StackTrace@', 
		@Source = '@Source@', 
		@ExecutionTime = GETDATE()
	
	BEGIN TRANSACTION SystemExceptions_Insert WITH MARK N'Inserting new record into SystemExceptions';  
		BEGIN TRY
			
			INSERT INTO [dbo].[SystemExceptions]( [Message], [StackTrace], [Source], [ExecutionTime] )
			VALUES ( @Message, @StackTrace, @Source, @ExecutionTime )
			
			SELECT
				SCOPE_IDENTITY() AS Result
		
		END TRY
		BEGIN CATCH
			--return the data of the error
			SELECT 
				-1 AS Result
		        
			IF @@TRANCOUNT > 0
		        ROLLBACK TRANSACTION SystemExceptions_Insert;
		END CATCH
		
		IF @@TRANCOUNT > 0
		    COMMIT TRANSACTION SystemExceptions_Insert;
";

            sql = sql.Replace("@Message@", msg)
                     .Replace("@StackTrace@", stackTrace)
                     .Replace("@Source@", source);
            int back = -1;

            var buff = ExecuteScalar(sql);
            back = buff == null ? -1 : Convert.ToInt32(buff);
            if (back <= 0)
            {
                return SpResponse<int>.Failure("Error while registering exception on DB.", LastException ?? (Exception)LastSqlException);
            }
            return SpResponse<int>.Successful(back);
        }

        public SpResponse<int> Debug(string comment, string className, string methodName, string executor, string processType)
        {
            return RegisterLog(comment, className, methodName, executor, "DEBUG", processType);
        }
        public SpResponse<int> Info(string comment, string className, string methodName, string executor, string processType)
        {
            return RegisterLog(comment, className, methodName, executor, "INFO", processType);
        }
        public SpResponse<int> Warning(string comment, string className, string methodName, string executor, string processType, Exception Ex = null)
        {
            return RegisterLog(comment, className, methodName, executor, "WARN", processType, Ex);
        }
        public SpResponse<int> Exception(string comment, string className, string methodName, string executor, string processType, Exception Ex = null)
        {
            return RegisterLog(comment, className, methodName, executor, "ERROR", processType, Ex);
        }
        public SpResponse<int> RegisterLog(string comment, string className, string methodName, string executor, string logLevel, string processType, Exception Ex = null)
        {
            string sql = @"
    SET NOCOUNT ON
	DECLARE @Comment NVARCHAR(500), @ClassName NVARCHAR(100), @MethodName NVARCHAR(100), @Executor NVARCHAR(50), @ExecutionTime DATETIME, @LogLevel NVARCHAR(10), @ProcessType NVARCHAR(50), @Exception INT
	SELECT 
		@Comment = '@Comment@', 
		@ClassName = '@ClassName@', 
		@MethodName = '@MethodName@', 
		@Executor = '@Executor@', 
		@ExecutionTime = GETDATE(), 
		@LogLevel = '@LogLevel@', 
		@ProcessType = '@ProcessType@', 
		@Exception = @Exception@
	
	BEGIN TRANSACTION SystemLog_Insert WITH MARK N'Inserting new record into SystemLog';  
		BEGIN TRY
			
			INSERT INTO [dbo].[SystemLog]( [Comment], [ClassName], [MethodName], [Executor], [ExecutionTime], [LogLevel], [ProcessType], [Exception] )
			VALUES ( @Comment, @ClassName, @MethodName, @Executor, @ExecutionTime, @LogLevel, @ProcessType, @Exception )
			
			SELECT
				SCOPE_IDENTITY() AS Result
		
		END TRY
		BEGIN CATCH
			--return the data of the error
			SELECT 
				-1 AS Result
		        
			IF @@TRANCOUNT > 0
		        ROLLBACK TRANSACTION SystemLog_Insert;
		END CATCH
		
		IF @@TRANCOUNT > 0
		    COMMIT TRANSACTION SystemLog_Insert;
";
            int exceptionId = -1, back;

            if (Ex != null)
            {
                var response = RegisterException(Ex);
                if (response.IsFailure)
                {
                    return SpResponse<int>.Failure("Error while registering exception on DB.", response.Errors.FirstOrDefault()?.Exception);
                }
                exceptionId = response.Result;
            }

            sql = sql.Replace("@Comment@", comment)
                     .Replace("@ClassName@", className)
                     .Replace("@MethodName@", methodName)
                     .Replace("@Executor@", executor)
                     .Replace("@LogLevel@", logLevel)
                     .Replace("@ProcessType@", processType)
                     .Replace("@Exception@", Ex == null ? "NULL" : exceptionId.ToString());

            var buff = ExecuteScalar(sql);
            back = buff == null ? -1 : Convert.ToInt32(buff);
            if (back <= 0)
            {
                return SpResponse<int>.Failure("Error while registering log on DB.", LastException ?? (Exception)LastSqlException);
            }
            return SpResponse<int>.Successful(back);
        }
    }
    public class ErrorOnResponse
    {
        public DateTime ExecutionTime { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public ErrorOnResponse()
        {
            this.ExecutionTime = DateTime.Now;
            this.Message = "An error occurred while processing your request.";
        }
        public ErrorOnResponse(string message, Exception Ex = null)
        {
            this.ExecutionTime = DateTime.Now;
            this.Message = message;
            this.Exception = Ex == null ? new Exception(this.Message) : Ex;
        }
    }
    public class SpResponse<T>
    {
        #region Properties
        public DateTime ExecutionTime { get; set; }
        public string ExecutionTimeString
        {
            get
            {
                return ExecutionTime.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        private List<T> _Results;
        public List<T> Results { get { return _Results; } set { _Results = value; } }
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
        public List<ErrorOnResponse> Errors { get; set; }
        private string _Message = String.Empty;
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        public bool ErrorOccured
        {
            get
            {
                if (Errors == null || Errors.Count == 0)
                    return false;
                return !Message.Equals("Success", StringComparison.OrdinalIgnoreCase);
            }
        }
        public bool IsOK { get { return !ErrorOccured; } }
        public bool IsFailure { get { return ErrorOccured; } }
        #endregion
        #region Class contructor
        public SpResponse()
        {
            Errors = new List<ErrorOnResponse>();
            Results = new List<T>();
            ExecutionTime = DateTime.Now;
        }
        #endregion
        #region Indexers of the class, by field number(position on the table) and by field name(case insensitive)
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

        public void AddResult(T result)
        {
            if (this.Results == null)
                this.Results = new List<T>();
            this.Results.Add(result);
        }
        public void AddResults(List<T> results)
        {
            if (this.Results == null)
                this.Results = new List<T>();
            this.Results.AddRange(results);
        }
        public SpResponse<T> Fail()
        {
            ExecutionTime = DateTime.Now;
            this.Errors.Add(new ErrorOnResponse(this.Message));
            return this;
        }
        public SpResponse<T> Fail(string message, Exception ex = null)
        {
            ExecutionTime = DateTime.Now;
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
        public SpResponse<T> Fail(Exception ex)
        {
            ExecutionTime = DateTime.Now;
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
        public SpResponse<T> Success(string message = "Success")
        {
            ExecutionTime = DateTime.Now;
            this.Message = message;
            return this;
        }
        public SpResponse<T> Success(T result, string message = "Success")
        {
            ExecutionTime = DateTime.Now;
            this.Result = result;
            this.Message = message;
            return this;
        }
        public SpResponse<T> Success(List<T> results, string message = "Success")
        {
            ExecutionTime = DateTime.Now;
            this.Results = results;
            this.Message = message;
            return this;
        }

        public static SpResponse<T> Failure(string message, Exception ex = null)
        {
            return new SpResponse<T>().Fail(message, ex);
        }
        public static SpResponse<T> Failure(Exception ex = null)
        {
            return new SpResponse<T>().Fail(ex);
        }
        public static SpResponse<T> Successful(List<T> results, string message = "Success")
        {
            return new SpResponse<T>().Success(results, message);
        }
        public static SpResponse<T> Successful(T result, string message = "Success")
        {
            return new SpResponse<T>().Success(result, message);
        }
        public static SpResponse<T> Successful(string message = "Success")
        {
            return new SpResponse<T>().Success(message);
        }

    }
}
