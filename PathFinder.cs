using System;
using System.Collections.Generic;
using AllegroSharp;
using System.Linq;

namespace SomePathingThing
{
    public class Node
    {
        public Node(Point2 point, float value)
        {
            this.Point = point;
            this.Value = value;
        }

        public readonly Point2 Point;
        public readonly float Value;
        public bool InGroup = false;
    }

    public class PathFinder
    {
        public static Node[,] MakeMap(Bitmap bitmap)
        {
            using (var @lock = bitmap.Lock(PixelFormat.Any, LockFlags.ReadOnly))
            {
                var map = new Node[bitmap.Height, bitmap.Width];
                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        var value = (pixel.Red + pixel.Green + pixel.Blue) / 3f;
                        map[y, x] = new Node(new Point2(x, y), value);
                    }
                }
                return map;
            }
        }
    }
}
