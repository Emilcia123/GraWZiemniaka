using GraWZiemniaka;
using System;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;

namespace AplikacjaDoZiemniaka
{
    public partial class Form1 : Form
    {
        private GameBoard board;
        private Player player1;
        private Player player2;
        private Player aktualnyGracz;

        private FlowLayoutPanel flowLayoutPanel1;
        private Label label1;
        private Label label2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem nowaGraToolStripMenuItem;
        private ToolStripMenuItem zapiszGrêToolStripMenuItem;
        private ToolStripMenuItem wczytajGrêToolStripMenuItem;

        public Form1()
        {
            InitializeComponent();
             Inicjalizacja();
            InitializeGame();
           
        }

        private void Inicjalizacja()
        {
            

            this.Width = 400;
            this.Height = 500;
            this.Text = "Gra w Ziemniaka";

          
            menuStrip1 = new MenuStrip();
            nowaGraToolStripMenuItem = new ToolStripMenuItem("Nowa Gra");

            nowaGraToolStripMenuItem.Click += nowaGraToolStripMenuItem_Click;
            zapiszGrêToolStripMenuItem = new ToolStripMenuItem("Zapisz Grê");
            zapiszGrêToolStripMenuItem.Click += zapiszGrêToolStripMenuItem_Click;
            menuStrip1.Items.Add(zapiszGrêToolStripMenuItem);


            menuStrip1.Items.Add(nowaGraToolStripMenuItem);
            this.MainMenuStrip = menuStrip1;
            this.Controls.Add(menuStrip1);

      
            flowLayoutPanel1 = new FlowLayoutPanel
            {
                Location = new Point(10, 30),
                Width = 300,
                Height = 300,
                AutoScroll = true
            };
            this.Controls.Add(flowLayoutPanel1);

          
            label1 = new Label
            {
                Location = new Point(10, 340),
                Width = 150,
                Text = "Gracz 1: 0"
            };
            label2 = new Label
            {
                Location = new Point(200, 340),
                Width = 150,
                Text = "Gracz 2: 0"
            };
            this.Controls.Add(label1);
            this.Controls.Add(label2);
        }

        private void InitializeGame()
        {
            player1 = new Player("Gracz 1");
            player2 = new Player("Gracz 2");
            aktualnyGracz = player1;
            board = new GameBoard(4);
            board.ZakonczonyRzad += ZdobytePunktyWRzedzie;

            DrawBoard();
        }

        private void DrawBoard()
        {
            flowLayoutPanel1.Controls.Clear();
            var ukonczoneRzedy = board.GetCompletedRows();
            var podswietloneZiemniaki = ukonczoneRzedy.SelectMany(row => row).ToHashSet();


            var grouped = board.Potatoes
                .GroupBy(p => p.X + p.Y) 
                .OrderBy(g => g.Key)
                .ToList();

            int maxRow = grouped.Max(g => g.Key);

            foreach (var group in grouped)
            {
                var rowPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true,
                    Margin = new Padding((maxRow - group.Key) * 25, 5, 0, 5), 
                };

                foreach (var potato in group.OrderBy(p => p.X))
                {
                    var btn = new Button
                    {
                        Width = 40,
                        Height = 40,
                        Text = potato.IsMarked ? "X" : "",
                        Tag = potato,
                        BackColor = podswietloneZiemniaki.Contains(potato) ? Color.Pink : SystemColors.Control
                    };
                    btn.Click += Potato_Click;
                    rowPanel.Controls.Add(btn);
                }

                flowLayoutPanel1.Controls.Add(rowPanel);
            }

            AktualizacjaWynikow();
        }


        private void Potato_Click(object? wysylajacy, EventArgs e)
        {
            var btn = wysylajacy as Button;
            var potato = btn?.Tag as Potato;
            if (potato == null) return;

            try
            {
                board.OznaczenieZiemniaka(potato.X, potato.Y, potato.Z, aktualnyGracz);
                ZamianaGracza();
                DrawBoard();
                SprawdzKoniecGry();
            }
            catch (WyjatekNieprawidlowyRuch ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void ZdobytePunktyWRzedzie(Player player, int wynik)
        {
            MessageBox.Show($"{player.Name} zdoby³ {wynik} punktów!");
        }

        private void ZamianaGracza()
        {
            aktualnyGracz = aktualnyGracz == player1 ? player2 : player1;
        }

        private void AktualizacjaWynikow()
        {
            label1.Text = $"{player1.Name}: {player1.Wynik}";
            label2.Text = $"{player2.Name}: {player2.Wynik}";

            if (aktualnyGracz == player1)
            {
                label1.Font = new Font(label1.Font, FontStyle.Bold);
                label1.ForeColor = Color.Plum;

                label2.Font = new Font(label2.Font, FontStyle.Regular);
                label2.ForeColor = Color.Black;
            }
            else
            {
                label2.Font = new Font(label2.Font, FontStyle.Bold);
                label2.ForeColor = Color.Plum;

                label1.Font = new Font(label1.Font, FontStyle.Regular);
                label1.ForeColor = Color.Black;
            }
        }


        private void SprawdzKoniecGry()
        {
           
            if (board.Potatoes.All(p => p.IsMarked))
            {
                string message;

                if (player1.Wynik > player2.Wynik)
                    message = $"Koniec gry! Wygra³ {player1.Name} z wynikiem {player1.Wynik} punktów.";
                else if (player2.Wynik > player1.Wynik)
                    message = $"Koniec gry! Wygra³ {player2.Name} z wynikiem {player2.Wynik} punktów.";
                else
                    message = "Koniec gry! Remis.";

                MessageBox.Show(message, "Gra zakoñczona", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void ZapiszGre()
        {
            var gameState = new GameState
            {
                Players = new List<DaneGracza>
        {
            new DaneGracza { Name = player1.Name, Wynik = player1.Wynik },
            new DaneGracza { Name = player2.Name, Wynik = player2.Wynik }
        }
            };

            string sciezkaFolderu = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZapisaneWyniki");
            Directory.CreateDirectory(sciezkaFolderu);

            string sciezkaPliku = Path.Combine(sciezkaFolderu, $"Wynik_{DateTime.Now:yyyyMMdd_HHmmss}.json");

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(sciezkaPliku, JsonSerializer.Serialize(gameState, options));

            MessageBox.Show("Gra zosta³a zapisana!", "Zapisano", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void zapiszGrêToolStripMenuItem_Click(object wysylajacy, EventArgs e)
        {
            ZapiszGre();
        }




        private void nowaGraToolStripMenuItem_Click(object wysylajacy, EventArgs e)
        {
            InitializeGame();
        }

        
    }
}
