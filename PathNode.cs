using System;
using System.Collections.Generic;

namespace SomePathingThing
{
    public class PathNode
    {
        public readonly int X;
        public readonly int Y;
        public List<PathNode> Neighbors = new List<PathNode>();

        public PathNode(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
