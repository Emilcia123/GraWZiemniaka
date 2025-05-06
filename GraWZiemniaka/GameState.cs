using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraWZiemniaka
{
    public class GameState
    {
        public List<DaneGracza> Players { get; set; } = new();
    }

    public class DaneGracza
    {
        public string Name { get; set; }
        public int Wynik { get; set; }
    }
}
