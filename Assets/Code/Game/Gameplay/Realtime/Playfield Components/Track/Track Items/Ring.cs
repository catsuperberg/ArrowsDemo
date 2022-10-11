using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Microinteracions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track.TrackItems
{
    public class Ring : MonoBehaviour, IMathContainer, IMicrointerationActivator
    {
        [SerializeField]
        private TMP_Text _operationText;
        
        public OperationInstance Operation {get; private set;}
        private bool _spent = false;
        
        public event EventHandler<MicrointeractionEventArgs> OnMicrointerationTriggered;
        
        public void Initialize(OperationInstance newOperation)
        {                       
            Operation = newOperation;
            UpdateApearance();
        }
        
        public void Spend()
            => _spent = true;
        
        void UpdateApearance()
        {
            _operationText.text = Operation.Type.ToSymbol() + Operation.Value;
        }
        
        public BigInteger ApplyOperation(BigInteger initialValue)
        {
            if(_spent)
                return initialValue;
                
            var newValue = Operation.Perform(initialValue);
            SpendAllInList(FindRingsInPair());                
            bool isBestChoice = IsBestInPair(initialValue);
            ActivateVibration(isBestChoice);
            ActivateSound(isBestChoice);
            ActivateAnimation(isBestChoice);
            return newValue;
                
        }
        
        void SpendAllInList(List<Ring> rings)
        {
            foreach(Ring ring in rings)
                ring.Spend();
        }
        
        List<Ring> FindRingsInPair()
        {
            var pairObjectContainer = gameObject.transform.parent.gameObject;
            var rings = pairObjectContainer.GetComponentsInChildren<Ring>().ToList();
            return rings;
        }
        
        bool IsBestInPair(BigInteger initialValue)
        {
            var pairObjectContainer = gameObject.transform.parent.gameObject;
            var pair = pairObjectContainer.GetComponent<OperationPairComponent>().Pair;
            return pair.IsBestOperation(Operation, initialValue);
        }
        
        void ActivateVibration(bool isPositive)
        {
            var vibrationType = (isPositive) ? VibrationEffect.SmallVibration : VibrationEffect.Negative;
            OnMicrointerationTriggered?.Invoke(this, new MicrointeractionEventArgs(new VibrationPacket(vibrationType)));
        }
        
        void ActivateSound(bool isPositive)
        {
            var soundEffect = (isPositive) ? SoundEffect.Affirmative : SoundEffect.Negative;
            OnMicrointerationTriggered?.Invoke(this, new MicrointeractionEventArgs(new SFXPacket(soundEffect)));
        }
        
        void ActivateAnimation(bool isPositive)
        {
            OnMicrointerationTriggered?.Invoke(this, new MicrointeractionEventArgs(new RingAnimationPacket(isPositive)));
        }
    }
}