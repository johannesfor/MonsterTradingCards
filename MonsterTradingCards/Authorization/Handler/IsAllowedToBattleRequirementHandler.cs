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
    public class IsAllowedToBattleRequirementHandler : IAuthorizationHandler<IsAllowedToBattleRequirement>
    {
        private readonly IUserContext userContext;
        private readonly ICardRepository cardRepository;

        public IsAllowedToBattleRequirementHandler(IUserContext userContext, ICardRepository cardRepository)
        {
            this.userContext = userContext;
            this.cardRepository = cardRepository;
        }
        public async Task<AuthorizationResult> Handle(IsAllowedToBattleRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail("You need to be logged in");

            IEnumerable<Card> deckOfUser = cardRepository.GetAllByUserId(userContext.User.Id.Value, true);

            if (!deckOfUser.Any())
                return AuthorizationResult.Fail("The user does not have a configured deck and therefore cannot battle");

            return AuthorizationResult.Succeed();
        }
    }
}
