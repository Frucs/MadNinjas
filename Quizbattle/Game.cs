namespace Quizbattle
{
    public class Game
    {
        public int Rounds { get; set; }
        public int PlayerAmount { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public Game()
        {

        }
        public void AddPlayer(Player player)
        {
            Players.Add(player);
            Console.WriteLine("Player " + player.Name + " added.");
        }
        public void NewGame()
        {
            Console.WriteLine("Spelet startar...\n" +
                "Vill du starta online klienten? (s/f)");
            string answer = Console.ReadLine() ?? "";
            if (answer.ToLower().Substring(0,1) == "s")
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
            Console.WriteLine("Coming soon...");
        }
        public void StartGame()
        {
            Console.WriteLine("Coming soon...");
        }
    }
}