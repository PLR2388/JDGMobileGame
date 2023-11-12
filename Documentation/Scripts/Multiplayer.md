# Multiplayer

## GameLoop

`GameLoop` acts as the central control point, managing crucial game phases and interactions in a Unity-based card game environment. Upon initialization, it sets up the necessary touch and input listeners, ensuring all events are correctly deregistered upon the object's destruction. The class integrates comprehensive UI interaction mechanisms, detailing functionalities such as pausing the game, card selection, and transitioning between game phases.

The gameplay is organized into various phases (e.g., `Choose`, `Attack`, `End`, and `GameOver`). The `ChoosePhase` handles card selection, playing the appropriate background music depending on the card's family. During the `Attack` phase, players can select opponents for combat. If players run out of cards or their health drops to zero, the `GameOver` phase redirects the player back to the main screen.

Audio cues enrich the player's experience, with contextual music being played based on the game's current state or card interactions. For example, unique melodies play during the drawing phase or when a battle begins. In-game turns and phases are handled using the `GameStateManager` class, allowing the game to progress seamlessly.

Furthermore, `GameLoop` manages vital game mechanics such as drawing cards, computing attacks, and monitoring player health. Comprehensive handlers ensure proper behavior when players interact with cards they own, choose opponents, or evaluate potential game-over scenarios.

In essence, `GameLoop` provides an organized framework for managing and transitioning between distinct gameplay phases, ensuring a dynamic, interactive, and immersive card game experience.
