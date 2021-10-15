using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UiUtils
{
    public class ToggleHelper : MonoBehaviour
    {
        [SerializeField]
        private GameObject OnStateOverlay;
        [SerializeField]
        private GameObject OffStateOverlay;
        
        private bool State;
        
        private void ToggleState()
        {
            State = !State;
            SetActiveOverlay(State);
        }     
        
        public void SetActiveState(bool state)
        {            
            State = state;
            SetActiveOverlay(State);            
        }
        
        private void SetActiveOverlay(bool state)
        {            
            OnStateOverlay.SetActive(state);
            OffStateOverlay.SetActive(!state);
        }
    }
}