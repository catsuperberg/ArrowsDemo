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
        private OperationExecutor _exec;
        private bool _spent = false;
        
        public event EventHandler<MicrointeractionEventArgs> OnMicrointerationTriggered;
        
        public void Initialize(OperationInstance newOperation, OperationExecutor exec)
        {
            if(exec == null)
                throw new System.Exception("OperationExecutor not provided to Ring");
            _exec = exec;                        
            Operation = newOperation;
            UpdateApearance();
        }
        
        public void Spend()
        {
            _spent = true;
        }
        
        void UpdateApearance()
        {
            if(_exec == null)
                throw new System.Exception("Ring wasn't initialized");
            _operationText.text = Operation.Type.ToSymbol() + Operation.Value;
        }
        
        public BigInteger ApplyOperation(BigInteger initialValue)
        {
            if(_spent)
                return initialValue;
                
            if(_exec == null)
                throw new System.Exception("Ring wasn't initialized");
            var newValue = _exec.Perform(Operation, initialValue);
            SpendAllInList(FindRingsInPair());                
            bool isBestChoice = IsBestInPair(initialValue);
            ActivateVibration(isBestChoice);
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
            return pair.BestOperation(initialValue, _exec) == Operation;
        }
        
        void ActivateVibration(bool positive)
        {
            var vibrationType = (positive) ? VibrationEffect.SmallVibration : VibrationEffect.Negative;
            OnMicrointerationTriggered?.Invoke(this, new MicrointeractionEventArgs(new VibrationPacket(vibrationType)));
        }
    }
}