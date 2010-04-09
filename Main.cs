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
                var groups = new List<Rectangle>();
                for (var startY = 0; startY < height; startY++)
                {
                    for (var startX = 0; startX < width; startX++)
                    {
                        if (inGroup[startY, startX])
                        {
                            continue;
                        }
                        inGroup[startY, startX] = true;
                        Color desired = bitmap.GetPixel(startX, startY);
                        var endX = startX;
                        while (true)
                        {
                            if (!(endX + 1 < width))
                            {
                                break;
                            }
                            if (bitmap.GetPixel(endX + 1, startY) != desired)
                            {
                                break;
                            }
                            if (inGroup[startY, endX + 1])
                            {
                                break;
                            }
                            inGroup[startY, endX + 1] = true;
                            endX++;
                        }
                        var endY = startY;
                        for ( ; endY + 1 < height; endY++)
                        {
                            for (var x = startX; x <= endX; x++)
                            {
                                if (bitmap.GetPixel(x, endY + 1) != desired)
                                {
                                    goto foundY;
                                }
                                if (inGroup[endY + 1, x])
                                {
                                    goto foundY;
                                }
                            }
                            // Have to do this after the initial loop since we only want lines where the whole thing matches.
                            for (var x = startX; x <= endX; x++)
                            {
                                inGroup[endY + 1, x] = true;
                            }
                        }
                    foundY:
                        var rectangle = new Rectangle(startX, startY, endX, endY);
                        groups.Add(rectangle);
                    }
                }
                Console.WriteLine("{0} groups", groups.Count);
                var colors = (from g in groups
                    select new Color((float)g.X2 / width, (float)g.Y2 / height, ((float)g.X1 + g.Y1) / (width + height))).ToArray();
                while (true)
                {
                    KeyboardState keyboardState = new KeyboardState();
                    Keyboard.GetState(ref keyboardState);
                    if (keyboardState.KeyDown(Key.Escape))
                    {
                        break;
                    }
                    bitmap.Draw(0, 0, DrawFlags.None);
                    for (var i = 0; i < groups.Count; i++)
                    {
                        var group = groups[i];
                        Primitives.DrawRectangle(group.X1, group.Y1, group.X2, group.Y2, colors[i], 0);
                    }
                    Display.Flip();
                }
            }
        }
    }
}
