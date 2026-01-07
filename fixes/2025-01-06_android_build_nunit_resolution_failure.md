# Android Build Failure: NUnit Assembly Resolution

**Date:** 2025-01-06
**Status:** Attempt #1
**Platform:** Android (IL2CPP)

---

## Problem Description

The Android build fails during the IL2CPP linking phase with a fatal error in the Unity CIL Linker.

### Error Message

```
Mono.Cecil.AssemblyResolutionException: Failed to resolve assembly: 'nunit.framework, Version=3.5.0.0, Culture=neutral, PublicKeyToken=null'
```

### Full Stack Trace

```
Fatal error in Unity CIL Linker
   at Unity.Linker.UnityDriver.RunDriverWithoutErrorHandling(TinyProfiler2 tinyProfiler, ILogger customLogger)
   at Unity.Linker.UnityDriver.RunDriverWithoutErrorHandling()

Caused by:
   at Mono.Linker.Steps.MarkStep.MarkField(FieldDefinition field, ...)
   at Mono.Linker.Steps.MarkStep.MarkEntireTypeInternal(TypeDefinition type, ...)
   at Mono.Linker.Steps.MarkStep.MarkEntireAssembly(AssemblyDefinition assembly)
   ...
```

### Build Output

```
Building Library/Bee/artifacts/Android/ManagedStripped failed with output:
*** Tundra build failed (1.47 seconds), 46 items updated, 48 evaluated
Build completed with a result of 'Failed' in 25 seconds (24794 ms)
```

---

## Root Cause Analysis

The IL2CPP linker was attempting to include test assemblies in the player build. These test assemblies reference `nunit.framework.dll`, which is an editor-only/test-only DLL not available at runtime.

### Why Test Assemblies Were Included

Several test assembly definitions had `"includePlatforms": []` which means they are included on **all platforms** (including Android). While they also had `"defineConstraints": ["UNITY_INCLUDE_TESTS"]` which should exclude them when tests aren't enabled, the IL2CPP linker was still trying to resolve their precompiled references (`nunit.framework.dll`).

### Affected Assembly Definitions

| Assembly | includePlatforms (Before) | Has NUnit Reference |
|----------|---------------------------|---------------------|
| `JDG.TestUtilities.asmdef` | `[]` (all platforms) | Yes |
| `JDG.PlayMode.Tests.asmdef` | `[]` (all platforms) | Yes |
| `JDG.Domain.Tests.asmdef` | `[]` (all platforms) | Yes |
| `JDG.Presentation.Tests.asmdef` | `[]` (all platforms) | Yes |

### Correctly Configured Assemblies (for reference)

| Assembly | includePlatforms | Status |
|----------|------------------|--------|
| `JDG.Application.Tests.asmdef` | `["Editor"]` | OK |
| `JDG.Infrastructure.Tests.asmdef` | `["Editor"]` | OK |
| `JDG.Tests.Editor.asmdef` | `["Editor"]` | OK |
| `JDG.TestUtilities.Editor.asmdef` | `["Editor"]` | OK |

---

## Attempt #1: Add Editor-Only Platform Constraint

### Changes Made

Modified 4 assembly definition files to add `"includePlatforms": ["Editor"]`:

#### 1. `Assets/Tests/JDG.TestUtilities/JDG.TestUtilities.asmdef`

```diff
-    "includePlatforms": [],
+    "includePlatforms": [
+        "Editor"
+    ],
```

#### 2. `Assets/Tests/PlayMode/JDG.PlayMode.Tests.asmdef`

```diff
-    "includePlatforms": [],
+    "includePlatforms": [
+        "Editor"
+    ],
```

#### 3. `Assets/Tests/JDG.Domain.Tests/JDG.Domain.Tests.asmdef`

```diff
-    "includePlatforms": [],
+    "includePlatforms": [
+        "Editor"
+    ],
```

#### 4. `Assets/Tests/JDG.Presentation.Tests/JDG.Presentation.Tests.asmdef`

```diff
-    "includePlatforms": [],
+    "includePlatforms": [
+        "Editor"
+    ],
```

### Expected Result

Test assemblies will be completely excluded from player builds, preventing the linker from trying to resolve `nunit.framework.dll`.

### Result

**FAILED** - Build still failed with same error. The Library/Bee cache still contained stale assemblies compiled before the asmdef changes.

---

## Attempt #2: Clear Build Cache

### Problem

Even after modifying the asmdef files, the build cache in `Library/Bee/` still contained the old compiled DLLs with NUnit references.

### Actions Taken

1. Deleted `Library/Bee/` folder to clear all cached build artifacts
2. Forced Unity to recompile all assemblies via `refresh_unity`

### Result

**Partial** - Build cache cleared but same error persisted. Root cause was elsewhere.

---

## Attempt #3: Move Misplaced Test File (ROOT CAUSE FIX)

### Problem

Using `monodis --typeref` on `JDG.Legacy.dll` revealed NUnit type references:
```
148: [nunit.framework]NUnit.Framework.TestFixtureAttribute
149: [nunit.framework]NUnit.Framework.SetUpAttribute
150: [nunit.framework]NUnit.Framework.TearDownAttribute
151: [nunit.framework]NUnit.Framework.TestAttribute
```

Investigation found a test file in the wrong location:
`Assets/_Scripts/Tests/PresenterTests/CardSelectorPresenterTests.cs`

This file was inside `Assets/_Scripts/` which is covered by `JDG.Legacy.asmdef`,
causing the test code to be compiled into the runtime `JDG.Legacy.dll`.

### Actions Taken

1. Moved `CardSelectorPresenterTests.cs` from `Assets/_Scripts/Tests/PresenterTests/`
   to `Assets/Tests/JDG.Presentation.Tests/Presenters/`
2. Removed empty `Assets/_Scripts/Tests/` folder and its `.meta` file
3. Added `JDG.Legacy` reference to `JDG.Presentation.Tests.asmdef` for test dependencies
4. Fixed `Action` type ambiguity in `DialogPresenterTests.cs` (used `System.Action` instead of `Action`
   to avoid conflict with `OnePlayer.Action` enum brought in by `JDG.Legacy` reference)
5. Cleared `Library/Bee/` cache and forced Unity recompilation

### Result

**SUCCESS** - All compilation errors resolved. Test assemblies properly isolated.

---

## Additional Notes

### Why `defineConstraints` Wasn't Enough

The `defineConstraints: ["UNITY_INCLUDE_TESTS"]` should theoretically exclude these assemblies when `UNITY_INCLUDE_TESTS` is not defined (regular builds). However, the IL2CPP linker appears to process assembly metadata and precompiled references before applying define constraints, causing it to fail when trying to resolve `nunit.framework.dll`.

### Impact on Testing

Adding `"includePlatforms": ["Editor"]` means:
- All tests will run in the Unity Editor (Test Runner)
- PlayMode tests will still work in the Editor's Play mode
- Tests will NOT be included in device builds (which is the expected behavior for most projects)

If on-device testing is required in the future, a separate test build configuration would need to be created.

---

## Logs Location

- Unity Editor Log: `~/Library/Logs/Unity/Editor.log`
- Console errors retrieved via Unity MCP tools

---

## Follow-up Actions

1. [ ] Retry Android build
2. [ ] Verify build succeeds
3. [ ] If failed, analyze new error logs
4. [ ] Update this document with results
