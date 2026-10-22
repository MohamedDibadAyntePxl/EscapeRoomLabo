using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace EscapeRoom
{
    internal class Program
    {

        static void Main(string[] args)
        {
            ShowTitle();   // titel tonen
            ShowIntro();   // intro tonen + wachten op Enter
            StartGame();   // het spel starten
        }
        static void StartGame()
        {
            int timeLeft = 15;

            bool keypadSolved = false;

            bool riddleSolved = false;

            bool escaped = false;

            do
            {
                // ShowTitle-, ShowStatus- en ShowMenu
                ShowTitle();
                ShowStatus(timeLeft, keypadSolved, riddleSolved);
                ShowMenu();

                // ===== NIEUW =====
                int action = ReadAction();                 // keuze opvangen

                if (action == 9)                           // opgeven
                {
                    break;                                 // stopt de do-while (staat buiten de switch!)
                }

                switch (action)
                {
                    case 1:
                        if (!keypadSolved)                 // al opgelost → negeren
                        {
                            if (TryKeypad())
                            {
                                keypadSolved = true;
                            }
                            else
                            {
                                timeLeft = ApplyPenalty(timeLeft, 2);  // nieuwe tijd opvangen
                            }
                        }
                        break;

                    case 2:
                        if (!riddleSolved)
                        {
                            if (SolveRiddle())
                            {
                                riddleSolved = true;
                            }
                            else
                            {
                                timeLeft = ApplyPenalty(timeLeft, 2);
                            }
                        }
                        break;

                    case 3:
                        if (FinalDoor(keypadSolved, riddleSolved))
                        {
                            escaped = true;
                        }
                        else if (keypadSolved && riddleSolved)   // enkel straf als je echt gegokt hebt
                        {
                            timeLeft = ApplyPenalty(timeLeft, 1);
                        }
                        break;

                    default:                                   // ongeldige keuze (ook -1)
                        ShowColoredText("Invalid option.", ConsoleColor.DarkRed, true);
                        timeLeft = ApplyPenalty(timeLeft, 2);
                        break;
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                // ===== EINDE NIEUW =====

            } while (timeLeft > 0 && !escaped);   // ← NIEUW: hier stond niks tussen de haakjes

            // ===== NIEUW =====
            if (escaped)
            {
                ShowColoredText("You escaped! Well done!", ConsoleColor.DarkGreen, true);
            }
            else if (timeLeft == 0)
            {
                ShowColoredText("Time's up! You are trapped forever...", ConsoleColor.Red, true);
            }
            else
            {
                ShowColoredText("You gave up.", ConsoleColor.Red, true);
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
            // ===== EINDE NIEUW =====
        }

        static void ShowTitle()                       // void = geeft niks terug, () = geen parameters
        {
            Console.Clear();                          // console leegmaken
            Console.WriteLine(@"
                            ___                         ___
                            | __|___ __ __ _ _ __  ___  | _ \___  ___ _ __
                            | _|(_-</ _/ _` | '_ \/ -_) |   / _ \/ _ \ '  \
                            |___/__/\__\__,_| .__/\___| |_|_\___/\___/_|_|_|
                                |_|
                            ================================================
");                                                   // @ = verbatim: backslashes letterlijk tonen
        }

        // ===== 2.2 Intro =====
        static void ShowIntro()
        {
            Console.WriteLine("Solve the puzzles before time runs out!");
            Console.WriteLine("Press enter to start your adventure...");
            Console.ReadLine();                       // wacht tot de gebruiker op Enter drukt
        }

        // ===== 2.3 Gekleurde tekst =====
        static void ShowColoredText(string text, ConsoleColor color, bool appendLine = false)
        // 3 parameters; "= false" is een standaardwaarde: laat je hem weg bij het aanroepen, dan is hij false
        {
            Console.ForegroundColor = color;          // tekstkleur instellen
            Console.Write(text);                      // tekst tonen (zelfde regel)
            if (appendLine)                           // moet er een nieuwe regel bij?
            {
                Console.WriteLine();                  // naar de volgende regel
            }
            Console.ResetColor();                     // kleur terugzetten naar normaal
        }

        // ===== 2.4 Status =====
        static void ShowStatus(int timeLeft, bool keypadSolved, bool riddleSolved)
        // types afgeleid uit de code: timeLeft is een getal, de andere twee zijn true/false
        {
            ShowColoredText($"Time left: {timeLeft} minutes", ConsoleColor.Blue, true);
            ShowColoredText($"Keypad: {(keypadSolved ? "solved" : "unsolved")}", ConsoleColor.Blue, true);
            // ternary: is keypadSolved true → "solved", anders "unsolved"
            ShowColoredText($"Riddle: {(riddleSolved ? "solved" : "unsolved")}", ConsoleColor.Blue, true);
            Console.WriteLine();                      // lege regel
        }

        // ===== 2.5 Menu =====
        static void ShowMenu()
        {
            ShowColoredText("Choose an action:", ConsoleColor.White, true);
            ShowColoredText("  1) Try keypad", ConsoleColor.White, true);
            ShowColoredText("  2) Solve riddle", ConsoleColor.White, true);
            ShowColoredText("  3) Open final door", ConsoleColor.White, true);
            ShowColoredText("  9) Give up", ConsoleColor.White, true);
            ShowColoredText("Your choice: ", ConsoleColor.White, false); // false = cursor blijft op deze regel
        }

        // ===== 3. Keuze inlezen =====
        static int ReadAction()                       // int = deze methode geeft een getal terug
        {
            string input = Console.ReadLine();        // keuze inlezen als tekst
            if (int.TryParse(input, out int choice))  // lukt het om er een getal van te maken?
            {
                return choice;                        // ja → geef dat getal terug
            }
            return -1;                                // nee → -1 = "ongeldig"
        }

        // ===== 4. Tijdstraf =====
        static int ApplyPenalty(int timeLeft, int penalty) // geeft de nieuwe tijd terug
        {
            int newTime = timeLeft - penalty;         // straf aftrekken
            if (newTime < 0)                          // nie onder 0 gaan
            {
                newTime = 0;
            }
            ShowColoredText($"Time penalty: -{penalty} minute(s).", ConsoleColor.DarkYellow, true);
            return newTime;                           // nieuwe tijd teruggeven
        }


        static bool TryKeypad()                       // bool = geeft true of false terug
        {
            ShowColoredText("Enter 3-digit keypad code: ", ConsoleColor.Magenta);
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int code))   // ! = "lukt NIET" → geen getal  /////// input int.equals"314" werkt ook //////////
            {
                ShowColoredText("Invalid code (not a number).", ConsoleColor.DarkRed, true);
                return false;
            }
            //// we hebben een getal (code)
            else if (code == 314)                          //  //hier kan input.Equals"314" ook
            {
                ShowColoredText("Keypad unlocked!.", ConsoleColor.Green, true);
                return true;
            }
            else
            {
                ShowColoredText("Wrong code", ConsoleColor.Red, true);
                return false;
            }


        }

        // ===== NIEUW: 6. Riddle =====
        static bool SolveRiddle()
        {
            ShowColoredText("Riddle: Speak friend and enter...", ConsoleColor.Magenta, true);
            ShowColoredText("Type the secret word: ", ConsoleColor.Magenta);

            string answer = Console.ReadLine();
            answer = answer.Trim().ToLower();              // spaties weg + alles klein

            if (answer == "open-sesame")
            {
                ShowColoredText("The wall slides aside.", ConsoleColor.Green, true);
                return true;
            }
            else
            {
                ShowColoredText("The wall remains silent.", ConsoleColor.Red, true);
                return false;
            }
        }

        // ===== NIEUW: 7. Final door =====
        static bool FinalDoor(bool keypadSolved, bool riddleSolved)
        {
            if (!keypadSolved || !riddleSolved)            // één van de twee nog niet opgelost
            {
                ShowColoredText("The final door won't budge. Solve the other puzzles first.", ConsoleColor.Red, true);
                return false;
            }

            ShowColoredText("Final number (1..5): ", ConsoleColor.Magenta);
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int number) || number < 1 || number > 5)
            {
                ShowColoredText("That is not between 1 and 5.", ConsoleColor.DarkRed, true);
                return false;
            }
            else if (number == 3)
            {
                ShowColoredText("You hear the lock click!", ConsoleColor.Green, true);
                return true;
            }
            else
            {
                ShowColoredText("A buzzer sounds. Wrong!", ConsoleColor.Red, true);
                return false;
            }
        }
    }
}

