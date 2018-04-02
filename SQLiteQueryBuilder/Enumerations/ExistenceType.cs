using SQLiteQueryBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteQueryBuilder.Enumerations
{
    public enum ExistenceType
    {
        [EnumMetadata(nameof(Exists), "exists", "EXISTS")]
        Exists,

        [EnumMetadata(nameof(NotExists), "nexists", "NOT EXISTS")]
        NotExists
    }
}
