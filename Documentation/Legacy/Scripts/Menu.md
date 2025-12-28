# Menu

## CardChoice

`CardChoice` is a Unity class that manages card selections and choices within a game menu.

### Features:
- **Card Selection**: Keeps track of a player's card selection. Notifies if player one has chosen their cards.
- **Card Verification**: Checks the chosen cards of a player, and if the chosen cards meet certain criteria, updates the game state and navigates to the appropriate game screen.
- **Random Decks**: Provides functionality to create random decks for players, ensuring diversified card choices.
- **Test Deck**: Sets a deck specifically for testing purposes which then proceeds to start the game.
- **Back Action**: Manages the back action within the game menu, ensuring the UI reflects the player's current stage of card choice.
- **Filtering**: Filters out certain cards based on predefined criteria.
- **Event Notification**: Uses Unity events to notify other systems when a player's card choice changes.

### Dependencies:
- **Cards Namespace**: Uses card objects and related functionality.
- **Sound Namespace**: Interacts with the audio system for game sound management.
- **Menu Namespace**: Contains the UI manager for the card choice.

This class interacts heavily with the `CardChoiceUIManager` for displaying messages, updating titles, and button texts based on the user's actions and the current state of card selections.

## CardChoiceUIManager

`CardChoiceUIManager` is responsible for handling the UI elements related to card selection within the game's menu. Key functionalities include:

- **UI Components**: Holds references to various UI elements, such as:
  - Main container and canvas.
  - Specific menus for card choices and two-player mode.
  - Text components for title and button descriptions.

- **Player-based UI Updates**: Contains the capability to update the title and button texts based on the current player (either player one or player two), utilizing localized keys for multi-language support.

- **Menu Visibility Control**: Offers methods to show or hide both the card choice menu and the two-player mode menu.

- **Message Box Display**: Can display a message box to the user, informing about the number of cards remaining to be chosen, with localized titles and messages.

It extends the `StaticInstance<CardChoiceUIManager>` class, ensuring that there is only one instance of this manager in the game. This script is designed to be part of the game's menu namespace.

## CardEvent

**CardEvent** provides a suite of classes encapsulating in-game card-related events as Unity events. Each class represents a specific type of card event, making it easy to trigger and handle interactions associated with different card types in a Unity-based card game. Here's a quick breakdown:

1. **CardEvent**: Represents generic in-game card events.
2. **InvocationCardEvent**: Specifically for in-game invocation card events.
3. **FieldCardEvent**: Represents events related to in-game field cards.
4. **EffectCardEvent**: Handles events for in-game effect cards.
5. **EquipmentCardEvent**: Dedicated to events concerning in-game equipment cards.

These classes are marked as serializable, allowing them to be used seamlessly within the Unity editor and ensuring a clean and organized way to manage card events within the game environment.

## CardState

**CardState** is an abstract class tailored to represent various states of a card within a Unity-based card game. By structuring card behaviors according to their states, this system offers a clean and modular approach to handle card interactions and visualize their conditions. Here's a brief overview:

1. **CardState (Abstract Base Class)**:
    - Represents a foundational card state from which specific states derive.
    - Contains core properties like context (linked to the `OnHover` script) and the associated `InGameCard`.
    - Provides abstract methods such as `EnterState()` and `OnClick()` that must be overridden by derived states.

2. **DefaultCardState**:
    - Represents the initial or default state of a card.
    - On entry, the card's color changes based on whether it's a collector card or not.
    - When clicked, the card transitions to its "selected" state.

3. **SelectedCardState**:
    - Represents the state when a card is actively selected.
    - On entry, the card's color turns green and the card is registered as selected.
    - Clicking a selected card reverts it back to its default state and deregisters the card as selected.

4. **NumberCardState**:
    - Represents a state where a specific number is showcased on the card.
    - On entry, the card's color is green, and a number is displayed.
    - Upon clicking, the card reverts to its default state, hiding the number and deselecting the card if needed.

By utilizing this state pattern, card interactions are effectively modularized, ensuring efficient event handling and a cohesive visual feedback system for the player.

## HandCardDisplay

**HandCardDisplay** is a Unity MonoBehaviour component responsible for visually representing a player's hand cards in the game. This system prioritizes modularity, offering clean and event-driven solutions to visualize and interact with player hand cards. Here's a brief rundown:

1. **Core Components**:
   - `prefabCard`: A serialized field that holds the prefab to be used for instantiating in-game cards.
   - `CreatedCards`: A list that keeps track of all the card GameObjects created and displayed on the screen.

2. **HandCardChangeEvent**:
   - A UnityEvent that triggers when there's a change in the collection of hand cards. This event enables dynamic adjustments based on card updates.

3. **Primary Features**:
   - **Dynamic Display**: Automatically visualizes cards in hand whenever they're updated, ensuring real-time synchronization with the game state.
   - **Context Awareness**: Only displays hand cards if they belong to the current player.
   - **Visual Card Creation**: Instantiates visual card representations from the provided prefab and sets them up according to the in-game card details.
   - **RectTransform Adjustments**: Dynamically adjusts the RectTransform size based on the number of cards in hand, ensuring a consistent layout.
   - **Event-Driven Behavior**: Uses Unity's event system to react to card changes and lifecycle events efficiently.

4. **Lifecycle Management**:
   - `Awake()`, `OnEnable()`, and `OnDisable()`: Handles the subscription and unsubscription of events, ensuring that the component stays updated and avoids potential memory leaks or stale references.

5. **Helper Methods**:
   - `DisplayHandCard()`: Checks the current player's turn and decides whether to display the hand cards.
   - `IsCurrentPlayerTurn()`: Determines if a card belongs to the current player.
   - `BuildCards()`, `CreateCards()`, `ClearCreatedCards()`: Manages the creation, setup, and destruction of visual card representations.

By using `HandCardDisplay`, developers can maintain a seamless card display system, ensuring players always have a coherent and updated view of their in-game hand cards.

## InGameMenuScript

`InGameMenuScript` is a Unity MonoBehaviour script responsible for managing in-game card interactions, event handling, and displaying related UI elements. Below is a breakdown of its functionality:

- **UI Components**: The script maintains references to various UI components, including card details, mini menus, hand displays, and related text/buttons.

- **Selected Card**: Keeps track of the currently selected in-game card for interaction.

- **Card Events**: Static events are defined to handle various card interactions, like clicks, invocations, and card placements on the field.

- **Card Handlers**: A dictionary (`CardHandlerMap`) maps different card types to their respective handlers, making it easy to define specific behaviors for each card type.

- **Initialization**: On start, the script initializes the UI states and card handlers.

- **Card Click Handling**: On a card click event, the script fetches the card's type and uses its respective handler to process the card interaction. It also displays a mini-menu near the clicked card's position.

- **Put Card Action**: Defines the behavior when a card is placed or 'put' on the field or elsewhere.

- **Card Detail View**: Provides the ability to toggle between detailed card views and a broader hand view.

- **Hand Display**: Enables toggling the hand card display, allowing users to view or hide their current hand of cards.

The script employs a modular approach by utilizing card handlers for different card types. This ensures that specific behaviors and interactions are encapsulated for each card type, promoting clarity and maintainability.

## MainMenuAction

`MainMenuAction` is a Unity MonoBehaviour script focused on controlling sound and music playback within the main menu of a game or application. Here's a quick overview:

- **Initialization**: Upon loading (`Awake` method), the script automatically plays the main theme music for the menu.

- **Main Theme**: Provides a method (`PlayMainTheme`) to play the primary theme music for the main menu.

- **Section-Specific Themes**: It offers dedicated methods to play music tailored for specific sections of the main menu:
  - **One Player Section**: Plays music tailored for the one-player menu (`PlayOnePlayerMenuMusic`).
  - **Two Player Section**: Plays music for the two-player menu (`PlayTwoPlayerMenuMusic`).
  - **Options Section**: Delivers music for the options/settings menu (`PlayOptionMenuMusic`).

- **Sound Effects**: In addition to music themes, the script has methods to play sound effects that enhance the user experience:
  - **Transition Sound**: Plays a sound effect when transitioning between menu sections or options (`PlayTransitionSound`).
  - **Back Navigation Sound**: Plays a sound effect when navigating back to a previous menu or option (`PlayBackSound`).

The class interacts extensively with the `AudioSystem` instance to manage the playback of both music and sound effects, ensuring a cohesive and immersive audio experience for users navigating the main menu.

## OnHover

The `OnHover` class is a Unity MonoBehaviour script designed to handle interactivity for in-game cards, specifically addressing hover and click events, as well as card states.

Here's a quick overview:

- **Card Interaction**: The script is equipped to handle mouse pointer interactions with the card, including pointer entering, exiting, and clicking on the card.

- **Card States**: The `OnHover` class uses a state pattern. Cards can be in different states, such as `DefaultCardState` or `NumberCardState`. The card's behavior and appearance change depending on its current state.

- **Card Selection**: Cards can be selected and deselected. There's an event listener that listens to deselection events and updates the card state accordingly.

- **Number Display**: Each card can display a number. Methods are available to update, show, and hide this number. The number is linked with the `NumberedCardEvent` event, which is responsible for updating the number displayed on the card.

- **Image Color Handling**: The script can modify the color of the card's image, allowing for visual feedback or indicating different card states.

- **Event Handling**:
  - `OnPointerEnter` and `OnPointerExit` are empty methods for potential future implementations related to mouse hover events.
  - `OnPointerClick` handles click events on the card, invoking specific actions depending on the card's in-game status and current state.

- **Components and Dependencies**:
  - The class requires an `Image` component to be attached to the same game object.
  - There's a reference to a `numberTextObject` which presumably displays a number on the card.
  - The card's information is stored in an `InGameCard` object.
  - There are references to various other classes and systems, like `CardSelector`, `CardSelectionManager`, and `InGameMenuScript`, indicating that this card system is part of a more extensive game framework.

In essence, the `OnHover` script provides a modular system for card interactions, state management, and display functionalities within a game or application.

## OptionMenu

The `OptionMenu` class, part of the `Menu` namespace, is a Unity MonoBehaviour script designed to manage the audio settings of a game or application, specifically focusing on music and sound effects volumes.

Here's a brief summary:

- **Volume Sliders**: The script incorporates two Unity UI sliders:
  - `volumeMusicSlider`: Controls the music volume.
  - `soundEffectSlider`: Controls the volume of sound effects.

- **Initialization**:
  - The `Start` method invokes the `InitializeSliders` function at the beginning of the object's lifecycle.
  - `InitializeSliders` sets the starting values of the sliders to match the current volume settings in the `AudioSystem`. It also attaches event listeners to detect and respond to changes in the slider values.

- **Volume Control**:
  - `MusicVolumeChanged(float value)`: Triggered when the music volume slider is adjusted. It updates the music volume in the `AudioSystem`.
  - `SoundEffectVolumeChanged(float value)`: Activated when the sound effects volume slider is changed. It updates the sound effects volume in the `AudioSystem`.

In essence, the `OptionMenu` script offers an interactive interface for users to adjust audio settings, ensuring that changes are immediately reflected in the game's audio system.

## TutoHandCardDisplay

`TutoHandCardDisplay` is a specialized class for managing and displaying in-game cards during a tutorial scenario in Unity. It is an extension of the `HandCardDisplay` class.

### Key Features:

- **Event Handling**: Subscribes to events when enabled and unsubscribes when disabled or destroyed to maintain clean lifecycle management.

- **Card Highlighting**: Has the ability to highlight specific in-game cards based on the current dialog index, specifically emphasizing the "MusiqueDeMegaDrive" card when certain dialog conditions are met.

- **Card Creation**: Dynamically creates visual representations for a collection of in-game cards, with an option to highlight specific cards based on their title and current dialog conditions.

- **Display Conditions**: Only displays the hand cards if certain conditions are met, such as the hand being empty or it being the current player's turn.

- **Rebuild Mechanism**: Provides functionality to clear the existing card representations and recreate them based on the provided cards.

### Methods Summary:

- `OnEnable()`: Subscribes to the necessary events.

- `OnDisable()` & `OnDestroy()`: Performs cleanup operations, like clearing created cards and unsubscribing from events.

- `SubscribeToEvents()` & `UnsubscribeFromEvents()`: Handles event subscriptions and unsubscriptions for card display updates.

- `UpdateCurrentDialogIndex(int index)`: Updates the current dialog index and checks card highlighting conditions.

- `HighlightCardOnDialogChange(int index)`: Highlights a card based on the dialog index.

- `ShouldHighlightCard(InGameCard handCard)`: Determines if a specific card should be highlighted.

- `CreateCards(ObservableCollection<InGameCard> handCards)`: Instantiates visual representations for the provided in-game cards.

- `RebuildHandDisplay(ObservableCollection<InGameCard> handCards)`: Clears any existing card representations and recreates them.

- `ShouldDisplayHandCard(ObservableCollection<InGameCard> handCards)`: Determines if the hand cards should be displayed.

- `DisplayHandCard(ObservableCollection<InGameCard> handCards)`: Displays the hand cards based on certain conditions.

This class ensures that in-game cards are displayed correctly during tutorial scenarios, providing users with visual cues and guidance as they progress through the tutorial.

## TutoInGameMenuScript

`TutoInGameMenuScript` represents the tutorial version of the in-game menu in a card game, with specific behaviors to guide new players. Here's a brief overview:

- **Initialization**: Caches component references like `TextMeshProUGUI`, `Button`, and custom `HighLightButton` components during the `Awake` method. Sets the initial state of the UI, including deactivating mini-menu cards and initializing card handlers.

- **Card Click Handling**: When a card is clicked, the script checks if it matches certain criteria based on the current dialogue index. If it does, it handles the card (e.g., shows a mini-menu) and potentially highlights certain actions for the player.

- **Card Actions**: Contains logic to handle the "Put Card" action where it triggers events based on the card's type. Additionally, it allows players to toggle between showing and hiding their hand cards, updating the interface accordingly.

- **UI Updates**: The script manages various UI elements, such as:
  - Toggling the visibility of the hand cards.
  - Updating button texts based on localization keys.
  - Activating or deactivating button interactivity and highlighting.

- **Visibility Management**: Offers methods to display or hide hand cards and update relevant button texts. Depending on the game's state, it might highlight certain actions, prompt the next phase, or continue the tutorial dialogue.

This script is an extension of the `InGameMenuScript`, specializing it for a tutorial context.
