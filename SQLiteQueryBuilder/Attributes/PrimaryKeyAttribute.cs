using System;

namespace SQLiteQueryBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute()
        { }
    }
}
