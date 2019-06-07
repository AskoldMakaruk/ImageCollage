using System;
using System.Drawing;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace stickerlib
{
    public class Meme : IDisposable
    {
        public Image<Rgba32> Bitmap;

        private double _height;
        private double _width;

        public double Height
        {
            get { return _height; }
            set
            {
                double ratio = value / _height;

                _width = _width * ratio;
                _height = _height * ratio;
            }
        }
        public double Width
        {
            get { return _width; }
            set
            {
                double ratio = value / _width;

                _width = _width * ratio;
                _height = _height * ratio;
            }
        }

        public int Space => (int) (Height * Width);

        public static double A4Price = 12;
        public static double RevenueRatio = 4;
        public double Price => A4Price * Space / MemeSheet.Space * RevenueRatio * 10;

        public int AccountId { get; set; }
        public int Id { get; set; }
        public byte[] Image { get => ToByteArr (); set { Bitmap = ToBitmap (value); } }

        public Meme () { }
        public Meme (Stream stream, int accountId)
        {
            AccountId = accountId;
            Bitmap = ToBitmap (stream);
            _height = Bitmap.Height;
            _width = Bitmap.Width;
        }

        private MemoryStream ToStream (Image<Rgba32> data)
        {
            MemoryStream memoryStream = new MemoryStream ();
            data.SaveAsPng (memoryStream);
            memoryStream.Seek (0, SeekOrigin.Begin);
            return memoryStream;
        }
        public Image<Rgba32> ToBitmap (Stream content)
        {
            content.Seek (0, SeekOrigin.Begin);
            var bm = SixLabors.ImageSharp.Image.Load (content);

            bm.Mutate (b => b.Resize ((int) Width == 0 ?
                bm.Width : (int) Width, (int) Height == 0 ?
                bm.Height : (int) Height));

            return bm;
        }
        public Image<Rgba32> ToBitmap (byte[] data)
        {
            MemoryStream stream = new MemoryStream (data);
            return ToBitmap (stream);
        }
        public byte[] ToByteArr ()
        {
            var stream = ToStream (Bitmap);
            var res = stream.ToArray ();
            stream.Close ();
            return res;
        }

        public void Dispose ()
        {
            Bitmap?.Dispose ();
        }
    }
}