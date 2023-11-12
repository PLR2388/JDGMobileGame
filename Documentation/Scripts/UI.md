# UI

## CardDisplay

### InfiniteScroll

#### Overview
The `InfiniteScroll` class is a Unity MonoBehaviour script that facilitates the display and selection of cards in a card-based game. It provides features for displaying available cards, selecting multiple cards, handling card limits, and issuing warnings in case of selection limits.

#### Features
1. **Display Cards:** Allows the user to view all available cards from which they can choose. Some cards, based on certain conditions, are omitted from the display list.
2. **Multiple Card Selection:** Users can select multiple cards up to a predefined limit. This includes keeping track of both general cards and rare cards, with each category having its distinct count limit.
3. **Selection Listeners:** The class uses event listeners to monitor when a card is selected or deselected. It adjusts the counters accordingly.
4. **Limit Check:** It checks if the user goes beyond the set limits for selected cards or rare cards. If a limit is breached, the last selected card is unselected, and a warning message is shown.
5. **Warning Display:** In case a user tries to select cards beyond the allowable limit, a warning message box pops up, informing the user of the violation.
6. **Dynamically Create Card Display:** For each available card, a display is created based on a given prefab.
7. **Player Switching:** It has the ability to reset and switch the card displays based on the player choosing the cards.

#### Dependencies:
1. **Card Types & Card Mappings:** The script references `Card`, `CardNames`, and `CardNameMappings` to fetch and map the card details.
2. **GameState:** Maintains the global state and contains information like the list of all cards for deck1 and deck2.
3. **CardSelectionManager:** Manages the card selection and provides utility functions like `UnselectCard`.
4. **LocalizationSystem:** For fetching localized values for display messages.
5. **MessageBox:** For showing warning dialogs to users.

## GenericUI

### HealthUI

#### Overview
The `HealthUI` class is a Unity MonoBehaviour script designed to manage and display the health status of two players (Player 1 and Player 2) in the user interface. It provides a real-time reflection of each player's health on the UI.

#### Features
1. **Dynamic Health Display:** Utilizes `TextMeshProUGUI` components to display health values for both Player 1 (`healthP1Text`) and Player 2 (`healthP2Text`).
2. **Initialization:** Upon awake, initializes the health UI for both players with the maximum health value from `PlayerStatus`.
3. **Event Listener:** Automatically listens to the `OnHealthChanged` event broadcasted by `PlayerStatus`. When a player's health changes, the UI is updated instantly without manual intervention.
4. **Health Update:** Provides a method to handle health change events (`ChangeHealthText`) and another to set the health text on the UI (`SetHealthText`).
5. **Player Identification:** The health change methods include an `isP1` boolean parameter to determine whether the health update pertains to Player 1 or Player 2.
6. **Clean Up:** Ensures that the health change listener is unregistered when the `HealthUI` object is destroyed to prevent any memory leaks or unintended behavior.

#### Dependencies:
1. **TextMeshPro:** Used to display the health values on the UI.
2. **PlayerStatus:** Holds the maximum health value and broadcasts health change events. (Note: The full implementation of `PlayerStatus` is not shown in the provided code.)

#### Important Note:
If there are changes to the `PlayerStatus` class or the manner in which health updates are broadcasted, ensure `HealthUI` is reviewed and updated accordingly to maintain proper functionality.

### InvocationMenuManager

#### Overview
The `InvocationMenuManager` class is a Unity MonoBehaviour script responsible for the management of the invocation menu in the game, including controlling the states and visibility of buttons contained within this menu.

#### Features
1. **Singleton Pattern:** Utilizes the `StaticInstance` to ensure a single instance of the `InvocationMenuManager` throughout the game.
2. **Initialization:** During the Awake phase, this script caches references to the `attackButton` and `actionButton` inside the invocation menu for efficient access.
3. **Dynamic Menu Display:** The menu can be displayed or hidden with the `Display()` and `Hide()` methods, respectively. The `Display()` method further allows the positioning of the menu based on the mouse (or touch) position.
4. **Button State Management:** Manages the interactability of buttons based on various game conditions, for example:
   - `UpdateAttackButton()` checks if the attacker can attack and sets the attack button state accordingly.
   - `Display()` method checks whether the game is in the attack phase, whether the attacker has actions available, and whether special actions are possible to determine the states of both the `attackButton` and `actionButton`.
   - `Enable()` method sets the attack button to an interactable state.
5. **Integration with CardManager:** Determines button states based on conditions and methods provided by the `CardManager`, such as `CanAttackerAttack()`, `HasAttackerAction()`, and `IsSpecialActionPossible()`.

#### Dependencies:
1. **Unity Engine:** Utilized for core game functionality including UI components and GameObject management.
2. **CardManager:** Required to check various game states and conditions for button interactability.
3. **InputManager:** Provides the current touch/mouse position for the `Display()` method.

### RoundDisplayManager

#### Overview
The `RoundDisplayManager` class is a Unity MonoBehaviour script designed to oversee the display elements related to game rounds. It handles components such as the textual representation of the round, player turn indicators, and specific camera orientations for different phases of the game.

#### Features
1. **Singleton Pattern:** Adopts the `StaticInstance` design to ensure there's only one instance of the `RoundDisplayManager` active during gameplay.
2. **UI Component References:** Holds references to various user interface elements, such as:
   - `playerText`: Displays which player's turn it currently is.
   - `roundText`: Shows the current game round or phase.
   - `playerCamera`: The in-game camera which can be rotated during specific game phases.
   - `inHandButton`: A button or UI element shown during particular game phases.
3. **Round Text Display:** The `SetRoundText()` method enables updating the text displayed for the game round.
4. **UI Adaptation:** The `AdaptUIToPhaseIdInNextRound()` method adjusts the UI elements based on the upcoming game phase. This includes:
   - Activating the `inHandButton` during the end phase.
   - Setting the text for the round based on the game phase (e.g., DRAW or ATTACK).
   - Rotating the camera for the end phase, if necessary.
5. **Camera Orientation:** The `RotateCameraForEndPhase()` method rotates the player's camera during the end phase of the game, offering a fresh perspective.
6. **Player Turn Indication:** The `SetPlayerTurnText()` method provides visual feedback about whose turn it currently is, using localized strings.

#### Dependencies:
1. **Unity Engine:** Used for fundamental game functionality, including managing GameObjects and UI components.
2. **TMPro:** Ensures aesthetically-pleasing and consistent text display across the game UI.
3. **GameStateManager:** Used to fetch the current game phase and determine which player's turn it is.
4. **LocalizationSystem:** Retrieves localized strings for various game elements and phases, ensuring compatibility across different languages and regions.

## SceneLoader

### Overview
The `SceneLoader` class provides utilities for scene navigation and application management within Unity. It includes methods for transitioning to specific game scenes, quitting the application, and interacting with users via toast notifications, particularly in the Android environment.

### Features

1. **Quitting the Game:**
   - `QuitGame()`: Terminates the game application. For debugging purposes, a log is printed in the Unity editor when this method is called.

2. **Navigating to Tutorial Scene:**
   - `GoToTutorial()`: Transitions the player to a predefined tutorial scene. It prepares the game state and stops any ongoing music before loading the tutorial.

3. **Handling Story Button Click:**
   - `OnClickStory()`: Invoked upon clicking the story button. It displays a toast message to the user, which may be a prompt or a related message.

4. **Toast Notifications:**
   - `ShowAndroidToastMessage(string message)`: Displays a toast notification to the user. The behavior of this method differs based on the platform:
     - In Unity Editor: The provided message is logged.
     - On Android: The message is shown as a native Android toast.

5. **Android Toast Display (Android Only):**
   - `DisplayAndroidToast(string message)`: Utilizes Android's Java interface to generate and display a native toast notification with the provided message.

### Dependencies:
1. **Unity Engine:** This script harnesses Unity's basic functionalities and scene management system.
2. **AudioSystem:** Used to manage the game's audio, such as halting music playback when transitioning to the tutorial.
3. **GameState:** Provides utilities to set up specific game states, such as building a deck for the tutorial.
4. **LocalizationSystem:** Assists in fetching localized strings, making the game more accessible to users of different languages and regions.

### Notes:
- When targeting platforms other than Android, ensure the platform-specific code for Android is handled appropriately, as demonstrated in the `#if UNITY_ANDROID` directive.
- Developers may want to expand upon this basic scene loader by adding transitions to more scenes or introducing more intricate toast messages or notifications as the game evolves.

## UpdateDescription

### Overview
The `UpdateDescription` class, attached to a Unity GameObject, manages the updating and visualization of details related to different card types, primarily Invocation cards and other card types. It primarily focuses on displaying card attributes such as title, description, detailed description, family, attack, defense, and collector image.

### Features

1. **Card Component Retrieval:**
   - Initializes and retrieves the `CardDisplay` component during the script instance loading using the `Awake()` method.

2. **Invocation Card Details Update:**
   - `UpdateInvocationCardDetails()`: Displays specific details for Invocation cards, including family, attack, and defense. It also sets the card type text to "Invocation".

3. **Non-Invocation Card Details Update:**
   - `UpdateOtherCardDetails()`: Handles the display of details for card types other than Invocation, deactivating the Invocation-specific options and updating the card type text based on the card's type.

4. **Collector Image Visibility Update:**
   - `UpdateCollectorImageStatus()`: Toggles the visibility of a special collector image based on the card's collector property.

5. **Continuous Card Update:**
   - During each frame in the `Update()` method, the class checks if there's been a change in the displayed card. If a change is detected, it updates all the necessary card details accordingly. This ensures that the displayed card details are always current and reflect the most recent card data.

### Dependencies:
1. **Unity Engine:** The script leverages various Unity functionalities, including the GameObject and MonoBehaviour features.
2. **TMPro:** Uses the TextMeshProUGUI component to display card-related texts.
3. **Cards Namespace:** Contains definitions and implementations for different card types, including `Card`, `InvocationCard`, and `CardDisplay`.
4. **LocalizationSystem:** Used for fetching localized string values to support multiple languages.

### Notes:
- Developers can further enhance this class by adding more features or optimizing the `Update()` method for performance. It's essential to ensure that the card details update efficiently.
- If expanding the game with additional card types or attributes, remember to update the `UpdateDescription` class to handle and display those new details appropriately.
