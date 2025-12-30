using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using DomainEffectAbilityName = JDG.Domain.Enums.EffectAbilityName;

/// <summary>
/// Provides effect abilities.
/// Phase 48: Originally wrapped legacy singleton for DI-compatible access.
/// Phase 84: Now owns the ability dictionary directly (EffectAbilityLibrary deleted).
/// Phase 102: Added modern IAbility support via GetModernAbility.
/// </summary>
#pragma warning disable CS0618 // Suppress obsolete warnings for legacy ability types
public class EffectAbilityProviderService : IEffectAbilityProvider
{
    private readonly Dictionary<EffectAbilityName, EffectAbility> _abilityDictionary;
    private Dictionary<DomainEffectAbilityName, IAbility> _modernAbilityDictionary;
    private readonly EffectAbilityFactory _factory;

    /// <summary>
    /// Initializes the effect ability provider with all abilities.
    /// Phase 84: Moved from EffectAbilityLibrary.
    /// Phase 102: Added modern ability factory initialization.
    /// </summary>
    public EffectAbilityProviderService(IPlayerRepository playerRepository = null)
    {
        // Initialize modern factory if repository provided
        if (playerRepository != null)
        {
            _factory = new EffectAbilityFactory(playerRepository);
            InitializeModernAbilities();
        }
        else
        {
            _modernAbilityDictionary = new Dictionary<DomainEffectAbilityName, IAbility>();
        }

        var abilities = new List<EffectAbility>
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
                CardSource.Both
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

        _abilityDictionary = abilities.ToDictionary(ability => ability.Name, ability => ability);
    }

    /// <summary>
    /// Gets an effect ability by its name.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The effect ability, or null if not found.</returns>
    public EffectAbility GetAbility(EffectAbilityName abilityName)
    {
        _abilityDictionary.TryGetValue(abilityName, out var ability);
        return ability;
    }

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    public bool HasAbility(EffectAbilityName abilityName)
    {
        return _abilityDictionary.ContainsKey(abilityName);
    }

    /// <summary>
    /// Gets a modern IAbility implementation by effect ability name.
    /// Phase 102: New method for clean architecture migration.
    /// </summary>
    /// <param name="abilityName">The domain ability name to look up.</param>
    /// <returns>The modern ability, or null if not found.</returns>
    public IAbility GetModernAbility(DomainEffectAbilityName abilityName)
    {
        _modernAbilityDictionary.TryGetValue(abilityName, out var ability);
        return ability;
    }

    /// <summary>
    /// Initializes the modern ability dictionary with all effect abilities.
    /// Maps DomainEffectAbilityName to IAbility implementations.
    /// </summary>
    private void InitializeModernAbilities()
    {
        _modernAbilityDictionary = new Dictionary<DomainEffectAbilityName, IAbility>
        {
            // Hand manipulation
            [DomainEffectAbilityName.LimitHandCardTo5] = _factory.CreateLimitHand(5),

            // Damage abilities
            [DomainEffectAbilityName.Lose2Point5StarsByInvocations] = _factory.CreateDamageOpponent(
                DamageCalculationType.ByPlayerInvocationCount, 2.5f),
            [DomainEffectAbilityName.LooseHPBasedOnNumberInvocation] = _factory.CreateDamageOpponent(
                DamageCalculationType.ByOpponentInvocationCount, 2.5f),
            [DomainEffectAbilityName.Loose1HPPerOpponentHandCards] = _factory.CreateDamageOpponent(
                DamageCalculationType.ByOpponentHandCount, 1f),

            // Healing abilities
            [DomainEffectAbilityName.GetHPFor1Sacrifice3ATKDEFCondition] = _factory.CreateHealPlayer(15f, 3),
            [DomainEffectAbilityName.Get7HalfHPFor1Sacrifice] = _factory.CreateHealPlayer(7.5f),
            [DomainEffectAbilityName.GetBackAllHPBySacrifice5AtkDef] = _factory.CreateHealPlayer(0f, 5), // 0 = full heal

            // Attack modification abilities
            [DomainEffectAbilityName.DirectAttackIfUnder5HP] = _factory.CreateEnableDirectAttack(5f),
            [DomainEffectAbilityName.DoubleAttackPerTurn] = _factory.CreateDoubleAttacks(1),
            [DomainEffectAbilityName.ManiabilitePourrieSkipAttackForOpponent] = _factory.CreateSkipAttackPhase(),

            // Card destruction abilities
            [DomainEffectAbilityName.DestroyAllCardsUnderManyConditions] = _factory.CreateDestroyMultiple(0), // All
            [DomainEffectAbilityName.DestroyOneCardByRemovingOneHandCard] = _factory.CreateDestroyMultiple(1),
            [DomainEffectAbilityName.DestroyEquipmentCard] = _factory.CreateDestroyMultiple(1),
            [DomainEffectAbilityName.DestroyOpponentInvocationCard] = _factory.CreateDestroyMultiple(1),
            [DomainEffectAbilityName.DestroyFieldFor7HalfCost] = _factory.CreateDestroyFieldCard(7.5f),

            // Card manipulation abilities
            [DomainEffectAbilityName.ChangeFieldCardFromDeck] = _factory.CreateChangeField(),
            [DomainEffectAbilityName.GetCardFromYellowDeck] = _factory.CreateDrawFromGraveyard(),
            [DomainEffectAbilityName.InvokeCardFromYellowTrash] = _factory.CreateInvokeFromDeck(),

            // View abilities
            [DomainEffectAbilityName.LookAndOrderDeckCards] = _factory.CreateLookDeck(5),
            [DomainEffectAbilityName.LookOpponentHandCardsAndChangeIt] = _factory.CreateLookHand(),

            // Stat modification abilities
            [DomainEffectAbilityName.SwitchAtkDef] = _factory.CreateSwapStats(),
            [DomainEffectAbilityName.DivideDEFOpponentBy2] = _factory.CreateDivideDefense(2),

            // Shield abilities
            [DomainEffectAbilityName.Add3ShieldsForUser] = _factory.CreateAddShields(3),

            // Control abilities
            [DomainEffectAbilityName.Control1OpponentInvocationCard] = _factory.CreateControlCard(),

            // Field effect abilities
            [DomainEffectAbilityName.ApplyFamilyFieldToInvocations] = _factory.CreateApplyFamilyField(0.5f)
        };
    }
}
#pragma warning restore CS0618
