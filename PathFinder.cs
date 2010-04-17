using System;
using System.Collections.Generic;
using AllegroSharp;
using System.Linq;

namespace SomePathingThing
{
    public class PathFinder
    {
        public IEnumerable<IEnumerable<float>> SelectRight(IEnumerable<IEnumerable<float>> map)
        {
            var target = map.First().First();
            var width = map.Min(row => row.TakeWhile(node => node == target).Count());
            return map.Select(row => row.Take(width));
        }

        public IEnumerable<IEnumerable<float>> SelectDown(IEnumerable<IEnumerable<float>> map)
        {
            var target = map.First().First();
            return map.TakeWhile(row => row.First() == target);
        }

        public IEnumerable<IEnumerable<float>> SelectLeft(IEnumerable<IEnumerable<float>> map)
        {
            return null;
        }

        public IEnumerable<IEnumerable<float>> SelectRightThenDown(float[][] map, int x, int y)
        {
            var target = map[y][x];
            var width = map[y].Skip(x).TakeWhile(node => node == target).Count();
            return map.AsEnumerable().Skip(y).Select(row => row.Skip(x).Take(width)).TakeWhile(row => row.All(node => node == target));
        }

        public IEnumerable<IEnumerable<float>> SelectDownThenRight(float[][] map, int x, int y)
        {
            var target = map[y][x];
            var space = from row in map.Skip(y).TakeWhile(row => row[x] == target)
                select row.Skip(x);
            var width = space.Min(row => row.TakeWhile(node => node == target).Count());
            return space.Select(row => row.Take(width));
        }

        public IEnumerable<Rectangle> MakeZones(float[][] map)
        {
            var zones = new List<Rectangle>();
            var height = map.Length;
            var width = map.Max(arr => arr.Length);
            var inGroup = new float[height, width];
            var topLeftArea = SelectRightThenDown(map, 0, 0);
            var topLeft = new Point2(0, 0);
            var topRight = new Point2(width, 0);
            var bottomLeft = new Point2(0, height);
            var bottomRight = new Point2(width, height);
            return null;
        }

        public static float[][] MakeMap(Bitmap bitmap)
        {
            return (from y in Enumerable.Range(0, bitmap.Height)
                select (from x in Enumerable.Range(0, bitmap.Width)
                    let pixel = bitmap.GetPixel(x, y)
                    select new float[] { pixel.Red, pixel.Green, pixel.Blue }.Average()).ToArray()).ToArray();
        }
    }
}
