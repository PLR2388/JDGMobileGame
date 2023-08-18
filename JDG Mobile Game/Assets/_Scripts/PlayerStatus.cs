using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class ChangePvEvent : UnityEvent<float, bool>
{
}

public class PlayerStatus : MonoBehaviour
{
    public static readonly ChangePvEvent ChangePvEvent = new ChangePvEvent();
    public const float MaxPv = 30f;

    [FormerlySerializedAs("currentPV")] [SerializeField]
    private float currentPv = 30f;

    [SerializeField] private bool isP1;

    [SerializeField] private int numberShield;

    [SerializeField] private bool blockAttack;

    private bool skipCurrentDraw;

    public bool SkipCurrentDraw
    {
        get => skipCurrentDraw;
        set => skipCurrentDraw = value;
    }

    public int NumberShield => numberShield;

    public bool BlockAttack
    {
        get => blockAttack;
        set => blockAttack = value;
    }

    public void ChangePv(float pv)
    {
        currentPv += pv;
        if (currentPv > MaxPv)
        {
            currentPv = MaxPv;
        }

        ChangePvEvent.Invoke(currentPv, isP1);
    }

    public float GetCurrentPv()
    {
        return currentPv;
    }

    public void SetNumberShield(int number)
    {
        numberShield = number;
    }

    public void DecrementShield()
    {
        numberShield--;
    }
}