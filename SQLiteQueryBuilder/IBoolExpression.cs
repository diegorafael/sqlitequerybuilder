using SQLiteQueryBuilder.Enumerations;

namespace SQLiteQueryBuilder
{
    public interface IBoolExpression
    {
        LogicalOperatorType LogicalOperator { get; }
        string GetStatement();
        string GetLogicalOperatorString();
    }
}
