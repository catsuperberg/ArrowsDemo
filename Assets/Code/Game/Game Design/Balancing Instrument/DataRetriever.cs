using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Game.GameDesign
{
    public class DataRetriever 
    {
        static int _numThreads = Math.Clamp((int)((System.Environment.ProcessorCount/2)*0.75), 3, int.MaxValue);
        PlaythroughSimulatorFactory _factory;
        
        public DataRetriever(PlaythroughSimulatorFactory factory)
        {
            _factory = factory ?? throw new System.ArgumentNullException(nameof(factory));
        }
        
        async public Task<IEnumerable<PlaythroughData>> SimulateForStatistics(int uniquePlaythroughsCount, IProgress<SimProgressReport> progress)
        {
            var progressReport = new SimProgressReport(uniquePlaythroughsCount);
            progress.Report(progressReport);
            var playthroughs = Enumerable.Range(0, uniquePlaythroughsCount)
                .Select(entry => _factory.CreateRandom())
                .ToList();
            // var results = playthroughs
            //     .AsParallel()
            //     .WithDegreeOfParallelism(_numThreads)
            //     .Select(playthrough => SimulateAndReport(playthrough, progress, progressReport))
            //     .ToList();
            // return results;
            return await Simulate(playthroughs, progress, progressReport);
        }
        
        async Task<IEnumerable<PlaythroughData>> Simulate(IEnumerable<PlaythroughSimulator> playthroughs, IProgress<SimProgressReport> progress, SimProgressReport report)
        {
            var results = new ConcurrentBag<PlaythroughData>();        
            var options = new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = _numThreads};
            var linkOptions = new DataflowLinkOptions() {PropagateCompletion = true};
            
            var spreadBlock = new TransformManyBlock<IEnumerable<PlaythroughSimulator>, PlaythroughSimulator>
                (simulators => simulators, options);
            var simulationBlock = new TransformBlock<PlaythroughSimulator, PlaythroughData>
                (simulator => SimulateAndReport(simulator, progress, report), options);
            var resultBlock = new ActionBlock<PlaythroughData>
                (data => results.Add(data), options);
                            
            spreadBlock.LinkTo(simulationBlock, linkOptions);
            simulationBlock.LinkTo(resultBlock, linkOptions);
            
            await spreadBlock.SendAsync(playthroughs);
            spreadBlock.Complete();
            await resultBlock.Completion;
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