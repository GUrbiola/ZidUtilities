namespace ZidUtilities.CommonCode.Log4Net
{
    /// <summary>
    /// Specifies the supported types of appenders for log4net configuration.
    /// </summary>
    /// <remarks>
    /// This enum lists the available appender implementations that callers can
    /// use when selecting a logging output destination. Enum members represent
    /// named constant values and do not accept parameters or return values.
    /// </remarks>
    public enum AppenderType
    {
        /// <summary>
        /// Writes log output to the console (standard output).
        /// </summary>
        /// <remarks>No parameters or return value; use this value to select console logging.</remarks>
        ConsoleAppender,

        /// <summary>
        /// Writes log output to a rolling file where files are rotated based on size or date.
        /// </summary>
        /// <remarks>
        /// Use this value when persisted logs on disk with rotation are required.
        /// Configuration controls rotation policy; enum member itself has no parameters or return value.
        /// </remarks>
        RollingFileAppender,

        /// <summary>
        /// Writes log output in JSON format, suitable for structured logging and ingestion by log systems.
        /// </summary>
        /// <remarks>
        /// Use this value for structured JSON logs. The formatting and any additional options
        /// are provided by the appender implementation; the enum member represents the selection.
        /// </remarks>
        JsonAppender
    }
}
