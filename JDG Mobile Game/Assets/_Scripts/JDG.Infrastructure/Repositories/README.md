# Infrastructure - Repositories

This directory contains the infrastructure implementations of repository interfaces defined in the Application layer.

## ScriptableObject Integration

The `CardRepository` bridges the old Unity ScriptableObject card system with the new domain layer.

### How It Works

1. **CardConverter** - Converts old ScriptableObject cards to domain entities
   - `InvocationCard` → Domain `Card` (Invocation type)
   - `EquipmentCard` → Domain `Card` (Equipment type)
   - `FieldCard` → Domain `Card` (Field type)
   - `EffectCard` → Domain `Card` (Effect type)
   - `ContreCard` → Domain `Card` (Contre type)

2. **CardRepository** - Manages card definitions and instances
   - Loads all cards from `Resources/Cards` folder on initialization
   - Maintains a registry of card definitions (templates)
   - Creates card instances with unique `CardId` when needed

3. **GameBootstrapper** - Initializes the repository at startup
   - Calls `CardRepository.Initialize()` after DI container setup
   - Loads all ~174 card definitions into memory

### Old vs New Card System

**Old System (ScriptableObjects):**
```csharp
// Old namespace: Cards, Cards.InvocationCards, etc.
public class InvocationCard : Card
{
    public InvocationCardStats BaseInvocationCardStats;
    public List<AbilityName> Abilities;
    public List<ConditionName> Conditions;
}
```

**New System (Domain Entities):**
```csharp
// New namespace: JDG.Domain.Entities
public class Card
{
    public CardId Id { get; }  // Unique Guid-based ID
    public CardType Type { get; }
    public CardStats? Stats { get; }  // Immutable value object
    public ReadOnlyCollection<AbilityName> Abilities { get; }
}
```

### Key Differences

1. **Immutability**: New cards are immutable after creation
2. **Type Safety**: New `CardId` and `CardStats` value objects prevent bugs
3. **Testability**: Domain cards have no Unity dependencies
4. **ECS-Ready**: Designed for future ECS migration with snapshot pattern

### Enum Conversions

The converter handles two sets of enums:

- **Old Enums**: Global namespace (`AbilityName`, `ConditionName`, etc.)
- **New Enums**: `JDG.Domain` namespace (same names, different namespace)

Conversion is done via string parsing since enum names match:
```csharp
var newAbility = (JDG.Domain.AbilityName)Enum.Parse(
    typeof(JDG.Domain.AbilityName),
    oldAbility.ToString()
);
```

### Usage Example

```csharp
// Get card repository
var cardRepo = ServiceLocator.GetCardRepository();

// Get a card definition by title
var card = cardRepo.GetCardByTitle("Slime");

// Create a new card instance (for player's deck)
var cardInstance = cardRepo.CreateCardInstance("Slime");
// cardInstance has a unique CardId

// Query cards
var invocations = cardRepo.GetCardsByType(CardType.Invocation);
var wizards = cardRepo.GetCardsByFamily(CardFamily.Wizard);
```

### Migration Notes

- Old code still uses ScriptableObjects directly
- New code uses the `CardRepository` via DI
- Both systems coexist during the transition
- ScriptableObjects remain the source of truth for card data
- Once migration is complete, we can consider moving card data to JSON/scriptable pipelines

## Future Improvements

1. **Asset Database Loading**: Replace Resources.Load with AssetDatabase for editor
2. **Async Loading**: Add async card loading for better performance
3. **Card Validation**: Validate card data at load time
4. **Hot Reload**: Support runtime card updates in editor
5. **Data Migration**: Consider migrating from ScriptableObjects to data files

## Testing

See `Assets/Tests/JDG.Infrastructure.Tests/Repositories/` for repository tests:
- `CardRepositoryTests.cs` - Tests card loading and querying
- `PlayerRepositoryTests.cs` - Tests player creation and management
- `GameStateRepositoryTests.cs` - Tests game state persistence
