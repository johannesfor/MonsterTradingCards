using MediatR;
using MonsterTradingCards.CAQ.Tradings;
using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Tradings
{
    public class RemoveTradingCommandHandler : IRequestHandler<RemoveTradingCommand>
    {
        private readonly ITradingRepository tradingRepository;

        public RemoveTradingCommandHandler(ITradingRepository tradingRepository)
        {
            this.tradingRepository = tradingRepository;
        }

        public async Task Handle(RemoveTradingCommand request, CancellationToken cancellationToken)
        {
            tradingRepository.Delete(request.TradeId);
        }
    }
}