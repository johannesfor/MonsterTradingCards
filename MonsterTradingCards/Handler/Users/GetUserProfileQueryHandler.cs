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
        private IUserRepository userRepository;
        public GetUserProfileQueryHandler(IUserContext userContext, IUserRepository userRepository)
        {
            this.userContext = userContext;
            this.userRepository = userRepository;
        }

        public async Task<UserProfile> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            User foundUser = userRepository.GetByUsername(request.UserName);

            if (foundUser == null)
                return null;

            return new UserProfile()
            {
                Name = foundUser.Name,
                Bio = foundUser.Bio,
                Image = foundUser.Image
            };
        }
    }
}