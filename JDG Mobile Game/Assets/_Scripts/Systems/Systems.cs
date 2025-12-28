using System;

/// <summary>
/// Don't specifically need anything here other than the fact it's persistent.
/// I like to keep one main object which is never killed, with sub-systems as children.
/// Phase 59: Marked obsolete - VContainer/LifetimeScope handles persistence now.
/// </summary>
[Obsolete("Systems singleton is obsolete. VContainer's LifetimeScope with DontDestroyOnLoad handles persistence. See SharedServicesScope.")]
public class Systems : PersistentSingleton<Systems>
{

}
