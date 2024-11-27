using System;

namespace M450_Bowling_Counter
{
    public class Throw
    {
        public int PinsKnockedDown { get; }
        public bool IsFoul { get; }

        private static readonly Random RandomGenerator = new Random();

        public Throw(int pinsKnockedDown, int foulChance = 5)
        {
            if (pinsKnockedDown < 0 || pinsKnockedDown > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(pinsKnockedDown), "Pins knocked down must be between 0 and 10.");
            }

            PinsKnockedDown = pinsKnockedDown;

            // Generate a random number between 1 and 100 and compare with foulChance
            IsFoul = RandomGenerator.Next(1, 101) <= foulChance;
        }

        public int GetScore()
        {
            return IsFoul ? 0 : PinsKnockedDown;
        }
    }

}
