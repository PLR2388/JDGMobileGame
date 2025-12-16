using NUnit.Framework;
using JDG.Application.Services;
using JDG.Presentation.Presenters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace JDG.Presentation.Tests.Presenters
{
    /// <summary>
    /// Unit tests for DialogPresenter.
    /// Phase 45: Moved to JDG.Presentation.Tests assembly.
    /// Tests the presenter's ability to show dialogs using IDialogService.
    /// </summary>
    [TestFixture]
    public class DialogPresenterTests
    {
        private DialogPresenter _presenter;
        private TestLocalizationService _localizationService;
        private MockDialogService _dialogService;
        private GameObject _canvasObject;
        private Transform _canvas;

        [SetUp]
        public void SetUp()
        {
            _canvasObject = new GameObject("TestCanvas");
            _canvas = _canvasObject.transform;
            _localizationService = new TestLocalizationService();
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
            var presenter = new DialogPresenter(_canvas, null, _dialogService);
            Assert.IsNotNull(presenter);
        }

        [Test]
        public void Constructor_WithNullDialogService_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var presenter = new DialogPresenter(_canvas, _localizationService, null);
            Assert.IsNotNull(presenter);
        }

        #endregion

        #region ShowPauseMenu Tests

        [Test]
        public void ShowPauseMenu_CallsLocalizationService()
        {
            // Arrange
            Action testAction = () => { };

            // Act
            _presenter.ShowPauseMenu(testAction);

            // Assert - Verify localization was called
            Assert.IsTrue(_localizationService.GetLocalizedValueCalled);
        }

        [Test]
        public void ShowPauseMenu_CallsDialogService()
        {
            // Arrange
            Action testAction = () => { };

            // Act
            _presenter.ShowPauseMenu(testAction);

            // Assert
            Assert.IsTrue(_dialogService.ShowMessageBoxCalled);
            Assert.AreEqual(1, _dialogService.ShowMessageBoxCallCount);
        }

        [Test]
        public void ShowPauseMenu_PassesCanvasToDialogService()
        {
            // Arrange
            Action testAction = () => { };

            // Act
            _presenter.ShowPauseMenu(testAction);

            // Assert
            Assert.AreEqual(_canvas, _dialogService.LastCanvas);
        }

        [Test]
        public void ShowPauseMenu_PassesCorrectOptions()
        {
            // Arrange
            Action testAction = () => { };

            // Act
            _presenter.ShowPauseMenu(testAction);

            // Assert
            Assert.IsNotNull(_dialogService.LastOptions);
            Assert.IsTrue(_dialogService.LastOptions.ShowPositiveButton);
            Assert.IsTrue(_dialogService.LastOptions.ShowNegativeButton);
        }

        #endregion

        #region ShowMessageBox Tests

        [Test]
        public void ShowMessageBox_WithNullActions_CallsDialogService()
        {
            // Act
            _presenter.ShowMessageBox("Title", "Message", null, null);

            // Assert
            Assert.IsTrue(_dialogService.ShowMessageBoxCalled);
        }

        [Test]
        public void ShowMessageBox_WithPositiveAction_SetsShowPositiveButton()
        {
            // Arrange
            Action positiveAction = () => { };

            // Act
            _presenter.ShowMessageBox("Title", "Message", positiveAction, null);

            // Assert
            Assert.IsNotNull(_dialogService.LastOptions);
            Assert.IsTrue(_dialogService.LastOptions.ShowPositiveButton);
            Assert.IsFalse(_dialogService.LastOptions.ShowNegativeButton);
        }

        [Test]
        public void ShowMessageBox_WithNegativeAction_SetsShowNegativeButton()
        {
            // Arrange
            Action negativeAction = () => { };

            // Act
            _presenter.ShowMessageBox("Title", "Message", null, negativeAction);

            // Assert
            Assert.IsNotNull(_dialogService.LastOptions);
            Assert.IsFalse(_dialogService.LastOptions.ShowPositiveButton);
            Assert.IsTrue(_dialogService.LastOptions.ShowNegativeButton);
        }

        [Test]
        public void ShowMessageBox_WithNoActions_SetsShowOkButton()
        {
            // Act
            _presenter.ShowMessageBox("Title", "Message", null, null);

            // Assert
            Assert.IsNotNull(_dialogService.LastOptions);
            Assert.IsTrue(_dialogService.LastOptions.ShowOkButton);
        }

        [Test]
        public void ShowMessageBox_SetsCorrectTitleAndMessage()
        {
            // Act
            _presenter.ShowMessageBox("Test Title", "Test Message", null, null);

            // Assert
            Assert.IsNotNull(_dialogService.LastOptions);
            Assert.AreEqual("Test Title", _dialogService.LastOptions.Title);
            Assert.AreEqual("Test Message", _dialogService.LastOptions.Message);
        }

        #endregion

        #region ShowWarning Tests

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
            Assert.IsTrue(_dialogService.ShowMessageBoxCalled);
        }

        [Test]
        public void ShowWarning_SetsShowOkButton()
        {
            // Act
            _presenter.ShowWarning("Test warning message");

            // Assert
            Assert.IsNotNull(_dialogService.LastOptions);
            Assert.IsTrue(_dialogService.LastOptions.ShowOkButton);
        }

        [Test]
        public void ShowWarning_SetsCorrectMessage()
        {
            // Act
            _presenter.ShowWarning("Test warning message");

            // Assert
            Assert.IsNotNull(_dialogService.LastOptions);
            Assert.AreEqual("Test warning message", _dialogService.LastOptions.Message);
        }

        #endregion

        #region MessageBoxOptions Tests

        [Test]
        public void MessageBoxOptions_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            bool positiveActionCalled = false;
            bool negativeActionCalled = false;
            Action positiveAction = () => positiveActionCalled = true;
            Action negativeAction = () => negativeActionCalled = true;

            var options = new MessageBoxOptions
            {
                Title = "Test Title",
                Message = "Test Description",
                ShowOkButton = false,
                ShowPositiveButton = true,
                ShowNegativeButton = true,
                OnPositive = positiveAction,
                OnNegative = negativeAction
            };

            // Assert
            Assert.AreEqual("Test Title", options.Title);
            Assert.AreEqual("Test Description", options.Message);
            Assert.IsTrue(options.ShowPositiveButton);
            Assert.IsTrue(options.ShowNegativeButton);
            Assert.IsFalse(options.ShowOkButton);

            // Verify actions are preserved
            options.OnPositive?.Invoke();
            options.OnNegative?.Invoke();
            Assert.IsTrue(positiveActionCalled);
            Assert.IsTrue(negativeActionCalled);
        }

        [Test]
        public void MessageBoxOptions_DefaultShowOkButton_WhenNoActionButtons()
        {
            // Arrange & Act
            var options = new MessageBoxOptions
            {
                Title = "Title",
                Message = "Description",
                ShowOkButton = true
            };

            // Assert
            Assert.IsTrue(options.ShowOkButton);
            Assert.IsFalse(options.ShowPositiveButton);
            Assert.IsFalse(options.ShowNegativeButton);
        }

        [Test]
        public void MessageBoxOptions_WithOkAction_StoresAction()
        {
            // Arrange
            bool okActionCalled = false;
            Action okAction = () => okActionCalled = true;

            // Act
            var options = new MessageBoxOptions
            {
                Title = "Title",
                Message = "Description",
                ShowOkButton = true,
                OnOk = okAction
            };

            // Assert
            options.OnOk?.Invoke();
            Assert.IsTrue(okActionCalled);
        }

        #endregion
    }

    #region Test Doubles

    /// <summary>
    /// Test double for ILocalizationService.
    /// Tracks method calls for verification.
    /// </summary>
    internal class TestLocalizationService : ILocalizationService
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
    /// Tracks ShowMessageBox calls with MessageBoxOptions.
    /// </summary>
    internal class MockDialogService : IDialogService
    {
        // ShowMessageBox tracking
        public bool ShowMessageBoxCalled { get; private set; }
        public int ShowMessageBoxCallCount { get; private set; }
        public object LastCanvas { get; private set; }
        public MessageBoxOptions LastOptions { get; private set; }

        // ShowCardSelector tracking
        public bool ShowCardSelectorCalled { get; private set; }
        public int ShowCardSelectorCallCount { get; private set; }
        public CardSelectorOptions LastCardSelectorOptions { get; private set; }

        public void ShowMessageBox(object canvas, MessageBoxOptions options)
        {
            ShowMessageBoxCalled = true;
            ShowMessageBoxCallCount++;
            LastCanvas = canvas;
            LastOptions = options;
        }

        public void ShowCardSelector(object canvas, CardSelectorOptions options)
        {
            ShowCardSelectorCalled = true;
            ShowCardSelectorCallCount++;
            LastCanvas = canvas;
            LastCardSelectorOptions = options;
        }

        public void ShowMessageBoxLegacy(object canvas, object config)
        {
            ShowMessageBoxCalled = true;
            ShowMessageBoxCallCount++;
            LastCanvas = canvas;
        }

        public void ShowCardSelectorLegacy(object canvas, object config)
        {
            ShowCardSelectorCalled = true;
            ShowCardSelectorCallCount++;
            LastCanvas = canvas;
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

        public void Reset()
        {
            ShowMessageBoxCalled = false;
            ShowMessageBoxCallCount = 0;
            ShowCardSelectorCalled = false;
            ShowCardSelectorCallCount = 0;
            LastCanvas = null;
            LastOptions = null;
            LastCardSelectorOptions = null;
        }
    }

    #endregion
}
