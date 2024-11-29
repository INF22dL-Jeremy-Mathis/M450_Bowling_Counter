using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M450_Bowling_Counter
{
    public class Game
    {
        private readonly List<Player> _players;
        private readonly IInputProvider _inputProvider;
        private readonly IOutputProvider _outputProvider;
        private int _foulChance;

        public Game(List<Player> players, IInputProvider inputProvider, IOutputProvider outputProvider)
        {
            _players = players;
            _inputProvider = inputProvider;
            _outputProvider = outputProvider;
        }

        public void Start(int _foulChance = 5)
        {
            for (int frameNumber = 1; frameNumber <= 10; frameNumber++)
            {
                _outputProvider.DisplayMessage($"\n--- Frame {frameNumber} ---");

                foreach (var player in _players)
                {
                    var frame = new Frame(frameNumber);
                    _outputProvider.DisplayMessage($"\n{player.Name} ist an der Reihe:\n");

                    int pins = 10;

                    // First Throw
                    int firstPinsKnockedDown = _inputProvider.GetPinsKnockedDown(pins);
                    Throw firstThrow = new Throw(firstPinsKnockedDown, _foulChance);
                    frame.AddThrow(firstThrow);


                    if (firstThrow.IsFoul)
                    {
                        _outputProvider.DisplayMessage("Foul!\n");
                    }
                    else if (firstThrow.PinsKnockedDown == 0)
                    {
                        _outputProvider.DisplayMessage("Gutterball!\n");
                    }
                    else if (firstThrow.PinsKnockedDown == 10)
                    {
                        _outputProvider.DisplayMessage("Strike!\n");
                    }

                    if (frameNumber == 10)
                    {
                        HandleTenthFrame(frame, firstThrow);
                    }
                    else if (firstPinsKnockedDown != 10)
                    {
                        // Normal second throw
                        int secondPinsKnockedDown = _inputProvider.GetPinsKnockedDown(pins - firstThrow.PinsKnockedDown);
                        Throw secondThrow = new Throw(secondPinsKnockedDown, _foulChance);
                        frame.AddThrow(secondThrow);

                        
                        if (secondThrow.IsFoul)
                        {
                            _outputProvider.DisplayMessage("Foul!\n");
                        }
                        else if (secondThrow.PinsKnockedDown == 0)
                        {
                            _outputProvider.DisplayMessage("Gutterball!\n");
                        }
                        // Check for Spare
                        else if (firstThrow.PinsKnockedDown + secondThrow.PinsKnockedDown == 10)
                        {
                            _outputProvider.DisplayMessage("Spare!\n");
                        }
                    }
                    player.AddFrame(frame);
                }
            }
        }

        private void HandleTenthFrame(Frame frame, Throw firstThrow)
        {
            int pins = firstThrow.PinsKnockedDown == 10 ? 10 : 10 - firstThrow.PinsKnockedDown;

            // Second Throw
            int secondPinsKnockedDown = _inputProvider.GetPinsKnockedDown(pins);
            Throw secondThrow = new Throw(secondPinsKnockedDown, _foulChance);
            frame.AddThrow(secondThrow);


            if (secondThrow.IsFoul)
            {
                _outputProvider.DisplayMessage("Foul!\n");
            }
            else if (secondThrow.PinsKnockedDown == 0)
            {
                _outputProvider.DisplayMessage("Gutterball!\n");
            }
            else if (secondThrow.PinsKnockedDown == 10)
            {
                _outputProvider.DisplayMessage("Strike!\n");
            }
            else if (firstThrow.PinsKnockedDown + secondThrow.PinsKnockedDown == 10)
            {
                _outputProvider.DisplayMessage("Spare!\n");
            }

            // Third Throw if needed
            if (firstThrow.PinsKnockedDown == 10 || firstThrow.PinsKnockedDown + secondThrow.PinsKnockedDown == 10)
            {
                // Reset pins if first Throw was a strike or spare
                pins = secondThrow.PinsKnockedDown == 10 ? 10 : 10 - secondThrow.PinsKnockedDown;

                int thirdPinsKnockedDown = _inputProvider.GetPinsKnockedDown(pins);
                Throw thirdThrow = new Throw(thirdPinsKnockedDown, _foulChance);
                frame.AddThrow(thirdThrow);

                if (thirdThrow.IsFoul)
                {
                    _outputProvider.DisplayMessage("Foul!\n");
                }
                else if (thirdThrow.PinsKnockedDown == 0)
                {
                    _outputProvider.DisplayMessage("Gutterball!\n");
                }
                // Check for Strike on third Throw
                if (thirdThrow.PinsKnockedDown == 10)
                {
                    _outputProvider.DisplayMessage("Strike!\n");
                }
                else if (secondThrow.PinsKnockedDown + thirdThrow.PinsKnockedDown == 10)
                {
                    _outputProvider.DisplayMessage("Spare!\n");
                }
            }
        }
    }
}
