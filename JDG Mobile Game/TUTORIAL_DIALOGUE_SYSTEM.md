# Tutorial Dialogue System - Complete Implementation Analysis

## System Overview

The tutorial dialogue system consists of **two parallel data sources**:
1. **Tutorial.asset** (DialogueObject) - Contains dialogue text + trigger types (when to advance)
2. **tutorial.json** (Scenario) - Contains actions to perform at each dialogue index (what to do)

---

## TRIGGER TYPES (NextDialogueTrigger enum)

| Value | Name | Behavior | How It Advances |
|-------|------|----------|-----------------|
| 0 | **Tap** | Waits for user tap | `InputManager.IsTap` returns true |
| 1 | **Automatic** | Advances immediately | Always returns true |
| 2 | **PutCard** | Hides dialogue, waits for card | Receives `PutCard` event |
| 3 | **NextPhase** | Shows dialogue, waits for button | Receives `NextPhase` event |
| 4 | **PutEffectCard** | Hides dialogue, waits for effect card | Receives `PutEffectCard` event |
| 6 | **Attack** | Hides dialogue, waits for attack | Receives `NextPhase` event (special handling) |
| 7 | **EndVideo** | Hides dialogue, waits for video | Receives `EndVideo` event |
| 8 | **EndGame** | Ends tutorial | Calls `LoadMainScreen()` |

---

## EVENT PUBLISHERS (Who sends DialogueTriggerCompletedEvent)

### 1. TutoInGameMenuScript.ClickPutCard() - Line 156
**Trigger:** `PutCard`
**When:** User places any card on the field
```csharp
_eventBus?.Publish(new DialogueTriggerCompletedEvent { TriggerType = (int)NextDialogueTrigger.PutCard });
```

### 2. TutoInGameMenuScript.HideHand() - Line 245
**Trigger:** `PutEffectCard`
**When:** User hides hand at dialogue index 38 with 2 invocation cards
```csharp
_eventBus?.Publish(new DialogueTriggerCompletedEvent { TriggerType = (int)NextDialogueTrigger.PutEffectCard });
```

### 3. TutoPlayerGameLoop.NextRound() - Line 355
**Trigger:** `NextPhase`
**When:** Transitioning to next round for Player 2 (AI turn ends)
```csharp
_eventBus?.Publish(new DialogueTriggerCompletedEvent { TriggerType = (int)NextDialogueTrigger.NextPhase });
```

### 4. TutoPlayerGameLoop.DisplayOpponentMessageBox() - Line 437
**Trigger:** `NextPhase`
**When:** User selects Jean-Michel Bruitages as attack target
```csharp
_eventBus?.Publish(new DialogueTriggerCompletedEvent { TriggerType = (int)NextDialogueTrigger.NextPhase });
```

### 5. VideoPlayerObserver.OnVideoEnd() - Line 65
**Trigger:** `EndVideo`
**When:** Video playback finishes
```csharp
_eventBus?.Publish(new DialogueTriggerCompletedEvent { TriggerType = (int)NextDialogueTrigger.EndVideo });
```

---

## DIALOGUE ADVANCEMENT FLOW

```
┌─────────────────────────────────────────────────────────────────┐
│                    DialogueUI.StepThroughDialogue()             │
│                                                                 │
│  for (int i = 0; i < dialogue.Length; i++)                      │
│  {                                                              │
│      // 1. Publish index change                                 │
│      _eventBus.Publish(DialogueIndexChangedEvent { Index = i }) │
│      _tutorialStateService.SetDialogIndex(i)                    │
│                                                                 │
│      // 2. Display text with typewriter                         │
│      yield return typewriterEffect.Run(dialogue[i])             │
│                                                                 │
│      // 3. Wait for trigger condition                           │
│      yield return WaitUntil(() => IsNextDialogueReady(trigger)) │
│  }                                                              │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│              IsNextDialogueReady(trigger) - Lines 186-249       │
│                                                                 │
│  switch (trigger)                                               │
│  {                                                              │
│      case Tap:                                                  │
│          return InputManager.IsTap;                             │
│                                                                 │
│      case Automatic:                                            │
│          return true;                                           │
│                                                                 │
│      case PutCard:                                              │
│          dialogueBox.SetActive(false);  // HIDE                 │
│          if (currentTrigger == PutCard) {                       │
│              dialogueBox.SetActive(true);  // SHOW              │
│              return true;  // ADVANCE                           │
│          }                                                      │
│          break;                                                 │
│                                                                 │
│      case NextPhase:                                            │
│          if (currentTrigger == NextPhase) {                     │
│              return true;  // ADVANCE                           │
│          }                                                      │
│          break;                                                 │
│                                                                 │
│      case Attack:                                               │
│          dialogueBox.SetActive(false);  // HIDE                 │
│          if (currentTrigger == NextPhase) {                     │
│              dialogueBox.SetActive(true);  // SHOW              │
│              return true;  // ADVANCE                           │
│          }                                                      │
│          break;                                                 │
│                                                                 │
│      case EndVideo:                                             │
│          dialogueBox.SetActive(false);  // HIDE                 │
│          if (currentTrigger == EndVideo) {                      │
│              dialogueBox.SetActive(true);  // SHOW              │
│              return true;  // ADVANCE                           │
│          }                                                      │
│          break;                                                 │
│                                                                 │
│      case EndGame:                                              │
│          _sceneLoaderService.LoadMainScreen();                  │
│          return false;                                          │
│  }                                                              │
│  return false;                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## SCENARIO ACTIONS (tutorial.json)

**When DialogueIndexChangedEvent is published:**
```
DialogueUI publishes DialogueIndexChangedEvent { Index = i }
        │
        ▼
TutoPlayerGameLoop.OnDialogueIndexChanged(evt)
        │
        ▼
TriggerScenarioAction(evt.DialogueIndex)
        │
        ▼
Find ActionScenario where scenario.Index == dialogueIndex
        │
        ▼
Execute actions: Highlight, PutCard, Attack, Image, Video, Action
```

### Action Types:

| Action | Example JSON | What Happens |
|--------|--------------|--------------|
| **highlight** | `"highlight": "hand_cards"` | Publishes `HighlightRequestedEvent` to pulse UI element |
| **putCard** | `"putCard": "Fisti;Jean-Michel"` | AI places cards from hand to field |
| **putCard (equip)** | `"putCard": "Equipment>Target"` | AI equips card to invocation |
| **attack** | `"attack": "Fisti>Cliché Raciste"` | AI attacks opponent's card |
| **attack (direct)** | `"attack": "Pyro-Barbare>"` | AI attacks player directly |
| **image** | `"image": "tuto1.png"` | Displays image overlay |
| **video** | `"video": "tuto1.mp4"` | Plays video |
| **action** | `"action": "next_phase"` | Calls `NextPhase()` to advance game phase |

### Highlight Mapping:

| JSON String | HighlightElement | UI Element |
|-------------|------------------|------------|
| `space` | Space | Play space area |
| `deck` | Deck | Deck pile |
| `yellow_trash` | YellowTrash | Graveyard |
| `field` | Field | Field card slot |
| `invocation_cards` | Invocations | Invocation slots |
| `effect_cards` | Effect | Effect card slots |
| `hand_cards` | InHandButton | "Main" button |
| `next_phase` | NextPhaseButton | "Phase suivante" button |
| `tentacules` | Tentacules | Tentacules card on field |
| `life_point` | LifePoints | Life points display |

---

## COMPLETE DIALOGUE FLOW (Index 0-43)

### Indices 0-10: Introduction
| Idx | Trigger | Scenario | Notes |
|-----|---------|----------|-------|
| 0 | Tap | - | "Laisse moi t'expliquer..." |
| 1 | Tap | highlight: space | |
| 2 | Tap | highlight: deck | |
| 3 | Tap | highlight: yellow_trash | |
| 4 | Tap | highlight: field | |
| 5 | Auto | - | |
| 6 | Tap | - | |
| 7 | Tap | highlight: invocation_cards | |
| 8 | Tap | - | |
| 9 | Tap | highlight: effect_cards | |
| 10 | Tap | - | |

### Indices 11-12: AI Places Cards
| Idx | Trigger | Scenario | Notes |
|-----|---------|----------|-------|
| 11 | Tap | putCard: Fisti;Jean-Michel Bruitages | AI places 2 cards |
| 12 | Auto | action: next_phase | AI ends turn |

### Indices 13-16: User Places Card (CRITICAL)
| Idx | Trigger | Scenario | Notes |
|-----|---------|----------|-------|
| **13** | **PutCard** | highlight: hand_cards | **HIDES dialogue, waits for PutCard event** |
| 14 | Auto | - | |
| 15 | Auto | - | |
| 16 | Tap | - | |

**Index 13 Flow:**
1. Dialogue shows → triggers scenario → highlights "Main" button
2. PutCard trigger → dialogue HIDES
3. User clicks "Main" → opens hand
4. User clicks Cliché Raciste → mini menu appears
5. User clicks "Poser" → `TutoInGameMenuScript.ClickPutCard()` → publishes `PutCard`
6. DialogueUI receives → `currentTrigger = PutCard`
7. `IsNextDialogueReady(PutCard)` returns true → dialogue SHOWS → advances to 14

### Indices 17-19: User Attacks
| Idx | Trigger | Scenario | Notes |
|-----|---------|----------|-------|
| **17** | **NextPhase** | highlight: next_phase | Waits for "Phase suivante" button |
| 18 | Auto | - | |
| **19** | **Attack** | highlight: tentacules | HIDES dialogue, waits for attack |

### Indices 20-28: AI Turn
| Idx | Trigger | Scenario | Notes |
|-----|---------|----------|-------|
| 20 | Tap | putCard: Le Pyro-Barbare | |
| 21 | Auto | putCard: Fistiland | |
| 22 | Auto | - | |
| 23 | Tap | - | |
| 24 | Tap | - | |
| 25 | Tap | - | |
| 26 | Tap | image: tuto1.png | |
| 27 | Tap | - | |
| 28 | Tap | putCard: equipment>Fisti, action: next_phase | |

### Indices 29-38: Video & Combat
| Idx | Trigger | Scenario | Notes |
|-----|---------|----------|-------|
| 29 | EndVideo | video: tuto1.mp4 | Waits for video |
| 30 | Auto | - | |
| 31 | Tap | - | |
| 32 | Tap | attack: Fisti>Cliché Raciste | AI attacks |
| 33 | Tap | highlight: life_point | |
| 34 | Tap | - | |
| 35 | Tap | - | |
| 36 | Auto | - | |
| 37 | Auto | - | |
| 38 | Tap | attack: Pyro-Barbare>, action: next_phase | Direct attack |

### Indices 39-43: Effect Cards & End
| Idx | Trigger | Scenario | Notes |
|-----|---------|----------|-------|
| 39 | PutEffectCard | - | Waits for effect card |
| 40 | Auto | - | |
| 41 | Tap | - | |
| 42 | Auto | - | |
| 43 | EndGame | - | Tutorial ends |

---

## KEY FILES

| File | Purpose |
|------|---------|
| `Assets/_Scripts/OnePlayer/DialogueBox/DialogueUI.cs` | Main dialogue controller, event listener |
| `Assets/_Scripts/OnePlayer/DialogueBox/NextDialogueTrigger.cs` | Trigger enum definition |
| `Assets/_Scripts/OnePlayer/TutoPlayerGameLoop.cs` | Tutorial game loop, scenario executor |
| `Assets/_Scripts/OnePlayer/Scenario.cs` | Data classes for ActionScenario |
| `Assets/_Scripts/OnePlayer/ScenarioDecoder.cs` | JSON parser for tutorial.json |
| `Assets/_Scripts/Menu/TutoInGameMenuScript.cs` | Tutorial menu, card placement events |
| `Assets/_Scripts/OnePlayer/VideoPlayerObserver.cs` | Video completion events |
| `Assets/DialogueData/Tutorial.asset` | Dialogue text + trigger types |
| `Assets/Resources/Scenarii/tutorial.json` | Scenario actions per index |

---

## FIXES APPLIED (Phase 144)

### Fix 1: TutoInGameMenuScript.cs - Inject _eventBus
**Problem:** `_eventBus` was NULL - `Construct()` only injected `ITutorialStateService`
**Solution:** Updated to inject all 6 dependencies including `IEventBus`

### Fix 2: tutorial.json - Add index 17 highlight
**Problem:** Index 17 has NextPhase trigger but no highlight for the button
**Solution:** Added `{ "index": 17, "highlight": "next_phase" }`

### Fix 3: Tutorial.asset verified
**Confirmed:** Index 13 = `02000000` (PutCard) is correct

---

## FIXES APPLIED (Phase 145)

### Fix 4: DialogueUI.cs - PutCard trigger alignment
**Problem:** `IsNextDialogueReady(PutCard)` checked for `NextPhase` event, but `TutoInGameMenuScript.ClickPutCard()` publishes `PutCard`
**Solution:** Changed line 197 from `NextDialogueTrigger.NextPhase` to `NextDialogueTrigger.PutCard`

Now all components are aligned:
- Tutorial.asset index 13 = `PutCard`
- TutoInGameMenuScript publishes `PutCard`
- DialogueUI expects `PutCard`

---

## FIXES APPLIED (Phase 146)

### Fix 5: Method Hiding Bug - virtual/override
**Problem:** `TutoInGameMenuScript.ClickPutCard()` used `new` keyword (method hiding). Unity button called base class method which doesn't publish events.
**Solution:**
- InGameMenuScript.cs: Changed `public void ClickPutCard()` to `public virtual void ClickPutCard()`
- TutoInGameMenuScript.cs: Changed `public new void ClickPutCard()` to `public override void ClickPutCard()`

Now polymorphism works correctly - clicking "Poser" calls the tutorial-specific override.
