# CommonCode.DataAccess.Sqlite

SQLite data access layer providing helper methods for executing database operations with ADO.NET and System.Data.SQLite.

## Features

- **SqliteConnector**: SQLite database connector with comprehensive query execution support
- **SqliteResponse<T>**: Generic response wrapper for all database operations
- **ErrorOnResponse**: Detailed error tracking and exception handling
- File-based database with zero configuration
- Synchronous and asynchronous query execution
- Automatic transaction management with autoTransact parameter
- Built-in logging system with SystemLog and SystemExceptions tables
- Execution timing and performance tracking
- Cross-platform compatibility

## Target Framework

.NET Framework 4.8

## Dependencies

- System.Data.SQLite
- CommonCode.DataAccess (for ErrorOnResponse)

## Installation

Install the System.Data.SQLite NuGet package:

```
Install-Package System.Data.SQLite
```

## Core Components

### SqliteConnector

The main class for executing SQLite commands. Provides methods for queries, non-queries, scalar operations, and more.

**Key Properties:**
- `Connection` - The underlying SQLiteConnection
- `ConnectionString` - Get/set connection string
- `LastMessage` - Status message ("OK" or error description)
- `Error` - Boolean indicating if last operation failed
- `RowsAffected` - Rows affected by last non-query
- `RowsRead` - Rows read by last query
- `LastException` - Last general exception thrown
- `LastSqliteException` - Last SQLite-specific exception
- `TimeOut` - Command timeout in seconds (0 = default)
- `OnExecution` - Boolean indicating if operation is in progress
- `ExecutionLapse` - TimeSpan of last operation duration
- `Server` - Database file path (DataSource)

### SqliteResponse<T>

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
- `SqliteResponse<T>.Successful(T result, string message = "Success")`
- `SqliteResponse<T>.Successful(List<T> results, string message = "Success")`
- `SqliteResponse<T>.Failure(string message, Exception ex = null)`

## Usage Examples

### Creating a Connection

```csharp
using ZidUtilities.CommonCode.DataAccess.Sqlite;

// Using file path (connection string is created automatically)
string dbPath = @"C:\data\myapp.db";
var connector = new SqliteConnector(dbPath);

// Or using a full connection string
string connStr = $"Data Source={dbPath};Version=3;";
var connector2 = new SqliteConnector(connStr);

// In-memory database (great for testing)
var memConnector = new SqliteConnector(":memory:");

// Test the connection
if (connector.TestConnection())
{
    Console.WriteLine("Connected successfully!");
    Console.WriteLine($"Database: {connector.Server}");
}
else
{
    Console.WriteLine($"Connection failed: {connector.LastMessage}");
}
```

### Creating Tables

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

string createTableSql = @"
    CREATE TABLE IF NOT EXISTS Users (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Username TEXT NOT NULL UNIQUE,
        Email TEXT NOT NULL,
        IsActive INTEGER DEFAULT 1,
        CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP
    )";

var response = connector.ExecuteNonQuery(createTableSql);

if (response.IsOK)
{
    Console.WriteLine("Table created successfully!");
}
else
{
    Console.WriteLine($"Error creating table: {response.Message}");
}
```

### ExecuteNonQuery - INSERT, UPDATE, DELETE

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

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
}

// INSERT with parameters (note: Dictionary<string, string>)
string paramSql = "INSERT INTO Users (Username, Email) VALUES (@username, @email)";
var parameters = new Dictionary<string, string>
{
    { "@username", "janedoe" },
    { "@email", "jane@example.com" }
};

var response2 = connector.ExecuteNonQuery(paramSql, parameters);

// Get the last inserted row ID
string lastIdQuery = "SELECT last_insert_rowid()";
var idResponse = connector.ExecuteScalar(lastIdQuery);
long lastId = Convert.ToInt64(idResponse.Result);
Console.WriteLine($"Inserted row ID: {lastId}");

// UPDATE with parameters
string updateSql = "UPDATE Users SET Email = @email WHERE Username = @username";
var updateParams = new Dictionary<string, string>
{
    { "@email", "newemail@example.com" },
    { "@username", "jdoe" }
};

var response3 = connector.ExecuteNonQuery(updateSql, updateParams);

// DELETE with parameters
string deleteSql = "DELETE FROM Users WHERE Id = @id";
var deleteParams = new Dictionary<string, string> { { "@id", "1" } };
var response4 = connector.ExecuteNonQuery(deleteSql, deleteParams);
```

### ExecuteTable - Query and Return DataTable

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

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
string paramQuery = "SELECT * FROM Users WHERE IsActive = @active";
var queryParams = new Dictionary<string, string>
{
    { "@active", "1" }
};

var response2 = connector.ExecuteTable(paramQuery, queryParams);

// With custom table name
var response3 = connector.ExecuteTable(query, null, false, "MyUsers");
Console.WriteLine($"Table name: {response3.Result.TableName}");

// Query with WHERE clause
string searchQuery = "SELECT * FROM Users WHERE Username LIKE @pattern ORDER BY Username";
var searchParams = new Dictionary<string, string> { { "@pattern", "%doe%" } };
var response4 = connector.ExecuteTable(searchQuery, searchParams);
```

### ExecuteDataSet - Multiple Result Sets

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

// SQLite executes multiple statements in one command
string multipleSql = @"
    SELECT * FROM Users;
    SELECT * FROM Orders;
    SELECT * FROM Products;";

var response = connector.ExecuteDataSet(multipleSql);

if (response.IsOK && response.Result != null)
{
    DataSet ds = response.Result;

    Console.WriteLine($"Retrieved {ds.Tables.Count} tables");

    if (ds.Tables.Count >= 3)
    {
        DataTable users = ds.Tables[0];
        DataTable orders = ds.Tables[1];
        DataTable products = ds.Tables[2];

        Console.WriteLine($"Users: {users.Rows.Count}");
        Console.WriteLine($"Orders: {orders.Rows.Count}");
        Console.WriteLine($"Products: {products.Rows.Count}");
    }
}
```

### ExecuteScalar - Single Value

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

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
    string maxDate = response2.Result.ToString();
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

// Check if table exists
string tableExistsQuery = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@tableName";
var tableParams = new Dictionary<string, string> { { "@tableName", "Users" } };
var existsResponse = connector.ExecuteScalar(tableExistsQuery, tableParams);
bool exists = Convert.ToInt32(existsResponse.Result) > 0;
```

### ExecuteColumn - Single Column as List

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

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

// Get all table names in database
string tablesQuery = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";
var tablesResponse = connector.ExecuteColumn(tablesQuery);
```

### Automatic Transactions with autoTransact

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

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
```

### Working with SQLite-Specific Features

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

// Get database schema information
string pragmaQuery = "PRAGMA table_info(Users)";
var schemaResponse = connector.ExecuteTable(pragmaQuery);

if (schemaResponse.IsOK && schemaResponse.Result != null)
{
    foreach (DataRow row in schemaResponse.Result.Rows)
    {
        Console.WriteLine($"Column: {row["name"]}, Type: {row["type"]}, NotNull: {row["notnull"]}");
    }
}

// Get all tables
string tablesQuery = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";
var tablesResponse = connector.ExecuteColumn(tablesQuery);

// Vacuum database (reclaim space)
var vacuumResponse = connector.ExecuteNonQuery("VACUUM");

// Enable foreign keys (SQLite-specific)
var fkResponse = connector.ExecuteNonQuery("PRAGMA foreign_keys = ON");

// Set journal mode (WAL is recommended for concurrent access)
var walResponse = connector.ExecuteNonQuery("PRAGMA journal_mode = WAL");

// Set synchronous mode
var syncResponse = connector.ExecuteNonQuery("PRAGMA synchronous = NORMAL");
```

### Error Handling Patterns

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

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
            }
        }
    }
}

// Alternative: Check Error property on connector
if (connector.Error)
{
    Console.WriteLine($"Connector error: {connector.LastMessage}");

    // Access specific exception types
    if (connector.LastSqliteException != null)
    {
        Console.WriteLine($"SQLite Error: {connector.LastSqliteException.Message}");
        Console.WriteLine($"Result Code: {connector.LastSqliteException.ResultCode}");
    }
    else if (connector.LastException != null)
    {
        Console.WriteLine($"General Exception: {connector.LastException.Message}");
    }
}
```

### Asynchronous Execution

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

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
```

### Working with Logging Tables

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

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
```

### Creating Tables from DataTable Schema

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

// Create a DataTable with schema
DataTable schema = new DataTable("Products");
schema.Columns.Add("Id", typeof(int)).AutoIncrement = true;
schema.Columns.Add("Name", typeof(string));
schema.Columns.Add("Price", typeof(decimal));
schema.Columns.Add("Stock", typeof(int)).DefaultValue = 0;
schema.Columns.Add("IsActive", typeof(bool)).DefaultValue = true;
schema.Columns["Id"].AutoIncrementSeed = 1;
schema.Columns["Id"].AutoIncrementStep = 1;
schema.Columns["Name"].AllowDBNull = false;
schema.PrimaryKey = new DataColumn[] { schema.Columns["Id"] };

// Create the table in SQLite
connector.CreateTableInSQL(schema);

// Or just get the script
string createScript = SqliteConnector.GetTableScript(schema);
Console.WriteLine(createScript);
```

### Bulk Insert Operations

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

// Bulk insert with transaction for better performance
string insertSql = "INSERT INTO Users (Username, Email) VALUES (@username, @email)";

var users = new[]
{
    new { Username = "user1", Email = "user1@example.com" },
    new { Username = "user2", Email = "user2@example.com" },
    new { Username = "user3", Email = "user3@example.com" }
};

// Manual transaction for multiple operations
// Note: BeginTransaction, CommitTransaction, RollbackTransaction are private
// So we use SQL statements directly
connector.ExecuteNonQuery("BEGIN TRANSACTION");

try
{
    foreach (var user in users)
    {
        var parameters = new Dictionary<string, string>
        {
            { "@username", user.Username },
            { "@email", user.Email }
        };

        var response = connector.ExecuteNonQuery(insertSql, parameters);
        if (!response.IsOK)
        {
            throw new Exception($"Failed to insert {user.Username}");
        }
    }

    connector.ExecuteNonQuery("COMMIT");
    Console.WriteLine($"Inserted {users.Length} records");
}
catch (Exception ex)
{
    connector.ExecuteNonQuery("ROLLBACK");
    Console.WriteLine($"Bulk insert failed: {ex.Message}");
}
```

### Performance Monitoring

```csharp
var connector = new SqliteConnector(@"C:\data\myapp.db");

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
using ZidUtilities.CommonCode.DataAccess.Sqlite;
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
    private readonly SqliteConnector _connector;

    public UserRepository(string dbPath)
    {
        _connector = new SqliteConnector(dbPath);
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        string createTable = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Email TEXT NOT NULL,
                IsActive INTEGER DEFAULT 1,
                CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP
            )";

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
                IsActive = Convert.ToInt32(row["IsActive"]) == 1
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
            IsActive = Convert.ToInt32(row["IsActive"]) == 1
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
        // Soft delete
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
var repo = new UserRepository(@"C:\data\myapp.db");
repo.AddUser("jdoe", "jdoe@example.com");
var users = repo.GetAllUsers();
var user = repo.GetUserById(1);
repo.UpdateUser(1, "newemail@example.com");
```

## SQLite-Specific Considerations

### Data Types
- SQLite uses dynamic typing: INTEGER, TEXT, REAL, BLOB, NULL
- Booleans are stored as INTEGER (0 = false, 1 = true)
- Dates are stored as TEXT in ISO8601 format
- Use `Convert.ToInt32(row["IsActive"]) == 1` for boolean columns

### Concurrent Access
- Enable WAL mode for better concurrent reads/writes:
  ```csharp
  connector.ExecuteNonQuery("PRAGMA journal_mode = WAL");
  ```

### Performance Optimization
1. Use transactions for bulk operations
2. Create indexes on frequently queried columns
3. Use appropriate PRAGMA settings
4. Vacuum database periodically to reclaim space

### File Management
- Database file is created automatically if it doesn't exist
- Use `:memory:` for in-memory databases (perfect for testing)
- Database file can be copied/backed up while closed

## Important Notes

### Parameters
- Parameters must be `Dictionary<string, string>`, not `Dictionary<string, object>`
- All values are passed as strings and converted by ADO.NET
- Always use parameterized queries to prevent SQL injection

### Transaction Management
- `BeginTransaction()`, `CommitTransaction()`, and `RollbackTransaction()` are **private** methods
- For single operations, use the `autoTransact` parameter
- For multiple operations, use SQL statements: "BEGIN TRANSACTION", "COMMIT", "ROLLBACK"

### Error Handling
- Always check `response.IsOK` or `response.IsFailure`
- Access errors via `response.Errors` collection
- Check `connector.Error`, `connector.LastMessage`, `connector.LastException`, and `connector.LastSqliteException`

### Best Practices
1. Use parameterized queries for all user input
2. Check IsOK before accessing Result
3. Handle null values from Result properties
4. Enable WAL mode for concurrent access
5. Use transactions for bulk operations
6. Create indexes for frequently queried columns
7. Vacuum database periodically
8. Back up database files regularly

## Related Projects

- **CommonCode.DataAccess**: SQL Server implementation with similar API
- **CommonCode.DataAccess.ActiveDirectory**: Active Directory data access
