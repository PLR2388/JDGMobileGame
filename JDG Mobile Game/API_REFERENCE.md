# API Reference - JDG Mobile Game

This document provides detailed documentation for all public interfaces in the JDG Mobile Game codebase.

## Table of Contents

1. [Repository Interfaces](#repository-interfaces)
   - ICardRepository, IPlayerRepository, IGameStateRepository, IDeckRepository, IInGameCardStateRepository
2. [Service Interfaces](#service-interfaces)
   - Core: IAudioService, ILocalizationService, IDialogService, ISceneLoaderService, ICardFactory
   - Player/State: IPlayerService, ICardStateService, ICardSelectionService
   - Combat: ICombatQueryService, ICombatLogic, ICardPlacementLogic
   - Data: ICardVisualService, ICardDataProvider, ICardSyncService
   - Input/UI: IInputService, ICanvasProvider, IRoundDisplayService, IInvocationMenuService
   - Other: IConditionProvider, ITutorialStateService
3. [EventBus](#eventbus)
4. [Use Cases](#use-cases)
5. [Domain Events](#domain-events)
6. [Ability System](#ability-system)
   - IAbility, AbilityContext, AbilityResult
   - IPassiveAbility, IEquipmentAbility, AbilityTrigger
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
    // Get deck by name (returns card IDs)
    CardId[] GetDeck(string deckName);

    // Save a deck
    void SaveDeck(string deckName, CardId[] cardIds);

    // Get all available deck names
    IEnumerable<string> GetAllDeckNames();

    // Delete a deck
    void DeleteDeck(string deckName);

    // Get the default deck for a player
    CardId[] GetDefaultDeck(PlayerId playerId);
}
```

---

### IInGameCardStateRepository

Manages runtime card state during gameplay. This is the primary repository for querying and modifying cards in play.

Located in: `JDG.Application/Repositories/IInGameCardStateRepository.cs`

```csharp
public interface IInGameCardStateRepository
{
    // Get card by unique ID
    InGameCard GetById(Guid id);

    // Get all cards for a player by location
    IEnumerable<InGameCard> GetCardsInLocation(CardOwner owner, CardLocation location);

    // Field queries
    IEnumerable<InGameCard> GetFieldCards(CardOwner owner);
    IEnumerable<InGameCard> GetAllFieldCards();
    InGameCard GetFieldCard(CardOwner owner);
    int GetFieldCardCount(CardOwner owner);

    // Hand queries
    IEnumerable<InGameCard> GetHandCards(CardOwner owner);
    int GetHandCardCount(CardOwner owner);

    // Deck queries
    IEnumerable<InGameCard> GetDeckCards(CardOwner owner);
    int GetDeckCardCount(CardOwner owner);

    // Graveyard queries
    IEnumerable<InGameCard> GetGraveyardCards(CardOwner owner);

    // Invocation queries
    IEnumerable<InGameCard> GetInvocations(CardOwner owner);
    InGameCard GetInvocationAtSlot(CardOwner owner, int slot);
    int GetInvocationCount(CardOwner owner);

    // Equipment queries
    IEnumerable<InGameCard> GetEquipments(CardOwner owner);
    InGameCard GetEquipmentForInvocation(Guid invocationId);

    // State modifications
    void SetCardLocation(Guid cardId, CardLocation newLocation);
    void UpdateCard(InGameCard card);
    void RemoveCard(Guid cardId);

    // Bulk operations
    void ClearAllCards();
    void ClearCardsForOwner(CardOwner owner);
}
```

**Usage Example:**
```csharp
public class CombatService
{
    private readonly IInGameCardStateRepository _cardStateRepo;

    public IEnumerable<InGameCard> GetAttackableTargets(CardOwner attacker)
    {
        var defender = attacker == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1;
        return _cardStateRepo.GetInvocations(defender)
            .Where(card => !card.IsProtected);
    }
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

    // Card localization (Phase 126)
    string GetCardTitle(string cardId);
    string GetCardDescription(string cardId);
    string GetCardDetailedDescription(string cardId);
    bool HasCardLocalization(string cardId);
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

### IPlayerService

Manages player state including health, shields, and deck operations.

```csharp
public interface IPlayerService
{
    // Health management
    int GetHealth(CardOwner owner);
    void SetHealth(CardOwner owner, int health);
    void DamagePlayer(CardOwner owner, int damage);
    void HealPlayer(CardOwner owner, int amount);

    // Shield management
    int GetShields(CardOwner owner);
    void SetShields(CardOwner owner, int shields);
    void AddShields(CardOwner owner, int amount);
    bool UseShield(CardOwner owner);

    // Deck operations
    void InitializeDeck(CardOwner owner, IEnumerable<CardId> cardIds);
    bool HasCardsInDeck(CardOwner owner);
}
```

---

### ICardStateService

Provides card state operations during gameplay.

```csharp
public interface ICardStateService
{
    // Card state queries
    bool IsCardAttackable(Guid cardId);
    bool HasCardAttackedThisTurn(Guid cardId);
    bool IsCardProtected(Guid cardId);

    // Card state modifications
    void SetCardAttacked(Guid cardId, bool hasAttacked);
    void ResetCardForNewTurn(Guid cardId);
    void MarkCardAsProtected(Guid cardId, bool isProtected);

    // Equipment operations
    void AttachEquipment(Guid invocationId, Guid equipmentId);
    void DetachEquipment(Guid invocationId);
    Guid? GetAttachedEquipment(Guid invocationId);

    // Stat modifications
    void ModifyStats(Guid cardId, int atkModifier, int defModifier);
    void ResetStats(Guid cardId);
}
```

---

### ICardSelectionService

Manages card selection state and user input for card choices.

```csharp
public interface ICardSelectionService
{
    // Selection state
    bool IsInSelectionMode { get; }
    IReadOnlyList<Guid> GetSelectedCardIds();

    // Selection operations
    void StartSelection(CardSelectionConfig config);
    void AddToSelection(Guid cardId);
    void RemoveFromSelection(Guid cardId);
    void ClearSelection();
    void ConfirmSelection();
    void CancelSelection();

    // Events
    event Action<IReadOnlyList<Guid>> OnSelectionConfirmed;
    event Action OnSelectionCancelled;
}
```

---

### ICardVisualService

Resolves card visual materials for rendering.

```csharp
public interface ICardVisualService
{
    // Get material for card display
    Material GetCardMaterial(string cardName);
    Material GetCardBackMaterial();
    Material GetEmptySlotMaterial();
}
```

---

### ICombatQueryService

Queries combat-related state without modifying it.

```csharp
public interface ICombatQueryService
{
    // Query available actions
    IEnumerable<Guid> GetAttackableInvocations(CardOwner defender);
    bool CanAttackDirectly(CardOwner attacker);
    bool CanCardAttack(Guid cardId);
}
```

---

### ICombatLogic

Pure combat calculation logic (no state modification).

```csharp
public interface ICombatLogic
{
    // Calculate combat results
    CombatResult CalculateCombat(int attackerAtk, int defenderDef);
    int CalculateDamageToPlayer(int attackerAtk);
    bool WouldDestroyTarget(int attackerAtk, int targetDef);

    // Combat validation
    bool IsValidAttackTarget(InGameCard attacker, InGameCard target);
    bool CanBypassDefenders(InGameCard attacker);
}
```

---

### ICardPlacementLogic

Validates and determines card placement on the field.

```csharp
public interface ICardPlacementLogic
{
    // Validate placement
    bool CanPlaceCard(InGameCard card, CardOwner owner, int? slot = null);
    int? GetAvailableSlot(CardOwner owner, CardType cardType);
    bool IsSlotOccupied(CardOwner owner, int slot);

    // Placement requirements
    IEnumerable<PlacementRequirement> GetPlacementRequirements(InGameCard card);
    bool MeetsRequirements(InGameCard card, CardOwner owner);
}
```

---

### ICardDataProvider

Provides access to card definition data.

```csharp
public interface ICardDataProvider
{
    // Card data access
    Card GetCardData(string cardName);
    IEnumerable<Card> GetAllCards();
    IEnumerable<Card> GetCardsByFamily(CardFamily family);
}
```

---

### ICardSyncService

Synchronizes domain card state with presentation layer.

```csharp
public interface ICardSyncService
{
    // Sync operations
    void SyncCardToView(Guid cardId);
    void SyncAllCards();
    void RefreshCardVisuals(Guid cardId);
    void NotifyCardMoved(Guid cardId, CardLocation from, CardLocation to);
}
```

---

### IInputService

Handles touch and input events for cards.

```csharp
public interface IInputService
{
    // Touch state
    bool IsTouching { get; }
    Vector2 TouchPosition { get; }

    // Input detection
    bool DetectLongPress(float threshold = 0.5f);
    bool DetectSwipe(out SwipeDirection direction);

    // Card hit detection
    Guid? GetCardAtPosition(Vector2 screenPosition);
    bool IsOverCard(Vector2 screenPosition);
}
```

---

### IConditionProvider

Provides condition evaluation for card abilities.

```csharp
public interface IConditionProvider
{
    // Get condition by name
    ICondition GetCondition(ConditionName name);

    // Evaluate conditions
    bool EvaluateCondition(ConditionName name, AbilityContext context);
}
```

---

### ICanvasProvider

Provides access to UI canvas for dialog display.

```csharp
public interface ICanvasProvider
{
    Canvas GetMainCanvas();
}
```

---

### ITutorialStateService

Manages tutorial progression state.

```csharp
public interface ITutorialStateService
{
    // Tutorial state
    bool IsTutorialActive { get; }
    int CurrentStep { get; }

    // Tutorial control
    void StartTutorial();
    void AdvanceStep();
    void EndTutorial();
}
```

---

### IRoundDisplayService

Manages round/turn display UI.

```csharp
public interface IRoundDisplayService
{
    // Display control
    void ShowRoundIndicator(int roundNumber);
    void ShowPlayerTurnIndicator(CardOwner currentPlayer);
}
```

---

### IInvocationMenuService

Controls the invocation ability menu UI.

```csharp
public interface IInvocationMenuService
{
    // Menu control
    void ShowMenu(Guid invocationId, IEnumerable<AbilityName> availableAbilities);
    void HideMenu();
    bool IsMenuVisible { get; }

    // Selection
    event Action<AbilityName> OnAbilitySelected;
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
| `DrawCardUseCase` | Draw card from deck | `CardDrawnEvent` |
| `ResetCardsForNewTurnUseCase` | Reset cards at turn start | `CardsResetForNewTurnEvent` |
| `HandleCardDeathUseCase` | Handle card death | `CardDiedEvent` |
| `HandleCardAddedToFieldUseCase` | Handle card placed on field | `CardAddedToFieldEvent` |
| `HandleCardRemovedFromFieldUseCase` | Handle card removed from field | `CardRemovedFromFieldEvent` |
| `HandleHandCardsChangeUseCase` | Handle hand card count change | `HandCardsChangedEvent` |
| `HandleFieldCardChangedUseCase` | Handle field card state change | - |
| `SummonPlayerEntityUseCase` | Summon player entity card | `CardAddedToFieldEvent` |

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

### Event Categories (58 total)

**Phase & Turn Events:**
- `PhaseChangedEvent` - Phase transition (Draw → Play → Attack)
- `PlayerTurnChangedEvent` - Turn switched to other player
- `TurnStartEvent` - Turn begins
- `TurnEndEvent` - Turn ends
- `CardsResetForNewTurnEvent` - Cards reset at turn start

**Card Events:**
- `CardDrawnEvent` - Card drawn from deck
- `CardPlayedEvent` - Card played to field
- `CardDestroyedEvent` - Card removed from game
- `CardDiedEvent` - Card dies and moves to graveyard
- `HandCardsChangedEvent` - Hand card count changes
- `CardAddedToFieldEvent` - Card placed on field
- `CardRemovedFromFieldEvent` - Card left field
- `CardSelectedEvent` - Card selected/clicked
- `CardDiscardedEvent` - Card discarded from hand

**Combat Events:**
- `AttackExecutedEvent` - Attack executed
- `PlayerHealthChangedEvent` - Player health changes
- `PlayerShieldChangedEvent` - Player shields change
- `PlayerDamagedEvent` - Player takes damage
- `GameStartedEvent` - Game initialized
- `GameOverEvent` - Game ended

**Input Events:**
- `TouchStartedEvent`, `TouchEndedEvent`, `LongTouchEvent`, `BackButtonPressedEvent`

**UI Events:**
- `NextPhaseButtonClickedEvent`, `AttackButtonClickedEvent`, `EndTurnButtonClickedEvent`
- `CardLocationChangedEvent`, `HandCardsDisplayChangedEvent`
- `InvocationCancelledEvent`, `ChoicePlayerChangedEvent`

**Ability Events:**
- `AbilityActivatedEvent`, `EffectAppliedEvent`, `AbilityExecutedEvent`
- `CardStatsModifiedEvent`, `ShieldsAddedEvent`, `PlayerHealedEvent`

**Card Play Request Events:**
- `InvocationCardPlayRequestedEvent`, `FieldCardPlayRequestedEvent`
- `EffectCardPlayRequestedEvent`, `EquipmentCardPlayRequestedEvent`
- `ContreCardPlayRequestedEvent`

**Card State Events (Phase 75):**
- `CardStatsResetEvent`, `CardControlledEvent`, `CardFreedEvent`
- `InvocationTurnCountIncrementedEvent`, `InvocationDeathCountIncrementedEvent`
- `EquipmentAttachedEvent`, `EquipmentDetachedEvent`, `FieldCardReplacedEvent`

**Selection & Highlight Events:**
- `CardNumberedEvent`, `CardAddedToSelectionEvent`, `CardRemovedFromSelectionEvent`
- `HighlightRequestedEvent`

**Dialogue Events:**
- `DialogueTriggerCompletedEvent`, `DialogueIndexChangedEvent`

**Click Events:**
- `InGameCardClickedEvent`

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

### AbilityContext

Provides all context needed for ability execution.

```csharp
public class AbilityContext
{
    // The card that owns this ability
    public InGameCard SourceCard { get; }

    // The owner of the source card
    public CardOwner Owner { get; }

    // Target card (if ability targets a specific card)
    public InGameCard TargetCard { get; set; }

    // Target player (if ability targets a player)
    public CardOwner? TargetPlayer { get; set; }

    // Additional parameters for the ability
    public Dictionary<string, object> Parameters { get; }

    // Services available during execution
    public IEventBus EventBus { get; }
    public IInGameCardStateRepository CardStateRepository { get; }
    public IPlayerService PlayerService { get; }

    // Constructor
    public AbilityContext(
        InGameCard sourceCard,
        CardOwner owner,
        IEventBus eventBus,
        IInGameCardStateRepository cardStateRepository,
        IPlayerService playerService)
    {
        SourceCard = sourceCard;
        Owner = owner;
        EventBus = eventBus;
        CardStateRepository = cardStateRepository;
        PlayerService = playerService;
        Parameters = new Dictionary<string, object>();
    }

    // Helper to get opponent
    public CardOwner GetOpponent() =>
        Owner == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1;
}
```

**Usage Example:**
```csharp
public class MyAbility : IAbility
{
    public AbilityResult Execute(AbilityContext context)
    {
        // Access the source card
        var attacker = context.SourceCard;

        // Get opponent's cards
        var opponent = context.GetOpponent();
        var enemyCards = context.CardStateRepository.GetInvocations(opponent);

        // Damage opponent
        context.PlayerService.DamagePlayer(opponent, 2);

        // Publish event
        context.EventBus.Publish(new AbilityExecutedEvent
        {
            AbilityName = Name,
            SourceCardId = attacker.Id
        });

        return AbilityResult.Success("Dealt 2 damage");
    }
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

### IPassiveAbility

Abilities that trigger automatically based on game events.

```csharp
public interface IPassiveAbility : IAbility
{
    // The trigger condition for this passive ability
    AbilityTrigger Trigger { get; }
}
```

### AbilityTrigger Enum

Defines when passive abilities activate.

```csharp
public enum AbilityTrigger
{
    OnSummon,       // When the card is summoned to field
    OnDeath,        // When the card dies
    OnAttack,       // When the card attacks
    OnDefend,       // When the card defends
    OnTurnStart,    // At the start of owner's turn
    OnTurnEnd,      // At the end of owner's turn
    OnCardDrawn,    // When a card is drawn
    OnCardPlayed,   // When any card is played
    Continuous,     // Always active while on field
    OnEquip,        // When equipment is attached
    OnUnequip,      // When equipment is removed
    OnHandChange    // When hand card count changes
}
```

### IEquipmentAbility

Abilities specific to equipment cards with attachment/detachment lifecycle.

```csharp
public interface IEquipmentAbility : IAbility
{
    // True if this equipment can be placed without normal restrictions
    bool CanAlwaysBePlaced { get; }

    // Called before the equipped card is destroyed
    // Return true to prevent destruction
    bool OnPreDestroy(AbilityContext context);
}
```

**Usage Example:**
```csharp
public class ShieldEquipmentAbility : IEquipmentAbility
{
    public AbilityName Name => AbilityName.ShieldEquipment;
    public string Description => "Prevents destruction once";
    public bool CanAlwaysBePlaced => false;

    private bool _hasProtected = false;

    public bool CanActivate(AbilityContext context) => true;

    public AbilityResult Execute(AbilityContext context)
    {
        // Boost defense when equipped
        context.SourceCard.DefenseModifier += 1;
        return AbilityResult.Success("Defense boosted");
    }

    public bool OnPreDestroy(AbilityContext context)
    {
        if (!_hasProtected)
        {
            _hasProtected = true;
            return true; // Prevent destruction
        }
        return false; // Allow destruction
    }
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
| `InGameCard` | (Legacy) | Runtime card instance during gameplay |
| `CardId` | JDG.Domain.ValueObjects | Card identifier |
| `PlayerId` | JDG.Domain.ValueObjects | Player identifier |
| `CardType` | JDG.Domain.Enums | Card type enum |
| `CardFamily` | JDG.Domain.Enums | Card family enum |
| `CardLocation` | JDG.Domain.Enums | Card location (Hand, Field, Deck, Graveyard) |
| `Phase` | JDG.Domain | Game phase enum (Draw, Play, Attack) |
| `CardOwner` | JDG.Domain | Player ownership enum |
| `AbilityName` | JDG.Domain.Enums | Ability identifier enum |
| `AbilityTrigger` | JDG.Application.Abilities | Passive ability trigger enum |
| `AbilityContext` | JDG.Application.Abilities | Execution context for abilities |
| `AbilityResult` | JDG.Application.Abilities | Result of ability execution |

### Key Interfaces

| Interface | Layer | Purpose |
|-----------|-------|---------|
| `IAbility` | Application | Base ability contract |
| `IPassiveAbility` | Application | Auto-triggered abilities |
| `IEquipmentAbility` | Application | Equipment-specific abilities |
| `IEventBus` | Application | Pub/sub event system |
| `ICardRepository` | Application | Card definition access |
| `IPlayerRepository` | Application | Player state persistence |
| `IInGameCardStateRepository` | Application | Runtime card state |
| `IPlayerService` | Application | Player health/shields |
| `ICardStateService` | Application | Card state operations |
