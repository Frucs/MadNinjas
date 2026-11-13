// ConsoleUI.cs
// Ansvar: Hanterar konsolgränssnittet i Quiz Battle.
// Syfte: Gör det möjligt för spelaren att navigera och spela via terminalen.
// ShowMainMenu() visar huvudmenyn och låter användaren välja
// mellan att spela, visa topplistan eller avsluta programmet.
// Klassen ska även hantera spelstart, kategorival, frågor,
// svar och visning av slutresultat.
// Spelregler och poängberäkning hanteras av spelmotorn,
// inte av ConsoleUI.
    using Quizbattle; // Hämtar Johans Player-klass
    namespace Quizbattle;  

public class ConsoleUI
{

    public int ShowMainMenu()
    


    {
        
        Console.WriteLine("=== Quiz Battle ===");
        Console.WriteLine("1. SPELA");
        Console.WriteLine("2. Topplista");
        Console.WriteLine("3. Avsluta");


        while (true)
        {
            Console.Write("Välj ett alternativ (1-3): ");

            string? input = Console.ReadLine();

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 3)
            {
                return choice;

            }
            Console.WriteLine("Felaktigt val! Försök igen.");
    } // Avslutar while-loopen
} // Avslutar ShowMainMenu-metoden

// Läser in spelarnas namn och antalet rundor.
// Returnerar inställningarna som senare används av spelmotorn.
public (List<string> PlayerNames, int Rounds) ReadGameSetup()
{
    int playerCount = ConsoleInput.ReadInt(
        "Antal spelare (2-4): ", 2, 4);

    List<string> playerNames = new List<string>();

    for (int i = 1; i <= playerCount; i++)
    {
        string name = ConsoleInput.ReadUniqueName(
            $"Namn på spelare {i}: ", playerNames);

        playerNames.Add(name);
    }

    int rounds = ConsoleInput.ReadInt(
        "Antal rundor (1-10): ", 1, 10);

    return (playerNames, rounds);
}

// Visar kategorier och låter spelaren välja en kategori eller slumpa.
// Returnerar kategoriens index, eller -1 om spelaren väljer Slumpa.
public int ReadCategoryChoice(IReadOnlyList<string> categoryNames) // Tar emot en lista med kategorinamn.
{
    if (categoryNames.Count == 0) // Kontrollerar om listan är tom.
    {
        throw new ArgumentException("Inga kategorier finns tillgängliga."); // Meddelar att metoden inte kan fortsätta utan kategorier.
    }

    Console.WriteLine("Välj kategori:"); // Visar instruktionen för spelaren.

    for (int i = 0; i < categoryNames.Count; i++) // Går igenom alla kategorier i listan.
    {
        Console.WriteLine($"{i + 1}. {categoryNames[i]}"); // Visar kategorins nummer och namn.
    }

    int randomOption = categoryNames.Count + 1; // Placerar Slumpa efter sista kategorin.

    Console.WriteLine($"{randomOption}. Slumpa"); // Visar alternativet Slumpa.

    int choice = ConsoleInput.ReadInt( // Använder vår metod för säker inmatning.
        "Ditt val: ", 1, randomOption); // Tillåter endast nummer inom det giltiga intervallet.

    if (choice == randomOption) // Kontrollerar om spelaren valde Slumpa.
    {
        return -1; // -1 betyder att spelmotorn senare ska välja slumpmässigt.
    }

    return choice - 1; // Omvandlar spelarens val till ett index som börjar på 0.
}





        // Visar en flervalsfråga med fyra alternativ och läser spelarens svar.
// Returnerar svaret som text ("1", "2", "3" eller "4").
public string ReadMultipleChoiceAnswer(
    string categoryName,
    string questionText,
    int points,
    IReadOnlyList<string> options)
{
    if (options.Count != 4) // Säkerställer att frågan har exakt fyra alternativ.
    {
        throw new ArgumentException("Frågan måste ha fyra svarsalternativ.");
    }

    Console.WriteLine(); // Skapar en tom rad för bättre läsbarhet.
    Console.WriteLine($"Kategori: {categoryName}"); // Visar frågans kategori.
    Console.WriteLine($"Poäng: {points}"); // Visar hur många grundpoäng frågan är värd.
    Console.WriteLine(questionText); // Visar själva frågan.

    for (int i = 0; i < options.Count; i++) // Går igenom alla svarsalternativ.
    {
        Console.WriteLine($"{i + 1}. {options[i]}"); // Visar alternativens nummer och text.
    }

    int answer = ConsoleInput.ReadInt("Ditt svar (1-4): ", 1, 4); // Läser ett giltigt svar.

        return answer.ToString(); // Returnerar svaret som text.
} // Avslutar ReadMultipleChoiceAnswer-metoden


// Visar en sant/falskt-fråga och tar emot spelarens svar.
// Returnerar "true" eller "false" enligt gruppens kontrakt.
public string ReadTrueFalseAnswer(
    string categoryName,
    string questionText,
    int points)
{
    Console.WriteLine(); // Skapar en tom rad för bättre läsbarhet.

    Console.WriteLine($"Kategori: {categoryName}"); // Visar frågans kategori.

    Console.WriteLine($"Poäng: {points}"); // Visar frågans grundpoäng.

    Console.WriteLine(questionText); // Visar frågan.

    Console.WriteLine("S = Sant, F = Falskt"); // Visar svarsalternativen.

    bool answer = ConsoleInput.ReadTrueFalse(
        "Ditt svar (S/F): "); // Använder vår befintliga metod för säker inmatning.

    return answer ? "true" : "false"; // Omvandlar bool till det textformat som spelmotorn förväntar sig.
}
// Visar resultatet efter att spelaren har svarat på en fråga.
// Visar rätt eller fel, rätt svar, erhållna poäng och totalpoäng.
public void ShowAnswerResult(
    string playerName,
    bool isCorrect,
    string correctAnswer,
    int earnedPoints,
    int totalScore)
{
    Console.WriteLine(); // Skapar en tom rad för tydligare utskrift.

    if (isCorrect) // Kontrollerar om spelaren svarade rätt.
    {
        Console.WriteLine("Rätt svar!"); // Visar att svaret var korrekt.
    }
    else // Körs om spelaren svarade fel.
    {
        Console.WriteLine("Fel svar!"); // Visar att svaret var fel.
        Console.WriteLine($"Rätt svar: {correctAnswer}"); // Visar det korrekta svaret.
    }

    Console.WriteLine($"Poäng för frågan: {earnedPoints}"); // Visar poängen för svaret.

    Console.WriteLine($"{playerName} har totalt {totalScore} poäng."); // Visar spelarens totala poäng.
}

// Visar slutresultatet efter att alla rundor är färdigspelade.
// Visar spelarnas placeringar, poäng, rätta svar och vinnare.
public void ShowFinalResult(
    IReadOnlyList<Player> ranking, // Tar emot spelarna i sorterad ordning.
    IReadOnlyList<Player> winners) // Tar emot en eller flera vinnare.
{
    Console.WriteLine(); // Skapar en tom rad.
    Console.WriteLine("=== SLUTRESULTAT ==="); // Visar rubriken.

    for (int i = 0; i < ranking.Count; i++) // Går igenom spelarna.
    {
        Player player = ranking[i]; // Hämtar den aktuella spelaren.

        Console.WriteLine(
            $"{i + 1}. {player.Name} - {player.Points} poäng" +
            $"({player.CorrectAnswers} rätt)"); // Visar placering och resultat.
    }

    Console.WriteLine(); // Skapar mellanrum före vinnaren.

    if (winners.Count == 1) // Kontrollerar om det finns en ensam vinnare.
    {
        Console.WriteLine($"Vinnare: {winners[0].Name}!"); // Visar vinnaren.
    }
    else if (winners.Count > 1) // Kontrollerar om flera delar vinsten.
    {
        Console.WriteLine("Delad seger!"); // Visar att flera vann.

        foreach (Player winner in winners) // Går igenom vinnarna.
        {
            Console.WriteLine(winner.Name); // Skriver ut varje vinnare.
        }
    }
    else // Hanterar en tom vinnarlista.
    {
        Console.WriteLine("Ingen vinnare kunde utses."); // Visar meddelande.
    }
}

} // Avslutar ConsoleUI-klassen

