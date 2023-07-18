using System.Collections.Generic;
using System.Linq;

public class EffectAbilityLibrary : StaticInstance<EffectAbilityLibrary>
{
    private List<EffectAbility> effectAbilities = new List<EffectAbility>
    {
        new LimitHandCardsEffectAbility(
            EffectAbilityName.LimitHandCardTo5,
            "Limit the number of cards in the hands to 5",
            5
        ),
        new ReduceOpponentStarsByInvocationCardsNumberEffectAbility(
            EffectAbilityName.Lose2Point5StarsByInvocations,
            "Opponent will lose 2.5 Stars by invocations on our field",
            2.5f
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
            "Get field from deck and replace the one already there or put it if nothing"
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
        )
    };

    public Dictionary<EffectAbilityName, EffectAbility> effectAbilityDictionary;

    protected override void Awake()
    {
        base.Awake();
        effectAbilityDictionary = effectAbilities.ToDictionary(ability => ability.Name, ability => ability);
    }
}