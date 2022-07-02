using DataManagement;
using ExtensionMethods;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Meta.Skins;
using System;
using TMPro;
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
            
            foreach(var skin in _skinCollection.AccesibleNames)
                CreateMenuElementForSkin(skin);
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