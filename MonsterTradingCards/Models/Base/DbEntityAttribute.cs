using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Models.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbEntityAttribute : Attribute
    {
        public string EntityName { get; }

        public DbEntityAttribute(string entityName)
        {
            EntityName = entityName;
        }
    }
}
