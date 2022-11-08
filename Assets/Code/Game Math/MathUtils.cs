using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Timers;

namespace GameMath
{        
    
    
    public class BigIntMedianCalculator
    {
        IEnumerable<BigIntMedianCalculator> _loverLevelCalculators = null;
        IEnumerable<BigInteger> _finalLevelValues = null;
        
        public BigIntMedianCalculator(IEnumerable<BigInteger> values, int groupsPerLevel, int levelDepth)
        {
            if(levelDepth == 1)
            {
                FinalLevelContructor(values);
                return;
            }
            
            var groups = Split(values, groupsPerLevel);
            _loverLevelCalculators = groups.Select(entry => new BigIntMedianCalculator(entry, groupsPerLevel, levelDepth-1));
        }       
        
        void FinalLevelContructor(IEnumerable<BigInteger> values)
        {
            _finalLevelValues = values;
        }
        
        public BigInteger Calculate()
            => (_finalLevelValues != null) ?
                MathUtils.Median(_finalLevelValues.ToList()) :
                MathUtils.Median(_loverLevelCalculators.Select(entry => entry.Calculate()).ToList());
                
        IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> data, int chunkSize)
            => data.Select((x, idx) => new { index = idx, val = x})
							.GroupBy(i => i.index / chunkSize)
							.Select(g => g.Select(x => x.val));
                            
        BigInteger Median(IEnumerable<BigInteger> values)
        {
            int numberCount = values.Count();
            int halfIndex = numberCount/2;
            var sortedNumbers = values.OrderBy(n=>n);
            BigInteger median;
            if ((numberCount % 2) == 0)
                median = ((sortedNumbers.ElementAt(halfIndex) + sortedNumbers.ElementAt((halfIndex - 1)))/ 2);
            else 
                median = sortedNumbers.ElementAt(halfIndex);
            return 0;
        }
    }
    
    public static class WeightedRandom
    {
        public static Dictionary<float, T> FrequencyToWeights<T>(Dictionary<int, T> frequency)
        {            
            if(frequency == null || !frequency.Any())
                throw new Exception("No values provided to convert to weights");
                
            var weights = new Dictionary<float, T>();
            var sumOfOccurances = frequency.Sum(x => x.Key);
            var occurrenceAccumulator = 0;
            var sortedFrequency = from entry in frequency orderby entry.Key ascending select entry;
            foreach(var entry in sortedFrequency)
            {
                occurrenceAccumulator += entry.Key;
                var weight = (float)occurrenceAccumulator / (float)sumOfOccurances;
                weights.Add(weight, entry.Value);
            }
            return weights;
        } 
        
        public static T NextFrom<T>(Dictionary<float, T> weightedValues, Random rand = null)
        {
            if(weightedValues == null || !weightedValues.Any())
                throw new Exception("No values provided to get next weighted random");
            
            var sortedWeights = from entry in weightedValues orderby entry.Key descending select entry;
            var maxValue = sortedWeights.Sum(entry => entry.Key);
            var minValue = sortedWeights.Last().Key;
            var random = rand != null ? (rand.NextDouble()*(maxValue-minValue)+minValue) : GlobalRandom.RandomDouble(minValue, maxValue);
            return sortedWeights.First(entry => random >= entry.Key).Value;
        }
        
        public static T NextFrom<T>(Dictionary<int, T> frequency)
        {
            var weights = FrequencyToWeights(frequency);
            return NextFrom(weights);
        }
        
        public static T NextFrom<T>(Dictionary<int, T> frequency, Random rand)
        {
            var weights = FrequencyToWeights(frequency);
            return NextFrom(weights, rand);
        }
    }
    
    public class IntAutoCounter : IUpdatedNotification, IFinishNotification
    {
        public int InitialValue {get;}
        public int ValueToStopAt {get;}
        public int Increment {get;}
        public int PeriodMs {get;}
        
        public int CurrentValue {get; private set;}
        private Timer _timer;
        private bool _counterIncreasing;
        
        public event EventHandler OnUpdated;
        public event EventHandler OnFinished;
        
        public IntAutoCounter(int initialValue, int valueToStopAt, int increment, int periodMs)
        {
            InitialValue = initialValue;
            ValueToStopAt = valueToStopAt;
            Increment = increment;
            PeriodMs = periodMs;
            
            ExceptionOnNoincrement();
            ExceptionOnSameValue();
            ExceptionOnOverflow();            
        }
        
        public void ExceptionOnNoincrement()
        {
            if(Increment == 0)
                throw new Exception("IntAutoCounter initialized with no increment");
        }
        
        public void ExceptionOnSameValue()
        {
            if(InitialValue == ValueToStopAt)
                throw new Exception("IntAutoCounter initialized with same values for start and stop");
        }

        public void ExceptionOnOverflow()
        {
            _counterIncreasing = Increment > 0;
            bool overflowDetected;                
            overflowDetected = (_counterIncreasing) ? InitialValue > ValueToStopAt : InitialValue < ValueToStopAt;
            if(overflowDetected)  
                throw new OverflowException("IntAutoCounter detected that with provided values int overflow is needed to finish");
        }
        
        public void StartCountFromBeginning()
        {
            if(_timer != null)
                _timer.Elapsed -= PerformIncrement;
                
            CurrentValue = InitialValue;
            _timer = new Timer();
            _timer.Interval = PeriodMs;
            _timer.Elapsed += PerformIncrement;
            _timer.Start();
        }
        
        void PerformIncrement(object sender, ElapsedEventArgs e)
        {
            CurrentValue += Increment;
            var targetReached = (_counterIncreasing) ? CurrentValue >= ValueToStopAt : CurrentValue <= ValueToStopAt; 
            if(targetReached)
                Finished();
            else
                OnUpdated?.Invoke(this, EventArgs.Empty);
        }
        
        void Finished()
        {                        
            _timer.Stop();
            _timer.Elapsed -= PerformIncrement;
            OnUpdated?.Invoke(this, EventArgs.Empty);
            OnFinished?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public static class FibonacciUtils
    {
        static bool IsPerfectSquare(int x)
        {
            int s = (int)Math.Sqrt(x);
            return (s * s == x);
        }
        
        public static bool IsFibonacci(int n)
        {
            return IsPerfectSquare(5 * n * n + 4)
                || IsPerfectSquare(5 * n * n - 4);
        }
    }
    
    public static class GlobalRandom
    {
        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());
        private static readonly object _syncLock = new object();
        const string _chars = "abcdefghijklmnoprqrstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 _-";
        
        public static int RandomInt(int min, int max)
        {
            lock(_syncLock)  // synchronize
            {
                return _random.Next(min, max);
            }
        }
        
        public static int RandomIntInclusive(int from, int to)
        {
            lock(_syncLock)  // synchronize
            {
                return _random.Next(from, to+1);
            }
        }
        
        public static int RandomIntInclusive(int max)
        {
            lock(_syncLock)  // synchronize
            {
                return _random.Next(0, max < int.MaxValue ? max+1 : int.MaxValue);
            }
        }
        
        public static bool RandomBool()
        {
            lock(_syncLock)  // synchronize
            {
                return Convert.ToBoolean(_random.Next(2));
            }
        }
                
        public static double RandomDouble()
        {
            lock(_syncLock) // synchronize
            {
                return _random.NextDouble();
            }
        }
                
        public static double RandomDouble(double min, double max)
        {
            lock(_syncLock) // synchronize
            {                
                return _random.NextDouble() * (max - min) + min;
            }
        }
        
        public static double RandomDouble((double min, double max) range)
        {
            lock(_syncLock) // synchronize
            {                
                return _random.NextDouble() * (range.max - range.min) + range.min;
            }
        }
                
        public static string RandomString(int length)
        {
            lock(_syncLock) // synchronize
            {                
                return new string(Enumerable.Repeat(_chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
            }
        }
    }
    
    public static class MathUtils
    {        
        public static float SecondsToMs(float time) => time * 1000;
        public static float PartToNegativeDB(float partValue) => (partValue > 0) ?  UnityEngine.Mathf.Log10(Math.Clamp(partValue, 0, 1)) * 20 : -80.0f;
        
        public static T MathClamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if(val.CompareTo(max) > 0) return max;
            else return val;
        }
        
        public static System.Double RoundToHalf(System.Double passednumber)
        {
            return Math.Round(passednumber * 2, MidpointRounding.AwayFromZero) / 2;
        }    
            
        public static int RandomSign()
        {
            return (int)((GlobalRandom.RandomInt(0,2) - 0.5) * 2);
        } 
        
        public static (int, int) GetPositionOnSpiralGrid(int index)
        {
            // (di, dj) is a vector - direction in which we move right now
            int di = 1;
            int dj = 0;
            // length of current segment
            int segment_length = 1;

            // current position (i, j) and how much of current segment we passed
            int i = 0;
            int j = 0;
            int segment_passed = 0;
            for (int k = 0; k < index; ++k) 
            {
                // make a step, add 'direction' vector (di, dj) to current position (i, j)
                i += di;
                j += dj;
                ++segment_passed;

                if (segment_passed == segment_length) 
                {
                    // done with current segment
                    segment_passed = 0;

                    // 'rotate' directions
                    int buffer = di;
                    di = -dj;
                    dj = buffer;

                    // increase segment length if necessary
                    if (dj == 0) 
                        ++segment_length;
                }
            }
            return (i, j);
        }        
        
        /// <summary>
        /// Partitions the given list around a pivot element such that all elements on left of pivot are <= pivot
        /// and the ones at thr right are > pivot. This method can be used for sorting, N-order statistics such as
        /// as median finding algorithms.
        /// Pivot is selected ranodmly if random number generator is supplied else its selected as last element in the list.
        /// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 171
        /// </summary>
        private static int Partition<T>(this IList<T> list, int start, int end, Random rnd = null) where T : IComparable<T>
        {
            if (rnd != null)
                list.Swap(end, rnd.Next(start, end+1));

            var pivot = list[end];
            var lastLow = start - 1;
            for (var i = start; i < end; i++)
            {
                if (list[i].CompareTo(pivot) <= 0)
                    list.Swap(i, ++lastLow);
            }
            list.Swap(end, ++lastLow);
            return lastLow;
        }

        /// <summary>
        /// Returns Nth smallest element from the list. Here n starts from 0 so that n=0 returns minimum, n=1 returns 2nd smallest element etc.
        /// Note: specified list would be mutated in the process.
        /// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 216
        /// </summary>
        public static T NthOrderStatistic<T>(this IList<T> list, int n, Random rnd = null) where T : IComparable<T>
        {
            return NthOrderStatistic(list, n, 0, list.Count - 1, rnd);
        }
        
        private static T NthOrderStatistic<T>(this IList<T> list, int n, int start, int end, Random rnd) where T : IComparable<T>
        {
            while (true)
            {
                var pivotIndex = list.Partition(start, end, rnd);
                if (pivotIndex == n)
                    return list[pivotIndex];

                if (n < pivotIndex)
                    end = pivotIndex - 1;
                else
                    start = pivotIndex + 1;
            }
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            if (i==j)   //This check is not required but Partition function may make many calls so its for perf reason
                return;
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        /// <summary>
        /// Note: specified list would be mutated in the process.
        /// </summary>
        public static T Median<T>(this IList<T> list) where T : IComparable<T>
        {
            return list.NthOrderStatistic((list.Count - 1)/2);
        }        

        public static double Median<T>(this IEnumerable<T> sequence, Func<T, double> getValue)
        {
            var list = sequence.Select(getValue).ToList();
            var mid = (list.Count - 1) / 2;
            return list.NthOrderStatistic(mid);
        }
        
        public static T FastMedian<T>(T[] sourceNumbers) 
        {
            //Framework 2.0 version of this method. there is an easier way in F4        
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                throw new System.Exception("Median of empty array not defined.");

            //make sure the list is sorted, but use a new array
            // T[] sortedPNumbers = (T[])sourceNumbers.Clone();
            Array.Sort(sourceNumbers);

            int size = sourceNumbers.Length;
            int mid = size / 2;
            return sourceNumbers[mid];
        }
    }   
}