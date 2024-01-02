using MonsterTradingCards.Contracts.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Models
{
    public class UserContext : IUserContext
    {
        public User User { get; set; }
        public bool IsAdmin { get; set; }
    }
}
