using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZidUtilities.CommonCode.Log4Net
{
    /// <summary>
    /// Represents a JSON file appender configuration for log4net.
    /// Provides helpers to create default configurations and to parse
    /// pattern layout text into JSON member mappings.
    /// </summary>
    public class JsonFileAppender : IAppender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileAppender"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor sets no defaults; properties should be assigned by the caller
        /// or by using the <see cref="GetDefault"/> factory method.
        /// </remarks>
        public JsonFileAppender()
        {
        }

        public string Name { get; set; }
        public string Type { get { return "log4net.Appender.RollingFileAppender"; } }
        public string LoggingLevel { get; set; }
        public AppenderType AppenderType { get { return AppenderType.JsonAppender; } }
        public string FilePath { get; set; }
        public bool AppendToFile { get; set; }
        public string RollingStyle { get; set; }

        public string MaxFileSize { get; set; }
        public int MaxSizeRollBackups { get; set; }

        public string DatePattern { get; set; }
        public string Layout { get { return "log4net.Layout.SerializedLayout, log4net.Ext.Json"; } }
        public List<Log4NetJsonMember> Members { get; set; }

        /// <summary>
        /// Creates and returns a list of default JSON members commonly used for logging.
        /// </summary>
        /// <returns>
        /// A <see cref="List{Log4NetJsonMember}"/> containing default members:
        /// date, level, logger, message (as messageObject), and exception.
        /// </returns>
        public static List<Log4NetJsonMember> GetDefaultJsonMembers()
        {

            return new List<Log4NetJsonMember>
            {

                new Log4NetJsonMember { Name = "date", Value = "date" },
                new Log4NetJsonMember { Name = "level", Value = "level" },
                new Log4NetJsonMember { Name = "logger", Value = "logger" },
                new Log4NetJsonMember { Name = "message", Value = "messageObject" },
                new Log4NetJsonMember { Name = "exception", Value = "exception" }
            };
        }

        /// <summary>
        /// Builds the XML configuration fragment for this appender.
        /// </summary>
        /// <returns>
        /// A formatted XML string representing the appender configuration suitable
        /// for embedding in a log4net configuration file.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string t = "true", f = "false";
            sb.AppendLine($"<appender name=\"{Name}\" type=\"{Type}\">");

            if (!string.IsNullOrEmpty(LoggingLevel))
                sb.AppendLine($"  <level value=\"{LoggingLevel}\" />");

            if (FilePath.EndsWith(".json"))
                sb.AppendLine($"  <file value=\"{FilePath}\" />");
            else
                sb.AppendLine($"  <file value=\"{FilePath}.json\" />");

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

            sb.AppendLine($"  <layout type=\"{Layout}\">");
            sb.AppendLine($"    <decorator type=\"log4net.Layout.Decorators.StandardTypesDecorator, log4net.Ext.Json\" />");
            foreach (var member in Members)
            {
                //sb.AppendLine($"    <member value=\"{member.Name}:{member.Value}\" />");
                sb.AppendLine($"    {member.ToString()}");
            }
            sb.AppendLine($"  </layout>");
            sb.AppendLine($"</appender>");
            return sb.ToString();
        }

        /// <summary>
        /// Creates and returns a default configured <see cref="JsonFileAppender"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="IAppender"/> instance pre-populated with common defaults:
        /// name, logging level, file path, append behavior, rolling style, date pattern,
        /// and default JSON members.
        /// </returns>
        public static IAppender GetDefault()
        {
            JsonFileAppender appender = new JsonFileAppender
            {
                Name = "JsonFileAppender",
                LoggingLevel = "ALL",
                FilePath = "logs/Log.json",
                AppendToFile = true,
                RollingStyle = "Date",
                DatePattern = "yyyyMMdd",
                Members = GetDefaultJsonMembers()
            };
            return appender;
        }

        /// <summary>
        /// Parses a log4net pattern layout string and maps recognized tokens to JSON members.
        /// </summary>
        /// <param name="text">
        /// The pattern layout text to parse (for example: "%date %level %logger - %message").
        /// May be null or whitespace; in that case the default JSON members are returned.
        /// </param>
        /// <returns>
        /// A list of <see cref="Log4NetJsonMember"/> representing the parsed members in the order
        /// they were discovered. If no tokens are recognized the method returns the default
        /// JSON members from <see cref="GetDefaultJsonMembers"/>.
        /// </returns>
        /// <remarks>
        /// Recognized tokens include common log4net pattern keywords such as %date, %level,
        /// %logger, %message, %exception and %property{...}. Unknown tokens are sanitized
        /// and included as best-effort members. Duplicate member names are omitted.
        /// </remarks>
        public static List<Log4NetJsonMember> GetMembersFromPattern(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return GetDefaultJsonMembers();

            var members = new List<Log4NetJsonMember>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Match tokens like %date{...}, %property{...} or simple %level
            var tokenPattern = @"%[a-zA-Z]+\{[^}]*\}|%[a-zA-Z]+";
            var matches = System.Text.RegularExpressions.Regex.Matches(text, tokenPattern);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                string token = match.Value; // e.g. %date{yyyy-MM-dd} or %level or %property{log4net:UserName}
                if (string.IsNullOrEmpty(token))
                    continue;

                // remove leading '%'
                string core = token.Substring(1);

                string name = null;
                string value = token; // default to the token text

                // handle property separately to extract inner name
                if (core.StartsWith("property", StringComparison.OrdinalIgnoreCase))
                {
                    // extract inner between braces
                    var propMatch = System.Text.RegularExpressions.Regex.Match(core, @"property\{([^}]*)\}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (propMatch.Success)
                    {
                        var inner = propMatch.Groups[1].Value.Trim();
                        // Map some known log4net property names to friendly names
                        if (inner.Equals("log4net:UserName", StringComparison.OrdinalIgnoreCase) ||
                            inner.Equals("username", StringComparison.OrdinalIgnoreCase))
                        {
                            name = "username";
                        }
                        else if (inner.Equals("log4net:Identity", StringComparison.OrdinalIgnoreCase) ||
                                 inner.Equals("identity", StringComparison.OrdinalIgnoreCase))
                        {
                            name = "identity";
                        }
                        else
                        {
                            // sanitize inner into a simple name: take last part after ':' or '.' and remove non-alphanum
                            var parts = inner.Split(new[] { ':', '.' }, StringSplitOptions.RemoveEmptyEntries);
                            var candidate = parts.LastOrDefault() ?? inner;
                            // keep only letters/digits/underscore
                            var sanitized = System.Text.RegularExpressions.Regex.Replace(candidate, @"[^\w]", "_");
                            name = sanitized.ToLowerInvariant();
                        }

                        value = "%property{" + inner + "}";
                    }
                    else
                    {
                        // fallback: use entire core text
                        name = "property";
                        value = token;
                    }
                }
                else if (core.StartsWith("date", StringComparison.OrdinalIgnoreCase) || core.StartsWith("d", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "date";
                }
                else if (core.StartsWith("thread", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "thread";
                }
                else if (core.IndexOf("level", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    value = name = "level";
                }
                else if (core.IndexOf("-5level", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    value = name = "level";
                }
                else if (core.StartsWith("logger", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "logger";
                }
                else if (core.StartsWith("message", StringComparison.OrdinalIgnoreCase) || core.StartsWith("m", StringComparison.OrdinalIgnoreCase))
                {
                    // for JSON we usually want the message object
                    name = "message";
                    value = "messageObject";
                }
                else if (core.StartsWith("exception", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "exception";
                }
                else if (core.StartsWith("location", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "location";
                }
                else if (core.StartsWith("username", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "username";
                }
                else if (core.StartsWith("identity", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "identity";
                }
                else if (core.StartsWith("file", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "file";
                }
                else if (core.StartsWith("line", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "line";
                }
                else if (core.StartsWith("class", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "class";
                }
                else if (core.StartsWith("method", StringComparison.OrdinalIgnoreCase))
                {
                    value = name = "method";
                }
                else if (core.StartsWith("newline", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                else
                {
                    // Generic fallback: use the token name (strip braces) as member name
                    var simple = core;
                    // remove {..} if present
                    var braceIndex = simple.IndexOf('{');
                    if (braceIndex >= 0)
                        simple = simple.Substring(0, braceIndex);
                    // sanitize
                    simple = System.Text.RegularExpressions.Regex.Replace(simple, @"[^\w]", "_").ToLowerInvariant();
                    name = simple;
                    value = token;
                }

                if (string.IsNullOrEmpty(name))
                    continue;

                if (!seen.Contains(name))
                {
                    seen.Add(name);
                    members.Add(new Log4NetJsonMember { Name = name, Value = value });
                }
            }

            if (members.Count == 0)
                return GetDefaultJsonMembers();

            return members;
        }
    }
}
