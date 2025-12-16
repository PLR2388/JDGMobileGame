using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Application;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using Sound;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using Random = UnityEngine.Random;

namespace Menu
{
    /// <summary>
    /// Manages the card choices and selections in the game menu.
    /// Phase 9: Removed CardSelectionManager singleton dependency via DI.
    /// Phase 17-18: Removed GameState singleton dependency via IDeckManagementService.
    /// Phase 23: Migrated static UnityEvent to EventBus (ChangeChoicePlayer).
    /// Phase 24-25: Added ICardCollectionService dependency for CardFactory.
    /// Phase 8: Uses IAudioService instead of AudioSystem.Instance.
    /// Phase 8: Uses CardChoiceUIManager DI instead of .Instance.
    /// Phase 41: Migrated to clean JDG.Application.Services.ICardSelectionService.
    /// Phase 7: Added IAbilityProvider for ability system migration.
    /// </summary>
    public class CardChoice : MonoBehaviour
    {
        [SerializeField] private GameObject container;

        /// <summary>
        /// Indicates if the player one has chosen their cards.
        /// </summary>
        public bool isPlayerOneCardChosen;

        // Phase 41: Migrated to clean ICardSelectionService
        private JDG.Application.Services.ICardSelectionService _cardSelectionService;

        // Phase 17-18: Injected dependencies
        private IDeckManagementService _deckManagementService;

        // Phase 23: EventBus for static UnityEvent migration
        private IEventBus _eventBus;

        // Phase 24-25: Injected for CardFactory
        private ICardCollectionService _cardCollectionService;

        // Phase 8: IAudioService instead of AudioSystem.Instance
        private JDG.Application.Services.IAudioService _audioService;

        // Phase 8: CardChoiceUIManager DI instead of .Instance
        private CardChoiceUIManager _cardChoiceUIManager;

        // Phase 7: IAbilityProvider for ability system migration
        private IAbilityProvider _abilityProvider;

        // Phase 45: Flag to track if DI was properly initialized
        private bool _diInitialized;

        /// <summary>
        /// VContainer method injection for dependencies.
        /// Phase 9: Inject ICardSelectionService instead of using singleton.
        /// Phase 17-18: Inject IDeckManagementService instead of GameState.Instance.
        /// Phase 23: Inject IEventBus for static UnityEvent migration.
        /// Phase 24-25: Inject ICardCollectionService for CardFactory.
        /// Phase 8: Inject IAudioService instead of AudioSystem.Instance.
        /// Phase 8: Inject CardChoiceUIManager instead of using .Instance.
        /// Phase 41: Migrated to clean JDG.Application.Services.ICardSelectionService.
        /// Phase 7: Inject IAbilityProvider for ability system migration.
        /// </summary>
        [Inject]
        public void Construct(
            JDG.Application.Services.ICardSelectionService cardSelectionService,
            IDeckManagementService deckManagementService,
            IEventBus eventBus,
            ICardCollectionService cardCollectionService,
            JDG.Application.Services.IAudioService audioService,
            CardChoiceUIManager cardChoiceUIManager,
            IAbilityProvider abilityProvider = null)
        {
            _cardSelectionService = cardSelectionService;
            _deckManagementService = deckManagementService;
            _eventBus = eventBus;
            _cardCollectionService = cardCollectionService;
            _audioService = audioService;
            _cardChoiceUIManager = cardChoiceUIManager;
            _abilityProvider = abilityProvider;
            _diInitialized = true;
        }

        /// <summary>
        /// Unity Awake - ensure services are initialized.
        /// Phase 45: Added fallback initialization if VContainer DI is not set up.
        /// </summary>
        private void Awake()
        {
            // VContainer injection runs before Awake, so if _diInitialized is false,
            // VContainer DI is not set up properly in this scene
            if (!_diInitialized)
            {
                Debug.LogWarning("CardChoice: VContainer DI not initialized. Using fallback initialization. " +
                    "Please ensure LegacyServicesScope is attached to a GameObject in the scene.");
                InitializeFallbackServices();
            }
        }

        /// <summary>
        /// Fallback initialization for when VContainer DI is not set up.
        /// Creates minimal required services using FindObjectOfType and direct instantiation.
        /// Phase 45: Added to handle scenes without proper VContainer setup.
        /// </summary>
        private void InitializeFallbackServices()
        {
            // Create EventBus if not injected
            if (_eventBus == null)
            {
                _eventBus = new JDG.Infrastructure.Events.EventBus();
            }

            // Find CardChoiceUIManager in scene if not injected
            if (_cardChoiceUIManager == null)
            {
                _cardChoiceUIManager = FindObjectOfType<CardChoiceUIManager>();
            }

            // Create CardSelectionService if not injected (only needs EventBus)
            if (_cardSelectionService == null)
            {
                _cardSelectionService = new JDG.Infrastructure.Services.CardSelectionService(_eventBus);
            }

            // Create DeckManagementService if not injected
            // Requires multiple dependencies, so we create a minimal stub
            if (_deckManagementService == null)
            {
                // Get CardDataProvider (needs to load cards from resources)
                var cardDataProvider = new JDG.Infrastructure.Services.CardDataProvider();

                // Create minimal CardCollectionServiceAdapter (can be null for card choice)
                ICardCollectionService cardCollection = null;

                // Create AbilityProvider if not already set
                if (_abilityProvider == null)
                {
                    var registry = new JDG.Application.Abilities.AbilityRegistry();
                    _abilityProvider = new AbilityProviderService(registry, null, _eventBus);
                }

                _deckManagementService = new DeckManagementService(
                    _eventBus,
                    cardCollection,
                    cardDataProvider,
                    _abilityProvider);
            }

            // CardCollectionService can remain null for Main Screen scene
            // (only used in game scene for field cards)

            _diInitialized = true;
        }

        /// <summary>
        /// Checks and counts the selected cards in the deck.
        /// </summary>
        /// <param name="deck">The collection of cards to check.</param>
        /// <returns>The number of selected cards.</returns>
        private int CheckCard(ICollection<Card> deck)
        {
            var numberSelected = 0;
            var hoverComponents = container.GetComponentsInChildren<OnHover>();
            foreach (var hoverComponent in hoverComponents)
            {
                var isSelected = hoverComponent.IsSelected;
                if (!isSelected) continue;
                numberSelected++;
                deck.Add(hoverComponent.gameObject.GetComponent<CardDisplay>().Card);
            }

            return numberSelected;
        }

        /// <summary>
        /// Deselects all the cards.
        /// </summary>
        private void DeselectAllCards()
        {
            // Phase 9: Use injected service instead of CardSelectionManager.Instance
            _cardSelectionService?.ClearSelection();
        }

        /// <summary>
        /// Verifies and manages the cards chosen by the player.
        /// </summary>
        public void CheckPlayerCards()
        {
            var deck = new List<Card>();
            var numberSelected = CheckCard(deck);

            if (numberSelected == DeckConfiguration.MaxDeckCards)
            {
                // Phase 8: Use injected CardChoiceUIManager instead of .Instance
                _cardChoiceUIManager.UpdateTitleAndButtonTextForPlayer(isPlayerOneCardChosen);
                if (isPlayerOneCardChosen)
                {
                    // Phase 8: Use IAudioService instead of AudioSystem.Instance
                    _audioService?.StopMusic();
                    SceneLoaderSystem.LoadGameScreen();
                    isPlayerOneCardChosen = false;
                    _eventBus.Publish(new ChoicePlayerChangedEvent { PlayerIndex = 1 });

                    _deckManagementService.Player2DeckCards =
                        deck.Select(card => CardFactory.CreateInGameCard(card, CardOwner.Player2, _eventBus, _cardCollectionService, _abilityProvider)).ToList();
                }
                else
                {
                    isPlayerOneCardChosen = true;
                    _eventBus.Publish(new ChoicePlayerChangedEvent { PlayerIndex = 2 });

                    _deckManagementService.Player1DeckCards =
                        deck.Select(card => CardFactory.CreateInGameCard(card, CardOwner.Player1, _eventBus, _cardCollectionService, _abilityProvider)).ToList();
                    DeselectAllCards();
                }
            }
            else
            {
                var remainedCards = DeckConfiguration.MaxDeckCards - numberSelected;
                // Phase 8: Use injected CardChoiceUIManager instead of .Instance
                _cardChoiceUIManager.DisplayMessageBox(remainedCards);
            }
        }

        /// <summary>
        /// Filters and retrieves the cards that meet specific criteria.
        /// </summary>
        /// <param name="sourceCards">The source cards to filter from.</param>
        /// <returns>A list of filtered cards.</returns>
        private static List<Card> FilterCards(List<Card> sourceCards)
        {
            return sourceCards.Where(card =>
                    card.Type != CardType.Contre &&
                    card.Title != CardNameMappings.CardNameMap[CardNames.AttaqueDeLaTourEiffel] &&
                    card.Title != CardNameMappings.CardNameMap[CardNames.BlagueInterdite] &&
                    card.Title != CardNameMappings.CardNameMap[CardNames.UnBonTuyau])
                .ToList();
        }

        /// <summary>
        /// Generates a random deck of cards.
        /// </summary>
        /// <param name="numberOfCards">The number of cards required in the deck.</param>
        /// <param name="initialDeck">The initial collection of cards.</param>
        /// <param name="cards">The list of cards to choose from.</param>
        public static void GetRandomDeck(int numberOfCards, ref List<Card> initialDeck, List<Card> cards)
        {
            var deckAllCard = FilterCards(cards);

            while (initialDeck.Count != numberOfCards)
            {
                GetRandomCards(deckAllCard, initialDeck);
            }
        }

        /// <summary>
        /// Sets random decks for players and starts the game.
        /// </summary>
        public void RandomDeck()
        {
            var deck1 = new List<Card>();
            var deck2 = new List<Card>();

            var deck1AllCard = FilterCards(_deckManagementService.Deck1AllCards);
            var deck2AllCard = FilterCards(_deckManagementService.Deck2AllCards);

            while (deck1.Count != DeckConfiguration.MaxDeckCards)
            {
                GetRandomCards(deck1AllCard, deck1);
            }

            while (deck2.Count != DeckConfiguration.MaxDeckCards)
            {
                GetRandomCards(deck2AllCard, deck2);
            }

            _deckManagementService.Player1DeckCards =
                deck1.Select(card1 => CardFactory.CreateInGameCard(card1, CardOwner.Player1, _eventBus, _cardCollectionService, _abilityProvider)).ToList();
            _deckManagementService.Player2DeckCards =
                deck2.Select(card2 => CardFactory.CreateInGameCard(card2, CardOwner.Player2, _eventBus, _cardCollectionService, _abilityProvider)).ToList();
            // Phase 8: Use IAudioService instead of AudioSystem.Instance
            _audioService?.StopMusic();
            SceneLoaderSystem.LoadGameScreen();
        }

        /// <summary>
        /// Sets a deck for testing purposes and starts the game.
        /// </summary>
        public void DeckToTest()
        {
            var deck1 = new List<Card>();
            var deck2 = new List<Card>();

            var deck1AllCard = _deckManagementService.Deck1AllCards;
            var deck2AllCard = _deckManagementService.Deck2AllCards;

            deck2.Add(GetSpecificCard(CardNames.LycéeMagiqueGeorgesPompidou, deck2AllCard));
            deck1.Add(GetSpecificCard(CardNames.SandrineLePorteManteauExtraterrestre, deck1AllCard));
            deck1.Add(GetSpecificCard(CardNames.BenzaieJeune, deck1AllCard));
            deck1.Add(GetSpecificCard(CardNames.AlphaMan, deck1AllCard));
            deck1.Add(GetSpecificCard(CardNames.FiltreDégueulasseFMV, deck1AllCard));

            while (deck1.Count != DeckConfiguration.MaxDeckCards)
            {
                GetRandomCards(deck1AllCard, deck1);
            }

            deck1.Reverse();


            while (deck2.Count != DeckConfiguration.MaxDeckCards)
            {
                GetRandomCards(deck2AllCard, deck2);
            }

            deck2.Reverse();

            _deckManagementService.Player1DeckCards =
                deck1.Select(card1 => CardFactory.CreateInGameCard(card1, CardOwner.Player1, _eventBus, _cardCollectionService, _abilityProvider)).ToList();
            _deckManagementService.Player2DeckCards =
                deck2.Select(card2 => CardFactory.CreateInGameCard(card2, CardOwner.Player2, _eventBus, _cardCollectionService, _abilityProvider)).ToList();
            // Phase 8: Use IAudioService instead of AudioSystem.Instance
            _audioService?.StopMusic();
            SceneLoaderSystem.LoadGameScreen();
        }

        /// <summary>
        /// Retrieves a specific card based on its name.
        /// </summary>
        /// <param name="cardNames">The name identifier of the card.</param>
        /// <param name="cards">The list of cards to search from.</param>
        /// <returns>The specific card found; null otherwise.</returns>
        public static Card GetSpecificCard(CardNames cardNames, List<Card> cards)
        {
            var nameCard = CardNameMappings.CardNameMap[cardNames];
            var card = cards.Find(x => x.Title == nameCard);
            if (card != null)
            {
                cards.Remove(card);
            }

            return card;
        }

        /// <summary>
        /// Gets a random card from the list and adds it to the deck.
        /// </summary>
        /// <param name="allCards">The list of cards to choose from.</param>
        /// <param name="deck">The deck to which the card is added.</param>
        private static void GetRandomCards(IList<Card> allCards, ICollection<Card> deck)
        {
            var randomIndex = Random.Range(0, allCards.Count - 1);
            var card = allCards[randomIndex];
            if (card.Type == CardType.Contre) return;
            if (card == null) return;
            deck.Add(card);
            allCards.Remove(card);
        }

        /// <summary>
        /// Handles the back action in the game menu.
        /// Phase 8: Uses injected CardChoiceUIManager instead of .Instance.
        /// </summary>
        public void Back()
        {
            if (isPlayerOneCardChosen)
            {
                // Phase 8: Use injected CardChoiceUIManager instead of .Instance
                _cardChoiceUIManager.UpdateTitleAndButtonTextForPlayer(true);
                isPlayerOneCardChosen = false;
                // Phase 17-18: Use IDeckManagementService instead of GameState.Instance
                _deckManagementService.Player1DeckCards = new List<InGameCard>();
                DeselectAllCards();
                _eventBus.Publish(new ChoicePlayerChangedEvent { PlayerIndex = 1 });
            }
            else
            {
                DeselectAllCards();
                // Phase 8: Use injected CardChoiceUIManager instead of .Instance
                _cardChoiceUIManager.ShowChoiceCardMenu(false);
                _cardChoiceUIManager.ShowTwoPlayerModeMenu(true);
            }
        }
    }
}