using System;
using System.Collections.Generic;
using System.Text;

namespace Blackjack
{
    internal class Game
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
            Console.WriteLine("Your hand:");
            foreach (Card card in playerCards)
            {
                card.Look();
            }

            // 6. Visa dealerns första kort
            dealerCards[0].Look();
            // 7. Fråga spelaren om Hit eller Stand
            string player = "";
            int playerScore = CalculateScore(playerCards);

            while (playerScore < 21 && player.ToLower() != "stand")
            {
                Console.WriteLine("Draw a card (Hit) or stop (Stand)?");
                player = Console.ReadLine();

                if (player.ToLower() == "hit")
                {
                    Card newCard = deck.Draw();
                    playerCards.Add(newCard);
                    newCard.Look();

                    playerScore = CalculateScore(playerCards);
                }
                else if (player.ToLower() != "stand")
                {
                    Console.WriteLine("Please enter Hit or Stand.");
                }
            }

            // 8. Kontrollera om spelaren får över 21
            if (playerScore > 21)
            {
                Console.WriteLine("You got " + playerScore + ". You lose!");
                return;
            }

            // 9. Dealerns tur
            int dealerScore = CalculateScore(dealerCards);

            Console.WriteLine("Dealer's turn!");
            
            // 10. Dealern drar kort tills den har minst 17
            while (dealerScore < 17)
            {
                Card newCard = deck.Draw();
                dealerCards.Add(newCard);
                newCard.Look();

                dealerScore = CalculateScore(dealerCards);
            }

            // 11. Visa och jämför poängen
            playerScore = CalculateScore(playerCards);
            dealerScore = CalculateScore(dealerCards);

            Console.WriteLine("Your score: " + playerScore);
            Console.WriteLine("Dealer's score: " + dealerScore);

            // 12. Visa vinnaren
            if (dealerScore > 21)
            {
                Console.WriteLine("Dealer busts! You win!");
            }
            else if (playerScore > dealerScore)
            {
                Console.WriteLine("You win!");
            }
            else if (dealerScore > playerScore)
            {
                Console.WriteLine("Dealer wins!");
            }
            else
            {
                Console.WriteLine("It's a tie!");
            }


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
