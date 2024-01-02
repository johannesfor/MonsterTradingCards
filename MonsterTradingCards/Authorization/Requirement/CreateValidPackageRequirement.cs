using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Requirement
{
    public class CreateValidPackageRequirement : IAuthorizationRequirement
    {
        public IEnumerable<Card> Cards;
    }
}
