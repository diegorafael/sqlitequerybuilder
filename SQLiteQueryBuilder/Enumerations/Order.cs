using SQLiteQueryBuilder.Attributes;

namespace SQLiteQueryBuilder.Enumerations
{
    public enum Order
    {
        [EnumMetadata(nameof(Ascending), "asc", "ASC")]
        Ascending = 0,

        [EnumMetadata(nameof(Descending), "desc", "DESC")]
        Descending = 1
    }
}