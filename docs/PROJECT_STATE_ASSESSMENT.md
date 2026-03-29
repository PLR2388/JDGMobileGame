# JDG Mobile Game - Project State Assessment

**Date**: 2026-03-28
**Branch**: `refactor-v3` (354 commits ahead of `master`)
**Unity Version**: 6000.3.6f1
**Current Version**: 2.1.1

---

## Executive Summary

The JDG Trading Card Game has undergone a massive 165-phase clean architecture refactoring. The architectural work is **excellent** -- well-layered, properly isolated, thoroughly tested. However, **the game is not release-ready today**. Several blocking issues remain before the `refactor-v3` branch can be merged to `master` and shipped as an update on Google Play.

| Dimension | Score | Verdict |
|-----------|-------|---------|
| Architecture Quality | 9/10 | Excellent |
| Code Quality | 8/10 | Very Good |
| Test Health | 7.5/10 | Good (25 failures remaining) |
| Release Readiness | 4/10 | Not Ready -- blockers exist |
| Migration Completeness | ~75-80% | Core logic migrated, UI/legacy still coexisting |

**Estimated time to fix blockers**: 2-5 focused days.

---

## 1. Refactoring Status

### What Was Accomplished (165 Phases)

The refactoring is the most significant engineering effort in the project's history: **952 files changed, ~62K lines added, ~12K removed**.

**Milestones achieved:**

- **Clean Architecture foundation** -- 4-layer separation (Domain, Application, Infrastructure, Presentation) with assembly definitions enforcing boundaries. Domain layer is pure C# with zero Unity dependencies.
- **Dependency Injection** -- VContainer 1.15.4 fully integrated with a 3-scope hierarchy (SharedServicesScope, GameSceneScope, MainScreenScope). 44+ injection points, 38+ service registrations. ServiceLocator pattern completely eliminated.
- **Event-Driven architecture** -- Custom EventBus with 58 domain events. All 9 legacy static UnityEvents migrated. Thread-safe, copy-on-iterate, proper IDisposable subscriptions.
- **Ability system** -- 57 modern `IAbility` implementations across 11 factories, replacing all legacy ability base classes (Ability, EffectAbility, FieldAbility, EquipmentAbility all removed).
- **MVP Presentation** -- 4 presenters (RoundDisplay, InvocationMenu, Dialog, CardDisplay) with thin MonoBehaviour views.
- **Comprehensive testing** -- 1,397 test methods across 88 test files and 8 assemblies, including 289 ability scenario tests, 39 synergy integration tests, and 16 E2E PlayMode tests.
- **Strangler Fig pattern** -- Legacy code coexists cleanly via a Bridge layer (CardConverter, LegacySystemInitializer, CardRepositoryInitializer).

### Migration Progress by Area

| Area | Status | Details |
|------|--------|---------|
| Domain entities & events | 95% | Pure C#, no Unity deps |
| Application use cases & abilities | 90% | 57 abilities, core use cases migrated |
| Service layer | 85% | Interfaces defined, implementations wrapped |
| Infrastructure (DI, EventBus, Repos) | 95% | Fully operational |
| Presentation (UI/Menu) | 40% | Only 4 presenters; Menu, MessageBox, CardChoice still legacy |
| InGameCard hierarchy | 30% | 27 MonoBehaviour card variants still in default assembly |
| Game Loop orchestration | 70% | Uses DI but legacy orchestration patterns remain |

### Code Distribution

| Layer | Files | LOC | % of Codebase |
|-------|-------|-----|---------------|
| **Clean Architecture** (Domain + Application + Infrastructure + Presentation + Core) | 123 | ~13,100 | 38% |
| **Legacy** (Services + Units + Menu + Managers + Multiplayer + OnePlayer + Cards) | 144 | ~19,600 | 62% |
| **Total** | **267** | **~32,700** | |

The 62% legacy figure looks high but is misleading -- the core game logic (abilities, combat, card management, events) is fully in clean layers. Legacy code is primarily UI, card MonoBehaviours, and tutorial scripting.

---

## 2. What's Working Well

### Architecture
- **Zero circular dependencies** between clean layers. Assembly definitions enforce this at compile time.
- **Domain layer** (`noEngineReferences: true`) can be tested and reasoned about without Unity.
- **Application layer** depends only on Domain -- all Unity-specific concerns are in Infrastructure/Presentation.
- **DI scopes** are well-designed: root for singletons (EventBus, repos), child scopes for scene-specific services.

### Testing
- **1,397 test methods** across 88 files -- a test-to-code ratio of ~137%.
- **Zero ignored/skipped tests** -- no test rot.
- **Professional test infrastructure**: `AbilityScenarioTestBase`, `TestFixtures`, custom assertions, PlayMode helpers.
- **Coverage by layer**: Domain (116 tests), Application (794), Infrastructure (220), Presentation (76), PlayMode (172).

### Code Health
- **Only 1 active TODO** in the entire codebase (`LocalizationServiceTests.cs` -- low priority).
- **Null safety** improved through DI and explicit checks (Phase 148-150).
- **Well-documented** -- every phase is logged in REFACTORING_STATUS.md with rationale.

---

## 3. Blocking Issues (Must Fix Before Merge/Release)

### 3.1 -- 25 Failing EditMode Tests

**Severity**: CRITICAL

The test suite has 25 remaining failures (reduced from 88 in commit `bf12fea0`):

- **11 pre-existing failures**: DIContainerPlayTests (5), ServiceIntegrationPlayTests (1), EquipmentInvocationSynergyTests (5)
- **14 tests needing logic updates**: ProtectionAbility (4), ResurrectionAbility (5), CombatAbility (2), SacrificeAbility (3) scenario tests

Additionally, **4-5 test methods** have `NotImplementedException` stubs that need to be completed or removed:
- `DrawCardUseCaseTests.cs` (lines 125, 130)
- `DrawCardsAbilityTests.cs` (line 235)
- `DestroyCardAbilityTests.cs` (line 289)
- `SummonPlayerEntityUseCaseTests.cs` (lines 157, 162, 167)

**Action**: Fix all failures, or mark with `[Ignore("reason")]` for documented deferrals. Complete or delete stub tests.

### 3.2 -- Accidental Test Scene in Repository

**Severity**: HIGH

File `Assets/InitTestScenec68c6f4a-61f6-4e6a-8374-79dc02b5cc8a.unity` (+ `.meta`) is auto-generated NUnit test runner debris. Must be deleted before merge.

### 3.3 -- Development Files in Repository Root

**Severity**: MEDIUM

Files that should be removed or gitignored:
- `new_phases.txt` -- internal planning notes
- `fixes/2025-01-06_android_build_nunit_resolution_failure.md`
- `fixes/2025-01-07_editmode_test_failures.md`
- `fixes/editmode-test-fixes.md`
- `scripts/discover_card_powers.py`

### 3.4 -- 114 Debug.Log Statements in Production Code

**Severity**: HIGH (mobile performance impact)

Debug.Log calls are **not stripped** in Android Release builds unless explicitly configured. They cause GC allocations and battery drain.

Top offenders:
- `GameSceneScope.cs` -- 23 calls
- `CardChoice.cs` -- 13 calls
- `MainScreenScope.cs` -- 11 calls

**Action**: Wrap in `#if UNITY_EDITOR` or remove entirely. Keep only `LogError`/`LogWarning` for runtime issues.

### 3.5 -- Android Build Verification Needed

A previous NUnit build failure was fixed by adding `"includePlatforms": ["Editor"]` to test assembly definitions. This fix must be verified with a fresh IL2CPP Android build.

The `com.coplaydev.unity-mcp` git package dependency (development tool) should also be confirmed as excluded from player builds.

---

## 4. Recommended Fixes (Non-Blocking but Important)

| # | Issue | Impact | Effort |
|---|-------|--------|--------|
| 1 | `AbilityRegistry` uses `Dictionary` (not thread-safe) | Low risk in single-threaded Unity, but inconsistent with thread-safe EventBus | Low |
| 2 | Domain events use `object` for card parameters | Type safety loss | Medium |
| 3 | Events mix `Guid` and `CardId` identifiers | Inconsistency, potential bugs | Medium |
| 4 | `CardSyncService` matches by `Title` not `CardId` | Bug risk if duplicate card titles exist | Medium |
| ~~5~~ | ~~Enum typos~~ | **FIXED** | Done |
| 6 | Duplicate scope creation logic in GameSceneScope/MainScreenScope | Maintenance burden | Low |
| 7 | Presentation references `JDG.Core` (minor architecture violation) | Clean architecture purity | Low |
| 8 | Missing `en.json` for English UI strings | English localization incomplete | Medium |

---

## 5. Post-Merge Technical Debt

These items are acceptable to defer but should be tracked:

- **Move `CardSelectorPresenter`** to `JDG.Presentation` assembly (blocked by InGameCard dependency)
- **Move 7 use cases** to `JDG.Application` (blocked by legacy types)
- **Migrate InGameCard hierarchy** (27 MonoBehaviour variants) to domain entities -- the largest remaining legacy migration
- **Refactor Menu/UI system** to MVP presenters (CardChoice, HandCardDisplay, etc.)
- **Replace `CardName.cs`** (681-line enum) with data-driven approach
- **Monitor 84 deleted C# files** for orphaned references in scenes/prefabs
- **Set up CI/CD** -- no GitHub Actions workflow files exist currently

---

## 6. Is the Refactoring Successful?

**Yes.** The refactoring is a success by any reasonable measure:

1. **The architecture is sound.** Clean separation of concerns with compile-time enforcement via assembly definitions. No circular dependencies. Pure domain layer. Proper DI with scope hierarchy.

2. **The migration strategy worked.** The Strangler Fig pattern allowed incremental migration without breaking the live game. Legacy code coexists cleanly through a well-defined Bridge layer.

3. **The ability system is fully modernized.** All 57 abilities use the new `IAbility` interface. All legacy ability base classes are removed. The AbilityRegistry provides centralized access through DI.

4. **Testing is comprehensive.** 1,397 tests with professional infrastructure. Scenario tests cover all abilities. Integration tests verify synergies. E2E tests validate key combinations.

5. **The codebase is maintainable.** New features can be added following clean architecture patterns. The event bus decouples systems. DI makes components testable.

The remaining 62% of "legacy" code is functional, stable, and wrapped behind interfaces. It will continue to work as-is. Further migration is a quality-of-life improvement, not a requirement.

---

## 7. Is the Game Ready for Release?

**Nearly ready.** Most blocking issues have been resolved. One remaining item requires manual verification.

### Release Checklist

| Priority | Item | Status |
|----------|------|--------|
| **BLOCKING** | ~~Fix or skip 25 failing tests~~ | **DONE** -- all 1,417 pass |
| **BLOCKING** | ~~Delete accidental test scene file~~ | **DONE** |
| **BLOCKING** | ~~Remove/gitignore development files~~ | **DONE** |
| **BLOCKING** | ~~Wrap/remove 114 Debug.Log calls~~ | **DONE** -- guarded with #if UNITY_EDITOR |
| **BLOCKING** | Verify Android IL2CPP build succeeds | Requires manual build |
| **RECOMMENDED** | Verify all 4 scenes load correctly | Requires manual verification |
| **RECOMMENDED** | Playtest: 1 full PvP game + tutorial | Requires manual verification |
| **RECOMMENDED** | ~~Fix enum typos before they become permanent~~ | **DONE** |
| **NICE TO HAVE** | Squash 354 commits into logical groups before merge | Optional |

### Remaining Manual Steps

| Task | Time |
|------|------|
| Android IL2CPP build verification | 1-2 hours |
| Scene load + playtest | 2-4 hours |
| **Total** | **3-6 hours** |

---

## 8. Conclusion

The JDG Mobile Game refactoring represents **165 phases of disciplined, well-documented architectural work**. The clean architecture foundation is solid, the ability system is fully modernized, and the test suite is comprehensive (1,417 passing, 0 failing). The Strangler Fig migration strategy has been executed effectively.

The game is **architecturally and operationally ready** for release, pending only manual verification steps (Android build + playtest). The `refactor-v3` branch can be merged to `master` and a new version shipped to Google Play after those checks pass.

**Recommendation**: Run an Android build + playtest cycle, then merge and release.
