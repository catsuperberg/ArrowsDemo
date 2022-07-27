using System;
using System.Collections.Generic;

namespace DataManagement
{
    public interface IConfigurable : IUpdatedNotification
    {
        void UpdateField(string fieldName, string fieldValue);    
        void UpdateFields(List<(string fieldName, string fieldValue)> updatedValues);     
    }  
}