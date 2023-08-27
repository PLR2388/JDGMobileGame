using System.Collections.Generic;
using Cards;
using UnityEngine.Events;

public class CardSelectorConfig
{
    public string Title { get; }
    public bool ShowOkButton { get; }
    public List<InGameCard> Cards { get; }
    public UnityAction<InGameCard> OkAction { get; }
    public UnityAction<List<InGameCard>> OkMultipleAction { get; }
    public bool ShowPositiveButton { get; }
    public UnityAction<InGameCard> PositiveAction { get; }
    public UnityAction<List<InGameCard>> PositiveMultipleAction { get; }
    public bool ShowNegativeButton { get; }
    public UnityAction NegativeAction { get; }
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
        bool displayOrder = false)
    {
        Title = title;
        Cards = cards;
        ShowOkButton = showOkButton;
        ShowPositiveButton = showPositiveButton;
        ShowNegativeButton = showNegativeButton;
        OkAction = okAction;
        OkMultipleAction = okMultipleAction;
        PositiveAction = positiveAction;
        PositiveMultipleAction = positiveMultipleAction;
        NegativeAction = negativeAction;
        NumberCardSelection = numberCardSelection;
        DisplayOrder = displayOrder;
    }
}