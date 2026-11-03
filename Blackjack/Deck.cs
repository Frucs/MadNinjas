using System.Security.AccessControl;

namespace Blackjack
{
    public class Deck
    {
        List<Card> Cards = new List<Card>();
        public Deck()
        {
            Card dummy = new Card("Dummy", "Hearts");
            foreach (string face in dummy.Faces)
            {
                foreach (string color in dummy.Colors)
                {
                    Console.WriteLine(face + " of " + color + " added to Deck.");
                    Cards.Add(new Card(face, color));
                }
            }
        }
    }
}