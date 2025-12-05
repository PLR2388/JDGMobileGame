using Cards;
using UnityEngine;

/// <summary>
/// Represents an abstract card state. Contains common functionality and provides the interface for concrete card states.
/// Phase 9: Removed CardSelectionManager singleton dependency via DI.
/// </summary>
public abstract class CardState
{
    protected OnHover context;
    protected InGameCard card;
    protected ICardSelectionService cardSelectionService;

    /// <summary>
    /// Initializes a new instance of the CardState class.
    /// </summary>
    /// <param name="context">Reference to the OnHover script.</param>
    /// <param name="card">Reference to the InGameCard that is associated with the state.</param>
    /// <param name="cardSelectionService">Card selection service for managing card selection.</param>
    public CardState(OnHover context, InGameCard card, ICardSelectionService cardSelectionService)
    {
        this.context = context;
        this.card = card;
        this.cardSelectionService = cardSelectionService;
    }

    /// <summary>
    /// Logic to be executed when a card enters a particular state.
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// Logic to be executed when a card is clicked.
    /// </summary>
    public abstract void OnClick();
}

/// <summary>
/// Represents the default state of a card.
/// </summary>
public class DefaultCardState : CardState
{
    public DefaultCardState(OnHover context, InGameCard card, ICardSelectionService cardSelectionService)
        : base(context, card, cardSelectionService)
    {
    }

    public override void EnterState()
    {
        context.SetImageColor(card?.Collector == true ? Color.yellow : Color.white);
    }

    public override void OnClick()
    {
        context.SetState(new SelectedCardState(context, card, cardSelectionService));
    }
}

/// <summary>
/// Represents the selected state of a card.
/// </summary>
public class SelectedCardState : CardState
{
    public SelectedCardState(OnHover context, InGameCard card, ICardSelectionService cardSelectionService)
        : base(context, card, cardSelectionService)
    {
    }

    public override void EnterState()
    {
        context.SetImageColor(Color.green);
        // Phase 9: Use injected service instead of CardSelectionManager.Instance
        cardSelectionService?.SelectCard(card);
    }

    public override void OnClick()
    {
        context.SetState(new DefaultCardState(context, card, cardSelectionService));
        // Phase 9: Use injected service instead of CardSelectionManager.Instance
        cardSelectionService?.UnselectCard(card);
    }
}

/// <summary>
/// Represents the state when a number is displayed on the card.
/// </summary>
public class NumberCardState : CardState
{
    public NumberCardState(OnHover context, InGameCard card, ICardSelectionService cardSelectionService)
        : base(context, card, cardSelectionService)
    {
    }

    public override void EnterState()
    {
        context.SetImageColor(Color.green);
        context.DisplayNumber();
    }

    public override void OnClick()
    {
        context.SetState(new DefaultCardState(context, card, cardSelectionService));
        context.HideNumber();
        // Phase 9: Use injected service instead of CardSelectionManager.Instance
        cardSelectionService?.UnselectCard(card);
    }
}
