using System;
using System.Data;
using System.IO;
using ZidUtilities.CommonCode.Files;

namespace ZidUtilities.Tester
{
    /// <summary>
    /// Demonstrates all 14 Excel styles available in DataExporter.
    /// Creates sample Excel files showcasing each style.
    /// </summary>
    public class ExcelStylesDemo
    {
        private string _outputDir;

        public ExcelStylesDemo()
        {
            _outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StyleDemos");
            if (!Directory.Exists(_outputDir))
            {
                Directory.CreateDirectory(_outputDir);
            }
        }

        /// <summary>
        /// Creates a sample DataTable with employee data for demonstration purposes.
        /// </summary>
        private DataTable CreateSampleData()
        {
            var dt = new DataTable("Employees");
            dt.Columns.Add("Employee ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Salary", typeof(decimal));
            dt.Columns.Add("Start Date", typeof(DateTime));

            dt.Rows.Add(1001, "John Smith", "Sales", 65000, new DateTime(2018, 3, 15));
            dt.Rows.Add(1002, "Jane Doe", "Marketing", 72000, new DateTime(2019, 7, 22));
            dt.Rows.Add(1003, "Mike Johnson", "IT", 85000, new DateTime(2017, 1, 10));
            dt.Rows.Add(1004, "Sarah Williams", "HR", 58000, new DateTime(2020, 5, 8));
            dt.Rows.Add(1005, "Robert Brown", "Finance", 78000, new DateTime(2016, 11, 3));
            dt.Rows.Add(1006, "Emily Davis", "Sales", 63000, new DateTime(2021, 2, 14));
            dt.Rows.Add(1007, "David Wilson", "IT", 92000, new DateTime(2015, 9, 25));
            dt.Rows.Add(1008, "Lisa Garcia", "Marketing", 69000, new DateTime(2019, 4, 12));

            return dt;
        }

        /// <summary>
        /// Generates Excel files demonstrating all 14 available styles.
        /// </summary>
        public void GenerateAllStyleDemos()
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("Excel Styles Demonstration");
            Console.WriteLine("==============================================");
            Console.WriteLine();

            var styles = new[]
            {
                ExcelStyle.Default,
                ExcelStyle.Simple,
                ExcelStyle.Ocean,
                ExcelStyle.Forest,
                ExcelStyle.Sunset,
                ExcelStyle.Monochrome,
                ExcelStyle.Corporate,
                ExcelStyle.Mint,
                ExcelStyle.Lavender,
                ExcelStyle.Autumn,
                ExcelStyle.Steel,
                ExcelStyle.Cherry,
                ExcelStyle.Sky,
                ExcelStyle.Charcoal
            };

            foreach (var style in styles)
            {
                GenerateStyleDemo(style);
            }

            Console.WriteLine();
            Console.WriteLine($"All style demonstrations saved to: {_outputDir}");
            Console.WriteLine();
            Console.WriteLine("Style descriptions:");
            Console.WriteLine("  Default     - Navy blue headers with light blue alternating rows");
            Console.WriteLine("  Simple      - Minimal styling with gray bottom border");
            Console.WriteLine("  Ocean       - Deep blue ocean theme - Professional and calming");
            Console.WriteLine("  Forest      - Forest green theme - Natural and balanced");
            Console.WriteLine("  Sunset      - Warm sunset orange theme - Energetic and modern");
            Console.WriteLine("  Monochrome  - Black and white - Minimalist and elegant");
            Console.WriteLine("  Corporate   - Corporate blue-gray theme - Executive and professional");
            Console.WriteLine("  Mint        - Fresh mint green theme - Clean and modern");
            Console.WriteLine("  Lavender    - Elegant lavender purple theme - Sophisticated");
            Console.WriteLine("  Autumn      - Autumn brown and orange theme - Warm and earthy");
            Console.WriteLine("  Steel       - Steel gray and silver theme - Modern and sleek");
            Console.WriteLine("  Cherry      - Cherry red theme - Bold and energetic");
            Console.WriteLine("  Sky         - Light sky blue theme - Airy and professional");
            Console.WriteLine("  Charcoal    - Charcoal dark theme - Strong and professional");
        }

        /// <summary>
        /// Generates a single Excel file with the specified style.
        /// </summary>
        private void GenerateStyleDemo(ExcelStyle style)
        {
            string fileName = Path.Combine(_outputDir, $"{style}_Style_Demo.xlsx");
            var data = CreateSampleData();

            var exporter = new DataExporter
            {
                ExportType = ExportTo.XLSX,
                ExportExcelStyle = style,
                ExportWithStyles = true,
                UseAlternateRowStyles = true,
                WriteHeaders = true,
                AutoCellAdjust = WidthAdjust.ByAllRows,
                Author = "ZidUtilities",
                Company = "Excel Styles Demo",
                Title = $"{style} Style Demonstration",
                Subject = "DataExporter Style Showcase"
            };

            exporter.ExportToFile(fileName, data);
            Console.WriteLine($"[CREATED] {style}_Style_Demo.xlsx");
        }

        /// <summary>
        /// Cleans up the demo files directory.
        /// </summary>
        public void CleanupDemos()
        {
            if (Directory.Exists(_outputDir))
            {
                Directory.Delete(_outputDir, true);
                Console.WriteLine($"Cleaned up demo files from: {_outputDir}");
            }
        }
    }
}
