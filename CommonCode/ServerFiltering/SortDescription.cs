namespace ZidUtilities.CommonCode.ServerFiltering
{
    /// <summary>
    /// Represents the sorting of 1 field, and the direction of this sorting
    /// </summary>
    public class SortDescription
    {
        /// <summary>
        /// Sorted field
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// Direction of the sort
        /// </summary>
        public string dir { get; set; }
    }
}