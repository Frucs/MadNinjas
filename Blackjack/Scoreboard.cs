using System;
using System.Text.Json;

public class ScoreBoard
{
	public ScoreBoard
	{
		private const string FilePath = "json1.json";  //här sparas topplistn
	public List<ScoreEntry> Load()  
	{
		try
	   }
		if (!FilePath.Exists(FilePath))
			return new List<ScoreEntry>();  //om filen inte finns så retunerar den tom

		string json = FileReadAllText(FilePath);
		return JsonSerializer.Deserialize<List<ScoreEntry>>(json) ?? new List<ScoreEntry>(); //konvert till scoreentry objekt men ifall de inte funkar tom 
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
