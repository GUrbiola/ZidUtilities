namespace ZidUtilities.CommonCode.Log4Net
{
    /// <summary>
    /// Represents a single member of a Log4Net JSON-like structure.
    /// This class holds a name and a value and provides a string representation
    /// formatted as an XML-like element used by Log4Net converters.
    /// </summary>
    public class Log4NetJsonMember
    {
        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> containing the member's name. May be null.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the member.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> containing the member's value. May be null.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Returns a string that represents the current object formatted as an XML-like element.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> in the format:
        /// <c>&lt;member value="Name:Value" /&gt;</c>.
        /// If <see cref="Name"/> or <see cref="Value"/> is null, the literal "null" will appear
        /// in its place as produced by string interpolation.
        /// </returns>
        public override string ToString()
        {
            return $"<member value=\"{Name}:{Value}\" />";
        }
    }
}
