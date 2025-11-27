using System;
using System.Collections;
using ZidUtilities.CommonCode.DifferenceEngine.Structure;

namespace ZidUtilities.CommonCode.DifferenceEngine.Engine
{
    /// <summary>
    /// Specifies the processing level used by the diff engine.
    /// This enum controls the trade-off between execution speed and the thoroughness/accuracy
    /// of the difference detection algorithm.
    /// </summary>
    public enum DiffEngineLevel
    {
        /// <summary>
        /// FastImperfect: Prioritizes execution speed over perfect accuracy.
        /// Use this level when performance is critical and small or rare mismatches
        /// in the reported differences are acceptable.
        /// </summary>
        FastImperfect,

        /// <summary>
        /// Medium: Balances speed and accuracy.
        /// Use this level when a compromise between performance and correctness is desired.
        /// It aims to produce reasonably accurate results without the full cost of the SlowPerfect level.
        /// </summary>
        Medium,

        /// <summary>
        /// SlowPerfect: Prioritizes thoroughness and accuracy over execution speed.
        /// Use this level when correctness is paramount and the diff engine should attempt
        /// to find the most accurate set of differences, even if it requires more processing time.
        /// </summary>
        SlowPerfect
    }
}
