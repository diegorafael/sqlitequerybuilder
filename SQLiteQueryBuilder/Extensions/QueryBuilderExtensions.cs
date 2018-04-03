using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SQLiteQueryBuilder.Extensions
{
    public static class QueryBuilderExtensions
    {
        public static IBoolExpression GetCondition<T>(this T type, string propertyName, Enumerations.BoolComparisonType comparisonType = Enumerations.BoolComparisonType.Equals, object value = null)
        {
            return GetCondition<T>(type, null, propertyName, comparisonType, value);
        }

        public static IBoolExpression GetCondition<T>(this T type, string entityAlias, string propertyName, Enumerations.BoolComparisonType comparisonType = Enumerations.BoolComparisonType.Equals, object value = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return new Condition(typeof(T), entityAlias, propertyName, value?.GetType(), null, value, comparisonType);
        }

        // ToDo: Implement
        //public static IBoolExpression GetCondition<T>(this T type, Expression<Func<T, bool>> expression = null)
        //{
        //    // Handle Expression 
        //    return null;
        //}
    }
}
