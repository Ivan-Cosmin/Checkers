using Checkers.ViewModels;
using Microsoft.Win32;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Checkers.Models
{
    internal class GameModel : BaseNotification
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
            _isWhiteTurn = false;
            MatchStarted = false;
        }
        public GameModel(bool MultipleJump, List<List<PieceModel>> board, bool isWhiteTurn)
        {
            _board = board;
            _multipleJump = MultipleJump;
            _isWhiteTurn = isWhiteTurn;
            MatchStarted = true;
        }

        #region Properties and Members
        private List<List<PieceModel>> _board;
        private bool _multipleJump;
        private static bool _isWhiteTurn;
        private static int _whiteCount;
        private static int _blackCount;
        private static readonly int _boardSize = 8;
        public bool MatchStarted;

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
        public bool MultipleJump
        {
            get { return _multipleJump; }
            set { _multipleJump = value;}
        }
        public int WhiteCount
        {
            get { return _whiteCount; }
            set { _whiteCount = value; }
        }
        public int BlackCount
        {
            get { return _blackCount; }
            set { _blackCount = value; }
        }
        #endregion

        #region Methods

        public bool MovePiece(PieceModel Piece , ref Tuple<int, int> Pos)
        {
            if (Piece == null)
                return false;

            //0 begin, 7 end of the board
            if(!Piece.IsKing() && 
              ((Piece.Type == PieceType.BlackPawn && Pos.Item1 == 0) ||
               (Piece.Type == PieceType.WhitePawn && Pos.Item1 == 7)))
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
            if (_multipleJump && Math.Abs(x1-x2) == 2 && HasChangeToJump2Square(Piece))
            { 
                return;
            }
            ChangeTurn();

        }

        public Tuple<string, int> Winner()
        {
            if(_whiteCount == 0 || (_isWhiteTurn && HasNoMove() == true))
                return new Tuple<string, int>("black" ,_blackCount); //black win
            else
            if(_blackCount == 0 || (!_isWhiteTurn && HasNoMove() == true))
                return new Tuple<string, int> ("white" , _whiteCount); //white win
            return null;
        }

        public PieceModel GetPiece(int x, int y)
        {
            return _board[x][y];
        }

        #region Boolean Methods
        private bool HasNoMove()
        {
            foreach (List<PieceModel> line in _board)
                foreach (PieceModel piece in line)
                {
                    if (piece == null)
                        continue;
                   if(_isWhiteTurn && (piece.Type == PieceType.WhitePawn || piece.Type == PieceType.WhiteKing))
                    {
                        if (HasChangeToJump2Square(piece) == true || HasChangeToJump1Square(piece) == true)
                            return false;
                    }
                    else
                    if(!_isWhiteTurn && (piece.Type == PieceType.BlackPawn || piece.Type == PieceType.BlackKing))
                    {
                        if (HasChangeToJump2Square(piece) == true || HasChangeToJump1Square(piece) == true)
                            return false;
                    }
                }
            return true;
        }
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
        private bool HasChangeToJump1Square(PieceModel Piece)
        {
            if (Piece.Type == PieceType.WhitePawn)
            {
                if (HasChangeToJumpDown1Square(Piece) == true)
                    return true;
            }
            else if (Piece.Type == PieceType.BlackPawn)
            {
                if (HasChangeToJumpUp1Square(Piece) == true)
                    return true;
            }
            else if (Piece.IsKing())
            {
                if (HasChangeToJumpDown1Square(Piece) == true || HasChangeToJumpUp1Square(Piece) == true)
                    return true;
            }

            return false;
        }
        private bool HasChangeToJumpUp1Square(PieceModel Piece)
        {
            if ( //leftUp
                    Piece.X - 1 >= 0 && Piece.X - 1 < _boardSize &&
                    Piece.Y - 1 >= 0 && Piece.Y - 1 < _boardSize &&
                    _board[Piece.X - 1][Piece.Y - 1] == null
                   )
                return true;

            if ( //rightUP
                Piece.X - 1 >= 0 && Piece.X - 1 < _boardSize &&
                Piece.Y + 1 >= 0 && Piece.Y + 1 < _boardSize &&
                _board[Piece.X - 1][Piece.Y + 1] == null
               )
                return true;

            return false;
        }
        private bool HasChangeToJumpDown1Square(PieceModel Piece)
        {
            if ( //leftDown
                     Piece.X + 1 >= 0 && Piece.X + 1 < _boardSize &&
                     Piece.Y - 1 >= 0 && Piece.Y - 1 < _boardSize &&
                     _board[Piece.X + 1][Piece.Y - 1] == null
                    )
                return true;

            if ( //rightDown
                Piece.X + 1 >= 0 && Piece.X + 1 < _boardSize &&
                Piece.Y + 1 >= 0 && Piece.Y + 1 < _boardSize &&
                _board[Piece.X + 1][Piece.Y + 1] == null
               )
                return true;
            return false;
        }
        private bool HasChangeToJumpDown2Square( PieceModel Piece)
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
        private bool HasChangeToJumpUp2Square(PieceModel Piece)
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
        private bool HasChangeToJump2Square(PieceModel Piece)
        {
            if (Piece.Type == PieceType.WhitePawn)
            {
                if(HasChangeToJumpDown2Square( Piece) == true)
                    return true;
            }
            else if (Piece.Type == PieceType.BlackPawn)
            {
                if(HasChangeToJumpUp2Square( Piece) == true)
                    return true;
            }
            else if(Piece.IsKing())
            {
                if(HasChangeToJumpDown2Square( Piece) == true || HasChangeToJumpUp2Square( Piece) == true)
                    return true;
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
                if (Piece.Type == PieceType.WhitePawn || Piece.Type == PieceType.WhiteKing)
                    _blackCount--;
                else
                    _whiteCount--;
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
              (!_isWhiteTurn && (Piece.Type == PieceType.BlackPawn || Piece.Type == PieceType.BlackKing)) && Winner() == null)
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
            if(!MatchStarted)
                MatchStarted = true;
        }
        public void CounterPieces()
        {
            _whiteCount = 0;
            _blackCount = 0;
            foreach (List<PieceModel> line in _board)
                foreach (PieceModel piece in line)
                {
                    if (piece == null)
                        continue;
                    if (piece.Type == PieceType.WhitePawn || piece.Type == PieceType.WhiteKing)
                        _whiteCount++;
                    else
                        _blackCount++;
                }
        }
        #endregion

    }
}
