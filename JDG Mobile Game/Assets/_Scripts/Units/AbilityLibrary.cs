using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;


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
            AbilityName.Win1Atk1DefDeveloper,
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
        )
    };
}