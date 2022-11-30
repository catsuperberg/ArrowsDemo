using System;
using System.Collections.Generic;

namespace Utils
{
    public class MovingAverage
    {
        private readonly int _length;
        private int _circIndex = -1;
        private bool _filled;
        private double _current = double.NaN;
        private readonly double _oneOverLength;
        private readonly double[] _circularBuffer;
        private double _total;

        public MovingAverage(int length)
        {
            _length = length;
            _oneOverLength = 1.0 / length;
            _circularBuffer = new double[length];
        }       

        public MovingAverage Update(double value)
        {
            double lostValue = _circularBuffer[_circIndex];
            _circularBuffer[_circIndex] = value;

            // Maintain totals for Push function
            _total += value;
            _total -= lostValue;

            // If not yet filled, just return. Current value should be double.NaN
            if (!_filled)
            {
                _current = double.NaN;
                return this;
            }

            // Compute the average
            double average = 0.0;
            for (int i = 0; i < _circularBuffer.Length; i++)
                average += _circularBuffer[i];

            _current = average * _oneOverLength;
            return this;
        }

        public MovingAverage Push(double value)
        {
            // Apply the circular buffer
            if (++_circIndex == _length)
                _circIndex = 0;

            double lostValue = _circularBuffer[_circIndex];
            _circularBuffer[_circIndex] = value;

            // Compute the average
            _total += value;
            _total -= lostValue;

            // If not yet filled, just return. Current value should be double.NaN
            if (!_filled && _circIndex != _length - 1)
            {
                _current = double.NaN;
                return this;
            }
            else
                _filled = true; // Set a flag to indicate this is the first time the buffer has been filled

            _current = _total * _oneOverLength;
            return this;
        }

        public int Length { get { return _length; } }
        public double Current { get { return _current; } }
    }
    
    internal static class MovingAverageExtensions
    {
        public static IEnumerable<double> MovingAverage<T>(this IEnumerable<T> inputStream, Func<T, double> selector, int period)
        {
            var ma = new MovingAverage(period);
            foreach (var item in inputStream)
            {
                ma.Push(selector(item));
                yield return ma.Current;
            }
        }
        
        
        public static IEnumerable<T> MovingAverage<T>(
            this IEnumerable<T> inputStream, Func<T, double> selector,
            Func<T, double, T> recreator, int period)
        {
            var ma = new MovingAverage(period);
            foreach (var item in inputStream)
            {
                ma.Push(selector(item));
                yield return recreator(item, ma.Current);
            }
        }

        public static IEnumerable<double> MovingAverage(this IEnumerable<double> inputStream, int period)
        {
            var ma = new MovingAverage(period);
            foreach (var item in inputStream)
            {
                ma.Push(item);
                yield return ma.Current;
            }
        }
    }
}