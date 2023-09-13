using System.Collections.Generic;
using Cards;
using UnityEngine.Events;

/// <summary>
/// Represents a generic UI configuration.
/// </summary>
public abstract class UIConfig
{
    /// <summary>
    /// Gets the title of the UI.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Indicates whether to display a negative button.
    /// </summary
    public bool ShowNegativeButton { get; }

    /// <summary>
    /// Indicates whether to display a positive button.
    /// </summary>
    public bool ShowPositiveButton { get; }

    /// <summary>
    /// Indicates whether to display an OK button.
    /// </summary>
    public bool ShowOkButton { get; }

    /// <summary>
    /// Gets the action to be executed on pressing the negative button.
    /// </summary>
    public UnityAction NegativeAction { get; }

    protected UIConfig(string title, bool showNegativeButton, bool showPositiveButton, bool showOkButton, UnityAction negativeAction)
    {
        Title = title;
        ShowNegativeButton = showNegativeButton;
        ShowPositiveButton = showPositiveButton;
        ShowOkButton = showOkButton;
        NegativeAction = negativeAction;
    }
}

/// <summary>
/// Represents a configuration for a message box UI.
/// </summary>
public class MessageBoxConfig : UIConfig
{
    /// <summary>
    /// Gets the description text for the message box.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the action to be executed on pressing the OK button.
    /// </summary>
    public UnityAction OkAction { get; }

    /// <summary>
    /// Gets the action to be executed on pressing the positive button.
    /// </summary>
    public UnityAction PositiveAction { get; }

    public MessageBoxConfig(string title, string description = null, bool showOkButton = false, UnityAction okAction = null, bool showPositiveButton = false,
        UnityAction positiveAction = null, bool showNegativeButton = false,
        UnityAction negativeAction = null) : base(title, showNegativeButton, showPositiveButton, showOkButton, negativeAction)
    {
        Description = description;
        OkAction = okAction;
        PositiveAction = positiveAction;
    }
}

/// <summary>
/// Contains actions related to card selections.
/// </summary>
public struct CardActions
{
    /// <summary>
    /// Gets or sets the action for a single card selection.
    /// </summary>
    public UnityAction<InGameCard> SingleAction { get; }


    /// <summary>
    /// Gets or sets the action for multiple card selections.
    /// </summary>
    public UnityAction<List<InGameCard>> MultipleAction { get; }

    public CardActions(UnityAction<InGameCard> singleAction, UnityAction<List<InGameCard>> multipleAction)
    {
        SingleAction = singleAction;
        MultipleAction = multipleAction;
    }
}

/// <summary>
/// Represents a configuration for a card selector UI.
/// </summary>
public class CardSelectorConfig : UIConfig
{
    /// <summary>
    /// Gets the list of in-game cards for selection.
    /// </summary>
    public List<InGameCard> Cards { get; }

    /// <summary>
    /// Gets or sets the actions related to OK button selections.
    /// </summary>
    public CardActions OkActions { get; }

    /// <summary>
    /// Gets or sets the actions related to positive button selections.
    /// </summary>
    public CardActions PositiveActions { get; }

    /// <summary>
    /// Gets the number of cards to be selected.
    /// </summary>
    public int NumberCardSelection { get; }
    
    /// <summary>
    /// Indicates whether to display the order of card selections.
    /// </summary>
    public bool ShowOrder { get; }

    public CardSelectorConfig(
        string title,
        List<InGameCard> cards,
        bool showOkButton = false,
        bool showPositiveButton = false,
        bool showNegativeButton = false,
        UnityAction<InGameCard> okAction = null,
        UnityAction<List<InGameCard>> okMultipleAction = null,
        UnityAction<InGameCard> positiveAction = null,
        UnityAction<List<InGameCard>> positiveMultipleAction = null,
        UnityAction negativeAction = null,
        int numberCardSelection = 1,
        bool showOrder = false) : base(title, showNegativeButton, showPositiveButton, showOkButton, negativeAction)
    {
        Cards = cards;
        OkActions = new CardActions(okAction, okMultipleAction);
        PositiveActions = new CardActions(positiveAction, positiveMultipleAction);
        NumberCardSelection = numberCardSelection >= 1 ? numberCardSelection : 1;
        ShowOrder = showOrder;
    }
}