# Getting Started - JDG Mobile Game Developer Guide

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Project Setup](#project-setup)
3. [Project Structure](#project-structure)
4. [Understanding Clean Architecture](#understanding-clean-architecture)
5. [Running the Game](#running-the-game)
6. [Development Workflows](#development-workflows)
7. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Software

| Software | Version | Purpose |
|----------|---------|---------|
| **Unity Hub** | Latest | Project management |
| **Unity Editor** | 6000.0.60f1 | Development environment |
| **Git** | Latest | Version control |
| **IDE** | Rider 2023.2+ or VS Code | Code editing |

### Recommended IDE Extensions

**For Rider:**
- Unity Support (built-in)
- .NET Decompiler (for debugging)

**For VS Code:**
- C# (Microsoft)
- Unity Debugger
- Unity Tools

---

## Project Setup

### 1. Clone the Repository

```bash
git clone https://github.com/PLR2388/JDGMobileGame
cd JDGMobileGame
```

### 2. Open in Unity Hub

1. Open Unity Hub
2. Click "Add" → "Add project from disk"
3. Select the `JDG Mobile Game` folder
4. Unity Hub will prompt to install Unity 6000.0.60f1 if not present

### 3. Open the Project

1. Click on the project in Unity Hub
2. Wait for Unity to import assets (first time takes several minutes)
3. Ignore initial warnings - they resolve after import completes

### 4. Verify Setup

1. Open the `_preload` scene (`Assets/Scenes/_preload.unity`)
2. Press Play
3. The game should load and show the main menu

---

## Project Structure

```
JDG Mobile Game/
├── Assets/
│   ├── _Scripts/                 # All C# code
│   │   ├── JDG.Domain/          # Pure C# business entities
│   │   ├── JDG.Application/     # Use cases and interfaces
│   │   ├── JDG.Infrastructure/  # Implementations
│   │   ├── JDG.Presentation/    # UI presenters
│   │   ├── Services/            # Legacy use cases (migrating)
│   │   ├── Managers/            # Legacy managers
│   │   ├── Units/               # Card logic (InGameCard)
│   │   ├── Cards/               # Card display (PlayerCards)
│   │   └── Bridge/              # Legacy-to-modern adapters
│   │
│   ├── Resources/
│   │   └── Cards/               # Card ScriptableObjects
│   │
│   ├── Scenes/
│   │   ├── _preload             # Initialization scene
│   │   ├── MainScreen           # Main menu
│   │   ├── Game                 # 2-player game
│   │   └── TutoPlayerGame       # Tutorial
│   │
│   └── Tests/                   # Unit & integration tests
│       ├── JDG.Domain.Tests/
│       ├── JDG.Application.Tests/
│       ├── JDG.Infrastructure.Tests/
│       ├── JDG.Presentation.Tests/
│       └── PlayMode/
│
├── ARCHITECTURE.md              # Clean Architecture details
├── REFACTORING_STATUS.md        # Migration progress
├── API_REFERENCE.md             # Interface documentation
└── HOW_TO_GUIDES.md            # Step-by-step procedures
```

### Key Folders Explained

| Folder | Purpose | Assembly |
|--------|---------|----------|
| `JDG.Domain/` | Business entities (Card, Player), value objects | JDG.Domain.asmdef |
| `JDG.Application/` | Use cases, interfaces, abilities | JDG.Application.asmdef |
| `JDG.Infrastructure/` | Repositories, EventBus, DI config | JDG.Infrastructure.asmdef |
| `JDG.Presentation/` | Presenters, view interfaces | JDG.Presentation.asmdef |
| `Services/` | Legacy services (being migrated) | Default assembly |
| `Units/` | InGameCard hierarchy (legacy) | Default assembly |
| `Bridge/` | Adapters between old/new code | Default assembly |

---

## Understanding Clean Architecture

### The 4-Layer Model

```
┌─────────────────────────────────────────────────────────┐
│  PRESENTATION  (JDG.Presentation + MonoBehaviours)      │
│  - Presenters handle UI logic                           │
│  - Views are thin MonoBehaviours                        │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼ depends on
┌─────────────────────────────────────────────────────────┐
│  APPLICATION   (JDG.Application)                        │
│  - Use Cases (DrawCardUseCase, AttackUseCase, etc.)    │
│  - Service Interfaces (ICardRepository, etc.)          │
│  - Abilities (IAbility implementations)                │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼ depends on
┌─────────────────────────────────────────────────────────┐
│  INFRASTRUCTURE (JDG.Infrastructure)                    │
│  - Repository implementations                           │
│  - EventBus (pub/sub messaging)                        │
│  - DI Container configuration                          │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼ depends on
┌─────────────────────────────────────────────────────────┐
│  DOMAIN        (JDG.Domain)                            │
│  - Entities (Card, Player)                             │
│  - Value Objects (CardId, CardStats)                   │
│  - Domain Events                                        │
│  - NO Unity dependencies (pure C#)                      │
└─────────────────────────────────────────────────────────┘
```

### Dependency Rules

**Allowed:**
- Presentation → Application → Domain
- Infrastructure → Application → Domain

**Forbidden:**
- Domain → anything else
- Application → Infrastructure
- Lower layer → Higher layer

### Key Patterns

1. **Dependency Injection (VContainer)**
   - All dependencies injected via constructor or `[Inject]`
   - Configured in `SharedServicesScope`, `GameSceneScope`, `MainScreenScope`

2. **EventBus (Pub/Sub)**
   - Decoupled communication between components
   - Use cases publish events, presenters subscribe

3. **MVP Pattern**
   - Presenter contains logic
   - View interface abstracts UI
   - MonoBehaviour implements view interface

---

## Running the Game

### In Unity Editor

1. Open the `_preload` scene (`Assets/Scenes/_preload.unity`)
2. Press the Play button
3. The game automatically loads MainScreen

**Important**: Always start from `_preload` scene - it initializes DI and singletons.

### Running Tests

1. Window → General → Test Runner
2. Select "EditMode" or "PlayMode" tab
3. Click "Run All" or run individual tests

**Test Categories:**
- EditMode: Fast unit tests (no Unity runtime)
- PlayMode: Integration tests (need Unity runtime)

### Building for Android

1. File → Build Settings
2. Select Android platform
3. Click "Switch Platform" (first time only)
4. Click "Build" or "Build and Run"

---

## Development Workflows

### Adding a New Feature

1. **Identify the layer** - Where does the logic belong?
   - Game rules → Domain
   - Operations → Application (Use Case)
   - Data access → Infrastructure
   - UI logic → Presentation

2. **Create interfaces first** - Define contracts in Application layer

3. **Implement in Infrastructure** - Repositories, services

4. **Register in DI** - Add to appropriate Scope

5. **Write tests** - Unit tests for each layer

### Modifying Existing Code

1. **Find the file** - Use IDE search or grep
2. **Check dependencies** - Find References in IDE
3. **Run tests before** - Establish baseline
4. **Make changes** - Follow existing patterns
5. **Run tests after** - Ensure no regressions

### Common Tasks

| Task | Where to Look |
|------|---------------|
| Add a card ability | `JDG.Application/Abilities/Implementations/` |
| Change game rules | `JDG.Application/UseCases/` |
| Fix UI bug | `JDG.Presentation/Presenters/` or MonoBehaviours |
| Add domain event | `JDG.Domain/Events/` |
| Change card data | `Resources/Cards/` (ScriptableObjects) |

---

## Troubleshooting

### Common Issues

#### "Type or namespace not found" errors

**Cause**: Assembly reference missing
**Fix**: Check the `.asmdef` file includes required references

#### "Cannot resolve type" at runtime

**Cause**: DI registration missing
**Fix**: Add registration to appropriate Scope (SharedServicesScope, GameSceneScope, etc.)

#### Tests fail with "No such host"

**Cause**: Tests trying to access network/Unity services
**Fix**: Use mocks instead of real services in tests

#### "NullReferenceException" on start

**Cause**: DI not initialized or wrong scene
**Fix**: Start from `_preload` scene, not other scenes

#### Build fails with "Duplicate class"

**Cause**: Class defined in multiple assemblies
**Fix**: Check `.asmdef` files for overlapping folders

### Getting Help

1. Check existing tests for usage examples
2. Search `REFACTORING_STATUS.md` for migration notes
3. Read inline XML documentation in interfaces
4. Ask on [JDG Mobile Game Discord](https://discord.gg/ujgVYD3e5Q)

---

## Next Steps

- Read [ARCHITECTURE.md](ARCHITECTURE.md) for detailed layer information
- Read [API_REFERENCE.md](API_REFERENCE.md) for interface documentation
- Read [HOW_TO_GUIDES.md](HOW_TO_GUIDES.md) for step-by-step procedures
- Explore the test files for usage examples
