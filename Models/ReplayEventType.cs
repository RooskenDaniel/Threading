using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Models
{
    public enum ReplayEventType
    {
        SOFT_DROP,
        HARD_DROP,
        LEFT,
        RIGHT,
        ROTATE_LEFT,
        ROTATE_RIGHT,
        HOLD,
        AUTO_MOVE,
        PIECE_SPAWNED
    }
}
