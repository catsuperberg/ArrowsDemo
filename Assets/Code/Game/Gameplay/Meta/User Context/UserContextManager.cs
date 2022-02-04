using Game.Gameplay.Meta.PassiveIncome;
using Game.Gameplay.Meta.UpgradeSystem;
using System;

namespace Game.Gameplay.Meta
{
    public class UserContextManager : IUserContextRetriver
    {
        UserContext _context;
        public UserContext Context {get {return _context;}}
        
        public UserContextManager() // TEMP temporary constructor
        {
            _context = new UserContext(new UpgradeContext(), new PassiveInvomceContext());
        }
    }
}