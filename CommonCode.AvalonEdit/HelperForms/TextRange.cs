namespace ZidUtilities.CommonCode.AvalonEdit.HelperForms
{
    /// <summary>
    /// Represents a contiguous range of text defined by an offset and length.
    /// </summary>
    public class TextRange
    {
        /// <summary>
        /// Gets or sets the zero-based offset in the document where the range starts.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the length of the range in number of characters.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRange"/> class.
        /// </summary>
        public TextRange()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRange"/> class.
        /// </summary>
        /// <param name="offset">The zero-based offset in the document where the range starts.</param>
        /// <param name="length">The length of the range in number of characters.</param>
        public TextRange(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }
    }
}
