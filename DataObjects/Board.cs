using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Board : ObservableCollection<Piece>
    {
        // dictionaries to simplify converting named position to Point coordinates
        // from each board perspective
        public static readonly Dictionary<string, Point> PositionsWhite = new Dictionary<string, Point>
        {
            { "a1", new Point(0, 7) },
            { "a2", new Point(0, 6) },
            { "a3", new Point(0, 5) },
            { "a4", new Point(0, 4) },
            { "a5", new Point(0, 3) },
            { "a6", new Point(0, 2) },
            { "a7", new Point(0, 1) },
            { "a8", new Point(0, 0) },
            { "b1", new Point(1, 7) },
            { "b2", new Point(1, 6) },
            { "b3", new Point(1, 5) },
            { "b4", new Point(1, 4) },
            { "b5", new Point(1, 3) },
            { "b6", new Point(1, 2) },
            { "b7", new Point(1, 1) },
            { "b8", new Point(1, 0) },
            { "c1", new Point(2, 7) },
            { "c2", new Point(2, 6) },
            { "c3", new Point(2, 5) },
            { "c4", new Point(2, 4) },
            { "c5", new Point(2, 3) },
            { "c6", new Point(2, 2) },
            { "c7", new Point(2, 1) },
            { "c8", new Point(2, 0) },
            { "d1", new Point(3, 7) },
            { "d2", new Point(3, 6) },
            { "d3", new Point(3, 5) },
            { "d4", new Point(3, 4) },
            { "d5", new Point(3, 3) },
            { "d6", new Point(3, 2) },
            { "d7", new Point(3, 1) },
            { "d8", new Point(3, 0) },
            { "e1", new Point(4, 7) },
            { "e2", new Point(4, 6) },
            { "e3", new Point(4, 5) },
            { "e4", new Point(4, 4) },
            { "e5", new Point(4, 3) },
            { "e6", new Point(4, 2) },
            { "e7", new Point(4, 1) },
            { "e8", new Point(4, 0) },
            { "f1", new Point(5, 7) },
            { "f2", new Point(5, 6) },
            { "f3", new Point(5, 5) },
            { "f4", new Point(5, 4) },
            { "f5", new Point(5, 3) },
            { "f6", new Point(5, 2) },
            { "f7", new Point(5, 1) },
            { "f8", new Point(5, 0) },
            { "g1", new Point(6, 7) },
            { "g2", new Point(6, 6) },
            { "g3", new Point(6, 5) },
            { "g4", new Point(6, 4) },
            { "g5", new Point(6, 3) },
            { "g6", new Point(6, 2) },
            { "g7", new Point(6, 1) },
            { "g8", new Point(6, 0) },
            { "h1", new Point(7, 7) },
            { "h2", new Point(7, 6) },
            { "h3", new Point(7, 5) },
            { "h4", new Point(7, 4) },
            { "h5", new Point(7, 3) },
            { "h6", new Point(7, 2) },
            { "h7", new Point(7, 1) },
            { "h8", new Point(7, 0) }
        };
        public static readonly Dictionary<string, Point> PositionsBlack = new Dictionary<string, Point>
        {
            { "a1", new Point(7, 0) },
            { "a2", new Point(7, 1) },
            { "a3", new Point(7, 2) },
            { "a4", new Point(7, 3) },
            { "a5", new Point(7, 4) },
            { "a6", new Point(7, 5) },
            { "a7", new Point(7, 6) },
            { "a8", new Point(7, 7) },
            { "b1", new Point(6, 0) },
            { "b2", new Point(6, 1) },
            { "b3", new Point(6, 2) },
            { "b4", new Point(6, 3) },
            { "b5", new Point(6, 4) },
            { "b6", new Point(6, 5) },
            { "b7", new Point(6, 6) },
            { "b8", new Point(6, 7) },
            { "c1", new Point(5, 0) },
            { "c2", new Point(5, 1) },
            { "c3", new Point(5, 2) },
            { "c4", new Point(5, 3) },
            { "c5", new Point(5, 4) },
            { "c6", new Point(5, 5) },
            { "c7", new Point(5, 6) },
            { "c8", new Point(5, 7) },
            { "d1", new Point(4, 0) },
            { "d2", new Point(4, 1) },
            { "d3", new Point(4, 2) },
            { "d4", new Point(4, 3) },
            { "d5", new Point(4, 4) },
            { "d6", new Point(4, 5) },
            { "d7", new Point(4, 6) },
            { "d8", new Point(4, 7) },
            { "e1", new Point(3, 0) },
            { "e2", new Point(3, 1) },
            { "e3", new Point(3, 2) },
            { "e4", new Point(3, 3) },
            { "e5", new Point(3, 4) },
            { "e6", new Point(3, 5) },
            { "e7", new Point(3, 6) },
            { "e8", new Point(3, 7) },
            { "f1", new Point(2, 0) },
            { "f2", new Point(2, 1) },
            { "f3", new Point(2, 2) },
            { "f4", new Point(2, 3) },
            { "f5", new Point(2, 4) },
            { "f6", new Point(2, 5) },
            { "f7", new Point(2, 6) },
            { "f8", new Point(2, 7) },
            { "g1", new Point(1, 0) },
            { "g2", new Point(1, 1) },
            { "g3", new Point(1, 2) },
            { "g4", new Point(1, 3) },
            { "g5", new Point(1, 4) },
            { "g6", new Point(1, 5) },
            { "g7", new Point(1, 6) },
            { "g8", new Point(1, 7) },
            { "h1", new Point(0, 0) },
            { "h2", new Point(0, 1) },
            { "h3", new Point(0, 2) },
            { "h4", new Point(0, 3) },
            { "h5", new Point(0, 4) },
            { "h6", new Point(0, 5) },
            { "h7", new Point(0, 6) },
            { "h8", new Point(0, 7) }
        };
        // active position dictionary for current board perspective
        private Dictionary<string, Point> _position;
        // ended up having to manage Color property's field manually in order to set position library
        // automatically when board's Color property changes
        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                if (_color == Color.White)
                {
                    _position = PositionsWhite;
                }
                else
                {
                    _position = PositionsBlack;
                }
            }
        }
        public Board(Color perspective = Color.White)
        {
            // set color and position dictionary perspective
            if (perspective == Color.White)
            {
                Color = Color.White;
                // _position = PositionsWhite;
            }
            else
            {
                Color = Color.Black;
                // _position = PositionsBlack;
            }
            // initialize pieces
            // White Pieces
            Add(new Piece(Color.White, PieceType.Rook, _position["a1"]));
            Add(new Piece(Color.White, PieceType.Knight, _position["b1"]));
            Add(new Piece(Color.White, PieceType.Bishop, _position["c1"]));
            Add(new Piece(Color.White, PieceType.Queen, _position["d1"]));
            Add(new Piece(Color.White, PieceType.King, _position["e1"]));
            Add(new Piece(Color.White, PieceType.Bishop, _position["f1"]));
            Add(new Piece(Color.White, PieceType.Knight, _position["g1"]));
            Add(new Piece(Color.White, PieceType.Rook, _position["h1"]));

            Add(new Piece(Color.White, PieceType.Pawn, _position["a2"]));
            Add(new Piece(Color.White, PieceType.Pawn, _position["b2"]));
            Add(new Piece(Color.White, PieceType.Pawn, _position["c2"]));
            Add(new Piece(Color.White, PieceType.Pawn, _position["d2"]));
            Add(new Piece(Color.White, PieceType.Pawn, _position["e2"]));
            Add(new Piece(Color.White, PieceType.Pawn, _position["f2"]));
            Add(new Piece(Color.White, PieceType.Pawn, _position["g2"]));
            Add(new Piece(Color.White, PieceType.Pawn, _position["h2"]));

            // Black Pieces
            Add(new Piece(Color.Black, PieceType.Rook, _position["a8"]));
            Add(new Piece(Color.Black, PieceType.Knight, _position["b8"]));
            Add(new Piece(Color.Black, PieceType.Bishop, _position["c8"]));
            Add(new Piece(Color.Black, PieceType.Queen, _position["d8"]));
            Add(new Piece(Color.Black, PieceType.King, _position["e8"]));
            Add(new Piece(Color.Black, PieceType.Bishop, _position["f8"]));
            Add(new Piece(Color.Black, PieceType.Knight, _position["g8"]));
            Add(new Piece(Color.Black, PieceType.Rook, _position["h8"]));

            Add(new Piece(Color.Black, PieceType.Pawn, _position["a7"]));
            Add(new Piece(Color.Black, PieceType.Pawn, _position["b7"]));
            Add(new Piece(Color.Black, PieceType.Pawn, _position["c7"]));
            Add(new Piece(Color.Black, PieceType.Pawn, _position["d7"]));
            Add(new Piece(Color.Black, PieceType.Pawn, _position["e7"]));
            Add(new Piece(Color.Black, PieceType.Pawn, _position["f7"]));
            Add(new Piece(Color.Black, PieceType.Pawn, _position["g7"]));
            Add(new Piece(Color.Black, PieceType.Pawn, _position["h7"]));
        }

        public Point getSquare(string squareName)
        {
            return _position[squareName];
        }

        public Piece pieceAt(string squareName)
        {
            Piece piece = null;

            piece = this.FirstOrDefault<Piece>((p) => p.Position == _position[squareName]);

            return piece;
        }
    }
}
