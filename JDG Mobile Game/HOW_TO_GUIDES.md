# How-To Guides - JDG Mobile Game

Step-by-step procedures for common development tasks.

## Table of Contents

1. [How to Add a New Card Ability](#how-to-add-a-new-card-ability)
2. [How to Create a New Use Case](#how-to-create-a-new-use-case)
3. [How to Add a New Presenter](#how-to-add-a-new-presenter)
4. [How to Write Tests](#how-to-write-tests)
5. [How to Add a New Domain Event](#how-to-add-a-new-domain-event)
6. [How to Register a Service in DI](#how-to-register-a-service-in-di)

---

## How to Add a New Card Ability

### Step 1: Define the Ability Name

Add your ability to the `AbilityName` enum in `JDG.Domain/AbilityName.cs`:

```csharp
public enum AbilityName
{
    // ... existing abilities ...
    MyNewAbility = 71,  // Use next available number
}
```

### Step 2: Create the Ability Implementation

Create a new class in the appropriate factory file under `JDG.Application/Abilities/Implementations/`:

```csharp
// In an existing factory file, e.g., EffectAbility.cs
public class MyNewAbility : IAbility
{
    private readonly IEventBus _eventBus;
    private readonly IPlayerRepository _playerRepository;

    public MyNewAbility(IEventBus eventBus, IPlayerRepository playerRepository)
    {
        _eventBus = eventBus;
        _playerRepository = playerRepository;
    }

    public AbilityName Name => AbilityName.MyNewAbility;
    public string Description => "Description of what this ability does";

    public bool CanActivate(AbilityContext context)
    {
        // Return true if ability can be activated
        return context.OwnerCards != null;
    }

    public AbilityResult Execute(AbilityContext context)
    {
        // Implement ability logic
        // Use repositories to modify game state
        // Publish events for side effects

        return AbilityResult.Success("Ability executed successfully");
    }
}
```

### Step 3: Register in the Factory

Add a factory method in the appropriate factory class:

```csharp
// In the factory class (e.g., EffectAbilityFactory.cs)
public class EffectAbilityFactory
{
    private readonly IEventBus _eventBus;
    private readonly IPlayerRepository _playerRepository;

    public EffectAbilityFactory(IEventBus eventBus, IPlayerRepository playerRepository)
    {
        _eventBus = eventBus;
        _playerRepository = playerRepository;
    }

    public IAbility CreateMyNewAbility()
    {
        return new MyNewAbility(_eventBus, _playerRepository);
    }
}
```

### Step 4: Register in AbilityRegistry

Add the registration in `GameLifetimeScope.cs` or where abilities are registered:

```csharp
// In RegisterAbilities() method
registry.Register(AbilityName.MyNewAbility, () =>
    effectAbilityFactory.CreateMyNewAbility());
```

### Step 5: Write Tests

Create tests in `JDG.Application.Tests/Abilities/`:

```csharp
[TestFixture]
public class MyNewAbilityTests
{
    private MyNewAbility _ability;
    private IEventBus _eventBus;
    private IPlayerRepository _playerRepository;

    [SetUp]
    public void SetUp()
    {
        _eventBus = Substitute.For<IEventBus>();
        _playerRepository = Substitute.For<IPlayerRepository>();
        _ability = new MyNewAbility(_eventBus, _playerRepository);
    }

    [Test]
    public void Execute_WhenConditionMet_ReturnsSuccess()
    {
        // Arrange
        var context = new AbilityContext { /* ... */ };

        // Act
        var result = _ability.Execute(context);

        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
}
```

---

## How to Create a New Use Case

### Step 1: Create the Use Case Class

Create a new file in `JDG.Application/UseCases/`:

```csharp
using JDG.Application.Repositories;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for [describe what it does].
    /// </summary>
    public class MyNewUseCase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public MyNewUseCase(
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        public MyNewResult Execute(MyNewInput input)
        {
            // 1. Validate input
            if (input.PlayerId == null)
                return MyNewResult.Failure("Player ID required");

            // 2. Get entities from repositories
            var player = _playerRepository.GetPlayer(input.PlayerId);

            // 3. Execute business logic
            // ... your logic here ...

            // 4. Save changes
            _playerRepository.SavePlayer(player);

            // 5. Publish events
            _eventBus.Publish(new MyNewEvent { /* ... */ });

            // 6. Return result
            return MyNewResult.Success();
        }
    }

    public class MyNewInput
    {
        public PlayerId PlayerId { get; set; }
        // Add other input parameters
    }

    public class MyNewResult
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }

        public static MyNewResult Success() => new MyNewResult
        {
            IsSuccess = true,
            Message = "Success"
        };

        public static MyNewResult Failure(string message) => new MyNewResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}
```

### Step 2: Register in DI Container

Add registration in `SharedServicesScope.cs`:

```csharp
// In Configure() method
builder.Register<MyNewUseCase>(Lifetime.Transient);
```

### Step 3: Write Tests

Create tests in `JDG.Application.Tests/UseCases/`:

```csharp
[TestFixture]
public class MyNewUseCaseTests
{
    private MyNewUseCase _useCase;
    private IPlayerRepository _playerRepository;
    private IEventBus _eventBus;

    [SetUp]
    public void SetUp()
    {
        _playerRepository = Substitute.For<IPlayerRepository>();
        _eventBus = Substitute.For<IEventBus>();
        _useCase = new MyNewUseCase(_playerRepository, _eventBus);
    }

    [Test]
    public void Execute_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        var player = TestFixtures.PlayerFactory.CreatePlayer1();
        _playerRepository.GetPlayer(Arg.Any<PlayerId>()).Returns(player);

        var input = new MyNewInput { PlayerId = player.Id };

        // Act
        var result = _useCase.Execute(input);

        // Assert
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public void Execute_PublishesEvent()
    {
        // Arrange
        var player = TestFixtures.PlayerFactory.CreatePlayer1();
        _playerRepository.GetPlayer(Arg.Any<PlayerId>()).Returns(player);

        var input = new MyNewInput { PlayerId = player.Id };

        // Act
        _useCase.Execute(input);

        // Assert
        _eventBus.Received(1).Publish(Arg.Any<MyNewEvent>());
    }
}
```

---

## How to Add a New Presenter

### Step 1: Create the View Interface

Create in `JDG.Presentation/Views/`:

```csharp
namespace JDG.Presentation.Views
{
    public interface IMyNewView
    {
        void Show();
        void Hide();
        void UpdateDisplay(string data);
        void SetInteractable(bool interactable);
    }
}
```

### Step 2: Create the Presenter

Create in `JDG.Presentation/Presenters/`:

```csharp
using System;
using JDG.Application;
using JDG.Domain.Events;
using JDG.Presentation.Views;

namespace JDG.Presentation.Presenters
{
    public class MyNewPresenter : IDisposable
    {
        private readonly IMyNewView _view;
        private readonly IEventBus _eventBus;
        private IDisposable _subscription;

        public MyNewPresenter(IMyNewView view, IEventBus eventBus)
        {
            _view = view;
            _eventBus = eventBus;
        }

        public void Initialize()
        {
            // Subscribe to events
            _subscription = _eventBus.Subscribe<SomeEvent>(OnSomeEvent);

            // Initial state
            _view.Hide();
        }

        private void OnSomeEvent(SomeEvent evt)
        {
            _view.UpdateDisplay(evt.Data);
            _view.Show();
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
```

### Step 3: Implement the View (MonoBehaviour)

Create in default assembly (e.g., `Assets/_Scripts/UI/`):

```csharp
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using JDG.Application;
using JDG.Presentation.Views;
using JDG.Presentation.Presenters;

public class MyNewManager : MonoBehaviour, IMyNewView
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Text _displayText;
    [SerializeField] private Button _actionButton;

    private MyNewPresenter _presenter;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _presenter = new MyNewPresenter(this, eventBus);
    }

    private void Start()
    {
        _presenter.Initialize();
    }

    private void OnDestroy()
    {
        _presenter?.Dispose();
    }

    // IMyNewView implementation
    public void Show() => _panel.SetActive(true);
    public void Hide() => _panel.SetActive(false);
    public void UpdateDisplay(string data) => _displayText.text = data;
    public void SetInteractable(bool interactable) => _actionButton.interactable = interactable;
}
```

### Step 4: Register in Scene Scope

Add to `GameSceneScope.cs`:

```csharp
builder.RegisterComponentInHierarchy<MyNewManager>();
```

### Step 5: Write Tests

Create in `JDG.Presentation.Tests/Presenters/`:

```csharp
[TestFixture]
public class MyNewPresenterTests
{
    private MyNewPresenter _presenter;
    private IMyNewView _view;
    private IEventBus _eventBus;

    [SetUp]
    public void SetUp()
    {
        _view = Substitute.For<IMyNewView>();
        _eventBus = Substitute.For<IEventBus>();
        _presenter = new MyNewPresenter(_view, _eventBus);
    }

    [Test]
    public void Initialize_HidesView()
    {
        // Act
        _presenter.Initialize();

        // Assert
        _view.Received(1).Hide();
    }
}
```

---

## How to Write Tests

### Unit Test Structure (AAA Pattern)

```csharp
[Test]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data and mocks
    var mockService = Substitute.For<IService>();
    mockService.GetData().Returns("test");
    var sut = new SystemUnderTest(mockService);

    // Act - Execute the method being tested
    var result = sut.DoSomething();

    // Assert - Verify the results
    Assert.AreEqual("expected", result);
    mockService.Received(1).GetData();
}
```

### Using NSubstitute for Mocking

```csharp
// Create mock
var mockRepository = Substitute.For<IPlayerRepository>();

// Setup return values
mockRepository.GetPlayer(Arg.Any<PlayerId>()).Returns(player);

// Setup callbacks
mockRepository.When(x => x.SavePlayer(Arg.Any<Player>()))
    .Do(x => savedPlayer = x.Arg<Player>());

// Verify calls
mockRepository.Received(1).SavePlayer(Arg.Is<Player>(p => p.Health == 25));
```

### Using Test Fixtures

```csharp
using JDG.TestUtilities;

[Test]
public void MyTest()
{
    // Use pre-built test data
    var player = TestFixtures.PlayerFactory.CreatePlayer1();
    var card = TestFixtures.CardFactory.CreateInvocation("TestCard");
    var (p1, p2, gameState) = TestFixtures.GameStateFixtures.CreateBasicGameSetup();
}
```

### Running Tests

1. Open Unity Editor
2. Window → General → Test Runner
3. Select EditMode or PlayMode tab
4. Click "Run All" or select specific tests

---

## How to Add a New Domain Event

### Step 1: Create the Event Struct

Create in `JDG.Domain/Events/`:

```csharp
using System;

namespace JDG.Domain.Events
{
    /// <summary>
    /// Event published when [describe when this happens].
    /// </summary>
    public struct MyNewEvent
    {
        public Guid CardId { get; set; }
        public CardOwner Owner { get; set; }
        public string Data { get; set; }

        // Events should be immutable value types
        // Use readonly struct if targeting C# 7.2+
    }
}
```

### Step 2: Publish the Event

In your use case or service:

```csharp
_eventBus.Publish(new MyNewEvent
{
    CardId = card.Id.ToGuid(),
    Owner = player.Id.ToCardOwner(),
    Data = "some data"
});
```

### Step 3: Subscribe to the Event

In a presenter or handler:

```csharp
_subscription = _eventBus.Subscribe<MyNewEvent>(OnMyNewEvent);

private void OnMyNewEvent(MyNewEvent evt)
{
    // Handle the event
    Debug.Log($"Event received: {evt.Data}");
}
```

---

## How to Register a Service in DI

### Step 1: Create the Interface

```csharp
namespace JDG.Application.Services
{
    public interface IMyService
    {
        void DoSomething();
        string GetData();
    }
}
```

### Step 2: Create the Implementation

```csharp
namespace JDG.Infrastructure.Services
{
    public class MyService : IMyService
    {
        private readonly IDependency _dependency;

        public MyService(IDependency dependency)
        {
            _dependency = dependency;
        }

        public void DoSomething()
        {
            // Implementation
        }

        public string GetData()
        {
            return "data";
        }
    }
}
```

### Step 3: Register in Appropriate Scope

**For shared services (SharedServicesScope.cs):**
```csharp
builder.Register<MyService>(Lifetime.Singleton).AsImplementedInterfaces();
```

**For game scene only (GameSceneScope.cs):**
```csharp
builder.Register<MyService>(Lifetime.Scoped).AsImplementedInterfaces();
```

**For menu scene only (MainScreenScope.cs):**
```csharp
builder.Register<MyService>(Lifetime.Scoped).AsImplementedInterfaces();
```

### Step 4: Inject and Use

```csharp
public class Consumer
{
    private readonly IMyService _myService;

    public Consumer(IMyService myService)
    {
        _myService = myService;
    }

    public void DoWork()
    {
        var data = _myService.GetData();
    }
}
```

---

## Tips and Best Practices

### General Guidelines

1. **Follow existing patterns** - Look at similar code for examples
2. **Write tests first** - TDD helps design better APIs
3. **Keep classes small** - Single Responsibility Principle
4. **Use interfaces** - Makes testing easier
5. **Publish events** - For side effects and cross-cutting concerns

### Common Mistakes to Avoid

1. **Direct singleton access** - Use DI instead
2. **Circular dependencies** - Use events or reorganize
3. **Unity types in Domain/Application** - Keep these layers pure C#
4. **Forgetting to dispose subscriptions** - Implement IDisposable

### Where to Put Code

| Type of Code | Location |
|--------------|----------|
| Business entities | JDG.Domain/Entities |
| Value objects | JDG.Domain/ValueObjects |
| Business operations | JDG.Application/UseCases |
| Service interfaces | JDG.Application/Services |
| Service implementations | JDG.Infrastructure/Services or default assembly |
| UI logic | JDG.Presentation/Presenters |
| MonoBehaviours | Default assembly |
| Tests | Assets/Tests/* |
