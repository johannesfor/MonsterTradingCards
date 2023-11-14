using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Models
{
    public class User
    {
        public Guid? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Bio {  get; set; }
        public string Image { get; set; }
        public int Coins { get; set; }
        public int Elo {  get; set; }
        public int PlayedGames { get; set; }
        public string Name { get; set; }
    }
}
