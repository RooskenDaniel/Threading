using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Models
{
    public class Replay
    {
        public string Filename { get; set; }

        public Replay(string filename)
        {
            Filename = filename;
        }
    }
}
