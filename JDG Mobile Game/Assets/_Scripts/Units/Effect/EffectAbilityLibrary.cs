using System.Collections.Generic;
using System.Linq;
using Cards;

/// <summary>
/// Represents a library of effect abilities that can be applied in the game.
/// </summary>
public class EffectAbilityLibrary : StaticInstance<EffectAbilityLibrary>
{
    private readonly List<EffectAbility> effectAbilities = new List<EffectAbility>
    {
        new LimitHandCardsEffectAbility(
            EffectAbilityName.LimitHandCardTo5,
            "Limit the number of cards in the hands to 5",
            5
        ),
        new LooseHPOpponentEffectAbility(
            EffectAbilityName.Lose2Point5StarsByInvocations,
            "Opponent will lose 2.5 Stars by invocations on our field",
            2.5f,
            DamageType.ByPlayerInvocationCount
        ),
        new FamilyFieldToInvocationsEffectAbility(
            EffectAbilityName.ApplyFamilyFieldToInvocations,
            "Can apply field family to invocation cards by paying 0.5 HP per turn",
            0.5f,
            "Convocation au lycée"
        ),
        new DestroyCardsEffectAbility(
            EffectAbilityName.DestroyAllCardsUnderManyConditions,
            "Destroy all cards on field if player sacrifice an invocation card, remove the first card on its deck and remove a card from his hand",
            0,
            true,
            true,
            true
        ),
        new GetHPBackEffectAbility(
            EffectAbilityName.GetHPFor1Sacrifice3ATKDEFCondition,
            "Get 15HP for the sacrifice of 1 invocation card that has at least 3 ATK or 3 DEF",
            1,
            3,
            15
        ),
        new DirectAttackEffectAbility(
            EffectAbilityName.DirectAttackIfUnder5HP,
            "Can attack player if his HP is under 5",
            5
        ),
        new ChangeFieldCardEffectAbility(
            EffectAbilityName.ChangeFieldCardFromDeck,
            "Get field from deck and replace the one already there or put it if nothing",
            0
        ),
        new DestroyCardsEffectAbility(
            EffectAbilityName.DestroyOneCardByRemovingOneHandCard,
            "Destroy 1 card by removing one hand card",
            1,
            false,
            false,
            true
        ),
        new DestroyFieldCardAbility(
            EffectAbilityName.DestroyFieldFor7HalfCost,
            "Destroy a field for 7.5 HP",
            7.5f
        ),
        new GetHPBackEffectAbility(
            EffectAbilityName.Get7HalfHPFor1Sacrifice,
            "Get 7.5 HP by sacrifing one invocation",
            1,
            0,
            7.5f
        ),
        new GetCardFromDeckYellowEffectAbility(
            EffectAbilityName.GetCardFromYellowDeck,
            "Get a card from the yellow trash or the deck",
            1,
            true,
            true
        ),
        new SkipOpponentAttackEffectAbility(
            EffectAbilityName.ManiabilitePourrieSkipAttackForOpponent,
            "Maniabilité pourrie skip opponent attack phase",
            "Maniabilité pourrie"
        ),
        new SwitchAtkDefEffectAbility(
            EffectAbilityName.SwitchAtkDef,
            "Switch ATK and DEF during a turn"
        ),
        new LookDeckCardsEffectAbility(EffectAbilityName.LookAndOrderDeckCards,
            "Look at the next 5 deck cards from the opponent or from you and reorder them",
            5
        ),
        new LooseHPOpponentEffectAbility(EffectAbilityName.LooseHPBasedOnNumberInvocation,
            "Opponent loose 2.5HP per invocation on his field",
            2.5f,
            DamageType.ByOpponentInvocationCount
        ),
        new DestroyCardsEffectAbility(
            EffectAbilityName.DestroyEquipmentCard,
            "Destroy one equipment card",
            1,
            types: new List<CardType>
            {
                CardType.Equipment
            }),
        new LookHandCardsEffectAbility(
            EffectAbilityName.LookOpponentHandCardsAndChangeIt,
            "Look opponent hand cards and remove one of them by removing also one of them from player's handcards"
        ),
        new IncrementNumberAttackEffectAbility(
            EffectAbilityName.DoubleAttackPerTurn,
            "Give 2 attacks for one turn",
            2
        ),
        new InvokeCardFromDeckYellowEffectAbility(
            EffectAbilityName.InvokeCardFromYellowTrash,
            "Invoke a card from the yellow trash",
            fromYellowTrash: true
        ),
        new DivideDEFOpponentEffectAbility(
            EffectAbilityName.DivideDEFOpponentBy2,
            "Divide opponent invocations DEF by 2 for one turn",
            2.0f
        ),
        new AddShieldsForUserEffectAbility(
            EffectAbilityName.Add3ShieldsForUser,
            "Add 3 shields to protect the user",
            3
        ),
        new DestroyCardsEffectAbility(
            EffectAbilityName.DestroyOpponentInvocationCard,
            "Destroy one invocation from opponent",
            1,
            types: new List<CardType>
            {
                CardType.Invocation
            },
            fromCurrentPlayer: false),
        new LooseHPOpponentEffectAbility(
            EffectAbilityName.Loose1HPPerOpponentHandCards,
            "Opponent loose 1HP per card in his hand",
            1,
            DamageType.ByOpponentHandCount
        ),
        new GetHPBackEffectAbility(
            EffectAbilityName.GetBackAllHPBySacrifice5AtkDef,
            "Sacrife a 5 atk or def invocation to get back all HP",
            1,
            5,
            0
        ),
        new ControlOpponentInvocationCardEffectAbility(
            EffectAbilityName.Control1OpponentInvocationCard,
            "Control an opponent invocation card during 1 turn"
        )
    };

    /// <summary>
    /// Dictionary that maps an effect ability name to its respective effect ability instance.
    /// </summary>
    public Dictionary<EffectAbilityName, EffectAbility> EffectAbilityDictionary;

    /// <summary>
    /// Initializes the effect abilities library.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        EffectAbilityDictionary = effectAbilities.ToDictionary(ability => ability.Name, ability => ability);
    }
}