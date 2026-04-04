# Application - Abilities

This directory contains the new ability system using Clean Architecture principles.

## Architecture

### Old vs New Ability System

**Old System:**
```csharp
// Old: Tightly coupled to Unity
public class DrawCardsAbility : Ability
{
    public override void ApplyEffect(
        Transform canvas,          // Unity dependency
        PlayerCards playerCards,   // Direct state mutation
        PlayerCards opponentCards)
    {
        // Directly mutates player cards
        playerCards.HandCards.Add(card);
        playerCards.Deck.Remove(card);

        // Shows UI directly
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }
}
```

**New System:**
```csharp
// New: Pure logic with DI
public class DrawCardsAbility : IAbility
{
    private readonly DrawCardUseCase _drawCardUseCase;

    public AbilityResult Execute(AbilityContext context)
    {
        // Uses use case for game actions
        var result = _drawCardUseCase.Execute(context.CurrentPlayerId);

        // Returns result (UI handles separately)
        return AbilityResult.Success($"Drew {cardsDrawn} card(s)");
    }
}
```

### Key Improvements

✅ **No Unity Dependencies** - Pure C# classes, fully testable
✅ **Dependency Injection** - All dependencies injected via constructor
✅ **Use Cases** - Abilities delegate actions to use cases
✅ **Event-Driven** - Publishes events, doesn't show UI directly
✅ **Immutable Context** - AbilityContext carries all needed state
✅ **Factory Pattern** - Factories create abilities with dependencies

## Core Interfaces

### IAbility

Base interface for all abilities (active and passive).

```csharp
public interface IAbility
{
    AbilityName Name { get; }
    string Description { get; }
    bool CanActivate(AbilityContext context);
    AbilityResult Execute(AbilityContext context);
}
```

### IPassiveAbility

For abilities that trigger automatically.

```csharp
public interface IPassiveAbility : IAbility
{
    AbilityTrigger Trigger { get; }  // When does it trigger?
}
```

### AbilityContext

Carries all state needed by abilities.

```csharp
public class AbilityContext
{
    public PlayerId CurrentPlayerId { get; }
    public PlayerId OpponentPlayerId { get; }
    public Card SourceCard { get; }
    public AbilityName AbilityName { get; }

    // Optional targets
    public Card TargetCard { get; set; }
    public PlayerId TargetPlayerId { get; set; }
}
```

### AbilityResult

Return value from ability execution.

```csharp
public class AbilityResult
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public bool RequiresUserInput { get; }

    public static AbilityResult Success(string message);
    public static AbilityResult Failure(string message);
    public static AbilityResult NeedsUserInput(string message);
}
```

## Example Implementations

### 1. DrawCardsAbility

Simple ability that draws cards using the DrawCardUseCase.

```csharp
var ability = new DrawCardsAbility(2, drawCardUseCase);
var context = new AbilityContext(player1Id, player2Id, card, AbilityName.Draw2Cards);
var result = ability.Execute(context);
```

### 2. DestroyCardAbility

More complex ability that interacts with player fields.

```csharp
var ability = new DestroyCardAbility(
    AbilityName.KillOpponentInvocation,
    CardType.Invocation,
    playerRepository,
    eventBus
);

if (ability.CanActivate(context))
{
    var result = ability.Execute(context);
}
```

## Usage Pattern with Factories

Abilities should be created using factories for proper DI:

```csharp
// 1. Register factories in DI container
builder.Register<DrawCardsAbilityFactory>(Lifetime.Singleton);
builder.Register<DestroyCardAbilityFactory>(Lifetime.Singleton);

// 2. Get factory from DI
var drawFactory = ServiceLocator.Get<DrawCardsAbilityFactory>();

// 3. Create ability instance
IAbility draw2Cards = drawFactory.CreateDraw2Cards();

// 4. Execute ability
var context = new AbilityContext(playerId, opponentId, sourceCard, AbilityName.Draw2Cards);
var result = draw2Cards.Execute(context);

// 5. Handle result
if (result.IsSuccess)
{
    Debug.Log(result.Message);  // "Drew 2 card(s)"
}
```

## Ability Triggers (Passive Abilities)

Passive abilities activate automatically based on triggers:

```csharp
public enum AbilityTrigger
{
    OnSummon,       // When card enters field
    OnDeath,        // When card dies
    OnAttack,       // When card attacks
    OnDefend,       // When card is attacked
    OnTurnStart,    // Start of turn
    OnTurnEnd,      // End of turn
    OnCardDrawn,    // When any card is drawn
    OnCardPlayed,   // When any card is played
    Continuous      // Always active (while on field)
}
```

Example passive ability:

```csharp
public class OnSummonDrawAbility : IPassiveAbility
{
    public AbilityTrigger Trigger => AbilityTrigger.OnSummon;

    public AbilityResult Execute(AbilityContext context)
    {
        // Draw card when this card is summoned
        return _drawCardUseCase.Execute(context.CurrentPlayerId);
    }
}
```

## Migration Strategy

### Phase 1: Create Interfaces & Examples (✅ Current)
- ✅ Define IAbility, IPassiveAbility, AbilityContext, AbilityResult
- ✅ Create 2 example abilities (Draw, Destroy)
- ✅ Document new architecture

### Phase 2: Create Ability Manager
- Create AbilityManager to handle ability registration & execution
- Create AbilityRegistry to map AbilityName → IAbility
- Integrate with event bus for passive ability triggers

### Phase 3: Migrate Existing Abilities (68 total)
Priority order:
1. Draw/Discard abilities (high usage)
2. Destroy/Kill abilities (high impact)
3. Stat modification abilities (moderate complexity)
4. Summon/Search abilities (complex)
5. Conditional abilities (most complex)

### Phase 4: Remove Old System
- Delete old Ability.cs base class
- Remove Unity dependencies from ability execution
- Move UI responses to presentation layer

## Testing Abilities

Abilities are pure C# - easy to test:

```csharp
[Test]
public void DrawCardsAbility_DrawsCorrectNumber()
{
    // Arrange
    var mockUseCase = new Mock<DrawCardUseCase>();
    mockUseCase.Setup(x => x.Execute(It.IsAny<PlayerId>()))
        .Returns(DrawCardResult.Success(mockCard));

    var ability = new DrawCardsAbility(2, mockUseCase.Object);
    var context = new AbilityContext(player1Id, player2Id, sourceCard, AbilityName.Draw2Cards);

    // Act
    var result = ability.Execute(context);

    // Assert
    Assert.IsTrue(result.IsSuccess);
    mockUseCase.Verify(x => x.Execute(player1Id), Times.Exactly(2));
}
```

## Benefits

1. **Testable** - No Unity dependencies, easy to unit test
2. **Reusable** - Same ability logic across different card types
3. **Composable** - Abilities can be combined
4. **Maintainable** - Clear separation of concerns
5. **Type-Safe** - Compile-time checking of ability parameters
6. **Event-Driven** - Abilities publish events, UI reacts
7. **DI-Ready** - All dependencies injected, easy to mock

## Next Steps

1. Create AbilityManager for orchestration
2. Create AbilityRegistry for ability lookup
3. Integrate with StartGameUseCase to wire up card abilities
4. Migrate high-priority abilities (Draw, Kill, etc.)
5. Add comprehensive ability tests
6. Remove old ability system once migration complete
