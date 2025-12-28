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
│  │  Use Cases:           Service Interfaces:       Abilities:              ││
│  │  - StartGameUseCase   - ICardVisualService      - IAbility interface    ││
│  │  - DrawCardUseCase    - ICombatQueryService     - 57 implementations    ││
│  │  - PlayCardUseCase    - ICardCollectionService  - 11 ability factories  ││
│  │  - AttackUseCase      - ICardSelectionService                           ││
│  │  - EndTurnUseCase                                                       ││
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
│  │  - PlayerRepository   - 30+ events       - GameSceneScope               ││
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
│  │  - Card               - CardId             - AbilityName (70 values)    ││
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

**Note**: JDG.Presentation currently references JDG.Infrastructure for CardVisualService. This is a known issue to be fixed by moving ICardVisualService to JDG.Application.

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
│   └── All Ability Factories
│
├── Transient:
│   ├── StartGameUseCase
│   ├── DrawCardUseCase
│   ├── PlayCardUseCase
│   ├── AttackUseCase
│   └── EndTurnUseCase
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
builder.Register<StartGameUseCase>(Lifetime.Transient);
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

### Domain Events (30+)

| Category | Events |
|----------|--------|
| Game | GameStartedEvent, PhaseChangedEvent, PlayerTurnChangedEvent |
| Cards | CardPlayedEvent, CardDrawnEvent, CardDestroyedEvent |
| Combat | AttackEvent, DamageDealtEvent |
| Selection | CardAddedToSelectionEvent, CardRemovedFromSelectionEvent |

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

### Unit Tests (EditMode)
- **Domain**: Entity creation, value object equality
- **Application**: Use cases with mocked repositories
- **Infrastructure**: Repository behavior, EventBus
- **Presentation**: Presenters with mocked views

### Integration Tests (PlayMode)
- Full game flow testing
- DI container verification
- EventBus propagation

### Test Utilities
- `CardFactory` - Create test cards
- `PlayerFactory` - Create test players
- `GameStateFixtures` - Pre-configured scenarios
- `NSubstitute` - Mocking framework

## Migration Strategy (Strangler Fig)

Legacy code coexists with clean architecture:

1. **Legacy types** (InGameCard, InGameInvocationCard) remain in default assembly
2. **Interfaces** abstract legacy types (IInGameCard)
3. **Adapters** bridge old → new (ModernAbilityAdapter, CardConverter)
4. **Gradual migration** - new code uses clean patterns

## Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| VContainer over Zenject | Lighter, faster, Unity-native |
| EventBus over UniRx | Simpler, zero dependencies |
| Strangler Fig migration | No breaking changes, safe transition |
| 4-layer architecture | Clear boundaries, testable |
| Factories for abilities | Flexible, DI-friendly |

## Future Work

1. **CardSelectorPresenter** - Move to JDG.Presentation (blocked by InGameCard)
2. **Use Cases** - Move 7 from Services/ to JDG.Application (blocked by legacy types)
3. **InGameCard → Domain** - Major refactor for pure domain entities
4. **Repository Persistence** - Implement save/load for DeckRepository
