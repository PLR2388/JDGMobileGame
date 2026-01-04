using System;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Events
{
    // ============================================
    // GAME EVENTS OVERVIEW
    // ============================================
    //
    // Phase 145: Documentation of event usage patterns
    //
    // EXTENSIBILITY EVENTS (published but no default subscribers):
    // These events are published for future features and custom extensions:
    // - AttackExecutedEvent: For attack animations, sound effects, analytics
    // - CardDiedEvent: For death animations, sound effects, graveyward UI updates
    // - CardAddedToFieldEvent: For summon animations, ability triggers
    // - CardRemovedFromFieldEvent: For removal animations
    // - CardDiscardedEvent: For discard animations
    // - CardStatsModifiedEvent: For stat change animations
    // - CardsResetForNewTurnEvent: For turn transition effects
    // - HandCardsChangedEvent: For hand count UI updates
    // - FieldCardReplacedEvent: For field card transition effects
    // - PlayerDamagedEvent: For damage animations, screen shake
    // - PlayerHealedEvent: For heal animations
    // - ShieldsAddedEvent: For shield animations
    // - AbilityExecutedEvent: For ability activation feedback
    // - ContreCardPlayRequestedEvent: For counter card UI handling
    //
    // These events are NOT dead code - they enable UI/sound/analytics without
    // modifying core game logic. Subscribe to them in MonoBehaviours as needed.
    //
    // ============================================
    // PHASE & TURN EVENTS
    // ============================================

    /// <summary>
    /// Published when the game phase changes (Draw, Choose, Attack, End, GameOver).
    /// Replaces GameStateManager phase transitions.
    /// </summary>
    public struct PhaseChangedEvent
    {
        public Phase OldPhase;
        public Phase NewPhase;
        public int TurnNumber;
    }

    /// <summary>
    /// Published when the active player changes.
    /// Replaces GameStateManager.ChangePlayer static UnityEvent.
    /// </summary>
    public struct PlayerTurnChangedEvent
    {
        public CardOwner NewPlayer;
        public int TurnNumber;
    }

    /// <summary>
    /// Published at the start of each turn.
    /// </summary>
    public struct TurnStartEvent
    {
        public CardOwner CurrentPlayer;
        public int TurnNumber;
    }

    /// <summary>
    /// Published at the end of each turn.
    /// </summary>
    public struct TurnEndEvent
    {
        public CardOwner CurrentPlayer;
        public int TurnNumber;
    }

    /// <summary>
    /// Published when cards are reset for a new turn.
    /// Phase 21-22: Replaces PlayerCards.ResetInvocationCardNewTurn() side effects.
    /// </summary>
    public struct CardsResetForNewTurnEvent
    {
    }

    // ============================================
    // CARD EVENTS
    // ============================================

    /// <summary>
    /// Published when a card is drawn from deck.
    /// </summary>
    public struct CardDrawnEvent
    {
        public Guid CardId;
        public CardOwner Owner;
        public string CardTitle;
        public int DeckCount;
        public int HandCount;
    }

    /// <summary>
    /// Published when a card is played from hand to field.
    /// </summary>
    public struct CardPlayedEvent
    {
        public Guid CardId;
        public CardOwner Owner;
        public CardType CardType;
        public string CardTitle;
        public int HandCount;
        public int FieldCount;
    }

    /// <summary>
    /// Published when a card is destroyed/killed.
    /// </summary>
    public struct CardDestroyedEvent
    {
        public Guid CardId;
        public CardOwner Owner;
        public Guid? KilledByCardId;
        public string Reason;
    }

    /// <summary>
    /// Published when a card dies and moves to the graveyard (yellow cards).
    /// Phase 21-22: Replaces PlayerCards.OnYellowTrashAdded() side effects.
    /// </summary>
    public struct CardDiedEvent
    {
        /// <summary>
        /// The card that died. Runtime type: IInGameInvocationCard.
        /// Cast with: (IInGameInvocationCard)DeadCard or use pattern matching.
        /// </summary>
        public object DeadCard;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when a player's hand card count changes.
    /// Phase 21-22: Replaces PlayerCards.OnHandCardsChange() side effects.
    /// </summary>
    public struct HandCardsChangedEvent
    {
        public CardOwner Owner;
        public int NewHandCount;
        public int Delta; // +1 for add, -1 for remove
    }

    // Phase 144: Removed dead CardStatsChangedEvent
    // (superseded by CardStatsModifiedEvent which is actually used)

    /// <summary>
    /// Published when a card is added to the field.
    /// Triggers abilities that respond to new cards.
    /// Phase 21-22: Uses object to avoid dependency on InGameCard (legacy type).
    /// </summary>
    public struct CardAddedToFieldEvent
    {
        /// <summary>
        /// The card added to the field. Runtime type: IInGameInvocationCard.
        /// Cast with: (IInGameInvocationCard)AddedCard or use pattern matching.
        /// </summary>
        public object AddedCard;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when a card is removed from the field.
    /// </summary>
    public struct CardRemovedFromFieldEvent
    {
        public Guid CardId;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when a card is selected/clicked by the player.
    /// </summary>
    public struct CardSelectedEvent
    {
        public Guid CardId;
        public CardOwner Owner;
    }

    // ============================================
    // COMBAT EVENTS
    // ============================================

    /// <summary>
    /// Published when an attack is executed.
    /// </summary>
    public struct AttackExecutedEvent
    {
        public Guid AttackerId;
        public Guid DefenderId;
        public int Damage;
        public bool DefenderDestroyed;
        public bool AttackerDestroyed;
    }

    /// <summary>
    /// Published when a player's health changes.
    /// Replaces PlayerStatus.OnHealthChanged static event.
    /// </summary>
    public struct PlayerHealthChangedEvent
    {
        public CardOwner Player;
        public float OldHealth;
        public float NewHealth;
        public float Delta;
    }

    /// <summary>
    /// Published when a player gains or loses shields.
    /// </summary>
    public struct PlayerShieldChangedEvent
    {
        public CardOwner Player;
        public int OldShields;
        public int NewShields;
    }

    /// <summary>
    /// Published when a player takes damage (after shields).
    /// </summary>
    public struct PlayerDamagedEvent
    {
        public CardOwner PlayerId;
        public float Damage;
        public float HealthDamage;
        public float CurrentHealth;
        public bool IsDefeated;
    }

    /// <summary>
    /// Published when a new game starts.
    /// </summary>
    public struct GameStartedEvent
    {
        public CardOwner Player1Id;
        public CardOwner Player2Id;
        public CardOwner StartingPlayer;
        public int TurnNumber;
    }

    /// <summary>
    /// Published when the game ends.
    /// </summary>
    public struct GameOverEvent
    {
        public CardOwner Winner;
        public string Reason;
    }

    // ============================================
    // INPUT EVENTS
    // ============================================

    /// <summary>
    /// Published when player touches the screen.
    /// Replaces InputManager.OnTouch static UnityEvent.
    /// </summary>
    public struct TouchStartedEvent
    {
        public Vector2 Position;
        public float Timestamp;
    }

    /// <summary>
    /// Published when player releases touch.
    /// Replaces InputManager.OnReleaseTouch static UnityEvent.
    /// </summary>
    public struct TouchEndedEvent
    {
        public Vector2 Position;
        public float Duration;
    }

    /// <summary>
    /// Published when player performs long touch.
    /// Replaces InputManager.OnLongTouch static UnityEvent.
    /// </summary>
    public struct LongTouchEvent
    {
        public Vector2 Position;
        public float Duration;
    }

    /// <summary>
    /// Published when Android back button is pressed.
    /// Replaces InputManager.OnBackPressed static UnityEvent.
    /// </summary>
    public struct BackButtonPressedEvent
    {
        public float Timestamp;
    }

    // ============================================
    // UI EVENTS
    // ============================================

    // Phase 144: Removed dead ButtonClickedEvent
    // (superseded by specific button events: NextPhaseButtonClickedEvent, AttackButtonClickedEvent, etc.)

    /// <summary>
    /// Published when next phase button is clicked.
    /// </summary>
    public struct NextPhaseButtonClickedEvent
    {
        public Phase CurrentPhase;
    }

    /// <summary>
    /// Published when attack button is clicked.
    /// </summary>
    public struct AttackButtonClickedEvent
    {
        public Guid AttackerId;
    }

    /// <summary>
    /// Published when end turn button is clicked.
    /// </summary>
    public struct EndTurnButtonClickedEvent
    {
        public CardOwner CurrentPlayer;
    }

    /// <summary>
    /// Published when card locations need to be updated on the UI.
    /// Phase 23: Replaces CardLocation.UpdateLocation static UnityEvent.
    /// </summary>
    public struct CardLocationChangedEvent
    {
        public CardOwner? Player; // Optional: null if both players affected
    }

    /// <summary>
    /// Published when hand cards display needs to be updated.
    /// Phase 23: Replaces HandCardDisplay.HandCardChange static UnityEvent.
    /// </summary>
    public struct HandCardsDisplayChangedEvent
    {
        public CardOwner Player;
        public object HandCards; // Using object to avoid dependency on ObservableCollection<InGameCard>
    }

    /// <summary>
    /// Published when an invocation card effect is cancelled.
    /// Phase 23: Replaces InvocationFunctions.CancelInvocationEvent static UnityEvent.
    /// </summary>
    public struct InvocationCancelledEvent
    {
        /// <summary>
        /// The cancelled card. Runtime type: IInGameInvocationCard.
        /// </summary>
        public object CancelledCard;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when choice player changes in card selection menu.
    /// Phase 23: Replaces CardChoice.ChangeChoicePlayer static UnityEvent.
    /// </summary>
    public struct ChoicePlayerChangedEvent
    {
        public int PlayerIndex; // 0 or 1
    }

    // ============================================
    // ABILITY EVENTS
    // ============================================

    /// <summary>
    /// Published when an ability is activated.
    /// </summary>
    public struct AbilityActivatedEvent
    {
        public Guid CardId;
        public AbilityName AbilityName;
    }

    /// <summary>
    /// Published when an effect is applied.
    /// </summary>
    public struct EffectAppliedEvent
    {
        public Guid SourceCardId;
        public Guid TargetCardId;
        public string EffectDescription;
    }

    // ============================================
    // ABILITY EVENTS
    // ============================================

    /// <summary>
    /// Published when an ability is executed.
    /// </summary>
    public struct AbilityExecutedEvent
    {
        public AbilityName AbilityName;
        public CardOwner PlayerId;
        public bool IsSuccess;
        public string Message;
    }

    /// <summary>
    /// Published when a card is discarded from hand to graveyard.
    /// </summary>
    public struct CardDiscardedEvent
    {
        public Guid CardId;
        public CardOwner Owner;
        public string CardTitle;
    }

    /// <summary>
    /// Published when shields are added to a player.
    /// </summary>
    public struct ShieldsAddedEvent
    {
        public CardOwner PlayerId;
        public int ShieldsAdded;
        public int NewShields;
    }

    /// <summary>
    /// Published when a card's ATK/DEF stats are modified.
    /// Phase 159: Changed from int to float for half-star support.
    /// </summary>
    public struct CardStatsModifiedEvent
    {
        public Guid CardId;
        public CardOwner Owner;
        public float AtkChange;
        public float DefChange;
        public float NewAtk;
        public float NewDef;
    }

    /// <summary>
    /// Published when a player is healed.
    /// </summary>
    public struct PlayerHealedEvent
    {
        public CardOwner PlayerId;
        public float HealAmount;
        public float NewHP;
    }

    // ============================================
    // CARD SELECTION EVENTS
    // Phase 121: Replaces CardSelector.NumberedCardEvent static UnityEvent
    // ============================================

    /// <summary>
    /// Published when a card receives a selection order number (for multi-select).
    /// Phase 121: Replaces CardSelector.NumberedCardEvent static UnityEvent.
    /// </summary>
    public struct CardNumberedEvent
    {
        public object Card; // Using object to avoid dependency on InGameCard
        public int Number;
    }

    // ============================================
    // HIGHLIGHT EVENTS
    // Phase 122: Replaces HighLightPlane.Highlight static UnityEvent
    // ============================================

    /// <summary>
    /// Published when a highlight effect is requested on a game element.
    /// Phase 122: Replaces HighLightPlane.Highlight static UnityEvent.
    /// </summary>
    public struct HighlightRequestedEvent
    {
        public int Element; // HighlightElement enum value (Invocations, Space, Deck, etc.)
        public bool IsActivated;
    }

    // ============================================
    // DIALOGUE EVENTS
    // Phase 123: Replaces DialogueUI static UnityEvents
    // ============================================

    /// <summary>
    /// Published when a dialogue trigger is completed.
    /// Phase 123: Replaces DialogueUI.TriggerDoneEvent static UnityEvent.
    /// </summary>
    public struct DialogueTriggerCompletedEvent
    {
        public int TriggerType; // NextDialogueTrigger enum value
    }

    /// <summary>
    /// Published when the dialogue index changes.
    /// Phase 123: Replaces DialogueUI.DialogIndex static UnityEvent.
    /// </summary>
    public struct DialogueIndexChangedEvent
    {
        public int DialogueIndex;
    }

    // ============================================
    // CARD CLICK EVENTS
    // Phase 109: Replaces InGameMenuScript.EventClick static UnityEvent
    // ============================================

    /// <summary>
    /// Published when user clicks on a card in-game to show the card menu.
    /// Phase 109: Replaces InGameMenuScript.EventClick static UnityEvent.
    /// </summary>
    public struct InGameCardClickedEvent
    {
        /// <summary>
        /// The clicked card. Runtime type: IInGameCard (base interface for all card types).
        /// Use pattern matching to determine specific card type (IInGameInvocationCard, etc.).
        /// </summary>
        public object Card;
    }

    // ============================================
    // CARD PLAY REQUEST EVENTS
    // Phase 36: Replaces static UnityEvents in InGameMenuScript
    // ============================================

    /// <summary>
    /// Published when user requests to play an invocation card.
    /// Phase 36: Replaces InGameMenuScript.InvocationCardEvent static UnityEvent.
    /// </summary>
    public struct InvocationCardPlayRequestedEvent
    {
        /// <summary>
        /// The invocation card to play. Runtime type: IInGameInvocationCard.
        /// </summary>
        public object InvocationCard;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when user requests to play a field card.
    /// Phase 36: Replaces InGameMenuScript.FieldCardEvent static UnityEvent.
    /// </summary>
    public struct FieldCardPlayRequestedEvent
    {
        /// <summary>
        /// The field card to play. Runtime type: IInGameFieldCard.
        /// </summary>
        public object FieldCard;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when user requests to play an effect card.
    /// Phase 36: Replaces InGameMenuScript.EffectCardEvent static UnityEvent.
    /// </summary>
    public struct EffectCardPlayRequestedEvent
    {
        /// <summary>
        /// The effect card to play. Runtime type: IInGameEffectCard.
        /// </summary>
        public object EffectCard;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when user requests to play an equipment card.
    /// Phase 36: Replaces InGameMenuScript.EquipmentCardEvent static UnityEvent.
    /// </summary>
    public struct EquipmentCardPlayRequestedEvent
    {
        /// <summary>
        /// The equipment card to play. Runtime type: IInGameEquipmentCard.
        /// </summary>
        public object EquipmentCard;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when user requests to play a contre (counter) card.
    /// Contre cards are played in response to opponent actions and are discarded immediately.
    ///
    /// WARNING (Phase 155): This event is published by ContreCardHandler but HAS NO SUBSCRIBERS.
    /// The contre card feature is incomplete - playing a contre card currently does nothing.
    /// To complete this feature, create a ContreFunctions class that subscribes to this event
    /// and handles: executing the contre effect, discarding the card, and updating game state.
    /// </summary>
    public struct ContreCardPlayRequestedEvent
    {
        /// <summary>
        /// The contre card to play. Runtime type: IInGameCard.
        /// </summary>
        public object ContreCard;
        public CardOwner Owner;
    }

    // ============================================
    // CARD STATE EVENTS
    // Phase 75: Added for UseCase migration
    // ============================================

    /// <summary>
    /// Published when a card's stats are reset to base values.
    /// Phase 75: Used by HandleCardDeathUseCase and CardStateService.
    /// </summary>
    public struct CardStatsResetEvent
    {
        public ValueObjects.CardId CardId;
        public CardOwner Owner;
        public float BaseAttack;
        public float BaseDefense;
    }

    /// <summary>
    /// Published when a card is controlled by the opponent.
    /// Phase 75: Used by abilities that take control of opponent cards.
    /// </summary>
    public struct CardControlledEvent
    {
        public ValueObjects.CardId CardId;
        public CardOwner OriginalOwner;
        public CardOwner NewController;
    }

    /// <summary>
    /// Published when a card is freed from opponent control.
    /// Phase 75: Used when control effects end.
    /// </summary>
    public struct CardFreedEvent
    {
        public ValueObjects.CardId CardId;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when an invocation card's turn count is incremented.
    /// Phase 75: Used for abilities that trigger after X turns on field.
    /// </summary>
    public struct InvocationTurnCountIncrementedEvent
    {
        public ValueObjects.CardId CardId;
        public CardOwner Owner;
        public int NewTurnCount;
    }

    /// <summary>
    /// Published when an invocation card's death count is incremented.
    /// Phase 75: Used for abilities that track death counts.
    /// </summary>
    public struct InvocationDeathCountIncrementedEvent
    {
        public ValueObjects.CardId CardId;
        public CardOwner Owner;
        public int NewDeathCount;
    }

    /// <summary>
    /// Published when equipment is attached to an invocation card.
    /// Phase 75: Used for equipment ability triggers.
    /// </summary>
    public struct EquipmentAttachedEvent
    {
        public ValueObjects.CardId EquipmentCardId;
        public ValueObjects.CardId TargetCardId;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when equipment is detached from an invocation card.
    /// Phase 75: Used for equipment ability cleanup.
    /// </summary>
    public struct EquipmentDetachedEvent
    {
        public ValueObjects.CardId EquipmentCardId;
        public ValueObjects.CardId PreviousTargetCardId;
        public CardOwner Owner;
    }

    /// <summary>
    /// Published when a field card is replaced.
    /// Phase 75: Used for field card transition effects.
    /// </summary>
    public struct FieldCardReplacedEvent
    {
        public ValueObjects.CardId? OldFieldCardId;
        public ValueObjects.CardId? NewFieldCardId;
        public CardOwner Owner;
    }
}
