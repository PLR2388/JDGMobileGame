using Cards;
using UnityEngine;

/// <summary>
/// Adapter service that bridges ICardPoolService to CardPoolManager.
/// Phase 9: Temporary bridge during migration from singleton to DI.
/// Phase 19-20: Now injects CardPoolManager instead of using .Instance.
/// </summary>
public class CardPoolService : ICardPoolService
{
    private readonly CardPoolManager _cardPoolManager;

    public CardPoolService(CardPoolManager cardPoolManager)
    {
        _cardPoolManager = cardPoolManager;
    }

    public Transform CardPoolHolder => _cardPoolManager.cardPoolHolder;

    public GameObject GetPooledObject(InGameCard inGameCard)
    {
        return _cardPoolManager.GetPooledObject(inGameCard);
    }
}
