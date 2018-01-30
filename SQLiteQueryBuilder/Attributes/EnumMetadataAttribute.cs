using System;

namespace SQLiteQueryBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumMetadataAttribute : Attribute
    {
        public string FullName { get; }
        public string Abbreviation { get; }
        public object Value { get; }

        public EnumMetadataAttribute(string fullname, string abbreviation, object value = null)
        {
            FullName = fullname;
            Abbreviation = abbreviation;
            Value = value;
        }
    }
}
