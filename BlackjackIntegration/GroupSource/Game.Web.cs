using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack
{
    // This partial class connects the web controls to the group's cards, scoring,
    // deck, and balance manager without changing their original implementations.
    internal partial class Game
    {
        private BalanceManager webBalance = new BalanceManager();
        private string webPhase = "idle";
        private string webPlayerName = "";
        private int webBet;
        private int webRounds;
        private int webCardsRemaining = 52;
        private string webMessage = "Enter your name to start playing.";
        private string webTone = "neutral";
        private string? webOutcome;
        private Queue<Card>? webScriptedCards;

        // Deterministic card order is used only by the separate integration checks.
        internal Game(IEnumerable<Card> testCards) : this()
        {
            webScriptedCards = new Queue<Card>(testCards);
        }

        public WebGameState WebStart(string name)
        {
            if (webPhase == "playing")
                throw new InvalidOperationException("Finish the round before starting a new game.");
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 40)
                throw new ArgumentException("Enter a name between 1 and 40 characters.");

            webPlayerName = name.Trim();
            webBalance = new BalanceManager();
            webBet = 0;
            webRounds = 0;
            playerCards.Clear();
            dealerCards.Clear();
            ResetWebDeck();
            webPhase = "idle";
            webOutcome = null;
            webTone = "neutral";
            webMessage = "Choose your bet and deal the cards.";
            return WebState();
        }

        public WebGameState WebDeal(int bet)
        {
            if (string.IsNullOrEmpty(webPlayerName) || webPhase == "ended")
                throw new InvalidOperationException("Start a game first.");
            if (webPhase == "playing")
                throw new InvalidOperationException("Finish the round before dealing again.");
            if (bet < 1 || bet > webBalance.Balance)
                throw new ArgumentException($"The bet must be a whole number between 1 and {webBalance.Balance} SEK.");

            if (webCardsRemaining < 15) ResetWebDeck();
            webBet = bet;
            webRounds += 1;
            webOutcome = null;
            webTone = "neutral";
            playerCards.Clear();
            dealerCards.Clear();
            for (int index = 0; index < 2; index++)
            {
                playerCards.Add(DrawWebCard());
                dealerCards.Add(DrawWebCard());
            }
            webPhase = "playing";
            webMessage = "Your turn. Hit or stand.";

            bool playerBlackjack = CalculateScore(playerCards) == 21;
            bool dealerBlackjack = CalculateScore(dealerCards) == 21;
            if (playerBlackjack && dealerBlackjack)
                ResolveWebRound("push", "Both have blackjack. Your bet is returned.");
            else if (playerBlackjack)
                ResolveWebRound("blackjack", $"Blackjack! You win {(int)Math.Floor(bet * 1.5)} SEK.");
            else if (dealerBlackjack)
                ResolveWebRound("loss", "The dealer has blackjack. You lose your bet.");
            return WebState();
        }

        public WebGameState WebHit()
        {
            RequireWebRound();
            playerCards.Add(DrawWebCard());
            int total = CalculateScore(playerCards);
            if (total > 21)
                ResolveWebRound("loss", $"You scored {total} and went over 21.");
            else if (total == 21)
                return WebStand();
            else
                webMessage = "Hit again or stand.";
            return WebState();
        }

        public WebGameState WebStand()
        {
            RequireWebRound();
            while (CalculateScore(dealerCards) < 17)
                dealerCards.Add(DrawWebCard());
            int playerTotal = CalculateScore(playerCards);
            int dealerTotal = CalculateScore(dealerCards);
            if (dealerTotal > 21)
                ResolveWebRound("win", $"The dealer went over 21. You win {webBet} SEK!");
            else if (playerTotal > dealerTotal)
                ResolveWebRound("win", $"You win {webBet} SEK!");
            else if (playerTotal == dealerTotal)
                ResolveWebRound("push", "Push. Your bet is returned.");
            else
                ResolveWebRound("loss", "The dealer wins. You lose your bet.");
            return WebState();
        }

        public WebGameState WebEnd()
        {
            if (webPhase == "playing")
                throw new InvalidOperationException("Finish the round before ending the game.");
            if (string.IsNullOrEmpty(webPlayerName) || webPhase == "ended")
                throw new InvalidOperationException("There is no active game to end.");
            webPhase = "ended";
            webMessage = "Game ended. Your result is ready to save.";
            return WebState();
        }

        public WebGameState WebState()
        {
            bool hidden = webPhase == "playing";
            return new WebGameState(
                webPhase, webPlayerName,
                hidden ? webBalance.Balance - webBet : webBalance.Balance,
                webBet, webRounds,
                playerCards.Select(ToWebCard).ToArray(),
                dealerCards.Select((card, index) => hidden && index == 1
                    ? new CardView("?", "hidden") : ToWebCard(card)).ToArray(),
                CalculateScore(playerCards),
                hidden ? null : CalculateScore(dealerCards),
                dealerCards.Count == 0 ? 0 : CalculateScore(new List<Card> { dealerCards[0] }),
                hidden, webMessage, webTone, webOutcome);
        }

        private void ResolveWebRound(string outcome, string message)
        {
            // Derin's balance methods apply net gains/losses. The stake has only
            // been reserved in WebState, so it must not be subtracted again here.
            RequireWebRound();
            switch (outcome)
            {
                case "win": webBalance.Win(webBet); break;
                case "loss": webBalance.Lose(webBet); break;
                case "blackjack": webBalance.BlackjackWin(webBet); break;
            }
            webPhase = "resolved";
            webOutcome = outcome;
            webTone = outcome == "win" || outcome == "blackjack" ? "win"
                : outcome == "loss" ? "loss" : "neutral";
            webMessage = message + (webBalance.Balance == 0
                ? " Your balance is empty. End the game to save your result." : "");
        }

        private void RequireWebRound()
        {
            if (webPhase != "playing")
                throw new InvalidOperationException("Deal the cards to start a round.");
        }

        private void ResetWebDeck()
        {
            deck = new Deck();
            deck.Shuffle();
            webCardsRemaining = 52;
        }

        private Card DrawWebCard()
        {
            Card card = webScriptedCards == null ? deck.Draw() : webScriptedCards.Dequeue();
            webCardsRemaining -= 1;
            return card;
        }

        private static CardView ToWebCard(Card card)
        {
            string rank = card.Face switch
            {
                "Ace" => "A", "Two" => "2", "Three" => "3", "Four" => "4",
                "Five" => "5", "Six" => "6", "Seven" => "7", "Eight" => "8",
                "Nine" => "9", "Ten" => "10", "Jack" => "J", "Queen" => "Q",
                "King" => "K", _ => throw new InvalidOperationException("Unknown card rank.")
            };
            string suit = card.Color switch
            {
                "Hearts" => "hearts", "Diamonds" => "diamonds", "Clubs" => "clubs",
                "Spade" or "Spades" => "spades",
                _ => throw new InvalidOperationException("Unknown card suit.")
            };
            return new CardView(rank, suit);
        }
    }

    internal record CardView(string Rank, string Suit);

    internal record WebGameState(
        string Phase, string PlayerName, int Balance, int Bet, int Rounds,
        IReadOnlyList<CardView> PlayerCards, IReadOnlyList<CardView> DealerCards,
        int PlayerTotal, int? DealerTotal, int DealerVisibleTotal,
        bool HideDealerHole, string Message, string Tone, string? Outcome, bool ResultSaved = false);
}
