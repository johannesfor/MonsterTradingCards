using MediatR.Behaviors.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Requirement
{
    public class IsUsernameEqualToUserContextRequirement : IAuthorizationRequirement
    {
        public string Username { get; set; }
    }
}
