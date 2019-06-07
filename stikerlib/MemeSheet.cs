using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Linq;
namespace stickerlib
{
    public class MemeSheet
    {
        public static int Width = 3508, Height = 2480;
        public static int Space => Width * Height;
        public int HeightLeft=> Height - (int)Content.Sum(c => c.Height);
        public List<Row> Content = new List<Row> ();

        public double FreeSpace
        {
            get
            {
                double i = 0;
                foreach (Row c in Content)
                    foreach (var m in c.Memes)
                        i += m.Height * m.Width;
                return (Space - i) / Space * 100;
            }
        }
        public Image<Rgba32> ToBitmap ()
        {
            Console.WriteLine ($"Starting convertation with {FreeSpace}% free space...");
            Image<Rgba32> result = new Image<Rgba32> (Width, Height);

            float x = 0, y = 0;
            foreach (var row in Content)
            {
                foreach (var meme in row.Memes)
                {
                    var b = meme.ToBitmap (meme.Image);
                    result.Mutate (m => m.DrawImage (b, new Point ((int) x, (int) y), 1));
                    x += (float) meme.Width;
                    b.Dispose ();
                    b = null;
                }
                //test
                y += (int)row.Height;
                x = 0;
            }

            Console.WriteLine ($"Convertation finised...");
            return result;
        }
    }
}