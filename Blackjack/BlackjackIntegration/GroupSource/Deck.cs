namespace Blackjack
{
    public class Deck
    {
        List<Card> Cards = new List<Card>();
        // Konstruktor, Skapa en ny Kortlek
        public Deck()
        {
            Card dummy = new Card("Dummy", "Hearts");
            foreach (string face in dummy.Faces)
            {
                foreach (string color in dummy.Colors)
                {
                    Cards.Add(new Card(face, color));
                }
            }
        }
        // Ta ett kort ur Kortleken, den tas från högst upp ur leken.
        public Card Draw()
        {
            Card temp = Cards[0];
            Cards.RemoveAt(0);

            return temp;
        }
        // Kolla på alla korten i kortleken, från Högst upp till ner.
        public void Look()
        {
            foreach (Card card in Cards)
            {
                Console.WriteLine(card.FullFace);
            }
        }
        // Blanda kortleken, Chatgpt kallade den "Fisher-Yates shuffle".
        public void Shuffle()
        {
            Random random = new Random();

            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int randomIndex = random.Next(i + 1);

                // Byt plats på korten
                Card temp = Cards[i];
                Cards[i] = Cards[randomIndex];
                Cards[randomIndex] = temp;
            }
        }
    }
}