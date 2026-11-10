using System;
using System.Collections.Generic;
using System.Text;

namespace Quizbattle;

public abstract class Question : IQuestion  //abstract class som implementerar IQuestion interface
{
    protected Question(string text, Difficulty difficulty) //Constructor!!
    {
        if (string.IsNullOrWhiteSpace(text)) //kollar ifall texten är null eller white space
        {
            throw new ArgumentException("Fråga kan inte vara tom.", nameof(text));  //throwar ett argument exception om texten är nuell ellerwhite space
        }
    }
}