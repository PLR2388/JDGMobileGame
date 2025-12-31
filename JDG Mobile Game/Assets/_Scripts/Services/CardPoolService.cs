using Cards;
using UnityEngine;

/// <summary>
/// Adapter service that bridges ICardPoolService to CardPoolManager.
/// Phase 9: Temporary bridge during migration from singleton to DI.
/// Phase 19-20: Now injects CardPoolManager instead of using .Instance.
/// Phase 135: Added null checks for defensive programming.
/// </summary>
public class CardPoolService : ICardPoolService
{
    private readonly CardPoolManager _cardPoolManager;

    public CardPoolService(CardPoolManager cardPoolManager)
    {
        _cardPoolManager = cardPoolManager;
    }

    public Transform CardPoolHolder
    {
        get
        {
            // Phase 135: Add null check for defensive programming
            if (_cardPoolManager == null)
            {
                Debug.LogError("CardPoolService: CardPoolManager is null! Cannot get CardPoolHolder.");
                return null;
            }
            return _cardPoolManager.cardPoolHolder;
        }
    }

    public GameObject GetPooledObject(InGameCard inGameCard)
    {
        // Phase 135: Add null check for defensive programming
        if (_cardPoolManager == null)
        {
            Debug.LogError("CardPoolService: CardPoolManager is null! Cannot get pooled object.");
            return null;
        }
        return _cardPoolManager.GetPooledObject(inGameCard);
    }
}
