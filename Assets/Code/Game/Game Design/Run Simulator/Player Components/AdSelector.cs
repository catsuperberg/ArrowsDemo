using Game.Gameplay.Realtime.OperationSequence.Operation;
using GameMath;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Game.GameDesign
{    
    public interface IAdSelector
    {        
        const float _adSeconds = 15;
        
        public AdReport AccountForAd();
    }
    
    public static class AdSelectorGrades
    {
        static Dictionary<int, IAdSelector> _gradeFrequencies = new Dictionary<int, IAdSelector>(){
                    {3, new FastAdSelector()},
                    {5, new SlowAdSelector()},
                    {2, new AdSkipper()}};                
                
        public static IAdSelector GetRandomGrade()
            => WeightedRandom.NextFrom(_gradeFrequencies);
    }
    
    public class AdReport
    {
        public readonly float Multiplier;
        public readonly float SecondsToWatch;

        public AdReport(float multiplier, float secondsToWatch)
        {
            Multiplier = multiplier;
            SecondsToWatch = secondsToWatch;
        }
    }
    
    public class FastAdSelector : IAdSelector
    {       
        float _chanceOfLowerMultiplier = 0.1f;
        readonly Random _random = new Random(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());
        
        public AdReport AccountForAd()
        {
            var chanceCheck = _random.NextDouble();
            var multiplier = (chanceCheck >= _chanceOfLowerMultiplier) ? 4 : 3;
            var seconds = IAdSelector._adSeconds;
            return new AdReport(multiplier, seconds);
        }
    }
    
    public class SlowAdSelector : IAdSelector
    {       
        readonly Random _random = new Random(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());
        
        public AdReport AccountForAd()
        {
            var multiplier = _random.Next(1, 4+1);
            var seconds = (multiplier == 1) ? 0 : IAdSelector._adSeconds;
            return new AdReport(multiplier, seconds);
        }
    }
    
    public class AdSkipper : IAdSelector
    {       
        public AdReport AccountForAd()
            => new AdReport(1, 0);
    }
}