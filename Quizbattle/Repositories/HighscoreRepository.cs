using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;

namespace Quizbattle.Repositories   
{
    public class HighscoreRepository
    {
        // sparar alla resultat i en JSON-fil.
        public void SaveHighscores(List<HighscoreEntry> highscores)
        { 

            Directory.CreateDirectory("data");
        
            string json = JsonSerializer.Serialize(highscores);
             File.WriteAllText("data/highscores.json", json);
        }
        // Hämtar  alla resultat från JSON-filen.
        public List<HighscoreEntry> LoadHighscores()
        {
           if (!File.Exists("data/highscores.json"))
            {
                return new List<HighscoreEntry>();
            }
            try
            {
                string json = File.ReadAllText("data/highscores.json");
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<HighscoreEntry>();
                } 

                List<HighscoreEntry>? result = JsonSerializer.Deserialize<List<HighscoreEntry>>(json);
                if (result == null)
                {
                    return new List<HighscoreEntry>();
                }
                return result;

            }   
                catch (JsonException)
                {
                return new List<HighscoreEntry>(); 
                }
            
        }
        

    }
}
