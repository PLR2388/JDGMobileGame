using System;
using System.Linq;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Application.UseCases;
using JDG.Application.Mappers;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Presentation.Views;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for the card hand view.
    /// Handles card selection from hand and playing cards.
    /// </summary>
    public class CardHandPresenter : IDisposable
    {
        private readonly ICardHandView _view;
        private readonly IEventBus _eventBus;
        private readonly IPlayerRepository _playerRepository;
        private readonly ICardRepository _cardRepository;
        private readonly PlayCardUseCase _playCardUseCase;
        private readonly PlayerId _playerId;

        private IDisposable _cardDrawnSubscription;
        private IDisposable _cardPlayedSubscription;
        private Guid? _selectedCardId;

        public CardHandPresenter(
            ICardHandView view,
            IEventBus eventBus,
            IPlayerRepository playerRepository,
            ICardRepository cardRepository,
            PlayCardUseCase playCardUseCase,
            PlayerId playerId)
        {
            _view = view;
            _eventBus = eventBus;
            _playerRepository = playerRepository;
            _cardRepository = cardRepository;
            _playCardUseCase = playCardUseCase;
            _playerId = playerId;

            _view.OnCardSelected += HandleCardSelected;
            SubscribeToEvents();
            RefreshHand();
        }

        /// <summary>
        /// Refreshes the hand display with current cards.
        /// </summary>
        public void RefreshHand()
        {
            var player = _playerRepository.GetPlayer(_playerId);
            if (player == null)
                return;

            var handCards = player.Hand
                .Select(card => card.ToDTO(isInHand: true))
                .ToList();

            _view.UpdateHand(handCards);
        }

        /// <summary>
        /// Attempts to play the currently selected card.
        /// </summary>
        public void PlaySelectedCard()
        {
            if (!_selectedCardId.HasValue)
                return;

            var cardId = CardId.FromGuid(_selectedCardId.Value);
            var result = _playCardUseCase.Execute(_playerId, cardId);

            if (!result.IsSuccess)
            {
                // Show error (could expose an error event)
                UnityEngine.Debug.LogWarning($"Failed to play card: {result.Message}");
            }

            _selectedCardId = null;
            _view.ClearHighlights();
        }

        /// <summary>
        /// Enables or disables hand interaction.
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            _view.SetInteractable(interactable);
        }

        private void HandleCardSelected(Guid cardId)
        {
            if (_selectedCardId == cardId)
            {
                // Deselect
                _selectedCardId = null;
                _view.ClearHighlights();
            }
            else
            {
                // Select
                _selectedCardId = cardId;
                _view.HighlightCard(cardId);
            }
        }

        private void SubscribeToEvents()
        {
            _cardDrawnSubscription = _eventBus.Subscribe<CardDrawnEvent>(OnCardDrawn);
            _cardPlayedSubscription = _eventBus.Subscribe<CardPlayedEvent>(OnCardPlayed);
        }

        private void OnCardDrawn(CardDrawnEvent evt)
        {
            if (evt.Owner == _playerId.ToCardOwner())
            {
                RefreshHand();
            }
        }

        private void OnCardPlayed(CardPlayedEvent evt)
        {
            if (evt.Owner == _playerId.ToCardOwner())
            {
                RefreshHand();
            }
        }

        public void Dispose()
        {
            _view.OnCardSelected -= HandleCardSelected;
            _cardDrawnSubscription?.Dispose();
            _cardPlayedSubscription?.Dispose();
        }
    }
}
