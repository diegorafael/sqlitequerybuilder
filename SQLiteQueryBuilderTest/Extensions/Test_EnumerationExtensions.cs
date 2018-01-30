using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLiteQueryBuilder.Attributes;
using SQLiteQueryBuilder.Extensions;

namespace SQLiteQueryBuilderTest.Extensions
{
    /// <summary>
    /// Teste unitário da classe EnumerationExtensions
    /// </summary>
    [TestClass]
    public class Test_EnumerationExtensions
    {
        private const string Name1 = "Item 1";
        private const string Name2 = "Item 2";
        private const string Abbreviation1 = "Abbr 1";
        private const string Abbreviation2 = "Abbr 2";
        private const int Value1 = 2;
        private const char Value2 = 'X';
        private const string AttrValue = "Some value";

        private Enum EnumWithSharedAttribute { get { return TestEnum.EnumItem1; } }
        private Enum EnumWithSharedAndCustomAttributes { get { return TestEnum.EnumItem2; } }

        /// <summary>
        /// Tenta obter um atributo que conhecidamente está declarado para determinado campo de Enumeração
        /// </summary>
        [TestMethod]
        public void GetAValidAttribute()
        {
            var attr = EnumWithSharedAttribute.GetAttribute<EnumMetadataAttribute>();
            Assert.IsInstanceOfType(attr, typeof(EnumMetadataAttribute));
        }

        /// <summary>
        /// Tenta obter um atributo que conhecidamente não está declarado para determinado campo de Enumeração
        /// </summary>
        [TestMethod]
        public void GetAnInvalidAttribute()
        {
            var attr = EnumWithSharedAttribute.GetAttribute<MyTestAttribute>();
            Assert.IsNull(attr,string.Format(Constants.ERR_RETURNED_A_NOT_EXPECTED_VALUE, nameof(EnumExtensions.GetAttribute), attr?.GetType().Name, "nulo"));
        }

        /// <summary>
        /// Verifica se o tipo de um determinado valor presenta em uma propriedade `object` de um campo de enum equivale ao tipo do valor esperado
        /// </summary>
        [TestMethod]
        public void CheckAttributeValueType()
        {
            var attr = EnumWithSharedAndCustomAttributes.GetAttribute<EnumMetadataAttribute>();
            Assert.IsInstanceOfType(attr.Value, Value2.GetType());
        }

        /// <summary>
        /// Verifica se o valor de uma propriedade `object` de um campo de enum corresponde ao esperado
        /// </summary>
        [TestMethod]
        public void CheckAttributeValue()
        {
            var attr = EnumWithSharedAndCustomAttributes.GetAttribute<MyTestAttribute>();
            Assert.IsInstanceOfType(attr, typeof(MyTestAttribute));
        }
        
        private enum TestEnum
        {
            [EnumMetadata(Name1, Abbreviation1, Value1)]
            EnumItem1 = 0,

            [MyTest(AttrValue)]
            [EnumMetadata(Name2, Abbreviation2, Value2)]
            EnumItem2 = 10
        }

        private class MyTestAttribute : Attribute
        {
            public MyTestAttribute(string valuedAttribute)
            {
                AttributeProperty = valuedAttribute;
            }

            public string AttributeProperty { get; private set; }
        }
    }
}
