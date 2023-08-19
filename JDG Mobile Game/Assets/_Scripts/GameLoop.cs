using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
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
            switch (GameStateManager.Instance.PhaseId)
            {
                case 1:
                {
                    HandleSingleTouch(cardTouch, currentOwner, false);
                }
                    break;
                case 2:
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

    protected void ChoosePhase()
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
            switch (currentFieldCard.GetFamily())
            {
                case CardFamily.Comics:
                    AudioSystem.Instance.PlayMusic(Music.CanardCity);
                    break;
                case CardFamily.Rpg:
                    AudioSystem.Instance.PlayMusic(Music.Rpg);
                    break;
                case CardFamily.Wizard:
                    AudioSystem.Instance.PlayMusic(Music.Wizard);
                    break;
                default:
                    AudioSystem.Instance.PlayMusic(Music.DrawPhase);
                    break;
            }
        }
    }

    protected static void GameOver()
    {
        SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
    }

    protected void PlayAttackMusic()
    {
      AudioSystem.Instance.PlayMusic(Music.Fight);
    }

    protected void DisplayAvailableOpponent()
    {
        var notEmptyOpponent = CardManager.Instance.BuildInvocationCardsForAttack();
        DisplayOpponentMessageBox(notEmptyOpponent);
        InputManager.Instance.DisableDetectionTouch();
    }

    protected void DisplayOpponentMessageBox(List<InGameCard> invocationCards)
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

    protected void ComputeAttack()
    {
        CardManager.Instance.HandleAttack();
        UIManager.Instance.UpdateAttackButton(GameStateManager.Instance.IsP1Turn);

        // Check if one player die

        var playerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
        var opponentPlayerStatus = PlayerManager.Instance.GetOpponentPlayerStatus();
        if (playerStatus.GetCurrentPv() <= 0)
        {
            GameStateManager.Instance.SetPhase(5);
            GameOver();
        }
        else if (opponentPlayerStatus.GetCurrentPv() <= 0)
        {
            GameStateManager.Instance.SetPhase(5);
            GameOver();
        }
    }

    protected void Draw()
    {
        DoDraw();
        GameStateManager.Instance.IncrementNumberOfTurn();
        GameStateManager.Instance.SetPhase(GameStateManager.Instance.PhaseId + 1);
        UIManager.Instance.SetRoundText("Phase de pose");
    }

    private void DoDraw()
    {
        CardManager.Instance.OnTurnStart();

        void OnNoCards()
        {
            GameStateManager.Instance.SetPhase(5);
            GameOver();
        }

        CardManager.Instance.Draw(OnNoCards);
    }

    protected virtual void NextRound()
    {
        UIManager.Instance.HideInvocationMenu();
        if (GameStateManager.Instance.NumberOfTurn == 1 && GameStateManager.Instance.IsP1Turn)
        {
            GameStateManager.Instance.SetPhase(3);
        }
        else
        {
            GameStateManager.Instance.SetPhase(GameStateManager.Instance.PhaseId + 1);
        }

        var playerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
        if (GameStateManager.Instance.PhaseId == 2 && playerStatus.BlockAttack)
        {
            GameStateManager.Instance.SetPhase(3);
        }

        UIManager.Instance.AdaptUIToPhaseIdInNextRound(GameStateManager.Instance.PhaseId);

        switch (GameStateManager.Instance.PhaseId) 
        {
            case 1:
                ChoosePhase();
                break;
            case 2:
                PlayAttackMusic();
                break;
            case 3:
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