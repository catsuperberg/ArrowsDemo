using Game.Gameplay.Meta.Skins;
using System;
using System.Linq;
using UnityEngine;
// using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class ProjectileSkinShop : MonoBehaviour
    {
        [SerializeField]
        GameObject BuyerPrefab;
        [SerializeField]
        GameObject SelectorPrefab;
        [SerializeField]
        Transform ContainerForElements;
        
        ProjectileCollection _skinCollection;
        // SkinShopService _shopService;

        [Inject]
        public void Construct(ProjectileCollection skinCollection, SkinShopService shopService)
        {
            _skinCollection = skinCollection ?? throw new ArgumentNullException(nameof(skinCollection));
            // _shopService = shopService ?? throw new ArgumentNullException(nameof(shopService));
            var orderedSkinEntries = from entry in _skinCollection.SkinNamesAndPrices orderby entry.Value ascending select entry;
            
            foreach(var skin in orderedSkinEntries)
                CreateCorrectElementWithContainer(skin.Key);
        }
        
        void CreateCorrectElementWithContainer(string name)
        {
            var elementContainerObj = new GameObject("Container for: " + name);
            elementContainerObj.transform.SetParent(ContainerForElements);
            var skinContainer = elementContainerObj.transform;
            // var skinContainer = Instantiate(elementContainerObj, Vector3.zero, Quaternion.identity, ContainerForElements).transform;
            
            if(_skinCollection.BoughtSkins.Contains(name))
                CreateSelectorForSkin(name, skinContainer);
            else
                CreateBuyerForSkin(name, skinContainer);
            
        }
        
        void CreateSelectorForSkin(string name, Transform skinContainer)
        {            
            var elementGO = Instantiate(SelectorPrefab, skinContainer, worldPositionStays: false);
            elementGO.name = name;            
            SetParentRectTransformToChildSettings(skinContainer.gameObject, elementGO);  
            // var element = elementGO.GetComponent<SkinSelector>();
            // element.AttachToSkin(name, _skinCollection);
            // element.OnSkinBought += PromoteBuyerToSelector;
        }
        
        void CreateBuyerForSkin(string name, Transform skinContainer)
        {
            var elementGO = Instantiate(BuyerPrefab, skinContainer, worldPositionStays: false);
            elementGO.name = name;     
            SetParentRectTransformToChildSettings(skinContainer.gameObject, elementGO);     
            
            var element = elementGO.GetComponent<SkinBuyer>();
            element.AttachToSkin(name, _skinCollection);
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