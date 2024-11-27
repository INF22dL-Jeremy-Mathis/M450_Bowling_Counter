using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M450_Bowling_Counter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IInputProvider inputProvider = new ConsoleInputProvider();
            IOutputProvider outputProvider = new ConsoleOutputProvider();

            List<Player> players = PlayerFactory.CreatePlayers(inputProvider, outputProvider);

            Game game = new Game(players, inputProvider, outputProvider);
            game.Start();

            ScoreBoard scoreBoard = new ScoreBoard(players, outputProvider);
            scoreBoard.Display();

            outputProvider.DisplayMessage("\nDrücke eine beliebige Taste um zu beenden...");
            Console.ReadKey();
        }
    }
}
