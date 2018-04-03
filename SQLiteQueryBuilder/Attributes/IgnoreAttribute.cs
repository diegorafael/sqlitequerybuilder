using System;

namespace SQLiteQueryBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
        public IgnoreAttribute(){ }
    }
}
