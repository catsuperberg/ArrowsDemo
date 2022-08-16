using System.Collections.Generic;

namespace DataManagement
{
    public interface IConfigurableCollectionReader
    {
        public IList<ConfigurableClassData> RegisteredConfigurables {get;}
        
        public bool ClassRegistered(string className);
        public IList<ConfigurableField> GetFields(string className);
    }
}