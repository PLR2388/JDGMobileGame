# Phase 166: InGameCard Migration to Clean Architecture

**Date**: 2026-03-29
**Branch**: `refactor-v3`
**Tests**: 1,417 passed, 0 failed (unchanged)

## Summary

Phase 166 resolved the primary migration blocker: the **InGameCard hierarchy** was stuck in the default assembly because it depended on ScriptableObject types and legacy enums also in the default assembly. JDG.Infrastructure (the target) can't reference the default assembly.

## What Was Done

### 1. Populated JDG.Cards Assembly (Pre-planned, Previously Empty)

Moved the following from the default assembly into `JDG.Cards`:

**Legacy Enums:**
- `CardOwner` (wrapped in `namespace Cards` to prevent global namespace collision with `JDG.Domain.CardOwner`)
- `CardType`, `CardFamily`, `CardName`, `ConditionName`

**ScriptableObject Card Definitions:**
- `Card`, `InvocationCard`, `EffectCard`, `EquipmentCard`, `FieldCard`, `ContreCard`
- Used `git mv` to preserve `.meta` GUIDs -- all 166 card assets remain linked

**Condition Hierarchy:**
- `Condition` base class + 8 implementations
- Refactored `CanBeSummoned(PlayerCards)` to `CanBeSummoned(IPlayerCardCollection)` to decouple from MonoBehaviour

### 2. Moved Ability Provider Interfaces to JDG.Application

| Interface | New Location |
|-----------|-------------|
| `IAbilityProvider` | `JDG.Application.Services` |
| `IFieldAbilityProvider` | `JDG.Application.Services` |
| `IEquipmentAbilityProvider` | `JDG.Application.Services` |
| `IEffectAbilityProvider` | `JDG.Application.Services` |

These were already clean (only referenced JDG.Application/JDG.Domain types). Added `namespace JDG.Application.Services` wrapper.

### 3. Moved InGameCard Hierarchy to JDG.Infrastructure

| Class | Old Location | New Location |
|-------|-------------|-------------|
| `InGameCard` | `_Scripts/Units/` (namespace `Cards`) | `JDG.Infrastructure/Cards/` (namespace `JDG.Infrastructure.Cards`) |
| `InGameInvocationCard` | `_Scripts/Units/Invocation/` (namespace `_Scripts.Units.Invocation`) | `JDG.Infrastructure/Cards/` |
| `InGameEffectCard` | `_Scripts/Units/Effect/` (namespace `Cards.EffectCards`) | `JDG.Infrastructure/Cards/` |
| `InGameFieldCard` | `_Scripts/Units/Field/` (no namespace) | `JDG.Infrastructure/Cards/` |
| `InGameEquipmentCard` | `_Scripts/Units/Equipment/` (no namespace) | `JDG.Infrastructure/Cards/` |
| `CardFactory` | `_Scripts/Units/` (no namespace) | `JDG.Infrastructure/Cards/` |

All now in unified `namespace JDG.Infrastructure.Cards`.

### 4. Supporting Changes

**New Types Created:**
- `ICardCollectionProvider` in `JDG.Application.Services` -- returns `IPlayerCardCollection` instead of `PlayerCards`, enabling InGameInvocationCard to work without referencing the default assembly
- `CardIdGenerator` in `JDG.Application.Services` -- extracted from `LocalizationService` (pure string transform, no Unity dependencies)
- `GraveyardCards` property added to `IPlayerCardCollection` -- enables `NumberInvocationDeadCondition` to work via interfaces

**DI Updates:**
- `CardCollectionServiceAdapter` now implements both `ICardCollectionService` (legacy) and `ICardCollectionProvider` (clean)
- `CardFactory` registered with `ICardCollectionProvider` instead of `ICardCollectionService`
- Both interfaces registered from same singleton in `GameSceneScope`

**Consumer Updates:**
- ~60 files updated with `using JDG.Infrastructure.Cards;`
- Old using statements (`using _Scripts.Units.Invocation;`, `using Cards.EffectCards;`) removed where no longer needed
- Test assemblies updated with `JDG.Cards` reference

## Assembly Dependency Graph (After Phase 166)

```
JDG.Domain (pure C#, no Unity)
    ^
    |
JDG.Application (pure C#, no Unity)
    ^           ^
    |           |
JDG.Cards    JDG.Infrastructure
(SOs, enums,  (InGameCard, EventBus,
 conditions)   repositories, services)
    ^           ^
    |           |
    +-----------+
          |
    Default Assembly
    (PlayerCards, Managers,
     Menu, GameLoop, UI)
```

## What's Now Unblocked

- **InGameCard is no longer in the default assembly** -- the primary migration blocker is resolved
- **Service implementations** (AbilityProviderService, ConditionProviderService, CardSyncService) can now move to JDG.Infrastructure as follow-up work
- **Service interfaces** that still reference concrete types can be incrementally refactored to use IInGameCard interfaces and moved to JDG.Application

## Remaining Migration Work

| Item | Status | Notes |
|------|--------|-------|
| Move ability provider services to JDG.Infrastructure | Deferred | Straightforward follow-up |
| Move CardSyncService to JDG.Infrastructure | Deferred | |
| Refactor ICombatService to use interfaces | Deferred | Then movable to JDG.Application |
| Refactor ICardPlacementService | Stays in default assembly | Depends on Transform (Unity type) |
| Refactor ICardCollectionService | Stays in default assembly | Returns PlayerCards (MonoBehaviour) |
| Refactor PlayerCards away from MonoBehaviour | Future | Largest remaining work item |

## Commits

1. `ed65dc8d` - Move legacy enums, ScriptableObjects, and Conditions to JDG.Cards assembly
2. `16e1b9f6` - Move InGameCard hierarchy to JDG.Infrastructure + provider interfaces to JDG.Application
