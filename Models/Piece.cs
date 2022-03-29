using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    public class Piece
    {
        public readonly CellState cellColor;
        //contains x,y coordinates relative to rotation point of the shape
        private readonly IReadOnlyCollection<CoordinatesPair> RelativecellPostions;
        //tracks the location of the cells on the board
        public CoordinatesPair[] Postions { get; private set; }

        public Piece(List<CoordinatesPair> RelativecellPostions, CellState Color)
        {
            this.RelativecellPostions = RelativecellPostions.AsReadOnly();
            Postions = new CoordinatesPair[RelativecellPostions.Count];
            cellColor = Color;
        }

        public void SetPosition(int x, int y)
        {
            for (int i = 0; i < RelativecellPostions.Count; i++)
            {
                int cellX = x + RelativecellPostions.ElementAt(i).x;
                int cellY = y + RelativecellPostions.ElementAt(i).y;
                Postions[i] = new CoordinatesPair(cellX, cellY);
            }
        }

        public bool HasPostion()
        {
            for (int i = 0; i < Postions.Length; i++)
            {
                if (Postions[i] == null)
                {
                    return false;
                }
            }
            return true;
        }

        public void ClearPosition()
        {
            for (int i = 0; i < Postions.Length; i++)
            {
                Postions[i] = null;
            }
        }

        public void MoveX(int distance)
        {
            if (!HasPostion())
            {
                return;
            }
            foreach (CoordinatesPair coordinates in Postions)
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
            foreach (CoordinatesPair coordinates in Postions)
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

        /// <summary>
        /// Gets the cellState of the given coordinates appearence 
        /// as if the piece was layed out on a grid 
        /// where 0,0 is the bottom left
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public CellState GetCellAppearance(int x, int y)
        {
            //get left most and bottom most positions
            int? minX = null;
            int? minY = null;
            foreach (CoordinatesPair coordinates in RelativecellPostions)
            {
                if (minX == null || coordinates.x < minX)
                {
                    minX = coordinates.x;
                }

                if (minY == null || coordinates.y < minY)
                {
                    minY = coordinates.y;
                }
            }
            foreach (CoordinatesPair coordinates in RelativecellPostions)
            {
                if (x == coordinates.x - minX.Value && y == coordinates.y - minY.Value)
                {
                    return cellColor;
                }
            }
            return CellState.EMPTY;
        }
    }
}
