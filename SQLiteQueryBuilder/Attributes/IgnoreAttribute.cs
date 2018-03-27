using System;

namespace SQLiteQueryBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class IgnoreAttribute : Attribute
    {
        protected IgnoreAttribute(){ }
    }
}
