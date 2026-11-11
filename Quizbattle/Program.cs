using QuizBattle; // Ger tillgång till klassen ConsoleUI

ConsoleUI ui = new ConsoleUI(); // Skapar ett objekt av vår klass

int choice = ui.ShowMainMenu(); // Visar menyn och hämtar spelarens val

Console.WriteLine($"Du valde alternativ {choice}"); // Visar valet