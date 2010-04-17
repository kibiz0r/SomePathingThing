using System;

namespace SomePathingThing
{
    public struct Vector2
    {
        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public readonly float X;
        public readonly float Y;

        public static readonly Vector2 Zero = new Vector2();

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator /(Vector2 v, float f)
        {
            return new Vector2(v.X / f, v.Y / f);
        }
    }
}
