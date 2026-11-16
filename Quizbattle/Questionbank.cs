using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Quizbattle;

public class Questionbank //håller i frågorna som inte har blivit ställda än 
{
    private readonly List<Category> _categories;
    public Questionbank(List<Category> categories)
    {
        //kopierar kategorierna och frågorna för att undivika att de ändras utanför questionbanken + spelet kan starta med en full lista av f¨ågor 
        _categories = new List<Category>();
        foreach (Category category in categories)
        {
            var questionsCopy = new List<IQuestion>(category.Questions);
            if (questionsCopy.Count > 0)
            {
                _categories.Add(new Category(category.Name, questionsCopy));
            }
       
        }
    }
    public bool IsEmpty => _categories.Count == 0;
    //bara kategorier med frågor som inte har blivit ställda än finns 
    public IReadOnlyList<Category> GetAvailableCategories()
    {
        return _categories.AsReadOnly();
    }
    public int GetRemainingCount(Category category)
    {
        Category? found = FindCategory(category);
        return found == null ? 0 : found.Questions.Count;
    }

    //tar en slumpad fråga från en kategori och tar bort den så den inte körs två gånger
    public IQuestion DrawQuestion(Category category)
    {
        Category? found = FindCategory(category);
        if (found == null)
        {
            throw new InvalidOperationException($"Kategorin {category.Name} har inga frågor kvar.");
        }

        int index = Random.Shared.Next(found.Questions.Count);
        IQuestion question = found.Questions[index];
        found.Questions.RemoveAt(index);

        if (found.Questions.Count == 0)
        {
            _categories.Remove(found);
        }


        return question;

    }

    //tar en slumpad kategori och sedan en slumpad fråga 
    public IQuestion DrawRandomQuestion(out Category category)
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Det finns inga frågor kvar :( ");
        }

        category = _categories[Random.Shared.Next(_categories.Count)];
        return DrawQuestion(category);
    }
    private Category? FindCategory(Category category)
    {
        return _categories.FirstOrDefault(c => c.Name == category.Name);
    }

}
