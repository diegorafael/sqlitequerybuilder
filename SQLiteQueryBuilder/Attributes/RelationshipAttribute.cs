using SQLite;
using System;

namespace SQLiteQueryBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class RelationshipAttribute : IgnoreAttribute
    {
        protected RelationshipAttribute(string foreignKey, string inverseForeignKey, string inverseProperty)
        {
            InverseForeignKey = inverseForeignKey;
            InverseProperty = inverseProperty;
            ForeignKey = foreignKey;
        }

        public string ForeignKey { get; private set; }
        public string InverseProperty { get; private set; }
        public string InverseForeignKey { get; private set; }
        public bool ReadOnly { get; set; }
    }
}
