# Cards

## Card Location

### CardLocation

CardLocation manages the positions of different types of cards for each player in a card game. It provides methods for getting the deck location, initializing the player cards, adding a listener to the UpdateLocation event, updating the position of the cards, hiding cards, fetching the physical game object of a card, and updating the display properties and position of a card game object.

### PlayerCardLocations

PlayerCardLocations represents the locations of different types of cards in the card game. It includes fields for the locations of invocation cards, equipment cards, effect cards, the field card, the deck, and the yellow trash pile.

## CardDisplay

CardDisplay handles the display of cards in a card game. It provides properties for accessing and setting the InGameCard and Card objects, a property for determining the current material to be used for the card, an Awake method that initializes the component, an InitializeCard method that initializes the card, and two methods for showing and hiding the card face (not currently used but may be useful later). It also provides an UpdateCardMaterial method that updates the material used for the card's display.

## CardFamily

CardFamily defines an enumeration of card families and an extension class for that enumeration. The enumeration defines a set of card families, while the extension class provides a method for converting a CardFamily value to its localized name.

## CardName

CardName defines an enumeration of card names and a static class that provides a mapping between the enumeration and their respective human-friendly string representations. The enumeration defines a set of card names, while the static class provides a method for converting a CardNames value to its localized name

## CardOwner

CardOwner defines an enumeration of card owners, indicating whether a card belongs to Player 1, Player 2, or has not been assigned an owner.

## CardType

CardType defines an enumeration of card types, as well as an extension class for that enumeration. The enumeration defines a set of card types, while the extension class provides a method for converting a CardType value to its localized name.

## PhysicalCardDisplay

PhysicalCardDisplay handles the physical display of cards in the game. It provides a public property for getting or setting the card associated with the display, a public property for indicating whether the card face is hidden, and two public methods for hiding and displaying the card face. It also provides a private method for updating the card's material display based on its status and assigned card.

## PlayerCards

PlayerCards represents all the cards of a player. It provides methods for adding and removing cards from the player's hand, effect cards, invocation cards, and yellow cards. It also provides methods for checking if a card is in the player's hand, invocation cards, or yellow cards, as well as methods for resetting the attack number of invocation cards during a new turn and applying powers on a new invocation card on field.

## PlayerStatus

PlayerStatus represents the health and status of a player in the game. It provides methods for changing the player's health, getting the number of shields the player has, and enabling and disabling the player's ability to block an attack. It also triggers an event whenever the player's health changes.
