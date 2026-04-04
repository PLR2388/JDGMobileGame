# Utilities

## EnumGenerator

The `EnumGenerator` class in Unity is designed to automate the creation of enums and their corresponding string mappings based on JSON localization data. When the script instance is loaded via the `Awake` method, it reads a JSON file (specifically the French localization in this case) and parses it to generate enums and mappings. The process is recursive, allowing for the handling of nested JSON objects by building up enum entries and a dictionary that maps these entries to their respective JSON keys. This is done through the `GenerateEnumAndMapping` method, which constructs the enums and mappings as it traverses the JSON structure. Once the enums and mappings are generated, they are saved into .cs files using the `SaveToFile` method. The resulting files, `LocalizationKeys.cs` and `LocalizationKeyStrings.cs`, contain the enums and dictionary mappings that can be used throughout the application to access localized strings by enum reference, ensuring type safety and ease of refactoring within the Unity project.

## StaticInstance

`StaticInstance`, `Singleton`, and `PersistentSingleton` are abstract classes in Unity that manage the instantiation of objects following specific design patterns:

1. `StaticInstance<T>`: Acts as a non-destructive singleton variant where the current instance is overridden by any new instances. This approach is useful for resetting the state without manually cleaning up the existing instance. It provides a static property `Instance` to access the current active instance of the type `T`.

2. `Singleton<T>`: Inherits from `StaticInstance<T>` to implement the classic singleton pattern, ensuring only one instance of the object type `T` exists at a time. If a new instance is created, it is destroyed immediately, maintaining the original instance.

3. `PersistentSingleton<T>`: An extension of `Singleton<T>`, this class maintains the singleton instance across different scenes in Unity, which is ideal for objects that need to persist, such as managers for game state, data, or continuous audio playback.

Each class uses Unity's `Awake` method to establish its instance management rules and `OnApplicationQuit` to perform any necessary cleanup. The `PersistentSingleton<T>` also leverages `DontDestroyOnLoad` to prevent the object from being destroyed when loading a new scene, thus preserving its state throughout the game's runtime. These classes are useful templates for different scenarios where singleton-like behavior is desired in Unity projects.
