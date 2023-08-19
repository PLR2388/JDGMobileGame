using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour
{
    public static readonly UnityEvent ChangePlayer = new UnityEvent();

    protected const float ClickDuration = 2;
    
    protected bool stopDetectClicking;
    protected bool clicking;
    protected float totalDownTime;

    // Start is called before the first frame update
    protected void Start()
    {
        Draw();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (GameStateManager.Instance.PhaseId)
        {
            case 1:
                SeeCardAndApplyAction();
                break;
            case 2:
                ChooseAttack();
                break;
        }

        // Make sure user is on Android platform
        if (Application.platform != RuntimePlatform.Android) return;
        // Check if Back was pressed this frame
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        void PositiveAction()
        {
            SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
        }

        UIManager.Instance.DisplayPauseMenu(PositiveAction);
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

    protected void SeeCardAndApplyAction()
    {
        if (!Input.GetMouseButton(0) || GameStateManager.Instance.PhaseId != 1 || stopDetectClicking) return;
        var position = InputManager.TouchPosition;
        if (Camera.main == null) return;
        var hit = Physics.Raycast(Camera.main.ScreenPointToRay(position), out var hitInfo);
        if (hit)
        {
            HandleClickDuringDrawPhase(hitInfo);
        }
        else
        {
            UIManager.Instance.HideBigImage();
        }
    }

    protected void ChooseAttack()
    {
        AudioSystem.Instance.PlayMusic(Music.Fight);
        if (!Input.GetMouseButton(0) || GameStateManager.Instance.PhaseId != 2 || stopDetectClicking) return;
        var position = InputManager.TouchPosition;
        if (Camera.main == null) return;
        var hit = Physics.Raycast(Camera.main.ScreenPointToRay(position), out var hitInfo);
        if (hit)
        {
            HandleClick(hitInfo);
        }
        else
        {
            UIManager.Instance.HideBigImage();
        }
    }

    protected void HandleClickDuringDrawPhase(RaycastHit hitInfo)
    {
        var objectTag = hitInfo.transform.gameObject.tag;
        var personTag = CardManager.Instance.GetCurrentPlayerCards().Tag;
        var opponentTag = CardManager.Instance.GetOpponentPlayerCards().Tag;
        var cardObject = hitInfo.transform.gameObject;

        if (objectTag == personTag)
        {
            var cardSelected = cardObject.GetComponent<PhysicalCardDisplay>().card;
            if (Input.GetMouseButtonDown(0))
            {
                totalDownTime = 0;
                clicking = true;
                var mousePosition = InputManager.TouchPosition;
                if (cardSelected is InGameInvocationCard invocationCard)
                {
                    CardManager.Instance.SetAttacker(invocationCard);
                    UIManager.Instance.DisplayInvocationMenu(mousePosition, false, GameStateManager.Instance.IsP1Turn);

                }
            }

            if (clicking && Input.GetMouseButton(0))
            {
                totalDownTime += Time.deltaTime;

                if (totalDownTime >= ClickDuration)
                {
                    Debug.Log("Long click");
                    clicking = false;
                    UIManager.Instance.HideInvocationMenu();
                    UIManager.Instance.DisplayCardInBigImage(cardObject.GetComponent<PhysicalCardDisplay>().card);
                }
            }

            if (clicking && Input.GetMouseButtonUp(0))
            {
                clicking = false;
            }
        }
        else if (objectTag == opponentTag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                totalDownTime = 0;
                clicking = true;
                var opponentInvocationCard = cardObject.GetComponent<PhysicalCardDisplay>().card;
                if (opponentInvocationCard is InGameInvocationCard { IsControlled: true } invocationCard)
                {
                    CardManager.Instance.SetAttacker(invocationCard);
                    var mousePosition = InputManager.TouchPosition;
                    UIManager.Instance.DisplayInvocationMenu(mousePosition, false, GameStateManager.Instance.IsP1Turn);
                }
            }

            if (clicking && Input.GetMouseButton(0))
            {
                totalDownTime += Time.deltaTime;

                if (totalDownTime >= ClickDuration)
                {
                    clicking = false;
                    UIManager.Instance.DisplayCardInBigImage(cardObject.GetComponent<PhysicalCardDisplay>().card);
                }
            }

            if (clicking && Input.GetMouseButtonUp(0))
            {
                clicking = false;
            }
        }
        else
        {
            UIManager.Instance.HideBigImage();
        }
    }

    protected void HandleClick(RaycastHit hitInfo)
    {
        var objectTag = hitInfo.transform.gameObject.tag;

        var ownPlayerCards = CardManager.Instance.GetCurrentPlayerCards();
        var personTag = ownPlayerCards.Tag;
        var opponentTag = CardManager.Instance.GetOpponentPlayerCards().Tag;
        var cardObject = hitInfo.transform.gameObject;

        if (objectTag == personTag)
        {
            var cardSelected = cardObject.GetComponent<PhysicalCardDisplay>().card;
            if (Input.GetMouseButtonDown(0))
            {
                totalDownTime = 0;
                clicking = true;
                var mousePosition = InputManager.TouchPosition;
                if (cardSelected is InGameInvocationCard invocationCard)
                {
                    CardManager.Instance.SetAttacker(invocationCard);
                    UIManager.Instance.DisplayInvocationMenu(mousePosition, true, GameStateManager.Instance.IsP1Turn);
                }
            }

            if (clicking && Input.GetMouseButton(0))
            {
                totalDownTime += Time.deltaTime;

                if (totalDownTime >= ClickDuration)
                {
                    Debug.Log("Long click");
                    clicking = false;
                    UIManager.Instance.HideInvocationMenu();
                    UIManager.Instance.DisplayCardInBigImage(cardObject.GetComponent<PhysicalCardDisplay>().card);
                }
            }

            if (clicking && Input.GetMouseButtonUp(0))
            {
                clicking = false;
            }
        }
        else if (objectTag == opponentTag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                totalDownTime = 0;
                clicking = true;
                var opponentInvocationCard = cardObject.GetComponent<PhysicalCardDisplay>().card;
                if (opponentInvocationCard is InGameInvocationCard { IsControlled: true } invocationCard)
                {
                    var mousePosition = InputManager.TouchPosition;
                    CardManager.Instance.SetAttacker(invocationCard);
                    UIManager.Instance.DisplayInvocationMenu(mousePosition, true, GameStateManager.Instance.IsP1Turn);
                }
            }

            if (clicking && Input.GetMouseButton(0))
            {
                totalDownTime += Time.deltaTime;

                if (totalDownTime >= ClickDuration)
                {
                    clicking = false;
                    UIManager.Instance.DisplayCardInBigImage(cardObject.GetComponent<PhysicalCardDisplay>().card);
                }
            }

            if (clicking && Input.GetMouseButtonUp(0))
            {
                clicking = false;
            }
        }
        else
        {
            UIManager.Instance.HideBigImage();
        }
    }

    protected void DisplayAvailableOpponent()
    {
        var notEmptyOpponent = CardManager.Instance.BuildInvocationCardsForAttack();
        DisplayOpponentMessageBox(notEmptyOpponent);
        stopDetectClicking = true;
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
            stopDetectClicking = false;
        }

        void NegativeAction()
        {
            stopDetectClicking = false;
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

    protected void RemoveCombineEffectCard(List<InGameEffectCard> effectCards,
        ObservableCollection<InGameCard> yellowCards)
    {
        foreach (var effectCard in effectCards.Where(effectCard => effectCard.Title == "Attaque de la tour Eiffel"))
        {
            effectCards.Remove(effectCard);
            yellowCards.Add(effectCard);
            break;
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

        if (GameStateManager.Instance.PhaseId == 1)
        {
            ChoosePhase();
        }

        if (GameStateManager.Instance.PhaseId != 3) return;
        PlayerCards currentPlayerCard = CardManager.Instance.GetCurrentPlayerCards();

        var invocationCards = currentPlayerCard.invocationCards;

        foreach (var invocationCard in invocationCards)
        {
            invocationCard.UnblockAttack();
            invocationCard.incrementNumberTurnOnField();
        }

        GameStateManager.Instance.ToggleTurn();
        ChangePlayer.Invoke();

        GameStateManager.Instance.SetPhase(0);
        Draw();
    }
}