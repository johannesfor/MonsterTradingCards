using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Models
{
    public class BattleTask
    {
        public Guid UserId { get; set; }
        public TaskCompletionSource<bool> Task { get; set; }
        public IEnumerable<string> Log {  get; set; }
    }
}
