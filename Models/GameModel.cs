using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Models
{
    internal class GameModel
    {
        public GameModel()
        {
        _board = new List<List<PieceModel>>();
            for (int i_index = 0; i_index < _boardSize; i_index++)
            {
                _board.Add(new List<PieceModel>());
                for (int j_index = 0; j_index < _boardSize; j_index++)
                {
                    //_board[i_index].Add(new PieceModel((i_index + j_index) % 2 == 0, i_index, j_index));
                }
            }
        } 
        #region Properties and Members

        private List<List<PieceModel>> _board;
        private static readonly int _boardSize = 8;

        public List<List<PieceModel>> Board
        {
            get { return _board; }
        }

        public int BoardSize
        {
            get { return _boardSize; }
        }

        #endregion


    }
}
