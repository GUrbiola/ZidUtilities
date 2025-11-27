# SimpleDictionaryPersister Test Suite

## Overview
Comprehensive test suite for the `SimpleDictionaryPersister` class with **32 automated tests** covering all functionality.

## Test Results
✅ **32/32 Tests Passed (100% Pass Rate)**

## Test Coverage

### Basic Functionality (13 tests)
- ✅ Constructor sets default values
- ✅ Add/Update settings
- ✅ Get settings
- ✅ Contains setting check
- ✅ Remove setting
- ✅ Settings count
- ✅ Clear all settings
- ✅ Case-insensitive keys
- ✅ Indexer get/set operations

### PlainText Serialization (5 tests)
- ✅ Serialize without encryption
- ✅ Deserialize without encryption
- ✅ Serialize with encryption
- ✅ Deserialize with encryption
- ✅ Custom separator support

### JSON Serialization (4 tests)
- ✅ Serialize without encryption
- ✅ Deserialize without encryption
- ✅ Serialize with encryption
- ✅ Deserialize with encryption

### XML Serialization (4 tests)
- ✅ Serialize without encryption
- ✅ Deserialize without encryption
- ✅ Serialize with encryption
- ✅ Deserialize with encryption

### File Location Tests (2 tests)
- ✅ Save to Application Folder
- ✅ Custom file names

### Edge Cases (4 tests)
- ✅ Empty dictionary serialization
- ✅ Special characters in values
- ✅ Multiline values
- ✅ Enumerator iteration

## Running the Tests

### Option 1: From Visual Studio
1. Build the solution
2. Set `Tester` as the startup project
3. Run the project (F5 or Ctrl+F5)
4. Select option `1` when prompted

### Option 2: From Command Line
```bash
cd "D:\Just For Fun\ZidUtilities\Tester\bin\Debug"
Tester.exe
# Then enter '1' when prompted
```

### Option 3: Programmatically
```csharp
using Tester;

var tests = new SimpleDictionaryPersisterTests();
tests.RunAllTests();
```

## Test Output
The test suite provides:
- ✅ Green `[PASS]` for successful tests
- ❌ Red `[FAIL]` for failed tests with detailed error messages
- Summary statistics (Total, Passed, Failed, Pass Rate)

## Test Structure

Each test:
1. **Setup** - Creates necessary test data
2. **Execute** - Performs the operation being tested
3. **Assert** - Verifies expected results
4. **Cleanup** - Removes temporary files created during testing

## Tested Features

### Serialization Formats
- **PlainText**: Simple key-value pairs with configurable separator
- **JSON**: JSON format with proper structure
- **XML**: XML serialization using .NET's built-in serializer

### Encryption
- Tests both encrypted and unencrypted modes
- Verifies encryption actually encrypts (values not visible in plaintext)
- Verifies successful decryption with correct password
- Uses the modernized Crypter class with secure defaults

### Storage Locations
- Application Folder
- User AppData Folder (default)
- Common AppData Folder
- Custom Folder

## Files Created During Tests
All test files are automatically cleaned up after each test. Temporary files:
- `test_*.txt` - PlainText format tests
- `test_*.json` - JSON format tests
- `test_*.xml` - XML format tests

## Dependencies
- `ZidUtilities.CommonCode` - Contains SimpleDictionaryPersister and Crypter classes
- .NET Framework 4.8

## Code Quality
- Clean, readable test code
- Comprehensive coverage of all public methods
- Tests both happy paths and edge cases
- Self-contained (no external dependencies for tests)
- Automatic cleanup (no leftover test files)

## Future Enhancements
Potential additions:
- Performance benchmarks
- Concurrent access tests
- Large dataset tests
- Invalid input tests (malformed files, etc.)
- File permission error handling tests
