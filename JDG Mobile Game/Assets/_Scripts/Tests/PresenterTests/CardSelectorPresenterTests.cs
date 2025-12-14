using NUnit.Framework;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Application.Services;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unit tests for CardSelectorPresenter.
/// Part of Phase 39+ - Test coverage improvement sprint.
///
/// NOTE: Many tests are limited because CardSelectorPresenter uses CardSelector.Instance
/// and MessageBox.Instance directly. After Phase 40 (IDialogService injection),
/// more comprehensive tests can be added.
/// TODO: Add comprehensive tests after Phase 40 refactoring.
/// </summary>
[TestFixture]
public class CardSelectorPresenterTests
{
    private CardSelectorPresenter _presenter;
    private TestLocalizationServiceForSelector _localizationService;
    private GameObject _canvasObject;
    private GameObject _nextPhaseButton;
    private Transform _canvas;

    [SetUp]
    public void SetUp()
    {
        _canvasObject = new GameObject("TestCanvas");
        _nextPhaseButton = new GameObject("NextPhaseButton");
        _canvas = _canvasObject.transform;
        _localizationService = new TestLocalizationServiceForSelector();
        _presenter = new CardSelectorPresenter(_canvas, _nextPhaseButton, _localizationService);
    }

    [TearDown]
    public void TearDown()
    {
        if (_canvasObject != null)
        {
            Object.DestroyImmediate(_canvasObject);
        }
        if (_nextPhaseButton != null)
        {
            Object.DestroyImmediate(_nextPhaseButton);
        }
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WithValidParameters_CreatesPresenter()
    {
        // Assert
        Assert.IsNotNull(_presenter);
    }

    [Test]
    public void Constructor_WithNullCanvas_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardSelectorPresenter(null, _nextPhaseButton, _localizationService);

        // Assert
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithNullNextPhaseButton_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardSelectorPresenter(_canvas, null, _localizationService);

        // Assert
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithNullLocalizationService_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardSelectorPresenter(_canvas, _nextPhaseButton, null);

        // Assert
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithAllNullParameters_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardSelectorPresenter(null, null, null);

        // Assert
        Assert.IsNotNull(presenter);
    }

    #endregion

    #region ShowOpponentSelector Tests

    [Test]
    public void ShowOpponentSelector_WithNullTargets_ShowsWarning_WhenSingletonAvailable()
    {
        // Skip test if singletons are not available
        if (MessageBox.Instance == null)
        {
            Assert.Ignore("MessageBox singleton not available in test environment");
            return;
        }

        // Arrange
        bool callbackInvoked = false;
        UnityAction<InGameInvocationCard> onSelected = (card) => callbackInvoked = true;
        UnityAction onCancelled = () => { };

        // Act - This should show warning because no targets
        _presenter.ShowOpponentSelector(null, onSelected, onCancelled);

        // Assert - Verify localization was called (for warning title/message)
        Assert.IsTrue(_localizationService.GetLocalizedValueCalled);
    }

    [Test]
    public void ShowOpponentSelector_WithEmptyTargets_ShowsWarning_WhenSingletonAvailable()
    {
        // Skip test if singletons are not available
        if (MessageBox.Instance == null)
        {
            Assert.Ignore("MessageBox singleton not available in test environment");
            return;
        }

        // Arrange
        var emptyList = new List<InGameCard>();
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act - This should show warning because empty targets
        _presenter.ShowOpponentSelector(emptyList, onSelected, onCancelled);

        // Assert - Verify localization was called
        Assert.IsTrue(_localizationService.GetLocalizedValueCalled);
    }

    [Test]
    public void ShowOpponentSelector_HidesNextPhaseButton()
    {
        // Arrange
        _nextPhaseButton.SetActive(true);
        var emptyList = new List<InGameCard>();
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Skip full test if singleton not available
        if (MessageBox.Instance == null)
        {
            // At minimum, verify button hiding logic works
            // The method tries to hide the button before checking targets
            // Since we can't fully test without singleton, just verify initial state
            Assert.IsTrue(_nextPhaseButton.activeSelf);
            Assert.Ignore("MessageBox singleton not available - partial test only");
            return;
        }

        // Act
        _presenter.ShowOpponentSelector(emptyList, onSelected, onCancelled);

        // Assert - Button should be hidden at start
        Assert.IsFalse(_nextPhaseButton.activeSelf);
    }

    [Test]
    public void ShowOpponentSelector_WithNullNextPhaseButton_DoesNotThrow()
    {
        // Arrange
        var presenter = new CardSelectorPresenter(_canvas, null, _localizationService);
        var emptyList = new List<InGameCard>();
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Skip if singleton not available
        if (MessageBox.Instance == null)
        {
            Assert.Ignore("MessageBox singleton not available in test environment");
            return;
        }

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => presenter.ShowOpponentSelector(emptyList, onSelected, onCancelled));
    }

    #endregion

    #region CardSelectorConfig Tests

    [Test]
    public void CardSelectorConfig_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var testCards = new List<InGameCard> { new TestInGameCardForSelector() };
        bool positiveActionCalled = false;
        bool negativeActionCalled = false;
        UnityAction<InGameCard> positiveAction = (card) => positiveActionCalled = true;
        UnityAction negativeAction = () => negativeActionCalled = true;

        // Act - Constructor: title, cards, showOkButton, showPositiveButton, showNegativeButton, okAction, okMultipleAction, positiveAction, positiveMultipleAction, negativeAction, numberCardSelection, showOrder
        var config = new CardSelectorConfig(
            title: "Test Title",
            cards: testCards,
            showOkButton: false,
            showPositiveButton: true,
            showNegativeButton: true,
            okAction: null,
            okMultipleAction: null,
            positiveAction: positiveAction,
            positiveMultipleAction: null,
            negativeAction: negativeAction
        );

        // Assert
        Assert.AreEqual("Test Title", config.Title);
        Assert.AreEqual(testCards, config.Cards);
        Assert.IsTrue(config.ShowPositiveButton);
        Assert.IsTrue(config.ShowNegativeButton);
        Assert.IsFalse(config.ShowOkButton);
    }

    [Test]
    public void CardSelectorConfig_WithEmptyCards_AcceptsEmptyList()
    {
        // Arrange
        var emptyCards = new List<InGameCard>();

        // Act
        var config = new CardSelectorConfig(
            title: "Title",
            cards: emptyCards,
            showPositiveButton: true,
            showNegativeButton: false
        );

        // Assert
        Assert.IsNotNull(config.Cards);
        Assert.AreEqual(0, config.Cards.Count);
    }

    [Test]
    public void CardSelectorConfig_WithNullCards_AcceptsNull()
    {
        // Act
        var config = new CardSelectorConfig(
            title: "Title",
            cards: null,
            showPositiveButton: true,
            showNegativeButton: false
        );

        // Assert
        Assert.IsNull(config.Cards);
    }

    [Test]
    public void CardSelectorConfig_NumberCardSelection_DefaultsToOne()
    {
        // Arrange & Act
        var config = new CardSelectorConfig(
            title: "Title",
            cards: new List<InGameCard>()
        );

        // Assert
        Assert.AreEqual(1, config.NumberCardSelection);
    }

    [Test]
    public void CardSelectorConfig_NumberCardSelection_EnsuresMinimumOfOne()
    {
        // Arrange & Act - Try to set 0 or negative
        var config = new CardSelectorConfig(
            title: "Title",
            cards: new List<InGameCard>(),
            numberCardSelection: 0
        );

        // Assert - Should be clamped to 1
        Assert.AreEqual(1, config.NumberCardSelection);
    }

    [Test]
    public void CardSelectorConfig_InheritsFromUIConfig()
    {
        // Arrange & Act
        var config = new CardSelectorConfig("Title", new List<InGameCard>());

        // Assert
        Assert.IsInstanceOf<UIConfig>(config);
    }

    [Test]
    public void CardSelectorConfig_PositiveActions_StoresCardAction()
    {
        // Arrange
        bool actionCalled = false;
        UnityAction<InGameCard> positiveAction = (card) => actionCalled = true;

        // Act
        var config = new CardSelectorConfig(
            title: "Title",
            cards: new List<InGameCard>(),
            showPositiveButton: true,
            positiveAction: positiveAction
        );

        // Assert
        Assert.IsNotNull(config.PositiveActions);
        config.PositiveActions.SingleAction?.Invoke(null);
        Assert.IsTrue(actionCalled);
    }

    [Test]
    public void CardActions_Struct_StoresBothActions()
    {
        // Arrange
        bool singleCalled = false;
        bool multipleCalled = false;
        UnityAction<InGameCard> singleAction = (card) => singleCalled = true;
        UnityAction<List<InGameCard>> multipleAction = (cards) => multipleCalled = true;

        // Act
        var actions = new CardActions(singleAction, multipleAction);

        // Assert
        actions.SingleAction?.Invoke(null);
        actions.MultipleAction?.Invoke(null);
        Assert.IsTrue(singleCalled);
        Assert.IsTrue(multipleCalled);
    }

    #endregion

    #region Edge Case Tests

    [Test]
    public void ShowOpponentSelector_WithValidTargets_RequiresSingleton()
    {
        // This test documents that valid targets path requires CardSelector.Instance
        // Skip test if singletons are not available
        if (CardSelector.Instance == null || MessageBox.Instance == null)
        {
            Assert.Ignore("CardSelector or MessageBox singleton not available - this path requires singletons");
            return;
        }

        // Arrange
        var targets = new List<InGameCard> { new TestInGameCardForSelector() };
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act & Assert - Should not throw when singletons available
        Assert.DoesNotThrow(() => _presenter.ShowOpponentSelector(targets, onSelected, onCancelled));
    }

    #endregion
}

#region Test Doubles

/// <summary>
/// Test double for ILocalizationService.
/// Tracks method calls for verification.
/// </summary>
public class TestLocalizationServiceForSelector : ILocalizationService
{
    public bool GetLocalizedValueCalled { get; private set; }
    public string LastRequestedKey { get; private set; }
    public int GetLocalizedValueCallCount { get; private set; }

    public string GetLocalizedValue(string key)
    {
        GetLocalizedValueCalled = true;
        GetLocalizedValueCallCount++;
        LastRequestedKey = key;
        return key; // Return key itself for testing
    }

    public void SetLanguage(GameLanguage language)
    {
        // No-op for testing
    }

    public GameLanguage GetCurrentLanguage()
    {
        return GameLanguage.English;
    }

    public bool HasKey(string key)
    {
        return true;
    }

    public void Reset()
    {
        GetLocalizedValueCalled = false;
        GetLocalizedValueCallCount = 0;
        LastRequestedKey = null;
    }
}

/// <summary>
/// Test double for InGameCard.
/// Minimal implementation for CardSelectorPresenter testing.
/// </summary>
public class TestInGameCardForSelector : InGameCard
{
    public TestInGameCardForSelector()
    {
        title = "TestCard";
        materialCard = null; // No material needed for selection tests
    }
}

#endregion
