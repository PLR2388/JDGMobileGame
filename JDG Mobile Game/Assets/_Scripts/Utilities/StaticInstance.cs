using System;
using UnityEngine;

/// <summary>
/// A static instance is similar to a singleton, but instead of destroying any new
/// instances, it overrides the current instance. This is handy for resetting the state
/// and saves you doing it manually
/// </summary>
/// <remarks>
/// DEPRECATED: This pattern is being replaced by VContainer dependency injection.
/// New code should use constructor injection via VContainer LifetimeScopes instead.
/// See SharedServicesScope, GameSceneScope, or MainScreenScope for DI registration.
/// </remarks>
[Obsolete("Use VContainer dependency injection instead. Register services in SharedServicesScope/GameSceneScope.")]
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour {
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// This transforms the static instance into a basic singleton. This will destroy any new
/// versions created, leaving the original instance intact
/// </summary>
/// <remarks>
/// DEPRECATED: This pattern is being replaced by VContainer dependency injection.
/// </remarks>
[Obsolete("Use VContainer dependency injection instead. Register services in SharedServicesScope/GameSceneScope.")]
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour {
    protected override void Awake() {
        if (Instance != null) Destroy(gameObject);
        base.Awake();
    }
}

/// <summary>
/// Finally we have a persistent version of the singleton. This will survive through scene
/// loads. Perfect for system classes which require stateful, persistent data. Or audio sources
/// where music plays through loading screens, etc
/// </summary>
/// <remarks>
/// DEPRECATED: This pattern is being replaced by VContainer dependency injection.
/// Use SharedServicesScope with DontDestroyOnLoad for persistent services.
/// </remarks>
[Obsolete("Use VContainer dependency injection instead. Register in SharedServicesScope for persistence.")]
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour {
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}