using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class ChangePvEvent : UnityEvent<float, bool>
{
}

public class PlayerStatus : MonoBehaviour
{
    public static readonly ChangePvEvent OnHealthChanged = new ChangePvEvent();
    public const float MaxHealth = 30f;
    
    [SerializeField]
    private float currentHealth = 30f;

    [SerializeField] private bool isP1;

    /// <summary>
    /// Gets the number of shields the player has.
    /// </summary>
    public int NumberShield { get; private set; }

    /// <summary>
    /// Gets or sets whether the player can block an attack.
    /// </summary>
    public bool BlockAttack { get; private set; }

    /// <summary>
    /// Changes the player's health by the given amount and triggers the OnHealthChanged event.
    /// </summary>
    /// <param name="pv">Amount to change the health by (can be positive or negative).</param>
    public void ChangePv(float pv)
    {
        currentHealth += pv;
        if (currentHealth > MaxHealth)
        {
            currentHealth = MaxHealth;
        }

        OnHealthChanged.Invoke(currentHealth, isP1);
    }

    /// <summary>
    /// Gets the current health of the player.
    /// </summary>
    /// <returns>The current health value.</returns>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Sets the shield count for the player.
    /// </summary>
    /// <param name="number">The number of shields to set.</param>
    public void SetShieldCount(int number)
    {
        NumberShield = number;
    }

    /// <summary>
    /// Decreases the shield count by one.
    /// </summary>
    public void DecrementShield()
    {
        NumberShield--;
    }
    
    /// <summary>
    /// Enables the player to block an attack.
    /// </summary>
    public void EnableBlockAttack()
    {
        BlockAttack = true;
    }
    
    /// <summary>
    /// Disables the player's ability to block an attack.
    /// </summary>
    public void DisableBlockAttack()
    {
        BlockAttack = false;
    }
}