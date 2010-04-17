using System;

namespace SomePathingThing
{
    public class Map
    {
        public MapNode[,] Nodes { get; set; }

        public Map(float[][] values)
        {
            var height = values.Length;
            var width = values[0].Length;
            Nodes = new MapNode[height, width];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Nodes[y, x] = new MapNode { Value = values[y][x] };
                }
            }
        }
    }
}
