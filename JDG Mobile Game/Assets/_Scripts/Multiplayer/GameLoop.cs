using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using Sound;
using UnityEngine;
using VContainer;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using JDG.Infrastructure.Services;

/// <summary>
/// Phase 17-18: Removed CardManager singleton dependency via Phase 4 services.
/// Phase 19-20: Injected UIManager instead of using .Instance.
/// Phase 28: Uses IPlayerStatusProvider instead of PlayerManager.Instance.
/// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 35: Uses IDialogService instead of MessageBox/CardSelector.Instance.
/// Phase 8: Uses IAudioService instead of AudioSystem.Instance.
/// </summary>
public class GameLoop : MonoBehaviour
{
    private IEventBus _eventBus;
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

    // Phase 19-20: Injected UIManager (protected so TutoPlayerGameLoop can access)
    protected UIManager _uiManager;

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

    /// <summary>
    /// VContainer injection point. Called before Start().
    /// Phase 17-18: Added Phase 4 services to replace CardManager.Instance.
    /// Phase 19-20: Added UIManager and InputManager injection.
    /// Phase 28: Added IPlayerStatusProvider to replace PlayerManager.Instance.
    /// Phase 34: Added ILocalizationService to replace LocalizationSystem.Instance.
    /// Phase 35: Added IDialogService to replace MessageBox/CardSelector.Instance.
    /// Phase 8: Added IAudioService to replace AudioSystem.Instance.
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
        UIManager uiManager,
        InputManager inputManager,
        IPlayerStatusProvider playerStatusProvider,
        ILocalizationService localizationService,
        IDialogService dialogService,
        IAudioService audioService)
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
        _uiManager = uiManager;
        _inputManager = inputManager;
        _playerStatusProvider = playerStatusProvider;
        _localizationService = localizationService;
        _dialogService = dialogService;
        _audioService = audioService;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Subscribe to EventBus events instead of static UnityEvents
        _eventBus.Subscribe<LongTouchEvent>(OnLongTouch);
        _eventBus.Subscribe<TouchStartedEvent>(OnTouch);
        _eventBus.Subscribe<TouchEndedEvent>(OnReleaseTouch);
        _eventBus.Subscribe<BackButtonPressedEvent>(OnBackPressed);
        Draw();
    }

    protected virtual void OnDestroy()
    {
        // EventBus subscriptions are automatically managed
        // No manual cleanup needed
    }

    #region UI Interaction

    /// <summary>
    /// Action when back is pressed
    /// </summary>
    protected void OnBackPressed(BackButtonPressedEvent evt)
    {
        void PositiveAction()
        {
            SceneLoaderSystem.LoadMainScreen();
        }

        // Phase 19-20: Use injected UIManager instead of .Instance
        _uiManager.DisplayPauseMenu(PositiveAction);
    }

    /// <summary>
    /// When user just stops to touch the screen
    /// </summary>
    protected void OnReleaseTouch(TouchEndedEvent evt)
    {
        // Phase 19-20: Use injected UIManager instead of .Instance
        _uiManager.HideBigImage();
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
            // Phase 19-20: Use injected UIManager instead of .Instance
            _uiManager.DisplayCardOnLargeView(cardTouch);
        }
    }

    /// <summary>
    /// Called when user presses the next phase button
    /// </summary>
    protected virtual void NextRound()
    {
        // Phase 9: Use injected service instead of InvocationMenuManager.Instance
        _invocationMenuService.Hide();
        if (_gameStateService.TurnNumber == 1 && _gameStateService.CurrentPlayer == JDG.Domain.ValueObjects.PlayerId.Player1)
        {
            _gameStateService.SetPhase(JDG.Domain.Phase.End);
        }
        else
        {
            _gameStateService.NextPhase();
        }

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
    /// </summary>
    private void GameOver()
    {
        _gameStateService.SetPhase(JDG.Domain.Phase.GameOver);
        SceneLoaderSystem.LoadMainScreen();
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
    /// Display all available opponent after pressing Attack button
    /// </summary>
    protected void DisplayAvailableOpponent()
    {
        // Phase 17-18: Use ICombatService instead of CardManager.Instance
        var notEmptyOpponent = _combatService.BuildValidTargets();
        DisplayOpponentMessageBox(notEmptyOpponent);
        // Phase 19-20: Use injected InputManager instead of .Instance
        _inputManager.DisableDetectionTouch();
    }

    /// <summary>
    /// Display the MessageBox with the available opponents
    /// </summary>
    /// <param name="invocationCards">Available opponents list</param>
    private void DisplayOpponentMessageBox(List<InGameCard> invocationCards)
    {
        void PositiveAction(InGameInvocationCard invocationCard)
        {
            if (invocationCard != null)
            {
                // Phase 17-18: Use ICombatService instead of CardManager.Instance
                _combatService.Opponent = invocationCard;
                ComputeAttack();
            }
            // Phase 19-20: Use injected InputManager instead of .Instance
            _inputManager.EnableDetectionTouch();
        }

        void NegativeAction()
        {
            // Phase 19-20: Use injected InputManager instead of .Instance
            _inputManager.EnableDetectionTouch();
        }

        // Phase 19-20: Use injected UIManager instead of .Instance
        _uiManager.DisplayOpponentAvailableMessageBox(invocationCards, PositiveAction, NegativeAction);
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