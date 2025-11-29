using System.Collections.Generic;
using System.Text;

namespace ZidUtilities.CommonCode.Log4Net
{
    /// <summary>
    /// Represents the root logger configuration for a log4net configuration fragment.
    /// This class produces the XML fragment for the &lt;root&gt; element including the
    /// configured level and references to appenders.
    /// </summary>
    public class Log4NetRoot
    {
        /// <summary>
        /// Gets or sets the logging level for the root logger.
        /// Typical values are "ALL", "DEBUG", "INFO", "WARN", "ERROR", "FATAL".
        /// The default value is "ALL".
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetRoot"/> class.
        /// The constructor sets the default <see cref="Level"/> to "ALL".
        /// </summary>
        public Log4NetRoot()
        {
            Level = "ALL";
        }

        /// <summary>
        /// Builds an XML fragment representing the &lt;root&gt; configuration element.
        /// The produced XML contains a &lt;level value="..." /&gt; element and one
        /// &lt;appender-ref ref="..." /&gt; element for each appender in <paramref name="appenders"/>.
        /// </summary>
        /// <param name="appenders">
        /// A list of <see cref="IAppender"/> instances that should be referenced from the root logger.
        /// Each appender's <see cref="IAppender.Name"/> property is used as the value of the
        /// generated &lt;appender-ref ref="..." /&gt; entries. This parameter must not be null;
        /// if empty, the returned XML will contain no &lt;appender-ref&gt; elements.
        /// </param>
        /// <returns>
        /// A string containing the XML representation of the &lt;root&gt; element including
        /// the configured level and appender references. The returned string ends with a newline.
        /// </returns>
        public string ToString(List<IAppender> appenders)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<root>");
            sb.AppendLine($"  <level value=\"{Level}\" />");
            foreach (IAppender appender in appenders)
                sb.AppendLine($"  <appender-ref ref=\"{appender.Name}\" />");
            sb.AppendLine("</root>");
            return sb.ToString();
        }
    }
}
