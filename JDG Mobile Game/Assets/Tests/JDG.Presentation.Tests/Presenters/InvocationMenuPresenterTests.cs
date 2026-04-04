using NUnit.Framework;
using JDG.Application.Services;
using JDG.Presentation.Presenters;
using JDG.Presentation.Views;
using UnityEngine;

namespace JDG.Presentation.Tests.Presenters
{
    /// <summary>
    /// Unit tests for InvocationMenuPresenter.
    /// Phase 45: Moved to JDG.Presentation.Tests assembly.
    /// Tests the presenter's ability to manage invocation menu display state.
    /// </summary>
    [TestFixture]
    public class InvocationMenuPresenterTests
    {
        private InvocationMenuPresenter _presenter;
        private TestInvocationMenuView _view;
        private TestCombatQueryService _combatService;

        [SetUp]
        public void SetUp()
        {
            _view = new TestInvocationMenuView();
            _combatService = new TestCombatQueryService();
            _presenter = new InvocationMenuPresenter(_view, _combatService);
        }

        [Test]
        public void Display_InAttackPhase_ShowsAttackButton()
        {
            // Arrange
            _combatService.SetCanAttack(true);
            var screenPosition = new Vector3(100, 200, 0);

            // Act
            _presenter.Display(screenPosition, isAttackPhase: true);

            // Assert
            Assert.IsTrue(_view.IsMenuVisible);
            Assert.AreEqual(screenPosition, _view.MenuPosition);
            Assert.IsTrue(_view.AttackButtonVisible);
            Assert.IsTrue(_view.AttackButtonInteractable);
        }

        [Test]
        public void Display_InAttackPhaseWithCannotAttack_ShowsDisabledAttackButton()
        {
            // Arrange
            _combatService.SetCanAttack(false);
            var screenPosition = new Vector3(100, 200, 0);

            // Act
            _presenter.Display(screenPosition, isAttackPhase: true);

            // Assert
            Assert.IsTrue(_view.AttackButtonVisible);
            Assert.IsFalse(_view.AttackButtonInteractable);
        }

        [Test]
        public void Display_NotInAttackPhase_HidesAttackButton()
        {
            // Arrange
            _combatService.SetCanAttack(true);
            var screenPosition = new Vector3(100, 200, 0);

            // Act
            _presenter.Display(screenPosition, isAttackPhase: false);

            // Assert
            Assert.IsFalse(_view.AttackButtonVisible);
        }

        [Test]
        public void Display_WithActionAndNotAttackPhase_ShowsActionButton()
        {
            // Arrange
            _combatService.SetHasAction(true);
            _combatService.SetActionPossible(true);
            var screenPosition = new Vector3(100, 200, 0);

            // Act
            _presenter.Display(screenPosition, isAttackPhase: false);

            // Assert
            Assert.IsTrue(_view.ActionButtonVisible);
            Assert.IsTrue(_view.ActionButtonInteractable);
        }

        [Test]
        public void Display_WithActionButInAttackPhase_HidesActionButton()
        {
            // Arrange
            _combatService.SetHasAction(true);
            _combatService.SetActionPossible(true);
            var screenPosition = new Vector3(100, 200, 0);

            // Act
            _presenter.Display(screenPosition, isAttackPhase: true);

            // Assert
            Assert.IsFalse(_view.ActionButtonVisible);
        }

        [Test]
        public void Display_WithActionNotPossible_ShowsDisabledActionButton()
        {
            // Arrange
            _combatService.SetHasAction(true);
            _combatService.SetActionPossible(false);
            var screenPosition = new Vector3(100, 200, 0);

            // Act
            _presenter.Display(screenPosition, isAttackPhase: false);

            // Assert
            Assert.IsTrue(_view.ActionButtonVisible);
            Assert.IsFalse(_view.ActionButtonInteractable);
        }

        [Test]
        public void Display_WithNoAction_HidesActionButton()
        {
            // Arrange
            _combatService.SetHasAction(false);
            var screenPosition = new Vector3(100, 200, 0);

            // Act
            _presenter.Display(screenPosition, isAttackPhase: false);

            // Assert
            Assert.IsFalse(_view.ActionButtonVisible);
        }

        [Test]
        public void Hide_HidesMenu()
        {
            // Act
            _presenter.Hide();

            // Assert
            Assert.IsFalse(_view.IsMenuVisible);
        }

        [Test]
        public void UpdateAttackButton_WithCanAttack_EnablesButton()
        {
            // Arrange
            _combatService.SetCanAttack(true);

            // Act
            _presenter.UpdateAttackButton();

            // Assert
            Assert.IsTrue(_view.AttackButtonVisible);
            Assert.IsTrue(_view.AttackButtonInteractable);
        }

        [Test]
        public void UpdateAttackButton_WithCannotAttack_DisablesButton()
        {
            // Arrange
            _combatService.SetCanAttack(false);

            // Act
            _presenter.UpdateAttackButton();

            // Assert
            Assert.IsTrue(_view.AttackButtonVisible);
            Assert.IsFalse(_view.AttackButtonInteractable);
        }

        [Test]
        public void EnableAttackButton_CallsViewMethod()
        {
            // Act
            _presenter.EnableAttackButton();

            // Assert
            Assert.IsTrue(_view.EnableAttackButtonCalled);
        }
    }

    #region Test Doubles

    /// <summary>
    /// Test double for IInvocationMenuView.
    /// Tracks all method calls for verification.
    /// </summary>
    internal class TestInvocationMenuView : IInvocationMenuView
    {
        public bool IsMenuVisible { get; private set; }
        public Vector3 MenuPosition { get; private set; }
        public bool AttackButtonVisible { get; private set; }
        public bool AttackButtonInteractable { get; private set; }
        public bool ActionButtonVisible { get; private set; }
        public bool ActionButtonInteractable { get; private set; }
        public bool EnableAttackButtonCalled { get; private set; }

        public void ShowMenu(Vector3 screenPosition)
        {
            IsMenuVisible = true;
            MenuPosition = screenPosition;
        }

        public void HideMenu()
        {
            IsMenuVisible = false;
        }

        public void SetAttackButtonState(bool visible, bool interactable)
        {
            AttackButtonVisible = visible;
            AttackButtonInteractable = interactable;
        }

        public void SetActionButtonState(bool visible, bool interactable)
        {
            ActionButtonVisible = visible;
            ActionButtonInteractable = interactable;
        }

        public void EnableAttackButton()
        {
            EnableAttackButtonCalled = true;
        }
    }

    /// <summary>
    /// Test double for ICombatQueryService.
    /// Provides configurable combat state for testing.
    /// </summary>
    internal class TestCombatQueryService : ICombatQueryService
    {
        private bool _canAttack;
        private bool _hasAction;
        private bool _actionPossible;

        // Test configuration methods
        public void SetCanAttack(bool canAttack) => _canAttack = canAttack;
        public void SetHasAction(bool hasAction) => _hasAction = hasAction;
        public void SetActionPossible(bool actionPossible) => _actionPossible = actionPossible;

        // ICombatQueryService implementation
        public bool CanAttackerAttack() => _canAttack;
        public bool HasAttackerAction() => _hasAction;
        public bool IsSpecialActionPossible() => _actionPossible;
    }

    #endregion
}
