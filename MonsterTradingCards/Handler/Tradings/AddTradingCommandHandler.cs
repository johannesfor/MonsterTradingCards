using MediatR;
using MonsterTradingCards.CAQ.Tradings;
using MonsterTradingCards.Contracts.Factory;
using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Tradings
{
    public class AddTradingCommandHandler : IRequestHandler<AddTradingCommand>
    {
        private readonly IUserContext userContext;
        private readonly ITradingRepository tradingRepository;

        public AddTradingCommandHandler(IUserContext userContext, ITradingRepository tradingRepository)
        {
            this.userContext = userContext;
            this.tradingRepository = tradingRepository;
        }

        public async Task Handle(AddTradingCommand request, CancellationToken cancellationToken)
        {
            request.Trading.UserId = userContext.User.Id.Value;
            tradingRepository.Add(request.Trading);
        }
    }
}