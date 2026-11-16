using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Quizbattle;

public class TrueFalseQuestion : Question
{
    public TrueFalseQuestion(string text, Difficulty difficulty, bool correctAnswer)
        : base(text, difficulty) //anropar basklassn question konstruktor
    {
        CorrectAnswer = correctAnswer; //sätter värdet för CorrectAnswer baserat på parameter
    }

    public bool CorrectAnswer { get; } //retunerar det korrekta svaret för true/false fråga
    public override bool CheckAnswer(string input)
    {
        if (!bool.TryParse(input, out bool answer))//försökr konventera input tll en boolean
        {
            return false; //retunerar false om konverteringen misslyckas 
        }

        return answer == CorrectAnswer;
    }
    public override string GetCorrectAnswerText()
    {
        return CorrectAnswer ? "Sant" : "Falskt"; //retunerar sant om CorrectAnswer är true annars falskt
    }
}
