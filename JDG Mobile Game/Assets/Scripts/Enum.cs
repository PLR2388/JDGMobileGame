﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Condition
{
    SpecificCardOnField,
    SacrificeSpecificCard,
    SpecificEquipmentAttached,
    SpecificField,
    SacrificeFamily,
    SpecificFamilyOnField,
    NumberCard,
    SacrificeThresholdATK,
    SacrificeThresholdDEF,
    NumberInvocationCardInYellowTrash, 
    ComeFromYellowTrash //true or false
}

public enum StartEffect
{
    GetSpecificCard,
    GetSpecificTypeCard,
    GetCardFamily,
    GetCardSource,
    RemoveAllInvocationCards,
    InvokeSpecificCard,
    PutField,
    DestroyField,
    Divide2ATK,
    Divide2DEF,
    SendToDeath,
    DrawXCards,
    Condition
}

public enum PermEffect
{
    CanOnlyAttackIt,
    GiveStat,
    IncreaseATK,
    IncreaseDEF, //Increase ATK or DEF depending of the number of same family cards on field
    Family,
    PreventInvocationCards,
    ProtectBehind,
    ImpossibleAttackByInvocation,
    ImpossibleToBeAffectedByEffect,
    Condition,
    NumberTurn
}

public enum ActionEffect
{
    SacrificeInvocation,
    SpecificField,
    IncreaseStarsATKAndDEF,
    GiveATK,
    GiveDEF,
    SpecificFamily,
    BackToLife,
    SkipOpponentAttack
}

public enum DeathEffect
{
    ComeBackToHand,
    GetSpecificCard,
    GetCardSource,
    KillAlsoOtherCard
}

// Equipment
public enum InstantEffect //Happen only at the beginning
{
    AddDEF,
    AddATK,
    MultiplyATK,
    MultiplyDEF,
    SetATK,
    SetDEF,
    BlockATK,
    DirectATK, // Direct attack opponent stars
    SwitchEquipment, // Change previous equipmentCard by this one
    DisableBonus, // Remove native card bonus
}

public enum PermanentEffect //Must be frequently change
{
    AddATKBaseOnHandCards,
    AddDEFBaseOnHandCards,
    BlockOpponentDuringInvocation, // Prevent opponent's attack during invocation
    PreventAttackOnInvocation, // Prevent opponent to attack this card
}

// Effect
public enum Effect
{
    AffectPV, // Indicate unit numberofPv
    AffectOpponent,
    NumberInvocationCard,
    NumberHandCard,
    DestroyCards,
    SacrificeInvocation,
    SameFamily,
    CheckTurn,
    ChangeHandCards,
    Sources,
    HandMax,
    SeeOpponentHand,
    RemoveCardOption,
    RemoveHand,
    RemoveDeck,
    SpecialInvocation,
    InvocationStat,
    Duration,
    Combine,
    RevertStat,
    TakeControl,
    NumberAttacks,
    SkipAttack,
    SeeCards,
    ChangeOrder,
    AttackDirectly, // if PV less than or equal to this number, can attack directly
    ProtectAttack,
    SkipFieldsEffect,
    ChangeField,
    SkipContre
}