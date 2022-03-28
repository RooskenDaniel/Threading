using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class PlayField
    {
        private const int PIECES_IN_QUEUE = 7;
        private const int TICKS_PER_AUTO_MOVE = 30;

        //grid coodinates go up to down, left to right
        private CellState[,] grid;
        private Piece currentPiece;
        //keeps track of next couple of pieces to show in gui
        public ImmutableQueue<Piece> IncomingPieces
        { get
            {
                return ImmutableQueue.CreateRange(_incomingPieces);
            } }
        private readonly Queue<Piece> _incomingPieces = new Queue<Piece>();

        private int ticksSinceAutoMove = 0;
        public PlayField(int width, int height)
        {
            grid = new CellState[width, height];
            currentPiece = null;
            spawnNextPiece();
        }

        /// <summary>
        /// Returns the cellstate the cell in this coodinates should be drawn as. 
        /// Might differ from the acutal state of the cell in the grid.
        /// </summary>
        public CellState getCellAppearance(int x, int y)
        {
            //check if the cell contains a falling piece
            foreach(CoordinatesPair location in currentPiece.Postions)
            {
                if(location.x == x && location.y == y)
                {
                    return currentPiece.cellColor;
                }
            }
            //draw cell in grid
            return grid[x, y];
        }

        public void Tick(double gameTime)
        {
            if (ticksSinceAutoMove >= TICKS_PER_AUTO_MOVE)
            {
                MovePieceDown(currentPiece, 1);
                ticksSinceAutoMove = 0;
            }
            else
            {
                ticksSinceAutoMove++;
            }
        }

        public void MovePieceRight()
        {
            //todo
        }

        public void MovePieceLeft()
        {
            //todo
        }

        public void RotatePieceLeft()
        {
            //todo
        }

        public void RotatePieceRight()
        {
            //todo
        }

        public void SoftDrop()
        {
            //todo
        }

        public void HardDrop()
        {
            //todo
        }

        public void HoldPiece()
        {
            //todo
        }

        private void spawnNextPiece()
        {
            //handle queue
            while(_incomingPieces.Count < PIECES_IN_QUEUE)
            {
                _incomingPieces.Enqueue(PieceGenerator.GetNext());
            }
            currentPiece = _incomingPieces.Dequeue();

            //set spawn position
            currentPiece.SetPosition(4, grid.GetLength(1));
        }

        private void LandPiece(Piece piece)
        {
            foreach (CoordinatesPair location in piece.Postions)
            {
                if (location.y > grid.GetLength(1) - 1)
                {
                    EndGame();
                    return;
                }
                else
                {
                    grid[location.x, location.y] = piece.cellColor;
                } 
            }
            spawnNextPiece();
        }

        private void MovePieceDown(Piece piece, int distance)
        {
            piece.MoveY(-distance);
            //check for collison
            bool landed = false;
            for(int i = 0; i < currentPiece.Postions.Length; i++)
            {
                CoordinatesPair cellPos = currentPiece.Postions[i];
                if(cellPos.y < 0 || cellPos.y <= grid.GetLength(1) - 1 && grid[cellPos.x, cellPos.y] != CellState.EMPTY)
                {
                    piece.MoveY(1);
                    i = 0;
                    landed = true;
                }
            }
            if (landed)
            {
                LandPiece(piece);
            }
        }
        private void EndGame()
        {
            //todo
        }

    }
}
