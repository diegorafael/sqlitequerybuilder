using SQLiteQueryBuilder.Attributes;

namespace SQLiteQueryBuilder.Enumerations
{
    public enum BoolComparisonType
    {
        [EnumMetadata(nameof(Equals), "eq", "=")]
        Equals,

        [EnumMetadata(nameof(NotEquals), "diff", "!=")]
        NotEquals,

        [EnumMetadata(nameof(GreaterThan), "gt", ">")]
        GreaterThan,

        [EnumMetadata(nameof(GreaterThanOrEqualTo), "gteq", ">=")]
        GreaterThanOrEqualTo,

        [EnumMetadata(nameof(LessThan), "lt", "<")]
        LessThan,

        [EnumMetadata(nameof(LessThanOrEqualTo), "lteq", "<=")]
        LessThanOrEqualTo,

        [EnumMetadata(nameof(In), "in", "IN")]
        In,

        [EnumMetadata(nameof(NotIn), "nin", "NOT IN")]
        NotIn,

        [EnumMetadata(nameof(Like), "like", "LIKE")]
        Like
    }
}
