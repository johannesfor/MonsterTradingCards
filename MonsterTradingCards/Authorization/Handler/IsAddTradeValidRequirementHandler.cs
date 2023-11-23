using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Handler
{
    public class IsAddTradeValidRequirementHandler : IAuthorizationHandler<IsAddTradeValidRequirement>
    {
        private readonly IUserContext userContext;
        private readonly ICardRepository cardRepository;
        private readonly ITradingRepository tradingRepository;

        public IsAddTradeValidRequirementHandler(IUserContext userContext, ICardRepository cardRepository, ITradingRepository tradingRepository)
        {
            this.userContext = userContext;
            this.cardRepository = cardRepository;
            this.tradingRepository = tradingRepository;
        }
        public async Task<AuthorizationResult> Handle(IsAddTradeValidRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail();

            Card foundCard = cardRepository.Get(requirement.Trading.CardToTrade);

            if (foundCard == null)
                return AuthorizationResult.Fail("Diese Karte existiert nicht");

            if (foundCard.UserId != userContext.User.Id)
                return AuthorizationResult.Fail("Dieser Benutzer ist nicht der Besitzer dieser Karte");

            if (foundCard.IsInDeck)
                return AuthorizationResult.Fail("Befindet sich derzeitig im Deck. Bitte zuerst entfernen");

            Trading foundTrade = tradingRepository.GetByCardId(foundCard.Id.Value);

            if (foundTrade != null)
                return AuthorizationResult.Fail("Es gibt bereits einen Trade mit dieser Card");

            return AuthorizationResult.Succeed();
        }
    }
}
