using System.Collections.Generic;

namespace ZidUtilities.CommonCode.DataComparison
{
    /// <summary>
    /// Represents a container for a set of comment strings produced during data comparison operations.
    /// </summary>
    /// <remarks>
    /// Instances of this class hold zero or more comment messages that describe comparison results,
    /// warnings, or other notes associated with a single row or comparison unit.
    /// </remarks>
    public class CommentRow
    {
        /// <summary>
        /// The collection of comment strings for this row.
        /// </summary>
        /// <value>
        /// A <see cref="List{String}"/> containing each comment related to this row.
        /// The list is non-null and is initialized by the default constructor.
        /// </value>
        public List<string> Comments;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentRow"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor creates an empty <see cref="List{String}"/> and assigns it to the
        /// <see cref="Comments"/> field so callers can immediately add comment strings.
        /// </remarks>
        public CommentRow()
        {
            Comments = new List<string>();
        }
    }
}