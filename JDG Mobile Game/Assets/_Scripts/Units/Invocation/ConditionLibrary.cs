using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation.Condition;
using Cards;

/// <summary>
/// Represents a library of conditions used in the game.
/// </summary>
public class ConditionLibrary : StaticInstance<ConditionLibrary>
{
    private readonly List<Condition> conditions = new()
    {
        new InvocationCardOnFieldCondition(
            ConditionName.BenzaieJeuneOrBenzaieOnField,
            "Check if Benzaie jeune or Benzaie are on the field",
            new List<string>
            {
                "Benzaie jeune",
                "Benzaie"
            }
        ),
        new FieldCardOnFieldCondition(
            ConditionName.ZozanKebabOnField,
            "Check if the current field card on field is Zozan Kebab",
            "Zozan Kebab"
        ),
        new InvocationCardOnFieldCondition(
            ConditionName.ArchibalVonGrenierOnField,
            "Check if Archibald Von Grenier is on the field",
            new List<string>
            {
                "Archibald Von Grenier"
            }
        ),
        new EquipmentCardOnCardCondition(
            ConditionName.BenzaieJeuneCassetteVhsEquiped,
            "Check if Benzaie jeune has a Cassette VHS",
            "Cassette VHS",
            "Benzaie jeune"),
        new EquipmentCardOnCardCondition(
            ConditionName.JoueurDuGrenierCanarangEquiped,
            "Check if Joueur du Grenier has Canarang",
            "Canarang",
            "Joueur Du Grenier"
        ),
        new SpecificAtkDefInvocationCardOnFieldCondition(
            ConditionName.ThreeAtk3Def,
            "Check if there is an invocation card with at least 3 ATK or 3 DEF on field",
            3,
            3
        ),
        new FieldCardOnFieldCondition(
            ConditionName.ForetDesElfesSylvainsOnField,
            "Check if the current field card on field is Forêt des elfes sylvains",
            "Forêt des elfes sylvains"
        ),
        new InvocationCardOnFieldCondition(
            ConditionName.JoueurDuGrenierOnFieldCondition,
            "Check if Joueur Du Grenier is on the field",
            new List<string>
            {
                "Joueur Du Grenier"
            }
        ),
        new SpecificFamilyInvocationCardOnFieldCondition(
            ConditionName.WizardOnField,
            "Check if an invocation card of the wizard family is on Field",
            CardFamily.Wizard
        ),
        new FieldCardOnFieldCondition(
            ConditionName.LyceeMagiqueGeorgesPompidouOnField,
            "Check if the current field card on field is Lycée magique Georges Pompidou",
            "Lycée magique Georges Pompidou"
        ),
        new SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(
            ConditionName.Developer3Atk3Def2Cards,
            "Check if at least 2 cards are on the field with Developer as Family and at least 3 ATK or 3 DEF",
            CardFamily.Developer,
            3,
            3,
            2
        ),
        new SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(
            ConditionName.HardCorner3Atk3Def2Cards,
            "Check if at least 2 cards are on the field with HardCorner as Family and at least 3 ATK or 3 DEF",
            CardFamily.HardCorner,
            3,
            3,
            2
        ),
        new SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(
            ConditionName.Japan2Cards,
            "Check if there are at least 2 cards with family Japan",
            CardFamily.Japan,
            2
        ),
        new NumberInvocationDeadCondition(
            ConditionName.TenDeathYellowTrash,
            "Check if there are 10 invocations cards in the yellow trash",
            10
        ),
        new SpecificFamilyInvocationCardOnFieldCondition(
            ConditionName.ComicsOnField,
            "Check if there is an invocation card with Comics Family",
            CardFamily.Comics
        ),
        new SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(
            ConditionName.Incarnation2Cards,
            "Check if there is 2 card belonging to Incarnation family",
            CardFamily.Incarnation,
            2
        ),
        new SpecificCardBackFromDeathCondition(
            ConditionName.GranolaxAlreadyDead,
            "Check if Granolax was resurected",
            new List<string>
            {
                "Granolax"
            }
        ),
        new SpecificFamilyInvocationCardOnFieldCondition(
            ConditionName.HumanOnField,
            "Check if there is invocation card bellonging to Human family",
            CardFamily.Human
        ),
        new EquipmentCardOnCardCondition(
            ConditionName.SebDuGrenierMerdePlastiqueBleuEquiped,
            "Check if Seb du Grenier has Merde tournoyante en plastique bleu",
            "Merde tournoyante en plastique bleu",
            "Seb Du Grenier"
        ),
        new InvocationCardOnFieldCondition(
            ConditionName.SebDuGrenierOnField,
            "Check if Seb Du Grenier is on Field",
            new List<string>
            {
                "Seb Du Grenier"
            }
        ),
        new EquipmentCardOnCardCondition(
            ConditionName.ClicheRacisteMerdeRoseEquiped,
            "Check if Cliché raciste has ",
            "Merde magique en plastique rose",
            "Cliché Raciste"
        ),
        new InvocationCardOnFieldCondition(
            ConditionName.MechaGronolaxOrGranolaxOnField,
            "Check if Granolax or Mecha-Granolax are on the field",
            new List<string>
            {
                "Granolax",
                "Mecha-Granolax"
            }
        ),
        new SpecificFamilyInvocationCardOnFieldCondition(
            ConditionName.JapanOnField,
            "Check if there are cards whose family is Japan",
            CardFamily.Japan
        )
    };

    /// <summary>
    /// Gets or sets the dictionary that maps condition names to condition instances.
    /// </summary>
    public Dictionary<ConditionName, Condition> ConditionDictionary;

    /// <summary>
    /// Initializes the condition library and populates the condition dictionary.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        ConditionDictionary = conditions.ToDictionary(condition => condition.Name, condition => condition);
    }
}