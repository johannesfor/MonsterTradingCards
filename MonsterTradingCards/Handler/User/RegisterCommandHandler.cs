using MediatR;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MonsterTradingCards.CAQ.User;
using MonsterTradingCards.Contracts;
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
        public RegisterCommandHandler(IUserRepository userRepository) {
            this.userRepository = userRepository;
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
                Coins = 0,
                Elo = 100,
                PlayedGames = 0
            };

            userRepository.Add(user);

            //TODO: Eigentlich muss hier ein richtiger JWT Token erstellt werden mittels Secret. Inhalt ist der Benutzername.
            //Aufgrund des vorgegeben Curl-Skripts kann ich das leider nicht machen
            return user.Username + "-mtcgToken";
        }
    }
}
