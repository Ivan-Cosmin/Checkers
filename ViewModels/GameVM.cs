using Checkers.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Checkers.ViewModels
{
    internal class GameVM
    {
        public GameVM()
        {
            _game = new GameModel();
            InitBoard();
        }
        private GameModel _game;
        public ObservableCollection<ObservableCollection<BoardCellModel>> GameBoard { get; set; }

        private readonly string _whiteSquareImagePath = "..\\..\\Resources\\Images\\white_square.jpg";
        private readonly string _blackSquareImagePath = "..\\..\\Resources\\Images\\black_square.jpg";
        private readonly string _whitePawnImagePath = "..\\..\\Resources\\Images\\white_pawn.png";
        private readonly string _redPawnImagePath = "..\\..\\Resources\\Images\\red_pawn.png";

        private void InitBoard()
        {
            GameBoard = new ObservableCollection<ObservableCollection<BoardCellModel>>();
            for (int i_index = 0; i_index < _game.BoardSize; i_index++)
            {
                GameBoard.Add(new ObservableCollection<BoardCellModel>());
                for (int j_index = 0; j_index < _game.BoardSize; j_index++)
                {
                    GameBoard[i_index].Add(new BoardCellModel());
                    GameBoard[i_index][j_index].X = i_index;
                    GameBoard[i_index][j_index].Y = j_index;
                    GameBoard[i_index][j_index].BackgroundImage = (i_index + j_index) % 2 == 0 ? GetImage(_whiteSquareImagePath): GetImage(_blackSquareImagePath);
                    GameBoard[i_index][j_index].PieceImage = null;
                }
            }
        }

        private ImageSource GetImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return null;
            return new ImageSourceConverter()
                .ConvertFromString(System.IO.Path.GetFullPath(imagePath)) as ImageSource;
        }

    }
}
