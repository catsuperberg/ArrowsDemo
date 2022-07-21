using Game.Gameplay.Meta.Skins;
using System;
using System.Linq;
using UnityEngine;
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

        [Inject]
        public void Construct(ProjectileCollection skinCollection)
        {
            _skinCollection = skinCollection ?? throw new ArgumentNullException(nameof(skinCollection));
            var orderedSkinEntries = from entry in _skinCollection.SkinNamesAndPrices orderby entry.Value ascending select entry;
            
            foreach(var skin in orderedSkinEntries)
                CreateMenuElementForSkin(skin.Key);
        }
        
        void CreateMenuElementForSkin(string name)
        {
            var elementGO = Instantiate(BuyerPrefab, Vector3.zero, Quaternion.identity, ContainerForElements);
            elementGO.name = name;
            var element = elementGO.GetComponent<SkinBuyer>();
            element.AttachToSkin(name, _skinCollection);
        }
    }
}