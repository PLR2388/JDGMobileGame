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
    [SerializeField] private string bonusFamily = null;
    [SerializeField] private float numberTurnOnField;
    [SerializeField] private int numberDeaths;
    [SerializeField] private bool blockAttackNextTurn = false;
    private bool hasAlreadyAttackThisTurn = false;

    private void Awake()
    {
        this.type = "invocation";
        this.bonusAttack = 0;
        this.bonusDefense = 0;
        this.numberTurnOnField = 0;
        this.numberDeaths = 0;
    }

    public float getBonusDefense()
    {
        return bonusDefense;
    }

    public void setBonusDefense(float bonus)
    {
        bonusDefense = bonus;
    }

    public InvocationActionEffect InvocationActionEffect
    {
        get { return invocationActionEffect; }
    }

    public void blockAttack()
    {
        blockAttackNextTurn = true;
    }

    public void unblockAttack()
    {
        blockAttackNextTurn = false;
    }

    public float getBonusAttack()
    {
        return bonusAttack;
    }

    public void setBonusAttack(float bonus)
    {
        bonusAttack = bonus;
    }

    public void incrementNumberDeaths()
    {
        numberDeaths++;
    }

    public void resetNumberDeaths()
    {
        numberDeaths = 0;
    }


    public String[] GetFamily()
    {
        return family;
    }

    /**
     * Use it if we override the family
     */
    public string[] GetCurrentFamily()
    {
        if (bonusFamily != null)
        {
            return new[] {bonusFamily};
        }
        else
        {
            return family;
        }
    }

    public void SetCurrentFamily(string familyString)
    {
        bonusFamily = familyString;
    }

    public void UnSetCurrentFamily()
    {
        bonusFamily = null;
    }

    public InvocationStartEffect GetInvocationStartEffect()
    {
        return invocationStartEffect;
    }

    public InvocationDeathEffect GetInvocationDeathEffect()
    {
        return invocationDeathEffect;
    }

    public float GetAttack()
    {
        return attack;
    }

    public float GetDefense()
    {
        return defense;
    }

    public bool hasAttack()
    {
        return hasAlreadyAttackThisTurn;
    }

    public void attackTurnDone()
    {
        hasAlreadyAttackThisTurn = true;
    }

    public void resetNewTurn()
    {
        hasAlreadyAttackThisTurn = false;
    }

    public float GetCurrentDefense()
    {
        return defense + bonusDefense;
    }

    public float GetCurrentAttack()
    {
        return attack + bonusAttack;
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
            return InvocationFonctions.isInvocationPossible(this.invocationConditions, "Player1");
        }
        else
        {
            return InvocationFonctions.isInvocationPossible(this.invocationConditions, "Player2");
        }
    }
}