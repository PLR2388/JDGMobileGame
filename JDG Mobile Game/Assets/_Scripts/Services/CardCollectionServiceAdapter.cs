using Cards;

/// <summary>
/// Adapter that bridges ICardCollectionService to CardManager singleton.
/// Part of Phase 6 - temporary adapter during migration.
///
/// This adapter allows CardPlacementService to be dependency-injected while
/// still using CardManager singleton internally. Once CardManager is fully
/// decomposed, this adapter can be replaced with direct CardCollectionService registration.
///
/// Note: This is a temporary bridge pattern, similar to AudioService wrapping AudioSystem.
/// </summary>
public class CardCollectionServiceAdapter : ICardCollectionService
{
    public PlayerCards GetCurrentPlayerCards()
    {
        return CardManager.Instance.GetCurrentPlayerCards();
    }

    public PlayerCards GetOpponentPlayerCards()
    {
        return CardManager.Instance.GetOpponentPlayerCards();
    }
}
