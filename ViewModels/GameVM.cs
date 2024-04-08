using Checkers.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.ViewModels
{
    internal class GameVM
    {
        public GameVM(GameModel game)
        {
            ObservableCollection<ObservableCollection<CellVM>> board = CellBoardToCellVMBoard(game.Board);
            
            GameBoard = board;
        }

        private ObservableCollection<ObservableCollection<CellVM>> CellBoardToCellVMBoard(List<List<Cell>> board)
        {
            ObservableCollection<ObservableCollection<CellVM>> cellVMBoard = new ObservableCollection<ObservableCollection<CellVM>>();

            foreach (List<Cell> row in board)
            {
                ObservableCollection<CellVM> cellVMRow = new ObservableCollection<CellVM>();
                foreach (Cell cell in row)
                {
                    cellVMRow.Add(new CellVM(cell));
                }
                cellVMBoard.Add(cellVMRow);
            }

            return cellVMBoard;
        }
        public ObservableCollection<ObservableCollection<CellVM>> GameBoard { get; set; }
    }
}
