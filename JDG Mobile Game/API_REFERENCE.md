# API Reference - JDG Mobile Game

This document provides detailed documentation for all public interfaces in the JDG Mobile Game codebase.

## Table of Contents

1. [Repository Interfaces](#repository-interfaces)
2. [Service Interfaces](#service-interfaces)
3. [EventBus](#eventbus)
4. [Use Cases](#use-cases)
5. [Domain Events](#domain-events)
6. [Ability System](#ability-system)
7. [DI Registration](#di-registration)

---

## Repository Interfaces

Located in: `JDG.Application/Repositories/`

### ICardRepository

Provides read-only access to card definitions.

```csharp
public interface ICardRepository
{
    // Get a specific card by ID
    Card GetCard(CardId cardId);

    // Create a new instance from a card definition
    Card CreateCardInstance(string cardDefinitionName);

    // Get all available card definitions
    IEnumerable<Card> GetAllCardDefinitions();

    // Filter cards by type (Invocation, Equipment, etc.)
    IEnumerable<Card> GetCardsByType(CardType type);

    // Filter cards by family
    IEnumerable<Card> GetCardsByFamily(CardFamily family);

    // Find a card by its title
    Card GetCardByTitle(string title);
}
```

**Usage Example:**
```csharp
public class MyService
{
    private readonly ICardRepository _cardRepository;

    public MyService(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public void ListAllInvocations()
    {
        var invocations = _cardRepository.GetCardsByType(CardType.Invocation);
        foreach (var card in invocations)
        {
            Debug.Log($"Card: {card.Title}");
        }
    }
}
```

---

### IPlayerRepository

Manages player entities and state persistence.

```csharp
public interface IPlayerRepository
{
    // Get player by ID
    Player GetPlayer(PlayerId playerId);

    // Save player state
    void SavePlayer(Player player);

    // Create new player with deck
    Player CreatePlayer(PlayerId playerId, CardId[] deckCardIds, int maxHealth = 30);

    // Reset player to initial state
    void ResetPlayer(PlayerId playerId);
}
```

**Usage Example:**
```csharp
public class MyUseCase
{
    private readonly IPlayerRepository _playerRepository;

    public void DamagePlayer(PlayerId playerId, int damage)
    {
        var player = _playerRepository.GetPlayer(playerId);
        player.TakeDamage(damage);
        _playerRepository.SavePlayer(player);
    }
}
```

---

### IGameStateRepository

Manages game state (phase, turn, current player).

```csharp
public interface IGameStateRepository
{
    // Current game phase
    Phase CurrentPhase { get; }
    void SetPhase(Phase phase);

    // Turn tracking
    int TurnNumber { get; }
    void IncrementTurn();

    // Active player
    PlayerId CurrentPlayer { get; }
    void SetCurrentPlayer(PlayerId playerId);
    void SwitchPlayer();

    // Game over state
    bool IsGameOver { get; }
    void SetGameOver(bool isGameOver);

    // Reset all state
    void ResetGameState();
}
```

---

### IDeckRepository

Manages deck configurations (saved decks for deck building).

```csharp
public interface IDeckRepository
{
    // Get all saved decks
    IEnumerable<DeckConfiguration> GetAllDecks();

    // Get specific deck
    DeckConfiguration GetDeck(string deckId);

    // Save a deck
    void SaveDeck(DeckConfiguration deck);

    // Delete a deck
    void DeleteDeck(string deckId);
}
```

---

## Service Interfaces

Located in: `JDG.Application/Services/`

### IAudioService

Handles music and sound effect playback.

```csharp
public interface IAudioService
{
    // Music control
    void PlayMusic(string musicName);
    void StopMusic();

    // Sound effects
    void PlaySoundEffect(string sfxName);
    void PlayTransitionSound();
    void PlayBackSound();

    // Volume control (0.0 to 1.0)
    void SetMasterVolume(float volume);
    void SetMusicVolume(float volume);
    void SetSfxVolume(float volume);
    float GetMusicVolume();
    float GetSfxVolume();

    // Play music for a card family
    void PlayFamilyMusic(object family);
}
```

**Usage Example:**
```csharp
public class GameStartHandler
{
    [Inject] private IAudioService _audioService;

    public void OnGameStart()
    {
        _audioService.PlayMusic("BattleTheme");
        _audioService.PlaySoundEffect("GameStart");
    }
}
```

---

### ILocalizationService

Handles text localization and translations.

```csharp
public interface ILocalizationService
{
    // Get translated text for a key
    string GetLocalizedValue(string key);

    // Language management
    void SetLanguage(GameLanguage language);
    GameLanguage GetCurrentLanguage();

    // Check if key exists
    bool HasKey(string key);
}

public enum GameLanguage
{
    Unknown, French, English, Spanish, German, Japanese
}
```

**Usage Example:**
```csharp
public class UIComponent
{
    [Inject] private ILocalizationService _localization;

    public void UpdateText()
    {
        var text = _localization.GetLocalizedValue("GAME_OVER_MESSAGE");
        myLabel.text = text;
    }
}
```

---

### IDialogService

Shows dialogs and card selection prompts.

```csharp
public interface IDialogService
{
    // Async methods (modern pattern)
    Task<bool> ShowMessageBoxAsync(string title, string message, MessageBoxType type);
    Task<List<Guid>> ShowCardSelectorAsync(CardSelectorConfig config);
    Task<bool> ShowConfirmAsync(string message);
    Task ShowInfoAsync(string message);

    // Callback methods (legacy pattern support)
    void ShowMessageBox(object canvas, MessageBoxOptions options);
    void ShowCardSelector(object canvas, CardSelectorOptions options);
}

public enum MessageBoxType { YesNo, Ok, OkCancel, Custom }

public class MessageBoxOptions
{
    public string Title { get; set; }
    public string Message { get; set; }
    public bool ShowOkButton { get; set; }
    public bool ShowPositiveButton { get; set; }
    public bool ShowNegativeButton { get; set; }
    public Action OnOk { get; set; }
    public Action OnPositive { get; set; }
    public Action OnNegative { get; set; }
}

public class CardSelectorOptions
{
    public string Title { get; set; }
    public List<object> Cards { get; set; }
    public int NumberCardSelection { get; set; }
    public Action<object> OnOkSingle { get; set; }
    public Action<List<object>> OnOkMultiple { get; set; }
    public Action OnNegative { get; set; }
}
```

---

### ISceneLoaderService

Handles scene transitions.

```csharp
public interface ISceneLoaderService
{
    // Load main menu
    void LoadMainScreen();

    // Load game scene
    void LoadGameScreen();

    // Load any scene by name
    void LoadSceneAsync(string sceneName);

    // Exit application
    void QuitGame();
}
```

---

### ICardFactory

Creates in-game card instances.

```csharp
public interface ICardFactory
{
    // Create any card type
    object CreateCard(object cardSO, CardOwner owner);

    // Create specific card types
    object CreateInvocationCard(object cardSO, CardOwner owner);
    object CreateEffectCard(object cardSO, CardOwner owner);
    object CreateFieldCard(object cardSO, CardOwner owner);
    object CreateEquipmentCard(object cardSO, CardOwner owner);
}
```

---

## EventBus

Located in: `JDG.Application/IEventBus.cs`

The EventBus provides decoupled pub/sub communication.

```csharp
public interface IEventBus
{
    // Publish an event to all subscribers
    void Publish<T>(T eventData) where T : struct;

    // Subscribe to an event type (returns disposable)
    IDisposable Subscribe<T>(Action<T> handler) where T : struct;

    // Clear subscriptions
    void ClearSubscriptions<T>() where T : struct;
    void ClearAllSubscriptions();
}
```

**Publishing Events:**
```csharp
public class MyUseCase
{
    private readonly IEventBus _eventBus;

    public void DoSomething()
    {
        // Publish event
        _eventBus.Publish(new CardPlayedEvent
        {
            CardId = card.Id.ToGuid(),
            CardTitle = card.Title,
            Owner = CardOwner.Player1
        });
    }
}
```

**Subscribing to Events:**
```csharp
public class MyPresenter : IDisposable
{
    private readonly IEventBus _eventBus;
    private IDisposable _subscription;

    public void Initialize()
    {
        _subscription = _eventBus.Subscribe<CardPlayedEvent>(OnCardPlayed);
    }

    private void OnCardPlayed(CardPlayedEvent evt)
    {
        Debug.Log($"Card played: {evt.CardTitle}");
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}
```

---

## Use Cases

Located in: `JDG.Application/UseCases/`

Use cases encapsulate business operations. Each returns a result object.

### Core Use Cases

| Use Case | Purpose | Events Published |
|----------|---------|------------------|
| `StartGameUseCase` | Initialize new game | `GameStartedEvent` |
| `DrawCardUseCase` | Draw card from deck | `CardDrawnEvent` |
| `PlayCardUseCase` | Play card to field | `CardPlayedEvent` |
| `AttackUseCase` | Attack with card | `AttackEvent`, `DamageDealtEvent` |
| `EndTurnUseCase` | End current turn | `TurnEndedEvent`, `PlayerTurnChangedEvent` |

### Use Case Pattern

```csharp
public class DrawCardUseCase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IEventBus _eventBus;

    public DrawCardUseCase(IPlayerRepository playerRepository, IEventBus eventBus)
    {
        _playerRepository = playerRepository;
        _eventBus = eventBus;
    }

    public DrawCardResult Execute(PlayerId playerId)
    {
        var player = _playerRepository.GetPlayer(playerId);

        if (player == null)
            return DrawCardResult.Failure("Player not found");

        var drawnCard = player.DrawCard();

        if (drawnCard == null)
            return DrawCardResult.Failure("Deck is empty");

        _playerRepository.SavePlayer(player);

        _eventBus.Publish(new CardDrawnEvent
        {
            Owner = playerId.ToCardOwner(),
            CardId = drawnCard.Id.ToGuid(),
            CardTitle = drawnCard.Title
        });

        return DrawCardResult.Success(drawnCard);
    }
}
```

---

## Domain Events

Located in: `JDG.Domain/Events/`

Domain events are value types (structs) that represent things that happened.

### Event Categories

**Game Events:**
- `GameStartedEvent` - Game initialized
- `PhaseChangedEvent` - Phase transition (Draw → Play → Attack)
- `PlayerTurnChangedEvent` - Turn switched to other player
- `GameOverEvent` - Game ended

**Card Events:**
- `CardDrawnEvent` - Card drawn from deck
- `CardPlayedEvent` - Card played to field
- `CardDestroyedEvent` - Card removed from game
- `CardAddedToFieldEvent` - Card placed on field
- `CardRemovedFromFieldEvent` - Card left field

**Combat Events:**
- `AttackEvent` - Attack initiated
- `DamageDealtEvent` - Damage applied
- `DirectAttackEvent` - Direct attack on player

**Selection Events:**
- `CardAddedToSelectionEvent` - Card selected
- `CardRemovedFromSelectionEvent` - Card deselected

---

## Ability System

Located in: `JDG.Application/Abilities/`

### IAbility Interface

```csharp
public interface IAbility
{
    AbilityName Name { get; }
    string Description { get; }

    // Check if ability can activate
    bool CanActivate(AbilityContext context);

    // Execute the ability
    AbilityResult Execute(AbilityContext context);
}
```

### AbilityResult

```csharp
public class AbilityResult
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public bool RequiresUserInput { get; }
    public bool RequiresLegacyExecution { get; }

    public static AbilityResult Success(string message);
    public static AbilityResult Failure(string message);
    public static AbilityResult NeedsUserInput(string message);
    public static AbilityResult NeedsLegacyExecution(string message);
}
```

### Ability Factories

| Factory | Abilities Created |
|---------|-------------------|
| `DeckSearchAbilityFactory` | GetSpecificCard, GetFamilyCard |
| `SacrificeAbilityFactory` | SacrificeCard, InvokeSpecificCard |
| `StatModifierAbilityFactory` | GiveFamilyStats, CopyStats |
| `ProtectionAbilityFactory` | CantBeAttacked, ProtectBehind |
| `CombatAbilityFactory` | MutualDestruction, DirectAttack |
| `EffectAbilityFactory` | SwapStats, DestroyMultiple |
| `EquipmentAbilityFactory` | SetStats, BonusStats |
| `FieldAbilityFactory` | FamilyBoost, HealPerFamily |
| `SpecialAbilityFactory` | SendAllCardsToHand, IncrementAttacks |

---

## DI Registration

### Scope Hierarchy

```
SharedServicesScope (Root - persists across scenes)
├── Singletons: IEventBus, ICardRepository, IPlayerRepository, etc.
├── Transient: Use cases
│
├── GameSceneScope (Game scene)
│   └── Scene-specific: CardPoolManager, InputManager, GameLoop
│
└── MainScreenScope (Menu scene)
    └── Menu-specific: CardChoice, DeckManagementService
```

### Registration Patterns

**Singleton (shared state):**
```csharp
builder.Register<CardRepository>(Lifetime.Singleton).AsImplementedInterfaces();
```

**Transient (new instance each time):**
```csharp
builder.Register<DrawCardUseCase>(Lifetime.Transient);
```

**MonoBehaviour injection:**
```csharp
builder.RegisterComponentInHierarchy<CardPoolManager>();
```

**Factory registration:**
```csharp
builder.Register<ICardFactory>(container =>
{
    var providers = container.Resolve<...>();
    return new CardFactory(providers);
}, Lifetime.Singleton);
```

### Injecting Dependencies

**Constructor injection (preferred):**
```csharp
public class MyService
{
    private readonly ICardRepository _cardRepository;

    public MyService(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }
}
```

**Field injection (MonoBehaviours):**
```csharp
public class MyMonoBehaviour : MonoBehaviour
{
    [Inject] private ICardRepository _cardRepository;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        // Called by VContainer after instantiation
    }
}
```

---

## Quick Reference

### Common Import Statements

```csharp
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;
using JDG.Domain.Events;
using JDG.Application;
using JDG.Application.UseCases;
using JDG.Application.Services;
using JDG.Application.Repositories;
using JDG.Application.Abilities;
```

### Key Types

| Type | Namespace | Purpose |
|------|-----------|---------|
| `Card` | JDG.Domain.Entities | Domain card entity |
| `Player` | JDG.Domain.Entities | Domain player entity |
| `CardId` | JDG.Domain.ValueObjects | Card identifier |
| `PlayerId` | JDG.Domain.ValueObjects | Player identifier |
| `CardType` | JDG.Domain.Enums | Card type enum |
| `Phase` | JDG.Domain | Game phase enum |
| `CardOwner` | JDG.Domain | Player ownership enum |
