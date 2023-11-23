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
    public class IsMakeTradeValideRequirementHandler : IAuthorizationHandler<IsMakeTradeValideRequirement>
    {
        private readonly IUserContext userContext;
        private readonly ICardRepository cardRepository;
        private readonly ITradingRepository tradingRepository;

        public IsMakeTradeValideRequirementHandler(IUserContext userContext, ICardRepository cardRepository, ITradingRepository tradingRepository)
        {
            this.userContext = userContext;
            this.cardRepository = cardRepository;
            this.tradingRepository = tradingRepository;
        }
        public async Task<AuthorizationResult> Handle(IsMakeTradeValideRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail();

            Trading foundTrade = tradingRepository.Get(requirement.TradeId);
            Card foundCard = cardRepository.Get(requirement.CardId);

            if (foundCard == null)
                return AuthorizationResult.Fail("Diese Karte existiert nicht");

            if (foundTrade == null)
                return AuthorizationResult.Fail("Dieser Trade existiert nicht");

            if (foundCard.UserId != userContext.User.Id)
                return AuthorizationResult.Fail("Nicht der Benutzer der Karte");

            if (foundCard.IsInDeck)
                return AuthorizationResult.Fail("Karte ist derzeitig im Deck");

            if (foundTrade.UserId == userContext.User.Id)
                return AuthorizationResult.Fail("Mit sich selber darf nicht gehandelt werden");

            if (foundTrade.MinimumDamage > foundCard.Damage || foundTrade.Type != foundCard.CardType)
                return AuthorizationResult.Fail("Die von dir angebotene Karte entspricht nicht den Requirements");

            return AuthorizationResult.Succeed();
        }
    }
}
