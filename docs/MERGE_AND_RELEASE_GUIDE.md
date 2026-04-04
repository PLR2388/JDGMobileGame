# Merge & Release Guide for refactor-v3

**Date**: 2026-04-01
**Branch**: `refactor-v3`
**Target**: `master`

---

## Pre-Merge Verification

All automated checks have been completed:

- 1,414 tests passing, 0 failing, 0 stubs
- All 14 assemblies compile (6 clean architecture + legacy + 5 test + 2 utility)
- Legacy enum duplication eliminated
- Debug.Log calls guarded with `#if UNITY_EDITOR`
- No accidental test scenes or development artifacts

The following manual steps remain before merging.

---

## Step 1: Android IL2CPP Build Verification

**Why**: Test assembly definitions were updated with `"includePlatforms": ["Editor"]` to fix a previous NUnit build failure. This must be verified with a real build.

1. Open the project in Unity 6000.0.60f1
2. Go to **File > Build Settings**
3. Select **Android** platform
4. Set **Scripting Backend** to **IL2CPP**
5. Click **Build** (or **Build and Run** if you have a device connected)
6. Verify the build completes without errors

**What to watch for**:
- Any `TypeLoadException` or assembly resolution errors at runtime
- The `com.coplaydev.unity-mcp` package should not be included in the player build (it's an editor-only dependency)

**Estimated time**: 1-2 hours (including build time)

---

## Step 2: Scene Load Verification

**Why**: 84 C# files were deleted during the refactoring. Scenes and prefabs may hold serialized references to removed scripts, causing `MissingReferenceException` at runtime.

Test each scene loads without errors:

1. **`_preload.unity`** - Should auto-transition to MainScreen
2. **`MainScreen.unity`** - Main menu should display, buttons should be interactive
3. **`Game.unity`** - Load a 2-player game, verify the board renders
4. **`TutoPlayerGame.unity`** - Start tutorial, verify dialogue and highlights work

**What to watch for**:
- Yellow "Missing Script" warnings in the console
- UI elements that don't render or respond to input
- NullReferenceException in the console on scene load

**Estimated time**: 30 minutes

---

## Step 3: Playtest

**Why**: The refactoring touched combat logic, ability execution, card sync, and event routing. A full game confirms end-to-end correctness.

### 3a: Tutorial Game
1. Start **TutoPlayerGame** scene
2. Play through the full tutorial
3. Verify: dialogue boxes appear, highlights work, card placement works, combat resolves

### 3b: PvP Game (2-player on same device)
1. Start a game from MainScreen
2. Play at least 3-4 full turns per player
3. Test the following during the game:
   - **Card placement**: Play at least 1 of each card type (Invocation, Equipment, Effect, Field)
   - **Combat**: Attack with invocations, verify ATK/DEF calculations
   - **Abilities**: Play cards with abilities, verify they trigger (check console for errors)
   - **Card draw**: Verify draw phase works each turn
   - **End game**: Play until one player reaches 0 HP, verify game over screen

**What to watch for**:
- Cards not appearing on the field after being played
- Combat not resolving (stuck turn)
- Abilities not triggering or triggering incorrectly
- Any exception in the Unity console

**Estimated time**: 1-2 hours

---

## Step 4: Merge to master

Once all verification passes:

```bash
git checkout master
git merge refactor-v3
git push origin master
```

Or create a PR:

```bash
gh pr create --base master --head refactor-v3 \
  --title "Clean Architecture Refactoring (172 Phases)" \
  --body "Complete clean architecture migration: 172 phases, 1,414 tests, 57 abilities, 58 domain events."
```

---

## Step 5: Release to Google Play

1. Bump version in **Project Settings > Player > Version**
2. Build a signed AAB: **File > Build Settings > Build App Bundle**
3. Upload to [Google Play Console](https://play.google.com/console)
4. Release to internal testing track first, then promote to production

---

## Rollback Plan

If critical issues are found after merge:

```bash
git checkout master
git revert --no-commit HEAD
git commit -m "revert: Roll back refactor-v3 merge due to [reason]"
git push origin master
```

The `refactor-v3` branch will still exist for debugging and fixing.
