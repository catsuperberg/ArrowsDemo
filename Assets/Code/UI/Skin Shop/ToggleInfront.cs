using UnityEngine;

public class ToggleInfront : MonoBehaviour
{
    [SerializeField]
    Transform FirstUIElement;
    [SerializeField]
    Transform SecondUIElement;
    
    public void SwitchToFirst()
    {   
        if(FirstUIElement.GetSiblingIndex() < SecondUIElement.GetSiblingIndex())
            Toggle();
        
    }
    
    public void SwitchToSecond()
    {        
        if(SecondUIElement.GetSiblingIndex() < FirstUIElement.GetSiblingIndex())
            Toggle();
    }
    
    public void Toggle()
    {        
        var firstIndex = FirstUIElement.GetSiblingIndex();
        var secondIndex = SecondUIElement.GetSiblingIndex();
        FirstUIElement.SetSiblingIndex(secondIndex);
        SecondUIElement.SetSiblingIndex(firstIndex);
    }
}
