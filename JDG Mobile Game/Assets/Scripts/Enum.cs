using System.Collections;
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
    IncreaseATK,
    IncreaseDEF,
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