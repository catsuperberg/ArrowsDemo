using DataManagement;
using System;

namespace Game.Gameplay.Meta.UpgradeSystem
{
    public class UpgradeContext : Configurable
    {
        [StoredField]
        public int CrossbowLevel {get; private set;} = 1;
        [StoredField]
        public int ArrowLevel {get; private set;} = 1;
        [StoredField]
        public int InitialArrowCount {get; private set;} = 1;
        [StoredField]
        public int PassiveIncome {get; private set;} = 1;

        public UpgradeContext()
        {
        }

        public UpgradeContext(int crossbowLevel, int arrowLevel, int initialArrowCount)
        {
            CrossbowLevel = crossbowLevel;
            ArrowLevel = arrowLevel;
            InitialArrowCount = initialArrowCount;
        }

        public UpgradeContext(IRegistryIngester registry)
        {
            registry.Register(this, true, true);            
        }    
        
        internal override void SetFieldValue(string fieldName, string fieldValue)
        {
            switch(fieldName)
            {
                case nameof(CrossbowLevel):
                    CrossbowLevel = Convert.ToInt32(fieldValue);
                    break;
                case nameof(ArrowLevel):
                    ArrowLevel = Convert.ToInt32(fieldValue);
                    break;
                case nameof(InitialArrowCount):
                    InitialArrowCount = Convert.ToInt32(fieldValue);
                    break;
                case nameof(PassiveIncome):
                    PassiveIncome = Convert.ToInt32(fieldValue);
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
    }
}