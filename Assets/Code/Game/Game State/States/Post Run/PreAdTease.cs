using ExtensionMethods;
using Game.Gameplay.Realtime;
using GameMath;
using System;
using System.Numerics;
using TMPro;
using UI;
using UnityEngine;

namespace Game.GameState
{    
    public class PreAdTease : MonoBehaviour
    {
        readonly System.Random _random = new System.Random(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());
        
        [SerializeField]        
        private TMP_Text SkipMultiplierField;
        [SerializeField]        
        private TMP_Text AdMultiplierField;
        [SerializeField]        
        private TMP_Text DefaultReward;
        [SerializeField]        
        private TMP_Text MultipliedReward;
        [SerializeField]        
        private CoinDisplay PlayerCoinDisplay;
                
        public BigInteger RewardThatPlayerGets {get; private set;}
        public bool AdRequested {get; private set;} = false;
        public event EventHandler OnCoinsTransferred;   
        
        private bool _adToBePlayed = false;
        private bool _transferingCoins = false;
        private bool _spent = false;
        private IntAutoCounter BonusMultiplierCounter;
        private ExponentialCountCalculator _coinCalculator;
        
        [SerializeField]
        private int _adMultiplier = 4;
        [SerializeField]
        private double _coinTransferTime = 0.8;   
        
        [SerializeField]
        private int _defaultMultiplier = 1;
        [SerializeField]
        private int _skipMultiplierValue = 3;
        [SerializeField, Range(0, 1f)]
        private float _skipMultiplierChance = 0.25f;
             
        private BigInteger _defaultReward;
        private BigInteger _multipliedReward;
        private BigInteger _originalPlayerCoins;
        private BigInteger _animatedPlayerCoins;
        
        private TMP_Text _sourceCoinText;
        private BigInteger _sourceValue;
        
        
        public void SkipAd()
        {            
            if(!_spent)
            {
                CounterFinished(this, EventArgs.Empty);
                StartCountOffToPlayerCoins(DefaultReward, _defaultReward, false); 
                _spent = true;               
            }
        }
        
        public void ShowAd()
        {           
            if(!_spent)
            {
                CounterFinished(this, EventArgs.Empty);
                StartCountOffToPlayerCoins(MultipliedReward, _multipliedReward, true);
                _spent = true;
                AdRequested = true;
            }
        }
        
        void StartCountOffToPlayerCoins(TMP_Text sourceCoinText, BigInteger sourceValue, bool adToBePlayed)
        {
            _sourceCoinText = sourceCoinText;
            _sourceValue = sourceValue;
            _adToBePlayed = adToBePlayed;
            _animatedPlayerCoins = _originalPlayerCoins;
            _coinCalculator =  new ExponentialCountCalculator(_sourceValue, 0, _coinTransferTime);
            _transferingCoins = true;
        }
        
        void Update()
        {
            if(_transferingCoins)
                TransferOneFrameWorthOfCoins();
        }
        
        void TransferOneFrameWorthOfCoins()
        {
            BigInteger frameWorthOfCoins;
            if(_adToBePlayed)
            {
                frameWorthOfCoins = _coinCalculator.GetDeltaForGivenTime(_multipliedReward, Time.deltaTime);
                _multipliedReward -= frameWorthOfCoins;
                _animatedPlayerCoins += frameWorthOfCoins;
            }
            else
            {
                frameWorthOfCoins = _coinCalculator.GetDeltaForGivenTime(_defaultReward, Time.deltaTime);
                _defaultReward -= frameWorthOfCoins;
                _animatedPlayerCoins += frameWorthOfCoins;
            }                     
            
            var transferComplete = _animatedPlayerCoins >= _originalPlayerCoins + _sourceValue || frameWorthOfCoins == 0;
            if(transferComplete)
            {
                _transferingCoins = false;
                _animatedPlayerCoins = _animatedPlayerCoins + _sourceValue;
                RewardThatPlayerGets = _sourceValue;
            }
                
            PlayerCoinDisplay.ForceSetDisplayedAmmount(_animatedPlayerCoins);
            UpdateAppearance();
            
            if(transferComplete)
                OnCoinsTransferred?.Invoke(this, EventArgs.Empty);  
        }
        
        public void Initialize(RunFinishContext finishContext, BigInteger PlayerCoins)
        {            
            var chanceCheck = _random.NextDouble();
            _defaultMultiplier = (chanceCheck >= _skipMultiplierChance) ? _defaultMultiplier : _skipMultiplierValue;
            _defaultReward = finishContext.RewardForTheRun * _defaultMultiplier;
            
            // _defaultReward = finishContext.RewardForTheRun;
            _originalPlayerCoins = PlayerCoins;
            _multipliedReward = _defaultReward*_adMultiplier;
            UpdateAppearance();
        }
        
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
            _multipliedReward = _defaultReward*_adMultiplier;
            UnityMainThreadDispatcher.Instance().Enqueue(() => {UpdateAppearance();});
        }
        
        void CounterFinished(object caller, EventArgs args)
        {
            if(BonusMultiplierCounter == null)
                return;
            BonusMultiplierCounter.OnUpdated -= MultipierChanged;
            BonusMultiplierCounter.OnFinished -= CounterFinished;
            BonusMultiplierCounter = null;
        }
        
        void UpdateAppearance()
        {
            UpdateMultiplier();
            UpdateDefaultReward();
            UpdateMultipliedReward();
        }
        
        void UpdateMultiplier()
        {
            AdMultiplierField.text = "X" + _adMultiplier.ToString();
            SkipMultiplierField.text = "X" + _defaultMultiplier.ToString();
        }
        
        void UpdateDefaultReward()
        {
            DefaultReward.text = _defaultReward.ParseToReadable();
        }
        
        void UpdateMultipliedReward()
        {
            MultipliedReward.text = _multipliedReward.ParseToReadable();
        }
    }
}