namespace CommonCode.Log4Net
{
    /// <summary>
    /// Represents a logging appender abstraction used by the logging system.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface provide a target for log messages (for example,
    /// console, rolling file, or JSON appenders). The interface exposes metadata such as
    /// the appender's name, type identifier, logging level and a strongly-typed
    /// <see cref="AppenderType"/> indicating the concrete appender category.
    /// </remarks>
    public interface IAppender
    {
        /// <summary>
        /// Gets or sets the name of the appender.
        /// </summary>
        /// <value>
        /// A string representing a human-friendly name for the appender instance.
        /// This can be used to identify the appender in configuration and diagnostics.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets the runtime type identifier or implementation type name for the appender.
        /// </summary>
        /// <value>
        /// A string that identifies the implementation type (for example, a fully-qualified
        /// type name or a short token used in configuration) for the appender.
        /// </value>
        string Type { get; }

        /// <summary>
        /// Gets or sets the configured logging level for this appender.
        /// </summary>
        /// <value>
        /// A string representing the minimum logging level that this appender will process
        /// (for example, "DEBUG", "INFO", "WARN", "ERROR"). Implementations should interpret
        /// this value according to the logging framework's level semantics.
        /// </value>
        string LoggingLevel { get; set; }

        /// <summary>
        /// Gets the <see cref="AppenderType"/> enumeration value indicating the appender category.
        /// </summary>
        /// <value>
        /// An <see cref="AppenderType"/> value describing whether the appender is a console,
        /// rolling file, JSON, or another supported category.
        /// </value>
        AppenderType AppenderType { get; }

        /// <summary>
        /// Returns a textual representation of the appender.
        /// </summary>
        /// <returns>
        /// A string containing a human-readable description of the appender. Typical implementations
        /// include the name, type and configured logging level to aid in diagnostics.
        /// </returns>
        string ToString();
    }
}
