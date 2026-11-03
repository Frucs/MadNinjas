using System;
public class BalanceManager
{
    public int Balance { get; private set; } = 100;  //spelarensd saldo start 100kr
    public int AskForBet()
    {
        while (true)
        {
            Console.Write($"Du har {Balance} kr. Insats (1-{Balance}):");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int bet))
            {
                if (bet >= 1 && bet <= Balance)
                    return bet;
            }
            Console.WriteLine($"Insatsen måste vara ett heltal mellan 1 och {Balance}.}");
        }
    }

    public void Win(int bet)  //ökar saldo med vinst
    {
        Balance += bet;
    }
    public void Lose(int bet) //minska saldo vid förlust
    {
        Balance -= bet;
    }
    public void BlackjackWin(int bet)  //ger bonus vid blackjack 1.5x
    {
        int win = (int)Math.Floor(bet * 1.5);
        Balance += win;
    }
}