using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Infrastructure.Cards;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Implementation of IRaycastService.
/// Performs raycasting to detect card interactions using Unity's physics system.
///
/// Note: This service is in the default assembly because it depends on legacy types
/// (InGameCard, PhysicalCardDisplay). It will be moved to JDG.Infrastructure once
/// legacy card types are migrated to the Domain layer.
///
/// Part of Phase 1 migration - replaces CardRaycastManager singleton with DI.
/// </summary>
public class RaycastService : IRaycastService
{
    private readonly IInputService _inputService;
    private readonly IEventBus _eventBus;
    private readonly Camera _mainCamera;

    public RaycastService(IInputService inputService, IEventBus eventBus)
    {
        _inputService = inputService;
        _eventBus = eventBus;
        _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("RaycastService: Main camera is not set in the scene.");
        }
    }

    /// <summary>
    /// Retrieves the InGameCard under the user's current touch or click position.
    /// </summary>
    /// <returns>The InGameCard being touched or null if no card is detected.</returns>
    [CanBeNull]
    public InGameCard GetTouchedCard()
    {
        if (_mainCamera == null)
        {
            Debug.LogWarning("RaycastService: Cannot raycast without a main camera.");
            return null;
        }

        var touchedObject = RaycastUnderTouch();
        return touchedObject?.GetComponent<PhysicalCardDisplay>()?.Card;
    }

    /// <summary>
    /// Performs a raycast to detect objects under the user's current touch or click.
    /// </summary>
    /// <returns>The transform of the object being touched or null if no object is detected.</returns>
    [CanBeNull]
    private Transform RaycastUnderTouch()
    {
        var domainPosition = _inputService.GetCurrentTouchPosition();
        if (domainPosition == null)
        {
            return null;
        }

        // Convert domain Vector2 to Unity Vector3
        var unityPosition = new Vector3(domainPosition.Value.X, domainPosition.Value.Y, 0);

        if (Physics.Raycast(_mainCamera.ScreenPointToRay(unityPosition), out var hitInfo))
        {
            return hitInfo.transform;
        }

        return null;
    }
}
