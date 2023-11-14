using MediatR;
using MonsterTradingCards.CAQ.Deck;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Users
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfile>
    {
        private IUserContext userContext;
        public GetUserProfileQueryHandler(IUserContext userContext)
        {
            this.userContext = userContext;
        }

        public async Task<UserProfile> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            return new UserProfile()
            {
                Name = userContext.User.Name,
                Bio = userContext.User.Bio,
                Image = userContext.User.Image
            };
        }
    }
}