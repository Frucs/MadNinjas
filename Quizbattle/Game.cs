namespace Quizbattle
{
    public class Game
    {
        public int Rounds { get; set; }
        public int CurrentRound { get; set; } = 0;
        public int PlayerAmount { get; set; }
        public ConsoleUI ui;
        public List<Player> Players { get; set; } = new List<Player>();
        private HighscoreService highscoreService { get; set; } = new HighscoreService();
        public Game()
        {

        }
        // Console UI använder denna metod för att lägga til Spelare före spelet startas
        public void AddPlayer(Player player)
        {
            Players.Add(player);
            Console.WriteLine("Spelare " + player.Name + " har joinat.");
        }
        // NewGame används för nuvarande inte, men kanske kommer sedan!
        public void NewGame()
        {
            Console.WriteLine("Spelet startar...\n" +
                "Vill du starta online klienten? (s/f)");
            string answer = Console.ReadLine() ?? "";
            if (answer.ToLower().Substring(0, 1) == "s")
            {
                OnlineClient();
            }
            else if (answer.ToLower().Substring(0, 1) == "f")
            {
                Console.WriteLine("Spelet startar...");
            }
        }
        public void OnlineClient()
        {
            Console.WriteLine("Online spelet startar...");
            Console.WriteLine("Kommer snart...");
        }
        public void StartGame()
        {
            Console.WriteLine("Spelet Startar...");
            GameLoop();
        }

        void GameLoop()
        {
            QuestionLoader Loader = new QuestionLoader();
            string path = Path.Combine(
                AppContext.BaseDirectory,
                "data",
                "questions.json"
            );
            Questionbank bank = new Questionbank(Loader.Load(path));
            Random random = new Random();
            while (CurrentRound < Rounds)
            {
                CurrentRound += 1;
                Console.WriteLine("\n=== Runda " + CurrentRound + " av " + Rounds + " ===");
                foreach (Player player in Players)
                {
                    Console.WriteLine("Det är " + player.Name + "s tur.");
                    List<string> categoryNames = new List<string>();
                    List<int> categoryNumbers = new List<int>();
                    foreach(Category cat in bank._categories)
                    {
                        categoryNames.Add(cat.Name);
                        categoryNumbers.Add(cat.Questions.Count);
                    }
                    int nextCatIndex = ui.ReadCategoryChoice(categoryNames, categoryNumbers);
                    if (nextCatIndex == -1) { nextCatIndex = random.Next(0, categoryNames.Count); }
                    IQuestion question = bank.DrawQuestion(bank._categories[nextCatIndex]);
                    string answer = "";
                    // Display Frågor
                    if (question is MultipleChoiceQuestion multiplechoice)
                    {
                        answer = ui.ReadMultipleChoiceAnswer(bank._categories[nextCatIndex].Name, multiplechoice.Text, multiplechoice.Options);
                    }
                    if (question is TrueFalseQuestion truefalsechoice)
                    {
                        answer = ui.ReadTrueFalseAnswer(bank._categories[nextCatIndex].Name, truefalsechoice.Text);
                    }
                    if (question.CheckAnswer(answer))
                    {
                        Console.WriteLine("Du hade rätt!!");
                        int roundPoints = GetScore(question, player.CorrectAnswersInARow);
                        player.CorrectAnswers += 1;
                        player.Points += roundPoints;
                        player.CorrectAnswersInARow += 1;
                        if (player.CorrectAnswersInARow > 1)
                        {
                            Console.Write("Snyggt! " + player.CorrectAnswersInARow + " rätta svar i rad!! ");
                        }
                        Console.WriteLine("Du fick " + roundPoints + " poäng! Du har nu Totalt: " + player.Points + "\n");
                    }
                    else
                    {
                        player.CorrectAnswersInARow = 0;
                        if (question is MultipleChoiceQuestion multiplechoice2)
                        {
                            Console.WriteLine("Du hade fel!! Svaret var: " + multiplechoice2.Options[multiplechoice2.CorrectIndex] + "\n");
                        }
                        if (question is TrueFalseQuestion truefalsechoice2)
                        {
                            if (truefalsechoice2.CorrectAnswer)
                            {
                                Console.WriteLine("Du hade fel!! Svaret var Sant\n");
                            }
                            else
                            {
                                Console.WriteLine("Du hade fel!! Svaret var Falskt\n");
                            }
                        }
                    }
                }
            }
            // Lägger till spelarna i topplistan
            DateTime TimeToday = DateTime.Today;
            foreach (Player player in Players)
            {
                HighscoreEntry playerEntry = new HighscoreEntry(player.Name, player.Points, player.CorrectAnswers, TimeToday);
                highscoreService.Addhighscore(playerEntry);
            }
            // Visar topplistan
            List<HighscoreEntry> highscore = highscoreService.GetTopHighscores();
            Console.WriteLine("=== TOPPLISTAN ===");
            foreach (HighscoreEntry entry in highscore)
            {
                // Visar namn, poäng, antal rätta svar och datum.
                Console.WriteLine($"{entry.Name} - {entry.Score} poäng - {entry.CorrectAnswers} rätta svar - {entry.Date:yyyy-MM-dd}");
            }
        }
        int GetScore(IQuestion question, int inARow)
        {
            if (question.Difficulty is Difficulty.Easy)
            {
                return 10 + (inARow * 5);
            }
            if (question.Difficulty is Difficulty.Medium)
            {
                return 20 + (inARow * 5);
            }
            if (question.Difficulty is Difficulty.Hard)
            {
                return 30 + (inARow * 5);
            }
            return 0;
        }
    }
}