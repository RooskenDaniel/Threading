using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class Piece
    {
        public readonly CellState cellColor;
        //contains x,y coordinates relative to rotation point of the shape
        private readonly IReadOnlyCollection<CoordinatesPair> RelativecellPostions;
        //tracks the location of the cells on the board
        private readonly CoordinatesPair[] _Postions;
        public CoordinatesPair[] Postions { get { return _Postions; } }

        public Piece(List<CoordinatesPair> RelativecellPostions, CellState Color)
        {
            this.RelativecellPostions = RelativecellPostions.AsReadOnly();
            _Postions = new CoordinatesPair[RelativecellPostions.Count];
            this.cellColor = Color;
        }

        public void SetPosition(int x, int y)
        {
            for (int i = 0; i < RelativecellPostions.Count; i++)
            {
                int cellX = x + RelativecellPostions.ElementAt(i).x;
                int cellY = y + RelativecellPostions.ElementAt(i).y;
                _Postions[i] = new CoordinatesPair(cellX, cellY);
            }
         }

        public bool HasPostion()
        {
            for (int i = 0; i < _Postions.Length; i++)
            {
                if (_Postions[i] == null)
                {
                    return false;
                }
            }
            return true;
        }

        public void ClearPosition()
        {
            for(int i = 0; i < _Postions.Length; i++)
            {
                _Postions[i] = null;
            }
        }

        public void MoveX(int distance)
        {
            if (!HasPostion())
            {
                return;
            }
            foreach(CoordinatesPair coordinates in _Postions)
            {
                coordinates.x += distance;
            }
        }

        public void MoveY(int distance)
        {
            if (!HasPostion())
            {
                return;
            }
            foreach (CoordinatesPair coordinates in _Postions)
            {
                coordinates.y += distance;
            }
        }

        public void RotateRight()
        {
            if (!HasPostion())
            {
                return;
            }
        }

        public void RotateLeft()
        {
            if (!HasPostion())
            {
                return;
            }
        }
    }
}
