using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZidUtilities.CommonCode.Log4Net
{
    /// <summary>
    /// Helper class to construct and persist log4net configuration XML.
    /// </summary>
    public class Log4NetConfigHelper
    {
        /// <summary>
        /// Gets or sets the root logging level for the generated log4net configuration.
        /// </summary>
        public string RootLevel { get; set; }

        /// <summary>
        /// Gets or sets the list of appenders used to generate the configuration.
        /// </summary>
        public List<IAppender> Appenders { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetConfigHelper"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor sets a sensible default logging level and initializes the appender list.
        /// </remarks>
        public Log4NetConfigHelper()
        {
            RootLevel = "ALL";
            Appenders = new List<IAppender>();
        }

        /// <summary>
        /// Builds a log4net configuration XML string from the helper's state.
        /// </summary>
        /// <param name="appenderNames">
        /// A list of appender names that will be referenced from the &lt;root&gt; element.
        /// Each name results in an &lt;appender-ref ref="name" /&gt; entry under &lt;root&gt;.
        /// </param>
        /// <returns>
        /// A string containing the complete log4net XML configuration.
        /// The returned XML includes the &lt;log4net&gt; root element, a &lt;root&gt; section
        /// with the configured <see cref="RootLevel"/> and appender references, followed by
        /// the textual representation of each configured appender from <see cref="Appenders"/>.
        /// </returns>
        public string GetConfigXmlString(List<string> appenderNames)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<log4net>");

            sb.AppendLine("  <root>");
            sb.AppendLine($"    <level value=\"{RootLevel}\" />");
            foreach (string appender in appenderNames)
                sb.AppendLine($"    <appender-ref ref=\"{appender}\" />");
            sb.AppendLine("  </root>");

            foreach (var appender in Appenders)
                sb.AppendLine(appender.ToString());

            sb.AppendLine("</log4net>");
            return sb.ToString();
        }

        /// <summary>
        /// Serializes the current log4net configuration to a file.
        /// </summary>
        /// <param name="filePath">
        /// The file system path where the configuration XML will be written.
        /// This method will overwrite the file if it already exists.
        /// </param>
        /// <remarks>
        /// The method collects appender names from the <see cref="Appenders"/> collection,
        /// generates the configuration XML using <see cref="GetConfigXmlString"/>, and writes
        /// the resulting content to disk using <see cref="File.WriteAllText(string, string)"/>.
        /// IO exceptions (for example <see cref="IOException"/> or <see cref="UnauthorizedAccessException"/>)
        /// may be thrown by the underlying file API and are not caught here.
        /// </remarks>
        public void SaveConfigToFile(string filePath)
        {
            List<string> appenderNames = Appenders.Select(a => a.Name).ToList();
            File.WriteAllText(filePath, GetConfigXmlString(appenderNames));
        }

        /// <summary>
        /// Builds an in-memory stream containing the generated log4net configuration XML.
        /// </summary>
        /// <returns>
        /// A readable <see cref="Stream"/> positioned at the beginning containing the
        /// UTF-8 encoded log4net configuration XML. The caller is responsible for
        /// disposing the returned stream.
        /// </returns>
        /// <remarks>
        /// This method:
        /// - Collects appender names from the <see cref="Appenders"/> collection,
        /// - Generates the XML using <see cref="GetConfigXmlString"/>,
        /// - Encodes the XML as UTF-8 and returns it as a <see cref="MemoryStream"/>.
        /// The returned stream is non-writable and ready for immediate reading.
        /// </remarks>
        public Stream GetConfigXmlStream()
        {
            // Guard against null Appenders collection.
            var appenderNames = (Appenders ?? new List<IAppender>()).Select(a => a.Name).ToList();

            // Generate XML string.
            string xmlString = GetConfigXmlString(appenderNames);

            // Encode to UTF-8 bytes and return a MemoryStream positioned at 0.
            // Use a non-writable MemoryStream backed by the byte array.
            byte[] bytes = Encoding.UTF8.GetBytes(xmlString);
            return new MemoryStream(bytes, writable: false);
        }
    }
}
