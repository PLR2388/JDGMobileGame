# Phase 15: Event-Driven Architecture Migration Guide

This guide shows how to migrate from static UnityEvents to the EventBus pattern.

## Overview

**Old Pattern (Static UnityEvents):**
```csharp
// Subscribe
GameStateManager.ChangePlayer.AddListener(OnPlayerChanged);
InputManager.OnTouch.AddListener(OnTouch);

// Invoke
GameStateManager.ChangePlayer.Invoke();
```

**New Pattern (EventBus):**
```csharp
// Subscribe
_eventBus.Subscribe<PlayerTurnChangedEvent>(OnPlayerTurnChanged);
_eventBus.Subscribe<TouchStartedEvent>(OnTouchStarted);

// Publish
_eventBus.Publish(new PlayerTurnChangedEvent { ... });
```

## Components Created in Phase 15

### 1. GameStateService
Replaces `GameStateManager` singleton with DI-friendly service.

**Old:**
```csharp
// Static singleton access
GameStateManager.Instance.SetPhase(Phase.Attack);
GameStateManager.Instance.HandleEndTurn();
var phase = GameStateManager.Instance.Phase;
```

**New:**
```csharp
// Inject dependency
public class SomeClass
{
    private readonly GameStateService _gameStateService;

    public SomeClass(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    public void DoSomething()
    {
        _gameStateService.SetPhase(Phase.Attack);
        _gameStateService.HandleEndTurn();
        var phase = _gameStateService.CurrentPhase;
    }
}
```

### 2. InputService
Already exists! Wraps `InputManager` and publishes to EventBus.

**Old:**
```csharp
// Subscribe to static events
InputManager.OnTouch.AddListener(OnTouch);
InputManager.OnLongTouch.AddListener(OnLongTouch);
InputManager.OnReleaseTouch.AddListener(OnReleaseTouch);
```

**New:**
```csharp
// Use IInputService with EventBus
private readonly IInputService _inputService;
private readonly IEventBus _eventBus;

public SomeClass(IInputService inputService, IEventBus eventBus)
{
    _inputService = inputService;
    _eventBus = eventBus;

    // Subscribe via EventBus
    _eventBus.Subscribe<TouchStartedEvent>(OnTouchStarted);
    _eventBus.Subscribe<LongTouchEvent>(OnLongTouch);
    _eventBus.Subscribe<TouchEndedEvent>(OnTouchEnded);
}

private void OnTouchStarted(TouchStartedEvent evt)
{
    Debug.Log($"Touch at {evt.Position}");
}
```

## Events Mapping

### Game State Events

| Old Event | New Event | Properties |
|-----------|-----------|------------|
| `GameStateManager.ChangePlayer` | `PlayerTurnChangedEvent` | `NewPlayer`, `TurnNumber` |
| N/A | `PhaseChangedEvent` | `OldPhase`, `NewPhase`, `TurnNumber` |
| N/A | `TurnStartEvent` | `CurrentPlayer`, `TurnNumber` |
| N/A | `TurnEndEvent` | `CurrentPlayer`, `TurnNumber` |
| N/A | `GameOverEvent` | `Winner`, `Reason` |

### Input Events

| Old Event | New Event | Properties |
|-----------|-----------|------------|
| `InputManager.OnTouch` | `TouchStartedEvent` | `Position`, `Timestamp` |
| `InputManager.OnLongTouch` | `LongTouchEvent` | `Position`, `Duration` |
| `InputManager.OnReleaseTouch` | `TouchEndedEvent` | `Position`, `Duration` |
| `InputManager.OnBackPressed` | `BackButtonPressedEvent` | `Timestamp` |

## Migration Examples

### Example 1: GameLoop.cs Migration

**Before:**
```csharp
public class GameLoop : MonoBehaviour
{
    void Start()
    {
        InputManager.OnTouch.AddListener(OnTouch);
        InputManager.OnLongTouch.AddListener(OnLongTouch);
        InputManager.OnReleaseTouch.AddListener(OnReleaseTouch);
        InputManager.OnBackPressed.AddListener(OnBackPressed);
    }

    private void OnTouch()
    {
        // Handle touch
    }
}
```

**After:**
```csharp
public class GameLoop : MonoBehaviour
{
    private IEventBus _eventBus;
    private GameStateService _gameStateService;

    // VContainer injects dependencies
    [Inject]
    public void Construct(IEventBus eventBus, GameStateService gameStateService)
    {
        _eventBus = eventBus;
        _gameStateService = gameStateService;
    }

    void Start()
    {
        _eventBus.Subscribe<TouchStartedEvent>(OnTouchStarted);
        _eventBus.Subscribe<LongTouchEvent>(OnLongTouch);
        _eventBus.Subscribe<TouchEndedEvent>(OnTouchEnded);
        _eventBus.Subscribe<BackButtonPressedEvent>(OnBackPressed);
    }

    private void OnTouchStarted(TouchStartedEvent evt)
    {
        Debug.Log($"Touch at {evt.Position.X}, {evt.Position.Y}");
    }

    private void OnLongTouch(LongTouchEvent evt)
    {
        Debug.Log($"Long touch at {evt.Position.X}, {evt.Position.Y}");
    }

    void OnDestroy()
    {
        // EventBus auto-disposes subscriptions on Dispose()
        _eventBus.Dispose();
    }
}
```

### Example 2: Changing Phase with Events

**Before:**
```csharp
public void OnNextPhaseButtonClicked()
{
    GameStateManager.Instance.NextPhase();
    // No event published automatically
}
```

**After:**
```csharp
public void OnNextPhaseButtonClicked()
{
    _gameStateService.NextPhase();
    // PhaseChangedEvent automatically published!
}

// Elsewhere, subscribe to phase changes
_eventBus.Subscribe<PhaseChangedEvent>(OnPhaseChanged);

private void OnPhaseChanged(PhaseChangedEvent evt)
{
    Debug.Log($"Phase changed: {evt.OldPhase} → {evt.NewPhase}");
    UpdateUI(evt.NewPhase);
}
```

### Example 3: End Turn Flow

**Before:**
```csharp
public void EndTurn()
{
    GameStateManager.Instance.HandleEndTurn();
    // ChangePlayer event fires, but no phase change event
}
```

**After:**
```csharp
public void EndTurn()
{
    _gameStateService.HandleEndTurn();
    // Publishes: TurnEndEvent, PlayerTurnChangedEvent, PhaseChangedEvent, TurnStartEvent
}

// Multiple subscribers can react independently
_eventBus.Subscribe<TurnEndEvent>(OnTurnEnd);
_eventBus.Subscribe<PlayerTurnChangedEvent>(OnPlayerChanged);
_eventBus.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
_eventBus.Subscribe<TurnStartEvent>(OnTurnStart);
```

## Benefits of EventBus Pattern

### 1. Type Safety
```csharp
// Old: Any number of parameters, no compile-time checking
ChangePlayer.Invoke();

// New: Strongly typed events
_eventBus.Publish(new PlayerTurnChangedEvent
{
    NewPlayer = CardOwner.Player1,
    TurnNumber = 5
});
```

### 2. Decoupling
```csharp
// Old: Tight coupling to static singletons
GameStateManager.Instance.SetPhase(Phase.Attack);

// New: Dependency injection
public SomeClass(GameStateService gameState) { ... }
```

### 3. Testability
```csharp
// Old: Hard to test - requires Unity runtime
[Test]
public void TestSomething()
{
    GameStateManager.Instance... // Null in tests!
}

// New: Easy to mock
[Test]
public void TestSomething()
{
    var mockEventBus = new Mock<IEventBus>();
    var mockGameState = new Mock<GameStateService>();
    var sut = new SomeClass(mockGameState.Object);
    // Test away!
}
```

### 4. Better Event History
```csharp
// EventBus events are structs with full data
_eventBus.Subscribe<PhaseChangedEvent>(evt =>
{
    Debug.Log($"Turn {evt.TurnNumber}: {evt.OldPhase} → {evt.NewPhase}");
    // Full context in the event!
});
```

## Migration Checklist

### Phase 15 (✅ Current)
- [x] Create GameStateService with EventBus integration
- [x] InputService already publishes to EventBus
- [x] Register services in DI container
- [x] Document migration patterns

### Phase 16 (Next)
- [ ] Migrate GameLoop.cs to use EventBus
- [ ] Migrate TutoPlayerGameLoop.cs to use EventBus
- [ ] Update all event subscribers to EventBus pattern
- [ ] Remove static UnityEvents from managers
- [ ] Deprecate old GameStateManager singleton
- [ ] Update all game logic to use services instead of singletons

## Next Steps

1. Update `GameLoop.cs` and `TutoPlayerGameLoop.cs` to inject `IEventBus` and `GameStateService`
2. Replace `AddListener()` calls with `EventBus.Subscribe<T>()`
3. Replace `GameStateManager.Instance` calls with `GameStateService`
4. Test that all events fire correctly
5. Remove old static UnityEvents once migration is complete

## Code Files

- `GameStateService.cs` - Assets/_Scripts/JDG.Infrastructure/Services/GameStateService.cs
- `InputService.cs` - Assets/_Scripts/JDG.Infrastructure/Services/InputService.cs
- `GameEvents.cs` - Assets/_Scripts/JDG.Domain/Events/GameEvents.cs
- `EventBus.cs` - Assets/_Scripts/JDG.Infrastructure/Events/EventBus.cs
