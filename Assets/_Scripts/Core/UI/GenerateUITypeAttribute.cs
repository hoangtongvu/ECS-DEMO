using System;

namespace Core.UI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GenerateUITypeAttribute : Attribute
    {
        public GenerateUITypeAttribute(string uiTypeName)
        {
        }
    }

}