using JDG.Application.Services;

/// <summary>
/// Adapter service that bridges IInvocationMenuService to InvocationMenuManager singleton.
/// Phase 9: Temporary bridge during migration from singleton to DI.
/// Will be removed once InvocationMenuManager is refactored.
/// </summary>
public class InvocationMenuService : IInvocationMenuService
{
    public void Display(bool isAttackPhase)
    {
        InvocationMenuManager.Instance.Display(isAttackPhase);
    }

    public void Hide()
    {
        InvocationMenuManager.Instance.Hide();
    }

    public void Enable()
    {
        InvocationMenuManager.Instance.Enable();
    }

    public void UpdateAttackButton()
    {
        InvocationMenuManager.Instance.UpdateAttackButton();
    }
}
