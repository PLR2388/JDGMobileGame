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
    public const float MAXPv = 30f;

    [FormerlySerializedAs("currentPV")] [SerializeField]
    private float currentPv = 30f;

    [SerializeField] private bool isP1 = false;

    [SerializeField] private int numberShield = 0;

    public int NumberShield => numberShield;

    public void ChangePv(float pv)
    {
        currentPv += pv;
        if (currentPv > MAXPv)
        {
            currentPv = MAXPv;
        }

        ChangePvEvent.Invoke(currentPv, isP1);
    }

    public float GETCurrentPv()
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