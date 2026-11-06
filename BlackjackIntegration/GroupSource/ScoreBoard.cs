using System;
using System.Text.Json;

public class ScoreBoard
{
    private const string FilePath = "json1.json";  //här sparas topplistn

    public ScoreBoard()
    {
    }
    public List<ScoreEntry> Load()
    {
        try
        {
            if (!File.Exists(FilePath))
                return new List<ScoreEntry>();  //om filen inte finns så retunerar den tom

            string json = File.ReadAllText(FilePath);
            // Recover only valid completed results, including from structurally damaged JSON.
            return (JsonSerializer.Deserialize<List<ScoreEntry>>(json) ?? new List<ScoreEntry>())
                .Where(score => score is not null && !string.IsNullOrWhiteSpace(score.Name)
                    && score.Name.Length <= 40 && score.FinalBalance >= 0 && score.RoundsPlayed > 0)
                .OrderByDescending(score => score.FinalBalance).Take(5).ToList(); //konvert till scoreentry objekt men ifall de inte funkar tom
        }
        catch
        {
            return new List<ScoreEntry>();
        }
    }

    public void AddResult(string name, int finalBalance, int roundsPlayed) // lägger till ett nytt result
    {
        var scores = Load();
        scores.Add(new ScoreEntry
        {
            Name = name,
            FinalBalance = finalBalance,
            RoundsPlayed = roundsPlayed
        });

        scores = scores  //behåller bara 5 bästa resultaten
            .OrderByDescending(s => s.FinalBalance)
            .Take(5)
            .ToList();
        Save(scores);
    }

    private void Save(List<ScoreEntry> scores)
    {
        string json = JsonSerializer.Serialize(scores, new JsonSerializerOptions { WriteIndented = true });
        // Replace the file only after the complete JSON has been written.
        string temporaryPath = FilePath + ".tmp";
        File.WriteAllText(temporaryPath, json);
        File.Move(temporaryPath, FilePath, overwrite: true);
    }
    public void ShowTopList()
    {
        var scores = Load();

        Console.WriteLine("\n=== TOPPLISTA ===");

        int place = 1;

        foreach (var score in scores)
        {
            Console.WriteLine(
                $"{place}. {score.Name} - {score.FinalBalance} kr ({score.RoundsPlayed} rundor)");
            place++;
        }
    }
}