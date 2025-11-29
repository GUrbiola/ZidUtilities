using ICSharpCode.TextEditor.Document;

namespace ZidUtilities.CommonCode.ICSharpTextEditor.HelperForms  
{
    /// <summary>
    /// Represents a contiguous range of text within an <see cref="IDocument"/>.
    /// The class inherits from <see cref="AbstractSegment"/>, providing the
    /// <c>offset</c> and <c>length</c> members that define the segment.
    /// </summary>
    public class TextRange : AbstractSegment
    {
        /// <summary>
        /// The underlying document that this text range refers to.
        /// </summary>
        private IDocument _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRange"/> class.
        /// </summary>
        /// <param name="document">The <see cref="IDocument"/> that contains the text for this range. Must not be null.</param>
        /// <param name="offset">The zero-based offset in the document where the range starts.</param>
        /// <param name="length">The length of the range in number of characters. Must be greater than or equal to zero.</param>
        /// <remarks>
        /// The constructor stores a reference to the supplied <paramref name="document"/>
        /// and sets the inherited <c>offset</c> and <c>length</c> fields to define the segment.
        /// After construction, the instance represents the slice of text starting at
        /// <paramref name="offset"/> and spanning <paramref name="length"/> characters.
        /// </remarks>
        public TextRange(IDocument document, int offset, int length)
        {
            _document = document;
            this.offset = offset;
            this.length = length;
        }
    }
}
