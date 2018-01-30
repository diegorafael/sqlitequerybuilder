using SQLiteQueryBuilder.Attributes;

namespace SQLiteQueryBuilder.Enumerations
{
    public enum BoolOperatorType
    {
        [EnumMetadata(nameof(And), "and", "AND")]
        And,

        [EnumMetadata(nameof(Or), "or", "OR")]
        Or
    }
}
