# JDG Mobile Game - Clean Architecture

## Overview

This project follows **Clean Architecture** principles with 4 distinct layers, implemented using **VContainer** for dependency injection and **EventBus** for decoupled communication.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           PRESENTATION LAYER                                 │
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ JDG.Presentation Assembly                                               ││
│  │  - RoundDisplayPresenter, InvocationMenuPresenter                       ││
│  │  - DialogPresenter, CardDisplayPresenter                                ││
│  │  - View interfaces (IRoundDisplayView, IInvocationMenuView)             ││
│  └─────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ Default Assembly (MonoBehaviours)                                       ││
│  │  - RoundDisplayManager, InvocationMenuManager (thin views)              ││
│  │  - CardSelectorPresenter (blocked by legacy deps)                       ││
│  │  - UIManager (marked [Obsolete])                                        ││
│  └─────────────────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────────┘
                                     │
                                     ▼ Depends on
┌─────────────────────────────────────────────────────────────────────────────┐
│                           APPLICATION LAYER                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ JDG.Application Assembly                                                ││
│  │                                                                         ││
│  │  Use Cases:                      Service Interfaces:       Abilities:              ││
│  │  - DrawCardUseCase               - ICardVisualService      - IAbility interface    ││
│  │  - ResetCardsForNewTurnUseCase   - ICombatQueryService     - 57 implementations    ││
│  │  - HandleCardDeathUseCase        - ICardCollectionService  - 11 ability factories  ││
│  │  - SummonPlayerEntityUseCase     - ICardSelectionService                           ││
│  │  - + 4 more event handlers                                                        ││
│  │                                                                         ││
│  │  Repository Interfaces:                                                 ││
│  │  - ICardRepository, IPlayerRepository, IGameStateRepository             ││
│  └─────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ Default Assembly (Services/) - Legacy Type Dependencies                ││
│  │  - SummonPlayerEntityUseCase, HandleCardDeathUseCase, etc. (7 total)   ││
│  │  - Will move to JDG.Application once legacy types migrate              ││
│  └─────────────────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────────┘
                                     │
                                     ▼ Depends on
┌─────────────────────────────────────────────────────────────────────────────┐
│                          INFRASTRUCTURE LAYER                                │
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ JDG.Infrastructure Assembly                                             ││
│  │                                                                         ││
│  │  Repositories:        Events:            DI:                            ││
│  │  - CardRepository     - EventBus         - SharedServicesScope          ││
│  │  - PlayerRepository   - 58 events        - GameSceneScope               ││
│  │  - GameStateRepository                   - MainScreenScope              ││
│  │  - DeckRepository                                                       ││
│  │                                                                         ││
│  │  Registry:                                                              ││
│  │  - AbilityRegistry (all 57 abilities registered)                        ││
│  └─────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ Default Assembly (Services/)                                            ││
│  │  - CombatService, CardPlacementService, TurnService                     ││
│  │  - CardVisualService (needs Cards namespace)                            ││
│  │  - AbilityProviderService                                               ││
│  └─────────────────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────────┘
                                     │
                                     ▼ Depends on
┌─────────────────────────────────────────────────────────────────────────────┐
│                             DOMAIN LAYER                                     │
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ JDG.Domain Assembly (Pure C# - NO Unity dependencies)                  ││
│  │                                                                         ││
│  │  Entities:            Value Objects:       Enums:                       ││
│  │  - Card               - CardId             - AbilityName (71 values)    ││
│  │  - Player             - PlayerId           - CardType                   ││
│  │  - PlayerState        - CardStats          - Phase                      ││
│  │                       - Vector2            - CardOwner                  ││
│  │                       - DeckConfiguration  - CardFamily                 ││
│  │                                                                         ││
│  │  Domain Events:                                                         ││
│  │  - GameStartedEvent, PhaseChangedEvent, CardPlayedEvent, etc.          ││
│  └─────────────────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### Domain Layer (JDG.Domain)
- **Pure C#** - No Unity dependencies (`noEngineReferences: true`)
- Contains business entities (Card, Player)
- Contains value objects (CardId, CardStats)
- Contains domain events and enums
- **RULE**: This layer has NO external dependencies

### Application Layer (JDG.Application)
- Contains use cases (business operations)
- Contains service interfaces (contracts)
- Contains ability interfaces and implementations
- Contains repository interfaces
- **RULE**: Only depends on Domain layer

### Infrastructure Layer (JDG.Infrastructure)
- Contains repository implementations
- Contains EventBus implementation
- Contains DI configuration (VContainer scopes)
- Contains AbilityRegistry
- **RULE**: Depends on Domain and Application

### Presentation Layer (JDG.Presentation)
- Contains presenters (business logic for UI)
- Contains view interfaces
- MonoBehaviours implement view interfaces
- **RULE**: Depends on Domain and Application

## Dependency Rules

```
Allowed:
  Presentation → Application → Domain
  Infrastructure → Application → Domain
  Presentation → Infrastructure (AVOID - see note below)

Forbidden:
  Domain → anything
  Application → Infrastructure
  Application → Presentation
```

**Note**: ICardVisualService is defined in JDG.Application. The Presentation layer should not reference Infrastructure directly.

## Assembly Structure

```
Assets/
├── _Scripts/
│   ├── JDG.Domain/              # JDG.Domain.asmdef
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   └── Enums/
│   │
│   ├── JDG.Application/         # JDG.Application.asmdef
│   │   ├── UseCases/
│   │   ├── Services/            # Interfaces only
│   │   ├── Abilities/
│   │   │   ├── IAbility.cs
│   │   │   └── Implementations/
│   │   ├── Cards/
│   │   └── Repositories/        # Interfaces only
│   │
│   ├── JDG.Infrastructure/      # JDG.Infrastructure.asmdef
│   │   ├── DI/
│   │   ├── Events/
│   │   ├── Repositories/
│   │   └── Services/
│   │
│   ├── JDG.Presentation/        # JDG.Presentation.asmdef
│   │   ├── Presenters/
│   │   ├── Views/               # Interfaces only
│   │   └── MonoBehaviours/
│   │
│   ├── Core/                    # JDG.Core.asmdef
│   │   └── LocalizationKeys.cs
│   │
│   └── [Default Assembly]       # No .asmdef
│       ├── Services/            # Legacy use cases
│       ├── Managers/            # Legacy managers
│       ├── Units/               # InGameCard hierarchy
│       └── Bridge/              # Adapters
│
└── Tests/
    ├── JDG.Domain.Tests/
    ├── JDG.Application.Tests/
    ├── JDG.Infrastructure.Tests/
    ├── JDG.Presentation.Tests/
    └── PlayMode/
```

## Dependency Injection (VContainer)

### Scope Hierarchy

```
SharedServicesScope (Root - DontDestroyOnLoad)
│
├── Singletons:
│   ├── IEventBus → EventBus
│   ├── ICardRepository → CardRepository
│   ├── IPlayerRepository → PlayerRepository
│   ├── IGameStateRepository → GameStateRepository
│   ├── AbilityRegistry
│   └── 8 Ability Factories (DrawCards, DestroyCard, DeckSearch, Sacrifice, StatModifier, Protection, Combat, Special)
│
├── Transient:
│   ├── DrawCardUseCase
│   ├── ResetCardsForNewTurnUseCase
│   ├── HandleCardDeathUseCase
│   ├── SummonPlayerEntityUseCase
│   └── + 4 more event handler use cases
│
├── GameSceneScope (Child)
│   ├── CardPoolManager
│   ├── InputManager
│   ├── GameLoop
│   └── Scene-specific MonoBehaviours
│
└── MainScreenScope (Child)
    ├── CardChoice
    ├── DeckManagementService
    └── Menu services
```

### Registration Patterns

**Singleton (state preserved)**:
```csharp
builder.Register<CardRepository>(Lifetime.Singleton).AsImplementedInterfaces();
```

**Transient (no memory leaks)**:
```csharp
builder.Register<DrawCardUseCase>(Lifetime.Transient);
```

**MonoBehaviour injection**:
```csharp
builder.RegisterComponentInHierarchy<CardPoolManager>();
```

## Event-Driven Communication

### EventBus Pattern

```csharp
// Publishing (from Use Case)
_eventBus.Publish(new CardPlayedEvent(card, player));

// Subscribing (from Presenter)
_eventBus.Subscribe<CardPlayedEvent>(OnCardPlayed);

// Cleanup
_eventBus.Unsubscribe<CardPlayedEvent>(OnCardPlayed);
```

### Domain Events (58)

| Category | Events |
|----------|--------|
| Game | GameStartedEvent, PhaseChangedEvent, PlayerTurnChangedEvent |
| Cards | CardPlayedEvent, CardDrawnEvent, CardDestroyedEvent, CardNumberedEvent |
| Combat | AttackExecutedEvent, PlayerDamagedEvent, PlayerHealthChangedEvent |
| Selection | CardAddedToSelectionEvent, CardRemovedFromSelectionEvent |
| UI | InGameCardClickedEvent, HighlightRequestedEvent |
| Dialogue | DialogueTriggerCompletedEvent, DialogueIndexChangedEvent |
| Card Play | InvocationCardPlayRequestedEvent, FieldCardPlayRequestedEvent, EffectCardPlayRequestedEvent, EquipmentCardPlayRequestedEvent |

## MVP Pattern (Presentation)

```
┌─────────────────┐    implements    ┌──────────────────────┐
│ View Interface  │◄─────────────────│ MonoBehaviour (View) │
│ IRoundDisplayView│                  │ RoundDisplayManager  │
└────────┬────────┘                  └──────────────────────┘
         │                                      │
         │ injected                             │ creates
         ▼                                      ▼
┌─────────────────────────────────────────────────────┐
│               RoundDisplayPresenter                  │
│  - Subscribes to EventBus                           │
│  - Contains business logic                          │
│  - Calls view interface methods                     │
└─────────────────────────────────────────────────────┘
```

## Testing Strategy

### Test Assemblies (8 total)

| Assembly | Type | Purpose |
|----------|------|---------|
| `JDG.Domain.Tests` | EditMode | Entity creation, value object equality |
| `JDG.Application.Tests` | EditMode | Use cases, abilities, scenarios |
| `JDG.Infrastructure.Tests` | EditMode | Services, repositories, EventBus |
| `JDG.Presentation.Tests` | EditMode | Presenters with mocked views |
| `JDG.PlayMode.Tests` | PlayMode | DI container, full game flow |
| `JDG.TestUtilities` | Shared | Test doubles, fixtures |
| `JDG.TestUtilities.Editor` | Editor | Editor-only test helpers |
| `JDG.Tests.Editor` | EditMode | Additional editor tests |

### Test Types
- **Unit Tests** (EditMode): 618+ tests covering domain logic, use cases, services
- **Scenario Tests**: Combat, abilities, card placement, game loop
- **Integration Tests** (PlayMode): DI resolution, service wiring
- **Presenter Tests**: MVP pattern with mocked views

### Test Utilities
- `NSubstitute` - Mocking framework
- `TestEventBus` - Event capture and verification
- `TestGameStateRepository` - In-memory game state
- Scenario test fixtures in `JDG.Application.Tests/Scenarios/`

## Migration Strategy (Strangler Fig)

Legacy code coexists with clean architecture:

1. **Legacy types** (InGameCard, InGameInvocationCard) remain in default assembly
2. **Interfaces** abstract legacy types (IInGameCard, IInGameInvocationCard)
3. **Bridge layer** converts Unity assets to domain entities (CardConverter)
4. **Gradual migration** - new code uses clean patterns, old code wrapped by adapters

## Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| VContainer over Zenject | Lighter, faster, Unity-native |
| EventBus over UniRx | Simpler, zero dependencies |
| Strangler Fig migration | No breaking changes, safe transition |
| 4-layer architecture | Clear boundaries, testable |
| Factories for abilities | Flexible, DI-friendly |

## Sequence Diagrams

### Card Play Flow

```
┌────────┐     ┌──────────┐     ┌───────────────┐     ┌─────────────┐     ┌──────────┐
│ Player │     │ GameLoop │     │ PlayCardUseCase│     │ Repository  │     │ EventBus │
└───┬────┘     └────┬─────┘     └───────┬───────┘     └──────┬──────┘     └────┬─────┘
    │               │                    │                    │                 │
    │ Touch Card    │                    │                    │                 │
    │──────────────>│                    │                    │                 │
    │               │                    │                    │                 │
    │               │ Execute(playerId, │                    │                 │
    │               │   cardId)         │                    │                 │
    │               │───────────────────>│                    │                 │
    │               │                    │                    │                 │
    │               │                    │ GetPlayer(id)     │                 │
    │               │                    │───────────────────>│                 │
    │               │                    │                    │                 │
    │               │                    │ <─── Player ───────│                 │
    │               │                    │                    │                 │
    │               │                    │ player.PlayCard()  │                 │
    │               │                    │──────┐             │                 │
    │               │                    │      │             │                 │
    │               │                    │<─────┘             │                 │
    │               │                    │                    │                 │
    │               │                    │ SavePlayer(player) │                 │
    │               │                    │───────────────────>│                 │
    │               │                    │                    │                 │
    │               │                    │ Publish(CardPlayedEvent)            │
    │               │                    │──────────────────────────────────────>│
    │               │                    │                    │                 │
    │               │ <── PlayCardResult─│                    │                 │
    │               │                    │                    │                 │
    │ Update UI     │                    │                    │                 │
    │<──────────────│                    │                    │                 │
```

### Attack Flow

```
┌────────┐    ┌──────────┐    ┌─────────────┐    ┌─────────────┐    ┌──────────┐
│ Player │    │ GameLoop │    │AttackUseCase│    │CombatService│    │ EventBus │
└───┬────┘    └────┬─────┘    └──────┬──────┘    └──────┬──────┘    └────┬─────┘
    │              │                  │                  │                │
    │ Select       │                  │                  │                │
    │ Attacker     │                  │                  │                │
    │─────────────>│                  │                  │                │
    │              │                  │                  │                │
    │ Select       │                  │                  │                │
    │ Target       │                  │                  │                │
    │─────────────>│                  │                  │                │
    │              │                  │                  │                │
    │              │ Execute(attacker,│                  │                │
    │              │   target)        │                  │                │
    │              │─────────────────>│                  │                │
    │              │                  │                  │                │
    │              │                  │ CalculateDamage()│                │
    │              │                  │─────────────────>│                │
    │              │                  │                  │                │
    │              │                  │ <── DamageResult─│                │
    │              │                  │                  │                │
    │              │                  │ Publish(AttackEvent)              │
    │              │                  │───────────────────────────────────>│
    │              │                  │                  │                │
    │              │                  │ Publish(DamageDealtEvent)         │
    │              │                  │───────────────────────────────────>│
    │              │                  │                  │                │
    │              │ <── AttackResult─│                  │                │
```

### Turn Lifecycle

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              TURN LIFECYCLE                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   ┌──────────────┐      ┌──────────────┐      ┌──────────────┐             │
│   │  DRAW PHASE  │ ───> │  PLAY PHASE  │ ───> │ ATTACK PHASE │             │
│   └──────────────┘      └──────────────┘      └──────────────┘             │
│         │                      │                      │                     │
│         ▼                      ▼                      ▼                     │
│   ┌──────────────┐      ┌──────────────┐      ┌──────────────┐             │
│   │ DrawCardUse  │      │ PlayCardUse  │      │ AttackUseCase│             │
│   │    Case      │      │    Case      │      │              │             │
│   └──────────────┘      └──────────────┘      └──────────────┘             │
│         │                      │                      │                     │
│         ▼                      ▼                      ▼                     │
│   CardDrawnEvent        CardPlayedEvent         AttackEvent                 │
│                                                 DamageDealtEvent            │
│                                                                             │
│   After Attack Phase: EndTurnUseCase ──> TurnEndedEvent                    │
│                                      ──> PlayerTurnChangedEvent            │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Component Interaction Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           COMPONENT INTERACTIONS                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────┐                              ┌─────────────────┐       │
│  │   MonoBehaviour │                              │    Presenter    │       │
│  │    (GameLoop)   │                              │ (RoundDisplay)  │       │
│  └────────┬────────┘                              └────────┬────────┘       │
│           │                                                │                │
│           │ calls                                          │ subscribes     │
│           ▼                                                ▼                │
│  ┌─────────────────┐      publishes       ┌─────────────────────────┐      │
│  │    Use Case     │ ──────────────────>  │       EventBus          │      │
│  │ (DrawCardUseCase)│                      │                         │      │
│  └────────┬────────┘                      └─────────────────────────┘      │
│           │                                          │                      │
│           │ uses                                     │ notifies             │
│           ▼                                          ▼                      │
│  ┌─────────────────┐                      ┌─────────────────┐              │
│  │   Repository    │                      │     View        │              │
│  │(PlayerRepository)│                      │ (MonoBehaviour) │              │
│  └────────┬────────┘                      └─────────────────┘              │
│           │                                                                 │
│           │ stores                                                          │
│           ▼                                                                 │
│  ┌─────────────────┐                                                       │
│  │  Domain Entity  │                                                       │
│  │    (Player)     │                                                       │
│  └─────────────────┘                                                       │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Ability System Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                            ABILITY SYSTEM                                    │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌───────────────────────────────────────────────────────────────────────┐ │
│  │                        AbilityRegistry                                 │ │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  │ │
│  │  │Draw1Card    │  │DestroyCard  │  │GiveFamilyATK│  │DirectAttack │  │ │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘  │ │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  │ │
│  │  │ProtectBehind│  │CopyStats    │  │Resurrection │  │FieldBonus   │  │ │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘  │ │
│  │                        ... 57 abilities total ...                      │ │
│  └───────────────────────────────────────────────────────────────────────┘ │
│                                     │                                       │
│                                     │ GetAbility(AbilityName)               │
│                                     ▼                                       │
│  ┌───────────────────────────────────────────────────────────────────────┐ │
│  │                         IAbility Interface                             │ │
│  │  ┌─────────────────────────────────────────────────────────────────┐  │ │
│  │  │  Name: AbilityName                                              │  │ │
│  │  │  Description: string                                            │  │ │
│  │  │  CanActivate(AbilityContext) : bool                            │  │ │
│  │  │  Execute(AbilityContext) : AbilityResult                       │  │ │
│  │  └─────────────────────────────────────────────────────────────────┘  │ │
│  └───────────────────────────────────────────────────────────────────────┘ │
│                                     │                                       │
│                          ┌──────────┴──────────┐                           │
│                          ▼                      ▼                           │
│              ┌─────────────────┐    ┌─────────────────┐                    │
│              │ AbilityContext  │    │  AbilityResult  │                    │
│              │ - SourceCard       │    │ - IsSuccess     │                    │
│              │ - CurrentPlayerId  │    │ - Message       │                    │
│              │ - OpponentPlayerId │    │ - RequiresInput │                    │
│              │ - AbilityName      │    └─────────────────┘                    │
│              └─────────────────┘                                           │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## DI Container Scope Hierarchy

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                       DI CONTAINER SCOPE HIERARCHY                           │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌───────────────────────────────────────────────────────────────────────┐ │
│  │                   SharedServicesScope (ROOT)                           │ │
│  │                   DontDestroyOnLoad - persists across scenes           │ │
│  │  ┌─────────────────────────────────────────────────────────────────┐  │ │
│  │  │ SINGLETONS (shared state):                                      │  │ │
│  │  │ - IEventBus → EventBus                                          │  │ │
│  │  │ - ICardRepository → CardRepository                              │  │ │
│  │  │ - IPlayerRepository → PlayerRepository                          │  │ │
│  │  │ - IGameStateRepository → GameStateRepository                    │  │ │
│  │  │ - IAudioService → AudioService                                  │  │ │
│  │  │ - ILocalizationService → LocalizationService                    │  │ │
│  │  │ - IDialogService → DialogService                                │  │ │
│  │  │ - AbilityRegistry (all 57 abilities)                            │  │ │
│  │  └─────────────────────────────────────────────────────────────────┘  │ │
│  │  ┌─────────────────────────────────────────────────────────────────┐  │ │
│  │  │ TRANSIENT (new instance per request):                           │  │ │
│  │  │ - DrawCardUseCase, ResetCardsForNewTurnUseCase                  │  │ │
│  │  │ - HandleCardDeathUseCase, SummonPlayerEntityUseCase, + 4 more   │  │ │
│  │  └─────────────────────────────────────────────────────────────────┘  │ │
│  └─────────────────────────────────────────────────────────────────────┬─┘ │
│                                                                        │   │
│        ┌───────────────────────────────────┬───────────────────────────┘   │
│        │                                   │                               │
│        ▼                                   ▼                               │
│  ┌─────────────────────────┐    ┌─────────────────────────┐              │
│  │    GameSceneScope       │    │   MainScreenScope       │              │
│  │    (Game scene only)    │    │   (Menu scene only)     │              │
│  │  ┌───────────────────┐  │    │  ┌───────────────────┐  │              │
│  │  │ - CardPoolManager │  │    │  │ - CardChoice      │  │              │
│  │  │ - InputManager    │  │    │  │ - DeckManagement  │  │              │
│  │  │ - GameLoop        │  │    │  │   Service         │  │              │
│  │  │ - PlayerCards x2  │  │    │  │ - Menu services   │  │              │
│  │  └───────────────────┘  │    │  └───────────────────┘  │              │
│  └─────────────────────────┘    └─────────────────────────┘              │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Bridge Layer

The Bridge layer provides essential compatibility between legacy Unity types and modern clean architecture:

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                            BRIDGE LAYER                                      │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ LegacySystemInitializer                                             │   │
│  │ - Initializes static fields in extension classes                   │   │
│  │   (CardTypeExtensions, CardFamilyExtensions, etc.)                 │   │
│  │ - Called from SharedServicesScope.RegisterBuildCallback()          │   │
│  │ - PERMANENT: Required for extension method localization            │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                     │                                       │
│                                     ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ CardRepositoryInitializer                                           │   │
│  │ - Loads ScriptableObject cards from Resources/                      │   │
│  │ - Uses CardConverter to transform to domain entities               │   │
│  │ - PERMANENT: Required for Unity asset loading                       │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                     │                                       │
│                                     ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ CardConverter                                                       │   │
│  │ - Converts legacy ScriptableObject → Domain Card entity            │   │
│  │ - Handles enum translations (Cards.CardFamily → Domain.CardFamily) │   │
│  │ - PERMANENT: Required until cards stored in non-Unity format       │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

For complete bridge documentation, see `Assets/_Scripts/Bridge/README.md`.

## Future Work

1. **CardSelectorPresenter** - Move to JDG.Presentation (blocked by InGameCard)
2. **Use Cases** - Move 7 from Services/ to JDG.Application (blocked by legacy types)
3. **InGameCard → Domain** - Major refactor for pure domain entities
4. **Repository Persistence** - DeckRepository uses PlayerPrefs (functional but simple)
5. **Card Storage Alternatives** - Investigate JSON/database storage to simplify Bridge layer (Phase 125)

## Documentation

| Document | Description |
|----------|-------------|
| ARCHITECTURE.md | This file - architecture overview and diagrams |
| REFACTORING_STATUS.md | Detailed phase-by-phase refactoring log |
| Bridge/README.md | Bridge layer documentation and migration status |

---

**Last Updated**: 2026-02-04
