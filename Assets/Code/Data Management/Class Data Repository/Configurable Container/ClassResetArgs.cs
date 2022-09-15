using System;
using System.Collections.Generic;

namespace DataManagement
{
    public class ClassResetArgs : EventArgs
    {
        public readonly Type ClassType;

        public ClassResetArgs(Type classType)
        {
            ClassType = classType;
        }
    }
}