//#define USE_HASH_TABLE

using System;
using System.Collections;

namespace ZidUtilities.CommonCode.DifferenceEngine.Structure
{
	/// <summary>
	/// Represents a contiguous span of difference between a source and a destination sequence.
	/// The span records the start indexes in source/destination, the length, and the kind of change.
	/// </summary>
	public class DiffResultSpan : IComparable
	{
		/// <summary>
		/// Special value used for unavailable index positions.
		/// </summary>
		private const int BAD_INDEX = -1;

		private int _destIndex;
		private int _sourceIndex;
		private int _length;
		private DiffResultSpanStatus _status;

		/// <summary>
		/// Gets the starting index in the destination sequence for this span.
		/// </summary>
		public int DestIndex { get { return _destIndex; } }

		/// <summary>
		/// Gets the starting index in the source sequence for this span.
		/// </summary>
		public int SourceIndex { get { return _sourceIndex; } }

		/// <summary>
		/// Gets the length (number of elements) covered by this span.
		/// </summary>
		public int Length { get { return _length; } }

		/// <summary>
		/// Gets the status of this span describing the type of change (NoChange, Replace, DeleteSource, AddDestination).
		/// </summary>
		public DiffResultSpanStatus Status { get { return _status; } }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DiffResultSpan"/> class.
		/// </summary>
		/// <param name="status">The type of change this span represents.</param>
		/// <param name="destIndex">The starting index in the destination sequence, or <see cref="BAD_INDEX"/> when not applicable.</param>
		/// <param name="sourceIndex">The starting index in the source sequence, or <see cref="BAD_INDEX"/> when not applicable.</param>
		/// <param name="length">The number of elements covered by the span (must be >= 0).</param>
		protected DiffResultSpan( DiffResultSpanStatus status, int destIndex, int sourceIndex, int length)
		{
			_status = status;
			_destIndex = destIndex;
			_sourceIndex = sourceIndex;
			_length = length;
		}

		/// <summary>
		/// Creates a span that represents no change between source and destination.
		/// </summary>
		/// <param name="destIndex">The starting index in the destination sequence for the unchanged block.</param>
		/// <param name="sourceIndex">The starting index in the source sequence for the unchanged block.</param>
		/// <param name="length">The length of the unchanged block.</param>
		/// <returns>A new <see cref="DiffResultSpan"/> describing an unchanged block.</returns>
		public static DiffResultSpan CreateNoChange(int destIndex, int sourceIndex, int length)
		{
			return new DiffResultSpan(DiffResultSpanStatus.NoChange,destIndex,sourceIndex,length); 
		}

		/// <summary>
		/// Creates a span that represents a replacement: source elements are replaced by destination elements.
		/// </summary>
		/// <param name="destIndex">The starting index in the destination sequence for the replaced block.</param>
		/// <param name="sourceIndex">The starting index in the source sequence for the replaced block.</param>
		/// <param name="length">The length of the replacement block (number of elements in the span).</param>
		/// <returns>A new <see cref="DiffResultSpan"/> describing a replacement block.</returns>
		public static DiffResultSpan CreateReplace(int destIndex, int sourceIndex, int length)
		{
			return new DiffResultSpan(DiffResultSpanStatus.Replace,destIndex,sourceIndex,length); 
		}

		/// <summary>
		/// Creates a span that represents deletion of elements from the source sequence.
		/// </summary>
		/// <param name="sourceIndex">The starting index in the source sequence for the deleted block.</param>
		/// <param name="length">The number of elements deleted from the source.</param>
		/// <returns>A new <see cref="DiffResultSpan"/> describing a deletion from the source. The destination index is set to <see cref="BAD_INDEX"/>.</returns>
		public static DiffResultSpan CreateDeleteSource(int sourceIndex, int length)
		{
			return new DiffResultSpan(DiffResultSpanStatus.DeleteSource,BAD_INDEX,sourceIndex,length); 
		}

		/// <summary>
		/// Creates a span that represents addition of elements to the destination sequence.
		/// </summary>
		/// <param name="destIndex">The starting index in the destination sequence where elements were added.</param>
		/// <param name="length">The number of elements added to the destination.</param>
		/// <returns>A new <see cref="DiffResultSpan"/> describing an addition to the destination. The source index is set to <see cref="BAD_INDEX"/>.</returns>
		public static DiffResultSpan CreateAddDestination(int destIndex, int length)
		{
			return new DiffResultSpan(DiffResultSpanStatus.AddDestination,destIndex,BAD_INDEX,length); 
		}

		/// <summary>
		/// Adds the specified number to the current length of the span.
		/// </summary>
		/// <param name="i">The amount to add to the length. Can be zero or positive; negative values will decrease the length.</param>
		/// <remarks>
		/// This method mutates the existing span instance by increasing its <see cref="Length"/>.
		/// </remarks>
		public void AddLength(int i)
		{
			_length += i;
		}

		/// <summary>
		/// Returns a string that represents the current span, including status, destination index, source index, and length.
		/// </summary>
		/// <returns>A human-readable representation of the span.</returns>
		public override string ToString()
		{
			return string.Format("{0} (Dest: {1},Source: {2}) {3}",
				_status.ToString(),
				_destIndex.ToString(),
				_sourceIndex.ToString(),
				_length.ToString());
		}
		#region IComparable Members

		/// <summary>
		/// Compares this instance to another object and returns an integer that indicates whether this instance
		/// precedes, follows, or occurs in the same position in the sort order as the other object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance. Expected to be a <see cref="DiffResultSpan"/>.</param>
		/// <returns>
		/// A signed integer that indicates the relative values of this instance and <paramref name="obj"/>.
		/// Less than zero if this instance precedes <paramref name="obj"/> in the sort order,
		/// zero if they are equal, greater than zero if this instance follows <paramref name="obj"/>.
		/// Comparison is performed using the <see cref="DestIndex"/>.
		/// </returns>
		public int CompareTo(object obj)
		{
			return _destIndex.CompareTo(((DiffResultSpan)obj)._destIndex);
		}

		#endregion
	}
}