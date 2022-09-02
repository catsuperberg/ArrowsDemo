using DataManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Meta.Skins
{
    public class SkinResource
    {
        public readonly string Name;
        public readonly UnityEngine.Object GameObjectResourse;
        public readonly Sprite Icon;
        public readonly BigInteger Price;
        public readonly bool AdWatchRequired;

        public SkinResource(string name, UnityEngine.Object gameObjectResourse, Sprite icon, BigInteger price, bool adWatchRequired)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));

            Name = name;
            GameObjectResourse = gameObjectResourse ?? throw new ArgumentNullException(nameof(gameObjectResourse));
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            Price = price;
            AdWatchRequired = adWatchRequired;
        }
    }
}