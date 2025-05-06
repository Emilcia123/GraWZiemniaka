using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraWZiemniaka
{
    public class Player
    {
        public string Name { get; }
        public int Wynik { get; private set; }

        public Player(string name)
        {
            Name = name;
            Wynik = 0;
        }

        public void DodajWynik(int punkty)
        {
            Wynik += punkty;
        }
    }
}
