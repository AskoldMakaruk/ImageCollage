using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace stickerlib
{
    public static class MemeMachine
    {
        public static IEnumerable<MemeSheet> FromPath (string path)
        {
            foreach (var sheet in ComposeMemes (MemeMachine.LoadMemes (path))) yield return sheet;
        }

        internal static List<Meme> LoadMemes (string path)
        {
            var res =  new List<Meme>();
            var files = Directory.GetFiles (path).ToList ();
            int i = 0;
            var directories = Directory.GetDirectories (path).ToList ();
            for (int j = 0; j < directories.Count; j++)
            {
                foreach (var file in Directory.GetFiles (directories[j]))
                    files.Add (file);
                foreach (var directory in Directory.GetDirectories (directories[j]))
                    directories.Add (directory);
            }

            foreach (var mem in files)
            {
                if (!(mem.EndsWith (".png") || mem.EndsWith (".jpg") || mem.EndsWith (".jpeg"))) continue;
                using (FileStream fs = File.Open (mem, FileMode.Open))
                {
                    double p = (double) i / files.Count * 100;
                    Console.Title = $"Saving.....{p.ToString().Substring(0, p.ToString().IndexOf(",") + 2)}%";
                    i++;
                    res.Add( (new Meme (fs, 0)));
                }
            }
return res;
        }

        internal static IEnumerable<MemeSheet> ComposeMemes (List<Meme> source)
        {
            for (int i = 3; i < 10; i++)
            {
                var result = new List<MemeSheet> ();

                foreach (var row in AllRows (source, i))
                {
                    var sheet = result.FirstOrDefault (s => s.HeightLeft >= row.Height);
                    if (sheet == null)
                    {
                        sheet = new MemeSheet ();
                        result.Add (sheet);
                    }
                    sheet.Content.Add (row);
                }

                foreach (var sheet in result)
                {
                    if (sheet.FreeSpace < 5)
                    {
                        foreach (var row in sheet.Content)
                        {
                            foreach (var meme in row.Memes)
                            {
                                source.Remove (meme);
                            }
                        }
                        yield return sheet;
                    }
                }
            }
        }

        internal static IEnumerable<Row> AllRows (List<Meme> source, int defaultLenght = 6)
        {
            List<Meme> done = new List<Meme> ();
            int rowLenght = source.Count >= defaultLenght ? defaultLenght : source.Count;
            if (rowLenght < 1) yield break;
            foreach (var memes in Combinations.CombinationsRosettaWoRecursion (source.ToArray (), rowLenght))
            {
                if (done.Any (m => memes.Contains (m))) continue;
                var row = new Row ();
                foreach (var meme in memes)
                {
                    row.Add (meme);
                }
                if (row.Height > 150 && row.Height < 500)
                {
                    foreach (var m in row.Memes)
                        source.Remove (m);
                    yield return row;
                    foreach (var res in AllRows (source))
                        yield return res;
                    yield break;
                }
            }
        }
    }
    internal static class Combinations
    {
        // Enumerate all possible m-size combinations of [0, 1, ..., n-1] array
        // in lexicographic order (first [0, 1, 2, ..., m-1]).
        private static IEnumerable<int[]> CombinationsRosettaWoRecursion (int m, int n)
        {
            int[] result = new int[m];
            Stack<int> stack = new Stack<int> (m);
            stack.Push (0);
            while (stack.Count > 0)
            {
                int index = stack.Count - 1;
                int value = stack.Pop ();
                while (value < n)
                {
                    result[index++] = value++;
                    stack.Push (value);
                    if (index != m) continue;
                    yield return (int[]) result.Clone (); // thanks to @xanatos
                    //yield return result;
                    break;
                }
            }
        }

        public static IEnumerable<T[]> CombinationsRosettaWoRecursion<T> (T[] array, int m)
        {
            if (array.Length < m)
                throw new ArgumentException ("Array length can't be less than number of selected elements");
            if (m < 1)
                throw new ArgumentException ("Number of selected elements can't be less than 1");
            T[] result = new T[m];
            foreach (int[] j in CombinationsRosettaWoRecursion (m, array.Length))
            {
                for (int i = 0; i < m; i++)
                {
                    result[i] = array[j[i]];
                }
                yield return result;
            }
        }
    }
}