using System;
using GameSettings;

namespace UiUtils
{
    public class GraphicsSelectable : IEnumSelectable
    {
        private Array _values;
        private int _length;
        
        public GraphicsSelectable()
        {
            _values = Enum.GetValues(typeof(GraphicsPresets));
            _length = _values.Length;
        }
        
        public Array GetValues()
        {
            return _values;
        }
        
        public int Length()
        {
            return _length;
        }
        
        public string ValueName(int value)
        {
            return GraphicsPersetName((GraphicsPresets)value);
        }        
        
        private string GraphicsPersetName(GraphicsPresets presets)
        {
            switch (presets)
            {
                case GraphicsPresets.Low:
                    return "Low";
                case GraphicsPresets.Medium:
                    return "Medium";
                case GraphicsPresets.Heigh:
                    return "Heigh";
                default:
                    return "unknown";
            }
        }
    }
}