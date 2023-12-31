﻿using MediatR;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.CAQ.Users
{
    public class UpdateUserProfileCommand : IRequest
    {
        public UserProfile UserProfile { get; set; }
        public string Username { get; set; }
    }
}
