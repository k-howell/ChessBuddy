using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface IBoardManager
    {
        Board GetBoard();
        void LoadGame(Game game);
        void FlipBoard();
        void ResetBoard();
        int GetTurn();
        int SetTurn(int turn);
        void MakeMove(string origin, string destination);
        void MakeMove(Piece piece, string destination);
        void NextGameMove();
        void PrevGameMove();
    }
}
