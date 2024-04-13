using Checkers.ViewModels;
using Commands;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace Models
{
    class BoardCellModel : BaseNotification
    {
        public BoardCellModel()
        {}

        public int X { get; set; }
        public int Y { get; set; }
        public bool IsBlack { get; set; }
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
        public BoardCellModel(Action<object> func)
        {
            execute = func;
        }

        private string _cellBorderColor;
        public string CellBorderColor
        {
            get => _cellBorderColor;
            set
            {
                _cellBorderColor = value;
                NotifyPropertyChanged(nameof(CellBorderColor));
            }
        }

        private readonly Action<object> execute;

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

        private ICommand _onClickCommand;
        public ICommand OnClickCommand
        {
            get
            {
                if (_onClickCommand == null)
                    _onClickCommand = new RelayCommand(execute);
                return _onClickCommand;
            }
        }

    }

}
