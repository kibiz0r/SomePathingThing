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

        public static void AllegroMain()
        {
            Allegro.InstallSystem();
            Keyboard.Install();
            Image.Init();
            Font.Init();
            Ttf.Init();
            var font = Ttf.LoadFont("Arial.ttf", 12, TtfFlags.None);
            var black = new Color(0, 0, 0);
            var gray = new Color(0.5f, 0.5f, 0.5f);
            var yellow = new Color(1, 1, 0);
            var red = new Color(1, 0, 0);
            var random = new Random();
            var visualization = true;
            var keyboardState = new KeyboardState();
            using (var display = Display.Create(800, 600))
            {
                var bitmap = Image.LoadBitmap("map.png");
                Blender.Set(BlendOperation.Add, BlendMode.Alpha, BlendMode.InverseAlpha, new Color(1, 1, 1));
                Display.Clear(black);
                bitmap.Draw(0, 0, DrawFlags.None);
                font.Draw(Display.Width / 2, Display.Height - 50, FontDrawFlags.AlignCentre, "Do you want visualization? (Y/N)");
                Display.Flip();
                keyboardState = new KeyboardState();
                while (!(keyboardState.KeyDown(Key.Y) || keyboardState.KeyDown(Key.N)))
                {
                    Keyboard.GetState(ref keyboardState);
                }
                visualization = keyboardState.KeyDown(Key.Y);
                var map = PathFinder.MakeMap(bitmap);
                var height = map.GetLength(0);
                var width = map.GetLength(1);
                var zones = new List<Rectangle>();
                while (true)
                {
                    Rectangle? biggestRectangle = null;
                    for (var startY = 0; startY < height; startY++)
                    {
                        for (var startX = 0; startX < width; startX++)
                        {
                            var startNode = map[startY, startX];
                            if (startNode.InGroup)
                            {
                                continue;
                            }
                            var value = startNode.Value;
                            var endX = startX;
                            for (; endX < width; endX++)
                            {
                                var node = map[startY, endX];
                                if (node.InGroup || value != node.Value)
                                {
                                    break;
                                }
                            }
                            if (biggestRectangle.HasValue)
                            {
                                var necessaryHeight = (int)(biggestRectangle.Value.GetArea() / ((float)endX - startX));
                                var jumpY = startY + necessaryHeight;
                                if (jumpY >= height || map[jumpY, startX].InGroup || value != map[jumpY, startX].Value)
                                {
                                    continue;
                                }
                            }
                            var endY = startY;
                            for (; endY < height; endY++)
                            {
                                for (var x = startX; x < endX; x++)
                                {
                                    var node = map[endY, x];
                                    if (node.InGroup || value != node.Value)
                                    {
                                        goto foundY;
                                    }
                                }
                            }
                        foundY:
                                if (visualization)
                            {
                                Keyboard.GetState(ref keyboardState);
                                if (keyboardState.KeyDown(Key.Escape))
                                {
                                    return;
                                }
                                Display.Clear(black);
                                bitmap.Draw(0, 0, DrawFlags.None);
                                foreach (var zone in zones)
                                {
                                    Primitives.DrawFilledRectangle(zone.X1, zone.Y1, zone.X2, zone.Y2, gray);
                                }
                                if (biggestRectangle.HasValue)
                                {
                                    Primitives.DrawFilledRectangle(biggestRectangle.Value.X1,
                                                                   biggestRectangle.Value.Y1,
                                                                   biggestRectangle.Value.X2,
                                                                   biggestRectangle.Value.Y2,
                                                                   yellow);
                                }
                                Primitives.DrawFilledRectangle(startX, startY, endX, endY, red);
                                Display.Flip();
                            }
                            var rectangle = new Rectangle(startX, startY, endX, endY);
                            if (!biggestRectangle.HasValue || rectangle.GetArea() > biggestRectangle.Value.GetArea())
                            {
                                biggestRectangle = rectangle;
                            }
                        }
                    }
                    if (!biggestRectangle.HasValue)
                    {
                        break;
                    }
                    for (var y = biggestRectangle.Value.Y1; y < biggestRectangle.Value.Y2; y++)
                    {
                        for (var x = biggestRectangle.Value.X1; x < biggestRectangle.Value.X2; x++)
                        {
                            map[y, x].InGroup = true;
                        }
                    }
                    zones.Add(biggestRectangle.Value);
                }
                Display.Clear(black);
                bitmap.Draw(0, 0, DrawFlags.None);
                foreach (var zone in zones)
                {
                    var color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                    Primitives.DrawFilledRectangle(zone.X1, zone.Y1, zone.X2, zone.Y2, color);
                }
                font.Draw(Display.Width / 2, Display.Height - 50, FontDrawFlags.AlignCentre, zones.Count.ToString());
                Display.Flip();
                keyboardState = new KeyboardState();
                while (!(keyboardState.KeyDown(Key.Escape) || keyboardState.KeyDown(Key.Enter)))
                {
                    Keyboard.GetState(ref keyboardState);
                }
                if (keyboardState.KeyDown(Key.Escape))
                {
                    return;
                }
                var pathNodes = new List<PathNode>();
                foreach (var zone in zones)
                {
                    var nodes = new PathNode[]
                    {
                        new PathNode(zone.X1, zone.Y1),
                        new PathNode(zone.X1, zone.Y2),
                        new PathNode(zone.X2, zone.Y1),
                        new PathNode(zone.X2, zone.Y2)
                    };
                    nodes[0].Neighbors.AddRange(new PathNode[] { nodes[1], nodes[2], nodes[3] });
                    nodes[1].Neighbors.AddRange(new PathNode[] { nodes[0], nodes[2], nodes[3] });
                    nodes[2].Neighbors.AddRange(new PathNode[] { nodes[0], nodes[1], nodes[3] });
                    nodes[3].Neighbors.AddRange(new PathNode[] { nodes[0], nodes[1], nodes[2] });
                    pathNodes.AddRange(nodes);
                }
                var foundDuplicates = true;
                while (foundDuplicates)
                {
                    foundDuplicates = false;
                    for (var i = 0; i < pathNodes.Count; i++)
                    {
                        var node1 = pathNodes[i];
                        for (var j = 0; j < pathNodes.Count;)
                        {
                            var node2 = pathNodes[j];
                            if (!object.ReferenceEquals(node1, node2) && node1.X == node2.X && node1.Y == node2.Y)
                            {
                                node1.Neighbors.AddRange(node2.Neighbors);
                                foreach (var neighbor in node2.Neighbors)
                                {
                                    for (var k = 0; k < neighbor.Neighbors.Count; k++)
                                    {
                                        if (object.ReferenceEquals(neighbor.Neighbors[k], node2))
                                        {
                                            neighbor.Neighbors[k] = node1;
                                        }
                                    }
                                }
                                pathNodes.RemoveAt(j);
                                foundDuplicates = true;
                            }
                            else
                            {
                                j++;
                            }
                        }
                    }
                }
                Display.Clear(black);
                bitmap.Draw(0, 0, DrawFlags.None);
                foreach (var node in pathNodes)
                {
                    foreach (var neighbor in node.Neighbors)
                    {
                        Primitives.DrawLine(node.X, node.Y, neighbor.X, neighbor.Y, red, 0);
                    }
                }
                font.Draw(Display.Width / 2, Display.Height - 50, FontDrawFlags.AlignCentre, pathNodes.Count.ToString());
                Display.Flip();
                keyboardState = new KeyboardState();
                while (!keyboardState.KeyDown(Key.Escape))
                {
                    Keyboard.GetState(ref keyboardState);
                }
            }
        }
    }
}
