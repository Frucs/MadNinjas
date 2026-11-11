using Quizbattle; // Gruppens highscore-kod
using System;
using System.Collections.Generic;




ConsoleUI ui = new ConsoleUI(); // Skapar ett objekt av vår klass

int choice = ui.ShowMainMenu(); // Visar menyn och hämtar spelarens val

Console.WriteLine($"Du valde alternativ {choice}"); // Visar valet



HighscoreService highscoreService = new HighscoreService();

List<HighscoreEntry> topplista = highscoreService.GetTopHighscores();

Console.WriteLine("=== TOPPLISTAN ===");

foreach (HighscoreEntry entry in topplista)
{
    Console.WriteLine($"{entry.Name} - {entry.Score} poäng");
}
