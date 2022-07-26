using System;
using System.Linq;
using System.Collections.Generic;

namespace DataManagement
{
    public interface IRegistryBackend : IUpdatedNotification
    {
        public string Name {get;} 
        public Lookup<string, List<ConfigurableField>> CurrentConfigurablesData {get;}
        public IList<IConfigurable> ObjectsToUpdateOnChange {get;}
        
        public void UpdateRegisteredField(string className, string fieldName, string fieldValue);
        public void UpdateInstanceWithStoredValues(IConfigurable instance);
        public void WriteToRegistry(Dictionary<string, List<ConfigurableField>> sourceConfigurables, bool overrideRegistered);
        public void OverrideConfigurables(Dictionary<string, List<ConfigurableField>> newConfigurables);
        public void OverrideClassData(string className, List<ConfigurableField> newData);
        public void RegisterNewConfigurablesForClass(Type objectType, List<ConfigurableField> fields);
        public void RegisterInstanceForUpdates(IConfigurable instance, string className);
        public void UnregisterInstance(IConfigurable instance);
        public void UpdateAllRegisteredOfClass(string className);
    }
}