using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class NumberedCardEvent : UnityEvent<InGameCard, int>
{
}

public class MessageBox : StaticInstance<MessageBox>
{
    
    [SerializeField] private GameObject prefab;

    private void SetValueGameObject(GameObject newGameObject, MessageBoxConfig config)
    {
        var titleTextTransform = newGameObject.transform.GetChild(0).Find("TitleText");
        var descriptionTextTransform = newGameObject.transform.GetChild(0).Find("DescriptionText");
        var positiveButtonTransform = newGameObject.transform.GetChild(0).Find("PositiveButton");
        var negativeButtonTransform = newGameObject.transform.GetChild(0).Find("NegativeButton");
        var okButtonTransfrom = newGameObject.transform.GetChild(0).Find("OkButton");
        var containerTransform = newGameObject.transform.GetChild(0).GetChild(0).Find("Container");

        var titleText = titleTextTransform.GetComponent<TextMeshProUGUI>();
        var descriptionText = descriptionTextTransform.GetComponent<TextMeshProUGUI>();
        var positiveButtonText = positiveButtonTransform.GetComponentInChildren<TextMeshProUGUI>();
        var negativeButtonText = negativeButtonTransform.GetComponentInChildren<TextMeshProUGUI>();
        var okButtonText = okButtonTransfrom.GetComponentInChildren<TextMeshProUGUI>();
        var positiveButton = positiveButtonTransform.gameObject;
        var negativeButton = negativeButtonTransform.gameObject;
        var okButton = okButtonTransfrom.gameObject;
        var container = containerTransform.gameObject;
        container.SetActive(false);
        positiveButtonText.text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_YES);
        negativeButtonText.text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_NO);
        okButtonText.text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_OK);
        
        
       
        titleText.text = config.Title;
        descriptionText.text = config.Description;

        positiveButton.SetActive(config.ShowPositiveButton);
        okButton.SetActive(config.ShowOkButton);
        negativeButton.SetActive(config.ShowNegativeButton);

        var okBtn = okButton.GetComponent<Button>();
        okBtn.onClick.RemoveAllListeners();
        okBtn.onClick.AddListener(() =>
        {
            config.OkAction?.Invoke();
            Destroy(newGameObject);
        });

        var positiveBtn = positiveButton.GetComponent<Button>();
        positiveBtn.onClick.RemoveAllListeners();
        positiveBtn.onClick.AddListener(() =>
        {
            config.PositiveAction?.Invoke();
            Destroy(newGameObject);
        });

        var negativeBtn = negativeButton.GetComponent<Button>();
        negativeBtn.onClick.RemoveAllListeners();
        negativeBtn.onClick.AddListener(() =>
        {
            config.NegativeAction?.Invoke();
            Destroy(newGameObject);
        });
    }

    public void CreateMessageBox(Transform canvas, MessageBoxConfig config)
    {
        var message = Instantiate(prefab);
        message.SetActive(true);
        message.transform.SetParent(canvas); // Must set parent after removing DDOL to avoid errors
        
        SetValueGameObject(message, config);
    }
}