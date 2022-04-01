using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Models
{
    public class ReplayEvent
    {
        public int Timestamp { get; set; }
        public ReplayEvent EventType { get; set; }

        public Object Data { get; set; }
    }
}
