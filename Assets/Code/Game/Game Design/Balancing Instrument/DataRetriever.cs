using Game.GameDesign;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Threading.Tasks;

namespace Game.GameDesign
{
    public class DataRetriever 
    {
        static int _numThreads = Mathf.Clamp((int)((System.Environment.ProcessorCount/2)*0.75), 3, int.MaxValue);
        PlaythroughSimulatorFactory _factory;
        
        public DataRetriever(PlaythroughSimulatorFactory factory)
        {
            _factory = factory ?? throw new System.ArgumentNullException(nameof(factory));
        }
        
        async public Task<IEnumerable<PlaythroughData>> SimulateForStatistics(int uniquePlaythroughsCount, IProgress<SimProgressReport> progress)
        {
            var progressReport = new SimProgressReport(uniquePlaythroughsCount);
            progress.Report(progressReport);
            var playthroughs = Enumerable.Range(0, uniquePlaythroughsCount).Select(entry => _factory.CreateRandom());
            var results = playthroughs
                .AsParallel()
                .WithDegreeOfParallelism(_numThreads)
                .Select(playthrough => SimulateAndReport(playthrough, progress, progressReport))
                .ToList();
            return results;
        }
        
        PlaythroughData SimulateAndReport(PlaythroughSimulator playthrough, IProgress<SimProgressReport> progress, SimProgressReport report)
        {
            var result = playthrough.Simulate(); 
            report.IncrementDone();
            progress.Report(report);
            return result;
        }
    }
}