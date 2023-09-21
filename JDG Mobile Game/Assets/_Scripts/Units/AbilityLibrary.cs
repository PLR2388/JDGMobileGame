using System.Collections.Generic;
using System.Linq;
using Cards;

/// <summary>
/// Represents a library of abilities for cards.
/// This library contains a collection of abilities that can be assigned to various cards based on their type and function.
/// </summary>
public class AbilityLibrary : StaticInstance<AbilityLibrary>
{
    /// <summary>
    /// Dictionary storing abilities keyed by their names.
    /// </summary>
    public Dictionary<AbilityName, Ability> AbilityDictionary;

    /// <summary>
    /// Initializes the ability dictionary upon object creation.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        AbilityDictionary = abilities.ToDictionary(ability => ability.Name, ability => ability);
    }

    /// <summary>
    /// A list of abilities available in the library.
    /// </summary>
    private readonly List<Ability> abilities = new()
    {
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune,
            "Invocation can't live without Benzaie or Benzaie jeune",
            list: new List<string>
            {
                "Benzaie jeune",
                "Benzaie"
            }
        ),
        new CanOnlyAttackItselfAbility(
            AbilityName.CanOnlyAttackItself,
            "Opponent can only attack this card"
        ),
        new GetFamilyInDeckAbility(
            AbilityName.AddSpatialFromDeck,
            "Add an invocation card belonging to the Spatial family from the deck to the hand",
            CardFamily.Spatial
        ),
        new SacrificeCardAbility(
            AbilityName.SacrificeArchibaldVonGrenier,
            "Sacrifice Archibald Von Grenier",
            "Archibald Von Grenier"
        ),
        new CantBeAttackAbility(
            AbilityName.CantBeAttackIfComics,
            "Invocation can't be attack if there is another card on the field belonging to Comics family",
            CardFamily.Comics
        ),
        new GiveAtkDefFamilyAbility(
            AbilityName.GiveAtkDefToComics,
            "Invocation give 0.5 Atk and 0.5 Def to comic invocation card on field",
            CardFamily.Comics,
            0.5f,
            0.5f
        ),
        new SendAllCardsInHand(
            AbilityName.SendAllCardToHands,
            "Invocation send all card from fields to hands except itself"
        ),
        new SacrificeCardAbility(
            AbilityName.SacrificeBenzaieJeune,
            "Sacrifice Benzaie jeune",
            "Benzaie jeune"
        ),
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetNounoursFromDeck,
            "Get Nounours from deck to hand",
            "Nounours"
        ),
        new SacrificeCardAbility(
            AbilityName.SacrificeJoueurDuGrenier,
            "Sacrifice Joueur du Grenier",
            "Joueur Du Grenier"
        ),
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetPetitePortionDeRizFromDeck,
            "Get Petite portions de « riz » from deck to hand",
            "Petite portions de « riz »"
        ),
        new InvokeSpecificCardAbility(
            AbilityName.InvokeTentacules,
            "Invoke Tentacules from deck to field",
            "Tentacules"
        ),
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetLycéeMagiqueGeorgesPompidouFromDeck,
            "Get Lycée magique Georges Pompidou from deck to hand",
            "Lycée magique Georges Pompidou"
        ),
        new OptionalSacrificeForAtkDefAbility(
            AbilityName.SacrificeSebDuGrenierOnHardCornerForAtkDef,
            "Sacrifice Seb du Grenier on Hard Corner field to have 4,5 in ATK and DEF",
            "Seb Du Grenier",
            "Le Hard Corner",
            4.5f,
            4.5f
        ),
        new WinAtkDefFamilyAbility(
            AbilityName.Win1Atk1DefDeveloper,
            "Win 1 ATK and 1 DEF for every Developer on Field",
            CardFamily.Developer,
            1,
            1
        ),
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.Sacrifice3Atk3Def,
            "Invocation needs a sacrifice of an invocation that has at least 3 atk or 3 def",
            3,
            3
        ),
        new OptionalChangeFieldFromDeckAbility(
            AbilityName.ChangeFieldWithFieldFromDeck,
            "Change current field with field from deck"
        ),
        new WinAtkDefFamilityAtkDefConditionAbility(
            AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition,
            "Win 1 ATK and 1 DEF for every japon invocation card with 2 ATK and 2 DEF",
            CardFamily.Japan,
            1,
            1,
            2,
            2
        ),
        new InvokeSpecificCardAbility(
            AbilityName.InvokeDresseurBidulmon,
            "Invoke Dresseur de Bidulmon from deck to field",
            "Dresseur de Bidulmon"
        ),
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetZozanKebabFromDeck,
            "Get Zozan Kebab from deck to hand",
            "Zozan Kebab"
        ),
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.SacrificeWizard,
            "Invocation needs a sacrifice of a wizard invocation card",
            cardFamily: CardFamily.Wizard
        ),
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetConvocationAuLyceeFromDeck,
            "Get Convocation au lycée from deck to hand",
            "Convocation au lycée"
        ),
        new ProtectBehindDuringAttackAbility(
            AbilityName.ProtectedBehindStarlightUnicorn,
            "Invocation protects himself behind Starlight Unicorn if present",
            "Starlight Unicorn"
        ),
        // TODO : Not use right now as Canard-signal is a Contre card
        // When this is done, add it to Inspecteur Magret
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetCanardSignal,
            "Get Canard-signal from deck to hand",
            "Canard-signal"
        ),
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.SacrificeDeveloper3Atk3Def,
            "Invocation needs 2 developer sacrifice with at least 3 Atk or 3 Def",
            3,
            3,
            CardFamily.Developer,
            2
        ),
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.SacrificeHardCorner3Atk3Def,
            "Invocation needs 2 hard corner sacrifce with at least 3 Atk or 3 Def",
            3,
            3,
            CardFamily.HardCorner,
            2
        ),
        new CantBeAttackAbility(
            AbilityName.CantBeAttackKill,
            "Invocation can't be attacked or killed by other invocation cards"
        ),
        new BackToHandAfterDeathAbility(
            AbilityName.ComesBackFromDeath,
            "Invocation comes back to player hand everytime he died"
        ),
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.Sacrifice2Japan,
            "Invocation needs 2 Japan sacrifice to be invoke",
            0,
            0,
            CardFamily.Japan,
            2
        ),
        new DestroyFieldAtkDefAttackConditionAbility(
            AbilityName.DestroyFieldATK,
            "Invocation can destroy a field but divide Atk by 2",
            2,
            1
        ),
        new KillOpponentInvocationCardAbility(
            AbilityName.KillOpponentInvocation,
            "Kill an invocation opponent when invoke"
        ),
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutJDG,
            "Invocation can't live without Joueur Du Grenier",
            list: new List<string>
            {
                "Joueur Du Grenier"
            }
        ),
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetForetElfesSylvains,
            "Get Forêt des elfes sylvains from deck to hand",
            "Forêt des elfes sylvains"
        ),
        new InvokeSpecificCardChoiceAbility(
            AbilityName.InvokeSebOrJDG,
            "Offer the choice to invoke Joueur Du Grenier or Seb Du Grenier",
            new List<string>
            {
                "Joueur Du Grenier",
                "Seb Du Grenier"
            }
        ),
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutComics,
            "Invocation cannot live without Comic invocation card",
            family: CardFamily.Comics
        ),
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.Sacrifice2Incarnation,
            "Invocation needs 2 Incarnation sacrifice to be invoke",
            0,
            0,
            CardFamily.Incarnation,
            2
        ),
        new DestroyFieldAtkDefAttackConditionAbility(
            AbilityName.DestroyFieldDEF,
            "Invocation can destroy a field but divide Def by 2",
            1,
            2
        ),
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetBenzaieJeuneFromDeck,
            "Get Benzaie jeune from deck to hand",
            "Benzaie jeune"
        ),
        new GetTypeCardFromDeckWithoutAttackAbility(
            AbilityName.GetEquipmentCardWithoutAttack,
            "Invocation can get Equipment card and loose its attack phase",
            CardType.Equipment
        ),
        new SacrificeCardAbility(
            AbilityName.SacrificeGranolax,
            "Sacrifice Granolax",
            "Granolax"
        ),
        new OptionalSacrificeForAtkDefAbility(
            AbilityName.SacrificeJDGOnStudioDevForAtkDef,
            "Sacrifice Joueur Du Grenier on Hard Corner field to have 4,5 in ATK and DEF",
            "Joueur Du Grenier",
            "Studio de développement",
            4.5f,
            4.5f
        ),
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutHuman,
            "Invocation cannot live without Human invocation card",
            family: CardFamily.Human
        ),
        new CopyAtkDefAbility(
            AbilityName.CopyBenzaieJeune,
            "Invocation copy atk and def Benzaie jeune",
            "Benzaie jeune"
        ),
        new LimitTurnExistenceAbility(
            AbilityName.SurviveOneTurn,
            "Invocation survive only one turn",
            1
        ),
        new GetSpecificCardAfterDeathAbility(
            AbilityName.GiveDeathWhenDie,
            "Invocation gives La Mort from deck to hand when he die",
            "La Mort"
        ),
        new ProtectBehindDuringAttackDefConditionAbility(
            AbilityName.ProtectBehindGreaterDef,
            "Invocation protect itself behind invocation card with greater def"
        ),
        new SacrificeCardAbility(
            AbilityName.SacrificeSebDuGrenier,
            "Sacrifice Seb Du Grenier",
            "Seb Du Grenier"
        ),
        new WinAtkDefFamilyAbility(
            AbilityName.Win1Atk1DefFistiland,
            "Invocation wins 1 ATK and 1 DEF for every Fistiland on Field",
            CardFamily.Fistiland,
            1,
            1
        ),
        new SacrificeCardAbility(
            AbilityName.SacrificeClicheRaciste,
            "Sacrifice Cliché Raciste",
            "Cliché Raciste"
        ),
        new KillBothCardsIfAttackAbility(
            AbilityName.KillEnemyIfDestroy,
            "Invocation kills its opponent if detroy by another invocation card"
        ),
        new SacrificeToInvokeAbility(
            AbilityName.SacrificeToInvoke,
            "Sacrifice Sheik Point to invoke non-collector card from yellow trash"
        ),
        new GetSpecificCardFromDeckOrYellowCardAbility(
            AbilityName.GetPatronInfogramesFromDeckYellowTrash,
            "Get Patron D'Infogrames from deck or yellow trash",
            "Patron D'Infogrames"
        ),
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutGranolaxOrMechaGranolax,
            "Invocation can't live without Granolax or Mecha-Granolax",
            list: new List<string>
            {
                "Granolax",
                "Mecha-Granolax"
            }
        ),
        new SkipOpponentAttackAbility(
            AbilityName.SkipOpponentAttackEveryTurn,
            "Player can choose to skip attack of one of his opponent invocation cards"
        ),
        new BackToHandAfterDeathAbility(
            AbilityName.ComesBackFromDeath5Times,
            "Invocation comes back to player hand 5 times everytime he died",
            5
        ),
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutJapon,
            "Tentacules cannot live without Japan invocation card",
            family: CardFamily.Japan
        ),
        new DrawCardsAbility(
            AbilityName.Draw2Cards,
            "Can draw maximum 2 card when invoke",
            2
        ),
        new GiveAtkDefToFamilyMemberAbility(
            AbilityName.GiveAktDefToFistilandMember,
            "Invocation can give his atk and def to Fistiland member",
            CardFamily.Fistiland
        ),
        new GiveAtkDefToFamilyMemberAbility(
            AbilityName.GiveAktDefToRpgMember,
            "Invocation can give his atk and def to Rpg member",
            CardFamily.Rpg
        ),
        new DefaultAbility(
            AbilityName.Default,
            "Default ability for invocation without power"
        )
    };
}