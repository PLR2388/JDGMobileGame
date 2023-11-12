# Systems

## Generated

### LocalizationKeys

The `LocalizationKeys` enumeration provides a comprehensive list of keys to facilitate game localization. These keys encompass various game actions, titles, button names, card-related prompts, family classifications, game phases, informational and warning messages, questions, and types. This structure ensures that every in-game text can be easily translated or customized across different languages or regions, promoting a more globally accessible gaming experience.

### LocalizationKeyStrings

`LocalizationKeyStrings` is a static class in C# that defines a dictionary, `KeyMappings`, which maps enum values of type `LocalizationKeys` to their corresponding string representations. These string representations are intended to be used as keys for fetching localized text in various languages, aiding in the internationalization and localization of an application.

The keys cover a wide range of application aspects including actions, button labels, card choices, families, game phases, questions, warnings, and more. This makes it easier for developers to manage and reference localization keys in a consistent manner throughout the application.

By using this dictionary, developers can ensure that they are using consistent key strings across different parts of the application, reducing the risk of errors or missing translations.

To use a key, simply reference the dictionary with the desired `LocalizationKeys` enum value:

```csharp
string localizedKey = LocalizationKeyStrings.KeyMappings[LocalizationKeys.ACTION_CONFIRM_TITLE];
```

Remember to set up the appropriate localized translations for each key in your localization system to retrieve the correct text for the end user's language.

## AudioSystem

### Overview
`AudioSystem` is a Unity class designed to manage the playback and control of audio within the game. It handles both music and sound effects, ensuring seamless audio experiences for players.

### Features
1. **Dynamic Audio Management**: Store and manage multiple music tracks and sound effects using serialized fields.
2. **Volume Control**: Adjust and retrieve the volume level for both music and sound effects separately.
3. **Music Indexing**: Easily find and play a specific music track by retrieving its index from the array.
4. **Sound Effect Indexing**: Quickly find and play a specific sound effect by retrieving its index from the array.
5. **Family Specific Music**: Assign and play music tracks associated with different card families.
6. **Easy Audio Control**: Convenient methods to play, stop, and change the volume of your audio assets.
7. **Transition and Back Sound Effects**: Predefined methods to play common game sound effects such as transitioning between scenes or going back.

## LocalizationSystem

### Overview
`LocalizationSystem` is a class designed to provide an efficient and easy way to manage and retrieve localized text resources for your game or application. Based on a provided language, it ensures that the content is tailored to the linguistic preferences of the end-user, enhancing the user experience.

### Features
1. **Automated Initialization**: On start, the system loads the default language localization data.
2. **Dynamic Data Loading**: Ability to load different localized text data from files.
3. **JSON Parsing**: Processes JSON formatted localized data and converts it into a usable dictionary format for efficient retrieval.
4. **Nested JSON Support**: Handles nested JSON objects for structured localization data.
5. **Text Retrieval by Key**: Retrieve localized text using a predefined key, ensuring flexibility and robustness in the design.
6. **Error Handling**: In case a localization file is missing or a text entry is not found, appropriate error logs and default strings are provided for seamless user experience.

## ResourceSystem

### Overview
`ResourceSystem` is a specialized management class tailored for handling card resources in a game or application. The system ensures that all card-related assets are efficiently loaded, organized, and readily accessible for quick retrieval.

### Features
1. **Initialization at Awake**: The system auto-initializes upon the script instance being loaded, ensuring all resources are assembled and ready before any gameplay.
2. **Efficient Resource Loading**: Loads all card resources dynamically from Unity's built-in resource system.
3. **Resource Organization**: Maintains a list of all loaded cards and also organizes them into a dictionary for rapid lookup based on card titles.
4. **Quick Card Retrieval**: Provides a method to fetch a card's details instantly using its title.

## SceneLoaderSystem

### Overview
`SceneLoaderSystem` provides a streamlined system for handling and transitioning between different game scenes. This system ensures that scenes are loaded efficiently, and provides functionalities to shift between main screens, preload screens, and game screens.

### Key Features
1. **Preload Scene Transition**: If the currently active scene is set to the preload scene (`_preload`), the system automatically transitions to the main screen upon initialization.
2. **Centralized Scene Naming**: Contains a centralized storage for scene names, making it easier to reference and manage scene names without hard-coding them in multiple places.
3. **Efficient Asynchronous Loading**: Scenes are loaded asynchronously, ensuring smooth transitions without causing freezes or hitches.
4. **Versatile Scene Loading Methods**:
   - `LoadMainScreen()`: Loads the main screen scene.
   - `LoadGameScreen()`: Loads the game screen scene for local 2-player gameplay.

## Systems

### Overview
The `Systems` class serves as a central and persistent main object that remains active throughout the game's runtime. Designed for organization and simplicity, it acts as a parent to multiple sub-systems, ensuring they persist across various scenes and transitions without being destroyed.

### Key Features
1. **Persistent Nature**: Built upon the `PersistentSingleton` pattern, `Systems` ensures that the main object and its child sub-systems remain undestroyed when transitioning between scenes.
2. **Organized Hierarchy**: Aims to maintain an organized hierarchy by having one main object (`Systems`) with various sub-systems as its children. This helps in easily managing and accessing these systems.
3. **Minimalistic Design**: Although minimal in code, it provides a foundational structure for building more complex and layered game systems.
