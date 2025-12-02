# CommonCode.DataAccess.ActiveDirectory

Active Directory management and query utilities for enterprise directory services.

## Features

### Core Components
- **AdManager**: Main Active Directory manager
  - User and group management
  - Organizational unit operations
  - Directory searching and querying
  - Authentication and authorization

- **AdManagerFilter**: Advanced filtering for directory searches
  - LDAP filter construction
  - Complex query building
  - Attribute-based filtering

- **AdManagerPath**: Active Directory path management
  - LDAP path construction
  - Distinguished name handling
  - Domain and OU navigation

### Data Models
- **BasicEmployee**: Employee data model
  - Common employee attributes
  - Contact information
  - Organizational data

- **AdAttribute**: Active Directory attribute wrapper
  - Attribute name and value management
  - Type handling
  - Multi-value support

- **AdKeyAttributes**: Key attribute definitions
  - Predefined common attributes
  - Attribute name constants

### Enumerations
- **Enums**: Active Directory enumerations
  - Account types
  - Search scopes
  - Object classes
  - Status flags

## Features
- User account management (create, update, delete)
- Group membership operations
- Directory search and filtering
- Organizational unit management
- User authentication
- Attribute querying and modification
- Bulk operations support

## Dependencies

- System.DirectoryServices
- System.DirectoryServices.AccountManagement

## Target Framework

.NET Framework 4.8

## Installation

Add a reference to `CommonCode.DataAccess.ActiveDirectory.dll` in your project and ensure you have the required System.DirectoryServices references.

## Usage Examples

### Basic Active Directory Connection

```csharp
using ZidUtilities.CommonCode.DataAccess.ActiveDirectory;

// Initialize AD Manager with domain credentials
var adManager = new AdManager(
    domain: "CONTOSO",
    username: "administrator",
    password: "P@ssw0rd"
);

// Or connect to specific domain controller
var adManager = new AdManager(
    ldapPath: "LDAP://DC01.contoso.com",
    username: "administrator",
    password: "P@ssw0rd"
);
```

### Searching for Users

```csharp
// Search for a user by username
AdManager activeDirectoryManager = new AdManager("contoso.com");
activeDirectoryManager.Paths.Add(new AdManagerPath("Worker", "Users"));
BasicEmployee user = activeDirectoryManager.GetBasicEmployee("jdoe");

if (user != null)
{
    Console.WriteLine($"Found user: {user.DisplayName}");
    Console.WriteLine($"Email: {user.Email}");
    Console.WriteLine($"Department: {user.Department}");
}

// Search for users by filter
activeDirectoryManager.Filter = AdManagerFilter.CreateFilter().FilterBy(FilterableAttribute.UserName, FilterComparer.Equals, "jane.doe");
activeDirectoryManager.Attributes.Add(new AdAttribute("sAMAccountName", "UserName", AdType.String));
activeDirectoryManager.Attributes.Add(new AdAttribute("employeeNumber", "EmployeeId", AdType.String));
activeDirectoryManager.Attributes.Add(new AdAttribute("distinguishedName", "DistinguishedName", AdType.String));

DataTable results = activeDirectoryManager.QueryActiveDirectory();

if (results != null && results.Rows.Count > 0)
{
    string employeeId = results.Rows[0]["EmployeeId"].ToString();
    string userName = results.Rows[0]["UserName"].ToString();
    string distinguishedName = results.Rows[0]["DistinguishedName"].ToString();

    Console.WriteLine($"Employee ID: {employeeId}, User Name: {userName}, DN: {distinguishedName}");
}

```

### Updating User Attributes

```csharp
// Update user properties
AdManager activeDirectoryManager = new AdManager("contoso.com");
activeDirectoryManager.Paths.Add(new AdManagerPath("Worker", "Users"));

// Update single attributes
activeDirectoryManager.UpdateUserProperty("telephoneNumber", "555-1234", "CN=Jane Doe,OU=IT,OU=Users,DC=faradayfuture,DC=com");

// Another way to update single attribute
string username = "jdoe";
DirectoryEntry searchRoot = new DirectoryEntry("LDAP://DC=contoso,DC=com"); 
DirectoryEntry user;
using (DirectorySearcher searcher = new DirectorySearcher(searchRoot))
{
    searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
    SearchResult result = searcher.FindOne();

    if (result != null)
    {
        user = result.GetDirectoryEntry();
        activeDirectoryManager.UpdateUserProperty("title", "Sr. Application Developer", user);
    }
    else
    {
        Console.WriteLine("User not found.");
    }
}

// Update multiple attributes at once
List<AdAttribute> attributesToUpdate = new List<AdAttribute>
{
    new AdAttribute("mobile", "555-9999", AdType.String),
    new AdAttribute("manager", "CN=Jane Smith,OU=Users,DC=contoso,DC=com", AdType.String),
    new AdAttribute("description", "Full stack developer", AdType.String)
};>
activeDirectoryManager.AttributeBatchUpdate("CN=Jane Doe,OU=IT,OU=Users,DC=faradayfuture,DC=com", attributesToupdate);
//OR
//activeDirectoryManager.AttributeBatchUpdate(<DirectoryEntry user>, attributesToupdate);
```

### User Authentication

```csharp
// Authenticate user credentials
AdManager adManager = new AdManager("contoso.com");
adManager.Paths.Add(new AdManagerPath("Worker", "Users"));

bool isValid = adManager.IsValidCredential("jdoe", "UserPassword123");

if (isValid)
{
    Console.WriteLine("Authentication successful!");
}
else
{
    Console.WriteLine("Authentication failed!");
}
```


### Using AdManagerPath for LDAP Paths

```csharp
// Build LDAP paths safely
var path = new AdManagerPath("DC=contoso,DC=com");

// Add organizational units
path.AddOU("IT");
path.AddOU("Users");

// Get full LDAP path
string ldapPath = path.ToString();
// Result: "LDAP://OU=Users,OU=IT,DC=contoso,DC=com"

// Parse existing path
var existingPath = AdManagerPath.Parse("LDAP://CN=John Doe,OU=Users,DC=contoso,DC=com");
string cn = existingPath.GetCommonName(); // "John Doe"
```

## Use Cases

- **Enterprise user management systems** - Centralized user provisioning and deprovisioning
- **Authentication services** - Validate credentials against Active Directory
- **HR system integration** - Sync employee data between HR systems and AD
- **Access control systems** - Manage group memberships for application access
- **Directory synchronization** - Keep AD in sync with other identity systems
- **Employee directory applications** - Build searchable employee directories
- **Self-service password reset** - Allow users to reset passwords
- **Compliance reporting** - Audit user accounts and group memberships
- **Automated onboarding/offboarding** - Streamline employee lifecycle management

## Error Handling

```csharp
using System;

try
{
    var adManager = new AdManager("CONTOSO", "admin", "password");
    var user = adManager.FindUser("jdoe");

    if (user == null)
    {
        Console.WriteLine("User not found");
    }
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine("Insufficient permissions: " + ex.Message);
}
catch (DirectoryServicesCOMException ex)
{
    Console.WriteLine("AD error: " + ex.Message);
}
catch (Exception ex)
{
    Console.WriteLine("Unexpected error: " + ex.Message);
}
```

## Security Considerations

1. **Use service accounts** with minimum required permissions
2. **Never hard-code credentials** - use secure configuration storage
3. **Use SSL/TLS** for LDAP connections (LDAPS://  on port 636)
4. **Implement proper exception handling** to avoid exposing sensitive information
5. **Log all administrative operations** for audit purposes
6. **Validate user input** before querying Active Directory
7. **Use parameterized queries** to prevent LDAP injection attacks

## Performance Tips

- Use filters to limit result sets
- Cache frequently accessed data
- Reuse AdManager instances when possible
- Use paging for large result sets
- Query only the attributes you need
