using System;

namespace ZidUtilities.CommonCode.Win.Forms
{
    /// <summary>
    /// Defines the expected format for text input in TextInputDialog.
    /// Used for validation and input guidance.
    /// </summary>
    public enum TextInputFormat
    {
        /// <summary>
        /// No specific format required - accepts any text
        /// </summary>
        None,

        /// <summary>
        /// Email address format (contains @ and domain)
        /// </summary>
        Email,

        /// <summary>
        /// Phone number format (digits, spaces, dashes, parentheses allowed)
        /// </summary>
        PhoneNumber,

        /// <summary>
        /// Numeric value (integer or decimal)
        /// </summary>
        Numeric,

        /// <summary>
        /// Integer value only
        /// </summary>
        Integer,

        /// <summary>
        /// Custom format (validation provided by caller via event or regex)
        /// </summary>
        Custom
    }
}
