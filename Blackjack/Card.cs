namespace Blackjack
{
    public class Card
    {
        public string Face = "Ace";
        public string[] Faces = ["Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King"];
        public string[] Colors = ["Hearts", "Clubs", "Diamonds", "Spade"];
        public string Color = "Hearts";
        public string FullFace = "Ace of Hearts";
        public int Value = 14;
        public int SecondaryValue = 0;
        public Card(string face, string color)
        {
            Face = face;
            Color = color;
            FullFace = face + " of " + color;
            switch (Face)
            {
                case ("Ace"):
                    Value = 14;
                    SecondaryValue = 1;
                    break;
                case ("Two"):
                    Value = 2;
                    SecondaryValue = Value;
                    break;
                case ("Three"):
                    Value = 3;
                    SecondaryValue = Value;
                    break;
                case ("Four"):
                    Value = 4;
                    SecondaryValue = Value;
                    break;
                case ("Five"):
                    Value = 5;
                    SecondaryValue = Value;
                    break;
                case ("Six"):
                    Value = 6;
                    SecondaryValue = Value;
                    break;
                case ("Seven"):
                    Value = 7;
                    SecondaryValue = Value;
                    break;
                case ("Eight"):
                    Value = 8;
                    SecondaryValue = Value;
                    break;
                case ("Nine"):
                    Value = 9;
                    SecondaryValue = Value;
                    break;
                case ("Ten"):
                    Value = 10;
                    SecondaryValue = Value;
                    break;
                case ("Jack"):
                    Value = 11;
                    SecondaryValue = Value;
                    break;
                case ("Queen"):
                    Value = 12;
                    SecondaryValue = Value;
                    break;
                case ("King"):
                    Value = 13;
                    SecondaryValue = Value;
                    break;
            }
        }
        public void Look()
        {
            Console.WriteLine(FullFace);
        }
    }
}