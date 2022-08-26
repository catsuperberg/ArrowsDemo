using Game.Gameplay.Meta.Skins;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace UI
{
    public class CrossbowSkinShopManager : SkinShopManager
    {
        [Inject]
        public void Construct([Inject(Id = "crossbowsShop")] SkinShopService shopService)
        {
            ConstructBase(shopService);
        }
    }
}