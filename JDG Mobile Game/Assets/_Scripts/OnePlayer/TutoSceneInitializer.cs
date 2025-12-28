using UnityEngine;
using VContainer;

/// <summary>
/// Initializer for the tutorial scene that ensures decks are built before gameplay starts.
/// This fixes the issue where loading TutoPlayerGame directly from the Unity Editor
/// (instead of through SceneLoader.GoToTutorial()) results in empty decks.
///
/// When decks are empty, GameLoop.DoDraw() calls OnNoCards() which triggers GameOver()
/// and navigates back to MainScreen after about 1 second.
///
/// This script must run BEFORE PlayerCards.Construct() tries to access the decks.
/// By using VContainer injection, we ensure decks are populated during the DI phase.
/// </summary>
public class TutoSceneInitializer : MonoBehaviour
{
    /// <summary>
    /// VContainer injection point. Called during container build phase.
    /// Ensures tutorial decks are populated before any gameplay MonoBehaviour needs them.
    /// </summary>
    [Inject]
    public void Construct(IDeckManagementService deckManagementService)
    {
        // Check if decks are empty (happens when loading from Editor directly)
        if (deckManagementService.Player1DeckCards == null ||
            deckManagementService.Player1DeckCards.Count == 0 ||
            deckManagementService.Player2DeckCards == null ||
            deckManagementService.Player2DeckCards.Count == 0)
        {
            Debug.Log("TutoSceneInitializer: Decks empty, building tutorial decks for Editor playback");
            deckManagementService.BuildTutorialDecks();
            Debug.Log($"TutoSceneInitializer: Built tutorial decks - P1: {deckManagementService.Player1DeckCards?.Count ?? 0} cards, P2: {deckManagementService.Player2DeckCards?.Count ?? 0} cards");
        }
        else
        {
            Debug.Log($"TutoSceneInitializer: Decks already populated - P1: {deckManagementService.Player1DeckCards.Count} cards, P2: {deckManagementService.Player2DeckCards.Count} cards");
        }
    }
}
