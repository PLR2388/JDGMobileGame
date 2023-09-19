using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardState
{
    protected OnHover context;
    protected InGameCard card;

    public CardState(OnHover context, InGameCard card)
    {
        this.context = context;
        this.card = card;
    }

    // This method gets called when a state is set as the current state
    public virtual void EnterState() { }

    // Handle click events for the card
    public virtual void OnClick() { }

    // ... Add other common methods or events that should be handled by states
}

public class DefaultCardState : CardState
{
    public DefaultCardState(OnHover context, InGameCard card) : base(context, card) { }

    public override void EnterState()
    {
        // Default behavior when entering this state
        context.GetComponent<Image>().color =  card?.Collector == true ? Color.yellow :  Color.white;
    }

    public override void OnClick()
    {
        // Handle click event for the default state
        // For example, if clicking selects the card, set the new state
        context.SetState(new SelectedCardState(context, card));
    }
}

public class SelectedCardState : CardState
{
    public SelectedCardState(OnHover context, InGameCard card) : base(context, card) { }

    public override void EnterState()
    {
        // Behavior when entering the selected state
        context.GetComponent<Image>().color = Color.green;
        CardSelectionManager.Instance.SelectCard(card);
    }

    public override void OnClick()
    {
        // Handle click event for the selected state
        // For example, if clicking deselects the card, set the default state
        context.SetState(new DefaultCardState(context, card));
        CardSelectionManager.Instance.UnselectCard(card);
    }
}

public class NumberCardState : CardState
{
    public NumberCardState(OnHover context, InGameCard card): base (context, card) {}
    
    public override void EnterState()
    {
        context.GetComponent<Image>().color = Color.green;
        context.DisplayNumber();
    }

    public override void OnClick()
    {
        context.SetState(new DefaultCardState(context, card));
        context.HideNumber();
        CardSelectionManager.Instance.UnselectCard(card);
    }
}
