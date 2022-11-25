using System;
using System.Linq;

namespace Game.GameDesign
{
    public struct ColumnDataPoints
    {
        public readonly string[] Labels;
        public readonly double[] Values;

        public ColumnDataPoints(string[] labels, double[] values)
        {
            if(labels.Count() != values.Count())
                throw new Exception("Different number of labels and values");
            
            Labels = labels;
            Values = values;
        }
    }
}