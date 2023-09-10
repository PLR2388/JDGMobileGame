using Cards;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Manages the raycasting functionality to detect card interactions in the game.
/// </summary>
public class CardRaycastManager : Singleton<CardRaycastManager>
{
    private Camera mainCamera;

    /// <summary>
    /// Initializes the CardRaycastManager on scene start.
    /// Ensures the presence of the main camera in the scene.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            throw new System.Exception("Main camera is not set in the scene.");
        }
    }
    
    /// <summary>
    /// Retrieves the InGameCard under the user's touch or click.
    /// </summary>
    /// <returns>The InGameCard being touched or null if no card is detected.</returns>
    [CanBeNull]
    public InGameCard GetTouchedCard()
    {
        var touchedObject = RaycastUnderTouch();
        return touchedObject?.GetComponent<PhysicalCardDisplay>()?.card;
    }

    /// <summary>
    /// Performs a raycast to detect objects under the user's touch or click.
    /// </summary>
    /// <returns>The transform of the object being touched or null if no object is detected.</returns>
    private Transform RaycastUnderTouch()
    {
        var position = InputManager.TouchPosition;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(position), out var hitInfo))
        {
            return hitInfo.transform;
        }
        return null;
    }
}