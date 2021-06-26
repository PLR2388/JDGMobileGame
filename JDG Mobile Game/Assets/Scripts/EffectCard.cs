using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "EffectCard")]
public class EffectCard : Card
{
    [SerializeField] private EffectCardEffect effectCardEffect;
    [SerializeField] private int lifeTime; // Time in laps before the effect stop

    public EffectCardEffect GetEffectCardEffect()
    {
        return effectCardEffect;
    }

    private void Awake()
    {
        this.type = "effect";
    }

    public int GetLifeTime()
    {
        return lifeTime;
    }

    public void SetLifeTime(int life)
    {
        lifeTime = life;
    }

    public void DecrementLifeTime()
    {
        lifeTime -= 1;
    }
}