namespace Wacton.Colonies.Ancillary
{
    public struct Coordinate
    {
        public readonly int X;
        public readonly int Y;

        public Coordinate(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", this.X, this.Y);
        }
    }
}
