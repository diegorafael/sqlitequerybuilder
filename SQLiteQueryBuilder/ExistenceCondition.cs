using SQLiteQueryBuilder.Attributes;
using SQLiteQueryBuilder.Enumerations;
using SQLiteQueryBuilder.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SQLiteQueryBuilder
{
    public class ExistenceCondition : BaseBoolExpression
    {
        protected QueryBuilder InnerQuery { get; }
        protected ExistenceType ExistenceType { get; }

        const string uniqueRegister = "1";

        public ExistenceCondition(QueryBuilder innerExistsQuery, ExistenceType existenceType = ExistenceType.Exists, LogicalOperatorType sufixOperator = LogicalOperatorType.And)
        {
            InnerQuery = innerExistsQuery;
            ExistenceType = existenceType;
            LogicalOperator = sufixOperator;
        }
        
        public override string GetStatement()
        {
            string ret = string.Empty;
            string statementStructure = " {0} ( {1} ) ";

            ret = string.Format(statementStructure, GetExistenceTypeOperator(), GetInnerQuery());
            return ret;
        }

        private string GetExistenceTypeOperator()
        {
            string ret = string.Empty;

            ret = ExistenceType.GetAttribute<EnumMetadataAttribute>()?.Value?.ToString();

            return ret;
        }

        private string GetInnerQuery()
        {
            string ret = string.Empty;
            var originalColumns = InnerQuery.GetSelectColumns();
            InnerQuery.SetSelectColumns(uniqueRegister);
            ret = InnerQuery.BuildSelect();
            InnerQuery.SetSelectColumns(originalColumns);

            return ret;
        }
    }
}
