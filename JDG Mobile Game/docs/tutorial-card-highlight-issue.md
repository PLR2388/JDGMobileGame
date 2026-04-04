# Tutorial Card Highlight Issue

## Description
During the tutorial, the "Musique de Megadrive" card should be highlighted (flashing green) to guide the player to select it, but the highlight effect is not appearing.

## Expected Behavior
- Tutorial prompts player to select the "Musique de Megadrive" card
- The card should have a green flashing highlight effect to indicate it's the correct card to select
- This visual cue helps guide new players through the tutorial flow

## Actual Behavior
- The "Musique de Megadrive" card is displayed in the hand/card selection area
- No green highlight/flash effect is visible on the card
- Player has no visual guidance on which card to select
- See screenshot: Arrow points to where highlight should appear on "Musique de Megadrive"

## Location
- Scene: Tutorial (`TutoPlayerGame.unity`)
- Trigger: Card selection phase in tutorial sequence
- Card: "Musique de Megadrive" (Effect card)

## Files to Investigate
- `Assets/_Scripts/OnePlayer/TutoPlayerGameLoop.cs` - Tutorial game loop
- `Assets/_Scripts/OnePlayer/DialogueTutoHandler.cs` - Tutorial dialogue/hint handler
- `Assets/Resources/Scenarii/tutorial.json` - Tutorial scenario definitions
- Card highlight/selection system components
- `Assets/_Scripts/Cards/` - Card display and highlight logic

## Status
**FIXED** - Phase 159

## Root Cause

The `TutoHandCardDisplay` class cached the dialogue index in a local `currentDialogIndex` field, which was updated via `DialogueIndexChangedEvent` subscription. However:

1. When the hand popup is **closed**, `OnDisable()` unsubscribes from the event
2. Dialogue advances past index 35 while the hand is closed
3. `TutoHandCardDisplay` doesn't receive this update (unsubscribed)
4. When hand **reopens**, `currentDialogIndex` is still at its stale value (≤35)
5. `ShouldHighlightCard()` returns false for "Musique de Mega Drive" because `currentDialogIndex > 35` is false

### Event Flow (Before Fix)
```
1. Hand opens at dialogue index ≤35
2. currentDialogIndex = 35 (or less)
3. Hand closes → event unsubscribed
4. Dialogue advances to 36, 37, 38...
5. TutoHandCardDisplay misses these events!
6. Hand reopens → currentDialogIndex still ≤35
7. ShouldHighlightCard("Musique de Mega Drive") → false (stale index)
```

## Fix Applied (Phase 159)

Modified `TutoHandCardDisplay.cs` to inject `ITutorialStateService` and use its `CurrentDialogIndex` property instead of the cached field:

### Changes
1. Added `ITutorialStateService` dependency injection
2. Updated `ShouldHighlightCard()` to query service directly

### Code Change
```csharp
// Before (buggy - used stale cached value)
private bool ShouldHighlightCard(InGameCard handCard)
{
    return ... && currentDialogIndex > StartHighlightMusiqueDeMegadriveIndex;
}

// After (fixed - queries service for current value)
private bool ShouldHighlightCard(InGameCard handCard)
{
    var dialogIndex = _tutorialStateService?.CurrentDialogIndex ?? currentDialogIndex;
    return ... && dialogIndex > StartHighlightMusiqueDeMegadriveIndex;
}
```

### File Modified
- `Assets/_Scripts/Menu/TutoHandCardDisplay.cs`

## Possible Causes (Historical)

### 1. Highlight Component Not Triggered
- The tutorial script may not be calling the highlight method on the correct card
- Card reference may be incorrect or null

### 2. Highlight Effect Disabled/Missing
- The highlight shader/material may be missing or disabled
- Animation component may not be properly configured

### 3. Card Identification Issue
- Tutorial may be looking for the card by wrong name/ID
- Card may have been renamed or its identifier changed

### 4. Z-Order/Rendering Issue
- Highlight effect may be rendering behind the card
- Sorting layer issues preventing visibility

## Cards Visible in Screenshot
1. L'Elfette (Invocation) - leftmost
2. **Musique de Megadrive** (Effect) - should be highlighted
3. Studio de Scenaristes Canadien (Invocation)
4. Le Japon (Field/Terrain)

## Reproduction Steps
1. Start a new tutorial game
2. Progress to the card selection phase where "Musique de Megadrive" should be played
3. Observe that the card has no green highlight effect

## Notes
- Other cards in hand appear normal (no highlight)
- The "Retour" (Back) button is visible, suggesting this is an interactive selection phase
- The opponent's side shows face-down cards in the top area
