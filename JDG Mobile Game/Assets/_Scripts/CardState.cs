using Cards;
using UnityEngine;

/// <summary>
/// Represents an abstract card state. Contains common functionality and provides the interface for concrete card states.
/// </summary>
public abstract class CardState
{
    protected OnHover context;
    protected InGameCard card;

    /// <summary>
    /// Initializes a new instance of the CardState class.
    /// </summary>
    /// <param name="context">Reference to the OnHover script.</param>
    /// <param name="card">Reference to the InGameCard that is associated with the state.</param>
    public CardState(OnHover context, InGameCard card)
    {
        this.context = context;
        this.card = card;
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
    public DefaultCardState(OnHover context, InGameCard card) : base(context, card)
    {
    }

    public override void EnterState()
    {
        context.SetImageColor(card?.Collector == true ? Color.yellow : Color.white);
    }

    public override void OnClick()
    {
        context.SetState(new SelectedCardState(context, card));
    }
}

/// <summary>
/// Represents the selected state of a card.
/// </summary>
public class SelectedCardState : CardState
{
    public SelectedCardState(OnHover context, InGameCard card) : base(context, card)
    {
    }

    public override void EnterState()
    {
        context.SetImageColor(Color.green);
        CardSelectionManager.Instance.SelectCard(card);
    }

    public override void OnClick()
    {
        context.SetState(new DefaultCardState(context, card));
        CardSelectionManager.Instance.UnselectCard(card);
    }
}

/// <summary>
/// Represents the state when a number is displayed on the card.
/// </summary>
public class NumberCardState : CardState
{
    public NumberCardState(OnHover context, InGameCard card) : base(context, card)
    {
    }

    public override void EnterState()
    {
        context.SetImageColor(Color.green);
        context.DisplayNumber();
    }

    public override void OnClick()
    {
        context.SetState(new DefaultCardState(context, card));
        context.HideNumber();
        CardSelectionManager.Instance.UnselectCard(card);
    }
}
