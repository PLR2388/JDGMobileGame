using UnityEngine;
using UnityEngine.UI;

public class InvocationMenuManager : StaticInstance<InvocationMenuManager>
{
    [SerializeField] private GameObject invocationMenu;
     private Button attackButton;
     private Button actionButton;

    protected override void Awake()
    {
        base.Awake();
        // Cache the components
        if (invocationMenu.transform.childCount > 0)
            attackButton = invocationMenu.transform.GetChild(0).GetComponent<Button>();
        if (invocationMenu.transform.childCount > 1)
            actionButton = invocationMenu.transform.GetChild(1).GetComponent<Button>();
    }

    public void UpdateAttackButton()
    {
        attackButton.interactable = CardManager.Instance.CanAttackerAttack();
    }

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

    public void Hide()
    {
        invocationMenu.SetActive(false);
    }

    public void Enable()
    {
        attackButton.interactable = true;
    }
}