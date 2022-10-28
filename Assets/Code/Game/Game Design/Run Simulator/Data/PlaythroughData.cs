using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Game.GameDesign
{
    public class PlaythroughData
    {
        public readonly IEnumerable<RunData> Runs;
        public readonly int NumberOfRuns;
        public readonly TimeSpan CombinedTime;

        public PlaythroughData(IEnumerable<RunData> playthroughRuns)
        {
            Runs = playthroughRuns ?? throw new System.ArgumentNullException(nameof(playthroughRuns));
            NumberOfRuns = Runs.Count();
            CombinedTime = Runs.Aggregate(new TimeSpan(0), (sum, entry) => sum += entry.CombinedSeconds);
        }
    }
}