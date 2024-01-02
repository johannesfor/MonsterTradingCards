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
    public class MakeTradeCommandHandler : IRequestHandler<MakeTradeCommand>
    {
        private readonly IUserContext userContext;
        private readonly ITradingRepository tradingRepository;
        private readonly ICardRepository cardRepository;

        public MakeTradeCommandHandler(IUserContext userContext, ITradingRepository tradingRepository, ICardRepository cardRepository)
        {
            this.userContext = userContext;
            this.tradingRepository = tradingRepository;
            this.cardRepository = cardRepository;
        }

        public async Task Handle(MakeTradeCommand request, CancellationToken cancellationToken)
        {
            Trading foundTrade = tradingRepository.Get(request.TradeId);

            cardRepository.Update(new Card()
            {
                Id = foundTrade.CardToTrade,
                UserId = userContext.User.Id
            }, nameof(Card.UserId));

            cardRepository.Update(new Card()
            {
                Id = request.CardId,
                UserId = foundTrade.UserId
            }, nameof(Card.UserId));

            tradingRepository.Delete(foundTrade.Id.Value);
        }
    }
}