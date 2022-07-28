using System;
using System.Linq;
using System.Collections.Generic;

namespace DataManagement
{
    public interface IRegistryBackend
    {
        public string Name {get;} 
        public IConfigurableCollectionReader Configurables {get;}
        public IList<IConfigurable> ObjectsToUpdateOnChange {get;}
        
        public event EventHandler<RegistryChangeArgs> OnNewData;
        
        public void UpdateRegisteredField(string className, ConfigurableField field);
        public void UpdateInstanceWithStoredValues(IConfigurable instance);
        public void WriteToRegistry(List<ConfigurableClassData> configurablesToPush, bool overrideOnPresent);
        // public void OverrideConfigurables(ConfigurablesCollection newConfigurables);
        public void OverrideClassData(string className, List<ConfigurableField> newData);
        public void RegisterClassIfNew(Type objectType, List<ConfigurableField> fields);
        public void RegisterInstanceForUpdates(IConfigurable instance, string className);
        public void UnregisterInstance(IConfigurable instance);
        public void UpdateAllRegisteredOfClass(string className);
    }
}