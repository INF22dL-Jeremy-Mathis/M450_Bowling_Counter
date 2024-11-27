using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M450_Bowling_Counter
{
    public class ScoreCalculator
    {
        private readonly Player _player;

        public ScoreCalculator(Player player)
        {
            _player = player;
        }

        public int CalculateTotalScore()
        {
            int totalScore = 0;

            for (int i = 0; i < _player.Frames.Count; i++)
            {
                var frame = _player.Frames[i];

                if (frame.FrameNumber < 10)
                {
                    if (frame.IsStrike)
                    {
                        totalScore += 10 + GetStrikeBonus(i);
                    }
                    else if (frame.IsSpare)
                    {
                        totalScore += 10 + GetSpareBonus(i);
                    }
                    else
                    {
                        totalScore += frame.Throws.Sum(r => r.GetScore());
                    }
                }
                else // 10th frame
                {
                    totalScore += frame.Throws.Sum(r => r.GetScore());
                }
            }

            return totalScore;
        }

        private int GetStrikeBonus(int frameIndex)
        {
            int bonus = 0;
            var frames = _player.Frames;
            int nextFrameIndex = frameIndex + 1;

            if (nextFrameIndex < frames.Count)
            {
                var nextFrame = frames[nextFrameIndex];
                bonus += nextFrame.Throws[0].GetScore();

                if (nextFrame.Throws.Count > 1)
                {
                    bonus += nextFrame.Throws[1].GetScore();
                }
                else if (nextFrame.IsStrike && frameIndex + 2 < frames.Count)
                {
                    bonus += frames[frameIndex + 2].Throws[0].GetScore();
                }
            }

            return bonus;
        }

        private int GetSpareBonus(int frameIndex)
        {
            int bonus = 0;
            var frames = _player.Frames;
            int nextFrameIndex = frameIndex + 1;

            if (nextFrameIndex < frames.Count)
            {
                var nextFrame = frames[nextFrameIndex];
                bonus += nextFrame.Throws[0].GetScore();
            }

            return bonus;
        }
    }
}
