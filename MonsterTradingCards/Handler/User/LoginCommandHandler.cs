using MediatR;
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
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private IUserRepository userRepository;
        public LoginCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            User foundUser = userRepository.GetByUsername(request.Username);
            if (foundUser == null)
                throw new ArgumentException("A user with this username is not existing");

            if (foundUser.Password != request.Password.HashPassword())
                throw new ArgumentException("Invalid password");

            //TODO: Eigentlich muss hier ein richtiger JWT Token erstellt werden mittels Secret. Inhalt ist der Benutzername.
            //Aufgrund des vorgegeben Curl-Skripts kann ich das leider nicht machen
            return foundUser.Username + "-mtcgToken";
        }
    }
}