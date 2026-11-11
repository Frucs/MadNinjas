// ConsoleInput.cs
// Ansvar: Hanterar användarens inmatning i Quiz Battle.
// Syfte: Säkerställer att spelaren anger giltiga värden.
// Klassen förhindrar att felaktig inmatning kraschar spelet.
// ReadInt() läser ett heltal inom ett angivet intervall.
// Metoden kan återanvändas för exempelvis antal spelare,
// antal rundor och menyval.

namespace QuizBattle;

public static class ConsoleInput
{
    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);

            string? input = Console.ReadLine();

            if (int.TryParse(input, out int number)
                && number >= min
                && number <= max)
            {
                return number;
            }

            Console.WriteLine($"Ange ett heltal mellan {min} och {max}.");
        }
    }
    // Läser ett svar på en sant-eller-falskt-fråga.
// Returnerar true för sant och false för falskt.
public static bool ReadTrueFalse(string prompt)
{
    while (true)
    {
        Console.Write(prompt);

        string? input = Console.ReadLine()?.Trim().ToLowerInvariant();

        if (input == "s")
        {
            return true;
        }

        if (input == "f")
        {
            return false;
        }

        Console.WriteLine("Ogiltigt svar! Skriv s för sant eller f för falskt.");
    }
}

// Läser ett unikt spelarnamn som inte får vara tomt.
// Kontrollerar även att namnet inte redan används.
public static string ReadUniqueName(string prompt, IReadOnlyList<string> taken)
{
    while (true)
    {
        Console.Write(prompt);

        string name = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Namnet får inte vara tomt.");
            continue;
        }

        if (taken.Any(existing =>
            string.Equals(existing, name, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Namnet används redan. Välj ett annat.");
            continue;
        }

        return name;
    }
}
}