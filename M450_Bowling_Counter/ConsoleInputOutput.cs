using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M450_Bowling_Counter
{
    public class ConsoleInputProvider : IInputProvider
    {
        public int GetPinsKnockedDown(int pinsLeft)
        {
            int pinsKnockedDown;
            while (true)
            {
                Console.Write($"Anzahl umgeworfener Pins (0 bis {pinsLeft}): ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out pinsKnockedDown) && pinsKnockedDown >= 0 && pinsKnockedDown <= pinsLeft)
                {
                    break;
                }
                else
                {

                    // delete previous line
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"Ungültiger Input! ");
                }
            }

            return pinsKnockedDown;
        }

        public int GetPlayerCount()
        {
            int playerCount;
            while (!int.TryParse(Console.ReadLine(), out playerCount) || playerCount < 1)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Ungültiger input. Die Spieleranzahl muss mindestens 1 Betragen: ");
            }
            Console.Write("\n");
            return playerCount;
        }

        public string GetPlayerName(int playerNumber)
        {
            string name;
            do
            {
                name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"Name von Spieler {playerNumber} darf nicht leer sein. Bitte einen gültigen Namen eingeben: ");
                }
            } while (string.IsNullOrWhiteSpace(name));

            return name;
        }
    }

    public class ConsoleOutputProvider : IOutputProvider
    {
        public void DisplayMessage(string message)
        {
            Console.Write(message);
        }
    }
}
