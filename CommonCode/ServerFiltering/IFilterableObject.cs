namespace ZidUtilities.CommonCode.ServerFiltering
{
    /// <summary>
    /// Represents an object whose fields can be filtered by server-side filtering logic.
    /// Implementing types provide parsing, field type resolution and value comparison helpers
    /// used when applying filter expressions to object instances.
    /// </summary>
    public interface IFilterableObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for the object.
        /// </summary>
        /// <value>
        /// An integer representing the object's unique identifier.
        /// Implementations should return a stable identifier for the object instance.
        /// </value>
        int Id { get; set; }

        /// <summary>
        /// Parses a raw filter string into a structured <see cref="FilterMerge"/> representing
        /// one or more filter descriptions and their join logic.
        /// </summary>
        /// <param name="rawFilter">
        /// The raw filter string to parse. The expected format depends on the application's
        /// filter syntax (for example: "Field1==Value;Field2!=Value2" or other custom formats).
        /// Implementations should handle validation and throw or return an appropriate
        /// result for invalid input.
        /// </param>
        /// <returns>
        /// A <see cref="FilterMerge"/> instance containing parsed <see cref="FilterDescription"/>
        /// objects and their join logic. The returned object represents the logical structure
        /// of the provided filter string and may be empty if no valid filters are present.
        /// </returns>
        FilterMerge ParseFilterString(string rawFilter);

        /// <summary>
        /// Resolves the <see cref="FieldType"/> of a named field on the object.
        /// </summary>
        /// <param name="fieldName">
        /// The name of the field whose type is requested. This should match the field or
        /// property name used in filtering expressions.
        /// </param>
        /// <returns>
        /// A <see cref="FieldType"/> enum value indicating the data type of the field.
        /// Implementations should map the field name to its correct type to allow correct
        /// parsing and comparison of filter values.
        /// </returns>
        FieldType GetFieldType(string fieldName);

        /// <summary>
        /// Compares a field's current value on the object to a provided string value using
        /// the specified operator.
        /// </summary>
        /// <param name="field">
        /// The name of the field to evaluate. This should correspond to a field or property
        /// on the implementing object.
        /// </param>
        /// <param name="value">
        /// The comparison value represented as a string. Implementations are responsible for
        /// converting this string to the appropriate type (based on <see cref="GetFieldType"/>) 
        /// prior to comparison.
        /// </param>
        /// <param name="oper">
        /// The operator to use for comparison (for example: "==", "!=", "&gt;", "&lt;", "Contains").
        /// The exact set of supported operators should be documented by the implementation.
        /// </param>
        /// <returns>
        /// True if the field's value satisfies the comparison with the provided value using
        /// the operator; otherwise false. Implementations should handle type conversion errors
        /// and decide whether to treat them as comparison failures (return false) or to throw.
        /// </returns>
        bool ValueOnFieldComparison(string field, string value, string oper);
    }
}