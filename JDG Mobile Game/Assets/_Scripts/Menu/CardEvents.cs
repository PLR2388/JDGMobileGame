using System;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using UnityEngine.Events;

/// <summary>
/// Represents in-game card-based events as Unity events.
/// </summary>
[Serializable]
public class CardEvent : UnityEvent<InGameCard>
{
}

/// <summary>
/// Represents in-game invocation card events as Unity events.
/// </summary>
[Serializable]
public class InvocationCardEvent : UnityEvent<InGameInvocationCard>
{
}

/// <summary>
/// Represents in-game field card events as Unity events.
/// </summary>
[Serializable]
public class FieldCardEvent : UnityEvent<InGameFieldCard>
{
}

/// <summary>
/// Represents in-game effect card events as Unity events.
/// </summary>
[Serializable]
public class EffectCardEvent : UnityEvent<InGameEffectCard>
{
}

/// <summary>
/// Represents in-game equipment card events as Unity events.
/// </summary>
[Serializable]
public class EquipmentCardEvent : UnityEvent<InGameEquipmentCard>
{
}