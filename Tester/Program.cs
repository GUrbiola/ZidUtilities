using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZidUtilities.CommonCode;
using ZidUtilities.CommonCode.DataComparison;

namespace Tester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ZidUtilities Tester");
            Console.WriteLine("==================");
            Console.WriteLine();
            Console.WriteLine("Select a test to run:");
            Console.WriteLine("1. SimpleDictionaryPersister Tests");
            Console.WriteLine("2. DataExporter/DataImporter Tests");
            Console.WriteLine("3. Data Comparison Tests (Database required)");
            Console.WriteLine("4. Excel Styles Demo (Generate sample files)");
            Console.WriteLine();
            Console.Write("Enter choice (1, 2, 3, or 4): ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RunSimpleDictionaryPersisterTests();
                    break;
                case "2":
                    RunDataExporterImporterTests();
                    break;
                case "3":
                    RunDataComparisonTests();
                    break;
                case "4":
                    RunExcelStylesDemo();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Running SimpleDictionaryPersister tests by default...");
                    RunSimpleDictionaryPersisterTests();
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void RunSimpleDictionaryPersisterTests()
        {
            var tests = new SimpleDictionaryPersisterTests();
            tests.RunAllTests();
        }

        static void RunDataExporterImporterTests()
        {
            var tests = new DataExporterImporterTests();
            tests.RunAllTests();
        }

        static void RunExcelStylesDemo()
        {
            var demo = new ExcelStylesDemo();
            demo.GenerateAllStyleDemos();
        }

        static void RunDataComparisonTests()
        {
            SqlConnection conx = new SqlConnection("Data Source=GRD-WEBAPP-01\\SQLSERVER;Initial Catalog=Synchrony;Integrated Security=False;Persist Security Info=True;User ID=SqlAdmin;Password=SqlAdmin123");
            DataTable adData = null;
            DataTable adpData = GetAllRecordsFromAdp(conx);
            adpData.TableName = "ADP";

            if (File.Exists("ActiveDirectoryData.xml"))
            {
                adData = (DataTable)("ActiveDirectoryData.xml".DeserializeFromXmlFile());
            }
            else
            {
                adData = GetMatchedRecords(conx);
                adData.TableName = "ActiveDirectory";
                adData.SerializeToXmlFile("ActiveDirectoryData.xml");
            }

            if (File.Exists("ADPData.xml"))
            {
                adpData = (DataTable)("ADPData.xml".DeserializeFromXmlFile());
            }
            else
            {
                adpData = GetAllRecordsFromAdp(conx);
                adpData.TableName = "ADP";
                adpData.SerializeToXmlFile("ADPData.xml");
            }


            DataComparer comparer = new DataComparer();
            comparer.Label1 = "AD";
            comparer.Label2 = "ADP";
            comparer.PrimaryKeyFields.Add(0);// First column is the primary key (AssociateId)
            comparer.RunComparison(adData, adpData);

            comparer.GenerateXmlTrackFile("C:\\Temp\\DataComparisonReport.xml", "TestComparison", true, false);

            string notepadPlusPlusPath = @"C:\Program Files\Notepad++\notepad++.exe";
            if (File.Exists(notepadPlusPlusPath))
                System.Diagnostics.Process.Start(notepadPlusPlusPath, "C:\\Temp\\DataComparisonReport.xml");

            comparer.GenerateHtmlTrackFile("C:\\Temp\\DataComparisonReport.html", false, true);
            System.Diagnostics.Process.Start("C:\\Temp\\DataComparisonReport.html");
        }

        private static DataTable GetMatchedRecords(SqlConnection conx)
        {
            SqlCommand cmd = new SqlCommand("EXEC dbo.GetAllMatchedFromAd", conx);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable result = new DataTable();
            adapter.Fill(result);
            return result;
        }

        private static DataTable GetAllRecordsFromAdp(SqlConnection conx)
        {
            SqlCommand cmd = new SqlCommand("EXEC dbo.GetAllRecordsFromAdp", conx);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable result = new DataTable();
            adapter.Fill(result);
            return result;
        }
    }
}
