using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M450_Bowling_Counter
{
    public class Frame
    {
        public int FrameNumber { get; }
        public List<Throw> Throws { get; }
        public bool IsStrike => Throws.FirstOrDefault()?.PinsKnockedDown == 10;
        public bool IsSpare => Throws.Count >= 2 && Throws[0].GetScore() + Throws[1].GetScore() == 10 && !IsStrike;

        public Frame(int frameNumber)
        {
            FrameNumber = frameNumber;
            Throws = new List<Throw>();
        }

        public void AddThrow(Throw Throw)
        {
            Throws.Add(Throw);
        }
    }
}
