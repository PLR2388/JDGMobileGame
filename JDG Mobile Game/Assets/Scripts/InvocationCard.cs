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
    [SerializeField] private InvocationConditions invocationConditions;
    [SerializeField] private InvocationStartEffect invocationStartEffect;
    [SerializeField] private InvocationPermEffect invocationPermEffect;
    [SerializeField] private InvocationActionEffect invocationActionEffect;
    [SerializeField] private InvocationDeathEffect invocationDeathEffect;
    [SerializeField] private float bonusAttack;
    [SerializeField] private float bonusDefense;
    [SerializeField] private float numberTurnOnField;
    [SerializeField] private int numberDeaths;


    private void Awake()
    {
        this.type = "invocation";
        this.bonusAttack = 0;
        this.bonusDefense = 0;
        this.numberTurnOnField = 0;
        this.numberDeaths = 0;
    }


    public String[] GetFamily()
    {
        return family;
    }

    public InvocationStartEffect GetInvocationStartEffect()
    {
        return invocationStartEffect;
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

    public int getNumberDeaths()
    {
        return numberDeaths;
    }

    public bool isInvocationPossible()
    {
        if (GameLoop.isP1Turn)
        {
            return InvocationFonctions.isInvocationPossible(this.invocationConditions,"Player1");
        }
        else
        {
            return InvocationFonctions.isInvocationPossible(this.invocationConditions,"Player2");
        }
    }
    
    
}