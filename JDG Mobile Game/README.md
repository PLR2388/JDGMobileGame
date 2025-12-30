# JDG Mobile Game

A Unity-based mobile card game implementing Clean Architecture with dependency injection.

## Quick Start

1. **Prerequisites**: Unity 2022.3.9f1+, .NET 6.0+
2. **Open Project**: Open the `JDG Mobile Game` folder in Unity Hub
3. **Run**: Open `_preload` scene and press Play

For detailed setup instructions, see [GETTING_STARTED.md](GETTING_STARTED.md).

## Architecture

The project uses Clean Architecture with 4 main layers:

```
┌─────────────────────────────────────────┐
│        JDG.Presentation (MVP)           │  ← Presenters, Views
├─────────────────────────────────────────┤
│        JDG.Infrastructure               │  ← DI, EventBus, Repositories
├─────────────────────────────────────────┤
│        JDG.Application                  │  ← Use Cases, Services, Abilities
├─────────────────────────────────────────┤
│        JDG.Domain (Pure C#)             │  ← Entities, Value Objects, Events
└─────────────────────────────────────────┘
```

**Key Technologies:**
- **VContainer** - Dependency Injection
- **EventBus** - Event-driven communication (30+ domain events)
- **IAbility** - Modern ability system (57 implementations)

For detailed architecture documentation, see [ARCHITECTURE.md](ARCHITECTURE.md).

## Documentation

| Document | Purpose |
|----------|---------|
| [ARCHITECTURE.md](ARCHITECTURE.md) | System architecture and design patterns |
| [GETTING_STARTED.md](GETTING_STARTED.md) | Developer setup and onboarding |
| [API_REFERENCE.md](API_REFERENCE.md) | Interface and API documentation |
| [HOW_TO_GUIDES.md](HOW_TO_GUIDES.md) | Step-by-step procedures |
| [REFACTORING_STATUS.md](REFACTORING_STATUS.md) | Migration history (123 phases) |

## Project Structure

```
Assets/
├── _Scripts/
│   ├── JDG.Domain/          # Pure C# domain layer
│   ├── JDG.Application/     # Use cases and abilities
│   ├── JDG.Infrastructure/  # DI and repositories
│   ├── JDG.Presentation/    # MVP presenters
│   └── Bridge/              # Legacy compatibility
├── Tests/                   # 618+ unit tests
└── Scenes/                  # Unity scenes
```

## Testing

```bash
# Run EditMode tests via Unity Test Runner
# Window → General → Test Runner → EditMode → Run All
```

Test assemblies:
- `JDG.Domain.Tests` - Domain logic
- `JDG.Application.Tests` - Use cases and abilities
- `JDG.Infrastructure.Tests` - Services and repositories
- `JDG.Presentation.Tests` - Presenter tests

## License

Proprietary - All rights reserved.
