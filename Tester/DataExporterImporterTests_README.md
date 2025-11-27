# DataExporter/DataImporter Test Suite

## Overview
Comprehensive test suite for the `DataExporter` and `DataImporter` classes with **33 automated tests** covering all major functionality.

## Test Results
✅ **33/33 Tests Passed (100% Pass Rate)**

## Test Coverage

### DataExporter Tests (16 tests)
- ✅ Constructor sets default values
- ✅ Export DataTable to XLSX
- ✅ Export DataSet to XLSX (multiple sheets)
- ✅ Export without headers
- ✅ Export with styles (Default and Simple)
- ✅ Export with ignored columns
- ✅ Export to tab-delimited TXT
- ✅ Export to TXT with custom separator
- ✅ Export to CSV
- ✅ Export to Stream (XLSX)
- ✅ Export to Stream (CSV)
- ✅ Export List<T> to XLSX
- ✅ Export Dictionary<K,V> to XLSX
- ✅ Export ICollection<T> to XLSX
- ✅ Export empty DataTable
- ✅ Set Excel metadata (Author, Company, Subject, Title)

### DataImporter Tests (9 tests)
- ✅ Constructor sets default values
- ✅ Import from CSV
- ✅ Import from tab-delimited TXT
- ✅ Import from TXT with custom separator
- ✅ Import from XLSX
- ✅ Import with predefined DataStructure
- ✅ Import file without headers
- ✅ Handle errors gracefully
- ✅ Handle multiple data types (int, string, double, DateTime, bool)

### Round-Trip Tests (5 tests)
- ✅ XLSX: Export then import
- ✅ CSV: Export then import
- ✅ TXT: Export then import
- ✅ Preserve data types through round-trip
- ✅ Complex DataSet with multiple tables

### Edge Case Tests (3 tests)
- ✅ Special characters in data (@#$%^&*(), quotes, commas, tabs)
- ✅ Null/DBNull values
- ✅ Large datasets (1000+ rows)

## Supported Export Formats

### XLSX (Excel 2007+)
- Powered by ClosedXML library
- Supports multiple worksheets
- Customizable styles (Default, Simple)
- Alternate row coloring
- Auto-adjusting column widths
- Excel metadata (Author, Company, Subject, Title)
- Cell remarks and comments
- Formula support

### CSV (Comma-Separated Values)
- Standard CSV format
- Proper field escaping
- Header support
- Compatible with Excel, Google Sheets, etc.

### TXT (Text Files)
- Tab-delimited (default)
- Custom separator support
- Fixed-width format support
- Header support

## Supported Import Formats

### XLSX/XLS (Excel)
- Uses ExcelDataReader library
- Reads all worksheets
- Auto-detects column types
- Header row support
- Sheet name specification

### CSV
- Handles quoted fields
- Comma delimiter
- Header row support
- Automatic data type detection

### TXT
- Tab-delimited or custom separator
- Fixed-width format support
- Header row support
- Configurable filler characters

## Running the Tests

### Option 1: From Visual Studio
1. Build the solution
2. Set `Tester` as the startup project
3. Run the project (F5 or Ctrl+F5)
4. Select option `2` when prompted

### Option 2: From Command Line
```bash
cd "D:\Just For Fun\ZidUtilities\Tester\bin\Debug"
Tester.exe
# Then enter '2' when prompted
```

### Option 3: Programmatically
```csharp
using Tester;

var tests = new DataExporterImporterTests();
tests.RunAllTests();
```

## Example Usage

### DataExporter - Simple Export
```csharp
using CommonCode.Files;
using System.Data;

// Create sample data
var dt = new DataTable();
dt.Columns.Add("Name");
dt.Columns.Add("Age", typeof(int));
dt.Rows.Add("John Doe", 30);
dt.Rows.Add("Jane Smith", 25);

// Export to XLSX
var exporter = new DataExporter
{
    ExportType = ExportTo.XLSX,
    Author = "Your Name",
    Company = "Your Company"
};
exporter.ExportToFile("output.xlsx", dt);

// Export to CSV
exporter.ExportType = ExportTo.CSV;
exporter.ExportToFile("output.csv", dt);

// Export to TXT (tab-delimited)
exporter.ExportType = ExportTo.TXT;
exporter.Separator = "\t";
exporter.ExportToFile("output.txt", dt);
```

### DataExporter - Advanced Features
```csharp
var exporter = new DataExporter
{
    ExportType = ExportTo.XLSX,
    ExportWithStyles = true,
    ExportExcelStyle = ExcelStyle.Default,
    UseAlternateRowStyles = true,
    WriteHeaders = true,
    AutoCellAdjust = WidthAdjust.ByAllRows,
    Author = "Test Author",
    Company = "Test Company",
    Title = "Sales Report"
};

// Ignore specific columns
exporter.IgnoredColumns.Add("InternalID");
exporter.IgnoredColumns.Add("TempField");

// Export DataSet with multiple sheets
DataSet ds = new DataSet();
ds.Tables.Add(salesData);
ds.Tables.Add(summaryData);
exporter.ExportToFile("report.xlsx", ds);
```

### DataExporter - Export Collections
```csharp
// Export List<T>
List<Person> people = GetPeople();
exporter.ExportToFile("people.xlsx", people);

// Export Dictionary<K,V>
Dictionary<int, Employee> employees = GetEmployees();
exporter.ExportToFile("employees.xlsx", employees);

// Export to Stream
Stream stream = exporter.ExportToStream(dataTable);
// Use stream for download, email attachment, etc.
```

### DataImporter - Simple Import
```csharp
using CommonCode.Files;

// Import from CSV
var importer = new DataImporter
{
    FileName = "data.csv",
    ImportType = DataImporter.ImportFrom.CSV,
    FileHeader = true
};

if (importer.ImportFromFile())
{
    DataTable result = importer.ImportResultDataTable;
    Console.WriteLine($"Imported {result.Rows.Count} rows");
}
```

### DataImporter - Advanced Import
```csharp
// Define data structure
var structure = new DataStructure();
structure.Fields.Add(new Field("Name", nullable: true, type: DataImporter.FieldType.String));
structure.Fields.Add(new Field("Age", nullable: false, type: DataImporter.FieldType.Integer));
structure.Fields.Add(new Field("Salary", nullable: true, type: DataImporter.FieldType.FloatingPoint));
structure.Fields.Add(new Field("HireDate", nullable: true, type: DataImporter.FieldType.Date));
structure.Fields.Add(new Field("Active", nullable: false, type: DataImporter.FieldType.Bit));

var importer = new DataImporter
{
    FileName = "employees.xlsx",
    ImportType = DataImporter.ImportFrom.XLSX,
    SheetName = "EmployeeData",
    FileHeader = true,
    DataStructure = structure
};

if (importer.ImportFromFile())
{
    if (importer.WasCleanExecution)
    {
        Console.WriteLine("Import successful!");
    }
    else
    {
        Console.WriteLine($"Import completed with {importer.Errors.Count} errors:");
        foreach (var error in importer.Errors)
        {
            Console.WriteLine($"  Row {error.Location}: {error.Description}");
        }
    }
}
```

### DataImporter - Import from TXT with Custom Separator
```csharp
var importer = new DataImporter
{
    FileName = "data.txt",
    ImportType = DataImporter.ImportFrom.TXT,
    Separator = DataImporter.Delimiter.Separator,
    SeparatorChar = '|',  // Pipe-delimited
    FileHeader = true
};
importer.ImportFromFile();
```

## Key Features

### DataExporter
- **Multiple Format Support**: XLSX, CSV, TXT
- **Stream or File Export**: Export directly to file or to memory stream
- **Generic Type Support**: Export List<T>, Dictionary<K,V>, ICollection<T>
- **Styling Options**: Custom Excel styles, alternate row coloring
- **Column Control**: Ignore specific columns, auto-adjust widths
- **Excel Features**: Metadata, formulas, cell remarks
- **Async Support**: Background worker for large exports with progress events

### DataImporter
- **Multiple Format Support**: XLSX, XLS, CSV, TXT
- **Flexible Parsing**: Tab-delimited, custom separators, fixed-width
- **Type Safety**: Define expected data types with DataStructure
- **Error Handling**: Collects errors with row numbers for debugging
- **Header Support**: Optional header rows
- **Sheet Selection**: Choose specific worksheets in Excel files
- **Data Type Support**: Integer, Float, String, Character, Date, Boolean

## Dependencies

### DataExporter
- **ClosedXML**: Excel file generation
- **System.Data**: DataTable/DataSet support
- **System.ComponentModel**: Background worker for async operations

### DataImporter
- **ExcelDataReader**: Excel file reading
- **ExcelDataReader.DataSet**: Convert Excel to DataSet
- **System.Data**: DataTable support
- **System.IO**: File operations

## Test Data

Tests use:
- Sample DataTables with mixed types (string, int, double)
- Sample DataSets with multiple tables
- Generic collections (List<Person>, Dictionary<int, Person>)
- CSV/TXT files created on-the-fly
- Large datasets (1000+ rows)
- Special characters, null values, edge cases

All test data is created programmatically and cleaned up automatically.

## File Cleanup

The test suite automatically:
1. Creates a `TestData` directory in the application folder
2. Generates test files during tests
3. Cleans up all test files after completion

No manual cleanup required!

## Performance Notes

- **Small datasets** (< 100 rows): Near-instant export/import
- **Medium datasets** (100-1000 rows): < 1 second
- **Large datasets** (1000+ rows): 1-5 seconds
- **XLSX** is slower than CSV/TXT but provides more features
- **Stream exports** are faster than file exports for in-memory operations

## Common Use Cases

1. **Report Generation**: Export database queries to Excel for analysis
2. **Data Migration**: Import CSV files into application database
3. **Bulk Operations**: Export/import large datasets for batch processing
4. **File Conversion**: Convert between XLSX, CSV, and TXT formats
5. **Data Sharing**: Export application data for sharing with other tools
6. **Backup/Restore**: Export data for backup, import for restoration

## Error Handling

### DataExporter
- Validates file paths before export
- Handles empty DataTables gracefully
- Truncates oversized cell values (Excel limit: 32,750 characters)
- Supports DBNull values

### DataImporter
- Returns `false` if file doesn't exist
- Collects all errors during import (doesn't stop on first error)
- Provides row numbers for debugging
- Handles nullable fields properly
- Type conversion errors are logged but don't stop import

## Best Practices

1. **Always check import results**: Use `WasCleanExecution` and review `Errors` collection
2. **Define DataStructure for imports**: Better type safety and error messages
3. **Use appropriate format**: XLSX for features, CSV/TXT for speed
4. **Handle large datasets**: Consider async export with progress events
5. **Validate data before export**: Ensure data fits Excel constraints
6. **Set Excel metadata**: Improves file organization and searchability

## Troubleshooting

**Q: Import fails silently**
- Check that the file exists and is accessible
- Verify the file format matches the ImportType
- Check the Errors collection for details

**Q: Data types are wrong after import**
- Define a DataStructure with explicit types
- Use the appropriate FieldType for each column

**Q: Excel file is too large**
- Use CSV/TXT for very large datasets
- Consider splitting into multiple files
- Use stream export to avoid writing to disk

**Q: Special characters are corrupted**
- Ensure UTF-8 encoding for text files
- Use XLSX for guaranteed character support

## Future Enhancements

Potential improvements:
- Support for more Excel features (charts, pivot tables, etc.)
- JSON export/import
- XML export/import
- Progress reporting for synchronous operations
- Column mapping/transformation during import
- Data validation rules

---

**Test Suite Version**: 1.0
**Last Updated**: 2024
**Maintainer**: ZidUtilities Team
