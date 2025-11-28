using NUnit.Framework;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Repositories;

namespace JDG.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class GameStateRepositoryTests
    {
        private GameStateRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new GameStateRepository();
        }

        [Test]
        public void ResetGameState_InitializesToDefaultValues()
        {
            // Act
            _repository.ResetGameState();

            // Assert
            Assert.AreEqual(JDG.Domain.Phase.Draw, _repository.CurrentPhase);
            Assert.AreEqual(1, _repository.TurnNumber);
            Assert.AreEqual(PlayerId.Player1, _repository.CurrentPlayer);
            Assert.IsFalse(_repository.IsGameOver);
        }

        [Test]
        public void SetPhase_UpdatesCurrentPhase()
        {
            // Act
            _repository.SetPhase(JDG.Domain.Phase.Attack);

            // Assert
            Assert.AreEqual(JDG.Domain.Phase.Attack, _repository.CurrentPhase);
        }

        [Test]
        public void IncrementTurn_IncreasesTurnNumber()
        {
            // Arrange
            _repository.ResetGameState();

            // Act
            _repository.IncrementTurn();

            // Assert
            Assert.AreEqual(2, _repository.TurnNumber);
        }

        [Test]
        public void SwitchPlayer_SwitchesBetweenPlayers()
        {
            // Arrange
            _repository.SetCurrentPlayer(PlayerId.Player1);

            // Act
            _repository.SwitchPlayer();

            // Assert
            Assert.AreEqual(PlayerId.Player2, _repository.CurrentPlayer);

            // Act again
            _repository.SwitchPlayer();

            // Assert
            Assert.AreEqual(PlayerId.Player1, _repository.CurrentPlayer);
        }

        [Test]
        public void SetGameOver_UpdatesGameOverState()
        {
            // Arrange
            _repository.ResetGameState();

            // Act
            _repository.SetGameOver(true);

            // Assert
            Assert.IsTrue(_repository.IsGameOver);
        }
    }
}
