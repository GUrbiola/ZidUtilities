# ZidUtilities Solution

A comprehensive suite of reusable .NET libraries and utilities for Windows application development.

## Overview

ZidUtilities is a collection of libraries providing common functionality for .NET Framework 4.8 applications, with a focus on Windows Forms development, data access, file operations, and enterprise integration.

## Projects

### Core Libraries

#### CommonCode
Core utility library providing foundational functionality:
- Security & Cryptography (encryption, password generation)
- Data management (extensions, serialization, token lists)
- Data comparison and difference detection
- Image processing
- Server utilities

### Text Editors

#### CommonCode.ICSharpTextEditor
Advanced text editor control for Windows Forms with:
- Syntax highlighting for multiple languages
- Code folding and bracket matching
- Extensible plugin architecture
- Customizable toolbar

#### CommonCode.AvalonEdit
Utilities and extensions for AvalonEdit WPF-based text editor.

### Data Access

#### CommonCode.DataAccess
Generic data access layer providing unified interface for SQL databases with connection management, query execution, and transaction support.

#### CommonCode.DataAccess.Sqlite
SQLite-specific implementation for embedded database solutions.

#### CommonCode.DataAccess.ActiveDirectory
Active Directory management and query utilities for enterprise directory services.

### File Operations

#### CommonCode.Files
File import/export utilities supporting:
- Excel (XLSX) with multiple themes
- CSV and TXT with custom separators
- HTML with filterable tables

### Windows Forms

#### CommonCode.Win
Comprehensive Windows Forms controls and utilities featuring:
- **ZidGrid**: Advanced data grid with filtering, themes, and plugin architecture
- **Custom Controls**: AnimatedWaitTextBox, VerticalProgressBar, ToastForm, ThemeManager
- **Dialogs**: ProcessingDialog, MultiSelectionDialog, TextInputDialog, and more
- **Theming**: Unified theme system (ZidThemes) for consistent UI

#### WinControls
Additional Windows Forms controls library extending CommonCode.Win.

### Web

#### CommonCode.Web
Web utilities and helpers for ASP.NET applications.

### Logging

#### CommonCode.Log4Net
Log4Net configuration and management with:
- JSON-formatted logging
- Multiple appenders (Console, RollingFile, JSON)
- Programmatic configuration
- Structured logging support

### Applications

#### ZidUtilities
Main executable project demonstrating the utility libraries.

### Testing Projects

- **Tester**: Testing project for core features
- **TesterWin**: Testing project for Windows Forms controls

## Target Framework

All projects target .NET Framework 4.8

## Getting Started

### Quick Start

1. **Clone or download** the repository
2. **Open** `ZidUtilities.sln` in Visual Studio 2017 or later
3. **Build** the solution (Build > Build Solution or Ctrl+Shift+B)
4. **Reference** the required DLLs in your project
5. **Explore** the example projects (Tester, TesterWin, ZidUtilities)

### Adding to Your Project

#### Option 1: Direct DLL Reference

```
1. Build the ZidUtilities solution
2. In your project, right-click References > Add Reference
3. Browse to the bin\Debug or bin\Release folder
4. Select the DLLs you need (e.g., CommonCode.dll, CommonCode.Win.dll)
5. Click OK
```

#### Option 2: Project Reference

```
1. Right-click your solution > Add > Existing Project
2. Browse and add the library projects you need
3. In your project, right-click References > Add Reference
4. Go to Projects tab and select the libraries
5. Click OK
```

### Quick Examples

#### Encrypt Data

```csharp
using ZidUtilities.CommonCode;

var crypter = new Crypter();
string encrypted = crypter.Encrypt("sensitive data", "your-password");
string decrypted = crypter.Decrypt(encrypted, "your-password");
```

#### Export Data to Excel

```csharp
using ZidUtilities.CommonCode.Files;

var exporter = new DataExporter
{
    ExportType = ExportTo.XLSX,
    ExportExcelStyle = ExcelStyle.Professional,
    WriteHeaders = true
};

exporter.ExportDataToFile(myDataSet, @"C:\output\report.xlsx");
```

#### Show a Processing Dialog

```csharp
using ZidUtilities.CommonCode.Win.Dialogs;

using (var dialog = new ProcessingDialog())
{
    dialog.Title = "Processing";
    dialog.Message = "Please wait...";
    dialog.ShowProgressBar = true;

    dialog.DoWork += (s, e) =>
    {
        // Your long-running operation
        for (int i = 0; i < 100; i++)
        {
            dialog.ReportProgress(i);
            System.Threading.Thread.Sleep(50);
        }
    };

    dialog.ShowDialog();
}
```

#### Use ZidGrid with Themes

```csharp
using ZidUtilities.CommonCode.Win.Controls;

var grid = new ZidGrid
{
    Dock = DockStyle.Fill,
    ReadOnly = true
};

grid.DataSource = GetYourData();
grid.ApplyTheme(ZidThemes.Professional);

this.Controls.Add(grid);
```

## Key Features

- Advanced data grid control with themes and plugins
- Comprehensive dialog system with unified styling
- File import/export with multiple format support
- Enterprise Active Directory integration
- Embedded SQLite database support
- Advanced text editors for Windows Forms and WPF
- JSON-based structured logging
- Cryptography and security utilities
- Extensive extension methods and helpers

## Documentation

Each project contains its own comprehensive README.md with:
- Detailed feature descriptions
- Installation instructions
- Usage examples with code
- Best practices
- Common use cases

### Documentation Index

- [CommonCode](CommonCode/README.md) - Core utilities, encryption, password generation
- [CommonCode.Files](CommonCode.Files/README.md) - Data export (Excel, CSV, HTML, TXT)
- [CommonCode.DataAccess](CommonCode.DataAccess/README.md) - Generic data access layer
- [CommonCode.DataAccess.Sqlite](CommonCode.DataAccess.Sqlite/README.md) - SQLite database access
- [CommonCode.DataAccess.ActiveDirectory](CommonCode.DataAccess.ActiveDirectory/README.md) - Active Directory management
- [CommonCode.Win](CommonCode.Win/README.md) - Windows Forms controls, dialogs, themes
- [CommonCode.Log4Net](CommonCode.Log4Net/README.md) - Logging configuration and JSON logging
- [CommonCode.ICSharpTextEditor](CommonCode.ICSharpTextEditor/README.md) - Text editor for Windows Forms
- [CommonCode.AvalonEdit](CommonCode.AvalonEdit/README.md) - Text editor utilities for WPF
- [WinControls](WinControls/README.md) - Additional Windows Forms controls

## Project Structure

```
ZidUtilities/
├── CommonCode/                      # Core utilities
│   ├── Crypter.cs                  # Encryption/decryption
│   ├── PasswordGenerator.cs        # Password generation
│   ├── Extensions.cs               # Extension methods
│   └── ...
├── CommonCode.Win/                  # Windows Forms controls
│   ├── Controls/
│   │   ├── ZidGrid.cs             # Advanced data grid
│   │   ├── ToastForm.cs           # Toast notifications
│   │   └── ...
│   ├── Dialogs/
│   │   ├── ProcessingDialog.cs    # Progress dialogs
│   │   ├── TextInputDialog.cs     # Input dialogs
│   │   └── ...
│   └── ThemeManager.cs            # Unified theming
├── CommonCode.Files/                # File operations
│   ├── DataExporter.cs            # Export to Excel/CSV/HTML
│   └── DataImporter.cs            # Import from files
├── CommonCode.DataAccess/           # Generic data access
│   ├── SqlConnector.cs            # Base connector
│   └── SqlResponse.cs             # Response object
├── CommonCode.DataAccess.Sqlite/    # SQLite implementation
├── CommonCode.DataAccess.ActiveDirectory/ # AD integration
├── CommonCode.Log4Net/              # Logging utilities
├── CommonCode.ICSharpTextEditor/    # WinForms text editor
├── CommonCode.AvalonEdit/           # WPF text editor
├── WinControls/                     # Additional controls
├── ZidUtilities/                    # Main demo application
├── Tester/                          # Test project
└── TesterWin/                       # WinForms test project
```

## Requirements

- **Visual Studio**: 2017 or later
- **.NET Framework**: 4.8
- **Operating System**: Windows 7 or later

### Optional Dependencies (NuGet)

The solution uses several NuGet packages:
- `log4net` - For logging functionality
- `Newtonsoft.Json` - For JSON operations
- `ClosedXML` - For Excel export
- `System.Data.SQLite` - For SQLite database access
- `AvalonEdit` - For WPF text editor

These are automatically restored when building the solution.

## Building the Solution

### Visual Studio

```
1. Open ZidUtilities.sln
2. Restore NuGet packages (automatic on build)
3. Build > Build Solution (or press Ctrl+Shift+B)
4. Output DLLs will be in each project's bin\Debug or bin\Release folder
```

### Command Line

```bash
# Using MSBuild
cd "D:\Just For Fun\ZidUtilities"
msbuild ZidUtilities.sln /p:Configuration=Release

# Using .NET CLI (if available)
dotnet build ZidUtilities.sln --configuration Release
```

## Common Use Cases

### Desktop Applications
- Business applications with data grids
- Admin tools and utilities
- Database management tools
- Report generators
- File processors

### Enterprise Integration
- Active Directory user management
- LDAP queries and authentication
- Employee directory applications
- Access control systems

### Data Processing
- Excel report generation
- CSV/TXT file import/export
- Data transformation
- Batch processing

### Security
- Data encryption/decryption
- Password generation
- File encryption
- Secure credential storage

## Troubleshooting

### Build Errors

**Missing NuGet packages**
```
Solution: Right-click solution > Restore NuGet Packages
```

**Framework version mismatch**
```
Solution: Ensure .NET Framework 4.8 is installed
Download from: https://dotnet.microsoft.com/download/dotnet-framework/net48
```

**ICSharpCode.TextEditor not found**
```
Solution: The library is included in the repository.
Check that it's properly referenced in the project.
```

### Runtime Errors

**System.Data.SQLite not loading**
```
Solution: Ensure correct platform (x86/x64) version is referenced
NuGet package includes both versions
```

**Log4Net configuration errors**
```
Solution: Use Log4NetConfigHelper for programmatic configuration
Avoid XML-based configuration for simplicity
```

## Contributing

This is a personal utilities library, but contributions and suggestions are welcome:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request with detailed description

## License

See individual project directories for licensing information.

## Support

For issues, questions, or suggestions:
- Check individual project README files
- Review the example projects (Tester, TesterWin, ZidUtilities)
- Examine the code documentation and XML comments

## Version History

- See commit history for detailed changes
- Each major update includes new features and improvements
- Breaking changes are documented in commit messages
