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
        string player = "";
        int balance = 0;
        int bet = 0;
        public Game()
        {

        }


        public void StartGame(string name = "test")
        {
            // Kollar om profilen finns redan, annars skapar den en ny profil
            ScoreBoard scoreBoard = new ScoreBoard();
            List<ScoreEntry> scoress = scoreBoard.Load();
            bool hasProfile = false;
            foreach (ScoreEntry score in scoress)
            {
                if (score.Name == name)
                {
                    Console.WriteLine("Welcome back, " + name + "!");
                    hasProfile = true;
                    balance = score.FinalBalance;
                    break;
                }
            }
            if (!hasProfile)
            {
                scoreBoard.NewProfile(name);
                balance = 100;
            }
            // 1. Visa välkomstmeddelande
            Console.WriteLine("Din saldo: " + balance);
            bool BetOkay = false;
            while (!BetOkay)
            {
                Console.WriteLine("Din bet:");
                bet = int.Parse(Console.ReadLine());
                if (bet > 0 && bet <= balance)
                {
                    BetOkay = true;
                }
                else
                {
                    Console.WriteLine("Invalid bet. Please enter a valid amount.");
                }
            }


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
            Console.WriteLine("Dealer's Card:");
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
                WinGame(name);
            }
            else if (playerScore > dealerScore)
            {
                WinGame(name);
            }
            else if (dealerScore > playerScore)
            {
                Console.WriteLine("Dealer wins!");
                LoseGame(name);
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
        private void WinGame(string winname)
        {
            Console.WriteLine("You win!");
            BalanceManager balMan = new BalanceManager();
            balMan.Win(bet);  // Öka spelarens saldo med bet
            ScoreBoard scoreBoard = new ScoreBoard();
            List<ScoreEntry> scoress = scoreBoard.Load();
            foreach (ScoreEntry score in scoress)
            {
                if (score.Name == winname)
                {
                    score.FinalBalance = balance + bet;
                    break;
                }
            }
            scoreBoard.Save(scoress);//spara nya scoreboarden
            //scoreBoard.AddResult(winname, balance + bet, 1);  // Lägg till poängen i score board
        }
        private void LoseGame(string losename)
        {
            Console.WriteLine("You Lose!");
            BalanceManager balMan = new BalanceManager();
            balMan.Win(bet);  // Dra spelarens saldo med bet
            ScoreBoard scoreBoard = new ScoreBoard();
            List<ScoreEntry> scoress = scoreBoard.Load();
            foreach (ScoreEntry score in scoress)
            {
                if (score.Name == losename)
                {
                    score.FinalBalance = balance - bet;
                    break;
                }
            }
            scoreBoard.Save(scoress);//spara nya scoreboarden
            //scoreBoard.AddResult(winname, balance - bet, 1);  // dra poängen i score board
        }

    }
}
