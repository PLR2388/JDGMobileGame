# Managers

## CardManager

This class provides functionality for managing cards in the game. It allows retrieving the card set for the current player and the opponent player, drawing a card for the current player, handling the end turn for the current player, checking if the attacker can execute an attack or has any actions available, calculating the damage result from an attack, handling the attack logic between the attacker and opponent, executing the special action for the attacker, building a list of cards that can be targeted for an attack, and checking if it's possible for the attacker to perform a special action.

## CardPoolManager

This class manages a pool of card GameObjects that can be used in the CardSelector class. It provides methods for initializing the card pool, adding a card GameObject to the card pool, retrieving a pooled card GameObject that matches the provided InGameCard, and destroying all the GameObjects in the card pool when the MonoBehaviour is destroyed.

## CardRaycastManager

This class manages the raycasting functionality to detect card interactions in the game. It provides methods for initializing itself on scene start, retrieving the InGameCard under the user's touch or click, and performing a raycast to detect objects under the user's touch or click. The CardRaycastManager class is used by other classes in the game to detect card interactions.

## CardSelectionManager

This class manages the selection of cards in the game. It provides methods for selecting and deselecting cards, checking if a card is selected, and clearing the selection. It also provides events for when a card is selected, deselected, or the selection changes. The CardSelectionManager class is used by other classes in the game to manage the selection of cards.

## GameState

This class manages the state of the game, including the decks for both players. It provides methods for building the tutorial decks, instantiating specific cards, and getting the list of InGameCard objects for each player's deck and the list of all cards in the game. It is used by other classes in the game to manage the state of the game and to access the decks for both players.

## GameStateManager

This class manages the state of the game, including the current phase, current player, and turn counter. It provides methods for accessing the current phase, current player, and turn counter, toggling the current player, setting the current phase, transitioning to the next phase in the sequence, incrementing the turn counter, and handling the end of a turn. It is used by other classes in the game to manage the flow of the game.

## InputManager

This class manages touch and click input, as well as the Android back button. It provides methods for checking if the user is tapping, touching, or releasing the screen, getting the position of the current touch or click input, enabling and disabling touch detection, and invoking events when a touch/click starts, a long touch/click is detected, a touch/click ends, and the Android back button is pressed.

This class is used by other classes in the game to handle user input. For example, the CardManager class uses the InputManager class to check if the user is touching a card before performing an action. The PlayerManager class uses the InputManager class to determine when to register a touch event.

## PlayerCardManager

This class manages the player's hand and deck of cards. It provides methods for drawing cards, processing end-of-turn logic, and getting the PlayerCards component. It is used by the PlayerManager and CardManager classes to manage the player's cards.

## PlayerManager

This class manages player-related functionalities such as retrieving the current player status and handling attacks on the opponent. It provides methods for retrieving the current player's status, retrieving the opponent player's status, initializing the shield count for both players to zero, and handling the attack on the opponent player and decrementing the shield if the opponent has one, otherwise applying the damage. It is used by other classes in the game to manage player-related gameplay.

## UIManager

This class manages the user interface elements and interactions for the game. It provides methods for displaying a large card view of a given card, displaying a message box to inform the user about the available opponents for invocation, hiding the large card viewer, and displaying a pause menu with given positive action. It is used by other classes in the game to display information to the user and to handle user input.

## UnitManager

This class manages and instantiates units (cards) in the game. It provides methods for initializing the physical card instances based on the provided deck and deck location, and for generating a unique name for a card based on its title and the player it belongs to. It is used by other classes in the game to instantiate and manage the physical card objects.
