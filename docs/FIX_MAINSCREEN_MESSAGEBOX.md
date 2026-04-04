# Fix: MessageBox Null in MainScreen Card Choice

**Date**: 2026-03-29
**Severity**: User-facing bug
**Scene**: MainScreen (deck selection / "Choix des cartes")

## Symptom

When Player 2 selects fewer than 30 cards and presses the confirm button, **no validation message box appears**. The user gets no feedback that their deck is incomplete. The console shows:

```
DialogService: MessageBox is null! Cannot show message box.
Ensure SetDialogComponents() was called from GameSceneScope.
```

## Root Cause

`DialogService` is registered as a **Singleton** in `SharedServicesScope` (the root DI scope that lives in `_preload` scene via DontDestroyOnLoad). Its `MessageBox` and `CardSelector` UI components are set via `SetDialogComponents()`.

However, `SetDialogComponents()` was **only called in `GameSceneScope`** (the Game scene DI scope). When the MainScreen scene loads for deck selection, `DialogService` still has null references for its UI components.

### Call chain that fails

```
CardChoice.CheckPlayerCards()
  -> CardChoiceUIManager.DisplayMessageBox(remainedCards)
    -> IDialogService.ShowMessageBox(canvas, options)
      -> DialogService._messageBox is null -> LogError, no dialog shown
```

### Why GameSceneScope worked

`GameSceneScope.Configure()` finds `MessageBox` and `CardSelector` components (which live on `Systems/UI` in the `_preload` scene, persisted via DontDestroyOnLoad) using `FindFirstObjectByType<MessageBox>()`, then calls `dialogService.SetDialogComponents(messageBox, cardSelector)` in its `RegisterBuildCallback`.

### Why MainScreenScope didn't work

`MainScreenScope` never called `SetDialogComponents()`. It relied on the singleton `DialogService` from `SharedServicesScope`, which was created without any UI components.

## Fix

Added the same `SetDialogComponents()` call to `MainScreenScope.RegisterBuildCallback`:

```csharp
// Phase 166: Wire DialogService with MessageBox/CardSelector from _preload scene
var messageBox = FindFirstObjectByType<MessageBox>();
var cardSelector = FindFirstObjectByType<CardSelector>();
if (messageBox != null)
{
    var dialogService = container.Resolve<IDialogService>() as DialogService;
    if (dialogService != null)
    {
        dialogService.SetDialogComponents(messageBox, cardSelector);
    }
}
```

The `MessageBox` and `CardSelector` components exist on the `Systems/UI` GameObject in the `_preload` scene (DontDestroyOnLoad), so `FindFirstObjectByType` finds them from any scene.

## Files Modified

- `JDG Mobile Game/Assets/_Scripts/DI/MainScreenScope.cs` -- Added `SetDialogComponents` call in `RegisterBuildCallback`

## Testing

1. Launch the game from `_preload` scene
2. Navigate to "Choix des cartes" (card selection)
3. Select fewer than 30 cards for Player 2
4. Press confirm
5. **Expected**: A message box appears indicating how many cards remain to select
6. **Before fix**: No message box, error in console
