using NUnit.Framework;
using JDG.Presentation.Views;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using System.Collections.Generic;

/// <summary>
/// Unit tests for RoundDisplayPresenter.
/// Part of Phase 28 - MonoBehaviour Wave 1 MVP migration.
/// Tests in default assembly to access presenters which depend on legacy services.
/// </summary>
[TestFixture]
public class RoundDisplayPresenterTests
{
    private RoundDisplayPresenter _presenter;
    private TestRoundDisplayView _view;
    private TestEventBus _eventBus;
    private TestLocalizationService _localizationService;

    [SetUp]
    public void SetUp()
    {
        _view = new TestRoundDisplayView();
        _eventBus = new TestEventBus();
        _localizationService = new TestLocalizationService();
        _presenter = new RoundDisplayPresenter(_view, _eventBus, _localizationService);
    }

    [Test]
    public void Initialize_SetsInitialDisplay()
    {
        // Arrange & Act
        _presenter.Initialize(PlayerId.Player1, Phase.Draw);

        // Assert
        Assert.AreEqual(1, _view.SetRoundTextCalls.Count);
        Assert.AreEqual("PHASE_DRAW", _view.SetRoundTextCalls[0]);
    }

    [Test]
    public void AdaptUIToNextRound_EndPhase_ShowsInHandButton()
    {
        // Arrange
        _presenter.Initialize(PlayerId.Player1, Phase.End);

        // Act
        _presenter.AdaptUIToNextRound(shouldRotateCamera: false);

        // Assert
        Assert.IsTrue(_view.InHandButtonVisible);
        Assert.AreEqual("PHASE_DRAW", _view.LastRoundText);
    }

    [Test]
    public void AdaptUIToNextRound_EndPhase_RotatesCameraWhenRequested()
    {
        // Arrange
        _presenter.Initialize(PlayerId.Player1, Phase.End);

        // Act
        _presenter.AdaptUIToNextRound(shouldRotateCamera: true);

        // Assert
        Assert.IsTrue(_view.CameraRotated);
    }

    [Test]
    public void AdaptUIToNextRound_EndPhase_DoesNotRotateCameraWhenNotRequested()
    {
        // Arrange
        _presenter.Initialize(PlayerId.Player1, Phase.End);

        // Act
        _presenter.AdaptUIToNextRound(shouldRotateCamera: false);

        // Assert
        Assert.IsFalse(_view.CameraRotated);
    }

    [Test]
    public void AdaptUIToNextRound_AttackPhase_HidesInHandButton()
    {
        // Arrange
        _presenter.Initialize(PlayerId.Player1, Phase.Attack);

        // Act
        _presenter.AdaptUIToNextRound(shouldRotateCamera: false);

        // Assert
        Assert.IsFalse(_view.InHandButtonVisible);
        Assert.AreEqual("PHASE_ATTACK", _view.LastRoundText);
    }

    [Test]
    public void OnPhaseChanged_UpdatesDisplay()
    {
        // Arrange
        _presenter.Initialize(PlayerId.Player1, Phase.Draw);
        _view.SetRoundTextCalls.Clear(); // Clear initial call

        // Act
        _eventBus.Publish(new PhaseChangedEvent { NewPhase = Phase.Choose });

        // Assert
        Assert.AreEqual(1, _view.SetRoundTextCalls.Count);
        Assert.AreEqual("PHASE_CHOOSE", _view.SetRoundTextCalls[0]);
    }

    [Test]
    public void OnPlayerTurnChanged_UpdatesPlayerText()
    {
        // Arrange
        _presenter.Initialize(PlayerId.Player1, Phase.Draw);
        _view.SetPlayerTurnTextCalls.Clear(); // Clear initial call

        // Act
        _eventBus.Publish(new PlayerTurnChangedEvent { NewPlayer = JDG.Domain.Enums.CardOwner.Player2 });

        // Assert
        Assert.AreEqual(1, _view.SetPlayerTurnTextCalls.Count);
        // When player 2's turn, display shows player 1 (opposite logic)
        Assert.AreEqual("PLAYER_ONE", _view.SetPlayerTurnTextCalls[0]);
    }
}

#region Test Doubles

/// <summary>
/// Test double for IRoundDisplayView.
/// Tracks all method calls for verification.
/// </summary>
public class TestRoundDisplayView : IRoundDisplayView
{
    public List<string> SetRoundTextCalls { get; } = new List<string>();
    public List<string> SetPlayerTurnTextCalls { get; } = new List<string>();
    public string LastRoundText { get; private set; }
    public string LastPlayerTurnText { get; private set; }
    public bool InHandButtonVisible { get; private set; }
    public bool CameraRotated { get; private set; }

    public void SetRoundText(string text)
    {
        SetRoundTextCalls.Add(text);
        LastRoundText = text;
    }

    public void SetPlayerTurnText(string playerName)
    {
        SetPlayerTurnTextCalls.Add(playerName);
        LastPlayerTurnText = playerName;
    }

    public void SetInHandButtonVisible(bool visible)
    {
        InHandButtonVisible = visible;
    }

    public void RotateCamera()
    {
        CameraRotated = true;
    }
}

/// <summary>
/// Test double for IEventBus that tracks published events
/// and triggers subscribed handlers.
/// </summary>
public class TestEventBus : IEventBus
{
    private readonly Dictionary<System.Type, List<object>> _subscriptions = new Dictionary<System.Type, List<object>>();
    public List<object> PublishedEvents { get; } = new List<object>();

    public void Publish<T>(T eventData) where T : struct
    {
        PublishedEvents.Add(eventData);

        var eventType = typeof(T);
        if (_subscriptions.ContainsKey(eventType))
        {
            foreach (var handler in _subscriptions[eventType])
            {
                ((System.Action<T>)handler)(eventData);
            }
        }
    }

    public System.IDisposable Subscribe<T>(System.Action<T> handler) where T : struct
    {
        var eventType = typeof(T);
        if (!_subscriptions.ContainsKey(eventType))
        {
            _subscriptions[eventType] = new List<object>();
        }
        _subscriptions[eventType].Add(handler);
        return new TestSubscription(() => _subscriptions[eventType].Remove(handler));
    }

    public void ClearSubscriptions<T>() where T : struct
    {
        var eventType = typeof(T);
        if (_subscriptions.ContainsKey(eventType))
        {
            _subscriptions[eventType].Clear();
        }
    }

    public void ClearAllSubscriptions()
    {
        _subscriptions.Clear();
        PublishedEvents.Clear();
    }

    private class TestSubscription : System.IDisposable
    {
        private readonly System.Action _unsubscribe;

        public TestSubscription(System.Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            _unsubscribe();
        }
    }
}

/// <summary>
/// Test double for ILocalizationService.
/// Returns the key itself for simplicity.
/// </summary>
public class TestLocalizationService : ILocalizationService
{
    public string GetLocalizedValue(string key)
    {
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
        return true; // All keys exist in test
    }
}

#endregion
