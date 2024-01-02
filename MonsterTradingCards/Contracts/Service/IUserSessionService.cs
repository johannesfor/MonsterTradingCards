using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Contracts.Service
{
    public interface IUserSessionService
    {
        string CreateSession(string username);
        string GetUsernameByToken(string token);
        bool IsTokenValid(string token);
    }
}
