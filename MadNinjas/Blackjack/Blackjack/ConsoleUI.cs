namespace Blackjack
{
    internal class ConsoleUI
    {
        public void Run()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("=== Blackjack ===");
                Console.WriteLine("1. Spela");
                Console.WriteLine("2. Topplista");
                Console.WriteLine("3. Regler");
                Console.WriteLine("4. Avsluta");
                Console.Write("Välj 1–4: ");

                string? choice = Console.ReadLine();

                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        string playerName = ReadPlayerName();
                        Game game = new Game(); // Skapar ett spel.
                        game.StartGame(playerName);       // Startar spelet.
                        break;

                    case "2":
                        ScoreBoard board = new ScoreBoard();
                        board.ShowTopList();
                        break;

                    case "3":
                        Console.WriteLine("Kom närmare 21 än dealern utan att gå över.");
                        Console.WriteLine("Du kan dra kort eller stanna.");
                        Console.WriteLine("Dealern drar kort tills summan är minst 17.\n");
                        break;

                    case "4":
                        running = false;
                        Console.WriteLine("Tack för att du spelade!");
                        break;

                    default:
                        Console.WriteLine("Felaktigt val. Ange en siffra mellan 1 och 4.\n");
                        break;
                }
            }
        }
        public string ReadPlayerName()
        {
            while (true)
            {
                Console.Write("Ditt namn: ");
                string? name = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name.Trim();
                }

                Console.WriteLine("Namnet får inte vara tomt. Försök igen.");
            }
        }
    }
}