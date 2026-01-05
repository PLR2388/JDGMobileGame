# JDG Ability Test Status

Last Updated: 2026-01-05

## Summary

| Category | Total | Scenario | E2E | Passing | Coverage |
|----------|-------|----------|-----|---------|----------|
| Invocation | 61 | 3/61 | 0/10 | 3 | 5% |
| Equipment | 19 | 0/19 | 0/4 | 0 | 0% |
| Field | 12 | 0/12 | 0/10 | 0 | 0% |
| Effect | 25 | 0/25 | 0/4 | 0 | 0% |
| Conditions | 23 | 0/23 | - | 0 | 0% |
| Combinations | 16 | - | 0/16 | 0 | 0% |
| **Total** | **156** | **3/140** | **0/44** | **3** | **2%** |

---

## Test Files

### Scenario Tests (JDG.Application.Tests/Abilities/Scenarios/)

| File | Abilities Covered | Status |
|------|-------------------|--------|
| DrawAbilityScenarioTests.cs | 3 | Complete |
| SacrificeAbilityScenarioTests.cs | 16 | Pending |
| DeckSearchAbilityScenarioTests.cs | 12 | Pending |
| InvokeAbilityScenarioTests.cs | 4 | Pending |
| ProtectionAbilityScenarioTests.cs | 5 | Pending |
| DestructionAbilityScenarioTests.cs | 5 | Pending |
| StatBoostAbilityScenarioTests.cs | 9 | Pending |
| DependencyAbilityScenarioTests.cs | 6 | Pending |
| ResurrectionAbilityScenarioTests.cs | 3 | Pending |
| ControlAbilityScenarioTests.cs | 3 | Pending |
| EquipmentAbilityScenarioTests.cs | 19 | Pending |
| FieldAbilityScenarioTests.cs | 12 | Pending |
| EffectAbilityScenarioTests.cs | 25 | Pending |
| ConditionScenarioTests.cs | 23 | Pending |

### E2E Tests (PlayMode/Abilities/)

| File | Scenarios | Status |
|------|-----------|--------|
| InvocationAbilityE2ETests.cs | 10 | Pending |
| EquipmentAbilityE2ETests.cs | 4 | Pending |
| FieldAbilityE2ETests.cs | 10 | Pending |
| EffectAbilityE2ETests.cs | 4 | Pending |
| KeyCombinationE2ETests.cs | 16 | Pending |

---

## Invocation Abilities (61)

### Draw Abilities (3)

#### Draw1Card
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DrawAbilityScenarioTests.cs | [x] Complete | 4 tests |
| E2E | - | N/A | |

**Cards using this ability:** None documented

**Scenarios to cover:**
- [x] Basic draw from deck to hand
- [x] Draw when deck is empty (should fail gracefully)
- [x] Draw triggers OnCardDrawn event

**Edge cases:**
- [x] Deck has exactly 1 card
- [ ] Multiple draws in same turn

**Known issues:** None

---

#### Draw2Cards
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DrawAbilityScenarioTests.cs | [x] Complete | 4 tests |
| E2E | - | N/A | |

**Cards using this ability:** L'Elfette

**Scenarios to cover:**
- [x] L'Elfette OnSummon triggers Draw2Cards
- [x] Draw2Cards with only 1 card in deck (partial draw)
- [x] Hand size increases by 2

**Edge cases:**
- [x] Deck has exactly 2 cards
- [ ] Combined with DrawOneMoreCard field ability

**Known issues:** None

---

#### Draw3Cards
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DrawAbilityScenarioTests.cs | [x] Complete | 5 tests |
| E2E | - | N/A | |

**Cards using this ability:** Theodule, Sangoku

**Scenarios to cover:**
- [x] Sangoku OnSummon triggers Draw3Cards
- [x] Draw3Cards with only 2 cards in deck (partial draw)
- [x] Hand size increases by 3

**Edge cases:**
- [ ] Deck has exactly 3 cards
- [x] Multiple Draw3Cards in same turn

**Known issues:** None

---

### Sacrifice Abilities (16)

#### SacrificeArchibaldVonGrenier
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | InvocationAbilityE2ETests.cs | [ ] Pending | |

**Cards using this ability:** Alpha V De Gelganech

**Scenarios to cover:**
- [ ] Sacrifice when Archibald Von Grenier on field
- [ ] Cannot activate without Archibald on field
- [ ] Card goes to graveyard after sacrifice

**Edge cases:**
- [ ] Multiple Archibalds on field
- [ ] Archibald destroyed same turn

**Known issues:** None

---

#### SacrificeBenzaieJeune
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | InvocationAbilityE2ETests.cs | [ ] Pending | |

**Cards using this ability:** Benzaie

**Scenarios to cover:**
- [ ] Sacrifice when Benzaie jeune on field
- [ ] Cannot activate without Benzaie jeune on field
- [ ] Benzaie gains stats after sacrifice

**Edge cases:**
- [ ] Benzaie jeune has equipment attached
- [ ] Multiple Benzaie jeune on field

**Known issues:** None

---

#### SacrificeJoueurDuGrenier
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | InvocationAbilityE2ETests.cs | [ ] Pending | |

**Cards using this ability:** Canardman, Enfant De Juron, Henry Potdebeurre

**Scenarios to cover:**
- [ ] Sacrifice when Joueur Du Grenier on field
- [ ] Cannot activate without JDG on field
- [ ] Different cards using same ability

**Edge cases:**
- [ ] JDG has Canarang equipped (special condition)

**Known issues:** None

---

#### Sacrifice3Atk3Def
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Dictateur Sympa

**Scenarios to cover:**
- [ ] Sacrifice card with 3+ ATK and 3+ DEF
- [ ] Cannot sacrifice card with insufficient stats
- [ ] Stat calculation includes equipment bonuses

**Edge cases:**
- [ ] Card reaches 3/3 only with equipment
- [ ] Multiple valid sacrifice targets

**Known issues:** None

---

#### SacrificeWizard
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Georges Tuseki

**Scenarios to cover:**
- [ ] Sacrifice when Wizard family card on field
- [ ] Cannot activate without Wizard on field

**Edge cases:**
- [ ] Card family changed to Wizard by field

**Known issues:** None

---

#### SacrificeDeveloper3Atk3Def
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Jean-Claude

**Scenarios to cover:**
- [ ] Sacrifice Developer with 3+ ATK/DEF
- [ ] Must be Developer family AND 3/3+ stats

**Edge cases:**
- [ ] Developer family from field card change

**Known issues:** None

---

#### SacrificeHardCorner3Atk3Def
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Jean-Guy

**Scenarios to cover:**
- [ ] Sacrifice HardCorner with 3+ ATK/DEF
- [ ] Must be HardCorner family AND 3/3+ stats

**Edge cases:**
- [ ] HardCorner family from field card change

**Known issues:** None

---

#### Sacrifice2Japan
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Koaloutre-Ornithambas Lapinzord nain de Californie

**Scenarios to cover:**
- [ ] Sacrifice 2 Japan family cards
- [ ] Cannot activate with only 1 Japan card

**Edge cases:**
- [ ] Sacrifice self if Japan family

**Known issues:** None

---

#### Sacrifice2Incarnation
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Lolhitler

**Scenarios to cover:**
- [ ] Sacrifice 2 Incarnation family cards
- [ ] Cannot activate with only 1 Incarnation card

**Edge cases:**
- [ ] Mixed families (one card has multiple families)

**Known issues:** None

---

#### SacrificeGranolax
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | InvocationAbilityE2ETests.cs | [ ] Pending | |

**Cards using this ability:** Mecha-Granolax

**Scenarios to cover:**
- [ ] Sacrifice Granolax to summon Mecha-Granolax
- [ ] Cannot activate without Granolax on field

**Edge cases:**
- [ ] Granolax with equipment

**Known issues:** None

---

#### SacrificeSebDuGrenier
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Patron D'Infogrames, Professeur Humblebundledore, Vieux Sage

**Scenarios to cover:**
- [ ] Sacrifice Seb Du Grenier from field
- [ ] Multiple cards with same sacrifice requirement

**Edge cases:**
- [ ] Seb with Merde plastique bleu equipped

**Known issues:** None

---

#### SacrificeClicheRaciste
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Sailor Justice

**Scenarios to cover:**
- [ ] Sacrifice Cliche Raciste from field

**Edge cases:**
- [ ] Cliche Raciste with Merde rose equipped

**Known issues:** None

---

#### SacrificeToInvoke
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | InvocationAbilityE2ETests.cs | [ ] Pending | |

**Cards using this ability:** Sheik Point

**Scenarios to cover:**
- [ ] Sacrifice self to invoke another card
- [ ] Target card comes from deck

**Edge cases:**
- [ ] No valid invoke target in deck

**Known issues:** None

---

#### SacrificeSebDuGrenierOnHardCornerForAtkDef
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Daffy

**Scenarios to cover:**
- [ ] Sacrifice Seb when HardCorner field present
- [ ] Gain ATK/DEF from sacrifice

**Edge cases:**
- [ ] Field card removed after sacrifice starts

**Known issues:** None

---

#### SacrificeJDGOnStudioDevForAtkDef
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | SacrificeAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Mohammad

**Scenarios to cover:**
- [ ] Sacrifice JDG when Studio de developpement field present
- [ ] Gain ATK/DEF from sacrifice

**Edge cases:**
- [ ] Field card changed during sacrifice

**Known issues:** None

---

### Deck Search Abilities (12)

#### AddSpatialFromDeck
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Alpha V De Gelganech

**Scenarios to cover:**
- [ ] Search deck for Spatial family card
- [ ] Card added to hand
- [ ] Deck is shuffled after search

**Edge cases:**
- [ ] No Spatial in deck
- [ ] Multiple Spatial cards in deck (player choice)

**Known issues:** None

---

#### GetNounoursFromDeck
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Benzaie jeune

**Scenarios to cover:**
- [ ] Search deck for Nounours card
- [ ] Nounours added to hand

**Edge cases:**
- [ ] Nounours not in deck

**Known issues:** None

---

#### GetPetitePortionDeRizFromDeck
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Carole du service marketing

**Scenarios to cover:**
- [ ] Search deck for Petite portion de riz effect card

**Edge cases:**
- [ ] Card not in deck

**Known issues:** None

---

#### GetLyceeMagiqueGeorgesPompidouFromDeck
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Clodo du coin

**Scenarios to cover:**
- [ ] Search deck for Lycee magique field card

**Edge cases:**
- [ ] Field card not in deck

**Known issues:** None

---

#### ChangeFieldWithFieldFromDeck
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Dictateur Sympa

**Scenarios to cover:**
- [ ] Search deck for any field card
- [ ] Replace current field card

**Edge cases:**
- [ ] No field in deck
- [ ] No current field (just place new one)

**Known issues:** None

---

#### GetZozanKebabFromDeck
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Frangipanus

**Scenarios to cover:**
- [ ] Search deck for Zozan Kebab field card

**Edge cases:**
- [ ] Card not in deck

**Known issues:** None

---

#### GetConvocationAuLyceeFromDeck
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Gerard Choixpeau

**Scenarios to cover:**
- [ ] Search deck for Convocation au lycee effect card

**Edge cases:**
- [ ] Card not in deck

**Known issues:** None

---

#### GetBenzaieJeuneFromDeck
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Maman

**Scenarios to cover:**
- [ ] Search deck for Benzaie jeune card

**Edge cases:**
- [ ] Card not in deck

**Known issues:** None

---

#### GetEquipmentCardWithoutAttack
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Manuel Ferrara

**Scenarios to cover:**
- [ ] Search deck for equipment without ATK bonus
- [ ] Filter equipment cards correctly

**Edge cases:**
- [ ] No matching equipment in deck

**Known issues:** None

---

#### GetPatronInfogramesFromDeckYellowTrash
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Spaghetti

**Scenarios to cover:**
- [ ] Search deck OR graveyard for Patron Infogrames
- [ ] Can retrieve from graveyard

**Edge cases:**
- [ ] Card in graveyard only
- [ ] Card in both deck and graveyard

**Known issues:** None

---

#### GetCanardSignal
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** None documented

**Scenarios to cover:**
- [ ] Search for Canard Signal contre card

**Edge cases:**
- [ ] Card not in deck

**Known issues:** None

---

#### GetForetElfesSylvains
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DeckSearchAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** None documented

**Scenarios to cover:**
- [ ] Search for Foret des elfes sylvains field card

**Edge cases:**
- [ ] Card not in deck

**Known issues:** None

---

### Protection Abilities (5)

#### CantBeAttackIfComics
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | ProtectionAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Babs

**Scenarios to cover:**
- [ ] Cannot be attacked if Comics card on field
- [ ] Can be attacked if no Comics on field

**Edge cases:**
- [ ] Self is Comics (protects self)
- [ ] Comics removed during attack

**Known issues:** None

---

#### ProtectedBehindStarlightUnicorn
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | ProtectionAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Granolax

**Scenarios to cover:**
- [ ] Cannot be attacked if Starlight Unicorn on field
- [ ] Can be attacked if no Starlight Unicorn

**Edge cases:**
- [ ] Starlight Unicorn destroyed same turn

**Known issues:** None

---

#### CantBeAttackKill
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | ProtectionAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Jean-Louis La Chaussette

**Scenarios to cover:**
- [ ] Survives combat that would normally destroy
- [ ] Takes damage but survives

**Edge cases:**
- [ ] Effect destruction (not combat)
- [ ] Multiple attacks same turn

**Known issues:** None

---

#### SurviveOneTurn
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | ProtectionAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Papy Grenier

**Scenarios to cover:**
- [ ] Survives first destruction attempt
- [ ] Dies on second destruction

**Edge cases:**
- [ ] Multiple attacks same turn
- [ ] Survives effect destruction too

**Known issues:** None

---

#### ProtectBehindGreaterDef
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | ProtectionAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Cards using this ability:** Patate

**Scenarios to cover:**
- [ ] Protected by cards with higher DEF
- [ ] Can be attacked if no higher DEF card

**Edge cases:**
- [ ] Equal DEF (not protected)
- [ ] DEF boosted by equipment

**Known issues:** None

---

### Dependency Abilities (6)

#### CantLiveWithoutBenzaieOrBenzaieJeune
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DependencyAbilityScenarioTests.cs | [ ] Pending | |
| E2E | KeyCombinationE2ETests.cs | [ ] Pending | |

**Cards using this ability:** Alpha Man

**Scenarios to cover:**
- [ ] Survives with Benzaie on field
- [ ] Survives with Benzaie jeune on field
- [ ] Destroyed when both removed

**Edge cases:**
- [ ] Both Benzaie and Benzaie jeune present
- [ ] Dependency destroyed same turn as summon

**Known issues:** None

---

#### CantLiveWithoutJDG
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DependencyAbilityScenarioTests.cs | [ ] Pending | |
| E2E | KeyCombinationE2ETests.cs | [ ] Pending | |

**Cards using this ability:** La Petite Fille

**Scenarios to cover:**
- [ ] Survives with Joueur Du Grenier on field
- [ ] Destroyed when JDG removed

**Edge cases:**
- [ ] Multiple JDG on field
- [ ] JDG destroyed same turn

**Known issues:** None

---

#### CantLiveWithoutComics
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DependencyAbilityScenarioTests.cs | [ ] Pending | |
| E2E | KeyCombinationE2ETests.cs | [ ] Pending | |

**Cards using this ability:** L'homme-banane

**Scenarios to cover:**
- [ ] Survives with any Comics card on field
- [ ] Destroyed when last Comics removed

**Edge cases:**
- [ ] Self counts as Comics
- [ ] Family changed to Comics by field

**Known issues:** None

---

#### CantLiveWithoutHuman
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DependencyAbilityScenarioTests.cs | [ ] Pending | |
| E2E | KeyCombinationE2ETests.cs | [ ] Pending | |

**Cards using this ability:** Moise le plus grand de tous les hebreux

**Scenarios to cover:**
- [ ] Survives with any Human card on field
- [ ] Destroyed when last Human removed

**Edge cases:**
- [ ] Self counts as Human

**Known issues:** None

---

#### CantLiveWithoutGranolaxOrMechaGranolax
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DependencyAbilityScenarioTests.cs | [ ] Pending | |
| E2E | KeyCombinationE2ETests.cs | [ ] Pending | |

**Cards using this ability:** Starlight Unicorn

**Scenarios to cover:**
- [ ] Survives with Granolax on field
- [ ] Survives with Mecha-Granolax on field
- [ ] Destroyed when both removed

**Edge cases:**
- [ ] Granolax sacrificed for Mecha-Granolax

**Known issues:** None

---

#### CantLiveWithoutJapon
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | DependencyAbilityScenarioTests.cs | [ ] Pending | |
| E2E | KeyCombinationE2ETests.cs | [ ] Pending | |

**Cards using this ability:** Tentacules

**Scenarios to cover:**
- [ ] Survives with any Japan card on field
- [ ] Destroyed when last Japan removed

**Edge cases:**
- [ ] Family changed to Japan by field

**Known issues:** None

---

## Equipment Abilities (19)

### Stat Multiplication

#### MultiplyDefBy2ButPreventAttack
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | EquipmentAbilityE2ETests.cs | [ ] Pending | |

**Equipment:** Canarang

**Scenarios to cover:**
- [ ] DEF multiplied by 2 when equipped
- [ ] Card cannot attack while equipped
- [ ] Can attack again if equipment removed

**Edge cases:**
- [ ] Equipment switched to another card
- [ ] Stacking with other DEF modifiers

**Known issues:** None

---

#### MultiplyAtkBy3
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | EquipmentAbilityE2ETests.cs | [ ] Pending | |

**Equipment:** Le salami

**Scenarios to cover:**
- [ ] ATK multiplied by 3 when equipped
- [ ] High damage output calculation

**Edge cases:**
- [ ] Stacking with other ATK modifiers

**Known issues:** None

---

#### MultiplyAtkBy2AndDefByHalf
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | EquipmentAbilityE2ETests.cs | [ ] Pending | |

**Equipment:** Parachute dore

**Scenarios to cover:**
- [ ] ATK multiplied by 2, DEF divided by 2
- [ ] Rounding for half DEF

**Edge cases:**
- [ ] Odd DEF value
- [ ] Combined with other modifiers

**Known issues:** None

---

### Stat Addition

#### Earn1ATKAndMinus1DEF
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Canarang, Turbo Cradozaure

**Scenarios to cover:**
- [ ] +1 ATK, -1 DEF when equipped
- [ ] DEF cannot go below 0

**Edge cases:**
- [ ] Card with 1 DEF

**Known issues:** None

---

#### Earn2ATK
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Main bionique

**Scenarios to cover:**
- [ ] +2 ATK when equipped

**Edge cases:**
- [ ] Stacking with other ATK bonuses

**Known issues:** None

---

#### Earn1ATKAnd1DEF
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Merde magique en plastique rose, Merde tournoyante en plastique bleu

**Scenarios to cover:**
- [ ] +1 ATK, +1 DEF when equipped

**Edge cases:**
- [ ] Multiple copies of same equipment (only 1 per card)

**Known issues:** None

---

### Dynamic Stats

#### EarnOneQuarterATKPerHandCards
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Cicatrice maudite

**Scenarios to cover:**
- [ ] ATK increases with hand size
- [ ] Updates when hand size changes

**Edge cases:**
- [ ] Empty hand
- [ ] Maximum hand size

**Known issues:** None

---

#### EarnOneQuarterDEFPerHandCards
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Petite culotte

**Scenarios to cover:**
- [ ] DEF increases with hand size
- [ ] Updates when hand size changes

**Edge cases:**
- [ ] Empty hand

**Known issues:** None

---

### Stat Setting

#### SetATKToOne
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Hypno-boobs

**Scenarios to cover:**
- [ ] ATK set to 1 regardless of base
- [ ] Overrides other ATK modifiers

**Edge cases:**
- [ ] High base ATK card

**Known issues:** None

---

#### SetDefToZero
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** La drooogue !

**Scenarios to cover:**
- [ ] DEF set to 0 regardless of base
- [ ] Overrides other DEF modifiers

**Edge cases:**
- [ ] Combined with CantBeAttackByOtherInvocations

**Known issues:** None

---

### Combat Modification

#### DirectAttack
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | EquipmentAbilityE2ETests.cs | [ ] Pending | |

**Equipment:** Cassette VHS

**Scenarios to cover:**
- [ ] Can attack player directly even with defenders
- [ ] Bypasses protection abilities

**Edge cases:**
- [ ] Combined with high ATK

**Known issues:** None

---

#### CantBeAttackByOtherInvocations
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** La drooogue !

**Scenarios to cover:**
- [ ] Cannot be targeted by attacks
- [ ] Can still be affected by effects

**Edge cases:**
- [ ] Only invocation on field

**Known issues:** None

---

#### PreventNewOpponentToAttack
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Cosplay pourri

**Scenarios to cover:**
- [ ] New opponent cards cannot attack
- [ ] Existing cards can still attack

**Edge cases:**
- [ ] Card summoned then returned to hand

**Known issues:** None

---

### Special Equipment

#### SwitchEquipmentCard
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Tatouage golde

**Scenarios to cover:**
- [ ] Can switch equipment to another card
- [ ] Previous card loses equipment effects

**Edge cases:**
- [ ] No valid target

**Known issues:** None

---

#### ProtectOneTimeFromDestruction
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** None documented

**Scenarios to cover:**
- [ ] Survives first destruction
- [ ] Equipment destroyed instead

**Edge cases:**
- [ ] Multiple destruction effects same turn

**Known issues:** None

---

#### CancelInvocationAbility
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Un bon tuyau

**Scenarios to cover:**
- [ ] Equipped card's ability disabled
- [ ] Ability restored when equipment removed

**Edge cases:**
- [ ] Passive vs active abilities

**Known issues:** None

---

#### Remove1ATKAnd1DEF
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Deltaplane

**Scenarios to cover:**
- [ ] -1 ATK, -1 DEF when equipped
- [ ] Stats cannot go below 0

**Edge cases:**
- [ ] Card with 1/1 stats

**Known issues:** None

---

#### Loose2ATK
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | EquipmentAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Equipment:** Turbo Cradozaure

**Scenarios to cover:**
- [ ] -2 ATK when equipped
- [ ] ATK cannot go below 0

**Edge cases:**
- [ ] Low ATK card

**Known issues:** None

---

## Field Abilities (12)

### Family Stat Boosts

#### Earn1DEFForSpatialFamily
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Studio de developpement

**Scenarios to cover:**
- [ ] +1 DEF for all Spatial family cards
- [ ] Effect applies to both players

**Edge cases:**
- [ ] Card gains Spatial family during game

**Known issues:** None

---

#### Earn1ATKForJapanFamily
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Le Hard Corner

**Scenarios to cover:**
- [ ] +1 ATK for all Japan family cards

**Edge cases:**
- [ ] Card gains Japan family during game

**Known issues:** None

---

#### Earn1HalfATKAndMinusHalfDEFForHCFamily
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Le grenier

**Scenarios to cover:**
- [ ] +1.5 ATK, -0.5 DEF for HardCorner cards
- [ ] Half-star calculation

**Edge cases:**
- [ ] Rounding behavior

**Known issues:** None

---

#### Earn2DEFAndMinusOneATKForIncarnationFamily
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Lycee magique Georges Pompidou

**Scenarios to cover:**
- [ ] +2 DEF, -1 ATK for Incarnation cards

**Edge cases:**
- [ ] Low ATK card

**Known issues:** None

---

#### EarnHalfATKAndDefForRpgFamily
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Fistiland

**Scenarios to cover:**
- [ ] +0.5 ATK, +0.5 DEF for RPG cards
- [ ] Half-star calculation

**Edge cases:**
- [ ] Rounding behavior

**Known issues:** None

---

#### Earn1HalfDEFAndMinusHalfATKForDevFamily
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Studio de developpement

**Scenarios to cover:**
- [ ] +1.5 DEF, -0.5 ATK for Developer cards

**Edge cases:**
- [ ] Half-star calculation

**Known issues:** None

---

### Family Changes

#### ChangePatronInfogramFamilyToDev
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Studio de developpement

**Scenarios to cover:**
- [ ] Patron Infogrames becomes Developer family
- [ ] Gets Developer field bonuses

**Edge cases:**
- [ ] Field removed mid-game

**Known issues:** None

---

#### ChangeJMBruitagesFamilyToDev
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Magasin de jeux video du coin

**Scenarios to cover:**
- [ ] Jean-Michel Bruitages becomes Developer family

**Edge cases:**
- [ ] Original family abilities

**Known issues:** None

---

### Draw Effects

#### DrawOneMoreCard
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Foret des elfes sylvains

**Scenarios to cover:**
- [ ] Draw 2 cards per turn instead of 1
- [ ] Effect on turn start

**Edge cases:**
- [ ] Combined with other draw effects

**Known issues:** None

---

#### SkipDrawToGetFistilandInvocation
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Canardcity

**Scenarios to cover:**
- [ ] Can choose to skip draw for Fistiland card
- [ ] Normal draw still available

**Edge cases:**
- [ ] No Fistiland in deck

**Known issues:** None

---

### HP Effects

#### EarnHalfHPPerWizardInvocationEachTurn
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | FieldAbilityE2ETests.cs | [ ] Pending | |

**Field:** Le Japon

**Scenarios to cover:**
- [ ] +0.5 HP per Wizard each turn
- [ ] Triggers on turn start

**Edge cases:**
- [ ] Multiple Wizards
- [ ] HP cap

**Known issues:** None

---

#### Earn2ATKAndMinus1DEFForComicsFamily
| Test Type | File | Status | Notes |
|-----------|------|--------|-------|
| Scenario | FieldAbilityScenarioTests.cs | [ ] Pending | |
| E2E | - | N/A | |

**Field:** None documented

**Scenarios to cover:**
- [ ] +2 ATK, -1 DEF for Comics cards

**Edge cases:**
- [ ] Low DEF card

**Known issues:** None

---

## Effect Abilities (25)

_Documentation for 25 effect abilities with scenarios, edge cases, and status tracking._

---

## Conditions (23)

### Card Presence Conditions

- [ ] BenzaieJeuneOrBenzaieOnField
- [ ] ArchibalVonGrenierOnField
- [ ] ZozanKebabOnField
- [ ] JoueurDuGrenierOnFieldCondition
- [ ] ForetDesElfesSylvainsOnField
- [ ] WizardOnField
- [ ] LyceeMagiqueGeorgesPompidouOnField
- [ ] ComicsOnField
- [ ] HumanOnField
- [ ] SebDuGrenierOnField
- [ ] MechaGronolaxOrGranolaxOnField
- [ ] JapanOnField

### Equipment Conditions

- [ ] BenzaieJeuneCassetteVhsEquiped
- [ ] JoueurDuGrenierCanarangEquiped
- [ ] SebDuGrenierMerdePlastiqueBleuEquiped
- [ ] ClicheRacisteMerdeRoseEquiped

### Stat Conditions

- [ ] ThreeAtk3Def
- [ ] Developer3Atk3Def2Cards
- [ ] HardCorner3Atk3Def2Cards
- [ ] Japan2Cards
- [ ] Incarnation2Cards

### Graveyard Conditions

- [ ] TenDeathYellowTrash
- [ ] GranolaxAlreadyDead

---

## Key Combinations (16)

### Equipment + Invocation (4)

- [ ] Parachute dore + Benzaie
- [ ] Canarang + Henry Potdebeurre
- [ ] Cassette VHS + Georges Tuseki
- [ ] Le salami + Benzaie

### Field + Family (10)

- [ ] Foret des elfes sylvains + Police invocations
- [ ] Lycee magique Georges Pompidou + Spatial invocations
- [ ] Magasin de jeux video du coin + Human invocations
- [ ] Studio de developpement + Comics invocations
- [ ] Canardcity + None invocations
- [ ] Fistiland + Developer invocations
- [ ] Le Hard Corner + Fistiland invocations
- [ ] Le Japon + Incarnation invocations
- [ ] Le grenier + HardCorner invocations
- [ ] Zozan Kebab + Rpg invocations

### Dependency Chains (6)

- [ ] Moise without Human
- [ ] Alpha Man without BenzaieOrBenzaieJeune
- [ ] L'homme-banane without Comics
- [ ] La Petite Fille without JDG
- [ ] Starlight Unicorn without GranolaxOrMechaGranolax
- [ ] Tentacules without Japon

---

## Change Log

| Date | Phase | Changes |
|------|-------|---------|
| 2026-01-05 | Initial | Created status document with all 156 abilities |
| 2026-01-05 | Phase 1 | Created foundation: AbilityScenarioFixtures, AbilityTestAssertions, AbilityScenarioTestBase |
| 2026-01-05 | Phase 2a | Implemented Draw ability scenario tests (Draw1Card, Draw2Cards, Draw3Cards) - 13 tests |
