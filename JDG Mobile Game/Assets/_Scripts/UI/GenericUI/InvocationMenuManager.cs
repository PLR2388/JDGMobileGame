using UnityEngine;
using UnityEngine.UI;
using VContainer;
using JDG.Presentation.Views;
using JDG.Presentation.Presenters;

/// <summary>
/// View implementation for invocation context menu.
/// Phase 17-18: Removed CardManager singleton dependency via ICombatService.
/// Phase 19-20: Converted from singleton to regular MonoBehaviour with VContainer registration.
/// Phase 28: Migrated to MVP pattern - now implements IInvocationMenuView, business logic moved to InvocationMenuPresenter.
/// </summary>
public class InvocationMenuManager : MonoBehaviour, IInvocationMenuView
{
    [SerializeField] private GameObject invocationMenu;
    private Button attackButton;
    private Button actionButton;

    // Phase 28: Presenter handles business logic
    private InvocationMenuPresenter _presenter;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 28: Injects services needed to create the presenter.
    /// </summary>
    [Inject]
    public void Construct(ICombatService combatService)
    {
        // Phase 28: Create presenter with this view and injected services
        _presenter = new InvocationMenuPresenter(this, combatService);
    }

    /// <summary>
    /// Initialize the view, caching necessary components.
    /// Phase 19-20: No longer calls base.Awake() since not a singleton.
    /// </summary>
    private void Awake()
    {
        // Cache the button components
        if (invocationMenu.transform.childCount > 0)
            attackButton = invocationMenu.transform.GetChild(0).GetComponent<Button>();
        if (invocationMenu.transform.childCount > 1)
            actionButton = invocationMenu.transform.GetChild(1).GetComponent<Button>();
    }

    #region IInvocationMenuView Implementation

    /// <summary>
    /// Shows the invocation menu at the specified screen position.
    /// Phase 28: Pure view operation, no business logic.
    /// </summary>
    public void ShowMenu(Vector3 screenPosition)
    {
        if (invocationMenu != null)
        {
            invocationMenu.SetActive(true);
            invocationMenu.transform.position = screenPosition;
        }
    }

    /// <summary>
    /// Hides the invocation menu.
    /// Phase 28: Pure view operation, no business logic.
    /// </summary>
    public void HideMenu()
    {
        if (invocationMenu != null)
        {
            invocationMenu.SetActive(false);
        }
    }

    /// <summary>
    /// Sets the attack button's visibility and interactability.
    /// Phase 28: Pure view operation, no business logic.
    /// </summary>
    public void SetAttackButtonState(bool visible, bool interactable)
    {
        if (attackButton != null)
        {
            attackButton.gameObject.SetActive(visible);
            attackButton.interactable = interactable;
        }
    }

    /// <summary>
    /// Sets the action button's visibility and interactability.
    /// Phase 28: Pure view operation, no business logic.
    /// </summary>
    public void SetActionButtonState(bool visible, bool interactable)
    {
        if (actionButton != null)
        {
            actionButton.gameObject.SetActive(visible);
            actionButton.interactable = interactable;
        }
    }

    /// <summary>
    /// Enables the attack button.
    /// Phase 28: Pure view operation, no business logic.
    /// </summary>
    public void EnableAttackButton()
    {
        if (attackButton != null)
        {
            attackButton.interactable = true;
        }
    }

    #endregion

    #region Legacy Public Methods (for backward compatibility)

    /// <summary>
    /// Updates the state of the attack button based on whether the attacker can attack.
    /// Phase 28: Delegates to presenter for business logic.
    /// DEPRECATED: Direct calls to this method should eventually use presenter directly.
    /// </summary>
    public void UpdateAttackButton()
    {
        if (_presenter != null)
        {
            _presenter.UpdateAttackButton();
        }
    }

    /// <summary>
    /// Displays the invocation menu and sets button states based on game conditions.
    /// Phase 28: Delegates to presenter for business logic.
    /// DEPRECATED: Direct calls to this method should eventually use presenter directly.
    /// </summary>
    /// <param name="isAttackPhase">If set to <c>true</c>, indicates that the game is in the attack phase.</param>
    public void Display(bool isAttackPhase)
    {
        if (_presenter != null)
        {
            var mousePosition = InputManager.TouchPosition;
            _presenter.Display(mousePosition, isAttackPhase);
        }
    }

    /// <summary>
    /// Hides the invocation menu.
    /// Phase 28: Delegates to presenter for business logic.
    /// DEPRECATED: Direct calls to this method should eventually use presenter directly.
    /// </summary>
    public void Hide()
    {
        if (_presenter != null)
        {
            _presenter.Hide();
        }
    }

    /// <summary>
    /// Enables the attack button.
    /// Phase 28: Delegates to presenter for business logic.
    /// DEPRECATED: Direct calls to this method should eventually use presenter directly.
    /// </summary>
    public void Enable()
    {
        if (_presenter != null)
        {
            _presenter.EnableAttackButton();
        }
    }

    #endregion
}