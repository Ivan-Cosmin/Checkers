using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Checkers.Models;
using Checkers.View;
using Checkers.ViewModels;

namespace Checkers.Services
{
    internal class FileServices
    {
        public void SaveGame(GameModel Game)
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
                        writer.WriteElementString("IsWhiteTurn", Game.IsWhiteTurn.ToString());
                        writer.WriteElementString("MultipleJump", Game.MultipleJump.ToString());
                        writer.WriteStartElement("Board");
                        for (int i = 0; i < Game.BoardSize; i++)
                        {
                            for (int j = 0; j < Game.BoardSize; j++)
                            {
                                if (Game.Board[i][j] != null)
                                {
                                    writer.WriteStartElement("Piece");
                                    writer.WriteElementString("X", i.ToString());
                                    writer.WriteElementString("Y", j.ToString());
                                    writer.WriteElementString("Type", Game.Board[i][j].Type.ToString());
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

        public void WriteMultipleJump(GameModel Game)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            using (XmlWriter writer = XmlWriter.Create("..\\..\\Resources\\Settings\\MultipleJump.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("MultipleJump");
                writer.WriteElementString("Value", Game.MultipleJump.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
        public bool ReadMultipleJump()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("..\\..\\Resources\\Settings\\MultipleJump.xml");
            XmlNode multipleJumpNode = doc.SelectSingleNode("MultipleJump");
            bool MultipleJump = bool.Parse(multipleJumpNode.SelectSingleNode("Value").InnerText);
            return MultipleJump;
        }

        public void LoadGame(GameModel Game)
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
                    for (int i = 0; i < Game.BoardSize; i++)
                    {
                        board.Add(new List<PieceModel>());
                        for (int j = 0; j < Game.BoardSize; j++)
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
                        board[x][y] = new PieceModel(x, y, type);
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

        public void SaveStatistics(Tuple<string, int> winner)
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
                    if (winner.Item2 >= blackPieces)
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

        internal List<int> GetStatistics()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Statistics.xml");

            XmlNode black = xmlDoc.SelectSingleNode("/Players/Player[@Name='Black']");
            XmlNode white = xmlDoc.SelectSingleNode("/Players/Player[@Name='White']");
            int blackGamesWon = int.Parse(black.SelectSingleNode("GamesWon").InnerText);
            int blackPieces = int.Parse(black.SelectSingleNode("MaxPiecesLeft").InnerText);
            int whiteGamesWon = int.Parse(white.SelectSingleNode("GamesWon").InnerText);
            int whitePieces = int.Parse(white.SelectSingleNode("MaxPiecesLeft").InnerText);

            return new List<int>
            {
               whitePieces,
               blackPieces,
               whiteGamesWon,
               blackGamesWon
            };
        }
    }
}
