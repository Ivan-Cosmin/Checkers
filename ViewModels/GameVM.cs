using Checkers.Models;
using Models;
using Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Checkers.ViewModels
{
    internal class GameVM
    {
        public GameVM()
        {
            _game = new GameModel(false);
            InitBoard();
            UpdateBoard();
        }
        private GameModel _game;
        public ObservableCollection<ObservableCollection<BoardCellModel>> GameBoard { get; set; }

        private readonly string _normalBorderColor = "#DDD4CE";
        private readonly string _whiteSquareImagePath = "..\\..\\Resources\\Images\\white_square.jpg";
        private readonly string _blackSquareImagePath = "..\\..\\Resources\\Images\\black_square.jpg";
        private readonly string _whitePawnImagePath = "..\\..\\Resources\\Images\\white_pawn.png";
        private readonly string _whiteKingImagePath = "..\\..\\Resources\\Images\\white_king.png";
        private readonly string _blackPawnImagePath = "..\\..\\Resources\\Images\\black_pawn.png";
        private readonly string _blackKingImagePath = "..\\..\\Resources\\Images\\black_king.png";
        private readonly string _selectedPieceColor = "Red";

        
        // CellBorderColor property
        private SolidColorBrush _cellBorderColor = new SolidColorBrush(Colors.Black);
        private bool _multipleJump;
        public bool MultipleJump
        {
            get { return _multipleJump; }
            set { _multipleJump = value; }
        }

        public SolidColorBrush CellBorderColor
        {
            get { return _cellBorderColor; }
            set { _cellBorderColor = value; }
        }
        public BoardCellModel SelectedPiece { get; set; }

        private void InitBoard()
        {
            // Create the board
            GameBoard = new ObservableCollection<ObservableCollection<BoardCellModel>>();
            for (int line_index = 0; line_index < _game.BoardSize; line_index++)
            {
                GameBoard.Add(new ObservableCollection<BoardCellModel>());
                for (int column_index = 0; column_index < _game.BoardSize; column_index++)
                {
                    BoardCellModel cell = new BoardCellModel(ClickCommand)
                    { 
                        X = line_index, 
                        Y = column_index,
                        BackgroundImage = (line_index + column_index) % 2 == 0 ? GetImage(_whiteSquareImagePath) : GetImage(_blackSquareImagePath),
                        IsBlack = (line_index + column_index) % 2 == 1,
                        PieceImage = null,
                        CellBorderColor = _normalBorderColor
                    };

                    GameBoard[line_index].Add(cell);
                }
            }
        }

        private void UpdateBoard()
        {   
            List<List<PieceModel>> board = _game.Board;
            int boardSize = _game.BoardSize;
            ImageSource image = null;

            for (int line_index = 0; line_index < boardSize; line_index++)
                for (int column_index = 0; column_index < boardSize; column_index++)
                {
                    PieceModel piece = board[line_index][column_index];

                    if (piece == null)
                        image = null;

                    else if (!piece.IsKing())
                       image = GetImage(piece.Type == PieceType.WhitePawn ?
                            _whitePawnImagePath : _blackPawnImagePath);

                    else if (piece.IsKing())
                        image = GetImage(piece.Type == PieceType.WhiteKing ?
                            _whiteKingImagePath : _blackKingImagePath);

                    GameBoard[line_index][column_index].PieceImage = image;
                }
        }

        private ImageSource GetImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return null;
            return new ImageSourceConverter()
                .ConvertFromString(System.IO.Path.GetFullPath(imagePath)) as ImageSource;
        }

        private void ClickCommand(object parameter)
        {
            BoardCellModel clickedCell = parameter as BoardCellModel;
            PieceModel currentPiece = _game.GetPiece(clickedCell.X, clickedCell.Y);

            if (currentPiece == null)
            {
                if (SelectedPiece != null && clickedCell.IsBlack)
                {
                    PieceModel PrevPiece = _game.GetPiece(SelectedPiece.X, SelectedPiece.Y);
                    MovePiece(PrevPiece, new Tuple<int, int>(clickedCell.X, clickedCell.Y));
                    ResetSelectedPiece();
                }
            }
            else
            {
                if (_game.SelectPiece(currentPiece))
                {
                    if(SelectedPiece != null)
                        ResetSelectedPiece();

                    SelectedPiece = clickedCell;
                    SelectedPiece.CellBorderColor = _selectedPieceColor;
                }
            }
        }

        private void MovePiece(PieceModel Piece, Tuple<int, int> Pos)
        {
            if (SelectedPiece != null && _game.MovePiece(Piece, ref Pos))
                UpdateBoard();
        }

        private void ResetSelectedPiece()
        {
            if (SelectedPiece != null)
            {
                SelectedPiece.CellBorderColor = _normalBorderColor;
                SelectedPiece = null;
            }
        }

    }
}
