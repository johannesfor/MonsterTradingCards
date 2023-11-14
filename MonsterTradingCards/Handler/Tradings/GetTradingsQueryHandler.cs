using MediatR;
using MonsterTradingCards.CAQ.Tradings;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Tradings
{
    public class GetTradingsQueryHandler : IRequestHandler<GetTradingsQuery, IEnumerable<Trading>>
    {
        private readonly IUserContext userContext;
        private readonly ITradingRepository tradingRepository;

        public GetTradingsQueryHandler(IUserContext userContext, ITradingRepository tradingRepository)
        {
            this.userContext = userContext;
            this.tradingRepository = tradingRepository;
        }

        public async Task<IEnumerable<Trading>> Handle(GetTradingsQuery request, CancellationToken cancellationToken)
        {
            return tradingRepository.GetAll().Where(trade => trade.UserId != userContext.User.Id);
        }
    }
}