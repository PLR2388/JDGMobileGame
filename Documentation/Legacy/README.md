# Legacy Documentation

**WARNING**: This documentation is outdated and describes the pre-refactoring codebase.

These files were created in December 2023 before the Clean Architecture refactoring and do not reflect the current architecture.

## Current Documentation

For up-to-date architecture information, see:
- `/JDG Mobile Game/ARCHITECTURE.md` - Clean Architecture overview
- `/JDG Mobile Game/REFACTORING_STATUS.md` - Migration progress and phase details

## Why These Files Are Kept

These files are preserved for historical reference and to help understand legacy code that has not yet been migrated. They describe:

- Old Singleton-based managers (now replaced by DI)
- Old event system (now replaced by EventBus)
- Old card handling (being abstracted via IInGameCard)

## Files in This Folder

| File | Describes | Current Status |
|------|-----------|----------------|
| Managers.md | Singleton managers | Replaced by VContainer DI |
| Cards.md | PlayerCards, InGameCard | Partially migrated to JDG.Domain |
| Systems.md | Legacy systems | Replaced by use cases |
| UI.md | Old UI patterns | Migrated to MVP pattern |
| Others | Various legacy code | See REFACTORING_STATUS.md |

---

*Archived: 2025-12-28*
