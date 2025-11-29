using System.Text;

namespace ZidUtilities.CommonCode.Log4Net
{
    public class RollingFileAppender : IAppender
    {
        public string Name { get; set; }
        public string Type { get { return "log4net.Appender.RollingFileAppender"; } }
        public string LoggingLevel { get; set; }
        public AppenderType AppenderType { get { return AppenderType.RollingFileAppender; } }
        public string FilePath { get; set; }
        public bool AppendToFile { get; set; }
        public string RollingStyle { get; set; }

        public string MaxFileSize { get; set; }
        public int MaxSizeRollBackups { get; set; }

        public string DatePattern { get; set; }

        public Log4NetLayout Layout { get; set; }

        /// <summary>
        /// Creates and returns a default <see cref="Log4NetLayout"/> configured for use
        /// with a rolling file appender.
        /// </summary>
        /// <remarks>
        /// This method does not accept any parameters. It constructs a new
        /// <see cref="Log4NetLayout"/> with the default layout type
        /// ("log4net.Layout.PatternLayout") and a default conversion pattern suitable
        /// for rolling file appenders.
        /// </remarks>
        /// <returns>
        /// A new <see cref="Log4NetLayout"/> instance initialized with default
        /// values for <see cref="Log4NetLayout.Type"/> and
        /// <see cref="Log4NetLayout.ConversionPattern"/>.
        /// </returns>
        public static Log4NetLayout DefaultLayout()
        {
            return new Log4NetLayout
            {
                Type = "log4net.Layout.PatternLayout",
                ConversionPattern = "[%date] [%thread] [%-5level] %message%newline%exception"
            };
        }

        /// <summary>
        /// Constructs a default <see cref="RollingFileAppender"/> instance and returns it
        /// as the <see cref="IAppender"/> interface type.
        /// </summary>
        /// <remarks>
        /// This method does not accept any parameters. It creates a new
        /// <see cref="RollingFileAppender"/> configured with sensible defaults for:
        /// name, logging level, file path, append behavior, rolling style, date pattern,
        /// and layout (using <see cref="DefaultLayout"/>).
        /// </remarks>
        /// <returns>
        /// An <see cref="IAppender"/> representing a preconfigured rolling file appender
        /// ready to be used in log4net configuration generation.
        /// </returns>
        public static IAppender GetDefault()
        {
            RollingFileAppender appender = new RollingFileAppender
            {
                Name = "RollingFileAppender",
                LoggingLevel = "ALL",
                FilePath = "logs/Log.txt",
                AppendToFile = true,
                RollingStyle = "Date",
                DatePattern = "yyyyMMdd",
                Layout = DefaultLayout()
            };
            return appender;
        }

        /// <summary>
        /// Builds the log4net XML configuration fragment that represents this appender.
        /// </summary>
        /// <remarks>
        /// This method does not accept parameters. It reads the current instance
        /// properties (for example <see cref="FilePath"/>, <see cref="AppendToFile"/>,
        /// <see cref="RollingStyle"/>, <see cref="MaxFileSize"/>, and
        /// <see cref="Layout"/>) and emits a string containing the corresponding
        /// log4net XML configuration for a rolling file appender.
        /// </remarks>
        /// <returns>
        /// A <see cref="string"/> containing the XML configuration fragment for this
        /// rolling file appender built from the instance's property values.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string t = "true", f = "false";
            sb.AppendLine($"<appender name=\"{Name}\" type=\"{Type}\">");

            if (!string.IsNullOrEmpty(LoggingLevel))
                sb.AppendLine($"  <level value=\"{LoggingLevel}\" />");

            sb.AppendLine($"  <file type=\"log4net.Util.PatternString\" value=\"{FilePath}\" />");
            sb.AppendLine($"  <appendToFile value=\"{(AppendToFile ? f : t)}\" />");
            sb.AppendLine($"  <rollingStyle value=\"{RollingStyle}\" />");

            if (RollingStyle == "Size")
            {
                sb.AppendLine($"  <maxSizeRollBackups value=\"{MaxSizeRollBackups}\" />");
                sb.AppendLine($"  <maximumFileSize value=\"{MaxFileSize}\" />");
            }
            else if (RollingStyle == "Date")
            {
                sb.AppendLine($"  <datePattern value=\"{DatePattern}\" />");
            }

            sb.AppendLine($"  <layout type=\"{Layout.Type}\">");
            sb.AppendLine($"    <conversionPattern value=\"{Layout.ConversionPattern}\" />");
            sb.AppendLine($"  </layout>");
            sb.AppendLine($"</appender>");
            return sb.ToString();
        }
    }
}
