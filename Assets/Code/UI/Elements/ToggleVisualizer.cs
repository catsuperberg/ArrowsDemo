using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI
{    
    public class ToggleVisualizer : MonoBehaviour
    {
        [SerializeField]
        private Animator ToggleAnimator;
        [SerializeField]
        private string AnimationBoolName;
        [SerializeField]
        private string QuickSwitchOn;
        [SerializeField]
        private string QuickSwitchOff;
        
        private bool _toggleState = false;
        
        public void Initialize(bool initialState)
        {
            _toggleState = initialState;        
            QuickSwitchToState();
            UpdateAnimationState();
        }
        
        void QuickSwitchToState()
        {
            if(_toggleState == ToggleAnimator.GetBool(AnimationBoolName))
                return;
            var triggerName = (_toggleState) ? QuickSwitchOn : QuickSwitchOff;
            ToggleAnimator.SetTrigger(triggerName);
            ToggleAnimator.ResetTrigger(triggerName);
        }  
        
        public void Toggle()
        {
            _toggleState = !_toggleState;
            UpdateAnimationState();
        }  
        
        public void SetState(bool state)
        {            
            _toggleState = state;
            UpdateAnimationState();
        }
        
        void UpdateAnimationState()
        {
            ToggleAnimator.SetBool(AnimationBoolName, _toggleState);        
        }
    }
}