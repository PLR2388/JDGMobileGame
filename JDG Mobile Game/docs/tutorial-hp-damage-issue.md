# Tutorial HP Damage Issue

## Description
During the tutorial, when the AI (Joueur 2) attacks the player (Joueur 1), the player should end up at 27.5 HP but remains at 30 HP.

## Expected Behavior
- Player (J1) starts with 30 HP
- AI attacks with a monster that has 3.5 ATK
- Player's defending card has 1 DEF
- Damage calculation: 3.5 ATK - 1 DEF = 2.5 damage to player HP
- Player HP after attack: 30 - 2.5 = **27.5 HP**

## Actual Behavior
- AI attack occurs ("Fisti détruit le Cliché raciste !")
- Player's card is destroyed
- Player HP is at **26/30** instead of dropping to 27.5
- See screenshot: J1 shows "Point de vie J2: 26/30" when it should be 27.5/30

## Location
- Scene: Tutorial
- Trigger: AI attack phase in tutorial sequence

## Files to Investigate
- `Assets/_Scripts/OnePlayer/TutoPlayerGameLoop.cs` - Tutorial game loop
- `Assets/Resources/Scenarii/tutorial.json` - Tutorial scenario definitions
- Combat/damage calculation services

## Status
**FIXED** - Root cause identified and fixed

## Root Cause Analysis
The damage value was being cast to `int` in `PlayerManager.HandleAttackIfOpponentIsPlayer()`, which truncated decimal values. However, the tutorial card-vs-card attack goes through a different code path (`CombatService.HandleAttackOverInvocation`) which doesn't use PlayerManager.

## Failed Attempts

### Attempt 1 (Reverted)
Tried to fix by inverting the formula and negating the value:
- Changed `DEF - ATK` to `ATK - DEF` in CombatService.ComputeDamageAttack()
- Added negation in PlayerManager.HandleAttackIfOpponentIsPlayer()

**Why it failed:** The original formula is fine (returns negative for damage). Also, this code path is for direct player attacks, not card-vs-card combat.

### Attempt 2 (Reverted)
Removed the `(int)` cast in PlayerManager.HandleAttackIfOpponentIsPlayer():
```csharp
// Before
_playerService.ChangeHealth(opponentId, (int)diff);

// After
_playerService.ChangeHealth(opponentId, diff);
```

**Why it failed:** The tutorial attack "Fisti>Cliché Raciste" is card-vs-card combat, which goes through `CombatService.HandleAttackOverInvocation()` → `HandleNegativeAttackResult()` → `PlayerStatus.ChangePv()`. This path doesn't go through PlayerManager at all.

## Code Paths for Damage

### Path A: Direct Player Attack (PlayerManager)
- `HandleAttack()` → `HandleAttackIfOpponentIsPlayer()` → `PlayerManager.HandleAttackIfOpponentIsPlayer()`
- Uses: `ComputeDamageAttack()` which returns `DEF - ATK`
- **Fixed:** Removed `(int)` cast

### Path B: Card-vs-Card Attack (CombatService) - USED IN TUTORIAL
- `HandleAttack()` → `HandleAttackOverInvocation()` → `HandleNegativeAttackResult()`
- Uses: `Opponent.Defense - Attacker.Attack` (line 235)
- Calls: `opponentPlayerStatus.ChangePv(resultAttack)` (line 387)
- **This path appears to use floats correctly** - needs further investigation

## Why HP Should Not Be Int
- Card stats (ATK/DEF) can have decimal values (e.g., 3.5 ATK, 1 DEF)
- Damage calculation produces decimals: 3.5 - 1 = 2.5 damage
- HP must remain float to accurately reflect combat results
- Casting to int causes cumulative rounding errors over multiple attacks

## Card Stats Verified
- **Fisti**: Base ATK 2.5, Base DEF 2.5
- **Cliché Raciste**: ATK 1, DEF 1
- **Equipment "Merde magique en plastique rose"**: +1 ATK, +1 DEF (equipped to Fisti at index 28)
- **Fisti with equipment**: 2.5 + 1 = 3.5 ATK

## ROOT CAUSE IDENTIFIED (via Debug Logs)

### Debug Output
```
Attacker: Fisti, ATK=5, DEF=1
Defender: Cliché Raciste, ATK=1, DEF=1
Equipment on Attacker: Merde magique en plastique rose
Result: 1 - 5 = -4
Opponent HP before: 30
ChangePv(-4) → newHealth=26 ✓
```

### The Problem: Fisti's Stats Are Wrong!

| Stat | Base | Equipment Bonus | Expected | Actual |
|------|------|-----------------|----------|--------|
| ATK  | 2.5  | +1              | 3.5      | **5**  |
| DEF  | 2.5  | +1              | 3.5      | **1**  |

**The HP calculation is correct** (30 + (-4) = 26). The bug is that **Fisti's stats are wrong**:
- ATK went from 2.5 to 5 (+2.5 instead of +1)
- DEF went from 2.5 to 1 (-1.5 instead of +1)

### Expected vs Actual Damage

| Scenario | ATK | DEF | Damage | HP Result |
|----------|-----|-----|--------|-----------|
| Expected | 3.5 | 1   | 2.5    | 27.5      |
| Actual   | 5   | 1   | 4      | 26        |

### Root Cause - DATA ERROR IN CARD ASSET

The equipment card "Merde magique en plastique rose" had the **wrong ability assigned** in its ScriptableObject asset!

**Investigation:**
1. Checked `Assets/Resources/Cards/Merde magique en plastique rose.asset`
2. Found: `EquipmentAbilities: 0c000000` (little-endian hex = decimal 12)
3. Enum value 12 = `Earn3ATKAndMinus1DEF` (+3 ATK, -1 DEF)
4. The description says "+1 ATK and +1 DEF" which should be enum value 13 = `Earn1ATKAnd1DEF`

**Enum Values (EquipmentAbilityName.cs):**
```
12 = Earn3ATKAndMinus1DEF → +3 ATK, -1 DEF (WRONG - was assigned)
13 = Earn1ATKAnd1DEF     → +1 ATK, +1 DEF (CORRECT - should be)
```

**Math that explains debug output:**
- Fisti base: 2.5 ATK, 2.5 DEF
- With wrong ability (+3/-1): 2.5+3=5.5→5 ATK, 2.5-1=1.5→1 DEF (truncated via int cast)
- This matches the debug: ATK=5, DEF=1 ✓

## Fix Applied

Changed the equipment card asset to use the correct ability:

**File:** `Assets/Resources/Cards/Merde magique en plastique rose.asset`
```yaml
# Before (wrong)
EquipmentAbilities: 0c000000  # 12 = Earn3ATKAndMinus1DEF

# After (correct)
EquipmentAbilities: 0d000000  # 13 = Earn1ATKAnd1DEF
```

## Secondary Fix - Float Stats Support (Phase 159)

The int truncation issue has been fully resolved by changing the domain `CardStats` from `int` to `float`:

### Files Changed
- `JDG.Domain/ValueObjects/CardStats.cs` - Changed Attack/Defense from int to float
- `JDG.Domain/Entities/Card.cs` - Changed CreateInvocation/ModifyStats/SetStats to float
- `Services/CardSyncService.cs` - Removed int casts in CreateLinkedCard
- `Services/AbilityExecutorAdapter.cs` - Removed int casts in ConvertToDomainCard
- `Bridge/CardConverter.cs` - Removed int casts in ConvertInvocationCard
- `JDG.Application/Abilities/Implementations/EquipmentAbility.cs` - Changed all stat bonuses to float
- `JDG.Application/Abilities/Implementations/FieldAbility.cs` - Removed int casts
- `JDG.Application/Abilities/ModifyStatsAbility.cs` - Changed modifiers to float

### Data Flow After Fix
1. Fisti base stats: 2.5 ATK, 2.5 DEF (float)
2. CreateLinkedCard passes float directly: 2.5/2.5
3. Equipment ability (+1/+1 float): 3.5/3.5
4. SyncCardState syncs float: 3.5/3.5 ✓

## Expected Result After Full Fix

| Scenario | ATK | DEF | Damage | HP Result |
|----------|-----|-----|--------|-----------|
| Before fix (wrong ability) | 5   | 1   | 4      | 26        |
| After data fix only | 3   | 1   | 2      | 28        |
| After full fix (float stats) | 3.5 | 1   | 2.5    | **27.5**  |

Both issues are now fixed:
1. ✅ Wrong equipment ability (data error in card asset)
2. ✅ Int truncation (domain now uses float)
