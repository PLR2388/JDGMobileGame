# Presentation Layer - MVP Pattern

This layer implements the Model-View-Presenter (MVP) pattern for UI components.

## Architecture

### Views (Interfaces)
- `IGameView` - Main game screen
- `ICardHandView` - Player's hand of cards
- `ICardFieldView` - Cards on the field
- `IPlayerStatusView` - Player health/shields display

Views define **what** the UI can display and respond to, but not **how**.

### Presenters
- `GamePresenter` - Main game flow orchestration
- `CardHandPresenter` - Hand interaction logic
- `PlayerStatusPresenter` - Player status updates

Presenters handle **business logic** and **event subscriptions**. They:
- Subscribe to domain events via EventBus
- Call use cases to execute actions
- Update views based on state changes
- Are completely Unity-agnostic (pure C#)

### MonoBehaviours (View Implementations)
- `GameViewController` - Unity implementation of IGameView
- `PlayerStatusViewController` - Unity implementation of IPlayerStatusView

MonoBehaviours implement view interfaces and:
- Connect Unity UI elements (buttons, text, sliders)
- Create presenters with dependencies from ServiceLocator
- Delegate user input to presenters
- Update Unity UI based on presenter commands

## Usage Pattern

```csharp
// 1. MonoBehaviour implements IView
public class MyViewController : MonoBehaviour, IMyView
{
    private MyPresenter _presenter;

    void Start()
    {
        // 2. Get dependencies from DI
        var deps = ServiceLocator.Get<...>();

        // 3. Create presenter
        _presenter = new MyPresenter(this, deps...);
    }

    // 4. Implement IView methods
    public void ShowData(DataDTO data)
    {
        _myText.text = data.Value;
    }

    // 5. Forward user input to presenter
    void OnButtonClicked()
    {
        _presenter?.HandleButtonClick();
    }

    void OnDestroy()
    {
        _presenter?.Dispose(); // Unsubscribe from events
    }
}
```

## Benefits

✅ **Testable** - Presenters are pure C# (no Unity dependencies)
✅ **Separation of Concerns** - UI logic separate from Unity components
✅ **Event-Driven** - Reactive to domain events
✅ **Reusable** - Views can be swapped (e.g., different UI frameworks)
✅ **Maintainable** - Clear responsibilities

## Migration Strategy

1. **Create View Interface** - Define what UI must display
2. **Create Presenter** - Implement logic and event handling
3. **Create MonoBehaviour** - Implement view interface
4. **Wire Up** - Connect in Unity inspector
5. **Test** - Unit test presenter without Unity

## Next Steps

- Create remaining view implementations (CardHandView, CardFieldView)
- Add animation controllers
- Implement card dragging/selection
- Add UI transitions and effects
