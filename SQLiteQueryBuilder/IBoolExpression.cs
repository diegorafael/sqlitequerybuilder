using SQLiteQueryBuilder.Enumerations;

namespace SQLiteQueryBuilder
{
    public interface IBoolExpression
    {
        BoolOperatorType LogicalOperator { get; }
        string GetStatement();
        string GetLogicalOperatorString();
    }
}
