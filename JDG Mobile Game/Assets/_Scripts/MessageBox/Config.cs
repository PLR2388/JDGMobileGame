using System.Collections.Generic;
using Cards;
using UnityEngine.Events;

public abstract class UIConfig
{
    public string Title { get; }
    public bool ShowNegativeButton { get; }
    public bool ShowPositiveButton { get; }
    public bool ShowOkButton { get; }
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

public class MessageBoxConfig : UIConfig
{
    public string Description { get; }
    public UnityAction OkAction { get; }
    public UnityAction PositiveAction { get; }

    public MessageBoxConfig(string title, string description = null, bool showOkButton = false, UnityAction okAction = null, bool showPositiveButton = false,
        UnityAction positiveAction = null, bool showNegativeButton = false,
        UnityAction negativeAction = null): base(title, showNegativeButton, showPositiveButton, showOkButton, negativeAction)
    {
        Description = description;
        OkAction = okAction;
        PositiveAction = positiveAction;
    }
}

public class CardSelectorConfig : UIConfig
{
    public List<InGameCard> Cards { get; }
    public UnityAction<InGameCard> OkAction { get; }
    public UnityAction<List<InGameCard>> OkMultipleAction { get; }
 
    public UnityAction<InGameCard> PositiveAction { get; }
    public UnityAction<List<InGameCard>> PositiveMultipleAction { get; }
    
    public int NumberCardSelection { get; }
    public bool DisplayOrder { get; }
    
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
        bool displayOrder = false): base(title, showNegativeButton, showPositiveButton, showOkButton, negativeAction)
    {
        Cards = cards;
        OkAction = okAction;
        OkMultipleAction = okMultipleAction;
        PositiveAction = positiveAction;
        PositiveMultipleAction = positiveMultipleAction;
        NumberCardSelection = numberCardSelection;
        DisplayOrder = displayOrder;
    }
}
