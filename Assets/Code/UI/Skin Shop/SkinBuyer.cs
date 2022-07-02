using DataManagement;
using ExtensionMethods;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Meta.Skins;
using System;
using System.Numerics;
using TMPro;
using UnityEngine;

namespace UI
{    
    public class SkinBuyer : MonoBehaviour
    {
        [SerializeField]
        TMP_Text PriceText;
        [SerializeField]
        UnityEngine.UI.Image Icon;
        
        bool _bought;
        
        public void AttachToSkin(string name, ProjectileCollection skinCollection)
        {
            if (skinCollection is null)
                throw new ArgumentNullException(nameof(skinCollection));

            PriceText.text = skinCollection.GetSkinPrice(name).ToString();
            var icon = skinCollection.GetSkinIcon(name);
            Icon.sprite = icon;
        }
        
        public void Buy()
        {
            
        }
    }
}