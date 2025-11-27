namespace ZidUtilities.CommonCode.ServerFiltering
{
    /// <summary>
    /// Represents a description of a server-side filter containing a field name,
    /// an operator, and a value. Can be constructed with only a field or by parsing
    /// a raw filter string.
    /// </summary>
    public class FilterDescription
    {
        /// <summary>
        /// The name of the field to filter on.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// The operator to apply for filtering (for example, "eq", "contains", etc.).
        /// May be null if the filter was constructed with only a field.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// The value to compare the field against. May be null if the filter was constructed
        /// with only a field.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="FilterDescription"/> with only a field name.
        /// </summary>
        /// <param name="field">The field name to filter on. Operator and Value will remain null.</param>
        public FilterDescription(string field)
        {
            Field = field;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FilterDescription"/> with field, operator, and value.
        /// </summary>
        /// <param name="field">The field name to filter on.</param>
        /// <param name="oper">The operator to apply (for example, "eq", "lt", "gt", "contains").</param>
        /// <param name="value">The comparison value for the field.</param>
        public FilterDescription(string field, string oper, string value)
        {
            Field = field;
            Operator = oper;
            Value = value;
        }

        /// <summary>
        /// Parses a raw filter string into a <see cref="FilterDescription"/>.
        /// </summary>
        /// <param name="rawFilter">
        /// The raw filter string. Expected formats:
        /// - "field" (only field name),
        /// - "field;operator;value" (three semicolon-separated parts).
        /// </param>
        /// <returns>
        /// A <see cref="FilterDescription"/> instance constructed from the parsed data.
        /// If <paramref name="rawFilter"/> contains a semicolon, the method splits on ';'
        /// and returns a description populated with field, operator, and value.
        /// If no semicolon is present, a description containing only the field is returned.
        /// </returns>
        /// <remarks>
        /// This method does not perform extensive validation. If the input contains ';'
        /// it is assumed to have at least three segments; otherwise an exception may occur.
        /// </remarks>
        public static FilterDescription LoadFromString(string rawFilter)
        {
            string[] data;

            if (rawFilter.Contains(";"))
            {
                data = rawFilter.Split(';');
                string field = data[0];
                string oper = data[1];
                string val = data[2];
                return new FilterDescription(field, oper, val);
            }
            else
            {
                return new FilterDescription(rawFilter);
            }
        }
    }
}