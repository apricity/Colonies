namespace SDLTest
{
    using System;
    using System.Drawing;

    using SdlDotNet.Core;
    using SdlDotNet.Graphics;
    using SdlDotNet.Graphics.Primitives;

    public static class Program
    {
        private const int GridSize = 20;
        private const int Width = 50;
        private const int Height = 50;
        private static Surface screen;
        private static readonly Random Random = new Random();

        public static void Main(string[] args)
        {
            screen = Video.SetVideoMode(Width * GridSize, Height * GridSize, 32, false, false, false, true);
            Video.WindowCaption = "Checkered";

            var rectangles = new Rectangle[Width,Height];
            for (var x = 0; x < Width; x++)
            {
                var flag = x % 2 == 0;

                for (var y = 0; y < Height; y++)
                {
                    rectangles[x, y] = new Rectangle(new Point(x * GridSize, y * GridSize), new Size(GridSize, GridSize));

                    Color color;
                    if (flag)
                    {
                        color = y % 2 == 0 ? Color.Black : Color.White;
                    }
                    else
                    {
                        color = y % 2 == 0 ? Color.White : Color.Black;
                        
                    }

                    screen.Fill(rectangles[x, y], color);
                }
            }

            screen.Update();

            Events.TargetFps = 50;
            Events.Quit += QuitEventHandler;
            Events.Tick += TickEventHandler;

            Events.Run();
        }

        private static void TickEventHandler(object sender, TickEventArgs e)
        {
            //for (var i = 0; i < 17; i++)
            //{
            //    var rect = new Rectangle(new Point(Random.Next(Width - 100), Random.Next(Height - 100)),
            //                new Size(10 + Random.Next(Width - 90), 10 + Random.Next(Height - 90)));
            //    var Col = Color.FromArgb(Random.Next(255), Random.Next(255), Random.Next(255));
            //    var CircCol = Color.FromArgb(Random.Next(255), Random.Next(255), Random.Next(255));
            //    short radius = (short)(10 + Random.Next(Height - 90));
            //    var Circ = new Circle(new Point(Random.Next(Width - 100), Random.Next(Height - 100)), radius);
            //    screen.Fill(rect, Col);
            //    Circ.Draw(screen, CircCol, false, true);
            //    screen.Update();
            //    Video.WindowCaption = Events.Fps.ToString();
            //}
        }

        private static void QuitEventHandler(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }
    }
}
