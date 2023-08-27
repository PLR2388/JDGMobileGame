using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : StaticInstance<MessageBox>, IMessageBoxBaseComponent
{

    [SerializeField] private GameObject prefab;
    
    private void AssignButtonAction(Button button, UnityAction action, GameObject objectToDestroy)
    {
        if(button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => 
            {
                action?.Invoke();
                Destroy(objectToDestroy);
            });
        }
    }
    
    private TextMeshProUGUI GetDescriptionText(GameObject parent)
    {
        var descriptionTextTransform = parent.transform.GetChild(0).Find("DescriptionText");
        return descriptionTextTransform.GetComponent<TextMeshProUGUI>();
    }

    private Button GetButton(GameObject parent, string childName)
    {
        var buttonTransform = parent.transform.GetChild(0).Find(childName);
        return buttonTransform.GetComponent<Button>();
    }

    public void SetNewValueGameObject(GameObject newGameObject, UIConfig config)
    {
        this.SetValueGameObject(newGameObject, config);
        var messageBoxConfig = config as MessageBoxConfig;
        var containerTransform = newGameObject.transform.GetChild(0).GetChild(0).Find("Container");
        var container = containerTransform.gameObject;
        container.SetActive(false);
        
        GetDescriptionText(newGameObject).text = messageBoxConfig?.Description;

        var okBtn = GetButton(newGameObject, "OkButton");
        var positiveBtn = GetButton(newGameObject, "PositiveButton");
        var negativeBtn = GetButton(newGameObject, "NegativeButton");
        AssignButtonAction(okBtn, messageBoxConfig?.OkAction, newGameObject);
        AssignButtonAction(positiveBtn, messageBoxConfig?.PositiveAction, newGameObject);
        AssignButtonAction(negativeBtn, messageBoxConfig?.NegativeAction, newGameObject);
    }

    public void CreateMessageBox(Transform canvas, MessageBoxConfig config)
    {
        var message = Instantiate(prefab);
        message.SetActive(true);
        message.transform.SetParent(canvas); // Must set parent after removing DDOL to avoid errors

        SetNewValueGameObject(message, config);
    }
}