using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track.TrackItems
{
    public class Ring : MonoBehaviour, IMathContainer
    {
        [SerializeField]
        private TMP_Text _operationText;
        
        private OperationInstance _operation;
        private OperationExecutor _exec;
        private bool _spent = false;
        
        public void Initialize(OperationInstance newOperation, OperationExecutor exec)
        {
            if(exec == null)
                throw new System.Exception("OperationExecutor not provided to Ring");
            _exec = exec;                        
            _operation = newOperation;
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
            _operationText.text = _operation.Type.ToSymbol() + _operation.Value;
        }
        
        public BigInteger ApplyOperation(BigInteger initialValue)
        {
            if(!_spent)
            {
                if(_exec == null)
                    throw new System.Exception("Ring wasn't initialized");
                var newValue = _exec.Perform(_operation, initialValue);
                SpendAllInList(FindRingsInPair());
                return newValue;
            }
            else
                return initialValue;
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
            // foreach(Ring ring in rings)
            //     if(ring.gameObject.name != gameObject.name)
            //         rings.Remove(ring);
            return rings;
        }
    }
}