using System;

namespace SomePathingThing
{
    public struct Cursor
    {
        public Cursor(Point2 position, Rectangle zone)
        {
            this.Position = position;
            this.Zone = zone;
        }

        public readonly Point2 Position;
        public readonly Rectangle Zone;

        public static Cursor? TopLeft(Cursor cursor, Map map)
        {
            var startX = cursor.Position.X;
            var startY = cursor.Position.Y;
            var target = map.Nodes[startY, startX];
            var height = map.Nodes.GetLength(0);
            var width = map.Nodes.GetLength(1);
            while (target.InGroup)
            {
                start = start + new Point2(1, 0);
                if (startX >= width)
                {
                    start = new Point2(0, startY + 1);
                }
                if (startY >= height)
                {
                    return null;
                }
                target = map.Nodes[startY, startX];
            }
            var endX = startX;
            for (; endX < width; endX++)
            {
                if (target.Value != map.Nodes[startY, endX].Value)
                {
                    break;
                }
            }
            var endY = startY;
            for (; endY < height; endY++)
            {
                for (var x = 0; x < endX; x++)
                {
                    if (target.Value != map.Nodes[endY, x].Value)
                    {
                        goto foundY;
                    }
                }
            }
        foundY:
                return new Cursor(new Point2(endX, endY), new Rectangle(startX, startY, endX, endY));
        }

        public static Cursor? TopRight(Cursor cursor, Map map)
        {
            var endX = cursor.Position.X;
            var startY = cursor.Position.Y;
            var target = map.Nodes[startY, startX];
            var height = map.Nodes.GetLength(0);
            var width = map.Nodes.GetLength(1);
            while (target.InGroup)
            {
                start = start + new Point2(0, 1);
                if (startX >= height)
                {
                    start = new Point2(startX - 1, 0);
                }
                if (startX < 0)
                {
                    return null;
                }
                target = map.Nodes[startY, startX];
            }
            var endY = startY;
            for (; endY < height; endY++)
            {
                if (target.Value != map.Nodes[endY, endX].Value)
                {
                    break;
                }
            }
            var startX = endX;
            for (; startX >= 0; startX--)
            {
                for (var y = startY; y < endY; y++)
                {
                    if (target.Value != map.Nodes[y, startX].Value)
                    {
                        goto foundX;
                    }
                }
            }
        foundX:
                startX++;
            return new Cursor(new Point2(startX, endY), new Rectangle(startX, startY, endX, endY));
        }
    }
}
