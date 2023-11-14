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
        public GetUserStatsQueryHandler(IUserContext userContext)
        {
            this.userContext = userContext;
        }

        public async Task<UserStats> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
        {
            return new UserStats()
            {
                Elo = userContext.User.Elo,
                PlayedGames = userContext.User.PlayedGames,
            };
        }
    }
}