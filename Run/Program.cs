using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp.Formats.Png;
using stickerlib;
namespace Run
{
    class Program
    {
        static void Main (string[] args)
            {
                var memes = MemeMachine.FromPath (@"C:\Users\askol\OneDrive\Desktop\pack\").ToList ();
            //var columns = MemeMachine.ComposeRow (memes).ToList ();
            
            int i =0;
            foreach (var meme in MemeMachine.ComposeMemes (memes))
            {
                meme.ToBitmap ().Save (new FileStream ($@"C:\Users\askol\OneDrive\Desktop\meme { i }.png", FileMode.Create), new PngEncoder ());
                i++;
            }
        }
    }
}