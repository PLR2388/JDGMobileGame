using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Application.Services;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;

/// <summary>
/// Provides conditions.
/// Phase 56: Originally wrapped legacy singleton for DI-compatible access.
/// Phase 84: Now owns the condition dictionary directly (ConditionLibrary deleted).
/// </summary>
#pragma warning disable CS0618 // Suppress obsolete warnings for legacy condition types
public class ConditionProviderService : IConditionProvider
{
    private readonly Dictionary<ConditionName, Condition> _conditionDictionary;

    /// <summary>
    /// Initializes the condition provider with all conditions.
    /// Phase 84: Moved from ConditionLibrary.
    /// </summary>
    public ConditionProviderService()
    {
        var conditions = new List<Condition>
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
                DomainCardFamily.Wizard
            ),
            new FieldCardOnFieldCondition(
                ConditionName.LyceeMagiqueGeorgesPompidouOnField,
                "Check if the current field card on field is Lycée magique Georges Pompidou",
                "Lycée magique Georges Pompidou"
            ),
            new SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(
                ConditionName.Developer3Atk3Def2Cards,
                "Check if at least 2 cards are on the field with Developer as Family and at least 3 ATK or 3 DEF",
                DomainCardFamily.Developer,
                3,
                3,
                2
            ),
            new SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(
                ConditionName.HardCorner3Atk3Def2Cards,
                "Check if at least 2 cards are on the field with HardCorner as Family and at least 3 ATK or 3 DEF",
                DomainCardFamily.HardCorner,
                3,
                3,
                2
            ),
            new SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(
                ConditionName.Japan2Cards,
                "Check if there are at least 2 cards with family Japan",
                DomainCardFamily.Japan,
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
                DomainCardFamily.Comics
            ),
            new SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(
                ConditionName.Incarnation2Cards,
                "Check if there is 2 card belonging to Incarnation family",
                DomainCardFamily.Incarnation,
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
                DomainCardFamily.Human
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
                DomainCardFamily.Japan
            )
        };

        _conditionDictionary = conditions.ToDictionary(condition => condition.Name, condition => condition);
    }

    /// <summary>
    /// Gets a condition by its name.
    /// </summary>
    /// <param name="conditionName">The condition name to look up.</param>
    /// <returns>The condition, or null if not found.</returns>
    public object GetCondition(object conditionName)
    {
        if (conditionName is not ConditionName name)
            return null;

        _conditionDictionary.TryGetValue(name, out var condition);
        return condition;
    }

    /// <summary>
    /// Gets a typed condition by its name.
    /// </summary>
    /// <param name="conditionName">The condition name to look up.</param>
    /// <returns>The condition, or null if not found.</returns>
    public Condition GetConditionTyped(ConditionName conditionName)
    {
        _conditionDictionary.TryGetValue(conditionName, out var condition);
        return condition;
    }

    /// <summary>
    /// Checks if a condition exists for the given name.
    /// </summary>
    /// <param name="conditionName">The condition name to check.</param>
    /// <returns>True if the condition exists.</returns>
    public bool HasCondition(object conditionName)
    {
        if (conditionName is not ConditionName name)
            return false;

        return _conditionDictionary.ContainsKey(name);
    }
}
#pragma warning restore CS0618
