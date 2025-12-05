using UnityEngine;

/// <summary>
/// Implementation of ICardPoolService.
/// Bridges to legacy CardPoolManager singleton during migration.
///
/// Part of Phase 9 - Removes singleton dependencies from UI components.
/// This service will be refactored once CardPoolManager lifecycle is managed by DI.
/// </summary>
public class CardPoolService : ICardPoolService
{
    public Transform CardPoolHolder => CardPoolManager.Instance?.cardPoolHolder;

    public GameObject GetPooledObject(InGameCard inGameCard)
    {
        return CardPoolManager.Instance?.GetPooledObject(inGameCard);
    }
}
