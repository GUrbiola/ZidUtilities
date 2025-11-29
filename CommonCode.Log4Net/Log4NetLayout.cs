namespace ZidUtilities.CommonCode.Log4Net
{
    /// <summary>
    /// Represents configuration information for a log4net layout.
    /// </summary>
    /// <remarks>
    /// Instances of this class hold the information required to configure a log4net
    /// layout element, such as the layout type and an optional conversion pattern.
    /// </remarks>
    public class Log4NetLayout
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLayout"/> class.
        /// </summary>
        /// <remarks>
        /// This default constructor creates an empty <see cref="Log4NetLayout"/> instance.
        /// After construction, callers typically set the <see cref="Type"/> and/or
        /// <see cref="ConversionPattern"/> properties to configure the layout.
        /// </remarks>
        public Log4NetLayout()
        {
        }

        /// <summary>
        /// Gets or sets the fully-qualified type name of the log4net layout to use.
        /// </summary>
        /// <value>
        /// A string containing the layout type, for example
        /// "log4net.Layout.PatternLayout" or another layout implementation's fully-qualified name.
        /// </value>
        /// <remarks>
        /// The <see cref="Type"/> property is used by configuration code to instantiate
        /// the appropriate layout implementation at runtime.
        /// </remarks>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the conversion pattern for layouts that support pattern formatting.
        /// </summary>
        /// <value>
        /// A conversion pattern string (for example, "%date %-5level %logger - %message%newline") used
        /// by pattern-based layouts to format log event data.
        /// </value>
        /// <remarks>
        /// Only certain layout types (such as <c>PatternLayout</c>) use this property; other
        /// layout implementations may ignore it. Set this property when you need to control
        /// the textual formatting of log messages.
        /// </remarks>
        public string ConversionPattern { get; set; }
    }
}
