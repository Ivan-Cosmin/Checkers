using Checkers.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Checkers.Models
{
    internal class GameModel
    {
        public GameModel(bool MultipleJump)
        {
            _board = new List<List<PieceModel>>();
            _multipleJump = MultipleJump;
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
        private bool _multipleJump;
        public static bool _isWhiteTurn = false;
        private static readonly int _boardSize = 8;

        public List<List<PieceModel>> Board
        {
            get { return _board; }
        }
        public int BoardSize
        {
            get { return _boardSize; }
        }
        public bool IsWhiteTurn
        {
            get { return _isWhiteTurn; }
            set { _isWhiteTurn = value; }
        }
        #endregion

        #region Methods

        public bool MovePiece(PieceModel Piece , ref Tuple<int, int> Pos)
        {
            if (Piece == null)
                return false;

            //0 begin, 7 end of the board
            if(!Piece.IsKing() && (Pos.Item1 == 0 || Pos.Item1 == 7))
                Piece.ChangeInKing();

            if (Piece.Type == PieceType.WhitePawn)
            {
                MoveDownPiece(Piece, ref Pos);
                return true;
            }

            if (Piece.Type == PieceType.BlackPawn)
            {
                MoveUpPiece(Piece, ref Pos);
                return true;
            }

            if (Piece.IsKing())
            {
                MoveUpPiece(Piece, ref Pos);
                MoveDownPiece(Piece, ref Pos);
                return true;
            }

            return false;
        }
        private void MovePiece(int x1, int y1, int x2, int y2)
        {
            PieceModel Piece = new PieceModel(x2, y2, _board[x1][y1].Type);
            _board[x2][y2] = Piece;
            _board[x1][y1] = null;
            if (_multipleJump && Math.Abs(x1-x2) == 2 && HasChangeToJump( Piece))
            { 
                return;
            }
            ChangeTurn();
        }
        public PieceModel GetPiece(int x, int y)
        {
            return _board[x][y];
        }


        #region Boolean Methods
        private int CanMoveUp(PieceModel Piece, ref Tuple<int, int> Pos)
        {
            int diffX = Piece.X - Pos.Item1;
            int diffY = Piece.Y - Pos.Item2;
            if (diffX == 1 && (diffY == -1 || diffY == 1))
                return 1;
            else
            if (diffX == 2 && (diffY == -2 || diffY == 2))
                return 2;
            return 0;
        }
        private int CanMoveDown(PieceModel Piece, ref Tuple<int, int> Pos)
        {
            int diffX = Piece.X - Pos.Item1;
            int diffY = Piece.Y - Pos.Item2;
            if (diffX == -1 && (diffY == -1 || diffY == 1))
                return 1;
            else
            if (diffX == -2 && (diffY == -2 || diffY == 2))
                return 2;
            return 0;
        }
        private bool HasChangeToJumpDown( PieceModel Piece)
        {
            if ( //leftDown
                    Piece.X + 2 >= 0 && Piece.X + 2 < _boardSize &&
                    Piece.Y - 2 >= 0 && Piece.Y - 2 < _boardSize &&
                    _board[Piece.X + 2][Piece.Y - 2] == null &&
                    IsEnemy( Piece, _board[Piece.X + 1][Piece.Y - 1])
                   )
                return true;

            if ( //rightDown
                Piece.X + 2 >= 0 && Piece.X + 2 < _boardSize &&
                Piece.Y + 2 >= 0 && Piece.Y + 2 < _boardSize &&
                _board[Piece.X + 2][Piece.Y + 2] == null &&
                IsEnemy( Piece, _board[Piece.X + 1][Piece.Y + 1])
               )
                return true;
            return false;
        }
        private bool HasChangeToJumpUp(PieceModel Piece)
        {
            if ( //leftUp
                    Piece.X - 2 >= 0 && Piece.X - 2 < _boardSize &&
                    Piece.Y - 2 >= 0 && Piece.Y - 2 < _boardSize &&
                    _board[Piece.X - 2][Piece.Y - 2] == null &&
                    IsEnemy( Piece, _board[Piece.X - 1][Piece.Y - 1])
                   )
                return true;

            if ( //rightUP
                Piece.X - 2 >= 0 && Piece.X - 2 < _boardSize &&
                Piece.Y + 2 >= 0 && Piece.Y + 2 < _boardSize &&
                _board[Piece.X - 2][Piece.Y + 2] == null &&
                IsEnemy( Piece, _board[Piece.X - 1][Piece.Y + 1])
               )
                return true;
            return false;
        }
        private bool HasChangeToJump(PieceModel Piece)
        {
            if (Piece.Type == PieceType.WhitePawn)
            {
                if(HasChangeToJumpDown( Piece) == true)
                    return true;
                return false;
            }
            else if (Piece.Type == PieceType.BlackPawn)
            {
                if(HasChangeToJumpUp( Piece) == true)
                    return true;
                return false;
            }
            else if(Piece.IsKing())
            {
                if(HasChangeToJumpDown( Piece) == true || HasChangeToJumpUp( Piece) == true)
                    return true;
                return false;
            }

            return false;
        }
        private bool RemoveEnemyPiece(PieceModel Piece, ref Tuple<int, int> Pos)
        {
            int PossibleEnemyPosX = (Piece.X + Pos.Item1) / 2;
            int PossibleEnemyPosY = (Piece.Y + Pos.Item2) / 2;

            if (IsEnemy(Piece, _board[PossibleEnemyPosX][PossibleEnemyPosY]))
            {
                _board[PossibleEnemyPosX][PossibleEnemyPosY] = null;
                return true;
            }
            return false;
        }
        private bool IsEnemy( PieceModel Piece, PieceModel EnemyPiece)
        {
            if (EnemyPiece == null)
                return false;

            if((Piece.Type == PieceType.WhitePawn || Piece.Type == PieceType.WhiteKing) &&
          (EnemyPiece.Type == PieceType.BlackPawn || EnemyPiece.Type == PieceType.BlackKing))
                return true;

            if((Piece.Type == PieceType.BlackPawn || Piece.Type == PieceType.BlackKing) &&
          (EnemyPiece.Type == PieceType.WhitePawn || EnemyPiece.Type == PieceType.WhiteKing))
                return true;

            return false;
        }
        public bool SelectPiece(PieceModel Piece)
        {
            if (Piece == null)
                return false;

            if((_isWhiteTurn && (Piece.Type == PieceType.WhitePawn || Piece.Type == PieceType.WhiteKing)) || 
              (!_isWhiteTurn && (Piece.Type == PieceType.BlackPawn || Piece.Type == PieceType.BlackKing)))
                return true;
            return false;
        }
        #endregion

        public void MoveUpPiece(PieceModel Piece, ref Tuple<int, int> Pos)
        {
            int howManySquare = CanMoveUp(Piece, ref Pos);
            if (howManySquare == 1)
                MovePiece(Piece.X, Piece.Y, Pos.Item1, Pos.Item2);
            else
                if (howManySquare == 2)
                    if (RemoveEnemyPiece(Piece, ref Pos))
                        MovePiece(Piece.X, Piece.Y, Pos.Item1, Pos.Item2);
                
        }
        public void MoveDownPiece(PieceModel Piece, ref Tuple<int, int> Pos)
        {
            int howManySquare = CanMoveDown(Piece, ref Pos);
            if (howManySquare == 1)
                MovePiece(Piece.X, Piece.Y, Pos.Item1, Pos.Item2);
            else
                if (howManySquare == 2)
                    if (RemoveEnemyPiece(Piece, ref Pos))
                        MovePiece(Piece.X, Piece.Y, Pos.Item1, Pos.Item2);

        }
        private void ChangeTurn()
        {
            _isWhiteTurn = !_isWhiteTurn;
        }

        #endregion

    }
}
