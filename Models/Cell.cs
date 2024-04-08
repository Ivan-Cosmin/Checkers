using Checkers.ViewModels;

namespace Models
{
    class Cell : BaseNotification
    {
        private bool _isBlack;

        private bool _isOccupied;

        private CheckerTypes _content;


        public int Line { get; set; }

        public int Column { get; set; }

        public bool IsBlack
        {
            get { return _isBlack; }

            set
            {
                _isBlack = value;
                NotifyPropertyChanged("IsBlack");
            }
        }

        public bool IsOccupied
        {
            get { return _isOccupied; }

            set
            {
                _isOccupied = value;
                NotifyPropertyChanged("IsOccupied");
            }
        }

        public CheckerTypes Content
        {
            get { return _content; }

            set
            {
                _content = value;
                NotifyPropertyChanged("Content");
            }
        }

        public Cell(bool isBlack, int line, int column, CheckerTypes content = default)
        {
            Line = line;
            Column = column;
            IsBlack = isBlack;
            if (content != default)
            {
                IsOccupied = true;
            }
            else
            {
                IsOccupied = false;

            }
            Content = content;
        }
    }

}
