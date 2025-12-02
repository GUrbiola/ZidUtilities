# CommonCode.Log4Net

Log4Net configuration helper library for programmatically generating log4net XML configuration files with support for console, rolling file, and JSON appenders.

## Features

- **Programmatic Configuration**: Generate log4net XML configuration files using C# code instead of manual XML editing
- **Multiple Appender Types**: Support for Console, Rolling File, and JSON appenders
- **Automatic JSON Member Generation**: Convert pattern layouts to JSON members automatically
- **Type-Safe Configuration**: Strongly-typed classes for all configuration elements
- **Easy Integration**: Generate config files and load them with standard log4net API

## Core Components

### Log4NetConfigHelper

Main helper class for building and saving log4net configuration files.

**Properties:**
- `RootLevel` - Root logging level (ALL, DEBUG, INFO, WARN, ERROR, FATAL, OFF)
- `Appenders` - List of `IAppender` objects to include in configuration

**Methods:**
- `SaveConfigToFile(string filePath)` - Generates and saves the XML configuration file
- `GetConfigXmlString(List<string> appenderNames)` - Returns the XML configuration as a string

### Appender Classes

**ConsoleAppender** - Colored console output
- Static method: `GetDefault()` - Returns pre-configured console appender
- Static method: `DefaultMappings()` - Returns default color mappings for log levels
- Static method: `DefaultLayout()` - Returns default pattern layout

**RollingFileAppender** - File logging with rolling capabilities
- Properties: `FilePath`, `AppendToFile`, `RollingStyle`, `MaxFileSize`, `MaxSizeRollBackups`, `DatePattern`
- Static method: `GetDefault()` - Returns pre-configured file appender
- Static method: `DefaultLayout()` - Returns default pattern layout

**JsonFileAppender** - JSON-formatted file logging
- Properties: `FilePath`, `AppendToFile`, `RollingStyle`, `DatePattern`, `Members`
- Static method: `GetDefault()` - Returns pre-configured JSON appender
- Static method: `GetDefaultJsonMembers()` - Returns default JSON member list
- Static method: `GetMembersFromPattern(string pattern)` - Converts pattern layout to JSON members

### Supporting Classes

**Log4NetLayout** - Layout configuration for text-based appenders
- `Type` - Layout type (e.g., "log4net.Layout.PatternLayout")
- `ConversionPattern` - Pattern string for formatting log messages

**Log4NetJsonMember** - JSON member configuration
- `Name` - Member name in JSON output
- `Value` - Value pattern (can be log4net pattern tokens or literal values)

**Log4NetMapping** - Color mapping for console appender
- `Level` - Log level (DEBUG, INFO, WARN, ERROR, FATAL)
- `ForeColor` - Foreground color
- `BackColor` - Background color

## Dependencies

- log4net (NuGet package)
- log4net.Ext.Json (for JSON appender support)

## Target Framework

.NET Framework 4.8

## Installation

1. Install log4net NuGet package:
   ```
   Install-Package log4net
   ```

2. For JSON logging support, install:
   ```
   Install-Package log4net.Ext.Json
   ```

3. Add reference to `CommonCode.Log4Net.dll` in your project

## Usage Examples

### Basic Console Appender Configuration

```csharp
using log4net;
using log4net.Config;
using ZidUtilities.CommonCode.Log4Net;

// Create configuration helper
Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";

// Add console appender using defaults
config.Appenders.Add(ConsoleAppender.GetDefault());

// Save configuration to file
config.SaveConfigToFile("log4net.config");

// Load configuration into log4net
XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

// Get logger and start logging
ILog log = LogManager.GetLogger(typeof(Program));
log.Debug("This is a debug message");
log.Info("This is an info message");
log.Warn("This is a warning message");
log.Error("This is an error message");
log.Fatal("This is a fatal message");
```

### Rolling File Appender with Custom Settings

```csharp
using log4net;
using log4net.Config;
using ZidUtilities.CommonCode.Log4Net;

Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";

// Create rolling file appender
RollingFileAppender rollingFileAppender = new RollingFileAppender()
{
    Name = "RollingFileAppender",
    LoggingLevel = "ALL",
    FilePath = "logs/application.log",
    AppendToFile = true,
    RollingStyle = "Date",        // Date-based rolling
    DatePattern = "yyyyMMdd",      // Creates new file each day
    Layout = new Log4NetLayout
    {
        Type = "log4net.Layout.PatternLayout",
        ConversionPattern = "[%date] [%thread] [%-5level] %message%newline%exception"
    }
};

config.Appenders.Add(rollingFileAppender);

// Save and load configuration
config.SaveConfigToFile("log4net.config");
XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

// Use logger
ILog log = LogManager.GetLogger("MyApp");
log.Info("Application started");
```

### Size-Based Rolling File Appender

```csharp
Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";

// Size-based rolling
RollingFileAppender sizeRollingAppender = new RollingFileAppender()
{
    Name = "SizeRollingAppender",
    LoggingLevel = "ALL",
    FilePath = "logs/app.log",
    AppendToFile = true,
    RollingStyle = "Size",
    MaxFileSize = "10MB",
    MaxSizeRollBackups = 5,        // Keep 5 backup files
    Layout = RollingFileAppender.DefaultLayout()
};

config.Appenders.Add(sizeRollingAppender);
config.SaveConfigToFile("log4net.config");
XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
```

### JSON File Appender

```csharp
using log4net;
using log4net.Config;
using ZidUtilities.CommonCode.Log4Net;
using System.Collections.Generic;

Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";

// Create JSON appender
JsonFileAppender jsonAppender = new JsonFileAppender()
{
    Name = "JsonFileAppender",
    LoggingLevel = "ALL",
    FilePath = "logs/application.json",
    AppendToFile = true,
    RollingStyle = "Date",
    DatePattern = "yyyyMMdd",
    Members = JsonFileAppender.GetDefaultJsonMembers()
};

config.Appenders.Add(jsonAppender);

// Save and load configuration

//FROM FILE
config.SaveConfigToFile("log4net.config");
XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

//OR
//FROM STREAM
//XmlConfigurator.Configure(l4Config.GetConfigXmlStream());

ILog log = LogManager.GetLogger("MyApp");
log.Info("User logged in");
log.Error("Payment failed", new Exception("Insufficient funds"));

// JSON output format:
// {"date":"2024-01-15 10:30:45","level":"INFO","logger":"MyApp","message":"User logged in","exception":""}
// {"date":"2024-01-15 10:30:46","level":"ERROR","logger":"MyApp","message":"Payment failed","exception":"System.Exception: Insufficient funds..."}
```

### JSON Appender with Custom Members

```csharp
Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";

// Define custom JSON structure
List<Log4NetJsonMember> customMembers = new List<Log4NetJsonMember>
{
    new Log4NetJsonMember { Name = "timestamp", Value = "date" },
    new Log4NetJsonMember { Name = "severity", Value = "level" },
    new Log4NetJsonMember { Name = "source", Value = "logger" },
    new Log4NetJsonMember { Name = "text", Value = "messageObject" },
    new Log4NetJsonMember { Name = "error", Value = "exception" },
    new Log4NetJsonMember { Name = "thread_id", Value = "thread" },
    new Log4NetJsonMember { Name = "user", Value = "username" }
};

JsonFileAppender jsonAppender = new JsonFileAppender()
{
    Name = "CustomJsonAppender",
    LoggingLevel = "ALL",
    FilePath = "logs/custom.json",
    AppendToFile = true,
    RollingStyle = "Date",
    DatePattern = "yyyyMMdd",
    Members = customMembers
};

config.Appenders.Add(jsonAppender);
XmlConfigurator.Configure(config.GetConfigXmlStream());
```

### Auto-Generate JSON Members from Pattern

The `JsonFileAppender.GetMembersFromPattern()` method automatically converts a pattern layout string into JSON members:

```csharp
Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";

// Define your desired pattern
string pattern = "%date [%thread] [%username] [%identity] %level [%location] [%file] [%line] [%class] [%method] %logger - %message%newline%exception";

// Automatically convert pattern to JSON members
JsonFileAppender jsonAppender = new JsonFileAppender()
{
    Name = "AutoJsonAppender",
    LoggingLevel = "ALL",
    FilePath = "logs/auto.json",
    AppendToFile = true,
    RollingStyle = "Date",
    DatePattern = "yyyyMMdd",
    Members = JsonFileAppender.GetMembersFromPattern(pattern)
};

config.Appenders.Add(jsonAppender);
config.SaveConfigToFile("log4net.config");
XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

// The generated JSON will include all fields from the pattern:
// date, thread, username, identity, level, location, file, line, class, method, logger, message, exception
```

### Multiple Appenders Configuration

```csharp
using log4net;
using log4net.Config;
using ZidUtilities.CommonCode.Log4Net;

Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";

// 1. Console appender for immediate feedback
config.Appenders.Add(ConsoleAppender.GetDefault());

// 2. Rolling file appender for text logs
RollingFileAppender textAppender = new RollingFileAppender()
{
    Name = "TextFileAppender",
    LoggingLevel = "ALL",
    FilePath = "logs/app.log",
    AppendToFile = true,
    RollingStyle = "Date",
    DatePattern = "yyyyMMdd",
    Layout = new Log4NetLayout
    {
        Type = "log4net.Layout.PatternLayout",
        ConversionPattern = "[%date] [%thread] [%-5level] %logger - %message%newline%exception"
    }
};
config.Appenders.Add(textAppender);

// 3. JSON appender for structured logging
string jsonPattern = "%date [%thread] %level %logger - %message%exception";
JsonFileAppender jsonAppender = new JsonFileAppender()
{
    Name = "JsonFileAppender",
    LoggingLevel = "ALL",
    FilePath = "logs/app.json",
    AppendToFile = true,
    RollingStyle = "Date",
    DatePattern = "yyyyMMdd",
    Members = JsonFileAppender.GetMembersFromPattern(jsonPattern)
};
config.Appenders.Add(jsonAppender);

//load configuration
XmlConfigurator.Configure(config.GetConfigXmlStream());

// All appenders will receive log messages
ILog log = LogManager.GetLogger("MyApp");
log.Info("This message goes to console, text file, and JSON file");
```

### Custom Console Appender Colors

```csharp
using ZidUtilities.CommonCode.Log4Net;
using System.Collections.Generic;

Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";

// Create custom color mappings
List<Log4NetMapping> customColors = new List<Log4NetMapping>
{
    new Log4NetMapping { Level = "DEBUG", ForeColor = "Cyan", BackColor = "Black" },
    new Log4NetMapping { Level = "INFO", ForeColor = "Green, HighIntensity", BackColor = "Black" },
    new Log4NetMapping { Level = "WARN", ForeColor = "Yellow, HighIntensity", BackColor = "Black" },
    new Log4NetMapping { Level = "ERROR", ForeColor = "Red, HighIntensity", BackColor = "Black" },
    new Log4NetMapping { Level = "FATAL", ForeColor = "White, HighIntensity", BackColor = "Red" }
};

ConsoleAppender consoleAppender = new ConsoleAppender
{
    Name = "CustomConsoleAppender",
    LoggingLevel = "ALL",
    Mappings = customColors,
    Layout = new Log4NetLayout
    {
        Type = "log4net.Layout.PatternLayout",
        ConversionPattern = "[%date{HH:mm:ss}] [%-5level] %message%newline"
    }
};

config.Appenders.Add(consoleAppender);
config.SaveConfigToFile("log4net.config");
XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
```

### Complete Application Example

```csharp
using System;
using log4net;
using log4net.Config;
using ZidUtilities.CommonCode.Log4Net;

public class Program
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Program));

    static void Main(string[] args)
    {
        // Configure logging first
        ConfigureLogging();

        log.Info("Application started");

        try
        {
            // Your application logic
            DoWork();

            log.Info("Application completed successfully");
        }
        catch (Exception ex)
        {
            log.Fatal("Application crashed", ex);
            throw;
        }
    }

    private static void ConfigureLogging()
    {
        Log4NetConfigHelper config = new Log4NetConfigHelper();
        config.RootLevel = "ALL";

        // Console appender
        config.Appenders.Add(ConsoleAppender.GetDefault());

        // Rolling file appender
        RollingFileAppender fileAppender = new RollingFileAppender()
        {
            Name = "RollingFileAppender",
            LoggingLevel = "ALL",
            FilePath = "logs/application.log",
            AppendToFile = true,
            RollingStyle = "Date",
            DatePattern = "yyyyMMdd",
            Layout = new Log4NetLayout
            {
                Type = "log4net.Layout.PatternLayout",
                ConversionPattern = "[%date] [%thread] [%-5level] %logger - %message%newline%exception"
            }
        };
        config.Appenders.Add(fileAppender);

        // JSON appender
        JsonFileAppender jsonAppender = new JsonFileAppender()
        {
            Name = "JsonFileAppender",
            LoggingLevel = "ALL",
            FilePath = "logs/application.json",
            AppendToFile = true,
            RollingStyle = "Date",
            DatePattern = "yyyyMMdd",
            Members = JsonFileAppender.GetDefaultJsonMembers()
        };
        config.Appenders.Add(jsonAppender);

        // Save configuration and load it
        config.SaveConfigToFile("log4net.config");
        XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
    }

    private static void DoWork()
    {
        log.Debug("Starting work process");

        for (int i = 0; i < 10; i++)
        {
            log.InfoFormat("Processing item {0} of {1}", i + 1, 10);

            try
            {
                ProcessItem(i);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error processing item {0}: {1}", i, ex.Message);
            }
        }

        log.Debug("Work process completed");
    }

    private static void ProcessItem(int itemId)
    {
        if (itemId == 5)
        {
            log.Warn("Item 5 requires special handling");
        }

        // Simulate work
        System.Threading.Thread.Sleep(100);

        if (itemId == 7)
        {
            throw new InvalidOperationException("Item 7 processing failed");
        }
    }
}
```

### Different Logging Levels Example

```csharp
Log4NetConfigHelper config = new Log4NetConfigHelper();

// Set root level to INFO - DEBUG messages won't be logged
config.RootLevel = "INFO";

config.Appenders.Add(ConsoleAppender.GetDefault());
config.SaveConfigToFile("log4net.config");
XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

ILog log = LogManager.GetLogger("MyApp");

log.Debug("This will NOT be logged");        // Below INFO level
log.Info("This will be logged");             // INFO level
log.Warn("This will be logged");             // WARN level
log.Error("This will be logged");            // ERROR level
log.Fatal("This will be logged");            // FATAL level

// Root levels: ALL < DEBUG < INFO < WARN < ERROR < FATAL < OFF
```

### Separate Appenders for Different Log Levels

```csharp
Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";

// General log - all levels
RollingFileAppender generalLog = new RollingFileAppender()
{
    Name = "GeneralLog",
    LoggingLevel = "ALL",
    FilePath = "logs/general.log",
    AppendToFile = true,
    RollingStyle = "Date",
    DatePattern = "yyyyMMdd",
    Layout = RollingFileAppender.DefaultLayout()
};
config.Appenders.Add(generalLog);

// Error log - only errors and fatal
RollingFileAppender errorLog = new RollingFileAppender()
{
    Name = "ErrorLog",
    LoggingLevel = "ERROR",       // Only ERROR and FATAL
    FilePath = "logs/errors.log",
    AppendToFile = true,
    RollingStyle = "Date",
    DatePattern = "yyyyMMdd",
    Layout = new Log4NetLayout
    {
        Type = "log4net.Layout.PatternLayout",
        ConversionPattern = "[%date] [%-5level] %logger - %message%newline%exception"
    }
};
config.Appenders.Add(errorLog);

config.SaveConfigToFile("log4net.config");
XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

ILog log = LogManager.GetLogger("MyApp");
log.Info("This goes only to general.log");
log.Error("This goes to both general.log and errors.log");
```

### Dynamic Pattern Configuration

```csharp
Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";

// Build pattern based on requirements
string pattern = "%date";

bool includeThread = true;
bool includeUsername = false;
bool includeLevel = true;
bool includeLogger = true;
bool includeLocation = false;

if (includeThread)
    pattern += " [%thread]";

if (includeUsername)
    pattern += " [%username]";

if (includeLevel)
    pattern += " [%-5level]";

if (includeLogger)
    pattern += " %logger";

if (includeLocation)
    pattern += " [%location]";

pattern += " - %message%newline%exception";

// Apply to rolling file
RollingFileAppender appender = new RollingFileAppender()
{
    Name = "DynamicAppender",
    LoggingLevel = "ALL",
    FilePath = "logs/dynamic.log",
    AppendToFile = true,
    RollingStyle = "Date",
    DatePattern = "yyyyMMdd",
    Layout = new Log4NetLayout
    {
        Type = "log4net.Layout.PatternLayout",
        ConversionPattern = pattern
    }
};

config.Appenders.Add(appender);
config.SaveConfigToFile("log4net.config");
XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
```

### Getting the Generated XML Configuration

You can retrieve the XML configuration as a string without saving to a file:

```csharp
Log4NetConfigHelper config = new Log4NetConfigHelper();
config.RootLevel = "ALL";
config.Appenders.Add(ConsoleAppender.GetDefault());

// Get appender names
List<string> appenderNames = config.Appenders.Select(a => a.Name).ToList();

// Get XML string
string xmlConfig = config.GetConfigXmlString(appenderNames);

Console.WriteLine(xmlConfig);

// Output:
// <log4net>
//   <root>
//     <level value="ALL" />
//     <appender-ref ref="ConsoleAppender" />
//   </root>
//   <appender name="ConsoleAppender" type="log4net.Appender.ColoredConsoleAppender">
//     ...
//   </appender>
// </log4net>
```

## Common Pattern Layout Tokens

Use these tokens in your `ConversionPattern`:

- `%date` or `%date{format}` - Log timestamp (e.g., `%date{yyyy-MM-dd HH:mm:ss}`)
- `%thread` - Thread ID
- `%level` or `%-5level` - Log level (padded to 5 characters)
- `%logger` - Logger name
- `%message` - Log message
- `%exception` - Exception information with stack trace
- `%newline` - Platform-specific line separator
- `%username` - Current Windows username
- `%identity` - Current thread identity
- `%location` - Full location (class.method:line)
- `%class` - Class name
- `%method` - Method name
- `%file` - Source file name
- `%line` - Line number
- `%property{name}` - Custom property value

## Log Level Hierarchy

From least to most restrictive:
1. **ALL** - Logs everything
2. **DEBUG** - Detailed diagnostic information
3. **INFO** - Informational messages
4. **WARN** - Warning messages
5. **ERROR** - Error events
6. **FATAL** - Critical errors
7. **OFF** - Logging disabled

When you set a level, all levels above it are also logged. For example, setting `INFO` will log INFO, WARN, ERROR, and FATAL, but not DEBUG.

## Best Practices

1. **Always configure logging at application startup** before any logging calls
2. **Use meaningful appender names** for easier debugging
3. **Set appropriate root levels** for production vs. development environments
4. **Use date-based rolling for production** to organize logs by day
5. **Use size-based rolling for high-volume applications** to prevent huge files
6. **Include exception details** using `%exception` in your pattern
7. **Separate error logs** for easier monitoring and alerting
8. **Use JSON for log aggregation systems** (ELK, Splunk, etc.)
9. **Check generated XML** by examining the saved log4net.config file
10. **Test configuration** in development before deploying to production

## File Organization

Recommended folder structure:
```
YourApp/
├── bin/
│   └── log4net.config          (generated config file)
├── logs/
│   ├── application.log         (rolling text logs)
│   ├── application.20241201.log
│   ├── application.20241202.log
│   ├── errors.log              (error-only logs)
│   ├── application.json        (JSON logs)
│   └── application.20241201.json
└── Program.cs
```

## Common Patterns

### Development Configuration
```csharp
config.RootLevel = "DEBUG";
config.Appenders.Add(ConsoleAppender.GetDefault());
// Shows all debug info in console
```

### Production Configuration
```csharp
config.RootLevel = "INFO";
// Rolling file for general logs
// Separate file for errors
// JSON for log aggregation
```

### Testing Configuration
```csharp
config.RootLevel = "WARN";
// Only warnings and errors during tests
```

## Troubleshooting

**Logs not appearing:**
- Verify `XmlConfigurator.Configure()` is called before logging
- Check that generated log4net.config file exists
- Verify root level allows your log level
- Ensure output directory exists for file appenders

**JSON appender not working:**
- Install `log4net.Ext.Json` NuGet package
- Verify Members list is populated
- Check file path has `.json` extension

**Configuration errors:**
- Examine generated log4net.config XML for syntax errors
- Validate pattern syntax in ConversionPattern
- Ensure all required properties are set on appenders

## Related Resources

- [log4net Documentation](https://logging.apache.org/log4net/)
- [log4net Pattern Layout](https://logging.apache.org/log4net/release/sdk/log4net.Layout.PatternLayout.html)
- [log4net.Ext.Json](https://github.com/log4net/log4net.Ext.Json)
