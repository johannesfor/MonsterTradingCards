using MediatR;
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
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand>
    {
        private IUserRepository userRepository;
        private IUserContext userContext;
        public UpdateUserProfileCommandHandler(IUserRepository userRepository, IUserContext userContext)
        {
            this.userRepository = userRepository;
            this.userContext = userContext;
        }
        public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            User toUpdate = userContext.User;
            toUpdate.Name = request.UserProfile.Name;
            toUpdate.Bio = request.UserProfile.Bio;
            toUpdate.Image = request.UserProfile.Image;

            userRepository.Update(toUpdate, nameof(User.Name), nameof(User.Bio), nameof(User.Image));
        }
    }
}