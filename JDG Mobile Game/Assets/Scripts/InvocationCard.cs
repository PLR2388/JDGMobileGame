using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "InvocationCard")]
public class InvocationCard : Card
{
    [SerializeField] private float attack;
    [SerializeField] private float defense;
    [SerializeField] private CardFamily[] family;
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
    [SerializeField] private bool blockAttackNextTurn = false;
    private bool hasAlreadyAttackThisTurn = false;

    private void Awake()
    {
        this.type = CardType.Invocation;
        this.bonusAttack = 0;
        this.bonusDefense = 0;
        this.numberTurnOnField = 0;
        this.numberDeaths = 0;
    }

    public float GETBonusDefense() => bonusDefense;

    public void SetBonusDefense(float bonus)
    {
        bonusDefense = bonus;
    }

    public InvocationActionEffect InvocationActionEffect => invocationActionEffect;

    public void BlockAttack()
    {
        blockAttackNextTurn = true;
    }

    public void UnblockAttack()
    {
        blockAttackNextTurn = false;
    }

    public float GETBonusAttack()
    {
        return bonusAttack;
    }

    public void SetBonusAttack(float bonus)
    {
        bonusAttack = bonus;
    }

    public void IncrementNumberDeaths()
    {
        numberDeaths++;
    }

    public void ResetNumberDeaths()
    {
        numberDeaths = 0;
    }


    public CardFamily[] GetFamily()
    {
        return family;
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

    public bool CanAttack()
    {
        return !hasAlreadyAttackThisTurn && !blockAttackNextTurn;
    }

    public void AttackTurnDone()
    {
        hasAlreadyAttackThisTurn = true;
    }

    public void ResetNewTurn()
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

    public EquipmentCard GETEquipmentCard()
    {
        return equipmentCard;
    }

    public void SetEquipmentCard(EquipmentCard card)
    {
        this.equipmentCard = card;
    }

    public int GETNumberDeaths()
    {
        return numberDeaths;
    }

    public bool IsInvocationPossible()
    {
        return InvocationFunctions.IsInvocationPossible(this.invocationConditions,
            GameLoop.IsP1Turn ? "Player1" : "Player2");
    }
}