using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ZidUtilities.CommonCode.ServerFiltering
{
    /// <summary>
    /// Base implementation of <see cref="IFilterableObject"/> that provides
    /// server-side filtering helpers used by derived types.
    /// </summary>
    public class KendoFilterableObject : IFilterableObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for the object.
        /// Implementations should return a stable identifier for the object instance.
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Resolves the <see cref="FieldType"/> for a named field on the object.
        /// Implementing classes should override this method to map field names to the
        /// appropriate <see cref="FieldType"/> values used by the filtering logic.
        /// </summary>
        /// <param name="fieldName">The name of the field whose type is requested.</param>
        /// <returns>
        /// A <see cref="FieldType"/> enum value indicating the data type of the field.
        /// The default implementation throws <see cref="NotImplementedException"/>.
        /// </returns>
        public virtual FieldType GetFieldType(string fieldName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the runtime value of a field or property by name for this object instance.
        /// Implementing classes should override this method to return the actual value for
        /// the supplied field name. The returned value will be used by the filtering comparison
        /// logic in <see cref="ValueOnFieldComparison"/>.
        /// </summary>
        /// <param name="fieldName">The name of the field or property to retrieve the value for.</param>
        /// <returns>
        /// The field value as an <see cref="object"/>. If the field is not present or has no value,
        /// implementations may return null. The default implementation throws <see cref="NotImplementedException"/>.
        /// </returns>
        public virtual object GetFieldValue(string fieldName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compares a named field's current value on this object to a provided string value using
        /// the specified operator. This method performs type-specific parsing and comparison
        /// based on the result of <see cref="GetFieldType(string)"/>.
        /// </summary>
        /// <param name="field">The name of the field to evaluate. Should correspond to a field or property on the object.</param>
        /// <param name="value">
        /// The comparison value represented as a string. This method converts the string to the
        /// appropriate runtime type before performing the comparison (e.g. integer, double, DateTime, bool, or string).
        /// </param>
        /// <param name="oper">
        /// The operator to use for comparison (for example: "eq", "neq", "gte", "lt", "contains", etc.).
        /// The set of supported operators depends on the detected field type; unsupported operators return false.
        /// </param>
        /// <returns>
        /// True if the field's value satisfies the comparison with the provided value using the operator; otherwise false.
        /// If conversion fails or an operator is not supported for the field type, the method returns false.
        /// </returns>
        public bool ValueOnFieldComparison(string field, string value, string oper)
        {
            switch (GetFieldType(field))
            {
                case FieldType.Integer:
                    int? intFieldValue = this.GetFieldValue(field) as System.Int32?;
                    int? intValue = Convert.ToInt32(value);
                    switch (oper.ToLower())
                    {
                        case "eq"://equal to
                            return intFieldValue == intValue;
                        case "neq"://not equal to
                            return intFieldValue != intValue;
                        case "gte"://greater or equal to
                            return intFieldValue >= intValue;
                        case "gt"://greater than
                            return intFieldValue > intValue;
                        case "lte"://lesser or equal to
                            return intFieldValue <= intValue;
                        case "lt"://lesser than
                            return intFieldValue < intValue;
                        case "isempty"://is empty
                        case "isnull"://is null
                            return !intFieldValue.HasValue;
                        case "isnotempty"://is empty
                        case "isnotnull"://is not null
                            return intFieldValue.HasValue;
                        default:
                            return false;
                    }
                case FieldType.Float:
                    double? doubleFieldValue = this.GetFieldValue(field) as Double?;
                    double? doubleValue = Convert.ToDouble(value);
                    switch (oper.ToLower())
                    {
                        case "eq"://equal to
                            return doubleFieldValue == doubleValue;
                        case "neq"://not equal to
                            return doubleFieldValue != doubleValue;
                        case "gte"://greater or equal to
                            return doubleFieldValue >= doubleValue;
                        case "gt"://greater than
                            return doubleFieldValue > doubleValue;
                        case "lte"://lesser or equal to
                            return doubleFieldValue <= doubleValue;
                        case "lt"://lesser than
                            return doubleFieldValue < doubleValue;
                        case "isempty"://is empty
                        case "isnull"://is null
                            return !doubleFieldValue.HasValue;
                        case "isnotempty"://is empty
                        case "isnotnull"://is not null
                            return doubleFieldValue.HasValue;
                        default:
                            return false;
                    }
                case FieldType.Date:
                    DateTime? dateFieldValue = this.GetFieldValue(field) as System.DateTime?;
                    DateTime? dateValue;
                    DateTimeOffset helperDT;

                    if (String.IsNullOrEmpty(value) || value.Equals("null", StringComparison.CurrentCultureIgnoreCase))
                        dateValue = null; 
                    else if (DateTimeOffset.TryParseExact(value.Substring(0, 33).Remove(25, 3), "ddd MMM dd yyyy HH:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out helperDT))
                        dateValue = helperDT.DateTime;
                    else
                        dateValue = null;


                    switch (oper.ToLower())
                    {
                        case "eq"://equal to
                            return dateFieldValue == dateValue;
                        case "neq"://not equal to
                            return dateFieldValue != dateValue;
                        case "gte"://greater or equal to
                            return dateFieldValue >= dateValue;
                        case "gt"://greater than
                            return dateFieldValue > dateValue;
                        case "lte"://lesser or equal to
                            return dateFieldValue <= dateValue;
                        case "lt"://lesser than
                            return dateFieldValue < dateValue;
                        case "isempty"://is empty
                        case "isnull"://is null
                            return !dateFieldValue.HasValue;
                        case "isnotempty"://is empty
                        case "isnotnull"://is not null
                            return dateFieldValue.HasValue;
                        default:
                            return false;
                    }
                case FieldType.Boolean:
                    bool? boolFieldValue = this.GetFieldValue(field) as System.Boolean?;
                    bool? boolValue = Convert.ToBoolean(value);
                    switch (oper.ToLower())
                    {
                        case "eq"://equal to
                            return boolFieldValue == boolValue;
                        case "isempty"://is empty
                        case "isnull"://is null
                            return !boolFieldValue.HasValue;
                        case "isnotempty"://is empty
                        case "isnotnull"://is not null
                            return boolFieldValue.HasValue;
                        default:
                            return false;
                    }
                default:
                case FieldType.String:
                    string strFieldValue = this.GetFieldValue(field).ToString();
                    switch (oper.ToLower())
                    {
                        case "eq"://equal to
                            return strFieldValue.Equals(value, StringComparison.CurrentCultureIgnoreCase);
                        case "neq"://not equal to
                            return !strFieldValue.Equals(value, StringComparison.CurrentCultureIgnoreCase);
                        case "startswith"://starts with
                            return strFieldValue.StartsWith(value, StringComparison.CurrentCultureIgnoreCase);
                        case "endswith"://ends with
                            return strFieldValue.EndsWith(value, StringComparison.CurrentCultureIgnoreCase);
                        case "contains"://contains
                            return strFieldValue.ToUpper().Contains(value.ToUpper());
                        case "doesnotcontain"://does not contain
                            return !strFieldValue.ToUpper().Contains(value.ToUpper());
                        case "isempty"://is empty
                        case "isnull"://is null
                            return String.IsNullOrEmpty(strFieldValue);
                        case "isnotempty"://is empty
                        case "isnotnull"://is not null
                            return !String.IsNullOrEmpty(strFieldValue);
                        default:
                            return false;
                    }
            }

        }

        /// <summary>
        /// Parses a raw, serialized filter string into a <see cref="FilterMerge"/> instance.
        /// The expected raw format is a series of column filters separated by '$', where each
        /// column filter may represent a single filter or two filters joined by '!' (AND) or '|' (OR).
        /// </summary>
        /// <param name="rawFilter">
        /// The raw filter string that encodes one or more column-level filters. Example format:
        /// "Field1;eq;Value$Field2;contains;Value2!Field3;lt;10"
        /// </param>
        /// <returns>
        /// A <see cref="FilterMerge"/> containing the parsed <see cref="FilterDescription"/> entries
        /// and their join logic. If <paramref name="rawFilter"/> is null or empty, an empty
        /// <see cref="FilterMerge"/> instance is returned.
        /// </returns>
        public FilterMerge ParseFilterString(string rawFilter)
        {
            FilterMerge back = new FilterMerge();
            #region Code to apply the server side filtering
            if (!String.IsNullOrEmpty(rawFilter))
            {
                IEnumerable<string> columnsFilters = rawFilter.Split('$').Where(x => !String.IsNullOrEmpty(x));

                foreach (string cf in columnsFilters)
                {
                    char separator = (cf.Contains("!") ? '!' : (cf.Contains("|") ? '|' : '?'));//determine filters join logic if any...(single filter)
                    //is a single filter, if is only 1 term for the split, if any of the members is "undefined" or the separator was not found
                    bool isSingleFilter = cf.Split(separator).Length < 2 || cf.Split(separator)[0].ToLower().Contains("undefined") || cf.Split(separator)[1].ToLower().Contains("undefined") || separator == '?';

                    if (isSingleFilter)
                    {
                        back.Filters.Add(FilterDescription.LoadFromString(cf));
                        back.Filters.Add(FilterDescription.LoadFromString("empty"));
                        back.JoinLogic.Add(FilterMerge.FilterJoin.SingleFilter);
                    }
                    else
                    {
                        if (cf.Contains("!"))//"AND" logic
                        {
                            back.Filters.Add(FilterDescription.LoadFromString(cf.Split(separator)[0]));
                            back.Filters.Add(FilterDescription.LoadFromString(cf.Split(separator)[1]));
                            back.JoinLogic.Add(FilterMerge.FilterJoin.AndLogic);
                        }
                        else
                        {//"OR" logic
                            back.Filters.Add(FilterDescription.LoadFromString(cf.Split(separator)[0]));
                            back.Filters.Add(FilterDescription.LoadFromString(cf.Split(separator)[1]));
                            back.JoinLogic.Add(FilterMerge.FilterJoin.OrLogic);
                        }
                    }
                }
            }
            #endregion
            return back;
        }
    }
}