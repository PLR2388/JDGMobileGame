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

    private void OnBackPressed()
    {
        void PositiveAction()
        {
            SceneLoaderManager.LoadMainScreen();
        }

        UIManager.Instance.DisplayPauseMenu(PositiveAction);
    }

    private void OnReleaseTouch()
    {
        UIManager.Instance.HideBigImage();
    }

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


    private void OnLongTouch()
    {
        UIManager.Instance.HideInvocationMenu();
        var cardTouch = RaycastManager.GetCardTouch();
        if (cardTouch != null)
        {
            UIManager.Instance.DisplayCardInBigImage(cardTouch);
        }
    }

    private void ChoosePhase()
    {
        UIManager.Instance.EnableInvocationMenu();
        ChoosePhaseMusic();
    }

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

    private static void GameOver()
    {
        SceneLoaderManager.LoadMainScreen();
    }

    private void PlayAttackMusic()
    {
      AudioSystem.Instance.PlayMusic(Music.Fight);
    }

    protected void DisplayAvailableOpponent()
    {
        var notEmptyOpponent = CardManager.Instance.BuildInvocationCardsForAttack();
        DisplayOpponentMessageBox(notEmptyOpponent);
        InputManager.Instance.DisableDetectionTouch();
    }

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

    private void Draw()
    {
        DoDraw();
        GameStateManager.Instance.IncrementNumberOfTurn();
        GameStateManager.Instance.NextPhase();
        
        ChoosePhase();
        UIManager.Instance.SetRoundText("Phase de pose");
    }

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
    private void EndTurnPhase()
    {
        CardManager.Instance.HandleEndTurn();
        GameStateManager.Instance.HandleEndTurn();
        Draw();
    }
}