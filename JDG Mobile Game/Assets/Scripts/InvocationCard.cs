using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Card", menuName = "InvocationCard")]
public class InvocationCard : Card
{
    [SerializeField] private float attack;
    [SerializeField] private float defense;
    [SerializeField] private string[] family;
    [SerializeField] private EquipmentCard equipmentCard;

    private void Awake()
    {
        this.type = "invocation";
    }

    public String[] GetFamily()
    {
        return family;
    }

    public float GetAttack()
    {
        return attack;
    }

    public float GetDefense()
    {
        return defense;
    }

    public EquipmentCard getEquipmentCard()
    {
        return equipmentCard;
    }
}