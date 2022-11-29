using UnityEngine;

namespace Game.GameDesign
{        
    public interface IValueAnalizer
    {
        public SimValueType Type {get;}
        public string GetValue();
    }
}