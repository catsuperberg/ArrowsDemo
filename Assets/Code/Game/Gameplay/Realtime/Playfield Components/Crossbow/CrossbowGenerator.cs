using Game.Gameplay.Meta.Skins;
using System;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Crossbow
{
    public class CrossbowGenerator : MonoBehaviour, ICrossbowProvider
    {                   
        SkinCollection _crossbowCollection;
        
        [Inject]
        public void Construct([Inject(Id = "crossbows")] SkinCollection crossbowCollection)
        {
            _crossbowCollection = crossbowCollection ?? throw new ArgumentNullException(nameof(crossbowCollection));
        }
        
        public GameObject CreateSelected() 
            => Instantiate(_crossbowCollection.GetSelectedSkinResource(), Vector3.up*2.1f, Quaternion.Euler(Vector3.up*90));
    }
}