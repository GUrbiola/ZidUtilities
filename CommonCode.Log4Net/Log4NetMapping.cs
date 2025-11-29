namespace ZidUtilities.CommonCode.Log4Net
{
    /// <summary>
    /// Represents a mapping between a log level and the console colors used to render that level.
    /// Use instances of this class to configure how log entries at a given <see cref="Level"/> should
    /// be displayed (foreground and background colors).
    /// </summary>
    public class Log4NetMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetMapping"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor does not take any parameters and does not return a value.
        /// After construction, set the <see cref="Level"/>, <see cref="ForeColor"/>, and
        /// <see cref="BackColor"/> properties as needed.
        /// </remarks>
        public Log4NetMapping()
        {
        }

        /// <summary>
        /// Gets or sets the log level this mapping applies to.
        /// </summary>
        /// <value>
        /// A string representing the log level (for example: "DEBUG", "INFO", "WARN", "ERROR", "FATAL").
        /// </value>
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets the foreground color to use when rendering log entries of the specified <see cref="Level"/>.
        /// </summary>
        /// <value>
        /// A string describing the foreground color. This can be a color name (e.g., "Red") or a color code,
        /// depending on the consumer's expected format.
        /// </value>
        public string ForeColor { get; set; }

        /// <summary>
        /// Gets or sets the background color to use when rendering log entries of the specified <see cref="Level"/>.
        /// </summary>
        /// <value>
        /// A string describing the background color. This can be a color name (e.g., "Black") or a color code,
        /// depending on the consumer's expected format.
        /// </value>
        public string BackColor { get; set; }
    }
}
