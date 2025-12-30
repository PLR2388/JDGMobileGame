# Bridge Layer

The Bridge layer provides compatibility between Unity's ScriptableObject-based card system and the modern clean architecture. These files are **permanent infrastructure**, not temporary migration code.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                    MODERN ARCHITECTURE                          │
│  ┌─────────────┐   ┌─────────────┐   ┌─────────────────────┐   │
│  │   Domain    │   │ Application │   │   Infrastructure    │   │
│  │  (Entities) │   │ (Use Cases) │   │   (Repositories)    │   │
│  └──────┬──────┘   └──────┬──────┘   └──────────┬──────────┘   │
│         │                 │                      │              │
└─────────┼─────────────────┼──────────────────────┼──────────────┘
          │                 │                      │
    ┌─────▼─────────────────▼──────────────────────▼─────┐
    │                   BRIDGE LAYER                      │
    │  ┌────────────────┐  ┌────────────────────────┐    │
    │  │ CardConverter  │  │ LegacySystemInitializer│    │
    │  └───────┬────────┘  └───────────┬────────────┘    │
    │          │                       │                  │
    │  ┌───────▼───────────────────────▼────────────┐    │
    │  │        CardRepositoryInitializer           │    │
    │  └────────────────────────────────────────────┘    │
    └────────────────────────┬────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                    UNITY DATA LAYER                             │
│  ┌─────────────────┐   ┌─────────────────┐                     │
│  │ ScriptableObjects│   │  InGameCard    │                     │
│  │   (Card Data)    │   │ (Unity Objects)│                     │
│  └─────────────────┘   └─────────────────┘                     │
└─────────────────────────────────────────────────────────────────┘
```

## Bridge Files

### LegacySystemInitializer.cs

**Purpose**: Initializes static fields in extension classes with DI services.

**Why It Exists**: Extension methods (CardTypeExtensions, CardFamilyExtensions, MessageBoxBaseComponentExtensions) use static properties for ILocalizationService. Since DI cannot inject into static fields, this initializer bridges the gap.

**Called From**: `SharedServicesScope.RegisterBuildCallback()`

**Status**: **Permanent** - Required for extension method localization support.

```
SharedServicesScope.Configure()
        │
        ▼
RegisterBuildCallback()
        │
        ▼
LegacySystemInitializer.Initialize()
        │
        ├──► CardTypeExtensions.LocalizationService = ...
        ├──► CardFamilyExtensions.LocalizationService = ...
        └──► MessageBoxBaseComponentExtensions.LocalizationService = ...
```

**History**:
- Phase 46: Replaced LegacyCardLoader MonoBehaviour
- Phase 66: Removed unused GameStateService
- Phase 111: Documented as permanent bridge infrastructure
- Phase 115-118: Removed legacy ability class initialization (classes deleted)

---

### CardRepositoryInitializer.cs

**Purpose**: Loads ScriptableObject card data into the domain `CardRepository`.

**Why It Exists**: Unity stores card data in ScriptableObjects (serialized assets). The domain layer works with pure C# entities. This initializer bridges the two.

**Called From**: `LegacySystemInitializer.LoadCards()`

**Status**: **Permanent** - Always needed to load card data from Unity assets.

```
LegacySystemInitializer.LoadCards()
        │
        ▼
CardRepositoryInitializer.Initialize()
        │
        ├──► Resources.LoadAll<Card>()
        │
        ▼
CardConverter.ConvertToDomain()
        │
        ▼
CardRepository.RegisterCardDefinition()
```

---

### CardConverter.cs

**Purpose**: Converts legacy ScriptableObject cards to domain `Card` entities.

**Why It Exists**: ScriptableObjects use Unity types and legacy enum namespaces. Domain entities use pure C# types and domain namespaces. This converter translates between them.

**Key Conversions**:
- `Cards.CardFamily` → `JDG.Domain.Enums.CardFamily`
- `global::ConditionName` → `JDG.Domain.Enums.ConditionName`
- `InvocationCard` → `DomainCard.CreateInvocation()`
- `EquipmentCard` → `DomainCard.CreateEquipment()`
- `FieldCard` → `DomainCard.CreateField()`
- `EffectCard` → `DomainCard.CreateEffect()`

**Status**: **Permanent** - Required until cards are stored in a different format (e.g., JSON, database).

---

## Dependency Graph

```
SharedServicesScope
        │
        ▼
LegacySystemInitializer.Initialize() ────► Extension class static fields
        │
        ▼
LegacySystemInitializer.LoadCards()
        │
        ▼
CardRepositoryInitializer.Initialize()
        │
        ▼
CardConverter.ConvertToDomain()
        │
        ▼
CardRepository (Domain)
```

## Migration Status

| File | Status | Notes |
|------|--------|-------|
| LegacySystemInitializer | **Permanent** | Initializes extension class static fields |
| CardRepositoryInitializer | **Permanent** | Loads card data from Unity assets |
| CardConverter | **Permanent** | Converts ScriptableObjects to domain entities |

## Removed Files (Phase 118)

| File | Removed Date | Reason |
|------|--------------|--------|
| ModernAbilityAdapter.cs | Phase 118 | Legacy Ability class deleted; no longer needed |

## Usage Guidelines

1. **Don't delete these files** - They are essential infrastructure
2. **Don't add new bridge patterns** - New code should use DI and interfaces
3. **Update documentation** when modifying these files
4. **Test thoroughly** after any changes - these affect game startup

## Related Components

- `Services/AbilityExecutorAdapter.cs` - Executes modern IAbility implementations
- `Services/CardCollectionServiceAdapter.cs` - Bridges ICardCollectionService to scene objects
- `DI/SharedServicesScope.cs` - Calls LegacySystemInitializer
