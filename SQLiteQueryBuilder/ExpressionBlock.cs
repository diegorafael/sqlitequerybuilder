using SQLiteQueryBuilder.Enumerations;
using System;
using System.Collections.Generic;

namespace SQLiteQueryBuilder
{
    public class ExpressionBlock : BaseBoolExpression
    {
        protected List<IBoolExpression> Expressions;

        string newLine = "\n";
        string blockFormat = "( {0} )";

        public ExpressionBlock(params IBoolExpression[] expression) : this(LogicalOperatorType.And, expression)
        { }

        public ExpressionBlock(LogicalOperatorType logicalOperator, params IBoolExpression[] expression)
        {
            Expressions = new List<IBoolExpression>();
            LogicalOperator = logicalOperator;

            ValidateExpressionParams(expression);

            AddBoolExpression(expression);
        }

        protected virtual void ValidateExpressionParams(params IBoolExpression[] expression)
        {
            if (expression == null || expression.Length == 0 || expression[0] == null)
                throw new ArgumentNullException(nameof(expression));
        }

        public void AddBoolExpression(params IBoolExpression[] expression)
        {
            ValidateExpressionParams(expression);

            foreach (var expr in expression)
                Expressions.Add(expr);
        }

        public override string GetStatement()
        {
            string ret = string.Empty;
            long count = Expressions.Count;

            for (int i = 0; i < count; i++)
            {
                var expr = Expressions[i];
                ret += (string.IsNullOrWhiteSpace(ret) ? string.Empty : newLine) +
                       expr.GetStatement() +
                       (i != (count - 1) ? expr.GetLogicalOperatorString() : string.Empty);
            }

            ret = string.Format(blockFormat, ret) + newLine;

            return ret;
        }
    }
}
