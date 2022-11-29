using System.Collections.Generic;

namespace Utils
{
    public class KeeperDictionary<Tone,Ttwo>
    {
        public IReadOnlyDictionary<Tone,Ttwo> CurrentValues => _values;
        public IReadOnlyDictionary<Tone,Ttwo> PreviousValues => _previousValues;
        Dictionary<Tone,Ttwo> _values = new Dictionary<Tone,Ttwo>();
        Dictionary<Tone,Ttwo> _previousValues = new Dictionary<Tone,Ttwo>();
        readonly Ttwo _defaultValue;
        
        public KeeperDictionary(Ttwo defaultValue)
        {
            _defaultValue = defaultValue;
        }
        
        public void Add(Tone key,Ttwo value)
        {
            _values.Add(key, value);
        }
        
        public void ClearCurrent()
        {
            _previousValues = _values;
            _values = new Dictionary<Tone,Ttwo>();
        }
        
        public Ttwo GetValueOrDefault(Tone key)
            => _values.TryGetValue(key, out var value) ? 
                value : 
                _previousValues.TryGetValue(key, out value) ?
                    value :
                    _defaultValue;
    }
}