using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class changePVEvent : UnityEvent<float,bool>
{
}

public class PlayerStatus : MonoBehaviour
{

    public static changePVEvent ChangePvEvent = new changePVEvent();
     public static float maxPV = 30f;

    [SerializeField] private float currentPV = 30f;

    [SerializeField] private bool isP1 = false;

    public void changePV(float pv)
    {
        currentPV += pv;
        if (currentPV > maxPV)
        {
            currentPV = maxPV;
        }
        
        ChangePvEvent.Invoke(currentPV,isP1);
    }

    public float getCurrentPV()
    {
        return currentPV;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
