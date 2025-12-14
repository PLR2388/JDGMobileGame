using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Application.Services;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unit tests for CardSelectorPresenter.
/// Part of Phase 39+ - Test coverage improvement sprint.
/// Phase 40: Updated to use IDialogService injection for comprehensive testing.
/// </summary>
[TestFixture]
public class CardSelectorPresenterTests
{
    private CardSelectorPresenter _presenter;
    private TestLocalizationServiceForSelector _localizationService;
    private MockDialogServiceForSelector _dialogService;
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
        _dialogService = new MockDialogServiceForSelector();
        _presenter = new CardSelectorPresenter(_canvas, _nextPhaseButton, _localizationService, _dialogService);
    }

    [TearDown]
    public void TearDown()
    {
        if (_canvasObject != null)
        {
            UnityEngine.Object.DestroyImmediate(_canvasObject);
        }
        if (_nextPhaseButton != null)
        {
            UnityEngine.Object.DestroyImmediate(_nextPhaseButton);
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
        var presenter = new CardSelectorPresenter(null, _nextPhaseButton, _localizationService, _dialogService);

        // Assert
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithNullNextPhaseButton_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardSelectorPresenter(_canvas, null, _localizationService, _dialogService);

        // Assert
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithNullLocalizationService_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardSelectorPresenter(_canvas, _nextPhaseButton, null, _dialogService);

        // Assert
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithNullDialogService_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardSelectorPresenter(_canvas, _nextPhaseButton, _localizationService, null);

        // Assert
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithAllNullParameters_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardSelectorPresenter(null, null, null, null);

        // Assert
        Assert.IsNotNull(presenter);
    }

    #endregion

    #region ShowOpponentSelector Tests
    // Phase 40: Now using IDialogService injection for comprehensive testing

    [Test]
    public void ShowOpponentSelector_WithNullTargets_ShowsWarning()
    {
        // Arrange
        bool callbackInvoked = false;
        UnityAction<InGameInvocationCard> onSelected = (card) => callbackInvoked = true;
        UnityAction onCancelled = () => { };

        // Act - This should show warning because no targets
        _presenter.ShowOpponentSelector(null, onSelected, onCancelled);

        // Assert - Verify localization was called (for warning title/message)
        Assert.IsTrue(_localizationService.GetLocalizedValueCalled);
        // Verify ShowMessageBoxLegacy was called (for warning)
        Assert.IsTrue(_dialogService.ShowMessageBoxLegacyCalled);
    }

    [Test]
    public void ShowOpponentSelector_WithEmptyTargets_ShowsWarning()
    {
        // Arrange
        var emptyList = new List<InGameCard>();
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act - This should show warning because empty targets
        _presenter.ShowOpponentSelector(emptyList, onSelected, onCancelled);

        // Assert - Verify localization was called
        Assert.IsTrue(_localizationService.GetLocalizedValueCalled);
        // Verify ShowMessageBoxLegacy was called (for warning)
        Assert.IsTrue(_dialogService.ShowMessageBoxLegacyCalled);
    }

    [Test]
    public void ShowOpponentSelector_HidesNextPhaseButton()
    {
        // Arrange
        _nextPhaseButton.SetActive(true);
        var emptyList = new List<InGameCard>();
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act
        _presenter.ShowOpponentSelector(emptyList, onSelected, onCancelled);

        // Assert - Button should be hidden at start
        Assert.IsFalse(_nextPhaseButton.activeSelf);
    }

    [Test]
    public void ShowOpponentSelector_WithNullNextPhaseButton_DoesNotThrow()
    {
        // Arrange
        var presenter = new CardSelectorPresenter(_canvas, null, _localizationService, _dialogService);
        var emptyList = new List<InGameCard>();
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => presenter.ShowOpponentSelector(emptyList, onSelected, onCancelled));
    }

    [Test]
    public void ShowOpponentSelector_WithValidTargets_ShowsCardSelector()
    {
        // Arrange
        var targets = new List<InGameCard> { new TestInGameCardForSelector() };
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act
        _presenter.ShowOpponentSelector(targets, onSelected, onCancelled);

        // Assert - Verify ShowCardSelectorLegacy was called
        Assert.IsTrue(_dialogService.ShowCardSelectorLegacyCalled);
        Assert.IsFalse(_dialogService.ShowMessageBoxLegacyCalled); // Should NOT show warning
    }

    [Test]
    public void ShowOpponentSelector_WithValidTargets_PassesCanvasToDialogService()
    {
        // Arrange
        var targets = new List<InGameCard> { new TestInGameCardForSelector() };
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act
        _presenter.ShowOpponentSelector(targets, onSelected, onCancelled);

        // Assert
        Assert.AreEqual(_canvas, _dialogService.LastCanvas);
    }

    [Test]
    public void ShowOpponentSelector_WithValidTargets_PassesCorrectConfigType()
    {
        // Arrange
        var targets = new List<InGameCard> { new TestInGameCardForSelector() };
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act
        _presenter.ShowOpponentSelector(targets, onSelected, onCancelled);

        // Assert
        Assert.IsInstanceOf<CardSelectorConfig>(_dialogService.LastCardSelectorConfig);
    }

    [Test]
    public void ShowOpponentSelector_WithValidTargets_ConfigHasCorrectButtons()
    {
        // Arrange
        var targets = new List<InGameCard> { new TestInGameCardForSelector() };
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act
        _presenter.ShowOpponentSelector(targets, onSelected, onCancelled);

        // Assert
        var config = _dialogService.LastCardSelectorConfig as CardSelectorConfig;
        Assert.IsNotNull(config);
        Assert.IsTrue(config.ShowPositiveButton);
        Assert.IsTrue(config.ShowNegativeButton);
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
    public void ShowOpponentSelector_WithValidTargets_DoesNotThrow()
    {
        // Arrange
        var targets = new List<InGameCard> { new TestInGameCardForSelector() };
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => _presenter.ShowOpponentSelector(targets, onSelected, onCancelled));
    }

    [Test]
    public void ShowOpponentSelector_WithMultipleTargets_AllCardsPassedToConfig()
    {
        // Arrange
        var targets = new List<InGameCard>
        {
            new TestInGameCardForSelector(),
            new TestInGameCardForSelector(),
            new TestInGameCardForSelector()
        };
        UnityAction<InGameInvocationCard> onSelected = (card) => { };
        UnityAction onCancelled = () => { };

        // Act
        _presenter.ShowOpponentSelector(targets, onSelected, onCancelled);

        // Assert
        var config = _dialogService.LastCardSelectorConfig as CardSelectorConfig;
        Assert.IsNotNull(config);
        Assert.AreEqual(3, config.Cards.Count);
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

/// <summary>
/// Mock IDialogService for CardSelectorPresenter testing.
/// Phase 40: Added to enable comprehensive presenter testing.
/// Tracks method calls and captures parameters for verification.
/// </summary>
public class MockDialogServiceForSelector : IDialogService
{
    // ShowMessageBoxLegacy tracking
    public bool ShowMessageBoxLegacyCalled { get; private set; }
    public int ShowMessageBoxLegacyCallCount { get; private set; }
    public object LastCanvas { get; private set; }
    public object LastConfig { get; private set; }

    // ShowCardSelectorLegacy tracking
    public bool ShowCardSelectorLegacyCalled { get; private set; }
    public int ShowCardSelectorLegacyCallCount { get; private set; }
    public object LastCardSelectorConfig { get; private set; }

    public void ShowMessageBoxLegacy(object canvas, object config)
    {
        ShowMessageBoxLegacyCalled = true;
        ShowMessageBoxLegacyCallCount++;
        LastCanvas = canvas;
        LastConfig = config;
    }

    public void ShowCardSelectorLegacy(object canvas, object config)
    {
        ShowCardSelectorLegacyCalled = true;
        ShowCardSelectorLegacyCallCount++;
        LastCanvas = canvas;
        LastCardSelectorConfig = config;
    }

    public Task<bool> ShowMessageBoxAsync(string title, string message, MessageBoxType type)
    {
        return Task.FromResult(true);
    }

    public Task<List<Guid>> ShowCardSelectorAsync(JDG.Application.Services.CardSelectorConfig config)
    {
        return Task.FromResult<List<Guid>>(null);
    }

    public Task<bool> ShowConfirmAsync(string message)
    {
        return Task.FromResult(true);
    }

    public Task ShowInfoAsync(string message)
    {
        return Task.CompletedTask;
    }

    public void ShowMessageBox(object canvas, MessageBoxOptions options)
    {
        ShowMessageBoxLegacyCalled = true;
        ShowMessageBoxLegacyCallCount++;
        LastCanvas = canvas;
    }

    public void ShowCardSelector(object canvas, CardSelectorOptions options)
    {
        ShowCardSelectorLegacyCalled = true;
        ShowCardSelectorLegacyCallCount++;
        LastCanvas = canvas;
    }

    public void Reset()
    {
        ShowMessageBoxLegacyCalled = false;
        ShowMessageBoxLegacyCallCount = 0;
        ShowCardSelectorLegacyCalled = false;
        ShowCardSelectorLegacyCallCount = 0;
        LastCanvas = null;
        LastConfig = null;
        LastCardSelectorConfig = null;
    }
}

#endregion
