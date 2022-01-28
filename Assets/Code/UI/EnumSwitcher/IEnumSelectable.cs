using System;

namespace UiUtils
{
    public interface IEnumSelectable
    {
        public Array GetValues();
        public int Length();
        public string ValueName(int value);
    }
}
