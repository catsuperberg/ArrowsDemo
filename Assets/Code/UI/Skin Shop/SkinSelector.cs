using DataManagement;
using Game.Gameplay.Meta.Skins;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{    
    public class SkinSelector : MonoBehaviour
    {
        [SerializeField]
        UnityEngine.UI.Image Icon;
        [SerializeField]
        GameObject Border;
        [SerializeField]
        Button SelectButton;
        [SerializeField]
        TMP_Text ButtonText;
        [SerializeField]
        string UnselectedText;
        [SerializeField]
        string SelectedText;
        
        SkinShopService _shopService;
        public string SkinName {get; private set;}
        
        [Inject]
        public void Construct([Inject(Id = "userRegistryAccessor")] IRegistryValueReader userRegistry, SkinShopService shopService)
        {            
            if(userRegistry == null)
                throw new ArgumentNullException("IRegistryValueReader not provided to " + this.GetType().Name);
            if(shopService == null)
                throw new ArgumentNullException("SkinShopService not provided to " + this.GetType().Name);

            _shopService = shopService;
            userRegistry.OnUpdated += DataInRegistryUpdated;
        }
        
        void DataInRegistryUpdated(object caller, EventArgs args)
        {
            UpdateAppearance();
        }
        
        public void AttachToSkin(string name)
        {
            SkinName = name;
            var icon = _shopService.SkinIcon(SkinName);
            Icon.sprite = icon;
            UnityMainThreadDispatcher.Instance().Enqueue(() => {UpdateAppearance();});   
        }
        
        public void Select()
        {
            _shopService.SelectSkin(SkinName);
        }
        
        void UpdateAppearance()
        {
            var skinSelected = _shopService.IsSelectedSkin(SkinName);
            SelectButton.interactable = !skinSelected;
            Border.SetActive(skinSelected);
            ButtonText.text = skinSelected ? SelectedText : UnselectedText;
        }   
    }
}