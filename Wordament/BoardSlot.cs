using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordamentCheater.Wordament
{
    public class BoardSlot
    {
        public string Letter { get; set; }
        public BoardSlot[] Neighbors { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
