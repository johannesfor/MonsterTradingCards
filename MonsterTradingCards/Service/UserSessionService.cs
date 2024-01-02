using MonsterTradingCards.Contracts.Service;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Service
{
    public class UserSessionService : IUserSessionService
    {
        private static readonly List<UserSession> sessions = new List<UserSession>();

        public string CreateSession(string username)
        {
            string token = username + "-mtcgToken"; //Hier müsste eig. ein richtiger Token erzeugt werden, bedingt durch das CURL-Skript fällt dies weg
            UserSession session = new UserSession(token, username);
            sessions.Add(session);
            return token;
        }

        public string GetUsernameByToken(string token)
        {
            UserSession? userSession = sessions.Find(user => user.Token == token);
            return userSession?.UserName;
        }

        public bool IsTokenValid(string token)
        {
            UserSession? userSession = sessions.Find(user => user.Token == token);
            return userSession != null;
        }
        
    }
}
