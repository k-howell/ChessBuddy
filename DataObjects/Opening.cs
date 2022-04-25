using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Opening
    {
        public string ECO { get; set; }
        public string Name { get; set; }
        public string Variation { get; set; }

        // public List<Move> Moves { get; set; } // in future implementation of playing openings like a game
    }
}
