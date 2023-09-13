using TMPro;
using UnityEngine;

/// <summary>
/// Represents the core functionalities required for a message box component.
/// </summary>
public interface IMessageBoxBaseComponent
{
    /// <summary>
    /// Sets the values of UI elements for a given game object based on the provided UI configuration.
    /// </summary>
    /// <param name="newGameObject">The game object whose UI values need to be set.</param>
    /// <param name="config">Configuration details for the UI.</param>
    void SetNewValueGameObject(GameObject newGameObject, UIConfig config);
}

/// <summary>
/// Constants related to the message box base component's UI elements.
/// </summary>
static class MessageBoxBaseComponentConstants
{
    public const string TitleText = "TitleText";
    public const string PositiveButton = "PositiveButton";
    public const string NegativeButton = "NegativeButton";
    public const string OkButton = "OkButton";
}

/// <summary>
/// Extension methods for the IMessageBoxBaseComponent interface.
/// </summary>
public static class MessageBoxBaseComponentExtensions
{
    /// <summary>
    /// Finds a child transform within a parent game object based on the child's name.
    /// </summary>
    /// <param name="parent">The parent game object.</param>
    /// <param name="childName">The name of the child transform to find.</param>
    /// <returns>The found child transform.</returns>
    private static Transform FindChildTransform(GameObject parent, string childName)
    {
        return parent.transform.GetChild(0).Find(childName);
    }

    /// <summary>
    /// Sets UI element values such as texts and visibility for a given game object based on the provided UI configuration.
    /// </summary>
    /// <param name="baseComponent">The instance of IMessageBoxBaseComponent.</param>
    /// <param name="newGameObject">The game object whose UI values need to be set.</param>
    /// <param name="config">Configuration details for the UI.</param>
    public static void SetValueGameObject(this IMessageBoxBaseComponent baseComponent, GameObject newGameObject, UIConfig config)
    {
        SetTitleText(newGameObject, config.Title);
        ConfigureButton(newGameObject, MessageBoxBaseComponentConstants.PositiveButton, config.ShowPositiveButton, LocalizationKeys.BUTTON_YES);
        ConfigureButton(newGameObject, MessageBoxBaseComponentConstants.OkButton, config.ShowOkButton, LocalizationKeys.BUTTON_OK);
        ConfigureButton(newGameObject, MessageBoxBaseComponentConstants.NegativeButton, config.ShowNegativeButton, LocalizationKeys.BUTTON_NO);
    }
    
    /// <summary>
    /// Sets the title text for a UI element within the provided game object.
    /// </summary>
    /// <param name="parent">The game object containing the title text UI element.</param>
    /// <param name="title">The title text to set.</param>
    private static void SetTitleText(GameObject parent, string title)
    {
        var titleTextTransform = FindChildTransform(parent, MessageBoxBaseComponentConstants.TitleText);
        var titleText = titleTextTransform.GetComponent<TextMeshProUGUI>();
        titleText.text = title;
    }
    
    /// <summary>
    /// Configures a button's text and visibility within the provided game object based on specified parameters.
    /// </summary>
    /// <param name="parent">The game object containing the button.</param>
    /// <param name="buttonName">The name of the button within the game object.</param>
    /// <param name="setActive">Determines whether the button should be visible or not.</param>
    /// <param name="localizationKey">The localization key for the button's text.</param>
    private static void ConfigureButton(GameObject parent, string buttonName, bool setActive, LocalizationKeys localizationKey)
    {
        var buttonTransform = FindChildTransform(parent, buttonName);
        var buttonText = buttonTransform.GetComponentInChildren<TextMeshProUGUI>();
        var button = buttonTransform.gameObject;
        buttonText.text = LocalizationSystem.Instance.GetLocalizedValue(localizationKey);
        button.SetActive(setActive);
    }
}