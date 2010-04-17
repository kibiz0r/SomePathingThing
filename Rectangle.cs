using System;

namespace SomePathingThing
{
    public struct Rectangle
    {
        public Rectangle(int x1, int y1, int x2, int y2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }

        public int GetArea()
        {
            return Math.Abs(X2 - X1) * Math.Abs(Y2 - Y1);
        }

        public readonly int X1;
        public readonly int Y1;
        public readonly int X2;
        public readonly int Y2;
    }
}
