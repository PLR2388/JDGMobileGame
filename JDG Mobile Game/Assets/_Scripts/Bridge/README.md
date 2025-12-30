# Bridge Layer

The Bridge layer provides compatibility between the legacy Unity-based card system and the modern clean architecture. These files are essential infrastructure, not temporary code.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                    MODERN ARCHITECTURE                           │
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
    │  └───────────────────┬────────────────────────┘    │
    │                      │                              │
    │  ┌───────────────────▼────────────────────────┐    │
    │  │          ModernAbilityAdapter              │    │
    │  └────────────────────────────────────────────┘    │
    └────────────────────────┬────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                    LEGACY ARCHITECTURE                           │
│  ┌─────────────────┐   ┌─────────────────┐   ┌───────────────┐  │
│  │ ScriptableObjects│   │  InGameCard    │   │Legacy Ability │  │
│  │   (Card Data)    │   │ (Unity Objects)│   │ (Base Class)  │  │
│  └─────────────────┘   └─────────────────┘   └───────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

## Bridge Files

### LegacySystemInitializer.cs

**Purpose**: Initializes legacy static fields with modern DI services.

**Why It Exists**: Legacy ability classes (`Ability`, `EffectAbility`, `FieldAbility`) have static properties for services. DI can't inject into static fields, so this initializer bridges the gap.

**Called From**: `SharedServicesScope.RegisterBuildCallback()`

**Removal Condition**: When all 33+ legacy ability implementations are migrated to `IAbility`.

```
SharedServicesScope.Configure()
        │
        ▼
RegisterBuildCallback()
        │
        ▼
LegacySystemInitializer.Initialize()
        │
        ├──► Ability.LocalizationService = ...
        ├──► Ability.DialogService = ...
        ├──► EffectAbility.LocalizationService = ...
        ├──► FieldAbility.LocalizationService = ...
        └──► CardTypeExtensions.LocalizationService = ...
```

---

### CardRepositoryInitializer.cs

**Purpose**: Loads ScriptableObject card data into the domain `CardRepository`.

**Why It Exists**: Unity stores card data in ScriptableObjects (serialized assets). The domain layer works with pure C# entities. This initializer bridges the two.

**Called From**: `LegacySystemInitializer.Initialize()`

**Removal Condition**: **Permanent** - Always needed to load card data from Unity assets.

```
LegacySystemInitializer.Initialize()
        │
        ▼
CardRepositoryInitializer.LoadCards()
        │
        ├──► Resources.LoadAll<InvocationCard>()
        ├──► Resources.LoadAll<EffectCard>()
        ├──► Resources.LoadAll<FieldCard>()
        └──► Resources.LoadAll<EquipmentCard>()
                │
                ▼
        CardConverter.ToDomainCard()
                │
                ▼
        CardRepository.Add()
```

---

### CardConverter.cs

**Purpose**: Converts legacy ScriptableObject cards to domain `Card` entities.

**Why It Exists**: ScriptableObjects use Unity types. Domain entities use pure C# types. This converter translates between them.

**Key Conversions**:
- `Cards.CardFamily` → `JDG.Domain.Enums.CardFamily`
- `Cards.CardType` → `JDG.Domain.Enums.CardType`
- `Ability.AbilityName` → `JDG.Domain.AbilityName`
- `Sprite` → Image path (string)

**Removal Condition**: **Permanent** - Required until cards are stored in a different format (e.g., JSON, database).

---

### ModernAbilityAdapter.cs

**Purpose**: Wraps modern `IAbility` implementations to satisfy the legacy `Ability` base class interface.

**Why It Exists**: Legacy `InGameCard` classes expect abilities that extend `Ability`. Modern abilities implement `IAbility`. This adapter allows modern abilities to work with legacy cards.

**Used By**: `AbilityProviderService`, `AbilityExecutorAdapter`

**Removal Condition**: When all `InGameCard` classes use `IAbility` instead of `Ability`.

```
AbilityProviderService.GetAbility()
        │
        ▼
AbilityRegistry.Get(abilityName)
        │
        ▼
IAbility implementation
        │
        ▼
ModernAbilityAdapter.Wrap(ability)
        │
        ▼
Legacy Ability-compatible object
```

---

## Dependency Graph

```
SharedServicesScope
        │
        ▼
LegacySystemInitializer ─────────────┐
        │                             │
        ▼                             ▼
CardRepositoryInitializer     Static field initialization
        │                     (Ability.LocalizationService, etc.)
        ▼
CardConverter
        │
        ▼
CardRepository (Domain)


AbilityRegistry ◄───── AbilityProviderService
        │                     │
        ▼                     ▼
IAbility ──────────► ModernAbilityAdapter
                              │
                              ▼
                     InGameCard classes
```

## Migration Status

| File | Status | Removal Condition |
|------|--------|-------------------|
| LegacySystemInitializer | Active | When all legacy abilities deleted |
| CardRepositoryInitializer | Permanent | Always needed (data loading) |
| CardConverter | Permanent | Until cards use non-Unity format |
| ModernAbilityAdapter | Active | When InGameCard uses IAbility |

## Usage Guidelines

1. **Don't delete these files** - They are essential infrastructure
2. **Don't add new legacy patterns** - New code should use DI and IAbility
3. **Update documentation** when modifying these files
4. **Test thoroughly** after any changes - these affect game startup

## Related Components

- `Services/AbilityExecutorAdapter.cs` - Executes both legacy and modern abilities
- `Services/CardCollectionServiceAdapter.cs` - Bridges ICardCollectionService to scene objects
- `DI/SharedServicesScope.cs` - Calls LegacySystemInitializer
