using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        
        async public Task<IEnumerable<PlaythroughData>> SimulateForStatistics(
            int uniquePlaythroughsCount, int repeatsPerSimulator, IProgress<SimProgressReport> progress, 
            CompletionConditions completionConditions)
        {
            return await Simulate(uniquePlaythroughsCount, repeatsPerSimulator, progress, completionConditions, _factory.CreateRandom);
        }
        
        async public Task<IEnumerable<PlaythroughData>> SimulateAverage(
            int uniquePlaythroughsCount, int repeatsPerSimulator, IProgress<SimProgressReport> progress, 
            CompletionConditions completionConditions, AveragePlayerData playerData)
        {
            Func<PlaythroughSimulator> creationMethod = () => _factory.CreateAverage(playerData);
            return await Simulate(uniquePlaythroughsCount, repeatsPerSimulator, progress, completionConditions, creationMethod);
        }
        
        async Task<IEnumerable<PlaythroughData>> Simulate(
            int uniquePlaythroughsCount, int repeatsPerSimulator, IProgress<SimProgressReport> progress, 
            CompletionConditions completionConditions, Func<PlaythroughSimulator> CreatePlaythrough)
        {
            var progressReport = new SimProgressReport(uniquePlaythroughsCount);
            progress.Report(progressReport);
            var playthroughsPerSimulator = DividePlaythroughsBetweenSimulators(uniquePlaythroughsCount, repeatsPerSimulator);
            
            var results = playthroughsPerSimulator
                .AsParallel()
                .WithDegreeOfParallelism(_numThreads)
                .Select(repeats => 
                    {
                        var result = CreatePlaythrough().Simulate(repeats, completionConditions); 
                        Report(progress, progressReport, repeats);
                        return result;
                    })
                .SelectMany(resultArray => resultArray)
                .ToArray();   
            
            return results;
        }
        
        IEnumerable<int> DividePlaythroughsBetweenSimulators(int playthroughsCount, int repeatsPerSimulator)
        {
            if(repeatsPerSimulator > playthroughsCount)
                throw new Exception("Failed to divide playthroughs between simulators repeatsPerSimulator can't be higher than playthroughsCount");
            var repeatsRange = new List<int>();
            repeatsRange = Enumerable.Repeat(repeatsPerSimulator, playthroughsCount/repeatsPerSimulator).ToList();
            var partialSimulator = playthroughsCount%repeatsPerSimulator;
            if(partialSimulator != 0)
                repeatsRange.Add(partialSimulator); 
            return repeatsRange;
        }
        
        void Report(IProgress<SimProgressReport> progress, SimProgressReport report, int count)
        {
            for(int i = 0; i < count; i++)
                report.IncrementDone();
            progress.Report(report);
        }
    }
}