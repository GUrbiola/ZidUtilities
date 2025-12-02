# CommonCode.DataAccess

SQL Server data access layer providing helper methods for executing database operations with ADO.NET.

## Features

- **SqlConnector**: SQL Server database connector with comprehensive query execution support
- **SqlResponse<T>**: Generic response wrapper for all database operations
- **ErrorOnResponse**: Detailed error tracking and exception handling
- Synchronous and asynchronous query execution
- Automatic transaction management with autoTransact parameter
- Built-in logging system with SystemLog and SystemExceptions tables
- Execution timing and performance tracking

## Target Framework

.NET Framework 4.8

## Dependencies

- System.Data
- System.Data.SqlClient

## Core Components

### SqlConnector

The main class for executing SQL Server commands. Provides methods for queries, non-queries, scalar operations, and more.

**Key Properties:**
- `Connection` - The underlying SqlConnection
- `ConnectionString` - Get/set connection string
- `LastMessage` - Status message ("OK" or error description)
- `Error` - Boolean indicating if last operation failed
- `RowsAffected` - Rows affected by last non-query
- `RowsRead` - Rows read by last query
- `LastException` - Last general exception thrown
- `LastSqlException` - Last SQL-specific exception
- `TimeOut` - Command timeout in seconds (0 = default)
- `OnExecution` - Boolean indicating if operation is in progress
- `ExecutionLapse` - TimeSpan of last operation duration

### SqlResponse<T>

Generic response wrapper returned by all execute methods.

**Properties:**
- `Result` - Single result of type T
- `Results` - List of results of type T
- `Message` - Status message
- `Errors` - List of ErrorOnResponse objects
- `IsOK` - True if operation succeeded
- `IsFailure` - True if operation failed
- `ErrorOccured` - True if errors exist
- `ExecutionTime` - DateTime when response was created

**Static Factory Methods:**
- `SqlResponse<T>.Successful(T result, string message = "Success")`
- `SqlResponse<T>.Successful(List<T> results, string message = "Success")`
- `SqlResponse<T>.Failure(string message, Exception ex = null)`

## Usage Examples

### Creating a Connection

```csharp
using ZidUtilities.CommonCode.DataAccess;

// Using connection string
string connStr = "Server=localhost;Database=MyDb;Integrated Security=true;";
var connector = new SqlConnector(connStr);

// Test the connection
if (connector.TestConnection())
{
    Console.WriteLine("Connected successfully!");
}
else
{
    Console.WriteLine($"Connection failed: {connector.LastMessage}");
}
```

### ExecuteNonQuery - INSERT, UPDATE, DELETE

```csharp
var connector = new SqlConnector(connectionString);

// Simple INSERT without parameters
string insertSql = "INSERT INTO Users (Username, Email) VALUES ('jdoe', 'jdoe@example.com')";
var response = connector.ExecuteNonQuery(insertSql);

if (response.IsOK)
{
    Console.WriteLine($"Success! Rows affected: {connector.RowsAffected}");
}
else
{
    Console.WriteLine($"Error: {response.Message}");
    if (response.Errors.Count > 0)
    {
        Console.WriteLine($"Exception: {response.Errors[0].Exception.Message}");
    }
}

// INSERT with parameters (note: Dictionary<string, string>)
string paramSql = "INSERT INTO Users (Username, Email) VALUES (@username, @email)";
var parameters = new Dictionary<string, string>
{
    { "@username", "janedoe" },
    { "@email", "jane@example.com" }
};

var response2 = connector.ExecuteNonQuery(paramSql, parameters);

// UPDATE with parameters
string updateSql = "UPDATE Users SET Email = @email WHERE Username = @username";
var updateParams = new Dictionary<string, string>
{
    { "@email", "newemail@example.com" },
    { "@username", "jdoe" }
};

var response3 = connector.ExecuteNonQuery(updateSql, updateParams);
```

### ExecuteTable - Query and Return DataTable

```csharp
var connector = new SqlConnector(connectionString);

// Simple SELECT
string query = "SELECT * FROM Users";
var response = connector.ExecuteTable(query);

if (response.IsOK && response.Result != null)
{
    DataTable users = response.Result;
    Console.WriteLine($"Retrieved {users.Rows.Count} users");

    foreach (DataRow row in users.Rows)
    {
        Console.WriteLine($"Username: {row["Username"]}, Email: {row["Email"]}");
    }
}

// SELECT with parameters
string paramQuery = "SELECT * FROM Users WHERE CreatedDate > @date";
var queryParams = new Dictionary<string, string>
{
    { "@date", "2024-01-01" }
};

var response2 = connector.ExecuteTable(paramQuery, queryParams);

// With custom table name
var response3 = connector.ExecuteTable(query, null, false, "MyUsers");
Console.WriteLine($"Table name: {response3.Result.TableName}");
```

### ExecuteDataSet - Multiple Result Sets

```csharp
var connector = new SqlConnector(connectionString);

// Query that returns multiple result sets
string multipleSql = @"
    SELECT * FROM Users;
    SELECT * FROM Orders;
    SELECT * FROM Products;";

var response = connector.ExecuteDataSet(multipleSql);

if (response.IsOK && response.Result != null)
{
    DataSet ds = response.Result;

    Console.WriteLine($"Retrieved {ds.Tables.Count} tables");

    DataTable users = ds.Tables[0];
    DataTable orders = ds.Tables[1];
    DataTable products = ds.Tables[2];

    Console.WriteLine($"Users: {users.Rows.Count}");
    Console.WriteLine($"Orders: {orders.Rows.Count}");
    Console.WriteLine($"Products: {products.Rows.Count}");
}
```

### ExecuteScalar - Single Value

```csharp
var connector = new SqlConnector(connectionString);

// Get count
string countQuery = "SELECT COUNT(*) FROM Users";
var response = connector.ExecuteScalar(countQuery);

if (response.IsOK && response.Result != null)
{
    int count = Convert.ToInt32(response.Result);
    Console.WriteLine($"Total users: {count}");
}

// Get max value
string maxQuery = "SELECT MAX(CreatedDate) FROM Users";
var response2 = connector.ExecuteScalar(maxQuery);

if (response2.IsOK && response2.Result != null && response2.Result != DBNull.Value)
{
    DateTime maxDate = Convert.ToDateTime(response2.Result);
    Console.WriteLine($"Latest user created: {maxDate}");
}

// Get single string value
string nameQuery = "SELECT Username FROM Users WHERE Id = @id";
var nameParams = new Dictionary<string, string> { { "@id", "1" } };
var response3 = connector.ExecuteScalar(nameQuery, nameParams);

if (response3.IsOK && response3.Result != null)
{
    string username = response3.Result.ToString();
    Console.WriteLine($"Username: {username}");
}
```

### ExecuteColumn - Single Column as List

```csharp
var connector = new SqlConnector(connectionString);

// Get list of all usernames
string query = "SELECT Username FROM Users ORDER BY Username";
var response = connector.ExecuteColumn(query);

if (response.IsOK && response.Result != null)
{
    List<string> usernames = response.Result;

    Console.WriteLine($"Found {usernames.Count} usernames:");
    foreach (string username in usernames)
    {
        Console.WriteLine($"  - {username}");
    }
}

// With parameters
string activeQuery = "SELECT Email FROM Users WHERE IsActive = @active";
var activeParams = new Dictionary<string, string> { { "@active", "1" } };
var response2 = connector.ExecuteColumn(activeQuery, activeParams);
```

### Automatic Transactions with autoTransact

```csharp
var connector = new SqlConnector(connectionString);

// Single operation with auto-transaction
// If it fails, it automatically rolls back
string insertSql = "INSERT INTO Users (Username, Email) VALUES (@username, @email)";
var parameters = new Dictionary<string, string>
{
    { "@username", "testuser" },
    { "@email", "test@example.com" }
};

var response = connector.ExecuteNonQuery(insertSql, parameters, autoTransact: true);

if (response.IsOK)
{
    Console.WriteLine("Insert committed successfully");
}
else
{
    Console.WriteLine("Insert failed and was rolled back");
}

// Note: For multiple operations, you need to manage transactions manually
// (BeginTransaction, CommitTransaction, RollbackTransaction are private)
```

### Error Handling Patterns

```csharp
var connector = new SqlConnector(connectionString);

string query = "SELECT * FROM NonExistentTable";
var response = connector.ExecuteTable(query);

// Check using IsOK property
if (!response.IsOK)
{
    Console.WriteLine("Operation failed!");
    Console.WriteLine($"Message: {response.Message}");

    // Access errors collection
    if (response.Errors.Count > 0)
    {
        foreach (var error in response.Errors)
        {
            Console.WriteLine($"Error at {error.ExecutionTime}: {error.Message}");
            if (error.Exception != null)
            {
                Console.WriteLine($"Exception: {error.Exception.GetType().Name}");
                Console.WriteLine($"Stack: {error.Exception.StackTrace}");
            }
        }
    }
}

// Alternative: Check Error property on connector
if (connector.Error)
{
    Console.WriteLine($"Connector error: {connector.LastMessage}");

    // Access specific exception types
    if (connector.LastSqlException != null)
    {
        Console.WriteLine($"SQL Error Number: {connector.LastSqlException.Number}");
    }
    else if (connector.LastException != null)
    {
        Console.WriteLine($"General Exception: {connector.LastException.Message}");
    }
}
```

### Asynchronous Execution

```csharp
var connector = new SqlConnector(connectionString);

// Subscribe to events
connector.StartExecution += (query, time) =>
{
    Console.WriteLine($"Started at {time}: {query.Substring(0, Math.Min(50, query.Length))}...");
};

connector.FinishExecution += (query, time) =>
{
    Console.WriteLine($"Finished at {time}");
};

// Start async query
string query = "SELECT * FROM LargeTable";
connector.AsyncExecuteDataSet(query);

// Check if still executing
while (connector.OnExecution)
{
    Console.WriteLine("Still executing...");
    System.Threading.Thread.Sleep(100);
}

// Get results
if (connector.AsyncResult == 1)
{
    DataSet results = connector.Results;
    Console.WriteLine($"Async query returned {results.Tables.Count} tables");
}
else
{
    Console.WriteLine($"Async query failed: {connector.LastMessage}");
}

// Cancel execution if needed
// connector.CancelExecute();
```

### Working with Logging Tables

```csharp
var connector = new SqlConnector(connectionString);

// Create the logging tables (SystemLog and SystemExceptions)
connector.CreateLogTables();

// Log debug message
var debugResponse = connector.Debug(
    comment: "User login attempt",
    className: "UserService",
    methodName: "Login",
    executor: "jdoe",
    processType: "Authentication"
);

// Log info message
var infoResponse = connector.Info(
    comment: "Configuration loaded",
    className: "AppService",
    methodName: "Initialize",
    executor: "SYSTEM",
    processType: "Startup"
);

// Log warning with exception
try
{
    // Some operation that might have issues
    throw new Exception("Sample warning");
}
catch (Exception ex)
{
    var warnResponse = connector.Warning(
        comment: "Operation completed with warnings",
        className: "DataService",
        methodName: "ProcessData",
        executor: "admin",
        processType: "DataProcessing",
        Ex: ex
    );
}

// Log error with exception
try
{
    // Some operation that fails
    throw new InvalidOperationException("Sample error");
}
catch (Exception ex)
{
    var errorResponse = connector.Exception(
        comment: "Critical operation failed",
        className: "PaymentService",
        methodName: "ProcessPayment",
        executor: "jdoe",
        processType: "Payment",
        Ex: ex
    );
}

if (debugResponse.IsOK)
{
    Console.WriteLine($"Log entry created with ID: {debugResponse.Result}");
}
```

### Creating Tables from DataTable Schema

```csharp
var connector = new SqlConnector(connectionString);

// Create a DataTable with schema
DataTable schema = new DataTable("Products");
schema.Columns.Add("Id", typeof(int)).AutoIncrement = true;
schema.Columns.Add("Name", typeof(string)).MaxLength = 100;
schema.Columns.Add("Price", typeof(decimal));
schema.Columns.Add("Stock", typeof(int)).DefaultValue = 0;
schema.Columns.Add("IsActive", typeof(bool)).DefaultValue = true;
schema.Columns["Id"].AutoIncrementSeed = 1;
schema.Columns["Id"].AutoIncrementStep = 1;
schema.Columns["Name"].AllowDBNull = false;
schema.PrimaryKey = new DataColumn[] { schema.Columns["Id"] };

// Create the table in SQL Server
connector.CreateTableInSQL(schema);

// Or just get the script
string createScript = SqlConnector.GetTableScript(schema);
Console.WriteLine(createScript);
```

### Performance Monitoring

```csharp
var connector = new SqlConnector(connectionString);

// Set timeout (in seconds)
connector.TimeOut = 30;

// Execute query and measure performance
string query = "SELECT * FROM LargeTable";
var response = connector.ExecuteTable(query);

// Check execution time
TimeSpan duration = connector.ExecutionLapse;
Console.WriteLine($"Query took: {duration.TotalMilliseconds} ms");
Console.WriteLine($"Rows read: {connector.RowsRead}");

// For write operations
string insertSql = "INSERT INTO Logs (Message) VALUES ('Test')";
var insertResponse = connector.ExecuteNonQuery(insertSql);
Console.WriteLine($"Insert took: {connector.ExecutionLapse.TotalMilliseconds} ms");
Console.WriteLine($"Rows affected: {connector.RowsAffected}");
```

### Complete Repository Pattern Example

```csharp
using ZidUtilities.CommonCode.DataAccess;
using System.Data;
using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}

public class UserRepository
{
    private readonly SqlConnector _connector;

    public UserRepository(string connectionString)
    {
        _connector = new SqlConnector(connectionString);
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        string createTable = @"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
            BEGIN
                CREATE TABLE Users (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Username NVARCHAR(100) NOT NULL UNIQUE,
                    Email NVARCHAR(200) NOT NULL,
                    IsActive BIT DEFAULT 1,
                    CreatedDate DATETIME DEFAULT GETDATE()
                )
            END";

        _connector.ExecuteNonQuery(createTable);
    }

    public List<User> GetAllUsers()
    {
        string query = "SELECT Id, Username, Email, IsActive FROM Users WHERE IsActive = @active";
        var parameters = new Dictionary<string, string> { { "@active", "1" } };

        var response = _connector.ExecuteTable(query, parameters);

        if (!response.IsOK || response.Result == null)
            return new List<User>();

        var users = new List<User>();
        foreach (DataRow row in response.Result.Rows)
        {
            users.Add(new User
            {
                Id = Convert.ToInt32(row["Id"]),
                Username = row["Username"].ToString(),
                Email = row["Email"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"])
            });
        }
        return users;
    }

    public bool AddUser(string username, string email)
    {
        string query = "INSERT INTO Users (Username, Email) VALUES (@username, @email)";
        var parameters = new Dictionary<string, string>
        {
            { "@username", username },
            { "@email", email }
        };

        var response = _connector.ExecuteNonQuery(query, parameters, autoTransact: true);
        return response.IsOK;
    }

    public User GetUserById(int userId)
    {
        string query = "SELECT Id, Username, Email, IsActive FROM Users WHERE Id = @id";
        var parameters = new Dictionary<string, string> { { "@id", userId.ToString() } };

        var response = _connector.ExecuteTable(query, parameters);

        if (!response.IsOK || response.Result == null || response.Result.Rows.Count == 0)
            return null;

        DataRow row = response.Result.Rows[0];
        return new User
        {
            Id = Convert.ToInt32(row["Id"]),
            Username = row["Username"].ToString(),
            Email = row["Email"].ToString(),
            IsActive = Convert.ToBoolean(row["IsActive"])
        };
    }

    public bool UpdateUser(int userId, string email)
    {
        string query = "UPDATE Users SET Email = @email WHERE Id = @id";
        var parameters = new Dictionary<string, string>
        {
            { "@email", email },
            { "@id", userId.ToString() }
        };

        var response = _connector.ExecuteNonQuery(query, parameters, autoTransact: true);
        return response.IsOK && _connector.RowsAffected > 0;
    }

    public bool DeleteUser(int userId)
    {
        string query = "UPDATE Users SET IsActive = 0 WHERE Id = @id";
        var parameters = new Dictionary<string, string> { { "@id", userId.ToString() } };

        var response = _connector.ExecuteNonQuery(query, parameters, autoTransact: true);
        return response.IsOK && _connector.RowsAffected > 0;
    }

    public int GetUserCount()
    {
        string query = "SELECT COUNT(*) FROM Users WHERE IsActive = 1";
        var response = _connector.ExecuteScalar(query);

        if (!response.IsOK || response.Result == null)
            return 0;

        return Convert.ToInt32(response.Result);
    }
}

// Usage
var repo = new UserRepository("Server=localhost;Database=MyDb;Integrated Security=true;");
repo.AddUser("jdoe", "jdoe@example.com");
var users = repo.GetAllUsers();
var user = repo.GetUserById(1);
repo.UpdateUser(1, "newemail@example.com");
```

## Important Notes

### Parameters
- Parameters must be `Dictionary<string, string>`, not `Dictionary<string, object>`
- All values are passed as strings and converted by ADO.NET
- Always use parameterized queries to prevent SQL injection

### Transaction Management
- `BeginTransaction()`, `CommitTransaction()`, and `RollbackTransaction()` are **private** methods
- For single operations, use the `autoTransact` parameter: `ExecuteNonQuery(sql, params, autoTransact: true)`
- For multiple operations in a transaction, you need to manage the transaction externally or use SQL BEGIN TRANSACTION/COMMIT statements

### Error Handling
- Always check `response.IsOK` or `response.IsFailure`
- Access errors via `response.Errors` collection
- Check `connector.Error`, `connector.LastMessage`, `connector.LastException`, and `connector.LastSqlException` for detailed information

### Best Practices
1. Use parameterized queries for all user input
2. Check IsOK before accessing Result
3. Handle null values from Result properties
4. Use appropriate timeout values for long-running queries
5. Log errors using the built-in logging system
6. Monitor execution time with ExecutionLapse property
7. Use autoTransact for atomic operations

## Related Projects

- **CommonCode.DataAccess.Sqlite**: SQLite implementation with similar API
- **CommonCode.DataAccess.ActiveDirectory**: Active Directory data access
