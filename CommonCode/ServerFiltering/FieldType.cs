namespace ZidUtilities.CommonCode.ServerFiltering
{
    /// <summary>
    /// Defines the data type of a field used for server-side filtering and comparisons.
    /// Use this enum to indicate how values should be parsed, validated, and compared.
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// Integer numeric type (whole numbers).
        /// Typical use: counts, identifiers, and integer-based comparisons.
        /// </summary>
        Integer,

        /// <summary>
        /// Floating-point numeric type (decimal values).
        /// Typical use: measurements, currency (when appropriate), and fractional values.
        /// </summary>
        Float,

        /// <summary>
        /// Date/time type.
        /// Typical use: fields that represent dates or timestamps and require date-based comparisons.
        /// </summary>
        Date,

        /// <summary>
        /// Boolean type.
        /// Typical use: fields that represent true/false values or binary flags.
        /// </summary>
        Boolean,

        /// <summary>
        /// String/text type.
        /// Typical use: free-form text, identifiers stored as text, and pattern matching.
        /// </summary>
        String
    }
}