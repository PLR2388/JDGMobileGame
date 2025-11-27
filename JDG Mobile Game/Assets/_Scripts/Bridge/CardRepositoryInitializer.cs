using UnityEngine;
using JDG.Infrastructure.Repositories;
using Cards;

namespace JDG.Infrastructure.Bridge
{
    /// <summary>
    /// Bridge class that initializes CardRepository with ScriptableObject cards.
    /// Lives in default assembly so it can access both old (Cards) and new (JDG.Infrastructure) code.
    /// </summary>
    public static class CardRepositoryInitializer
    {
        /// <summary>
        /// Loads all ScriptableObject cards and converts them to domain entities.
        /// Must be called from default assembly since it needs access to old Card classes.
        /// </summary>
        public static void Initialize(CardRepository repository)
        {
            int loadedCount = 0;

            // Load all cards from Resources/Cards folder
            var allScriptableCards = Resources.LoadAll<Card>("Cards");

            foreach (var scriptableCard in allScriptableCards)
            {
                try
                {
                    // Convert ScriptableObject to domain Card entity
                    var domainCard = CardConverter.ConvertToDomain(scriptableCard);

                    if (domainCard != null)
                    {
                        repository.RegisterCardDefinition(domainCard);
                        loadedCount++;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to convert card '{scriptableCard.Title}': {ex.Message}");
                }
            }

            Debug.Log($"CardRepositoryInitializer: Loaded {loadedCount} card definitions from ScriptableObjects");
        }
    }
}
