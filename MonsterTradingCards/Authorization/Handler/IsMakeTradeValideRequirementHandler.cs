using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.Contracts.Factory;
using MonsterTradingCards.Contracts.Repository;
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
                return AuthorizationResult.Fail("You need to be logged in");

            Trading foundTrade = tradingRepository.Get(requirement.TradeId);
            Card foundCard = cardRepository.Get(requirement.CardId);

            if (foundCard == null)
                return AuthorizationResult.Fail("This card does not exist");

            if (foundTrade == null)
                return AuthorizationResult.Fail("This trade does not exist");

            if (foundCard.UserId != userContext.User.Id)
                return AuthorizationResult.Fail("Not the user of the card");

            if (foundCard.IsInDeck)
                return AuthorizationResult.Fail("Card is currently in the deck. Please remove first");

            if (foundTrade.UserId == userContext.User.Id)
                return AuthorizationResult.Fail("It is not allowed to trade with yourself");

            if (foundTrade.MinimumDamage > foundCard.Damage || foundTrade.Type != foundCard.CardType)
                return AuthorizationResult.Fail("The card you offer does not meet the requirements");

            return AuthorizationResult.Succeed();
        }
    }
}
