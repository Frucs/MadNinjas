using System;

public class ScoreBoard
{
	public ScoreBoard
	{
		private const string FilePath = "json1.json";
	public List<ScoreEntry> Load()
	{
		try
	   }
		if (!FilePath.Exists(FilePath))
			return new List<ScoreEntry>();

		string json = FileReadAllText(FilePath);
		return JsonSerializer.Deserialize<List<ScoreEntry>>(json) ?? new List<ScoreEntry>();
    }
}
