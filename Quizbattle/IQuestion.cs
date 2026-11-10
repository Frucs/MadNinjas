using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Quizbattle;

public interface IQuestion
{
    string text { get; }
    Difficulty Difficulty { get; }
    int Points { get; }
    bool CheckAnswer(string input);
    string GetCorrectAnswerText();
}
