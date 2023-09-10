using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using Sound;
using UnityEngine;

public class GameLoop : MonoBehaviour
{

    // Start is called before the first frame update
    protected void Start()
    {
        InputManager.OnLongTouch.AddListener(OnLongTouch);
        InputManager.OnTouch.AddListener(OnTouch);
        InputManager.OnReleaseTouch.AddListener(OnReleaseTouch);
        InputManager.OnBackPressed.AddListener(OnBackPressed);
        Draw();
    }

    private void OnDestroy()
    {
        InputManager.OnLongTouch.RemoveListener(OnLongTouch);
        InputManager.OnTouch.RemoveListener(OnTouch);
        InputManager.OnReleaseTouch.RemoveListener(OnReleaseTouch);
        InputManager.OnBackPressed.RemoveListener(OnBackPressed);
    }

    #region UI Interaction

    /// <summary>
    /// Action when back is pressed
    /// </summary>
    private void OnBackPressed()
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
    private void OnReleaseTouch()
    {
        UIManager.Instance.HideBigImage();
    }

    /// <summary>
    /// When user just touches the screen
    /// </summary>
    private void OnTouch()
    {
        var cardTouch = CardRaycastManager.Instance.GetTouchedCard();
        var currentOwner = GameStateManager.Instance.IsP1Turn ? CardOwner.Player1 : CardOwner.Player2;
        if (cardTouch != null)
        {
            switch (GameStateManager.Instance.Phase)
            {
                case Phase.Choose:
                {
                    HandleSingleTouch(cardTouch, currentOwner, false);
                }
                    break;
                case Phase.Attack:
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
    private static void HandleSingleTouch(InGameCard cardTouch, CardOwner currentOwner, bool isAttackPhase)
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
    private void OnLongTouch()
    {
        InvocationMenuManager.Instance.Hide();
        var cardTouch = CardRaycastManager.Instance.GetTouchedCard();
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
        if (GameStateManager.Instance.NumberOfTurn == 1 && GameStateManager.Instance.IsP1Turn)
        {
            GameStateManager.Instance.SetPhase(Phase.End);
        }
        else
        {
            GameStateManager.Instance.NextPhase();
        }

        var playerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
        if (GameStateManager.Instance.Phase == Phase.Attack && playerStatus.BlockAttack)
        {
            GameStateManager.Instance.SetPhase(Phase.End);
        }

        RoundDisplayManager.Instance.AdaptUIToPhaseIdInNextRound();

        switch (GameStateManager.Instance.Phase)
        {
            case Phase.Attack:
                PlayAttackMusic();
                break;
            case Phase.End:
                EndTurnPhase();
                break;
        }
    }

    #endregion

    #region Phase Behavior

    /// <summary>
    /// Called when choose phase starts
    /// </summary>
    private void ChoosePhase()
    {
        InvocationMenuManager.Instance.Enable();
        ChoosePhaseMusic();
    }

    /// <summary>
    /// Choose the right Choose music
    /// </summary>
    private void ChoosePhaseMusic()
    {
        var currentFieldCard = CardManager.Instance.GetCurrentPlayerCards().FieldCard;
        if (currentFieldCard == null)
        {
            AudioSystem.Instance.PlayMusic(Music.DrawPhase);
        }
        else
        {
            AudioSystem.Instance.PlayFamilyMusic(currentFieldCard.GetFamily());
        }
    }

    /// <summary>
    /// Redirect player after a Gameover
    /// </summary>
    private static void GameOver()
    {
        GameStateManager.Instance.SetPhase(Phase.GameOver);
        SceneLoaderSystem.LoadMainScreen();
    }

    /// <summary>
    /// Play the attack music
    /// </summary>
    private void PlayAttackMusic()
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
    private void ComputeAttack()
    {
        CardManager.Instance.HandleAttack();
        InvocationMenuManager.Instance.UpdateAttackButton();
        HandlePlayerDeath();
    }

    /// <summary>
    /// Check if one of the player die
    /// </summary>
    private static void HandlePlayerDeath()
    {
        // Check if one player die
        var playerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
        var opponentPlayerStatus = PlayerManager.Instance.GetOpponentPlayerStatus();
        if (playerStatus.GetCurrentPv() <= 0)
        {
            GameOver();
        }
        else if (opponentPlayerStatus.GetCurrentPv() <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Draw a card during draw phase
    /// </summary>
    private void Draw()
    {
        DoDraw();
        GameStateManager.Instance.IncrementNumberOfTurn();
        GameStateManager.Instance.NextPhase();

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
    private void EndTurnPhase()
    {
        CardManager.Instance.HandleEndTurn();
        GameStateManager.Instance.HandleEndTurn();
        Draw();
    }

    #endregion

}