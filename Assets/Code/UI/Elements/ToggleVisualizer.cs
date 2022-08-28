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
        private string QuickSwitchBoolName;
        
        private bool _toggleState = false;
        
        public void Initialize(bool initialState)
        {
            _toggleState = initialState;     
            QuickSwitchToState(); 
        }
        
        void OnEnable()
        {
            QuickSwitchToState(); 
        }
        
        void QuickSwitchToState()
        {
            if(_toggleState == ToggleAnimator.GetBool(AnimationBoolName))
                return;
            
            ToggleAnimator.SetBool(QuickSwitchBoolName, true);      
            UpdateAnimationState();
            DisableQuickSwitchAfterDelay();    
        }  
        
        void DisableQuickSwitchAfterDelay()
        {
            StartCoroutine(DisableQuickSwitchCoroutine(Time.deltaTime*2));
        }
        
        IEnumerator DisableQuickSwitchCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            
            ToggleAnimator.SetBool(QuickSwitchBoolName, false); 
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