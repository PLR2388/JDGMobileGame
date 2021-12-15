using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "InvocationCard")]
public class InvocationCard : Card
{
    [SerializeField] protected float attack;
    [SerializeField] protected float defense;
    [SerializeField] private CardFamily[] family;
    [SerializeField] private EquipmentCard equipmentCard;
    [SerializeField] private InvocationConditions invocationConditions;
    [SerializeField] private InvocationStartEffect invocationStartEffect;
    [SerializeField] private InvocationPermEffect invocationPermEffect;
    [SerializeField] private InvocationActionEffect invocationActionEffect;
    [SerializeField] private InvocationDeathEffect invocationDeathEffect;
    [SerializeField] private float bonusAttack;
    [SerializeField] private float bonusDefense;
    [SerializeField] private int numberTurnOnField;
    [SerializeField] private int numberDeaths;
    [SerializeField] private bool blockAttackNextTurn = false;
    [SerializeField] private bool affectedByEffect = true;
    private bool hasAlreadyAttackThisTurn = false;
    private CardFamily? currentFamily = null;

    private void Awake()
    {
        this.type = CardType.Invocation;
        this.bonusAttack = 0;
        this.bonusDefense = 0;
        this.numberTurnOnField = 0;
        this.numberDeaths = 0;
    }

    public int NumberTurnOnField => numberTurnOnField;

    public void incrementNumberTurnOnField()
    {
        numberTurnOnField++;
    }

    public void DeactivateEffect()
    {
    }

    public void Init()
    {
        numberTurnOnField = 0;
        numberDeaths = 0;
        SetEquipmentCard(null);
        SetBonusAttack(0);
        SetBonusDefense(0);
        SetCurrentFamily(null);
        if (invocationActionEffect != null)
        {
            var keys = invocationActionEffect.Keys;
            var values = invocationActionEffect.Values;
            for (var i = keys.Count - 1; i >= 0; i--)
            {
                if (keys[i] != ActionEffect.Beneficiary) continue;
                keys.Remove(keys[i]);
                values.Remove(values[i]);
            }
        }
    }

    public void SetCurrentFamily(CardFamily? family)
    {
        currentFamily = family;
    }

    public float GETBonusDefense() => bonusDefense;

    public void SetBonusDefense(float bonus)
    {
        bonusDefense = bonus;
        if (equipmentCard != null)
        {
            var instantEffect = equipmentCard.EquipmentInstantEffect;
            if (instantEffect != null)
            {
                if (instantEffect.Keys.Contains(InstantEffect.SetDef))
                {
                    var index = instantEffect.Keys.IndexOf(InstantEffect.SetDef);
                    var def = float.Parse(instantEffect.Values[index]);
                    bonusDefense = def - GetDefense();
                }
            }
        }
    }

    public InvocationActionEffect InvocationActionEffect
    {
        get { return invocationActionEffect; }
        set
        {
            invocationActionEffect = value;
        }
    }

    public InvocationConditions InvocationConditions => invocationConditions;
    
    public InvocationPermEffect InvocationPermEffect => invocationPermEffect;

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
        if (equipmentCard != null)
        {
            var instantEffect = equipmentCard.EquipmentInstantEffect;
            if (instantEffect != null)
            {
                if (instantEffect.Keys.Contains(InstantEffect.SetAtk))
                {
                    var index = instantEffect.Keys.IndexOf(InstantEffect.SetAtk);
                    var atk = float.Parse(instantEffect.Values[index]);
                    bonusAttack = atk - GetAttack();
                }
            }
        }
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
        if (currentFamily.HasValue)
        {
            return new []
            {
                currentFamily.Value
            };
        }
        else
        {
            return family;
        }
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
        if (equipmentCard == null) return !hasAlreadyAttackThisTurn && !blockAttackNextTurn;
        var instantEffect = equipmentCard.EquipmentInstantEffect;
        var equipmentBlockedAttack = false;
        if (instantEffect != null)
        {
            equipmentBlockedAttack = instantEffect.Keys.Contains(InstantEffect.BlockAtk);
        }
        return !hasAlreadyAttackThisTurn && !blockAttackNextTurn & !equipmentBlockedAttack;
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
    
    public float GetBonusDefense()
    {
        return bonusDefense;
    }

    public float GetBonusAttack()
    {
        return bonusAttack;
    }

    public EquipmentCard GETEquipmentCard()
    {
        return equipmentCard;
    }

    public void SetEquipmentCard(EquipmentCard card)
    {
        if (equipmentCard != null && card == null)
        {
            RemoveEquipmentCardEffect(equipmentCard.EquipmentInstantEffect);
        }
        equipmentCard = card;
    }

    private void RemoveEquipmentCardEffect(EquipmentInstantEffect equipmentCardEquipmentInstantEffect)
    {
        if (equipmentCardEquipmentInstantEffect != null)
        {
            var keys = equipmentCardEquipmentInstantEffect.Keys;
            var values = equipmentCardEquipmentInstantEffect.Values;
            for (var i = 0; i < keys.Count; i++)
            {
                 switch (keys[i])
            {
                case InstantEffect.AddAtk:
                {
                    var newBonusAttack = -float.Parse(values[i]) + GetBonusAttack();
                    SetBonusAttack(newBonusAttack);
                }
                    break;
                case InstantEffect.AddDef:
                {
                    var newBonusDefense = -float.Parse(values[i]) + GetBonusDefense();
                    SetBonusDefense(newBonusDefense);
                }
                    break;
                case InstantEffect.MultiplyAtk:
                {
                    var multiplicator = int.Parse(values[i]);
                    if (multiplicator > 1)
                    {
                        var newBonusAttack = -(multiplicator - 1) * GetAttack() + GetBonusAttack();
                        SetBonusAttack(newBonusAttack);
                    }
                }
                    break;
                case InstantEffect.MultiplyDef:
                {
                    var multiplicator = int.Parse(values[i]);
                    if (multiplicator > 1)
                    {
                        var newBonusDefense = -(multiplicator - 1) * GetDefense() + GetBonusDefense();
                        SetBonusDefense(newBonusDefense);
                    } else if (multiplicator < 0)
                    {
                        var newBonusDefense = -(GetDefense() / multiplicator) + GetBonusDefense();
                        SetBonusDefense(newBonusDefense);
                    }
               
                }
                    break;
                case InstantEffect.SetAtk:
                {
                    SetBonusAttack(0);
                }
                    break;
                case InstantEffect.SetDef:
                {
                    SetBonusDefense(0);
                }
                    break;
                case InstantEffect.BlockAtk:
                {
                    UnblockAttack();
                }
                    break;
                case InstantEffect.DirectAtk:
                {
                }
                    break;
                case InstantEffect.SwitchEquipment:
                {
                }
                    break;
                case InstantEffect.DisableBonus:
                {
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            }
        }
    }

    public int GETNumberDeaths()
    {
        return numberDeaths;
    }

    public bool IsInvocationPossible()
    {
        return InvocationFunctions.IsInvocationPossible(invocationConditions,
            GameLoop.IsP1Turn ? "Player1" : "Player2");
    }

    private void DealWithStartEquipmentInstantEffect(EquipmentInstantEffect equipmentInstantEffect)
    {
        var keys = equipmentInstantEffect.Keys;
        var values = equipmentInstantEffect.Values;

        for (var i=0; i<keys.Count; i++)
        {
            var value = values[i];
            switch (keys[i])
            {
                case InstantEffect.AddAtk:
                {
                    var attack = float.Parse(value);
                    bonusAttack += attack;
                }
                    break;
                case InstantEffect.AddDef:
                {
                    var defense = float.Parse(value);
                    bonusDefense += defense;
                }
                    break;
                default:
                    break;
            }
        }
    }
    
    private void DealWithEndEquipmentInstantEffect(EquipmentInstantEffect equipmentInstantEffect)
    {
        var keys = equipmentInstantEffect.Keys;
        var values = equipmentInstantEffect.Values;

        for (var i=0; i<keys.Count; i++)
        {
            var value = values[i];
            switch (keys[i])
            {
                case InstantEffect.AddAtk:
                {
                    var attack = float.Parse(value);
                    bonusAttack -= attack;
                }
                    break;
                case InstantEffect.AddDef:
                {
                    var defense = float.Parse(value);
                    bonusDefense -= defense;
                }
                    break;
                default:
                    break;
            }
        }
    }
}