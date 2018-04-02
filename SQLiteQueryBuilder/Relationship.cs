using SQLiteQueryBuilder.Enumerations;
using System;

namespace SQLiteQueryBuilder
{
    public class Relationship
    {
        public Relationship(string name, Type relatedType, EnumRelationshipType relationshipType)
        {
            Name = name;
            RelatedType = relatedType;
            RelationshipType = relationshipType;
        }

        public string Name { get; set; }
        public Type RelatedType { get; set; }
        public EnumRelationshipType RelationshipType { get; set; }
    }
}
