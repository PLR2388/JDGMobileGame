# Scriptables

## Card

The `Card` class, housed within the `Cards` namespace, epitomizes the foundational blueprint in Unity. Designed for extensibility, this class serves as a base for crafting specialized card types. Leveraging Unity's `ScriptableObject` capabilities, it facilitates the creation of card assets directly within the Unity Editor, streamlining the card development process.

Each card possesses a set of core attributes:
- **Title**: A succinct identifier for the card.
- **Description**: A concise summary of the card's essence.
- **Detailed Description**: A more in-depth narrative or exposition concerning the card.
- **Card Type**: A categorization, signifying the card's role or function within the game.
- **Material Card**: Defines the card's visual appearance using Unity's Material system.
- **Collector Flag**: A binary indicator, denoting if a particular card is of collector's edition significance.

This system promotes an organized approach to card creation, fostering a modular architecture that's primed for expansion and variation.

## ContreCard

The `ContreCard` class, part of the `_Scripts.Scriptables` namespace, represents a specialized card type within the game known as "Contre". Building upon the foundational architecture of the `Card` class from the `Cards` namespace, this class introduces specific characteristics exclusive to the "Contre" card type.

The defining feature of the `ContreCard` is its intrinsic association with the "Contre" card type. During its initialization via the `Awake` method, the card's type is automatically set to "Contre". With this specialization, developers can conveniently generate new "Contre" cards directly within the Unity Editor, harnessing Unity's `ScriptableObject` capabilities.

The `ContreCard` system is an example of extending base game mechanics to create more intricate gameplay elements, ensuring a layered and engaging gaming experience. Whether employed as a strategic element or a unique gameplay twist, the "Contre" card type provides added depth to the overall card system.

## EffectCard

The `EffectCard` class, nestled within the `Cards.EffectCards` namespace, epitomizes a distinct card type known as "Effect" in the game, enhancing the foundational structure of the primary `Card` class.

Central to the `EffectCard` is its capability to possess a collection of abilities, represented by the `EffectAbilities` list. These abilities define the core actions or outcomes the card can trigger or apply, enriching the strategic depth and dynamism of gameplay. As with its nature, upon initialization in the `Awake` method, the card is inherently categorized as an "Effect" type.

Utilizing the advantages of Unity's `ScriptableObject` system, developers can seamlessly create new "Effect" cards directly within the Unity Editor. The `EffectCard` system is emblematic of the game's layered mechanics, offering players a diverse range of strategic options and pathways to navigate the gameplay experience.

## EquipmentCard

The `EquipmentCard` class, situated in the `Cards.EquipmentCards` namespace, embodies an equipment card type in the game, evolving upon the foundational attributes of the main `Card` class.

A distinguishing feature of the `EquipmentCard` is its inherent association with a set of abilities, cataloged within the `EquipmentAbilities` list. These abilities encapsulate the various actions or attributes this card can confer, accentuating gameplay depth and strategic considerations. To signify its identity, upon initialization in the `Awake` method, the card is automatically designated as an "Equipment" type.

Integrated with Unity's `ScriptableObject` framework, the `EquipmentCard` system enables developers to effortlessly introduce new equipment cards via the Unity Editor. With the integration of equipment-based mechanics, players are presented with an enriched array of tactical choices and gameplay nuances.

## FieldCard

The `FieldCard` class, housed within the `Cards.FieldCards` namespace, represents a distinct field card variant in the game, building upon the foundational properties of the general `Card` class.

A unique attribute of `FieldCard` is its affiliation with a specific `CardFamily`, elucidating the faction or lineage to which the card pertains. This family linkage offers thematic consistency and potentially strategic synergies during gameplay. Moreover, the `FieldCard` boasts a set of associated abilities, stored within the `FieldAbilities` list. These capabilities provide a myriad of strategic options and gameplay interactions specific to field card dynamics.

Upon instantiation in the `Awake` method, the card is immediately classified as a "Field" type. Benefitting from Unity's `ScriptableObject` infrastructure, the `FieldCard` system empowers developers to seamlessly create and manage new field cards directly within the Unity Editor, enhancing the game's tactical depth and providing players with a rich landscape of strategic choices.

## InvocationCard

Within the `Cards.InvocationCards` namespace lies the `InvocationCard` class, a specialized card type that expands upon the foundational properties and methods of the primary `Card` class. Tailored to represent invocation cards within the game, this class encapsulates unique attributes and interactions.

A key feature of the `InvocationCard` is its distinct set of base statistics, delineated in the `InvocationCardStats` structure. These stats include the card's attack and defense values, granting the card its combative capabilities. Moreover, the invocation card is also linked to specific `CardFamily` factions, adding layers of strategic depth and thematic resonance to its in-game use. The card's susceptibility to specific game effects is captured by the `AffectedByEffect` attribute.

To offer varied gameplay scenarios and challenges, the `InvocationCard` is associated with a collection of conditions (enumerated in the `Conditions` list) and abilities (found in the `Abilities` list). These collections encapsulate unique game mechanics and potential interactions tied explicitly to the invocation card type.

Upon instantiation, the card's type is promptly set to "Invocation" in the `Awake` method. With Unity's `ScriptableObject` framework, developers can effortlessly create and manage distinct invocation cards within the Unity Editor, enriching gameplay dynamics and ensuring a diverse strategic environment for players.
