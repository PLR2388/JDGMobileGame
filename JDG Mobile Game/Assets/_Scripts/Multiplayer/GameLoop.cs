using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using Sound;
using UnityEngine;
using VContainer;
using JDG.Application;
using JDG.Domain.Events;
using JDG.Infrastructure.Services;

public class GameLoop : MonoBehaviour
{
    private IEventBus _eventBus;
    protected GameStateService _gameStateService;
    protected IRaycastService _raycastService;

    /// <summary>
    /// VContainer injection point. Called before Start().
    /// </summary>
    [Inject]
    public void Construct(IEventBus eventBus, GameStateService gameStateService, IRaycastService raycastService)
    {
        _eventBus = eventBus;
        _gameStateService = gameStateService;
        _raycastService = raycastService;
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

        UIManager.Instance.DisplayPauseMenu(PositiveAction);
    }

    /// <summary>
    /// When user just stops to touch the screen
    /// </summary>
    protected void OnReleaseTouch(TouchEndedEvent evt)
    {
        UIManager.Instance.HideBigImage();
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
    protected static void HandleSingleTouch(InGameCard cardTouch, CardOwner currentOwner, bool isAttackPhase)
    {

        if (cardTouch is InGameInvocationCard invocationCard)
        {
            if (invocationCard.CardOwner == currentOwner || invocationCard.IsControlled)
            {
                CardManager.Instance.Attacker = invocationCard;
                InvocationMenuManager.Instance.Display(isAttackPhase);
            }
        }
    }

    /// <summary>
    /// Called when user touches during a long time
    /// </summary>
    protected void OnLongTouch(LongTouchEvent evt)
    {
        InvocationMenuManager.Instance.Hide();
        var cardTouch = _raycastService.GetTouchedCard();
        if (cardTouch != null)
        {
            UIManager.Instance.DisplayCardOnLargeView(cardTouch);
        }
    }

    /// <summary>
    /// Called when user presses the next phase button
    /// </summary>
    protected virtual void NextRound()
    {
        InvocationMenuManager.Instance.Hide();
        if (_gameStateService.TurnNumber == 1 && _gameStateService.CurrentPlayer == JDG.Domain.ValueObjects.PlayerId.Player1)
        {
            _gameStateService.SetPhase(JDG.Domain.Phase.End);
        }
        else
        {
            _gameStateService.NextPhase();
        }

        var playerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
        if (_gameStateService.CurrentPhase == JDG.Domain.Phase.Attack && playerStatus.BlockAttack)
        {
            _gameStateService.SetPhase(JDG.Domain.Phase.End);
        }

        RoundDisplayManager.Instance.AdaptUIToPhaseIdInNextRound(true);

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
        InvocationMenuManager.Instance.Enable();
        ChoosePhaseMusic();
    }

    /// <summary>
    /// Choose the right Choose music
    /// </summary>
    protected void ChoosePhaseMusic()
    {
        var currentFieldCard = CardManager.Instance.GetCurrentPlayerCards().FieldCard;
        if (currentFieldCard == null)
        {
            AudioSystem.Instance.PlayMusic(Music.DrawPhase);
        }
        else
        {
            AudioSystem.Instance.PlayFamilyMusic(currentFieldCard.Family);
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
    /// </summary>
    protected void PlayAttackMusic()
    {
        AudioSystem.Instance.PlayMusic(Music.Fight);
    }

    /// <summary>
    /// Display all available opponent after pressing Attack button
    /// </summary>
    protected void DisplayAvailableOpponent()
    {
        var notEmptyOpponent = CardManager.Instance.BuildInvocationCardsForAttack();
        DisplayOpponentMessageBox(notEmptyOpponent);
        InputManager.Instance.DisableDetectionTouch();
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
                CardManager.Instance.Opponent = invocationCard;
                ComputeAttack();
            }
            InputManager.Instance.EnableDetectionTouch();
        }

        void NegativeAction()
        {
            InputManager.Instance.EnableDetectionTouch();
        }

        UIManager.Instance.DisplayOpponentAvailableMessageBox(invocationCards, PositiveAction, NegativeAction);
    }

    /// <summary>
    /// Compute Attack after player chooses an opponent
    /// </summary>
    protected void ComputeAttack()
    {
        CardManager.Instance.HandleAttack();
        InvocationMenuManager.Instance.UpdateAttackButton();
        HandlePlayerDeath();
    }

    /// <summary>
    /// Check if one of the player die
    /// </summary>
    private void HandlePlayerDeath()
    {
        // Check if one player die
        var playerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
        var opponentPlayerStatus = PlayerManager.Instance.GetOpponentPlayerStatus();
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
        RoundDisplayManager.Instance.SetRoundText(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PHASE_CHOOSE)
        );
    }

    /// <summary>
    /// Do the actual draw
    /// </summary>
    private void DoDraw()
    {
        CardManager.Instance.OnTurnStart();

        void OnNoCards()
        {
            GameOver();
        }

        CardManager.Instance.Draw(OnNoCards);
    }

    /// <summary>
    /// Called when a turn end for a player
    /// </summary>
    protected void EndTurnPhase()
    {
        CardManager.Instance.HandleEndTurn();
        _gameStateService.HandleEndTurn();
        Draw();
    }

    #endregion

}