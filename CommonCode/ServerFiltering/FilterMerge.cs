using System.Collections.Generic;

namespace ZidUtilities.CommonCode.ServerFiltering
{
    /// <summary>
    /// Represents a merge of multiple <see cref="FilterDescription"/> instances with specified join logic
    /// between them. This class holds the filters and the logical connections (AND/OR) used to combine them.
    /// </summary>
    public class FilterMerge
    {
        /// <summary>
        /// Describes how individual filters are joined together when multiple filters exist.
        /// </summary>
        public enum FilterJoin
        {
            /// <summary>
            /// Join filters using OR logic. The combined result should match if any filter matches.
            /// </summary>
            OrLogic,
            /// <summary>
            /// Join filters using AND logic. The combined result should match only if all filters match.
            /// </summary>
            AndLogic,
            /// <summary>
            /// Represents a single filter (no join operation between multiple filters).
            /// </summary>
            SingleFilter
        }

        /// <summary>
        /// Gets or sets the list of filter descriptions contained in this merge.
        /// </summary>
        /// <remarks>
        /// Each entry is a <see cref="FilterDescription"/> representing a single condition (field/operator/value).
        /// The ordering of this list corresponds to the ordering expected by <see cref="JoinLogic"/>.
        /// </remarks>
        public List<FilterDescription> Filters { get; set; }

        /// <summary>
        /// Gets or sets the list of join logic items that describe how adjacent filters in <see cref="Filters"/>
        /// are combined.
        /// </summary>
        /// <remarks>
        /// The list length is typically <c>Filters.Count - 1</c> when more than one filter exists. Each element
        /// in this list applies between the filter at the same index and the next filter in <see cref="Filters"/>.
        /// </remarks>
        public List<FilterJoin> JoinLogic { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMerge"/> class with empty
        /// <see cref="Filters"/> and <see cref="JoinLogic"/> lists.
        /// </summary>
        /// <remarks>
        /// Use this constructor when you want to populate filters and join logic after construction.
        /// </remarks>
        public FilterMerge()
        {
            Filters = new List<FilterDescription>();
            JoinLogic = new List<FilterJoin>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMerge"/> class using the same join logic
        /// for each adjacency between the provided filters.
        /// </summary>
        /// <param name="joinLogic">
        /// The <see cref="FilterJoin"/> value to use for every join between adjacent filters. For example,
        /// specify <see cref="FilterJoin.OrLogic"/> to join all filters with OR.
        /// </param>
        /// <param name="filters">
        /// A params array of <see cref="FilterDescription"/> instances to include in the merge.
        /// The order supplied will be preserved in the <see cref="Filters"/> property.
        /// </param>
        /// <remarks>
        /// This constructor populates <see cref="Filters"/> with the provided <paramref name="filters"/>
        /// and fills <see cref="JoinLogic"/> with <paramref name="joinLogic"/> repeated
        /// <c>Filters.Count - 1</c> times (one entry for each adjacency between filters).
        /// </remarks>
        public FilterMerge(FilterJoin joinLogic, params FilterDescription[] filters)
        {
            Filters = new List<FilterDescription>();
            foreach (FilterDescription filter in filters)
            {
                Filters.Add(filter);
            }
            JoinLogic = new List<FilterJoin>();
            for (int i = 0; i < Filters.Count - 1; i++)
            {
                JoinLogic.Add(joinLogic);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMerge"/> class with the specified lists.
        /// </summary>
        /// <param name="filters">
        /// The list of <see cref="FilterDescription"/> instances to set as <see cref="Filters"/>.
        /// </param>
        /// <param name="joinLogic">
        /// The list of <see cref="FilterJoin"/> instances to set as <see cref="JoinLogic"/>.
        /// </param>
        /// <remarks>
        /// It is expected (but not enforced) that <paramref name="joinLogic"/> has a length of
        /// <c>filters.Count - 1</c> when multiple filters are present, so that each adjacency has a corresponding join value.
        /// This constructor does not validate the counts and directly assigns the provided lists.
        /// </remarks>
        public FilterMerge(List<FilterDescription> filters, List<FilterJoin> joinLogic)
        {
            Filters = filters;
            JoinLogic = joinLogic;
        }

    }
}