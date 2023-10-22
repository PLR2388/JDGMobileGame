using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : StaticInstance<MessageBox>, IMessageBoxBaseComponent
{

    [SerializeField] private GameObject prefab;

    private const string OkButton = "OkButton";
    private const string DescriptionText = "DescriptionText";
    private const string Container = "Container";
    private const string PositiveButton = "PositiveButton";
    private const string NegativeButton = "NegativeButton";

    /// <summary>
    /// Assigns the specified action to the button and destroys the associated object upon button click.
    /// </summary>
    /// <param name="button">The button to assign the action to.</param>
    /// <param name="action">The action to be performed on button click.</param>
    /// <param name="objectToDestroy">The object to be destroyed on button click.</param>
    private void AssignButtonAction(Button button, UnityAction action, GameObject objectToDestroy)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                action?.Invoke();
                Destroy(objectToDestroy);
            });
        }
    }

    /// <summary>
    /// Retrieves the description text component from a given parent object.
    /// </summary>
    /// <param name="parent">The parent GameObject.</param>
    /// <returns>The TextMeshProUGUI component representing the description text.</returns>
    private TextMeshProUGUI GetDescriptionText(GameObject parent)
    {
        var descriptionTextTransform = parent.transform.GetChild(0).Find(DescriptionText);
        return descriptionTextTransform.GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Retrieves a button component from a given parent object using the specified child name.
    /// </summary>
    /// <param name="parent">The parent GameObject.</param>
    /// <param name="childName">The name of the child GameObject containing the button.</param>
    /// <returns>The Button component.</returns>
    private Button GetButton(GameObject parent, string childName)
    {
        var buttonTransform = parent.transform.GetChild(0).Find(childName);
        return buttonTransform.GetComponent<Button>();
    }

    /// <summary>
    /// Configures the specified GameObject with the given UI configuration.
    /// </summary>
    /// <param name="newGameObject">The GameObject to be configured.</param>
    /// <param name="config">The UI configuration to apply.</param>
    public void SetNewValueGameObject(GameObject newGameObject, UIConfig config)
    {
        this.SetValueGameObject(newGameObject, config);
        var messageBoxConfig = config as MessageBoxConfig;
        var containerTransform = newGameObject.transform.GetChild(0).GetChild(0).Find(Container);
        var container = containerTransform.gameObject;
        container.SetActive(false);

        GetDescriptionText(newGameObject).text = messageBoxConfig?.Description;

        var okBtn = GetButton(newGameObject, OkButton);
        var positiveBtn = GetButton(newGameObject, PositiveButton);
        var negativeBtn = GetButton(newGameObject, NegativeButton);
        AssignButtonAction(okBtn, messageBoxConfig?.OkAction, newGameObject);
        AssignButtonAction(positiveBtn, messageBoxConfig?.PositiveAction, newGameObject);
        AssignButtonAction(negativeBtn, messageBoxConfig?.NegativeAction, newGameObject);
    }

    /// <summary>
    /// Creates and configures a new message box within the specified canvas using the given configuration.
    /// </summary>
    /// <param name="canvas">The parent canvas for the message box.</param>
    /// <param name="config">The configuration to apply to the message box.</param>
    public void CreateMessageBox(Transform canvas, MessageBoxConfig config)
    {
        var message = Instantiate(prefab);
        message.SetActive(true);
        message.transform.SetParent(canvas); // Must set parent after removing DDOL to avoid errors

        SetNewValueGameObject(message, config);
    }
}