using JDG.Application.Services;

/// <summary>
/// Adapter service that bridges IInvocationMenuService to InvocationMenuManager.
/// Phase 9: Temporary bridge during migration from singleton to DI.
/// Phase 19-20: Now injects InvocationMenuManager instead of using .Instance.
/// </summary>
public class InvocationMenuService : IInvocationMenuService
{
    private readonly InvocationMenuManager _invocationMenuManager;

    public InvocationMenuService(InvocationMenuManager invocationMenuManager)
    {
        _invocationMenuManager = invocationMenuManager;
    }

    public void Display(bool isAttackPhase)
    {
        _invocationMenuManager.Display(isAttackPhase);
    }

    public void Hide()
    {
        _invocationMenuManager.Hide();
    }

    public void Enable()
    {
        _invocationMenuManager.Enable();
    }

    public void UpdateAttackButton()
    {
        _invocationMenuManager.UpdateAttackButton();
    }
}
