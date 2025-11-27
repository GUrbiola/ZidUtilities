using System;
using System.Collections;

namespace ZidUtilities.CommonCode.DifferenceEngine.Structure
{
    /// <summary>
    /// Represents the type of change for a span in a difference/diff result.
    /// Use this enum to indicate whether a span has no change, was replaced,
    /// was deleted from the source, or was added to the destination.
    /// </summary>
    /// <remarks>
    /// This enum does not have parameters or return values. It is used as a value
    /// to describe the status of a span when comparing two sequences.
    /// </remarks>
    public enum DiffResultSpanStatus
    {
        /// <summary>
        /// Indicates that the span has no changes between source and destination.
        /// </summary>
        /// <remarks>
        /// No parameters. No return value. Use this value when the content is identical.
        /// </remarks>
        NoChange,

        /// <summary>
        /// Indicates that the span in the destination replaces the span in the source.
        /// </summary>
        /// <remarks>
        /// No parameters. No return value. Use this value when the content differs and
        /// the destination's content should replace the source's content.
        /// </remarks>
        Replace,

        /// <summary>
        /// Indicates that the span from the source was deleted (not present in destination).
        /// </summary>
        /// <remarks>
        /// No parameters. No return value. Use this value when the source contains content
        /// that does not exist in the destination and should be considered removed.
        /// </remarks>
        DeleteSource,

        /// <summary>
        /// Indicates that the span was added to the destination (not present in source).
        /// </summary>
        /// <remarks>
        /// No parameters. No return value. Use this value when new content exists in the
        /// destination that did not exist in the source.
        /// </remarks>
        AddDestination
    }
}
