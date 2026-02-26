using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web;
using System.Xml;
using ZidUtilities.CommonCode.ServerFiltering;

namespace ZidUtilities.CommonCode
{
    public static class Extensions
    {
        /// <summary>
        /// Represents a tick value used to indicate a non-expiring timestamp (max Int64).
        /// </summary>
        /// <remarks>It is used on AD Attributes such as accountExpires to denote no expiration.</remarks>
        public const long NonExpiringTickValue = 9223372036854775807;

        #region Empty Extensions
        /// <summary>
        /// Generates a short random token string.
        /// </summary>
        /// <returns>
        /// A Base64-encoded string derived from a new GUID byte array. Suitable for lightweight token needs.
        /// </returns>
        public static string GenerateToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
        #endregion

        #region Extensions for booleans
        /// <summary>
        /// Converts a boolean value to its lowercase string representation.
        /// </summary>
        /// <param name="boolean">The boolean value to convert.</param>
        /// <returns>
        /// "true" if <paramref name="boolean"/> is true; otherwise "false".
        /// </returns>
        public static string BoolAsString(this bool boolean)
        {
            return boolean ? "true" : "false";
        }
        #endregion

        #region Extensions for byte arrays
        /// <summary>
        /// Converts a byte array containing image data into a System.Drawing.Image.
        /// </summary>
        /// <param name="rawData">Byte array representing image binary data.</param>
        /// <returns>
        /// An <see cref="Image"/> instance created from the provided bytes.
        /// </returns>
        public static System.Drawing.Image AsImage(this byte[] rawData)
        {
            var ms = new MemoryStream(rawData);
            var returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }
        #endregion

        #region Extensions for System.Drawing.Color
        /// <summary>
        /// Converts a <see cref="Color"/> to its HTML (hex) string representation.
        /// </summary>
        /// <param name="htmlColor">The <see cref="Color"/> to convert.</param>
        /// <returns>
        /// A string containing the HTML color code (for example "#RRGGBB" or named color).
        /// </returns>
        public static string ColorToString(this Color htmlColor)
        {
            var aux = Color.FromArgb(htmlColor.ToArgb());
            return ColorTranslator.ToHtml(aux);
        }

        /// <summary>
        /// Computes the average color by mixing two colors equally.
        /// </summary>
        /// <param name="one">First color to mix.</param>
        /// <param name="two">Second color to mix.</param>
        /// <returns>
        /// A new <see cref="Color"/> that represents the half-way mix of <paramref name="one"/> and <paramref name="two"/>.
        /// </returns>
        public static Color HalfMix(this Color one, Color two)
        {
            return Color.FromArgb(
            (one.A + two.A) >> 1,
            (one.R + two.R) >> 1,
            (one.G + two.G) >> 1,
            (one.B + two.B) >> 1);
        }

        /// <summary>
        /// Returns a color with high contrast relative to the input color.
        /// </summary>
        /// <param name="color">The background color to evaluate.</param>
        /// <returns>
        /// <see cref="Color.Black"/> if the input is light; otherwise <see cref="Color.White"/> for dark colors.
        /// </returns>
        public static Color GetHighContrastColor(this Color color)
        {
            // Calculate the perceptive luminance (Y) - human eye favors green color
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            // Return black for light colors and white for dark colors
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        /// <summary>
        /// Randomly varies each RGB channel of a color within given maxima.
        /// </summary>
        /// <param name="color">Base color to vary.</param>
        /// <param name="red">Maximum random delta for the red channel (exclusive upper bound for RNG).</param>
        /// <param name="green">Maximum random delta for the green channel (exclusive upper bound for RNG).</param>
        /// <param name="blue">Maximum random delta for the blue channel (exclusive upper bound for RNG).</param>
        /// <returns>
        /// A new <see cref="Color"/> with each channel adjusted up or down by a random amount within the specified limits.
        /// </returns>
        public static Color Variate(this Color color, int red, int green, int blue)
        {
            Random rng = new Random();
            int nr = rng.Next(1, red);
            int ng = rng.Next(1, green);
            int nb = rng.Next(1, blue);
            Color back = Color.FromArgb
                (
                    color.R + nr > 255 ? color.R - nr : color.R + nr,
                    color.G + ng > 255 ? color.G - ng : color.G + ng,
                    color.B + nb > 255 ? color.B - nb : color.B + nb
                );

            return back;
        }
        #endregion

        #region Extensions for DataTable
        /// <summary>
        /// Saves a <see cref="DataTable"/> to a CSV file.
        /// </summary>
        /// <param name="dt">The table to save.</param>
        /// <param name="fileName">Path to the destination CSV file.</param>
        /// <remarks>
        /// Columns and rows are serialized with comma separators. Fields are escaped using <c>EnsureCsvField()</c>.
        /// </remarks>
        public static void SaveToCsv(this DataTable dt, string fileName)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            string headerLine = "";
            foreach (string cn in columnNames)
                headerLine += headerLine.IsEmpty() ? cn.EnsureCsvField() : "," + cn.EnsureCsvField();
            sb.AppendLine(headerLine);

            foreach (DataRow row in dt.Rows)
            {
                string line = "";
                int index = 0;
                foreach (string cell in row.ItemArray.Select(x => x.ToString()))
                {
                    if (index == 0)
                        line += cell.EnsureCsvField();
                    else
                        line += "," + cell.EnsureCsvField();
                    index++;
                }
                sb.AppendLine(line);
            }

            File.WriteAllText(fileName, sb.ToString());
        }

        /// <summary>
        /// Builds a SQL Server CREATE TABLE statement representing the schema of the provided <see cref="DataTable"/>.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> whose schema will be translated to SQL.</param>
        /// <returns>
        /// A string containing a CREATE TABLE statement plus any ALTER TABLE statements for default constraints.
        /// </returns>
        public static string GetCreateTableSql(this DataTable table)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder alterSql = new StringBuilder();

            sql.AppendFormat("CREATE TABLE [{0}] (", table.TableName);

            for (int i = 0; i < table.Columns.Count; i++)
            {
                bool isNumeric = false;
                bool usesColumnDefault = true;

                sql.AppendFormat("\n\t[{0}]", table.Columns[i].ColumnName);

                switch (table.Columns[i].DataType.ToString().ToUpper())
                {
                    case "SYSTEM.INT16":
                        sql.Append(" smallint");
                        isNumeric = true;
                        break;
                    case "SYSTEM.INT32":
                        sql.Append(" int");
                        isNumeric = true;
                        break;
                    case "SYSTEM.INT64":
                        sql.Append(" bigint");
                        isNumeric = true;
                        break;
                    case "SYSTEM.DATETIME":
                        sql.Append(" datetime");
                        usesColumnDefault = false;
                        break;
                    case "SYSTEM.STRING":
                        sql.AppendFormat(" nvarchar({0})", table.Columns[i].MaxLength);
                        break;
                    case "SYSTEM.SINGLE":
                        sql.Append(" single");
                        isNumeric = true;
                        break;
                    case "SYSTEM.DOUBLE":
                        sql.Append(" double");
                        isNumeric = true;
                        break;
                    case "SYSTEM.DECIMAL":
                        sql.AppendFormat(" decimal(18, 6)");
                        isNumeric = true;
                        break;
                    default:
                        sql.AppendFormat(" nvarchar({0})", table.Columns[i].MaxLength);
                        break;
                }

                if (table.Columns[i].AutoIncrement)
                {
                    sql.AppendFormat(" IDENTITY({0},{1})",
                        table.Columns[i].AutoIncrementSeed,
                        table.Columns[i].AutoIncrementStep);
                }
                else
                {
                    // DataColumns will add a blank DefaultValue for any AutoIncrement column. 
                    // We only want to create an ALTER statement for those columns that are not set to AutoIncrement. 
                    if (table.Columns[i].DefaultValue != null)
                    {
                        if (usesColumnDefault)
                        {
                            if (isNumeric)
                            {
                                alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ({2}) FOR [{1}];",
                                    table.TableName,
                                    table.Columns[i].ColumnName,
                                    table.Columns[i].DefaultValue);
                            }
                            else
                            {
                                alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ('{2}') FOR [{1}];",
                                    table.TableName,
                                    table.Columns[i].ColumnName,
                                    table.Columns[i].DefaultValue);
                            }
                        }
                        else
                        {
                            // Default values on Date columns, e.g., "DateTime.Now" will not translate to SQL.
                            // This inspects the caption for a simple XML string to see if there is a SQL compliant default value, e.g., "GETDATE()".
                            try
                            {
                                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();

                                xml.LoadXml(table.Columns[i].Caption);

                                alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ({2}) FOR [{1}];",
                                    table.TableName,
                                    table.Columns[i].ColumnName,
                                    xml.GetElementsByTagName("defaultValue")[0].InnerText);
                            }
                            catch
                            {
                                // Handle
                            }
                        }
                    }
                }

                if (!table.Columns[i].AllowDBNull)
                {
                    sql.Append(" NOT NULL");
                }

                sql.Append(",");
            }

            if (table.PrimaryKey.Length > 0)
            {
                StringBuilder primaryKeySql = new StringBuilder();

                primaryKeySql.AppendFormat("\n\tCONSTRAINT PK_{0} PRIMARY KEY (", table.TableName);

                for (int i = 0; i < table.PrimaryKey.Length; i++)
                {
                    primaryKeySql.AppendFormat("{0},", table.PrimaryKey[i].ColumnName);
                }

                primaryKeySql.Remove(primaryKeySql.Length - 1, 1);
                primaryKeySql.Append(")");

                sql.Append(primaryKeySql);
            }
            else
            {
                sql.Remove(sql.Length - 1, 1);
            }

            sql.AppendFormat("\n);\n{0}", alterSql.ToString());

            return sql.ToString();
        }
        #endregion

        #region Extensions for DateTime
        /// <summary>
        /// Converts a nullable <see cref="DateTime"/> to Active Directory (AD) ticks representation.
        /// </summary>
        /// <param name="date">Nullable date to convert.</param>
        /// <returns>
        /// AD tick representation as a <see cref="long"/>; returns <see cref="NonExpiringTickValue"/> when <paramref name="date"/> is null or not set.
        /// </returns>
        public static long DateToAdTicks(this DateTime? date)
        {
            if (date.HasValue && date.Value > DateTime.MinValue)
            {
                var beginningOfTimes = new DateTime(1601, 1, 1).Subtract(new TimeSpan(1, 0, 0, 0));
                return new DateTime(date.Value.Ticks - beginningOfTimes.Ticks).Ticks;
            }

            return NonExpiringTickValue;
        }

        /// <summary>
        /// Converts a nullable <see cref="DateTime"/> to Active Directory (AD) ticks representation.
        /// Duplicate naming variant retained for compatibility.
        /// </summary>
        /// <param name="date">Nullable date to convert.</param>
        /// <returns>
        /// AD tick representation as a <see cref="long"/>; returns <see cref="NonExpiringTickValue"/> when <paramref name="date"/> is null or not set.
        /// </returns>
        public static long DateToADTicks(this DateTime? date)
        {
            if (date.HasValue && date.Value > DateTime.MinValue)
            {
                DateTime adBeginningOfTimes = new DateTime(1601, 1, 1).Subtract(new TimeSpan(1, 0, 0, 0));
                return new DateTime(date.Value.Ticks - adBeginningOfTimes.Ticks).Ticks;
            }
            return NonExpiringTickValue;
        }

        /// <summary>
        /// Determines whether two nullable dates represent the same calendar date.
        /// </summary>
        /// <param name="tDate">First nullable date.</param>
        /// <param name="oDate">Second nullable date.</param>
        /// <returns>
        /// True if both are null or both have values with identical Date components; otherwise false.
        /// </returns>
        public static bool IsDateEqualTo(this DateTime? tDate, DateTime? oDate)
        {
            if (tDate == null && oDate == null)
            {
                return true;
            }
            else if (!tDate.HasValue && !oDate.HasValue)
            {
                return true;
            }
            else if (tDate.HasValue && oDate.HasValue)
            {
                return tDate.Value.Date.Equals(oDate.Value.Date);
            }
            return false;
        }

        /// <summary>
        /// Determines whether a nullable date equals a specific <see cref="DateTime"/> by date component.
        /// </summary>
        /// <param name="tDate">Nullable date to compare.</param>
        /// <param name="oDate">Non-nullable date to compare against.</param>
        /// <returns>
        /// True if <paramref name="tDate"/> has a value and the Date components are equal; otherwise false.
        /// </returns>
        public static bool IsDateEqualTo(this DateTime? tDate, DateTime oDate)
        {
            if (tDate == null || !tDate.HasValue)
                return false;
            return tDate.Value.Date.Equals(oDate.Date);
        }

        /// <summary>
        /// Compares a nullable <see cref="DateTime"/> to a <see cref="DateTime"/> by date component.
        /// </summary>
        /// <param name="date1">Nullable date to compare.</param>
        /// <param name="date2">Non-nullable date to compare against.</param>
        /// <returns>True if <paramref name="date1"/> has a value and their Date components are equal; otherwise false.</returns>
        public static bool IsEqualTo(this DateTime? date1, DateTime date2)
        {
            if (date1 == null)
            {
                return false;
            }

            return date1.Value.Date.Equals(date2.Date);
        }

        /// <summary>
        /// Compares two nullable <see cref="DateTime"/> values by date component.
        /// </summary>
        /// <param name="date1">First nullable date.</param>
        /// <param name="date2">Second nullable date.</param>
        /// <returns>True if both are null or both have the same Date component; otherwise false.</returns>
        public static bool IsEqualTo(this DateTime? date1, DateTime? date2)
        {
            if (date1 == null && date2 == null)
            {
                return true;
            }

            if (date1.HasValue && date2.HasValue)
            {
                return date1.Value.Date.Equals(date2.Value.Date);
            }

            return false;
        }

        /// <summary>
        /// Formats a nullable date as "MM/dd/yyyy" or returns an empty string when null.
        /// </summary>
        /// <param name="dt">Nullable date to format.</param>
        /// <returns>Formatted date string or empty string if null.</returns>
        public static string SafeStringDate(this DateTime? dt)
        {
            return dt.HasValue ? dt.Value.ToString("MM/dd/yyyy") : "";
        }

        /// <summary>
        /// Returns the first day of the month for the specified date.
        /// </summary>
        /// <param name="date">Input date.</param>
        /// <returns>A <see cref="DateTime"/> representing the first day of the month at midnight with the same Kind as input.</returns>
        public static DateTime BeginningOfTheMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Returns a new <see cref="DateTime"/> with the same date components but replaced time components.
        /// </summary>
        /// <param name="dateTime">Source date/time.</param>
        /// <param name="hours">Hour component (0-23).</param>
        /// <param name="minutes">Minute component (0-59).</param>
        /// <param name="seconds">Second component (0-59). Default is 0.</param>
        /// <param name="milliseconds">Millisecond component (0-999). Default is 0.</param>
        /// <returns>
        /// A <see cref="DateTime"/> with updated time components and the same Kind as the original.
        /// </returns>
        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds = 0, int milliseconds = 0)
        {
            return new DateTime(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            hours,
            minutes,
            seconds,
            milliseconds,
            dateTime.Kind);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to Active Directory ticks representation.
        /// </summary>
        /// <param name="date">Date to convert.</param>
        /// <returns>
        /// AD tick representation as a <see cref="long"/>; returns <see cref="NonExpiringTickValue"/> when <paramref name="date"/> is not set.
        /// </returns>
        public static long DateToAdTicks(this DateTime date)
        {
            if (date > DateTime.MinValue)
            {
                var beginningOfTimes = new DateTime(1601, 1, 1);
                return new DateTime(date.Ticks - new DateTime(1601, 1, 1).Ticks).Ticks;
            }

            return NonExpiringTickValue;
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to Active Directory ticks representation.
        /// Duplicate naming variant retained for compatibility.
        /// </summary>
        /// <param name="date">Date to convert.</param>
        /// <returns>AD tick representation as <see cref="long"/>; returns <see cref="NonExpiringTickValue"/> when <paramref name="date"/> is not set.</returns>
        public static long DateToADTicks(this DateTime date)
        {
            if (date > DateTime.MinValue)
            {
                DateTime adBeginningOfTimes = new DateTime(1601, 1, 1);
                return new DateTime(date.Ticks - new DateTime(1601, 1, 1).Ticks).Ticks;
            }
            return NonExpiringTickValue;
        }

        /// <summary>
        /// Calculates the elapsed time from the provided <see cref="DateTime"/> until now.
        /// </summary>
        /// <param name="input">The starting <see cref="DateTime"/>.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the elapsed time since <paramref name="input"/>.</returns>
        public static TimeSpan Elapsed(this DateTime input)
        {
            return DateTime.Now.Subtract(input);
        }

        /// <summary>
        /// Returns the first second (00:00:00.000) of the date portion of the specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">Input <see cref="DateTime"/>.</param>
        /// <returns>A <see cref="DateTime"/> representing the start of the given day.</returns>
        public static DateTime FirstSecondOfDay(this DateTime date)
        {
            return date.ChangeTime(0, 0, 0, 0);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to a database-friendly value (returns <see cref="DBNull.Value"/> for MinValue).
        /// </summary>
        /// <param name="dt">Date to convert.</param>
        /// <returns>
        /// <see cref="DBNull.Value"/> if <paramref name="dt"/> equals <see cref="DateTime.MinValue"/>; otherwise the original <see cref="DateTime"/>.
        /// </returns>
        public static object ForDatabase(this DateTime dt)
        {
            if (dt == DateTime.MinValue)
                return DBNull.Value;
            return dt;
        }

        /// <summary>
        /// Converts a nullable <see cref="DateTime"/> to a database-friendly value.
        /// </summary>
        /// <param name="dt">Nullable date to convert.</param>
        /// <returns>
        /// <see cref="DBNull.Value"/> if <paramref name="dt"/> is null or equals <see cref="DateTime.MinValue"/>; otherwise the original nullable date.
        /// </returns>
        public static object ForDatabase(this DateTime? dt)
        {
            if (!dt.HasValue || dt.Value == DateTime.MinValue)
                return DBNull.Value;
            return dt;
        }

        /// <summary>
        /// Returns the last calendar day of the month for the given date.
        /// </summary>
        /// <param name="dateTime">Input date.</param>
        /// <returns>A <see cref="DateTime"/> set to the last day of the month (time component preserved as midnight).</returns>
        public static DateTime GetLastDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Determines whether a date falls between two other dates.
        /// </summary>
        /// <param name="dt">Date to test.</param>
        /// <param name="startDate">Inclusive start date.</param>
        /// <param name="endDate">Inclusive end date.</param>
        /// <param name="compareTime">
        /// If true, compares full DateTime values including time-of-day; if false (default), compares only the Date components.
        /// </param>
        /// <returns>True if <paramref name="dt"/> is within the inclusive range; otherwise false.</returns>
        public static Boolean IsBetween(this DateTime dt, DateTime startDate, DateTime endDate, Boolean compareTime = false)
        {
            return compareTime ? dt >= startDate && dt <= endDate : dt.Date >= startDate.Date && dt.Date <= endDate.Date;
        }

        /// <summary>
        /// Determines if a <see cref="DateTime"/> equals a nullable <see cref="DateTime"/> by date component.
        /// </summary>
        /// <param name="tDate">Non-nullable date.</param>
        /// <param name="oDate">Nullable date to compare.</param>
        /// <returns>True if <paramref name="oDate"/> has value and their Date components match; otherwise false.</returns>
        public static bool IsDateEqualTo(this DateTime tDate, DateTime? oDate)
        {
            if (oDate == null || !oDate.HasValue)
                return false;
            return tDate.Date.Equals(oDate.Value.Date);
        }

        /// <summary>
        /// Compares a <see cref="DateTime"/> to a nullable <see cref="DateTime"/> by date component.
        /// </summary>
        /// <param name="date1">Non-nullable date.</param>
        /// <param name="date2">Nullable date to compare.</param>
        /// <returns>True if <paramref name="date2"/> has a value and their Date components are equal; otherwise false.</returns>
        public static bool IsEqualTo(this DateTime date1, DateTime? date2)
        {
            if (date2 == null)
            {
                return false;
            }

            return date1.Date.Equals(date2.Value.Date);
        }

        /// <summary>
        /// Determines whether the given <see cref="DateTime"/> falls on a weekday.
        /// </summary>
        /// <param name="d">The date to test.</param>
        /// <returns>True if the day is Monday-Friday; otherwise false.</returns>
        public static bool IsWeekday(this DateTime d)
        {
            return d.DayOfWeek.IsWeekday();
        }

        /// <summary>
        /// Determines whether the given <see cref="DateTime"/> falls on a weekend.
        /// </summary>
        /// <param name="d">The date to test.</param>
        /// <returns>True if the day is Saturday or Sunday; otherwise false.</returns>
        public static bool IsWeekend(this DateTime d)
        {
            return d.DayOfWeek.IsWeekend();
        }

        /// <summary>
        /// Returns the last second of the day (23:59:59.000) for the provided date.
        /// </summary>
        /// <param name="date">Input date.</param>
        /// <returns>A <see cref="DateTime"/> representing the last second of the given day.</returns>
        public static DateTime LastSecondOfDay(this DateTime date)
        {
            return date.ChangeTime(23, 59, 59, 0);
        }

        /// <summary>
        /// Returns the next date that falls on the specified <see cref="DayOfWeek"/> relative to the current date.
        /// </summary>
        /// <param name="dt">Reference date.</param>
        /// <param name="day">Desired day of the week.</param>
        /// <returns>
        /// The next date that is the specified <paramref name="day"/>. If the computed date is earlier in the month than the reference, it advances by 7 days.
        /// </returns>
        public static DateTime NextDayOfWeek(this DateTime dt, DayOfWeek day)
        {
            var d = new GregorianCalendar().AddDays(dt, -((int)dt.DayOfWeek) + (int)day);
            return (d.Day < dt.Day) ? d.AddDays(7) : d;
        }

        /// <summary>
        /// Returns the next Sunday after the provided date.
        /// </summary>
        /// <param name="dt">Reference date.</param>
        /// <returns>The next Sunday date.</returns>
        public static DateTime NextSunday(this DateTime dt)
        {
            return new GregorianCalendar().AddDays(dt, -((int)dt.DayOfWeek) + 7);
        }

        /// <summary>
        /// Given a date and a start-of-week setting, returns the start date of the week that contains the given date.
        /// </summary>
        /// <param name="dt">The original date time.</param>
        /// <param name="startOfWeek">The day of the week considered as start of week.</param>
        /// <returns>Previous date considered start of the week (same week as <paramref name="dt"/>).</returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Returns the Friday date of the current week.
        /// </summary>
        /// <param name="dt">Reference date (unused within calculation; method uses current date).</param>
        /// <returns>A <see cref="DateTime"/> representing this week's Friday.</returns>
        public static DateTime ThisWeekFriday(this DateTime dt)
        {
            var today = DateTime.Now;
            return new GregorianCalendar().AddDays(today, -((int)today.DayOfWeek) + 5);
        }

        /// <summary>
        /// Returns the Monday date of the current week.
        /// </summary>
        /// <param name="dt">Reference date (unused within calculation; method uses current date).</param>
        /// <returns>A <see cref="DateTime"/> representing this week's Monday.</returns>
        public static DateTime ThisWeekMonday(this DateTime dt)
        {
            var today = DateTime.Now;
            return new GregorianCalendar().AddDays(today, -((int)today.DayOfWeek) + 1);
        }       

        /// <summary>
        /// Returns a string representation of the elapsed time since <paramref name="initialTime"/> using the "c" format.
        /// </summary>
        /// <param name="initialTime">Start time to measure from.</param>
        /// <returns>A formatted string representing the elapsed time span.</returns>
        public static string TimeElapsed(this DateTime initialTime)
        {
            return DateTime.Now.Subtract(initialTime).ToString("c");

        }

        /// <summary>
        /// Formats a <see cref="DateTime"/> as an ISO-like compact string: "yyyyMMdd HHmmss.ff".
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> to format.</param>
        /// <returns>Formatted string representation of the date/time.</returns>
        public static string ToIsoDate(this DateTime value) { return value.ToString("yyyyMMdd HHmmss.ff"); }
        
        /// <summary>
        /// Formats a <see cref="DateTime"/> as an ISO-like compact string: "yyyyMMdd HHmmss.ff".
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> to format.</param>
        /// <returns>Formatted string representation of the date/time.</returns>
        public static string ToIsoDate2(this DateTime value) { return value.ToString("yyyy-MM-ddTHH:mm:ss.ff"); }

        /// <summary>
        /// Determines whether a <see cref="DayOfWeek"/> represents a weekday.
        /// </summary>
        /// <param name="d">Day of week to test.</param>
        /// <returns>True when the day is Monday-Friday; otherwise false.</returns>
        public static bool IsWeekday(this DayOfWeek d)
        {
            switch (d)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Saturday: return false;

                default: return true;
            }
        }

        /// <summary>
        /// Determines whether a <see cref="DayOfWeek"/> represents a weekend.
        /// </summary>
        /// <param name="d">Day of week to test.</param>
        /// <returns>True when the day is Saturday or Sunday; otherwise false.</returns>
        public static bool IsWeekend(this DayOfWeek d)
        {
            return !d.IsWeekday();
        }

        /// <summary>
        /// Determines whether a nullable double has a positive (> 0) value.
        /// </summary>
        /// <param name="number">Nullable double to test.</param>
        /// <returns>True if <paramref name="number"/> has a value and that value is greater than zero; otherwise false.</returns>
        public static bool HasPositiveValue(this double? number)
        {
            return number.HasValue && number.Value > 0;
        }

        #endregion

        #region Extensions for HttpSessionStateBase
        /// <summary>
        /// Retrieves a value from <see cref="HttpSessionStateBase"/> and converts it to the requested type.
        /// </summary>
        /// <typeparam name="T">Type to convert the session value to.</typeparam>
        /// <param name="session">Session object to read from.</param>
        /// <param name="key">Key identifying the session entry.</param>
        /// <returns>
        /// The converted value if present; otherwise the default value for <typeparamref name="T"/>.
        /// </returns>
        public static T GetValue<T>(this HttpSessionStateBase session, string key)
        {
            return session.GetValue<T>(key, default(T));
        }

        /// <summary>
        /// Retrieves a value from <see cref="HttpSessionStateBase"/> and converts it to the requested type, or returns a provided default.
        /// </summary>
        /// <typeparam name="T">Type to convert the session value to.</typeparam>
        /// <param name="session">Session object to read from.</param>
        /// <param name="key">Key identifying the session entry.</param>
        /// <param name="defaultValue">Value to return if the session entry is not present.</param>
        /// <returns>
        /// The converted value if present; otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static T GetValue<T>(this HttpSessionStateBase session, string key, T defaultValue)
        {
            if (session[key] != null)
            {
                return (T)Convert.ChangeType(session[key], typeof(T));
            }

            return defaultValue;
        }
        #endregion

        #region Extensions for ICollection
        /// <summary>
        /// Converts a collection of objects into a <see cref="DataTable"/> using property names as columns.
        /// </summary>
        /// <typeparam name="T">Type of objects contained in the collection.</typeparam>
        /// <param name="list">Collection to convert.</param>
        /// <param name="tableName">Optional table name to assign to the resulting <see cref="DataTable"/>.</param>
        /// <returns>A populated <see cref="DataTable"/> containing one column per public property and one row per item.</returns>
        public static DataTable ConvertToDataTable<T>(this ICollection<T> list, string tableName = "")
        {
            var table = CreateTable<T>(tableName);
            var entityType = typeof(T);
            var properties = TypeDescriptor.GetProperties(entityType);

            foreach (var item in list)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// Exports a collection of objects to a CSV file using their public properties as columns.
        /// </summary>
        /// <typeparam name="T">Type of objects in the collection.</typeparam>
        /// <param name="data">Collection to export.</param>
        /// <param name="fileName">Destination CSV file path.</param>
        /// <remarks>
        /// Properties are used as headers. Field values are escaped using <c>EnsureCsvField()</c>.
        /// </remarks>
        public static void ExportToCSV<T>(this ICollection<T> data, string fileName)
        {
            bool header = true;
            using (StreamWriter wr = new StreamWriter(fileName))
            {
                if (data != null)
                {
                    var entityType = typeof(T);
                    var properties = TypeDescriptor.GetProperties(entityType);

                    foreach (T d in data)
                    {
                        StringBuilder line = new StringBuilder();

                        if (header)
                        {
                            StringBuilder headerLine = new StringBuilder();

                            foreach (PropertyDescriptor prop in properties)
                            {
                                headerLine.AppendFormat(headerLine.Length == 0 ? "{0}" : ",{0}", prop.Name.EnsureCsvField());
                            }
                            wr.WriteLine(headerLine.ToString());
                            header = false;
                        }

                        foreach (PropertyDescriptor prop in properties)
                        {
                            object value = prop.GetValue(d);
                            if (value != null)
                            {
                                line.AppendFormat(line.Length == 0 ? "{0}" : ",{0}", prop.Name.EnsureCsvField());
                            }
                            else
                            {
                                line.Append(",");
                            }
                        }
                        wr.WriteLine(line.ToString());
                    }
                }
                wr.Flush();
                wr.Close();
            }
        }
        #endregion

        #region Extensions for IDataRecord
        /// <summary>
        /// Determines whether the given data record contains a column with the specified name.
        /// </summary>
        /// <param name="dr">The data record to inspect.</param>
        /// <param name="columnName">The column name to look for (case-insensitive).</param>
        /// <returns>True if a column with the specified name exists; otherwise false.</returns>
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
        #endregion

        #region Extensions for IEnumerable
        /// <summary>
        /// Checks whether an enumerable is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">Type of the enumerable elements.</typeparam>
        /// <param name="enumerable">The enumerable to check.</param>
        /// <returns>True if <paramref name="enumerable"/> is null or has no elements; otherwise false.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        /// <summary>
        /// Returns distinct elements from a sequence by using a specified key selector to compare values.
        /// </summary>
        /// <typeparam name="TSource">Type of source elements.</typeparam>
        /// <typeparam name="TKey">Type of the key used to determine uniqueness.</typeparam>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <param name="keySelector">Function to extract the key for each element.</param>
        /// <returns>An enumerable that contains distinct elements from the source sequence.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
        #endregion

        #region Extensions for IList
        /// <summary>
        /// Converts a list of objects into a DataTable.
        /// </summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="list">The list to convert.</param>
        /// <param name="tableName">Optional table name to assign to the DataTable. If empty, a default is used.</param>
        /// <returns>A DataTable populated with the objects from <paramref name="list"/>.</returns>
        public static DataTable ConvertToDataTable<T>(this IList<T> list, string tableName = "")
        {
            var table = CreateTable<T>(tableName);
            var entityType = typeof(T);
            var properties = TypeDescriptor.GetProperties(entityType);

            foreach (var item in list)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }
        /// <summary>
        /// Shuffles the elements of the list in place using a pseudo-random order.
        /// </summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        #endregion

        #region Extensions for int
        /// <summary>
        /// Ensures a nullable integer is converted to a CSV-safe string representation.
        /// </summary>
        /// <param name="integer">The nullable integer to format.</param>
        /// <returns>An empty string for null, otherwise the integer as a CSV-safe string.</returns>
        public static string EnsureCsvField(this int? integer)
        {
            if (!integer.HasValue)
                return "";
            return integer.ToString().EnsureCsvField();
        }
        /// <summary>
        /// Converts an integer in the range 0-15 to its single hex character representation.
        /// </summary>
        /// <param name="c">Integer value expected between 0 and 15 inclusive.</param>
        /// <returns>A single-character hex string ("0"-"9", "A"-"F") for valid inputs; otherwise an empty string.</returns>
        public static string FromDecIntToHexChar(this int c)
        {
            if (c < 10 && c >= 0)
            {
                return c.ToString();
            }

            switch (c)
            {
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
            }

            return string.Empty;
        }
        /// <summary>
        /// Converts an integer to its hexadecimal string representation.
        /// </summary>
        /// <param name="val">Integer value to convert (non-negative expected).</param>
        /// <returns>Hexadecimal representation as a string (uppercase letters).</returns>
        public static string FromDecIntToHexString(this int val)
        {
            char[] baseSymbols = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            var result = string.Empty;
            var targetBase = baseSymbols.Length;

            do
            {
                result = baseSymbols[val % targetBase] + result;
                val = val / targetBase;
            }
            while (val > 0);

            return result;
        }
        /// <summary>
        /// Generates a random hexadecimal-like string of specified length based on GUIDs.
        /// </summary>
        /// <param name="length">Desired length of the returned string.</param>
        /// <returns>A random string of the requested length composed from concatenated GUID hex digits.</returns>
        public static string GenerateRandomString(int length)
        {
            StringBuilder builder = new StringBuilder();
            if (length > 0)
            {
                while (builder.Length < length)
                {
                    string str = Guid.NewGuid().ToString("N");
                    int delta = length - builder.Length;

                    if (delta >= str.Length)
                    {
                        builder.Append(str);
                    }
                    else
                    {
                        builder.Append(str.Substring(0, delta));
                    }
                }

            }
            return builder.ToString();
        }
        /// <summary>
        /// Clamps an integer to be within the inclusive bounds specified by <paramref name="lo"/> and <paramref name="hi"/>.
        /// If <paramref name="hi"/> is less than <paramref name="lo"/>, the bounds are swapped.
        /// </summary>
        /// <param name="x">Value to clamp.</param>
        /// <param name="lo">Lower bound.</param>
        /// <param name="hi">Upper bound.</param>
        /// <returns>The clamped value within [lo, hi].</returns>
        public static int InRange(this int x, int lo, int hi)
        {
            if (hi < lo)
            {
                int temp = hi;
                hi = lo;
                lo = temp;
            }

            if (x < lo)
                return lo;
            if (x > hi)
                return hi;
            return x;
        }
        /// <summary>
        /// Determines whether an integer lies within the inclusive range [lo, hi].
        /// </summary>
        /// <param name="x">Value to check.</param>
        /// <param name="lo">Lower bound.</param>
        /// <param name="hi">Upper bound.</param>
        /// <returns>True if <paramref name="x"/> is between <paramref name="lo"/> and <paramref name="hi"/> inclusive.</returns>
        public static bool IsInRange(this int x, int lo, int hi)
        {
            return x >= lo && x <= hi;
        }
        #endregion

        #region Extensions for IQueryable
        /// <summary>
        /// Applies an additional ordering (ThenBy) to an already ordered queryable using a property name and string direction.
        /// </summary>
        /// <typeparam name="T">Element type of the queryable.</typeparam>
        /// <param name="source">The ordered source queryable.</param>
        /// <param name="propertyName">Name of the property to order by.</param>
        /// <param name="direction">If equal to "DESC" (case-insensitive) a descending order is applied; otherwise ascending.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> with the additional ordering applied.</returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName, string direction)
        {
            return OrderingHelper(source, propertyName, direction.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? true : false, true);
        }

        /// <summary>
        /// Applies an additional ascending ordering (ThenBy) to an already ordered queryable using a property name.
        /// </summary>
        /// <typeparam name="T">Element type of the queryable.</typeparam>
        /// <param name="source">The ordered source queryable.</param>
        /// <param name="propertyName">Name of the property to order by.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> with the additional ascending ordering applied.</returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, true);
        }

        /// <summary>
        /// Applies an additional descending ordering (ThenByDescending) to an already ordered queryable using a property name.
        /// </summary>
        /// <typeparam name="T">Element type of the queryable.</typeparam>
        /// <param name="source">The ordered source queryable.</param>
        /// <param name="propertyName">Name of the property to order by.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> with the additional descending ordering applied.</returns>
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, true);
        }

        /// <summary>
        /// Orders a queryable by a property name and a string direction.
        /// </summary>
        /// <typeparam name="T">Element type of the queryable.</typeparam>
        /// <param name="source">The source queryable to order.</param>
        /// <param name="propertyName">Name of the property to order by.</param>
        /// <param name="direction">If equal to "DESC" (case-insensitive) a descending order is applied; otherwise ascending.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> with the specified ordering applied.</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, string direction)
        {
            return OrderingHelper(source, propertyName, direction.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? true : false, false);
        }

        /// <summary>
        /// Orders a queryable by a property name in ascending order.
        /// </summary>
        /// <typeparam name="T">Element type of the queryable.</typeparam>
        /// <param name="source">The source queryable to order.</param>
        /// <param name="propertyName">Name of the property to order by.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> ordered ascending by the given property.</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, false);
        }

        /// <summary>
        /// Orders a queryable by a property name in descending order.
        /// </summary>
        /// <typeparam name="T">Element type of the queryable.</typeparam>
        /// <param name="source">The source queryable to order.</param>
        /// <param name="propertyName">Name of the property to order by.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> ordered descending by the given property.</returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, false);
        }

        /// <summary>
        /// Internal helper that constructs an expression tree to order a queryable by a property name.
        /// </summary>
        /// <typeparam name="T">Element type of the queryable.</typeparam>
        /// <param name="source">Source queryable to be ordered.</param>
        /// <param name="propertyName">Name of the property or field to sort by.</param>
        /// <param name="descending">If true, applies descending order; otherwise ascending.</param>
        /// <param name="anotherLevel">If true, applies a ThenBy/ThenByDescending on an already ordered sequence; otherwise OrderBy/OrderByDescending.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> with the ordering applied.</returns>
        private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), string.Empty); // I don't care about some naming
            MemberExpression property = Expression.PropertyOrField(param, propertyName);
            LambdaExpression sort = Expression.Lambda(property, param);

            MethodCallExpression call = Expression.Call(
            typeof(Queryable),
            (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
            new[] { typeof(T), property.Type },
            source.Expression,
            Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }
        #endregion

        #region Extensions for List
        /// <summary>
        /// Performs a bulk insert of a list into a SQL Server table. If the table does not exist, attempts to create it.
        /// </summary>
        /// <typeparam name="T">Type of items in the list; used to infer table structure.</typeparam>
        /// <param name="list">The items to insert.</param>
        /// <param name="connectionString">The connection string to the SQL Server database.</param>
        /// <param name="tableName">Optional explicit table name; if not provided the type name (pluralized by adding 's') is used.</param>
        /// <remarks>
        /// This method will attempt to create a table using the schema produced by <see cref="Extensions2.CreateTable{T}"/> if one does not exist.
        /// It uses <see cref="SqlBulkCopy"/> to perform the insert. Exceptions from ADO.NET are propagated.
        /// </remarks>
        public static void BulkInsert<T>(this List<T> list, string connectionString, string tableName = "")
        {
            if (tableName.IsEmpty())
                tableName = typeof(T).Name;
            tableName = tableName.EndsWith("s") ? tableName : tableName + "s";

            string checkTable = $@"IF OBJECT_ID(N'dbo.{tableName}', N'U') IS NULL
BEGIN
    @CREATETABLESCRIPT@
END";
            DataTable table = list.ConvertToDataTable(tableName);

            checkTable = checkTable.Replace("@CREATETABLESCRIPT@", table.GetCreateTableSql());

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.Connection = connection;
            command.CommandText = checkTable;
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                using (var bulkCopy = new SqlBulkCopy(connectionString))
                {
                    bulkCopy.BulkCopyTimeout = 120;
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(table);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Determines whether a list of strings contains a specified value, ignoring case.
        /// </summary>
        /// <param name="list">The list to search.</param>
        /// <param name="text">The text to find.</param>
        /// <returns>True if an equal string is found ignoring case; otherwise false.</returns>
        public static bool InsensitiveContains(this List<string> list, string text)
        {
            if (list.FindIndex(x => x.Equals(text, StringComparison.OrdinalIgnoreCase)) == -1)
                return false;
            return true;
        }

        /// <summary>
        /// Checks if a string value is contained in a list of strings, case-insensitive.
        /// </summary>
        /// <param name="list">The list of strings to search.</param>
        /// <param name="text">The text to search for. Null is treated as empty string.</param>
        /// <returns>True if found; otherwise false.</returns>
        public static bool InsensitiveListContains(this List<string> list, string text)
        {
            if (list.FindIndex(x => x.Equals(text ?? string.Empty, StringComparison.OrdinalIgnoreCase)) == -1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes all occurrences of a given string from the list using case-insensitive comparison.
        /// </summary>
        /// <param name="list">The list to modify.</param>
        /// <param name="text">The value to remove (null is treated as empty string).</param>
        public static void InsensitiveListRemove(this List<string> list, string text)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Equals(text ?? string.Empty, StringComparison.CurrentCultureIgnoreCase))
                {
                    list.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes all occurrences of a given string from the list using case-insensitive comparison.
        /// </summary>
        /// <param name="list">The list to modify.</param>
        /// <param name="text">The value to remove.</param>
        public static void InsensitiveRemove(this List<string> list, string text)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Equals(text, StringComparison.CurrentCultureIgnoreCase))
                {
                    list.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Compares two lists of strings for equality of contents. Order is not significant.
        /// </summary>
        /// <param name="list1">First list to compare.</param>
        /// <param name="list2">Second list to compare.</param>
        /// <param name="caseSensitive">When true performs case-sensitive comparison; otherwise case-insensitive.</param>
        /// <returns>True if both lists contain the same items (respecting case sensitivity), otherwise false.</returns>
        public static bool IsSameList(this List<string> list1, List<string> list2, bool caseSensitive = false)
        {
            if (caseSensitive)
            {
                if (list1 != null && list2 != null)
                {
                    if (list1.Count != list2.Count)
                    {
                        return false;
                    }

                    if (list1.Any(iteml1 => list2.FindIndex(x => x.Equals(iteml1, StringComparison.Ordinal)) == -1))
                    {
                        return false;
                    }

                    if (list2.Any(iteml2 => list1.FindIndex(x => x.Equals(iteml2, StringComparison.Ordinal)) == -1))
                    {
                        return false;
                    }
                }
                else
                {
                    return list1 == null && list2 == null;
                }
            }
            else
            {
                if (list1 != null && list2 != null)
                {
                    if (list1.Count != list2.Count)
                    {
                        return false;
                    }

                    if (
                        list1.Any(
                    iteml1 => list2.FindIndex(x => x.Equals(iteml1, StringComparison.OrdinalIgnoreCase)) == -1))
                    {
                        return false;
                    }

                    if (
                        list2.Any(
                    iteml2 => list1.FindIndex(x => x.Equals(iteml2, StringComparison.OrdinalIgnoreCase)) == -1))
                    {
                        return false;
                    }
                }
                else
                {
                    return list1 == null && list2 == null;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string exists on the list, with an option for case sensitivity.
        /// </summary>
        /// <param name="list">The list of strings to search.</param>
        /// <param name="str">The string to find.</param>
        /// <param name="caseSensitive">True to perform case-sensitive search; false to ignore case.</param>
        /// <returns>True if found; otherwise false.</returns>
        public static bool IsStringOnList(this List<string> list, string str, bool caseSensitive)
        {
            if (caseSensitive)
            {
                return list.Contains(str);
            }
            else
            {
                return list.FindIndex(x => x.Equals(str, StringComparison.OrdinalIgnoreCase)) != -1;
            }
        }

        /// <summary>
        /// Compares two lists of strings for equality of contents ignoring case. Order is not significant.
        /// </summary>
        /// <param name="list1">First list to compare.</param>
        /// <param name="list2">Second list to compare.</param>
        /// <returns>True if lists contain the same elements ignoring case; otherwise false.</returns>
        public static bool ListCompare(this List<string> list1, List<string> list2)
        {
            if (list1 != null && list2 != null)
            {
                if (list1.Count != list2.Count)
                    return false;

                foreach (string iteml1 in list1)
                {
                    if (list2.FindIndex(x => x.Equals(iteml1, StringComparison.OrdinalIgnoreCase)) == -1)
                    {
                        return false;
                    }
                }

                foreach (string iteml2 in list2)
                {
                    if (list1.FindIndex(x => x.Equals(iteml2, StringComparison.OrdinalIgnoreCase)) == -1)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return list1 == null && list2 == null;
            }

            return true;
        }

        /// <summary>
        /// Builds an HTML table string from a list of objects using the specified property names as columns.
        /// </summary>
        /// <typeparam name="T">Type of objects in the list.</typeparam>
        /// <param name="obj">List of objects to render as a table.</param>
        /// <param name="fields">Property names to include as table columns (order determines column order).</param>
        /// <returns>A string containing the HTML table; empty string if input is null or fields are not provided.</returns>
        public static string ConvertToHtmlTableString<T>(this List<T> obj, params string[] fields)
        {
            if (obj == null || fields == null || fields.Length == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            sb.AppendLine("<table style=\"border: 1px black solid;\" border=\"1px\">");
            sb.AppendLine("<tbody>");
            sb.AppendLine("<tr style=\"border: 1px black solid; font-weight:bold;\">");

            foreach (string field in fields)
            {
                PropertyInfo prop = entityType.GetProperty(field);
                if (prop != null)
                {
                    sb.AppendLine(String.Format("<th style=\"border: 1px black solid;\">{0}</th>", field));
                }
            }
            sb.AppendLine("</tr>");

            foreach (T item in obj)
            {
                sb.AppendLine("<tr style=\"border: 1px black solid;\">");

                foreach (string field in fields)
                {
                    PropertyInfo prop = entityType.GetProperty(field);
                    if (prop != null)
                    {
                        object bufferRawValue = prop.GetValue(item, null);
                        string formattedValueBuffer;

                        if (prop.PropertyType == typeof(float) || prop.PropertyType == typeof(double))
                        {
                            formattedValueBuffer = ((float)bufferRawValue).ToString("f2");
                        }
                        else if (prop.PropertyType == typeof(bool))
                        {
                            formattedValueBuffer = ((bool)bufferRawValue) ? "True" : "False";
                        }
                        else if (prop.PropertyType == typeof(DateTime))
                        {
                            formattedValueBuffer = ((DateTime)bufferRawValue).ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            formattedValueBuffer = bufferRawValue == null ? "" : bufferRawValue.ToString();
                        }

                        sb.AppendLine(String.Format("<td style=\"border: 1px black solid; text-align: left;\">{0}</td>", formattedValueBuffer));
                    }
                }

                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// Selects a random sample of elements from a list, optionally excluding specified elements.
        /// </summary>
        /// <typeparam name="T">Type of elements in the list.</typeparam>
        /// <param name="source">The source list to sample from.</param>
        /// <param name="sampleSize">Number of elements to sample.</param>
        /// <param name="exclude">Elements to exclude from sampling.</param>
        /// <returns>A new list containing up to <paramref name="sampleSize"/> randomly selected items not in <paramref name="exclude"/>.</returns>
        public static List<T> SampleList<T>(this List<T> source, int sampleSize, params T[] exclude)
        {
            Random az = new Random();
            List<T> back = new List<T>();

            if (source == null || source.Count == 0)
                return back;

            if (exclude.Length >= source.Count)
                return back;

            List<T> buff = new List<T>();
            buff.AddRange(source);

            foreach (T exc in exclude)
                buff.Remove(exc);

            while (back.Count < sampleSize && buff.Count > 0)
            {
                T randomItem = buff[az.Next(0, buff.Count)];
                back.Add(randomItem);
                buff.Remove(randomItem);
            }

            return back;
        }

        /// <summary>
        /// Apply the multi column sorting needed to a list of nodes
        /// </summary>
        /// <param name="initialRecords">Initial list of records</param>
        /// <param name="sort">List of sort descriptions</param>
        /// <returns>List of records sorted</returns>
        public static List<T> ApplyMultiSorting<T>(this List<T> initialRecords, List<SortDescription> sort)
        {
            #region Code for the the multi column sorting
            if (sort != null && sort.Count > 0 && !sort[0].field.Equals("none", StringComparison.CurrentCultureIgnoreCase))
            {
                IQueryable<T> sortHelper = initialRecords.AsQueryable();
                IOrderedQueryable<T> buffer = null;
                foreach (SortDescription sd in sort)
                {
                    buffer = (buffer == null)
                        ?
                            (sortHelper.OrderBy(sd.field, sd.dir))
                        :
                            (buffer.ThenBy(sd.field, sd.dir));
                }
                return buffer == null ? initialRecords : buffer.ToList();
            }
            #endregion

            return initialRecords;
        }

        public static List<T> ApplyFilters<T>(this List<T> res, string rawFilter) where T : IFilterableObject
        {
            //parse the string to objects
            if (String.IsNullOrEmpty(rawFilter) || (res == null || res.Count == 0))
                return res;

            FilterMerge filtersMerged = res[0].ParseFilterString(rawFilter);

            //then navigate through the object to apply all the filtering
            for (int filterIndex = 0, joinIndex = 0; filterIndex < filtersMerged.Filters.Count; filterIndex += 2, joinIndex++)
            {
                FilterDescription f1 = filtersMerged.Filters[filterIndex], f2 = filtersMerged.Filters[filterIndex + 1];
                FilterMerge.FilterJoin join = filtersMerged.JoinLogic[joinIndex];

                if (join == FilterMerge.FilterJoin.SingleFilter)
                {
                    res = res.Where(x => x.ValueOnFieldComparison(f1.Field, f1.Value, f1.Operator)).ToList();
                }
                else
                {
                    List<int> resultList;
                    //Apply the first filter
                    List<int> list1 = res.Where(x => x.ValueOnFieldComparison(f1.Field, f1.Value, f1.Operator)).Select(x => x.Id).ToList();
                    //Apply the second filter
                    List<int> list2 = res.Where(x => x.ValueOnFieldComparison(f2.Field, f2.Value, f2.Operator)).Select(x => x.Id).ToList();

                    if (join == FilterMerge.FilterJoin.AndLogic)//last value of tuple is true -> "AND" Logic, get results that are on both filters
                    {
                        resultList = list1.Where(x => list2.Contains(x)).ToList();
                    }
                    else
                    {//last value of tuple is false -> "OR" Logic, get results that are on either of the filters
                        resultList = list1.ToList();
                        foreach (int i in list2)
                        {
                            if (!resultList.Contains(i))
                                resultList.Add(i);
                        }
                    }
                    res = res.Where(x => resultList.Contains(x.Id)).ToList();
                }
            }

            return res;
        }

        #endregion

        #region Extensions for long
        /// <summary>
        /// Converts Active Directory style ticks to a nullable DateTime.
        /// </summary>
        /// <param name="ticks">AD ticks value.</param>
        /// <returns>A DateTime corresponding to the ticks, or null for special non-expiring or non-positive values.</returns>
        public static DateTime? ADTicksToDate(this long ticks)
        {

            if (ticks != Extensions.NonExpiringTickValue && ticks > 0)
            {
                DateTime adBeginningOfTimes = new DateTime(1601, 1, 1).Subtract(new TimeSpan(1, 0, 0, 0));
                return adBeginningOfTimes.AddTicks(ticks);
            }

            return null;
        }

        /// <summary>
        /// Converts ticks (relative to 1601-01-01) to a nullable DateTime.
        /// </summary>
        /// <param name="ticks">Tick count to convert.</param>
        /// <returns>A DateTime representing the tick count, or null for special non-expiring or non-positive values.</returns>
        public static DateTime? TicksToDate(this long ticks)
        {
            if (ticks != Extensions.NonExpiringTickValue && ticks > 0)
            {
                var beginningOfTimes = new DateTime(1601, 1, 1).Subtract(new TimeSpan(1, 0, 0, 0));
                return beginningOfTimes.AddTicks(ticks);
            }

            return null;
        }
        #endregion

        #region Extensions for object
        /// <summary>
        /// Converts an object to a <see cref="DateTime"/> and returns its Date component.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>The date portion of the converted DateTime; if conversion fails returns Date of default (1900-01-01).</returns>
        public static DateTime AsDate(this object value)
        {
            return value.AsDateTime().Date;
        }

        /// <summary>
        /// Converts an object to a <see cref="DateTime"/>. If conversion is not possible returns 1900-01-01.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>A DateTime parsed from the object's string representation, or 1900-01-01 if empty or parse fails.</returns>
        public static DateTime AsDateTime(this object value)
        {
            var back = new DateTime(1900, 1, 1);
            var buff = value.AsString();
            if (string.IsNullOrEmpty(buff))
            {
                return new DateTime(1900, 1, 1);
            }

            DateTime.TryParse(buff, out back);
            return back;
        }

        /// <summary>
        /// Converts an object to an integer. Returns 0 if conversion is not possible or input is empty.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>The parsed integer, or 0 if parsing fails.</returns>
        public static int AsInteger(this object value)
        {
            var back = 0;
            var buff = value.AsString();
            if (string.IsNullOrEmpty(buff))
            {
                return 0;
            }

            int.TryParse(buff, out back);
            return back;
        }

        /// <summary>
        /// Converts an object to a string safely, returning an empty string for null.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>The object's string representation or an empty string if null.</returns>
        public static string AsString(this object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }

        /// <summary>
        /// Checks whether an object can be interpreted as a decimal number (Double). Outputs parsed value if true.
        /// </summary>
        /// <param name="o">Object to evaluate.</param>
        /// <param name="Val">Parsed double value when method returns true; Double.NaN otherwise.</param>
        /// <returns>True if <paramref name="o"/> is a decimal-like number; otherwise false.</returns>
        public static bool IsDecimal(this object o, out double Val)
        {
            if (o is Decimal)
            {
                Val = Convert.ToDouble(o);
                return true;
            }
            if (o is Single)
            {
                Val = Convert.ToDouble(o);
                return true;
            }
            if (o is Double)
            {
                Val = Convert.ToDouble(o);
                return true;
            }
            if (Double.TryParse(o.ToString(), out Val))
                return true;
            Val = Double.NaN;
            return false;
        }

        /// <summary>
        /// Checks whether an object should be considered "empty".
        /// </summary>
        /// <param name="obj">Object to check.</param>
        /// <returns>
        /// True when the object represents an empty value (empty string, zero-length collection or array, default value for value types, Guid.Empty, DateTime.MinValue).
        /// False if the object is null or DBNull (these are handled by <see cref="IsNullOrDbNull"/>).
        /// </returns>
        public static bool IsEmptyObject(this object obj)
        {
            if (IsNullOrDbNull(obj))
                return false;

            if (obj is string)
                return ((string)obj).Length == 0;
            if (obj is StringBuilder)
                return ((StringBuilder)obj).Length == 0;
            if (obj is ICollection)
                return ((ICollection)obj).Count == 0;
            if (obj is Array)
                return ((Array)obj).Length == 0;
            if (obj is IList)
                return ((IList)obj).Count == 0;
            if (obj is ValueType)
                return obj == (ValueType)(0) || obj == (ValueType)(-1);
            if (obj is Guid)
                return ((Guid)obj) == Guid.Empty;
            if (obj is DateTime)
                return ((DateTime)obj) == DateTime.MinValue;

            return false;
        }

        /// <summary>
        /// Checks whether an object represents an integer value. Outputs parsed value if true.
        /// </summary>
        /// <param name="o">Object to test.</param>
        /// <param name="Val">Parsed numeric value when true; int.MinValue otherwise.</param>
        /// <returns>True if object can be interpreted as an integer; otherwise false.</returns>
        public static bool IsInteger(this object o, out double Val)
        {
            int aux;
            if (o is Int64)
            {
                Val = Convert.ToInt64(o);
                return true;
            }
            if (o is Int32)
            {
                Val = Convert.ToInt32(o);
                return true;
            }
            if (o is Int16)
            {
                Val = Convert.ToInt16(o);
                return true;
            }
            if (int.TryParse(o.ToString(), out aux))
            {
                Val = aux;
                return true;
            }
            Val = int.MinValue;
            return false;
        }

        /// <summary>
        /// Determines whether an object is null or represents a DBNull value.
        /// </summary>
        /// <param name="obj">Object to test.</param>
        /// <returns>True if <paramref name="obj"/> is null or <see cref="DBNull"/>; otherwise false.</returns>
        public static bool IsNullOrDbNull(this object obj)
        {
            return (obj == null || obj is DBNull);
        }

        /// <summary>
        /// Determines whether an object is null/DBNull or considered empty by <see cref="IsEmptyObject"/>.
        /// </summary>
        /// <param name="obj">Object to test.</param>
        /// <returns>True if the object is null/DBNull or empty; otherwise false.</returns>
        public static bool IsNullOrDDbNullOrEmpty(this object obj)
        {
            return (IsNullOrDbNull(obj) || IsEmptyObject(obj));
        }

        /// <summary>
        /// Checks whether an object is numeric (integer or decimal). Outputs parsed value if true.
        /// </summary>
        /// <param name="o">Object to test.</param>
        /// <param name="Val">Parsed double value when true; Double.NaN otherwise.</param>
        /// <returns>True if the object is numeric; otherwise false.</returns>
        public static bool IsNumeric(this object o, out double Val)
        {
            if (IsInteger(o, out Val))
                return true;
            if (IsDecimal(o, out Val))
                return true;

            Val = double.NaN;
            return false;
        }

        /// <summary>
        /// Serializes an object to an XML file using the project's <c>XmlObjectSerializer</c>.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="fileName">The file path where the XML will be saved.</param>
        /// <param name="root">Optional root element name used by the serializer.</param>
        /// <param name="version">Optional serializer version parameter.</param>
        public static void SerializeToXmlFile(this object obj, string fileName, string root = "Data", int version = 1)
        {
            var xDoc = XmlObjectSerializer.Serialize(obj, version, root);
            xDoc.Save(fileName);
        }

        /// <summary>
        /// Serializes an object to an XML string using the project's <c>XmlObjectSerializer</c>.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="root">Optional root element name used by the serializer.</param>
        /// <param name="version">Optional serializer version parameter.</param>
        /// <returns>An XML string representing the serialized object.</returns>
        public static string SerializeToXmlString(this object obj, string root = "Data", int version = 1)
        {
            TextWriter tw = new StringWriter();
            var xDoc = XmlObjectSerializer.Serialize(obj, version, root);
            xDoc.Save(tw);
            return tw.ToString();
        }

        /// <summary>
        /// Converts an object graph into a Workday-specific XML fragment.
        /// Recursively processes properties, collections and nested objects into XML elements prefixed with "pi:".
        /// </summary>
        /// <param name="obj">The object to convert to Workday XML.</param>
        /// <returns>An XML string fragment representing the object suitable for Workday integrations.</returns>
        public static string ToWorkdayXml(this object obj)
        {
            StringBuilder back = new StringBuilder();
            string thisClassName = obj.GetType().Name;

            back.AppendLine(String.Format("<pi:{0}>", thisClassName));

            foreach (var pi in obj.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    if (pi.Name.Equals("Identifier_Type", StringComparison.CurrentCultureIgnoreCase) || pi.Name.Equals("Identifier_Value", StringComparison.CurrentCultureIgnoreCase) || pi.Name.Equals("Country", StringComparison.CurrentCultureIgnoreCase))
                    {
                        back.AppendLine(String.Format("<pi:{0} pi:PriorValue=''>{1}</pi:{0}>", pi.Name, HttpUtility.HtmlEncode(pi.GetValue(obj, null))));
                    }
                    else if (pi.Name.Equals("First_Address_Line_Data1", StringComparison.CurrentCultureIgnoreCase))
                    {
                        back.AppendLine(String.Format("<pi:First_Address_Line_Data  pi:Label=\"Address Line 1\" pi:Type=\"ADDRESS_LINE_1\">{0}</pi:First_Address_Line_Data>", HttpUtility.HtmlEncode(pi.GetValue(obj, null))));
                    }
                    else if (pi.Name.Equals("First_Address_Line_Data2"))
                    {
                        string temp = (pi.GetValue(obj, null) ?? "").ToString();
                        if (!String.IsNullOrEmpty(temp))
                            back.AppendLine(String.Format("<pi:First_Address_Line_Data  pi:Label=\"Address Line 2\" pi:Type=\"ADDRESS_LINE_2\">{0}</pi:First_Address_Line_Data>", HttpUtility.HtmlEncode(temp)));
                    }
                    else
                    {
                        back.AppendLine(String.Format("<pi:{0}>{1}</pi:{0}>", pi.Name, HttpUtility.HtmlEncode(pi.GetValue(obj, null))));
                    }
                }
                else if (pi.PropertyType == typeof(bool))
                {
                    bool buffBool = (bool)pi.GetValue(obj, null);
                    back.AppendLine(String.Format("<pi:{0}>{1}</pi:{0}>", pi.Name, buffBool ? "true" : "false"));
                }
                else if (pi.PropertyType == typeof(int))
                {
                    int buffInt = (int)pi.GetValue(obj, null);
                    back.AppendLine(String.Format("<pi:{0}>{1}</pi:{0}>", pi.Name, buffInt));
                }
                else if (pi.PropertyType == typeof(float))
                {
                    float buffFloat = (float)pi.GetValue(obj, null);
                    back.AppendLine(String.Format("<pi:{0}>{1}</pi:{0}>", pi.Name, buffFloat.ToString("N2")));
                }
                else if (pi.PropertyType == typeof(DateTime))
                {
                    DateTime buffDateTime = (DateTime)pi.GetValue(obj, null);
                    back.AppendLine(String.Format("<pi:{0}>{1}</pi:{0}>", pi.Name, buffDateTime.ToString("MM/dd/yyyy")));
                }
                else if (typeof(IEnumerable).IsAssignableFrom(pi.PropertyType))
                {
                    object propValue = pi.GetValue(obj, null);
                    var elems = propValue as IList;
                    if (elems != null)
                    {
                        foreach (var item in elems)
                        {
                            back.Append(item.ToWorkdayXml());
                        }
                    }
                }
                else if (!pi.PropertyType.IsPrimitive)
                {
                    back.Append(pi.GetValue(obj, null).ToWorkdayXml());
                }
            }

            back.AppendLine(String.Format("</pi:{0}>", thisClassName));
            return back.ToString();
        }
        #endregion

        #region Extensions for Point
        /// <summary>
        /// Determines whether the provided point is the initial (0,0) point.
        /// </summary>
        /// <param name="p">The point to check.</param>
        /// <returns>True if the point's X and Y are both zero; otherwise false.</returns>
        public static bool IsInitialPoint(this Point p)
        {
            return p.X == 0 && p.Y == 0;
        }

        /// <summary>
        /// Sets the provided point's coordinates to the initial point (0,0).
        /// </summary>
        /// <param name="p">The point to modify.</param>
        public static void SetToInitialPoint(this Point p)
        {
            p.X = 0;
            p.Y = 0;
        }
        #endregion

        #region Extensions for Random
        /// <summary>
        /// Simulates a coin toss using the Random instance.
        /// </summary>
        /// <param name="rng">The random number generator.</param>
        /// <returns>True or false with approximately equal probability.</returns>
        public static bool CoinToss(this Random rng)
        {
            return rng.Next(2) == 0;
        }

        /// <summary>
        /// Returns a randomly selected element from the provided list of items.
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="rng">The random number generator.</param>
        /// <param name="things">Array of candidate items.</param>
        /// <returns>A randomly selected item from the array.</returns>
        public static T OneOf<T>(this Random rng, params T[] things)
        {
            return things[rng.Next(things.Length)];
        }
        #endregion

        #region Extensions for Stream
        /// <summary>
        /// Copies all data from the input stream to the output stream without closing either stream.
        /// </summary>
        /// <param name="input">The input stream to read from.</param>
        /// <param name="output">The output stream to write to.</param>
        public static void CopyStream(this Stream input, Stream output)
        {
            var buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        /// <summary>
        /// Reads the contents of a stream into a byte array and closes the stream.
        /// </summary>
        /// <param name="stream">The stream to read from. Its Length property is used to size the array.</param>
        /// <returns>A byte array containing the stream data.</returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            int int32 = Convert.ToInt32(stream.Length);
            byte[] buffer = new byte[int32 + 1];
            stream.Read(buffer, 0, int32);
            stream.Close();
            return buffer;
        }
        #endregion

        #region Extensions for StringBuilder 
        /// <summary>
        /// Appends the specified string to the StringBuilder only if the provided condition is true.
        /// </summary>
        /// <param name="builder">The StringBuilder instance to append to.</param>
        /// <param name="condition">If true, the value will be appended; otherwise nothing is appended.</param>
        /// <param name="value">The string value to append when the condition is true.</param>
        /// <returns>The same StringBuilder instance (for fluent usage).</returns>
        public static StringBuilder AppendIf(this StringBuilder builder, bool condition, string value)
        {
            if (condition) builder.Append(value);
            return builder;
        }
        #endregion

        #region Extensions for string
        /// <summary>
        /// Appends a single character to the end of the string.
        /// </summary>
        /// <param name="Str">The original string.</param>
        /// <param name="c">The character to append.</param>
        /// <returns>The resulting string after appending the character.</returns>
        public static string AppendChar(this string Str, char c)
        {
            if (String.IsNullOrEmpty(Str))
                return c.ToString();
            return Str.Insert(Str.Length, c.ToString());
        }

        /// <summary>
        /// Converts text values that represent boolean-like states to a boolean.
        /// </summary>
        /// <param name="str">The input string to interpret.</param>
        /// <param name="emptyIsFalse">If true, empty or null string returns false; otherwise an exception is thrown for empty input.</param>
        /// <returns>True when the string matches known "true" values; otherwise false.</returns>
        /// <exception cref="Exception">Thrown when input is empty and emptyIsFalse is false.</exception>
        public static bool AsBoolean(this string str, bool emptyIsFalse = true)
        {
            if (String.IsNullOrEmpty(str))
            {
                if (emptyIsFalse)
                {
                    return false;
                }
                else
                {
                    throw new Exception("Cannot convert empty string to boolean.");
                }
            }

            return str.InStringList(false, "1", "True", "Active", "Yes", "Y", "Enabled", "On", "Ok", "Valid", "Approve", "Affirm", "Agree");
        }

        /// <summary>
        /// Parses a compact ISO-like date/time string in several possible lengths
        /// (e.g. "YYYYMMDD HHmmss.ms", "YYYYMMDD HHmmss", "YYYYMMDD", "YYYYMM", "YYYY").
        /// </summary>
        /// <param name="value">The compact date/time string.</param>
        /// <returns>A DateTime parsed from the input; DateTime.MinValue if parsing fails.</returns>
        public static DateTime AsDateFromISOString(this string value)
        {
            DateTime back = DateTime.MinValue;
            int year, month = 1, day = 1, hour = 0, minute = 0, second = 0, millisecond = 0;

            if (value.Length == 18)
            {// YYYYMMDD HHmmss.ms
                if (int.TryParse(value.Substring(0, 4), out year))//parse YYYY
                    if (int.TryParse(value.Substring(4, 2), out month))//parse MM
                        if (int.TryParse(value.Substring(6, 2), out day))//parse DD
                            if (int.TryParse(value.Substring(9, 2), out hour))//parse HH
                                if (int.TryParse(value.Substring(11, 2), out minute))//parse mm
                                    if (int.TryParse(value.Substring(13, 2), out second))//parse ss
                                        if (int.TryParse(value.Substring(16, 2), out millisecond))//parse ms
                                            back = new DateTime(year, month, day, hour, minute, second, millisecond);

            }
            else
            {
                if (value.Length >= 15)
                {// YYYYMMDD HHmmss
                    if (int.TryParse(value.Substring(0, 4), out year))//parse YYYY
                        if (int.TryParse(value.Substring(4, 2), out month))//parse MM
                            if (int.TryParse(value.Substring(6, 2), out day))//parse DD
                                if (int.TryParse(value.Substring(9, 2), out hour))//parse HH
                                    if (int.TryParse(value.Substring(11, 2), out minute))//parse mm
                                        if (int.TryParse(value.Substring(13, 2), out second))//parse ss
                                            back = new DateTime(year, month, day, hour, minute, second);
                }
                else
                {// YYYYMMDD HHmm
                    if (value.Length >= 13)
                    {
                        if (int.TryParse(value.Substring(0, 4), out year))//parse YYYY
                            if (int.TryParse(value.Substring(4, 2), out month))//parse MM
                                if (int.TryParse(value.Substring(6, 2), out day))//parse DD
                                    if (int.TryParse(value.Substring(9, 2), out hour))//parse HH
                                        if (int.TryParse(value.Substring(11, 2), out minute))//parse mm
                                            back = new DateTime(year, month, day, hour, minute, second);
                    }
                    else
                    {
                        if (value.Length >= 11)
                        {// YYYYMMDD HH
                            if (int.TryParse(value.Substring(0, 4), out year))//parse YYYY
                                if (int.TryParse(value.Substring(4, 2), out month))//parse MM
                                    if (int.TryParse(value.Substring(6, 2), out day))//parse DD
                                        if (int.TryParse(value.Substring(9, 2), out hour))//parse HH
                                            back = new DateTime(year, month, day, hour, minute, second);
                        }
                        else
                        {
                            if (value.Length >= 8)
                            {// YYYYMMDD
                                if (int.TryParse(value.Substring(0, 4), out year))//parse YYYY
                                    if (int.TryParse(value.Substring(4, 2), out month))//parse MM
                                        if (int.TryParse(value.Substring(6, 2), out day))//parse DD
                                            back = new DateTime(year, month, day);
                            }
                            else
                            {
                                if (value.Length >= 6)
                                {// YYYYMM
                                    if (int.TryParse(value.Substring(0, 4), out year))//parse YYYY
                                        if (int.TryParse(value.Substring(4, 2), out month))//parse MM
                                            back = new DateTime(year, month, day);
                                }
                                else
                                {// YYYY
                                    if (int.TryParse(value.Substring(0, 4), out year))//parse YYYY
                                        back = new DateTime(year, month, day);
                                }
                            }
                        }
                    }
                }
            }

            return back;

        }

        /// <summary>
        /// Attempts to parse a US-style date string (M/D/YYYY or M-D-YYYY) into a DateTime.
        /// Accepts variable digit counts for month/day/year (2-digit or 4-digit year).
        /// </summary>
        /// <param name="text">The date string to parse.</param>
        /// <returns>A DateTime if parsing succeeds; otherwise null.</returns>
        public static DateTime? AsUSDate(this string text)
        {
            DateTime? Back = null;
            DateTime Helper;
            CultureInfo enUS = new CultureInfo("en-US");

            string separator;
            if (text.Contains("/"))
                separator = "/";
            else if (text.Contains("-"))
                separator = "-";
            else
                return Back;


            try
            {
                string[] Data = text.Split(separator[0]);
                string M = "", D = "", Y = "";

                int monthDigits = Data[0].Length;
                int dayDigits = Data[1].Length;
                int yearDigits = Data[2].Length;

                if (monthDigits == 2)
                    M = "MM";
                else if (monthDigits == 1)
                    M = "M";

                if (dayDigits == 2)
                    D = "dd";
                else if (dayDigits == 1)
                    D = "d";

                if (yearDigits == 2)
                    Y = "yy";
                else if (yearDigits == 4)
                    Y = "yyyy";

                if (!DateTime.TryParseExact(text, String.Format("{1}{0}{2}{0}{3}", separator, M, D, Y), enUS, DateTimeStyles.None, out Helper))
                {
                    Back = null;
                }
                else
                {
                    Back = Helper;
                }
            }
            catch (Exception)
            {
                Back = null;
            }
            return Back;
        }

        /// <summary>
        /// Decodes a Base64-encoded string using UTF-8 encoding.
        /// </summary>
        /// <param name="base64EncodedData">The Base64-encoded input string.</param>
        /// <returns>The decoded string.</returns>
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Encodes a plain text string into Base64 using UTF-8 encoding.
        /// </summary>
        /// <param name="plainText">The input plain text string.</param>
        /// <returns>The Base64-encoded representation of the input.</returns>
        public static string Base64Encode(this string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Splits a string by a character separator and returns a trimmed list of the parts.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="separator">The separator character (default ',').</param>
        /// <returns>A list of trimmed substrings. Empty list when input is null or empty.</returns>
        public static List<string> CharSeparatedStringAsList(this string str, char separator = ',')
        {
            List<string> back = new List<string>();

            if (!String.IsNullOrEmpty(str))
            {
                foreach (string x in str.Split(new char[] { separator }))
                {
                    back.Add(x.Trim());
                }
            }

            return back;
        }

        /// <summary>
        /// Determines whether a string contains another string using the specified StringComparison.
        /// </summary>
        /// <param name="source">The source string to search within.</param>
        /// <param name="toCheck">The substring to search for.</param>
        /// <param name="comp">The StringComparison to use (e.g., OrdinalIgnoreCase).</param>
        /// <returns>True if the substring is found; otherwise false.</returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Converts simple HTML content into plain text by replacing common entities and removing tags.
        /// Intended for simple markup, not for full HTML parsing.
        /// </summary>
        /// <param name="value">The input HTML string.</param>
        /// <returns>A plain text representation with tags removed and basic entities decoded.</returns>
        public static string ConvertHtmlToPlainText(this string value)
        {
            var tagStripper = new Regex(@"<(.|\n)+?>");
            var lineBreakStripper = new Regex(@"<br(.)*?>");
            var trimLeadingSpaces = new Regex(@" *\n *");

            // safety check
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var plainText = value;

            // replace all &gt; with >
            plainText = plainText.Replace("&gt;", ">");

            // replace all &lt; with <
            plainText = plainText.Replace("&lt;", "<");

            // replace all &quot; with "
            plainText = plainText.Replace("&quot;", "\"");

            // replace all "&nbsp;" with " "
            plainText = plainText.Replace("&nbsp;", " ");

            // check to see if there is any html left in here. if there are no tags, return early.
            if (tagStripper.IsMatch(plainText) == false)
            {
                return plainText;
            }

            // first strip out the existing linebreaks. HTML ignores them when rendering
            plainText = plainText.Replace("\r", string.Empty);
            plainText = plainText.Replace("\n", string.Empty);

            // replace all "</p>" instances with line breaks
            plainText = plainText.Replace("</p>", "\n");

            // replace all "<br>"
            plainText = lineBreakStripper.Replace(plainText, "\n");

            // strip out remaining tags
            plainText = tagStripper.Replace(plainText, string.Empty);

            // now clean out all spaces that lead off a new line
            plainText = trimLeadingSpaces.Replace(plainText, "\n");

            return plainText;
        }

        /// <summary>
        /// Converts plain text into a simple HTML representation by encoding "<", ">", and quotes
        /// and replacing line breaks with <br />.
        /// </summary>
        /// <param name="value">The input plain text.</param>
        /// <returns>An HTML-safe string with line breaks converted to <br /> tags.</returns>
        public static string ConvertPlainTextToHtml(this string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return
            value.Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("\r\n", "<br />")
            .Replace("\n", "<br />")
            .Replace("\r", "<br />");
        }

        /// <summary>
        /// Copies a file from source to target while permitting reading of the source file even if locked for writing.
        /// </summary>
        /// <param name="sourceFile">Path to the source file.</param>
        /// <param name="targetFile">Path to the destination file.</param>
        public static void CopyNoLock(this string sourceFile, string targetFile)
        {
            using (var inputFile = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var outputFile = new FileStream(targetFile, FileMode.Create))
                {
                    var buffer = new byte[0x10000];
                    int bytes;

                    while ((bytes = inputFile.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputFile.Write(buffer, 0, bytes);
                    }
                }
            }


        }

        /// <summary>
        /// Creates a DataTable whose columns correspond to the public properties of type T.
        /// </summary>
        /// <typeparam name="T">The entity type to reflect properties from.</typeparam>
        /// <param name="tableName">Optional table name; if empty the type's name is used.</param>
        /// <returns>A populated DataTable schema with columns matching the type's properties.</returns>
        public static DataTable CreateTable<T>(string tableName = "")
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(String.IsNullOrEmpty(tableName) ? entityType.Name : tableName);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            return table;
        }

        /// <summary>
        /// Deserializes an XML file into an object using the XmlObjectDeserializer helper.
        /// </summary>
        /// <param name="fileName">Path to the XML file to load.</param>
        /// <param name="ver">Optional deserializer version number.</param>
        /// <param name="typeConverter">Optional type converter used during deserialization.</param>
        /// <returns>The deserialized object instance.</returns>
        public static object DeserializeFromXmlFile(this string fileName, int ver = 1)
        {
            if (File.Exists(fileName) == false)
                throw new FileNotFoundException("The specified XML file was not found.", fileName);

            return XmlObjectDeserializer.Deserialize(fileName.ReadFromTextFile(), ver, null);
        }

        /// <summary>
        /// Deserializes an XML string into an object using the XmlObjectDeserializer helper.
        /// </summary>
        /// <param name="xmlString">The XML string to deserialize.</param>
        /// <param name="ver">Optional deserializer version number.</param>
        /// <param name="typeConverter">Optional type converter used during deserialization.</param>
        /// <returns>The deserialized object instance.</returns>
        public static object DeserializeFromXmlString(this string xmlString, int ver = 1)
        {
            return XmlObjectDeserializer.Deserialize(xmlString, ver, null);
        }

        /// <summary>
        /// Computes the Levenshtein edit distance between two strings.
        /// </summary>
        /// <param name="s">The first string.</param>
        /// <param name="t">The second string.</param>
        /// <returns>The number of insertions, deletions, or substitutions required to convert s to t.</returns>
        public static int EditDistance(this string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
                return m;

            if (m == 0)
                return n;

            // Step 2
            for (int i = 0; i <= n; i++)
                d[i, 0] = i;

            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min
                    (
                        Math.Min
                        (
                            d[i - 1, j] + 1, d[i, j - 1] + 1
                        ),
                        d[i - 1, j - 1] + cost
                    );
                }
            }
            // Step 7
            return d[n, m];
        }

        /// <summary>
        /// Ensures that the given string starts and ends with the specified delimiter.
        /// </summary>
        /// <param name="str">The input string to enclose.</param>
        /// <param name="delimiter">The delimiter to apply; if null will be treated as empty string.</param>
        /// <returns>The input string guaranteed to start and end with the delimiter.</returns>
        public static string EncloseString(this string str, string delimiter)
        {
            delimiter = delimiter ?? "";

            if (String.IsNullOrEmpty(str))
                return delimiter + delimiter;

            if (!str.StartsWith(delimiter))
                str = delimiter + str;

            if (!str.EndsWith(delimiter))
                str = str + delimiter;

            return str;
        }

        /// <summary>
        /// Prepares a string so it is safe for inclusion as a CSV field: quotes inner quotes and
        /// quotes the field if it contains commas.
        /// </summary>
        /// <param name="str">The input string to make CSV-safe.</param>
        /// <returns>A CSV-safe representation of the string.</returns>
        public static string EnsureCsvField(this string str)
        {
            if (str.Contains("\""))
                str = "\"" + str.Replace("\"", "\"\"") + "\"";

            if (str.Contains(","))
                str = str.EnsureQuotedString();

            return str;
        }

        /// <summary>
        /// Ensures a string is wrapped with double quotes. Returns empty quoted string for null/empty input.
        /// </summary>
        /// <param name="str">The string to quote.</param>
        /// <returns>The quoted string (starts and ends with ").</returns>
        public static string EnsureQuotedString(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return "\"\"";
            if (!str.StartsWith("\""))
                str = "\"" + str;
            if (!str.EndsWith("\""))
                str = str + "\"";
            return str;
        }

        /// <summary>
        /// Ensures a string is wrapped with single quotes. Returns empty single-quoted string for null/empty input.
        /// </summary>
        /// <param name="str">The string to single-quote.</param>
        /// <returns>The single-quoted string (starts and ends with ').</returns>
        public static string EnsureSingleQuotedString(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return "''";
            if (!str.StartsWith("'"))
                str = "'" + str;
            if (!str.EndsWith("'"))
                str = str + "'";
            return str;
        }

        /// <summary>
        /// Removes invalid file/path characters from the provided file name.
        /// </summary>
        /// <param name="fileName">Input file name or path fragment.</param>
        /// <returns>A sanitized file name with invalid characters removed.</returns>
        public static string EnsureValidFileName(this string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                return "";

            //fileName = fileName.Replace("/", "-");
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            fileName = r.Replace(fileName, "");
            return fileName;
        }

        /// <summary>
        /// Replaces characters that are not valid in XML text with corresponding XML entities.
        /// </summary>
        /// <param name="s">Input text to encode for XML.</param>
        /// <returns>XML-encoded string safe to embed in XML text nodes.</returns>
        public static string EnsureValidXmlText(this string s)
        {
            return (s ?? "")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("&", "&amp;");
        }

        /// <summary>
        /// Shortcut for string.Format: formats the string using the supplied arguments.
        /// </summary>
        /// <param name="s">The composite format string.</param>
        /// <param name="args">Arguments referenced by the format items in the format string.</param>
        /// <returns>The formatted string.</returns>
        public static string F(this string s, params object[] args)
        {
            return string.Format(s, args);
        }

        /// <summary>
        /// Returns the first N characters of a string or the entire string if shorter.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="length">The maximum number of characters to return.</param>
        /// <returns>The substring of up to the specified length from the start of the string.</returns>
        public static string FirstChars(this string str, int length)
        {
            if (!String.IsNullOrEmpty(str))
                return str.Length > length ? str.Substring(0, length) : str;
            return str;

        }

        /// <summary>
        /// Returns the first character of a string (or empty/null behavior propagated to FirstChars).
        /// </summary>
        /// <param name="source">The input string.</param>
        /// <returns>The first character as a string, or the original value's behavior per FirstChars.</returns>
        public static string FirstChar(this string source)
        {
            return source.FirstChars(1);
        }

        /// <summary>
        /// Returns the first non-empty word from a string or the original string if no space found.
        /// </summary>
        /// <param name="str">The input string containing words separated by spaces.</param>
        /// <returns>The first non-empty word found; otherwise the input string.</returns>
        public static string FirstWord(this string str)
        {
            if (!str.IsEmpty() && str.Contains(' '))
            {
                string[] arr = str.Split(' ');
                foreach (string s in arr)
                {
                    if (!s.IsEmpty())
                        return s;
                }
            }

            return str;
        }

        /// <summary>
        /// Formats phone numbers consisting of numeric characters into common U.S. formats:
        /// (xxx) xxx-xxxx or xxx-xxxx. If input cannot be converted, returns original.
        /// </summary>
        /// <param name="str">The input phone string possibly containing non-numeric characters.</param>
        /// <returns>A formatted phone string or the original input if not enough digits.</returns>
        public static string FormatAsPhone(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;
            string onlyNumbers = str.RemoveNonNumeric();

            if (onlyNumbers.Length > 10)//keep only last 10 digits
                onlyNumbers = onlyNumbers.Substring(onlyNumbers.Length - 10);

            if (onlyNumbers.Length == 10)
                return String.Format("({0}) {1}-{2}", onlyNumbers.Substring(0, 3), onlyNumbers.Substring(3, 3), onlyNumbers.Substring(6, 4));
            else if (onlyNumbers.Length == 7)
                return String.Format("{0}-{1}", onlyNumbers.Substring(0, 3), onlyNumbers.Substring(3, 4));
            else
                return str;

        }

        /// <summary>
        /// Formats a string as an SSN (xxx-xx-xxxx) using SafeSubstring to avoid exceptions on short strings.
        /// </summary>
        /// <param name="str">The input string containing SSN digits.</param>
        /// <returns>Formatted SSN string or "--" when input is null/empty.</returns>
        public static string FormatAsSsn(this string str)
        {
            string back = "--";

            if (!string.IsNullOrEmpty(str))
            {
                back = string.Format("{0}-{1}-{2}", str.SafeSubstring(0, 3), str.SafeSubstring(3, 2), str.SafeSubstring(5));
            }

            return back;
        }

        /// <summary>
        /// Converts a hexadecimal string to its decimal integer value.
        /// </summary>
        /// <param name="str">The hex string to convert (without 0x prefix).</param>
        /// <returns>The base-10 integer equivalent of the hex string (0 when input is null/empty).</returns>
        public static int FromHexStringToDecInt(this string str)
        {
            int pot = 0, back = 0, b = 10;

            if (!string.IsNullOrEmpty(str))
            {
                for (var i = str.Length - 1; i >= 0; i--)
                {
                    var buff = (int)Math.Pow(b, pot);
                    back += buff * str[i].FromHexCharToDecInt();
                    pot++;
                }
            }

            return back;
        }

        /// <summary>
        /// Capitalizes the first letter of the input string and makes remaining characters lower-case.
        /// </summary>
        /// <param name="Word">The word to convert.</param>
        /// <returns>The string starting with uppercase followed by lowercase characters.</returns>
        public static string StartWithUpperCase(this string Word)
        {
            if (String.IsNullOrEmpty(Word))
                return Word;
            return Word.Substring(0, 1).ToUpper() + (Word.Length > 1 ? Word.Substring(1).ToLower() : "");
        }

        /// <summary>
        /// Extracts only lower-case letters from the input string and optionally limits the returned length.
        /// </summary>
        /// <param name="Word">The input string to inspect.</param>
        /// <param name="MaxLength">Maximum number of characters to return; -1 to return all found.</param>
        /// <returns>A string made only of the lower-case letters from the input, optionally truncated.</returns>
        public static string GetLowerCasedLetters(this string Word, int MaxLength = -1)
        {
            string back;
            back = String.Concat(Word.Select(X => char.IsLower(X) ? X.ToString() : ""));
            if (MaxLength == -1)
                return back;
            return back.Substring(0, Math.Min(MaxLength, back.Length));
        }

        /// <summary>
        /// Extracts only uppercase letters from the input string and optionally restricts length.
        /// </summary>
        /// <param name="Word">The input string.</param>
        /// <param name="MaxLength">Maximum number of characters to return; -1 returns all found.</param>
        /// <returns>A string of uppercase letters optionally truncated to MaxLength.</returns>
        public static string GetUpperCasedLetters(this string Word, int MaxLength = -1)
        {
            string back;
            back = String.Concat(Word.Select(X => char.IsUpper(X) ? X.ToString() : ""));
            if (MaxLength == -1)
                return back;
            return back.Substring(0, Math.Min(MaxLength, back.Length));
        }

        /// <summary>
        /// Prepends indentation (tabs or spaces) to the input string.
        /// </summary>
        /// <param name="source">The string to indent.</param>
        /// <param name="indent">Number of indentation units to add.</param>
        /// <param name="useSpaces">If greater than 0, uses that many spaces per indent; otherwise a tab is used.</param>
        /// <returns>The indented string.</returns>
        public static string Indent(this string source, int indent = 1, int useSpaces = 0)
        {
            string back = String.IsNullOrEmpty(source) ? "" : source;
            string spaces = "";

            if (useSpaces > 0)
            {
                for (int i = 0; i < useSpaces; i++)
                {
                    spaces += " ";
                }
            }
            else
            {
                spaces = "\t";
            }

            for (int i = 0; i < indent; i++)
            {
                back = spaces + back;
            }
            return back;
        }

        /// <summary>
        /// Checks whether txtSource contains txtSearch using case-insensitive comparison.
        /// </summary>
        /// <param name="txtSource">The source text.</param>
        /// <param name="txtSearch">The text to search for.</param>
        /// <returns>True if found ignoring case; otherwise false.</returns>
        public static bool InsensitiveCaseContains(this string txtSource, string txtSearch)
        {
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(txtSource, txtSearch, CompareOptions.IgnoreCase) >= 0;
        }

        /// <summary>
        /// Checks whether txtSource contains txtSearch using case-insensitive comparison (alternate name).
        /// </summary>
        /// <param name="txtSource">The source text.</param>
        /// <param name="txtSearch">The text to search for.</param>
        /// <returns>True if found ignoring case; otherwise false.</returns>
        public static bool InsensitiveStringContains(this string txtSource, string txtSearch)
        {
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(txtSource, txtSearch, CompareOptions.IgnoreCase)
            >= 0;
        }

        /// <summary>
        /// Determines if the source string matches any string in the provided list.
        /// </summary>
        /// <param name="source">The source string to test (not null).</param>
        /// <param name="caseSensitive">If true, comparison is case-sensitive; otherwise case-insensitive.</param>
        /// <param name="list">The array of candidate strings.</param>
        /// <returns>True if a match is found; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
        public static bool InStringList(this string source, bool caseSensitive, params string[] list)
        {
            if (null == source) throw new ArgumentNullException("source is null");
            foreach (string s in list)
            {
                if (caseSensitive)
                {
                    if (source.Equals(s))
                        return true;
                }
                else
                {
                    if (source.Equals(s, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the string contains any non-alphanumeric characters (including punctuation).
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>True if the string contains non-alphanumeric characters; otherwise false.</returns>
        public static bool IsAlphaNumeric(this string str)
        {
            Regex alphaNumericFormat = new Regex(@"[^a-zA-Z0-9\s]");
            return alphaNumericFormat.IsMatch(str);
        }

        /// <summary>
        /// Returns whether a string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="str">The input string to test.</param>
        /// <returns>True if null, empty, or whitespace; otherwise false.</returns>
        public static bool IsEmpty(this string str)
        {
            return String.IsNullOrEmpty(str) || String.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Validates a string to determine whether it represents an English (MM/dd/YYYY) date using the given separator.
        /// </summary>
        /// <param name="str">The date string to validate.</param>
        /// <param name="separator">The separator character (default '/').</param>
        /// <returns>True if the string represents a valid date in English format; otherwise false.</returns>
        public static bool IsEnglishDate(this string str, char separator = '/')
        {
            // MM/dd/YYYY
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            var rawData = str.Split(separator);
            int year, month, day;

            if (rawData.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(rawData[2], out year) || !int.TryParse(rawData[0], out month)
                || !int.TryParse(rawData[1], out day))
            {
                return false;
            }

            if (year <= 0 || month <= 0 || day <= 0 || month > 12)
            {
                return false;
            }

            if (day > DateTime.DaysInMonth(year, month))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the string represents an integer number (optional leading minus).
        /// </summary>
        /// <param name="s">Input string.</param>
        /// <returns>True if the string is an integer; otherwise false.</returns>
        public static bool IsInteger(this string s)
        {
            Regex regularExpression = new Regex("^-[0-9]+$|^[0-9]+$");
            return regularExpression.Match(s).Success;
        }

        /// <summary>
        /// Validates whether a string is in IPv4 dotted decimal format.
        /// </summary>
        /// <param name="str">The IP address string to validate.</param>
        /// <returns>True if a valid IPv4 address; otherwise false.</returns>
        public static bool IsIPAddress(this string str)
        {
            Regex validIpAddress = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            return validIpAddress.IsMatch(str);
        }

        /// <summary>
        /// Validates whether a string appears to be an email address using a regex pattern.
        /// </summary>
        /// <param name="str">The email string to validate.</param>
        /// <returns>True if the string matches the email pattern; otherwise false.</returns>
        public static bool IsMailFormat(this string str)
        {
            var emailFormat =
            new Regex(
            @"^(?:[a-zA-Z0-9_'^&amp;/+-])+(?:\.(?:[a-zA-Z0-9_'^&amp;/+-])+)*@(?:(?:\[?(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))\.){3}(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\]?)|(?:[a-zA-Z0-9-]+\.)+(?:[a-zA-Z]){2,}\.?)$");

            return emailFormat.IsMatch(str);
        }

        /// <summary>
        /// Checks whether a string is null, empty or blank (white-space).
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>True when null, empty or whitespace; otherwise false.</returns>
        public static bool IsNullEmptyOrBlank(this string str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Determines if a string matches a numeric currency-like pattern (simple regex).
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns>True if the string matches the numeric pattern; otherwise false.</returns>
        public static bool IsNumeric(this string str)
        {
            Regex isNumeric = new Regex(@"^(\$|)([1-9]\d{0,2}(\,\d{3})*|([1-9]\d*))(\.\d{2})?");
            return isNumeric.IsMatch(str);
        }

        /// <summary>
        /// Tests whether a string contains only numeric characters (0-9).
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns>True if only numeric characters are present; otherwise false.</returns>
        public static bool IsOnlyNumbers(this string str)
        {
            Regex onlyIncludeDigits = new Regex("[^0-9]");
            return onlyIncludeDigits.IsMatch(str);
        }

        /// <summary>
        /// Tests whether a string contains only alphabetic characters (A-Z, a-z).
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns>True if only alphabetic characters are present; otherwise false.</returns>
        public static bool IsOnlyAlphabet(this string str)
        {
            Regex onlyIncludeChars = new Regex("[^a-zA-Z]");
            return onlyIncludeChars.IsMatch(str);
        }

        /// <summary>
        /// Validates whether a string represents a day-first date format (dd/MM/yyyy) using the provided separator.
        /// </summary>
        /// <param name="str">The date string to validate.</param>
        /// <param name="separator">The separator character used in the date (e.g., '/').</param>
        /// <returns>True if the string is a valid Spanish-style date; otherwise false.</returns>
        public static bool IsSpanishDate(this string str, char separator)
        {
            // dd/MM/YYYY
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            var rawData = str.Split(separator);
            int year, month, day;

            if (rawData.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(rawData[2], out year) || !int.TryParse(rawData[1], out month)
                || !int.TryParse(rawData[0], out day))
            {
                return false;
            }

            if (year <= 0 || month <= 0 || day <= 0 || month > 12)
            {
                return false;
            }

            if (day > DateTime.DaysInMonth(year, month))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the leftmost substring of the specified length.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="count">Number of characters to return from the left.</param>
        /// <returns>The substring from position 0 of length count.</returns>
        public static string Left(this string s, int count)
        {
            return s.Substring(0, count);
        }

        /// <summary>
        /// Returns a substring starting at the specified index with the specified length.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="index">Zero-based starting index.</param>
        /// <param name="count">Number of characters to include.</param>
        /// <returns>The requested substring.</returns>
        public static string Mid(this string s, int index, int count)
        {
            return s.Substring(index, count);
        }

        /// <summary>
        /// Reads the entire contents of a text file and returns it as a string.
        /// </summary>
        /// <param name="file">The file path to read.</param>
        /// <returns>The file contents or an empty string if the file does not exist.</returns>
        public static string ReadFromTextFile(this string file)
        {
            string back = "";
            if (File.Exists(file))
            {
                using (StreamReader rdr = new StreamReader(file))
                {
                    back = rdr.ReadToEnd();
                }
            }
            return back;
        }

        /// <summary>
        /// Removes all non-numeric characters from the input string and returns just the digits.
        /// </summary>
        /// <param name="value">The string to process.</param>
        /// <returns>A string containing only the numeric characters found in the input.</returns>
        public static string RemoveNonNumeric(this string value)
        {
            string back = "";
            List<char> digits = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            if (!String.IsNullOrEmpty(value))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (digits.Contains(value[i]))
                        back += value[i].ToString();
                }
            }
            return back;
        }

        /// <summary>
        /// Reads a file, replaces occurrences of an original string with a new string, and overwrites the file.
        /// </summary>
        /// <param name="fileName">The path of the file to update.</param>
        /// <param name="originalString">The string to replace.</param>
        /// <param name="newString">The string to use as replacement.</param>
        public static void ReplaceStringInFile(this string fileName, string originalString, string newString)
        {
            if (File.Exists(fileName))
            {
                var fileContents = File.ReadAllText(fileName);

                fileContents = fileContents.Replace(originalString, newString);

                File.WriteAllText(fileName, fileContents);
            }
        }

        /// <summary>
        /// Returns the rightmost substring of the specified length.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="count">Number of characters to return from the right.</param>
        /// <returns>The substring from the end of the string of length count.</returns>
        public static string Right(this string s, int count)
        {
            return s.Substring(s.Length - count, count);
        }

        /// <summary>
        /// Safely returns a substring starting at startIndex with an optional length parameter.
        /// Prevents exceptions for out-of-range startIndex and handles negative or default length values.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="startIndex">Zero-based starting index for the substring.</param>
        /// <param name="lenght">Optional length of the substring; -1 returns the remainder of the string.</param>
        /// <returns>The requested substring or an empty string if the parameters are invalid.</returns>
        public static string SafeSubstring(this string str, int startIndex, int lenght = -1)
        {
            if (String.IsNullOrEmpty(str) || str.Length <= startIndex || lenght == 0)
            {
                return String.Empty;
            }
            else if (lenght == -1)
            {
                return str.Substring(startIndex);
            }
            else
            {
                if (str.Length - (startIndex) <= lenght)
                    return str.Substring(startIndex);
                else
                    return str.Substring(startIndex, lenght);
            }
        }

        /// <summary>
        /// Returns a trimmed version of the input string, safe against null values.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The trimmed string or empty string when input is null.</returns>
        public static string SafeTrim(this string str)
        {
            return (str ?? "").Trim();
        }

        /// <summary>
        /// Converts a hex HTML color code (e.g., "#FF0000") to a System.Drawing.Color.
        /// </summary>
        /// <param name="htmlColor">Hex or named HTML color string.</param>
        /// <returns>The corresponding Color instance.</returns>
        public static Color StringToColor(this string htmlColor)
        {
            return ColorTranslator.FromHtml(htmlColor);
        }

        /// <summary>
        /// Attempts to parse the input string to an integer returning 0 if parse fails.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>The parsed integer or 0 on failure.</returns>
        public static int ToInteger(this string s)
        {
            int integerValue = 0;
            int.TryParse(s, out integerValue);
            return integerValue;
        }

        /// <summary>
        /// Converts the input string to Title Case using the current culture.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The string converted to Title Case; returns original when null/empty.</returns>
        public static string ToTitleCase(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Removes the last character from a string, if present.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The string without the last character; returns original when null/empty.</returns>
        public static string TruncateLastChar(this string str)
        {
            if (!String.IsNullOrEmpty(str))
                return str.Substring(0, str.Length - 1);
            return str;
        }

        /// <summary>
        /// Truncates the last N characters from a string in a safe manner.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="charNumber">Number of characters to remove from the end (default 1).</param>
        /// <returns>The truncated string, or empty if length less than charNumber.</returns>
        public static string TruncateLastChars(this string str, int charNumber = 1)
        {
            var back = string.Empty;

            if (charNumber < 0 || string.IsNullOrEmpty(str))
            {
                back = str;
            }
            else
            {
                back = str.Length >= charNumber ? str.Substring(0, str.Length - charNumber) : string.Empty;
            }

            return back;
        }

        /// <summary>
        /// Returns the substring between the first occurrence of startStr and the first occurrence of endStr.
        /// Supports case-insensitive operation and selecting nth instance of the start delimiter.
        /// </summary>
        /// <param name="str">The input string to search.</param>
        /// <param name="startStr">The starting delimiter string.</param>
        /// <param name="endStr">The ending delimiter string.</param>
        /// <param name="caseSensitive">If false, comparison will be case-insensitive.</param>
        /// <param name="instance">Which instance of the start delimiter to use (1-based).</param>
        /// <returns>The substring found between the start and end delimiters.</returns>
        /// <exception cref="Exception">Thrown when delimiters are not found or are in invalid order.</exception>
        public static string ValueBetween(this string str, string startStr, string endStr, bool caseSensitive = false, int instance = 1)
        {
            string back = "";
            string backup = str;
            if (!caseSensitive)
            {
                str = str.ToUpper();
                startStr = startStr.ToUpper();
                endStr = endStr.ToUpper();
            }

            if (!str.Contains(startStr) || !str.Contains(endStr))
            {
                throw new Exception("Did not find starting or ending delimeter.");
            }

            if (str.IndexOf(startStr, StringComparison.Ordinal) > str.IndexOf(endStr, StringComparison.Ordinal))
            {
                throw new Exception("Position of start delimeter is after an ending delimeter.");
            }

            while (instance > 0)
            {
                instance--;
                if (str.IndexOf(startStr, StringComparison.Ordinal) < 0)
                    throw new Exception("Instance number: " + instance + ", was not found on original string.");

                backup = backup.Substring(str.IndexOf(startStr, StringComparison.Ordinal) + 1);
                str = str.Substring(str.IndexOf(startStr, StringComparison.Ordinal) + 1);
            }

            back = backup.Substring(0, str.IndexOf(endStr, StringComparison.Ordinal));

            return back;
        }

        /// <summary>
        /// Counts the number of words in the string based on spaces and line breaks.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The number of words found; returns 0 for null/empty input.</returns>
        public static int WordCount(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return 0;
            return str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        /// <summary>
        /// Returns the nth word (1-based) from the string.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="wordNumber">The 1-based index of the word to return.</param>
        /// <returns>The word at the requested position or an empty string if not available.</returns>
        public static string GetWordNumber(this string str, int wordNumber)
        {
            if (String.IsNullOrEmpty(str))
                return "";

            string[] words = str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < wordNumber)
                return "";

            return words[wordNumber - 1];
        }

        /// <summary>
        /// Appends a line of text to the specified file. Creates file if it doesn't exist.
        /// </summary>
        /// <param name="text">The line of text to write.</param>
        /// <param name="file">The target file path.</param>
        /// <param name="append">Whether to append to the file (true) or overwrite (false).</param>
        public static void WriteLineToTextFile(this string text, string file, bool append = true)
        {
            using (StreamWriter writer = new StreamWriter(file, append))
            {
                writer.WriteLine(text);
            }
        }

        /// <summary>
        /// Writes text to the specified file. Creates file if it doesn't exist.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="file">The target file path.</param>
        /// <param name="append">Whether to append to the file (true) or overwrite (false).</param>
        public static void WriteToTextFile(this string text, string file, bool append = true)
        {
            using (StreamWriter writer = new StreamWriter(file, append))
            {
                writer.Write(text);
            }
        }
        #endregion

        #region Extensions for generic type <T>
        /// <summary>
        /// Creates an HTML table string from an object using the specified property names as columns.
        /// </summary>
        /// <typeparam name="T">The type of the object whose properties will be rendered.</typeparam>
        /// <param name="obj">The object instance from which to read property values. If null returns an empty string.</param>
        /// <param name="fields">An array of property names to include as rows in the generated HTML table. Only existing properties on type T are included.</param>
        /// <returns>
        /// A string containing an HTML table representation of the requested properties and their values.
        /// Returns an empty string if the object is null or no fields are provided.
        /// Note: numeric/date formatting is applied for float/double, bool and DateTime as implemented in the method.
        /// </returns>
        public static string GetPropertiesAsHtmlTableString<T>(this T obj, params string[] fields)
        {
            if (obj == null || fields == null || fields.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var entityType = typeof(T);
            var properties = TypeDescriptor.GetProperties(entityType);

            sb.AppendLine("<table style=\"border: 1px black solid;\" border=\"1px\">");
            foreach (var field in fields)
            {
                var prop = entityType.GetProperty(field);
                if (prop != null)
                {
                    var bufferRawValue = prop.GetValue(obj, null);
                    string formattedValueBuffer;
                    sb.AppendLine("<tr style=\"border: 1px black solid; \">");
                    sb.AppendLine(
                    string.Format(
                    "<td style=\"border: 1px black solid; font-weight:bold; text-align: right;\">{0}: </td>",
                    field));

                    if (prop.PropertyType == typeof(float) || prop.PropertyType == typeof(double))
                    {
                        formattedValueBuffer = ((float)bufferRawValue).ToString("f2");
                    }
                    else if (prop.PropertyType == typeof(bool))
                    {
                        formattedValueBuffer = (bool)bufferRawValue ? "True" : "False";
                    }
                    else if (prop.PropertyType == typeof(DateTime))
                    {
                        formattedValueBuffer = ((DateTime)bufferRawValue).ToString("MM/dd/yyyy");
                    }
                    else
                    {
                        formattedValueBuffer = bufferRawValue == null ? string.Empty : bufferRawValue.ToString();
                    }

                    sb.AppendLine(
                    string.Format(
                    "<td style=\"border: 1px black solid; text-align: left;\">{0}</td>",
                    formattedValueBuffer));
                    sb.AppendLine("</tr>");
                }
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the source value is present in the provided list of values.
        /// </summary>
        /// <typeparam name="T">The type of the source and the list elements.</typeparam>
        /// <param name="source">The value to test for membership in the list. Must not be null.</param>
        /// <param name="list">A variadic list of values to compare against the source.</param>
        /// <returns>True if the source equals any element in the list; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="source"/> is null.</exception>
        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source)
                throw new ArgumentNullException("source is null");
            return list.Contains(source);
        }
        #endregion

        #region Extensions for XmlWriter
        /// <summary>
        /// Writes an end element to the XML writer to close the current node.
        /// </summary>
        /// <param name="xw">The XmlWriter instance to operate on. Must not be null.</param>
        public static void CloseNode(this XmlWriter xw)
        {
            xw.WriteEndElement();
        }

        /// <summary>
        /// Opens an XML element with the specified name and optionally writes a string value, then optionally closes it.
        /// </summary>
        /// <param name="xw">The XmlWriter to write to.</param>
        /// <param name="autoClose">If true the method writes the end element immediately after writing the value.</param>
        /// <param name="name">The name of the element to open.</param>
        /// <param name="value">An optional value to write as the element's inner text. If null or empty nothing is written.</param>
        public static void OpenNode(this XmlWriter xw, bool autoClose, string name, object value = null)
        {
            xw.WriteStartElement(name);

            if (value != null && !String.IsNullOrEmpty(value.ToString()))
                xw.WriteString(value.ToString());

            if (autoClose)
                xw.WriteEndElement();
        }

        /// <summary>
        /// Opens an XML element with the specified name, writes attributes (if provided), writes an optional value, and optionally closes the element.
        /// </summary>
        /// <param name="xw">The XmlWriter to write to.</param>
        /// <param name="autoClose">If true the method writes the end element immediately after writing attributes and value.</param>
        /// <param name="name">The name of the element to open.</param>
        /// <param name="value">An optional value to write as the element's inner text. If null or empty nothing is written.</param>
        /// <param name="attributes">A sequence of attribute name/value pairs. The method processes the array as alternating attribute name and value;
        /// if an odd number of strings is supplied the last attribute will be written with an empty value.</param>
        public static void OpenNode(this XmlWriter xw, bool autoClose, string name, object value, params string[] attributes)
        {
            xw.WriteStartElement(name);
            if (attributes != null && attributes.Length > 0)
            {
                string attName = String.Empty;
                foreach (string att in attributes)
                {
                    if (String.IsNullOrEmpty(attName))
                    {
                        attName = att;
                    }
                    else
                    {
                        xw.WriteAttributeString(attName, att);
                        attName = String.Empty;
                    }
                }

                if (!String.IsNullOrEmpty(attName))
                    xw.WriteAttributeString(attName, "");
            }

            if (value != null && !String.IsNullOrEmpty(value.ToString()))
                xw.WriteString(value.ToString());

            if (autoClose)
                xw.WriteEndElement();
        }
        #endregion


        /// <summary>
        /// A set of SQL reserved words (upper-cased). Used to detect reserved tokens when parsing SQL.
        /// </summary>
        public static HashSet<string> ReservedWords = new HashSet<string>{"@@FETCH_STATUS", "@@IDENTITY","ADD","ALL","ALTER","AND","ANY ","AS","ASC","AUTHORIZATION","AVG","BACKUP",
            "BETWEEN","BREAK","BROWSE","BULK","BY","CASCADE","CASE","CATCH", "CHECK","CHECKPOINT","CLOSE",
            "CLUSTERED","COALESCE","COLLATE","COLUMN","COMMIT","COMPUTE","CONSTRAINT","CONTAINS","CONTAINSTABLE",
            "CONTINUE","CONVERT","COUNT","CREATE","CROSS","CURRENT","CURRENT_DATE","CURRENT_TIME","CURRENT_TIMESTAMP",
            "CURRENT_USER","CURSOR","DATABASE","DATABASEPASSWORD","DATEADD","DATEDIFF","DATENAME","DATEPART",
            "DBCC","DEALLOCATE","DECLARE","DEFAULT","DELAY","DELETE","DENY","DESC","DISK","DISTINCT","DISTRIBUTED",
            "DOUBLE","DROP","DUMP","ELSE","ENCRYPTION","ERRLVL","ESCAPE","EXCEPT","EXEC","EXECUTE",
            "EXISTS","EXIT","EXPRESSION","FETCH","FILE","FILLFACTOR","FOR","FOREIGN","FREETEXT","FREETEXTTABLE",
            "FROM","FULL","FUNCTION","GOTO","GRANT","GROUP","HAVING","HOLDLOCK","IDENTITY","IDENTITY_INSERT",
            "IDENTITYCOL","IF","IN","INDEX","INNER","INSERT","INTERSECT","INTO","IS","JOIN","KEY",
            "KILL","LEFT","LIKE","LINENO","LOAD","MAX","MIN","NATIONAL","NOCHECK","NONCLUSTERED",
            "NOT","NULL","NULLIF","OF","OFF","OFFSETS","ON","OPEN","OPENDATASOURCE","OPENQUERY",
            "OPENROWSET","OPENXML","OPTION","OR","ORDER","OUTER","OVER","PERCENT","PLAN","PRECISION",
            "PRIMARY","PRINT","PROC","PROCEDURE","PUBLIC","RAISERROR","READ","READTEXT","RECONFIGURE","REFERENCES",
            "REPLICATION","RESTORE","RESTRICT","RETURN","REVOKE","RIGHT","ROLLBACK","ROWCOUNT","ROWGUIDCOL",
            "RULE","SAVE","SCHEMA","SELECT","SESSION_USER","SET","SETUSER","SHUTDOWN","SOME","STATISTICS",
            "SUM","SYSTEM_USER","TABLE","TEXTSIZE","THEN","TO","TOP","TRAN","TRANSACTION","TRIGGER","TRUNCATE", "TRY",
            "TSEQUAL","UNION","UNIQUE","UPDATE","UPDATETEXT","USE","USER","VALUES","VARYING","VIEW","WAITFOR",
            "WHEN","WHERE","WHILE","WITH","WRITETEXT"};

        /// <summary>
        /// A set of SQL data type names (upper-cased). Used to identify SQL data types while parsing.
        /// </summary>
        public static HashSet<string> SqlDataTypes = new HashSet<string>{
            "BIGINT", "NUMERIC","BIT","INT","SMALLINT","TINYINT","SMALLMONEY","MONEY","DECIMAL", //exact numeric type
            "FLOAT","REAL",//aproximate numeric type
            "DATE","DATETIME","DATETIME2","DATETIMEOFFSET","TIME","SMALLDATETIME",//date type
            "CHAR","VARCHAR","TEXT",//char type
            "NCHAR","NVARCHAR","NTEXT",//unicode char types
            "BINARY","VARBINARY","IMAGE",//binary types
            "XML","CURSOR","TIMESTAMP","UNIQUEIDENTIFIER","HIERARCHYID","SQL_VARIANT","TABLE"//other data types
        };

        #region Extensions related to EzSql
        /// <summary>
        /// Parses a text string and converts it into a list of tokens, these tokens are have different types like: words, operators, comparators, comments, strings, brackets, commas, empty spaces.
        /// It is meant to be used in T-SQL parsing. Makes easier to analyze and manipulate T-SQL code programmatically.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <returns>A list of tokens extracted from the input text.</returns>
        public static TokenList GetTokens(this string Text)
        {
            TokenList Back = new TokenList();
            int StringLength = String.IsNullOrEmpty(Text) ? 0 : Text.Length;
            Token Current = null;

            for (int index = 0; index < StringLength; index++)
            {
                char CurChar = Text[index];
                char[] wordTokenBreaker;
                if (Current != null)
                {
                    switch (Current.Type)
                    {
                        ////can not be a comma, because a comma is a 1 char token
                        //case TokenType.COMMA:
                        //    break;
                        ////can not be aa oppenbracket, because a oppenbracket is a 1 char token
                        //case TokenType.OPENBRACKET:
                        //    break;
                        ////can not be a closebracket, because a closebracket is a 1 char token
                        //case TokenType.CLOSEBRACKET:
                        //    break;
                        //can not be any of the next either, because this is decided when the token is added to the list
                        //case TokenType.RESERVED:
                        //case TokenType.VARIABLE:
                        //case TokenType.TEMPTABLE:
                        //    break;
                        case TokenType.EMPTYSPACE:
                            #region Code to process an empty space token + something
                            if (CurChar.IsWhiteSpace())
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else
                            {
                                Back.AddToken(Current);
                                Current = null;
                                if (CurChar.IsComma())
                                {
                                    Back.AddToken(new Token(TokenType.COMMA, ","));
                                }
                                else if (CurChar.IsStringOperator())
                                {
                                    Current = new Token(TokenType.STRING, "'");
                                }
                                else if (CurChar.IsOpenBracket())
                                {
                                    Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsCloseBracket())
                                {
                                    Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsOperator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    //must check if this chars means the start of comment
                                    if (CurChar == '-' && nextc == '-')
                                    {
                                        Current = new Token(TokenType.LINECOMMENT, "--");
                                        index++;
                                    }
                                    else if (CurChar == '/' && nextc == '*')
                                    {
                                        Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                                    }
                                }
                                else if (CurChar.IsComparator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    if (nextc.IsComparator())
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                                    }
                                }
                                else
                                {
                                    Current = new Token(TokenType.WORD, CurChar.ToString());
                                }
                            }
                            #endregion
                            break;
                        case TokenType.WORD:
                            wordTokenBreaker = new char[] { ' ', '\t', '\r', '\n', '\'', ',', '(', '[', '{', '}', ']', ')', '-', '*', '+', '/', '>', '<', '=', ';' };
                            #region Code to process a word token + something
                            if (wordTokenBreaker.Contains(CurChar))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                if (CurChar.IsWhiteSpace())
                                {
                                    Current = new Token(TokenType.EMPTYSPACE, CurChar.ToString());
                                }
                                else if (CurChar.IsStringOperator())
                                {
                                    Current = new Token(TokenType.STRING, "'");
                                }
                                else if (CurChar.IsComma())
                                {
                                    Back.AddToken(new Token(TokenType.COMMA, ","));
                                }
                                else if (CurChar.IsSemmiColon())
                                {
                                    Back.AddToken(new Token(TokenType.SEMMICOLON, ";"));
                                }
                                else if (CurChar.IsOpenBracket())
                                {
                                    Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsCloseBracket())
                                {
                                    Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsOperator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    //must check if this chars means the start of comment
                                    if (CurChar == '-' && nextc == '-')
                                    {
                                        Current = new Token(TokenType.LINECOMMENT, "--");
                                        index++;
                                    }
                                    else if (CurChar == '/' && nextc == '*')
                                    {
                                        Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                                    }
                                }
                                else if (CurChar.IsComparator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    if (nextc.IsComparator())
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                                    }
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.LINECOMMENT:
                            #region Code to process a char inside a linecomment, a token breaker are "\n" and "\r\n"
                            if (CurChar == '\r')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '\n')
                                {
                                    Current.Text += "\r\n";
                                    Back.AddToken(Current);
                                    index++;
                                    Current = null;
                                }
                                else
                                {
                                    Current.Text += "\r";
                                }
                            }
                            else if (CurChar == '\n')
                            {
                                Current.Text += "\n";
                                Back.AddToken(Current);
                                Current = null;
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.BLOCKCOMMENT:
                            #region Code to process a char inside a block comment, the only token breaker is "*/"
                            if (CurChar == '*')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '/')
                                {
                                    Current.Text += "*/";
                                    Back.AddToken(Current);
                                    Current = null;
                                    index++;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.STRING:
                            #region Code to process a char inside a string, only token breaker is "'", but not "''"
                            if (CurChar == '\'')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '\'')
                                {//double '', so is an escaped ', it is a string still then
                                    Current.Text += "''";
                                    index++;
                                }
                                else
                                {
                                    Current.Text += "'";
                                    Back.AddToken(Current);
                                    Current = null;
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;

                    }
                }
                else if (Current == null)
                {
                    #region no previous token, must check if create a new one, or let the current stay null and add a new instance of token
                    if (IsWhiteSpace(CurChar))
                    {
                        Current = new Token(TokenType.EMPTYSPACE, CurChar.ToString());
                    }
                    else if (CurChar.IsComma())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.COMMA, CurChar.ToString()));
                    }
                    else if (CurChar.IsStringOperator())
                    {
                        Current = new Token(TokenType.STRING, CurChar.ToString());
                    }
                    else if (CurChar.IsOpenBracket())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                    }
                    else if (CurChar.IsCloseBracket())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                    }
                    else if (CurChar.IsOperator())
                    {
                        char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                        //must check if this chars means the start of comment
                        if (CurChar == '-' && nextc == '-')
                        {
                            Current = new Token(TokenType.LINECOMMENT, "--");
                            index++;
                        }
                        else if (CurChar == '/' && nextc == '*')
                        {
                            Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                            index++;
                        }
                        else
                        {
                            Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                        }
                    }
                    else if (CurChar.IsComparator())
                    {
                        char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                        if (nextc.IsComparator())
                        {
                            Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                            index++;
                        }
                        else
                        {
                            Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                        }
                    }
                    else
                    {
                        Current = new Token(TokenType.WORD, CurChar.ToString());
                    }
                    #endregion
                }
            }
            if (Current != null)
                Back.AddToken(Current);

            List<int> ToReplace = new List<int>();
            for (int i = 0; i < Back.TokenCount - 2; i++)
            {
                if (
                    Back[i].Type == TokenType.BLOCKSTART &&
                Back[i + 1].Type == TokenType.EMPTYSPACE &&
                Back[i + 2].Type == TokenType.RESERVED &&
                (Back[i + 2].Text.Equals("tran", StringComparison.CurrentCultureIgnoreCase) ||
                Back[i + 2].Text.Equals("transaction", StringComparison.CurrentCultureIgnoreCase)))
                {
                    ToReplace.Add(i);
                }
            }

            if (ToReplace.Count > 0)
            {
                int found = 0;
                foreach (int i in ToReplace)
                {
                    int buffIndex = i - (found * 2);

                    Token buff = new Token(TokenType.BEGINTRANSACTION, String.Format("{0}{1}{2}", Back[buffIndex].Text, Back[buffIndex + 1].Text, Back[buffIndex + 2].Text));
                    Back.RemoveTokenAt(buffIndex);
                    Back.RemoveTokenAt(buffIndex);
                    Back.RemoveTokenAt(buffIndex);
                    Back.AddTokenAt(buffIndex, buff);

                    found++;
                }
            }

            return Back;
        }

        /// <summary>
        /// Initiates the whole parsing process but stops at the first token found. Done this way for efficiency, instead of parsing the whole string when only the first token is needed.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <returns>A token extracted from the input text.</returns>
        public static Token GetFirstToken(this string Text)
        {
            TokenList Back = new TokenList();
            int StringLength = String.IsNullOrEmpty(Text) ? 0 : Text.Length;
            Token Current = null;

            for (int index = 0; index < StringLength; index++)
            {
                if (Back.TokenCount > 1)
                    break;

                char CurChar = Text[index];
                char[] wordTokenBreaker;
                if (Current != null)
                {
                    switch (Current.Type)
                    {
                        ////can not be a comma, because a comma is a 1 char token
                        //case TokenType.COMMA:
                        //    break;
                        ////can not be aa oppenbracket, because a oppenbracket is a 1 char token
                        //case TokenType.OPENBRACKET:
                        //    break;
                        ////can not be a closebracket, because a closebracket is a 1 char token
                        //case TokenType.CLOSEBRACKET:
                        //    break;
                        //can not be any of the next either, because this is decided when the token is added to the list
                        //case TokenType.RESERVED:
                        //case TokenType.VARIABLE:
                        //case TokenType.TEMPTABLE:
                        //    break;
                        case TokenType.EMPTYSPACE:
                            #region Code to process an empty space token + something
                            if (CurChar.IsWhiteSpace())
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else
                            {
                                Back.AddToken(Current);
                                Current = null;
                                if (CurChar.IsComma())
                                {
                                    Back.AddToken(new Token(TokenType.COMMA, ","));
                                }
                                else if (CurChar.IsStringOperator())
                                {
                                    Current = new Token(TokenType.STRING, "'");
                                }
                                else if (CurChar.IsOpenBracket())
                                {
                                    Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsCloseBracket())
                                {
                                    Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsOperator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    //must check if this chars means the start of comment
                                    if (CurChar == '-' && nextc == '-')
                                    {
                                        Current = new Token(TokenType.LINECOMMENT, "--");
                                        index++;
                                    }
                                    else if (CurChar == '/' && nextc == '*')
                                    {
                                        Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                                    }
                                }
                                else if (CurChar.IsComparator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    if (nextc.IsComparator())
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                                    }
                                }
                                else
                                {
                                    Current = new Token(TokenType.WORD, CurChar.ToString());
                                }
                            }
                            #endregion
                            break;
                        case TokenType.WORD:
                            wordTokenBreaker = new char[] { ' ', '\t', '\r', '\n', '\'', ',', '(', '[', '{', '}', ']', ')', '-', '*', '+', '/', '>', '<', '=' };
                            #region Code to process a word token + something
                            if (wordTokenBreaker.Contains(CurChar))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                if (CurChar.IsWhiteSpace())
                                {
                                    Current = new Token(TokenType.EMPTYSPACE, CurChar.ToString());
                                }
                                else if (CurChar.IsStringOperator())
                                {
                                    Current = new Token(TokenType.STRING, "'");
                                }
                                else if (CurChar.IsComma())
                                {
                                    Current = new Token(TokenType.STRING, ",");
                                }
                                else if (CurChar.IsOpenBracket())
                                {
                                    Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsCloseBracket())
                                {
                                    Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsOperator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    //must check if this chars means the start of comment
                                    if (CurChar == '-' && nextc == '-')
                                    {
                                        Current = new Token(TokenType.LINECOMMENT, "--");
                                        index++;
                                    }
                                    else if (CurChar == '/' && nextc == '*')
                                    {
                                        Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                                    }
                                }
                                else if (CurChar.IsComparator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    if (nextc.IsComparator())
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                                    }
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.LINECOMMENT:
                            #region Code to process a char inside a linecomment, a token breaker are "\n" and "\r\n"
                            if (CurChar == '\r')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '\n')
                                {
                                    Current.Text += "\r\n";
                                    Back.AddToken(Current);
                                    Current = null;
                                }
                                else
                                {
                                    Current.Text += "\r";
                                }
                            }
                            else if (CurChar == '\n')
                            {
                                Current.Text += "\n";
                                Back.AddToken(Current);
                                Current = null;
                            }
                            else
                            {
                                Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.BLOCKCOMMENT:
                            #region Code to process a char inside a block comment, the only token breaker is "*/"
                            if (CurChar == '*')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '/')
                                {
                                    Current.Text += "*/";
                                    Back.AddToken(Current);
                                    Current = null;
                                    index++;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.STRING:
                            #region Code to process a char inside a string, only token breaker is "'", but not "''"
                            if (CurChar == '\'')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '\'')
                                {//double '', so is an escaped ', it is a string still then
                                    Current.Text += "''";
                                    index++;
                                }
                                else
                                {
                                    Current.Text += "'";
                                    Back.AddToken(Current);
                                    Current = null;
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;

                    }
                }
                else if (Current == null)
                {
                    #region no previous token, must check if create a new one, or let the current stay null and add a new instance of token
                    if (IsWhiteSpace(CurChar))
                    {
                        Current = new Token(TokenType.EMPTYSPACE, CurChar.ToString());
                    }
                    else if (CurChar.IsComma())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.COMMA, CurChar.ToString()));
                    }
                    else if (CurChar.IsStringOperator())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.STRING, CurChar.ToString()));
                    }
                    else if (CurChar.IsOpenBracket())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                    }
                    else if (CurChar.IsCloseBracket())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                    }
                    else if (CurChar.IsOperator())
                    {
                        char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                        //must check if this chars means the start of comment
                        if (CurChar == '-' && nextc == '-')
                        {
                            Current = new Token(TokenType.LINECOMMENT, "--");
                            index++;
                        }
                        else if (CurChar == '/' && nextc == '*')
                        {
                            Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                            index++;
                        }
                        else
                        {
                            Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                        }
                    }
                    else if (CurChar.IsComparator())
                    {
                        char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                        if (nextc.IsComparator())
                        {
                            Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                            index++;
                        }
                        else
                        {
                            Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                        }
                    }
                    else
                    {
                        Current = new Token(TokenType.WORD, CurChar.ToString());
                    }
                    #endregion
                }
            }
            if (Current != null)
                Back.AddToken(Current);

            return Back.TokenCount > 0 ? Back[0] : new Token(TokenType.EMPTYSPACE, "");
        }

        /// <summary>
        /// Parses the whole string and returns the last token found.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <returns>A token extracted from the input text.</returns>
        public static Token GetLastToken(this string Text)
        {
            return Text.GetTokens().List.LastOrDefault() ?? new Token(TokenType.EMPTYSPACE, "");
        }

        /// <summary>
        /// Checks if the provided word is a reserved SQL keyword.
        /// </summary>
        /// <param name="Word">The word to check.</param>
        /// <returns>True if the word is a reserved SQL keyword; otherwise, false.</returns>
        public static bool IsReserved(this string Word)
        {
            return ReservedWords.Contains(Word.ToUpper());
        }

        /// <summary>
        /// Determines whether the specified string represents a valid SQL data type.
        /// </summary>
        /// <remarks>The method checks the string against a predefined list of SQL data types. The
        /// comparison is case-insensitive.</remarks>
        /// <param name="Word">The string to evaluate. This value is case-insensitive.</param>
        /// <returns><see langword="true"/> if the specified string matches a valid SQL data type; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool IsSqlDataType(this string Word)
        {
            return SqlDataTypes.Contains(Word.ToUpper());
        }

        /// <summary>
        /// Parses a text string and converts it into a list of generic tokens based on the specified programming language.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <param name="language">The programming language to use for parsing rules.</param>
        /// <returns>A GenericTokenList containing the parsed tokens.</returns>
        public static GenericTokenList GetTokens(this string Text, GenericLanguage language)
        {
            LanguageConfig config = LanguageConfig.GetLanguageConfig(language);
            GenericTokenList Back = new GenericTokenList();
            int StringLength = String.IsNullOrEmpty(Text) ? 0 : Text.Length;
            GenericToken Current = null;

            for (int index = 0; index < StringLength; index++)
            {
                char CurChar = Text[index];
                char nextChar = Text.Length > index + 1 ? Text[index + 1] : '\0';

                if (Current != null)
                {
                    switch (Current.Type)
                    {
                        case GenericTokenType.EMPTYSPACE:
                            if (CurChar.IsWhiteSpace())
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            break;

                        case GenericTokenType.WORD:
                            if (IsWordBreaker(CurChar, config))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.NUMBER:
                            if (char.IsDigit(CurChar) || CurChar == '.' || (config.AllowUnderscoreInNumbers && CurChar == '_'))
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else if (IsWordBreaker(CurChar, config))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            else
                            {
                                // Number followed by letter = switch to WORD
                                Current.Type = GenericTokenType.WORD;
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.LINECOMMENT:
                            if (CurChar == '\r' && nextChar == '\n')
                            {
                                Current.Text += "\r\n";
                                Back.AddToken(Current);
                                Current = null;
                                index++;
                            }
                            else if (CurChar == '\n')
                            {
                                Current.Text = Current.Text.AppendChar('\n');
                                Back.AddToken(Current);
                                Current = null;
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.BLOCKCOMMENT:
                            if (config.BlockCommentEnd != null && CurChar == config.BlockCommentEnd[0])
                            {
                                bool isEnd = true;
                                for (int i = 1; i < config.BlockCommentEnd.Length && index + i < StringLength; i++)
                                {
                                    if (Text[index + i] != config.BlockCommentEnd[i])
                                    {
                                        isEnd = false;
                                        break;
                                    }
                                }
                                if (isEnd)
                                {
                                    Current.Text += config.BlockCommentEnd;
                                    Back.AddToken(Current);
                                    Current = null;
                                    index += config.BlockCommentEnd.Length - 1;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.STRING:
                            if (config.StringEscapeChar.HasValue && CurChar == config.StringEscapeChar.Value && nextChar != '\0')
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                                Current.Text = Current.Text.AppendChar(nextChar);
                                index++;
                            }
                            else if (config.StringDelimiters.Contains(CurChar))
                            {
                                // Check if this is the closing delimiter
                                if (Current.Text.Length > 0 && Current.Text[0] == CurChar)
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                    Back.AddToken(Current);
                                    Current = null;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;
                    }
                }
                else // Current == null
                {
                    index = ProcessNewToken(Text, index, ref Current, Back, config);
                }
            }

            if (Current != null)
                Back.AddToken(Current);

            // Post-processing: identify RESERVED words and DATATYPE
            for (int i = 0; i < Back.TokenCount; i++)
            {
                if (Back[i].Type == GenericTokenType.WORD)
                {
                    if (config.ReservedKeywords.Contains(Back[i].Text))
                    {
                        Back[i].Type = GenericTokenType.RESERVED;
                    }
                    else if (config.DataTypes.Contains(Back[i].Text))
                    {
                        Back[i].Type = GenericTokenType.DATATYPE;
                    }
                }
            }

            return Back;
        }

        /// <summary>
        /// Parses a text string and returns the first token found based on the specified programming language.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <param name="language">The programming language to use for parsing rules.</param>
        /// <returns>The first GenericToken found, or an empty EMPTYSPACE token if no token is found.</returns>
        public static GenericToken GetFirstToken(this string Text, GenericLanguage language)
        {
            LanguageConfig config = LanguageConfig.GetLanguageConfig(language);
            GenericTokenList Back = new GenericTokenList();
            int StringLength = String.IsNullOrEmpty(Text) ? 0 : Text.Length;
            GenericToken Current = null;

            for (int index = 0; index < StringLength; index++)
            {
                if (Back.TokenCount > 1)
                    break;

                char CurChar = Text[index];
                char nextChar = Text.Length > index + 1 ? Text[index + 1] : '\0';

                if (Current != null)
                {
                    switch (Current.Type)
                    {
                        case GenericTokenType.EMPTYSPACE:
                            if (CurChar.IsWhiteSpace())
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            break;

                        case GenericTokenType.WORD:
                            if (IsWordBreaker(CurChar, config))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.NUMBER:
                            if (char.IsDigit(CurChar) || CurChar == '.' || (config.AllowUnderscoreInNumbers && CurChar == '_'))
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else if (IsWordBreaker(CurChar, config))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            else
                            {
                                Current.Type = GenericTokenType.WORD;
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.LINECOMMENT:
                            if (CurChar == '\r' && nextChar == '\n')
                            {
                                Current.Text += "\r\n";
                                Back.AddToken(Current);
                                Current = null;
                                index++;
                            }
                            else if (CurChar == '\n')
                            {
                                Current.Text = Current.Text.AppendChar('\n');
                                Back.AddToken(Current);
                                Current = null;
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.BLOCKCOMMENT:
                            if (config.BlockCommentEnd != null && CurChar == config.BlockCommentEnd[0])
                            {
                                bool isEnd = true;
                                for (int i = 1; i < config.BlockCommentEnd.Length && index + i < StringLength; i++)
                                {
                                    if (Text[index + i] != config.BlockCommentEnd[i])
                                    {
                                        isEnd = false;
                                        break;
                                    }
                                }
                                if (isEnd)
                                {
                                    Current.Text += config.BlockCommentEnd;
                                    Back.AddToken(Current);
                                    Current = null;
                                    index += config.BlockCommentEnd.Length - 1;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.STRING:
                            if (config.StringEscapeChar.HasValue && CurChar == config.StringEscapeChar.Value && nextChar != '\0')
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                                Current.Text = Current.Text.AppendChar(nextChar);
                                index++;
                            }
                            else if (config.StringDelimiters.Contains(CurChar))
                            {
                                if (Current.Text.Length > 0 && Current.Text[0] == CurChar)
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                    Back.AddToken(Current);
                                    Current = null;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;
                    }
                }
                else
                {
                    index = ProcessNewToken(Text, index, ref Current, Back, config);
                }
            }

            if (Current != null)
                Back.AddToken(Current);

            // Post-processing for the first token
            if (Back.TokenCount > 0)
            {
                GenericToken firstToken = Back[0];
                if (firstToken.Type == GenericTokenType.WORD)
                {
                    if (config.ReservedKeywords.Contains(firstToken.Text))
                    {
                        firstToken.Type = GenericTokenType.RESERVED;
                    }
                    else if (config.DataTypes.Contains(firstToken.Text))
                    {
                        firstToken.Type = GenericTokenType.DATATYPE;
                    }
                }
                return firstToken;
            }

            return new GenericToken(GenericTokenType.EMPTYSPACE, "");
        }

        /// <summary>
        /// Parses a text string and returns the last token found based on the specified programming language.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <param name="language">The programming language to use for parsing rules.</param>
        /// <returns>The last GenericToken found, or an empty EMPTYSPACE token if no token is found.</returns>
        public static GenericToken GetLastToken(this string Text, GenericLanguage language)
        {
            GenericTokenList tokens = Text.GetTokens(language);
            return tokens.List.LastOrDefault() ?? new GenericToken(GenericTokenType.EMPTYSPACE, "");
        }

        private static int ProcessNewToken(string Text, int index, ref GenericToken Current, GenericTokenList Back, LanguageConfig config)
        {
            char CurChar = Text[index];
            char nextChar = Text.Length > index + 1 ? Text[index + 1] : '\0';

            if (CurChar.IsWhiteSpace())
            {
                Current = new GenericToken(GenericTokenType.EMPTYSPACE, CurChar.ToString());
            }
            else if (CurChar == ',')
            {
                Back.AddToken(new GenericToken(GenericTokenType.COMMA, ","));
            }
            else if (CurChar == '.')
            {
                // Check if it's a decimal number starting with a dot
                if (char.IsDigit(nextChar))
                {
                    Current = new GenericToken(GenericTokenType.NUMBER, ".");
                }
                else
                {
                    Back.AddToken(new GenericToken(GenericTokenType.DOT, "."));
                }
            }
            else if (CurChar == ';')
            {
                Back.AddToken(new GenericToken(GenericTokenType.SEMICOLON, ";"));
            }
            else if (CurChar == ':')
            {
                Back.AddToken(new GenericToken(GenericTokenType.COLON, ":"));
            }
            else if (config.OpenBrackets.Contains(CurChar))
            {
                Back.AddToken(new GenericToken(GenericTokenType.OPENBRACKET, CurChar.ToString()));
            }
            else if (config.CloseBrackets.Contains(CurChar))
            {
                Back.AddToken(new GenericToken(GenericTokenType.CLOSEBRACKET, CurChar.ToString()));
            }
            else if (config.StringDelimiters.Contains(CurChar))
            {
                Current = new GenericToken(GenericTokenType.STRING, CurChar.ToString());
            }
            else if (config.LineCommentStart != null && CurChar == config.LineCommentStart[0])
            {
                bool isComment = true;
                for (int i = 1; i < config.LineCommentStart.Length && index + i < Text.Length; i++)
                {
                    if (Text[index + i] != config.LineCommentStart[i])
                    {
                        isComment = false;
                        break;
                    }
                }
                if (isComment)
                {
                    Current = new GenericToken(GenericTokenType.LINECOMMENT, config.LineCommentStart);
                    return index + config.LineCommentStart.Length - 1;
                }
                else
                {
                    // It's an operator - add it immediately
                    Back.AddToken(new GenericToken(GenericTokenType.OPERATOR, CurChar.ToString()));
                }
            }
            else if (config.BlockCommentStart != null && CurChar == config.BlockCommentStart[0])
            {
                bool isComment = true;
                for (int i = 1; i < config.BlockCommentStart.Length && index + i < Text.Length; i++)
                {
                    if (Text[index + i] != config.BlockCommentStart[i])
                    {
                        isComment = false;
                        break;
                    }
                }
                if (isComment)
                {
                    Current = new GenericToken(GenericTokenType.BLOCKCOMMENT, config.BlockCommentStart);
                    return index + config.BlockCommentStart.Length - 1;
                }
                else if (config.Operators.Contains(CurChar.ToString()))
                {
                    // It's an operator - add it immediately
                    Back.AddToken(new GenericToken(GenericTokenType.OPERATOR, CurChar.ToString()));
                }
                else
                {
                    Current = new GenericToken(GenericTokenType.WORD, CurChar.ToString());
                }
            }
            else if (char.IsDigit(CurChar))
            {
                Current = new GenericToken(GenericTokenType.NUMBER, CurChar.ToString());
            }
            else if (config.Operators.Contains(CurChar.ToString()))
            {
                // Check for multi-character operators
                string twoCharOp = CurChar.ToString() + nextChar.ToString();
                if (config.Operators.Contains(twoCharOp))
                {
                    Back.AddToken(new GenericToken(GenericTokenType.OPERATOR, twoCharOp));
                    return index + 1;
                }
                else
                {
                    Back.AddToken(new GenericToken(GenericTokenType.OPERATOR, CurChar.ToString()));
                }
            }
            else
            {
                Current = new GenericToken(GenericTokenType.WORD, CurChar.ToString());
            }

            return index;
        }

        private static bool IsWordBreaker(char c, LanguageConfig config)
        {
            return c.IsWhiteSpace() || c == ',' || c == '.' || c == ';' || c == ':' ||
                   config.OpenBrackets.Contains(c) || config.CloseBrackets.Contains(c) ||
                   config.StringDelimiters.Contains(c) || config.Operators.Contains(c.ToString());
        }


        #endregion

        #region Extensions for char
        /// <summary>
        /// Converts a hexadecimal character (0-9, A-F, a-f) to its decimal integer equivalent.
        /// </summary>
        /// <param name="c">The hexadecimal character to convert. Valid characters are '0'-'9', 'A'-'F', 'a'-'f'.</param>
        /// <returns>
        /// The integer value represented by the hex character in the range 0 to 15.
        /// Returns -1 if the character is not a valid hexadecimal digit.
        /// </returns>
        public static int FromHexCharToDecInt(this char c)
        {
            switch (c)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                case 'a':
                    return 10;
                case 'B':
                case 'b':
                    return 11;
                case 'C':
                case 'c':
                    return 12;
                case 'D':
                case 'd':
                    return 13;
                case 'E':
                case 'e':
                    return 14;
                case 'F':
                case 'f':
                    return 15;
            }

            return -1;
        }

        /// <summary>
        /// Determines whether the character is a closing bracket.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is a closing bracket: ')', ']' or '}'.</returns>
        private static bool IsCloseBracket(this char c)
        {
            return (c == ')' || c == ']' || c == '}');
        }

        /// <summary>
        /// Determines whether the character is a comma.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is a comma ','; otherwise false.</returns>
        private static bool IsComma(this char c)
        {
            return c == ',';
        }

        /// <summary>
        /// Determines whether the character is a comparator symbol.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is one of '>', '<' or '='; otherwise false.</returns>
        private static bool IsComparator(this char c)
        {
            return (c == '>' || c == '<' || c == '=');
        }

        /// <summary>
        /// Determines whether the character is an opening bracket.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is an opening bracket: '(', '[' or '{'. Returns false otherwise.</returns>
        private static bool IsOpenBracket(this char c)
        {
            return (c == '(' || c == '[' || c == '{');
        }

        /// <summary>
        /// Determines whether the character is an arithmetic operator or division symbol used in token parsing.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is one of '+', '/', '-', '*' ; otherwise false.</returns>
        private static bool IsOperator(this char c)
        {
            return (c == '+' || c == '/' || c == '-' || c == '*');
        }

        /// <summary>
        /// Determines whether the character is a semicolon ';'.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is ';'; otherwise false.</returns>
        private static bool IsSemmiColon(this char c)
        {
            return c == ';';
        }

        /// <summary>
        /// Determines whether the character is a SQL string delimiter (single quote).
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is a single quote '\''; otherwise false.</returns>
        private static bool IsStringOperator(this char c)
        {
            return c == '\'';
        }

        /// <summary>
        /// Determines whether the character is whitespace used in parsing.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is a space, tab, carriage return or newline; otherwise false.</returns>
        private static bool IsWhiteSpace(this char c)
        {
            return (c == ' ' || c == '\t' || c == '\r' || c == '\n');
        }
        #endregion

    }

}
