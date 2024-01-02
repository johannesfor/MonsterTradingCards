using MediatR;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Users
{
    public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, UserStats>
    {
        private IUserContext userContext;
        private IUserRepository userRepository;
        public GetUserStatsQueryHandler(IUserContext userContext, IUserRepository userRepository)
        {
            this.userContext = userContext;
            this.userRepository = userRepository;
        }

        public async Task<UserStats> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
        {
            User user = userRepository.Get(userContext.User.Id.Value);
            return new UserStats()
            {
                Elo = user.Elo,
                PlayedGames = user.PlayedGames,
            };
        }
    }
}