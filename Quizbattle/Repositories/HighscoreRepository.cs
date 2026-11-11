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
        public void SaveHighscores(List<HighscoreEntry> highscores) 
        {
            string json = JsonSerializer.Serialize(highscores);
             File.WriteAllText("highscores.json", json);
        }
        public List<HighscoreEntry> LoadHighscores()
        {
           if (!File.Exists("highscores.json"))
            {
                return new List<HighscoreEntry>();
            }
            try
            {
                string json = File.ReadAllText("highscores.json");
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
