using Cards;

public class ContreCardHandler : CardHandler
{
    public ContreCardHandler(InGameMenuScript menuScript) : base(menuScript)
    {
    }

    public override void HandleCard(InGameCard card)
    {
        menuScript.putCardButtonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_CONTRE));
        menuScript.putCardButton.interactable = true;
    }

    public override void HandleCardPut(InGameCard card)
    {
        throw new System.NotImplementedException();
    }
}