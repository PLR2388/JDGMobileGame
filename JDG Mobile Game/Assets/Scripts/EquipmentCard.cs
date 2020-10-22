using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card",menuName="EquipmentCard")]
public class EquipmentCard : Card
{
    private void Awake()
    {
        this.type= "equipment";
    }
}
