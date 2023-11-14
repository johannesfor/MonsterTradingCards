using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class UpdateDeckCommandAuthorizer : AbstractRequestAuthorizer<UpdateDeckCommand>
    {
        public override void BuildPolicy(UpdateDeckCommand request)
        {
            UseRequirement(new IsUpdateDeckValidRequirement() { CardIds = request.CardIds });
        }
    }
}
