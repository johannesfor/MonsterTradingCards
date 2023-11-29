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
    public class IsUpdateDeckValidRequirementHandler : IAuthorizationHandler<IsUpdateDeckValidRequirement>
    {
        private readonly IUserContext userContext;
        private readonly ICardRepository cardRepository;

        public IsUpdateDeckValidRequirementHandler(IUserContext userContext, ICardRepository cardRepository)
        {
            this.userContext = userContext;
            this.cardRepository = cardRepository;
        }
        public async Task<AuthorizationResult> Handle(IsUpdateDeckValidRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail("You need to be logged in");

            if (requirement.CardIds.Count() != 4)
                return AuthorizationResult.Fail("The cards given are too many or too few for the deck. Need to be 4");

            IEnumerable<Guid> cardsOfUser = cardRepository.GetAllByUserId(userContext.User.Id.Value, false).Select(card => card.Id.Value);

            if (cardsOfUser.Intersect(requirement.CardIds).Count() != requirement.CardIds.Count())
                return AuthorizationResult.Fail("The user is not in possession of these cards");

            return AuthorizationResult.Succeed();
        }
    }
}
