using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame.Managers
{
    public class LoadLevelsManager
    {
        public char[,] LoadLevel(string path)
        {
            using var sw = new StreamReader(path);
            var size = sw.ReadLine()!.Split().Select(int.Parse);
            var (r, c) = (size.First(), size.Last());
            var level = new char[r, c];
            for (var i = 0; i < r; i++)
            {
                var row = sw.ReadLine()!.ToArray();
                for (var j = 0; j < c; j++)
                    level[i, j] = row[j];
            }

            return level;
        }
    }
}
