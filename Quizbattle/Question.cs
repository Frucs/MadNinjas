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
        Text = text;
        Difficulty = difficulty;
        Points = GetPointsFor(difficulty); //anropar GetPointsFor metoden och skickar in difficulty som parameter
    }
    public string Text { get; } //retunerar text för frpågan
    public Difficulty Difficulty { get; }  //retunberar svårighetsniuvå
    public int Points  { get; }  //retunerar poängen för frpgan

    public abstract bool CheckAnswer(string input); //absstract metod som måste implementeras i subclasses plus de nedan

    public abstract string GetCorrectAnswerText(); 
    

    private static int GetPointsFor(Difficulty difficulty)
    {
        return difficulty switch //switch expression som retunerar poäng baserat på svårighetsnivån
        {
            Difficulty.Easy => 10,
            Difficulty.Medium => 20,
            Difficulty.Hard => 30,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty)) //throwar ett argument out of range exception om svårighetsnivån inte matchar någon av de definierade nivåerna
        };
    }
}