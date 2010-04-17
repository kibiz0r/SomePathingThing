using System;

namespace SomePathingThing
{
    public class MapCursor
    {
        public Point2 Position { get; set; }
        public Point2 PrimaryDirection { get; set; }
        public Point2 SecondaryDirection { get; set; }

        /*public Rectangle MoveNext(Map map)
        {
            var start = Position;
            var target = map.Nodes[start.Y, start.X];
            while (target.InGroup)
            {
                start = start + PrimaryDirection;
                target = map.Nodes[start.Y, start.X];
            }
        }*/
    }
}
