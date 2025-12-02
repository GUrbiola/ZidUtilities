# CommonCode.Files

File import and export utilities supporting multiple formats including Excel, CSV, TXT, and HTML.

## Features

### Data Export
- **DataExporter**: Export data to multiple formats
  - **XLSX**: Excel files using ClosedXML with theming support
  - **CSV**: Comma-separated values
  - **TXT**: Text files with custom separators or fixed widths
  - **HTML**: HTML tables with filterable columns

### Excel Styling
Predefined Excel workbook themes:
- **Default**: Navy blue headers with light blue alternating rows
- **Simple**: Minimal styling with gray borders
- **Ocean**: Deep blue ocean theme
- **Forest**: Forest green theme
- **Sunset**: Warm sunset orange theme
- **Monochrome**: Black and white minimalist
- **Corporate**: Blue-gray executive theme
- **Mint**: Fresh mint green
- **Lavender**: Elegant purple
- **Autumn**: Warm brown and orange
- **Steel**: Modern gray and silver
- **Cherry**: Bold red
- **Sky**: Light sky blue
- **Charcoal**: Strong dark theme

### Data Import
- **DataImporter**: Import data from various file formats
  - **TXT**: Text files with delimiters or fixed-width columns
  - **CSV**: Comma-separated values with proper quote handling
  - **XLS/XLSX**: Excel files with sheet selection support
  - Automatic data type conversion and validation
  - Progress tracking and error reporting
  - Synchronous and asynchronous import modes

### File Extensions
- **FilesExtensions**: Extension methods for file operations
- Simplified file handling utilities

## Dependencies

- ClosedXML.Excel
- ExcelDataReader
- System.Data

## Target Framework

.NET Framework 4.8

## Usage Examples

### Exporting DataTable to Excel with Styling

```csharp
using ZidUtilities.CommonCode.Files;
using System.Data;

// Create a DataTable with sample data
DataTable dt = new DataTable("Employees");
dt.Columns.Add("ID", typeof(int));
dt.Columns.Add("Name", typeof(string));
dt.Columns.Add("Department", typeof(string));
dt.Columns.Add("Salary", typeof(decimal));

dt.Rows.Add(1, "John Doe", "IT", 75000);
dt.Rows.Add(2, "Jane Smith", "HR", 65000);
dt.Rows.Add(3, "Bob Johnson", "Sales", 70000);

// Create DataSet for export
DataSet ds = new DataSet();
ds.Tables.Add(dt);

// Configure and export to Excel
DataExporter exporter = new DataExporter
{
    ExportType = ExportTo.XLSX,
    ExportExcelStyle = ExcelStyle.Ocean,
    WriteHeaders = true,
    ExportWithStyles = true,
    UseAlternateRowStyles = true,
    AutoCellAdjust = WidthAdjust.ByHeaders,
    Author = "Your Name",
    Company = "Your Company",
    Title = "Employee Report"
};

// Export to file
exporter.ExportDataToFile(ds, @"C:\temp\employees.xlsx");
```

### Exporting to CSV

```csharp
DataExporter csvExporter = new DataExporter
{
    ExportType = ExportTo.CSV,
    Separator = ",",
    WriteHeaders = true
};

csvExporter.ExportDataToFile(ds, @"C:\temp\employees.csv");
```

### Exporting to HTML with Filterable Table

```csharp
DataExporter htmlExporter = new DataExporter
{
    ExportType = ExportTo.HTML,
    ExportHtmlStyle = ExcelStyle.Corporate,
    WriteHeaders = true
};

htmlExporter.ExportDataToFile(ds, @"C:\temp\employees.html");
```

### Exporting to Fixed-Width Text File

```csharp
DataExporter txtExporter = new DataExporter
{
    ExportType = ExportTo.TXT,
    DelimitedByLenght = true,
    CharFiller = ' ',
    Widhts = new List<int> { 10, 30, 20, 15 }, // Column widths
    WriteHeaders = true
};

txtExporter.ExportDataToFile(ds, @"C:\temp\employees.txt");
```

### Exporting to Stream

```csharp
using (MemoryStream stream = new MemoryStream())
{
    DataExporter exporter = new DataExporter
    {
        ExportType = ExportTo.XLSX,
        ExportExcelStyle = ExcelStyle.Default,
        WriteHeaders = true,
        ExportWithStyles = true
    };

    exporter.ExportDataToStream(ds, stream);

    // Stream now contains the Excel file data
    byte[] fileBytes = stream.ToArray();
}
```

### Using Export Events for Progress Tracking

```csharp
DataExporter exporter = new DataExporter
{
    ExportType = ExportTo.XLSX,
    ExportExcelStyle = ExcelStyle.Forest
};

exporter.OnStartExportation += (firedAt, records, progress, exportType) =>
{
    Console.WriteLine($"Export started at {firedAt} with {records} records");
};

exporter.OnProgress += (firedAt, records, progress, exportType) =>
{
    Console.WriteLine($"Progress: {progress}%");
};

exporter.OnCompletedExportation += (firedAt, records, exportType, stream, path) =>
{
    Console.WriteLine($"Export completed at {firedAt}. Saved to {path}");
};

exporter.ExportDataToFile(ds, @"C:\temp\output.xlsx");
```

### Ignoring Specific Columns

```csharp
DataExporter exporter = new DataExporter
{
    ExportType = ExportTo.XLSX,
    IgnoredColumns = new List<string> { "Password", "SSN", "InternalID" },
    WriteHeaders = true
};

exporter.ExportDataToFile(ds, @"C:\temp\safe_export.xlsx");
```

### Adding Cell Remarks (Comments) to Excel

```csharp
DataExporter exporter = new DataExporter
{
    ExportType = ExportTo.XLSX,
    ExportWithStyles = true,
    Remarks = new List<CellRemark>
    {
        new CellRemark
        {
            Row = 2,
            Column = 4,
            Comment = "This salary requires manager approval",
            Style = ExcelCellStyle.Alert
        }
    }
};

exporter.ExportDataToFile(ds, @"C:\temp\employees_with_notes.xlsx");
```

## Common Use Cases

- **Report Generation**: Export database query results to Excel with professional styling
- **Data Exchange**: Export data to CSV for use in other applications
- **Web Reports**: Generate filterable HTML tables for web dashboards
- **Fixed-Width Files**: Generate mainframe-compatible text files
- **Audit Trails**: Export data with custom styling to highlight important information
- **Batch Processing**: Stream exports for large datasets without saving to disk

## Advanced Features

### Column Width Adjustment Strategies
- **ByHeaders**: Adjust width based on header text length
- **ByFirst10Rows**: Sample first 10 rows for optimal width
- **ByFirst100Rows**: Sample first 100 rows for better accuracy
- **ByAllRows**: Scan all rows (slower but most accurate)
- **None**: Use default widths

### Excel Cell Styles
Apply semantic styles to specific cells:
- **Good**: Green background for positive values
- **Bad**: Red background for negative/error values
- **Neutral**: Gray for neutral information
- **Calculation**: Blue for calculated fields
- **Check**: Yellow for items needing review
- **Alert**: Orange for warnings
- **None**: No special styling

## DataImporter Class - Detailed Guide

The **DataImporter** class provides robust data import capabilities from multiple file formats with automatic type conversion, error handling, and progress tracking.

### Core Concepts

#### ImportFrom Enumeration
Defines the source file format:
- **TXT**: Text files with custom delimiters or fixed-width columns
- **CSV**: Comma-separated values (handles quoted fields properly)
- **XLS**: Legacy Excel format (.xls files)
- **XLSX**: Modern Excel format (.xlsx files)

#### Delimiter Enumeration
Defines how fields are separated:
- **Tab**: Tab-delimited files (\t)
- **Separator**: Custom character delimiter (comma, pipe, semicolon, etc.)
- **Lenght**: Fixed-width columns (each field has a specific length)

#### FieldType Enumeration
Supported data types for automatic conversion:
- **Integer**: Whole numbers (int)
- **FloatingPoint**: Decimal numbers (float)
- **Character**: Single character (char)
- **String**: Text data (string)
- **Date**: Date and time values (DateTime)
- **Bit**: Boolean values (bool)

### DataStructure and Field Classes

The **DataStructure** class defines the schema of your imported data:
```csharp
public class DataStructure
{
    public string Name { get; set; }           // Table name
    public List<Field> Fields { get; set; }    // Column definitions
}

public class Field
{
    public string Name { get; set; }                      // Column name
    public bool Nullable { get; set; }                    // Allow null values
    public DataImporter.FieldType FieldType { get; set; } // Data type
    public int Length { get; set; }                       // For fixed-width or max length
}
```

### Key Properties

- **ImportType**: File format to import (TXT, CSV, XLS, XLSX)
- **FileName**: Full path to the source file
- **FileHeader**: Whether the file has a header row (default: true)
- **Separator**: Type of delimiter (Tab, Separator, Lenght)
- **SeparatorChar**: Character used as separator (default: ',')
- **SheetName**: Excel sheet name to import (defaults to first sheet)
- **FillerChar**: Character used to fill fixed-width fields (default: ' ')
- **DataStructure**: Schema definition for the data
- **ImportResultDataTable**: Output DataTable after successful import
- **Errors**: List of errors encountered during import
- **WasCleanExecution**: True if no errors occurred

### Events

Track import progress with these events:
- **OnStartImportation**: Fired when import begins
- **OnProgress**: Fired during import to report progress
- **OnCompletedImportation**: Fired when import finishes

## DataImporter Usage Examples

### Importing from CSV File

```csharp
using ZidUtilities.CommonCode.Files;
using System.Data;

// Simple CSV import with automatic schema detection
DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.CSV,
    FileName = @"C:\data\employees.csv",
    FileHeader = true,           // First row contains column names
    SeparatorChar = ','          // Comma separator
};

// Import the data
bool success = importer.ImportFromFile();

if (success && importer.WasCleanExecution)
{
    DataTable result = importer.ImportResultDataTable;
    Console.WriteLine($"Imported {result.Rows.Count} rows with {result.Columns.Count} columns");

    // Access the data
    foreach (DataRow row in result.Rows)
    {
        Console.WriteLine($"{row["Name"]} - {row["Email"]}");
    }
}
else
{
    // Handle errors
    foreach (var error in importer.Errors)
    {
        Console.WriteLine($"Error at row {error.Location}: {error.Description}");
    }
}
```

### Importing from Excel with Specific Schema

```csharp
// Define the data structure
DataStructure schema = new DataStructure
{
    Name = "Products",
    Fields = new List<Field>
    {
        new Field("ProductID", nullable: false, DataImporter.FieldType.Integer),
        new Field("ProductName", nullable: false, DataImporter.FieldType.String, length: 100),
        new Field("Price", nullable: false, DataImporter.FieldType.FloatingPoint),
        new Field("InStock", nullable: false, DataImporter.FieldType.Bit),
        new Field("LastUpdated", nullable: true, DataImporter.FieldType.Date)
    }
};

DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.XLSX,
    FileName = @"C:\data\products.xlsx",
    SheetName = "ProductData",    // Specific sheet name
    FileHeader = true,
    DataStructure = schema        // Use custom schema
};

bool success = importer.ImportFromFile();

if (success)
{
    DataTable products = importer.ImportResultDataTable;

    // Process the data
    foreach (DataRow row in products.Rows)
    {
        int id = Convert.ToInt32(row["ProductID"]);
        string name = row["ProductName"].ToString();
        float price = Convert.ToSingle(row["Price"]);
        bool inStock = Convert.ToBoolean(row["InStock"]);

        Console.WriteLine($"{id}: {name} - ${price} (In Stock: {inStock})");
    }
}
```

### Importing from Tab-Delimited Text File

```csharp
DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.TXT,
    FileName = @"C:\data\data.txt",
    Separator = DataImporter.Delimiter.Tab,    // Tab-delimited
    FileHeader = true
};

// Let the importer automatically detect the schema from headers
bool success = importer.ImportFromFile();

if (success && importer.WasCleanExecution)
{
    DataTable result = importer.ImportResultDataTable;

    // Display column information
    Console.WriteLine("Columns:");
    foreach (DataColumn col in result.Columns)
    {
        Console.WriteLine($"  {col.ColumnName} ({col.DataType.Name})");
    }

    Console.WriteLine($"\nTotal rows: {result.Rows.Count}");
}
```

### Importing from Fixed-Width Text File

```csharp
// Define fixed-width schema
DataStructure fixedSchema = new DataStructure
{
    Name = "FixedWidthData",
    Fields = new List<Field>
    {
        new Field("ID", nullable: false, DataImporter.FieldType.Integer, length: 10),
        new Field("Name", nullable: false, DataImporter.FieldType.String, length: 30),
        new Field("Department", nullable: false, DataImporter.FieldType.String, length: 20),
        new Field("Salary", nullable: false, DataImporter.FieldType.FloatingPoint, length: 15)
    }
};

DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.TXT,
    FileName = @"C:\data\fixed_width.txt",
    Separator = DataImporter.Delimiter.Lenght,    // Fixed-width columns
    FillerChar = ' ',                             // Space padding
    FileHeader = false,                           // No header row
    DataStructure = fixedSchema
};

bool success = importer.ImportFromFile();

if (success)
{
    DataTable result = importer.ImportResultDataTable;
    Console.WriteLine($"Imported {result.Rows.Count} fixed-width records");
}
```

### Importing with Custom Separator (Pipe-Delimited)

```csharp
DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.TXT,
    FileName = @"C:\data\pipe_delimited.txt",
    Separator = DataImporter.Delimiter.Separator,
    SeparatorChar = '|',        // Pipe separator
    FileHeader = true
};

bool success = importer.ImportFromFile();

if (success)
{
    DataTable result = importer.ImportResultDataTable;

    foreach (DataRow row in result.Rows)
    {
        // Process pipe-delimited data
        Console.WriteLine(string.Join(" | ", row.ItemArray));
    }
}
```

### Asynchronous Import with Progress Tracking

```csharp
DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.XLSX,
    FileName = @"C:\data\large_file.xlsx",
    FileHeader = true
};

// Subscribe to events
importer.OnStartImportation += (firedAt, records, progress, importType) =>
{
    Console.WriteLine($"Import started at {firedAt}");
    Console.WriteLine($"Total records to process: {records}");
};

importer.OnProgress += (firedAt, records, progress, importType) =>
{
    int percentage = (int)((progress * 100.0) / records);
    Console.WriteLine($"Progress: {percentage}% ({progress}/{records} rows)");
};

importer.OnCompletedImportation += (firedAt, records, importType, pathResult) =>
{
    Console.WriteLine($"Import completed at {firedAt}");
    Console.WriteLine($"Processed {records} records from {pathResult}");
};

// Import asynchronously (runs on background thread)
bool started = importer.ImportFromFile(GoAsync: true);

if (started)
{
    // Continue with other work while import runs in background
    Console.WriteLine("Import running in background...");

    // Wait for completion or do other work
    while (importer.ImportResultDataTable == null)
    {
        System.Threading.Thread.Sleep(100);
    }

    Console.WriteLine($"Final result: {importer.ImportResultDataTable.Rows.Count} rows");
}
```

### Importing Excel with Automatic Sheet Detection

```csharp
DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.XLSX,
    FileName = @"C:\data\workbook.xlsx",
    // SheetName not specified - will use first sheet automatically
    FileHeader = true
};

bool success = importer.ImportFromFile();

if (success)
{
    DataTable result = importer.ImportResultDataTable;
    Console.WriteLine($"Imported from sheet: {result.TableName}");
    Console.WriteLine($"Rows: {result.Rows.Count}, Columns: {result.Columns.Count}");
}
```

### Error Handling and Validation

```csharp
DataStructure schema = new DataStructure
{
    Name = "Users",
    Fields = new List<Field>
    {
        new Field("UserID", nullable: false, DataImporter.FieldType.Integer),
        new Field("Username", nullable: false, DataImporter.FieldType.String, length: 50),
        new Field("Email", nullable: false, DataImporter.FieldType.String, length: 100),
        new Field("Age", nullable: true, DataImporter.FieldType.Integer),
        new Field("IsActive", nullable: false, DataImporter.FieldType.Bit)
    }
};

DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.CSV,
    FileName = @"C:\data\users.csv",
    FileHeader = true,
    DataStructure = schema
};

bool success = importer.ImportFromFile();

if (success)
{
    if (importer.WasCleanExecution)
    {
        Console.WriteLine("Import completed without errors!");
        DataTable users = importer.ImportResultDataTable;
        Console.WriteLine($"Successfully imported {users.Rows.Count} users");
    }
    else
    {
        Console.WriteLine("Import completed with errors:");
        foreach (ErorrInfo error in importer.Errors)
        {
            Console.WriteLine($"  Row {error.Location}: {error.Description}");
        }

        // You can still access the partial data
        DataTable partialData = importer.ImportResultDataTable;
        Console.WriteLine($"Partial import: {partialData.Rows.Count} rows imported successfully");
    }
}
else
{
    Console.WriteLine("Import failed to start. Check file path and format.");
}
```

### CSV with Quoted Fields (Commas in Data)

The CSV importer properly handles quoted fields that contain commas:

```csharp
// Sample CSV content:
// Name,Address,City
// "John Doe","123 Main St, Apt 4B",Springfield
// "Jane Smith","456 Oak Ave, Suite 200",Portland

DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.CSV,
    FileName = @"C:\data\addresses.csv",
    FileHeader = true
};

bool success = importer.ImportFromFile();

if (success)
{
    DataTable result = importer.ImportResultDataTable;

    foreach (DataRow row in result.Rows)
    {
        // Commas inside quoted fields are preserved
        Console.WriteLine($"{row["Name"]} lives at {row["Address"]}, {row["City"]}");
    }
}
```

### Importing into Existing Database Table

```csharp
using ZidUtilities.CommonCode.DataAccess;

// Import from file
DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.XLSX,
    FileName = @"C:\data\orders.xlsx",
    FileHeader = true
};

if (importer.ImportFromFile() && importer.WasCleanExecution)
{
    DataTable importedData = importer.ImportResultDataTable;

    // Insert into database using SqlConnector
    SqlConnector connector = new SqlConnector(connectionString);

    foreach (DataRow row in importedData.Rows)
    {
        string insertQuery = @"
            INSERT INTO Orders (OrderID, CustomerName, Amount, OrderDate)
            VALUES (@orderId, @customerName, @amount, @orderDate)";

        var parameters = new Dictionary<string, string>
        {
            { "@orderId", row["OrderID"].ToString() },
            { "@customerName", row["CustomerName"].ToString() },
            { "@amount", row["Amount"].ToString() },
            { "@orderDate", row["OrderDate"].ToString() }
        };

        var response = connector.ExecuteNonQuery(insertQuery, parameters);

        if (!response.IsOK)
        {
            Console.WriteLine($"Failed to insert order {row["OrderID"]}: {response.Message}");
        }
    }
}
```

### Creating DataStructure from Existing DataTable

```csharp
// If you have an existing DataTable as a template
DataTable template = GetTemplateTable(); // Your existing table

// Create DataStructure from it
DataStructure schema = new DataStructure(template);

// Use it for import
DataImporter importer = new DataImporter
{
    ImportType = DataImporter.ImportFrom.CSV,
    FileName = @"C:\data\newdata.csv",
    FileHeader = true,
    DataStructure = schema    // Uses same structure as template
};

bool success = importer.ImportFromFile();
```

### Complete Import Workflow Example

```csharp
using ZidUtilities.CommonCode.Files;
using System;
using System.Data;
using System.IO;

public class ImportManager
{
    public DataTable ImportFile(string filePath, bool showProgress = true)
    {
        // Determine file type from extension
        string extension = Path.GetExtension(filePath).ToLower();

        DataImporter.ImportFrom importType;
        switch (extension)
        {
            case ".csv":
                importType = DataImporter.ImportFrom.CSV;
                break;
            case ".txt":
                importType = DataImporter.ImportFrom.TXT;
                break;
            case ".xls":
                importType = DataImporter.ImportFrom.XLS;
                break;
            case ".xlsx":
                importType = DataImporter.ImportFrom.XLSX;
                break;
            default:
                throw new ArgumentException($"Unsupported file type: {extension}");
        }

        // Create importer
        DataImporter importer = new DataImporter
        {
            ImportType = importType,
            FileName = filePath,
            FileHeader = true
        };

        // Add progress tracking if requested
        if (showProgress)
        {
            importer.OnStartImportation += (time, records, prog, type) =>
            {
                Console.WriteLine($"Starting import of {records} records...");
            };

            importer.OnProgress += (time, records, prog, type) =>
            {
                int percentage = (int)((prog * 100.0) / records);
                Console.Write($"\rProgress: {percentage}%");
            };

            importer.OnCompletedImportation += (time, records, type, path) =>
            {
                Console.WriteLine($"\nImport completed: {records} records");
            };
        }

        // Perform import
        bool success = importer.ImportFromFile();

        if (!success)
        {
            throw new Exception("Import failed to start");
        }

        if (!importer.WasCleanExecution)
        {
            Console.WriteLine("\nWarning: Import completed with errors:");
            foreach (var error in importer.Errors)
            {
                Console.WriteLine($"  Row {error.Location}: {error.Description}");
            }
        }

        return importer.ImportResultDataTable;
    }
}

// Usage
ImportManager manager = new ImportManager();
DataTable data = manager.ImportFile(@"C:\data\sales.xlsx", showProgress: true);
Console.WriteLine($"Imported {data.Rows.Count} rows");
```

## Import Best Practices

1. **Always check WasCleanExecution** after import to verify no errors occurred
2. **Use DataStructure** when you need specific data types or validation
3. **Subscribe to events** for long-running imports to provide user feedback
4. **Handle the Errors collection** to identify and log problematic rows
5. **Set FileHeader = true** when your file has column names in the first row
6. **For Excel files**, specify SheetName if you need a specific sheet (defaults to first sheet)
7. **Use GoAsync = true** for large files to keep UI responsive
8. **For fixed-width files**, ensure Field lengths exactly match your file format
9. **Set appropriate FieldType** values to enable automatic type conversion and validation
10. **Test with sample data** to verify the schema before processing large files

## Performance Tips

1. For large datasets (>10,000 rows), use `WidthAdjust.ByHeaders` or `WidthAdjust.ByFirst100Rows` for exports
2. Disable styles with `ExportWithStyles = false` for faster exports
3. Use streaming when exporting to web responses to reduce memory usage
4. Consider exporting to CSV for the fastest performance with large datasets
5. For imports, use asynchronous mode (`GoAsync = true`) for files over 10,000 rows
6. CSV format is fastest for both import and export operations
