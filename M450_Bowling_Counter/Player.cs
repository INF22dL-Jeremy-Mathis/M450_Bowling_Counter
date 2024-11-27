using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M450_Bowling_Counter
{
    public class Player
    {
        public string Name { get; }
        public List<Frame> Frames { get; }

        public Player(string name)
        {
            Name = name;
            Frames = new List<Frame>();
        }

        public void AddFrame(Frame frame)
        {
            Frames.Add(frame);
        }

        public int CalculateTotalScore()
        {
            var scoreCalculator = new ScoreCalculator(this);
            return scoreCalculator.CalculateTotalScore();
        }
    }
}
