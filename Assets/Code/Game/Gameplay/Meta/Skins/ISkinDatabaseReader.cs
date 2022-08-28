using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Gameplay.Meta.Skins
{
    public interface ISkinDatabaseReader<T>
    {        
        IList<T> Skins {get;}
    }
}