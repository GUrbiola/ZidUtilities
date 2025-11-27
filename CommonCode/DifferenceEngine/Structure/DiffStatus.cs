using System;
using System.Collections;

namespace ZidUtilities.CommonCode.DifferenceEngine.Structure
{
    /// <summary>
    /// Represents the comparison status of two items produced by the difference engine.
    /// </summary>
    /// <remarks>
    /// This enum is used to indicate whether two units (for example, lines or tokens)
    /// matched, did not match, or their match status is unknown. There are no parameters
    /// or return values associated with an enum type; it simply provides named constants.
    /// </remarks>
    internal enum DiffStatus
    {
        /// <summary>
        /// Indicates that the compared items matched.
        /// </summary>
        /// <remarks>
        /// Numeric value: 1.
        /// Parameters: None. Return value: Not applicable.
        /// </remarks>
        Matched = 1,

        /// <summary>
        /// Indicates that the compared items did not match.
        /// </summary>
        /// <remarks>
        /// Numeric value: -1.
        /// Parameters: None. Return value: Not applicable.
        /// </remarks>
        NoMatch = -1,

        /// <summary>
        /// Indicates that the comparison status of the items is unknown or has not been determined.
        /// </summary>
        /// <remarks>
        /// Numeric value: -2.
        /// Parameters: None. Return value: Not applicable.
        /// </remarks>
        Unknown = -2
    }
}
