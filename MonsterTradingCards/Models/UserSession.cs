using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Models
{
    public class UserSession
    {
        public string Token { get; }
        public string UserName { get; }
        public DateTime LoginTimestamp { get; }
        public UserSession(string token, string userName)
        {
            Token = token;
            UserName = userName;
            LoginTimestamp = DateTime.Now;
        }
    }
}
