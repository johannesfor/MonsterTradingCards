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
                return AuthorizationResult.Fail("You need to be logged in");

            Card foundCard = cardRepository.Get(requirement.Trading.CardToTrade);

            if (foundCard == null)
                return AuthorizationResult.Fail("This card does not exist");

            if (foundCard.UserId != userContext.User.Id)
                return AuthorizationResult.Fail("This user is not the owner of this card");

            if (foundCard.IsInDeck)
                return AuthorizationResult.Fail("Is currently in the deck. Please remove first");

            Trading foundTrade = tradingRepository.GetByCardId(foundCard.Id.Value);

            if (foundTrade != null)
                return AuthorizationResult.Fail("There is already a trade with this card");

            return AuthorizationResult.Succeed();
        }
    }
}
