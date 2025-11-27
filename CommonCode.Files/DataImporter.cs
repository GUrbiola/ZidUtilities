using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.Files
{
    public partial class DataImporter
    {
        public enum ImportFrom { TXT, CSV, XLS, XLSX }
        public enum Delimiter { Tab, Separator, Lenght };
        public enum FieldType { Integer, FloatingPoint, Character, String, Date, Bit }

        public delegate void ImportEvent(DateTime FiredAt, int Records, int Progress, DataImporter.ImportFrom ExportType);
        public delegate void ImportCompletedEvent(DateTime FiredAt, int Records, DataImporter.ImportFrom ExportType, string PathResult);

        public event ImportEvent OnStartImportation;
        public event ImportCompletedEvent OnCompletedImportation;
        public event ImportEvent OnProgress;
        public List<ErorrInfo> Errors { get; set; }

        private DataStructure _DataStructure;
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

        /* OLD METHOD: Nuget package does everything with less code, this is unneccesary and dont need to install ACE.OLEDB on target PC.
        private void ImportFromExcelFile()
        {
            int RowNumber = 0, auxint;
            float auxfloat;
            DateTime auxdate;
            bool auxbool;

            if (String.IsNullOrEmpty(SheetName))
            {
                SheetName = GetExcelSheetNames(FileName)[0];
            }


            //string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;IMEX=1;HDR=" + (FileHeader ? "YES" : "NO") + ";TypeGuessRows=0;ImportMixedTypes=Text\"", FileName);
            string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;IMEX=1;HDR={1}\"", FileName, (FileHeader ? "YES" : "NO"));
            //Excel 12.0 Xml;HDR=YES;IMEX=1


            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM [{0}$]", SheetName);

                    connection.Open();

                    using (DbDataReader dr = command.ExecuteReader())
                    {
                        if (DataStructure == null || DataStructure.Fields == null || DataStructure.Fields.Count == 0)
                            DataStructure = new DataStructure(dr.GetSchemaTable());

                        while (dr.Read())
                        {
                            RowNumber++;
                            ThCurrentRecord = RowNumber;

                            if (RowNumber > ThRecordCount)
                                break;
                            try
                            {
                                DataRow rw = ImportResultDataTable.NewRow();
                                for (int Index2 = 0; Index2 < Math.Min(dr.FieldCount, DataStructure.Fields.Count); Index2++)
                                {
                                    string data = dr[Index2].ToString();
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


                            if (ThRecordCount / 100 > 0)
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
                        }
                    }
                    connection.Close();
                }
            }
        }
        */

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

        private void AddError(string error, int location)
        {
            Errors.Add(new ErorrInfo(error, location));
        }
        private void CleanErrorInfo()
        {
            if (Errors != null)
                Errors.Clear();
            else
                Errors = new List<ErorrInfo>();
        }
        /// <summary>
        /// This method retrieves the excel sheet names from 
        /// an excel workbook.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <returns>String[]</returns>
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
        void AsyncImporter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnCompletedImportation != null)
                OnCompletedImportation(DateTime.Now, ThRecordCount, ImportType, FileName);
        }
        void AsyncImporter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (OnProgress != null)
                OnProgress(DateTime.Now, ThRecordCount, ThCurrentRecord, ImportType);
        }
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
        public DataStructure(string name = "", List<Field> fields = null)
        {
            Name = name;
            Fields = fields ?? new List<Field>();
        }

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

        private DataImporter.FieldType AsFieldType(Type type)
        {
            Type[] intTypes = new Type[] { typeof(Byte), typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte), typeof(UInt16), typeof(UInt32), typeof(UInt64) };
            Type[] numTypes = new Type[] { typeof(Decimal), typeof(Double), typeof(Single) };
            Type[] boolTypes = new Type[] { typeof(Boolean) };
            Type[] dateTypes = new Type[] { typeof(DateTime) };

            if (intTypes.Contains(type))
                return DataImporter.FieldType.Integer;
            if (numTypes.Contains(type))
                return DataImporter.FieldType.FloatingPoint;
            if (boolTypes.Contains(type))
                return DataImporter.FieldType.Bit;
            if (dateTypes.Contains(type))
                return DataImporter.FieldType.Date;

            return DataImporter.FieldType.String;
        }
    }

    public class Field
    {
        public string Name { get; set; }
        public bool Nullable { get; set; }
        public DataImporter.FieldType FieldType { get; set; }
        public int Length { get; set; }
        public Field(string name = "NoName", bool nullable = true, DataImporter.FieldType type = DataImporter.FieldType.String, int lenght = 0)
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

        public ErorrInfo(string description = "", int location = -1)
        {
            Description = description;
            Location = location;
        }
        public override string ToString()
        {
            return String.Format("{0} - {1}", Location, Description);
        }
    }
}
