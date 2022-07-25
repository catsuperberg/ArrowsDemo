using Game.Gameplay.Meta.Skins;
using System;
using System.Linq;
using UnityEngine;
// using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class ProjectileSkinShopManager : MonoBehaviour
    {
        [SerializeField]
        GameObject BuyerPrefab;
        [SerializeField]
        GameObject SelectorPrefab;
        [SerializeField]
        Transform ContainerForElements;
        
        SkinShopService _shopService;

        [Inject]
        public void Construct(SkinShopService shopService)
        {
            _shopService = shopService ?? throw new ArgumentNullException(nameof(shopService));
            var orderedSkinEntries = from entry in _shopService.SkinsPriceTable orderby entry.Value ascending select entry;
            
            foreach(var skin in orderedSkinEntries)
                CreateCorrectElementWithContainer(skin.Key);
        }
        
        void CreateCorrectElementWithContainer(string name)
        {
            var elementContainerObj = new GameObject("Container for: " + name);
            elementContainerObj.transform.SetParent(ContainerForElements);
            var skinUIContainer = elementContainerObj.transform;
            
            if(_shopService.BoughtSkins.Contains(name))
                CreateSelectorForSkin(name, skinUIContainer);
            else
                CreateBuyerForSkin(name, skinUIContainer);
            
        }
        
        void CreateSelectorForSkin(string name, Transform skinUIContainer)
        {            
            var elementGO = Instantiate(SelectorPrefab, skinUIContainer, worldPositionStays: false);
            elementGO.name = name;            
            SetParentRectTransformToChildSettings(skinUIContainer.gameObject, elementGO);  
            var element = elementGO.GetComponent<SkinSelector>();
            element.AttachToSkin(name);
        }
        
        void CreateBuyerForSkin(string name, Transform skinUIContainer)
        {
            var elementGO = Instantiate(BuyerPrefab, skinUIContainer, worldPositionStays: false);
            elementGO.name = name;     
            SetParentRectTransformToChildSettings(skinUIContainer.gameObject, elementGO);     
            
            var element = elementGO.GetComponent<SkinBuyer>();
            element.AttachToSkin(name);
            element.OnSkinBought += PromoteBuyerToSelector;
        }
        
        void SetParentRectTransformToChildSettings(GameObject parent, GameObject child)
        {        
            var parentRectTransform = parent.GetComponent<RectTransform>();   
            if(parentRectTransform == null)
                parentRectTransform = (RectTransform)parent.AddComponent(typeof(RectTransform));
            parentRectTransform = child.GetComponent<RectTransform>();  
        }
        
        void PromoteBuyerToSelector(object caller, EventArgs args)
        {
            if(!(caller is SkinBuyer))
                return;
            
            var buyer = (caller as SkinBuyer);
            var skinName = buyer.SkinName;
            CreateSelectorForSkin(skinName, buyer.transform.parent);
            Destroy(buyer.gameObject);
        }
    }
}