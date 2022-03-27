using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal static class PieceGenerator
    {
        /// <summary>
        /// Generates new pieces in a semi-random order using the 'random bag' method
        /// </summary>
        private static List<PieceShape> shapeBag = new List<PieceShape>();
        private static readonly Random rng = new Random();

        public static Piece GetNext()
        {
            if (shapeBag.Count <= 0)
            {
                shapeBag = Enum.GetValues(typeof(PieceShape)).Cast<PieceShape>().ToList();
            }
            int i = rng.Next(0, shapeBag.Count - 1);
            Piece pickedPiece = Generate(shapeBag[i]);
            shapeBag.RemoveAt(i);
            return pickedPiece;
        }

        private static Piece Generate(PieceShape shape)
        {
            //contains x,y coordinates relative to rotation point of the shape
            List<CoordinatesPair> positions = new List<CoordinatesPair>();
            switch (shape)
            {
                case PieceShape.O:
                    positions.Add(new CoordinatesPair(0, 0));
                    positions.Add(new CoordinatesPair(0, 1));
                    positions.Add(new CoordinatesPair(1, 0));
                    positions.Add(new CoordinatesPair(1, 1));
                    return new Piece(positions, CellState.YELLOW);
                case PieceShape.I:
                    positions.Add(new CoordinatesPair(-1, 0));
                    positions.Add(new CoordinatesPair(0, 0));
                    positions.Add(new CoordinatesPair(1, 0));
                    positions.Add(new CoordinatesPair(2, 0));
                    return new Piece(positions, CellState.LIGHT_BLUE);
                case PieceShape.S:
                    positions.Add(new CoordinatesPair(-1, 0));
                    positions.Add(new CoordinatesPair(0, 0));
                    positions.Add(new CoordinatesPair(0, 1));
                    positions.Add(new CoordinatesPair(1, 1));
                    return new Piece(positions, CellState.GREEN);
                case PieceShape.Z:
                    positions.Add(new CoordinatesPair(0, 0));
                    positions.Add(new CoordinatesPair(1, 0));
                    positions.Add(new CoordinatesPair(-1, 1));
                    positions.Add(new CoordinatesPair(0, 1));
                    return new Piece(positions, CellState.RED);
                case PieceShape.L:
                    positions.Add(new CoordinatesPair(0, 0));
                    positions.Add(new CoordinatesPair(-1, 0));
                    positions.Add(new CoordinatesPair(1, 0));
                    positions.Add(new CoordinatesPair(1, 1));
                    return new Piece(positions, CellState.ORANGE);
                case PieceShape.J:
                    positions.Add(new CoordinatesPair(0, 0));
                    positions.Add(new CoordinatesPair(-1, 0));
                    positions.Add(new CoordinatesPair(1, 0));
                    positions.Add(new CoordinatesPair(-1, 1));
                    return new Piece(positions, CellState.DARK_BLUE);
                case PieceShape.T:
                    positions.Add(new CoordinatesPair(0, 0));
                    positions.Add(new CoordinatesPair(-1, 0));
                    positions.Add(new CoordinatesPair(1, 0));
                    positions.Add(new CoordinatesPair(0, 1));
                    return new Piece(positions, CellState.MAGENTA);
                default:
                    throw new ArgumentException("Invalid shape given");
            }
        }
    }
}
