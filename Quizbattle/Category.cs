using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Quizbattle;

public class Category //frågor i quizet organiuserad i kategorier, varje kateogri har ett namn och en lista av frågor
{
    public Category(string name, List<IQuestion> questions)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Kategori namn kan inte vara tom!", nameof(name)); 
        }
        Name = name;
        Questions = questions ?? new List<IQuestion>();
    }
    public string Name { get; }
    public List<IQuestion> Questions { get; }
}
    