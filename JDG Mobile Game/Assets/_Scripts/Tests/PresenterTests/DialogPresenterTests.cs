using NUnit.Framework;
using JDG.Application.Services;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unit tests for DialogPresenter.
/// Part of Phase 39+ - Test coverage improvement sprint.
///
/// NOTE: Many tests are limited because DialogPresenter uses MessageBox.Instance directly.
/// After Phase 40 (IDialogService injection), more comprehensive tests can be added.
/// TODO: Add comprehensive tests after Phase 40 refactoring.
/// </summary>
[TestFixture]
public class DialogPresenterTests
{
    private DialogPresenter _presenter;
    private TestLocalizationServiceForDialog _localizationService;
    private GameObject _canvasObject;
    private Transform _canvas;

    [SetUp]
    public void SetUp()
    {
        _canvasObject = new GameObject("TestCanvas");
        _canvas = _canvasObject.transform;
        _localizationService = new TestLocalizationServiceForDialog();
        _presenter = new DialogPresenter(_canvas, _localizationService);
    }

    [TearDown]
    public void TearDown()
    {
        if (_canvasObject != null)
        {
            Object.DestroyImmediate(_canvasObject);
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
        var presenter = new DialogPresenter(null, _localizationService);

        // Assert
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithNullLocalizationService_DoesNotThrow()
    {
        // Arrange & Act & Assert
        // This will throw NullReferenceException when methods are called,
        // but construction should succeed
        var presenter = new DialogPresenter(_canvas, null);
        Assert.IsNotNull(presenter);
    }

    #endregion

    #region ShowPauseMenu Tests
    // NOTE: These tests are limited because ShowPauseMenu calls MessageBox.Instance directly
    // TODO: After Phase 40, inject IDialogService and add comprehensive tests

    [Test]
    public void ShowPauseMenu_CallsLocalizationService()
    {
        // Arrange
        bool wasCalled = false;
        UnityAction testAction = () => wasCalled = true;

        // Skip test if MessageBox singleton is not available
        if (MessageBox.Instance == null)
        {
            Assert.Ignore("MessageBox singleton not available in test environment");
            return;
        }

        // Act
        _presenter.ShowPauseMenu(testAction);

        // Assert - Verify localization was called
        Assert.IsTrue(_localizationService.GetLocalizedValueCalled);
    }

    #endregion

    #region ShowMessageBox Tests
    // NOTE: These tests are limited because ShowMessageBox calls MessageBox.Instance directly
    // TODO: After Phase 40, inject IDialogService and add comprehensive tests

    [Test]
    public void ShowMessageBox_WithNullActions_DoesNotThrow_WhenSingletonAvailable()
    {
        // Skip test if MessageBox singleton is not available
        if (MessageBox.Instance == null)
        {
            Assert.Ignore("MessageBox singleton not available in test environment");
            return;
        }

        // Act & Assert
        Assert.DoesNotThrow(() => _presenter.ShowMessageBox("Title", "Message", null, null));
    }

    #endregion

    #region ShowWarning Tests
    // NOTE: These tests are limited because ShowWarning calls MessageBox.Instance directly
    // TODO: After Phase 40, inject IDialogService and add comprehensive tests

    [Test]
    public void ShowWarning_CallsLocalizationService_WhenSingletonAvailable()
    {
        // Skip test if MessageBox singleton is not available
        if (MessageBox.Instance == null)
        {
            Assert.Ignore("MessageBox singleton not available in test environment");
            return;
        }

        // Act
        _presenter.ShowWarning("Test warning message");

        // Assert - Verify localization was called for title
        Assert.IsTrue(_localizationService.GetLocalizedValueCalled);
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

#endregion
