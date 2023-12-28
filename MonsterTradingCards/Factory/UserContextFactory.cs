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
        private readonly IUserSessionService userSessionService;

        public UserContextFactory(IUserRepository userRepository, IUserSessionService userSessionService)
        {
            this.userRepository = userRepository;
            this.userSessionService = userSessionService;
        }

        public IUserContext Create(string authorizationToken)
        {
            string tokenWithoutBearer = authorizationToken?.Split(" ")?[1];

            if (userSessionService.IsTokenValid(tokenWithoutBearer))
            {
                User foundUser = userRepository.GetByUsername(userSessionService.GetUsernameByToken(tokenWithoutBearer));
                if (foundUser != null)
                    return new UserContext() { User = foundUser, IsAdmin = foundUser.Username.Equals("admin") };
            }
            return new UserContext();
        }
    }
}
