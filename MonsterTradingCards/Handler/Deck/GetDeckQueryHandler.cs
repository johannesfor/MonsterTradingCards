using MediatR;
using MonsterTradingCards.CAQ.Cards;
using MonsterTradingCards.CAQ.Deck;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Deck
{
    public class GetDeckQueryHandler : IRequestHandler<GetDeckQuery, IEnumerable<Card>>
    {
        private ICardRepository cardRepository;
        private IUserContext userContext;
        public GetDeckQueryHandler(ICardRepository cardRepository, IUserContext userContext)
        {
            this.cardRepository = cardRepository;
            this.userContext = userContext;
        }

        public async Task<IEnumerable<Card>> Handle(GetDeckQuery request, CancellationToken cancellationToken)
        {
            return cardRepository.GetAllByUserId(userContext.User.Id.Value, true);
        }
    }
}