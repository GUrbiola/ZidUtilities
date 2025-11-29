using System;
using System.Collections.Generic;
using System.Text;


namespace CommonCode.DataAccess.ActiveDirectory
{
    /// <summary>
    /// The ad attribute. Is meant to represent an attribute that will be pulled by the AdManager.
    /// </summary>
    public class AdAttribute
    {
        /// <summary>
        /// Constant that represent the value "never" regarding the expiration date on the AD account.
        /// </summary>
        private const long NonExpiringTickValue = 9223372036854775807;

        /// <summary>
        /// Function that will take care of the parsing for special fields
        /// </summary>
        private GetAttributeValue parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdAttribute"/> class.
        /// </summary>
        /// <param name="adName">
        /// The name of the attribute in AD.
        /// </param>
        /// <param name="alias">
        /// The alias that will be used for mapping on the retrieved results.
        /// If left empty the <paramref name="adName"/> value will be used as alias.
        /// </param>
        /// <param name="type">
        /// The type of value expected on the attribute. Defaults to <see cref="AdType.String"/>.
        /// </param>
        /// <param name="parser">
        /// Optional delegate used to parse attribute raw values when <see cref="AdType.Special"/> is used.
        /// The parser will receive the raw attribute object and should return a string representation.
        /// If null and the type is <see cref="AdType.Special"/>, the raw object's ToString() will be used.
        /// </param>
        public AdAttribute(string adName, string alias = "", AdType type = AdType.String, GetAttributeValue parser = null)
        {
            this.AdName = adName;
            this.Alias = string.IsNullOrEmpty(alias) ? adName : alias;
            this.Type = type;
            this.parser = parser;
        }

        /// <summary>
        /// Gets or sets the ad name.
        /// </summary>
        public string AdName { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public AdType Type { get; set; }

        /// <summary>
        /// Attempts to evaluate the raw value on the attribute and convert it into a meaning value.
        /// </summary>
        /// <param name="raw">
        /// The raw value returned by Active Directory for this attribute. This may be null.
        /// Supported runtime types depend on <see cref="Type"/>:
        /// - <see cref="AdType.String"/>: expects a string or an object with a meaningful ToString().
        /// - <see cref="AdType.Integer"/>: expects an integer-compatible value.
        /// - <see cref="AdType.DateTime"/>: expects a long representing FILETIME/AD ticks.
        /// - <see cref="AdType.AccountStatus"/>: expects an integer representing UserAccountControl flags.
        /// - <see cref="AdType.Special"/>: uses the provided parser delegate when available.
        /// - <see cref="AdType.StringList"/>: expects an enumerable of items convertible to string.
        /// </param>
        /// <returns>
        /// A string representation of the evaluated value. Returns empty string when <paramref name="raw"/> is null
        /// or when the conversion yields no meaningful value.
        /// </returns>
        public string Evaluate(object raw)
        {
            string back = string.Empty;

            if (raw != null)
            {
                switch (this.Type)
                {
                    case AdType.Integer:
                        back = ((int)raw).ToString();
                        break;
                    case AdType.DateTime:
                        long ticks = long.Parse(raw.ToString());
                        DateTime? buffDt = this.TicksToDate(ticks);

                        back = buffDt.HasValue ? buffDt.Value.ToString("yyyyMMdd HHmmss") : string.Empty;
                        break;
                    case AdType.AccountStatus:
                        int buff = Convert.ToInt32(raw);

                        if ((buff & (int)UserAccountControl.ACCOUNTDISABLE) == (int)UserAccountControl.ACCOUNTDISABLE)
                        {
                            back = "Disabled";
                        }
                        else if ((buff & (int)UserAccountControl.LOCKOUT) == (int)UserAccountControl.LOCKOUT)
                        {
                            back = "AD Locked";
                        }
                        else if ((buff & (int)UserAccountControl.NORMAL_ACCOUNT) == (int)UserAccountControl.NORMAL_ACCOUNT)
                        {
                            back = "Active";
                        }
                        else
                        {
                            back = "Unknown";
                        }
                        break;
                    case AdType.Special:
                        back = this.parser == null ? raw.ToString() : this.parser(raw);
                        break;
                    case AdType.StringList:
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in (System.Collections.IEnumerable)raw)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(";");
                            }
                            sb.Append(item.ToString());
                        }
                        back = sb.ToString();
                        break;
                    case AdType.String:
                    default:
                        back = raw.ToString();
                        break;
                }
            }

            return back;
        }

        /// <summary>
        /// Property used for updating values in AD
        /// </summary>
        public string UpdateValue { get; set; }

        /// <summary>
        /// Property used for updating list string values in AD
        /// </summary>
        public List<string> UpdateListValue { get; set; }

        /// <summary>
        /// Private function, that converts ticks(the way dates are stored in AD) to a date string.
        /// </summary>
        /// <param name="ticks">
        /// The ticks value retrieved from Active Directory. AD stores times as 100-nanosecond intervals
        /// since January 1, 1601 (UTC). A special constant <see cref="NonExpiringTickValue"/> denotes 'never'.
        /// </param>
        /// <returns>
        /// A <see cref="DateTime"/> representing the converted ticks in local time context of the method,
        /// wrapped in a nullable. Returns null when the ticks represent the non-expiring sentinel or are non-positive.
        /// </returns>
        private DateTime? TicksToDate(long ticks)
        {
            if (ticks != NonExpiringTickValue && ticks > 0)
            {
                DateTime beginningOfTimes = new DateTime(1601, 1, 1).Subtract(new TimeSpan(1, 0, 0, 0));
                return beginningOfTimes.AddTicks(ticks);
            }

            return null;
        }
    }
}
