using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZidUtilities.CommonCode.DataComparison;

namespace ZidUtilities.CommonCode.Files
{
    public static class FilesExtensions
    {
        public static void SaveToXlsx(this DataTable dt, string fileName, bool withStyles = true)
        {
            DataExporter dex = new DataExporter();
            dex.ExportExcelStyle = ExcelStyle.Simple;
            dex.ExportType = ExportTo.XLSX;
            dex.ExportWithStyles = withStyles;
            dex.UseAlternateRowStyles = withStyles;
            dex.WriteHeaders = true;
            dex.UseDefaultSheetNames = true;

            dex.ExportToFile(fileName, dt);
        }

        public static void GenerateXlsxTrackFile(this DataComparer comparer, string fileName, bool onlyChanges)
        {
            int workingRow = 1, workingColumn = 1, columncount;
            if (comparer.ComparisonResult == null || comparer.ComparisonResult.Count == 0)
                return;

            RowComparison firstRow = comparer.ComparisonResult[0];
            DataRow firstDataRow = firstRow.IsInTable1 ? firstRow.Row1 : firstRow.Row2;
            if (firstRow.IsInTable1)
                columncount = firstRow.Row1.Table.Columns.Count;
            else
                columncount = firstRow.Row2.Table.Columns.Count;

            XLWorkbook workBook = new XLWorkbook();
            var dataSheet = workBook.AddWorksheet("Comparison Data");

            #region Write headers of the file
            dataSheet.SheetView.FreezeRows(2);

            dataSheet.Cell(workingRow, workingColumn).Value = "File created at: " + DateTime.Now.ToString("MMM/dd/yyyy hh:mm tt");
            dataSheet.Cell(workingRow, workingColumn).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
            dataSheet.Cell(workingRow, workingColumn).Style.Font.Bold = true;
            dataSheet.Cell(workingRow, workingColumn).Style.Font.SetFontSize(14);
            dataSheet.Cell(workingRow, workingColumn).Style.Font.FontColor = XLColor.White;
            dataSheet.Range(dataSheet.Cell(workingRow, workingColumn), dataSheet.Cell(workingRow, columncount + 2)).Merge();
            workingRow++;

            dataSheet.Cell(workingRow, workingColumn).Value = "Comments";
            workingColumn++;
            dataSheet.Cell(workingRow, workingColumn).Value = "BadMatchColumns";
            workingColumn++;
            foreach (DataColumn tc in firstDataRow.Table.Columns)
            {
                dataSheet.Cell(workingRow, workingColumn).Value = tc.ColumnName;
                workingColumn++;
            }

            dataSheet.Cell(workingRow, 1).Style.Font.SetFontSize(12);
            dataSheet.Cell(workingRow, 1).Style.Fill.BackgroundColor = XLColor.White;
            dataSheet.Cell(workingRow, 1).Style.Font.Bold = true;
            dataSheet.Cell(workingRow, 1).Style.Font.FontColor = XLColor.FromHtml("#2E2E2E");
            dataSheet.Cell(workingRow, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            dataSheet.Cell(workingRow, 1).Style.Border.BottomBorderColor = XLColor.Gray;
            dataSheet.Cell(workingRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataSheet.Cell(workingRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            dataSheet.Cell(workingRow, 1).Style.Alignment.WrapText = true;
            dataSheet.Row(workingRow).Height = 35;
            var headerStyle = dataSheet.Cell(workingRow, 1).Style;

            dataSheet.Range(dataSheet.Cell(workingRow, 1), dataSheet.Cell(workingRow, workingColumn - 1)).Style = headerStyle;

            //reset working column 
            workingColumn = 1;
            //Next Row
            workingRow++;

            dataSheet.Columns().AdjustToContents();
            #endregion

            #region styles for the data rows
            //row style
            dataSheet.Cell(workingRow, workingColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#C5D9F1");
            dataSheet.Cell(workingRow, workingColumn).Style.Font.Bold = false;
            dataSheet.Cell(workingRow, workingColumn).Style.Font.SetFontSize(10);
            dataSheet.Cell(workingRow, workingColumn).Style.Font.FontColor = XLColor.FromHtml("#000080");
            dataSheet.Cell(workingRow, workingColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            var rowStyle = dataSheet.Cell(workingRow, workingColumn).Style;

            //alt row style
            dataSheet.Cell(workingRow + 1, workingColumn).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
            dataSheet.Cell(workingRow + 1, workingColumn).Style.Font.Bold = false;
            dataSheet.Cell(workingRow + 1, workingColumn).Style.Font.SetFontSize(10);
            dataSheet.Cell(workingRow + 1, workingColumn).Style.Font.FontColor = XLColor.FromHtml("#003300");
            dataSheet.Cell(workingRow + 1, workingColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            var rowAltStyle = dataSheet.Cell(workingRow + 1, workingColumn).Style;
            #endregion

            #region Write the details of the comparison.
            int index = 0;
            IXLStyle errorStyleGeneral = null, errorStyleDate = null, errorStyleMoney = null, errorStyleInt = null;
            foreach (RowComparison rowc in comparer.ComparisonResult)
            {
                string buff;
                if (rowc.ExactMatch)
                {
                    buff = "Exact Match";
                }
                else if (rowc.OnlyTable1)
                {
                    buff = "Only found in: " + comparer.Label1;
                }
                else if (rowc.OnlyTable2)
                {
                    buff = "Only found in: " + comparer.Label2;
                }
                else
                {
                    buff = "Differences found.";
                }


                if (onlyChanges && rowc.ExactMatch)
                {
                    index++;
                    continue;
                }

                dataSheet.Cell(workingRow, workingColumn).Value = buff;
                workingColumn++;
                dataSheet.Cell(workingRow, workingColumn).Value = String.Join(",", rowc.Differences.Select(x => x.ColumnName));
                workingColumn++;

                #region Write values on cells and set general styles
                for (int i = 0; i < columncount; i++)
                {
                    if (rowc.IsInTable1)
                        dataSheet.Cell(workingRow, workingColumn).Value = rowc.Row1[i].ToString();
                    else
                        dataSheet.Cell(workingRow, workingColumn).Value = "";

                    DataColumn dataCol = firstDataRow.Table.Columns[i];

                    switch (dataCol.DataType.ToString())
                    {
                        case "System.Byte":
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                            dataSheet.Cell(workingRow, 21).Style.NumberFormat.Format = "#,##0";
                            break;
                        case "System.DateTime":
                            dataSheet.Cell(workingRow, 8).Style.DateFormat.SetFormat("MM/dd/yyyy");
                            break;
                        case "System.Single":
                        case "System.Double":
                        case "System.Decimal":
                            dataSheet.Cell(workingRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            break;
                        case "System.Boolean":
                        case "System.Char":
                        case "System.String":
                        default:
                            break;
                    }
                    workingColumn++;
                }

                //set the row style
                dataSheet.Range(dataSheet.Cell(workingRow, 1), dataSheet.Cell(workingRow, workingColumn - 1)).Style = ((workingRow % 2 == 0) ? rowStyle : rowAltStyle);
                #endregion

                #region Change the styles for the cells with differences and write a comment with the details.
                foreach (CellDifference celld in rowc.Differences)
                {
                    DataColumn dataCol = firstDataRow.Table.Columns[celld.ColumnNumber];

                    switch (dataCol.DataType.ToString())
                    {
                        case "System.Byte":
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                            if (errorStyleInt == null)
                            {
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFC7CE");
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.Bold = false;
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.SetFontSize(10);
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.FontColor = XLColor.FromHtml("#9C0006");
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.NumberFormat.Format = "#,##0";
                                errorStyleInt = dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style;
                            }
                            else
                            {
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style = errorStyleInt;
                            }
                            break;
                        case "System.DateTime":
                            if (errorStyleDate == null)
                            {
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFC7CE");
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.Bold = false;
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.SetFontSize(10);
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.FontColor = XLColor.FromHtml("#9C0006");
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.DateFormat.SetFormat("MM/dd/yyyy");
                                errorStyleDate = dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style;
                            }
                            else
                            {
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style = errorStyleDate;
                            }

                            break;
                        case "System.Single":
                        case "System.Double":
                        case "System.Decimal":
                            if (errorStyleMoney == null)
                            {
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFC7CE");
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.Bold = false;
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.SetFontSize(10);
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.FontColor = XLColor.FromHtml("#9C0006");
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.NumberFormat.Format = "#,##0.00";
                                errorStyleMoney = dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style;
                            }
                            else
                            {
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style = errorStyleMoney;
                            }
                            break;
                        case "System.Boolean":
                        case "System.Char":
                        case "System.String":
                        default:
                            if (errorStyleGeneral == null)
                            {
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFC7CE");
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.Bold = false;
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.SetFontSize(10);
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style.Font.FontColor = XLColor.FromHtml("#9C0006");
                                errorStyleGeneral = dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style;
                            }
                            else
                            {
                                dataSheet.Cell(workingRow, celld.ColumnNumber + 3).Style = errorStyleGeneral;
                            }
                            break;
                    }
                    dataSheet.Cell(workingRow, celld.ColumnNumber + 3).GetComment().AddText(String.Format("{0}: {1} vs {2}: {3}", comparer.Label1, celld.Value1, comparer.Label2, celld.Value2));
                }
                #endregion

                workingRow++;
                //reset working column 
                workingColumn = 1;
                //maintain the index of the current row for the result datatable
                index++;
            }
            #endregion

            dataSheet.Range(dataSheet.Cell(2, 1), dataSheet.Cell(2, columncount + 2)).SetAutoFilter();
            workBook.SaveAs(fileName);
        }

    }
}
