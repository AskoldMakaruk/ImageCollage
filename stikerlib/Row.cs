using System.Collections.Generic;
using System.Linq;

namespace stickerlib
{
    public class Row
    {
        public List<Meme> Memes = new List<Meme> ();
        public int Count => Memes.Count;

        public double Width => Memes.Sum (m => m.Width);
        public double Height { get; set; }

        public Meme this [int num]
        {
            get => Memes[num];
            set => Memes[num] = value;
        }
        public void Add (Meme m)
        {
            if (Memes.Count == 0)
            {
                m.Width = MemeSheet.Width;
                Height = m.Height;
                Memes.Add (m);
            }
            else
            {
                m.Height = Height;
                Memes.Add (m);
                Resize ();
            }
        }
        public void Resize ()
        {
            double ratio = MemeSheet.Width / Width;

            Height = Height * ratio;
            foreach (Meme m in Memes) m.Height = Height;
        }
    }
}