# JDG Ability Test Status

Last Updated: 2026-01-05

## Summary

| Category | Total | Scenario Tests | Integration | E2E | Status |
|----------|-------|----------------|-------------|-----|--------|
| Invocation | 61 | 159 | - | - | Complete |
| Equipment | 19 | 26 | 10 | - | Complete |
| Field | 12 | 23 | 14 | - | Complete |
| Effect | 25 | 53 | - | - | Complete |
| Conditions | 23 | 28 | - | - | Complete |
| Combinations | 16 | - | 15 | 16 | Complete |
| **Total** | **156** | **289** | **39** | **16** | **100%** |

**Total Tests: 344**

---

## Test Files Overview

### Scenario Tests (JDG.Application.Tests/Abilities/Scenarios/)

| File | Tests | Abilities Covered | Status |
|------|-------|-------------------|--------|
| DrawAbilityScenarioTests.cs | 16 | Draw1Card, Draw2Cards, Draw3Cards | Complete |
| SacrificeAbilityScenarioTests.cs | 13 | 16 sacrifice abilities | Complete |
| DeckSearchAbilityScenarioTests.cs | 19 | 12 deck search abilities | Complete |
| ProtectionAbilityScenarioTests.cs | 20 | 5 protection + 6 dependency abilities | Complete |
| CombatAbilityScenarioTests.cs | 18 | Combat abilities (attack, direct attack) | Complete |
| StatBoostAbilityScenarioTests.cs | 15 | 9 stat modification abilities | Complete |
| EquipmentAbilityScenarioTests.cs | 26 | 19 equipment abilities | Complete |
| FieldAbilityScenarioTests.cs | 23 | 12 field abilities | Complete |
| EffectAbilityScenarioTests.cs | 53 | 25+ effect abilities (full catalog) | Complete |
| ConditionScenarioTests.cs | 28 | 23 summon conditions | Complete |
| InvokeAbilityScenarioTests.cs | 11 | 4 invoke abilities (InvokeTentacules, InvokeDresseurBidulmon, InvokeSebOrJDG, SacrificeToInvoke) | Complete |
| DestructionAbilityScenarioTests.cs | 14 | 5 destruction abilities (DestroyFieldATK/DEF, CanOnlyAttackItself, KillOpponent/Enemy) | Complete |
| ResurrectionAbilityScenarioTests.cs | 16 | 4 resurrection abilities (ComesBackFromDeath, ComesBackFromDeath5Times, GiveDeathWhenDie, CopyBenzaieJeune) | Complete |
| ControlAbilityScenarioTests.cs | 17 | 4 control abilities (SendAllCardToHands, SkipOpponentAttackEveryTurn, Control1Opponent, Win1ATK1DefJapon) | Complete |

### Integration Tests (JDG.Application.Tests/Abilities/Integration/)

| File | Tests | Scenarios | Status |
|------|-------|-----------|--------|
| EquipmentInvocationSynergyTests.cs | 10 | Equipment + Invocation combos | Complete |
| FieldFamilySynergyTests.cs | 14 | Field + Family synergies | Complete |
| DependencyChainTests.cs | 15 | Card dependency relationships | Complete |

### E2E Tests (PlayMode/Abilities/)

| File | Tests | Scenarios | Status |
|------|-------|-----------|--------|
| KeyCombinationE2ETests.cs | 16 | All 16 key combinations | Complete |

---

## Invocation Abilities (61)

### Draw Abilities (3) - Complete

| Ability | Test File | Tests | Status |
|---------|-----------|-------|--------|
| Draw1Card | DrawAbilityScenarioTests.cs | 5 | Complete |
| Draw2Cards | DrawAbilityScenarioTests.cs | 5 | Complete |
| Draw3Cards | DrawAbilityScenarioTests.cs | 6 | Complete |

**Cards using these abilities:**
- Draw1Card: Generic draw
- Draw2Cards: L'Elfette
- Draw3Cards: Theodule, Sangoku

### Sacrifice Abilities (16) - Complete

| Ability | Test File | Status |
|---------|-----------|--------|
| SacrificeArchibaldVonGrenier | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeBenzaieJeune | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeJoueurDuGrenier | SacrificeAbilityScenarioTests.cs | Complete |
| Sacrifice3Atk3Def | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeWizard | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeDeveloper3Atk3Def | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeHardCorner3Atk3Def | SacrificeAbilityScenarioTests.cs | Complete |
| Sacrifice2Japan | SacrificeAbilityScenarioTests.cs | Complete |
| Sacrifice2Incarnation | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeGranolax | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeSebDuGrenier | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeClicheRaciste | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeToInvoke | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeSebDuGrenierOnHardCornerForAtkDef | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeJDGOnStudioDevForAtkDef | SacrificeAbilityScenarioTests.cs | Complete |
| SacrificeBenzaieJeuneForAtkDef | SacrificeAbilityScenarioTests.cs | Complete |

### Deck Search Abilities (12) - Complete

| Ability | Test File | Status |
|---------|-----------|--------|
| AddSpatialFromDeck | DeckSearchAbilityScenarioTests.cs | Complete |
| GetNounoursFromDeck | DeckSearchAbilityScenarioTests.cs | Complete |
| GetPetitePortionDeRizFromDeck | DeckSearchAbilityScenarioTests.cs | Complete |
| GetLyceeMagiqueGeorgesPompidouFromDeck | DeckSearchAbilityScenarioTests.cs | Complete |
| ChangeFieldWithFieldFromDeck | DeckSearchAbilityScenarioTests.cs | Complete |
| GetZozanKebabFromDeck | DeckSearchAbilityScenarioTests.cs | Complete |
| GetConvocationAuLyceeFromDeck | DeckSearchAbilityScenarioTests.cs | Complete |
| GetBenzaieJeuneFromDeck | DeckSearchAbilityScenarioTests.cs | Complete |
| GetEquipmentCardWithoutAttack | DeckSearchAbilityScenarioTests.cs | Complete |
| GetPatronInfogramesFromDeckYellowTrash | DeckSearchAbilityScenarioTests.cs | Complete |
| GetCanardSignal | DeckSearchAbilityScenarioTests.cs | Complete |
| GetForetElfesSylvains | DeckSearchAbilityScenarioTests.cs | Complete |

### Protection Abilities (5) - Complete

| Ability | Test File | Status |
|---------|-----------|--------|
| CantBeAttackIfComics | ProtectionAbilityScenarioTests.cs | Complete |
| ProtectedBehindStarlightUnicorn | ProtectionAbilityScenarioTests.cs | Complete |
| CantBeAttackKill | ProtectionAbilityScenarioTests.cs | Complete |
| SurviveOneTurn | ProtectionAbilityScenarioTests.cs | Complete |
| ProtectBehindGreaterDef | ProtectionAbilityScenarioTests.cs | Complete |

### Dependency Abilities (6) - Complete

| Ability | Test File | Status |
|---------|-----------|--------|
| CantLiveWithoutBenzaieOrBenzaieJeune | ProtectionAbilityScenarioTests.cs, DependencyChainTests.cs | Complete |
| CantLiveWithoutJDG | ProtectionAbilityScenarioTests.cs, DependencyChainTests.cs | Complete |
| CantLiveWithoutComics | ProtectionAbilityScenarioTests.cs, DependencyChainTests.cs | Complete |
| CantLiveWithoutHuman | ProtectionAbilityScenarioTests.cs, DependencyChainTests.cs | Complete |
| CantLiveWithoutGranolaxOrMechaGranolax | ProtectionAbilityScenarioTests.cs, DependencyChainTests.cs | Complete |
| CantLiveWithoutJapon | ProtectionAbilityScenarioTests.cs, DependencyChainTests.cs | Complete |

### Combat Abilities - Complete

| Ability | Test File | Status |
|---------|-----------|--------|
| KillEnemyIfDestroy | CombatAbilityScenarioTests.cs | Complete |
| SkipOpponentAttackEveryTurn | CombatAbilityScenarioTests.cs | Complete |
| KillOpponentInvocation | CombatAbilityScenarioTests.cs | Complete |
| CanAttackDirectly | CombatAbilityScenarioTests.cs | Complete |
| DoubleAttackPerTurn | CombatAbilityScenarioTests.cs | Complete |
| GetCardFromGraveyardOnDeath | CombatAbilityScenarioTests.cs | Complete |
| PutCardInGraveyard | CombatAbilityScenarioTests.cs | Complete |
| ReturnToHand | CombatAbilityScenarioTests.cs | Complete |
| Control1OpponentInvocationCard | CombatAbilityScenarioTests.cs | Complete |

### Stat Boost Abilities (9) - Complete

| Ability | Test File | Status |
|---------|-----------|--------|
| GiveFamilyStats | StatBoostAbilityScenarioTests.cs | Complete |
| ConditionalStats | StatBoostAbilityScenarioTests.cs | Complete |
| CopyStats | StatBoostAbilityScenarioTests.cs | Complete |
| GiveAktDefToRpgMember | StatBoostAbilityScenarioTests.cs | Complete |
| GiveAktDefToFistilandMember | StatBoostAbilityScenarioTests.cs | Complete |
| AtkDefBoostOnSummon | StatBoostAbilityScenarioTests.cs | Complete |
| AtkBoostPerFamily | StatBoostAbilityScenarioTests.cs | Complete |
| DefBoostPerFamily | StatBoostAbilityScenarioTests.cs | Complete |
| DynamicStatModifier | StatBoostAbilityScenarioTests.cs | Complete |

---

## Equipment Abilities (19) - Complete

| Ability | Test File | Status |
|---------|-----------|--------|
| MultiplyDefBy2ButPreventAttack | EquipmentAbilityScenarioTests.cs | Complete |
| MultiplyAtkBy3 | EquipmentAbilityScenarioTests.cs | Complete |
| MultiplyAtkBy2AndDefByHalf | EquipmentAbilityScenarioTests.cs | Complete |
| Earn1ATKAndMinus1DEF | EquipmentAbilityScenarioTests.cs | Complete |
| Earn2ATK | EquipmentAbilityScenarioTests.cs | Complete |
| Earn1ATKAnd1DEF | EquipmentAbilityScenarioTests.cs | Complete |
| EarnOneQuarterATKPerHandCards | EquipmentAbilityScenarioTests.cs | Complete |
| EarnOneQuarterDEFPerHandCards | EquipmentAbilityScenarioTests.cs | Complete |
| SetATKToOne | EquipmentAbilityScenarioTests.cs | Complete |
| SetDefToZero | EquipmentAbilityScenarioTests.cs | Complete |
| DirectAttack | EquipmentAbilityScenarioTests.cs | Complete |
| CantBeAttackByOtherInvocations | EquipmentAbilityScenarioTests.cs | Complete |
| PreventNewOpponentToAttack | EquipmentAbilityScenarioTests.cs | Complete |
| SwitchEquipmentCard | EquipmentAbilityScenarioTests.cs | Complete |
| ProtectOneTimeFromDestruction | EquipmentAbilityScenarioTests.cs | Complete |
| CancelInvocationAbility | EquipmentAbilityScenarioTests.cs | Complete |
| Remove1ATKAnd1DEF | EquipmentAbilityScenarioTests.cs | Complete |
| Loose2ATK | EquipmentAbilityScenarioTests.cs | Complete |
| Earn2DEF | EquipmentAbilityScenarioTests.cs | Complete |

---

## Field Abilities (12) - Complete

| Ability | Test File | Status |
|---------|-----------|--------|
| Earn1DEFForSpatialFamily | FieldAbilityScenarioTests.cs | Complete |
| Earn1ATKForJapanFamily | FieldAbilityScenarioTests.cs | Complete |
| Earn1HalfATKAndMinusHalfDEFForHCFamily | FieldAbilityScenarioTests.cs | Complete |
| Earn2DEFAndMinusOneATKForIncarnationFamily | FieldAbilityScenarioTests.cs | Complete |
| EarnHalfATKAndDefForRpgFamily | FieldAbilityScenarioTests.cs | Complete |
| Earn1HalfDEFAndMinusHalfATKForDevFamily | FieldAbilityScenarioTests.cs | Complete |
| ChangePatronInfogramFamilyToDev | FieldAbilityScenarioTests.cs | Complete |
| ChangeJMBruitagesFamilyToDev | FieldAbilityScenarioTests.cs | Complete |
| DrawOneMoreCard | FieldAbilityScenarioTests.cs | Complete |
| SkipDrawToGetFistilandInvocation | FieldAbilityScenarioTests.cs | Complete |
| EarnHalfHPPerWizardInvocationEachTurn | FieldAbilityScenarioTests.cs | Complete |
| Earn2ATKAndMinus1DEFForComicsFamily | FieldAbilityScenarioTests.cs | Complete |

---

## Effect Abilities (25) - Complete

| Ability | Test File | Status |
|---------|-----------|--------|
| LimitHandCardTo5 | EffectAbilityScenarioTests.cs | Complete |
| SwitchAtkDef | EffectAbilityScenarioTests.cs | Complete |
| DivideDEFOpponentBy2 | EffectAbilityScenarioTests.cs | Complete |
| DoubleAttackPerTurn | EffectAbilityScenarioTests.cs | Complete |
| Control1OpponentInvocationCard | EffectAbilityScenarioTests.cs | Complete |
| ChangeFieldCard | EffectAbilityScenarioTests.cs | Complete |
| DestroyOpponentInvocation | EffectAbilityScenarioTests.cs | Complete |
| DestroyAllOpponentEquipment | EffectAbilityScenarioTests.cs | Complete |
| LookOpponentHand | EffectAbilityScenarioTests.cs | Complete |
| LookTopDeckCards | EffectAbilityScenarioTests.cs | Complete |
| InvokeFromDeck | EffectAbilityScenarioTests.cs | Complete |
| ReturnCardToHand | EffectAbilityScenarioTests.cs | Complete |
| SendAllCardsToHand | EffectAbilityScenarioTests.cs | Complete |
| SkipOpponentDraw | EffectAbilityScenarioTests.cs | Complete |
| DiscardRandomCard | EffectAbilityScenarioTests.cs | Complete |
| DestroyFieldCard | EffectAbilityScenarioTests.cs | Complete |
| NegateAbility | EffectAbilityScenarioTests.cs | Complete |
| BlockAttacks | EffectAbilityScenarioTests.cs | Complete |
| GainHP | EffectAbilityScenarioTests.cs | Complete |
| DealDirectDamage | EffectAbilityScenarioTests.cs | Complete |
| StealEquipment | EffectAbilityScenarioTests.cs | Complete |
| CopyAbility | EffectAbilityScenarioTests.cs | Complete |
| ShuffleDeck | EffectAbilityScenarioTests.cs | Complete |
| SwapFieldPositions | EffectAbilityScenarioTests.cs | Complete |
| ReduceStats | EffectAbilityScenarioTests.cs | Complete |

---

## Conditions (23) - Complete

| Condition | Test File | Status |
|-----------|-----------|--------|
| BenzaieJeuneOrBenzaieOnField | ConditionScenarioTests.cs | Complete |
| ArchibalVonGrenierOnField | ConditionScenarioTests.cs | Complete |
| ZozanKebabOnField | ConditionScenarioTests.cs | Complete |
| JoueurDuGrenierOnFieldCondition | ConditionScenarioTests.cs | Complete |
| ForetDesElfesSylvainsOnField | ConditionScenarioTests.cs | Complete |
| WizardOnField | ConditionScenarioTests.cs | Complete |
| LyceeMagiqueGeorgesPompidouOnField | ConditionScenarioTests.cs | Complete |
| ComicsOnField | ConditionScenarioTests.cs | Complete |
| HumanOnField | ConditionScenarioTests.cs | Complete |
| SebDuGrenierOnField | ConditionScenarioTests.cs | Complete |
| MechaGronolaxOrGranolaxOnField | ConditionScenarioTests.cs | Complete |
| JapanOnField | ConditionScenarioTests.cs | Complete |
| BenzaieJeuneCassetteVhsEquiped | ConditionScenarioTests.cs | Complete |
| JoueurDuGrenierCanarangEquiped | ConditionScenarioTests.cs | Complete |
| SebDuGrenierMerdePlastiqueBleuEquiped | ConditionScenarioTests.cs | Complete |
| ClicheRacisteMerdeRoseEquiped | ConditionScenarioTests.cs | Complete |
| ThreeAtk3Def | ConditionScenarioTests.cs | Complete |
| Developer3Atk3Def2Cards | ConditionScenarioTests.cs | Complete |
| HardCorner3Atk3Def2Cards | ConditionScenarioTests.cs | Complete |
| Japan2Cards | ConditionScenarioTests.cs | Complete |
| Incarnation2Cards | ConditionScenarioTests.cs | Complete |
| TenDeathYellowTrash | ConditionScenarioTests.cs | Complete |
| GranolaxAlreadyDead | ConditionScenarioTests.cs | Complete |

---

## Key Combinations (16) - Complete

### Equipment + Invocation Synergies

| Combination | Test File | Status |
|-------------|-----------|--------|
| Parachute dore + Benzaie | EquipmentInvocationSynergyTests.cs, KeyCombinationE2ETests.cs | Complete |
| Canarang + Joueur Du Grenier | EquipmentInvocationSynergyTests.cs, KeyCombinationE2ETests.cs | Complete |
| Cassette VHS + Benzaie jeune | EquipmentInvocationSynergyTests.cs, KeyCombinationE2ETests.cs | Complete |
| Le Salami + Canardman | EquipmentInvocationSynergyTests.cs, KeyCombinationE2ETests.cs | Complete |

### Field + Family Synergies

| Combination | Test File | Status |
|-------------|-----------|--------|
| Le Hard Corner + Fistiland | FieldFamilySynergyTests.cs, KeyCombinationE2ETests.cs | Complete |
| Studio de developpement + Developer | FieldFamilySynergyTests.cs, KeyCombinationE2ETests.cs | Complete |
| Tokyo-3 + Japan | FieldFamilySynergyTests.cs, KeyCombinationE2ETests.cs | Complete |
| Centre Spatial de Kourou + Spatial | FieldFamilySynergyTests.cs, KeyCombinationE2ETests.cs | Complete |
| Lycee magique Georges Pompidou + Wizard | FieldFamilySynergyTests.cs, KeyCombinationE2ETests.cs | Complete |
| Foret des elfes sylvains + Wizard | FieldFamilySynergyTests.cs, KeyCombinationE2ETests.cs | Complete |
| Zozan Kebab + Human | FieldFamilySynergyTests.cs, KeyCombinationE2ETests.cs | Complete |
| Canardcity + Fistiland | FieldFamilySynergyTests.cs, KeyCombinationE2ETests.cs | Complete |

### Dependency Chains

| Combination | Test File | Status |
|-------------|-----------|--------|
| Alpha Man + Benzaie/Benzaie jeune | DependencyChainTests.cs, KeyCombinationE2ETests.cs | Complete |
| Starlight Unicorn + Granolax/Mecha-Granolax | DependencyChainTests.cs, KeyCombinationE2ETests.cs | Complete |
| Henry Potdebeurre + Joueur Du Grenier | DependencyChainTests.cs, KeyCombinationE2ETests.cs | Complete |
| L'homme-banane + Comics | DependencyChainTests.cs, KeyCombinationE2ETests.cs | Complete |

---

## Change Log

| Date | Phase | Changes |
|------|-------|---------|
| 2026-01-05 | Initial | Created status document with all 156 abilities |
| 2026-01-05 | Phase 1 | Created foundation: AbilityScenarioFixtures, AbilityTestAssertions, AbilityScenarioTestBase |
| 2026-01-05 | Phase 2a | Implemented Draw ability scenario tests (Draw1Card, Draw2Cards, Draw3Cards) - 16 tests |
| 2026-01-05 | Phase 2b | Implemented Sacrifice ability scenario tests (16 abilities) - 13 tests |
| 2026-01-05 | Phase 2c | Implemented Deck Search ability scenario tests (12 abilities) - 19 tests |
| 2026-01-05 | Phase 2d | Implemented Protection and Dependency ability scenario tests - 20 tests |
| 2026-01-05 | Phase 2e | Implemented Combat ability scenario tests - 18 tests |
| 2026-01-05 | Phase 2f | Implemented Stat Boost ability scenario tests (9 abilities) - 15 tests |
| 2026-01-05 | Phase 3 | Implemented Equipment ability scenario tests (19 abilities) - 26 tests |
| 2026-01-05 | Phase 4 | Implemented Field ability scenario tests (12 abilities) - 23 tests |
| 2026-01-05 | Phase 5 | Implemented Effect ability scenario tests (25 abilities) - 35 tests |
| 2026-01-05 | Phase 6 | Implemented Condition scenario tests (23 conditions) - 28 tests |
| 2026-01-05 | Phase 7 | Implemented Integration tests (Equipment+Invocation, Field+Family, Dependency) - 39 tests |
| 2026-01-05 | Phase 8 | Implemented E2E PlayMode tests (Key Combinations) - 16 tests |
| 2026-01-05 | Final | Updated status document with complete test coverage - 268 total tests |
| 2026-01-05 | Gap Analysis | Compared tests to CARD_POWER_CATALOG.md checklist, identified ~28 missing abilities |
| 2026-01-05 | Gap Closure | Created 4 new test files + updated EffectAbilityScenarioTests.cs - 76 new tests added |
| 2026-01-05 | | - InvokeAbilityScenarioTests.cs (11 tests): InvokeTentacules, InvokeDresseurBidulmon, InvokeSebOrJDG, SacrificeToInvoke |
| 2026-01-05 | | - DestructionAbilityScenarioTests.cs (14 tests): DestroyFieldATK, DestroyFieldDEF, CanOnlyAttackItself, KillOpponentInvocation, KillEnemyIfDestroy |
| 2026-01-05 | | - ResurrectionAbilityScenarioTests.cs (16 tests): ComesBackFromDeath, ComesBackFromDeath5Times, GiveDeathWhenDie, CopyBenzaieJeune |
| 2026-01-05 | | - ControlAbilityScenarioTests.cs (17 tests): SendAllCardToHands, SkipOpponentAttackEveryTurn, Control1OpponentInvocationCard, Win1ATK1DefJapon |
| 2026-01-05 | | - EffectAbilityScenarioTests.cs (+18 tests): Full catalog coverage including Convocation au lycee, Fatalite, Kebab magique, etc. |
| 2026-01-05 | Complete | All abilities from CARD_POWER_CATALOG.md now have test coverage - 344 total tests |

---

## Notes

### Test Infrastructure
- **AbilityScenarioTestBase**: Base class for all scenario tests
- **AbilityScenarioFixtures**: Pre-configured game states for testing
- **AbilityTestAssertions**: Custom assertions for ability testing
- **TestCardFactory**: Factory for creating test cards

### Known Limitations
1. Condition tests verify configuration/existence only - functional tests require PlayerCards MonoBehaviour
2. E2E tests require Unity PlayMode test runner
3. Some edge cases deferred to manual testing

### Test Categories
- `[Category("Abilities")]` - All ability tests
- `[Category("Scenario")]` - Scenario-based tests
- `[Category("Integration")]` - Integration tests
- `[Category("E2E")]` - End-to-end tests
- `[Category("KeyCombinations")]` - Key combination tests
