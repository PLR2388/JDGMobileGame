using UnityEngine.Events;

public class MessageBoxConfig
{
    public string Title { get; }
    public string Description { get; }
    public bool ShowOkButton { get; }
    public UnityAction OkAction { get; }
    public bool ShowPositiveButton { get; }
    public UnityAction PositiveAction { get; }
    public bool ShowNegativeButton { get; }
    public UnityAction NegativeAction { get; }

    public MessageBoxConfig(string title, string description = null, bool showOkButton = false, UnityAction okAction = null, bool showPositiveButton = false,
        UnityAction positiveAction = null, bool showNegativeButton = false,
        UnityAction negativeAction = null)
    {
        Title = title;
        Description = description;
        ShowOkButton = showOkButton;
        OkAction = okAction;
        ShowPositiveButton = showPositiveButton;
        PositiveAction = positiveAction;
        ShowNegativeButton = showNegativeButton;
        NegativeAction = negativeAction;
    }
}