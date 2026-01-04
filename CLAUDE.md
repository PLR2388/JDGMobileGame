# JDG Mobile Game - Project Overview

## What is this project?

**JDG (Joueur du Grenier) Trading Card Game Mobile** is a digital adaptation of a French physical trading card game. It's a turn-based TCG where two players compete in 1v1 duels, each starting with 30 life points and a custom 30-card deck. The goal is to reduce the opponent's life to 0 through strategic card play and combat.

- **Platform**: Android (available on Google Play)
- **Engine**: Unity 2022.3.9f1
- **License**: MIT

## Architecture

The project follows **Clean Architecture** with 4 layers:

```
┌─────────────────────────────────────┐
│     PRESENTATION (MVP Pattern)      │  Presenters + View Interfaces
├─────────────────────────────────────┤
│     APPLICATION                     │  Use Cases, Abilities, Service Interfaces
├─────────────────────────────────────┤
│     INFRASTRUCTURE                  │  Repositories, EventBus, Service Implementations
├─────────────────────────────────────┤
│     DOMAIN (Pure C#)                │  Entities, Value Objects, Events
└─────────────────────────────────────┘
```

**Key Patterns:**
- **Dependency Injection**: VContainer 1.15.4
- **Event-Driven**: Custom EventBus (40+ domain events)
- **MVP**: Thin MonoBehaviour views with pure C# presenters
- **Strangler Fig**: Legacy code coexists with clean architecture during migration

## Project Structure

```
Assets/_Scripts/
├── JDG.Domain/           # Pure C# - Entities, Enums, Value Objects, Events
├── JDG.Application/      # Use Cases, Abilities (57 implementations), Service Interfaces
├── JDG.Infrastructure/   # DI Config, Repositories, EventBus, Services
├── JDG.Presentation/     # Presenters + Views (MVP)
├── JDG.Core/             # LocalizationKeys
├── Bridge/               # Legacy-to-modern adapters
├── Services/             # Legacy service implementations
├── Units/                # Legacy InGameCard hierarchy
└── Managers/             # Legacy managers
```

## Game Mechanics

**Card Types (5):**
- **Invocation**: Combat units (ATK/DEF stats, max 4 on field)
- **Equipment**: Modify invocation cards (1 per card)
- **Effect**: Temporary bonuses/penalties (max 4 active)
- **Field**: Modify entire field abilities (max 1)
- **Contra**: Instant counter cards

**Turn Structure:**
1. **Draw Phase**: Draw 1 card
2. **Play Phase**: Play unlimited cards
3. **Attack Phase**: Each invocation attacks once

## Key Systems

| System | Interface | Location |
|--------|-----------|----------|
| Audio | `IAudioService` | `Services/AudioService.cs` |
| Localization | `ILocalizationService` | `Services/LocalizationService.cs` |
| Cards | `ICardRepository` | `JDG.Infrastructure/Repositories/` |
| Combat | `CombatService` | `JDG.Infrastructure/Services/` |
| Events | `IEventBus` | `JDG.Infrastructure/Events/` |
| Abilities | `IAbility` + `AbilityRegistry` | `JDG.Application/Abilities/` |

## DI Scopes (VContainer)

- **SharedServicesScope** (root): Singletons - EventBus, Repositories, Audio, Localization
- **GameSceneScope** (child): Game-specific - CardPoolManager, GameLoop
- **MainScreenScope** (child): Menu-specific - CardChoice, DeckManagement

## Testing

- **615+ unit tests** across 7 test assemblies
- Test types: Unit, Scenario, Integration, Presenter
- Framework: NUnit + NSubstitute

```
Assets/Tests/
├── JDG.Domain.Tests/         # Entity/value object tests
├── JDG.Application.Tests/    # Use case/ability tests
├── JDG.Infrastructure.Tests/ # Service/repository tests
├── JDG.Presentation.Tests/   # Presenter tests (mocked views)
└── JDG.PlayMode.Tests/       # Full integration tests
```

## Scenes

1. `_preload.unity` - Initialization (auto-loads MainScreen)
2. `MainScreen.unity` - Main menu
3. `Game.unity` - 2-player game board
4. `TutoPlayerGame.unity` - Tutorial/single-player

## Important Files

| Purpose | File |
|---------|------|
| DI Configuration | `JDG.Infrastructure/DI/SharedServicesScope.cs` |
| Event Definitions | `JDG.Domain/Events/` |
| Ability Registry | `JDG.Application/Abilities/AbilityRegistry.cs` |
| Card Conversion | `Bridge/CardConverter.cs` |
| Game Loop | `Multiplayer/GameLoop.cs` |
| Architecture Docs | `ARCHITECTURE.md` |
| Refactoring Status | `REFACTORING_STATUS.md` |

## Current State

- **Refactoring Phase**: 158 (ongoing clean architecture migration)
- **Cards**: 174 unique cards (ScriptableObjects)
- **Abilities**: 57 implementations across 10 categories
- **Localization**: French only

## Development Notes

- Legacy `InGameCard` hierarchy still in default assembly (blocked migration)
- Some use cases pending migration to `JDG.Application`
- Bridge layer adapts legacy Unity types to domain entities
- All new code should follow clean architecture patterns

## Documentation

- `ARCHITECTURE.md` - Detailed architecture with diagrams
- `REFACTORING_STATUS.md` - Phase-by-phase migration log
- `API_REFERENCE.md` - Interface documentation
- `GETTING_STARTED.md` - Setup instructions
- `HOW_TO_GUIDES.md` - Common development tasks

## External Links

- [Google Play Store](https://play.google.com/store/apps/details?id=fr.wonderfulAppStudio.JDGMobileGame)
- [Game Rules PDF](https://www.parkage.com/files/rules/regle_TCG_joueur-du-grenier_liste-de_cartes.pdf)
- [Rules Video](https://www.youtube.com/watch?v=tBtRhNC-jFc)
