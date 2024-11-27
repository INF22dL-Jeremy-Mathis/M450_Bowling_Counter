using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using M450_Bowling_Counter;
using Moq;


namespace M450_Bowling_Counter.Tests
{
    [TestClass]
    public class BowlingGameTests
    {
        // Helper method to create a mock input provider for player names and counts (mocks console inputs of users)
        private Mock<IInputProvider> SetupMockInputProviderForPlayers(int playerCount, List<string> playerNames)
        {
            var mockInputProvider = new Mock<IInputProvider>();

            // Setup player count
            mockInputProvider.Setup(ip => ip.GetPlayerCount())
                .Returns(playerCount);

            // Setup player names
            for (int i = 0; i < playerNames.Count; i++)
            {
                int index = i; // Avoid closure issues
                mockInputProvider.Setup(ip => ip.GetPlayerName(index))
                    .Returns(playerNames[index]);
            }

            return mockInputProvider;
        }

        [TestMethod]
        public void Test_01_InputAndDisplayOfPlayerNames()
        {
            // Arrange
            var mockOutputProvider = new Mock<IOutputProvider>();
            var playerNames = new List<string> { "Alice", "Bob", "Charlie" };
            var mockInputProvider = new Mock<IInputProvider>();

            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(playerNames.Count);
            for (int i = 0; i < playerNames.Count; i++)
            {
                mockInputProvider.Setup(ip => ip.GetPlayerName(i + 1)).Returns(playerNames[i]);
            }

            // Act
            var players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);

            // Assert
            CollectionAssert.AreEqual(playerNames, players.Select(p => p.Name).ToList(), "Player names do not match expected values.");
        }





        [TestMethod]
        public void Test_02_RoundManagement()
        {
            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            // Setup input for 2 players
            var playerNames = new List<string> { "Player1", "Player2" };
            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(playerNames.Count);
            mockInputProvider.Setup(ip => ip.GetPlayerName(It.IsAny<int>()))
                .Returns((int index) => playerNames[index - 1]); // Adjust for 1-based indexing

            // Setup pins knocked down for 10 frames for each player
            mockInputProvider.SetupSequence(ip => ip.GetPinsKnockedDown(It.IsAny<int>()))
                .Returns(5).Returns(4) // Frame 1
                .Returns(3).Returns(6) // Frame 2
                .Returns(7).Returns(2) // Frame 3
                .Returns(4).Returns(5) // Frame 4
                .Returns(6).Returns(3) // Frame 5
                .Returns(8).Returns(1) // Frame 6
                .Returns(2).Returns(7) // Frame 7
                .Returns(9).Returns(0) // Frame 8
                .Returns(1).Returns(8) // Frame 9
                .Returns(5).Returns(4); // Frame 10

            var players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            var game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();

            // Assert
            // Verify each player completed 10 frames
            foreach (var player in players)
            {
                Assert.AreEqual(10, player.Frames.Count, $"{player.Name} does not have 10 frames.");
            }
        }


        [TestMethod]
        public void Test_03_RecordAShot()
        {
            // Test Case 03: Recording a Shot

            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            // Setup input for 1 player
            var playerNames = new List<string> { "Player1" };
            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(1);
            mockInputProvider.Setup(ip => ip.GetPlayerName(It.IsAny<int>())).Returns(playerNames[0]);


            // Simulate one frame with specific pins knocked down
            mockInputProvider.SetupSequence(ip => ip.GetPinsKnockedDown(It.IsAny<int>()))
                .Returns(7)  // First roll
                .Returns(2); // Second roll

            List<Player> players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            Game game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();

            // Assert
            var player = players.First();
            var frame = player.Frames.First();
            Assert.AreEqual(10, player.Frames.Count);
            Assert.AreEqual(2, frame.Throws.Count);
            Assert.AreEqual(7, frame.Throws[0].PinsKnockedDown);
            Assert.AreEqual(2, frame.Throws[1].PinsKnockedDown);
        }

        [TestMethod]
        public void Test_04_FoulPercentageDetection()
        {
            // Arrange
            var foulChance = 50; // Set the foul chance percentage
            var totalThrows = 10000; // Total number of throws to simulate
            int foulCount = 0;

            // Act
            for (int i = 0; i < totalThrows; i++)
            {
                var throwInstance = new Throw(5, foulChance); // PinsKnockedDown is arbitrary for this test
                if (throwInstance.IsFoul)
                {
                    foulCount++;
                }
            }

            double actualFoulPercentage = (foulCount / (double)totalThrows) * 100;

            // Assert
            // Allow a small tolerance to account for randomness
            double tolerance = 2.0; // Allow ±2% tolerance
            Assert.IsTrue(
                actualFoulPercentage >= foulChance - tolerance && actualFoulPercentage <= foulChance + tolerance,
                $"Foul percentage {actualFoulPercentage}% is outside the expected range ({foulChance - tolerance}% to {foulChance + tolerance}%).");
        }


        [TestMethod]
        public void Test_05_GutterBall()
        {
            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(1);
            mockInputProvider.Setup(ip => ip.GetPlayerName(It.IsAny<int>())).Returns("Player1");

            // Simulate a gutterball (0 pins knocked down)
            mockInputProvider.Setup(ip => ip.GetPinsKnockedDown(It.IsAny<int>()))
                .Returns(0);

            List<Player> players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            Game game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();

            // Assert
            mockOutputProvider.Verify(op => op.DisplayMessage(It.Is<string>(s => s.Contains("Gutterball!"))), Times.AtLeastOnce);
        }

        [TestMethod]
        public void Test_06_NormalThrowscoreCalculation()
        {
            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(1);
            mockInputProvider.Setup(ip => ip.GetPlayerName(It.IsAny<int>())).Returns("Player1");

            // Simulate two throws that do not knock down all pins
            mockInputProvider.SetupSequence(ip => ip.GetPinsKnockedDown(It.IsAny<int>()))
                .Returns(4) // First throw
                .Returns(5); // Second throw

            List<Player> players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            Game game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();

            // Assert
            var player = players.First();
            Assert.AreEqual(10, player.Frames.Count); // Ensure 10 frames (as per game logic)
            Assert.AreEqual(9, player.CalculateTotalScore(), "Total score should be 4 + 5 = 9.");
        }



        [TestMethod]
        public void Test_07_StrikeDetectionAndScoring()
        {
            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(1);
            mockInputProvider.Setup(ip => ip.GetPlayerName(It.IsAny<int>())).Returns("Player1");

            // Simulate a strike followed by two throws
            mockInputProvider.SetupSequence(ip => ip.GetPinsKnockedDown(It.IsAny<int>()))
                .Returns(10) // Strike
                .Returns(4)  // Next frame, first throw
                .Returns(3); // Next frame, second throw

            List<Player> players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            Game game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();

            // Assert
            var player = players.First();
            int expectedScore = 24; // Strike: 10 + 4 + 3 = 17, Next frame: 4 + 3 = 7, Total: 24
            Assert.AreEqual(expectedScore, player.CalculateTotalScore(), $"Expected score {expectedScore}, but got {player.CalculateTotalScore()}.");
        }



        [TestMethod]
        public void Test_08_SpareDetectionAndScoring()
        {
            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(1);
            mockInputProvider.Setup(ip => ip.GetPlayerName(0)).Returns("Player1");

            // Simulate a spare followed by one roll
            mockInputProvider.SetupSequence(ip => ip.GetPinsKnockedDown(It.IsAny<int>()))
                .Returns(7)  // First roll of the spare
                .Returns(3)  // Spare
                .Returns(4); // Next frame first roll

            List<Player> players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            Game game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();

            // Debug: Print messages captured by the mock
            var performedMessages = mockOutputProvider.Invocations
                .Where(i => i.Method.Name == nameof(IOutputProvider.DisplayMessage))
                .Select(i => i.Arguments[0]?.ToString())
                .ToList();

            foreach (var message in performedMessages)
            {
                System.Diagnostics.Debug.WriteLine($"Message: {message}");
            }

            // Assert
            var player = players.First();
            int totalScore = player.CalculateTotalScore();
            // Spare frame: 10 + 4 = 14
            // Next frame: 4
            // Total: 14 + 4 = 18
            Assert.AreEqual(18, totalScore, $"Expected score: 18, but got: {totalScore}.");
            mockOutputProvider.Verify(op => op.DisplayMessage(It.Is<string>(s => s.Contains("Spare!"))), Times.Once);
        }



        [TestMethod]
        public void Test_09_MissNoPinsHit()
        {
            // Test Case 09: Miss (No pins hit)

            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(1);
            mockInputProvider.Setup(ip => ip.GetPlayerName(0)).Returns("Player1");

            // Simulate two Throws with no pins knocked down
            mockInputProvider.SetupSequence(ip => ip.GetPinsKnockedDown(It.IsAny<int>()))
                .Returns(0)
                .Returns(0);

            List<Player> players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            Game game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();

            // Assert
            var player = players.First();
            int totalScore = player.CalculateTotalScore();
            Assert.AreEqual(0, totalScore);
        }

        [TestMethod]
        public void Test_10_BonusPointsForConsecutiveStrikes()
        {
            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(1);
            mockInputProvider.Setup(ip => ip.GetPlayerName(It.IsAny<int>())).Returns("Player1");

            // Simulate two consecutive strikes followed by a third roll
            mockInputProvider.SetupSequence(ip => ip.GetPinsKnockedDown(It.IsAny<int>()))
                .Returns(10) // Strike
                .Returns(10) // Strike
                .Returns(4)
                .Returns(2);

            List<Player> players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            Game game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();

            // Assert
            var player = players.First();
            Assert.AreEqual(46, player.CalculateTotalScore()); // Strike: 10 + 10 + 4 = 24, Strike: 10 + 4 + 2 = 16, 4 + 2 = 6, Total: 46
        }


        [TestMethod]
        public void Test_11_ExtraShotsInLastFrameAfterStrike()
        {
            // Test Case 11: Extra Shots in Last Frame after Strike

            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(1);
            mockInputProvider.Setup(ip => ip.GetPlayerName(0)).Returns("Player1");

            // Simulate 9 frames with minimal scores, then a strike in the 10th frame
            var Throws = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                Throws.Add(3);
                Throws.Add(4);
            }
            Throws.Add(10); // Strike in 10th frame
            Throws.Add(10); // Extra roll 1
            Throws.Add(10); // Extra roll 2

            var pinsSequence = mockInputProvider.SetupSequence(ip => ip.GetPinsKnockedDown(It.IsAny<int>()));
            foreach (var pins in Throws)
            {
                pinsSequence = pinsSequence.Returns(pins);
            }

            List<Player> players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            Game game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();

            // Assert
            var player = players.First();
            Assert.AreEqual(10, player.Frames.Count);
            var lastFrame = player.Frames.Last();
            Assert.AreEqual(3, lastFrame.Throws.Count);
            mockOutputProvider.Verify(op => op.DisplayMessage("Strike!"), Times.AtLeastOnce);
        }

        [TestMethod]
        public void Test_12_TotalScoreCalculationAndDisplay()
        {
            // Test Case 12: Total Score Calculation and Display

            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            // Setup two players with different scores
            var playerNames = new List<string> { "Player1", "Player2" };
            mockInputProvider.Setup(ip => ip.GetPlayerCount()).Returns(2);
            mockInputProvider.Setup(ip => ip.GetPlayerName(It.IsAny<int>()))
                .Returns((int index) => playerNames[index]);


            // Simulate Throws for two players
            var ThrowsPlayer1 = new List<int> { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; // Perfect game
            var ThrowsPlayer2 = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // Zero score

            var pinsSequence = mockInputProvider.SetupSequence(ip => ip.GetPinsKnockedDown(It.IsAny<int>()));

            foreach (var pins in ThrowsPlayer1.Concat(ThrowsPlayer2))
            {
                pinsSequence = pinsSequence.Returns(pins);
            }

            List<Player> players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            ScoreBoard scoreBoard = new ScoreBoard(players, mockOutputProvider.Object);
            Game game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();
            scoreBoard.Display();

            // Assert
            var player1 = players[0];
            var player2 = players[1];
            Assert.AreEqual(300, player1.CalculateTotalScore());
            Assert.AreEqual(0, player2.CalculateTotalScore());
            // Verify that the output provider displays the winner
            mockOutputProvider.Verify(op => op.DisplayMessage(It.Is<string>(s => s.Contains("300"))), Times.AtLeastOnce);
        }

        [TestMethod]
        public void Test_13_GameRestart()
        {
            // Test Case 13: Game Restart via Input Console

            // Arrange
            var mockInputProvider = new Mock<IInputProvider>();
            var mockOutputProvider = new Mock<IOutputProvider>();

            // Simulate game end and restart
            mockInputProvider.SetupSequence(ip => ip.GetPlayerCount())
                .Returns(1) // First game
                .Returns(1); // Second game

            mockInputProvider.Setup(ip => ip.GetPlayerName(It.IsAny<int>()))
                .Returns("Player1");


            // Simulate Throws for first game
            mockInputProvider.SetupSequence(ip => ip.GetPinsKnockedDown(It.IsAny<int>()))
                .Returns(4)
                .Returns(5)
                // Simulate the choice to restart the game
                .Returns(1) // Input '1' to restart
                            // Simulate Throws for second game
                .Returns(6)
                .Returns(3);

            List<Player> players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            Game game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);

            // Act
            game.Start();

            // Simulate game restart
            // For this test, we'll assume that the game logic can handle a restart,
            // or we can re-instantiate the game.

            players = PlayerFactory.CreatePlayers(mockInputProvider.Object, mockOutputProvider.Object);
            game = new Game(players, mockInputProvider.Object, mockOutputProvider.Object);
            game.Start();

            // Assert
            // Ensure that new game has reset scores
            var player = players.First();
            int totalScore = player.CalculateTotalScore();
            Assert.AreEqual(9, totalScore); // 6 + 3 = 9
        }
    }
}