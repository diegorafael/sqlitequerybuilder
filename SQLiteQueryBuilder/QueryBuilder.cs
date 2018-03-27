﻿using SQLiteQueryBuilder.Attributes;
using SQLiteQueryBuilder.Enumerations;
using SQLiteQueryBuilder.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SQLiteQueryBuilder
{
    public class QueryBuilder<T> : QueryBuilder where T : class
    {
        public QueryBuilder(string alias = null, params string[] columnWithAlias) : base(typeof(T), alias, columnWithAlias) { }

        public static string BuildSelectDistinct(string alias, string columnWithAlias = null, params IBoolExpression[] condition)
        {
            return BuildSelectDistinct(typeof(T), alias, columnWithAlias, condition);
        }

        // ToDo: Refazer comandos de delete usando um exists
        //public new string BuildDelete()
        //{
        //    return base.BuildDelete();
        //}

        //public static string BuildDelete(params IBoolExpression[] condition)
        //{
        //    return BuildDelete(typeof(T), condition);
        //}
    }

    public class QueryBuilder
    {
        public Type EntityType { get; }

        public string Alias { get; }

        Join[] joins = new Join[] { };
        IBoolExpression[] whereRestrictions = new IBoolExpression[] { };
        string[] columnsWithAlias;
        string aditionalSelectClauses = string.Empty;

        const string newLine = "\n";
        const string distinct = "DISTINCT";
        const string top = "TOP {0}";
        const string selectStructure = "SELECT {0} {1} " + newLine;
        const string deleteStructure = "DELETE " + newLine;
        const string countStructure = "SELECT COUNT(*) " + newLine;
        const string fromWithAliasStructure = "FROM {0} {1} " + newLine;
        const string fromNoAliasStructure = "FROM {0} " + newLine;
        const string whereStructure = "WHERE {0} " + newLine;
        const string columnSeparator = ", ";

        public QueryBuilder(Type entityType, string alias = null, params string[] columnWithAlias)
        {
            if (entityType == null)
                throw new NullReferenceException($"{nameof(entityType)} must be informed!");

            EntityType = entityType;

            if (string.IsNullOrWhiteSpace(alias))
                alias = EntityType.Name;
            Alias = alias;

            if (columnWithAlias == null || columnWithAlias.Length == 0 || string.IsNullOrWhiteSpace(string.Join(columnSeparator, columnWithAlias)))
                columnWithAlias = GetDefaultColumns();
            columnsWithAlias = columnWithAlias;
        }

        public void AddJoin<L, R>(JoinType joinType, string alias = null, params IBoolExpression[] condition) where L : class where R : class
        {
            if (string.IsNullOrWhiteSpace(alias))
                alias = typeof(R).Name;

            ValidateJoinInclusion<L, R>(joinType, alias, condition);

            var newElement = new List<Join>();
            if (joins != null && joins.Length > 0)
                newElement.AddRange(joins);
            newElement.AddRange(new[] { new Join<L, R>(joinType, alias, condition) });
            joins = newElement.ToArray();
        }

        private void ValidateJoinInclusion<L, R>(JoinType joinType, string alias, params IBoolExpression[] condition) where L : class where R : class
        {
            if (joins.Any(j => j.TRight == typeof(R) && j.Alias == alias))
                throw new InvalidOperationException($"One Join with then { typeof(R).Name } entity is already added with the alias '{ alias }'.");

            if (condition == null || condition.Length == 0 || condition[0] == null)
                throw new InvalidOperationException($"One IBoolExpression at least is required for create a Join statement.");

            if (!joins.Any(j => j.TLeft == typeof(L) || j.TRight == typeof(L)) && typeof(L) != EntityType)
                throw new InvalidOperationException($"The Left Type must be an previously added type in the join list or reference the parent type ({ EntityType.Name }).");
        }

        public void AddWhereCondition(params IBoolExpression[] condition)
        {
            var newItens = new List<IBoolExpression>();
            if (whereRestrictions != null && whereRestrictions.Length > 0)
                newItens.AddRange(whereRestrictions);
            foreach (var cond in condition)
                newItens.Add(cond);
            whereRestrictions = newItens.ToArray();
        }

        public static string BuildSelectDistinct(Type entityType, string alias = null, string columnWithAlias = null, params IBoolExpression[] condition)
        {
            QueryBuilder qb = new QueryBuilder(entityType, alias, columnWithAlias);

            if (condition != null)
                foreach (var item in condition)
                    qb.AddWhereCondition(item);

            return qb.BuildSelectDistinct();
        }

        public static string BuildCount(Type entityType, params IBoolExpression[] condition)
        {
            QueryBuilder qb = new QueryBuilder(entityType);

            if (condition != null)
                qb.AddWhereCondition(condition);

            return qb.BuildCount();
        }

        // ToDo: Refazer a construção da cláusula de delete. Colocar um subselect com um exists
        //public static string BuildDelete(Type entityType, params IBoolExpression[] condition)
        //{
        //    QueryBuilder qb = new QueryBuilder(entityType);

        //    if (condition != null)
        //        qb.AddWhereCondition(condition);

        //    return qb.BuildDelete();
        //}

        public string BuildSelectDistinct()
        {
            aditionalSelectClauses = distinct;
            return BuildSelect();
        }

        public string BuildSelect()
        {
            string ret = string.Empty;

            ret += GetSelectClause();
            ret += BuildBase();

            aditionalSelectClauses = string.Empty;
            return ret;
        }

        public string BuildSelectTop(int regs = 10)
        {
            if (regs <= 0)
                throw new ArgumentOutOfRangeException();

            aditionalSelectClauses = string.Format(top, regs);

            return BuildSelect();
        }

        private string BuildBase(bool fromWithAlias = true)
        {
            string ret = string.Empty;

            ret += GetFromClause(fromWithAlias);
            ret += GetWhereClause();

            return ret;
        }

        // ToDo: Refazer a construção da cláusula de delete. Colocar um subselect com um exists
        //private virtual string BuildDelete()
        //{
        //    string ret = string.Empty;

        //    ret += GetDeleteClause();
        //    ret += BuildBase(false);

        //    return ret;
        //}

        public virtual string BuildCount()
        {
            string ret = string.Empty;

            ret += GetCountClause();
            ret += BuildBase(true);

            return ret;
        }

        private string GetColumns()
        {
            string columns = string.Join(columnSeparator, columnsWithAlias);

            return columns;
        }

        private string[] GetDefaultColumns()
        {
            return Util.GetDatabaseColumnsForType(EntityType, Alias);
        }

        private string GetSelectClause()
        {
            string ret = string.Empty;
            string columns = GetColumns();
            ret = string.Format(selectStructure, aditionalSelectClauses, columns);
            return ret;
        }

        private string GetDeleteClause()
        {
            string ret = deleteStructure;
            return ret;
        }

        private string GetCountClause()
        {
            string ret = countStructure;
            return ret;
        }

        private string GetFromClause(bool withAlias = true)
        {
            string ret = string.Empty;

            if (withAlias)
                ret = string.Format(fromWithAliasStructure, EntityType.Name, Alias);
            else
                ret = string.Format(fromNoAliasStructure, EntityType.Name);

            foreach (var join in joins)
                ret += join.GetJoinStatement();

            return ret;
        }

        private string GetWhereClause()
        {
            string ret = string.Empty;
            string criteria = string.Empty;

            for (int i = 0; i < whereRestrictions.Length; i++)
            {
                var condition = whereRestrictions[i];
                criteria += condition.GetStatement();
                if (i + 1 < whereRestrictions.Length)
                    criteria += condition.GetLogicalOperatorString();

                criteria += newLine;
            }

            if (!string.IsNullOrWhiteSpace(criteria))
                ret = string.Format(whereStructure, criteria);

            return ret;
        }

        public int JoinClausesCount { get { return joins.Length; } }
        public int WhereConditionsCount { get { return whereRestrictions.Length; } }

        private class Join
        {
            public Type TLeft { get; }
            public Type TRight { get; }

            const string joinStructure = "    {0} JOIN {1} {2} ON {3} \n";
            JoinType type;
            List<IBoolExpression> conditions = new List<IBoolExpression>();

            public string Alias { get; }

            public Join(Type leftType, Type rightType, JoinType _type, string alias = null, params IBoolExpression[] condition)
            {
                TLeft = leftType;
                TRight = rightType;

                if (string.IsNullOrWhiteSpace(alias))
                    alias = TRight.Name;

                Alias = alias;
                type = _type;
                foreach (var cond in condition)
                    conditions.Add(cond);
            }

            public string GetJoinStatement()
            {
                string ret = string.Empty;
                string criteria = string.Empty;

                for (int i = 0; i < conditions.Count; i++)
                {
                    criteria += conditions[i].GetStatement();
                    if (i + 1 < conditions.Count)
                        criteria += conditions[i].GetLogicalOperatorString();
                }

                ret = string.Format(joinStructure, type.GetAttribute<EnumMetadataAttribute>().Value.ToString(), TRight.Name, Alias, criteria);

                return ret;
            }
        }

        private class Join<TLeft, TRight> : Join
        {
            public Join(JoinType _type, string alias = null, params IBoolExpression[] condition) : base(typeof(TLeft), typeof(TRight), _type, alias, condition)
            {

            }
        }
    }
}
