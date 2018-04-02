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
    public class Condition<T, K> : Condition
    {
        public Condition(string leftSidePropertyName,
                         BoolComparisonType comparison = BoolComparisonType.Equals,
                         object rightSideValueOrProperty = null,
                         LogicalOperatorType sufixOperator = LogicalOperatorType.And) : base(typeof(T),
                                                                                       null,
                                                                                       leftSidePropertyName,
                                                                                       typeof(K),
                                                                                       null,
                                                                                       rightSideValueOrProperty,
                                                                                       comparison,
                                                                                       sufixOperator)
        { }

        public Condition(string leftSidePropertyName,
                         string leftSideAlias = null,
                         BoolComparisonType comparison = BoolComparisonType.Equals,
                         object rightSideValueOrProperty = null,
                         string rightSideAlias = null,
                         LogicalOperatorType sufixOperator = LogicalOperatorType.And) : base(typeof(T),
                                                                                       leftSideAlias,
                                                                                       leftSidePropertyName,
                                                                                       typeof(K),
                                                                                       rightSideAlias,
                                                                                       rightSideValueOrProperty,
                                                                                       comparison,
                                                                                       sufixOperator)
        { }
    }

    public class Condition : BaseBoolExpression
    {
        protected string LeftSidePropertyName { get; }
        protected string LeftSideAlias { get; }
        protected string RightSideAlias { get; }
        protected object RightSideProperty { get; }
        protected BoolComparisonType ComparisonType { get; }
        protected Type LeftType { get; set; }
        protected Type RightType { get; set; }

        const string textValueFormat = "'{0}'";
        const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        const string timeFormat = @"hh\:mm\:ss";

        public Condition(Type leftType, string leftSideAlias, string leftSidePropertyName, Type rightType, string rightSideAlias, object rightSideValueOrProperty = null, BoolComparisonType comparison = BoolComparisonType.Equals, LogicalOperatorType sufixOperator = LogicalOperatorType.And)
        {
            LeftType = leftType;
            RightType = rightType;

            if (string.IsNullOrWhiteSpace(leftSideAlias))
                leftSideAlias = leftType.Name;

            if (string.IsNullOrWhiteSpace(rightSideAlias))
                rightSideAlias = rightType.Name;

            LeftSidePropertyName = leftSidePropertyName;
            LeftSideAlias = leftSideAlias;
            ComparisonType = comparison;
            RightSideProperty = rightSideValueOrProperty;
            RightSideAlias = rightSideAlias;
            LogicalOperator = sufixOperator;

            ValidateCondition();
        }

        private void ValidateCondition()
        {
            // ToDo: Throw exception if an invalid condition is created
        }

        public override string GetStatement()
        {
            string ret = string.Empty;
            string statementStructure = " {0}.{1} {2} {3} ";

            ret = string.Format(statementStructure, LeftSideAlias, LeftSidePropertyName, GetComparisonTypeOperator(), GetRightSideValue());
            return ret;
        }

        private string GetComparisonTypeOperator()
        {
            string ret = string.Empty;

            if (RightSideProperty == null)
            {
                if (ComparisonType == BoolComparisonType.Equals)
                    ret = "IS";
                else if (ComparisonType == BoolComparisonType.NotEquals)
                    ret = "IS NOT";
                else
                    throw new InvalidOperationException();
            }
            else
                ret = ComparisonType.GetAttribute<EnumMetadataAttribute>()?.Value?.ToString();

            return ret;
        }

        private string GetRightSideValue()
        {
            string ret = string.Empty;
            string elementsSeparator = ", ";

            if (ComparisonType == BoolComparisonType.In || ComparisonType == BoolComparisonType.NotIn)
            {
                IEnumerable<string> inValues = new string[] { };
                ret = "( {0} )";
                var elements = (RightSideProperty as IEnumerable);

                if (elements != null)
                {
                    foreach (var item in elements)
                        if (RightType == typeof(string) || RightType == typeof(char))
                            inValues = inValues.Union(new string[] { string.Format(textValueFormat, item) });
                        else if (RightType == typeof(DateTime))
                            inValues = inValues.Union(new string[] { string.Format(textValueFormat, ((DateTime)item).ToString(dateTimeFormat)) });
                        else if (RightType == typeof(TimeSpan))
                            inValues = inValues.Union(new string[] { string.Format(textValueFormat, ((TimeSpan)item).ToString(timeFormat)) });
                        else
                            inValues = inValues.Union(new string[] { item.ToString() });

                    ret = string.Format(ret, string.Join(elementsSeparator, inValues));
                }
                else
                    ret = string.Format(ret, (RightSideProperty as string));
            }
            else
            {
                if (RightSideProperty == null)
                    ret = "NULL";
                else
                {
                    if (RightType == typeof(string) || RightType == typeof(char))
                        ret = string.Format(textValueFormat, RightSideProperty);
                    else if (RightType == typeof(DateTime))
                        ret = string.Format(textValueFormat, ((DateTime)RightSideProperty).ToString(dateTimeFormat));
                    else if (RightType == typeof(TimeSpan))
                        ret = string.Format(textValueFormat, ((TimeSpan)RightSideProperty).ToString(timeFormat));
                    else if (RightType == typeof(bool))
                        ret = Convert.ToInt16(RightSideProperty).ToString() ?? "0";
                    else if (RightType == typeof(int) || RightType == typeof(long) || RightType == typeof(decimal) || RightType == typeof(float) || RightType == typeof(double) || RightType == typeof(byte) || RightType.GetTypeInfo().IsAssignableFrom(typeof(Enum).GetTypeInfo()))
                        ret = RightSideProperty.ToString();
                    else
                        ret = string.Concat(RightSideAlias, ".", RightSideProperty.ToString());
                }
            }

            return ret;
        }
    }
}
