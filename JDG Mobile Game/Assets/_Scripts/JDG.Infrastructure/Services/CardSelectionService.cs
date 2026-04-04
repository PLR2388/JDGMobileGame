using System.Collections.Generic;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for managing card selection state.
    /// Part of Phase 28 - MonoBehaviour Wave 1 service extraction.
    /// Pure C# service with no Unity dependencies.
    /// </summary>
    public class CardSelectionService : ICardSelectionService
    {
        private readonly IEventBus _eventBus;
        private readonly List<object> _selectedCards = new List<object>();

        public CardSelectionService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        /// <inheritdoc/>
        public IReadOnlyList<object> SelectedCards => _selectedCards.AsReadOnly();

        /// <inheritdoc/>
        public bool MultipleCardSelection { get; set; } = false;

        /// <inheritdoc/>
        public int MultipleSelectionLimit { get; set; } = 1;

        /// <inheritdoc/>
        public void SelectCard(object card)
        {
            if (card == null) return;

            // If card is already selected, do nothing
            if (_selectedCards.Contains(card)) return;

            // In single selection mode, clear previous selection
            if (!MultipleCardSelection && _selectedCards.Count > 0)
            {
                ClearSelection();
            }

            // Enforce selection limit in multiple selection mode
            if (MultipleCardSelection && _selectedCards.Count >= MultipleSelectionLimit)
            {
                // Remove oldest selection to make room
                if (_selectedCards.Count > 0)
                {
                    var oldestCard = _selectedCards[0];
                    _selectedCards.RemoveAt(0);
                    _eventBus.Publish(new CardRemovedFromSelectionEvent { Card = oldestCard });
                    _eventBus.Publish(new CardSelectionChangedEvent { SelectedCount = _selectedCards.Count });
                }
            }

            // Add card
            _selectedCards.Add(card);
            _eventBus.Publish(new CardAddedToSelectionEvent { Card = card });
            _eventBus.Publish(new CardSelectionChangedEvent { SelectedCount = _selectedCards.Count });
        }

        /// <inheritdoc/>
        public void UnselectCard(object card)
        {
            if (card == null) return;

            if (_selectedCards.Remove(card))
            {
                _eventBus.Publish(new CardRemovedFromSelectionEvent { Card = card });
                _eventBus.Publish(new CardSelectionChangedEvent { SelectedCount = _selectedCards.Count });
            }
        }

        /// <inheritdoc/>
        public void ClearSelection()
        {
            // Unselect cards in reverse order to avoid index issues
            while (_selectedCards.Count > 0)
            {
                var card = _selectedCards[0];
                _selectedCards.RemoveAt(0);
                _eventBus.Publish(new CardRemovedFromSelectionEvent { Card = card });
            }

            _eventBus.Publish(new CardSelectionChangedEvent { SelectedCount = 0 });
        }

        /// <inheritdoc/>
        public bool IsCardSelected(object card)
        {
            if (card == null) return false;
            return _selectedCards.Contains(card);
        }
    }
}
