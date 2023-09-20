using Cards;

public abstract class CardHandler
{
    protected InGameMenuScript menuScript;  // Reference to the main script for callbacks or other operations.

    public CardHandler(InGameMenuScript menuScript)
    {
        this.menuScript = menuScript;
    }

    public abstract void HandleCard(InGameCard card);

    public abstract void HandleCardPut(InGameCard card);
}