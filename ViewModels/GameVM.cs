using Checkers.Models;
using Models;
using Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using Checkers.View;
using Microsoft.Win32;
using System.Xml;

namespace Checkers.ViewModels
{
    internal class GameVM : BaseNotification
    {
        #region Constructors
        public GameVM()
        {
            InitNewGame();
            ReadMultipleJump();
            SaveGameCommand = new RelayCommand(SaveGame);
            LoadGameCommand = new RelayCommand(LoadGame);
            NewGameCommand = new RelayCommand(NewGame);
            DisplayInfoCommand = new RelayCommand(DisplayInfo);
            DisplayStatisticsCommand = new RelayCommand(DisplayStatistics);
            ToggleMultipleJumps = new RelayCommand(ToggleMultipleJumpsCommand, IsMultipleJumpsCheckable);
        }
        public GameVM(GameModel game)
        {
            _game = game;
            InitBoard();
            UpdateBoard();
            SaveGameCommand = new RelayCommand(SaveGame);
            LoadGameCommand = new RelayCommand(LoadGame);
            NewGameCommand = new RelayCommand(NewGame);
            DisplayInfoCommand = new RelayCommand(DisplayInfo);
            DisplayStatisticsCommand = new RelayCommand(DisplayStatistics);
            ToggleMultipleJumps = new RelayCommand(ToggleMultipleJumpsCommand, IsMultipleJumpsCheckable);
        }
        #endregion 

        #region Members
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
        private static string _message = null;
        public static bool MultipleJump { get; set; }
        public string PlayerTurn { get; set;}
        public string WhiteCount { get; set; }
        public string BlackCount { get; set; }

        private SolidColorBrush _cellBorderColor = new SolidColorBrush(Colors.Black);
        public SolidColorBrush CellBorderColor
        {
            get { return _cellBorderColor; }
            set { _cellBorderColor = value; }
        }
        public BoardCellModel SelectedPiece { get; set; }
        #endregion

        #region Methods
        private void InitBoard()
        {
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
            PlayerTurn = _game.IsWhiteTurn ? "White turn" : "Black turn";
            NotifyPropertyChanged("PlayerTurn");
            _game.CounterPieces();
            WhiteCount = _game.WhiteCount.ToString();
            NotifyPropertyChanged("WhiteCount");
            BlackCount = _game.BlackCount.ToString();
            NotifyPropertyChanged("BlackCount");

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
            {
                UpdateBoard();
                if (_game.Winner() != null)
                {
                    Tuple<string, int> winner = _game.Winner();
                    _message = "The winner is " + winner.Item1 + " with " + winner.Item2.ToString() + " pieces";
                    MessageBox.Show(_message);
                    SaveStatistics(winner);
                }
            }
        }

        private void SaveStatistics(Tuple<string, int> winner)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("Statistics.xml");

                XmlNode black = xmlDoc.SelectSingleNode("/Players/Player[@Name='Black']");
                XmlNode white = xmlDoc.SelectSingleNode("/Players/Player[@Name='White']");

                if (black != null)
                {
                    int blackGamesWon = int.Parse(black.SelectSingleNode("GamesWon").InnerText);
                    int blackPieces = int.Parse(black.SelectSingleNode("MaxPiecesLeft").InnerText);

                    blackGamesWon += 1; 
                    if(winner.Item2 >= blackPieces)
                        blackPieces = winner.Item2; 

                    black.SelectSingleNode("GamesWon").InnerText = blackGamesWon.ToString();
                    black.SelectSingleNode("MaxPiecesLeft").InnerText = blackPieces.ToString();
                }

                if (white != null)
                {
                    int whiteGamesWon = int.Parse(white.SelectSingleNode("GamesWon").InnerText);
                    int whitePieces = int.Parse(white.SelectSingleNode("MaxPiecesLeft").InnerText);

                    whiteGamesWon += 1;
                    if (winner.Item2 >= whitePieces)
                        whitePieces = winner.Item2;

                    white.SelectSingleNode("GamesWon").InnerText = whiteGamesWon.ToString();
                    white.SelectSingleNode("MaxPiecesLeft").InnerText = whitePieces.ToString();
                }

                xmlDoc.Save("Statistics.xml");

                MessageBox.Show("Fișierul XML a fost actualizat cu succes.", "Actualizare reușită", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la actualizarea fișierului XML: " + ex.Message, "Eroare la actualizare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ResetSelectedPiece()
        {
            if (SelectedPiece != null)
            {
                SelectedPiece.CellBorderColor = _normalBorderColor;
                SelectedPiece = null;
            }
        }
        public void WriteMultipleJump()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            using (XmlWriter writer = XmlWriter.Create("..\\..\\Resources\\Settings\\MultipleJump.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("MultipleJump");
                writer.WriteElementString("Value", _game.MultipleJump.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
        public void ReadMultipleJump()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("..\\..\\Resources\\Settings\\MultipleJump.xml");
            XmlNode multipleJumpNode = doc.SelectSingleNode("MultipleJump");
            MultipleJump = bool.Parse(multipleJumpNode.SelectSingleNode("Value").InnerText);
        }
        #endregion

        #region Commands
        public ICommand SaveGameCommand { get; set; }
        public ICommand LoadGameCommand { get; set; }
        public ICommand NewGameCommand { get; set; }
        public ICommand DisplayInfoCommand { get; set; }
        public ICommand DisplayStatisticsCommand { get; set; }
        public ICommand ToggleMultipleJumps { get; set; }
        #endregion

        #region Command Methods
        private void InitNewGame()
        {
            _game = new GameModel(MultipleJump);
            InitBoard();
            UpdateBoard();
        }
        public void NewGame(object parameter)
        {
            MessageBoxResult result = MessageBox.Show(
            "Do you want to start a new game?",
            "New Game Settings",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Window _current = Application.Current.Windows[0];
                GameWindow gameWindow = new GameWindow();
                gameWindow.Show();
                _current.Close();
            }
        }
        public void SaveGame(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Fișiere XML (*.xml)|*.xml|Toate fișierele (*.*)|*.*";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Title = "Save the game";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.NewLineOnAttributes = true;
                    using (XmlWriter writer = XmlWriter.Create(saveFileDialog.FileName, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Game");
                        writer.WriteElementString("IsWhiteTurn", _game.IsWhiteTurn.ToString());
                        writer.WriteElementString("MultipleJump", _game.MultipleJump.ToString());
                        writer.WriteStartElement("Board");
                        for (int i = 0; i < _game.BoardSize; i++)
                        {
                            for (int j = 0; j < _game.BoardSize; j++)
                            {
                                if (_game.Board[i][j] != null)
                                {
                                    writer.WriteStartElement("Piece");
                                    writer.WriteElementString("X", i.ToString());
                                    writer.WriteElementString("Y", j.ToString());
                                    writer.WriteElementString("Type", _game.Board[i][j].Type.ToString());
                                    writer.WriteEndElement();
                                }
                            }
                        }
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }

                    MessageBox.Show("Jocul a fost salvat cu succes.", "Salvare cu succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A apărut o eroare la salvarea jocului: " + ex.Message, "Eroare la salvare", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void LoadGame(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fișiere XML (*.xml)|*.xml|Toate fișierele (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Title = "Load the game";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(openFileDialog.FileName);

                    XmlNode gameNode = doc.SelectSingleNode("Game");
                    bool IsWhiteTurn = bool.Parse(gameNode.SelectSingleNode("IsWhiteTurn").InnerText);
                    bool MultipleJump = bool.Parse(gameNode.SelectSingleNode("MultipleJump").InnerText);

                    List<List<PieceModel>> board = new List<List<PieceModel>>();
                    for (int i = 0; i < _game.BoardSize; i++)
                    {
                        board.Add(new List<PieceModel>());
                        for (int j = 0; j < _game.BoardSize; j++)
                        {
                            board[i].Add(null);
                        }
                    }

                    XmlNode boardNode = gameNode.SelectSingleNode("Board");
                    foreach (XmlNode pieceNode in boardNode.SelectNodes("Piece"))
                    {
                        int x = int.Parse(pieceNode.SelectSingleNode("X").InnerText);
                        int y = int.Parse(pieceNode.SelectSingleNode("Y").InnerText);
                        PieceType type = (PieceType)Enum.Parse(typeof(PieceType), pieceNode.SelectSingleNode("Type").InnerText);
                        board[x][y] = new PieceModel(x,y,type);
                    }

                    Window _current = Application.Current.Windows[0];
                    GameWindow gameWindow = new GameWindow();
                    gameWindow.DataContext = new GameVM(new GameModel(MultipleJump, board, IsWhiteTurn));
                    gameWindow.Show();
                    _current.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("A apărut o eroare la încărcarea jocului: " + ex.Message, "Eroare la încărcare", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void ToggleMultipleJumpsCommand(object parameter)
        {
            _game.MultipleJump = MultipleJump;
            WriteMultipleJump();
            MessageBox.Show("Multiple jumps are now " + (MultipleJump ? "enabled" : "disabled"));
        }
        public bool IsMultipleJumpsCheckable(object parameter)
        {
            return !_game.MatchStarted;
        }
        private void DisplayStatistics(object obj)
        {
            Statistics statisticsWindow = new Statistics();
            statisticsWindow.Show();
        }
        private void DisplayInfo(object obj)
        {
            About aboutWindow = new About();
            aboutWindow.Show();
        }
        #endregion

    }
}
