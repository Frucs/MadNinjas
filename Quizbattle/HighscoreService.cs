using System;
using System.Collections.Generic;
using System.Text;
using Quizbattle.Repositories;
using System.Linq;

namespace Quizbattle
{
    internal class HighscoreService
    {
        private HighscoreRepository repository;
        private List<HighscoreEntry> highscores;
        public HighscoreService()
        {
            repository = new HighscoreRepository();
            highscores = repository.LoadHighscores();
        }
        public void Addhighscore(HighscoreEntry entry)
        {
            highscores.Add(entry);
            repository.SaveHighscores(highscores);
        }
        public List<HighscoreEntry> GetHighscores()
        {
            return highscores;
        }
        public List<HighscoreEntry> GetTopHighscores()
        {
            highscores.Sort((a, b) =>
            {
                int scoreComparison = b.Score.CompareTo(a.Score);

                if (scoreComparison != 0)
                {
                    return scoreComparison;
                }

                return b.CorrectAnswers.CompareTo(a.CorrectAnswers);
            });
            return highscores.Take(10).ToList();
        }
        public int GetRank(HighscoreEntry entry)
        {
            List<HighscoreEntry> toplist = GetTopHighscores();
            return toplist.FindIndex(x => x == entry) + 1;
        }



    }
}
