using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Models.Base
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnAttribute : Attribute
    {
        public string ColumnName { get; }
        public DbType DbType { get; }
        public bool IsPrimaryKey { get; }

        public DbColumnAttribute(string columnName, DbType dbType, bool isPrimaryKey = false)
        {
            ColumnName = columnName;
            DbType = dbType;
            IsPrimaryKey = isPrimaryKey;
        }
    }
}
