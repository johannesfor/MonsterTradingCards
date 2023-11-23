using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Factory
{
    public class UserContextFactory : IUserContextFactory
    {
        private readonly IUserRepository userRepository;

        public UserContextFactory(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IUserContext Create(string authorizationToken)
        {
            //Normalerweise würde dieser eigentliche JWT Token, die Email enthalten und wäre signiert mit einem privaten Key, umso sicherzustellen, dass niemand den Token gefaked hat
            //Aufgrund des vorgegeben CURL-Skripts von der Angabe ist das hardcoded
            if (authorizationToken != null)
            {
                User foundUser = userRepository.GetByUsername(authorizationToken.Split(" ")[1].Split("-")[0]);
                if (foundUser != null)
                    return new UserContext() { User = foundUser, IsAdmin = foundUser.Username.Equals("admin") };
            }
            return new UserContext();
        }
    }
}
