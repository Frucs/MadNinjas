using System;
using System.Collections.Generic;
using System.Text;

namespace Quizbattle
{
    public class HighscoreEntry
    { // Properties
        public string Name { get; set; } = "";
        public int Score { get; set; }
        public int CorrectAnswers { get; set; }
        public DateTime Date { get; set; }
        // Konstructor 
        public HighscoreEntry(string name, int score, int correctAnswers, DateTime date)
        {
            Name = name;
            Score = score;
            CorrectAnswers = correctAnswers;
            Date = date;
        }


    }
        
         
        
}
