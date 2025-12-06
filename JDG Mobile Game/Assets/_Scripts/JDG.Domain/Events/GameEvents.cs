using System;
using JDG.Domain;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Events
{
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
    /// Published when a card's stats change.
    /// </summary>
    public struct CardStatsChangedEvent
    {
        public Guid CardId;
        public int OldAttack;
        public int OldDefense;
        public int NewAttack;
        public int NewDefense;
    }

    /// <summary>
    /// Published when a card is added to the field.
    /// Triggers abilities that respond to new cards.
    /// </summary>
    public struct CardAddedToFieldEvent
    {
        public Guid CardId;
        public CardOwner Owner;
        public CardType CardType;
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
        public int OldHealth;
        public int NewHealth;
        public int Delta;
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
        public int Damage;
        public int HealthDamage;
        public int CurrentHealth;
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

    /// <summary>
    /// Published when hand cards collection changes.
    /// Replaces HandCardDisplay.HandCardChange static event.
    /// </summary>
    public struct HandCardsChangedEvent
    {
        public CardOwner Owner;
        public int NewCount;
        public int Delta; // +1 for add, -1 for remove
    }

    /// <summary>
    /// Published when a UI button is clicked.
    /// </summary>
    public struct ButtonClickedEvent
    {
        public string ButtonId;
    }

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
    /// </summary>
    public struct CardStatsModifiedEvent
    {
        public Guid CardId;
        public CardOwner Owner;
        public int AtkChange;
        public int DefChange;
        public int NewAtk;
        public int NewDef;
    }

    /// <summary>
    /// Published when a player is healed.
    /// </summary>
    public struct PlayerHealedEvent
    {
        public CardOwner PlayerId;
        public int HealAmount;
        public int NewHP;
    }
}
