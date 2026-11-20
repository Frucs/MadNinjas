# Quiz Battle

Quiz Battle är ett frågesportspel för konsolen där **2–4 spelare** tävlar mot varandra på samma dator. Spelarna svarar på frågor från olika kategorier och samlar poäng. Efter spelet sparas resultaten på en topplista.

## Funktioner

- Spela med 2–4 personer.
- Ange spelarnas namn och välj 1–10 rundor.
- Välj frågekategori eller låt spelet välja en slumpmässigt.
- Svara på flervalsfrågor eller sant/falskt-frågor.
- Frågorna har olika svårighetsgrader: lätt, medel och svår.
- En fråga används inte mer än en gång under samma spel.
- Se poäng efter varje svar och ett slutresultat när spelet är klart.
- Se de tio bästa resultaten på topplistan.

## Poängsystem

Poäng ges för rätt svar utifrån frågans svårighetsgrad:

| Svårighetsgrad | Poäng vid rätt svar |
|---|---:|
| Lätt | 10 |
| Medel | 20 |
| Svår | 30 |

- Fel svar ger 0 poäng.
- Vid flera rätta svar i rad får spelaren seriebonus: +5 poäng för det andra rätta svaret i följd, +10 för det tredje, +15 för det fjärde och +20 för det femte och efterföljande rätta svar.
- Ett fel svar nollställer serien.
- Spelaren med flest poäng vinner. Vid lika poäng vinner spelaren med flest rätta svar. Om det fortfarande är lika blir det delad seger.

## Topplista

Efter varje spel sparas varje spelares resultat med:

- Namn
- Poäng
- Antal rätta svar
- Datum

Topplistan visar de tio bästa resultaten. Resultaten sorteras med högst poäng först. Om flera resultat har samma poäng kommer resultatet med flest rätta svar först.

## Frågor och datafiler

Spelets frågor läses från:

`data/questions.json`

Topplistan sparas i:

`data/highscores.json`

Frågorna är indelade i kategorier och innehåller frågetyp, frågetext och svårighetsgrad. Flervalsfrågor har fyra svarsalternativ. Sant/falskt-frågor innehåller ett påstående.

**Viktigt:** Behåll JSON-filernas struktur och filnamn. Om frågefilen saknas eller har fel format kan spelet inte läsa in frågorna som det ska.

## Så startar du spelet

### Alternativ 1: Visual Studio

1. Klona eller ladda ner gruppens GitHub-repo.
2. Öppna lösningsfilen (`.sln`) eller projektfilen (`.csproj`) i Visual Studio.
3. Kontrollera att projektets datafiler finns i rätt mapp.
4. Bygg projektet.
5. Starta programmet med **Start** eller **Ctrl + F5**.

### Alternativ 2: Terminal

1. Klona gruppens GitHub-repo.
2. Öppna en terminal i mappen där projektfilen (`.csproj`) ligger.
3. Bygg projektet:

   ```bash
   dotnet build
   ```

4. Starta spelet:

   ```bash
   dotnet run
   ```

Du behöver ha .NET SDK installerat för att kunna bygga och köra projektet från terminalen.

## Huvudmeny

När spelet startar visas en meny med tre val:

1. **Spela** – starta en ny spelomgång.
2. **Topplista** – visa de bästa resultaten.
3. **Avsluta** – stäng programmet.

## Teknik och projektstruktur

Projektet är byggt i C# som ett konsolprogram. Koden är uppdelad i klasser och filer med tydliga ansvarsområden. Projektet använder objektorienterad programmering, inklusive arv och minst ett interface.

## Git och samarbete

Projektet utvecklas gemensamt i gruppens GitHub-repo.

- Arbeta i en separat branch för en uppgift.
- Gör små commits med tydliga meddelanden.
- Pusha ändringar regelbundet.
- Testa ändringarna innan de förs in i `main`.
- Lägg inte till mapparna `bin` och `obj` i repot.

---

Quiz Battle – gruppprojekt i C#
