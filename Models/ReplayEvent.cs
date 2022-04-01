using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Models
{
    public class ReplayEvent
    {
        public double Timestamp { get; set; }
        public ReplayEventType EventType { get; set; }

        public Object Data { get; set; }

        public ReplayEvent(double timestamp, ReplayEventType type, Object data)
        {
            this.Timestamp = timestamp;
            this.EventType = type;
            this.Data = data;
        }
        public ReplayEvent(double timestamp, ReplayEventType type)
        {
            this.Timestamp = timestamp;
            this.EventType = type;
            this.Data = null;
        }

    }
}
