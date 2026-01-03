# Tutorial System - Complete Technical Documentation

## Overview

The tutorial is a **data-driven, event-based guided gameplay experience** built on top of the normal game loop. It uses a combination of:
- **DialogueObject** (ScriptableObject) for dialogue text and triggers
- **tutorial.json** (JSON scenario file) for game actions
- **EventBus** for decoupled communication between components
- **Tuto* prefixed classes** that extend base gameplay classes with restrictions

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           TUTORIAL SYSTEM                               │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌─────────────────┐         ┌──────────────────────┐                  │
│  │ Tutorial.asset  │         │   tutorial.json      │                  │
│  │ (DialogueObject)│         │   (ActionScenarios)  │                  │
│  │ - 39 dialogues  │         │   - highlights       │                  │
│  │ - audio clips   │         │   - putCard          │                  │
│  │ - triggers      │         │   - attacks          │                  │
│  └────────┬────────┘         └──────────┬───────────┘                  │
│           │                             │                               │
│           ▼                             ▼                               │
│  ┌─────────────────┐         ┌──────────────────────┐                  │
│  │   DialogueUI    │◄───────►│  TutoPlayerGameLoop  │                  │
│  │                 │ Events  │                      │                  │
│  └────────┬────────┘         └──────────┬───────────┘                  │
│           │                             │                               │
│           │    ┌────────────────────────┼────────────────────┐         │
│           │    │         EventBus       │                    │         │
│           ▼    ▼                        ▼                    ▼         │
│  ┌─────────────────┐  ┌─────────────────────┐  ┌────────────────────┐  │
│  │ TutorialState   │  │  Highlight System   │  │ TutoInGameMenuScript│ │
│  │ Service         │  │  - HighLightPlane   │  │ TutoHandCardDisplay │ │
│  │                 │  │  - HighLightButton  │  │ TutoInvocationFuncs │ │
│  └─────────────────┘  │  - HighLightCard    │  └────────────────────┘  │
│                       │  - HighLightPhysical│                          │
│                       └─────────────────────┘                          │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 1. Dialogue System

### 1.1 Data Storage

**File**: `Assets/DialogueData/Tutorial.asset` (DialogueObject ScriptableObject)

Contains:
- `dialogue[]` - 39 French dialogue strings
- `audioClips[]` - Voice-over audio files
- `soundDialogueIndex[]` - Which dialogues have audio
- `nextDialogueTriggers[]` - What triggers the next line (NextDialogueTrigger enum)
- `responses[]` - Optional player response choices

### 1.2 NextDialogueTrigger Enum

Defined in `Assets/_Scripts/OnePlayer/DialogueBox/NextDialogueTrigger.cs`:

| Trigger | Description |
|---------|-------------|
| `Tap` | Player must tap/click to continue |
| `Automatic` | Continues immediately after typewriter effect |
| `PutCard` | Waits for player to place a card |
| `PutEffectCard` | Waits for effect card placement |
| `NextPhase` | Waits for next phase button click |
| `Attack` | Waits for attack action to complete |
| `EndVideo` | Waits for video to finish |
| `EndGame` | Loads main menu |
| `Undefined` | No trigger (should not happen) |

### 1.3 DialogueUI Flow

**File**: `Assets/_Scripts/OnePlayer/DialogueBox/DialogueUI.cs`

```
Start()
  └─► ShowDialogue(testDialogue)  // testDialogue = Tutorial.asset
        └─► StartCoroutine(StepThroughDialogue())

StepThroughDialogue() [Coroutine]
  FOR each dialogue line (index 0..38):
    │
    ├─► Publish DialogueIndexChangedEvent(index)
    │     └─► TutoPlayerGameLoop.OnDialogueIndexChanged()
    │     └─► TutorialStateService.SetDialogIndex(index)
    │
    ├─► Display text with typewriter effect
    │
    ├─► Play audio if exists
    │
    ├─► yield WaitUntil(IsNextDialogueReady)
    │     └─► Blocks until trigger condition met
    │
    └─► Show responses if any
```

### 1.4 Trigger Completion Flow

When player completes an action (places card, attacks, etc.):

```
Player Action (e.g., places card)
  └─► TutoInGameMenuScript.ClickPutCard()
        └─► Publish DialogueTriggerCompletedEvent(PutCard)
              └─► DialogueUI.OnTriggerCompleted()
                    └─► Sets currentTrigger = null
                          └─► IsNextDialogueReady() returns true
                                └─► Dialogue advances to next line
```

---

## 2. Scenario System

### 2.1 JSON Structure

**File**: `Assets/Resources/Scenarii/tutorial.json`

```json
[
  { "index": 1, "highlight": "space" },
  { "index": 2, "highlight": "deck" },
  { "index": 7, "highlight": "invocation_cards" },
  { "index": 11, "putCard": "Fisti;Jean-Michel Bruitages" },
  { "index": 19, "highlight": "tentacules" },
  { "index": 20, "putCard": "Merde magique>Fisti" },
  { "index": 32, "attack": "Fisti>Cliché Raciste" },
  { "index": 36, "attack": "Le Pyro-Barbare>" }
]
```

### 2.2 ActionScenario Fields

| Field | Format | Description |
|-------|--------|-------------|
| `index` | int | Dialogue line that triggers this action |
| `highlight` | string | Element to highlight (space, deck, tentacules, etc.) |
| `putCard` | string | Cards to place: "Card1;Card2" or "Equipment>Target" |
| `attack` | string | Attack: "Attacker>Defender" or "Attacker>" (direct attack) |
| `action` | string | "next_phase" to advance game phase |
| `image` | string | Image asset to display |
| `video` | string | Video asset to display |

### 2.3 TutoPlayerGameLoop Processing

**File**: `Assets/_Scripts/OnePlayer/TutoPlayerGameLoop.cs`

```csharp
OnDialogueIndexChanged(DialogueIndexChangedEvent evt)
  └─► TriggerScenarioAction(evt.DialogueIndex)

TriggerScenarioAction(int index)
  │
  ├─► Find ActionScenario where scenario.Index == index
  │
  ├─► HandleHighlight(scenario.Highlight)
  │     └─► Publish HighlightRequestedEvent
  │
  ├─► DisplayImage(scenario.Image) or DisplayVideo(scenario.Video)
  │
  ├─► PlaceCard(scenario.PutCard)
  │     ├─► "Card1;Card2" → Place multiple cards
  │     └─► "Equipment>Target" → Equip card to target
  │
  ├─► HandleAttack(scenario.Attack)
  │     └─► Execute scripted attack via ICombatService
  │
  └─► NextRound() if action == "next_phase"
        └─► Publish DialogueTriggerCompletedEvent(NextPhase)
```

---

## 3. Highlighting System

### 3.1 Highlight Elements

**Enum** in `Assets/_Scripts/OnePlayer/HighLightPlane.cs`:

```csharp
public enum HighlightElement
{
    Invocations,      // Invocation card zone
    Space,            // Main play area
    Deck,             // Player's deck
    YellowTrash,      // Graveyard
    Effect,           // Effect card zone
    Field,            // Field card zone
    InHandButton,     // Hand toggle button
    NextPhaseButton,  // Phase advance button
    Tentacules,       // Specific card
    LifePoints,       // HP display
    AttackButton,     // Attack action button
    OpponentSelector  // Card selection popup
}
```

### 3.2 JSON to Element Mapping

In `TutoPlayerGameLoop.cs` (lines 31-63):

| JSON highlight | HighlightElement |
|----------------|------------------|
| "space" | Space |
| "deck" | Deck |
| "yellow_trash" | YellowTrash |
| "field" | Field |
| "invocation_cards" | Invocations |
| "effect_cards" | Effect |
| "hand_cards" | InHandButton |
| "next_phase" | NextPhaseButton |
| "tentacules" | Tentacules |
| "life_point" | LifePoints |

### 3.3 Highlight Components

All highlight components:
1. Subscribe to `HighlightRequestedEvent` via EventBus
2. Check if event's element matches their assigned element
3. Set `isActivated` flag
4. Run pulsing coroutine (0.5s green, 0.5s default)

#### HighLightPlane.cs
- Highlights board areas with semi-transparent green overlay
- Pulses between `Color.green` and `Color.clear`

#### HighLightPhysicalCard.cs
- Highlights specific 3D cards (e.g., Tentacules)
- Pulses between green and white via `MeshRenderer.material.color`

#### HighLightButton.cs
- Highlights UI buttons
- Also enables button interaction during highlight

#### HighLightCard.cs
- Highlights card images in UI
- Used for hand cards display

#### OnHover.cs (Card Selector)
- Special case: checks `DisplayCards.CardToHighlight` static property
- Used to highlight specific cards in opponent selector popup

### 3.4 Pulse Animation

All components use identical timing:
```csharp
PulseCoroutine()
{
    while (isActivated)
    {
        yield return new WaitForSeconds(0.5f);
        SetColor(pulseColor);  // Green
        yield return new WaitForSeconds(0.5f);
        SetColor(defaultColor);  // White/Clear
    }
}
```

---

## 4. Tutorial-Specific Classes

### 4.1 TutoPlayerGameLoop

**Extends**: `GameLoop`
**File**: `Assets/_Scripts/OnePlayer/TutoPlayerGameLoop.cs`

**Key Additions**:
- Loads ActionScenarios from tutorial.json via ScenarioDecoder
- Subscribes to `DialogueIndexChangedEvent` to trigger scenario actions
- Handles multi-step attack flow (Phase 143):
  1. Player clicks Tentacules → deactivate highlight, wait frame, highlight AttackButton
  2. Player clicks Attack → deactivate highlight, set CardToHighlight, show selector
  3. Player selects target → execute attack, highlight NextPhaseButton

### 4.2 TutoHandCardDisplay

**Extends**: `HandCardDisplay`
**File**: `Assets/_Scripts/Menu/TutoHandCardDisplay.cs`

**Key Restrictions**:
- Only highlights permitted cards based on dialogue index
- Before index 35: Only ClichéRaciste can be highlighted
- After index 35: MusiqueDeMegaDrive becomes highlightable
- Dynamically adds `HighLightCard` component to permitted cards

### 4.3 TutoInGameMenuScript

**Extends**: `InGameMenuScript`
**File**: `Assets/_Scripts/Menu/TutoInGameMenuScript.cs`

**Key Restrictions**:
- `ClickOnCard()` checks if card is authorized:
  - Before index 36: Only ClichéRaciste playable
  - After index 36: Only MusiqueDeMegaDrive playable
- `ClickPutCard()` publishes `DialogueTriggerCompletedEvent(PutCard)`
- `HideHand()` publishes `DialogueTriggerCompletedEvent(PutEffectCard)` at index 38

### 4.4 TutoInvocationFunctions

**Extends**: `InvocationFunctions`
**File**: `Assets/_Scripts/Units/Invocation/TutoInvocationFunctions.cs`

**Special Behavior**:
- When ClichéRaciste is played, shows MessageBox asking to invoke Tentacules
- On acceptance: moves Tentacules from deck to field, highlights InHandButton

---

## 5. State Management

### 5.1 ITutorialStateService

**Interface**: `Assets/_Scripts/JDG.Application/Services/ITutorialStateService.cs`
**Implementation**: `Assets/_Scripts/Services/TutorialStateService.cs`

Tracks:
- `CurrentDialogIndex` - Current dialogue line number
- Published via `DialogIndexChanged` event

Used by:
- `TutoInGameMenuScript` to determine which cards are playable
- `TutoHandCardDisplay` to determine which cards to highlight

---

## 6. Complete Tutorial Flow Example

### Sequence: Player Places ClichéRaciste Card

```
Step 1: Dialogue reaches index 19
├─► DialogueUI publishes DialogueIndexChangedEvent(19)
├─► TutoPlayerGameLoop.OnDialogueIndexChanged(19)
│     └─► TriggerScenarioAction(19)
│           └─► HandleHighlight("tentacules")
│                 └─► Publish HighlightRequestedEvent(Tentacules, true)
│                       └─► HighLightPhysicalCard starts pulsing green
└─► DialogueUI waits for trigger (PutCard)

Step 2: Player clicks ClichéRaciste card in hand
├─► TutoInGameMenuScript.ClickOnCard(card)
│     └─► Checks: currentIndex (19) < 36, so ClichéRaciste is authorized
├─► Shows "Put" button

Step 3: Player clicks "Put" button
├─► TutoInGameMenuScript.ClickPutCard()
│     └─► Publish DialogueTriggerCompletedEvent(PutCard)
│           └─► DialogueUI.OnTriggerCompleted()
│                 └─► currentTrigger = null
│                       └─► Dialogue advances

Step 4: TutoInvocationFunctions handles card placement
├─► Shows MessageBox: "Invoke Tentacules from deck?"
└─► On Accept:
      ├─► Moves Tentacules from deck to field
      └─► Publish HighlightRequestedEvent(InHandButton, true)
```

---

## 7. Key File Locations

| Component | File Path |
|-----------|-----------|
| DialogueUI | `Assets/_Scripts/OnePlayer/DialogueBox/DialogueUI.cs` |
| DialogueObject | `Assets/_Scripts/OnePlayer/DialogueBox/DialogueObject.cs` |
| NextDialogueTrigger | `Assets/_Scripts/OnePlayer/DialogueBox/NextDialogueTrigger.cs` |
| TutoPlayerGameLoop | `Assets/_Scripts/OnePlayer/TutoPlayerGameLoop.cs` |
| TutoHandCardDisplay | `Assets/_Scripts/Menu/TutoHandCardDisplay.cs` |
| TutoInGameMenuScript | `Assets/_Scripts/Menu/TutoInGameMenuScript.cs` |
| TutoInvocationFunctions | `Assets/_Scripts/Units/Invocation/TutoInvocationFunctions.cs` |
| HighLightPlane | `Assets/_Scripts/OnePlayer/HighLightPlane.cs` |
| HighLightPhysicalCard | `Assets/_Scripts/OnePlayer/HighLightPhysicalCard.cs` |
| HighLightButton | `Assets/_Scripts/OnePlayer/HighLightButton.cs` |
| HighLightCard | `Assets/_Scripts/OnePlayer/HighLightCard.cs` |
| TutorialStateService | `Assets/_Scripts/Services/TutorialStateService.cs` |
| ITutorialStateService | `Assets/_Scripts/JDG.Application/Services/ITutorialStateService.cs` |
| ScenarioDecoder | `Assets/_Scripts/OnePlayer/ScenarioDecoder.cs` |
| Tutorial.asset | `Assets/DialogueData/Tutorial.asset` |
| tutorial.json | `Assets/Resources/Scenarii/tutorial.json` |
| TutoPlayerGame.unity | `Assets/Scenes/TutoPlayerGame.unity` |

---

## 8. Events Summary

| Event | Publisher | Subscribers |
|-------|-----------|-------------|
| `DialogueIndexChangedEvent` | DialogueUI | TutoPlayerGameLoop, TutorialStateService, TutoHandCardDisplay |
| `DialogueTriggerCompletedEvent` | TutoInGameMenuScript, TutoPlayerGameLoop | DialogueUI |
| `HighlightRequestedEvent` | TutoPlayerGameLoop, TutoInGameMenuScript | All HighLight* components |
| `TouchStartedEvent` | Input system | TutoPlayerGameLoop (for multi-step attack) |
| `InvocationCardPlayRequestedEvent` | OnHover/Card handlers | TutoInvocationFunctions |

---

## 9. Design Patterns Used

1. **Data-Driven Design**: Tutorial sequence defined in JSON, easily modifiable
2. **EventBus (Pub/Sub)**: Decouples dialogue from game actions and highlighting
3. **Template Method**: Tuto* classes extend base classes, override specific methods
4. **State Machine**: DialogueUI + TutorialStateService track progression
5. **Dependency Injection**: VContainer injects services into all components
6. **Adapter Pattern**: TutoPlayerGameLoop adapts scenarios to game actions

---

## 10. Adding New Tutorial Steps

### To add a new dialogue line:
1. Edit `Tutorial.asset` in Unity Inspector
2. Add text to `dialogue[]` array
3. Set trigger type in `nextDialogueTriggers[]`
4. Optionally add audio clip

### To add a new scenario action:
1. Edit `tutorial.json`
2. Add object with `index` matching dialogue line number
3. Add desired actions (highlight, putCard, attack, etc.)

### To add a new highlight element:
1. Add enum value to `HighlightElement` in `HighLightPlane.cs`
2. Add mapping in `TutoPlayerGameLoop.highlightMapping` dictionary
3. Create highlight component (HighLightPlane/Button/Card) on target GameObject
4. Set the `element` field in Inspector

---

## 11. Tutorial Deck Setup

**File**: `Assets/_Scripts/Services/DeckManagementService.cs` method `BuildTutorialDecks()`

### Player 1 (AI) Deck:
- Fisti, Jean-Michel Bruitages, Le Pyro-Barbare
- Plus 27 random cards

### Player 2 (Human) Deck:
- ClichéRaciste, MusiqueDeMegaDrive, LElfette (in hand)
- **Tentacules at position 0** (stays in deck to be invoked by ClichéRaciste ability)
- Plus 26 random cards

---

## 12. Debugging Tips

### Check dialogue progression:
- Monitor `ITutorialStateService.CurrentDialogIndex`
- Subscribe to `DialogueIndexChangedEvent` for logging

### Check highlight activation:
- Watch `HighlightRequestedEvent` publications
- Check `isActivated` field on highlight components

### Check trigger completion:
- Monitor `DialogueTriggerCompletedEvent` publications
- Verify `NextDialogueTrigger` enum matches expected trigger type
