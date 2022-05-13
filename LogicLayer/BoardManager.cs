using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DataObjects;

namespace LogicLayer
{
    public class BoardManager : IBoardManager
    {
        private Game _game;
        private int _turn; // evens white, odds black
        public Board Board { get; set; }
        public BoardManager()
        {
            Board = new Board();
            _turn = 0;
        }

        public BoardManager(Game game)
        {
            Board = new Board();
            _game = game;   
            _turn = 0;
        }

        public BoardManager(Game game, Board board, int turn)
        {
            _game = game;
            Board = board;
            _turn = turn;
        }

        public Board GetBoard()
        {
            return Board;
        }

        public int GetTurn()
        {
            return _turn;
        }

        public void FlipBoard()
        {
            // store pieces in temp array and clear board
            Piece[] temp = new Piece[Board.Count];
            Board.CopyTo(temp, 0);
            Board.Clear();

            // place new pieces, mirroring their position
            foreach(Piece p in temp)
            {
                int mirroredX = 7 - p.Position.X;
                int mirroredY = 7 - p.Position.Y;
                Board.Add(new Piece(p.Color, p.Type, new Point(mirroredX, mirroredY)));
            }

            if(Board.Color == DataObjects.Color.White)
            {
                Board.Color = DataObjects.Color.Black;
            }
            else
            {
                Board.Color = DataObjects.Color.White;
            }
        }

        public void ResetBoard()
        {
            if (Board.Color == DataObjects.Color.White)
            {
                Board = new Board(DataObjects.Color.White);
            }
            else
            {
                Board = new Board(DataObjects.Color.Black);
            }

            _turn = 0;
        }

        public int SetTurn(int turn)
        {
            if(_game != null)
            {
                // force turn into bounds
                if (turn < 0)
                {
                    turn = 0;
                }
                if (turn > _game.Moves.Count)
                {
                    turn = _game.Moves.Count;
                }

                while (turn > _turn && turn <= _game.Moves.Count)
                {
                    NextGameMove();
                }

                while (turn < _turn && turn >= 0)
                {
                    PrevGameMove();
                }
            }

            return _turn;
        }

        public void MakeMove(string origin, string destination)
        {
            try
            {
                Board.Remove(Board.pieceAt(destination));
            }
            catch (Exception)
            {
                
            }

            Board.pieceAt(origin).Position = Board.getSquare(destination);
        }

        public void MakeMove(Piece piece, string destination)
        {
            if(Board.pieceAt(destination) != null)
            {
                Board.Remove(Board.pieceAt(destination));
            }
            piece.Position = Board.getSquare(destination);
        }

        public void LoadGame(Game game)
        {
            _game = game;
        }

        public void LoadBoard(Board board)
        {
            Board = board;
        }

        public int NextGameMove()
        {
            if (_game != null && _game.Moves.Count >= _turn + 1)
            {
                Move move = _game.Moves[_turn];
                MakeMove(move.Origin, move.Destination);
                // if move was en passant remove captured pawn
                if (move.IsEnPassant)
                {
                    string enPassantSquare;
                    if(getTurnPlayer() == DataObjects.Color.White)
                    {
                        enPassantSquare = move.Destination[0] + "5"; // same file as move's destination, white en passant always takes 5th rank
                    }
                    else
                    {
                        enPassantSquare = move.Destination[0] + "4"; // same file as move's destination, black en passant always takes 4th rank
                    }

                    Board.Remove(Board.pieceAt(enPassantSquare));
                }
                if (move.Promotion != PieceType.None)
                {
                    Board.pieceAt(move.Destination).Type = move.Promotion;
                }
                if (move.IsCastle)
                {
                    if (move.Destination[0] == 'g')
                    {
                        Board.pieceAt("h" + move.Destination[1]).Position = Board.getSquare("f" + move.Destination[1]);
                    }
                    else
                    {
                        Board.pieceAt("a" + move.Destination[1]).Position = Board.getSquare("d" + move.Destination[1]);
                    }
                }
                // garbage solution for testing
                // FlipBoard();
                // FlipBoard();
                _turn++;
            }

            return _turn;
        }

        public int PrevGameMove()
        {
            if (_game != null && _turn >= 1)
            {
                // move to previous turn to undo
                _turn--;
                // move piece back where it was
                Move move = _game.Moves[_turn];
                MakeMove(move.Destination, move.Origin);
                // if previous move was a capture, need to replace the piece that was removed
                if (move.Capture != PieceType.None)
                {
                        
                    if (move.IsEnPassant)
                    {
                        // prev move was en passant.  must replace captured pawn 1 square up the file
                        if (getTurnPlayer(_turn + 1) == DataObjects.Color.White) // turn player of move being undone
                        {
                            string enPassantSquare = move.Destination[0] + "5"; // same file as move's destination, white en passant always takes 5th rank
                            Board.Add(new Piece(DataObjects.Color.Black,
                                                move.Capture, // should always be pawn
                                                Board.getSquare(enPassantSquare)));
                        }
                        else
                        {
                            string enPassantSquare = move.Destination[0] + "4"; // same file as move's destination, black en passant always takes 4th rank
                            Board.Add(new Piece(DataObjects.Color.White,
                                                move.Capture, // should always be pawn
                                                Board.getSquare(enPassantSquare)));
                        }
                    }
                    else
                    {
                        // not en passant capture, piece just goes at move's destination
                        Board.Add(new Piece(getTurnPlayer(_turn), // opponent's color
                                            move.Capture,
                                            Board.getSquare(move.Destination)));
                    }
                }
                // if previous move was a promotion, unpromote piece
                if (move.Promotion != PieceType.None)
                {
                    Board.pieceAt(move.Origin).Type = PieceType.Pawn;
                }
                // if previous move was a castle, move rook as well
                if (move.IsCastle)
                {
                    if(move.Destination[0] == 'g')
                    {
                        Board.pieceAt("f" + move.Destination[1]).Position = Board.getSquare("h" + move.Destination[1]);
                    }
                    else
                    {
                        Board.pieceAt("d" + move.Destination[1]).Position = Board.getSquare("a" + move.Destination[1]);
                    }
                }
                // garbage solution for testing
                // FlipBoard();
                // FlipBoard();
            }

            return _turn;
        }

        private DataObjects.Color getTurnPlayer()
        {
            DataObjects.Color turnPlayer;

            turnPlayer = _turn % 2 == 0 ? DataObjects.Color.White : DataObjects.Color.Black;

            return turnPlayer;
        }

        private DataObjects.Color getTurnPlayer(int turn)
        {
            DataObjects.Color turnPlayer;

            turnPlayer = turn % 2 == 1 ? DataObjects.Color.White : DataObjects.Color.Black;

            return turnPlayer;
        }
    }
}