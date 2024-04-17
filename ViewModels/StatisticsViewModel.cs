using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.Services;

namespace Checkers.ViewModels
{
    internal class StatisticsViewModel
    {
        List<int> ShowStatistics;
        FileServices fileStatistics = new FileServices();
        public StatisticsViewModel() 
        {
            fileStatistics = new FileServices();
            ShowStatistics = fileStatistics.GetStatistics();
        }

        public int MaxPiecesWonWhite => ShowStatistics[0];
        public int MaxPiecesWonBlack => ShowStatistics[1];
        public int NumberOfGamesWhite => ShowStatistics[2];
        public int NumberOfGamesBlack => ShowStatistics[3];
    }
}
