using TMPro;
using UnityEngine;

/// <summary>
/// Represents the user interface for displaying player health.
/// </summary>
public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthP1Text;
    [SerializeField] private TextMeshProUGUI healthP2Text;

    // Reference to the PlayerStatus objects could be useful but not shown here
    // for simplicity.

    /// <summary>
    /// Initialization logic for the health UI.
    /// Registers for health change events from the PlayerStatus.
    /// </summary>
    private void Awake()
    {
        // Initialize the health text for both players at the start.
        SetHealthText(PlayerStatus.MaxHealth, true);
        SetHealthText(PlayerStatus.MaxHealth, false);
        
        // Listens to a health change event if PlayerStatus broadcasts one.
        // This way, whenever a player's health changes, it'll automatically update
        // the UI without UIManager needing to manually call it.
        PlayerStatus.OnHealthChanged.AddListener(ChangeHealthText);
    }

    /// <summary>
    /// Updates the displayed health value for a given player.
    /// </summary>
    /// <param name="health">The health value to display.</param>
    /// <param name="isP1">Indicates if the health update is for Player 1. Otherwise, it's for Player 2.</param>
    private void SetHealthText(float health, bool isP1)
    {
        if (isP1)
        {
            healthP1Text.SetText($"{health} / {PlayerStatus.MaxHealth}");
        }
        else
        {
            healthP2Text.SetText($"{health} / {PlayerStatus.MaxHealth}");
        }
    }

    /// <summary>
    /// Handles the health change event from PlayerStatus.
    /// </summary>
    /// <param name="health">The updated health value.</param>
    /// <param name="isP1">If true, the health update is for Player 1; otherwise, it's for Player 2.</param>
    private void ChangeHealthText(float health, bool isP1)
    {
        SetHealthText(health, isP1);
    }

    /// <summary>
    /// Unregisters the event listener when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        PlayerStatus.OnHealthChanged.RemoveListener(ChangeHealthText);
    }
}