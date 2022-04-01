using System.Collections.Generic;
using System.Collections.Immutable;
using Tetris.Models;

namespace Tetris
{
    internal class PlayField
    {
        private const int PIECES_IN_QUEUE = 7;
        private const int TICKS_PER_AUTO_MOVE = 1;

        //grid coodinates go up to down, left to right
        private readonly CellState[,] grid;
        private Piece currentPiece;
        public Piece HeldPiece { get; private set; }
        public bool gameIsOver = false;
        //keeps track of next couple of pieces to show in gui
        public ImmutableQueue<Piece> IncomingPieces => ImmutableQueue.CreateRange(_incomingPieces);
        private readonly Queue<Piece> _incomingPieces = new Queue<Piece>();

        private int ticksSinceAutoMove = 0;

        public int score = 0;
        private string filenameTimestamp = System.DateTime.Now.ToString();

        private bool holdLock = false;

        private double timeStamp = 0;

        public Replay loadedReplay { get; set; } = null;
        public bool replayPlaybackMode = false;

        public PlayField(int width, int height)
        {
            grid = new CellState[width, height];
            currentPiece = null;
            SpawnNextPiece();
        }

        /// <summary>
        /// Returns the cellstate the cell in this coodinates should be drawn as. 
        /// Might differ from the acutal state of the cell in the grid.
        /// </summary>
        public CellState GetCellAppearance(int x, int y)
        {
            //check if the cell contains a falling piece
            foreach (CoordinatesPair location in currentPiece.Postions)
            {
                if (location.x == x && location.y == y)
                {
                    return currentPiece.cellColor;
                }
            }
            //draw cell in grid
            return grid[x, y];
        }

        public void Tick(double gameTime)
        {
            this.timeStamp = gameTime;
            if (ticksSinceAutoMove >= TICKS_PER_AUTO_MOVE)
            {
                MovePieceDown(currentPiece, 1);
                ticksSinceAutoMove = 0;
                RecordEvent(ReplayEventType.AUTO_MOVE);
            }
            else
            {
                ticksSinceAutoMove++;
            }
        }

        public void MovePieceRight()
        {
            bool willBeInSideWall = false;//The program checks wheter the tetrominoes will be inside the wall, if thats the case the tetromino shall not be moved
            for (int i = 0; i < currentPiece.Postions.Length; i++)
            {
                CoordinatesPair cellPos = currentPiece.Postions[i];
                if (cellPos.x > 8)
                {

                    willBeInSideWall = true;
                }
            }
            if (willBeInSideWall == false)
            {
                currentPiece.MoveX(1);
            }
            RecordEvent(ReplayEventType.RIGHT);
        }

        public void MovePieceLeft()
        {
            bool willBeInSideWall = false;//The program checks wheter the tetrominoes will be inside the wall, if thats the case the tetromino shall not be moved
            for (int i = 0; i < currentPiece.Postions.Length; i++)
            {
                CoordinatesPair cellPos = currentPiece.Postions[i];
                if (cellPos.x < 1)
                {
                    willBeInSideWall = true;
                }
            }
            if (willBeInSideWall == false)
            { 
                currentPiece.MoveX(-1);
            }
            RecordEvent(ReplayEventType.LEFT);
        }

        public void RotatePieceLeft()
        {
            currentPiece.RotateLeft();
            RecordEvent(ReplayEventType.ROTATE_LEFT);
        }

        public void RotatePieceRight()
        {
            currentPiece.RotateRight();
            RecordEvent(ReplayEventType.ROTATE_RIGHT);
        }

        public void SoftDrop()
        {
            MovePieceDown(currentPiece, 1);
            RecordEvent(ReplayEventType.SOFT_DROP);
        }

        public void HardDrop()
        {
            MovePieceDown(currentPiece, 19);
            RecordEvent(ReplayEventType.HARD_DROP);
        }

        public void HoldPiece()
        {
            if(currentPiece != null && !holdLock)
            {
                if(HeldPiece != null)
                {
                    Piece p = HeldPiece;
                    HeldPiece = currentPiece;
                    HeldPiece.ClearPosition();
                    currentPiece = p;
                    currentPiece.SetPosition(4, grid.GetLength(1));
                    holdLock = true;
                }
                else
                {
                    HeldPiece = currentPiece;
                    HeldPiece.ClearPosition();
                    SpawnNextPiece();
                }
            }
            RecordEvent(ReplayEventType.HOLD);
        }

        private void SpawnNextPiece()
        {
            //handle queue
            while (_incomingPieces.Count < PIECES_IN_QUEUE)
            {
                _incomingPieces.Enqueue(PieceGenerator.GetNext());
            }
            currentPiece = _incomingPieces.Dequeue();

            //set spawn position
            currentPiece.SetPosition(4, grid.GetLength(1));
            holdLock = false;
        }

        private void LandPiece(Piece piece)
        {
            foreach (CoordinatesPair location in piece.Postions)
            {
                if (location.y > grid.GetLength(1) - 1)
                {
                    gameIsOver = true;
                    return;
                }
                else
                {
                    grid[location.x, location.y] = piece.cellColor;
                }
            }
            clearLines(0);
            SpawnNextPiece();
        }

        private void clearLines(int linesCleared)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                bool rowISFilled = true;
                for (int y = 0; y < grid.GetLength(0); y++)
                {
                    if (grid[y, x] == CellState.EMPTY)
                    {
                        rowISFilled = false;
                        break;
                    }
                }
                if (rowISFilled == true)
                {
                    for (int y = 0; y < grid.GetLength(0); y++)
                    {
                        for (int xMover = x; xMover < grid.GetLength(1) - 1; xMover++)
                        {
                            grid[y, xMover] = grid[y, xMover + 1];
                        }
                    }
                    clearLines(linesCleared + 1);
                }
            }
            switch (linesCleared)
            {
                case 1:
                    score += 40;
                    break;
                case 2:
                    score += 100;
                    break;
                case 3:
                    score += 300;
                    break;
                case 4:
                    score += 1200;
                    break;
            }
        }

        //todo handle exceptions
        private async void RecordEvent(ReplayEventType replayEventType, object replayEventData)
        {
            if (!replayPlaybackMode)
            {
                if (this.loadedReplay == null)
                {
                    this.loadedReplay =  await ReplayManager.CreateReplay();
                }
                ReplayEvent replayEvent = new ReplayEvent(this.timeStamp, replayEventType, replayEventData);
                await ReplayManager.WriteEventToJson(replayEvent, this.loadedReplay);
            }
        }

        private void RecordEvent(ReplayEventType replayEventType)
        {
            RecordEvent(replayEventType, null);
        }

        private void MovePieceDown(Piece piece, int distance)
        {
            piece.MoveY(-distance);
            //check for collison
            bool landed = false;
            for (int i = 0; i < currentPiece.Postions.Length; i++)
            {
                CoordinatesPair cellPos = currentPiece.Postions[i];
                if (cellPos.y < 0 || cellPos.y <= grid.GetLength(1) - 1 && grid[cellPos.x, cellPos.y] != CellState.EMPTY)
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

    }
}
