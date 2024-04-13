using Checkers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Models
{
    internal class PieceModel
    {
        public PieceModel()
        {
        }
        public PieceModel(int x, int y, PieceType type)
        {
            X = x;
            Y = y;
            Type = type;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public PieceType Type { get; set; }

        public bool IsKing()
        {
            if (Type == PieceType.WhiteKing || Type == PieceType.BlackKing)
                return true;
            return false;
        }
        public void ChangeInKing()
        {
            if (Type == PieceType.WhitePawn)
                Type = PieceType.WhiteKing;

            if (Type == PieceType.BlackPawn)
                Type = PieceType.BlackKing;
        }
    }
}
