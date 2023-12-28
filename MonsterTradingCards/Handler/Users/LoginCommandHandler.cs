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
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private IUserRepository userRepository;
        private IUserSessionService userSessionService;
        public LoginCommandHandler(IUserRepository userRepository, IUserSessionService userSessionService)
        {
            this.userRepository = userRepository;
            this.userSessionService = userSessionService;
        }
        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            User foundUser = userRepository.GetByUsername(request.Username);
            if (foundUser == null)
                throw new ArgumentException("A user with this username is not existing");

            if (foundUser.Password != request.Password.HashPassword())
                throw new ArgumentException("Invalid password");

            return userSessionService.CreateSession(foundUser.Username);
        }
    }
}