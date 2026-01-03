using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Application.Cards;
using JDG.Presentation.Presenters;
using Sound;
using UnityEngine;
using VContainer;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Services;
using JDG.Domain.Events;
using JDG.Infrastructure.Services;

/// <summary>
/// Phase 17-18: Removed CardManager singleton dependency via Phase 4 services.
/// Phase 28: Uses IPlayerStatusProvider instead of PlayerManager.Instance.
/// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 35: Uses IDialogService instead of MessageBox/CardSelector.Instance.
/// Phase 8: Uses IAudioService instead of AudioSystem.Instance.
/// Phase 127: Removed UIManager - uses presenters directly.
/// </summary>
public class GameLoop : MonoBehaviour
{
    // Phase 127: SerializeField references for presenter construction
    [SerializeField] private GameObject bigImageCard;
    [SerializeField] protected GameObject nextPhaseButton;
    // Phase 136: Removed canvasTransform SerializeField - now uses ICanvasProvider
    // Phase 122: Changed to protected so TutoPlayerGameLoop can access for HighlightRequestedEvent
    protected IEventBus _eventBus;
    protected GameStateService _gameStateService;
    protected IRaycastService _raycastService;
    protected IInvocationMenuService _invocationMenuService;
    protected IRoundDisplayService _roundDisplayService;

    // Phase 17-18: Phase 4 services replacing CardManager
    // Changed to protected so TutoPlayerGameLoop can access them
    protected ICombatService _combatService;
    protected ICardCollectionService _cardCollectionService;
    protected ITurnService _turnService;
    protected ICardDrawService _cardDrawService;

    // Phase 127: Direct presenters replacing UIManager
    protected CardDisplayPresenter _cardDisplayPresenter;
    protected DialogPresenter _dialogPresenter;
    protected CardSelectorPresenter _cardSelectorPresenter;

    // Phase 127: ICardVisualService for CardDisplayPresenter
    protected ICardVisualService _cardVisualService;

    // Phase 19-20: Injected InputManager (protected so TutoPlayerGameLoop can access)
    protected InputManager _inputManager;

    // Phase 28: IPlayerStatusProvider instead of PlayerManager.Instance
    protected IPlayerStatusProvider _playerStatusProvider;

    // Phase 34: ILocalizationService instead of LocalizationSystem.Instance
    protected ILocalizationService _localizationService;

    // Phase 35: IDialogService instead of MessageBox/CardSelector.Instance
    protected IDialogService _dialogService;

    // Phase 8: IAudioService instead of AudioSystem.Instance
    protected IAudioService _audioService;

    // Phase 55: ISceneLoaderService instead of SceneLoaderSystem static calls
    protected ISceneLoaderService _sceneLoaderService;

    // Phase 136: ICanvasProvider instead of SerializeField canvasTransform
    protected ICanvasProvider _canvasProvider;

    // Phase 141: IAbilityExecutor for equipment ability execution with sync
    protected IAbilityExecutor _abilityExecutor;

    // Phase 144: Store subscriptions for proper disposal
    private IDisposable _longTouchSubscription;
    private IDisposable _touchStartedSubscription;
    private IDisposable _touchEndedSubscription;
    private IDisposable _backPressedSubscription;
    private IDisposable _gameOverSubscription;

    /// <summary>
    /// VContainer injection point. Called before Start().
    /// Phase 17-18: Added Phase 4 services to replace CardManager.Instance.
    /// Phase 28: Added IPlayerStatusProvider to replace PlayerManager.Instance.
    /// Phase 34: Added ILocalizationService to replace LocalizationSystem.Instance.
    /// Phase 35: Added IDialogService to replace MessageBox/CardSelector.Instance.
    /// Phase 8: Added IAudioService to replace AudioSystem.Instance.
    /// Phase 55: Added ISceneLoaderService to replace SceneLoaderSystem static calls.
    /// Phase 127: Removed UIManager, added ICardVisualService for presenters.
    /// Phase 136: Added ICanvasProvider to replace SerializeField canvasTransform.
    /// Phase 141: Added IAbilityExecutor for equipment ability execution with sync.
    /// </summary>
    [Inject]
    public void Construct(
        IEventBus eventBus,
        GameStateService gameStateService,
        IRaycastService raycastService,
        IInvocationMenuService invocationMenuService,
        IRoundDisplayService roundDisplayService,
        ICombatService combatService,
        ICardCollectionService cardCollectionService,
        ITurnService turnService,
        ICardDrawService cardDrawService,
        InputManager inputManager,
        IPlayerStatusProvider playerStatusProvider,
        ILocalizationService localizationService,
        IDialogService dialogService,
        IAudioService audioService,
        ISceneLoaderService sceneLoaderService,
        ICardVisualService cardVisualService,
        ICanvasProvider canvasProvider,
        IAbilityExecutor abilityExecutor)
    {
        _eventBus = eventBus;
        _gameStateService = gameStateService;
        _raycastService = raycastService;
        _invocationMenuService = invocationMenuService;
        _roundDisplayService = roundDisplayService;
        _combatService = combatService;
        _cardCollectionService = cardCollectionService;
        _turnService = turnService;
        _cardDrawService = cardDrawService;
        _inputManager = inputManager;
        _playerStatusProvider = playerStatusProvider;
        _localizationService = localizationService;
        _dialogService = dialogService;
        _audioService = audioService;
        _sceneLoaderService = sceneLoaderService;
        _cardVisualService = cardVisualService;
        _canvasProvider = canvasProvider;
        _abilityExecutor = abilityExecutor;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Phase 127: Initialize presenters (replacing UIManager)
        // Phase 136: Get canvas from ICanvasProvider instead of SerializeField
        // Phase 147: Added null check for canvas to prevent presenter crashes
        var canvas = _canvasProvider.GetGameCanvas() as Transform;
        if (canvas == null)
        {
            Debug.LogError("GameLoop: Canvas is null! UI presenters will not function correctly.");
            // Phase 148: Early return to prevent presenter crashes when canvas is null
            return;
        }
        _cardDisplayPresenter = new CardDisplayPresenter(bigImageCard, _cardVisualService);
        _dialogPresenter = new DialogPresenter(canvas, _localizationService, _dialogService);
        _cardSelectorPresenter = new CardSelectorPresenter(canvas, nextPhaseButton, _localizationService, _dialogService);

        // Subscribe to EventBus events instead of static UnityEvents
        // Phase 144: Store subscriptions for disposal in OnDestroy
        _longTouchSubscription = _eventBus.Subscribe<LongTouchEvent>(OnLongTouch);
        _touchStartedSubscription = _eventBus.Subscribe<TouchStartedEvent>(OnTouch);
        _touchEndedSubscription = _eventBus.Subscribe<TouchEndedEvent>(OnReleaseTouch);
        _backPressedSubscription = _eventBus.Subscribe<BackButtonPressedEvent>(OnBackPressed);
        // Phase 144: Subscribe to GameOverEvent to handle game ending
        _gameOverSubscription = _eventBus.Subscribe<GameOverEvent>(OnGameOver);

        // Defer Draw() to next frame to ensure all Start() methods complete first.
        // This fixes the race condition where Draw() might run before PlayerCards.Start()
        // has finished setting up hand cards and collection change handlers.
        StartCoroutine(DeferredDraw());
    }

    /// <summary>
    /// Waits one frame before drawing the first card, ensuring all MonoBehaviour
    /// Start() methods have completed their initialization.
    /// </summary>
    private IEnumerator DeferredDraw()
    {
        yield return null; // Wait one frame
        Draw();
    }

    /// <summary>
    /// Phase 144: Fixed - subscriptions must be manually disposed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        _longTouchSubscription?.Dispose();
        _touchStartedSubscription?.Dispose();
        _touchEndedSubscription?.Dispose();
        _backPressedSubscription?.Dispose();
        _gameOverSubscription?.Dispose();
    }

    #region UI Interaction

    /// <summary>
    /// Action when back is pressed
    /// </summary>
    protected void OnBackPressed(BackButtonPressedEvent evt)
    {
        void PositiveAction()
        {
            // Phase 55: Use ISceneLoaderService instead of SceneLoaderSystem
            _sceneLoaderService.LoadMainScreen();
        }

        // Phase 127: Use DialogPresenter directly instead of UIManager
        _dialogPresenter.ShowPauseMenu(PositiveAction);
    }

    /// <summary>
    /// When user just stops to touch the screen
    /// </summary>
    protected void OnReleaseTouch(TouchEndedEvent evt)
    {
        // Phase 127: Use CardDisplayPresenter directly instead of UIManager
        _cardDisplayPresenter.HideCard();
    }

    /// <summary>
    /// When user just touches the screen
    /// </summary>
    private void OnTouch(TouchStartedEvent evt)
    {
        var cardTouch = _raycastService.GetTouchedCard();
        // Convert domain CardOwner to global CardOwner enum
        var domainOwner = _gameStateService.CurrentPlayer.ToCardOwner();
        var currentOwner = (CardOwner)(int)domainOwner;
        if (cardTouch != null)
        {
            switch (_gameStateService.CurrentPhase)
            {
                case JDG.Domain.Phase.Choose:
                {
                    HandleSingleTouch(cardTouch, currentOwner, false);
                }
                    break;
                case JDG.Domain.Phase.Attack:
                {
                    HandleSingleTouch(cardTouch, currentOwner, true);
                }

                    break;
            }
        }
    }

    /// <summary>
    /// Display a menu and set attacker if user touches a card he owns
    /// </summary>
    /// <param name="cardTouch">Current card touched</param>
    /// <param name="currentOwner">Owner associated to the current player</param>
    /// <param name="isAttackPhase">Is the touch happen during attack phase</param>
    protected void HandleSingleTouch(InGameCard cardTouch, CardOwner currentOwner, bool isAttackPhase)
    {

        if (cardTouch is InGameInvocationCard invocationCard)
        {
            if (invocationCard.CardOwner == currentOwner || invocationCard.IsControlled)
            {
                // Phase 17-18: Use ICombatService instead of CardManager.Instance
                _combatService.Attacker = invocationCard;
                // Phase 9: Use injected service instead of InvocationMenuManager.Instance
                _invocationMenuService.Display(isAttackPhase);
            }
        }
    }

    /// <summary>
    /// Called when user touches during a long time
    /// </summary>
    protected void OnLongTouch(LongTouchEvent evt)
    {
        // Phase 9: Use injected service instead of InvocationMenuManager.Instance
        _invocationMenuService.Hide();
        var cardTouch = _raycastService.GetTouchedCard();
        if (cardTouch != null)
        {
            // Phase 127: Use CardDisplayPresenter directly instead of UIManager
            _cardDisplayPresenter.ShowCard(cardTouch);
        }
    }

    /// <summary>
    /// Called when user presses the next phase button.
    /// Note: Attack phase skip for Player 1 on Turn 1 is handled automatically by GameStateService.NextPhase().
    /// </summary>
    protected virtual void NextRound()
    {
        // Phase 9: Use injected service instead of InvocationMenuManager.Instance
        _invocationMenuService.Hide();

        // NextPhase() automatically skips Attack phase for Player 1 on Turn 1
        _gameStateService.NextPhase();

        // Check if attack is blocked by card effects
        var playerStatus = _playerStatusProvider.GetCurrentPlayerStatus();
        if (_gameStateService.CurrentPhase == JDG.Domain.Phase.Attack && playerStatus.BlockAttack)
        {
            _gameStateService.SetPhase(JDG.Domain.Phase.End);
        }

        // Phase 9: Use injected service instead of RoundDisplayManager.Instance
        _roundDisplayService.AdaptUIToPhaseIdInNextRound(true);

        switch (_gameStateService.CurrentPhase)
        {
            case JDG.Domain.Phase.Attack:
                PlayAttackMusic();
                break;
            case JDG.Domain.Phase.End:
                EndTurnPhase();
                break;
        }
    }

    #endregion

    #region Phase Behavior

    /// <summary>
    /// Called when choose phase starts
    /// </summary>
    protected virtual void ChoosePhase()
    {
        // Phase 9: Use injected service instead of InvocationMenuManager.Instance
        _invocationMenuService.Enable();
        ChoosePhaseMusic();
    }

    /// <summary>
    /// Choose the right Choose music
    /// Phase 8: Uses IAudioService instead of AudioSystem.Instance.
    /// </summary>
    protected void ChoosePhaseMusic()
    {
        // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
        var currentFieldCard = _cardCollectionService.GetCurrentPlayerCards().FieldCard;
        if (currentFieldCard == null)
        {
            // Phase 8: Use IAudioService instead of AudioSystem.Instance
            _audioService.PlayMusic(nameof(Music.DrawPhase));
        }
        else
        {
            // Phase 8: Use IAudioService instead of AudioSystem.Instance
            _audioService.PlayFamilyMusic(currentFieldCard.Family);
        }
    }

    /// <summary>
    /// Redirect player after a Gameover
    /// Phase 55: Uses ISceneLoaderService instead of SceneLoaderSystem.
    /// </summary>
    private void GameOver()
    {
        _gameStateService.SetPhase(JDG.Domain.Phase.GameOver);
        _sceneLoaderService.LoadMainScreen();
    }

    /// <summary>
    /// Handler for GameOverEvent from EventBus.
    /// Phase 144: Subscribes to GameOverEvent to trigger game over flow.
    /// </summary>
    private void OnGameOver(GameOverEvent evt)
    {
        Debug.Log($"GameLoop.OnGameOver: Winner = {evt.Winner}, Reason = {evt.Reason}");
        GameOver();
    }

    /// <summary>
    /// Play the attack music
    /// Phase 8: Uses IAudioService instead of AudioSystem.Instance.
    /// </summary>
    protected void PlayAttackMusic()
    {
        // Phase 8: Use IAudioService instead of AudioSystem.Instance
        _audioService.PlayMusic(nameof(Music.Fight));
    }

    /// <summary>
    /// Display all available opponent after pressing Attack button.
    /// Defense-in-depth: Uses GameStateService.ShouldSkipAttackPhase for Turn 1 restriction.
    /// </summary>
    protected void DisplayAvailableOpponent()
    {
        // Defense-in-depth: Block attack if attack phase should be skipped
        if (_gameStateService.ShouldSkipAttackPhase)
        {
            Debug.Log("GameLoop: Attack blocked - Player 1 cannot attack on Turn 1");
            return;
        }

        Debug.Log("GameLoop.DisplayAvailableOpponent: Called");
        // Phase 17-18: Use ICombatService instead of CardManager.Instance
        var notEmptyOpponent = _combatService.BuildValidTargets();
        Debug.Log($"GameLoop.DisplayAvailableOpponent: Found {notEmptyOpponent?.Count ?? 0} valid targets");
        DisplayOpponentMessageBox(notEmptyOpponent);
        // Phase 19-20: Use injected InputManager instead of .Instance
        _inputManager.DisableDetectionTouch();
    }

    /// <summary>
    /// Display the MessageBox with the available opponents
    /// Phase 127: Uses CardSelectorPresenter directly with interface types.
    /// Phase 140: Added tracing for debugging target display.
    /// </summary>
    /// <param name="invocationCards">Available opponents list</param>
    private void DisplayOpponentMessageBox(List<InGameCard> invocationCards)
    {
        Debug.Log($"GameLoop.DisplayOpponentMessageBox() - START, count: {invocationCards?.Count ?? -1}");
        if (invocationCards != null)
        {
            foreach (var card in invocationCards)
            {
                Debug.Log($"GameLoop.DisplayOpponentMessageBox() - Card: {card?.Title ?? "NULL"}, Type: {card?.GetType().FullName ?? "NULL"}");
            }
        }
        void OnCardSelected(IInGameInvocationCard selectedCard)
        {
            if (selectedCard != null)
            {
                // Phase 17-18: Use ICombatService instead of CardManager.Instance
                // Phase 144: Add null check for cast result
                var concreteCard = selectedCard as InGameInvocationCard;
                if (concreteCard == null)
                {
                    Debug.LogWarning($"GameLoop.OnCardSelected: Failed to cast {selectedCard.GetType().Name} to InGameInvocationCard");
                    _inputManager.EnableDetectionTouch();
                    return;
                }
                _combatService.Opponent = concreteCard;
                ComputeAttack();
            }
            // Phase 19-20: Use injected InputManager instead of .Instance
            _inputManager.EnableDetectionTouch();
        }

        void OnCancelled()
        {
            // Phase 19-20: Use injected InputManager instead of .Instance
            _inputManager.EnableDetectionTouch();
        }

        // Phase 127: Use CardSelectorPresenter directly with interface types
        IReadOnlyList<IInGameCard> cards = invocationCards?.Cast<IInGameCard>().ToList();
        _cardSelectorPresenter.ShowOpponentSelector(cards, OnCardSelected, OnCancelled);
    }

    /// <summary>
    /// Compute Attack after player chooses an opponent
    /// </summary>
    protected void ComputeAttack()
    {
        // Phase 17-18: Use ICombatService instead of CardManager.Instance
        _combatService.HandleAttack();
        // Phase 9: Use injected service instead of InvocationMenuManager.Instance
        _invocationMenuService.UpdateAttackButton();
        HandlePlayerDeath();
    }

    /// <summary>
    /// Check if one of the player die
    /// </summary>
    private void HandlePlayerDeath()
    {
        // Check if one player die
        var playerStatus = _playerStatusProvider.GetCurrentPlayerStatus();
        var opponentPlayerStatus = _playerStatusProvider.GetOpponentPlayerStatus();

        // Phase 148: Add null checks to prevent NullReferenceException
        if (playerStatus == null || opponentPlayerStatus == null)
        {
            Debug.LogWarning("GameLoop.HandlePlayerDeath: PlayerStatus or OpponentPlayerStatus is null");
            return;
        }

        if (playerStatus.GetCurrentHealth() <= 0)
        {
            GameOver();
        }
        else if (opponentPlayerStatus.GetCurrentHealth() <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Draw a card during draw phase
    /// </summary>
    protected void Draw()
    {
        DoDraw();
        _gameStateService.StartNewTurn();
        _gameStateService.NextPhase();

        ChoosePhase();
        // Phase 9: Use injected service instead of RoundDisplayManager.Instance
        // Phase 34: Use injected ILocalizationService instead of LocalizationSystem.Instance
        _roundDisplayService.SetRoundText(
            _localizationService.GetLocalizedValue(LocalizationKeys.PHASE_CHOOSE)
        );
    }

    /// <summary>
    /// Do the actual draw
    /// </summary>
    private void DoDraw()
    {
        // Phase 17-18: Use ITurnService instead of CardManager.Instance
        _turnService.OnTurnStart();

        void OnNoCards()
        {
            GameOver();
        }

        // Phase 17-18: Use ICardDrawService instead of CardManager.Instance
        _cardDrawService.DrawCard(OnNoCards);
    }

    /// <summary>
    /// Called when a turn end for a player
    /// </summary>
    protected void EndTurnPhase()
    {
        // Phase 17-18: Use ITurnService instead of CardManager.Instance
        _turnService.HandleEndTurn();
        _gameStateService.HandleEndTurn();
        Draw();
    }

    #endregion

}