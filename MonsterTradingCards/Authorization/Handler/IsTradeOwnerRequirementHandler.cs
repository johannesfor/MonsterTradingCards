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
    public class IsTradeOwnerRequirementHandler : IAuthorizationHandler<IsTradeOwnerRequirement>
    {
        private readonly IUserContext userContext;
        private readonly ITradingRepository tradingRepository;

        public IsTradeOwnerRequirementHandler(IUserContext userContext, ICardRepository cardRepository, ITradingRepository tradingRepository)
        {
            this.userContext = userContext;
            this.tradingRepository = tradingRepository;
        }
        public async Task<AuthorizationResult> Handle(IsTradeOwnerRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail();

            Trading foundTrade = tradingRepository.Get(requirement.TradeId);

            if (foundTrade == null)
                return AuthorizationResult.Fail("Trade nicht gefunden");

            if (foundTrade.UserId != userContext.User.Id)
                return AuthorizationResult.Fail("Dieser Benutzer ist nicht der Besitzer des Trades");

            return AuthorizationResult.Succeed();
        }
    }
}
