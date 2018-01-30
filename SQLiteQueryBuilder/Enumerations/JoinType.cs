using SQLiteQueryBuilder.Attributes;

namespace SQLiteQueryBuilder.Enumerations
{
    public enum JoinType
    {
        [EnumMetadata(nameof(Inner), "inner", "INNER")]
        Inner = 0,

        [EnumMetadata(nameof(Outer), "left", "LEFT")]
        Outer = 1
    }
}