using Quizbattle;
using System;
using System.Collections.Generic;


Console.WriteLine("Hello, World!");

HighscoreService highscoreService = new HighscoreService();

List<HighscoreEntry> topplista = highscoreService.GetTopHighscores();

Console.WriteLine("=== TOPPLISTAN ===");

foreach (HighscoreEntry entry in topplista)
{
    Console.WriteLine($"{entry.Name} - {entry.Score} poäng");
}
