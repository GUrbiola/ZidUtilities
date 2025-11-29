using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.Log4Net.Read
{
    public class Log4NetRecord
    {
        public string dateFormat { get; private set; }
        public DateTime date { get; set; }
        public string thread { get; set; }
        public MessageLevel level { get; set; }
        public string logger { get; set; }
        public string message { get; set; }
        public string exception { get; set; }
        public string username { get; set; }
        public string identity { get; set; }
        public string location { get; set; }
        public string _class { get; set; }
        public string method { get; set; }
        public string file { get; set; }
        public string line { get; set; }

        public Dictionary<string, string> custom { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetRecord"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor initializes the <see cref="custom"/> dictionary with a case-insensitive
        /// string comparer and sets the default <see cref="dateFormat"/> to "yyyy-MM-dd HH:mm:ss,fff".
        /// </remarks>
        public Log4NetRecord()
        {
            custom = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            dateFormat = "yyyy-MM-dd HH:mm:ss,fff";
        }

        /// <summary>
        /// Attempts to set the date format used by this record when parsing or formatting the <see cref="date"/>.
        /// </summary>
        /// <param name="format">A date/time format string to use for parsing/formatting. Example: "yyyy-MM-dd HH:mm:ss,fff".</param>
        /// <returns>
        /// True if the provided <paramref name="format"/> is non-empty and can successfully parse a canonical
        /// example date string ("2023-01-01 12:00:00,000"); otherwise false.
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="DateTime.TryParseExact(string,string,IFormatProvider,System.Globalization.DateTimeStyles,out DateTime)"/>
        /// to validate the format against the fixed example "2023-01-01 12:00:00,000". If validation succeeds,
        /// the internal <see cref="dateFormat"/> is updated. No exception is thrown for invalid input.
        /// </remarks>
        public bool SetDateFormat(string format)
        {
            DateTime helper;
            if (!String.IsNullOrEmpty(format) && DateTime.TryParseExact("2023-01-01 12:00:00,000", format, null, System.Globalization.DateTimeStyles.None, out helper))
            {
                dateFormat = format;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a verbose string representation of this record.
        /// </summary>
        /// <returns>
        /// A string containing the <see cref="date"/>, <see cref="thread"/>, <see cref="level"/>, <see cref="logger"/>, 
        /// <see cref="message"/>, and <see cref="exception"/> values in a human-readable format.
        /// </returns>
        public override string ToString()
        {
            return $"{date} [{thread}] {level} {logger} - {message} {exception}";
        }

        /// <summary>
        /// Returns a shorter string representation of this record.
        /// </summary>
        /// <returns>
        /// A compact string containing the <see cref="date"/>, <see cref="level"/>, <see cref="logger"/>, and <see cref="message"/>.
        /// </returns>
        public string ToShortString()
        {
            return $"{date} {level} {logger} - {message}";
        }

        /// <summary>
        /// Indexer to get or set record fields by name.
        /// </summary>
        /// <param name="key">
        /// The field name to get or set. Recognized keys (case-insensitive) are:
        /// "date", "thread", "level", "logger", "message", "exception", "location".
        /// Any other key is stored in or retrieved from the <see cref="custom"/> dictionary.
        /// </param>
        /// <value>
        /// For the getter: returns the string representation of the requested field or the corresponding
        /// value from <see cref="custom"/>. For "date" the value is formatted using <see cref="dateFormat"/>.
        /// Returns null if <paramref name="key"/> is null/empty or not found.
        /// For the setter: assigns the provided string to the matching field. Special parsing rules:
        /// - "date": parsed using <see cref="dateFormat"/> via <see cref="DateTime.TryParseExact"/>.
        /// - "level": parsed into the <see cref="MessageLevel"/> enum via <see cref="Enum.TryParse{TEnum}(string,bool,out TEnum)"/>.
        /// Other recognized keys are assigned directly; unknown keys are placed into <see cref="custom"/>.
        /// </value>
        /// <returns>
        /// When used as a getter, returns the string value for the specified key or null if missing.
        /// When used as a setter, no return value is produced.
        /// </returns>
        public string this[string key]
        {
            get
            {
                if (String.IsNullOrEmpty(key))
                    return null;

                if (key.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    return date.ToString(dateFormat);
                }
                else if (key.Equals("thread", StringComparison.OrdinalIgnoreCase))
                {
                    return thread;
                }
                else if (key.Equals("level", StringComparison.OrdinalIgnoreCase))
                {
                    return level.ToString();
                }
                else if (key.Equals("logger", StringComparison.OrdinalIgnoreCase))
                {
                    return logger;
                }
                else if (key.Equals("message", StringComparison.OrdinalIgnoreCase))
                {
                    return message;
                }
                else if (key.Equals("exception", StringComparison.OrdinalIgnoreCase))
                {
                    return exception;
                }
                else if (key.Equals("username", StringComparison.OrdinalIgnoreCase))
                {
                    return username;
                }
                else if (key.Equals("identity", StringComparison.OrdinalIgnoreCase))
                {
                    return identity;
                }
                else if (key.Equals("location", StringComparison.OrdinalIgnoreCase))
                {
                    return location;
                }
                else if (key.Equals("class", StringComparison.OrdinalIgnoreCase) || key.Equals("_class", StringComparison.OrdinalIgnoreCase))
                {
                    return _class;
                }
                else if (key.Equals("method", StringComparison.OrdinalIgnoreCase))
                {
                    return method;
                }
                else if (key.Equals("line", StringComparison.OrdinalIgnoreCase))
                {
                    return line;
                }
                else if (key.Equals("file", StringComparison.OrdinalIgnoreCase))
                {
                    return file;
                }
                else
                {
                    if (custom.ContainsKey(key))
                    {
                        return custom[key];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                if (String.IsNullOrEmpty(key))
                    return;

                if (key.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    if (DateTime.TryParseExact(value, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dateValue))
                    {
                        date = dateValue;
                    }
                }
                else if (key.Equals("thread", StringComparison.OrdinalIgnoreCase))
                {
                    thread = value;
                }
                else if (key.Equals("level", StringComparison.OrdinalIgnoreCase))
                {
                    if (Enum.TryParse(value, true, out MessageLevel levelValue))
                    {
                        level = levelValue;
                    }
                }
                else if (key.Equals("logger", StringComparison.OrdinalIgnoreCase))
                {
                    logger = value;
                }
                else if (key.Equals("message", StringComparison.OrdinalIgnoreCase))
                {
                    message = value;
                }
                else if (key.Equals("exception", StringComparison.OrdinalIgnoreCase))
                {
                    exception = value;
                }
                else if (key.Equals("location", StringComparison.OrdinalIgnoreCase))
                {
                    location = value;
                }
                else
                {
                    custom[key] = value;
                }
            }
        }

    }
}
