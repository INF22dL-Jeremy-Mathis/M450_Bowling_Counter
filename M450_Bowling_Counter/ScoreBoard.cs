using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M450_Bowling_Counter
{
    public class ScoreBoard
    {
        private readonly List<Player> _players;
        private readonly IOutputProvider _outputProvider;

        public ScoreBoard(List<Player> players, IOutputProvider outputProvider)
        {
            _players = players;
            _outputProvider = outputProvider;
        }

        public void Display()
        {
            var splitterLength = 100;
            var table = new StringBuilder();

            // Build the header lines
            table.AppendLine(new string('\n', 1));
            table.Append(BuildHeaderLine());
            table.Append(BuildThrowLine());
            table.AppendLine(new string('-', splitterLength));

            // Build each player's line
            foreach (var player in _players)
            {
                table.AppendLine(BuildPlayerLine(player));
                table.AppendLine(new string('-', splitterLength));
            }

            // Output the complete scoreboard
            _outputProvider.DisplayMessage(table.ToString());
        }

        private string BuildHeaderLine()
        {
            var header = new StringBuilder("Frame".PadRight(10));

            for (int frame = 1; frame <= 10; frame++)
            {
                header.Append($"| {frame,-6}");
            }

            header.Append("| Total".PadLeft(10));
            header.AppendLine();
            return header.ToString();
        }

        private string BuildThrowLine()
        {
            var throwLine = new StringBuilder("Wurf".PadRight(10));

            for (int frame = 1; frame <= 10; frame++)
            {
                if (frame == 10)
                {
                    throwLine.Append("| 1  2  3  ");
                }
                else
                {
                    throwLine.Append("| 1  2  ");
                }
            }

            throwLine.Append("|");
            throwLine.AppendLine();
            return throwLine.ToString();
        }

        private string BuildPlayerLine(Player player)
        {
            var playerLine = new StringBuilder(player.Name.PadRight(10));
            int totalScore = player.CalculateTotalScore();

            foreach (var frame in player.Frames)
            {
                playerLine.Append("| ");

                for (int i = 0; i < frame.Throws.Count; i++)
                {
                    var Throw = frame.Throws[i];
                    playerLine.Append($"{GetThrowSymbol(Throw, frame, i),-3}");
                }

                // Fill empty spaces if not enough Throws
                int expectedThrows = frame.FrameNumber == 10 ? 3 : 2;
                int missingThrows = expectedThrows - frame.Throws.Count;
                for (int i = 0; i < missingThrows; i++)
                {
                    playerLine.Append("   ");
                }
            }

            // Fill empty frames if any (in case of less than 10 frames)
            int missingFrames = 10 - player.Frames.Count;
            for (int i = 0; i < missingFrames; i++)
            {
                playerLine.Append("|      ");
            }

            playerLine.Append($"| {totalScore,3}");
            return playerLine.ToString();
        }

        private string GetThrowSymbol(Throw Throw, Frame frame, int ThrowIndex)
        {
            if (Throw.IsFoul)
            {
                return "F";
            }
            else if (Throw.PinsKnockedDown == 10 && ThrowIndex == 0 && frame.FrameNumber < 10)
            {
                return "X"; // Strike in frames 1-9
            }
            else if (frame.FrameNumber == 10 && Throw.PinsKnockedDown == 10)
            {
                return "X"; // Handle strikes in 10th frame
            }
            else if (ThrowIndex > 0 && frame.Throws.Count > 1 &&
                     frame.Throws[ThrowIndex - 1].GetScore() + Throw.GetScore() == 10 &&
                     frame.FrameNumber < 10)
            {
                return "/"; // Spare in frames 1-9
            }
            else if (Throw.PinsKnockedDown == 0)
            {
                return "-"; // Gutter ball
            }
            else
            {
                return Throw.PinsKnockedDown.ToString();
            }
        }
    }
}
