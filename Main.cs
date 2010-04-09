using System;
using AllegroSharp;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;

namespace SomePathingThing
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Allegro.RunMain(AllegroMain);
        }

        public static void AllegroMain()
        {
            Allegro.InstallSystem();
            Keyboard.Install();
            Image.Init();
            using (var display = Display.Create(800, 600))
            {
                var bitmap = Image.LoadBitmap("map.png");
                var width = bitmap.Width;
                var height = bitmap.Height;
                var inGroup = new bool[height, width];
                var groups = new List<List<Point2>>();
                var extremityGroups = new List<List<Point2>>();
                for (var startY = 0; startY < height; startY++)
                {
                    for (var startX = 0; startX < width; startX++)
                    {
                        if (inGroup[startY, startX])
                        {
                            continue;
                        }
                        Color desired = bitmap.GetPixel(startX, startY);
                        var group = new List<Point2>();
                        var extremities = new List<Point2>();
                        var stack = new Stack<Point2>();
                        stack.Push(new Point2(startX, startY));
                        while (stack.Count > 0)
                        {
                            var pixel = stack.Pop();
                            var x = pixel.X;
                            var y = pixel.Y;
                            if (inGroup[y, x] || bitmap.GetPixel(x, y) != desired)
                            {
                                continue;
                            }
                            group.Add(new Point2(x, y));
                            inGroup[y, x] = true;
                            bool pure = true;
                            if (y - 1 >= 0)
                            {
                                stack.Push(new Point2(x, y - 1));
                                if (bitmap.GetPixel(x, y - 1) != desired)
                                {
                                    pure = false;
                                }
                            }
                            if (x - 1 >= 0)
                            {
                                stack.Push(new Point2(x - 1, y));
                                if (bitmap.GetPixel(x - 1, y) != desired)
                                {
                                    pure = false;
                                }
                            }
                            if (y + 1 < height)
                            {
                                stack.Push(new Point2(x, y + 1));
                                if (bitmap.GetPixel(x, y + 1) != desired)
                                {
                                    pure = false;
                                }
                            }
                            if (x + 1 < width)
                            {
                                stack.Push(new Point2(x + 1, y));
                                if (bitmap.GetPixel(x + 1, y) != desired)
                                {
                                    pure = false;
                                }
                            }
                            if (!pure)
                            {
                                extremities.Add(new Point2(x, y));
                            }
                        }
                        groups.Add(group);
                        extremityGroups.Add(extremities);
                    }
                }
                Console.WriteLine("{0} groups", groups.Count);
                Console.WriteLine("{0} extremety groups", extremityGroups.Count);
                Color[] colors = new Color[]
                {
                    new Color(1, 0, 0),
                    new Color(0, 1, 0)
                };
                while (true)
                {
                    KeyboardState keyboardState = new KeyboardState();
                    Keyboard.GetState(ref keyboardState);
                    if (keyboardState.KeyDown(Key.Escape))
                    {
                        break;
                    }
                    bitmap.Draw(0, 0, DrawFlags.None);
                    for (var i = 0; i < extremityGroups.Count; i++)
                    {
                        var extremities = extremityGroups[i];
                        for (var j = 0; j + 1 < extremities.Count; j++)
                        {
                            Primitives.DrawLine(extremities[j].X, extremities[j].Y, extremities[j + 1].X, extremities[j + 1].Y, colors[i], 0);
                        }
                    }
                    Display.Flip();
                }
            }
        }
    }
}
