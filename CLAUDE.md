# ZidUtilities Solution Documentation

This document provides comprehensive documentation of the ZidUtilities solution structure, functionality, and tests for efficient context management.

**Last Updated:** 2025-12-17

---

## Table of Contents

1. [CommonCode](#1-commoncode)
2. [CommonCode.DataAccess](#2-commonccodedataaccess)
3. [CommonCode.DataAccess.ActiveDirectory](#3-commonccodedataaccessactivedirectory) (Pending)
4. [CommonCode.Files](#4-commoncodefiles) (Pending)
5. [CommonCode.ICSharpTextEditor](#5-commoncodecicsharptexteditor) (Pending)
6. [CommonCode.Win](#6-commoncodewin) (Pending)

---

## 1. CommonCode

### Overview
The **CommonCode** project is a core utility library providing foundational classes and extensions for .NET Framework 4.8 applications.

### Project Details
- **Location:** `D:\Just For Fun\ZidUtilities\CommonCode\`
- **Assembly Name:** ZidUtilities.CommonCode
- **Target Framework:** .NET Framework 4.8
- **Output Type:** Library

### Dependencies
- System core libraries (System, System.Core, System.Data, System.Xml)
- System.Drawing
- System.Web (for HTML encoding utilities)

### Key Namespaces

#### ZidUtilities.CommonCode (Root)
Core utilities including validation, encryption, extensions, serialization.

#### ZidUtilities.CommonCode.DataComparison
DataTable comparison engine for detailed row-by-row and cell-by-cell comparisons.

#### ZidUtilities.CommonCode.DifferenceEngine
Text/line diffing algorithm (similar to unified diff).

#### ZidUtilities.CommonCode.ServerFiltering
Server-side filtering abstractions (Kendo UI compatible).

### Main Classes

#### Validation and Checking

**Check.cs**
- Static validation methods for parameter checking
- Range validation (RangeMinExclusive, RangeMaxExclusive, RangeInclusive, RangeExclusive)
- Null checking (NotNull)
- Empty validation (NotEmpty for strings, collections, arrays, GUIDs)
- Boolean condition validation (True, NotTrue)
- Type checking (TypeOf)
- Supports: int, double, float, decimal, DateTime, strings, collections

#### Cryptography and Security

**Crypter.cs**
- Symmetric encryption/decryption and hashing utilities
- **Encryption Algorithms:** DES (deprecated), Triple_Des (legacy), Rijndael, AES (recommended), AES_GCM, ChaCha20_Poly1305
- String and file encryption/decryption with streaming support
- Hash functions: MD5, SHA256, SHA512
- Modern PBKDF2 key derivation (100,000 iterations default)
- Legacy compatibility mode

**PasswordGenerator.cs**
- Generate secure random passwords with configurable rules
- Rule-based password generation (uppercase, lowercase, digits, special chars)
- Configurable quantity per character type
- Shuffled output for unpredictability

#### Data Comparison

**DataComparer.cs** (ZidUtilities.CommonCode.DataComparison)
- Comprehensive DataTable comparison engine
- Row-by-row and cell-by-cell comparison
- Composite primary key support
- Case-sensitive/insensitive comparison
- Column-specific comparison rules
- Tolerance-based numeric comparison
- Output: XML, HTML track files
- **Supporting Classes:** RowComparison, CellDifference, ColumnComparisonDetail, CommentRow

**DiffEngine** (ZidUtilities.CommonCode.DifferenceEngine)
- Generic text/string/line diffing engine
- **Key Classes:** DiffEngine, DiffResultSpan, DiffState, IDiffList
- **Implementations:** DiffListString, DiffListText, DiffListTextFile, TextLine

#### Serialization

**XmlSerialization.cs**
- Custom XML serialization engine for complex object graphs
- XmlObjectSerializer and XmlObjectDeserializer
- Handles circular references and object graphs
- Type cache for compact XML
- Support for IXmlSerializable interface
- Handles collections, dictionaries, arrays, primitives, enums

**SimpleDictionaryPersister.cs**
- Simple settings persistence with encryption support
- Stores string key-value pairs
- Formats: XML, JSON, PlainText
- Optional AES encryption per value
- Save locations: ApplicationFolder, UserAppDataFolder, CommonAppDataFolder, CustomFolder

#### Image Processing

**ImageHelper.cs**
- Image manipulation and analysis utilities
- **Cropping:** Rectangle-based cropping
- **Scaling:** Aspect-ratio-preserving resize
- **Format conversion:** RGB, grayscale, bitonal (1bpp)
- **Metadata extraction:** Pixel format, DPI, color space, palette analysis
- **Codec detection:** MIME type, file extensions
- **JPEG quality control:** Thumbnail generation
- **Grayscale detection:** Deep palette scanning

#### Extensions

**Extensions.cs** (2000+ lines)
Central extension method library enhancing core .NET types.

**Key Categories:**
- **Boolean Extensions:** BoolAsString()
- **Byte Array Extensions:** AsImage()
- **Color Extensions:** ColorToString(), HalfMix(), GetHighContrastColor(), Variate()
- **DataTable Extensions:** SaveToCsv(), GetCreateTableSql()
- **DateTime Extensions:** AD tick conversion, date comparison, BeginningOfTheMonth(), ChangeTime(), SafeStringDate()
- **String Extensions:** Null/empty checks, truncation, padding, case conversion, CSV field escaping, token generation, serialization helpers (XML, Base64)
- **Collection Extensions:** Shuffle, distinct, grouping, case-insensitive contains
- **IEnumerable Extensions:** Batch processing, flattening, chunking
- **Regex Extensions:** Pattern matching helpers
- **SQL Extensions:** Connection string parsing, query building
- **Expression/Lambda Extensions:** Property name extraction

#### Tokenization

**TokenList.cs**
- SQL-focused tokenizer
- Token types: RESERVED, DATATYPE, VARIABLE, TEMPTABLE, BLOCKSTART, BLOCKEND
- SQL keyword recognition
- Block folding support

**GenericTokenList.cs**
- Generic language tokenizer
- Supports: C#, JavaScript, HTML, CSS, XML, JSON
- Token types: EMPTYSPACE, COMMA, DOT, SEMICOLON, OPERATOR, NUMBER, STRING

#### Server Filtering

**ServerFiltering Namespace**
- Server-side data filtering abstractions (Kendo UI compatible)
- FilterDescription: Filter conditions
- SortDescription: Sort criteria
- IFilterableObject, KendoFilterableObject: Data source wrappers
- FilterMerge: Merge multiple filters

### Design Patterns

1. **Extension Method Pattern:** Extensive use throughout Extensions.cs
2. **Strategy Pattern:** Multiple encryption algorithms, serialization formats
3. **Facade Pattern:** Complex functionality wrapped in simple APIs

### Tests

#### Tester (Console Application)
**Location:** `D:\Just For Fun\ZidUtilities\Tester\`

**Test Classes:**
- **CrypterUsageExample.cs:** Demonstrates Crypter usage patterns
- **SimpleDictionaryPersisterTests.cs:** Tests SimpleDictionaryPersister functionality
- **DataExporterImporterTests.cs:** 40+ test suite for data export/import (XLSX, CSV, TXT)
- **ExcelStylesDemo.cs:** Generates demo files for all 14 Excel styles

#### TesterWin (WinForms Application)
**Location:** `D:\Just For Fun\ZidUtilities\TesterWin\`

**Test Forms:**
- **FormMainMenu:** Main test launcher (entry point)
- Various Form1-9 plus named forms for testing different components

---

## 2. CommonCode.DataAccess

### Overview
Provides robust data access layer for SQL Server and SQLite databases using ADO.NET. Consists of two related projects with consistent API across different database providers.

### Project Details

#### CommonCode.DataAccess (SQL Server)
- **Location:** `D:\Just For Fun\ZidUtilities\CommonCode.DataAccess\`
- **Assembly Name:** ZidUtilities.CommonCode.DataAccess
- **Target Framework:** .NET Framework 4.8
- **Dependencies:** System.Data (SqlClient), System.Core, System.Xml

#### CommonCode.DataAccess.Sqlite
- **Location:** `D:\Just For Fun\ZidUtilities\CommonCode.DataAccess.Sqlite\`
- **Assembly Name:** ZidUtilities.CommonCode.DataAccess.Sqlite
- **Target Framework:** .NET Framework 4.8
- **Dependencies:** System.Data.SQLite (NuGet 2.0.2), System.Transactions, System.Data

### Key Namespaces

#### ZidUtilities.CommonCode.DataAccess
SQL Server data access operations.

#### ZidUtilities.CommonCode.DataAccess.Sqlite
SQLite-specific implementation with identical API patterns.

### Core Classes

#### SqlConnector (SQL Server)
Comprehensive SQL Server database connector providing ADO.NET wrapper methods.

**Key Properties:**
- `Connection: SqlConnection` - Underlying connection
- `ConnectionString: string` - Database connection string
- `LastMessage: string` - Status of last operation
- `Error: bool` - Indicates if last operation failed
- `RowsAffected: int` - Rows modified by last non-query
- `RowsRead: int` - Rows retrieved by last query
- `LastException: Exception` - Last general exception
- `LastSqlException: SqlException` - Last SQL-specific exception
- `TimeOut: int` - Command timeout in seconds
- `OnExecution: bool` - Indicates if operation is in progress
- `ExecutionLapse: TimeSpan` - Duration of last operation
- `Transaction: SqlTransaction` - Current transaction
- `Server: string` - SQL Server instance name
- `DataBase: string` - Current database name

**Query Execution Methods:**

1. **ExecuteNonQuery(sql, parameters, autoTransact)** → SqlResponse<int>
   - Executes INSERT, UPDATE, DELETE commands
   - Returns affected row count
   - Supports parameterized queries
   - Optional automatic transaction handling

2. **ExecuteScalar(sql, parameters, autoTransact)** → SqlResponse<object>
   - Returns single value (first column, first row)
   - Used for COUNT, MAX, MIN, single value queries

3. **ExecuteTable(sql, parameters, autoTransact, tableName)** → SqlResponse<DataTable>
   - Returns first result set as DataTable
   - Most commonly used for SELECT queries

4. **ExecuteDataSet(sql, parameters, autoTransact)** → SqlResponse<DataSet>
   - Returns multiple result sets
   - Handles complex queries with multiple SELECT statements

5. **ExecuteColumn(sql, parameters, autoTransact)** → SqlResponse<List<string>>
   - Returns single column as List<string>
   - Convenient for dropdown/lookup data

**Schema & DDL Operations:**
- **CreateTableInSQL(DataTable):** Creates SQL Server table from DataTable schema
- **GetTableScript(DataTable):** Generates CREATE TABLE script with type mapping

**Built-in Database Logging System:**
- **CreateLogTables():** Creates SystemLog and SystemExceptions tables
- **Debug/Info/Warning/Exception methods:** Log to database
- **RegisterLog/RegisterException:** Core logging methods

**SystemLog Table Schema:**
- Id, Comment, ClassName, MethodName, Executor, ExecutionTime, LogLevel, ProcessType, Exception (FK)

**SystemExceptions Table Schema:**
- Id, Message, StackTrace, Source, ExecutionTime

**Asynchronous Execution:**
- **AsyncExecuteDataSet(query):** Starts async query execution
- **CancelExecute():** Requests cancellation
- **ExtremeStop():** Forces termination
- **Events:** StartExecution, FinishExecution

**Connection Management:**
- **TestConnection():** Tests connection open/close
- **NewConnection:** Property that creates new connection instance

#### SqlResponse<T> (Generic Response Wrapper)
Standardized response container for all database operations.

**Properties:**
- `Result: T` - Single result value
- `Results: List<T>` - Multiple results
- `Message: string` - Status message
- `Errors: List<ErrorOnResponse>` - Error collection
- `IsOK: bool` - Success indicator
- `IsFailure: bool` - Failure indicator
- `ErrorOccured: bool` - Error presence check
- `ExecutionTime: DateTime` - Timestamp
- `ExecutionTimeString: string` - Formatted timestamp

**Methods:**
- AddResult, AddResults, Success, Fail (various overloads)

**Static Factory Methods:**
- `SqlResponse<T>.Successful(T result)`
- `SqlResponse<T>.Successful(List<T> results)`
- `SqlResponse<T>.Failure(string message, Exception)`

**Indexer Support:**
```csharp
response["result"]  // Access Result property
response["results"] // Access Results property
response["message"] // Access Message property
```

#### ErrorOnResponse
Captures error details for diagnostic purposes.

**Properties:**
- `ExecutionTime: DateTime`
- `Message: string`
- `Exception: Exception`

#### SqliteConnector
SQLite-specific implementation with identical API patterns to SqlConnector.

**Key Differences:**
- Uses SQLiteConnection, SQLiteCommand, SQLiteDataAdapter
- Returns SqliteResponse<T> instead of SqlResponse<T>
- Schema generation adapted for SQLite type system
- AUTOINCREMENT syntax for identity columns
- File-based database (Data Source=path)
- Support for :memory: in-memory databases

### Database Providers Supported

#### SQL Server
- **Provider:** System.Data.SqlClient
- **Connection String Example:** `Server=.\\SQLSERVER;Database=MyDb;Integrated Security=True`
- **Features:** Full T-SQL support, built-in logging, transactions, async execution

#### SQLite
- **Provider:** System.Data.SQLite (NuGet)
- **Connection String Example:** `Data Source=C:\data\myapp.db;Version=3;`
- **Features:** File-based or in-memory, same API as SQL Server, zero-configuration

### Key Patterns

1. **Response Pattern:** All operations return SqlResponse<T> wrapper
2. **Factory Pattern:** Static factory methods for response creation
3. **Repository Pattern Compatible:** Designed to be wrapped in repository classes
4. **Parameterized Queries:** Dictionary<string, string> for parameters
5. **Automatic Transaction Management:** Optional autoTransact parameter
6. **Error Aggregation:** Errors collected in response
7. **Performance Tracking:** Built-in execution timing
8. **Async/Event Pattern:** Event-driven asynchronous execution

### SQL Generation & Type Mapping

**Type Mappings (SQL Server):**
```
Boolean  → bit          String   → nvarchar(n)
Byte     → smallint     DateTime → datetime
Int16    → smallint     Single   → single
Int32    → int          Double   → double
Int64    → bigint       Decimal  → decimal(18,6)
```

**Type Mappings (SQLite):**
```
Boolean/Int* → INTEGER  Single/Double → REAL
String       → TEXT     Decimal       → NUMERIC
DateTime     → TEXT
```

### Tests

#### TesterWin Tests

**Form9.cs** (Lines 57-70)
- Tests SqlConnector with SQL Server Northwind database
- Connection: Main\SQLSERVER, Northwind database
- Query: `SELECT * FROM dbo.Employees`
- Tests ExecuteTable method, SqlResponse<DataTable> handling, error handling
- DataTable binding to ZidGrid component

**FormVeinsTest.cs**
- Comprehensive test of SqliteConnector with CRUD operations
- Database: IconCommanderDb.db (SQLite file)
- Tables: Veins, Collections
- Tests: Connection, ExecuteTable, foreign key lookups, JOIN queries, INSERT/UPDATE/DELETE operations, transaction handling
- Integration with UIGenerator for CRUD forms

**Test Methods:**
- `InitializeDatabase()` - Connection setup
- `LoadCollectionLookup()` - Foreign key data loading
- `LoadVeinsData()` - Complex JOIN query testing
- `btnAdd_Click()` - INSERT test with UIGenerator
- `btnEdit_Click()` - UPDATE test
- `btnDelete_Click()` - DELETE test

### Key Strengths

1. Consistent API across SQL Server and SQLite
2. Comprehensive error handling with detailed error information
3. Built-in logging system (database-backed)
4. Transaction support (automatic or manual)
5. Performance tracking (built-in execution timing)
6. Async support with event-driven operations
7. Type safety with generic response types
8. Well documented (extensive XML comments and README files)
9. Schema generation from DataTable definitions
10. Parameterized queries (SQL injection prevention)

### Limitations

1. Parameters as strings (Dictionary<string, string>) - requires conversion
2. Private transaction methods - manual management requires SQL statements
3. No ORM features - manual object mapping required
4. No LINQ support - raw SQL only
5. Synchronous primary - async support is limited
6. No explicit connection pooling management

### Documentation

- **CommonCode.DataAccess\README.md** (658 lines): Comprehensive usage guide with examples
- **CommonCode.DataAccess.Sqlite\README.md** (820 lines): SQLite-specific guide

### NuGet Packages

- ZidUtilities.CommonCode.DataAccess (1.0.0)
- ZidUtilities.CommonCode.DataAccess.Sqlite (1.0.0)

---

## 3. CommonCode.DataAccess.ActiveDirectory

### Overview
Comprehensive Active Directory management library providing CRUD operations (except Delete), authentication, group management, and user provisioning for enterprise AD integration.

### Project Details
- **Location:** `D:\Just For Fun\ZidUtilities\CommonCode.DataAccess.ActiveDirectory\`
- **Assembly Name:** ZidUtilities.CommonCode.DataAccess.ActiveDirectory
- **Target Framework:** .NET Framework 4.8
- **Version:** 1.0.0.0
- **License:** MIT
- **Author:** Gonzalo Urbiola

### Dependencies
- **System.DirectoryServices** - Core AD access functionality
- **System.DirectoryServices.AccountManagement** - Higher-level AD account management
- **System.DirectoryServices.ActiveDirectory** - Domain/forest operations
- **System.Drawing** - For handling user photos/images
- **System.Data** - DataTable operations for query results
- **System.Xml** - XML serialization support

### Key Namespaces

#### ZidUtilities.CommonCode.DataAccess.ActiveDirectory
Primary namespace containing all AD functionality, subdivided by responsibility:
- **Management:** AdManager (main operations)
- **Filtering:** AdManagerFilter (LDAP filter construction)
- **Path Management:** AdManagerPath (OU path handling)
- **Data Models:** BasicEmployee, AdKeyAttributes
- **Attribute Handling:** AdAttribute
- **Type Definitions:** Enums, delegates

### Core Classes

#### AdManager.cs (Primary Class)
Central Active Directory management class providing comprehensive AD operations.

**Key Properties:**
- `Domain` - DNS or NetBIOS domain identifier
- `DomainController` - Specific DC to connect to
- `Attributes` - List of AD attributes to retrieve in queries
- `Paths` - List of OUs where searches will execute
- `Filter` - LDAP filter for queries
- `Exception` - Last exception encountered
- `LastMessage` - Last operation status message

**Search and Query Operations:**
- `QueryActiveDirectory()` - Executes LDAP queries with filters, returns DataTable
- `GetBasicEmployee(string userName)` - Retrieves basic employee info by username
- `GetBasicEmployeeWithFilter(string value, FilterableAttribute field)` - Search by custom field
- `GetBasicEmployees()` - Retrieves all employees in configured paths
- `CalculateManagers()` - Computes manager relationships from DN/manager attributes

**Attribute Update Operations:**
- `UpdateUserProperty(...)` - Single attribute update (4 overloads: by DN or DirectoryEntry, with/without AdAttribute)
- `AttributeBatchUpdate(...)` - Batch update (2 overloads: by DN or DirectoryEntry reference)

**Authentication Operations:**
- `IsValidCredential(string user, string password)` - Validates user credentials against AD

**Account Management Operations:**
- `ChangePassword(string userName, string password)` - Changes user password
- `IsAccountLocked(string userName)` - Checks if account is locked
- `UnlockAccount(string userName)` - Unlocks user account
- `EnableAccount(string userName)` - Enables disabled account
- `DisableAccount(string userName)` - Disables active account ⚠️ **BUG:** Line 545 sets Enabled=true instead of false
- `ForcePasswordChangeAtNextLogon(...)` - Static method to force password change

**Group Membership Operations:**
- `DoesEmployeeBelongsToActiveDirectoryGroup(string userName, string groupName)` - Checks group membership
- `GetUserMembership(string userName)` - Gets all groups user belongs to
- `AddGroupMembership(string userName, string groupName)` - Adds user to group
- `RemoveGroupMembership(string userName, string groupName)` - Removes user from group

**User Photo Operations:**
- `GetUserPicture(string userName, bool thumbnailPhoto = true)` - Gets user photo by CN
- `GetUserPictureByUserName(string userName, bool thumbnailPhoto = true)` - Gets user photo by sAMAccountName
- `AddPictureByUserName(string fileNumber, string photoFileName)` - Uploads photo by username
- `AddPictureToUser(string dN, byte[] photoBinaryData)` - Uploads photo by DN

**Advanced User Operations:**
- `MoveUserToOu(string username, string targetOuDn, string domainName)` - Static method to move user to different OU
- `CopyAdUser(string sourceUserName, string newUserName, string newPassword, string targetOU)` - Copies user and memberships

#### AdManagerFilter.cs
LDAP filter builder with fluent API for complex query construction.

**Key Methods:**
- `CreateFilter()` - Static factory method
- `FilterBy(FilterableAttribute attr, FilterComparer cmp, string value)` - Adds attribute filter
- `And()` - Switches to AND grouping
- `Or()` - Switches to OR grouping
- `AccountIs(FilterAccountStatus status)` - Filters by account status (Active/Disabled/Locked)
- `CleanFilter()` - Resets filter
- `ToString()` - Generates final LDAP filter string

**LDAP Query Pattern:**
Generates filters in format: `(&(objectCategory=user)(ObjectClass=user){custom filters})`

**Bitwise Matching for UserAccountControl:**
- Disabled: `(userAccountControl:1.2.840.113556.1.4.803:=2)`
- Locked: `(userAccountControl:1.2.840.113556.1.4.803:=16)`
- Active: `(userAccountControl:1.2.840.113556.1.4.803:=512)`

#### AdManagerPath.cs
Represents OU search paths for AD queries.

**Features:**
- Constructs LDAP DN paths from OU segments
- `Type` property for categorizing results by source path
- `ToString()` generates full DN
- ⚠️ **Hardcoded Domain:** Currently hardcoded to "DC=faradayfuture,DC=com"

**Constructor:** `AdManagerPath(string type, params string[] folders)`

**Example:**
```csharp
var path = new AdManagerPath("Worker", "FF-Users");
// Generates: "OU=FF-Users,DC=faradayfuture,DC=com"
```

#### AdAttribute.cs
Represents an AD attribute with type-safe value conversion and custom parsing.

**Key Features:**
- Maps AD attribute names to friendly aliases
- Type conversion support (String, Integer, DateTime, AccountStatus, StringList, Special)
- Custom parser delegate support for special attributes
- `Evaluate(object raw)` - Converts raw AD values to strings
- Handles AD date format (ticks since 1/1/1601)
- UserAccountControl flag parsing
- Support for update operations via `UpdateValue` and `UpdateListValue` properties

**Constructor:** `AdAttribute(string adName, string alias = "", AdType type = AdType.String, GetAttributeValue parser = null)`

#### AdKeyAttributes.cs
Simple DTO for storing employee key identifiers.

**Properties:**
- `DistinguishedName` - LDAP DN
- `EmployeeId` - HR system ID (Workday)
- `UserName` - sAMAccountName

#### BasicEmployee.cs
Lightweight DTO for common employee information.

**Properties:**
- EmployeeId, DistinguishedName, UserName, FirstName, LastName, Email, Title, Status

#### Enums.cs
Defines all enumerations used throughout the library.

**Key Enumerations:**

**AdType:** String, Integer, DateTime, AccountStatus, StringList, Special

**FilterComparer:** Equals, StartsWith, EndsWith, Contains

**FilterableAttribute (16 attributes):**
- FirstName (givenName), LastName (sn), DisplayName (displayName)
- Title (title), EmployeeNumber (employeeNumber)
- ManagerId/ManagerName (manager), Department (department), DepartmentNumber (departmentNumber)
- Location (physicalDeliveryOfficeName), Email (mail), Division (division)
- UserName (sAMAccountName), Name (name)

**FilterAccountStatus:** Active, Disabled, Locked

**UserAccountControl (Flags enum with 21 flags):**
- ACCOUNTDISABLE (0x00000002)
- LOCKOUT (0x00000010)
- NORMAL_ACCOUNT (0x00000200)
- DONT_EXPIRE_PASSWD (0x00010000)
- PASSWORD_EXPIRED (0x00800000)
- And 16 more flags...

**GetAttributeValue (Delegate):** Template for custom attribute parsing functions

### Active Directory Operations Supported

**Search Operations:**
- LDAP query execution with custom filters
- Multi-OU searches
- Attribute-based filtering (16 filterable attributes)
- Complex filter building (AND/OR logic)
- Account status filtering
- Pagination support (PageSize = 10000)
- Result mapping to DataTable with type conversion

**Create Operations:**
- User creation via `CopyAdUser()` (creates by copying existing user)
- Group membership creation

**Update Operations:**
- Single attribute updates (multiple overloads)
- Batch attribute updates
- User photo updates (jpegPhoto, thumbnailPhoto)
- Password changes
- Account enable/disable
- Account unlock
- Force password change at next logon
- Group membership modifications

**Delete Operations:**
- ⚠️ No explicit delete methods found (removal from groups only)

**Authentication Operations:**
- Credential validation using PrincipalContext
- Group membership checking

### LDAP Query Patterns

**Base Filter Pattern:**
```
(&(objectCategory=user)(ObjectClass=user){custom filters})
```

**Filter Examples:**
```
Equality:    (sAMAccountName=jdoe)
Contains:    (displayName=*John*)
StartsWith:  (mail=john*)
EndsWith:    (sn=*son)
```

**Connection Pattern:**
```
LDAP://{DomainController}/{OU Path}
Example: LDAP://dc01.contoso.com/OU=Users,DC=contoso,DC=com
```

**DirectoryEntry Usage:**
```csharp
DirectoryEntry searchRoot = new DirectoryEntry(searchBase);
DirectorySearcher searcher = new DirectorySearcher(searchRoot, filter, attributeList);
searcher.PageSize = 10000;
SearchResultCollection results = searcher.FindAll();
```

### Authentication Methods

**Primary Authentication:**
- Method: `PrincipalContext.ValidateCredentials(username, password)`
- Context Type: ContextType.Domain
- Security: Uses domain's native authentication mechanism

**Administrative Operations:**
- Uses service account credentials via DirectoryEntry constructor
- Leverages PrincipalContext for high-level operations
- DirectoryEntry for low-level LDAP operations

### Tests

**Test Location:** `D:\Just For Fun\ZidUtilities\Tester\Program.cs`

**Coverage:** The Tester project references ActiveDirectory data but does NOT contain dedicated ActiveDirectory functionality tests.

**Found References:**
- Line 81-89: References "ActiveDirectoryData.xml" file for data comparison testing
- The test is actually comparing AD data (from SQL stored proc) with ADP (payroll) data
- No direct testing of AdManager class methods

**Conclusion:** ⚠️ No dedicated unit tests or integration tests exist for the ActiveDirectory project.

### Key Strengths

1. Comprehensive AD operation coverage
2. Fluent API for filter building
3. Type-safe attribute handling with conversion
4. Support for batch operations
5. Manager hierarchy calculation
6. Photo management capabilities
7. Well-documented code with XML comments
8. NuGet package ready with extensive README
9. Multiple operation overloads for flexibility

### Known Issues and Limitations

**Bugs:**
1. ⚠️ **DisableAccount Bug:** Line 545 sets `Enabled = true` instead of `false`
2. ⚠️ **Hardcoded Domain in AdManagerPath:** "DC=faradayfuture,DC=com"
3. ⚠️ **Hardcoded Domain in CopyAdUser:** Line 931 hardcodes "faradayfuture.com"
4. ⚠️ **Hardcoded Path in AddPictureByUserName:** Lines 713-714 hardcode domain and OU

**Limitations:**
1. No Delete Operations - Missing user/group deletion methods
2. No Tests - No unit or integration tests found
3. Limited Error Handling - Some methods catch exceptions but don't expose them to caller
4. No Async Support - All operations are synchronous
5. No Connection Pooling - No built-in connection pooling
6. No SSL/TLS Enforcement - Supports LDAP and LDAPS but doesn't force secure

**Security Considerations:**
- Credentials passed in plain text (not encrypted in memory)
- Exception messages may expose internal structure

### Documentation

- **CommonCode.DataAccess.ActiveDirectory\README.md** - Comprehensive usage guide (file exists)

### NuGet Package

- ZidUtilities.CommonCode.DataAccess.ActiveDirectory (1.0.0)

### Usage Scenarios

This library is well-suited for:
- Enterprise applications requiring Active Directory integration
- HR systems and employee directories
- Authentication services
- User provisioning workflows
- Group membership management
- Employee data retrieval with manager hierarchy
- User photo management

---

## 4. CommonCode.Files

### Overview
Robust file handling library for exporting and importing data to/from multiple formats (Excel, CSV, TXT, HTML) with extensive styling, validation, and error handling.

### Project Details
- **Location:** `D:\Just For Fun\ZidUtilities\CommonCode.Files\`
- **Assembly Name:** ZidUtilities.CommonCode.Files
- **Target Framework:** .NET Framework 4.8
- **Version:** 1.0.4
- **License:** MIT
- **Author:** Gonzalo Urbiola

### Dependencies
- **ClosedXML** (v0.87.1) - For XLSX export functionality
- **DocumentFormat.OpenXml** (v2.5) - Required by ClosedXML
- **ExcelDataReader** (v3.8.0) - For XLS/XLSX import functionality
- **ExcelDataReader.DataSet** (v3.8.0) - DataSet extensions
- **System.ValueTuple** (v4.5.0)
- **ZidUtilities.CommonCode** (v1.0.3) - Core utilities

### Key Namespaces

#### ZidUtilities.CommonCode.Files
Primary namespace containing all file handling functionality:
- DataExporter and DataImporter classes
- All enumerations
- Extension methods

### Core Classes

#### DataExporter.cs (2061 lines)
**Purpose:** Exports data from .NET data structures to multiple file formats with extensive styling and formatting.

**Supported Data Sources:**
- DataTable
- DataSet (multi-table → multi-sheet)
- List<T>
- Dictionary<K,V>
- ICollection<T>

**Export Targets:**
- XLSX (Excel 2007+)
- CSV (comma-separated values)
- TXT (tab-delimited, custom separator, or fixed-width)
- HTML (with JavaScript filtering)

**Key Export Methods:**
- `CreateXlsx(DataTable, string)` - Export to Excel file
- `CreateXlsx(DataSet, string)` - Export multiple sheets
- `CreateCsv(DataTable, string)` - Export to CSV
- `CreateTxt(DataTable, string, char)` - Export to TXT with custom separator
- `CreateHtml(DataTable, string)` - Export to HTML with filtering
- **Stream variants:** All methods have overloads for Stream output
- **Async variants:** Most methods have async versions with progress tracking

**Excel Styling Features:**
- **14 Professional Themes:** Default, Simple, Ocean, Forest, Sunset, Monochrome, Corporate, Mint, Lavender, Autumn, Steel, Cherry, Sky, Charcoal
- **ExcelCellStyle enum:** Good, Bad, Neutral, Calculation, Check, Alert, None
- **Alternate row styling** for better readability
- **Cell remarks/comments** via CellRemark class
- **Formula support** (cells starting with "=")
- **Document metadata** (Author, Company, Subject, Title)
- **Column management:** Ignore columns, auto-adjust widths
- **WidthAdjust strategies:** ByHeaders, ByFirst10Rows, ByFirst100Rows, ByAllRows, None

**HTML Export Features:**
- Complete HTML5 document generation
- Responsive CSS styling
- Client-side filtering with JavaScript
- Row count display (total and filtered)
- Mobile-responsive design
- Theme support matching Excel styles

**Progress Events:**
- `OnStartExportation` - Fired when export begins
- `OnProgress` - Periodic progress updates
- `OnCompletedExportation` - Fired when export completes

#### DataImporter.cs (965 lines)
**Purpose:** Imports data from various file formats into DataTable structures with automatic type conversion and validation.

**Supported Import Formats:**
- CSV (comma-separated values)
- TXT (tab-delimited, custom separator, or fixed-width)
- XLS (Excel 97-2003)
- XLSX (Excel 2007+)

**Key Import Methods:**
- `ImportFromCsv(string, DataStructure)` - Import from CSV with schema
- `ImportFromTxt(string, Delimiter, DataStructure)` - Import from TXT
- `ImportFromXlsx(string, DataStructure)` - Import from Excel
- `ImportFromXlsx(string, int, DataStructure)` - Import specific sheet
- **Async variants:** All methods have async versions with progress tracking

**Schema Management:**
- **DataStructure class:** Defines expected schema
- **Field class:** Represents a column with type and constraints
  - FieldType: Integer, FloatingPoint, Character, String, Date, Bit
  - Nullable support
  - Default values
  - Length constraints for strings
- **Automatic schema detection** from file headers
- **Manual schema definition** for validation and type enforcement

**Validation and Error Handling:**
- **ErorrInfo class:** Stores row-level error information
- Type conversion with validation
- Null value handling
- Default value assignment for non-nullable fields
- WasCleanExecution flag for validation
- Error collection with descriptions and locations
- Partial import support (continue on errors)

**Excel Import Features:**
- Sheet selection or automatic first sheet detection
- Header row detection
- Type inference from data
- Multi-sheet support (one sheet at a time)

**Progress Events:**
- `OnStartImportation` - Fired when import begins
- `OnProgress` - Updates every 1% for files >100 rows
- `OnCompletedImportation` - Fired when import completes

#### FilesExtensions.cs (272 lines)
**Purpose:** Extension methods for simplified file operations and specialized export scenarios.

**Key Extension Methods:**
- `SaveToXlsx(DataTable, string, bool)` - Simplified one-line Excel export
- `GenerateXlsxTrackFile(DataComparer, string, bool)` - Generates comparison tracking files

**Data Comparison Export:**
- Integrates with DataComparer from CommonCode
- Color-coded difference highlighting:
  - Red cells for differences
  - Cell comments showing both values (old vs new)
  - "BadMatchColumns" summary column
  - "Comments" column with match status
- Filter support for showing only changes
- Automatic styling based on data types

### File Operation Capabilities

#### Excel Operations (XLSX/XLS)
**Export:**
- Multi-sheet workbooks
- 14 predefined professional themes
- Custom styling with alternate row colors
- Header formatting
- Cell-level formatting and comments
- Formula support
- Metadata
- Column width auto-adjustment
- Freeze panes and auto-filtering

**Import:**
- Read XLS (Excel 97-2003) and XLSX (Excel 2007+) files
- Sheet selection
- Header detection
- Automatic type inference
- Schema enforcement

#### CSV Operations
**Export:**
- Proper escaping and quoting of special characters
- Header row support
- UTF-8 encoding

**Import:**
- Proper handling of quoted fields containing commas
- Quote removal from values
- Header detection
- Type conversion

#### Text File Operations (TXT)
**Export:**
- Tab-delimited format
- Custom separator characters
- Fixed-width columns with padding
- Header support

**Import:**
- Tab-delimited parsing
- Custom delimiter support
- Fixed-width column parsing with field lengths
- Filler character handling

#### HTML Operations
**Export:**
- Complete HTML5 documents
- Responsive design with CSS
- Filterable tables with JavaScript
- Client-side search functionality
- 14 theme options

### Excel Themes

The library includes 14 professional Excel themes with predefined color schemes:

1. **Default** - Classic blue and white
2. **Simple** - Minimalist gray
3. **Ocean** - Blue aquatic tones
4. **Forest** - Green nature tones
5. **Sunset** - Orange and warm tones
6. **Monochrome** - Black and white
7. **Corporate** - Professional gray-blue
8. **Mint** - Fresh green tones
9. **Lavender** - Purple pastel tones
10. **Autumn** - Brown and orange
11. **Steel** - Industrial gray
12. **Cherry** - Red and pink
13. **Sky** - Light blue
14. **Charcoal** - Dark gray

Each theme defines colors for:
- Header background and text
- Odd/even row backgrounds
- Good/Bad/Neutral cell styles
- Calculation, Check, Alert styles

### Tests

#### Tester Project (Console Application)

**DataExporterImporterTests.cs** (1005 lines) - 33 comprehensive test methods:

**DataExporter Tests (15 tests):**
- Constructor default values
- Export DataTable/DataSet to XLSX
- Export without headers
- Export with styles
- Export with ignored columns
- Export to TXT (tab-delimited, custom separator)
- Export to CSV
- Export to Stream (XLSX, CSV)
- Export List<T>, Dictionary, ICollection<T>
- Export empty DataTable
- Set Excel metadata

**DataImporter Tests (9 tests):**
- Constructor default values
- Import from CSV, TXT, XLSX
- Import with predefined DataStructure
- Import without headers
- Error handling for missing files
- Import with multiple data types

**Round-Trip Tests (5 tests):**
- XLSX export then import
- CSV export then import
- TXT export then import
- Data type preservation
- Complex DataSet handling

**Edge Case Tests (3 tests):**
- Special characters in data
- Null values handling
- Large dataset (1000 rows)

**ExcelStylesDemo.cs** (140 lines):
- Creates sample employee data
- Generates Excel files for all 14 themes
- Demonstrates proper usage of each style
- Includes metadata settings
- Shows theme descriptions

### Key Patterns

1. **Factory Pattern:** Multiple CreateXXX methods for different export formats
2. **Strategy Pattern:** WidthAdjust and ExcelStyle enumerations for runtime strategy selection
3. **Template Method:** Async/Sync methods follow same workflow
4. **Builder Pattern:** DataStructure and Field classes for schema construction
5. **Extension Methods:** FilesExtensions provides fluent API additions
6. **Event-Driven:** Progress reporting through events

### Key Strengths

1. Comprehensive format support (Excel, CSV, TXT, HTML)
2. 14 professional Excel themes with consistent styling
3. Advanced error handling with row-level error tracking
4. Schema validation and type conversion
5. Async operations with progress tracking
6. Stream-based operations for memory efficiency
7. Data comparison export with visual highlighting
8. Thoroughly tested (33 tests)
9. Well documented (817-line README)
10. NuGet package ready (v1.0.4)

### Use Cases

This library is ideal for:
- Report generation and distribution
- Data exchange between systems
- Web dashboards with filterable HTML exports
- Audit trails with comparison tracking
- Batch data processing
- ETL operations
- Data migration projects

### Documentation

- **CommonCode.Files\README.md** (817 lines): Comprehensive usage guide with examples for all features

### NuGet Package

- ZidUtilities.CommonCode.Files (1.0.4)

---

## 5. CommonCode.ICSharpTextEditor

### Overview
Feature-rich text editor wrapper around ICSharpCode.TextEditor for Windows Forms applications, providing 85+ syntax highlighting languages, code folding, bracket matching, integrated toolbar, and extensive customization.

### Project Details
- **Location:** `D:\Just For Fun\ZidUtilities\CommonCode.ICSharpTextEditor\`
- **Assembly Name:** ZidUtilities.CommonCode.ICSharpTextEditor
- **Target Framework:** .NET Framework 4.8
- **Version:** 1.0.8
- **Primary Dependency:** ICSharpCode.TextEditor (v3.2.1.6466)

### Dependencies
- **ICSharpCode.TextEditor** (v3.2.1.6466) - Core text editor library
- ClosedXML (v0.87.1)
- DocumentFormat.OpenXml (v2.5)
- ExcelDataReader (v3.8.0)
- ExcelDataReader.DataSet (v3.8.0)
- System.ValueTuple (v4.5.0)
- **ZidUtilities.CommonCode** (v1.0.3)
- **ZidUtilities.CommonCode.Files** (v1.0.4)
- System.Windows.Forms, System.Drawing, WindowsBase

### Key Namespaces

#### ZidUtilities.CommonCode.ICSharpTextEditor (Root)
Main classes: ExtendedEditor, SyntaxHighlighting, extensions, supporting classes.

#### ZidUtilities.CommonCode.ICSharpTextEditor.BracketMatching
Bracket/parenthesis matching strategies for different languages.

#### ZidUtilities.CommonCode.ICSharpTextEditor.FoldingStrategies
Code folding implementations for various language constructs.

#### ZidUtilities.CommonCode.ICSharpTextEditor.HelperForms
Supporting forms and helper classes (search, text ranges, highlighting).

### Core Classes

#### ExtendedEditor.cs (Primary User Control)
**Purpose:** Main user-facing control wrapping ICSharpCode.TextEditor with extended functionality.

**Key Features:**
- **Integrated Customizable Toolbar:** 12 toolbar buttons (Run, Stop, Kill, Comment, Uncomment, Search, Toggle Bookmark, Previous/Next Bookmark, Clear Bookmarks, Save, Load)
- **Syntax Highlighting:** 11 quick-access languages + 85+ total via SyntaxProvider
- **Code Folding:** Automatic folding based on language (regions, comments, blocks)
- **Bracket Matching:** Language-aware bracket/parenthesis matching
- **Keyboard Shortcuts:** Toolbar shortcuts and implicit shortcuts (case conversion, silent search, outlining)
- **Search & Replace:** Integrated search with forward/backward navigation
- **Bookmarks:** Line bookmarks with navigation
- **File Operations:** Load/save with syntax-aware file filters

**Key Properties:**
- `Editor` - Direct access to underlying TextEditorControl
- `EditorText` - Shortcut to text content
- `ShowToolbar` - Toggle toolbar visibility
- `TrackToolbarShortcuts` - Enable/disable shortcut processing
- `IsReadOnly` - Set read-only mode
- `Syntax` - Set syntax highlighting mode (SyntaxHighlighting enum)

**Custom Events:**
- `OnRun` - Triggered when Run button clicked (F5)
- `OnStop` - Triggered when Stop button clicked (Shift+F5)
- `OnKill` - Triggered when Kill button clicked (Ctrl+F5)
- `WhenKeyPress` - Triggered on key press after shortcut processing

**Toolbar Customization:**
- Designer-time customization via PropertyGrid
- Icon, tooltip, visibility, enabled state per button
- Multi-key shortcuts (e.g., Ctrl+K, C for comment)
- Two customizable text boxes for status/input

#### ICSharpTextEditorExtensions.cs
**Purpose:** Extension methods for simplified operations on TextEditorControl.

**Key Methods:**
- `CurrentLineNumber()` - Gets 0-based line number at caret
- `CurrentOffset()` - Gets document offset at caret
- `GetLineText(int lineNumber)` - Retrieves line text
- `InsertString(string, int position, bool refresh)` - Inserts text
- `MarkLine(int line, Color, TextMarkerType)` - Highlights entire line
- `SelectLine(int lineNumber)` - Selects and scrolls to line
- `SetMarker(int offset, int length, Color, TextMarkerType)` - Adds text marker
- `ScrollToLine(int lineNumber, bool center)` - Scrolls to line with optional centering
- `TryGetCurrentWord(out int offset, out int length)` - Identifies word at caret

#### SyntaxHighlighting.cs
**Purpose:** Manages syntax highlighting definitions and provides access to embedded syntax files.

**SyntaxHighlighting Enum (11 quick-access languages):**
- None, CSharp, XML, TransactSQL, MySql, JavaScript, HTML, CSS, VBNET, Json, CPlusPlus, Java

**SyntaxLanguage Enum (85+ total languages):**
- Includes: ActionScript, Ada, ANTLR, Assembly, AutoHotkey, Batch, Boo, C, C++, Ceylon, ChucK, Clojure, Cocoa, CoffeeScript, Cool, CSharp, CSS, D, Dart, Delphi, Eiffel, Elixir, Erlang, F#, Falcon, Fantom, Fortran95, Go, Groovy, Haskell, Haxe, HTML, Java, JavaScript, JSON, Julia, Kotlin, Lisp, Lua, MySQL, OCaml, Pascal, PHP, PowerShell, Prolog, Python, R, Rust, Scala, Scheme, Solidity, Swift, T-SQL, TCL, TypeScript, Vala, VBNET, VBScript, Verilog, VHDL, XML, and many more...

**SyntaxProvider Class:**
- `GetSyntaxFile(SyntaxLanguage)` - Returns XML syntax definition content
- **Note:** Most definitions from https://github.com/xv/ICSharpCode.TextEditor-Lexers; T-SQL is custom-created

#### InlineSyntaxProvider.cs
**Purpose:** Implements ISyntaxModeFileProvider to dynamically load syntax definitions from embedded resources.

**Key Functionality:**
- Loads syntax definitions from embedded strings
- Provides XmlTextReader for syntax mode files
- Registers syntax modes with HighlightingManager
- Enables runtime syntax switching

### Folding Strategies

#### GenericFoldingStrategy.cs
**Purpose:** Generic code folding strategy configurable for any language with start/end tokens.

**Key Features:**
- Configurable start/end fold tokens (e.g., "/*" and "*/", "#region" and "#endregion")
- "Spam folding" for consecutive comment lines (e.g., "///")
- Scans document for matching token pairs
- Creates fold markers between matched pairs

**Example Usage:**
```csharp
GenericFoldingStrategy csharpFolding = new GenericFoldingStrategy();
csharpFolding.StartFolding.Add("/*");
csharpFolding.EndFolding.Add("*/");
csharpFolding.StartFolding.Add("#region");
csharpFolding.EndFolding.Add("#endregion");
csharpFolding.SpamFolding.Add("///");
```

#### SqlFoldingStrategy.cs
**Purpose:** SQL-specific folding strategy using tokenization.

**Key Features:**
- Folds BEGIN...END blocks
- Folds block comments (`/* ... */`)
- Folds custom regions marked with `--fold...--/fold`
- Uses tokenization for accurate detection
- Ignores keywords in comments/strings

### Bracket Matching

#### GenericBracketMatcher.cs
**Purpose:** Provides bracket matching for generic languages using tokenization.

**Key Features:**
- Uses GenericLanguage tokenizer
- Ignores brackets in comments and strings
- Supports forward and backward searching
- Counts nested brackets to find correct match

#### SqlBracketMatcher.cs
**Purpose:** SQL-specific bracket matcher using SQL tokenization.

**Key Features:**
- Similar to GenericBracketMatcher but uses SQL tokens
- Ignores brackets in comments and strings
- Handles SQL-specific syntax correctly

### Helper Forms and Classes

#### SearchAndReplace.cs
**Purpose:** Dialog form for searching and replacing text.

**Key Features:**
- Search with match case and whole word options
- Replace single or replace all
- Scan region support for selected text
- Integrated with ExtendedEditor

#### ToolbarOption.cs
**Purpose:** Represents a customizable toolbar button.

**Key Properties:**
- `Name`, `Tooltip`, `Icon` - Display properties
- `Visible`, `Enabled` - State properties
- `ShortCut`, `ThenShortCut` - Keyboard shortcuts (supports two-key combinations)
- PropertyGrid integration with custom TypeConverter
- Designer support for Visual Studio

#### ImplicitShortcut.cs
**Purpose:** Represents shortcuts without visible buttons.

**Predefined Shortcuts:**
- ToUpperCase (Ctrl+Shift+U)
- ToLowerCase (Ctrl+Shift+L)
- SilentSearch (Ctrl+F3) - Search for word at cursor
- SearchForward (F3)
- SearchBackward (Shift+F3)
- ExpandOutlining (Ctrl+O, Ctrl+E)
- CollapseOutlining (Ctrl+O, Ctrl+C)
- ToggleOutlining (Ctrl+O, Ctrl+T)

#### ToolbarTextBox.cs
**Purpose:** Represents customizable text boxes in the toolbar for auxiliary input/display.

### Text Editing Capabilities

**Core Editing:**
- Insert, delete, replace text at any position
- Single/multiple selections, rectangular selection
- Full undo/redo stack
- Cut/copy/paste operations
- Line operations (get/set line text, select lines, mark lines)
- Word operations (identify word at cursor, convert case)
- Search & replace (forward/backward, regex support)
- Bookmarks (toggle, navigate, clear all)

**Advanced Features:**
- Text markers (highlight text ranges with colors)
- Precise scrolling control (scroll to line, center view)
- Selection manipulation (by line, offset, or text range)
- Document metrics (line count, text length, position)
- Read-only mode toggle

### Syntax Highlighting

**Supported Languages:** 85+ programming languages with XSHD (XML Syntax Highlighting Definition) files.

**Highlighting Components:**
- Keywords (language-specific reserved words)
- Comments (single-line and block)
- Strings (literals with escape sequences)
- Numbers (integer and floating-point)
- Operators (language-specific)
- Types (built-in and user-defined)
- Functions/methods
- Attributes/annotations

**Dynamic Loading:**
- Syntax definitions loaded from embedded resources at runtime
- InlineSyntaxProvider enables on-the-fly syntax switching
- Default color schemes defined in XSHD files

### Code Folding Support

**Fold Types by Language:**

**C# / Java / C++ / JavaScript:**
- Block comments (`/* ... */`)
- Regions (`#region ... #endregion` for C#)
- Consecutive documentation comments (`///`)

**VB.NET:**
- Regions (`#Region ... #End Region`)

**SQL (T-SQL / MySQL):**
- BEGIN...END blocks
- Block comments (`/* ... */`)
- Custom fold markers (`--fold ... --/fold`)

**HTML:**
- HTML comments (`<!-- ... -->`)
- Script, style, body, head tags

**XML:**
- Block elements (auto-detected)

**JSON:**
- Object blocks (`{ ... }`), array blocks (`[ ... ]`)

**CSS:**
- Block comments (`/* ... */`)

**Folding Controls:**
- Collapse all (Ctrl+O, Ctrl+C)
- Expand all (Ctrl+O, Ctrl+E)
- Toggle all (Ctrl+O, Ctrl+T)
- Individual fold markers clickable
- Automatic folding refresh on document changes (1-second delay)

### Tests

#### TesterWin Tests

**Form1.cs** - Primary test form:
1. Load SQL Test - T-SQL syntax highlighting with large stored procedure
2. Load C# Test - C# syntax highlighting
3. Load XML Test - XML syntax highlighting
4. Load HTML Test - HTML syntax highlighting
5. Load JavaScript Test - JavaScript syntax highlighting
6. Load CSS Test - CSS syntax highlighting
7. Load JSON Test - JSON syntax highlighting
8. Remove Highlighting Test - Switch to no syntax highlighting
9. Run Button Event Test - OnRun event handler
10. Key Press Event Test - WhenKeyPress event handler

**Form3.cs** - Diff testing:
1. SQL Diff Test - Side-by-side SQL comparison with syntax highlighting
2. C# Diff Test - Side-by-side C# comparison
3. JavaScript Diff Test - Side-by-side JS comparison

**Testing Coverage:**
- All major syntax highlighting modes
- File loading and saving
- Toolbar events (Run, Stop, Kill)
- Keyboard shortcuts
- Syntax switching
- Large file handling
- Multiple language support
- Diff/comparison scenarios

### Custom Features & Extensions

**Beyond Standard ICSharpCode.TextEditor:**

1. **Integrated Toolbar** - Customizable buttons, multi-key shortcuts, designer-time customization
2. **Implicit Shortcuts** - Case conversion, silent search, quick outlining
3. **Helper Text Boxes** - Two customizable text boxes in toolbar
4. **Enhanced Search** - Dialog integration, highlight all matches, search region support
5. **Syntax Switching** - Runtime language changes with automatic folding/bracket updates
6. **Extension Methods** - Simplified common operations
7. **Custom SQL Support** - Custom T-SQL syntax highlighting, SQL-aware tokenization
8. **Event System** - OnRun/OnStop/OnKill, WhenKeyPress, OptionChanged events
9. **Designer Integration** - ToolboxBitmap, custom TypeConverters, property categories
10. **Automatic Folding Refresh** - Timer-based refresh after edits (1-second delay)

### Key Strengths

1. 85+ programming languages with syntax highlighting
2. Fully customizable toolbar with designer support
3. Advanced code folding for multiple languages
4. Bracket matching with language awareness
5. Integrated search and replace
6. Extension methods for common operations
7. Event-driven architecture
8. Custom SQL enhancements
9. Production-ready (v1.0.8, NuGet packaged)
10. Thoroughly tested in TesterWin project

### Use Cases

This component is ideal for:
- Code editors and IDEs
- SQL query tools
- Script editors
- Configuration file editors
- Log viewers with syntax highlighting
- Diff/comparison tools
- Development tools requiring code display/editing

### Documentation

- Embedded XML documentation comments
- 85+ XSHD syntax definition files
- Test forms demonstrating usage

### NuGet Package

- ZidUtilities.CommonCode.ICSharpTextEditor (1.0.8)

---

## 6. CommonCode.Win

### Overview
Windows Forms-specific extension library providing enhanced controls, grids, dialogs, CRUD UI generation, theme management, and diff/comparison tools for rich desktop applications.

### Project Details
- **Location:** `D:\Just For Fun\ZidUtilities\CommonCode.Win\`
- **Assembly Name:** ZidUtilities.CommonCode.Win
- **Target Framework:** .NET Framework 4.8
- **Output Type:** Library

### Dependencies
- **ClosedXML** (v0.87.1) - Excel file manipulation
- **DocumentFormat.OpenXml** (v2.5) - Office document handling
- **ExcelDataReader** (v3.8.0) - Reading Excel files
- **ICSharpCode.TextEditor** (v3.2.1.6466) - Text editing components
- **System.Windows.Forms** - WinForms controls
- **ZidUtilities.CommonCode** (via NuGet v1.0.3)
- **ZidUtilities.CommonCode.Files** (via NuGet v1.0.4)
- **ZidUtilities.CommonCode.ICSharpTextEditor** (via NuGet v1.0.8)

### Key Namespaces

#### ZidUtilities.CommonCode.Win (Root)
Core WinForms utilities: ZidThemes, ThemeManager, Resources.

#### ZidUtilities.CommonCode.Win.Controls
Custom WinForms controls: AnimatedWaitTextBox, DoubleBufferedPanel, ToastForm, TokenSelect, VerticalProgressBar.

#### ZidUtilities.CommonCode.Win.Controls.AddressBar
Explorer-style breadcrumb navigation components.

#### ZidUtilities.CommonCode.Win.Controls.Diff
Visual diff/comparison controls for side-by-side text comparison.

#### ZidUtilities.CommonCode.Win.Controls.Grid
Enhanced grid control with filtering, theming, and plugin system.

#### ZidUtilities.CommonCode.Win.Controls.Grid.GridFilter
Grid filtering subsystem with multiple filter types.

#### ZidUtilities.CommonCode.Win.Forms
Common dialog forms for user input and selection.

#### ZidUtilities.CommonCode.Win.CRUD
CRUD UI generation from database schema.

### Core Components

#### A. Grid Control System (ZidGrid)

**ZidGrid.cs** - Central enhanced DataGridView wrapper

**Key Features:**
- Built-in filtering UI with column-specific filters
- Theme support (34 predefined themes via ZidThemes enum)
- Plugin architecture (IZidGridPlugin)
- Context menu system (ZidGridMenuItem)
- Column visibility, freezing, auto-resize
- Binary data safe rendering
- Row count tracking
- Export functionality (via plugins)

**Grid Filtering System:**

**DataGridFilterExtender** - Component that adds filtering to any DataGridView

**GridFiltersControl** - Visual filter UI panel

**Filter Types:**
- `TextGridFilter` - Text-based filtering
- `NumericGridFilter` - Number range filtering
- `DateGridFilter` - Date range filtering
- `BoolGridFilter` - Boolean filtering
- `EnumerationGridFilter` - Dropdown selection
- `DistinctValuesGridFilter` - Column value-based filter
- `NullGridFilter`, `EmptyGridFilter` - Special filters

**Filter Factories:**
Plugin system for custom filter types via IGridFilterFactory.

**Grid Plugins:**
- `ColumnVisibilityPlugin` - Show/hide columns
- `CopySpecialPlugin` - Advanced copy operations
- `DataExportPlugin` - Export to Excel/CSV
- `FreezeColumnsPlugin` - Freeze columns
- `QuickFilterPlugin` - Quick text search

**Grid Theming:**
- `GridThemes` - Pre-defined color schemes
- `GridThemeHelper` - Apply themes to grids

#### B. Theme Management

**ZidThemes.cs** - Unified theme enum (34 themes)

**Dialog Themes:**
- Default, Information, Success, Warning, Error, Professional

**Color Themes:**
- CodeProject, BlackAndWhite, Blue, Violet, Greenish, DarkMode

**Extended Themes:**
- Ocean, Sunset, Forest, Rose, Slate, Teal, Amber, Crimson, Indigo, Emerald, Lavender, Bronze, Navy, Mint, Coral, Steel, Gold, Plum, Aqua

**ThemeManager.cs** - Component to apply themes to forms/controls

**Features:**
- Automatic theme application to all controls on a form
- Cascading theme to child forms
- Theme persistence
- Custom color scheme support

#### C. CRUD and UI Generation

**UIGenerator.cs** - Automatically generate Insert/Update/Delete forms from table schema

**Key Features:**
- Load schema from: SQL Server, DataTable, or Dictionary
- Field configuration: aliases, masks, passwords, exclusions, foreign keys
- Custom validation and formatting per field
- Layout modes: Auto, Grid, Custom HTML
- Theme integration
- Password confirmation
- Form width customization

**Supporting Classes:**
- `FieldMetadata` - Schema and behavior for each field
- `MaskInfo` - Field masking/unmasking
- `ForeignKeyInfo` - Lookup values for dropdowns
- `FormatConfig` - Field formatting rules
- `FieldFormat enum` - Date, Currency, Email, Phone, etc.

**Usage Pattern:**
```csharp
var generator = new UIGenerator();
generator.LoadSchemaFromDatabase(tableName, connectionString);
generator.SetAlias("employee_id", "Employee ID");
generator.SetRequired("first_name");
generator.SetMask("ssn", "###-##-####");
generator.SetForeignKey("department_id", departmentList);
var dialog = generator.GenerateInsertDialog(theme);
```

#### D. Common Dialogs and Forms

**TextInputDialog** - Prompt user for text input
- Validation support
- Format enforcement
- Multi-line support
- Themed appearance

**SingleSelectionDialog** - Select one item from a list
- Search functionality
- Custom display formatting
- Return selected value

**MultiSelectionDialog** - Select multiple items
- Checkboxes for selection
- Select all/none buttons
- Return selected collection

**ComplexObjectSelectionDialog** - Select from complex objects
- Display member and value member support
- Custom object formatting
- Search and filter

**SqlConnectForm** - SQL Server connection builder
- Server browsing
- Database dropdown
- Authentication options (Windows/SQL)
- Test connection functionality

**MessageBoxDialog** - Themed message box replacement
- Custom themes matching ZidThemes
- Icon support
- Button customization
- Centered on parent

**ProcessingDialog** - Long-running operation progress
- Progress bar
- Status message updates
- Cancel support
- Estimated time remaining

**DialogStyle enum** - Matches ZidThemes for consistent dialog appearance

#### E. Diff/Comparison Controls

**SideToSideTextComparer** - Visual text comparison (like Beyond Compare)

**Key Features:**
- Side-by-side diff view
- Line-by-line comparison
- Color-coded differences (additions, deletions, changes)
- Synchronized scrolling
- Syntax highlighting integration
- Navigation between differences

**SideToSideLineComparer** - Line-by-line comparison

**DiffInspector** - Interactive diff viewer
- Expandable/collapsible diff sections
- Context lines display
- Change statistics
- Search within diffs

**DiffHighlight** - Syntax highlighting for diffs
- Integration with ICSharpCode.TextEditor
- Language-aware diff highlighting

#### F. Custom Controls

**Visual Controls:**

**AnimatedWaitTextBox** - Text box with animated waiting indicator
- Spinner animation during loading
- Customizable animation speed
- Theme-aware colors

**VerticalProgressBar** - Vertical orientation progress bar
- Bottom-to-top or top-to-bottom
- Custom colors
- Percentage display

**DoubleBufferedPanel** - Flicker-free panel
- Eliminates flicker during redraws
- Ideal for custom painting
- Performance optimized

**ToastForm** - Non-intrusive notification popup
- Auto-hide after timeout
- Slide-in animation
- Themed appearance
- Position options (top-right, bottom-right, etc.)

**TokenSelect** - Multi-token selection control (like tag input)
- Add/remove tokens
- Custom token display
- Validation support
- Auto-complete integration

**Navigation Controls:**

**AddressBar** - Explorer-style breadcrumb navigation

**Key Features:**
- Hierarchical navigation
- Click to navigate to parent levels
- Custom node support via IAddressNode
- Built-in implementations: GenericNode, FileSystemNode
- GenericNodeCollection for hierarchical data

**Supporting Interfaces/Classes:**
- `IAddressNode` - Interface for custom nodes
- `GenericNode` - Generic hierarchical node
- `FileSystemNode` - File system path navigation
- `GenericNodeCollection` - Collection with parent/child relationships

### Tests

#### TesterWin Tests

**FormZidGridMenuTest** - Tests ZidGrid context menus and plugins
- Column visibility plugin
- Export plugin (Excel/CSV)
- Freeze columns plugin
- Quick filter plugin
- Custom menu items

**FormThemeManagerTest** - Tests theme application
- Apply themes to forms
- Theme switching
- Control color updates
- Child form theming

**FormUIGeneratorTest** - Tests UIGenerator CRUD form generation
- Schema loading from SQL Server
- Field configuration (aliases, masks, required)
- Foreign key dropdowns
- Insert dialog generation
- Update dialog generation
- Delete confirmation dialogs
- Theme application

**FormTokenSelectTest** - Tests TokenSelect control
- Add tokens
- Remove tokens
- Validation
- Custom display

**FormVeinsTest** - Comprehensive integration test
- SqliteConnector integration
- UIGenerator for CRUD operations
- ZidGrid for data display
- Theme application

### Design Patterns

1. **Plugin Architecture** - IZidGridPlugin for extensible grid functionality
2. **Factory Pattern** - IGridFilterFactory for custom filter types
3. **Builder Pattern** - UIGenerator with fluent configuration API
4. **Template Method** - Common dialog base classes
5. **Strategy Pattern** - Multiple themes, filter types
6. **Observer Pattern** - Event-driven grid updates
7. **Facade Pattern** - ZidGrid wraps DataGridView complexity

### Key Strengths

1. **Comprehensive Grid System** - Advanced filtering, theming, plugins
2. **34 Professional Themes** - Consistent UI appearance
3. **CRUD UI Generation** - Automatic form generation from schema
4. **Rich Dialog Library** - Pre-built dialogs for common scenarios
5. **Diff/Comparison Tools** - Side-by-side text comparison with syntax highlighting
6. **Custom Controls** - Specialized controls for specific use cases
7. **Theme Management** - Centralized theming across entire application
8. **Plugin Extensibility** - Easy to extend grid functionality
9. **Well Tested** - Multiple test forms in TesterWin
10. **Designer Support** - Visual Studio designer integration

### Use Cases

This library is ideal for:
- Data-intensive Windows Forms applications
- Database management tools
- Report viewers and editors
- Admin panels and dashboards
- Configuration editors
- Data comparison and auditing tools
- Forms-over-data applications
- Legacy system modernization

### Documentation

- Embedded XML documentation comments
- Test forms demonstrating all features
- README files (likely present based on other projects)

### NuGet Package

- ZidUtilities.CommonCode.Win (version referenced in other projects)

### Recent Additions

**UIGenerator class** - Recently added for automatic CRUD form generation
**TokenSelect control** - Recently added for multi-token input
**Theme system enhancements** - Extended from original themes

### Integration with Other Projects

CommonCode.Win extensively integrates with:
- **CommonCode** - For core utilities and extensions
- **CommonCode.Files** - For data export functionality in grids
- **CommonCode.ICSharpTextEditor** - For diff viewers with syntax highlighting
- **CommonCode.DataAccess** - For UIGenerator schema loading

---

*End of Documentation*
