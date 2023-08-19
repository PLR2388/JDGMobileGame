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

    /// <summary>
    /// Action when back is pressed
    /// </summary>
    private void OnBackPressed()
    {
        void PositiveAction()
        {
            SceneLoaderManager.LoadMainScreen();
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
        var cardTouch = RaycastManager.GetCardTouch();
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
                CardManager.Instance.SetAttacker(invocationCard);
                UIManager.Instance.DisplayInvocationMenu(isAttackPhase);
            }
        }
    }

    /// <summary>
    /// Called when user touches during a long time
    /// </summary>
    private void OnLongTouch()
    {
        UIManager.Instance.HideInvocationMenu();
        var cardTouch = RaycastManager.GetCardTouch();
        if (cardTouch != null)
        {
            UIManager.Instance.DisplayCardInBigImage(cardTouch);
        }
    }

    /// <summary>
    /// Called when choose phase starts
    /// </summary>
    private void ChoosePhase()
    {
        UIManager.Instance.EnableInvocationMenu();
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
        SceneLoaderManager.LoadMainScreen();
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
                CardManager.Instance.SetOpponent(invocationCard);
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
        UIManager.Instance.UpdateAttackButton(GameStateManager.Instance.IsP1Turn);

        // Check if one player die

        var playerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
        var opponentPlayerStatus = PlayerManager.Instance.GetOpponentPlayerStatus();
        if (playerStatus.GetCurrentPv() <= 0)
        {
            GameStateManager.Instance.SetPhase(Phase.GameOver);
            GameOver();
        }
        else if (opponentPlayerStatus.GetCurrentPv() <= 0)
        {
            GameStateManager.Instance.SetPhase(Phase.GameOver);
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
        UIManager.Instance.SetRoundText("Phase de pose");
    }

    /// <summary>
    /// Do the actual draw
    /// </summary>
    private void DoDraw()
    {
        CardManager.Instance.OnTurnStart();

        void OnNoCards()
        {
            GameStateManager.Instance.SetPhase(Phase.GameOver);
            GameOver();
        }

        CardManager.Instance.Draw(OnNoCards);
    }

    /// <summary>
    /// Called when user presses the next phase button
    /// </summary>
    protected virtual void NextRound()
    {
        UIManager.Instance.HideInvocationMenu();
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

        UIManager.Instance.AdaptUIToPhaseIdInNextRound();

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
    
    /// <summary>
    /// Called when a turn end for a player
    /// </summary>
    private void EndTurnPhase()
    {
        CardManager.Instance.HandleEndTurn();
        GameStateManager.Instance.HandleEndTurn();
        Draw();
    }
}