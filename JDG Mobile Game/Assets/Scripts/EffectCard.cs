using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card",menuName="EffectCard")]
public class EffectCard : Card
{
    private void Awake()
    {
        this.type = "effect";
    }
}
