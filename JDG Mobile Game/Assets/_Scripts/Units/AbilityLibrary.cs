using System.Collections.Generic;
using System.Linq;
using Cards;


public class AbilityLibrary : StaticInstance<AbilityLibrary>
{
    public Dictionary<AbilityName, Ability> abilityDictionary;

    protected override void Awake()
    {
        base.Awake();
        abilityDictionary = abilities.ToDictionary(ability => ability.Name, ability => ability);
    }

    private readonly List<Ability> abilities = new List<Ability>
    {
        new CantLiveWithoutInvocationCardsAbility(
            AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune,
            "Alpha man can't live without Benzaie or Benzaie jeune",
            "Alpha Man",
            new List<string>
            {
                "Benzaie jeune",
                "Benzaie"
            }
        ),
        new CanOnlyAttackItselfAbility(
            AbilityName.CanOnlyAttackAlphaMan,
            "Opponent can only attack Alpha Man",
            "Alpha Man"
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
        new CantBeAttackIfFamilyAbility(
            AbilityName.BabsCantBeAttackIfComics,
            "Babs can't be attack if there is another card on the field belonging to Comics family",
            "Babs",
            CardFamily.Comics
        ),
        new GiveAtkDefFamilyAbility(
            AbilityName.BabsGiveAtkDefToComics,
            "Babs give 0.5 Atk and 0.5 Def to comic invocation card on field",
            "Babs",
            CardFamily.Comics,
            0.5f,
            0.5f
        ),
        new SendAllCardsInHand(
            AbilityName.BebeSendAllCardToHands,
            "Bébé Terreur-Nocturne send all card from fields to hands",
            "Bébé Terreur-Nocturne"
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
            "Daffy",
            "Seb Du Grenier",
            "Le Hard Corner",
            4.5f,
            4.5f
        ),
        new WinAtkDefFamilyAbility(
            AbilityName.DavidGnoufWin1Atk1DefDeveloper,
            "Win 1 ATK and 1 DEF for every Developer on Field",
            "David Gnouf",
            CardFamily.Developer,
            1,
            1
        ),
        new SacrificeCardMinAtkMinDefAbility(
            AbilityName.DictateurSympaSacrifice3Atk3Def,
            "Dictateur Sympa needs a sacrifice of an invocation that has at least 3 atk or 3 def",
            "Dictateur Sympa",
            3,
            3
        ),
        new OptionalChangeFieldFromDeckAbility(
            AbilityName.ChangeFieldWithFieldFromDeck,
            "Change current field with field from deck"
        ),
        new WinAtkDefFamilityAtkDefConditionAbility(
            AbilityName.DresseurBidulmonWin1ATK1DefJaponWith2ATK2DEFCondition,
            "Win 1 ATK and 1 DEF for every japon invocard card with 2 ATK and 2 DEF",
            "Dresseur de Bidulmon",
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
        new SacrificeCardFamilyAbility(
            AbilityName.GeorgesSacrificeWizard,
            "Georges Tuséki need a sacrifice of a wizard invocation card",
            CardFamily.Wizard,
            "Georges Tuséki"
        ),
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetConvocationAuLyceeFromDeck,
            "Get Convocation au lycée from deck to hand",
            "Convocation au lycée"
        ),
        new ProtectBehindDuringAttackAbility(
            AbilityName.GranolaxProtectedBehindStarlightUnicorn,
            "Granolax protect himself behind Starlight Unicorn if present",
            "Starlight Unicorn",
            "Granolax"
        ),
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetCanardSignal,
            "Get Canard-signal from deck to hand",
            "Canard-signal"
        ),
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.JeanClaudeSacrificeDeveloper3Atk3Def,
            "Jean Claude needs 2 developer sacrifce with at least 3 Atk or 3 Def",
            "Jean-Claude",
            3,
            3,
            CardFamily.Developer,
            2
        ),
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.JeanGuySacrificeHardCornerAtk3Def,
            "Jean Guy needs 2 hard corner sacrifce with at least 3 Atk or 3 Def",
            "Jean-Guy",
            3,
            3,
            CardFamily.HardCorner,
            2
        ),
        new CantBeAttackKillByInvocationAbility(
            AbilityName.JeanLouisCantBeAttackKill,
            "Jean-Louis La Chaussette can't be attacked or killed by other invocation cards",
            "Jean-Louis La Chaussette"
        ),
        new BackToHandAfterDeathAbility(
            AbilityName.JeanMarcComesBackFromDeath,
            "Jean-Marc Soul comes back to player hand everytime he died",
            "Jean-Marc Soul"
        ),
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.Koaloutre2JapanSacrifice,
            "Koaloutre-Ornithambas Lapinzord nain de Californie needs 2 Japan sacrifice to be invoke",
            "Koaloutre-Ornithambas Lapinzord nain de Californie",
            0,
            0,
            CardFamily.Japan,
            2
        ),
        new DestroyFieldAtkDefAttackConditionAbility(
            AbilityName.KoaloutreDestroyField,
            "Koaloutre-Ornithambas Lapinzord nain de Californie can destroy a field but divide Atk by 2",
            "Koaloutre-Ornithambas Lapinzord nain de Californie",
            2,
            1
            )
    };
}