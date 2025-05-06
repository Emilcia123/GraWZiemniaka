using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraWZiemniaka
{
    public class GameBoard
    {
        public List<Potato> Potatoes { get; set; } = new();
        public int Size { get; }

        public delegate void DelegatZakonczeniaRzedu(Player player, int score);
        public event DelegatZakonczeniaRzedu ZakonczonyRzad;

        public GameBoard(int size)
        {
            Size = size;
            GeneratePotatoes();
        }

        private void GeneratePotatoes()
        {
            Potatoes.Clear();
            for (int x = 0; x <= Size; x++)
            {
                for (int y = 0; y <= Size - x; y++)
                {
                    int z = Size - x - y;
                    Potatoes.Add(new Potato(x, y, z));
                }
            }
        }

        public void OznaczenieZiemniaka(int x, int y, int z, Player player)
        {
            var potato = Potatoes.FirstOrDefault(p => p.X == x && p.Y == y && p.Z == z);
            if (potato == null || potato.IsMarked)
                throw new WyjatekNieprawidlowyRuch("Nieprawidłowy ruch.");

            potato.IsMarked = true;

            var kierunki = new List<Func<Potato, int>> {
        p => p.X,
        p => p.Y,
        p => p.Z
    };

            foreach (var selector in kierunki)
            {
                int value = selector(potato);
                var row = Potatoes.Where(p => selector(p) == value).ToList();

                if (row.All(p => p.IsMarked))
                {
                    int wynik = row.Count;
                    player.DodajWynik(wynik);
                    ZakonczonyRzad?.Invoke(player, wynik);
                }
            }

        }


        public static GameBoard Load(string filename)
        {
            var data = File.ReadAllText(filename);
            return JsonSerializer.Deserialize<GameBoard>(data);
        }


        public List<List<Potato>> GetCompletedRows()
        {
            var kierunki = new List<Func<Potato, int>> {
        p => p.X,
        p => p.Y,
        p => p.Z
    };

            List<List<Potato>> ukonczoneRzedy = new();

            foreach (var selector in kierunki)
            {
                var grouped = Potatoes.GroupBy(selector);
                foreach (var group in grouped)
                {
                    var row = group.ToList();
                    if (row.All(p => p.IsMarked))
                    {
                        ukonczoneRzedy.Add(row);
                    }
                }
            }

            return ukonczoneRzedy;
        }

    }
}