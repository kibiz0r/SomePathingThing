using System;
using AllegroSharp;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace SomePathingThing
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Allegro.RunMain(AllegroMain);
        }

        public static IEnumerable<Rectangle> TopLeftCandidate(float[][] map, int targetX, int targetY)
        {
            var target = map[targetY][targetX];
            var startX = targetX;
            var startY = targetY;
            var endX = startX;
            var endY = startY;
            for (; endX < map[startY].Length; endX++)
            {
                if (map[startY][endX] != target)
                {
                    break;
                }
                yield return new Rectangle(startX, startY, endX, endY);
            }
            for (; endY < map.Length; endY++)
            {
                var done = false;
                for (var x = 0; x < map[endY].Length; x++)
                {
                    if (map[endY][x] != target)
                    {
                        done = true;
                        break;
                    }
                }
                if (done)
                {
                    break;
                }
                yield return new Rectangle(startX, startY, endX, endY);
            }
            yield return new Rectangle(startX, startY, endX, endY);
        }

        public static IEnumerable<Rectangle> TopRightCandidate(float[][] map, int targetX, int targetY)
        {
            var target = map[targetY][targetX];
            var startX = targetX;
            var startY = targetY;
            var endX = startX;
            var endY = startY;
            for (; endY < map.Length; endY++)
            {
                if (map[endY][startX] != target)
                {
                    break;
                }
                yield return new Rectangle(startX, startY, endX, endY);
            }
            for (; startX >= 0; startX--)
            {
                var done = false;
                for (var y = startY; y < endY; y++)
                {
                    if (map[y][startX] != target)
                    {
                        done = true;
                        break;
                    }
                }
                if (done)
                {
                    break;
                }
                yield return new Rectangle(startX, startY, endX, endY);
            }
            yield return new Rectangle(startX, startY, endX, endY);
        }

        public static IEnumerable<Rectangle> BottomRightCandidate(float[][] map, int targetX, int targetY)
        {
            var target = map[targetY][targetX];
            var startX = targetX;
            var startY = targetY;
            var endX = startX;
            var endY = startY;
            for (; startX >= 0; startX--)
            {
                if (map[endY][startX] != target)
                {
                    break;
                }
                yield return new Rectangle(startX, startY, endX, endY);
            }
            startX++;
            for (; startY > 0; startY--)
            {
                var done = false;
                for (var x = endX; x >= startX; x--)
                {
                    float[] row = map[startY];
                    if (row[x] != target)
                    {
                        done = true;
                        break;
                    }
                }
                if (done)
                {
                    break;
                }
                yield return new Rectangle(startX, startY, endX, endY);
            }
            yield return new Rectangle(startX, startY, endX, endY);
        }

        public static void AllegroMain()
        {
            Allegro.InstallSystem();
            Keyboard.Install();
            Image.Init();
            Font.Init();
            Ttf.Init();
            var font = Ttf.LoadFont("Arial.ttf", 12, TtfFlags.None);
            Action<string> drawStatus = delegate(string status)
            {
                font.Draw(Display.Width / 2, Display.Height - 50, FontDrawFlags.AlignCentre, status);
            };
            var yellow = new Color(1, 1, 0);
            var blue = new Color(0, 0, 1);
            var green = new Color(0, 1, 0);
            var black = new Color(0, 0, 0);
            var topLeft = new Rectangle(0, 0, 0, 0);
            var topRight = new Rectangle(0, 0, 0, 0);
            var bottomRight = new Rectangle(0, 0, 0, 0);
            Action drawRectangles = delegate()
            {
                Primitives.DrawFilledRectangle(topLeft.X1, topLeft.Y1, topLeft.X2, topLeft.Y2, yellow);
                Primitives.DrawFilledRectangle(topRight.X1, topRight.Y1, topRight.X2, topRight.Y2, blue);
                Primitives.DrawFilledRectangle(bottomRight.X1, bottomRight.Y1, bottomRight.X2, bottomRight.Y2, green);
            };
            using (var display = Display.Create(800, 600))
            {
                var bitmap = Image.LoadBitmap("map.png");
                Blender.Set(BlendOperation.Add, BlendMode.Alpha, BlendMode.InverseAlpha, new Color(1, 1, 1));
                var map = PathFinder.MakeMap(bitmap);
                var query = from y in Enumerable.Range(0, map.Length)
                    from x in Enumerable.Range(0, map[y].Length)
                        let target = map[y][x]
                        let width = map[y].Skip(x).TakeWhile(node => node == target).Count()
                        let height = map.Skip(y).TakeWhile(row => row.Skip(x).Take(width).All(node => node == target)).Count()
                        select new Rectangle(x, y, x + width, y + height);
                var random = new Random();
                foreach (var rectangle in query)
                {
                    var color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                    Primitives.DrawFilledRectangle(rectangle.X1, rectangle.Y1, rectangle.X2, rectangle.Y2, color);
                }
                foreach (var candidate in TopLeftCandidate(map, 0, 0))
                {
                    topLeft = new Rectangle(candidate.X1, candidate.Y1, candidate.X2, candidate.Y2 + 1);
                    Display.Clear(black);
                    bitmap.Draw(0, 0, DrawFlags.None);
                    drawRectangles();
                    drawStatus("Searching top left...");
                    Display.Flip();
                    Thread.Sleep(20);
                }
                foreach (var candidate in TopRightCandidate(map, map[0].Length - 1, 0))
                {
                    topRight = new Rectangle(candidate.X1, candidate.Y1, candidate.X2 + 1, candidate.Y2);
                    Display.Clear(black);
                    bitmap.Draw(0, 0, DrawFlags.None);
                    drawRectangles();
                    drawStatus("Searching top right...");
                    Display.Flip();
                    Thread.Sleep(20);
                }
                foreach (var candidate in BottomRightCandidate(map, map[0].Length - 1, map.Length - 1))
                {
                    bottomRight = new Rectangle(candidate.X1, candidate.Y1, candidate.X2, candidate.Y2 + 1);
                    Display.Clear(black);
                    bitmap.Draw(0, 0, DrawFlags.None);
                    drawRectangles();
                    drawStatus("Searching bottom right...");
                    Display.Flip();
                    Thread.Sleep(20);
                }
                Display.Clear(black);
                bitmap.Draw(0, 0, DrawFlags.None);
                drawRectangles();
                drawStatus("Done");
                Display.Flip();
                KeyboardState keyboardState = new KeyboardState();
                while (!keyboardState.KeyDown(Key.Escape))
                {
                    Keyboard.GetState(ref keyboardState);
                }
            }
        }
    }
}
