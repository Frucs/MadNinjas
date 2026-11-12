using Quizbattle;

ConsoleUI ui = new ConsoleUI();

while (true)
{
    int choice = ui.ShowMainMenu();

    switch (choice)
    {
        case 1:
            var (playerNames, rounds) = ui.ReadGameSetup();
            Game game = new Game
            {
                Rounds = rounds,
                PlayerAmount = playerNames.Count
            };

            foreach (string name in playerNames)
            {
                game.AddPlayer(new Player(name));
            }

            game.StartGame();
            break;

        case 2:
            HighscoreService highscoreService = new HighscoreService();
            List<HighscoreEntry> topplista = highscoreService.GetTopHighscores();

            Console.WriteLine("=== TOPPLISTAN ===");
            foreach (HighscoreEntry entry in topplista)
            {
                Console.WriteLine($"{entry.Name} - {entry.Score} poäng");
            }
            break;

        case 3:
            return;
    }

    Console.WriteLine();
}
