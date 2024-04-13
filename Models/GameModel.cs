using Checkers.ViewModels;
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
            for (int line_index = 0; line_index < _boardSize; line_index++)
            {
                _board.Add(new List<PieceModel>());
                for (int column_index = 0; column_index < _boardSize; column_index++)
                {
                    if ((line_index + column_index) % 2 == 1)
                    {
                        if (line_index < 3)
                        {
                            _board[line_index].Add(new PieceModel(line_index, column_index, PieceType.WhitePawn));
                            continue;
                        }

                        if (line_index > 4)
                        {
                            _board[line_index].Add(new PieceModel(line_index, column_index, PieceType.BlackPawn));
                            continue;
                        }

                        _board[line_index].Add(null); 
                    }
                    else
                        _board[line_index].Add(null);
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

        #region Methods
        public void MovePiece(int x1, int y1, int x2, int y2)
        {
            _board[x2][y2] = new PieceModel(x2, y2, _board[x1][y1].Type);
            _board[x1][y1] = null;
        }

        public PieceModel GetPiece(int x, int y)
        {
            return _board[x][y];
        }

        public void MoveUpPiece(PieceModel Piece, Tuple<int, int> Pos)
        {
            int diffX = Piece.X - Pos.Item1;
            int diffY = Piece.Y - Pos.Item2;
            if (diffX > 0 && diffX <= 1 && diffY >= -1 && diffY <= 1)
                MovePiece(Piece.X, Piece.Y, Pos.Item1, Pos.Item2);
        }
        public void MoveDownPiece(PieceModel Piece, Tuple<int, int> Pos)
        {
            int diffX = Piece.X - Pos.Item1;
            int diffY = Piece.Y - Pos.Item2;
            if (diffX >= -1 && diffX <= 0 && diffY >= -1 && diffY <= 1)
                MovePiece(Piece.X, Piece.Y, Pos.Item1, Pos.Item2);
        }
        #endregion

    }
}
