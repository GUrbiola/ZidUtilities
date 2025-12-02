using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZidUtilities.CommonCode.Files
{
    /// <summary>
    /// Specify the source format to import data from.
    /// </summary>
    public enum ImportFrom
    {
        /// <summary>
        /// Plain text file with arbitrary delimiter or fixed-length records (TXT).
        /// </summary>
        TXT,
        /// <summary>
        /// Comma-separated values text file (CSV).
        /// </summary>
        CSV,
        /// <summary>
        /// Excel 97-2003 workbook (.xls).
        /// </summary>
        XLS,
        /// <summary>
        /// Excel workbook (.xlsx).
        /// </summary>
        XLSX
    }

    /// <summary>
    /// Defines the kind of delimiter used when reading text files.
    /// Note: the member name 'Lenght' is preserved for backward compatibility and indicates fixed-length records.
    /// </summary>
    public enum Delimiter
    {
        /// <summary>
        /// Tab-delimited fields (uses '\t' as separator).
        /// </summary>
        Tab,
        /// <summary>
        /// A configurable separator character is used (see `SeparatorChar`).
        /// </summary>
        Separator,
        /// <summary>
        /// Fixed-width / fixed-length fields (uses per-field Length values from the DataStructure).
        /// </summary>
        Lenght
    }

    /// <summary>
    /// Describes the semantic data type of a field as used by the importer.
    /// This enumeration is used when creating DataColumns and parsing values from source files.
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// Integer numeric type (whole numbers).
        /// </summary>
        Integer,
        /// <summary>
        /// Floating-point numeric type (decimals).
        /// </summary>
        FloatingPoint,
        /// <summary>
        /// Single character field.
        /// </summary>
        Character,
        /// <summary>
        /// Text / string field.
        /// </summary>
        String,
        /// <summary>
        /// Date/time field.
        /// </summary>
        Date,
        /// <summary>
        /// Boolean / bit field.
        /// </summary>
        Bit
    }

    /// <summary>
    /// Delegate used to report import progress or start notifications.
    /// </summary>
    /// <param name="FiredAt">Timestamp when the event was fired.</param>
    /// <param name="Records">Total number of records expected to process.</param>
    /// <param name="Progress">Current progress value (semantic depends on caller: either processed count or percent).</param>
    /// <param name="ExportType">The source format being imported (see <see cref="ImportFrom"/>).</param>
    public delegate void ImportEvent(DateTime FiredAt, int Records, int Progress, ImportFrom ExportType);

    /// <summary>
    /// Delegate used to report completion of an import operation.
    /// </summary>
    /// <param name="FiredAt">Timestamp when the completion event was fired.</param>
    /// <param name="Records">Total number of records processed.</param>
    /// <param name="ExportType">The source format that was imported (see <see cref="ImportFrom"/>).</param>
    /// <param name="PathResult">Path or filename of the source file that was imported.</param>
    public delegate void ImportCompletedEvent(DateTime FiredAt, int Records, ImportFrom ExportType, string PathResult);

    /// <summary>
    /// Class that reads a file and imports the data into a DataTable according to a defined DataStructure.
    /// </summary>
    /// <remarks>
    /// This class provides methods to import data from various file formats into a structured DataTable, before importing the
    /// property DataStructure must be initialized with the fields to read. It supports text files (with configurable delimiters or fixed-length records)
    /// and Excel files (.xls, .xlsx).
    /// </remarks>
    public partial class DataImporter
    {
        /// <summary>
        /// Raised when an import operation begins. Handlers typically initialize progress UI or logging.
        /// </summary>
        public event ImportEvent OnStartImportation;

        /// <summary>
        /// Raised when an import operation completes (synchronously or asynchronously).
        /// Handlers typically finalize progress UI, log summary, or trigger downstream processing.
        /// </summary>
        public event ImportCompletedEvent OnCompletedImportation;

        /// <summary>
        /// Raised periodically to report progress during an import operation.
        /// The event provides timing, total record count, current progress, and import source type.
        /// </summary>
        public event ImportEvent OnProgress;

        /// <summary>
        /// Collection of errors collected during the last import attempt.
        /// Each entry contains a description and a location (for example, row number).
        /// Clear this list before starting a new import or inspect it after completion to determine issues.
        /// </summary>
        public List<ErorrInfo> Errors { get; set; }

        private DataStructure _DataStructure;
        /// <summary>
        /// Structure/schema of the data to be imported, defining fields and their types.
        /// </summary>
        public DataStructure DataStructure
        {
            get { return _DataStructure; }
            set { _DataStructure = value; }
        }
        [DefaultValue(true), Description("Defines if the file will have a header on it.")]
        public bool FileHeader { get; set; }
        [DefaultValue(Delimiter.Tab), Description("Defines the Kind of separators the file will have.")]
        public Delimiter Separator { get; set; }
        [DefaultValue(','), Description("Character used in the file as separator.")]
        public char SeparatorChar { get; set; }
        [DefaultValue(ImportFrom.TXT), Description("Defines the source from which the data will be retrieved.")]
        public ImportFrom ImportType { get; set; }
        [DefaultValue(""), Description("Path and Filename of the Source File.")]
        public string FileName { get; set; }
        [DefaultValue("Sheet1"), Description("Defines the name of the page to read, in case the source file is a .xls or a xlsx file.")]
        public string SheetName { get; set; }
        [DefaultValue(' '), Description("Char used to fill a field, when the data is smaller than the fixed length for that field.")]
        public char FillerChar { get; set; }
        [DefaultValue("Reading Records..."), Description("String showed in the progress window, when the records are being read")]
        public string ReadingString { get; set; }
        [DefaultValue("Reading Data From @FileName"), Description("String showed in the progress window title, when the records are being read")]
        public string ReadingTitleString { get; set; }
        public DataTable ImportResultDataTable { get; private set; }
        public bool WasCleanExecution { get { return Errors.Count == 0; } }

        private BackgroundWorker AsyncImporter;
        private bool RunningAsync;
        private int ThRecordCount;
        private int ThCurrentRecord;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataImporter"/> class with default settings.
        /// </summary>
        /// <remarks>
        /// Sets default values for the properties used in the import process, including separator characters,
        /// import type, file name, and reading status strings.
        /// </remarks>
        public DataImporter()
        {
            ReadingTitleString = "Reading Data From @FileName";
            ReadingString = "Reading Records...";
            FillerChar = ' ';
            //SheetName = "Sheet1";
            FileName = "";
            ImportType = ImportFrom.TXT;
            SeparatorChar = ',';
            Separator = Delimiter.Tab;
            FileHeader = true;
            Errors = new List<ErorrInfo>();
            ImportResultDataTable = null;
            DataStructure = new DataStructure();
        }

        /// <summary>
        /// Imports data from the configured source file into a DataTable.
        /// </summary>
        /// <param name="GoAsync">If true the import will run asynchronously using a BackgroundWorker; if false it runs synchronously.</param>
        /// <remarks>
        /// Initializes the result DataTable structure based on the DataStructure property,
        /// then reads the source file (text or Excel) and fills the DataTable with the imported data.
        /// Reports progress and handles errors according to the settings.
        /// </remarks>
        /// <returns>True if the import operation was started (or completed synchronously); false if there was nothing to import (no rows or file missing).</returns>
        public bool ImportFromFile(bool GoAsync = false)
        {
            ImportResultDataTable = new DataTable();
            CleanErrorInfo();
            ThRecordCount = RowCount();
            RunningAsync = GoAsync;

            if (ThRecordCount == 0)
            {
                return false;
            }

            foreach (Field f in DataStructure.Fields)
            {
                DataColumn aux = new DataColumn(f.Name);
                aux.AllowDBNull = f.Nullable;
                switch (f.FieldType)
                {
                    case FieldType.Integer:
                        aux.DataType = typeof(int);
                        break;
                    case FieldType.FloatingPoint:
                        aux.DataType = typeof(float);
                        break;
                    case FieldType.Character:
                        aux.DataType = typeof(char);
                        break;
                    case FieldType.String:
                        aux.DataType = typeof(string);
                        if (f.Length > 0)
                            aux.MaxLength = f.Length;
                        break;
                    case FieldType.Date:
                        aux.DataType = typeof(DateTime);
                        break;
                    case FieldType.Bit:
                        aux.DataType = typeof(bool);
                        break;
                }
                ImportResultDataTable.Columns.Add(aux);
            }

            if (GoAsync)
            {
                if (AsyncImporter != null || (AsyncImporter != null && AsyncImporter.IsBusy))
                {
                    AsyncImporter.CancelAsync();
                    AsyncImporter = null;
                }
                AsyncImporter = new BackgroundWorker();
                AsyncImporter.WorkerReportsProgress = true;
                AsyncImporter.WorkerSupportsCancellation = false;
                AsyncImporter.DoWork += new DoWorkEventHandler(AsyncImporter_DoWork);
                AsyncImporter.ProgressChanged += new ProgressChangedEventHandler(AsyncImporter_ProgressChanged);
                AsyncImporter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AsyncImporter_RunWorkerCompleted);

                if (OnStartImportation != null)
                    OnStartImportation(DateTime.Now, ThRecordCount, 0, ImportType);

                AsyncImporter.RunWorkerAsync();
            }
            else
            {
                if (OnStartImportation != null)
                    OnStartImportation(DateTime.Now, ThRecordCount, 0, ImportType);

                switch (ImportType)
                {
                    case ImportFrom.TXT:
                        ImportFromTxtFile();
                        break;
                    case ImportFrom.CSV:
                        Separator = Delimiter.Separator;
                        SeparatorChar = ',';
                        ImportFromTxtFile();
                        break;
                    case ImportFrom.XLS:
                    case ImportFrom.XLSX:
                        ImportFromExcelFile();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (OnCompletedImportation != null)
                    OnCompletedImportation(DateTime.Now, ThRecordCount, ImportType, FileName);
            }
            return true;
        }

        /// <summary>
        /// Reads data from an Excel file (XLS or XLSX) and loads it into the import result DataTable.
        /// </summary>
        /// <remarks>
        /// The method uses ExcelDataReader to open and read the workbook. If <see cref="SheetName"/> is empty,
        /// the first sheet name is selected. If the DataStructure is not previously defined it will be created from the sheet.
        /// </remarks>
        /// <returns>None. Populates <see cref="ImportResultDataTable"/> and may update <see cref="DataStructure"/>.</returns>
        private void ImportFromExcelFile()
        {
            DataSet dataOnFile;
            DataTable tableOnFile;

            if (String.IsNullOrEmpty(SheetName))
            {
                SheetName = GetExcelSheetNames(FileName)[0];
            }

            using (var stream = File.Open(FileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    dataOnFile = reader.AsDataSet(GetDefaultConfig());
                    tableOnFile = dataOnFile.Tables[SheetName];
                }
            }

            if (tableOnFile != null && tableOnFile.Rows != null && dataOnFile.Tables[SheetName].Rows.Count > 0)
            {
                if (DataStructure == null || DataStructure.Fields == null || DataStructure.Fields.Count == 0)
                    DataStructure = new DataStructure(tableOnFile);

                ImportResultDataTable = tableOnFile;
            }
            else if (tableOnFile != null)
            {
                ImportResultDataTable = tableOnFile.Clone();
            }
            else
            {
                ImportResultDataTable = null;
            }
        }

        /// <summary>
        /// Reads data from a text-based file (TXT or CSV) according to the configured delimiter and data structure,
        /// and fills the <see cref="ImportResultDataTable"/>.
        /// </summary>
        /// <remarks>
        /// Handles fixed-length records (when <see cref="Delimiter.Lenght"/> is selected) and delimited records.
        /// Populates DataStructure when a header row is present but fields are not yet defined.
        /// Records any parse or read errors into the <see cref="Errors"/> collection.
        /// </remarks>
        /// <returns>None. Populates <see cref="ImportResultDataTable"/> and appends any parsing errors to <see cref="Errors"/>.</returns>
        private void ImportFromTxtFile()
        {
            string line;
            float auxfloat;
            DateTime auxdate;
            bool auxbool;
            int RowNumber = 0, position, lenght, auxint;
            string[] RawData;

            using (StreamReader Rdr = new StreamReader(FileName))
            {
                if (FileHeader)
                {
                    string buff = Rdr.ReadLine();
                    if (DataStructure == null || DataStructure.Fields == null || DataStructure.Fields.Count == 0)
                    {
                        string[] buff2 = buff.Split(Separator == Delimiter.Tab ? new char[] { '\t' } : new char[] { SeparatorChar });
                        DataStructure = new DataStructure();
                        DataStructure.Name = "Table1";
                        foreach (string s in buff2)
                        {
                            DataStructure.Fields.Add(new Field(s));
                        }

                        ImportResultDataTable = new DataTable();
                        foreach (Field f in DataStructure.Fields)
                        {
                            DataColumn aux = new DataColumn(f.Name);
                            aux.AllowDBNull = f.Nullable;
                            switch (f.FieldType)
                            {
                                case FieldType.Integer:
                                    aux.DataType = typeof(int);
                                    break;
                                case FieldType.FloatingPoint:
                                    aux.DataType = typeof(float);
                                    break;
                                case FieldType.Character:
                                    aux.DataType = typeof(char);
                                    break;
                                case FieldType.String:
                                    aux.DataType = typeof(string);
                                    if (f.Length > 0)
                                        aux.MaxLength = f.Length;
                                    break;
                                case FieldType.Date:
                                    aux.DataType = typeof(DateTime);
                                    break;
                                case FieldType.Bit:
                                    aux.DataType = typeof(bool);
                                    break;
                            }
                            ImportResultDataTable.Columns.Add(aux);
                        }
                    }
                }

                while ((line = Rdr.ReadLine()) != null)
                {
                    RowNumber++;
                    ThCurrentRecord = RowNumber;
                    if (Separator == Delimiter.Lenght)
                    {
                        #region Code to read a file of fixed length
                        RawData = new string[DataStructure.Fields.Count];
                        int Index = 0;
                        position = 0;
                        lenght = line.Length;
                        while (position < lenght && Index < DataStructure.Fields.Count)
                        {
                            try
                            {
                                RawData[Index] = line.Substring(position, DataStructure.Fields[Index].Length);
                                position += DataStructure.Fields[Index].Length;
                            }
                            catch (Exception ex)
                            {
                                AddError(ex.Message, RowNumber);
                            }
                            finally
                            {
                                Index++;
                            }
                        }
                        try
                        {
                            DataRow rw = ImportResultDataTable.NewRow();
                            for (int Index2 = 0; Index2 < Math.Min(RawData.Length, DataStructure.Fields.Count); Index2++)
                            {
                                string data = RawData[Index2].Trim(new char[] { FillerChar });
                                switch (DataStructure.Fields[Index2].FieldType)
                                {
                                    case FieldType.Integer:
                                        if (int.TryParse(data, out auxint))
                                            rw[Index2] = auxint;
                                        else if (!DataStructure.Fields[Index2].Nullable)
                                            rw[Index2] = 0;
                                        break;
                                    case FieldType.FloatingPoint:
                                        if (float.TryParse(data, out auxfloat))
                                            rw[Index2] = auxfloat;
                                        else if (!DataStructure.Fields[Index2].Nullable)
                                            rw[Index2] = 0;
                                        break;
                                    case FieldType.Character:
                                        if (!String.IsNullOrEmpty(data))
                                        {
                                            rw[Index2] = data[0];
                                        }
                                        else if (!DataStructure.Fields[Index2].Nullable)
                                        {
                                            rw[Index2] = '-';
                                        }
                                        break;
                                    case FieldType.Date:
                                        if (DateTime.TryParse(data, out auxdate))
                                            rw[Index2] = auxdate;
                                        else if (!DataStructure.Fields[Index2].Nullable)
                                            rw[Index2] = new DateTime(1900, 1, 1);
                                        break;
                                    case FieldType.Bit:
                                        if (bool.TryParse(data, out auxbool))
                                            rw[Index2] = auxbool;
                                        else if (!DataStructure.Fields[Index2].Nullable)
                                            rw[Index2] = false;
                                        break;
                                    case FieldType.String:
                                    default:
                                        rw[Index2] = data;
                                        break;
                                }
                            }
                            ImportResultDataTable.Rows.Add(rw);
                            //if (RowReaded != null)
                            //    RowReaded(DateTime.Now, RowNumber);
                        }
                        catch (Exception ex)
                        {
                            AddError(ex.Message, RowNumber);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Code to read a file with a especified delimiter
                        if (ImportType == ImportFrom.CSV)
                            RawData = ReadCSVRow(line).ToArray();
                        else
                            RawData = line.Split(Separator == Delimiter.Tab ? new char[] { '\t' } : new char[] { SeparatorChar });

                        try
                        {
                            DataRow rw = ImportResultDataTable.NewRow();
                            for (int Index2 = 0; Index2 < Math.Min(RawData.Length, DataStructure.Fields.Count); Index2++)
                            {
                                string data = RawData[Index2];
                                switch (DataStructure.Fields[Index2].FieldType)
                                {
                                    case FieldType.Integer:
                                        if (int.TryParse(data, out auxint))
                                            rw[Index2] = auxint;
                                        else if (!DataStructure.Fields[Index2].Nullable)
                                            rw[Index2] = 0;
                                        break;
                                    case FieldType.FloatingPoint:
                                        if (float.TryParse(data, out auxfloat))
                                            rw[Index2] = auxfloat;
                                        else if (!DataStructure.Fields[Index2].Nullable)
                                            rw[Index2] = 0;
                                        break;
                                    case FieldType.Character:
                                        if (!String.IsNullOrEmpty(data))
                                        {
                                            rw[Index2] = data[0];
                                        }
                                        else if (!DataStructure.Fields[Index2].Nullable)
                                        {
                                            rw[Index2] = '-';
                                        }
                                        break;
                                    case FieldType.Date:
                                        if (DateTime.TryParse(data, out auxdate))
                                            rw[Index2] = auxdate;
                                        else if (!DataStructure.Fields[Index2].Nullable)
                                            rw[Index2] = new DateTime(1900, 1, 1);
                                        break;
                                    case FieldType.Bit:
                                        if (bool.TryParse(data, out auxbool))
                                            rw[Index2] = auxbool;
                                        else if (!DataStructure.Fields[Index2].Nullable)
                                            rw[Index2] = false;
                                        break;
                                    case FieldType.String:
                                    default:
                                        rw[Index2] = data;
                                        break;
                                }
                            }
                            ImportResultDataTable.Rows.Add(rw);
                        }
                        catch (Exception ex)
                        {
                            AddError(ex.Message, RowNumber);
                        }
                        #endregion
                    }

                    if (ThRecordCount > 100)
                    {

                        if (RowNumber % (ThRecordCount / 100) == 0)
                        {
                            if (RunningAsync)
                            {
                                if (AsyncImporter != null && AsyncImporter.IsBusy)
                                    AsyncImporter.ReportProgress(0, RowNumber);
                            }
                            else
                            {
                                if (OnProgress != null)
                                    OnProgress(DateTime.Now, ThRecordCount, RowNumber, ImportType);
                            }
                        }
                    }
                    else
                    {
                        if (RunningAsync)
                        {
                            if (AsyncImporter != null && AsyncImporter.IsBusy)
                                AsyncImporter.ReportProgress(0, RowNumber);
                        }
                        else
                        {
                            if (OnProgress != null)
                                OnProgress(DateTime.Now, ThRecordCount, RowNumber, ImportType);
                        }

                    }
                }
                Rdr.Close();
            }
        }

        /// <summary>
        /// Parses a single CSV row into a list of field values, handling quoted fields.
        /// </summary>
        /// <param name="line">The CSV line text to parse.</param>
        /// <remarks>
        /// This method handles the parsing of CSV fields, including those containing commas
        /// enclosed in double quotes. It accumulates character data into fields while respecting
        /// the quoting, and then adds each completed field to the result list.
        /// </remarks>
        /// <returns>A list of field values extracted from the CSV line in order. Quoted values are preserved without quotes.</returns>
        private List<string> ReadCSVRow(string line)
        {
            List<string> back = new List<string>();
            string curField = "";
            bool quotedValue = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (!quotedValue && line[i] == ',')
                {
                    back.Add(curField);
                    curField = "";
                }
                else
                {
                    if (line[i] == '"')
                    {
                        quotedValue = !quotedValue;
                    }
                    else
                    {
                        curField += line[i].ToString();
                    }
                }
            }
            back.Add(curField);
            return back;
        }

        #region Auxiliar Code
        /// <summary>
        /// Counts the number of rows present in the configured source file or in the selected Excel sheet.
        /// </summary>
        /// <remarks>
        /// This method opens the file or Excel sheet and counts the number of rows/lines
        /// it contains. It supports both text-based files (TXT/CSV) and Excel files (XLS/XLSX).
        /// </remarks>
        /// <returns>The number of lines/rows found in the source. Returns 0 when file does not exist or when no rows are found.</returns>
        private int RowCount()
        {
            int back = 0;
            if (File.Exists(FileName))
            {
                if (ImportType == ImportFrom.TXT || ImportType == ImportFrom.CSV)
                {
                    using (StreamReader Rdr = new StreamReader(FileName))
                    {
                        while (Rdr.ReadLine() != null)
                            back++;
                        Rdr.Close();
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(SheetName))
                    {
                        SheetName = GetExcelSheetNames(FileName)[0];
                    }

                    using (var stream = File.Open(FileName, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            if (reader.ResultsCount == 1)
                            {
                                back = reader.RowCount;
                            }
                            else
                            {
                                var dsResult = reader.AsDataSet(GetDefaultConfig());
                                back = dsResult.Tables[SheetName].Rows.Count;
                            }
                        }
                    }
                }
            }
            return back;
        }

        /// <summary>
        /// Builds the default ExcelDataSetConfiguration used when reading Excel files.
        /// </summary>
        /// <returns>An <see cref="ExcelDataSetConfiguration"/> configured to use column data types and header rows (UseHeaderRow=true).</returns>
        private ExcelDataSetConfiguration GetDefaultConfig()
        {
            ExcelDataSetConfiguration back = new ExcelDataSetConfiguration();
            back.UseColumnDataType = true;
            back.ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
            {
                // Gets or sets a value indicating whether to use a row from the 
                // data as column names.
                UseHeaderRow = true
            };
            return back;
        }

        /// <summary>
        /// Records an error encountered during import operation.
        /// </summary>
        /// <param name="error">Error description or message.</param>
        /// <param name="location">Row number or location where the error occurred (if available).</param>
        /// <remarks>
        /// Adds a new error entry to the Errors collection, containing the error description
        /// and the location (row number) where the error occurred during the import process.
        /// </remarks>
        /// <returns>None. Adds a new <see cref="ErorrInfo"/> instance to the <see cref="Errors"/> collection.</returns>
        private void AddError(string error, int location)
        {
            Errors.Add(new ErorrInfo(error, location));
        }

        /// <summary>
        /// Clears the current error list or initializes it if null.
        /// </summary>
        /// <remarks>
        /// This method empties the Errors collection, resetting the error state of the importer.
        /// It can be called before starting a new import to ensure that old errors do not persist.
        /// </remarks>
        /// <returns>None. After calling this method <see cref="Errors"/> will be an empty list.</returns>
        private void CleanErrorInfo()
        {
            if (Errors != null)
                Errors.Clear();
            else
                Errors = new List<ErorrInfo>();
        }

        /// <summary>
        /// Retrieves the list of sheet names from the specified Excel workbook.
        /// </summary>
        /// <param name="excelFile">File path to the Excel workbook.</param>
        /// <remarks>
        /// This method opens the Excel file and reads its metadata to find in which sheets data is present.
        /// It populates the result list with the names of the sheets found, in the order they appear in the workbook.
        /// </remarks>
        /// <returns>A list of sheet names found in the workbook, in workbook order. Returns an empty list if none found or file cannot be read.</returns>
        private List<string> GetExcelSheetNames(string excelFile)
        {
            List<string> back = new List<string>();
            using (var stream = File.Open(excelFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int resultsCount = reader.ResultsCount;

                    if (resultsCount > 0)
                    {
                        do
                        {
                            back.Add(reader.Name);
                            resultsCount--;
                        } while (resultsCount > 0 && reader.NextResult());
                    }
                }
            }

            return back;
        }
        #endregion

        #region Methods used by the background worker
        /// <summary>
        /// BackgroundWorker handler invoked when an asynchronous import operation completes.
        /// Raises the <see cref="OnCompletedImportation"/> event if assigned.
        /// </summary>
        /// <param name="sender">The background worker that completed its operation.</param>
        /// <param name="e">Event arguments providing run completion information.</param>
        /// <remarks>
        /// This method is called when the background worker finishes the import process.
        /// It triggers the OnCompletedImportation event, passing the final import statistics and file name.
        /// </remarks>
        /// <returns>None. Triggers the <see cref="OnCompletedImportation"/> event.</returns>
        void AsyncImporter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnCompletedImportation != null)
                OnCompletedImportation(DateTime.Now, ThRecordCount, ImportType, FileName);
        }

        /// <summary>
        /// BackgroundWorker progress changed handler.
        /// Raises the <see cref="OnProgress"/> event to notify listeners of current progress.
        /// </summary>
        /// <param name="sender">The background worker reporting progress.</param>
        /// <param name="e">Progress event arguments containing user state (unused).</param>
        /// <remarks>
        /// Reports the current progress of the import operation to the main thread,
        /// allowing updates to progress indicators in the UI or logs.
        /// </remarks>
        /// <returns>None. Calls <see cref="OnProgress"/> with the current record progress.</returns>
        void AsyncImporter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (OnProgress != null)
                OnProgress(DateTime.Now, ThRecordCount, ThCurrentRecord, ImportType);
        }

        /// <summary>
        /// BackgroundWorker DoWork handler which performs the import operation on a background thread.
        /// The method delegates to the appropriate import routine based on <see cref="ImportType"/>.
        /// </summary>
        /// <param name="sender">The background worker executing the operation.</param>
        /// <param name="e">DoWork event arguments.</param>
        /// <remarks>
        /// This is the main worker method that imports data from the source file.
        /// It initializes the import process and calls the specific methods to read
        /// data from text files or Excel files, depending on the configuration.
        /// </remarks>
        /// <returns>None. Executes the import routine and may raise <see cref="OnStartImportation"/> at start.</returns>
        void AsyncImporter_DoWork(object sender, DoWorkEventArgs e)
        {
            if (OnStartImportation != null)
                OnStartImportation(DateTime.Now, ThRecordCount, 0, ImportType);

            switch (ImportType)
            {
                case ImportFrom.TXT:
                    ImportFromTxtFile();
                    break;
                case ImportFrom.CSV:
                    Separator = Delimiter.Separator;
                    SeparatorChar = ',';
                    ImportFromTxtFile();
                    break;
                case ImportFrom.XLS:
                case ImportFrom.XLSX:
                    ImportFromExcelFile();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

    }

    public class DataStructure
    {
        public string Name { get; set; }
        public List<Field> Fields { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStructure"/> class with optional name and fields.
        /// </summary>
        /// <param name="name">Optional name for the data structure.</param>
        /// <param name="fields">Optional list of fields; if null an empty list will be created.</param>
        /// <remarks>
        /// The constructor can be used to create an empty data structure or to initialize
        /// it with a name and a predefined list of fields.
        /// </remarks>
        public DataStructure(string name = "", List<Field> fields = null)
        {
            Name = name;
            Fields = fields ?? new List<Field>();
        }

        /// <summary>
        /// Creates a <see cref="DataStructure"/> based on an existing <see cref="DataTable"/>'s columns.
        /// </summary>
        /// <param name="dataTable">The DataTable whose columns will be used to build the field list. If null, nothing is done.</param>
        /// <remarks>
        /// This constructor initializes the DataStructure by copying column metadata (name, type, length)
        /// from an existing DataTable. It is used to scaffold the DataStructure based on actual data.
        /// </remarks>
        /// <returns>None. Populates this instance's Name and Fields from the DataTable's columns.</returns>
        public DataStructure(DataTable dataTable)
        {
            if (dataTable != null)
            {
                Name = dataTable.TableName;
                Fields = new List<Field>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    Fields.Add(new Field(col.ColumnName, col.AllowDBNull, AsFieldType(col.DataType), col.MaxLength));
                }
            }
        }

        /// <summary>
        /// Converts a System.Type to the local <see cref="DataImporter.FieldType"/> representation used by this importer.
        /// </summary>
        /// <param name="type">The CLR type to evaluate.</param>
        /// <remarks>
        /// This method maps common .NET types to the FieldType enumeration used by the importer,
        /// allowing the dynamic determination of field types based on the data source schema.
        /// </remarks>
        /// <returns>The corresponding <see cref="DataImporter.FieldType"/> value. Defaults to String when unknown.</returns>
        private FieldType AsFieldType(Type type)
        {
            Type[] intTypes = new Type[] { typeof(Byte), typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte), typeof(UInt16), typeof(UInt32), typeof(UInt64) };
            Type[] numTypes = new Type[] { typeof(Decimal), typeof(Double), typeof(Single) };
            Type[] boolTypes = new Type[] { typeof(Boolean) };
            Type[] dateTypes = new Type[] { typeof(DateTime) };

            if (intTypes.Contains(type))
                return FieldType.Integer;
            if (numTypes.Contains(type))
                return FieldType.FloatingPoint;
            if (boolTypes.Contains(type))
                return FieldType.Bit;
            if (dateTypes.Contains(type))
                return FieldType.Date;

            return FieldType.String;
        }
    }

    public class Field
    {
        public string Name { get; set; }
        public bool Nullable { get; set; }
        public FieldType FieldType { get; set; }
        public int Length { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class representing a column definition.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="nullable">Indicates whether the column can contain nulls.</param>
        /// <param name="type">The semantic field type used by the importer.</param>
        /// <param name="lenght">Fixed length for fixed-width records (0 when not applicable).</param>
        /// <remarks>
        /// Defines the properties that compose a field in the data structure, including its name,
        /// nullability, type, and length (for fixed-width fields). These properties dictate how
        /// data is read from the source files and interpreted by the importer.
        /// </remarks>
        public Field(string name = "NoName", bool nullable = true, FieldType type = FieldType.String, int lenght = 0)
        {
            Nullable = nullable;
            Name = name;
            Length = lenght;
            FieldType = type;
        }
    }

    public class ErorrInfo
    {
        public string Description;
        public int Location;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErorrInfo"/> class to store an import error description and location.
        /// </summary>
        /// <param name="description">Error description or message.</param>
        /// <param name="location">Location (for example row number) where the error occurred. Defaults to -1 when unknown.</param>
        /// <remarks>
        /// This class is used to report and track errors that occur during the import process,
        /// including information about what went wrong and where (in which row) the error was encountered.
        /// </remarks>
        public ErorrInfo(string description = "", int location = -1)
        {
            Description = description;
            Location = location;
        }
        /// <summary>
        /// Returns a string representation combining location and description.
        /// </summary>
        /// <returns>A string in the format "Location - Description".</returns>
        public override string ToString()
        {
            return String.Format("{0} - {1}", Location, Description);
        }
    }
}
