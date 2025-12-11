namespace JDG.Domain.Events
{
    /// <summary>
    /// Event published when a card is added to the selection (CardSelectionService).
    /// Part of Phase 28 - replacing UnityEvent with EventBus.
    /// Renamed from CardSelectedEvent to avoid conflict with GameEvents.cs CardSelectedEvent (which is for card clicks).
    /// </summary>
    public struct CardAddedToSelectionEvent
    {
        /// <summary>
        /// The card that was added to selection.
        /// Using object type to avoid dependency on legacy InGameCard.
        /// </summary>
        public object Card { get; set; }
    }

    /// <summary>
    /// Event published when a card is removed from the selection (CardSelectionService).
    /// Part of Phase 28 - replacing UnityEvent with EventBus.
    /// </summary>
    public struct CardRemovedFromSelectionEvent
    {
        /// <summary>
        /// The card that was removed from selection.
        /// Using object type to avoid dependency on legacy InGameCard.
        /// </summary>
        public object Card { get; set; }
    }

    /// <summary>
    /// Event published when the card selection set changes (any select/deselect operation).
    /// Part of Phase 28 - replacing UnityEvent with EventBus.
    /// </summary>
    public struct CardSelectionChangedEvent
    {
        /// <summary>
        /// The number of cards currently selected.
        /// </summary>
        public int SelectedCount { get; set; }
    }
}
