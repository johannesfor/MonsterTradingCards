using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Contracts
{
    public interface IUserContext
    {
        public User User { get; set; }
        public bool IsAdmin { get; set; }
    }
}
