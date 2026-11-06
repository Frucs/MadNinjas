using Blackjack;

int passed = 0;
void Check(string title, Action body)
{
    body();
    passed++;
    Console.WriteLine($"PASS {title}");
}
void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new Exception($"Expected {expected}, received {actual}.");
}
void Throws(Action body)
{
    try { body(); }
    catch (ArgumentException) { return; }
    catch (InvalidOperationException) { return; }
    throw new Exception("Expected an argument or game phase error.");
}
Card CardFor(string rank)
{
    string face = rank switch
    {
        "A" => "Ace", "2" => "Two", "3" => "Three", "4" => "Four",
        "5" => "Five", "6" => "Six", "7" => "Seven", "8" => "Eight",
        "9" => "Nine", "10" => "Ten", "J" => "Jack", "Q" => "Queen", "K" => "King",
        _ => throw new ArgumentException("Unknown test rank.")
    };
    return new Card(face, "Hearts");
}
Game Scenario(params string[] cards)
{
    var game = new Game(cards.Select(CardFor));
    game.WebStart(" Andreas ");
    return game;
}

Check("deck contains 52 unique cards", () =>
{
    var deck = new Deck();
    deck.Shuffle();
    var cards = Enumerable.Range(0, 52).Select(_ => deck.Draw().FullFace).ToArray();
    Equal(52, cards.Distinct().Count());
});
Check("ace scoring changes from 11 to 1 after a hit", () =>
{
    var game = Scenario("A", "9", "6", "8", "5");
    Equal(17, game.WebDeal(10).PlayerTotal);
    Equal(12, game.WebHit().PlayerTotal);
});
Check("multiple aces and face card values use the group scoring", () =>
{
    var game = Scenario("A", "9", "A", "8", "9");
    Equal(12, game.WebDeal(10).PlayerTotal);
    var state = game.WebHit();
    Equal(21, state.PlayerTotal);
    Equal("resolved", state.Phase);
    Equal(20, Scenario("K", "9", "Q", "8").WebDeal(10).PlayerTotal);
});
Check("normal win settles once and phase guards prevent repeated payments", () =>
{
    var game = Scenario("10", "9", "8", "8");
    Equal(80, game.WebDeal(20).Balance);
    Throws(() => game.WebDeal(10));
    Throws(() => game.WebStart("New"));
    Throws(() => game.WebEnd());
    var state = game.WebStand();
    Equal("win", state.Outcome);
    Equal(120, state.Balance);
    Throws(() => game.WebStand());
    Throws(() => game.WebHit());
    Equal(120, game.WebState().Balance);
});
Check("natural blackjack rounds an odd bet bonus down", () =>
{
    var state = Scenario("A", "9", "K", "8").WebDeal(3);
    Equal("blackjack", state.Outcome);
    Equal(104, state.Balance);
    Equal(false, state.HideDealerHole);
});
Check("both natural blackjacks push", () =>
{
    var state = Scenario("A", "A", "K", "Q").WebDeal(20);
    Equal("push", state.Outcome);
    Equal(100, state.Balance);
});
Check("dealer natural blackjack loses exactly the stake", () =>
{
    var state = Scenario("9", "A", "K", "Q").WebDeal(20);
    Equal("loss", state.Outcome);
    Equal(80, state.Balance);
});
Check("player bust can consume the full balance and end only once", () =>
{
    var game = Scenario("10", "9", "8", "8", "K");
    Equal(0, game.WebDeal(100).Balance);
    var state = game.WebHit();
    Equal("loss", state.Outcome);
    Equal(0, state.Balance);
    Equal(28, state.PlayerTotal);
    state = game.WebEnd();
    Equal("ended", state.Phase);
    Equal("Andreas", state.PlayerName);
    Equal(1, state.Rounds);
    Throws(() => game.WebEnd());
    Throws(() => game.WebDeal(1));
});
Check("dealer stands on soft 17 and a tie leaves the balance unchanged", () =>
{
    var game = Scenario("10", "A", "7", "6");
    game.WebDeal(20);
    var state = game.WebStand();
    Equal(2, state.DealerCards.Count);
    Equal(17, state.DealerTotal);
    Equal("push", state.Outcome);
    Equal(100, state.Balance);
});
Check("hitting 21 automatically plays the dealer and handles dealer bust", () =>
{
    var game = Scenario("10", "10", "5", "6", "6", "K");
    game.WebDeal(10);
    var state = game.WebHit();
    Equal(21, state.PlayerTotal);
    Equal(26, state.DealerTotal);
    Equal("win", state.Outcome);
    Equal(110, state.Balance);
});
Check("hidden dealer card values never leave the bridge", () =>
{
    var game = Scenario("10", "9", "8", "8");
    var state = game.WebDeal(20);
    Equal(true, state.HideDealerHole);
    Equal<int?>(null, state.DealerTotal);
    Equal(9, state.DealerVisibleTotal);
    Equal("?", state.DealerCards[1].Rank);
    Equal("hidden", state.DealerCards[1].Suit);
    state = game.WebStand();
    Equal("8", state.DealerCards[1].Rank);
    Equal("hearts", state.DealerCards[1].Suit);
});
Check("invalid names bets and pregame actions are rejected", () =>
{
    var game = new Game();
    Throws(() => game.WebDeal(10));
    Throws(() => game.WebHit());
    Throws(() => game.WebStand());
    Throws(() => game.WebEnd());
    Throws(() => game.WebStart(""));
    Throws(() => game.WebStart("   "));
    Throws(() => game.WebStart(new string('x', 41)));
    game.WebStart("Andreas");
    foreach (int bet in new[] { -1, 0, 101, int.MaxValue }) Throws(() => game.WebDeal(bet));
    Equal(100, game.WebState().Balance);
    Equal(0, game.WebState().Rounds);
});
Check("new session resets cards balance rounds and name", () =>
{
    var game = Scenario("A", "9", "K", "8");
    game.WebDeal(20);
    game.WebEnd();
    var state = game.WebStart("Musse");
    Equal("idle", state.Phase);
    Equal(100, state.Balance);
    Equal(0, state.Rounds);
    Equal(0, state.PlayerCards.Count);
    Equal("Musse", state.PlayerName);
});
Check("scoreboard tolerates missing and malformed JSON and persists top five", () =>
{
    string previous = Directory.GetCurrentDirectory();
    string temporary = Path.Combine(Path.GetTempPath(), "blackjack-checks-" + Guid.NewGuid());
    Directory.CreateDirectory(temporary);
    try
    {
        Directory.SetCurrentDirectory(temporary);
        var board = new ScoreBoard();
        Equal(0, board.Load().Count);
        File.WriteAllText("json1.json", "{invalid json");
        Equal(0, board.Load().Count);
        File.WriteAllText("json1.json", "null");
        Equal(0, board.Load().Count);
        for (int index = 1; index <= 7; index++) board.AddResult($"Player {index}", index * 100, index);
        var scores = new ScoreBoard().Load();
        Equal(5, scores.Count);
        Equal(700, scores[0].FinalBalance);
        Equal("Player 7", scores[0].Name);
        Equal(7, scores[0].RoundsPlayed);
        Equal(300, scores[4].FinalBalance);
        Equal(true, scores.Select(entry => entry.FinalBalance).SequenceEqual(new[] { 700, 600, 500, 400, 300 }));
    }
    finally
    {
        Directory.SetCurrentDirectory(previous);
    }
});

// Independent reference scoring starts with every ace worth one, then upgrades
// one ace when possible. It never reads the group's Value/SecondaryValue fields.
int ReferenceScore(IEnumerable<Card> cards)
{
    int total = 0;
    bool hasAce = false;
    foreach (Card card in cards)
    {
        if (card.Face == "Ace") hasAce = true;
        total += card.Face switch
        {
            "Ace" => 1, "Two" => 2, "Three" => 3, "Four" => 4,
            "Five" => 5, "Six" => 6, "Seven" => 7, "Eight" => 8,
            "Nine" => 9, "Ten" or "Jack" or "Queen" or "King" => 10,
            _ => throw new Exception("Unexpected reference card.")
        };
    }
    return hasAce && total + 10 <= 21 ? total + 10 : total;
}
Check("group scoring matches an independent reference for all rank hands up to four cards", () =>
{
    string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    var game = new Game();
    var score = typeof(Game).GetMethod("CalculateScore",
        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
        ?? throw new Exception("Group scoring method missing.");
    int count = 0;
    void Visit(List<Card> hand)
    {
        if (hand.Count > 0)
        {
            int actual = (int)(score.Invoke(game, new object[] { hand }) ?? -1);
            Equal(ReferenceScore(hand), actual);
            count++;
        }
        if (hand.Count == 4) return;
        foreach (string rank in ranks)
        {
            hand.Add(CardFor(rank));
            Visit(hand);
            hand.RemoveAt(hand.Count - 1);
        }
    }
    Visit(new List<Card>());
    Equal(30_940, count);
});
Check("20000 seeded legal decks agree with independent outcomes balances and messages", () =>
{
    var coverage = new Dictionary<string, int>();
    void Cover(string key) => coverage[key] = coverage.GetValueOrDefault(key) + 1;
    string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    string[] suits = { "Hearts", "Diamonds", "Clubs", "Spade" };
    for (int seed = 1; seed <= 20_000; seed++)
    {
        var random = new Random(seed);
        var order = ranks.SelectMany(rank => suits.Select(suit => new Card(CardFor(rank).Face, suit))).ToList();
        for (int index = order.Count - 1; index > 0; index--)
        {
            int other = random.Next(index + 1);
            (order[index], order[other]) = (order[other], order[index]);
        }
        Equal(52, order.Select(card => card.FullFace).Distinct().Count());
        var remaining = new Queue<Card>(order);
        var player = new List<Card> { remaining.Dequeue() };
        var dealer = new List<Card> { remaining.Dequeue() };
        player.Add(remaining.Dequeue());
        dealer.Add(remaining.Dequeue());
        var game = new Game(order);
        game.WebStart("Reference player");
        int bet = 1 + seed % 100;
        var state = game.WebDeal(bet);
        bool playerNatural = ReferenceScore(player) == 21;
        bool dealerNatural = ReferenceScore(dealer) == 21;
        string expectedOutcome;
        if (playerNatural || dealerNatural)
        {
            expectedOutcome = playerNatural && dealerNatural ? "push" : playerNatural ? "blackjack" : "loss";
            if (playerNatural && dealerNatural) Cover("both-natural");
            else if (dealerNatural) Cover("dealer-natural");
        }
        else
        {
            int hitBelow = 12 + 2 * (seed % 5);
            while (ReferenceScore(player) < hitBelow)
            {
                player.Add(remaining.Dequeue());
                state = game.WebHit();
                if (ReferenceScore(player) >= 21) break;
            }
            if (ReferenceScore(player) > 21)
            {
                expectedOutcome = "loss";
                Cover("player-bust");
            }
            else
            {
                while (ReferenceScore(dealer) < 17) dealer.Add(remaining.Dequeue());
                // A hit reaching 21 has already played the actual dealer.
                if (ReferenceScore(player) != 21) state = game.WebStand();
                int playerTotal = ReferenceScore(player);
                int dealerTotal = ReferenceScore(dealer);
                expectedOutcome = dealerTotal > 21 || playerTotal > dealerTotal ? "win"
                    : playerTotal == dealerTotal ? "push" : "loss";
                if (dealerTotal > 21) Cover("dealer-bust");
            }
        }
        int expectedBalance = expectedOutcome switch
        {
            "win" => 100 + bet,
            "blackjack" => 100 + (int)Math.Floor(bet * 1.5),
            "loss" => 100 - bet,
            _ => 100
        };
        try
        {
            Equal("resolved", state.Phase);
            Equal(expectedOutcome, state.Outcome);
            Equal(expectedBalance, state.Balance);
            Equal(ReferenceScore(player), state.PlayerTotal);
            Equal<int?>(ReferenceScore(dealer), state.DealerTotal);
            Equal(player.Count, state.PlayerCards.Count);
            Equal(dealer.Count, state.DealerCards.Count);
            Equal(false, state.HideDealerHole);
            string expectedTone = expectedOutcome is "win" or "blackjack" ? "win"
                : expectedOutcome == "loss" ? "loss" : "neutral";
            Equal(expectedTone, state.Tone);
            Equal(expectedOutcome is "win" or "blackjack", state.Message.Contains("You win", StringComparison.Ordinal));
            if (expectedOutcome == "loss")
                Equal(true, state.Message.Contains("lose", StringComparison.Ordinal) || state.Message.Contains("over 21", StringComparison.Ordinal));
            Throws(() => game.WebStand());
            Equal(expectedBalance, game.WebState().Balance);
        }
        catch (Exception error)
        {
            throw new Exception($"Seed {seed}, player {string.Join(',', player.Select(c => c.Face))}, dealer {string.Join(',', dealer.Select(c => c.Face))}: {error.Message}", error);
        }
        Cover(expectedOutcome);
    }
    foreach (string required in new[] { "win", "loss", "push", "blackjack", "both-natural", "dealer-natural", "player-bust", "dealer-bust" })
    {
        if (coverage.GetValueOrDefault(required) == 0) throw new Exception($"Missing reference coverage: {required}.");
    }
    Console.WriteLine("Reference outcome coverage: " + string.Join(", ", coverage.Select(pair => $"{pair.Key}={pair.Value}")));
});

Check("scoreboard recovers null entries and invalid rows before keeping the top five", () =>
{
    string previous = Directory.GetCurrentDirectory();
    string temporary = Path.Combine(Path.GetTempPath(), "blackjack-json-" + Guid.NewGuid());
    Directory.CreateDirectory(temporary);
    try
    {
        Directory.SetCurrentDirectory(temporary);
        File.WriteAllText("json1.json", "[null,{\"Name\":\"Invalid\",\"FinalBalance\":9999,\"RoundsPlayed\":0},{\"Name\":\"\",\"FinalBalance\":500,\"RoundsPlayed\":2},{\"Name\":\"Negative\",\"FinalBalance\":-10,\"RoundsPlayed\":1},{\"Name\":\"Valid\",\"FinalBalance\":120,\"RoundsPlayed\":2}]");
        var board = new ScoreBoard();
        Equal(1, board.Load().Count);
        board.AddResult("Recovered", 130, 3);
        Equal(2, board.Load().Count);
        Equal("Recovered", board.Load()[0].Name);
        Equal(false, File.Exists("json1.json.tmp"));
    }
    finally { Directory.SetCurrentDirectory(previous); }
});
Console.WriteLine($"All {passed} integration checks passed.");
