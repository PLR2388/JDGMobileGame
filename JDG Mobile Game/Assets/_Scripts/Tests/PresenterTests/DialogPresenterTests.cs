using NUnit.Framework;
using JDG.Application.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unit tests for DialogPresenter.
/// Part of Phase 39+ - Test coverage improvement sprint.
/// Phase 40: Updated to use IDialogService injection for comprehensive testing.
/// </summary>
[TestFixture]
public class DialogPresenterTests
{
    private DialogPresenter _presenter;
    private TestLocalizationServiceForDialog _localizationService;
    private MockDialogService _dialogService;
    private GameObject _canvasObject;
    private Transform _canvas;

    [SetUp]
    public void SetUp()
    {
        _canvasObject = new GameObject("TestCanvas");
        _canvas = _canvasObject.transform;
        _localizationService = new TestLocalizationServiceForDialog();
        _dialogService = new MockDialogService();
        _presenter = new DialogPresenter(_canvas, _localizationService, _dialogService);
    }

    [TearDown]
    public void TearDown()
    {
        if (_canvasObject != null)
        {
            UnityEngine.Object.DestroyImmediate(_canvasObject);
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
        var presenter = new DialogPresenter(null, _localizationService, _dialogService);

        // Assert
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithNullLocalizationService_DoesNotThrow()
    {
        // Arrange & Act & Assert
        // This will throw NullReferenceException when methods are called,
        // but construction should succeed
        var presenter = new DialogPresenter(_canvas, null, _dialogService);
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithNullDialogService_DoesNotThrow()
    {
        // Arrange & Act & Assert
        // This will throw NullReferenceException when methods are called,
        // but construction should succeed
        var presenter = new DialogPresenter(_canvas, _localizationService, null);
        Assert.IsNotNull(presenter);
    }

    #endregion

    #region ShowPauseMenu Tests
    // Phase 40: Now using IDialogService injection for comprehensive testing

    [Test]
    public void ShowPauseMenu_CallsLocalizationService()
    {
        // Arrange
        bool wasCalled = false;
        UnityAction testAction = () => wasCalled = true;

        // Act
        _presenter.ShowPauseMenu(testAction);

        // Assert - Verify localization was called
        Assert.IsTrue(_localizationService.GetLocalizedValueCalled);
    }

    [Test]
    public void ShowPauseMenu_CallsDialogService()
    {
        // Arrange
        UnityAction testAction = () => { };

        // Act
        _presenter.ShowPauseMenu(testAction);

        // Assert
        Assert.IsTrue(_dialogService.ShowMessageBoxLegacyCalled);
        Assert.AreEqual(1, _dialogService.ShowMessageBoxLegacyCallCount);
    }

    [Test]
    public void ShowPauseMenu_PassesCanvasToDialogService()
    {
        // Arrange
        UnityAction testAction = () => { };

        // Act
        _presenter.ShowPauseMenu(testAction);

        // Assert
        Assert.AreEqual(_canvas, _dialogService.LastCanvas);
    }

    [Test]
    public void ShowPauseMenu_PassesCorrectConfigType()
    {
        // Arrange
        UnityAction testAction = () => { };

        // Act
        _presenter.ShowPauseMenu(testAction);

        // Assert
        Assert.IsInstanceOf<MessageBoxConfig>(_dialogService.LastConfig);
    }

    #endregion

    #region ShowMessageBox Tests
    // Phase 40: Now using IDialogService injection for comprehensive testing

    [Test]
    public void ShowMessageBox_WithNullActions_CallsDialogService()
    {
        // Act
        _presenter.ShowMessageBox("Title", "Message", null, null);

        // Assert
        Assert.IsTrue(_dialogService.ShowMessageBoxLegacyCalled);
    }

    [Test]
    public void ShowMessageBox_WithPositiveAction_SetsShowPositiveButton()
    {
        // Arrange
        UnityAction positiveAction = () => { };

        // Act
        _presenter.ShowMessageBox("Title", "Message", positiveAction, null);

        // Assert
        var config = _dialogService.LastConfig as MessageBoxConfig;
        Assert.IsNotNull(config);
        Assert.IsTrue(config.ShowPositiveButton);
        Assert.IsFalse(config.ShowNegativeButton);
    }

    [Test]
    public void ShowMessageBox_WithNegativeAction_SetsShowNegativeButton()
    {
        // Arrange
        UnityAction negativeAction = () => { };

        // Act
        _presenter.ShowMessageBox("Title", "Message", null, negativeAction);

        // Assert
        var config = _dialogService.LastConfig as MessageBoxConfig;
        Assert.IsNotNull(config);
        Assert.IsFalse(config.ShowPositiveButton);
        Assert.IsTrue(config.ShowNegativeButton);
    }

    [Test]
    public void ShowMessageBox_WithNoActions_SetsShowOkButton()
    {
        // Act
        _presenter.ShowMessageBox("Title", "Message", null, null);

        // Assert
        var config = _dialogService.LastConfig as MessageBoxConfig;
        Assert.IsNotNull(config);
        Assert.IsTrue(config.ShowOkButton);
    }

    [Test]
    public void ShowMessageBox_SetsCorrectTitleAndMessage()
    {
        // Act
        _presenter.ShowMessageBox("Test Title", "Test Message", null, null);

        // Assert
        var config = _dialogService.LastConfig as MessageBoxConfig;
        Assert.IsNotNull(config);
        Assert.AreEqual("Test Title", config.Title);
        Assert.AreEqual("Test Message", config.Description);
    }

    #endregion

    #region ShowWarning Tests
    // Phase 40: Now using IDialogService injection for comprehensive testing

    [Test]
    public void ShowWarning_CallsLocalizationService()
    {
        // Act
        _presenter.ShowWarning("Test warning message");

        // Assert - Verify localization was called for title
        Assert.IsTrue(_localizationService.GetLocalizedValueCalled);
    }

    [Test]
    public void ShowWarning_CallsDialogService()
    {
        // Act
        _presenter.ShowWarning("Test warning message");

        // Assert
        Assert.IsTrue(_dialogService.ShowMessageBoxLegacyCalled);
    }

    [Test]
    public void ShowWarning_SetsShowOkButton()
    {
        // Act
        _presenter.ShowWarning("Test warning message");

        // Assert
        var config = _dialogService.LastConfig as MessageBoxConfig;
        Assert.IsNotNull(config);
        Assert.IsTrue(config.ShowOkButton);
    }

    [Test]
    public void ShowWarning_SetsCorrectMessage()
    {
        // Act
        _presenter.ShowWarning("Test warning message");

        // Assert
        var config = _dialogService.LastConfig as MessageBoxConfig;
        Assert.IsNotNull(config);
        Assert.AreEqual("Test warning message", config.Description);
    }

    #endregion

    #region MessageBoxConfig Tests

    [Test]
    public void MessageBoxConfig_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        bool positiveActionCalled = false;
        bool negativeActionCalled = false;
        UnityAction positiveAction = () => positiveActionCalled = true;
        UnityAction negativeAction = () => negativeActionCalled = true;

        // Constructor order: title, description, showOkButton, okAction, showPositiveButton, positiveAction, showNegativeButton, negativeAction
        var config = new MessageBoxConfig(
            title: "Test Title",
            description: "Test Description",
            showOkButton: false,
            okAction: null,
            showPositiveButton: true,
            positiveAction: positiveAction,
            showNegativeButton: true,
            negativeAction: negativeAction
        );

        // Assert
        Assert.AreEqual("Test Title", config.Title);
        Assert.AreEqual("Test Description", config.Description);
        Assert.IsTrue(config.ShowPositiveButton);
        Assert.IsTrue(config.ShowNegativeButton);
        Assert.IsFalse(config.ShowOkButton);

        // Verify actions are preserved
        config.PositiveAction?.Invoke();
        config.NegativeAction?.Invoke();
        Assert.IsTrue(positiveActionCalled);
        Assert.IsTrue(negativeActionCalled);
    }

    [Test]
    public void MessageBoxConfig_DefaultShowOkButton_WhenNoActionButtons()
    {
        // Arrange & Act
        var config = new MessageBoxConfig(
            title: "Title",
            description: "Description",
            showOkButton: true
        );

        // Assert
        Assert.IsTrue(config.ShowOkButton);
        Assert.IsFalse(config.ShowPositiveButton);
        Assert.IsFalse(config.ShowNegativeButton);
    }

    [Test]
    public void MessageBoxConfig_WithOkAction_StoresAction()
    {
        // Arrange
        bool okActionCalled = false;
        UnityAction okAction = () => okActionCalled = true;

        // Act
        var config = new MessageBoxConfig(
            title: "Title",
            description: "Description",
            showOkButton: true,
            okAction: okAction
        );

        // Assert
        config.OkAction?.Invoke();
        Assert.IsTrue(okActionCalled);
    }

    [Test]
    public void MessageBoxConfig_InheritsFromUIConfig()
    {
        // Arrange & Act
        var config = new MessageBoxConfig("Title", "Description");

        // Assert
        Assert.IsInstanceOf<UIConfig>(config);
    }

    #endregion
}

#region Test Doubles

/// <summary>
/// Test double for ILocalizationService.
/// Tracks method calls for verification.
/// </summary>
public class TestLocalizationServiceForDialog : ILocalizationService
{
    public bool GetLocalizedValueCalled { get; private set; }
    public string LastRequestedKey { get; private set; }

    public string GetLocalizedValue(string key)
    {
        GetLocalizedValueCalled = true;
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
        LastRequestedKey = null;
    }
}

/// <summary>
/// Mock IDialogService for testing.
/// Phase 40: Added to enable comprehensive presenter testing.
/// Tracks method calls and captures parameters for verification.
/// </summary>
public class MockDialogService : IDialogService
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
