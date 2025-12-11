namespace JDG.Domain.Events
{
    /// <summary>
    /// Event published when a card is selected.
    /// Part of Phase 28 - replacing UnityEvent with EventBus.
    /// </summary>
    public struct CardSelectedEvent
    {
        /// <summary>
        /// The card that was selected.
        /// Using object type to avoid dependency on legacy InGameCard.
        /// </summary>
        public object Card { get; set; }
    }

    /// <summary>
    /// Event published when a card is deselected.
    /// Part of Phase 28 - replacing UnityEvent with EventBus.
    /// </summary>
    public struct CardDeselectedEvent
    {
        /// <summary>
        /// The card that was deselected.
        /// Using object type to avoid dependency on legacy InGameCard.
        /// </summary>
        public object Card { get; set; }
    }

    /// <summary>
    /// Event published when the selection changes (any select/deselect operation).
    /// Part of Phase 28 - replacing UnityEvent with EventBus.
    /// </summary>
    public struct SelectionChangedEvent
    {
        /// <summary>
        /// The number of cards currently selected.
        /// </summary>
        public int SelectedCount { get; set; }
    }
}
