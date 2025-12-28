# MessageBox

## CardSelector

### Overview:
`CardSelector` is a versatile and modular component responsible for handling card selection functionalities in the game. It efficiently manages user interactions with cards, offers rich configuration options for card-related UI, and handles event-driven operations tied to card selection.

### Features:
1. **Event-driven Operations**: Utilizes Unity's event system, providing an event (`NumberedCardEvent`) that emits when a card is numbered, which can be useful for various game mechanics.

2. **Integration with CardSelectionManager**: The script works closely with `CardSelectionManager` to track card selections and reflect changes, ensuring seamless and intuitive card interactions.

3. **UI Customizability**: Comes with a set of methods to manage and customize card-related UI components, such as buttons and text descriptions. Enables showing numbers on cards and offers flexibility in presenting card data.

4. **Dynamic Card Numbering**: Allows for the dynamic numbering of cards based on the order of selection, a feature that can be toggled on or off as per the game requirements.

5. **Prefab Instancing**: Allows for the creation of new card selection instances using a predefined prefab, ensuring uniformity across different parts of the game.

6. **Advanced Configuration Options**: Supports detailed configuration (`CardSelectorConfig`) for card interactions, including defining actions for various buttons, managing card selection limits, and controlling display properties.

For more detailed methods' explanations and their interactions, refer to the inline comments within the script.

## Config

This code provides a flexible framework for configuring user interfaces in Unity, focusing on UI elements such as buttons and actions associated with them.

### Main Features:

1. **UIConfig (Abstract Class)**: A base configuration class that represents generic UI settings.
   - Title of the UI.
   - Whether to display 'Negative', 'Positive', or 'OK' buttons.
   - Action to be executed on pressing the negative button.

2. **MessageBoxConfig (Class)**: Configuration settings specific to message boxes.
   - Description text for the message box.
   - Actions to be executed on pressing the 'OK' or 'Positive' buttons.

3. **CardActions (Struct)**: Represents actions related to card selections.
   - Action for a single card selection.
   - Action for multiple card selections.

4. **CardSelectorConfig (Class)**: Configuration settings specific to a card selector UI.
   - A list of in-game cards for selection.
   - Actions related to 'OK' button or positive button selections.
   - Number of cards to be selected.
   - Whether to display the order of card selections.

This modular approach allows developers to quickly spin up different UI configurations without having to rewrite or heavily modify their existing UI scripts.

## DisplayCards

`DisplayCards` is a Unity component dedicated to managing and displaying a collection of in-game cards. This class makes use of the Singleton design pattern and provides functionalities to set, display, and manage the lifecycle of card objects in the game.

### Key Features:

1. **Card Management with ObservableCollection**: Utilizes `ObservableCollection<InGameCard>` to monitor changes in the card list, allowing dynamic addition and removal of cards.

2. **Card Display**:
   - Sets the list of cards to be displayed via the `CardsList` property.
   - Dynamically fetches card game objects from a card pool and activates them.
   - Automatically updates the display area size based on the number of cards.

3. **Event-driven Architecture**:
   - An event listener (`CardList_CollectionChanged`) is set up to respond to changes in the card list.
   - Handles actions like adding new cards, hiding cards, and other potential card collection modifications.

4. **Clean Up on Destruction**: When the `DisplayCards` component is destroyed, it:
   - Returns card game objects to the pool.
   - Deactivates card game objects and clears associated lists.
   - Unregisters event listeners to prevent memory leaks.

5. **Interaction with Other Managers**:
   - Interacts with `CardPoolManager` to retrieve and manage card game objects.
   - Coordinates with `CardSelectionManager` to handle card selection states.

By using `DisplayCards`, developers can simplify the process of card management and display in their Unity projects, ensuring efficient and dynamic card operations.

## MessageBox

`MessageBox` is a Unity component that provides a streamlined and configurable approach to display message boxes within your Unity projects. Built with flexibility in mind, this class allows developers to instantiate and configure message boxes dynamically, integrating both description texts and multiple button actions.

### Key Features:

1. **Modular Button Configuration**:
   - Supports various button configurations like OK, Positive, and Negative buttons.
   - Allows developers to assign specific UnityActions to these buttons.
   - Ensures clean-up by destroying the associated GameObject upon button click.

2. **Dynamic Text Setting**:
   - Integrates with `TextMeshProUGUI` to display custom message descriptions.

3. **Prefab Integration**:
   - Uses a predefined MessageBox prefab which can be customized as per the project requirements.
   - Instantiates and activates the prefab dynamically within a specified canvas.

4. **Flexibility with UI Configurations**:
   - Enables custom configurations through the `MessageBoxConfig`.
   - Allows for the dynamic setting and adjusting of message box properties and actions using the provided configuration.

5. **Utilities**:
   - Contains helper methods to retrieve and configure internal components, ensuring ease of use and minimizing redundant code.

### Usage:

When you need to display a message box, use the `CreateMessageBox` method, specifying the canvas to which the message box should be added and providing the necessary `MessageBoxConfig` with the desired properties and actions.

By leveraging the `MessageBox` class, developers can easily create, configure, and manage message boxes in Unity, adding enhanced user feedback and interaction capabilities to their projects.

## MessageBoxBaseComponent

`IMessageBoxBaseComponent` serves as an interface that defines the essential functionalities for a message box component in Unity. It aims to provide a standardized way to set and manage UI elements for a message box, making it easier to develop and maintain a consistent user interface experience.

### Key Features:

1. **Core Interface**:
   - `IMessageBoxBaseComponent` offers a method `SetNewValueGameObject` which establishes the fundamental functionality to set UI elements for a given GameObject based on a provided UI configuration.

2. **Consistent UI Elements**:
   - The `MessageBoxBaseComponentConstants` static class defines constants related to the base UI elements of the message box, ensuring naming consistency across implementations.

3. **Utility Extension Methods**:
   - The `MessageBoxBaseComponentExtensions` class extends the base component with useful methods to:
     - Set the title text of a UI element.
     - Configure buttons' text and visibility based on specific parameters.
     - Retrieve child transforms within a parent GameObject using the child's name.

4. **Localization Integration**:
   - Button configurations use localization keys to set the button text, supporting multiple languages and ensuring internationalization readiness.

By integrating with `IMessageBoxBaseComponent`, developers can achieve a structured and consistent approach to building and managing message boxes, ensuring that UI components are configured uniformly and effectively across the application.
