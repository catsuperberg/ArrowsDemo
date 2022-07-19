using Game.Gameplay.Meta.Curencies;
using DataManagement;
using ExtensionMethods;
using System;
using TMPro;
using UnityEngine;
using Zenject;
using System.Numerics;

namespace UI
{
    public interface ICurencyDisplay
    {                
        void ForceSetDisplayedAmmount(BigInteger ammountToDisplay);
    }
}
