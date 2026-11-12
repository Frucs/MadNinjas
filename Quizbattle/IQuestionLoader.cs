using System;
using System.Collections.Generic;
using System.Text;

namespace Quizbattle;

public interface IQuestionLoader
{
    List<Category> Load(string path);  //laddar kategorier och frågor från en fil som anges av path och returnerar en lista av kategorier. Om filen inte hittas eller om JSON-parsningen misslyckas, kastas ett undantag
}
