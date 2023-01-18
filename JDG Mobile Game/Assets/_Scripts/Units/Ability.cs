using UnityEngine;

public enum AbilityName
{
    CanOnlyAttackItself
}

public abstract class Ability
{
    public delegate void ApplyPower();
    public AbilityName Name { get; set; }
    public string Description { get; set; }
    public bool CanBeActivated { get; set; }
    public abstract void ApplyEffect();
    public event ApplyPower OnTurnStart;
    
    public void ApplyPowerOnTurnStart() {
        OnTurnStart?.Invoke();
    }
}