using System;
using System.Collections.Generic;
using System.Text;
using Quizbattle.Repositories;
using System.Linq;

namespace Quizbattle
{
    // Hanterar logiken för topplistan.
    internal class HighscoreService
    {
        // Skapar kopplingen till Repository som hanterar filen.
        private HighscoreRepository repository;
        //Lista med alla sparade resultat.
        private List<HighscoreEntry> highscores;
        // Konstructor - körs när en ny HighscoreService skapas.
        public HighscoreService()
        {
            // Skapar ett nytt repository.
            repository = new HighscoreRepository();
            // Hämtar  tidigare sparade resultat från JSON-filen
            highscores = repository.LoadHighscores();
        }
        // Lägger till ett nytt resultat i topplistan och sparar det i JSON-filen.
        public void Addhighscore(HighscoreEntry entry)
        {
            highscores.Add(entry);
            repository.SaveHighscores(highscores);
        }
        // Hämtar alla resultat från listan.
        public List<HighscoreEntry> GetHighscores()
        {
            return highscores;
        }
        //Sorterar resultat och hämtar de 10 bästa spelnarna.
        public List<HighscoreEntry> GetTopHighscores()
        {
            // Sorterar först efter högst poäng.
            highscores.Sort((a, b) =>
            {
                int scoreComparison = b.Score.CompareTo(a.Score);

                // Om poängen skiljer sig används poängen för sortering.
                if (scoreComparison != 0)
                {
                    return scoreComparison;
                }
                // vid lika poäng jämförs antalet rätta svar.
                return b.CorrectAnswers.CompareTo(a.CorrectAnswers);
            });
            // Returnerar de 10 bästa resultaten.
            return highscores.Take(10).ToList();
        }
        // Räknar ut spelarens placering i topplistan baserat på deras resultat.
        public int GetRank(HighscoreEntry entry)
        {
            // Hämtar de 10 bästa resultaten och söker efter spelarens resultat i listan.

            List<HighscoreEntry> toplist = GetTopHighscores();
            // Letar efter spelarens plats och lägger till 1 eftersom listan är 0.
            return toplist.FindIndex(x => x == entry) + 1;
        }



    }
}
