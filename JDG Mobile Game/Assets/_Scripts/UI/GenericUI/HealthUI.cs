using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthP1Text;
    [SerializeField] private TextMeshProUGUI healthP2Text;

    // Reference to the PlayerStatus objects could be useful but not shown here
    // for simplicity.

    private void Awake()
    {
        // Initialize the health text for both players at the start.
        SetHealthText(PlayerStatus.MaxPv, true);
        SetHealthText(PlayerStatus.MaxPv, false);
        
        // We can listen to a health change event if PlayerStatus broadcasts one.
        // This way, whenever a player's health changes, it'll automatically update
        // the UI without UIManager needing to manually call it.
        PlayerStatus.ChangePvEvent.AddListener(ChangeHealthText);
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
            healthP1Text.SetText($"{health} / {PlayerStatus.MaxPv}");
        }
        else
        {
            healthP2Text.SetText($"{health} / {PlayerStatus.MaxPv}");
        }
    }

    // This method assumes PlayerStatus broadcasts a health change event with both 
    // the new health value and a flag indicating which player's health changed.
    private void ChangeHealthText(float health, bool isP1)
    {
        SetHealthText(health, isP1);
    }

    // OnDestroy or OnDisable could be used to unregister the event listener if needed.
    private void OnDestroy()
    {
        PlayerStatus.ChangePvEvent.RemoveListener(ChangeHealthText);
    }
}