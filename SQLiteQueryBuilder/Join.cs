using SQLiteQueryBuilder.Attributes;
using SQLiteQueryBuilder.Enumerations;
using SQLiteQueryBuilder.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteQueryBuilder
{
    internal class Join<TLeft, TRight> : Join
    {
        public Join(JoinType _type, string alias = null, params IBoolExpression[] condition) : base(typeof(TLeft), typeof(TRight), _type, alias, condition)
        {

        }
    }

    internal class Join
    {
        public Type TLeft { get; }
        public Type TRight { get; }

        protected const string joinStructure = "    {0} JOIN {1} {2} ON {3} \n";
        protected JoinType type;
        protected List<IBoolExpression> conditions = new List<IBoolExpression>();

        public string Alias { get; }

        public Join(Type leftType, Type rightType, JoinType _type, string alias = null, params IBoolExpression[] condition)
        {
            TLeft = leftType;
            TRight = rightType;

            if (string.IsNullOrWhiteSpace(alias))
                alias = QueryBuilder.GetDefaultAliasTo(TRight);

            Alias = alias;
            type = _type;
            foreach (var cond in condition)
                conditions.Add(cond);
        }

        public virtual string GetJoinStatement()
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

    internal class SubqueryJoin<T> : Join
    {
        internal const string SubqueryDefaultAlias = "subquery";

        public QueryBuilder SubqueryBuilder { get; }
        public FieldReference<T>[] SubgroupsToRanking { get; }
        public RankingField<T>[] RankingFields { get; }

        public SubqueryJoin(QueryBuilder qb, FieldReference<T>[] subgroupsToRanking, RankingField<T>[] rankingFields)
            : base(typeof(T), null, JoinType.Inner, qb.GetNextAlias(SubqueryDefaultAlias), new Condition<T,T>(Util.GetPrimaryKeyPropertyName(Activator.CreateInstance(typeof(T))), qb.Alias, BoolComparisonType.Equals, Util.GetPrimaryKeyPropertyName(Activator.CreateInstance(typeof(T))),  qb.GetNextAlias(SubqueryDefaultAlias)))
        {
            SubqueryBuilder = qb;
        }

        public override string GetJoinStatement()
        {
            string ret = string.Empty;
            string criteria = string.Empty;

            for (int i = 0; i < conditions.Count; i++)
            {
                criteria += conditions[i].GetStatement();
                if (i + 1 < conditions.Count)
                    criteria += conditions[i].GetLogicalOperatorString();
            }

            ret = string.Format(joinStructure, type.GetAttribute<EnumMetadataAttribute>().Value.ToString(), string.Format("(\n {0} \n)", SubqueryBuilder.BuildSelect()), Alias, criteria);

            return ret;
        }
    }

    internal class RankingField<T> : FieldReference<T>
    {
        public Order Order { get; }

        public RankingField(Expression<Func<T, object>> field, Order order = Order.Ascending) : base(field)
        {
            Order = order;
        }
    }

    internal class FieldReference<T>
    {
        public Expression<Func<T, object>> Field { get; }

        public FieldReference(Expression<Func<T, object>> field)
        {
            Field = field;
        }
    }
}
