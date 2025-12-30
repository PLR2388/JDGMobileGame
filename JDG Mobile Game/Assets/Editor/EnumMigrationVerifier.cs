using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JDG.Editor
{
    /// <summary>
    /// Phase 107: Editor script to verify enum compatibility between legacy and domain enums.
    /// Confirms that integer values match, ensuring ScriptableObjects don't need migration.
    /// </summary>
    public static class EnumMigrationVerifier
    {
        [MenuItem("JDG/Migration/Verify Enum Compatibility")]
        public static void VerifyEnumCompatibility()
        {
            Debug.Log("=== Enum Migration Verification ===");

            bool allPassed = true;

            // Verify CardType
            allPassed &= VerifyEnumValues<Cards.CardType, JDG.Domain.Enums.CardType>("CardType");

            // Verify CardFamily
            allPassed &= VerifyEnumValues<Cards.CardFamily, JDG.Domain.Enums.CardFamily>("CardFamily");

            // Verify EffectAbilityName
            allPassed &= VerifyEnumValues<EffectAbilityName, JDG.Domain.Enums.EffectAbilityName>("EffectAbilityName");

            // Verify EquipmentAbilityName
            allPassed &= VerifyEnumValues<EquipmentAbilityName, JDG.Domain.Enums.EquipmentAbilityName>("EquipmentAbilityName");

            // Verify FieldAbilityName
            allPassed &= VerifyEnumValues<FieldAbilityName, JDG.Domain.Enums.FieldAbilityName>("FieldAbilityName");

            if (allPassed)
            {
                Debug.Log("<color=green>All enum verifications PASSED!</color>");
                Debug.Log("ScriptableObjects use integer serialization - no asset migration needed.");
            }
            else
            {
                Debug.LogError("Some enum verifications FAILED! Review the logs above.");
            }
        }

        private static bool VerifyEnumValues<TLegacy, TDomain>(string enumName)
            where TLegacy : Enum
            where TDomain : Enum
        {
            var legacyValues = Enum.GetValues(typeof(TLegacy)).Cast<TLegacy>().ToList();
            var domainValues = Enum.GetValues(typeof(TDomain)).Cast<TDomain>().ToList();

            bool passed = true;

            // Check count matches
            if (legacyValues.Count != domainValues.Count)
            {
                Debug.LogError($"[{enumName}] Value count mismatch: Legacy={legacyValues.Count}, Domain={domainValues.Count}");
                passed = false;
            }

            // Check each value has matching integer and name
            foreach (var legacyValue in legacyValues)
            {
                int legacyInt = Convert.ToInt32(legacyValue);
                string legacyName = legacyValue.ToString();

                // Find matching domain value by integer
                var matchingDomain = domainValues.FirstOrDefault(d => Convert.ToInt32(d) == legacyInt);

                if (matchingDomain == null)
                {
                    Debug.LogError($"[{enumName}] No domain value for legacy {legacyName}={legacyInt}");
                    passed = false;
                    continue;
                }

                string domainName = matchingDomain.ToString();
                if (legacyName != domainName)
                {
                    Debug.LogWarning($"[{enumName}] Name mismatch at {legacyInt}: Legacy={legacyName}, Domain={domainName}");
                    // Name mismatch is a warning, not a failure (integer is what matters for serialization)
                }
            }

            if (passed)
            {
                Debug.Log($"[{enumName}] <color=green>PASSED</color> - {legacyValues.Count} values verified");
            }

            return passed;
        }

        [MenuItem("JDG/Migration/Report Legacy Enum Usage")]
        public static void ReportLegacyEnumUsage()
        {
            Debug.Log("=== Legacy Enum Usage Report ===");
            Debug.Log("The following legacy enums are marked [Obsolete]:");
            Debug.Log("- Cards.CardType -> Use JDG.Domain.Enums.CardType");
            Debug.Log("- Cards.CardFamily -> Use JDG.Domain.Enums.CardFamily");
            Debug.Log("- Cards.CardOwner -> Use JDG.Domain.CardOwner");
            Debug.Log("- EffectAbilityName -> Use JDG.Domain.Enums.EffectAbilityName");
            Debug.Log("- EquipmentAbilityName -> Use JDG.Domain.Enums.EquipmentAbilityName");
            Debug.Log("- FieldAbilityName -> Use JDG.Domain.Enums.FieldAbilityName");
            Debug.Log("");
            Debug.Log("Migration Strategy:");
            Debug.Log("1. ScriptableObjects: No changes needed (integer serialization)");
            Debug.Log("2. Code: Use 'using DomainXxx = JDG.Domain.Enums.Xxx;' pattern");
            Debug.Log("3. Legacy enums will be deleted in Phase 110 after verification");
        }
    }
}
