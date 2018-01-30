using SQLiteQueryBuilder.Attributes;
using SQLiteQueryBuilder.Enumerations;
using SQLiteQueryBuilder.Extensions;

namespace SQLiteQueryBuilder
{
    public abstract class BaseBoolExpression : IBoolExpression
    {
        public BoolOperatorType LogicalOperator { get; protected set; }

        public abstract string GetStatement();

        public virtual string GetLogicalOperatorString()
        {
            string ret = string.Empty;
            string space = " ";

            ret = string.Concat(space, LogicalOperator.GetAttribute<EnumMetadataAttribute>().Value.ToString(), space);
            return ret;
        }
    }
}
