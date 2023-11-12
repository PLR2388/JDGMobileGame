# One Player

## DialogueBox

### DialogueObject

**Purpose:**
`DialogueObject` is a Unity scriptable object that defines and manages dialogue configurations. It facilitates game storytelling by associating text dialogues with various gameplay elements such as triggers, responses, and audio cues.

**Features & Attributes:**
- **Dialogue:** An array of text lines representing the conversation or monologue.
- **Next Dialogue Triggers:** A sequence of triggers that determine how to progress to subsequent dialogues.
- **Responses:** Array of possible player responses within the dialogue, facilitating interactive conversations.
- **Audio Clips:** Accompanying audio files can be linked with specific dialogue lines for a more immersive experience.
- **Sound Dialogue Index:** Specifies the correspondence between dialogue lines and their associated audio clips.

### DialogueUI

**Purpose:**
The `DialogueUI` class is responsible for managing the game's dialogue user interface (UI). It dictates how dialogue text and player responses are presented on the screen, ensuring seamless interaction between the player and game narratives.

**Features & Attributes:**
- **Dialogue Box & Text Label:** Components for displaying dialogue content.
- **Events:** Utilizes UnityEvents to broadcast the current dialogue index and trigger execution.
- **Sound Integration:** Aligns spoken audio with the corresponding text dialogue.
- **Response Handling:** Incorporates player's responses within the dialogue flow.
- **Typewriter Effect:** Simulates the effect of text appearing gradually.
- **Trigger Management:** Dictates the progression flow of dialogues based on specific in-game triggers.

**Functionality:**
1. **Initialization:** On start, the dialogue box is closed, default settings are initialized, and listeners are added.
2. **Dialogue Presentation:** Steps through each line of dialogue, invoking associated events and playing corresponding audio.
3. **Trigger Handling:** Waits for specified triggers (e.g., tap, card placement) before progressing to the next dialogue.
4. **Audio Management:** Plays audio clips related to the current dialogue and adapts the speed of the typewriter effect to the audio clip's duration.
5. **Response Integration:** Displays possible player responses at the end of dialogues and handles user interaction.

### NextDialogueTrigger

**Purpose:**
The `NextDialogueTrigger` enum, within the `OnePlayer.DialogueBox` namespace, represents the diverse set of triggers that can initiate the display of the subsequent dialogue in a sequence.

**Triggers Enumerated:**
- **Tap:** A user input, typically a screen touch or mouse click.
- **Automatic:** The next dialogue is triggered automatically without any user intervention.
- **PutCard:** Triggered when a card is placed in the game.
- **NextPhase:** Initiates dialogue as the game progresses to the subsequent phase.
- **PutEffectCard:** Triggered specifically when an effect card is used or placed.
- **Undefined:** Represents an ambiguous or undefined state.
- **Attack:** Initiated when an attack action is performed in the game.
- **EndVideo:** Triggers the next dialogue at the conclusion of a video segment.
- **EndGame:** Initiates dialogue when the game reaches its endpoint.

### Response

**Purpose:**
The `Response` class, designed for use in a dialogue system, represents a potential player's reply or choice during a conversation.

**Key Features:**
1. **ResponseText:**
   - **Description:** The actual text of the player's response.
   - **Access:** Read-only property providing access to the `responseText` field.

2. **DialogueObject:**
   - **Description:** An associated dialogue sequence or content that is triggered upon choosing this particular response.
   - **Access:** Read-only property providing access to the `dialogueObject` field.

### ResponseHandler

**Purpose:**
The `ResponseHandler` class manages the visualization and interaction of potential player responses during a dialogue session in the game.

**Key Features:**

1. **Response Box & Templates:**
   - Holds UI references to the container (`responseBox`) for displaying responses, the UI button template (`responseButtonTemplate`) for individual responses, and the parent container (`responseContainer`) for these buttons.

2. **Initialization:**
   - On startup (`Start` method), the class initializes by obtaining the required `DialogueUI` component for future use.

3. **Display Responses:**
   - The `ShowResponses` method dynamically creates and displays buttons for all available player responses. It also automatically adjusts the height of the response box based on the number of responses.

4. **Response Button Creation:**
   - Using the `CreateResponseButton` method, a button is instantiated for each player response. This button, when clicked, triggers the continuation of the dialogue with the selected response.

5. **Handling Player's Selection:**
   - The `OnPickedResponse` method is called when a player selects a response, clearing the response box and continuing the dialogue based on the player's choice.

6. **Cleanup:**
   - The `ClearResponseBox` method hides the response box and cleans up any active response buttons from the scene.

### TypewriterEffect

**Purpose:**
The `TypewriterEffect` class is responsible for applying a progressive typing effect to the provided text, simulating a typewriter's behavior, where characters are displayed one by one at a specified speed.

**Key Features:**

1. **Typing Speed:**
   - The typing speed can be customized through the `defaultTypewriterSpeed` serialized field.
   - At initialization (`Start` method), the current typing speed is set to the default value.

2. **Adaptive Typing Speed:**
   - The `AdaptSpeedToLength` method adjusts the typing speed according to the desired duration and length of the text. This ensures that the entire text gets typed out within the given time frame.

3. **Typewriter Execution:**
   - The `Run` method starts the typewriter effect on the provided text and label. If the label is null, an error is logged.
   - This method uses the `TypeText` coroutine to type out the text.

4. **Coroutine Implementation:**
   - `TypeText` is the coroutine that handles the effect, progressively updating the `TMP_Text` label to display the text character by character at the current typing speed.

## DialogueTutoHandler

**Purpose:**
The `DialogueTutoHandler` class is designed to manage and store the current index of the dialogue tutorial. It extends the `StaticInstance` generic class, implying that it follows a Singleton pattern, ensuring that only one instance of this class exists during runtime.

**Key Features:**

1. **Current Dialogue Index:**
   - A public property, `CurrentDialogIndex`, keeps track of the current position or index within the tutorial dialogue.

2. **Initialization and Event Subscription:**
   - In the `Awake` method, the class subscribes to the `DialogIndex` event from `DialogueUI` to track changes to the dialogue index.

3. **Event Unsubscription:**
   - The `OnDestroy` method ensures that the handler unsubscribes from the `DialogIndex` event to avoid potential memory leaks or unintended behavior.

4. **Updating Dialogue Index:**
   - The `SaveDialogIndex` method updates the `CurrentDialogIndex` based on the index passed in, ensuring that the handler always reflects the current position within the tutorial dialogue.

## HighLightButton

**Purpose:**
The `HighLightButton` class provides functionality to dynamically highlight a button based on specific criteria and conditions.

**Key Features:**

1. **Configuration Options:**
   - `element`: Represents the associated element for the button.
   - `pulseColor`: Defines the color to be used when the button is pulsing (default is green).
   - `defaultColor`: Specifies the default color for the button (default is white).
   - `pulseDuration`: Dictates the duration (in seconds) of a single pulse effect on the button.

2. **Highlight Activation Status:**
   - The public boolean `isActivated` serves as a flag to determine whether the button should be highlighted.

3. **Component References:**
   - On initialization (`Awake` method), the script references both the `Button` and `Image` components of the GameObject it's attached to.

4. **Event Subscription:**
   - The `Start` method subscribes the `UpdateStatus` function to the `Highlight` event of `HighLightPlane`. This ensures that the button's highlighting status can be updated based on events from the `HighLightPlane`.

5. **Dynamic Highlighting:**
   - The `Update` method checks the `isActivated` status every frame:
     - If activated, the button becomes interactable and starts a pulsing effect.
     - If not activated, the button color resets to white.

6. **Pulsing Effect:**
   - The `PulseCoroutine` gives the button a dynamic pulsing effect. This is achieved by changing the button's color between the default and pulse colors based on the configured `pulseDuration`.

## HighLightCard

**Purpose:**
The `HighLightCard` class offers the functionality to highlight a card object in the UI dynamically based on specific criteria and conditions.

**Key Features:**

1. **Configuration Options:**
   - `element`: The type of card element that can be highlighted.
   - `pulseColor`: Specifies the color used when the card is pulsing (default is green).
   - `defaultColor`: Designates the default card color when not highlighted (default is clear/transparent).
   - `pulseDuration`: Sets the duration (in seconds) for each pulse in the highlighting effect.

2. **Highlight Activation Status:**
   - The boolean `isActivated` is a flag indicating whether the card's highlighting effect is active or not.

3. **Component References:**
   - The `Awake` method initializes a reference to the card's `Image` component for efficient color manipulation.

4. **Event Subscription and Clean-Up:**
   - Upon start (`Start` method), the class subscribes the `UpdateStatus` method to the `Highlight` event from `HighLightPlane`.
   - Before destruction (`OnDestroy` method), it ensures to unsubscribe from the event to prevent potential memory leaks.

5. **Dynamic Highlighting:**
   - The `Update` method reviews the `isActivated` status every frame:
     - If activated, the card exhibits a pulsing effect.
     - Otherwise, the card's color reverts to its default (clear).

6. **Pulsing Effect:**
   - The `PulseCoroutine` is responsible for imparting a pulsing effect to the card. This is achieved by alternating the card's color between its default and pulse colors, based on the predefined `pulseDuration`.

## HighLightPhysicalCard

**Purpose:**
The `HighLightPhysicalCard` class is designed to highlight a physical card within the game, giving developers a tool to visually emphasize particular card elements dynamically.

**Key Features:**

1. **Configuration Options:**
   - `element`: Specifies the type of card element eligible for highlighting.
   - `pulseColor`: Designates the color displayed when the card pulses (default is green).
   - `defaultColor`: Sets the default card color when not in a highlighted state (default is white).
   - `pulseDuration`: Configures the duration (in seconds) for each pulse effect.

2. **Highlighting Specific Card:**
   - Uses the static readonly `TargetCardName` to determine a specific card (in this case, "Tentacules") that should be affected by the highlight logic.

3. **Highlight Activation Status:**
   - Uses the boolean `isActivated` to indicate if the highlighting effect is currently active for the card.

4. **Component References:**
   - The `Awake` method initializes references for:
     - `meshRenderer`: To modify the card's visual appearance.
     - `cardDisplay`: To access details about the physical card.

5. **Event Subscription and Clean-Up:**
   - Upon start (`Start` method), it subscribes the `UpdateStatus` method to the `Highlight` event from `HighLightPlane`.
   - Before destruction (`OnDestroy` method), it ensures to unsubscribe from the event to mitigate potential memory leaks.

6. **Dynamic Highlighting:**
   - The `Update` method checks the `isActivated` status every frame:
     - If activated, the card demonstrates a pulsing effect.
     - Otherwise, the card's color returns to its default.

7. **Pulsing Effect:**
   - The `PulseCoroutine` delivers a pulsing effect by altering the card's color between the default and pulse colors based on the defined `pulseDuration`.

## HighLightPlane

**Purpose:**
The `HighLightPlane` class handles the visual highlighting of specific game elements, enabling developers to accentuate certain parts of the game dynamically.

**Key Features:**

1. **Highlight Elements Enumeration:**
   - Defines `HighlightElement` enumeration, representing various game elements that can be highlighted such as `Invocations`, `Deck`, `Effect`, `LifePoints`, etc.

2. **Custom Highlight Event:**
   - Introduces a custom `HighlightEvent` derived from `UnityEvent` which holds information about which game element to highlight and its activation state.

3. **Configuration Options:**
   - `element`: Determines the type of game element this instance of the class should focus on.
   - `PulseDuration`: Sets a constant time (0.5 seconds) for how long each pulse lasts.
   - `PulseColor`: A static readonly color (green) to represent the pulsing highlight.

4. **Component References:**
   - Uses `meshRenderer` to adjust the visual aspects of the game element.

5. **Global Highlight Notification:**
   - Maintains a public static event `Highlight` allowing various components in the game to be notified of highlight status changes.

6. **Lifecycle and Event Management:**
   - `Awake`: Initializes the mesh renderer component.
   - `Start`: Subscribes the `UpdateStatus` method to the global `Highlight` event.
   - `OnDestroy`: Ensures the component unsubscribes from the global `Highlight` event to avoid memory leaks.

7. **Dynamic Highlighting:**
   - The `Update` method reviews the `isActivated` state every frame. Depending on its value:
     - The game element is subjected to a pulsing highlight effect if activated.
     - Returns to its default appearance if not activated.

8. **Pulsing Effect Logic:**
   - The `PulseCoroutine` method manages the pulsing visual effect by alternating between the pulse color and clear color based on the `PulseDuration`.

## HighLightText

**Purpose:**
The `HightLightText` class is responsible for dynamically highlighting text in the game based on specific game events.

**Key Features:**

1. **Configuration Options:**
   - `element`: Defines the type of highlight element associated with this text.
   - `pulseDuration`: Specifies the time duration (defaulted to 0.5 seconds) for each pulse effect on the text.

2. **Internal State Management:**
   - Uses `isActivated` to determine if the text should be highlighted.
   - Utilizes `waitEndTurn` to control the execution flow of the pulse coroutine.

3. **Text Rendering:**
   - Contains a reference to the `TextMeshProUGUI` component, allowing the class to modify the visual appearance of the text.

4. **Lifecycle and Event Subscription:**
   - `Awake`: Initializes the `TextMeshProUGUI` component reference.
   - `Start`: Subscribes the `UpdateStatus` method to the global `Highlight` event from `HighLightPlane`.
   - `OnDestroy`: Ensures that the class unsubscribes from the global `Highlight` event to prevent potential memory leaks.

5. **Dynamic Text Highlighting:**
   - The `Update` method is invoked every frame and checks the `isActivated` state:
     - If activated, the text undergoes a pulsing highlight effect.
     - If not activated, the text returns to its default white color.

6. **Utility Methods:**
   - `SetTextColor`: A helper function to easily change the color of the text.
   - `PulseCoroutine`: Manages the pulsing effect on the text by alternating between the white and clear colors based on the `pulseDuration`.

## Scenario

**Purpose:**
The code provides a structure and framework for managing and converting game scenarios, especially those defined in JSON format, into actionable game elements.

**Key Components:**

1. **Scenario Class:**
   Represents the structured data of a game scenario. It primarily contains:
   - `ActionScenarios`: An array representing different actions that can be taken during the game scenario.

2. **ActionScenario Class:**
   Represents a single action within a scenario, containing attributes like:
   - `Index`: The position of the action in the scenario.
   - `Highlight`: The type of visual emphasis associated with the action.
   - `PutCard`: The card involved in the action.
   - `Image`: An image associated with the action.
   - `Video`: A video associated with the action.
   - `Attack`: Sequences or steps pertaining to attack actions.
   - `Action`: Specifies the type of action to be taken.

3. **JsonScenario Class:**
   Represents the raw JSON data for a scenario and provides methods to convert JSON data into structured Scenario objects.
   - `ToScenario()`: Converts the JSON scenario to a structured Scenario object.
   - Helper methods: Transform JSON data for actions and highlights into `ActionScenario` objects.

4. **ActionJsonScenario Class:**
   Represents the raw JSON data for an individual action within a scenario, including attributes that map directly to JSON properties.

5. **Enumerations:**
   - **Highlight:** Defines possible visual emphasis types for different game elements.
   - **Action:** Lists potential actions that can be taken within a scenario.

## ScenarioDecoder

**Purpose:**
`ScenarioDecoder` is responsible for decoding game scenarios from a JSON-formatted text asset and converting them into a structured `Scenario` object, suitable for game use.

**Key Components:**

1. **jsonFile:**
   A `TextAsset` that holds the raw JSON content detailing the game scenario.

2. **Scenario Property:**
   A property that, when accessed:
   - Parses the JSON text asset (`jsonFile`) into a `JsonScenario` object.
   - Converts the `JsonScenario` object into a structured `Scenario` object.
   - Returns the converted `Scenario` object.

## TutoPlayerGameLoop

The TutoPlayerGameLoop class represents the game loop for the tutorial player. It provides the following functionality:

* Triggers the scenario action based on the provided index.
* Handles the attack scenario based on the provided attack configuration.
* Places a card in the game using a scenario.
* Invokes the specified invocation cards.
* Equips the specified invocation card with the given equipment.
* Handles the highlighting of elements in the game based on the given highlight type.
* Handles the transition to the next round of the game.

The TutoPlayerGameLoop class is used by the tutorial mode to manage the game loop for the tutorial player. It is responsible for triggering the scenario actions, handling the attack scenario, placing cards in the game, invoking invocation cards, equipping invocation cards, handling the highlighting of elements in the game, and transitioning to the next round of the game.

## VideoPlayerObserver

**Description:**
The `VideoPlayerObserver` class is designed to monitor and interact with a `VideoPlayer` component in Unity. Its main function is to provide an event-triggered response when a video reaches its conclusion.

**Features:**
1. Acquires the associated `VideoPlayer` component when initialized.
2. Sets up an event listener to detect when the video reaches its end point.
3. If the video concludes, it triggers a specific dialogue event and deactivates its game object.
4. Ensures that all event listeners are removed when the object is destroyed to prevent any memory leaks or unintended behaviors.

**Dependencies:**
- `OnePlayer.DialogueBox`
- `UnityEngine`
- `UnityEngine.Video`

Note: For the observer to function correctly, the game object it's attached to must have a `VideoPlayer` component. If the component is absent, an error message will be logged.
