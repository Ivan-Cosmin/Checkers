using Checkers.ViewModels;
using System.Windows.Media;

namespace Models
{
    class BoardCellModel : BaseNotification
    {
        public BoardCellModel()
        {}

        public int X { get; set; }
        public int Y { get; set; }
        private ImageSource _backgroundImage;
        public ImageSource BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                _backgroundImage = value;
                NotifyPropertyChanged("BackgroundImage");
            }
        }

        private ImageSource _pieceImage;
        public ImageSource PieceImage
        {
            get { return _pieceImage; }
            set
            {
                _pieceImage = value;
                NotifyPropertyChanged("PieceImage");
            }
        }
    }

}
