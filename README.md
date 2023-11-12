# JDG - Trading Card Game Mobile

![JDG - Trading Card Game Logo](https://lh3.googleusercontent.com/oOF9nwlRl55WEyilhyOcN5t_V9zGC1r2QumS6SxcO5JhzdynZCdPkQTIjdVZ8kqs8Bw)

## Introduction

JDG - Trading Card Game Mobile is a Unity mobile video game available on [**Google Play**](https://play.google.com/store/apps/details?id=fr.wonderfulAppStudio.JDGMobileGame) adapting Joueur du Grenier Trading Card Game on phone. You can learn more about the game rules [**here**](https://www.parkage.com/files/rules/regle_TCG_joueur-du-grenier_liste-de_cartes.pdf) or in [**video**](https://www.youtube.com/watch?v=tBtRhNC-jFc).

To sum up, it's a one-to-one game in which each player can build their own card deck based on their preferences and strategy, following two rules:

- The deck must have exactly 30 cards, with a maximum of 5 shiny cards.
- No duplicate cards are allowed in the deck.

Each player starts with 30 life stars. The goal is to use your cards to reduce your opponent's life stars to 0 while defending your own life stars from your opponent's attacks. If a player draws the last card from their deck, the duel ends at the end of their turn, and the player with the most remaining life stars wins.

 There are 5 types of cards :
 - Summoning cards

  Central to the game. You place them on your playing field in one of the four designated slots. Only up to four summoning cards can be placed on the field at once, and some require specific conditions listed on the card. Each summoning card has attack (ATQ) and defense (DEF) star values ranging from 0 to 5.

- Equipment cards

Modify abilities or use of the summoning cards they're linked to. You place them on your field or your opponent's field below the summoning card they equip. Their effect is permanent, but if an summoning card is destroyed, its associated equipment card is also discarded. Only one equipment can be linked per summoning unless stated otherwise. If the equipment card is destroyed, it can be replaced.

- Effect cards

 Used to apply various game effects, like bonuses for your summoning cards or penalties affecting your opponent's card usage. You place them on your field in one of the four slots, so only four effect cards can be active at once. They remain as long as their effect lasts, usually for the turn they're played, unless stated otherwise.

- Field cards

 Modify the abilities of the field they're played on. You place them on your field or your opponent's field in the designated slot, so only one field card can be active on each field at once. Their effect is permanent, but if a field card is destroyed, all other cards on the associated field are also discarded.

- Contra cards

Used to counter an opponent's action. You don't place them on the field; their action is immediate, and once used, they're discarded. They can be used at any time during the duel, even during your opponent's turn, as long as it's in response to an opponent's action specified on the card.

The game works on a turn-by-turn basis. Each turn consists of three phases:

1. Draw Phase: At the start of the turn, the player draws a card from their deck and adds it to their hand.

2. Play Phase: The player can play as many cards as they wish onto the field based on what's in their hand. During this phase, and only this phase, they can summon their summoning cards and play their effect, equipment, and field cards, unless stated otherwise.

3. Attack Phase: The player can attack with the summoning cards on their field:
• They can target the opponent's summoning cards if there are any on the opponent's field.
• Or directly target the opponent's life stars if the opponent has no summoning cards on their field.

Each summoning card can only attack once per turn, unless stated otherwise. If a player has multiple summoning cards on their field, each must attack independently of the others. The attacks from summoning cards aren't cumulative, unless stated otherwise.


## Table of Contents

 1. [Installation](#installation)
 2. [Components Overview](#components-overview)
  1. [Scripts](#scripts)
      1. [Cards Scripts](#cards-scripts)
      2. [Managers](#managers)
      3. [Menu](#menu)
      4. [MessageBox](#messagebox)
      5. [Multiplayer](#multiplayer)
      6. [OnePlayer](#oneplayer)
      7. [Scriptables](#scriptables)
      8. [Sound](#sound)
      9. [Systems](#systems)
      10. [UI](#ui)
      11. [Units](#units)
      12. [Utilities](#utilities)
  2. [Assets](#assets)
      1. [Audio](#audio)
      2. [Cards](#cards)
      3. [Dialogue Data](#dialogue-data)
      4. [Image](#image)
      5. [Localization](#localization)
      6. [Material](#material)
      7. [Music](#music)
      8. [Prefab](#prefab)
      9. [Scenarii](#scenarii)
      10. [Sound Effect](#sound-effect)
      11. [Video](#video)
  3. [Scenes](#scenes)
 3. [Getting Started](#getting-started)
 4. [Contributing](#contributing)
 5. [License](#license)
 6. [Acknowledgments](#acknowledgments)

## Installation

1. Install [Unity Hub](https://unity.com/download)
2. Clone the current repository on your computer: `git clone https://github.com/PLR2388/JDGMobileGame`
3. Open JDG Mobile Game directory as a Unity project in Unity Hub
4. Download the necessary Unity Editor prompted
5. Launch JDG Mobile Game Project using the appropriate Unity Editor

## Components Overview

### Scripts

Store under the _Scripts, they are divided into sub-directories. Each script is documented with C# summaries.

#### Cards

The content revolves around the management, display, and categorization of cards in a card game, along with managing player status.

#### Managers

These files revolve around the management, interaction, and display of cards, players, and the game state. They also handle user input and user interface elements for smooth gameplay.

#### Menu

This system provides a comprehensive card game experience, allowing players to choose cards, adjust settings, engage in gameplay, and learn through tutorials.

#### MessageBox

The UI framework provides a comprehensive solution for crafting interactive UI elements in Unity. At its heart, `CardSelector` is a pivotal tool for managing and interacting with an assortment of cards, tailoring the user experience based on different conditions. The `Config` module ensures a structured approach to storing configurations for the UI, guaranteeing uniformity. The `DisplayCards` system empowers developers with the capacity to vividly visualize card elements with dynamic responsiveness based on interactions. The `MessageBox` and `MessageBoxBaseComponent` tandem serves as the backbone for crafting and manipulating dialog prompts, delivering both basic and advanced functionalities. Together, these components amalgamate to create a robust and seamless UI interaction system, simplifying development and enhancing user engagement.

#### Multiplayer

`GameLoop` serves as the central orchestrator for a Unity-based card game, handling user inputs, game phase transitions, and core gameplay mechanics like card drawing and combat. The class integrates touch events for card interactions, manages game phases such as selection and attack, and offers audio cues for an enriched user experience. It also ensures game continuity by monitoring player health and card availability, with mechanisms to address game-over scenarios.

#### OnePlayer

This system, comprised of multiple interconnected classes, facilitates an immersive dialogue and tutorial experience in Unity. The core component, `DialogueObject`, holds dialogue data while `DialogueUI` renders this data on-screen, with a typewriter effect achieved by `TypewriterEffect`. User responses are managed through `Response` and `ResponseHandler`. The flow of dialogues is determined by `NextDialogueTrigger`, while `Scenario` and `ScenarioDecoder` manage overarching narrative structures. Tutorials benefit from `DialogueTutoHandler`, which integrates with various highlight components like `HighLightButton`, `HighLightCard`, `HighLightPhysicalCard`, `HighLightPlane`, and `HighLightText` to emphasize UI and game elements. The `TutoPlayerGameLoop` oversees the tutorial game loop. Lastly, the `VideoPlayerObserver` monitors Unity's VideoPlayer, triggering events upon video completion. Collectively, these classes offer a holistic framework for narrative-driven experiences and tutorials in-game.

#### Scriptables

The Unity card system, built around the foundational `Card` class within the `Cards` namespace, offers a streamlined platform for game card development using Unity's `ScriptableObject` capabilities. Each card type extends this base with specialized attributes and interactions: `ContreCard` uniquely identifies with the "Contre" card type, `EffectCard` is enhanced by its collection of `EffectAbilities`, `EquipmentCard` offers equipment-related actions through `EquipmentAbilities`, `FieldCard` links to a specific `CardFamily` while introducing `FieldAbilities`, and `InvocationCard` is tailored with combat stats via `InvocationCardStats` and diverse game mechanics using `Conditions` and `Abilities` lists. These specialized card systems promote modular game design, offering developers and players a rich, layered, and strategic gameplay environment.

#### Sound

`SoundList` categorizes the game's audio assets into two enumerations: `Music` and `SoundEffect`. `Music` covers various game themes, while `SoundEffect` lists in-game effects. This structure facilitates organized access to the game's audio elements.

#### Systems

The "Systems" repository offers a suite of Unity-based classes designed to enhance game development workflows. It includes:

- **Localization Framework**: Comprising of `LocalizationKeys` (an enumeration for game localization) and `LocalizationKeyStrings` (a static dictionary mapping localization keys to their string representations), this setup ensures easy translation and customization for a global audience.

- **AudioSystem**: A comprehensive Unity class tailored for dynamic audio management in games, providing controls for music and sound effects, volume adjustments, indexing, and seamless audio experiences.

- **LocalizationSystem**: Designed for efficient localized text resource management, this system automates localization processes, parses JSON data, supports nested structures, and ensures user-tailored content delivery.

- **ResourceSystem**: Tailored for card games, this management system handles card resource organization, efficient loading, and quick retrieval, streamlining card-related functionalities.

- **SceneLoaderSystem**: A tool for handling game scene transitions. It supports efficient asynchronous loading, centralized scene naming, and automated transitions from preload scenes to main screens.

- **Systems**: Serving as a foundational class, `Systems` acts as a persistent, central hub for the game, housing multiple sub-systems to maintain game state across various scenes and transitions.

These tools collectively provide a cohesive framework to streamline game development, ensuring efficient systems, robust organization, and an enhanced gaming experience.

#### UI

The `InfiniteScroll` module facilitates endless content scrolling for user interfaces, ensuring a continuous and smooth user experience. `HealthUI` is dedicated to visually representing a player's health status, updating in real-time as the player's health changes. The `InvocationMenuManager` class manages the game's invocation menu, allowing players to select and invoke specific game actions. `RoundDisplayManager` oversees the game's round displays, including text indicators and camera orientation changes that correlate with game rounds. `SceneLoader` serves as the primary mechanism to navigate between different scenes, particularly aiding in transitioning to tutorial sections and handling application exit scenarios. Lastly, `UpdateDescription` focuses on the dynamic presentation of card details, adjusting the visualization based on card type, including specialized handling for Invocation cards, and other attributes.

#### Units

It covers a wide range of classes, each designed to contribute specific abilities, conditions, and behaviors to the game's mechanics. These include classes for invoking cards from the deck, sacrificing cards for benefits, managing attack and defense strategies, setting conditions for card summoning, and handling in-game events related to different card types. Additionally, it includes libraries and factories for organizing abilities and creating card instances, as well as base classes for defining common properties and actions of cards.

#### Utilities

The Utilities section introduces two key components for Unity projects: `EnumGenerator` and `StaticInstance` variants. `EnumGenerator` simplifies the process of creating enums and string mappings from JSON data, ideal for localization. It automatically parses JSON to generate and save enums and mappings, promoting type safety and convenience in accessing localized strings. The `StaticInstance` family—comprising `StaticInstance<T>`, `Singleton<T>`, and `PersistentSingleton<T>`—offers a spectrum of singleton patterns for object instantiation control. `StaticInstance<T>` allows for state reset by overriding instances, `Singleton<T>` ensures a single instance by destroying duplicates, and `PersistentSingleton<T>` extends this by persisting across scene loads. These abstract classes use Unity's lifecycle methods for setup and cleanup, aiding in managing game state, data, and uninterrupted audio across scenes. These utilities are indispensable for maintaining clean and efficient codebases in Unity development.

### Assets

#### Audio

This folder in Resources contains all audio lines used during the tutorial.

#### Cards

This folder stores all scriptable objects that represent each card of the game.

#### Dialogue Data

This folder stores Dialogue Object Scriptable Object which represents a dialogue. The Dialogue Object contains lines of text, triggers for the next line, optional responses the player can choose and music associated to a line of text with an index (for instance, if the fourth music has an associated index of 8, it means it must be played during the eighth line of text).

#### Image

This folder contains all the card images, the game board and the message box. Basically, it contains all the jpg files

#### Localization

At the moment, it contains translations for text that appears in C# code. It's only in French right now.

#### Material

This folder contains all images that have been converted to Unity material for use in the game and are associated with scriptable objects.

#### Music

It contains pieces of music used during the game. Most of them were composed by [Thomas - Compositeur](https://www.youtube.com/@thomas-compositeur7973), only the main Theme was composed by [Yannick Crémer](https://www.youtube.com/@YannickCremer)

#### Prefab
 - Card

 Card represents a 2D card one can see on hand card or during selection on Message box.

 - MessageBox

 MessageBox represents a PopUp displayed to the player to make a selection. They can select Yes/No answers, Ok answers or one or several cards from it.

 - PhysicalCard

 PhysicalCard is a 3D card displayed on the game board.

#### Scenarii

It contains Json file whose purpose is to describe what actions should be performed when a certain line of the dialog is reached.

#### Sound Effect

It contains all the sound effects used in the game. All of them were produced by Py MARREC.

#### Video

It contains all the videos that are played during the game. At the moment there is only one tuto video that comes from [this video](https://www.youtube.com/watch?v=tBtRhNC-jFc)

### Scenes

- _preload

This scene is used for initialization and should not be shown to the user, because it refers directly to the MainScreen scene.

- Game

Display of the game board for 2-player mode.

- MainScreen

Display the main menu to select options or game modes.

- TutoPlayerGame

Display the board for the tutorial

## Getting Started

When you're on Unity Editor, select the `_preload` scene and press the Play button to start the game in Unity Editor.

### Requirements for Running the Game

- **Unity Editor Version**: The game is developed for Unity Editor version 2022.3.9f1. Ensure you have this version installed for compatibility.
- **Additional Libraries**: No additional libraries are required. All necessary components are included within the project.
- **System Requirements**: A computer capable of running the Unity Editor is sufficient to develop and test the game.
- **Target Platform**: The game targets Android devices, specifically Android API 22 (Lollipop) and above.

### Testing the Game

- **Local Testing**: The game can be tested locally by using the play button in the Unity Editor.
- **Android Device Testing**: For testing on real Android devices, refer to Unity's official documentation on the [Android Build Process](https://docs.unity3d.com/Manual/android-BuildProcess.html).
- **Emulator Testing**: APKs can be built for testing on Android emulators such as NoxPlayer or Bluestacks.

## Contributing

1. Fork the Project: `git fork https://github.com/PLR2388/JDGMobileGame`
2. Create a Feature Branch: `git checkout -b feature/amazing-feature`
3. Commit Changes: `git commit -m 'Add Amazing Feature'`
4. Push to the Branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

### Contribution Guidelines

#### Code Contributions
1. **Coding Standards**: Please adhere to the [Default code style from Rider 2023.2](#link-to-style-guide). This helps maintain a consistent codebase and facilitates collaboration.
2. **Pull Requests**: Use the provided [pull request template](#link-to-pull-request-template) for your contributions. Ensure your code is well-documented and includes comments for complex logic.
3. **Testing**: None are currently available and we strongly encourage you to help us develop them.

#### Reporting Issues
- **Open Issues Anytime**: Feel free to open an issue at any point if you encounter a bug, have a feature request, or have a suggestion for improvement. Provide as much detail as possible in the issue to facilitate effective communication.

#### General Guidelines
- **Stay Updated**: Regularly pull the latest changes from the main branch to keep your local repository up-to-date.
- **Collaborate and Communicate**: Engage with the community on [JDG Mobile Game Discord](https://discord.gg/ujgVYD3e5Q). If you have questions or need help, don't hesitate to ask.
- **Respect and Courtesy**: Treat all community members with respect. Constructive feedback and discussions are always encouraged.
- **Documentation**: If your changes are significant or might not be self-explanatory, update the relevant documentation or README files.

By following these guidelines, you help maintain the quality and cohesion of the JDG - Trading Card Game Mobile project. We look forward to your contributions and are excited to see how you can help enhance this gaming experience!

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

About assets, Joueur du Grenier team asked to keep the game free of charge and without any monetization in order to have the right to use them.

## Acknowledgments

First of all, I'd like to thank the team at Joueur du Grenier for offering such a great game and giving me the opportunity to develop this game with their assets.

I'd like to thank all the participants of the [JDG Mobile Game Discord](https://discord.gg/ujgVYD3e5Q) who motivate me and contribute to the project.
To name some of them:
- Sir_Vladimir, who designed the new logo and card
- Py MARREC, who made some sound effects

And also a big praise to all the new contributors I don't know yet!
