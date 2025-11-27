using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZidUtilities.CommonCode.DataComparison
{
    /// <summary>
    /// Represents comparison configuration for a single column (name, type, case-sensitivity and tolerance).
    /// </summary>
    public class ColumnComparisonDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnComparisonDetail"/> class with default settings.
        /// Default values: <see cref="CaseSensitive"/> = false, <see cref="ColumnType"/> = "string", <see cref="Tolerance"/> = 0.
        /// </summary>
        public ColumnComparisonDetail()
        {
            CaseSensitive = false;
            ColumnName = "";
            ColumnType = "string";
            Tolerance = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnComparisonDetail"/> class with the specified column name.
        /// Other properties are set to defaults: <see cref="CaseSensitive"/> = false, <see cref="ColumnType"/> = "string", <see cref="Tolerance"/> = 0.
        /// </summary>
        /// <param name="columnName">The name of the column to which this comparison detail applies.</param>
        public ColumnComparisonDetail(string columnName)
        {
            CaseSensitive = false;
            ColumnName = columnName;
            ColumnType = "string";
            Tolerance = 0;
        }

        /// <summary>
        /// Gets or sets the column name this comparison detail applies to.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the column data type used to determine comparison logic.
        /// Expected values (case-insensitive): "int", "float", "double", "string", "date", "datetime".
        /// </summary>
        public string ColumnType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether string comparisons are case-sensitive.
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets the numeric tolerance used for numeric and date/time comparisons.
        /// For:
        /// - INT: interpreted as number of integer units difference allowed,
        /// - FLOAT/DOUBLE: interpreted as numeric difference allowed,
        /// - DATE: interpreted as allowed difference in days,
        /// - DATETIME: interpreted as allowed difference in seconds.
        /// </summary>
        public double Tolerance { get; set; }

        /// <summary>
        /// Compares two objects for equality according to the provided <see cref="ColumnComparisonDetail"/>.
        /// If <paramref name="compDetail"/> is null, default comparison settings are used (CaseSensitive=false, ColumnType="string", Tolerance=0).
        /// Comparison behavior per <see cref="ColumnType"/> (case-insensitive):
        /// - "INT": null or <see cref="DBNull.Value"/> are treated as 0; integer difference allowed up to <see cref="Tolerance"/>.
        /// - "FLOAT"/"DOUBLE": null or <see cref="DBNull.Value"/> are treated as 0; numeric difference allowed up to <see cref="Tolerance"/>.
        /// - "STRING": uses <see cref="CaseSensitive"/> to choose between case-sensitive or case-insensitive culture comparisons.
        /// - "DATE": attempts to parse values as <see cref="DateTime"/>; if both parse, compares difference in days against <see cref="Tolerance"/>;
        ///           otherwise falls back to string comparison using the current culture.
        /// - "DATETIME": attempts to parse values as <see cref="DateTime"/>; if both parse, compares absolute difference in seconds against <see cref="Tolerance"/>;
        ///               otherwise falls back to string comparison using the current culture.
        /// - default: falls back to string comparison using the current culture.
        /// </summary>
        /// <param name="obj1">The first object/value to compare. Can be null or <see cref="DBNull.Value"/>.</param>
        /// <param name="obj2">The second object/value to compare. Can be null or <see cref="DBNull.Value"/>.</param>
        /// <param name="compDetail">Optional comparison detail that controls how the values are compared. If null, defaults are used.</param>
        /// <returns>True if the two objects are considered equal under the specified comparison rules; otherwise false.</returns>
        public static bool IsEqual(object obj1, object obj2, ColumnComparisonDetail compDetail = null)
        {
            bool back = false;
            
            compDetail = compDetail ??  new ColumnComparisonDetail() { CaseSensitive = false, ColumnName = "", ColumnType = "string", Tolerance = 0 };

            switch (compDetail.ColumnType.ToUpper())
            {
                case "INT":
                    if (obj1 == null || obj1 == DBNull.Value)
                        obj1 = 0;
                    if (obj2 == null || obj2 == DBNull.Value)
                        obj2 = 0;

                    int diff2 = Convert.ToInt32(obj1) - Convert.ToInt32(obj2);
                    back = Math.Abs(diff2) <= compDetail.Tolerance;
                    break;
                case "FLOAT":
                case "DOUBLE":
                    if (obj1 == null || obj1 == DBNull.Value)
                        obj1 = 0;
                    if (obj2 == null || obj2 == DBNull.Value)
                        obj2 = 0;

                    double diff = Convert.ToDouble(obj1) - Convert.ToDouble(obj2);
                    back = Math.Abs(diff) <= compDetail.Tolerance;
                    break;
                case "STRING":
                    back = obj1.ToString().Equals(obj2.ToString(), compDetail.CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
                    break;
                case "DATE":
                    bool isDate1, isDate2;
                    DateTime date1, date2;
                    if (obj1 == null || obj1 == DBNull.Value)
                        obj1 = "";
                    if (obj2 == null || obj2 == DBNull.Value)
                        obj2 = "";

                    if (obj1 is DateTime)
                    {
                        isDate1 = true;
                        date1 = (DateTime)obj1;
                    }
                    else
                    {
                        isDate1 = DateTime.TryParse(obj1.ToString(), out date1);
                    }

                    if (obj2 is DateTime)
                    {
                        isDate2 = true;
                        date2 = (DateTime)obj2;
                    }
                    else
                    {
                        isDate2 = DateTime.TryParse(obj2.ToString(), out date2);
                    }

                    if (isDate1 && isDate2)
                    {
                        back = date1.Subtract(date2).TotalDays <= compDetail.Tolerance;
                    }
                    else
                    {
                        back = obj1.ToString().Equals(obj2.ToString(), StringComparison.CurrentCulture);
                    }
                    break;
                case "DATETIME":
                    bool isDateTime1, isDateTime2;
                    DateTime dateTime1, dateTime2;
                    if (obj1 == null || obj1 == DBNull.Value)
                        obj1 = "";
                    if (obj2 == null || obj2 == DBNull.Value)
                        obj2 = "";


                    if (obj1 is DateTime)
                    {
                        isDateTime1 = true;
                        dateTime1 = (DateTime)obj1;
                    }
                    else
                    {
                        isDateTime1 = DateTime.TryParse(obj1.ToString(), out dateTime1);
                    }

                    if (obj2 is DateTime)
                    {
                        isDateTime2 = true;
                        dateTime2 = (DateTime)obj2;
                    }
                    else
                    {
                        isDateTime2 = DateTime.TryParse(obj2.ToString(), out dateTime2);
                    }

                    if (isDateTime1 && isDateTime2)
                    {
                        back = Math.Abs(dateTime1.Subtract(dateTime2).TotalSeconds) <= compDetail.Tolerance;
                    }
                    else
                    {
                        back = obj1.ToString().Equals(obj2.ToString(), StringComparison.CurrentCulture);
                    }
                    break;
                default:
                    back = obj1.ToString().Equals(obj2.ToString(), StringComparison.CurrentCulture);
                    break;
            }
            return back;

        }
    }
}
