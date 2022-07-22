using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataManagement
{
    public class ListApplier : IOperationApplier
    {
        public string GetResultOfOperation(string baseValue, string incrementValue, OperationType typeOfOperation)
        {
            string result;
            switch(typeOfOperation)
            {
                case OperationType.Replace:
                    result = incrementValue;
                    break;
                case OperationType.Append:
                    result = AppendElementToList(baseValue, incrementValue);
                    break;
                default:
                    throw new ArgumentException("No such operation as: " + typeOfOperation + "found in class: " + this.GetType().Name);
            }
            return result;
        }
        
        string AppendElementToList(string baseJsonList, string valuesToAppendJson)
        {
            var baseList = JArray.Parse(baseJsonList);
            var listToAppend = JArray.Parse(valuesToAppendJson);
            baseList.Merge(listToAppend, new JsonMergeSettings{MergeArrayHandling = MergeArrayHandling.Union});
            return JsonConvert.SerializeObject(baseList);
        }
    }
}