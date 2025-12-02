# ZidUtilities

Main executable project for the ZidUtilities solution - a comprehensive suite of utilities for Windows application development.

## Description

ZidUtilities serves as the main application project that demonstrates and utilizes the various utility libraries in the ZidUtilities solution. This project is the entry point for the ZidUtilities application.

## Solution Structure

The ZidUtilities solution consists of the following libraries:

### Core Libraries
- **CommonCode**: Core utility library with cryptography, extensions, and common functionality
- **WinControls**: Additional Windows Forms controls

### Text Editors
- **CommonCode.ICSharpTextEditor**: Windows Forms text editor with syntax highlighting
- **CommonCode.AvalonEdit**: WPF-based text editor utilities

### Data Access
- **CommonCode.DataAccess**: Generic SQL data access layer
- **CommonCode.DataAccess.Sqlite**: SQLite implementation
- **CommonCode.DataAccess.ActiveDirectory**: Active Directory management

### File Operations
- **CommonCode.Files**: File import/export (Excel, CSV, TXT, HTML)

### Windows Forms
- **CommonCode.Win**: Advanced Windows Forms controls including ZidGrid, dialogs, and themes

### Web
- **CommonCode.Web**: Web utilities for ASP.NET applications

### Logging
- **CommonCode.Log4Net**: Log4Net configuration and JSON logging

## Target Framework

.NET Framework 4.8

## Build

Open the solution file `ZidUtilities.sln` in Visual Studio and build the solution. All projects will be compiled, and this main executable will be built last.

## Testing Projects

The solution includes separate testing projects:
- **Tester**: Testing project for non-Windows Forms features
- **TesterWin**: Testing project for Windows Forms controls

## Usage

This application serves as a demonstration and test harness for all the utilities and controls provided by the ZidUtilities library suite.
