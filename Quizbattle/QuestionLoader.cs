using System.Text.Json;
namespace Quizbattle;

public class QuestionLoader : IQuestionLoader
{
    public List<Category> Load(string path) //metod som laddar frågor från en JSON-fil och returnerar en lista av kategorier
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Frågefilen hittades inte: {path}", path); //throwar ett FileNotFoundException om filen inte hittas
        }
        string json = File.ReadAllText(path); //läser in innehållet i filen som en sträng

        try //försöker parsa JSON-strängen till en lista av kategorier
        {
            using JsonDocument document = JsonDocument.Parse(json);
            return ParseCategories(document.RootElement); //anropar ParseCategories metoden och skickar in root elementet i JSON-dokumentet
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Frågefilen är inte en giltig JSON-fil: {ex.Message}", ex); //throwar ett InvalidDataException om JSON-parsningen misslyckas
        }
        catch (KeyNotFoundException ex)
        {
            throw new InvalidDataException($"Frågefilen saknar nödvändiga fält: {ex.Message}", ex);
        }
    }

    private static List<Category> ParseCategories(JsonElement root)
    {
        var categories = new List<Category>();
        foreach (JsonElement categoryElement in root.GetProperty("categories").EnumerateArray())
        {
            string name = categoryElement.GetProperty("name").GetString() ?? "";
            var questions = new List<IQuestion>();

            foreach (JsonElement questionElement in categoryElement.GetProperty("questions").EnumerateArray())
            {
                questions.Add(ParseQuestion(questionElement));
            }

            categories.Add(new Category(name, questions));
        }
        return categories;
    }
    private static IQuestion ParseQuestion(JsonElement element)
    {
        string type = element.GetProperty("type").GetString() ?? "";
        string text = element.GetProperty("text").GetString() ?? "";
        Difficulty difficulty =
            ParseDifficulty(element.GetProperty("difficulty").GetString());

        switch (type)
        {
            case "multipleChoice":
                var options = new List<string>();
                foreach (JsonElement option in element.GetProperty("options").EnumerateArray())
                {
                    options.Add(option.GetString() ?? "");
                }
                int correctIndex = element.GetProperty("correctIndex").GetInt32();
                return new MultipleChoiceQuestion(text, difficulty, options, correctIndex);

            case "trueFalse":
                bool correct = element.GetProperty("correct").GetBoolean();
                return new TrueFalseQuestion(text, difficulty, correct);

            default:
                throw new InvalidDataException($"Okänd frågetyp '{type}' för frågan: {text}");
        }
    }

    private static Difficulty ParseDifficulty(string? value)
    {
        return value switch
        {
            "easy" => Difficulty.Easy,
            "medium" => Difficulty.Medium,
            "hard" => Difficulty.Hard,
            _ => throw new InvalidDataException($"Okänd svårighetsnivå '{value}'")
        };
    }
}