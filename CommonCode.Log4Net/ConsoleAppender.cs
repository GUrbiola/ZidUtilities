using System.Collections.Generic;
using System.Text;

namespace ZidUtilities.CommonCode.Log4Net
{
    /// <summary>
    /// Represents a colored console appender configuration for log4net.
    /// Provides properties to define the appender name, type, logging level,
    /// color mappings and layout, and methods to produce default configurations
    /// and the XML representation.
    /// </summary>
    public class ConsoleAppender : IAppender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAppender"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor does not perform any initialization beyond the default.
        /// Use the static <see cref="GetDefault"/> method to obtain a preconfigured appender.
        /// </remarks>
        public ConsoleAppender()
        {
        }

        public string Name { get; set; }
        public string Type { get { return "log4net.Appender.ColoredConsoleAppender"; } }
        public string LoggingLevel { get; set; }
        public AppenderType AppenderType { get { return AppenderType.ConsoleAppender; } }
        public List<Log4NetMapping> Mappings { get; set; }
        public Log4NetLayout Layout { get; set; }

        /// <summary>
        /// Builds and returns the XML string representation of this console appender.
        /// </summary>
        /// <returns>
        /// A string containing the log4net XML configuration for this appender,
        /// including level, mappings and layout elements.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<appender name=\"{Name}\" type=\"{Type}\">");

            if (!string.IsNullOrEmpty(LoggingLevel))
                sb.AppendLine($"  <level value=\"{LoggingLevel}\" />");

            foreach (var mapping in Mappings)
            {
                sb.AppendLine($"  <mapping>");
                sb.AppendLine($"    <level value=\"{mapping.Level}\" />");
                sb.AppendLine($"    <foreColor value=\"{mapping.ForeColor}\" />");
                sb.AppendLine($"    <backColor value=\"{mapping.BackColor}\" />");
                sb.AppendLine($"  </mapping>");
            }
            sb.AppendLine($"  <layout type=\"{Layout.Type}\">");
            sb.AppendLine($"    <conversionPattern value=\"{Layout.ConversionPattern}\" />");
            sb.AppendLine($"  </layout>");
            sb.AppendLine($"</appender>");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the default set of color mappings for common log levels.
        /// </summary>
        /// <returns>
        /// A list of <see cref="Log4NetMapping"/> instances representing the default
        /// foreground and background colors for various logging levels.
        /// </returns>
        public static List<Log4NetMapping> DefaultMappings()
        {
            return new List<Log4NetMapping>
            {
                new Log4NetMapping { Level = "DEBUG", ForeColor = "White, HighIntensity", BackColor = "Blue" },
                new Log4NetMapping { Level = "INFO", ForeColor = "White, HighIntensity", BackColor = "Green" },
                new Log4NetMapping { Level = "WARN", ForeColor = "Yellow, HighIntensity", BackColor = "Purple" },
                new Log4NetMapping { Level = "ERROR", ForeColor = "Yellow, HighIntensity", BackColor = "Red" },
                new Log4NetMapping { Level = "FATAL", ForeColor = "White, HighIntensity", BackColor = "Red" }
            };
        }

        /// <summary>
        /// Returns the default layout configuration for the console appender.
        /// </summary>
        /// <returns>
        /// A <see cref="Log4NetLayout"/> configured with a pattern layout and a
        /// default conversion pattern including timestamp, thread, level, message and exception.
        /// </returns>
        public static Log4NetLayout DefaultLayout()
        {
            return new Log4NetLayout
            {
                Type = "log4net.Layout.PatternLayout",
                ConversionPattern = "[%date{HH:mm:ss}] [%thread] [%-5level] %message%newline%exception"
            };
        }

        /// <summary>
        /// Creates and returns a preconfigured <see cref="IAppender"/> instance
        /// for a colored console appender with default mappings and layout.
        /// </summary>
        /// <returns>
        /// An <see cref="IAppender"/> (specifically a <see cref="ConsoleAppender"/>) with
        /// Name set to "ConsoleAppender", LoggingLevel set to "ALL", and default mappings and layout.
        /// </returns>
        public static IAppender GetDefault()
        {
            ConsoleAppender appender = new ConsoleAppender
            {
                Name = "ConsoleAppender",
                LoggingLevel = "ALL",
                Mappings = DefaultMappings(),
                Layout = DefaultLayout()
            };
            return appender;
        }
    }
}
