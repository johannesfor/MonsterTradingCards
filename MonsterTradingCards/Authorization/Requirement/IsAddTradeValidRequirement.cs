using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Requirement
{
    public class IsAddTradeValidRequirement : IAuthorizationRequirement
    {
        public Trading Trading { get; set; }
    }
}
