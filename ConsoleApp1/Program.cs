using SQLiteQueryBuilder;
using SQLiteQueryBuilder.Enumerations;
using SQLiteQueryBuilder.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            QueryBuilder qb = new QueryBuilder<XYZ>();
            qb.AddWhereCondition(new XYZ().GetCondition(nameof(XYZ.prop2), BoolComparisonType.Equals, "haha"));

            Console.WriteLine(qb.BuildSelectDistinct());
        }

        class XYZ
        {
            public int prop1 { get; set; }
            public string prop2 { get; set; }
        }
    }
}
