using System;
using System.Collections.Generic;
using System.Text;

namespace Quizbattle;

public class MultipleChoiceQuestion : Question
{
    public MultipleChoiceQuestion(string text, Difficulty difficulty, IReadOnlyList<string> options, int correctIndex)
        : base(text, difficulty) //anropar basklassn question konstruktor 
    {
        if (options == null || options.Count != 4) 
        {
            throw new ArgumentException("En flervalsfråga måste ha exakt 4 alternativ", nameof(options)); //throwar ett argument excpetion om options är null eller inte har exakt 4 alternativ 
        }
        if (correctIndex < 0 || correctIndex >= options.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(correctIndex)); 
        }

        Options = options;
        CorrectIndex = correctIndex;

    }
    public IReadOnlyList<string> Options { get;  } //retunerar en lista med alternativen för flervalsfrågan
    public int CorrectIndex { get; } //retunerar indexet för det korrekta alternativet

    public override bool CheckAnswer(string input)
    {
        if (!int.TryParse(input, out int number)) //försöker konvertera input till ett heltal
        {
            return false; //retunerar false om konverteringen misslyckas
        }
        return number - 1 == CorrectIndex; //retunerar true om det konverterade talet minus 1 är lika med CorrectIndex, annars false
    }
    public override string GetCorrectAnswerText()
    {
        return Options[CorrectIndex]; //retunerar texten för det korrekta alternativet baserat på CorrectIndex
    }
}