using TMPro;
using UnityEngine;

public interface IMessageBoxBaseComponent
{
    void SetNewValueGameObject(GameObject newGameObject, UIConfig config);
}

public static class MessageBoxBaseComponentExtensions
{
    private static Transform FindChildTransform(GameObject parent, string childName)
    {
        return parent.transform.GetChild(0).Find(childName);
    }

    public static void SetValueGameObject(this IMessageBoxBaseComponent baseComponent, GameObject newGameObject, UIConfig config)
    {
        var titleTextTransform = FindChildTransform(newGameObject, "TitleText");
        var positiveButtonTransform = FindChildTransform(newGameObject, "PositiveButton");
        var negativeButtonTransform = FindChildTransform(newGameObject, "NegativeButton");
        var okButtonTransfrom = FindChildTransform(newGameObject, "OkButton");

        var titleText = titleTextTransform.GetComponent<TextMeshProUGUI>();
        var positiveButtonText = positiveButtonTransform.GetComponentInChildren<TextMeshProUGUI>();
        var negativeButtonText = negativeButtonTransform.GetComponentInChildren<TextMeshProUGUI>();
        var okButtonText = okButtonTransfrom.GetComponentInChildren<TextMeshProUGUI>();
        var positiveButton = positiveButtonTransform.gameObject;
        var negativeButton = negativeButtonTransform.gameObject;

        var okButton = okButtonTransfrom.gameObject;

        positiveButtonText.text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_YES);
        negativeButtonText.text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_NO);
        okButtonText.text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_OK);
        titleText.text = config.Title;

        positiveButton.SetActive(config.ShowPositiveButton);
        okButton.SetActive(config.ShowOkButton);
        negativeButton.SetActive(config.ShowNegativeButton);
    }
}