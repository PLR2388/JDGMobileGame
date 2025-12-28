# Units

## Contre

### ContreCardHandler

In the README, `ContreCardHandler` is outlined as a specialized handler class within the game, responsible for managing the behaviors and interactions of 'contre' cards. It is derived from the `CardHandler` base class and is utilized to define the unique functionalities associated with contre cards. On instantiation, it takes an `InGameMenuScript` object which it uses to interface with the game's UI.

The primary role of `ContreCardHandler` is to handle the game logic when a contre card is played. This includes updating UI elements such as button texts—setting it to a localized 'Contre' text—and making the put card button interactable. While the `HandleCard` method is defined to set up these UI elements, the `HandleCardPut` method, which is supposed to manage the actual card placement on the board or in the player's hand, is not yet implemented, indicating that the behavior is still to be defined.

The class serves as a component of the game's architecture that separates the card's logic from other elements, ensuring that the game can easily be extended with new card types and behaviors while maintaining clean and understandable code.

## Effect

### EffectAbility

#### AddShieldsForUserEffectAbility

`AddShieldsForUserEffectAbility` is a class that defines an effect ability within a game, specifically adding a certain number of shields to the user's status. It extends `EffectAbility`, signifying that it is part of a broader system of abilities with effects.

Upon initialization, this ability requires a name, a description, and the specific number of shields it will provide. Its core functionality is divided into several methods:

1. `CanUseEffect`: It checks whether the effect can be applied, in this case, it's contingent on the player having no invocation cards.
2. `ApplyEffect`: This method enacts the primary effect of this ability, which is increasing the shield count of the player by the number specified during the ability's construction.
3. `OnTurnStart`: This method contains logic that is triggered at the start of the player's turn. Notably, if the player's shield count is zero, this ability removes itself from the effect cards and adds itself to the yellow cards, suggesting a one-time use or condition-based availability within the gameplay mechanics.

#### ChangeFieldCardEffectAbility

The `ChangeFieldCardEffectAbility` is a class that encapsulates the functionality of an effect ability within the game that enables a player to change their current Field card. This ability is initialized with a name, a description, and the duration of the effect in turns.

In terms of its operational methods:

1. `CanUseEffect`: It checks if the effect is applicable based on the presence of Field cards within the player's deck.

2. `ApplyEffect`: This central method activates the effect, allowing the player to select a new Field card from their deck. If there's no current Field card, the selected card becomes the new Field card. If there's already a Field card in play, the new selection replaces it, and the old card is added to the player's yellow cards, indicating a change in the field of play.

3. Within `ApplyEffect`, a card selector UI is configured and displayed to facilitate the selection of the new Field card. If the selection is not a Field card, a warning message is shown to the player.

#### ControlOpponentInvocationCardEffectAbility

This class provides the functionality to control an opponent's Invocation card for a turn. It is a strategic gameplay feature allowing players to turn the tide of the game by using the opponent's Invocation cards to their advantage.

##### Features:

- **Effect Initialization**: When instantiated, the ability is assigned a name and description, setting up its identity within the game.

- **Effect Validation**: `CanUseEffect` checks whether there are Invocation cards on the opponent's side to ensure that the ability can be activated.

- **Effect Activation**: `ApplyEffect` enables the player to select and control an opponent's Invocation card. This is facilitated through a UI selection process that prompts the player to choose one of the opponent's Invocation cards.

- **Effect Application**: Upon selection, the chosen Invocation card is marked as controlled, unblocked for attack, and moved from the opponent's Invocation card list to the player's list, symbolizing the transfer of control.

- **Turn-based Control**: `OnTurnStart` is called at the start of the player's turn to handle the invocation card's status. It ensures the controlled card is freed, attacks are unblocked for that turn, and then the card is returned to the opponent’s card list, reflecting the temporary nature of the control.

##### User Interface:

- **Card Selection**: Through a Card Selector UI, players can select which Invocation card to control from the opponent's available cards.

- **Message Box**: If the player does not select an Invocation card, a message box alerts them to choose the correct type of card, ensuring proper game flow.

##### Gameplay Impact:

- The ability to control an opponent's Invocation card can drastically influence the strategic decisions and outcomes of a turn, providing dynamic interactions and shifts in the power balance between players.

This ability is a key tactical element in the game, offering a unique interaction with the game's mechanics and opponent's strategy.

#### DestroyCardsEffectAbility

The `DestroyCardsEffectAbility` class defines an effect ability within the game that enables a player to destroy any cards on fields. The players only need to follow some requirements to have access to this power.

- **Purpose**: Represents the ability of a game entity to destroy cards.

#### DestroyFieldCardAbility

`DestroyFieldCardAbility` is a class designed for the game that provides the functionality to destroy field cards in play. Below is an overview of its functionality and use case:

- **Purpose**: Represents the ability of a game entity to destroy field cards, which can affect the state of the game by removing active cards from the field.

- **Usage**: This ability can be triggered during the game, presumably costing the player's health to use it, to remove field cards from either the player's side or the opponent's side.

- **Implementation Details**:
  - **Health Cost**: The ability requires the player to pay a certain amount of health, as defined by the `costHealth` variable.
  - **Initialization**: It takes in a name, description, and health cost for instantiation.
  - **Apply Effect**: The core method `ApplyEffect` collects all field cards and prompts the user to select a card for destruction through a card selection interface.
  - **Card Selection**: A UI element is provided for card selection, where the player can choose which card to destroy.
  - **Destroy Field**: Once a card is selected, `DestroyField` removes the card from the field and places it into the player's 'yellow cards' collection, clearing the field slot.

- **Conditions**:
  - The ability can only be used if there is at least one field card present on either the player's or the opponent's side, as checked by the `CanUseEffect` method.

- **Additional UI Integration**:
  - Error handling is present to prompt the player with a message box if no card is selected when attempting to use the ability.
  - Localized messages are used for UI elements, which indicates that the game supports multiple languages for accessibility.

This class is a key component for the strategic play in the card game, as it allows players to alter the state of the field by targeting and eliminating key cards of the opponent or their own for various strategic reasons. The health cost mechanic also adds a layer of complexity to decision-making, as players must balance the benefits of using the ability with the potential risks associated with the reduction in health.

#### DirectAttackEffectAbility

`DirectAttackEffectAbility` is a class in a card game that encapsulates the functionality for a game entity to execute a direct attack under specific conditions related to the opponent's health. Below is a summary of its key features and functionality:

- **Purpose**: Provides the logic for a card or player to perform a direct attack that is conditional upon the opponent's health.

- **Condition for Use**: This ability becomes available or can be used only when the opponent's current health is below a specified threshold, which is passed as an argument (`limitHpOpponent`) during the creation of the ability instance.

- **Initialization**: The constructor takes in three parameters:
  - `name`: The name identifier for the effect ability.
  - `description`: A textual description of what the effect ability does, to inform players or for use in the user interface.
  - `limitHpOpponent`: A float value representing the health threshold of the opponent that allows the direct attack to be executed.

- **Effect Activation Check**: It overrides the `CanUseEffect` method to determine if the direct attack ability can be triggered. The check compares the current health of the opponent (`opponentPlayerStatus.GetCurrentHealth()`) against the `limitHpOpponent`. If the opponent's health is less than the specified limit, the ability can be activated.

This ability is a strategic component of gameplay that allows players to potentially turn the tide of a match by enabling a powerful attack when the opponent is weakened. The direct attack could represent a finishing move or a strategic play to press the advantage when the opponent is vulnerable.

#### DivideDEFOpponentEffectAbility

`DivideDEFOpponentEffectAbility` is a class representing a card ability in the game that specifically targets and modifies the defense value of the opponent's invocation cards. Here is an overview of its functionality and purpose:

- **Functionality**: This ability divides the defense of each of the opponent's invocation cards by a specified factor, potentially making them more vulnerable to attacks.

- **Initialization Parameters**:
  - `name`: The identifier name of the ability.
  - `description`: A textual description of what the ability does.
  - `divideFactor`: The factor by which the defense of the opponent's invocation cards will be divided when the effect is applied.

- **Activation Check**: The `CanUseEffect` method checks if there are any invocation cards on the opponent's side to target. If there are, the ability can be activated.

- **Effect Application**: When the `ApplyEffect` method is invoked, it iterates through the opponent's invocation cards, dividing the defense value of each card by the `divideFactor`.

- **Reset Mechanism**: The `OnTurnStart` method is designed to reset the effects at the start of a turn. It does this by multiplying the defense of each of the opponent's invocation cards by the `divideFactor`, effectively reversing the division made by `ApplyEffect`.

The ability is strategic in nature, allowing players to weaken an opponent's defense temporarily, which can be a critical move during a turn to break through the opponent's strategies. The temporary nature of the effect (with the reset on turn start) suggests a careful balance in gameplay, providing a window of opportunity without permanent debilitation of the opponent's cards.

#### FamilyFieldToInvocationsEffectAbility

The `FamilyFieldToInvocationsEffectAbility` class in the provided script is an `EffectAbility` for a game that assigns the family property from a field card to invocation cards. Here's a breakdown of its purpose and functionality:

- **Purpose**: This ability is designed to transfer a specific attribute, namely the 'family' from a field card, to all invocation cards controlled by a player. It introduces strategic gameplay elements, allowing players to dynamically change their invocation cards' families to gain advantages or meet certain conditions during play.

- **Initialization Parameters**:
  - `name`: The unique identifier for the ability.
  - `description`: A detailed description of what the ability does.
  - `costPerTurn`: This represents a recurring cost that the player has to pay to maintain the effect every turn.
  - `cardName`: The name of the card to which this ability is tied.

- **Ability Mechanics**:
  - `ApplyPower`: This method is called to apply the field card's family to all of the player's invocation cards.
  - `SetNewFamilyToInvocation`: It sets the new family to a specific invocation card.
  - `CanUseEffect`: Determines if the ability can be used by checking if there is a field card and if there are invocation cards present.
  - `ApplyEffect`: The method that triggers the application of the effect.
  - `OnTurnStart`: At the start of each turn, this method presents the player with a choice via a message box: to continue applying the family effect for a cost or to reset the invocation cards' families to their original state.
  - `ResetInvocationsFamily`: This method resets the family on invocation cards to their original families and handles moving the effect card back to a specific pile (presumably the yellow pile, as indicated by the code).

- **Turn-Based Dynamics**:
  - A cost mechanism is included, requiring the player to pay a specified amount of points (PV) each turn to maintain the effect.
  - The player is given the choice to either pay the cost and maintain the effect for the next turn or to opt-out, which would reset the invocation cards' families.

- **Card Interaction**:
  - The ability includes methods (`OnInvocationCardAdded` and `OnInvocationCardRemoved`) to handle dynamic changes in the player's card collection, ensuring that new invocation cards receive the family effect and that the family effect is removed when an invocation card is no longer in play.

Overall, this ability modifies gameplay by allowing players to temporarily alter the family attributes of their invocation cards, providing strategic depth and requiring players to make critical decisions about resource management each turn.

#### GetCardFromDeckYellowEffectAbility

The `GetCardFromDeckYellowEffectAbility` class is a type of `EffectAbility` that grants the ability to draw a specified number of cards from either the player's main deck, a separate "yellow" pile, or both.

Here's a summary for the README:

- **Purpose**: This class provides the functionality to draw cards from different sources as part of the game's mechanics.
- **Card Sources**: Cards can be drawn from three sources, defined by an enumeration `CardSource`:
  - `Deck`: Draw cards from the main deck.
  - `Yellow`: Draw cards from a specific pile called the yellow pile.
  - `Both`: Draw cards from both the main deck and the yellow pile.
- **Functionality**:
  - **Initialization**: When creating an instance of this ability, you need to specify the number of cards to draw (`numberCards`) and the source from which to draw (`cardSource`).
  - **Card Retrieval**: The `GetCardSourceList` method determines from which pile or piles the cards will be drawn based on the `cardSource`.
  - **Card Selection**: When only one card is to be drawn, the `HandleCardSelection` method presents a UI to the player to select a card. If no card is selected, a warning message is displayed using `DisplayWarningMessageBox`.
  - **Moving Cards**: The selected card(s) are moved to the player's hand by the `MoveCardToPlayerHand` method, adjusting the respective source pile(s) accordingly.
- **Usage Conditions**: The ability to use this effect is checked by `CanUseEffect`, which ensures there are enough cards in the selected source to draw the specified number of cards.
- **Effect Application**: When the effect is applied using `ApplyEffect`, the card selection UI is presented to the player to carry out the card drawing process.

#### GetHPBackEffectAbility

The `GetHPBackEffectAbility` class in the provided code snippet defines an ability in a card game that allows a player to regain health by meeting certain conditions.

- **Purpose**: To provide an ability for players to recover health points (HP).
- **Functionality**:
  - Players can sacrifice a specific number of invocation cards to regain health.
  - The ability requires a condition to be met related to the attack/defense value of the invocation cards.
- **Key Parameters**:
  - `numberInvocations`: Number of invocation cards that need to be sacrificed to trigger the ability.
  - `atkDefCondition`: A threshold value that the attack or defense of an invocation card must meet or exceed to be eligible for sacrifice.
  - `hpToRecover`: The amount of health the player recovers when using this ability.
- **Behavior**:
  - If the number of invocation cards that meet the attack/defense condition is equal to or greater than the required number for sacrifice, the ability can be used.
  - Sacrificing an invocation card moves it to the player's yellow cards pile from the invocation cards pile.
  - The player's health is then increased by the specified `hpToRecover` amount, with an option to recover up to the player's maximum health if a certain condition (`MaxHealthRecovery`) is met.
- **User Interaction**:
  - If only one invocation card needs to be sacrificed, a card selection UI is presented for the player to choose the specific card.
  - A warning message is displayed if the player fails to choose a card for sacrifice.
- **Edge Cases**:
  - The ability accounts for situations where no sacrifice is needed (`numberInvocationToSacrifice` is 0), directly recovering health.
  - Handles the case for a single sacrifice requirement through a dedicated method `PerformSingleSacrifice`.

#### IncrementNumberAttackEffectAbility

The `IncrementNumberAttackEffectAbility` class defines an ability in the game that increases the number of attacks that invocation cards can perform each turn.

- **Purpose**: To provide a game ability that increases the attack capacity of invocation cards for each turn.
- **Functionality**:
  - This ability increments the number of attacks that each invocation card can make in a turn by a specified amount.
- **Key Parameters**:
  - `numberAttackPerTurn`: The increment to the number of attacks an invocation card is allowed to perform during a player's turn.
- **Behavior**:
  - When the ability is applied, all of the player's invocation cards have their allowed number of attacks for the turn set to `numberAttackPerTurn`.
  - The increment in the number of attacks applies to all current invocation cards and any that are added while the ability is in effect.
  - If an invocation card is removed, its allowed number of attacks is reset back to the default value.
- **Use Case**:
  - As part of the ability system, when a player activates this ability, all their invocation cards on the field will be able to attack more times than they normally would be able to in each turn, as defined by `numberAttackPerTurn`.

This ability enhances the offensive capabilities of a player's cards on the board, potentially allowing for a more aggressive gameplay strategy. It is a strategic asset that can be used to apply pressure on the opponent by increasing the frequency of attacks.

#### InvokeCardFromDeckYellowEffectAbility

The `InvokeCardFromDeckYellowEffectAbility` class defines an ability in the game to invoke a card from the player's deck or yellow trash pile.

- **Purpose**: To provide an ability that allows a player to invoke a card into play from their deck or specifically from the yellow trash pile, depending on the state of the `fromYellowTrash` flag.
- **Functionality**:
  - The ability lets the player add a card to their field of invocation cards, enhancing their in-game resources and options.
- **Key Parameters**:
  - `fromYellowTrash`: A boolean flag determining the source of the invocation—when set to `true`, the invocation will happen from the yellow trash; otherwise, it will default to the deck.
- **Behavior**:
  - The ability first checks if there is space for invocation (i.e., less than 4 invocation cards in play).
  - If invoking from the yellow trash, it also checks that there is at least one invocation-type card in the yellow trash.
  - When the effect is applied, and the source is the yellow trash, the player is presented with a UI to select which card they want to invoke. If a valid card is selected, it is moved from the yellow trash to the player's invocation cards.
  - If a non-invocation card is selected by mistake, a warning message is displayed to the player.
- **Use Case**:
  - When activated, this ability allows players to recover and utilize cards from the yellow trash, giving them a strategic advantage by reusing powerful cards or cards necessary for their strategy that were previously discarded.
  - This mechanic provides a form of resource recovery and management, potentially allowing for a comeback or a significant power play.

#### LimitHandCardsEffectAbility

The `LimitHandCardsEffectAbility` class is responsible for implementing an effect that enforces a maximum hand size limit on players in a card game.

- **Purpose**: This ability ensures that players do not exceed a specified maximum number of cards in their hand.
- **Functionality**:
  - It is designed to enforce a rule where each player can only hold a certain number of cards in their hand, specified by the `numberCards` parameter.
- **Key Parameters**:
  - `numberCards`: An integer representing the maximum allowed number of cards in a player's hand.
- **Behavior**:
  - When applied, it affects both the current player and the opponent.
  - If a player has fewer cards than the limit, the ability will draw cards from their deck until they reach the limit.
  - If a player has more cards than the limit, it prompts them to discard down to the limit by moving the excess cards to their yellow trash pile.
  - The UI canvas is used for displaying messages and prompts related to this effect.
- **Use Case**:
  - It provides a check and balance mechanism to prevent any one player from gaining an unfair advantage by hoarding cards.
  - By prompting players to choose which cards to discard, it also introduces strategic decision-making about card management.

#### LookDeckCardsEffectAbility

The `LookDeckCardsEffectAbility` class is a feature in the game that allows players to view and rearrange a certain number of cards from the top of their deck.

- **Purpose**: To enable players to strategically view and order the top cards of their deck.
- **Functionality**:
  - It provides the ability for players to look at a fixed number of cards from the top of their deck (`numberCards`) and decide if they want to rearrange them.
- **Key Parameters**:
  - `numberCards`: Specifies the number of top cards that the player is allowed to view and reorder.
- **Behavior**:
  - Before applying the effect, it checks if the deck has cards.
  - It prompts the player to choose between viewing their own or their opponent's deck.
  - After viewing, the player can reorder these cards based on their strategic preferences.
  - If the deck contains fewer cards than `numberCards`, the player can view and reorder all available cards in the deck.
  - A message box is displayed to confirm the ordering of the cards or to present a warning if the cards are not ordered properly.
- **Use Case**:
  - This ability is typically triggered by a game event or action that allows insight into future draws, providing a significant strategic advantage.
  - Players can plan future moves by setting up the order of their upcoming card draws.
  - The ability enhances the game's depth, requiring players to make critical decisions based on the potential order of their card draws.

#### LookHandCardsEffectAbility

The `LookHandCardsEffectAbility` class is a feature in the game that grants players the ability to view the hand cards of their opponent.

- **Purpose**: To permit a player to gain information by viewing the cards currently held in the opponent's hand.
- **Functionality**:
  - Allows for the inspection of the opponent's hand cards without necessarily altering the game state.
- **Condition**:
  - Can only be used if the opponent has one or more cards in their hand.
- **Behavior**:
  - Upon activation, it uses a card selector interface to display the opponent's hand cards to the player.
  - If the player's hand contains cards, they are presented with a choice to potentially remove a card from the opponent's hand.
  - If the player chooses to remove a card, they must also remove a card from their own hand, maintaining a balance in gameplay.
  - Provides interaction through message boxes for warnings and confirmations, enhancing user experience.
- **User Interaction**:
  - Players interact with this ability through UI elements, such as message boxes and card selection dialogs.
  - It features a positive and negative action choice, allowing players to decide on the strategic move of removing cards.
- **Use Case**:
  - This ability can be particularly useful for players to make informed decisions about their next moves based on the opponent's potential actions.
  - It introduces a layer of strategy, where players must weigh the benefits of gaining information against the cost of possibly weakening their own hand if they choose to remove cards.

#### LooseHPOpponentEffectAbility

The `LooseHPOpponentEffectAbility` class defines an effect ability within the game that allows a player to inflict damage to their opponent's health points (HP) based on specific conditions.

- **Purpose**: To offer a strategic game ability that reduces the opponent's HP by a calculated amount of damage, enhancing the competitive aspect of the game.
- **Functionality**:
  - Provides a mechanism to deal damage to an opponent, where the amount of damage is determined by the number of certain types of cards.
- **Damage Calculation Types**:
  - **ByPlayerInvocationCount**: Damage is multiplied by the count of invocation cards the player has.
  - **ByOpponentInvocationCount**: Damage is multiplied by the count of invocation cards the opponent has.
  - **ByOpponentHandCount**: Damage is multiplied by the number of cards in the opponent's hand.
- **Parameters**:
  - `damage`: A float value representing the base damage that can be scaled by the card counts.
  - `damageType`: An enum `DamageType` that determines how the final damage value is calculated.
- **Conditions for Use**:
  - The effect's applicability is determined by the type of damage calculation and the relevant card count (e.g., it can be applied if there are invocation cards present, according to the damage type).
- **Effect Application**:
  - When activated, the effect adjusts the opponent's HP by the calculated damage value, which varies depending on the damage type and the related card counts.
- **Intended Use Case**:
  - This ability is suited for players who want to adopt an aggressive strategy by directly targeting the opponent's HP, while also taking into account the state of the board (i.e., the number and type of cards in play).

#### SkipOpponentAttackEffectAbility

- **Purpose**: Provides a strategic gameplay ability that temporarily disables the opponent's attack capability.
- **Functionality**:
  - When activated, prevents the opponent from attacking the player for a specified number of turns.
- **Features**:
  - **Turn-based Skipping**: The ability can be set to skip the opponent's attack for one or more turns.
  - **Automatic Reversion**: The effect automatically expires after the set number of turns, reinstating the opponent's ability to attack.
- **Implementation Details**:
  - The ability is tied to a specific card, identified by `cardName`.
  - The effect is applied through the `ApplyEffect` method, which enables a block on the opponent's attack.
  - The `OnTurnStart` method checks the duration of the effect at the beginning of each turn and removes the block when the specified number of turns has passed.
- **Parameters**:
  - `cardName`: The name of the card associated with the effect.
  - `numberTurn`: An integer representing the number of turns during which the opponent's attack is skipped (defaulting to 1 if not specified).
- **Usage**:
  - This ability is particularly useful for players seeking to gain tactical breathing space or set up their board without immediate threat of an opponent's attack.
- **Intended Use Case**:
  - Ideal for defensive strategies that involve delaying opponent attacks while preparing countermeasures or establishing a stronger board presence.

#### SwitchAtkDefEffectAbility

- **Purpose**: Enhances gameplay with a dynamic effect that interchanges the attack and defense values of invocation cards.
- **Functionality**:
  - The ability can toggle the attack and defense statistics of a player's invocation cards, introducing unexpected twists in the game's strategic plays.
- **Features**:
  - **Comprehensive Application**: Applies the effect to all invocation cards the player controls, potentially affecting the entire board state.
- **Implementation Details**:
  - The `ApplyEffect` method triggers the switch for all invocation cards in the player's possession.
  - The `OnTurnStart` method ensures the switch effect is reassessed and applied consistently at the beginning of each turn.
  - The `OnInvocationCardAdded` and `OnInvocationCardRemoved` methods manage the effect in real-time as the player's card inventory changes.
- **Mechanics**:
  - A static helper method, `SwitchAttackAndDefense`, is used to perform the swap on individual invocation cards.
- **Intended Use Case**:
  - This ability is suited for decks that can benefit from flexible attack and defense configurations, allowing players to adapt to various combat scenarios.
  - Particularly useful in situations where the defense value is greater than the attack, providing an opportunity to deal significant damage unexpectedly.
- **Usage**:
  - This effect is an excellent addition to any player's arsenal who wishes to maintain an element of surprise, making it harder for opponents to predict the outcome of battles.

### EffectAbility

- **Overview**: `EffectAbility` serves as an abstract base class for defining various effect abilities in a card game. These abilities have the potential to alter the course of the game by manipulating card statistics, player statuses, and overall game mechanics.

- **Key Properties**:
  - `Name`: Identifies the effect ability from a predefined list of possible abilities (`EffectAbilityName` enumeration).
  - `Description`: A brief explanation of what the effect ability does.
  - `NumberOfTurn`: Specifies the duration of the effect in terms of the number of turns it remains active.
  - `Counter`: Tracks the number of turns that have passed since the effect was applied.

- **Core Functionalities**:
  - `CanUseEffect`: Determines whether the effect can be applied in the current state of the game, checking against player and opponent cards and statuses.
  - `ApplyEffect`: Implements the specific actions that constitute applying the effect to the game state.
  - `OnTurnStart`: Contains logic that should be executed at the beginning of each turn, which may include maintaining, modifying, or ending the effect.
  - `OnInvocationCardAdded`: Outlines behavior for when an invocation card is added to the player's collection, potentially triggering or modifying the effect.
  - `OnInvocationCardRemoved`: Defines actions to take when an invocation card is removed from play, which may affect the ongoing effect.

- **Usage in a Game**:
  - Effect abilities derived from this class can be attached to cards or other game elements to provide a wide range of strategic options and outcomes.
  - Abilities are designed to be flexible, allowing them to be easily included in or triggered by game events such as turns, card plays, or other special conditions.

- **Behavioral Notes**:
  - The `OnTurnStart` method also handles the lifecycle of an effect ability, automatically removing the effect card from the game and recycling it back into the player's deck when its duration has lapsed.

- **Intended Use**:
  - As a fundamental building block within the game's framework, this class is intended to be inherited by specific effect ability implementations that will define their own unique behaviors within the game's context.
  - It ensures consistent behavior and interface for all abilities, making the development of new abilities more structured and the gameplay experience more predictable.

### EffectAbilityLibrary

The `EffectAbilityLibrary` is a component of a game that manages a collection of `EffectAbility` instances. Each `EffectAbility` represents a special action or modifier that can be applied to game entities, such as cards or players, altering the game's state or the rules in some way.

The `EffectAbilityLibrary` is a comprehensive collection of distinct abilities that can affect gameplay in various ways. It contains and manages numerous instances of `EffectAbility`, each representing a unique power or rule modification that can be triggered under specific game conditions. These abilities can range from limiting the number of hand cards a player can hold, to altering attack and defense statistics, to destroying cards under certain prerequisites, and much more.

#### Features

- **Versatile Ability Set**: Includes a diverse set of predefined abilities such as hand card limitations, hit point (HP) modifications, deck manipulation, direct attacks, and more, offering a rich layer of strategic depth to the gameplay.
- **Dynamic Effects**: Abilities can target different game elements, from players' HP to the cards on the field, affecting the flow of the game with each activation.
- **Customizable Turn-Based Logic**: Many abilities are designed to last for a specified number of turns, with logic to execute at the start of a turn, when an invocation card is added or removed, providing a temporal aspect to the strategy.
- **Strategic Card Management**: Some abilities require the sacrifice of a player's own cards to trigger effects, prompting critical decision-making.
- **Opponent Disruption**: Several abilities are focused on disrupting the opponent's game plan by destroying their cards, skipping their attack phase, or even taking control of their cards temporarily.
- **Defensive Measures**: Players can gain shields or restore HP, bolstering their defenses against opponent attacks.
- **Game State Checks**: Before an ability is applied, the library checks whether the current game state allows for its activation, ensuring that effects occur at appropriate times.

#### Implementation

- **Singleton Pattern**: The class extends `StaticInstance`, indicating it uses the singleton pattern to ensure that only one instance of the library exists within the game.
- **Initialization**: During the `Awake` method, it initializes a dictionary mapping each `EffectAbilityName` enum to its corresponding `EffectAbility` instance, making it easy to access and apply these abilities throughout the game.

#### How to Use

- Retrieve an `EffectAbility` instance by accessing the `EffectAbilityDictionary` using an `EffectAbilityName` as the key.
- Invoke the methods on an `EffectAbility` instance to apply effects, handle turn logic, or react to changes in the game such as card additions or removals.

This library serves as a central hub for managing and applying the various effects that can impact the game, ensuring that players can utilize a wide range of strategies during play.

### EffectCardHandler

The `EffectCardHandler` class is dedicated to managing the specific behaviors and interactions of effect cards within the game environment.

`EffectCardHandler` is a specialized subclass of `CardHandler` designed to handle the intricacies and UI updates of effect cards in the game. It extends the generic card handling functionality with specific logic required by effect cards.

#### Features

- **Effect Card Management**: It encapsulates the behavior logic for when a player interacts with effect cards.
- **UI Integration**: Updates and manages UI elements that are relevant to effect cards, ensuring that the game's interface reflects the state and capabilities of the effect cards.
- **Condition Checking**: Before allowing a card to be played, it checks to ensure all conditions are met for its use, such as the effect's applicability and the player's capacity to hold effect cards.
- **Interactivity**: Controls the interactability of UI components based on the game's current state and the effect cards' conditions.

#### Usage

- **HandleCard**: Call this method with an `InGameCard` instance to manage the card's behavior. It will update the UI button text and interactability based on the card's effect abilities and the player's status.
- **HandleCardPut**: Use this method when a card is being placed. For effect cards, it will invoke a specific event (`EffectCardEvent`) tailored to handle the placing of an effect card.

#### Implementation Details

- The class constructor takes an `InGameMenuScript` instance, linking it with the game's menu system for UI manipulation.
- It leverages the singleton instances of `CardManager` and `PlayerManager` to access player cards and statuses, ensuring that it operates within the current game context.
- Localized text is used for button labels, aiding in internationalization and improving accessibility.

`EffectCardHandler` is essential for handling the game logic related to the activation and placement of effect cards, ensuring that the player's interaction with these cards is seamless and consistent with the game's rules.

### EffectFunction

The `EffectFunctions` class is designed to manage the interactions with effect cards in the game.

`EffectFunctions` is a MonoBehaviour class that manages the behavior and lifecycle of effect cards in the game. It plays a crucial role in the game's mechanics by handling the application of effects from cards and maintaining the state of the game in response to those effects.

#### Features

- **Event Handling**: Subscribes to effect card-related events at the start of the game and unsubscribes when the object is destroyed to properly manage memory and performance.
- **Effect Card Placement**: Facilitates the placement of effect cards on the playing field, with a cap on the number of active effect cards.
- **Effect Application**: Applies the effect of the effect card to the game state, altering player cards and statuses according to the card's abilities.
- **UI Interaction**: Interacts with the game's UI, specifically using a `miniCardMenu` to manage effect card options and displaying messages to the player if actions cannot be completed.

#### Usage

- The class listens for `EffectCardEvent` from `InGameMenuScript` and `TutoInGameMenuScript`, and calls the `PutEffectCard` method when the event is triggered.
- `PutEffectCard` is the core method, which activates upon an effect card's placement, checking the current number of effect cards on the field, applying effects, updating player's hand and effect card lists, or showing warnings if the limit is exceeded.

#### Implementation Details

- **Unity Inspector**: Uses serialized fields to link the `miniCardMenu` GameObject and `canvas` Transform, enabling in-editor assignment.
- **Localization**: Integrates with a localization system to retrieve user-facing strings for messages, aiding in internationalization and improving accessibility.
- **MessageBox**: Utilizes a message box system to inform players when they cannot place more effect cards on the field.

`EffectFunctions` is a critical component that ensures effect cards are played according to the rules and the effects are correctly applied to the game state, contributing to the strategic depth of the game.

### InGameEffectCard

The `InGameEffectCard` class extends the functionality of a base effect card for use within the game environment.

`InGameEffectCard` is a class that encapsulates the properties and behaviors of effect cards once they are in play. It inherits from `InGameCard` and integrates the foundational attributes with in-game specific functionality.

#### Features

- **Effect Abilities**: Holds a collection of `EffectAbility` instances, defining the special actions or bonuses that the card can exert within the game.
- **Initialization**: Constructs an `InGameEffectCard` using a base `EffectCard` and assigns the card's owner, ensuring the card's properties are correctly set for gameplay.
- **Synchronization**: Provides a `Reset` method to align the in-game card's properties with its base card, guaranteeing consistency with the original card definitions.

#### Behavior

- When an `InGameEffectCard` is instantiated, it pulls in the attributes from the corresponding `EffectCard`, including title, description, type, material, and any collector's information.
- The card's effect abilities are populated from a central `EffectAbilityLibrary`, enabling dynamic assignment and easy updates to abilities without the need to modify the card class directly.

#### Usage

- `InGameEffectCard` objects are used within the game to represent effect cards that have been played or are currently held by a player.
- The class is critical for gameplay logic that involves effect cards, as it manages the abilities that can be triggered and ensures the card's properties reflect its intended game function.

#### Design Considerations

- **Decoupling**: By separating the `InGameEffectCard` from the base `EffectCard`, the game maintains flexibility, allowing for in-game representations to have additional behaviors or temporary states without altering the base card definitions.
- **Extensibility**: The class design facilitates the addition of new effect abilities without changing the card's core structure, promoting scalability.

`InGameEffectCard` serves as a key component within the game's architecture, bridging the gap between a card's conceptual definition and its operational role during gameplay.

## Equipment

### EquipmentAbility

#### CancelInvocationAbility

The `CancelInvocationAbility` class defines an ability to negate or cancel the effects of an invocation in the game, providing a strategic defensive tool within the gameplay. When applied, this ability sets the `CancelEffect` property of an invocation card to true, effectively neutralizing the card's intended effect. This is a crucial mechanism that can be used to prevent an opponent from gaining an advantage through their invocation cards. The ability is reversible; it can be removed, resetting the `CancelEffect` property to false and restoring the original capabilities of the invocation card. This feature adds a layer of depth to the game, offering players the chance to counteract potent threats and to engage in more complex battles of wits with their adversaries.

#### CantBeAttackDestroyByInvocationAbility

The `CantBeAttackDestroyByInvocationAbility` class is an equipment ability in the game that offers a card immunity from being targeted or destroyed by invocation cards. When the ability is activated, it shields the designated invocation card from any attack, essentially making it invulnerable to direct assaults from an opponent's invocation cards. This protective effect is pivotal in maintaining strategic assets on the field, significantly impacting the flow of the game. The ability is not permanent and can be removed, at which point the card loses this protection and becomes susceptible to attacks once again. This toggling between states of vulnerability and invulnerability introduces a dynamic element to gameplay, prompting players to make timely and tactical decisions.

#### DirectAttackAbility

The `DirectAttackAbility` class encapsulates an equipment ability that endows an invocation card with the power to perform direct attacks on opponents, bypassing conventional combat rules. Upon activation, this ability allows the card holder to target the opponent directly, potentially altering the competitive landscape of the game. This can be a game-changing advantage, as it enables players to whittle down an opponent's life points directly. The functionality is reversible; once the ability is deactivated, the card reverts to normal attack protocols. This feature injects a layer of strategic depth into the game, encouraging players to deploy this ability judiciously.

#### EarnAtkDefAbility

The `EarnAtkDefAbility` is an equipment ability within the game that enhances an invocation card by providing additional attack and defense points. This augmentation can either be a fixed increase, denoted by the `bonusAttack` and `bonusDefense` values, or be proportional to the number of cards the player holds in their hand if `dependOnHandCardNumber` is set to true. Activation of this ability dynamically adjusts the offensive and defensive capabilities of the card, which could be pivotal during gameplay, affecting the outcome of battles and strategic maneuvers. Upon deactivation, the ability reverts the invocation card's stats back to their original state, highlighting the transient nature of the bonuses and the need for timely, tactical deployment.

#### MultiplyAtkDefAbility

The `MultiplyAtkDefAbility` is a class in the game that enhances an "invocation card" by multiplying its attack and defense stats with specified factors. This ability can be optionally configured to prevent the card from attacking. When applied, the card's attack and defense are multiplied by the provided factors, and if the option is set, the card's ability to attack is blocked. This effect persists across turns until it is removed, at which point the card's attack and defense values are reverted back to their original state by dividing by the same factors, and the card's ability to attack is restored if it had been blocked.

#### PreventAttackNewOpponentInvocationAbility

The `PreventAttackNewOpponentInvocationAbility` class defines an ability for an equipment card that prevents newly added opponent invocation cards from attacking. When an opponent's invocation card is added to the game, this ability is triggered, and the attack of that card is blocked. The effect of this ability is removed by re-allowing all of the opponent's previously blocked invocation cards to attack.

#### ProtectFromDestructionAbility

The `ProtectFromDestructionAbility` class grants an equipment card the ability to protect an invocation card from being destroyed a certain number of times, as specified by the `numberProtect` parameter. Each time the protected invocation card would be destroyed, this ability is triggered, and the destruction is prevented, decrementing the protection count. Once the limit of protections is reached, the equipment card is moved to the yellow cards pile, and the associated invocation card loses this protection. This ability adds a strategic layer to gameplay, allowing players to shield key cards from elimination.

#### SetAtkDefAbility

The `SetAtkDefAbility` class defines an ability for an equipment card that sets specific attack and defense values for an invocation card. It allows for the attack and defense values to be explicitly defined, overriding the card's current stats. If the values are set to null, the corresponding attribute will not be changed. When applied, the ability stores the original values before changing them, ensuring that they can be restored when the effect is removed. This ability is integral to gameplay strategies that require precise control over a card's offensive and defensive capabilities.

#### SwitchEquipmentCardAbility

The `SwitchEquipmentCardAbility` class allows an equipment card to enable a player to switch that equipment card with another one. When the ability is applied to an invocation card, if there is already an equipment card attached to it, the current equipment card is moved to the player's yellow cards pile and its effects are removed from the invocation card. This allows for strategic flexibility, enabling players to adapt their invocation cards to the changing demands of the game by swapping out equipment and their associated abilities. The ability emphasizes adaptability and dynamic gameplay.

### EquipmentAbility

The `EquipmentAbility` is an abstract base class defining common properties and methods for specific equipment abilities in the card game. It includes a name and description, a flag indicating if the ability can always be put into play, and virtual methods to apply and remove the ability's effects from invocation cards. Events for turn start, hand card changes, opponent card additions, and pre-destruction checks are also provided, with default implementations that can be overridden by derived classes. Each derived ability can manipulate the game state, such as altering attack and defense values or protecting cards from destruction, according to its specific logic.

### EquipmentAbilityLibrary

The `EquipmentAbilityLibrary` class acts as a centralized repository for managing different equipment abilities in the game. It contains a list of predefined abilities, which are then converted into a dictionary for quick lookup by name. This setup facilitates easy access to the various equipment abilities, each with unique effects and characteristics, such as multiplying attack/defense stats, setting specific values for these stats, or preventing attacks. The library’s design allows for efficient management and retrieval of abilities, enhancing the game's scalability and the ease of implementing new abilities into the system.

### EquipmentCardHandler

The `EquipmentCardHandler` class is responsible for managing the behavior of equipment cards within the game. It extends the `CardHandler` class, inheriting its basic functionalities and adding specific behaviors for equipment cards. This handler updates the game's UI elements to reflect the status of equipment cards, determining whether they can be equipped to invocation cards based on certain conditions such as available slots or special ability criteria. It also manages the actual placement of equipment cards onto invocation cards, triggering specific events when such an action occurs. This class ensures that the game's rules regarding equipment cards are followed and that their effects are correctly applied within the game environment.

### EquipmentFunctions

The `EquipmentFunctions` class in the `Cards.EquipmentCards` namespace manages the interactive functionalities of equipment cards within the game. It handles the display and user interaction for equipping these cards to invocation cards. At the start of the game, this class initializes event listeners for equipment-related events, and ensures these listeners are removed if the object is destroyed, maintaining clean lifecycle management.

When an equipment card is selected for use, `DisplayEquipmentPopUp` is called, which creates a pop-up that allows the player to choose an invocation card to equip. This selection considers whether the equipment card can always be put into play or if it requires an empty slot. Once an invocation card is chosen, the equipment card's abilities are applied to it, and the card is moved from the player's hand to the equipped state.

Additionally, the class handles the setup of the card selector UI, configuring positive and negative actions for the user's choice in the card selection process. This system provides a user-friendly way to manage equipment cards and their complex interactions with invocation cards during gameplay.

### InGameEquipmentCard

The `InGameEquipmentCard` class represents an equipment card within the game, including its associated abilities. It inherits from `InGameCard`, which provides basic card properties and behaviors. This class holds a reference to its base `EquipmentCard`, and it maintains a list of `EquipmentAbility` objects that define what the card can do in the game.

When an `InGameEquipmentCard` instance is created, it is initialized with the properties of its base card, such as title, description, and type. The class also pulls the corresponding abilities from the `EquipmentAbilityLibrary`, ensuring that the in-game representation of the card is equipped with the correct abilities as defined by the base card. The `Reset` method is used to synchronize the in-game card's details with its base card, allowing for accurate reflection of the card's abilities and details during gameplay. This class is essential for the execution of equipment abilities and their effects on the game's mechanics.

## Field

### FieldAbility

#### ChangeInvocationFamilyAbility

The `ChangeInvocationFamilyAbility` class represents a field ability that can alter the family attribute of a specific invocation card in a card game. This ability is initialized with a particular invocation card's name and the new family it should belong to. It includes methods to apply the family change effect, notify other field abilities of this change, and handle events related to the addition of invocation cards and the removal of field cards. The ability is designed to dynamically change the family of an invocation card, which can have various strategic implications during gameplay. It allows for in-game adaptation to the evolving conditions of play, offering players the opportunity to modify card attributes on the fly.

#### DrawMoreCardsAbility

The `DrawMoreCardsAbility` class encapsulates a field ability in a card game that allows a player to draw additional cards at the start of their turn. When initialized, it takes the number of extra cards to be drawn as a parameter. During the game, specifically at the turn's start, the ability checks if there are enough cards left in the player's deck. If so, it allows the player to draw the specified number of additional cards into their hand, providing a strategic advantage by increasing their options for play. This ability enhances the player's potential actions and can significantly affect the game's outcome by altering the player's card availability.

#### EarnATKDEFForFamilyAbility

The `EarnATKDEFForFamilyAbility` class provides a field ability that increases the attack and defense statistics of invocation cards based on their family affiliation. It is initialized with a specific card family and the bonus attack and defense values that should be applied to the cards of that family.

When the ability is in effect, it applies the bonus stats to all eligible invocation cards in the player's possession. It also handles scenarios where new invocation cards are added to the player's collection or an invocation card's family is changed, adjusting the card's stats accordingly. If the field card with this ability is removed, the bonus stats are reversed for all affected invocation cards.

This ability is a strategic game element that can influence the balance of power on the field by boosting the capabilities of cards belonging to a particular family, thereby encouraging players to build and adapt their strategies around specific card families.

#### EarnHPPerFamilyOnTurnStartAbility

The `EarnHPPerFamilyOnTurnStartAbility` class defines a field ability that grants players additional health points (HP) at the start of their turn based on the number of cards they have from a specific family. Upon initialization, this ability is associated with a particular card family and a fixed amount of HP that will be earned for each card of that family present in the player's invocation cards.

When a new turn begins, the ability calculates the total number of cards from the specified family and multiplies this by the predetermined HP value to determine the total HP gained. This HP is then added to the player's status, potentially providing a significant advantage by increasing the player's survivability in the game based on their deck composition. This mechanic encourages strategic deck building around particular families to maximize health gains.

#### GetCardFromFamilyIfSkipDrawAbility

The `GetCardFromFamilyIfSkipDrawAbility` class defines a field ability that gives players the option to skip their draw phase in order to obtain a card from a specified family directly from their deck or the yellow discard pile. This ability is tied to a particular family, and when a player's turn starts, they are prompted with a choice to forgo drawing a card to instead select a card of the specified family from the available cards.

If the player chooses to use this ability, a card selection UI is presented, allowing the player to pick a card from the family in question. The selected card is then moved from the deck or yellow cards to the player's hand, and the draw for that turn is skipped.

This ability adds a strategic layer to the game by offering players a trade-off between drawing a random card or targeting a specific family card that might be more beneficial to their current game situation.

### FieldAbility

The `FieldAbility` class serves as an abstract base for creating various types of field abilities in the game, each with a distinct effect on gameplay. It defines common properties such as the ability's name and description and includes virtual methods that can be overridden by derived classes to implement specific behaviors. These behaviors can be triggered when certain game events occur, such as the addition or removal of an invocation card, a change in an invocation card's family, or the start of a player's turn. This class provides a framework for extending the game's mechanics, allowing for a rich variety of strategic possibilities through different field abilities that players can leverage to gain advantages in the game.

### FieldAbilityLibrary

The `FieldAbilityLibrary` class acts as a repository for field abilities in the game, allowing for efficient access and management of various abilities. It stores a predefined list of `FieldAbility` objects, each representing a unique ability with specific effects on the game, such as modifying attack and defense stats or changing the family of invocation cards.

Upon initialization, the library converts the list of field abilities into a dictionary, facilitating quick retrieval of abilities by name. This design simplifies the process of activating and applying field abilities during gameplay and supports the game's expandability by making it easy to add new abilities to the library. The `FieldAbilityLibrary` ensures that field abilities are readily accessible and consistently managed throughout the game.

### FieldCardHandler

The `FieldCardHandler` class is designated to manage interactions specific to field cards within a card game. It inherits from `CardHandler`, providing specialized behavior for field cards. This includes handling the placement of field cards into the play area and updating the game's user interface to reflect the ability to place a field card, which is typically limited to when there is no current field card in play for the player.

Upon handling a field card, it checks whether the player's field card slot is available and sets the UI elements accordingly, such as enabling or disabling the button used to put a card into play. When a field card is placed, it triggers an event that manages the application of the field card's abilities or effects within the game environment. The `FieldCardHandler` ensures that the rules and mechanics related to field cards are appropriately enforced and reflected in the game's UI.

### FieldFunctions

The `FieldFunctions` class in the `Cards.FieldCards` namespace is responsible for managing the behavior and interaction of field cards within the game. It contains functionality for placing a field card onto the field and for executing the specific effects that come with each field card.

When the game starts, `FieldFunctions` sets up event listeners to respond to field card placement actions. These listeners call the `PutFieldCard` method whenever a field card event is triggered. The `PutFieldCard` method checks if the player can place a field card, ensures the mini card menu is hidden after the card is placed, then applies the card's abilities to the player's cards. It also interacts with the audio system to play music corresponding to the card's family.

Upon destruction of the object, `FieldFunctions` ensures that it unsubscribes from the events to prevent any potential memory leaks or unwanted behavior. This class is an integral part of the game's functionality, linking the field cards' in-game use with their defined abilities and ensuring a cohesive audio-visual experience.

### InGameFieldCard

The `InGameFieldCard` class represents a field card within a card game, extending the functionality of a generic `InGameCard`. It contains runtime behaviors and properties specific to field cards, such as a list of `FieldAbility` objects that dictate the card's effects on the game when it is in play.

When an `InGameFieldCard` is instantiated, it is initialized with data from a base `FieldCard`, including the card's owner, title, description, type, material, collector information, and associated card family. It also populates its list of abilities by referencing the `FieldAbilityLibrary` using the abilities specified in the base `FieldCard`.

The `Reset` method is used to synchronize the in-game card's details with its base card, ensuring that the in-game representation accurately reflects the base card's intended effects and properties. This class is vital for applying field-specific rules and abilities during gameplay, affecting the state of the game as long as the card remains on the field.

## Invocation

### Ability

#### BackToHandAfterDeathAbility

The `BackToHandAfterDeathAbility` class represents a unique ability in the game that allows an associated card to be returned to the player's hand after it has been 'killed' or 'died' a certain number of times. This ability can be set to trigger a specific maximum number of times, with the option for the effect to occur infinitely (denoted by setting this number to 0).

Upon the death of the card, the ability checks if the conditions for returning the card to the hand are met, including whether the card's death count is within the specified limit and if the card is not already in the player's hand. If these conditions are fulfilled, the card is moved from the yellow (discard) pile back into the player's hand, ready to be used again.

This ability adds a strategic layer to gameplay by giving cards a form of resilience, allowing players to reuse key cards multiple times within a game. It enhances the longevity of certain cards and can significantly influence a player's strategy and tactics.

#### CanOnlyAttackItselfAbility

The `CanOnlyAttackItselfAbility` class defines an ability in the card game that restricts the associated card to be the only one attackable. This ability is unique in that it changes the normal combat dynamics by forcing a card to attract all attacks.

Upon applying this ability, the card's 'aggro' status is set to true, indicating that it is compelled to attract attacks. The ability also includes methods to cancel and reactivate this effect. When the ability is canceled, the card's aggro status is set back to false, allowing it to resume normal attack patterns. Reactivating the ability sets the aggro status back to true, re-imposing the self-targetting restriction.

This ability introduces a unique strategic element to gameplay, as it can be used to limit the offensive capabilities cards, possibly as part of a larger strategic play or as a drawback to balance a card with powerful abilities.

#### CantBeAttackAbility

The `CantBeAttackAbility` class represents an ability in the card game that provides protection to the associated card from being attacked, based on specific conditions. The ability is linked to a particular card family, with an option to apply universally to any family.

When this ability is in effect, it evaluates whether the conditions for protection are met. If so, the associated card is marked as unable to be attacked (`CantBeAttack`). This protection status is dynamically updated based on the game state, specifically when cards are added or removed from play.

The ability also includes methods to apply, cancel, and reactivate this protective effect. When the ability is applied or reactivated, it checks if the card meets the criteria for protection and updates its attackable status accordingly. Conversely, canceling the ability removes this protection, making the card attackable again.

This ability adds strategic depth to the game, allowing players to shield key cards from opponents' attacks under certain conditions, influencing both the player's and the opponent's tactical decisions.

#### CantLiveWithoutAbility

The `CantLiveWithoutAbility` class represents an ability in a card game that creates a dependency of a card's existence on the presence of other specific cards or cards from a certain family. This ability can be initialized with a list of card titles or a card family that the card's existence depends upon.

The core functionality of this ability lies in its power to remove the card from play if the necessary conditions are not met. For example, if the required cards (as per the provided list or family) are not present in the player's collection, the card with this ability is removed from play and placed in the discard pile (yellow cards).

The ability includes methods to apply and reactivate this effect, as well as to handle the scenario when a card is removed from play. Each time these events occur, the game checks whether the conditions for the card's survival are met. If not, the card is removed, adding a layer of strategic depth to the game. Players must carefully consider their card plays and deck composition to ensure the survival of key cards with this ability.

#### CopyAtkDefAbility

The `CopyAtkDefAbility` class represents an ability in a card game that allows a card to copy the attack and defense statistics of another specified card. This ability is targeted towards a particular card, identified by its name (`cardToCopyName`), and applies its effect under certain conditions.

Key functionalities of this ability include:

1. **Copying Stats**: If the target card is present in the player's invocation cards, the ability holder's attack and defense values are replaced with those of the target card.

2. **Applying Effect**: The ability checks for the presence of the target card and copies its stats when applied.

3. **Canceling Effect**: The ability can be canceled, which resets the ability holder's stats to their base values.

4. **Reactivating Effect**: Reactivating the ability reapplies the copying effect.

5. **Handling Turn Start and Card Addition/Removal**: The ability also includes logic for copying stats at the start of a turn, when new cards are added, or when cards are removed from the player's collection.

The `CopyAtkDefAbility` adds a layer of strategic flexibility to the game, allowing players to adapt their cards' strengths dynamically based on the presence of other cards in play. This can lead to varied and situational tactics depending on the composition of a player's deck and the current state of the game.

#### DefaultAbility

The `DefaultAbility` class serves as a basic implementation of an ability in the card game, primarily used as a foundational element for more complex abilities. This class provides a minimalistic structure, focusing on setting up the fundamental properties of an ability, such as its name and description.

Upon instantiation, the `DefaultAbility` class is initialized with a specific name and a description that defines the ability. However, it does not include any added behaviors, effects, or logic beyond this basic setup.

This class can be used as a placeholder or a starting point for developing more detailed and functionally rich abilities. It ensures that every ability, even the simplest ones, has a consistent structure and basic properties like name and description, which are essential for game mechanics and player understanding.

#### DestroyFieldAtkDefAttackConditionAbility

The `DestroyFieldAtkDefAttackConditionAbility` is an ability in the card game that allows a player to destroy a field card under specific conditions related to the attacking card's attack and defense statistics. This ability is characterized by two key factors: an attack division factor and a defense division factor. These factors determine the cost to the attacking card's stats for using this ability.

When activated, the ability presents the player with an option to destroy a field card. If the player chooses to proceed, the attacking card’s attack and defense are divided by the respective factors (either attack or defense division factor, depending on the condition). This reduction in stats represents the cost of using the ability to remove a field card from play.

The ability includes logic for handling the destruction of the field card, whether it belongs to the player or the opponent, and the subsequent adjustment of the attacking card's attributes. It also includes UI components to display messages and choices to the player, enhancing the interactivity and strategic decision-making in the game.

This ability introduces a tactical layer to gameplay, allowing players to remove potentially beneficial field cards from play at the expense of weakening their own card's stats, thus balancing risk and reward.

#### DrawCardsAbility

The `DrawCardsAbility` class in the card game allows players to draw a specific number of cards from their deck. This ability is initialized with the number of cards to be drawn, as specified by the `numberCards` parameter.

Key features of the `DrawCardsAbility` include:

1. **Drawing Cards**: When the ability is activated, it enables the player to draw the specified number of cards. If the player's deck contains fewer cards than the specified number, the player draws all remaining cards in the deck.

2. **User Interaction**: The ability includes a user interface component that displays a message and offers the player a choice to draw the cards. This interactive element enhances player engagement and decision-making.

3. **Balancing Deck Size**: The ability is designed to handle cases where the deck size is smaller than the number of cards to be drawn, ensuring that the game mechanics remain balanced and functional regardless of the current deck size.

The `DrawCardsAbility` adds a strategic element to gameplay, as it provides players with the opportunity to replenish their hand, potentially gaining access to key cards that could influence the game's outcome. It is a valuable tool for deck cycling and maintaining momentum in the game.

#### GetFamilyInDeckAbility

The `GetFamilyInDeckAbility` class represents an ability in the card game that allows players to retrieve cards of a specific family from their deck. This ability is targeted towards a particular card family, defined during the ability's initialization.

Key functionalities of this ability include:

1. **Targeting a Card Family**: The ability is focused on a specific card family (`family`), allowing players to search their deck for cards belonging to this family.

2. **User Interaction**: When activated, the ability presents the player with a UI dialog offering the option to select a card from the specified family. This interaction enhances player engagement and decision-making.

3. **Retrieving and Moving Cards**: If the player selects a valid card from the specified family, that card is moved from the deck to the player's hand, effectively drawing the card into play.

4. **UI Components**: The ability includes UI elements for displaying messages and choices to the player, adding an interactive dimension to the game experience.

The `GetFamilyInDeckAbility` adds a strategic element to gameplay, enabling players to target specific types of cards within their deck, which can be crucial for executing particular strategies or responding to in-game scenarios.

#### GetSpecificCardAfterDeathAbility

The `GetSpecificCardAfterDeathAbility` class represents an ability in the card game that allows players to retrieve a specific card from their deck when a designated card dies. This ability is a specialized version of the `GetSpecificCardFromDeckAbility`, tailored to activate upon the death of a specific card.

Key aspects of this ability include:

1. **Specific Card Retrieval**: The ability is tied to a specific card name (`cardName`), which it aims to retrieve from the player's deck.

2. **Activation Upon Death**: The primary trigger for this ability is the death of a designated invocation card. When the card with this ability dies, the ability attempts to retrieve the specified card from the deck.

3. **Conditional Activation**: The ability checks if the dead card's title matches the invocation card's title and whether the effect is not canceled before proceeding to retrieve the specified card.

4. **Implementation of Base Class Methods**: While the `ApplyEffect` method of the base class (`GetSpecificCardFromDeckAbility`) is overridden, it currently does not implement additional functionality. The main functionality is provided by the overridden `OnCardDeath` method, which includes the logic to retrieve the specific card post-death.

The `GetSpecificCardAfterDeathAbility` adds a strategic layer to gameplay, allowing players to plan for future turns by ensuring access to specific cards upon the loss of key cards. This ability can be crucial for maintaining momentum or executing specific strategies despite the loss of important cards.

#### GetSpecificCardFromDeckAbility

The `GetSpecificCardFromDeckAbility` class in the card game allows players to retrieve a particular card from their deck. This ability is centered around a specific card, identified by its name (`CardName`).

Key features of the `GetSpecificCardFromDeckAbility` include:

1. **Targeted Card Retrieval**: The ability focuses on a specific card, with the aim of retrieving that card from the player's deck.

2. **Card Retrieval Process**: When activated, it checks if the targeted card exists in the player's deck. If found, the card is moved from the deck to the player's hand, effectively adding it to the player's available cards.

3. **User Interaction**: The ability includes a UI dialog offering the player the choice to retrieve the card. This interaction adds a layer of decision-making for the player.

4. **Flexible Gameplay Strategy**: By allowing players to target specific cards in their deck, this ability provides a strategic tool for players to access key cards that can influence the game's outcome.

The `GetSpecificCardFromDeckAbility` enhances the game by enabling strategic deck utilization, allowing players to plan and execute game strategies around specific cards in their deck.

#### GetSpecificCardFromDeckOrYellowCardAbility

The `GetSpecificCardFromDeckOrYellowCardAbility` class represents an ability in the card game that allows players to retrieve a specific card from either their deck or the yellow cards (discard pile). This ability is an extension of the `GetSpecificCardFromDeckAbility`, with the added functionality of accessing cards from the yellow cards.

Key features of the `GetSpecificCardFromDeckOrYellowCardAbility` include:

1. **Targeted Card Retrieval**: The ability focuses on retrieving a specific card, identified by `CardName`, from the player's deck or yellow cards.

2. **Checking Multiple Locations**: When activated, the ability checks both the deck and the yellow cards to determine if the specified card is present in either location.

3. **Retrieving and Moving Cards**: If the targeted card is found, it is moved from its current location (deck or yellow cards) to the player's hand.

4. **User Interaction**: The ability includes a user interface component that displays a message and offers the player a choice to retrieve the card. This interaction enhances player engagement and decision-making.

The `GetSpecificCardFromDeckOrYellowCardAbility` adds a strategic element to gameplay by providing players with a wider range of options to access specific cards, potentially pivotal for their game strategy. This ability can be crucial for regaining key cards that have been discarded earlier in the game.

#### GetTypeCardFromDeckWithoutAttackAbility

The `GetTypeCardFromDeckWithoutAttackAbility` class represents an ability in the card game that allows players to retrieve a specific type of card from their deck, with the additional condition that the card's retrieval negates the player's ability to attack in that turn.

Key features of this ability include:

1. **Target Card Type**: The ability is focused on a particular card type (`type`), allowing players to search their deck for cards of this type.

2. **Retrieving Card Process**: When activated, it checks if there is a card of the specified type in the player's deck. If found, the player is presented with an option to select one of these cards.

3. **Player Interaction**: The ability includes a UI dialog that offers the player the choice to retrieve a card. This interactive element adds depth to player decision-making.

4. **Attack Trade-off**: Retrieving a card using this ability results in the player losing their attack turn, indicated by setting the `SetRemainedAttackThisTurn` to 0. This trade-off adds a strategic layer to using the ability.

5. **No Card Scenario**: If there are no cards of the specified type in the deck, a message box is displayed to inform the player.

The `GetTypeCardFromDeckWithoutAttackAbility` introduces a strategic consideration to gameplay, where players can access specific types of cards from their deck at the cost of their attack turn, potentially influencing their tactical approach in various game situations.

#### GiveAtkDefFamilyAbility

The `GiveAtkDefFamilyAbility` class in the card game provides an ability to enhance the attack and defense values of all cards belonging to a specific family. This ability focuses on improving the stats of a group of cards based on their family association.

Key aspects of the `GiveAtkDefFamilyAbility` include:

1. **Family Targeting**: The ability is designed to target a specific card family (`family`), allowing it to affect all cards in the player's collection that belong to this family.

2. **Attack and Defense Boost**: The ability provides specified increments in attack (`attack`) and defense (`defense`) to the targeted family cards.

3. **Applying and Reverting Effects**: When the ability is activated, it applies the attack and defense increments to all applicable cards. Conversely, the `CancelEffect` method reverses these changes, reverting the cards to their original stats.

4. **Card Addition and Removal Handling**: The ability also includes logic for dynamically adjusting the stats of the family cards when new cards are added or existing cards are removed from the player's collection. This ensures that the ability's effect is consistently applied to all relevant cards.

5. **Event-Based Activation**: The ability can be triggered by various events, such as the addition or removal of cards, enhancing its versatility and strategic importance in different gameplay scenarios.

The `GiveAtkDefFamilyAbility` adds a strategic depth to gameplay, allowing players to strengthen a group of cards based on their family, which can be crucial for executing particular strategies or responding to in-game challenges.

#### GiveAtkDefToFamilyMemberAbility

The `GiveAtkDefToFamilyMemberAbility` class in the card game enables a player to transfer attack and defense values from one card to another member of the same card family. This ability strategically enhances the capabilities of a specific family member by reallocating stats.

Key aspects of the `GiveAtkDefToFamilyMemberAbility` include:

1. **Family Focus**: The ability is applicable to cards within a specified family (`family`), allowing for targeted stat boosts within that family.

2. **Stat Transfer Mechanism**: When activated, this ability allows the player to choose a card from the same family and transfer attack and defense values from the card possessing the ability to the chosen card.

3. **User Interaction and Choice**: The ability involves user interaction where the player selects the recipient card from a list of valid family members. This choice is facilitated by UI dialogs and card selectors.

4. **Reverting Effects on Card Death**: If the card that gave away its stats dies, the ability reverts the transferred stats back to their original state on the recipient card.

5. **Action Possibility Check**: The ability includes a method to check if the action of transferring stats is possible, based on the presence of other family member cards.

6. **Event-Based Activation**: The ability can be triggered by various game events, such as a card's action being touched or a card's death, adding layers of strategic complexity to gameplay.

The `GiveAtkDefToFamilyMemberAbility` introduces a tactical element to the game, encouraging players to make strategic decisions about stat allocation among family members to gain advantage or respond to in-game situations.

#### InvokeSpecificCardAbility

The `InvokeSpecificCardAbility` class in Unity represents an ability within a game to invoke a specific card from the deck based on its name. This ability is a part of the game's mechanics where certain actions can result in summoning specific cards to the field.

Key Features:

1. **Initialization**: It initializes with the name of the ability, a description, and the specific card's name that it can invoke.

2. **Ability Effect Application**: The core functionality is in the `ApplyEffect` method. This method checks if the specified card is present in the player's deck. If the card is found, it offers the player an option to invoke this card.

3. **User Interaction**: When the ability is activated, a message box is displayed with options to either proceed with the invocation or cancel it. This interaction is managed through a `MessageBoxConfig` object, which sets the title, message, and buttons for the message box.

4. **Card Invocation**: If the player chooses to invoke the card, it is removed from the deck and added to the player's field of invocation cards.

5. **Card Identification**: The ability identifies the specific card to invoke based on the card name provided during initialization.

This ability is useful in game scenarios where invoking specific cards can strategically benefit the player, allowing for more dynamic and strategic gameplay. It adds a layer of depth to the game, as players can plan and execute strategies based on the availability of specific cards in their deck.

#### InvokeSpecificCardChoiceAbility

The `InvokeSpecificCardChoiceAbility` class in Unity represents an ability within a game that allows a player to choose and invoke a specific card from the deck. This class is part of the game mechanics that offer players strategic choices for summoning specific cards onto the field.

Key Features:

1. **Initialization**: The ability is initialized with its name, a description, and a list of card names that can be chosen for invocation.

2. **Ability Effect Application**: The main functionality lies in the `ApplyEffect` method, which checks if any of the specified cards are present in the player's deck. If so, it provides the player with a choice to invoke one of these cards.

3. **User Interaction and Choice**: A message box is displayed with options for the player to choose from the specified cards. This is managed through a `MessageBoxConfig` object, which sets the title, message, choice options, and buttons.

4. **Card Invocation**: If the player chooses to invoke a card, the selected card is removed from the deck and added to the player's field of invocation cards.

5. **No Card Scenario**: If there are no eligible cards in the deck or if the player chooses not to invoke, a message box is displayed informing the player that no card was invoked.

This ability is essential for games requiring strategic decision-making, as it allows players to select from multiple options, potentially influencing the game's outcome based on their choice. The ability to choose from a list of cards adds a layer of strategy and unpredictability to the gameplay.

#### KillBothCardsIfAttackAbility

The `KillBothCardsIfAttackAbility` in Unity represents an ability within a game that triggers the destruction of both the attacker and the attacked cards under specific conditions during gameplay.

Key Features:

1. **Initialization**: The ability is initialized with a name and a description, detailing its functionality within the game.

2. **Attack Handling**: This ability primarily functions in scenarios where one card attacks another. When the card with this ability performs an attack or is attacked, specific conditions are checked.

3. **Destruction Condition**: The main condition for the ability to activate is the participation of the card (either as attacker or attacked) in combat. If the condition is met, both the attacker and attacked cards are moved to their respective player's 'yellow cards' area, effectively removing them from active play.

4. **Player Card Collections**: The ability interacts with the players' card collections, specifically the 'InvocationCards' and 'YellowCards' lists. It moves the involved cards to the 'YellowCards' list, which is typically used for cards that are out of active play.

5. **Cancellation Check**: The ability includes a check for whether its effect is currently canceled. If the effect is canceled, it doesn't proceed with the destruction of the cards.

This ability adds a strategic layer to the game, as players must consider the risk of losing their card when attacking or being attacked by a card with this ability. It encourages players to think critically about their offensive and defensive strategies.

#### KillOpponentInvocationCardAbility

The `KillOpponentInvocationCardAbility` in Unity is an ability designed for gameplay that enables a player to eliminate an opponent's Invocation Card.

Key Features:

1. **Initialization**: The ability is initialized with a specific name and description, detailing its role in the game.

2. **No Card Scenario**: It includes a function to display an informational message when no opponent Invocation Card is available to be destroyed. This enhances user experience by providing clear feedback.

3. **Ability Application**: The core function of this ability allows the player to target and destroy an opponent's Invocation Card. This is a critical gameplay mechanic, offering strategic depth to player interactions.

4. **Card Selection Process**: If the opponent has more than one Invocation Card, the player is presented with a choice of which card to destroy. This is facilitated through a card selector interface, enhancing player engagement and decision-making.

5. **Card Management**: Upon activation, the targeted Invocation Card is removed from the opponent’s active play area and added to their 'Yellow Cards' area, signifying its removal from the game. This interacts with the player's card collections, specifically handling the 'InvocationCards' and 'YellowCards' lists.

6. **User Interface Integration**: The ability integrates with the game's UI, particularly through message boxes and card selectors, to provide a seamless and intuitive user experience.

This ability adds a significant tactical element to the game, allowing players to directly influence their opponent's resources and strategy.

#### LimitTurnExistenceAbility

The `LimitTurnExistenceAbility` in Unity is an ability tailored for gameplay mechanics that restricts the duration of an invocation card's presence on the playing field to a specific number of turns.

Key Features:

1. **Initialization**: This ability is initialized with a name, a descriptive explanation, and a specific limit on the number of turns an invocation card can remain on the field.

2. **Turn-Based Mechanism**: The core functionality is invoked at the start of each turn. It checks the number of turns an associated invocation card has been on the field.

3. **Duration Check and Action**: If the invocation card's presence on the field reaches or exceeds the specified turn limit, the ability triggers an action to remove the card from active play.

4. **Card Removal Process**: The invocation card, upon reaching its turn limit, is moved to the player's 'Yellow Cards' collection. This represents the card being effectively removed from the field.

5. **Handling Equipment Cards**: If the invocation card has an attached equipment card, this ability also moves the equipment card to the 'Yellow Cards' collection, ensuring consistency in the game state.

6. **Effect Cancellation Check**: Before executing its primary function, the ability checks if its effect has been canceled, adding a layer of interactivity and dynamic response to game events.

The `LimitTurnExistenceAbility` introduces a strategic layer to gameplay, compelling players to make the most out of their invocation cards within a set duration, thus adding a layer of complexity and planning to the game.

#### OptionalChangeFieldFromDeckAbility

The `OptionalChangeFieldFromDeckAbility` in Unity is an ability that allows players to optionally change the current field card with another from their deck.

Key Features:

1. **Purpose**: This ability provides players with the option to replace the existing field card with another one from their deck.

2. **Initialization**: The ability is set up with a unique name and a description that outlines its function.

3. **User Interface Interaction**: It involves displaying message boxes and card selector interfaces to the player, allowing them to make informed decisions.

4. **Checking for Availability**: The ability first checks the player’s deck for any field cards. If available, it proceeds; if not, it displays an informational message.

5. **Player Choice**: The ability offers the player a choice to replace the field card. This is done through a message box that asks if the player wants to set a new field card.

6. **Card Selection Process**: If the player opts to change the field card, they are presented with a card selection interface displaying all field cards available in the deck.

7. **Field Card Replacement**: Upon selection, the chosen card becomes the new field card. If a field card is already present, it is moved to the player's 'YellowCards' collection before the new card is set.

8. **User Feedback**: If there are no field cards in the deck or if the player chooses not to set a new field card, an OK message box is displayed to inform them accordingly.

This ability adds a strategic layer to gameplay, allowing players to adapt their strategy by changing the field card to better suit their current situation or game plan.

#### OptionalSacrificeForAtkDefAbility

The `OptionalSacrificeForAtkDefAbility` in Unity is an ability that enables a player to optionally sacrifice a specific card to gain a boost in Attack (Atk) and Defense (Def) values.

Key Features:

1. **Purpose**: This ability allows players to strategically sacrifice a card in exchange for a significant boost in another card's Atk and Def.

2. **Initialization**: The ability is set up with its name, description, and parameters including the name of the card to be sacrificed, the required field, and the new Atk and Def values to be assigned.

3. **Activation Conditions**: The ability can only be activated if the required field is present and the specific card to be sacrificed is part of the player's cards.

4. **User Interaction**: Players are presented with a choice through a message box, asking if they want to sacrifice the specified card for a boost in Atk and Def.

5. **Effect Application**: On accepting, the specified card is removed from the player’s invocation cards and added to the 'YellowCards' collection, and the Atk and Def values of the ability holder’s card are updated.

6. **Reset on Death**: If the card benefiting from this ability dies, its Atk and Def values are reset to their base stats.

7. **Strategic Implication**: This ability introduces a layer of strategic depth, as players must weigh the cost of sacrificing a card against the potential benefits of increased Atk and Def.

This ability is particularly useful in scenarios where a significant power boost could turn the tide of the game, but it requires careful consideration due to the permanent sacrifice of a card.

#### ProtectBehindDuringAttackAbility

The `ProtectBehindDuringAttackAbility` in Unity is an ability designed to provide protection to a specified card during an attack by redirecting the attack to a protector card.

Key Features:

1. **Purpose**: This ability is focused on protecting a specific card during combat by ensuring that another card (the protector) takes the hit instead.

2. **Initialization**: The ability is initialized with its name, description, and the name of the protector card.

3. **Attack Redirection**: During an attack, if the targeted card is the one protected by this ability and the protector card is present among the player's cards, the attack is redirected to the protector card.

4. **Condition Checking**: The ability checks whether its effect is canceled and whether the protector card exists in the player's invocation cards before applying its effect.

5. **Targeted Protection**: The ability only applies its protection when the specified card is attacked, ensuring focused and strategic use.

6. **Game Mechanics Integration**: The ability integrates seamlessly into the game's attack and defense mechanics, adding a layer of strategic depth to card placement and attack decisions.

This ability is particularly useful in protecting key cards from being attacked, thereby preserving them for crucial moments in the game. It requires strategic foresight from the player to ensure the protector card is in play and strategically positioned to offer protection.

#### ProtectBehindDuringAttackDefConditionAbility

The `ProtectBehindDuringAttackDefConditionAbility` in Unity is an ability designed to offer protection to a card positioned behind another card during an attack, based on specific defensive conditions.

Key Features:

1. **Primary Function**: The ability is specifically created to protect a card that is positioned behind another card during combat, provided certain defensive conditions are met.

2. **Initialization Parameters**: It is initialized with its unique name and a descriptive explanation of its function.

3. **Defensive Condition**: The protective function of this ability is triggered based on the defensive strength of the cards involved. It compares the defense values of various cards to determine the applicability of the protection.

4. **Activation Mechanism**: The ability is activated when a card is attacked. It assesses if the attacked card meets the required conditions (i.e., having a card behind it with a higher defense value) for the ability to take effect.

5. **Protection Strategy**: If conditions are met, the ability redirects the attack to a card behind the initially attacked card, effectively protecting the latter.

6. **Multiple Card Handling**: In scenarios where multiple cards meet the defensive condition, the ability can randomly choose one of these cards to redirect the attack to, adding an element of unpredictability.

7. **Fallback Behavior**: In cases where the conditions are not met, the ability defaults to standard attack handling, ensuring consistent game mechanics.

This ability is particularly strategic in gameplay, requiring players to consider card placement and defense values carefully. It adds depth to the defense mechanism in the game, encouraging players to strategize their card arrangements to maximize protective effects.

#### SacrificeCardAbility

The `SacrificeCardAbility` in Unity is designed to allow players to strategically sacrifice a specific card during gameplay. This ability is part of the game's mechanics and can be crucial in certain gameplay scenarios.

**Key Features:**

1. **Purpose**: It enables a player to deliberately remove a specific card from play, known as sacrificing.

2. **Initialization Parameters**: The ability is initialized with its unique name, a description detailing its function, and the specific name of the card that can be sacrificed.

3. **Card Selection**: It specifically targets a card with a given name for sacrifice. The ability identifies this card from the player's collection of cards.

4. **Effect Application**: Upon activation, the targeted card is removed from the player's active play area (Invocation Cards) and moved to the YellowCards collection, signifying its sacrifice.

5. **Equipment Card Handling**: If the card being sacrificed has an equipment card attached, this equipment card is also moved to the YellowCards collection. This feature ensures that any additional elements associated with the sacrificed card are also appropriately handled.

6. **UI Integration**: The ability includes parameters for canvas transformation, enabling it to integrate with the game's UI for displaying any necessary elements or messages related to the ability's activation.

7. **Opponent Consideration**: While the primary effect concerns the player's own cards, the ability's parameters also include the opponent's cards, allowing for potential future expansions or interactions with opponent cards.

This ability is significant in gameplay strategies where sacrificing a card can lead to advantageous outcomes or fulfill certain conditions required for other game mechanics or abilities. It adds an element of depth and strategy, as players must weigh the benefits and consequences of sacrificing a specific card.

#### SacrificeCardMinAtkMinDefFamilyNumberAbility

The `SacrificeCardMinAtkMinDefFamilyNumberAbility` in Unity is an ability that allows players to strategically sacrifice a specified number of cards based on certain criteria like minimum attack, minimum defense, and card family. This ability enhances the strategic depth and decision-making in gameplay.

**Features of the Ability:**

1. **Selective Sacrifice**: It enables the player to sacrifice cards that meet specific criteria: a minimum attack value (`minAtk`), a minimum defense value (`minDef`), belonging to a particular card family (`family`), and a specified number of cards to be sacrificed (`numberCard`).

2. **Initialization Parameters**: The ability is initialized with a name, a description, minimum attack and defense values, a card family, and the number of cards to be sacrificed.

3. **Card Selection for Sacrifice**: The ability includes a method to retrieve a list of valid invocation cards from the player's collection that meet the specified criteria.

4. **UI Integration for Selection Process**: Incorporates a card selection interface allowing the player to choose from the valid cards for sacrifice. It displays a message prompting the player to choose the required number of cards for sacrifice.

5. **Sacrifice Execution**: When the conditions are met, the selected invocation cards are moved from the player's active area to the YellowCards collection, signifying their sacrifice.

6. **Handling of Multiple Cards**: The ability can handle scenarios where multiple cards need to be sacrificed, with UI elements and messages adapted accordingly.

7. **Strategic Depth**: By allowing players to sacrifice cards based on specific attributes, it adds an additional layer of strategic depth. Players must consider the best cards to sacrifice based on the current game state and their strategy.

This ability is particularly useful in gameplay scenarios where sacrificing cards with certain attributes can lead to advantageous positions, fulfill certain game mechanics, or activate other abilities. It encourages players to think strategically about the composition of their deck and the timing of using this ability.

#### SacrificeToInvokeAbility

The `SacrificeToInvokeAbility` in Unity represents a strategic game mechanic where players can sacrifice one of their cards to invoke another card. This ability adds depth to gameplay, offering players tactical choices to manage their resources effectively.

**Key Features:**

1. **Ability Initialization**: The ability is defined with a specific name and a detailed description, providing clarity on its functionality.

2. **Action-Based Ability**: It is marked as an action, meaning it requires player interaction to be activated.

3. **Sacrifice Mechanism**: Players can sacrifice a card from their collection, specifically from the YellowCards that are invocation-type and non-collector cards.

4. **Invocation Process**: After sacrificing a card, the player can invoke another card, adding it to their active game area.

5. **UI Integration**: The ability integrates with the game's UI, displaying messages and selections through `MessageBoxConfig` and `CardSelectorConfig`. This includes messages for scenarios where no cards are invoked or when selecting a card for invocation.

6. **Action Possibility Check**: The ability includes a check to determine if the action can be executed based on the player’s current card collection.

7. **Action Execution**: On triggering the ability, players are presented with a choice of invocation cards they can bring into play by sacrificing another card.

8. **Flexible Card Selection**: The ability uses `CardSelectorConfig` to allow players to choose which card to sacrifice and which to invoke, providing strategic flexibility.

9. **Handling of Negative Scenarios**: The ability is designed to handle scenarios where no valid cards are available for invocation, displaying appropriate messages to inform the player.

This ability is particularly beneficial in games where resource management and strategic planning are key. It allows players to make significant game-changing decisions, balancing the risk of sacrificing a card with the potential gains of invoking a more beneficial one.

#### SendAllCardsInHand

The `SendAllCardsInHand` ability in Unity is a game mechanic designed to reset the playing field by returning almost all invocation cards from the field to the players' hands. This ability can dramatically change the game state, offering a reset or defense mechanism against overwhelming board setups.

**Key Features:**

1. **Initialization**: Defined with a unique name and description, clarifying its purpose and effect in the game.

2. **Card Selection Process**: It selectively identifies invocation cards to be removed from the field, excluding the card that activates this ability.

3. **Field-to-Hand Transfer**: Moves identified invocation cards from both the current player's and the opponent's field back to their respective hands.

4. **Turn-Based Activation**: This ability can be triggered at the start of each turn, continually affecting the game as long as it remains active and its conditions are met.

5. **Effect Cancellation Check**: Includes a check for whether the effect of this ability has been canceled, ensuring it only activates when applicable.

6. **Wide Impact**: Affects all invocation cards on the field from both players, except for the one invoking this ability, making it a powerful tool for altering the game's dynamics.

7. **Strategic Reset**: Provides a strategic reset or a defensive maneuver, potentially disrupting the opponent's strategy or salvaging the player's position in challenging scenarios.

This ability is especially useful in games where board control and hand management are critical elements of strategy. It allows players to disrupt the current state of play, potentially turning the tide of the game or stalling an opponent's momentum.

#### SkipOpponentAttackAbility

The `SkipOpponentAttackAbility` in Unity is an ability designed to strategically disrupt the opponent's gameplay by skipping their attack turn. This ability adds a layer of tactical depth to games, particularly where turn-based combat and strategy play a significant role.

**Key Features:**

1. **Initialization and Description**: The ability is initialized with a unique name and a detailed description, making its purpose clear within the game's context.

2. **User Interaction**: Incorporates interactive message boxes with OK buttons, providing feedback or requiring player confirmation when the ability is activated.

3. **Effect Application**: The core function of this ability is to skip the opponent's attack. It intelligently scans the opponent's cards, offering the player a choice to block an attack from one of the opponent's invocation cards.

4. **UI Integration**: Utilizes Unity's UI elements, like message boxes and card selectors, for a seamless in-game experience.

5. **Turn-Based Activation**: Can be triggered at the start of each turn, assessing whether to apply the effect based on the current game state and player turn.

6. **Game State Management**: Interacts with the game's state manager to ensure the effect is applied correctly depending on the turn sequence.

7. **Strategic Disruption**: Provides players with a powerful tool to disrupt the opponent’s strategy, potentially protecting the player from imminent attacks or disrupting the opponent's game plan.

8. **Conditional Execution**: Includes checks for whether the effect of this ability has been canceled, ensuring it only activates when applicable and under the right conditions.

9. **Balanced Gameplay Impact**: While powerful, the ability is designed to maintain balance, offering a strategic advantage without overwhelming dominance.

This ability is particularly useful in card-based strategy games or turn-based combat scenarios, where controlling the opponent's ability to attack can provide a critical advantage or turn the tide in a challenging situation.

#### WinAtkDefFamilityAtkDefConditionAbility

The `WinAtkDefFamilityAtkDefConditionAbility` class in Unity represents a specialized ability for card-based games. This ability grants attack and defense boosts to cards of a specified family, contingent upon additional attack and defense conditions.

**Key Features:**

1. **Inheritance**: It extends `WinAtkDefFamilyAbility`, inheriting its basic functionality while adding new conditions for applying the ability.

2. **Initialization**:
   - Constructor accepts parameters for the name, description, card family, attack, defense, and additional attack and defense conditions.
   - Sets up the unique conditions (`invocationAtkCondition`, `invocationDefCondition`) that need to be met for the ability to activate.

3. **Effect Application**:
   - `ApplyEffect` method: Applies the ability's effect to the player's cards, enhancing their attack and defense based on the number of cards that meet the specified conditions.
   - Utilizes `ApplyPower` to calculate and apply the power boosts.

4. **Condition Checking**:
   - `CheckIfCardMeetCondition`: Determines if a card satisfies the specific attack and defense criteria alongside belonging to the specified family.
   - `CalculateNumberInvocation`: Counts how many cards meet these conditions.

5. **Dynamic Adjustment**:
   - `IncrementAtkDefInvocationCard` and `DecrementAtkDefInvocationCard`: Adjusts the attack and defense values of cards when new cards are added or removed.
   - `OnCardAdded` and `OnCardRemove`: Responds to changes in the player's card collection, adjusting the ability's effect accordingly.

6. **Reactivation and Cancellation**:
   - `ReactivateEffect`: Reapplies the ability's effect when conditions change.
   - `CancelEffect`: Reverses the ability's effect, removing the added attack and defense bonuses.

In summary, `WinAtkDefFamilityAtkDefConditionAbility` is a complex ability in a card game context, providing a dynamic and conditional boost to cards based on specific attributes. This ability contributes to strategic depth, allowing players to leverage card synergies and conditional advantages in gameplay.

#### WinAtkDefFamilyAbility

The `WinAtkDefFamilyAbility` class in Unity is designed to enhance the strategic gameplay in the card-based game by manipulating the attack and defense values of invocation cards. This ability is linked to the presence of other cards in the same family.

**Key Features:**

1. **Family-Specific Boosts**: It focuses on cards belonging to a specific family (`CardFamily`), adjusting their attack and defense stats.

2. **Attack and Defense Values**: The ability includes properties for attack (`attack`) and defense (`defense`) values, determining the extent of the stat modification.

3. **Initialization**:
   - The constructor sets up the ability with essential parameters like name, description, associated card family, and the attack/defense values.

4. **Increment and Decrement Functions**:
   - `IncrementAtkDefInvocationCard` and `DecrementAtkDefInvocationCard`: These methods adjust the stats of the invocation card based on the number of other family cards present.

5. **Effect Application**:
   - `ApplyEffect`: Applies the ability's effects, considering the number of relevant family cards on the field.
   - Utilizes `CalculateNumberCardInvocation` to count the number of qualifying family cards.
   - `ApplyPower`: Applies the calculated power boost based on the number of family cards.

6. **Dynamic Gameplay Interaction**:
   - `OnCardAdded` and `OnCardRemove`: Adjusts the ability's effect dynamically as cards are added or removed from the game, ensuring the stats reflect the current game state.
   - Reacts to changes in the player's card collection, maintaining the ability's relevance throughout the game.

7. **Reactivation and Cancellation**:
   - `ReactivateEffect`: Reapplies the ability's effect when conditions change or the game state is updated.
   - `CancelEffect`: Reverses any stat changes made by the ability, ensuring game balance is maintained.

8. **Handling Card Death**:
   - `OnCardDeath`: Resets the attack and defense of a dead card to its base values.

In summary, the `WinAtkDefFamilyAbility` class offers a dynamic and strategic element to card-based games, where the presence and attributes of family cards can significantly influence the power of an individual card. This ability adds depth and complexity to the game, encouraging players to consider the synergies and compositions of their card decks.

### Condition

#### EquipmentCardOnCardCondition

The `EquipmentCardOnCardCondition` class represents a specific condition in the card game, which checks if a designated equipment card is attached to a specific invocation card on the field. This condition is crucial for determining if certain cards can be summoned based on the equipment-attachment criterion.

Key aspects of this class include:

1. **Specific Equipment and Invocation Cards**: The condition requires the names of both the equipment card (`equipmentCardName`) and the invocation card (`invocationCardName`) to be specified. These names are used to check for the presence and attachment of the equipment card to the invocation card.

2. **Condition Evaluation**: The `CanBeSummoned` method evaluates the condition by checking if the specified equipment card is indeed attached to the designated invocation card among the player's cards.

3. **Summoning Dependency**: The ability to summon certain cards depends on the fulfillment of this condition, adding a layer of strategic complexity to the game. Players must carefully manage their equipment and invocation cards to meet such conditions.

This condition is particularly important in gameplay scenarios where the combination of specific equipment and invocation cards triggers special abilities or summoning opportunities, influencing the overall strategy and tactics of the game.

#### FieldCardOnFieldCondition

The `FieldCardOnFieldCondition` class represents a specific condition in the card game, focusing on the presence of a designated field card on the field. This condition is a key determinant in assessing whether certain cards can be summoned.

Key aspects of the `FieldCardOnFieldCondition` include:

1. **Field Card Identification**: It requires the name of a specific field card (`fieldName`) to be provided. This name is used to check for the card's presence on the field.

2. **Condition Evaluation**: The main functionality is the `CanBeSummoned` method, which evaluates whether the specified field card is currently on the field. This is done by checking the player's field card slot against the specified field card name.

3. **Summoning Dependency**: The ability to summon certain cards in the game is dependent on whether this condition is met. This adds a strategic element to gameplay, as players must consider the state of the field and manage their field cards to meet these summoning conditions.

This condition plays a significant role in the game's dynamics, influencing players' decisions and strategies based on the field cards in play. It emphasizes the importance of field control and the strategic deployment of field cards.

#### InvocationCardOnFieldCondition

The `InvocationCardOnFieldCondition` class is part of the card game's framework, designed to check for the presence of specific invocation cards on the field before allowing a card to be summoned. It's a specialized form of condition that plays a vital role in game strategy and flow.

Key Features:

1. **Purpose**: This condition checks if certain invocation cards, identified by their names, are present on the playing field. It's a prerequisite for the summoning of other cards, ensuring certain game conditions or strategies are met.

2. **Card Names List**: The class contains a list (`CardNames`) of the names of invocation cards it needs to check for. This list is essential for the condition to determine if the required cards are present on the field.

3. **Initialization**: In its constructor, the class takes a unique condition name (`ConditionName`), a description, and a list of card names. These parameters initialize the condition with the specific criteria it needs to check.

4. **Condition Evaluation**: The core functionality lies in the `CanBeSummoned` method. This method checks the player's invocation cards to see if any of them match the names listed in `CardNames`. If any of the required cards are found on the field, the method returns `true`, indicating that the condition to summon another card is met.

The `InvocationCardOnFieldCondition` class is a strategic component that require certain cards to be in play before others can be summoned. It adds depth to the gameplay, encouraging players to think tactically about card placement and sequencing.

#### NumberInvocationDeadCondition

The `NumberInvocationDeadCondition` in Unity is a game condition primarily used to determine if a card can be summoned based on the number of invocation cards that have been "destroyed" or used in gameplay.

Key Features:

1. **Purpose**: It assesses whether the number of invocation cards in a player's 'YellowCards' collection (representing cards that have been used or destroyed) meets or surpasses a pre-defined threshold.

2. **Initialization**: The condition is initialized with a unique identifier, a description, and a specific number representing the required threshold of 'dead' invocation cards.

3. **Summoning Criterion**: This condition is pivotal in determining the ability to summon certain cards. It checks if the number of invocation cards that have been 'killed' is equal to or greater than the specified number.

4. **Card Type Focus**: The evaluation is specific to invocation type cards within the player's YellowCards collection, which is a subset of the player's overall card collection.

5. **Evaluating the Condition**: The ability to summon is determined by counting the invocation cards in the YellowCards collection and comparing this count against the specified death threshold.

6. **Return Value**: The condition returns `true` if the number of 'dead' invocation cards meets or exceeds the set threshold, indicating that the conditions for summoning a card are satisfied. Otherwise, it returns `false`.

This condition adds a strategic element to gameplay, as players may need to manage their invocation cards and their usage/destruction to meet this condition for summoning more powerful or key cards.

#### SpecificAtkDefInvocationCardOnFieldCondition

The `SpecificAtkDefInvocationCardOnFieldCondition` in Unity is a condition class designed to determine the summoning feasibility of a card based on the attack and defense values of invocation cards already on the field.

**Key Features:**

1. **Purpose and Context**: It serves to check if there's any invocation card on the field that meets specific attack or defense thresholds, a crucial aspect for summoning certain cards.

2. **Parameters for Comparison**: The condition takes two primary parameters - `atk` (attack) and `def` (defense). These values set the benchmark for comparing against the stats of invocation cards on the field.

3. **Initialization and Description**: The condition is initialized with a unique name and a descriptive explanation, clarifying its role and functionality within the game.

4. **Evaluation Mechanism**: The core functionality lies in evaluating if any invocation card on the field has attack or defense values equal to or exceeding the specified `atk` and `def` values.

5. **Flexible Application**: This condition provides flexibility in summoning strategies, as it accommodates cards with varying attack and defense values.

6. **Player Cards Evaluation**: It examines the collection of player cards (`playerCards`) to check against the condition, making it an integral part of the game's strategic decision-making.

7. **Boolean Outcome**: The method `CanBeSummoned` returns a boolean value (`true` or `false`), indicating whether the condition for summoning a card is met based on the current state of the field.

This condition is particularly useful in strategic card games or any gameplay scenario involving summoning mechanics, where the presence and attributes of other cards on the field directly influence gameplay decisions and outcomes. It adds depth to the game by requiring players to consider not just their immediate choices but also the existing state of the play area.

#### SpecificCardBackFromDeathCondition

The `SpecificCardBackFromDeathCondition` in Unity is a specialized condition class, extending the `InvocationCardOnFieldCondition`. It plays a critical role in the summoning mechanics of a game, especially when a card's summoning depends on the revival of specific cards.

**Key Characteristics:**

1. **Purpose**: This condition checks whether a specific card, previously 'dead' (i.e., removed from play), has been resurrected and is now present on the field. This state is crucial for the summoning of another card.

2. **Inheritance**: It inherits from `InvocationCardOnFieldCondition`, implying that it shares common functionalities but with added specificity regarding cards returning from 'death'.

3. **Initialization**: It is initialized with a unique name, description, and a list of card names (`cardNames`). These card names are central to determining the condition's fulfillment.

4. **Evaluation Mechanism**: The core of this condition is its ability to evaluate if a specific card (or cards) listed in `cardNames` has been brought back to the field after being 'dead'. This is assessed by checking the `NumberOfDeaths` attribute of the card - if greater than 0, it indicates the card has been revived at least once.

5. **Player Cards Analysis**: The condition examines the `playerCards` collection to assess if the condition is met, making it a vital element in gameplay strategies involving resurrection or recycling of cards.

6. **Boolean Outcome**: Similar to other conditions, `CanBeSummoned` returns a boolean (`true` or `false`), indicating whether the specific condition for summoning another card is met based on the resurrection status of the specified card.

7. **Gameplay Implications**: This condition adds an intriguing layer to gameplay dynamics, emphasizing strategies around card revival and the impact of previously 'dead' cards returning to the field.

In summary, `SpecificCardBackFromDeathCondition` enhances the depth and complexity of card games by linking the summoning of new cards to the revival of specific ones. It encourages players to strategize around not just the current state of play but also the potential of cards that have been used before.

#### SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition

The `SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition` in Unity is a condition class that forms a part of the game's summoning mechanics. It is designed to check for the presence of a specific number of invocation cards on the field, meeting certain criteria related to card family, attack, and defense values.

**Core Features:**

1. **Purpose**: This condition is utilized to ascertain whether a certain number of invocation cards from a specified family, with minimum attack and defense values, are present on the field.

2. **Parameters**:
   - **Family**: The specific card family to be checked.
   - **Attack and Defense**: The minimum values for attack and defense that the cards must meet.
   - **Number of Cards**: The required number of cards on the field that must fulfill these criteria.

3. **Dual Initialization**: The class offers two constructors - one where attack and defense values are specified, and another where these values are not needed, broadening its applicability.

4. **Evaluation Method**:
   - The `CanBeSummoned` method evaluates the player's cards to determine if there are at least the required number of cards from the specified family on the field.
   - It checks that these cards meet or exceed the given attack and defense thresholds.

5. **Boolean Outcome**: Similar to other conditions, it returns a boolean (`true` or `false`) indicating whether the summoning conditions are met.

6. **Gameplay Impact**: This condition adds strategic depth to the game, encouraging players to consider the composition of their deck and the placement of cards on the field.

7. **Versatility**: By allowing different attack, defense, and number parameters, this condition can be adapted to various game scenarios, making it a versatile tool in game design.

In summary, the `SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition` enhances the gameplay by introducing strategic considerations around deck composition and card placement. Its focus on card families, coupled with attack and defense requirements, creates diverse gameplay scenarios and strategic depth.

#### SpecificFamilyInvocationCardOnFieldCondition

The `SpecificFamilyInvocationCardOnFieldCondition` in Unity is a condition class used in game mechanics to verify the presence of invocation cards from a specific card family on the field.

**Core Features:**

1. **Purpose**: This condition is primarily used to check if at least one invocation card from a specified card family is present on the field.

2. **Family Parameter**: The key attribute of this condition is the card family. It checks for the presence of cards from this specific family.

3. **Initialization**: Upon instantiation, the class requires the name of the condition, a description, and the specific card family to check for.

4. **Evaluation Method**:
   - The `CanBeSummoned` method assesses the player's cards to determine if any invocation cards belong to the specified family.
   - It returns a boolean value (`true` or `false`) based on the presence of such a card.

5. **Gameplay Impact**: This condition influences the strategic deployment of cards. Players must manage their decks and field placement to meet these family-specific conditions.

6. **Flexibility**: It can be adapted to various card families, allowing for diverse game scenarios and strategies centered around different card types.

In summary, the `SpecificFamilyInvocationCardOnFieldCondition` is a strategic element in gameplay, encouraging players to think tactically about the composition and deployment of their cards. It adds a layer of depth to the game by creating scenarios where specific families of cards become crucial for advancing in the game.

### Condition

The `Condition` class, along with the `ConditionName` enumeration, is a key part of the card game's mechanics, representing various conditions that can be evaluated against a player's cards. The `ConditionName` enum lists various specific conditions that can occur in the game, such as the presence of certain cards on the field or specific equipment being used.

Each `Condition` instance has a name and a description, which provide clarity about what the condition entails. The central feature of this class is the `CanBeSummoned` method. This abstract method is designed to be implemented by derived classes to determine whether a given condition can be met based on the current state of the player's cards.

For example, a condition might check if a particular card is present on the field, if a certain number of cards with specific attributes are in play, or if a specific sequence of events has occurred. The evaluation of these conditions can influence gameplay, affecting card summoning, ability activation, and other strategic elements of the game. This system allows for complex and dynamic interactions based on the evolving state of the game.

### ConditionLibrary

The `ConditionLibrary` class functions as a central repository for managing various game conditions in the card game. It holds a collection of `Condition` instances, each representing a specific game condition. These conditions are defined to check various aspects of gameplay, such as the presence of certain cards on the field, specific equipment being equipped to cards, or certain card families being represented in play.

The library includes conditions like checking for specific card titles on the field, verifying if a card with a particular equipment is present, evaluating if cards of a certain family meet specific attack and defense criteria, or confirming the resurrection of certain cards. Each condition is associated with a unique `ConditionName`.

Upon initialization, the `ConditionLibrary` populates a dictionary, mapping these condition names to their respective `Condition` objects, facilitating quick and efficient access to these conditions throughout the game. This design streamlines the process of checking and applying various conditions during gameplay, enhancing the game's strategic depth and complexity.

### InGameInvocationCard

The `InGameInvocationCard` class represents a card in the card game that has various attributes and abilities, making it an integral part of the gameplay. This class extends the `InGameCard` and is specific to invocation cards, which typically participate in the game's combat mechanics.

Key features of `InGameInvocationCard`:

1. **Base Card Information**: Each `InGameInvocationCard` is based on a `BaseInvocationCard`, containing its essential information like title, description, type, etc.

2. **Attack and Defense Stats**: The card has attack (`Attack`) and defense (`Defense`) values, crucial for in-game combat scenarios.

3. **Abilities and Conditions**: It can possess a list of abilities (`Abilities`) and conditions (`conditions`) that define its special actions and summoning requirements.

4. **Equipment and Family Association**: The card can be equipped with an `EquipmentCard` and belongs to one or more card families (`Families`).

5. **Gameplay Mechanics**:
   - `CancelEffect`: Indicates if the card's effect is canceled.
   - `CanDirectAttack`, `CantBeAttack`, and `Aggro`: Booleans indicating specific combat states.
   - `BlockAttack`/`UnblockAttack`: Methods to control the card's ability to attack.
   - `SetRemainedAttackThisTurn`: Sets the number of attacks the card can make in a turn.

6. **Turn and Death Tracking**: Tracks the number of turns (`NumberOfTurnOnField`) the card has been in play and the number of times it has been defeated (`NumberOfDeaths`).

7. **Control Mechanics**: Includes mechanisms to control (`ControlCard`) and free (`FreeCard`) the card, affecting its gameplay.

8. **Summoning and Action Checks**: Methods like `CanBeSummoned`, `HasAction`, and `CanAttack` determine the card's ability to be played, perform actions, or attack.

9. **State Reset and Updates**: Provides functionality to reset the card state for a new turn (`ResetNewTurn`) and update the card's attributes according to abilities (`UpdateInvocationCardForAbilities`).

`InGameInvocationCard` is a versatile class that encapsulates the behavior and attributes of invocation cards in the game, offering a wide range of functionalities that are key to the game's dynamics and player strategies.

### InvocationCardHandler

The `InvocationCardHandler` class is responsible for managing the behavior and interactions related to invocation cards. It extends the `CardHandler` class, specializing in handling invocation-specific actions and UI updates.

Key functionalities of `InvocationCardHandler` include:

1. **Constructor**: It initializes the handler with an `InGameMenuScript`, linking it to the game's UI script for menu interactions.

2. **Handling Invocation Cards**:
   - `HandleCard`: This method manages the interaction with an `InGameCard`, specifically tailored for invocation cards. It involves:
     - Checking if the invocation card can be summoned (`CanBeSummoned`), based on game rules and the current state of the player's cards.
     - Updating the UI, particularly the text and interactivity of the 'put card' button (`putCardButtonText`, `putCardButton`). The button is made interactable only if the conditions for summoning the invocation card are met and if there is space on the player's field (limited to 4 invocation cards).

3. **Card Placement Handling**:
   - `HandleCardPut`: This method is called when a card is placed on the field. It specifically handles the placement of an invocation card. When an invocation card is placed, it triggers the `InvocationCardEvent` in `InGameMenuScript`, signaling the game that an invocation card has been put into play.

The `InvocationCardHandler` is a crucial component of the game's backend, ensuring that invocation cards are managed correctly according to the game's rules and mechanics. It seamlessly integrates card logic with the game's UI, providing a smooth gameplay experience.

### InvocationFunctions

The `InvocationFunctions` class manages the behavior and interactions of invocation cards. This class is crucial for handling various operations such as placing invocation cards on the field, activating their effects, and managing their cancellation.

Key Features:

1. **Invocation Event Handling**: It uses Unity events to manage invocation card interactions. The class includes a `CancelInvocationEvent`, a serializable Unity event that triggers when an invocation needs to be canceled.

2. **Setup and Event Listeners**: In the `Start` method, the class sets up its initial state and attaches event listeners. This includes listening for invocation card events and handling cancel invocation events.

3. **Canceling Invocation Effects**: The `OnCancelEffect` method processes the cancellation of an invocation card's effects. It iterates through the card's abilities and either cancels or reactivates them based on the card's `CancelEffect` status.

4. **Placing Invocation Cards on the Field**: The `PutInvocationCard` method places the invocation card on the field if conditions are met. It checks whether adding the card to the field is possible and then applies the card's effect.

5. **Condition Check for Adding Cards**: The `CanAddCardToField` method checks if a new card can be added to the field, typically based on the maximum number of allowed invocation cards.

6. **Adding Cards to the Field**: The `AddCardToField` method physically adds an invocation card to the player's field, updating the game state accordingly.

7. **Applying Card Effects**: The `ApplyCardEffect` method activates the effects of the given invocation card. It applies each of the card's abilities, affecting the game based on their defined actions.

In summary, `InvocationFunctions` is a comprehensive class responsible for managing the lifecycle and interactions of invocation cards within the game. It plays a key role in ensuring the game's mechanics related to invocation cards are executed correctly, maintaining the flow and rules of the game.

### TutoInvocationFunctions

The `TutoInvocationFunctions` class in Unity, an extension of `InvocationFunctions`, is designed specifically for tutorial purposes in the card-based game. It manages the placement and effect application of invocation cards during the tutorial phase.

**Key Features:**

1. **Start Method**: On start-up, the class registers a listener for the `InvocationCardEvent` from `InGameMenuScript`, ensuring that the custom logic for placing invocation cards is activated during the tutorial.

2. **Placing Invocation Cards**:
   - `PutInvocationCard` function: It places the selected invocation card on the game field, provided the conditions to add a card to the field are met.
   - It calls `AddCardToField` (inherited method) to physically add the card to the field and `ApplyCardEffect` to trigger any specific effects of the card.

3. **Applying Card Effects**:
   - `ApplyCardEffect`: Tailored for tutorial scenarios, this method triggers specific actions or effects based on the card's title.
   - Special Handling: For instance, if the invocation card titled `ClichéRaciste` is used, it triggers a sequence to invoke another specific card, `Tentacules`, from the deck.

4. **Tutorial Guidance**:
   - The `ApplyCardEffect` includes a MessageBoxConfig, providing interactive guidance to players. It demonstrates how to invoke a card, reinforcing tutorial objectives.
   - Highlighting UI elements: It uses `HighLightPlane.Highlight` to draw attention to key interface elements, enhancing the learning experience.

5. **Customization for Learning**: This class is tailored to introduce players to game mechanics in a controlled and instructional manner, focusing on key aspects of invocation card usage.

In summary, `TutoInvocationFunctions` serves as an instructional tool within the game, guiding new players through the process of using invocation cards effectively. It provides contextual, hands-on learning experiences, making it an integral part of the game's tutorial system.

## Ability

The `Ability` class in Unity forms the backbone of the card game's mechanics, providing various powers and effects to the cards. This abstract class serves as a base for all specific abilities in the game. Each ability is unique, characterized by a name, a description, and potentially an associated action or effect.

**Key Features and Functionalities:**

1. **Enum AbilityName**: Defines a set of ability names that can be used in the game, such as `CanOnlyAttackItself`, `AddSpatialFromDeck`, `SacrificeArchibaldVonGrenier`, etc.

2. **Properties**:
   - `Name`: Identifies the ability.
   - `Description`: Provides a brief description of what the ability does.
   - `IsAction`: Indicates whether the ability involves an action.
   - `InvocationCard`: Associates the ability with a specific invocation card.

3. **Ability Effects**:
   - `ApplyEffect`: The primary method where the specific effect of the ability is implemented. This method is called to apply the ability's effect when necessary.
   - `CancelEffect` and `ReactivateEffect`: Methods to cancel or reactivate the ability's effect, providing dynamic interaction within the game.

4. **Game Events Handling**:
   - `OnTurnStart`: Handles operations related to the ability at the start of a turn.
   - `OnCardAdded` and `OnCardRemove`: Respond to events of adding or removing an invocation card from the field.
   - `OnAttackCard` and `OnCardAttacked`: Handle pre-attack logic and the outcome of an attack between invocation cards.

5. **Card Death Management**:
   - `OnCardDeath`: Manages the consequences of a card's death, potentially removing it from play and triggering related effects.

6. **Action Possibility Check**:
   - `IsActionPossible`: Determines if the ability's action can be executed based on the current game state.

7. **Interaction Triggers**:
   - `OnCardActionTouched`: Executes actions when an invocation card's action is triggered, allowing for interactive gameplay.

8. **Helper Methods**:
   - Includes various private methods such as `HandleNegativeAttackResult`, `HandleNeutralAttackResult`, and `IsEquipmentCardProtect`, which assist in determining the outcomes of card interactions.

In summary, the `Ability` class is a versatile and essential component of the game's architecture. It provides a framework for defining and implementing the diverse abilities of cards, crucial for creating dynamic and engaging gameplay. Each ability adds depth and strategy to the game, influencing how players interact with their cards and each other.

## AbilityLibrary

`AbilityLibrary` in Unity is a comprehensive collection of abilities for cards in a game, serving as a central repository where different abilities are stored and managed. It's a crucial component for assigning abilities to cards based on their type and functionality.

**Key Features and Functionalities:**

1. **Singleton Pattern Implementation**: Inherits from `StaticInstance<AbilityLibrary>`, ensuring that there's only one instance of the library in the game.

2. **Ability Dictionary**:
   - A dictionary (`AbilityDictionary`) that maps `AbilityName` enums to their corresponding `Ability` objects.
   - Initialized in the `Awake()` method, this dictionary is the primary way abilities are accessed and utilized in the game.

3. **List of Abilities**:
   - The library includes a predefined list of abilities (`abilities`), each instantiated with specific parameters like name, description, and additional settings.
   - Examples of abilities include `CantLiveWithoutAbility`, `CanOnlyAttackItselfAbility`, `GetFamilyInDeckAbility`, `SacrificeCardAbility`, etc.
   - Each ability is unique, tailored to specific game mechanics and strategies.

4. **Versatile and Extensible**:
   - This library can be easily extended to include new abilities as the game evolves.
   - It allows for dynamic assignment of abilities to cards, supporting a wide range of card interactions and gameplay complexities.

5. **Centralized Management**:
   - By centralizing all abilities in one library, the game maintains consistency and ease of management.
   - It simplifies the process of updating or adding new abilities.

In summary, the `AbilityLibrary` class acts as a centralized hub for all card abilities in the game. It facilitates the efficient management and assignment of abilities, contributing significantly to the game's flexibility and depth. This approach allows for streamlined development and maintenance of the game's mechanics, ensuring that abilities are consistently applied and easily accessible throughout the game.

## CardFactory

`CardFactory` in Unity is a specialized class designed to create specific instances of `InGameCard` objects based on the type and ownership of the cards. This class plays a pivotal role in the card creation process.

**Key Features and Functionalities:**

1. **Dynamic Card Creation**:
   - Capable of creating instances of various types of `InGameCard` based on the base card's type.
   - Supports different card categories like `EquipmentCard`, `EffectCard`, `InvocationCard`, and `FieldCard`.

2. **Method Implementation**:
   - The `CreateInGameCard` method is the core functionality of this class.
   - It takes a `Card` object and a `CardOwner` as parameters.

3. **Switch Statement for Card Type Handling**:
   - Utilizes a switch statement to determine the type of the provided `Card` object.
   - Depending on the type, it instantiates a corresponding subclass of `InGameCard`, such as `InGameEquipmentCard`, `InGameEffectCard`, `InGameInvocationCard`, or `InGameFieldCard`.

4. **Exception Handling**:
   - If an unsupported card type is provided, the method throws an `InvalidOperationException`.
   - This ensures that only valid card types are processed, maintaining the integrity of the card creation process.

5. **Flexibility and Extensibility**:
   - This factory pattern allows for easy addition of new card types in the future.
   - It separates the card instantiation logic from other parts of the game, making the codebase cleaner and more maintainable.

6. **Usage in Game Flow**:
   - Essential in scenarios where cards are dynamically generated or loaded during gameplay.
   - Ensures that each card is created with the correct type and assigned owner, which is critical for game mechanics and rules.

In summary, `CardFactory` serves as an efficient and centralized mechanism for creating card instances in the game. Its ability to handle different card types dynamically and throw exceptions for unsupported types makes it an integral part of the game's architecture, especially in managing diverse card interactions and behaviors.

## CardHandler

`CardHandler` in Unity is an abstract base class designed to manage card-specific interactions and behaviors within a game. This class is crucial for defining how cards are handled during gameplay, particularly regarding UI interactions and in-game effects.

**Key Features and Functionalities:**

1. **Abstract Base Class**:
   - Serves as a foundational class for more specific card handling implementations.
   - Allows for flexible extensions to handle different types of card interactions.

2. **UI and Game Interaction**:
   - Connected to the in-game menu script (`InGameMenuScript`), facilitating interactions with the game's UI and other game operations.

3. **Constructor Functionality**:
   - The constructor initializes the `CardHandler` with a reference to the `InGameMenuScript`.
   - This linkage is critical for integrating the card handling functionalities with the game's UI and overall flow.

4. **Abstract Methods**:
   - `HandleCard(InGameCard card)`: Defines the behavior when a card is interacted with, such as selecting or highlighting a card. Implementations should specify how the game and UI respond to these interactions.
   - `HandleCardPut(InGameCard card)`: Specifies the behavior when a card is placed or positioned in the game, like on a board or in a specific zone. This method is crucial for triggering any effects or game mechanics associated with placing the card.

5. **Extensibility and Customization**:
   - Being abstract, it allows different games or modules within a game to implement their own specific handling logic for different types of cards.
   - Encourages a clean and modular approach to handling various card actions and effects.

6. **Usage in Game Flow**:
   - Integral in scenarios where different types of cards require unique interaction behaviors.
   - Ensures a consistent and manageable way to implement and modify how players interact with cards during the game.

In summary, `CardHandler` acts as a template for developing specific card interaction mechanisms in a game. Its abstract nature allows for tailored implementations for different types of cards, ensuring that each card's unique characteristics and game rules are appropriately managed. This class is essential for maintaining a structured and flexible approach to card interactions within the game's architecture.

## InGameCard

`InGameCard` in Unity represents a card entity within a game, encompassing various properties and attributes essential for gameplay mechanics. It serves as a fundamental building block for card-based games, providing the necessary data and attributes each card needs to function within the game's context.

**Key Features and Functionalities:**

1. **Base Card Information**:
   - `BaseCard`: Holds the foundational data of the card, like its base attributes and states.

2. **Card Identification and Description**:
   - `title`: A string representing the card's title, used for identification and display purposes.
   - `Description`: Provides a brief overview or summary of the card.
   - `DetailedDescription`: Contains a more detailed description or lore associated with the card, adding depth to its background.

3. **Card Classification and Visuals**:
   - `type`: Specifies the card's type classification (e.g., spell, creature), critical for gameplay mechanics and rules.
   - `materialCard`: References the Material (visual aspect) associated with the card, essential for the graphical representation in the game.

4. **Collector's Item Flag**:
   - `collector`: A boolean flag indicating whether the card is a collector's item, which can influence its gameplay role or value.

5. **Owner and Accessibility**:
   - `CardOwner`: Enum property that identifies the card's owner, determining which player can utilize the card.
   - `Title`, `Type`, `MaterialCard`, `Collector`: Public getters providing read-only access to the card's title, type, material, and collector status.

6. **Gameplay Integration**:
   - Serves as a versatile entity that can be integrated into various card game mechanics, such as drawing, playing, or discarding cards.
   - Can be extended or used as a base for more specific card types (e.g., spell cards, creature cards) with additional properties and methods.

7. **Customization and Extension**:
   - Allows for easy customization and extension to fit different game types and rules.
   - Facilitates the creation of a diverse range of cards with unique attributes and roles within the game.

In summary, `InGameCard` acts as a core component, providing essential data and functionalities required for each card. Its structure supports a wide range of card attributes and is designed to be flexible and extendable, accommodating various game mechanics and styles.
