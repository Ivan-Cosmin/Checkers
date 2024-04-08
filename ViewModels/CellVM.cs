using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Checkers.ViewModels
{
    internal class CellVM
    {
        public CellVM(Cell cell)
        {
            SimpleCell = new Cell(cell.IsBlack, cell.Line, cell.Column, cell.Content);
        }
        public Cell SimpleCell { get; set;}

        public bool IsBlack
        {
            get { return SimpleCell.IsBlack; }
        }

    }
}
