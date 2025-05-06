namespace GraWZiemniaka
{
    public class Potato
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public bool IsMarked { get; set; }

        public Potato(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
            IsMarked = false;
        }
    }
}
