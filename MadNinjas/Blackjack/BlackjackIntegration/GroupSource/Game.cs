using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    internal partial class Game
    {
        private List<Card> playerCards = new List<Card>();
        private List<Card> dealerCards = new List<Card>();
        private Deck deck = new Deck();
        public Game()
        {

        }

        public void StartGame()
        {

            // 1. Visa välkomstmeddelande

            // 2. Blanda kortleken
            deck.Shuffle();

            // 3. Dela ut två kort till spelaren
            playerCards.Add(deck.Draw());
            playerCards.Add(deck.Draw());


            // 4. Dela ut två kort till dealern
            dealerCards.Add(deck.Draw());
            dealerCards.Add(deck.Draw());

            // 5. Visa spelarens kort
            foreach (Card card in playerCards)
            {
                card.Look();
            }

            // 6. Visa dealerns första kort
            dealerCards[0].Look();

            // 7. Fråga spelaren om Hit eller Stand

            // 8. Om Hit - dra ett nytt kort

            // 9. Kontrollera om spelaren får över 21

            // 10. Om Stand - låt dealern spela

            // 11. Dealern drar kort tills den har minst 17

            // 12. Jämför poängen

            // 13. Visa vinnaren


        }
        private int CalculateScore(List<Card> cards)
        {
            int totalScore = 0;

            // Gå igenom alla kort
            foreach (Card card in cards)
            {
                totalScore += card.Value;
            }

            // Räkna om ess om poängen är över 21
            foreach (Card card in cards)
            {
                if (totalScore > 21 && card.SecondaryValue < card.Value)
                {
                    totalScore -= card.Value - card.SecondaryValue;
                }
            }

            // Returnera poängen
            return totalScore;

        }
    }
}
