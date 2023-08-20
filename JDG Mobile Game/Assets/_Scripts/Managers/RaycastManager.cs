using Cards;
using JetBrains.Annotations;
using UnityEngine;

public class RaycastManager : Singleton<RaycastManager>
{
    [CanBeNull]
    public static InGameCard GetCardTouch()
    {
        if (Camera.main == null) return null;
        var position = InputManager.TouchPosition;
        var hit = Physics.Raycast(Camera.main.ScreenPointToRay(position), out var hitInfo);
        return hit ? hitInfo.transform.gameObject.GetComponent<PhysicalCardDisplay>()?.card : null;
    }
}