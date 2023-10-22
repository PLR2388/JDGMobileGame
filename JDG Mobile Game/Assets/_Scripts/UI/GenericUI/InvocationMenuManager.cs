using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the invocation menu, including its buttons and their states.
/// </summary>
public class InvocationMenuManager : StaticInstance<InvocationMenuManager>
{
    [SerializeField] private GameObject invocationMenu;
     private Button attackButton;
     private Button actionButton;

     /// <summary>
     /// Initialize the manager, caching necessary components.
     /// </summary>
    protected override void Awake()
    {
        base.Awake();
        // Cache the components
        if (invocationMenu.transform.childCount > 0)
            attackButton = invocationMenu.transform.GetChild(0).GetComponent<Button>();
        if (invocationMenu.transform.childCount > 1)
            actionButton = invocationMenu.transform.GetChild(1).GetComponent<Button>();
    }

     /// <summary>
     /// Updates the state of the attack button based on whether the attacker can attack.
     /// </summary>
    public void UpdateAttackButton()
    {
        attackButton.interactable = CardManager.Instance.CanAttackerAttack();
    }

     /// <summary>
     /// Displays the invocation menu and sets button states based on game conditions.
     /// </summary>
     /// <param name="isAttackPhase">If set to <c>true</c>, indicates that the game is in the attack phase.</param>
    public void Display(bool isAttackPhase)
    {
        var mousePosition = InputManager.TouchPosition;
        invocationMenu.SetActive(true);

        attackButton.gameObject.SetActive(isAttackPhase);
        attackButton.interactable = CardManager.Instance.CanAttackerAttack();

        bool hasAttackerAction = CardManager.Instance.HasAttackerAction();
        actionButton.gameObject.SetActive(hasAttackerAction && !isAttackPhase);
        if (hasAttackerAction)
            actionButton.interactable = CardManager.Instance.IsSpecialActionPossible();

        invocationMenu.transform.position = mousePosition;
    }

     /// <summary>
     /// Hides the invocation menu.
     /// </summary>
    public void Hide()
    {
        invocationMenu.SetActive(false);
    }
     
     /// <summary>
     /// Enables the attack button.
     /// </summary>
    public void Enable()
    {
        attackButton.interactable = true;
    }
}