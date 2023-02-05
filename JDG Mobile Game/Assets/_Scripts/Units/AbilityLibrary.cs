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
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune,
            "Alpha man can't live without Benzaie or Benzaie jeune",
            "Alpha Man",
            list: new List<string>
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
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
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
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.GeorgesSacrificeWizard,
            "Georges Tuséki need a sacrifice of a wizard invocation card",
            "Georges Tuséki",
            cardFamily: CardFamily.Wizard
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
        ),
        new KillOpponentInvocationCardAbility(
            AbilityName.KillOponentInvocation,
            "Kill an invocation opponent when invoke"
        ),
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutJDG,
            "La Petite Fille can't live without Joueur Du Grenier",
            "La Petite Fille",
            list: new List<string>
            {
                "Joueur Du Grenier"
            }
        ),
        new CanOnlyAttackItselfAbility(
            AbilityName.CanOnlyAttackPetiteFille,
            "Opponent can only attack La Petite Fille",
            "La Petite Fille"
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
            AbilityName.BananeCantLiveWithoutComics,
            "L'homme-banane cannot live without Comic invocation card",
            "L'homme-banane",
            family: CardFamily.Comics
        ),
        new SacrificeCardMinAtkMinDefFamilyNumberAbility(
            AbilityName.Lolhitler2IncarnationSacrifice,
            "Lolhitler needs 2 Incarnation sacrifice to be invoke",
            "Lolhitler",
            0,
            0,
            CardFamily.Japan,
            2
        ),
        new DestroyFieldAtkDefAttackConditionAbility(
            AbilityName.LolhitlerDestroyField,
            "Lolhitler can destroy a field but divide Def by 2",
            "Lolhitler",
            1,
            2
        ),
        new GetSpecificCardFromDeckAbility(
            AbilityName.GetBenzaieJeuneFromDeck,
            "Get Benzaie jeune from deck to hand",
            "Benzaie jeune"
        ),
        new GetTypeCardFromDeckWithoutAttackAbility(
            AbilityName.ManuelGetEquipmentCardWithoutAttack,
            "Manuel Ferrara can get Equipment card and loose its attack phase",
            "Manuel Ferrara",
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
            "Mohammad",
            "Joueur Du Grenier",
            "Studio de développement",
            4.5f,
            4.5f
        ),
        new CantLiveWithoutAbility(
            AbilityName.MoiseCantLiveWithoutHuman,
            "Moïse, le plus grand de tous les hébreux cannot live without Human invocation card",
            "Moïse, le plus grand de tous les hébreux",
            family: CardFamily.Human
        ),
        new CopyAtkDefAbility(
            AbilityName.NounoursCopyBenzaieJeune,
            "Nounours copy atk and def Benzaie jeune",
            "Nounours",
            "Benzaie jeune"
        ),
        new LimitTurnExistenceAbility(
            AbilityName.Papy1TurnSurvive,
            "Papy Grenier survive only one turn",
            "Papy Grenier",
            1
        ),
        new GetSpecificCardAfterDeathAbility(
            AbilityName.PapyGiveDeathWhenDie,
            "Papy Grenier gives La Mort from deck to hand when he die",
            "La Mort",
            "Papy Grenier"
        ),
        new ProtectBehindDuringAttackDefConditionAbility(
            AbilityName.PatateProtectBehindGreaterDef,
            "Patate protect itself behind invocation card with greater def",
            "Patate"
        ),
        new SacrificeCardAbility(
            AbilityName.SacrificeSebDuGrenier,
            "Sacrifice Seb Du Grenier",
            "Seb Du Grenier"
        ),
        new WinAtkDefFamilyAbility(
            AbilityName.ProprioFistiliandWin1Atk1DefFistiland,
            "Propriétaire De Fistiland win 1 ATK and 1 DEF for every Fistiland on Field",
            "Propriétaire De Fistiland",
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
            AbilityName.SandrineKillEnemyIfDestroy,
            "Sandrine le porte-manteau extraterrestre kill its opponent if detroy by another invocation card",
            "Sandrine le porte-manteau extraterrestre"
        ),
        new SacrificeToInvokeAbility(
            AbilityName.SheikPointSacrificeToInvoke,
            "Sacrifice Sheik Point to invoke non-collector card from yellow trash",
            "Sheik Point"
        ),
        new GetSpecificCardFromDeckOrYellowCardAbility(
            AbilityName.GetPatronInfogramesFromDeckYellowTrash,
            "Get Patron D'Infogrames from deck or yellow trash",
            "Patron D'Infogrames"
        ),
        new CantLiveWithoutAbility(
            AbilityName.CantLiveWithoutGranolaxOrMechaGranolax,
            "Starlight Unicorn can't live without Granolax or Mecha-Granolax",
            "Starlight Unicorn",
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
            AbilityName.ScenaristeCanadienComesBackFromDeath5times,
            "Studio de scénaristes Canadien comes back to player hand everytime he died",
            "Studio de scénaristes Canadien",
            5
        ),
        new CantLiveWithoutAbility(
            AbilityName.TentaculesCantLiveWithoutJapon,
            "Tentacules cannot live without Japan invocation card",
            "Tentacules",
            family:CardFamily.Japan
        ),
        new CanOnlyAttackItselfAbility(
            AbilityName.CanOnlyAttackTentacules,
            "Opponent can only attack Tentacules",
            "Tentacules"
        ),
        new DrawCardsAbility(
            AbilityName.Draw2Cards,
            "Can draw maximum 2 card when invoke",
            2
        )
    };
}