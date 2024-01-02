using MediatR;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Contracts.Service;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Users
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
    {
        private IUserRepository userRepository;
        private IUserSessionService userSessionService;
        public RegisterCommandHandler(IUserRepository userRepository, IUserSessionService userSessionService)
        {
            this.userRepository = userRepository;
            this.userSessionService = userSessionService;
        }
        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            User foundUser = userRepository.GetByUsername(request.Username);
            if (foundUser != null)
                throw new ArgumentException("Registration with this username is not possible");

            User user = new User()
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Password = request.Password.HashPassword(),
                Coins = 20,
                Elo = 100,
                PlayedGames = 0
            };

            userRepository.Add(user);

            return userSessionService.CreateSession(user.Username);
        }
    }
}
