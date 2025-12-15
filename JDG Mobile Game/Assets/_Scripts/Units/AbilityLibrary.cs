using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Domain;

/// <summary>
/// Represents a library of abilities for cards.
/// This library contains a collection of abilities that can be assigned to various cards based on their type and function.
/// Phase 24-25: Updated to use JDG.Domain.AbilityName instead of legacy global AbilityName.
/// </summary>
public class AbilityLibrary : StaticInstance<AbilityLibrary>
{
    /// <summary>
    /// Dictionary storing abilities keyed by their names.
    /// Phase 24-25: Now uses JDG.Domain.AbilityName (domain layer enum).
    /// </summary>
    public Dictionary<JDG.Domain.AbilityName, Ability> AbilityDictionary;

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
    private readonly List<Ability> abilities = new List<Ability>()
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
        // CanOnlyAttackItselfAbility removed - Phase 42b
        // GetFamilyInDeckAbility (AddSpatialFromDeck) removed - Phase 42b
        // SacrificeCardAbility (SacrificeArchibaldVonGrenier) removed - Phase 42b
        // CantBeAttackAbility (CantBeAttackIfComics) removed - Phase 42b
        // GiveAtkDefFamilyAbility removed - Phase 42b
        new SendAllCardsInHand(
            AbilityName.SendAllCardToHands,
            "Invocation send all card from fields to hands except itself"
        ),
        // SacrificeCardAbility (SacrificeBenzaieJeune) removed - Phase 42b
        // GetSpecificCardFromDeckAbility (GetNounoursFromDeck) removed - Phase 42b
        // SacrificeCardAbility (SacrificeJoueurDuGrenier) removed - Phase 42b
        // GetSpecificCardFromDeckAbility (GetPetitePortionDeRizFromDeck) removed - Phase 42b
        // InvokeSpecificCardAbility (InvokeTentacules) removed - Phase 42b
        // GetSpecificCardFromDeckAbility (GetLycéeMagiqueGeorgesPompidouFromDeck) removed - Phase 42b
        // OptionalSacrificeForAtkDefAbility (SacrificeSebDuGrenierOnHardCornerForAtkDef) removed - Phase 42b
        // WinAtkDefFamilyAbility (Win1Atk1DefDeveloper) removed - Phase 42b
        // SacrificeCardMinAtkMinDefFamilyNumberAbility (Sacrifice3Atk3Def) removed - Phase 42b
        new OptionalChangeFieldFromDeckAbility(
            AbilityName.ChangeFieldWithFieldFromDeck,
            "Change current field with field from deck"
        ),
        // WinAtkDefFamilityAtkDefConditionAbility removed - Phase 42b
        // InvokeSpecificCardAbility (InvokeDresseurBidulmon) removed - Phase 42b
        // GetSpecificCardFromDeckAbility (GetZozanKebabFromDeck) removed - Phase 42b
        // SacrificeCardMinAtkMinDefFamilyNumberAbility (SacrificeWizard) removed - Phase 42b
        // GetSpecificCardFromDeckAbility (GetConvocationAuLyceeFromDeck) removed - Phase 42b
        // ProtectBehindDuringAttackAbility removed - Phase 42b
        // GetSpecificCardFromDeckAbility (GetCanardSignal) removed - Phase 42b
        // SacrificeCardMinAtkMinDefFamilyNumberAbility (SacrificeDeveloper3Atk3Def, SacrificeHardCorner3Atk3Def) removed - Phase 42b
        // CantBeAttackAbility (CantBeAttackKill) removed - Phase 42b
        // BackToHandAfterDeathAbility (ComesBackFromDeath) removed - Phase 42b
        // SacrificeCardMinAtkMinDefFamilyNumberAbility (Sacrifice2Japan) removed - Phase 42b
        // DestroyFieldAtkDefAttackConditionAbility (DestroyFieldATK) removed - Phase 42b
        // KillOpponentInvocationCardAbility removed - Phase 42b
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutJDG,
            "Invocation can't live without Joueur Du Grenier",
            list: new List<string>
            {
                "Joueur Du Grenier"
            }
        ),
        // GetSpecificCardFromDeckAbility (GetForetElfesSylvains) removed - Phase 42b
        // InvokeSpecificCardChoiceAbility removed - Phase 42b
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutComics,
            "Invocation cannot live without Comic invocation card",
            family: CardFamily.Comics
        ),
        // SacrificeCardMinAtkMinDefFamilyNumberAbility (Sacrifice2Incarnation) removed - Phase 42b
        // DestroyFieldAtkDefAttackConditionAbility (DestroyFieldDEF) removed - Phase 42b
        // GetSpecificCardFromDeckAbility (GetBenzaieJeuneFromDeck) removed - Phase 42b
        // GetTypeCardFromDeckWithoutAttackAbility removed - Phase 42b
        // SacrificeCardAbility (SacrificeGranolax) removed - Phase 42b
        // OptionalSacrificeForAtkDefAbility (SacrificeJDGOnStudioDevForAtkDef) removed - Phase 42b
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutHuman,
            "Invocation cannot live without Human invocation card",
            family: CardFamily.Human
        ),
        // CopyAtkDefAbility removed - Phase 42b
        // LimitTurnExistenceAbility (SurviveOneTurn) removed - Phase 42b
        // GetSpecificCardAfterDeathAbility removed - Phase 42b
        // ProtectBehindDuringAttackDefConditionAbility removed - Phase 42b
        // SacrificeCardAbility (SacrificeSebDuGrenier) removed - Phase 42b
        // WinAtkDefFamilyAbility (Win1Atk1DefFistiland) removed - Phase 42b
        // SacrificeCardAbility (SacrificeClicheRaciste) removed - Phase 42b
        // KillBothCardsIfAttackAbility removed - Phase 42b
        // SacrificeToInvokeAbility removed - Phase 42b
        // GetSpecificCardFromDeckOrYellowCardAbility removed - Phase 42b
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutGranolaxOrMechaGranolax,
            "Invocation can't live without Granolax or Mecha-Granolax",
            list: new List<string>
            {
                "Granolax",
                "Mecha-Granolax"
            }
        ),
        // SkipOpponentAttackAbility removed - Phase 42b
        // BackToHandAfterDeathAbility (ComesBackFromDeath5Times) removed - Phase 42b
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutJapon,
            "Tentacules cannot live without Japan invocation card",
            family: CardFamily.Japan
        ),
        // DrawCardsAbility removed - Phase 42b: Using modern IAbility system
        // GiveAtkDefToFamilyMemberAbility (GiveAktDefToFistilandMember) removed - Phase 42b
        // GiveAtkDefToFamilyMemberAbility (GiveAktDefToRpgMember) removed - Phase 42b
        // DefaultAbility removed - Phase 42b: Using modern IAbility system
    };
}