using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using Cards.InvocationCards;
using JDG.Domain.ValueObjects;
using UnityEngine;
using VContainer;

/// <summary>
/// Represent all the cards of a player.
/// Phase 8: Removed singleton dependencies (GameState, UnitManager).
/// Phase 17-18: Now uses DeckConfiguration for constants.
/// Phase 21-22: Extracted BuildPlayer and ResetInvocationCardNewTurn to use cases.
/// Uses dependency injection for deck initialization.
/// </summary>
public class PlayerCards : MonoBehaviour
{
    #region Properties

    [SerializeField] private InvocationCard playerInvocationCard;
    [SerializeField] private Transform canvas;
    [SerializeField] private CardLocation cardLocation;
    [SerializeField] private PlayerCards opponentPlayerCards;
    [SerializeField] private bool _isPlayerOne;
    private InGameFieldCard _fieldCard;

    // Phase 8: Injected dependencies
    private IDeckInitializationService _deckInitService;

    // Phase 21-22: Injected use cases
    private SummonPlayerEntityUseCase _summonPlayerEntityUseCase;
    private ResetCardsForNewTurnUseCase _resetCardsForNewTurnUseCase;
    private HandleCardDeathUseCase _handleCardDeathUseCase;
    private HandleCardAddedToFieldUseCase _handleCardAddedToFieldUseCase;
    private HandleHandCardsChangeUseCase _handleHandCardsChangeUseCase;

    public bool IsPlayerOne
    {
        get => _isPlayerOne;
        private set => _isPlayerOne = value;
    }
    public bool SkipCurrentDraw { get; set; }
    public List<InGameCard> Deck = new List<InGameCard>();
    public readonly ObservableCollection<InGameCard> HandCards = new ObservableCollection<InGameCard>();
    public readonly ObservableCollection<InGameEffectCard> EffectCards = new ObservableCollection<InGameEffectCard>();
    public readonly ObservableCollection<InGameInvocationCard> InvocationCards = new ObservableCollection<InGameInvocationCard>();
    public readonly ObservableCollection<InGameCard> YellowCards = new ObservableCollection<InGameCard>();
    private List<InGameInvocationCard> oldInvocations = new List<InGameInvocationCard>();

    public InGameFieldCard FieldCard
    {
        get => _fieldCard;
        set
        {
            if (_fieldCard != value && _fieldCard != null)
            {
                foreach (var fieldCardFieldAbility in _fieldCard.FieldAbilities)
                {
                    fieldCardFieldAbility.OnFieldCardRemoved(this);
                }
            }

            _fieldCard = value;
            CardLocation.UpdateLocation.Invoke();
        }
    }

    public InGameCard Player;

    #endregion

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 8: Inject IDeckInitializationService instead of using singletons.
    /// Phase 21-22: Inject use cases for business logic extraction.
    /// </summary>
    [Inject]
    public void Construct(
        IDeckInitializationService deckInitService,
        SummonPlayerEntityUseCase summonPlayerEntityUseCase,
        ResetCardsForNewTurnUseCase resetCardsForNewTurnUseCase,
        HandleCardDeathUseCase handleCardDeathUseCase,
        HandleCardAddedToFieldUseCase handleCardAddedToFieldUseCase,
        HandleHandCardsChangeUseCase handleHandCardsChangeUseCase)
    {
        _deckInitService = deckInitService;
        _summonPlayerEntityUseCase = summonPlayerEntityUseCase;
        _resetCardsForNewTurnUseCase = resetCardsForNewTurnUseCase;
        _handleCardDeathUseCase = handleCardDeathUseCase;
        _handleCardAddedToFieldUseCase = handleCardAddedToFieldUseCase;
        _handleHandCardsChangeUseCase = handleHandCardsChangeUseCase;
    }

    /// <summary>
    /// Creates the player entity card.
    /// Phase 21-22: Delegates to SummonPlayerEntityUseCase.
    /// </summary>
    public void BuildPlayer()
    {
        Player = _summonPlayerEntityUseCase.Execute(playerInvocationCard, IsPlayerOne);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Phase 8: Use injected service instead of GameState.Instance
        Deck = _deckInitService.GetPlayerDeck(IsPlayerOne);
        var deckLocation = CardLocation.GetDeckLocation(IsPlayerOne);

        // Phase 8: Use injected service instead of UnitManager.Instance
        _deckInitService.InitializePhysicalCards(Deck, deckLocation, IsPlayerOne);

        // Phase 17-18: Use DeckConfiguration instead of GameState for constants
        for (var i = Deck.Count - DeckConfiguration.InitialNumberOfHandCards; i < Deck.Count; i++)
        {
            HandCards.Add(Deck[i]);
        }

        Deck.RemoveRange(Deck.Count - DeckConfiguration.InitialNumberOfHandCards, DeckConfiguration.InitialNumberOfHandCards);
        cardLocation.HideCards(HandCards.ToList());

        InvocationCards.CollectionChanged += InvocationCards_CollectionChanged;
        YellowCards.CollectionChanged += YellowCards_CollectionChanged;
        HandCards.CollectionChanged += HandCards_CollectionChanged;
        EffectCards.CollectionChanged += EffectCards_CollectionChanged;
    }

    /// <summary>
    /// Reset the attack number of invocations during a new turn.
    /// Phase 21-22: Delegates to ResetCardsForNewTurnUseCase.
    /// </summary>
    public void ResetInvocationCardNewTurn()
    {
        _resetCardsForNewTurnUseCase.Execute(InvocationCards);
    }

    /// <summary>
    /// Check if an invocationCard is on field (among Invocation Cards)
    /// </summary>
    /// <param name="invocationCard"></param>
    /// <returns></returns>
    public bool ContainsCardInInvocation(InGameInvocationCard invocationCard)
    {
        return InvocationCards.Any(invocation => invocation != null && invocation.Title == invocationCard.Title);
    }

    private void OnDestroy()
    {
        InvocationCards.CollectionChanged -= InvocationCards_CollectionChanged;
        YellowCards.CollectionChanged -= YellowCards_CollectionChanged;
        HandCards.CollectionChanged -= HandCards_CollectionChanged;
        EffectCards.CollectionChanged -= EffectCards_CollectionChanged;
    }

    #region Event Handlers

    /// <summary>
    /// Apply powers on a new invocation card on field.
    /// Phase 21-22: Delegates to HandleCardAddedToFieldUseCase.
    /// </summary>
    /// <param name="newInvocationCard"></param>
    private void OnInvocationCardAdded(InGameInvocationCard newInvocationCard)
    {
        _handleCardAddedToFieldUseCase.Execute(newInvocationCard, this, opponentPlayerCards);
    }

    /// <summary>
    /// Remove power on a invocation removed from field
    /// </summary>
    private void OnInvocationCardsRemoved()
    {
        var removedInvocationCard = oldInvocations.Except(InvocationCards).First();
        var cloneInvocationCards = InvocationCards.ToList();
        // Apply onCardRemove for invocation card that are still alive
        foreach (var ability in cloneInvocationCards.SelectMany(inGameInvocationCard => inGameInvocationCard.Abilities))
        {
            ability.OnCardRemove(removedInvocationCard, this);
        }

        foreach (var effectAbility in EffectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectAbility.OnInvocationCardRemoved(this, removedInvocationCard);
        }
    }

    /// <summary>
    /// Update Invocation cards location
    /// </summary>
    private void OnInvocationCardsChanged()
    {
        CardLocation.UpdateLocation.Invoke();
    }

    /// <summary>
    /// React to changes among invocation cards
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void InvocationCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnInvocationCardAdded(InvocationCards.Last());
                break;
            case NotifyCollectionChangedAction.Remove:
                OnInvocationCardsRemoved();
                break;
        }

        OnInvocationCardsChanged();
        oldInvocations = InvocationCards.ToList();
    }

    /// <summary>
    /// Reset and update powers for Invocation that goes to Yellow trash.
    /// Phase 21-22: Delegates to HandleCardDeathUseCase.
    /// </summary>
    private void OnYellowTrashAdded()
    {
        var newYellowTrashCard = YellowCards.Last();
        _handleCardDeathUseCase.Execute(newYellowTrashCard, this, opponentPlayerCards, canvas);
    }

    /// <summary>
    /// React to changes among Yellow cards
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void YellowCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnYellowTrashAdded();
                break;
        }

        CardLocation.UpdateLocation.Invoke();
    }

    /// <summary>
    /// React to changes among Hand cards
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void HandCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnHandCardsChange(1);
                break;
            case NotifyCollectionChangedAction.Remove:
                OnHandCardsChange(-1);
                break;
        }
    }

    /// <summary>
    /// React to changes among Effect cards
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EffectCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        CardLocation.UpdateLocation.Invoke();
    }

    /// <summary>
    /// Update powers, location and display when Hand cards change.
    /// Phase 21-22: Delegates to HandleHandCardsChangeUseCase for business logic.
    /// </summary>
    /// <param name="delta"></param>
    private void OnHandCardsChange(int delta)
    {
        _handleHandCardsChangeUseCase.Execute(this, delta);

        // UI updates remain in MonoBehaviour (not business logic)
        CardLocation.UpdateLocation.Invoke();
        HandCardDisplay.HandCardChange.Invoke(HandCards);
    }

    #endregion

}