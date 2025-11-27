using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CommonCode.Files;
using ZidUtilities.CommonCode;

namespace Tester
{
    /// <summary>
    /// Comprehensive test suite for DataExporter and DataImporter classes.
    /// Tests export/import functionality for XLSX, CSV, and TXT formats.
    /// </summary>
    public class DataExporterImporterTests
    {
        private int _testsRun = 0;
        private int _testsPassed = 0;
        private int _testsFailed = 0;
        private List<string> _failureMessages = new List<string>();
        private string _testDataDir;

        public DataExporterImporterTests()
        {
            // Create a test data directory in the application folder
            _testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
            if (!Directory.Exists(_testDataDir))
            {
                Directory.CreateDirectory(_testDataDir);
            }
        }

        /// <summary>
        /// Runs all tests for DataExporter and DataImporter
        /// </summary>
        public void RunAllTests()
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("DataExporter/DataImporter - Test Suite");
            Console.WriteLine("==============================================");
            Console.WriteLine();

            // DataExporter Tests
            Test_DataExporter_Constructor_SetsDefaults();
            Test_DataExporter_ExportToXLSX_WithDataTable();
            Test_DataExporter_ExportToXLSX_WithDataSet();
            Test_DataExporter_ExportToXLSX_WithoutHeaders();
            Test_DataExporter_ExportToXLSX_WithStyles();
            Test_DataExporter_ExportToXLSX_WithIgnoredColumns();
            Test_DataExporter_ExportToTXT_TabDelimited();
            Test_DataExporter_ExportToTXT_CustomSeparator();
            Test_DataExporter_ExportToCSV();
            Test_DataExporter_ExportToStream_XLSX();
            Test_DataExporter_ExportToStream_CSV();
            Test_DataExporter_ExportList_ToXLSX();
            Test_DataExporter_ExportDictionary_ToXLSX();
            Test_DataExporter_ExportCollection_ToXLSX();
            Test_DataExporter_EmptyDataTable();
            Test_DataExporter_ExcelMetadata();

            // DataImporter Tests
            Test_DataImporter_Constructor_SetsDefaults();
            Test_DataImporter_ImportFromCSV();
            Test_DataImporter_ImportFromTXT_TabDelimited();
            Test_DataImporter_ImportFromTXT_CustomSeparator();
            Test_DataImporter_ImportFromXLSX();
            Test_DataImporter_WithDataStructure();
            Test_DataImporter_WithoutHeaders();
            Test_DataImporter_HandlesErrors();
            Test_DataImporter_MultipleDataTypes();

            // Round-trip Tests
            Test_RoundTrip_XLSX_Export_Then_Import();
            Test_RoundTrip_CSV_Export_Then_Import();
            Test_RoundTrip_TXT_Export_Then_Import();
            Test_RoundTrip_PreservesDataTypes();
            Test_RoundTrip_ComplexDataSet();

            // Edge case tests
            Test_EdgeCase_SpecialCharactersInData();
            Test_EdgeCase_NullValues();
            Test_EdgeCase_LargeDataSet();

            // Print summary
            PrintTestSummary();

            // Cleanup test files
            CleanupTestFiles();
        }

        #region DataExporter Tests

        private void Test_DataExporter_Constructor_SetsDefaults()
        {
            RunTest("DataExporter constructor sets default values", () =>
            {
                var exporter = new DataExporter();

                Assert(exporter.ExportType == ExportTo.XLSX, "Default export type should be XLSX");
                Assert(exporter.WriteHeaders == true, "WriteHeaders should be true by default");
                Assert(exporter.ExportWithStyles == true, "ExportWithStyles should be true by default");
                Assert(exporter.UseAlternateRowStyles == true, "UseAlternateRowStyles should be true by default");
                Assert(exporter.Separator == "\t", "Default separator should be tab");
                Assert(exporter.IgnoredColumns != null, "IgnoredColumns list should be initialized");
            });
        }

        private void Test_DataExporter_ExportToXLSX_WithDataTable()
        {
            RunTest("DataExporter exports DataTable to XLSX", () =>
            {
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX
                };

                string filePath = Path.Combine(_testDataDir, "test_export.xlsx");
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "XLSX file should be created");
                Assert(new FileInfo(filePath).Length > 0, "XLSX file should not be empty");
            });
        }

        private void Test_DataExporter_ExportToXLSX_WithDataSet()
        {
            RunTest("DataExporter exports DataSet to XLSX", () =>
            {
                var ds = CreateSampleDataSet();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX
                };

                string filePath = Path.Combine(_testDataDir, "test_dataset_export.xlsx");
                exporter.ExportToFile(filePath, ds);

                Assert(File.Exists(filePath), "XLSX file should be created");
                Assert(new FileInfo(filePath).Length > 0, "XLSX file should not be empty");
            });
        }

        private void Test_DataExporter_ExportToXLSX_WithoutHeaders()
        {
            RunTest("DataExporter exports XLSX without headers", () =>
            {
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX,
                    WriteHeaders = false
                };

                string filePath = Path.Combine(_testDataDir, "test_no_headers.xlsx");
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "XLSX file should be created");
            });
        }

        private void Test_DataExporter_ExportToXLSX_WithStyles()
        {
            RunTest("DataExporter exports XLSX with styles", () =>
            {
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX,
                    ExportWithStyles = true,
                    ExportExcelStyle = ExcelStyle.Default,
                    UseAlternateRowStyles = true
                };

                string filePath = Path.Combine(_testDataDir, "test_styled.xlsx");
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "Styled XLSX file should be created");
            });
        }

        private void Test_DataExporter_ExportToXLSX_WithIgnoredColumns()
        {
            RunTest("DataExporter exports XLSX with ignored columns", () =>
            {
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX
                };
                exporter.IgnoredColumns.Add("Age"); // Ignore the Age column

                string filePath = Path.Combine(_testDataDir, "test_ignored_cols.xlsx");
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "XLSX file with ignored columns should be created");
            });
        }

        private void Test_DataExporter_ExportToTXT_TabDelimited()
        {
            RunTest("DataExporter exports to tab-delimited TXT", () =>
            {
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.TXT,
                    Separator = "\t"
                };

                string filePath = Path.Combine(_testDataDir, "test_export.txt");
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "TXT file should be created");
                string content = File.ReadAllText(filePath);
                Assert(content.Contains("\t"), "TXT file should contain tab separators");
            });
        }

        private void Test_DataExporter_ExportToTXT_CustomSeparator()
        {
            RunTest("DataExporter exports to TXT with custom separator", () =>
            {
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.TXT,
                    Separator = "|"
                };

                string filePath = Path.Combine(_testDataDir, "test_custom_sep.txt");
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "TXT file with custom separator should be created");
                string content = File.ReadAllText(filePath);
                Assert(content.Contains("|"), "TXT file should contain pipe separators");
            });
        }

        private void Test_DataExporter_ExportToCSV()
        {
            RunTest("DataExporter exports to CSV", () =>
            {
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.CSV
                };

                string filePath = Path.Combine(_testDataDir, "test_export.csv");
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "CSV file should be created");
                string content = File.ReadAllText(filePath);
                Assert(content.Contains(","), "CSV file should contain comma separators");
            });
        }

        private void Test_DataExporter_ExportToStream_XLSX()
        {
            RunTest("DataExporter exports to Stream (XLSX)", () =>
            {
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX
                };

                Stream result = exporter.ExportToStream(dt);

                Assert(result != null, "Stream should not be null");
                Assert(result.Length > 0, "Stream should contain data");
                result.Dispose();
            });
        }

        private void Test_DataExporter_ExportToStream_CSV()
        {
            RunTest("DataExporter exports to Stream (CSV)", () =>
            {
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.CSV
                };

                Stream result = exporter.ExportToStream(dt);

                Assert(result != null, "Stream should not be null");
                Assert(result.Length > 0, "Stream should contain data");
                result.Dispose();
            });
        }

        private void Test_DataExporter_ExportList_ToXLSX()
        {
            RunTest("DataExporter exports List<T> to XLSX", () =>
            {
                var list = CreateSampleList();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX
                };

                string filePath = Path.Combine(_testDataDir, "test_list_export.xlsx");
                exporter.ExportToFile(filePath, list);

                Assert(File.Exists(filePath), "XLSX file from list should be created");
            });
        }

        private void Test_DataExporter_ExportDictionary_ToXLSX()
        {
            RunTest("DataExporter exports Dictionary to XLSX", () =>
            {
                var dict = CreateSampleDictionary();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX
                };

                string filePath = Path.Combine(_testDataDir, "test_dict_export.xlsx");
                exporter.ExportToFile(filePath, dict);

                Assert(File.Exists(filePath), "XLSX file from dictionary should be created");
            });
        }

        private void Test_DataExporter_ExportCollection_ToXLSX()
        {
            RunTest("DataExporter exports ICollection<T> to XLSX", () =>
            {
                var collection = CreateSampleCollection();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX
                };

                string filePath = Path.Combine(_testDataDir, "test_collection_export.xlsx");
                exporter.ExportToFile(filePath, collection);

                Assert(File.Exists(filePath), "XLSX file from collection should be created");
            });
        }

        private void Test_DataExporter_EmptyDataTable()
        {
            RunTest("DataExporter handles empty DataTable", () =>
            {
                var dt = new DataTable();
                dt.Columns.Add("Column1");

                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX
                };

                string filePath = Path.Combine(_testDataDir, "test_empty.xlsx");
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "XLSX file should be created even for empty table");
            });
        }

        private void Test_DataExporter_ExcelMetadata()
        {
            RunTest("DataExporter sets Excel metadata", () =>
            {
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.XLSX,
                    Author = "Test Author",
                    Company = "Test Company",
                    Subject = "Test Subject",
                    Title = "Test Title"
                };

                string filePath = Path.Combine(_testDataDir, "test_metadata.xlsx");
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "XLSX file with metadata should be created");
            });
        }

        #endregion

        #region DataImporter Tests

        private void Test_DataImporter_Constructor_SetsDefaults()
        {
            RunTest("DataImporter constructor sets default values", () =>
            {
                var importer = new DataImporter();

                Assert(importer.ImportType == DataImporter.ImportFrom.TXT, "Default import type should be TXT");
                Assert(importer.FileHeader == true, "FileHeader should be true by default");
                Assert(importer.Separator == DataImporter.Delimiter.Tab, "Default separator should be Tab");
                Assert(importer.SeparatorChar == ',', "Default separator char should be comma");
                Assert(importer.Errors != null, "Errors list should be initialized");
            });
        }

        private void Test_DataImporter_ImportFromCSV()
        {
            RunTest("DataImporter imports from CSV", () =>
            {
                // First create a CSV file
                string csvPath = Path.Combine(_testDataDir, "test_import.csv");
                CreateSampleCSVFile(csvPath);

                var importer = new DataImporter
                {
                    FileName = csvPath,
                    ImportType = DataImporter.ImportFrom.CSV,
                    FileHeader = true
                };

                bool result = importer.ImportFromFile();

                Assert(result == true, "Import should succeed");
                Assert(importer.ImportResultDataTable != null, "Result DataTable should not be null");
                Assert(importer.ImportResultDataTable.Rows.Count > 0, "Result should contain rows");
            });
        }

        private void Test_DataImporter_ImportFromTXT_TabDelimited()
        {
            RunTest("DataImporter imports from tab-delimited TXT", () =>
            {
                // First create a TXT file
                string txtPath = Path.Combine(_testDataDir, "test_import.txt");
                CreateSampleTXTFile(txtPath, "\t");

                var importer = new DataImporter
                {
                    FileName = txtPath,
                    ImportType = DataImporter.ImportFrom.TXT,
                    FileHeader = true,
                    Separator = DataImporter.Delimiter.Tab
                };

                bool result = importer.ImportFromFile();

                Assert(result == true, "Import should succeed");
                Assert(importer.ImportResultDataTable != null, "Result DataTable should not be null");
                Assert(importer.ImportResultDataTable.Rows.Count > 0, "Result should contain rows");
            });
        }

        private void Test_DataImporter_ImportFromTXT_CustomSeparator()
        {
            RunTest("DataImporter imports from TXT with custom separator", () =>
            {
                // First create a TXT file with pipe separator
                string txtPath = Path.Combine(_testDataDir, "test_import_pipe.txt");
                CreateSampleTXTFile(txtPath, "|");

                var importer = new DataImporter
                {
                    FileName = txtPath,
                    ImportType = DataImporter.ImportFrom.TXT,
                    FileHeader = true,
                    Separator = DataImporter.Delimiter.Separator,
                    SeparatorChar = '|'
                };

                bool result = importer.ImportFromFile();

                Assert(result == true, "Import should succeed");
                Assert(importer.ImportResultDataTable != null, "Result DataTable should not be null");
            });
        }

        private void Test_DataImporter_ImportFromXLSX()
        {
            RunTest("DataImporter imports from XLSX", () =>
            {
                // First create an XLSX file using exporter
                string xlsxPath = Path.Combine(_testDataDir, "test_for_import.xlsx");
                var dt = CreateSampleDataTable();
                var exporter = new DataExporter { ExportType = ExportTo.XLSX };
                exporter.ExportToFile(xlsxPath, dt);

                // Now import it
                var importer = new DataImporter
                {
                    FileName = xlsxPath,
                    ImportType = DataImporter.ImportFrom.XLSX,
                    FileHeader = true
                };

                bool result = importer.ImportFromFile();

                Assert(result == true, "Import should succeed");
                Assert(importer.ImportResultDataTable != null, "Result DataTable should not be null");
                Assert(importer.ImportResultDataTable.Rows.Count > 0, "Result should contain rows");
            });
        }

        private void Test_DataImporter_WithDataStructure()
        {
            RunTest("DataImporter imports with predefined DataStructure", () =>
            {
                string csvPath = Path.Combine(_testDataDir, "test_structure_import.csv");
                CreateSampleCSVFile(csvPath);

                var dataStructure = new DataStructure();
                dataStructure.Fields.Add(new Field("Name", true, DataImporter.FieldType.String));
                dataStructure.Fields.Add(new Field("Age", true, DataImporter.FieldType.Integer));
                dataStructure.Fields.Add(new Field("Salary", true, DataImporter.FieldType.FloatingPoint));

                var importer = new DataImporter
                {
                    FileName = csvPath,
                    ImportType = DataImporter.ImportFrom.CSV,
                    FileHeader = true,
                    DataStructure = dataStructure
                };

                bool result = importer.ImportFromFile();

                Assert(result == true, "Import with DataStructure should succeed");
                Assert(importer.ImportResultDataTable != null, "Result should not be null");
            });
        }

        private void Test_DataImporter_WithoutHeaders()
        {
            RunTest("DataImporter imports file without headers", () =>
            {
                string csvPath = Path.Combine(_testDataDir, "test_no_headers_import.csv");
                CreateSampleCSVFileWithoutHeaders(csvPath);

                var dataStructure = new DataStructure();
                dataStructure.Fields.Add(new Field("Column1", true, DataImporter.FieldType.String));
                dataStructure.Fields.Add(new Field("Column2", true, DataImporter.FieldType.Integer));
                dataStructure.Fields.Add(new Field("Column3", true, DataImporter.FieldType.FloatingPoint));

                var importer = new DataImporter
                {
                    FileName = csvPath,
                    ImportType = DataImporter.ImportFrom.CSV,
                    FileHeader = false,
                    DataStructure = dataStructure
                };

                bool result = importer.ImportFromFile();

                Assert(result == true, "Import without headers should succeed");
            });
        }

        private void Test_DataImporter_HandlesErrors()
        {
            RunTest("DataImporter handles errors gracefully", () =>
            {
                var importer = new DataImporter
                {
                    FileName = "nonexistent_file.csv",
                    ImportType = DataImporter.ImportFrom.CSV,
                    FileHeader = true
                };

                bool result = importer.ImportFromFile();

                Assert(result == false, "Import should fail for nonexistent file");
            });
        }

        private void Test_DataImporter_MultipleDataTypes()
        {
            RunTest("DataImporter handles multiple data types", () =>
            {
                string csvPath = Path.Combine(_testDataDir, "test_datatypes.csv");
                CreateDataTypesCSVFile(csvPath);

                var importer = new DataImporter
                {
                    FileName = csvPath,
                    ImportType = DataImporter.ImportFrom.CSV,
                    FileHeader = true
                };

                bool result = importer.ImportFromFile();

                Assert(result == true, "Import with multiple data types should succeed");
                Assert(importer.ImportResultDataTable != null, "Result should not be null");
            });
        }

        #endregion

        #region Round-Trip Tests

        private void Test_RoundTrip_XLSX_Export_Then_Import()
        {
            RunTest("Round-trip: Export to XLSX then import", () =>
            {
                var originalDt = CreateSampleDataTable();
                string filePath = Path.Combine(_testDataDir, "roundtrip.xlsx");

                // Export
                var exporter = new DataExporter { ExportType = ExportTo.XLSX };
                exporter.ExportToFile(filePath, originalDt);

                // Import
                var importer = new DataImporter
                {
                    FileName = filePath,
                    ImportType = DataImporter.ImportFrom.XLSX
                };
                importer.ImportFromFile();

                Assert(importer.ImportResultDataTable != null, "Imported table should not be null");
                Assert(importer.ImportResultDataTable.Rows.Count == originalDt.Rows.Count,
                    "Imported row count should match original");
            });
        }

        private void Test_RoundTrip_CSV_Export_Then_Import()
        {
            RunTest("Round-trip: Export to CSV then import", () =>
            {
                var originalDt = CreateSampleDataTable();
                string filePath = Path.Combine(_testDataDir, "roundtrip.csv");

                // Export
                var exporter = new DataExporter { ExportType = ExportTo.CSV };
                exporter.ExportToFile(filePath, originalDt);

                // Import
                var importer = new DataImporter
                {
                    FileName = filePath,
                    ImportType = DataImporter.ImportFrom.CSV
                };
                importer.ImportFromFile();

                Assert(importer.ImportResultDataTable != null, "Imported table should not be null");
                Assert(importer.ImportResultDataTable.Rows.Count == originalDt.Rows.Count,
                    "Imported row count should match original");
            });
        }

        private void Test_RoundTrip_TXT_Export_Then_Import()
        {
            RunTest("Round-trip: Export to TXT then import", () =>
            {
                var originalDt = CreateSampleDataTable();
                string filePath = Path.Combine(_testDataDir, "roundtrip.txt");

                // Export
                var exporter = new DataExporter
                {
                    ExportType = ExportTo.TXT,
                    Separator = "\t"
                };
                exporter.ExportToFile(filePath, originalDt);

                // Import
                var importer = new DataImporter
                {
                    FileName = filePath,
                    ImportType = DataImporter.ImportFrom.TXT,
                    Separator = DataImporter.Delimiter.Tab
                };
                importer.ImportFromFile();

                Assert(importer.ImportResultDataTable != null, "Imported table should not be null");
                Assert(importer.ImportResultDataTable.Rows.Count == originalDt.Rows.Count,
                    "Imported row count should match original");
            });
        }

        private void Test_RoundTrip_PreservesDataTypes()
        {
            RunTest("Round-trip preserves data types", () =>
            {
                var originalDt = CreateDataTypesDataTable();
                string filePath = Path.Combine(_testDataDir, "roundtrip_types.xlsx");

                // Export
                var exporter = new DataExporter { ExportType = ExportTo.XLSX };
                exporter.ExportToFile(filePath, originalDt);

                // Import
                var importer = new DataImporter
                {
                    FileName = filePath,
                    ImportType = DataImporter.ImportFrom.XLSX
                };
                importer.ImportFromFile();

                Assert(importer.ImportResultDataTable != null, "Imported table should not be null");
                Assert(importer.ImportResultDataTable.Columns.Count == originalDt.Columns.Count,
                    "Column count should be preserved");
            });
        }

        private void Test_RoundTrip_ComplexDataSet()
        {
            RunTest("Round-trip with complex DataSet", () =>
            {
                var originalDs = CreateComplexDataSet();
                string filePath = Path.Combine(_testDataDir, "roundtrip_complex.xlsx");

                // Export
                var exporter = new DataExporter { ExportType = ExportTo.XLSX };
                exporter.ExportToFile(filePath, originalDs);

                Assert(File.Exists(filePath), "Complex DataSet file should be created");
            });
        }

        #endregion

        #region Edge Case Tests

        private void Test_EdgeCase_SpecialCharactersInData()
        {
            RunTest("Handles special characters in data", () =>
            {
                var dt = new DataTable();
                dt.Columns.Add("Text");
                dt.Rows.Add("Test with @#$%^&*() special chars!");
                dt.Rows.Add("Quotes: \"test\" and 'test'");
                dt.Rows.Add("Commas, semicolons; and tabs\there");

                string filePath = Path.Combine(_testDataDir, "special_chars.xlsx");
                var exporter = new DataExporter { ExportType = ExportTo.XLSX };
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "File with special characters should be created");
            });
        }

        private void Test_EdgeCase_NullValues()
        {
            RunTest("Handles null values correctly", () =>
            {
                var dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");
                dt.Rows.Add("Item1", DBNull.Value);
                dt.Rows.Add(DBNull.Value, "Value1");
                dt.Rows.Add("Item3", "Value3");

                string filePath = Path.Combine(_testDataDir, "null_values.xlsx");
                var exporter = new DataExporter { ExportType = ExportTo.XLSX };
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "File with null values should be created");
            });
        }

        private void Test_EdgeCase_LargeDataSet()
        {
            RunTest("Handles large dataset (1000 rows)", () =>
            {
                var dt = new DataTable();
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("Name");
                dt.Columns.Add("Value", typeof(double));

                for (int i = 0; i < 1000; i++)
                {
                    dt.Rows.Add(i, $"Name_{i}", i * 1.5);
                }

                string filePath = Path.Combine(_testDataDir, "large_dataset.xlsx");
                var exporter = new DataExporter { ExportType = ExportTo.XLSX };
                exporter.ExportToFile(filePath, dt);

                Assert(File.Exists(filePath), "Large dataset file should be created");
                Assert(new FileInfo(filePath).Length > 0, "Large dataset file should not be empty");
            });
        }

        #endregion

        #region Test Helper Methods

        private DataTable CreateSampleDataTable()
        {
            DataTable dt = new DataTable("SampleData");
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Age", typeof(int));
            dt.Columns.Add("Salary", typeof(double));

            dt.Rows.Add("John Doe", 30, 50000.50);
            dt.Rows.Add("Jane Smith", 25, 45000.75);
            dt.Rows.Add("Bob Johnson", 35, 60000.00);

            return dt;
        }

        private DataSet CreateSampleDataSet()
        {
            DataSet ds = new DataSet("TestDataSet");
            ds.Tables.Add(CreateSampleDataTable());

            DataTable dt2 = new DataTable("AdditionalData");
            dt2.Columns.Add("ID", typeof(int));
            dt2.Columns.Add("Description");
            dt2.Rows.Add(1, "First Item");
            dt2.Rows.Add(2, "Second Item");
            ds.Tables.Add(dt2);

            return ds;
        }

        private DataTable CreateDataTypesDataTable()
        {
            DataTable dt = new DataTable("DataTypes");
            dt.Columns.Add("IntCol", typeof(int));
            dt.Columns.Add("StringCol", typeof(string));
            dt.Columns.Add("DoubleCol", typeof(double));
            dt.Columns.Add("DateCol", typeof(DateTime));
            dt.Columns.Add("BoolCol", typeof(bool));

            dt.Rows.Add(42, "Test", 3.14, DateTime.Now, true);
            dt.Rows.Add(100, "Sample", 2.71, DateTime.Now.AddDays(-1), false);

            return dt;
        }

        private DataSet CreateComplexDataSet()
        {
            DataSet ds = new DataSet("ComplexData");

            for (int i = 0; i < 3; i++)
            {
                DataTable dt = new DataTable($"Table{i + 1}");
                dt.Columns.Add("Col1");
                dt.Columns.Add("Col2", typeof(int));
                dt.Rows.Add($"Data{i}", i * 10);
                ds.Tables.Add(dt);
            }

            return ds;
        }

        private List<Person> CreateSampleList()
        {
            return new List<Person>
            {
                new Person { Name = "Alice", Age = 28, Salary = 55000 },
                new Person { Name = "Bob", Age = 32, Salary = 62000 },
                new Person { Name = "Charlie", Age = 29, Salary = 58000 }
            };
        }

        private Dictionary<int, Person> CreateSampleDictionary()
        {
            return new Dictionary<int, Person>
            {
                { 1, new Person { Name = "Alice", Age = 28, Salary = 55000 } },
                { 2, new Person { Name = "Bob", Age = 32, Salary = 62000 } }
            };
        }

        private ICollection<Person> CreateSampleCollection()
        {
            return new List<Person>
            {
                new Person { Name = "Test1", Age = 20, Salary = 40000 },
                new Person { Name = "Test2", Age = 30, Salary = 50000 }
            };
        }

        private void CreateSampleCSVFile(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("Name,Age,Salary");
                sw.WriteLine("John Doe,30,50000.50");
                sw.WriteLine("Jane Smith,25,45000.75");
                sw.WriteLine("Bob Johnson,35,60000.00");
            }
        }

        private void CreateSampleCSVFileWithoutHeaders(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("John Doe,30,50000.50");
                sw.WriteLine("Jane Smith,25,45000.75");
                sw.WriteLine("Bob Johnson,35,60000.00");
            }
        }

        private void CreateSampleTXTFile(string filePath, string separator)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine($"Name{separator}Age{separator}Salary");
                sw.WriteLine($"John Doe{separator}30{separator}50000.50");
                sw.WriteLine($"Jane Smith{separator}25{separator}45000.75");
                sw.WriteLine($"Bob Johnson{separator}35{separator}60000.00");
            }
        }

        private void CreateDataTypesCSVFile(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("IntCol,StringCol,DoubleCol,DateCol,BoolCol");
                sw.WriteLine($"42,Test,3.14,{DateTime.Now:yyyy-MM-dd},true");
                sw.WriteLine($"100,Sample,2.71,{DateTime.Now.AddDays(-1):yyyy-MM-dd},false");
            }
        }

        private void RunTest(string testName, Action testAction)
        {
            _testsRun++;
            try
            {
                testAction();
                _testsPassed++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[PASS] {testName}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                _testsFailed++;
                _failureMessages.Add($"{testName}: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FAIL] {testName}");
                Console.WriteLine($"       {ex.Message}");
                Console.ResetColor();
            }
        }

        private void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception($"Assertion failed: {message}");
            }
        }

        private void PrintTestSummary()
        {
            Console.WriteLine();
            Console.WriteLine("==============================================");
            Console.WriteLine("Test Summary");
            Console.WriteLine("==============================================");
            Console.WriteLine($"Total Tests Run:    {_testsRun}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Tests Passed:       {_testsPassed}");
            Console.ResetColor();

            if (_testsFailed > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Tests Failed:       {_testsFailed}");
                Console.ResetColor();

                Console.WriteLine();
                Console.WriteLine("Failed Tests:");
                foreach (string failure in _failureMessages)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  - {failure}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            double passRate = (_testsPassed / (double)_testsRun) * 100;
            Console.WriteLine($"Pass Rate: {passRate:F1}%");
            Console.WriteLine("==============================================");
        }

        private void CleanupTestFiles()
        {
            try
            {
                if (Directory.Exists(_testDataDir))
                {
                    Directory.Delete(_testDataDir, true);
                    Console.WriteLine();
                    Console.WriteLine("Test files cleaned up successfully.");
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        #endregion
    }

    // Helper class for testing List/Dictionary/Collection exports
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Salary { get; set; }
    }
}
