using GameMath;
using System;
using TMPro;
using UI;
using UnityEngine;

namespace Game.GameState
{    
    public class PreAdTease : MonoBehaviour
    {
        [SerializeField]        
        private TMP_Text AdMultiplierField;
        [SerializeField]        
        private TMP_Text DefaultReward;
        [SerializeField]        
        private TMP_Text MultipliedReward;
        [SerializeField]        
        private CoinDisplay PlayerCoinDisplay;
        
        private IntAutoCounter BonusMultiplierCounter;
        
        private int _adMultiplier = 5;
        
        void Awake()
        {
            UpdateAppearance();
            BonusMultiplierCounter = new IntAutoCounter(initialValue: _adMultiplier, valueToStopAt: 2, 
                increment: -1, periodMs: 800);
            BonusMultiplierCounter.OnUpdated += MultipierChanged;
            BonusMultiplierCounter.OnFinished += CounterFinished;
            BonusMultiplierCounter.StartCountFromBeginning();
        }
        
        void MultipierChanged(object caller, EventArgs args)
        {
            _adMultiplier = BonusMultiplierCounter.CurrentValue;
            UnityMainThreadDispatcher.Instance().Enqueue(() => {UpdateAppearance();});
        }
        
        void CounterFinished(object caller, EventArgs args)
        {
            BonusMultiplierCounter.OnUpdated -= MultipierChanged;
            BonusMultiplierCounter.OnFinished -= CounterFinished;
        }
        
        void UpdateAppearance()
        {
            UpdateMultiplier();
        }
        
        void UpdateMultiplier()
        {
            AdMultiplierField.text = "X" + _adMultiplier.ToString();
        }
    }
}