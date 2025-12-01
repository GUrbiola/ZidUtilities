using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZidUtilities.CommonCode;

namespace ZidUtilities.CommonCode.Files
{
    /// <summary>
    /// Type of exportations supported
    /// </summary>
    public enum ExportTo
    {
        /// <summary>
        /// Closed xml export
        /// </summary>
        XLSX,
        /// <summary>
        /// Text file, separated by a char
        /// </summary>
        TXT,
        /// <summary>
        /// Text file, separated by commas
        /// </summary>
        CSV,
        /// <summary>
        /// HTML file with filterable table
        /// </summary>
        HTML
    };
    /// <summary>
    /// Predefined workbook style themes.
    /// </summary>
    public enum ExcelStyle
    {
        /// <summary>Navy blue headers with light blue alternating rows - Classic professional look</summary>
        Default,
        /// <summary>Minimal styling with gray bottom border - Clean and simple</summary>
        Simple,
        /// <summary>Deep blue ocean theme - Professional and calming</summary>
        Ocean,
        /// <summary>Forest green theme - Natural and balanced</summary>
        Forest,
        /// <summary>Warm sunset orange theme - Energetic and modern</summary>
        Sunset,
        /// <summary>Black and white monochrome - Minimalist and elegant</summary>
        Monochrome,
        /// <summary>Corporate blue-gray theme - Executive and professional</summary>
        Corporate,
        /// <summary>Fresh mint green theme - Clean and modern</summary>
        Mint,
        /// <summary>Elegant lavender purple theme - Sophisticated and refined</summary>
        Lavender,
        /// <summary>Autumn brown and orange theme - Warm and earthy</summary>
        Autumn,
        /// <summary>Steel gray and silver theme - Modern and sleek</summary>
        Steel,
        /// <summary>Cherry red theme - Bold and energetic</summary>
        Cherry,
        /// <summary>Light sky blue theme - Airy and professional</summary>
        Sky,
        /// <summary>Charcoal dark theme - Strong and professional</summary>
        Charcoal
    };
    /// <summary>
    /// Semantic cell styles for custom highlighting.
    /// </summary>
    public enum ExcelCellStyle { Good, Bad, Neutral, Calculation, Check, Alert, None };
    /// <summary>
    /// Strategy to auto adjust column widths when generating an Excel worksheet.
    /// </summary>
    public enum WidthAdjust { ByHeaders, ByFirst10Rows, ByFirst100Rows, ByAllRows, None }

    public class DataExporter
    {
        /// <summary>
        /// When true, text exports will use fixed column widths. When false, values are separated by <see cref="Separator"/>.
        /// </summary>
        public bool DelimitedByLenght { get; set; }
        /// <summary>
        /// Character used to pad values in fixed-width text exports.
        /// </summary>
        public char CharFiller { get; set; }
        /// <summary>
        /// Fixed widths to use per column when <see cref="DelimitedByLenght"/> is true.
        /// </summary>
        public List<int> Widhts;
        /// <summary>
        /// Delegate for export lifecycle events (start/progress).
        /// </summary>
        /// <param name="FiredAt">Timestamp when the event fired.</param>
        /// <param name="Records">Total number of records to process.</param>
        /// <param name="Progress">Current progress value (record index or percent-based depending on usage).</param>
        /// <param name="ExportType">Export target type.</param>
        public delegate void ExportEvent(DateTime FiredAt, int Records, int Progress, ExportTo ExportType);
        /// <summary>
        /// Delegate for export completion events.
        /// </summary>
        /// <param name="FiredAt">Timestamp when the event fired.</param>
        /// <param name="Records">Total number of records processed.</param>
        /// <param name="ExportType">Export target type.</param>
        /// <param name="StreamResult">Resulting stream when exporting to stream; null when exporting to file.</param>
        /// <param name="PathResult">Resulting file path when exporting to file; empty when exporting to stream.</param>
        public delegate void ExportCompletedEvent(DateTime FiredAt, int Records, ExportTo ExportType, Stream StreamResult, string PathResult);
        /// <summary>
        /// Event raised when an export starts.
        /// </summary>
        public event ExportEvent OnStartExportation;
        /// <summary>
        /// Event raised when an export completes.
        /// </summary>
        public event ExportCompletedEvent OnCompletedExportation;
        /// <summary>
        /// Event raised to report export progress.
        /// </summary>
        public event ExportEvent OnProgress;
        /// <summary>
        /// Selected export target type.
        /// </summary>
        public ExportTo ExportType { get; set; }
        /// <summary>
        /// Column names to skip when exporting.
        /// </summary>
        public List<string> IgnoredColumns { get; set; }
        /// <summary>
        /// Excel style theme to apply to headers and rows.
        /// </summary>
        public ExcelStyle ExportExcelStyle { get; set; }
        /// <summary>
        /// HTML style theme to apply when exporting to HTML.
        /// Uses the same ExcelStyle enum for consistency.
        /// </summary>
        public ExcelStyle ExportHtmlStyle { get; set; }
        /// <summary>
        /// Separator string used for text exports when <see cref="DelimitedByLenght"/> is false.
        /// </summary>
        public string Separator { get; set; }
        /// <summary>
        /// Optional remarks applied to specific cells in Excel exports.
        /// </summary>
        public List<CellRemark> Remarks { get; set; }
        /// <summary>
        /// Column width auto-adjust strategy for Excel exports.
        /// </summary>
        public WidthAdjust AutoCellAdjust;

        /// <summary>
        /// When true, writes column headers in exports (Excel, TXT, CSV).
        /// </summary>
        public bool WriteHeaders { get; set; }
        /// <summary>
        /// When true, applies styles to Excel exports.
        /// </summary>
        public bool ExportWithStyles { get; set; }
        /// <summary>
        /// When true, applies alternate row styles in Excel exports.
        /// </summary>
        public bool UseAlternateRowStyles { get; set; }

        /// <summary>
        /// Excel document property: Author.
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Excel document property: Company.
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// Excel document property: Subject.
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Excel document property: Title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// When true, uses default sheet names like &quot;Data&quot;, &quot;Data 1&quot; instead of <see cref="DataTable.TableName"/>.
        /// </summary>
        public bool UseDefaultSheetNames { get; set; }

        private string ThFilePath;
        private DataSet ThDataSet;
        private int ThRecordCount;
        private int ThCurrentRecord;
        private int ThCurPercentage;
        private bool ThIsStream;
        private Stream ThStream;

        private BackgroundWorker AsyncExporter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataExporter"/> class with default settings.
        /// </summary>
        public DataExporter()
        {
            ExportType = ExportTo.XLSX;
            IgnoredColumns = new List<string>();
            WriteHeaders = true;
            ExportWithStyles = true;
            UseAlternateRowStyles = true;
            ExportExcelStyle = ExcelStyle.Default;
            ExportHtmlStyle = ExcelStyle.Default;
            Separator = "\t";
            Remarks = new List<CellRemark>();
            DelimitedByLenght = false;
            CharFiller = ' ';
            Widhts = new List<int>();
            AutoCellAdjust = WidthAdjust.ByHeaders;
            UseDefaultSheetNames = true;
        }

        /// <summary>
        /// Export the provided <see cref="DataSet"/> to a file path synchronously or asynchronously.
        /// </summary>
        /// <param name="FilePath">Destination file path where the exported file will be saved.</param>
        /// <param name="Data">The dataset containing one or more tables to export.</param>
        /// <param name="GoAsync">If true, the export happens on a background thread and events will be fired; otherwise it runs synchronously.</param>
        public void ExportToFile(string FilePath, DataSet Data, bool GoAsync = false)
        {
            if (GoAsync)
            {
                if (AsyncExporter != null || (AsyncExporter != null && AsyncExporter.IsBusy))
                {
                    AsyncExporter.CancelAsync();
                    AsyncExporter = null;
                }
                AsyncExporter = new BackgroundWorker();
                AsyncExporter.WorkerReportsProgress = true;
                AsyncExporter.WorkerSupportsCancellation = false;
                AsyncExporter.DoWork += new DoWorkEventHandler(AsyncExporter_DoWork);
                AsyncExporter.ProgressChanged += new ProgressChangedEventHandler(AsyncExporter_ProgressChanged);
                AsyncExporter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AsyncExporter_RunWorkerCompleted);

                ThDataSet = Data;
                ThFilePath = FilePath;
                ThCurPercentage = 0;
                ThCurrentRecord = 0;
                ThRecordCount = 0;
                ThIsStream = false;

                foreach (DataTable table in Data.Tables)
                {
                    ThRecordCount += table.Rows.Count;
                }

                if (OnStartExportation != null)
                    OnStartExportation(DateTime.Now, ThRecordCount, 0, ExportType);

                AsyncExporter.RunWorkerAsync();

            }
            else
            {
                ThDataSet = Data;
                ThFilePath = FilePath;
                ThCurPercentage = 0;
                ThCurrentRecord = 0;
                ThRecordCount = 0;
                ThIsStream = false;

                foreach (DataTable table in Data.Tables)
                {
                    ThRecordCount += table.Rows.Count;
                }

                if (OnStartExportation != null)
                    OnStartExportation(DateTime.Now, ThRecordCount, 0, ExportType);

                if (File.Exists(FilePath))
                    File.Delete(FilePath);

                switch (ExportType)
                {
                    case ExportTo.XLSX:
                        CreateXLSX(Data, FilePath);
                        break;
                    case ExportTo.TXT:
                        Stream ResTxt = CreateTXT(Data);
                        ResTxt.Seek(0, SeekOrigin.Begin);
                        using (Stream file = File.OpenWrite(FilePath))
                        {
                            ResTxt.CopyStream(file);
                        }
                        break;
                    case ExportTo.CSV:
                        Stream ResCsv = CreateCSV(Data);
                        ResCsv.Seek(0, SeekOrigin.Begin);
                        using (Stream file = File.OpenWrite(FilePath))
                        {
                            ResCsv.CopyStream(file);
                        }
                        break;
                    case ExportTo.HTML:
                        Stream ResHtml = CreateHTML(Data);
                        ResHtml.Seek(0, SeekOrigin.Begin);
                        using (Stream file = File.OpenWrite(FilePath))
                        {
                            ResHtml.CopyStream(file);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if( OnCompletedExportation != null)
                    OnCompletedExportation(DateTime.Now, ThRecordCount, ExportType, null, FilePath);
            }
        }
        /// <summary>
        /// Export the provided <see cref="DataSet"/> to a stream synchronously or asynchronously.
        /// </summary>
        /// <param name="Data">The dataset containing one or more tables to export.</param>
        /// <param name="GoAsync">If true, the export happens on a background thread and the result will be provided via the OnCompletedExportation event; otherwise this method returns the stream.</param>
        /// <returns>
        /// If <paramref name="GoAsync"/> is false, returns a <see cref="Stream"/> containing the exported data. 
        /// If <paramref name="GoAsync"/> is true, returns null and the stream will be provided through the OnCompletedExportation event.
        /// </returns>
        public Stream ExportToStream(DataSet Data, bool GoAsync = false)
        {
            if (GoAsync)
            {
                if (AsyncExporter != null || (AsyncExporter != null && AsyncExporter.IsBusy))
                {
                    AsyncExporter.CancelAsync();
                    AsyncExporter = null;
                }
                AsyncExporter = new BackgroundWorker();
                AsyncExporter.WorkerReportsProgress = true;
                AsyncExporter.WorkerSupportsCancellation = false;
                AsyncExporter.DoWork += new DoWorkEventHandler(AsyncExporter_DoWork);
                AsyncExporter.ProgressChanged += new ProgressChangedEventHandler(AsyncExporter_ProgressChanged);
                AsyncExporter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AsyncExporter_RunWorkerCompleted);

                ThDataSet = Data;
                ThFilePath = "";
                ThCurPercentage = 0;
                ThCurrentRecord = 0;
                ThRecordCount = 0;
                ThIsStream = true;

                foreach (DataTable table in Data.Tables)
                {
                    ThRecordCount += table.Rows.Count;
                }

                if (OnStartExportation != null)
                    OnStartExportation(DateTime.Now, ThRecordCount, 0, ExportType);

                AsyncExporter.RunWorkerAsync();
                return null;
            }
            else
            {
                ThDataSet = Data;
                ThFilePath = "";
                ThCurPercentage = 0;
                ThCurrentRecord = 0;
                ThRecordCount = 0;
                ThIsStream = false;

                foreach (DataTable table in Data.Tables)
                {
                    ThRecordCount += table.Rows.Count;
                }

                if (OnStartExportation != null)
                    OnStartExportation(DateTime.Now, ThRecordCount, 0, ExportType);

                Stream back = null;
                switch (ExportType)
                {
                    case ExportTo.XLSX:
                        back = CreateXLSX(Data);
                        back.Seek(0, SeekOrigin.Begin);
                        break;
                    case ExportTo.TXT:
                        back = CreateTXT(Data);
                        back.Seek(0, SeekOrigin.Begin);
                        break;
                    case ExportTo.CSV:
                        back = CreateCSV(Data);
                        back.Seek(0, SeekOrigin.Begin);
                        break;
                    case ExportTo.HTML:
                        back = CreateHTML(Data);
                        back.Seek(0, SeekOrigin.Begin);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (OnCompletedExportation != null)
                    OnCompletedExportation(DateTime.Now, ThRecordCount, ExportType, ThStream, ThFilePath);

                return back;
            }
        }

        #region Methods used by the background worker
        /// <summary>
        /// Handler invoked when the asynchronous export worker completes.
        /// This fires the <see cref="OnCompletedExportation"/> event supplying either the produced stream (if streaming) or the file path.
        /// </summary>
        /// <param name="sender">The background worker that completed.</param>
        /// <param name="e">Event arguments for the completion.</param>
        void AsyncExporter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnCompletedExportation != null)
            {
                if (ThIsStream)
                {
                    OnCompletedExportation(DateTime.Now, ThRecordCount, ExportType, ThStream, ThFilePath);
                }
                else
                {
                    OnCompletedExportation(DateTime.Now, ThRecordCount, ExportType, null, ThFilePath);
                }
            }
        }
        /// <summary>
        /// Handler invoked periodically by the background worker to report progress.
        /// Fires the <see cref="OnProgress"/> event with updated progress information.
        /// </summary>
        /// <param name="sender">The background worker that reported progress.</param>
        /// <param name="e">Progress changed event arguments.</param>
        void AsyncExporter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (OnProgress != null)
                OnProgress(DateTime.Now, ThRecordCount, ThCurrentRecord + 1, ExportType);
        }
        /// <summary>
        /// Entry point for the background worker to perform the export operation.
        /// The export type selected by the <see cref="ExportType"/> property determines which creation method is invoked.
        /// </summary>
        /// <param name="sender">The background worker executing the work.</param>
        /// <param name="e">DoWork event arguments.</param>
        void AsyncExporter_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (ExportType)
            {
                case ExportTo.XLSX:
                    if (!ThIsStream)
                    {
                        CreateXLSX(ThDataSet, ThFilePath);
                    }
                    else
                    {
                        ThStream = CreateXLSX(ThDataSet);
                        ThStream.Seek(0, SeekOrigin.Begin);
                    }
                    break;
                case ExportTo.TXT:

                    if (!ThIsStream)
                    {
                        using (Stream file = File.OpenWrite(ThFilePath))
                        {
                            Stream ResTxt = CreateTXT(ThDataSet);
                            ResTxt.Seek(0, SeekOrigin.Begin);
                            ResTxt.CopyStream(file);
                        }
                    }
                    else
                    {
                        ThStream = CreateTXT(ThDataSet);
                        ThStream.Seek(0, SeekOrigin.Begin);
                    }
                    break;
                case ExportTo.CSV:
                    if (!ThIsStream)
                    {
                        using (Stream file = File.OpenWrite(ThFilePath))
                        {
                            Stream ResCsv = CreateCSV(ThDataSet);
                            ResCsv.Seek(0, SeekOrigin.Begin);
                            ResCsv.CopyStream(file);
                        }
                    }
                    else
                    {
                        ThStream = CreateCSV(ThDataSet);
                        ThStream.Seek(0, SeekOrigin.Begin);
                    }
                    break;
                case ExportTo.HTML:
                    if (!ThIsStream)
                    {
                        using (Stream file = File.OpenWrite(ThFilePath))
                        {
                            Stream ResHtml = CreateHTML(ThDataSet);
                            ResHtml.Seek(0, SeekOrigin.Begin);
                            ResHtml.CopyStream(file);
                        }
                    }
                    else
                    {
                        ThStream = CreateHTML(ThDataSet);
                        ThStream.Seek(0, SeekOrigin.Begin);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region Overloads for the export method
        /// <summary>
        /// Export a single <see cref="DataTable"/> to a file path.
        /// </summary>
        /// <param name="FilePath">Destination file path.</param>
        /// <param name="Data">Data table to export.</param>
        /// <param name="GoAsync">If true, runs asynchronously.</param>
        public void ExportToFile(string FilePath, DataTable Data, bool GoAsync = false)
        {
            DataSet helper = new DataSet();
            helper.Tables.Add(Data.Copy());
            ExportToFile(FilePath, helper, GoAsync);
        }
        /// <summary>
        /// Export a generic list to a file by converting it to a DataTable.
        /// </summary>
        /// <typeparam name="T">Type of elements in the list.</typeparam>
        /// <param name="FilePath">Destination file path.</param>
        /// <param name="Data">List of items to export.</param>
        /// <param name="GoAsync">If true, runs asynchronously.</param>
        public void ExportToFile<T>(string FilePath, IList<T> Data, bool GoAsync = false)
        {
            ExportToFile(FilePath, Data.ConvertToDataTable<T>(), GoAsync);
        }
        /// <summary>
        /// Export a dictionary's values to a file by converting values to a DataTable.
        /// </summary>
        /// <typeparam name="K">Type of dictionary keys (ignored).</typeparam>
        /// <typeparam name="V">Type of dictionary values (exported).</typeparam>
        /// <param name="FilePath">Destination file path.</param>
        /// <param name="Data">Dictionary to export.</param>
        /// <param name="GoAsync">If true, runs asynchronously.</param>
        public void ExportToFile<K, V>(string FilePath, IDictionary<K, V> Data, bool GoAsync = false)
        {
            List<V> listBuffer = new List<V>();
            foreach (KeyValuePair<K, V> pair in Data)
            {
                listBuffer.Add(pair.Value);
            }
            ExportToFile(FilePath, listBuffer.ConvertToDataTable<V>(), GoAsync);
        }
        /// <summary>
        /// Export a generic collection to a file by converting it to a DataTable.
        /// </summary>
        /// <typeparam name="T">Type of collection items.</typeparam>
        /// <param name="FilePath">Destination file path.</param>
        /// <param name="Data">Collection to export.</param>
        /// <param name="GoAsync">If true, runs asynchronously.</param>
        public void ExportToFile<T>(string FilePath, ICollection<T> Data, bool GoAsync = false)
        {
            List<T> listBuffer = new List<T>();
            foreach (T val in Data)
            {
                listBuffer.Add(val);
            }
            ExportToFile(FilePath, listBuffer.ConvertToDataTable<T>(), GoAsync);
        }
        /// <summary>
        /// Export a DataTable to a stream.
        /// </summary>
        /// <param name="Data">Data table to export.</param>
        /// <param name="GoAsync">If true, runs asynchronously.</param>
        /// <returns>Stream with exported content or null if running asynchronously.</returns>
        public Stream ExportToStream(DataTable Data, bool GoAsync = false)
        {
            DataSet helper = new DataSet();
            helper.Tables.Add(Data.Copy());
            return ExportToStream(helper, GoAsync);
        }
        /// <summary>
        /// Export a generic list to a stream by converting it to a DataTable.
        /// </summary>
        /// <typeparam name="T">Type of elements in the list.</typeparam>
        /// <param name="Data">List of items to export.</param>
        /// <param name="GoAsync">If true, runs asynchronously.</param>
        /// <returns>Stream with exported content or null if running asynchronously.</returns>
        public Stream ExportToStream<T>(IList<T> Data, bool GoAsync = false)
        {
            return ExportToStream(Data.ConvertToDataTable<T>(), GoAsync);
        }
        /// <summary>
        /// Export a dictionary's values to a stream by converting values to a DataTable.
        /// </summary>
        /// <typeparam name="K">Type of dictionary keys (ignored).</typeparam>
        /// <typeparam name="V">Type of dictionary values (exported).</typeparam>
        /// <param name="Data">Dictionary to export.</param>
        /// <param name="GoAsync">If true, runs asynchronously.</param>
        /// <returns>Stream with exported content or null if running asynchronously.</returns>
        public Stream ExportToStream<K, V>(IDictionary<K, V> Data, bool GoAsync = false)
        {
            List<V> listBuffer = new List<V>();
            foreach (KeyValuePair<K, V> pair in Data)
            {
                listBuffer.Add(pair.Value);
            }
            return ExportToStream(listBuffer.ConvertToDataTable<V>(), GoAsync);
        }
        /// <summary>
        /// Export a generic collection to a stream by converting it to a DataTable.
        /// </summary>
        /// <typeparam name="T">Type of collection items.</typeparam>
        /// <param name="Data">Collection to export.</param>
        /// <param name="GoAsync">If true, runs asynchronously.</param>
        /// <returns>Stream with exported content or null if running asynchronously.</returns>
        public Stream ExportToStream<T>(ICollection<T> Data, bool GoAsync = false)
        {
            List<T> listBuffer = new List<T>();
            foreach (T val in Data)
            {
                listBuffer.Add(val);
            }
            return ExportToStream(listBuffer.ConvertToDataTable<T>(), GoAsync);
        }
        #endregion

        #region Code to generate the XLSX file(Excel 2007-2010)
        /// <summary>
        /// Create an XLSX workbook from the provided <see cref="DataSet"/>.
        /// </summary>
        /// <param name="dataSet">Dataset containing worksheets to add.</param>
        /// <param name="filePath">
        /// Optional file path to save the workbook to disk. If provided the method saves to disk and returns null.
        /// If empty, the workbook is returned as a <see cref="Stream"/> (MemoryStream).</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the workbook if <paramref name="filePath"/> is empty; otherwise null.
        /// </returns>
        private Stream CreateXLSX(DataSet dataSet, string filePath = "")
        {
            XLWorkbook book = new XLWorkbook();
            if (!String.IsNullOrEmpty(Author))
                book.Properties.Author = Author;
            if (!String.IsNullOrEmpty(Company))
                book.Properties.Company = Company;
            if (!String.IsNullOrEmpty(Subject))
                book.Properties.Subject = Subject;
            if (!String.IsNullOrEmpty(Title))
                book.Properties.Title = Title;

            foreach (DataTable table in dataSet.Tables)
            {
                CreateWorkSheetXLSX(book, table);
            }

            if (!String.IsNullOrEmpty(filePath))
            {
                book.SaveAs(filePath);
                return null;
            }
            else
            {
                MemoryStream back = new MemoryStream();
                book.SaveAs(back);
                return back;
            }
        }
        /// <summary>
        /// Add a worksheet to the given <see cref="XLWorkbook"/> representing the provided <see cref="DataTable"/>.
        /// This method writes headers (optionally), data, applies styles and adjusts column widths based on settings.
        /// </summary>
        /// <param name="closedXMLBook">Workbook to which the worksheet will be added.</param>
        /// <param name="data">DataTable that becomes the worksheet's content.</param>
        private void CreateWorkSheetXLSX(XLWorkbook closedXMLBook, DataTable data)
        {
            int sheetCount = closedXMLBook.Worksheets.Count;
            int curRow = 1, curCol = 1, autoAdjustCells;
            string sheetName = UseDefaultSheetNames ? "Data" + (sheetCount > 0 ? " " + sheetCount.ToString() : "") : data.TableName;
            var newSheet = closedXMLBook.AddWorksheet(sheetName);

            switch (AutoCellAdjust)
            {
                case WidthAdjust.ByHeaders:
                    autoAdjustCells = 0;
                    break;
                case WidthAdjust.ByFirst10Rows:
                    autoAdjustCells = 10;
                    break;
                case WidthAdjust.ByFirst100Rows:
                    autoAdjustCells = 100;
                    break;
                case WidthAdjust.ByAllRows:
                    autoAdjustCells = 10000;
                    break;
                default:
                case WidthAdjust.None:
                    autoAdjustCells = -1;
                    break;
            }


            #region Style for the headers
            newSheet.Row(1).Height = 35;
            var headerStyle = newSheet.Cell(1, 1).Style;
            #endregion

            #region Define if the writing of the headers is needed
            if (WriteHeaders)
            {
                foreach (DataColumn col in data.Columns)
                {
                    string colName = String.IsNullOrEmpty(col.Caption) ? col.ColumnName : col.Caption;
                    if (IgnoredColumns != null && IgnoredColumns.Count > 0 && IgnoredColumns.IsStringOnList(colName, false))
                        continue;

                    newSheet.Cell(curRow, curCol).Value = colName;
                    if (ExportWithStyles)
                    {
                        newSheet.Cell(curRow, curCol).Style.HeaderStyle(ExportExcelStyle);
                    }
                    else
                    {
                        newSheet.Cell(curRow, curCol).Style.NoStyle();
                    }
                    curCol++;
                }
                curRow++;
                curCol = 1;
            }
            if (autoAdjustCells == 0)
            {
                newSheet.Columns().AdjustToContents();
            }

            #endregion

            #region Finally write the details of the data
            int tabLength = data.Rows.Count;
            int rowLength = data.Columns.Count;

            for (int rowNum = 0; rowNum < tabLength; rowNum++)
            {
                for (int colNum = 0; colNum < rowLength; colNum++)
                {
                    DataColumn col = data.Columns[colNum];
                    string colName = String.IsNullOrEmpty(col.Caption) ? col.ColumnName : col.Caption;

                    if (IgnoredColumns != null && IgnoredColumns.Count > 0 && IgnoredColumns.IsStringOnList(colName, false))
                        continue;

                    if (ExportWithStyles)
                    {
                        if (UseAlternateRowStyles)
                        {
                            if (rowNum % 2 == 0)
                            {
                                newSheet.Cell(curRow, curCol).Style.AlternateStyle(ExportExcelStyle);
                            }
                            else
                            {
                                newSheet.Cell(curRow, curCol).Style.NormalStyle(ExportExcelStyle);
                            }
                        }
                        else
                        {
                            newSheet.Cell(curRow, curCol).Style.NormalStyle(ExportExcelStyle);
                        }
                    }
                    else
                    {
                        newSheet.Cell(curRow, curCol).Style.NoStyle();
                    }



                    if (data.Rows[rowNum][colNum] == DBNull.Value || String.IsNullOrEmpty(data.Rows[rowNum][colNum].ToString()))
                    {
                        newSheet.Cell(curRow, curCol).Value = "";
                    }
                    else if (data.Rows[rowNum][colNum].ToString().StartsWith("="))
                    {
                        newSheet.Cell(curRow, curCol).FormulaA1 = data.Rows[rowNum][colNum].ToString();
                    }
                    else
                    {
                        try
                        {
                            newSheet.Cell(curRow, curCol).Value = data.Rows[rowNum][colNum].ToString();
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            newSheet.Cell(curRow, curCol).Value = data.Rows[rowNum][colNum].ToString().SafeSubstring(0, 32750);
                        }
                    }

                    if (Remarks != null && Remarks.Count > 0 && Remarks.Any(x => x.Row == curRow && x.Col == curCol))
                    {
                        CellRemark remark = Remarks.FirstOrDefault(x => x.Row == curRow && x.Col == curCol);
                        newSheet.Cell(curRow, curCol).Style.SetCustomStyle(remark.Style);
                        if (String.IsNullOrEmpty(remark.Comment))
                            newSheet.Cell(curRow, curCol).GetComment().AddText(remark.Comment);
                    }

                    curCol++;


                }

                if (autoAdjustCells > 0 && ((rowNum + 1) >= autoAdjustCells))
                {
                    autoAdjustCells = -1;
                    newSheet.Columns().AdjustToContents();
                }

                ThCurrentRecord++;
                if (AsyncExporter != null && AsyncExporter.IsBusy)
                {
                    if (ThRecordCount > 100)
                    {
                        if (ThCurrentRecord % (ThRecordCount / 100) == 0)
                        {
                            AsyncExporter.ReportProgress(((ThCurrentRecord) * 100) / ThRecordCount);
                            ThCurPercentage = ((ThCurrentRecord) * 100) / ThRecordCount;
                        }
                    }
                }
                else
                {
                    if (ThRecordCount > 100)
                    {
                        if (ThCurrentRecord % (ThRecordCount / 100) == 0)
                        {
                            if(OnProgress != null)
                                OnProgress(DateTime.Now, ThRecordCount, ThCurrentRecord, ExportType);
                        }
                    }

                }

                curCol = 1;
                curRow++;

            }
            if (autoAdjustCells > 0 && (autoAdjustCells >= tabLength))
            {
                newSheet.Columns().AdjustToContents();
            }

            #endregion

        }
        #endregion

        #region Code to generate the text file
        /// <summary>
        /// Create a text file stream (MemoryStream) for the given dataset.
        /// Supports fixed-width format when <see cref="DelimitedByLenght"/> is true, otherwise uses <see cref="Separator"/>.
        /// </summary>
        /// <param name="dataSet">Dataset to convert to text format.</param>
        /// <returns>A <see cref="Stream"/> (MemoryStream) containing the produced text.</returns>
        private Stream CreateTXT(DataSet dataSet)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            if (!DelimitedByLenght)
            {
                #region Code used for text files with separators
                foreach (DataTable table in dataSet.Tables)
                {
                    string curLine;
                    int colCount = table.Columns.Count;

                    if (WriteHeaders)
                    {
                        curLine = "";
                        for (int i = 0; i < colCount; i++)
                        {
                            string colName = String.IsNullOrEmpty(table.Columns[i].Caption) ? table.Columns[i].ColumnName : table.Columns[i].Caption;
                            if (IgnoredColumns != null && IgnoredColumns.Count > 0 && IgnoredColumns.IsStringOnList(colName, false))
                                continue;
                            curLine += String.Format("{0}{1}", colName, Separator);
                        }
                        tw.WriteLine(curLine.Substring(0, curLine.Length - Separator.Length));
                    }

                    for (int curRow = 0; curRow < table.Rows.Count; curRow++)
                    {
                        curLine = "";
                        for (int curCol = 0; curCol < colCount; curCol++)
                        {
                            string colName = table.Columns[curCol].ColumnName;
                            if (IgnoredColumns != null && IgnoredColumns.Count > 0 && IgnoredColumns.IsStringOnList(colName, false))
                                continue;
                            if (table.Rows[curRow][curCol] != null && table.Rows[curRow][curCol] != DBNull.Value)
                            {
                                curLine += String.Format("{0}{1}", table.Rows[curRow][curCol].ToString(), Separator);
                            }
                            else
                            {
                                curLine += String.Format("{0}", Separator);
                            }
                        }
                        tw.WriteLine(curLine.Substring(0, curLine.Length - Separator.Length));
                    }

                    ThCurrentRecord++;
                    if (AsyncExporter != null && AsyncExporter.IsBusy)
                    {
                        if (ThRecordCount > 100)
                        {
                            if (ThCurrentRecord % (ThRecordCount / 100) == 0)
                            {
                                AsyncExporter.ReportProgress(((ThCurrentRecord) * 100) / ThRecordCount);
                                ThCurPercentage = ((ThCurrentRecord) * 100) / ThRecordCount;
                            }
                        }
                    }
                    else
                    {
                        if (ThRecordCount > 100)
                        {
                            if (ThCurrentRecord % (ThRecordCount / 100) == 0)
                            {
                                if (OnProgress != null)
                                    OnProgress(DateTime.Now, ThRecordCount, ThCurrentRecord, ExportType);
                            }
                        }
                    }
                }
                tw.Flush();
                #endregion
            }
            else
            {
                #region Code used for text files with delimited length
                foreach (DataTable table in dataSet.Tables)
                {
                    string curLine;
                    int colCount = table.Columns.Count;

                    if (WriteHeaders)
                    {
                        curLine = "";
                        for (int i = 0; i < colCount; i++)
                        {
                            string colName = table.Columns[i].ColumnName;
                            if (IgnoredColumns != null && IgnoredColumns.Count > 0 && IgnoredColumns.IsStringOnList(colName, false))
                                continue;
                            curLine += colName.PadRight(Widhts[i], CharFiller);
                        }
                        tw.WriteLine(curLine);
                    }
                    for (int curRow = 0; curRow < table.Rows.Count; curRow++)
                    {
                        curLine = "";
                        for (int curCol = 0; curCol < colCount; curCol++)
                        {
                            string colName = table.Columns[curCol].ColumnName;
                            if (IgnoredColumns != null && IgnoredColumns.Count > 0 && IgnoredColumns.IsStringOnList(colName, false))
                                continue;
                            curLine += table.Rows[curRow][curCol].ToString().PadRight(Widhts[curCol], CharFiller);
                        }
                        tw.WriteLine(curLine);
                    }

                    ThCurrentRecord++;
                    if (AsyncExporter != null && AsyncExporter.IsBusy)
                    {
                        if (ThRecordCount > 100)
                        {
                            if (ThCurrentRecord % (ThRecordCount / 100) == 0)
                            {
                                AsyncExporter.ReportProgress(((ThCurrentRecord) * 100) / ThRecordCount);
                                ThCurPercentage = ((ThCurrentRecord) * 100) / ThRecordCount;
                            }
                        }
                    }
                    else
                    {
                        if (ThRecordCount > 100)
                        {
                            if (ThCurrentRecord % (ThRecordCount / 100) == 0)
                            {
                                if (OnProgress != null)
                                    OnProgress(DateTime.Now, ThRecordCount, ThCurrentRecord, ExportType);
                            }
                        }
                    }


                }
                tw.Flush();
                #endregion
            }

            return memoryStream;
        }
        #endregion

        #region Code to generate the csv file
        /// <summary>
        /// Create a CSV formatted stream (MemoryStream) from the given dataset.
        /// Fields are sanitized via EnsureCsvField extension.
        /// </summary>
        /// <param name="dataSet">Dataset to convert to CSV format.</param>
        /// <returns>A <see cref="Stream"/> (MemoryStream) containing the produced CSV.</returns>
        private Stream CreateCSV(DataSet dataSet)
        {
            string csvSeparator = ",";
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            foreach (DataTable table in dataSet.Tables)
            {
                string curLine;
                int colCount = table.Columns.Count;

                if (WriteHeaders)
                {
                    curLine = "";
                    for (int i = 0; i < colCount; i++)
                    {
                        string colName = table.Columns[i].ColumnName;
                        if (IgnoredColumns != null && IgnoredColumns.Count > 0 && IgnoredColumns.IsStringOnList(colName, false))
                            continue;
                        curLine += String.Format("{0}{1}", colName.EnsureCsvField(), csvSeparator);
                    }
                    tw.WriteLine(curLine.Substring(0, curLine.Length - Separator.Length));
                }

                for (int curRow = 0; curRow < table.Rows.Count; curRow++)
                {
                    curLine = "";
                    for (int curCol = 0; curCol < colCount; curCol++)
                    {
                        string colName = table.Columns[curCol].ColumnName;
                        if (IgnoredColumns != null && IgnoredColumns.Count > 0 && IgnoredColumns.IsStringOnList(colName, false))
                            continue;
                        if (table.Rows[curRow][curCol] != null && table.Rows[curRow][curCol] != DBNull.Value)
                        {
                            curLine += String.Format("{0}{1}", table.Rows[curRow][curCol].ToString().EnsureCsvField(), csvSeparator);
                        }
                        else
                        {
                            curLine += String.Format("{0}", csvSeparator);
                        }
                    }
                    tw.WriteLine(curLine.Substring(0, curLine.Length - Separator.Length));
                }

                ThCurrentRecord++;
                if (AsyncExporter != null && AsyncExporter.IsBusy)
                {
                    if (ThCurrentRecord % (ThRecordCount / 100) == 0)
                    {
                        AsyncExporter.ReportProgress(((ThCurrentRecord) * 100) / ThRecordCount);
                        ThCurPercentage = ((ThCurrentRecord) * 100) / ThRecordCount;
                    }
                }
                else
                {
                    if (ThRecordCount > 100)
                    {
                        if (ThCurrentRecord % (ThRecordCount / 100) == 0)
                        {
                            if (OnProgress != null)
                                OnProgress(DateTime.Now, ThRecordCount, ThCurrentRecord, ExportType);
                        }
                    }
                }
            }
            tw.Flush();
            return memoryStream;
        }
        #endregion

        #region Code to generate the HTML file
        /// <summary>
        /// Create an HTML file stream (MemoryStream) with filterable table from the given dataset.
        /// The generated HTML includes JavaScript for client-side filtering and displays total/filtered row counts.
        /// </summary>
        /// <param name="dataSet">Dataset to convert to HTML format.</param>
        /// <returns>A <see cref="Stream"/> (MemoryStream) containing the produced HTML.</returns>
        private Stream CreateHTML(DataSet dataSet)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream, Encoding.UTF8);

            // Start HTML document
            tw.WriteLine("<!DOCTYPE html>");
            tw.WriteLine("<html lang=\"en\">");
            tw.WriteLine("<head>");
            tw.WriteLine("    <meta charset=\"UTF-8\">");
            tw.WriteLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            tw.WriteLine("    <title>ZidGrid Html Data Export</title>");

            // Add styles
            WriteHtmlStyles(tw);

            tw.WriteLine("</head>");
            tw.WriteLine("<body>");

            // Header
            tw.WriteLine("    <div class=\"header\">");
            tw.WriteLine("        <h1>ZidGrid Html Data Export</h1>");
            tw.WriteLine($"        <div class=\"export-info\">Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>");
            tw.WriteLine("    </div>");

            // Process each table in the dataset
            int tableIndex = 0;
            foreach (DataTable table in dataSet.Tables)
            {
                string tableName = UseDefaultSheetNames ? "Data" + (tableIndex > 0 ? " " + tableIndex.ToString() : "") : table.TableName;

                tw.WriteLine("    <div class=\"container\">");
                if (!string.IsNullOrEmpty(tableName))
                {
                    tw.WriteLine($"        <h2>{System.Security.SecurityElement.Escape(tableName)}</h2>");
                }

                // Filter controls
                tw.WriteLine("        <div class=\"filter-container\">");
                tw.WriteLine($"            <div style=\"display: flex; gap: 10px; flex: 1;\">");
                tw.WriteLine($"                <input type=\"text\" id=\"filter{tableIndex}\" class=\"filter-input\" placeholder=\"Enter search text...\">");
                tw.WriteLine($"                <button class=\"filter-button\" onclick=\"filterTable({tableIndex})\">Filter</button>");
                tw.WriteLine($"                <button class=\"filter-button\" onclick=\"clearFilter({tableIndex})\">Clear</button>");
                tw.WriteLine($"            </div>");
                tw.WriteLine($"            <div class=\"row-count\" id=\"rowCount{tableIndex}\"></div>");
                tw.WriteLine("        </div>");

                // Table
                tw.WriteLine($"        <table id=\"dataTable{tableIndex}\" class=\"data-table\">");

                // Headers
                if (WriteHeaders)
                {
                    tw.WriteLine("            <thead>");
                    tw.WriteLine("                <tr>");
                    foreach (DataColumn col in table.Columns)
                    {
                        string colName = String.IsNullOrEmpty(col.Caption) ? col.ColumnName : col.Caption;
                        if (IgnoredColumns != null && IgnoredColumns.Count > 0 && IgnoredColumns.IsStringOnList(colName, false))
                            continue;

                        tw.WriteLine($"                    <th>{System.Security.SecurityElement.Escape(colName)}</th>");
                    }
                    tw.WriteLine("                </tr>");
                    tw.WriteLine("            </thead>");
                }

                // Body
                tw.WriteLine("            <tbody>");
                int rowCount = table.Rows.Count;
                for (int rowNum = 0; rowNum < rowCount; rowNum++)
                {
                    tw.WriteLine("                <tr>");
                    for (int colNum = 0; colNum < table.Columns.Count; colNum++)
                    {
                        string colName = table.Columns[colNum].ColumnName;
                        if (IgnoredColumns != null && IgnoredColumns.Count > 0 && IgnoredColumns.IsStringOnList(colName, false))
                            continue;

                        string cellValue = "";
                        if (table.Rows[rowNum][colNum] != null && table.Rows[rowNum][colNum] != DBNull.Value)
                        {
                            cellValue = System.Security.SecurityElement.Escape(table.Rows[rowNum][colNum].ToString());
                        }

                        tw.WriteLine($"                    <td>{cellValue}</td>");
                    }
                    tw.WriteLine("                </tr>");

                    ThCurrentRecord++;
                    if (AsyncExporter != null && AsyncExporter.IsBusy)
                    {
                        if (ThRecordCount > 100)
                        {
                            if (ThCurrentRecord % (ThRecordCount / 100) == 0)
                            {
                                AsyncExporter.ReportProgress(((ThCurrentRecord) * 100) / ThRecordCount);
                                ThCurPercentage = ((ThCurrentRecord) * 100) / ThRecordCount;
                            }
                        }
                    }
                    else
                    {
                        if (ThRecordCount > 100)
                        {
                            if (ThCurrentRecord % (ThRecordCount / 100) == 0)
                            {
                                if (OnProgress != null)
                                    OnProgress(DateTime.Now, ThRecordCount, ThCurrentRecord, ExportType);
                            }
                        }
                    }
                }
                tw.WriteLine("            </tbody>");
                tw.WriteLine("        </table>");
                tw.WriteLine("    </div>");

                tableIndex++;
            }

            // Add JavaScript for filtering
            WriteHtmlScript(tw, tableIndex);

            tw.WriteLine("</body>");
            tw.WriteLine("</html>");
            tw.Flush();

            return memoryStream;
        }

        /// <summary>
        /// Writes the CSS styles for the HTML export based on the selected theme.
        /// </summary>
        /// <param name="tw">TextWriter to write the styles to.</param>
        private void WriteHtmlStyles(TextWriter tw)
        {
            // Get theme colors
            var colors = GetHtmlThemeColors(ExportHtmlStyle);

            tw.WriteLine("    <style>");
            tw.WriteLine("        * {");
            tw.WriteLine("            margin: 0;");
            tw.WriteLine("            padding: 0;");
            tw.WriteLine("            box-sizing: border-box;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        body {");
            tw.WriteLine("            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;");
            tw.WriteLine("            background-color: #f5f5f5;");
            tw.WriteLine("            padding: 20px;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .header {");
            tw.WriteLine("            background: linear-gradient(135deg, " + colors.HeaderBg + ", " + colors.HeaderBgSecondary + ");");
            tw.WriteLine("            color: " + colors.HeaderFg + ";");
            tw.WriteLine("            padding: 30px;");
            tw.WriteLine("            border-radius: 8px;");
            tw.WriteLine("            margin-bottom: 30px;");
            tw.WriteLine("            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .header h1 {");
            tw.WriteLine("            font-size: 32px;");
            tw.WriteLine("            margin-bottom: 10px;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .export-info {");
            tw.WriteLine("            font-size: 14px;");
            tw.WriteLine("            opacity: 0.9;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .container {");
            tw.WriteLine("            background: white;");
            tw.WriteLine("            border-radius: 8px;");
            tw.WriteLine("            padding: 30px;");
            tw.WriteLine("            margin-bottom: 30px;");
            tw.WriteLine("            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .container h2 {");
            tw.WriteLine("            color: " + colors.HeaderBg + ";");
            tw.WriteLine("            margin-bottom: 20px;");
            tw.WriteLine("            font-size: 24px;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .filter-container {");
            tw.WriteLine("            margin-bottom: 20px;");
            tw.WriteLine("            display: flex;");
            tw.WriteLine("            justify-content: space-between;");
            tw.WriteLine("            align-items: center;");
            tw.WriteLine("            gap: 15px;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .filter-input {");
            tw.WriteLine("            flex: 1;");
            tw.WriteLine("            padding: 12px 16px;");
            tw.WriteLine("            border: 2px solid " + colors.BorderColor + ";");
            tw.WriteLine("            border-radius: 6px;");
            tw.WriteLine("            font-size: 14px;");
            tw.WriteLine("            transition: border-color 0.3s;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .filter-input:focus {");
            tw.WriteLine("            outline: none;");
            tw.WriteLine("            border-color: " + colors.HeaderBg + ";");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .filter-button {");
            tw.WriteLine("            padding: 12px 24px;");
            tw.WriteLine("            background-color: " + colors.HeaderBg + ";");
            tw.WriteLine("            color: " + colors.HeaderFg + ";");
            tw.WriteLine("            border: none;");
            tw.WriteLine("            border-radius: 6px;");
            tw.WriteLine("            font-size: 14px;");
            tw.WriteLine("            font-weight: 600;");
            tw.WriteLine("            cursor: pointer;");
            tw.WriteLine("            transition: background-color 0.3s, transform 0.1s;");
            tw.WriteLine("            white-space: nowrap;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .filter-button:hover {");
            tw.WriteLine("            background-color: " + colors.HeaderBgSecondary + ";");
            tw.WriteLine("            transform: translateY(-1px);");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .filter-button:active {");
            tw.WriteLine("            transform: translateY(0);");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .row-count {");
            tw.WriteLine("            font-size: 14px;");
            tw.WriteLine("            color: #666;");
            tw.WriteLine("            font-weight: 500;");
            tw.WriteLine("            white-space: nowrap;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .data-table {");
            tw.WriteLine("            width: 100%;");
            tw.WriteLine("            border-collapse: collapse;");
            tw.WriteLine("            font-size: 14px;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .data-table thead tr {");
            tw.WriteLine("            background-color: " + colors.HeaderBg + ";");
            tw.WriteLine("            color: " + colors.HeaderFg + ";");
            tw.WriteLine("            text-align: left;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .data-table th {");
            tw.WriteLine("            padding: 14px 12px;");
            tw.WriteLine("            font-weight: 600;");
            tw.WriteLine("            border-bottom: 2px solid " + colors.BorderColor + ";");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        .data-table td {");
            tw.WriteLine("            padding: 12px;");
            tw.WriteLine("            border-bottom: 1px solid " + colors.BorderColor + ";");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            if (UseAlternateRowStyles)
            {
                tw.WriteLine("        .data-table tbody tr:nth-child(even) {");
                tw.WriteLine("            background-color: " + colors.AlternateRowBg + ";");
                tw.WriteLine("        }");
                tw.WriteLine("        ");
            }
            tw.WriteLine("        .data-table tbody tr:hover {");
            tw.WriteLine("            background-color: " + colors.HoverRowBg + ";");
            tw.WriteLine("            transition: background-color 0.2s;");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        @media (max-width: 768px) {");
            tw.WriteLine("            .header h1 {");
            tw.WriteLine("                font-size: 24px;");
            tw.WriteLine("            }");
            tw.WriteLine("            ");
            tw.WriteLine("            .container {");
            tw.WriteLine("                padding: 15px;");
            tw.WriteLine("            }");
            tw.WriteLine("            ");
            tw.WriteLine("            .filter-container {");
            tw.WriteLine("                flex-direction: column;");
            tw.WriteLine("                align-items: stretch;");
            tw.WriteLine("            }");
            tw.WriteLine("            ");
            tw.WriteLine("            .data-table {");
            tw.WriteLine("                font-size: 12px;");
            tw.WriteLine("            }");
            tw.WriteLine("            ");
            tw.WriteLine("            .data-table th,");
            tw.WriteLine("            .data-table td {");
            tw.WriteLine("                padding: 8px 6px;");
            tw.WriteLine("            }");
            tw.WriteLine("        }");
            tw.WriteLine("    </style>");
        }

        /// <summary>
        /// Writes the JavaScript code for table filtering functionality.
        /// </summary>
        /// <param name="tw">TextWriter to write the script to.</param>
        /// <param name="tableCount">Number of tables in the document.</param>
        private void WriteHtmlScript(TextWriter tw, int tableCount)
        {
            tw.WriteLine("    <script>");
            tw.WriteLine("        // Cache for row text content to avoid repeated DOM access");
            tw.WriteLine("        var tableCache = {};");
            tw.WriteLine("        ");
            tw.WriteLine("        function filterTable(tableIndex) {");
            tw.WriteLine("            const input = document.getElementById('filter' + tableIndex);");
            tw.WriteLine("            const filter = input.value.toLowerCase().trim();");
            tw.WriteLine("            ");
            tw.WriteLine("            if (!filter) {");
            tw.WriteLine("                clearFilter(tableIndex);");
            tw.WriteLine("                return;");
            tw.WriteLine("            }");
            tw.WriteLine("            ");
            tw.WriteLine("            const table = document.getElementById('dataTable' + tableIndex);");
            tw.WriteLine("            const tbody = table.getElementsByTagName('tbody')[0];");
            tw.WriteLine("            const tr = tbody.getElementsByTagName('tr');");
            tw.WriteLine("            const totalCount = tr.length;");
            tw.WriteLine("            ");
            tw.WriteLine("            // Build cache if not exists");
            tw.WriteLine("            if (!tableCache[tableIndex]) {");
            tw.WriteLine("                tableCache[tableIndex] = [];");
            tw.WriteLine("                for (let i = 0; i < totalCount; i++) {");
            tw.WriteLine("                    const cells = tr[i].getElementsByTagName('td');");
            tw.WriteLine("                    let rowText = '';");
            tw.WriteLine("                    for (let j = 0; j < cells.length; j++) {");
            tw.WriteLine("                        rowText += (cells[j].textContent || cells[j].innerText).toLowerCase() + ' ';");
            tw.WriteLine("                    }");
            tw.WriteLine("                    tableCache[tableIndex][i] = rowText;");
            tw.WriteLine("                }");
            tw.WriteLine("            }");
            tw.WriteLine("            ");
            tw.WriteLine("            // Perform filtering using cached text");
            tw.WriteLine("            let visibleCount = 0;");
            tw.WriteLine("            for (let i = 0; i < totalCount; i++) {");
            tw.WriteLine("                if (tableCache[tableIndex][i].indexOf(filter) > -1) {");
            tw.WriteLine("                    tr[i].style.display = '';");
            tw.WriteLine("                    visibleCount++;");
            tw.WriteLine("                } else {");
            tw.WriteLine("                    tr[i].style.display = 'none';");
            tw.WriteLine("                }");
            tw.WriteLine("            }");
            tw.WriteLine("            ");
            tw.WriteLine("            updateRowCount(tableIndex, visibleCount, totalCount);");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        function clearFilter(tableIndex) {");
            tw.WriteLine("            const input = document.getElementById('filter' + tableIndex);");
            tw.WriteLine("            input.value = '';");
            tw.WriteLine("            ");
            tw.WriteLine("            const table = document.getElementById('dataTable' + tableIndex);");
            tw.WriteLine("            const tbody = table.getElementsByTagName('tbody')[0];");
            tw.WriteLine("            const tr = tbody.getElementsByTagName('tr');");
            tw.WriteLine("            const totalCount = tr.length;");
            tw.WriteLine("            ");
            tw.WriteLine("            // Show all rows");
            tw.WriteLine("            for (let i = 0; i < totalCount; i++) {");
            tw.WriteLine("                tr[i].style.display = '';");
            tw.WriteLine("            }");
            tw.WriteLine("            ");
            tw.WriteLine("            updateRowCount(tableIndex, totalCount, totalCount);");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        function updateRowCount(tableIndex, visible, total) {");
            tw.WriteLine("            const rowCountDiv = document.getElementById('rowCount' + tableIndex);");
            tw.WriteLine("            if (visible === total) {");
            tw.WriteLine("                rowCountDiv.textContent = 'Total Rows: ' + total;");
            tw.WriteLine("            } else {");
            tw.WriteLine("                rowCountDiv.textContent = 'Showing: ' + visible + ' of ' + total + ' rows';");
            tw.WriteLine("            }");
            tw.WriteLine("        }");
            tw.WriteLine("        ");
            tw.WriteLine("        // Initialize row counts on page load");
            tw.WriteLine("        window.addEventListener('DOMContentLoaded', function() {");
            for (int i = 0; i < tableCount; i++)
            {
                tw.WriteLine($"            const table{i} = document.getElementById('dataTable{i}');");
                tw.WriteLine($"            if (table{i}) {{");
                tw.WriteLine($"                const rowCount{i} = table{i}.getElementsByTagName('tbody')[0].getElementsByTagName('tr').length;");
                tw.WriteLine($"                updateRowCount({i}, rowCount{i}, rowCount{i});");
                tw.WriteLine($"            }}");
            }
            tw.WriteLine("        });");
            tw.WriteLine("    </script>");
        }

        /// <summary>
        /// Gets the color scheme for HTML export based on the selected theme.
        /// </summary>
        /// <param name="style">The theme style to use.</param>
        /// <returns>An object containing color values for the theme.</returns>
        private dynamic GetHtmlThemeColors(ExcelStyle style)
        {
            switch (style)
            {
                case ExcelStyle.Default:
                    return new
                    {
                        HeaderBg = "#000080",
                        HeaderBgSecondary = "#003366",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#E0F2FF",
                        HoverRowBg = "#CCE6FF",
                        BorderColor = "#CCCCCC"
                    };
                case ExcelStyle.Simple:
                    return new
                    {
                        HeaderBg = "#FFFFFF",
                        HeaderBgSecondary = "#F5F5F5",
                        HeaderFg = "#000000",
                        AlternateRowBg = "#F9F9F9",
                        HoverRowBg = "#F0F0F0",
                        BorderColor = "#DDDDDD"
                    };
                case ExcelStyle.Ocean:
                    return new
                    {
                        HeaderBg = "#003366",
                        HeaderBgSecondary = "#0066CC",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#CCE5FF",
                        HoverRowBg = "#B3D9FF",
                        BorderColor = "#0066CC"
                    };
                case ExcelStyle.Forest:
                    return new
                    {
                        HeaderBg = "#22572C",
                        HeaderBgSecondary = "#4C9900",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#DCE1C8",
                        HoverRowBg = "#C8D4AC",
                        BorderColor = "#4C9900"
                    };
                case ExcelStyle.Sunset:
                    return new
                    {
                        HeaderBg = "#E65C00",
                        HeaderBgSecondary = "#FF8000",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#FFE0B2",
                        HoverRowBg = "#FFCC99",
                        BorderColor = "#FF8000"
                    };
                case ExcelStyle.Monochrome:
                    return new
                    {
                        HeaderBg = "#333333",
                        HeaderBgSecondary = "#666666",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#F5F5F5",
                        HoverRowBg = "#EEEEEE",
                        BorderColor = "#999999"
                    };
                case ExcelStyle.Corporate:
                    return new
                    {
                        HeaderBg = "#4472C4",
                        HeaderBgSecondary = "#2F5496",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#D9E1F2",
                        HoverRowBg = "#C5D0E6",
                        BorderColor = "#2F5496"
                    };
                case ExcelStyle.Mint:
                    return new
                    {
                        HeaderBg = "#00B08A",
                        HeaderBgSecondary = "#00CC99",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#C6EFCE",
                        HoverRowBg = "#ADE7BC",
                        BorderColor = "#00CC99"
                    };
                case ExcelStyle.Lavender:
                    return new
                    {
                        HeaderBg = "#7030A0",
                        HeaderBgSecondary = "#8E44AD",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#EAD9F4",
                        HoverRowBg = "#DEC5ED",
                        BorderColor = "#8E44AD"
                    };
                case ExcelStyle.Autumn:
                    return new
                    {
                        HeaderBg = "#8C5225",
                        HeaderBgSecondary = "#BF5700",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#F4E0B0",
                        HoverRowBg = "#EBCE8F",
                        BorderColor = "#BF5700"
                    };
                case ExcelStyle.Steel:
                    return new
                    {
                        HeaderBg = "#607D8B",
                        HeaderBgSecondary = "#455A64",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#ECEFF1",
                        HoverRowBg = "#CFD8DC",
                        BorderColor = "#455A64"
                    };
                case ExcelStyle.Cherry:
                    return new
                    {
                        HeaderBg = "#C00000",
                        HeaderBgSecondary = "#880015",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#FFCDD2",
                        HoverRowBg = "#FFABBA",
                        BorderColor = "#880015"
                    };
                case ExcelStyle.Sky:
                    return new
                    {
                        HeaderBg = "#039BE5",
                        HeaderBgSecondary = "#0277BD",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#E1F5FE",
                        HoverRowBg = "#B3E5FC",
                        BorderColor = "#0277BD"
                    };
                case ExcelStyle.Charcoal:
                    return new
                    {
                        HeaderBg = "#263238",
                        HeaderBgSecondary = "#37474F",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#ECEFF1",
                        HoverRowBg = "#CFD8DC",
                        BorderColor = "#000000"
                    };
                default:
                    return new
                    {
                        HeaderBg = "#000080",
                        HeaderBgSecondary = "#003366",
                        HeaderFg = "#FFFFFF",
                        AlternateRowBg = "#E0F2FF",
                        HoverRowBg = "#CCE6FF",
                        BorderColor = "#CCCCCC"
                    };
            }
        }
        #endregion

    }
    public static class ClosedHelper
    {
        /// <summary>
        /// Apply header styling to an <see cref="IXLStyle"/> based on the chosen <see cref="ExcelStyle"/>.
        /// </summary>
        /// <param name="style">Style instance to modify.</param>
        /// <param name="ExportStyle">Predefined export style to apply.</param>
        public static void HeaderStyle(this IXLStyle style, ExcelStyle ExportStyle)
        {
            style.Font.SetFontSize(12);
            style.Font.Bold = true;
            style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            style.Alignment.WrapText = true;
            switch (ExportStyle)
            {
                case ExcelStyle.Default:
                    style.Fill.BackgroundColor = XLColor.Navy;
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorderColor = XLColor.Black;
                    style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    style.Border.TopBorderColor = XLColor.Black;
                    style.Border.TopBorder = XLBorderStyleValues.Thick;
                    style.Border.LeftBorderColor = XLColor.Black;
                    style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    style.Border.RightBorderColor = XLColor.Black;
                    style.Border.RightBorder = XLBorderStyleValues.Thick;
                    break;
                case ExcelStyle.Simple:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.Black;
                    style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    style.Border.BottomBorderColor = XLColor.Gray;
                    break;
                case ExcelStyle.Ocean:
                    style.Fill.BackgroundColor = XLColor.FromArgb(0, 51, 102); // Deep ocean blue
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.FromArgb(0, 102, 204);
                    break;
                case ExcelStyle.Forest:
                    style.Fill.BackgroundColor = XLColor.FromArgb(34, 87, 44); // Forest green
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.FromArgb(76, 153, 0);
                    break;
                case ExcelStyle.Sunset:
                    style.Fill.BackgroundColor = XLColor.FromArgb(230, 92, 0); // Sunset orange
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.FromArgb(255, 128, 0);
                    break;
                case ExcelStyle.Monochrome:
                    style.Fill.BackgroundColor = XLColor.FromArgb(51, 51, 51); // Dark gray
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    style.Border.BottomBorderColor = XLColor.Black;
                    break;
                case ExcelStyle.Corporate:
                    style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196); // Corporate blue
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.FromArgb(47, 84, 150);
                    break;
                case ExcelStyle.Mint:
                    style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 138); // Mint green
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.FromArgb(0, 204, 153);
                    break;
                case ExcelStyle.Lavender:
                    style.Fill.BackgroundColor = XLColor.FromArgb(112, 48, 160); // Lavender purple
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.FromArgb(142, 68, 173);
                    break;
                case ExcelStyle.Autumn:
                    style.Fill.BackgroundColor = XLColor.FromArgb(140, 82, 37); // Autumn brown
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.FromArgb(191, 87, 0);
                    break;
                case ExcelStyle.Steel:
                    style.Fill.BackgroundColor = XLColor.FromArgb(96, 125, 139); // Steel blue-gray
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.FromArgb(69, 90, 100);
                    break;
                case ExcelStyle.Cherry:
                    style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0); // Cherry red
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.FromArgb(136, 0, 21);
                    break;
                case ExcelStyle.Sky:
                    style.Fill.BackgroundColor = XLColor.FromArgb(3, 155, 229); // Sky blue
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.FromArgb(2, 119, 189);
                    break;
                case ExcelStyle.Charcoal:
                    style.Fill.BackgroundColor = XLColor.FromArgb(38, 50, 56); // Charcoal
                    style.Font.FontColor = XLColor.White;
                    style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    style.Border.BottomBorderColor = XLColor.Black;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("ExportStyle");
            }
        }

        /// <summary>
        /// Apply a normal cell style to an <see cref="IXLStyle"/> based on the chosen <see cref="ExcelStyle"/>.
        /// </summary>
        /// <param name="style">Style instance to modify.</param>
        /// <param name="ExportStyle">Predefined export style to apply.</param>
        public static void NormalStyle(this IXLStyle style, ExcelStyle ExportStyle)
        {
            //normal style
            style.Font.Bold = false;
            style.Font.SetFontSize(10);
            style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            switch (ExportStyle)
            {
                case ExcelStyle.Default:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.Navy;
                    style.Border.BottomBorderColor = XLColor.Black;
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.TopBorderColor = XLColor.Black;
                    style.Border.TopBorder = XLBorderStyleValues.Thin;
                    style.Border.LeftBorderColor = XLColor.Black;
                    style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    style.Border.RightBorderColor = XLColor.Black;
                    style.Border.RightBorder = XLBorderStyleValues.Thin;
                    break;
                case ExcelStyle.Simple:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.Black;
                    break;
                case ExcelStyle.Ocean:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(0, 51, 102);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(0, 102, 204);
                    break;
                case ExcelStyle.Forest:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(34, 87, 44);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(76, 153, 0);
                    break;
                case ExcelStyle.Sunset:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(230, 92, 0);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(255, 128, 0);
                    break;
                case ExcelStyle.Monochrome:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(51, 51, 51);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.Gray;
                    break;
                case ExcelStyle.Corporate:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(68, 114, 196);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(47, 84, 150);
                    break;
                case ExcelStyle.Mint:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(0, 176, 138);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(0, 204, 153);
                    break;
                case ExcelStyle.Lavender:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(112, 48, 160);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(142, 68, 173);
                    break;
                case ExcelStyle.Autumn:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(140, 82, 37);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(191, 87, 0);
                    break;
                case ExcelStyle.Steel:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(96, 125, 139);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(69, 90, 100);
                    break;
                case ExcelStyle.Cherry:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(192, 0, 0);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(136, 0, 21);
                    break;
                case ExcelStyle.Sky:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(3, 155, 229);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(2, 119, 189);
                    break;
                case ExcelStyle.Charcoal:
                    style.Fill.BackgroundColor = XLColor.White;
                    style.Font.FontColor = XLColor.FromArgb(38, 50, 56);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.Black;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("ExportStyle");
            }
        }

        /// <summary>
        /// Apply an alternate-row style to an <see cref="IXLStyle"/> based on the chosen <see cref="ExcelStyle"/>.
        /// </summary>
        /// <param name="style">Style instance to modify.</param>
        /// <param name="ExportStyle">Predefined export style to apply.</param>
        public static void AlternateStyle(this IXLStyle style, ExcelStyle ExportStyle)
        {
            //alt row style
            style.Font.Bold = false;
            style.Font.SetFontSize(10);
            style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


            switch (ExportStyle)
            {
                case ExcelStyle.Default:
                    style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                    style.Font.FontColor = XLColor.Navy;
                    style.Border.BottomBorderColor = XLColor.Black;
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.TopBorderColor = XLColor.Black;
                    style.Border.TopBorder = XLBorderStyleValues.Thin;
                    style.Border.LeftBorderColor = XLColor.Black;
                    style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    style.Border.RightBorderColor = XLColor.Black;
                    style.Border.RightBorder = XLBorderStyleValues.Thin;
                    break;
                case ExcelStyle.Simple:
                    style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                    style.Font.FontColor = XLColor.Black;
                    break;
                case ExcelStyle.Ocean:
                    style.Fill.BackgroundColor = XLColor.FromArgb(204, 229, 255); // Light ocean blue
                    style.Font.FontColor = XLColor.FromArgb(0, 51, 102);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(0, 102, 204);
                    break;
                case ExcelStyle.Forest:
                    style.Fill.BackgroundColor = XLColor.FromArgb(220, 237, 200); // Light green
                    style.Font.FontColor = XLColor.FromArgb(34, 87, 44);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(76, 153, 0);
                    break;
                case ExcelStyle.Sunset:
                    style.Fill.BackgroundColor = XLColor.FromArgb(255, 224, 178); // Light peach
                    style.Font.FontColor = XLColor.FromArgb(230, 92, 0);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(255, 128, 0);
                    break;
                case ExcelStyle.Monochrome:
                    style.Fill.BackgroundColor = XLColor.FromArgb(245, 245, 245); // Light gray
                    style.Font.FontColor = XLColor.FromArgb(51, 51, 51);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.Gray;
                    break;
                case ExcelStyle.Corporate:
                    style.Fill.BackgroundColor = XLColor.FromArgb(217, 225, 242); // Light blue-gray
                    style.Font.FontColor = XLColor.FromArgb(68, 114, 196);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(47, 84, 150);
                    break;
                case ExcelStyle.Mint:
                    style.Fill.BackgroundColor = XLColor.FromArgb(198, 239, 206); // Light mint
                    style.Font.FontColor = XLColor.FromArgb(0, 176, 138);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(0, 204, 153);
                    break;
                case ExcelStyle.Lavender:
                    style.Fill.BackgroundColor = XLColor.FromArgb(234, 221, 244); // Light lavender
                    style.Font.FontColor = XLColor.FromArgb(112, 48, 160);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(142, 68, 173);
                    break;
                case ExcelStyle.Autumn:
                    style.Fill.BackgroundColor = XLColor.FromArgb(244, 224, 176); // Light tan
                    style.Font.FontColor = XLColor.FromArgb(140, 82, 37);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(191, 87, 0);
                    break;
                case ExcelStyle.Steel:
                    style.Fill.BackgroundColor = XLColor.FromArgb(236, 239, 241); // Light steel
                    style.Font.FontColor = XLColor.FromArgb(96, 125, 139);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(69, 90, 100);
                    break;
                case ExcelStyle.Cherry:
                    style.Fill.BackgroundColor = XLColor.FromArgb(255, 205, 210); // Light pink
                    style.Font.FontColor = XLColor.FromArgb(192, 0, 0);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(136, 0, 21);
                    break;
                case ExcelStyle.Sky:
                    style.Fill.BackgroundColor = XLColor.FromArgb(225, 245, 254); // Very light blue
                    style.Font.FontColor = XLColor.FromArgb(3, 155, 229);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.FromArgb(2, 119, 189);
                    break;
                case ExcelStyle.Charcoal:
                    style.Fill.BackgroundColor = XLColor.FromArgb(236, 239, 241); // Light gray
                    style.Font.FontColor = XLColor.FromArgb(38, 50, 56);
                    style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    style.Border.BottomBorderColor = XLColor.Black;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("ExportStyle");
            }
        }

        /// <summary>
        /// Apply a minimal/no style to an <see cref="IXLStyle"/>.
        /// </summary>
        /// <param name="style">Style instance to modify.</param>
        public static void NoStyle(this IXLStyle style)
        {
            style.Fill.BackgroundColor = XLColor.White;
            style.Font.Bold = false;
            style.Font.SetFontSize(10);
            style.Font.FontColor = XLColor.Black;
            style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        }

        /// <summary>
        /// Apply a predefined custom cell style to an <see cref="IXLStyle"/> based on <see cref="ExcelCellStyle"/>.
        /// </summary>
        /// <param name="style">Style instance to modify.</param>
        /// <param name="cellStyle">The semantic cell style to apply.</param>
        public static void SetCustomStyle(this IXLStyle style, ExcelCellStyle cellStyle)
        {
            switch (cellStyle)
            {
                case ExcelCellStyle.Good:
                    style.Fill.BackgroundColor = XLColor.PaleGreen;
                    style.Font.FontColor = XLColor.DarkGreen;
                    break;
                case ExcelCellStyle.Bad:
                    style.Fill.BackgroundColor = XLColor.LightCoral;
                    style.Font.FontColor = XLColor.Firebrick;
                    break;
                case ExcelCellStyle.Neutral:
                    style.Fill.BackgroundColor = XLColor.Yellow;
                    style.Font.FontColor = XLColor.DarkOrange;
                    break;
                case ExcelCellStyle.Calculation:
                    style.Fill.BackgroundColor = XLColor.Silver;
                    style.Font.FontColor = XLColor.DarkOrange;
                    break;
                case ExcelCellStyle.Check:
                    style.Fill.BackgroundColor = XLColor.DimGray;
                    style.Font.FontColor = XLColor.Orange;
                    style.Font.Bold = true;
                    style.Border.BottomBorderColor = XLColor.Black;
                    style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    style.Border.TopBorderColor = XLColor.Black;
                    style.Border.TopBorder = XLBorderStyleValues.Thick;
                    style.Border.LeftBorderColor = XLColor.Black;
                    style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    style.Border.RightBorderColor = XLColor.Black;
                    style.Border.RightBorder = XLBorderStyleValues.Thick;
                    break;
                case ExcelCellStyle.Alert:
                    style.Fill.BackgroundColor = XLColor.Red;
                    style.Font.FontColor = XLColor.White;
                    style.Font.Bold = true;
                    style.Border.BottomBorderColor = XLColor.Black;
                    style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    style.Border.TopBorderColor = XLColor.Black;
                    style.Border.TopBorder = XLBorderStyleValues.Thick;
                    style.Border.LeftBorderColor = XLColor.Black;
                    style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    style.Border.RightBorderColor = XLColor.Black;
                    style.Border.RightBorder = XLBorderStyleValues.Thick;
                    break;
                case ExcelCellStyle.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("cellStyle");
            }
        }

    }
    public class CellRemark
    {
        /// <summary>
        /// 1-based row index of the remark target cell.
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// 1-based column index of the remark target cell.
        /// </summary>
        public int Col { get; set; }
        /// <summary>
        /// Text comment to attach to the cell.
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Visual style to apply to the cell.
        /// </summary>
        public ExcelCellStyle Style { get; set; }

        /// <summary>
        /// Initialize a new instance of <see cref="CellRemark"/> with default style set to <see cref="ExcelCellStyle.Bad"/>.
        /// </summary>
        public CellRemark()
        {
            Style = ExcelCellStyle.Bad;
        }
    }
}
