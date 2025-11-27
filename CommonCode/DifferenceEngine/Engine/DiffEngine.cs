using System;
using System.Collections;
using ZidUtilities.CommonCode.DifferenceEngine.Structure;

namespace ZidUtilities.CommonCode.DifferenceEngine.Engine
{
    /// <summary>
    /// DiffEngine computes differences between two IDiffList instances.
    /// It finds matching spans and produces a sequence of DiffResultSpan records
    /// describing adds, deletes and unchanged ranges. The engine supports
    /// different matching levels via <see cref="DiffEngineLevel"/>.
    /// </summary>
    public class DiffEngine
    {
        private IDiffList _source;
        private IDiffList _dest;
        private ArrayList _matchList;

        private DiffEngineLevel _level;

        private DiffStateList _stateList;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffEngine"/> class.
        /// The engine is created with default state and level set to FastImperfect.
        /// </summary>
        public DiffEngine()
        {
            _source = null;
            _dest = null;
            _matchList = null;
            _stateList = null;
            _level = DiffEngineLevel.FastImperfect;
        }

        /// <summary>
        /// Determine how many consecutive items match between destination and source lists.
        /// </summary>
        /// <param name="destIndex">Starting index in the destination list to compare from.</param>
        /// <param name="sourceIndex">Starting index in the source list to compare from.</param>
        /// <param name="maxLength">Maximum number of items to compare (upper bound on match length).</param>
        /// <returns>
        /// The number of consecutive matching items starting at the specified indices.
        /// Returns 0 if the first items do not match. The returned value is in range [0, maxLength].
        /// </returns>
        private int GetSourceMatchLength(int destIndex, int sourceIndex, int maxLength)
        {
            int matchCount;
            for (matchCount = 0; matchCount < maxLength; matchCount++)
            {
                if (_dest.GetByIndex(destIndex + matchCount).CompareTo(_source.GetByIndex(sourceIndex + matchCount)) != 0)
                {
                    break;
                }
            }
            return matchCount;
        }

        /// <summary>
        /// Scans the source range to find the longest contiguous match with the destination starting at destIndex.
        /// Updates the provided <paramref name="curItem"/> (<see cref="DiffState"/>) with match/no-match information.
        /// </summary>
        /// <param name="curItem">
        /// The DiffState item (corresponding to the destination index) to populate with match details.
        /// The method will call <see cref="DiffState.SetMatch"/> or <see cref="DiffState.SetNoMatch"/>.
        /// </param>
        /// <param name="destIndex">Index in the destination list to match against.</param>
        /// <param name="destEnd">Last inclusive index of the destination range to consider.</param>
        /// <param name="sourceStart">First inclusive index of the source range to consider.</param>
        /// <param name="sourceEnd">Last inclusive index of the source range to consider.</param>
        /// <remarks>
        /// The algorithm iterates over potential source start indices, computes the length of
        /// consecutive matches (bounded by remaining destination length and remaining source length),
        /// and retains the longest found match. If no match is found, <see cref="DiffState.SetNoMatch"/> is called.
        /// </remarks>
        private void GetLongestSourceMatch(DiffState curItem, int destIndex, int destEnd, int sourceStart, int sourceEnd)
        {

            int maxDestLength = (destEnd - destIndex) + 1;
            int curLength = 0;
            int curBestLength = 0;
            int curBestIndex = -1;
            int maxLength = 0;
            for (int sourceIndex = sourceStart; sourceIndex <= sourceEnd; sourceIndex++)
            {
                maxLength = Math.Min(maxDestLength, (sourceEnd - sourceIndex) + 1);
                if (maxLength <= curBestLength)
                {
                    //No chance to find a longer one any more
                    break;
                }
                curLength = GetSourceMatchLength(destIndex, sourceIndex, maxLength);
                if (curLength > curBestLength)
                {
                    //This is the best match so far
                    curBestIndex = sourceIndex;
                    curBestLength = curLength;
                }
                //jump over the match
                sourceIndex += curBestLength;
            }
            //DiffState cur = _stateList.GetByIndex(destIndex);
            if (curBestIndex == -1)
            {
                curItem.SetNoMatch();
            }
            else
            {
                curItem.SetMatch(curBestIndex, curBestLength);
            }

        }

        /// <summary>
        /// Processes the specified ranges of destination and source lists to identify matching spans.
        /// </summary>
        /// <param name="destStart">Start index (inclusive) in destination range to process.</param>
        /// <param name="destEnd">End index (inclusive) in destination range to process.</param>
        /// <param name="sourceStart">Start index (inclusive) in source range to process.</param>
        /// <param name="sourceEnd">End index (inclusive) in source range to process.</param>
        /// <remarks>
        /// The method searches for the best match (longest match) within the provided ranges.
        /// When a best match is found, it is added to the internal <see cref="_matchList"/> as a NoChange span.
        /// The method then recursively processes the lower (before the match) and upper (after the match)
        /// ranges to find additional matches.
        /// The search behavior is influenced by <see cref="_level"/> which adjusts whether the algorithm
        /// jumps over discovered matches or continues searching for longer ones.
        /// </remarks>
        private void ProcessRange(int destStart, int destEnd, int sourceStart, int sourceEnd)
        {
            int curBestIndex = -1;
            int curBestLength = -1;
            int maxPossibleDestLength = 0;
            DiffState curItem = null;
            DiffState bestItem = null;
            for (int destIndex = destStart; destIndex <= destEnd; destIndex++)
            {
                maxPossibleDestLength = (destEnd - destIndex) + 1;
                if (maxPossibleDestLength <= curBestLength)
                {
                    //we won't find a longer one even if we looked
                    break;
                }
                curItem = _stateList.GetByIndex(destIndex);

                if (!curItem.HasValidLength(sourceStart, sourceEnd, maxPossibleDestLength))
                {
                    //recalc new best length since it isn't valid or has never been done.
                    GetLongestSourceMatch(curItem, destIndex, destEnd, sourceStart, sourceEnd);
                }

                if (curItem.Status == DiffStatus.Matched)
                {
                    switch (_level)
                    {
                        case DiffEngineLevel.FastImperfect:
                            if (curItem.Length > curBestLength)
                            {
                                //this is longest match so far
                                curBestIndex = destIndex;
                                curBestLength = curItem.Length;
                                bestItem = curItem;
                            }
                            //Jump over the match 
                            destIndex += curItem.Length - 1;
                            break;
                        case DiffEngineLevel.Medium:
                            if (curItem.Length > curBestLength)
                            {
                                //this is longest match so far
                                curBestIndex = destIndex;
                                curBestLength = curItem.Length;
                                bestItem = curItem;
                                //Jump over the match 
                                destIndex += curItem.Length - 1;
                            }
                            break;
                        default:
                            if (curItem.Length > curBestLength)
                            {
                                //this is longest match so far
                                curBestIndex = destIndex;
                                curBestLength = curItem.Length;
                                bestItem = curItem;
                            }
                            break;
                    }
                }
            }
            if (curBestIndex < 0)
            {
                //we are done - there are no matches in this span
            }
            else
            {

                int sourceIndex = bestItem.StartIndex;
                _matchList.Add(DiffResultSpan.CreateNoChange(curBestIndex, sourceIndex, curBestLength));
                if (destStart < curBestIndex)
                {
                    //Still have more lower destination data
                    if (sourceStart < sourceIndex)
                    {
                        //Still have more lower source data
                        // Recursive call to process lower indexes
                        ProcessRange(destStart, curBestIndex - 1, sourceStart, sourceIndex - 1);
                    }
                }
                int upperDestStart = curBestIndex + curBestLength;
                int upperSourceStart = sourceIndex + curBestLength;
                if (destEnd > upperDestStart)
                {
                    //we still have more upper dest data
                    if (sourceEnd > upperSourceStart)
                    {
                        //set still have more upper source data
                        // Recursive call to process upper indexes
                        ProcessRange(upperDestStart, destEnd, upperSourceStart, sourceEnd);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the matching level and processes differences between the provided source and destination lists.
        /// </summary>
        /// <param name="source">The source list to compare (original).</param>
        /// <param name="destination">The destination list to compare (modified).</param>
        /// <param name="level">The desired match/quality level for the diff algorithm.</param>
        /// <returns>
        /// The elapsed processing time in seconds spent computing the diff.
        /// </returns>
        public double ProcessDiff(IDiffList source, IDiffList destination, DiffEngineLevel level)
        {
            _level = level;
            return ProcessDiff(source, destination);
        }

        /// <summary>
        /// Processes differences between the provided source and destination lists using the current engine level.
        /// This method populates internal state and the match list used to produce a final report.
        /// </summary>
        /// <param name="source">The source list to compare (original).</param>
        /// <param name="destination">The destination list to compare (modified).</param>
        /// <returns>
        /// The elapsed processing time in seconds spent computing the diff.
        /// The time is measured from start to completion of the matching algorithm.
        /// </returns>
        public double ProcessDiff(IDiffList source, IDiffList destination)
        {
            DateTime dt = DateTime.Now;
            _source = source;
            _dest = destination;
            _matchList = new ArrayList();

            int dcount = _dest.Count();
            int scount = _source.Count();


            if ((dcount > 0) && (scount > 0))
            {
                _stateList = new DiffStateList(dcount);
                ProcessRange(0, dcount - 1, 0, scount - 1);
            }

            TimeSpan ts = DateTime.Now - dt;
            return ts.TotalSeconds;
        }


        /// <summary>
        /// Add change spans to the provided report describing the differences between two ranges.
        /// </summary>
        /// <param name="report">ArrayList to which change spans will be appended (modified in place).</param>
        /// <param name="curDest">Current destination index where comparison begins.</param>
        /// <param name="nextDest">Next destination index that marks the end of the unchanged span.</param>
        /// <param name="curSource">Current source index where comparison begins.</param>
        /// <param name="nextSource">Next source index that marks the end of the unchanged span.</param>
        /// <returns>
        /// True if one or more change spans were added to the report; otherwise false.
        /// The method may add Replace, AddDestination or DeleteSource spans depending on differences.
        /// </returns>
        private bool AddChanges(ArrayList report, int curDest, int nextDest, int curSource, int nextSource)
        {
            bool retval = false;
            int diffDest = nextDest - curDest;
            int diffSource = nextSource - curSource;
            int minDiff = 0;
            if (diffDest > 0)
            {
                if (diffSource > 0)
                {
                    minDiff = Math.Min(diffDest, diffSource);
                    report.Add(DiffResultSpan.CreateReplace(curDest, curSource, minDiff));
                    if (diffDest > diffSource)
                    {
                        curDest += minDiff;
                        report.Add(DiffResultSpan.CreateAddDestination(curDest, diffDest - diffSource));
                    }
                    else
                    {
                        if (diffSource > diffDest)
                        {
                            curSource += minDiff;
                            report.Add(DiffResultSpan.CreateDeleteSource(curSource, diffSource - diffDest));
                        }
                    }
                }
                else
                {
                    report.Add(DiffResultSpan.CreateAddDestination(curDest, diffDest));
                }
                retval = true;
            }
            else
            {
                if (diffSource > 0)
                {
                    report.Add(DiffResultSpan.CreateDeleteSource(curSource, diffSource));
                    retval = true;
                }
            }
            return retval;
        }

        /// <summary>
        /// Produces a high-level diff report (list of <see cref="DiffResultSpan"/>) describing how to transform the source into the destination.
        /// </summary>
        /// <returns>
        /// An ArrayList of DiffResultSpan instances describing NoChange, Replace, DeleteSource and AddDestination operations.
        /// The returned list is ready for consumption after a prior call to <see cref="ProcessDiff(IDiffList, IDiffList)"/>.
        /// </returns>
        public ArrayList DiffReport()
        {
            ArrayList retval = new ArrayList();
            int dcount = _dest.Count();
            int scount = _source.Count();

            //Deal with the special case of empty files
            if (dcount == 0)
            {
                if (scount > 0)
                {
                    retval.Add(DiffResultSpan.CreateDeleteSource(0, scount));
                }
                return retval;
            }
            else
            {
                if (scount == 0)
                {
                    retval.Add(DiffResultSpan.CreateAddDestination(0, dcount));
                    return retval;
                }
            }


            _matchList.Sort();
            int curDest = 0;
            int curSource = 0;
            DiffResultSpan last = null;

            //Process each match record
            foreach (DiffResultSpan drs in _matchList)
            {
                if ((!AddChanges(retval, curDest, drs.DestIndex, curSource, drs.SourceIndex)) &&
                    (last != null))
                {
                    last.AddLength(drs.Length);
                }
                else
                {
                    retval.Add(drs);
                }
                curDest = drs.DestIndex + drs.Length;
                curSource = drs.SourceIndex + drs.Length;
                last = drs;
            }

            //Process any tail end data
            AddChanges(retval, curDest, dcount, curSource, scount);

            return retval;
        }
    }
}

