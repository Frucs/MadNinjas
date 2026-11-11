namespace Quizbattle
{
    public class Player
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public int CorrectAnswers { get; set; }
        public int CorrectAnswersInARow { get; set; }
        public Player(string name = "Johan", int points = 0, int correctAnswers = 0, int correctAnswersInARow = 0)
        {
            Name = name;
            Points = points;
            CorrectAnswers = correctAnswers;
            CorrectAnswersInARow = correctAnswersInARow;
        }
    }
}
