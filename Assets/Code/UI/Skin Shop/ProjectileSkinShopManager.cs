using Game.Gameplay.Meta.Skins;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace UI
{
    public class ProjectileSkinShopManager : SkinShopManager
    {
        [Inject]
        public void Construct([Inject(Id = "projectilesShop")] SkinShopService shopService)
        {
            ConstructBase(shopService);
        }
    }
}