using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    interface IBoardManager
    {
        void LoadGame(Game game);
        void FlipBoard();
        void ResetBoard();
        void MakeMove(string origin, string destination);
        void MakeMove(Piece piece, string destination);
        void NextGameMove();
        void PrevGameMove();
    }
}
