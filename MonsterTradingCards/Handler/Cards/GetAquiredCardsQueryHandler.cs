using MediatR;
using MonsterTradingCards.CAQ.Cards;
using MonsterTradingCards.CAQ.Packages;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Cards
{
    public class GetAquiredCardsQueryHandler : IRequestHandler<GetAquiredCardsQuery, IEnumerable<Card>>
    {
        private ICardRepository cardRepository;
        private IUserContext userContext;
        public GetAquiredCardsQueryHandler(ICardRepository cardRepository, IUserContext userContext)
        {
            this.cardRepository = cardRepository;
            this.userContext = userContext;
        }

        public async Task<IEnumerable<Card>> Handle(GetAquiredCardsQuery request, CancellationToken cancellationToken)
        {
            return cardRepository.GetAllByUserId(userContext.User.Id.Value, false);
        }
    }
}