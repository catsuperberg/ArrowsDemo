using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Gameplay.Meta.Skins
{
    public interface ISkinDatabase<T>
    {        
        IList<T> Skins {get;}
        string PathToDatabase {get;}
        
        void AddSkinsUniqueByName(List<T> skinsData);
        void SetSkinsDataKeepOldPropertiesOnNull(List<T> skinsData);
        bool AlreadyInDatabase(string skinName);
        void SaveToPermanent();
    }
}