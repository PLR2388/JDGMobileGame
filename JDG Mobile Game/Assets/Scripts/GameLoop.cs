﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
    public static bool IsP1Turn;

    public int phaseId;

    [SerializeField] private GameObject playerText;
    [SerializeField] private GameObject roundText;
    [SerializeField] private TextMeshProUGUI healthP1Text;
    [SerializeField] private TextMeshProUGUI healthP2Text;

    [SerializeField] private GameObject bigImageCard;
    [SerializeField] private GameObject invocationMenu;
    [SerializeField] private GameObject nextPhaseButton;
    [SerializeField] private Transform canvas;

    [FormerlySerializedAs("P1")] [SerializeField]
    private GameObject p1;

    [FormerlySerializedAs("P2")] [SerializeField]
    private GameObject p2;

    [SerializeField] private Card player;
    [SerializeField] private GameObject inHandButton;

    [FormerlySerializedAs("camera")] [SerializeField]
    private GameObject playerCamera;

    public static readonly UnityEvent ChangePlayer = new UnityEvent();

    private InvocationFunctions invocationFunctions;



    private readonly float ClickDuration = 2;

    private bool stopDetectClicking = false;
    private bool clicking = false;
    private float totalDownTime = 0;
    private Card cardSelected;
    private InvocationCard attacker;
    private int numberOfTurn = 0;
    private string endGameReason = "";

    private readonly Vector3 cameraRotation = new Vector3(0, 0, 180);

    // Start is called before the first frame update
    private void Start()
    {
        invocationFunctions = GetComponent<InvocationFunctions>();
        IsP1Turn = true;
        ChangeHealthText(PlayerStatus.MAXPv,true);
        ChangeHealthText(PlayerStatus.MAXPv,false);
        PlayerStatus.ChangePvEvent.AddListener(ChangeHealthText);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (phaseId)
        {
            case 0:
                Draw();
                break;
            case 1:
                ChoosePhase();
                break;
            case 2:
                ChooseAttack();
                break;
            case 5:
                GameOver();
                break;
        }
    }

    private void ChoosePhase()
    {
        if (IsP1Turn)
        {
            invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
            p1.GetComponent<PlayerCards>().ResetInvocationCardNewTurn();
        }
        else
        {
            invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
            p2.GetComponent<PlayerCards>().ResetInvocationCardNewTurn();
        }
    }

    private void GameOver()
    {
        MessageBox[] components = FindObjectsOfType<MessageBox>();
        if (components.Length == 0)
        {
            var messageBox = MessageBox.CreateOkMessageBox(canvas, "Fin de partie", endGameReason);
            messageBox.GetComponent<MessageBox>().OkAction =() =>
            {
                Destroy(messageBox);
                SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
            };
        }
    }

    private void ChangeHealthText(float pv, bool isP1)
    {
        if (isP1)
        {
            healthP1Text.SetText(pv + "/" + PlayerStatus.MAXPv);
        }
        else
        {
            healthP2Text.SetText(pv + "/" + PlayerStatus.MAXPv);
        }
    }

    private void ChooseAttack()
    {
        if (!Input.GetMouseButton(0) || phaseId != 2 || stopDetectClicking) return;
#if UNITY_EDITOR
        var position = Input.mousePosition;
#elif UNITY_ANDROID
        var position = Input.GetTouch(0).position;
#endif
        var hit = Physics.Raycast(Camera.main.ScreenPointToRay(position), out var hitInfo);
        if (hit)
        {
            HandleClick(hitInfo);
        }
        else
        {
            bigImageCard.SetActive(false);
        }
    }

    private void HandleClick(RaycastHit hitInfo)
    {
        var objectTag = hitInfo.transform.gameObject.tag;
        var ownPlayerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var personTag = IsP1Turn ? p1.GetComponent<PlayerCards>().Tag : p2.GetComponent<PlayerCards>().Tag;
        var opponentTag = IsP1Turn ? p2.GetComponent<PlayerCards>().Tag : p1.GetComponent<PlayerCards>().Tag;
        var cardObject = hitInfo.transform.gameObject;

        if (objectTag == personTag)
        {
            cardSelected = cardObject.GetComponent<PhysicalCardDisplay>().card;
            if (Input.GetMouseButtonDown(0))
            {
                totalDownTime = 0;
                clicking = true;
                var mousePosition = Input.mousePosition;
                if (cardSelected is InvocationCard invocationCard)
                {
                    attacker = invocationCard;
                    var canAttack = attacker.CanAttack() && ownPlayerCards.ContainsCardInInvocation(attacker);
                    var hasAction = attacker.InvocationActionEffect != null;
                    if (canAttack || hasAction)
                    {
                        invocationMenu.SetActive(true);
                        invocationMenu.transform.position = mousePosition;
                        invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = canAttack;
                        if (hasAction) {
                                invocationMenu.transform.GetChild(1).gameObject.SetActive(true);
                            invocationMenu.transform.GetChild(1).GetComponent<Button>().interactable = IsSpecialActionPossible();
                        } else {
                            invocationMenu.transform.GetChild(1).gameObject.SetActive(false);
                        }
                       
                    }
                }
            }

            if (clicking && Input.GetMouseButton(0))
            {
                totalDownTime += Time.deltaTime;

                if (totalDownTime >= ClickDuration)
                {
                    Debug.Log("Long click");
                    clicking = false;
                    invocationMenu.SetActive(false);
                    DisplayCurrentCard(cardObject.GetComponent<PhysicalCardDisplay>().card);
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
            }

            if (clicking && Input.GetMouseButton(0))
            {
                totalDownTime += Time.deltaTime;

                if (totalDownTime >= ClickDuration)
                {
                    clicking = false;
                    DisplayCurrentCard(cardObject.GetComponent<PhysicalCardDisplay>().card);
                }
            }

            if (clicking && Input.GetMouseButtonUp(0))
            {
                clicking = false;
            }
        }
        else
        {
            bigImageCard.SetActive(false);
        }
    }

    public void DisplayAvailableOpponent()
    {
        if (IsP1Turn)
        {
            var opponentCards = p2.GetComponent<PlayerCards>().invocationCards;
            DisplayCards(opponentCards);
        }
        else
        {
            List<InvocationCard> opponentCards = p1.GetComponent<PlayerCards>().invocationCards;
            DisplayCards(opponentCards);
        }
    }

    private bool IsSpecialActionPossible() {
        return invocationFunctions.IsSpecialActionPossible(attacker, attacker.InvocationActionEffect);
    }

    public void UseSpecialAction()
    {
        invocationFunctions.AskIfUserWantToUseActionEffect(attacker, attacker.InvocationActionEffect);
    }

    private void DisplayCards(List<InvocationCard> invocationCards)
    {
        var notEmptyOpponent = invocationCards.Where(t => t != null && t.Nom != null).Cast<Card>().ToList();

        if (notEmptyOpponent.Count == 0)
        {
            notEmptyOpponent.Add(player);
        }

        var opponentMessageBox = DisplayOpponentMessageBox(notEmptyOpponent);
        stopDetectClicking = true;
    }

    private GameObject DisplayOpponentMessageBox(List<Card> invocationCards)
    {
        nextPhaseButton.SetActive(false);

        var message = MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choisis ton adversaire :", invocationCards);
        message.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var invocationCard =
                (InvocationCard)message.GetComponent<MessageBox>().GETSelectedCard();

            if (invocationCard != null)
            {
                ComputeAttack(invocationCard);
            }

            stopDetectClicking = false;
            nextPhaseButton.SetActive(true);
            Destroy(message);
        };
        message.GetComponent<MessageBox>().NegativeAction = () =>
        {
            nextPhaseButton.SetActive(true);
            stopDetectClicking = false;
            Destroy(message);
        };
        return message;
    }

    private void ComputeAttack(InvocationCard opponent)
    {
        var attack = attacker.GetCurrentAttack();
        var defenseOpponent = opponent.GetCurrentDefense();

        var diff = defenseOpponent - attack;

        attacker.AttackTurnDone();
        invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = false;

        if (opponent.Nom == "Player")
        {
            // Directly attack the player
            if (IsP1Turn)
            {
                p2.GetComponent<PlayerStatus>().ChangePv(diff);
            }
            else
            {
                p1.GetComponent<PlayerStatus>().ChangePv(diff);
            }
        }
        else
        {
            if (diff > 0)
            {
                DealWithHurtAttack(diff);
            }
            else if (diff == 0)
            {
                DealWithEqualityAttack(opponent);
            }
            else
            {
                DealWithGoodAttack(opponent, diff);
            }
        }

        // Check if one player die
        var p1Pv = p1.GetComponent<PlayerStatus>().GETCurrentPv();
        var p2Pv = p2.GetComponent<PlayerStatus>().GETCurrentPv();

        if (p1Pv <= 0)
        {
            endGameReason = "Joueur 2 a gagné la partie";
            phaseId = 5;
        }
        else if (p2Pv <= 0)
        {
            endGameReason = "Joueur 1 a gagné la partie";
            phaseId = 5;
        }
    }

    /**
     * Attack that kill the opponent
     */
    private void DealWithGoodAttack(InvocationCard opponent, float diff)
    {
        opponent.IncrementNumberDeaths();
        if (IsP1Turn)
        {
            p2.GetComponent<PlayerStatus>().ChangePv(diff);

            if (opponent.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(opponent, false);
                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(opponent);
            }
            else
            {
                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(opponent);
            }
        }
        else
        {
            p1.GetComponent<PlayerStatus>().ChangePv(diff);
            if (opponent.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(opponent, true);
                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(opponent);
            }
            else
            {
                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(opponent);
            }
        }
    }

    /**
     * Attack that kill the attacker
     */
    private void DealWithHurtAttack(float diff)
    {
        attacker.IncrementNumberDeaths();
        if (IsP1Turn)
        {
            p1.GetComponent<PlayerStatus>().ChangePv(-diff);
            if (attacker.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(attacker, true);
                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(cardSelected);
            }
            else
            {
                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(cardSelected);
            }
        }
        else
        {
            p2.GetComponent<PlayerStatus>().ChangePv(-diff);
            if (attacker.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(attacker, false);
                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(cardSelected);
            }
            else
            {
                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(cardSelected);
            }
        }
    }

    private void DealWithEqualityAttack(InvocationCard opponent)
    {
        attacker.IncrementNumberDeaths();
        opponent.IncrementNumberDeaths();
        if (IsP1Turn)
        {
            if (attacker.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(attacker, true);
                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(attacker);
            }
            else
            {
                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(attacker);
            }

            if (opponent.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(opponent, false);
                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(opponent);
            }
            else
            {
                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(opponent);
            }
        }
        else
        {
            if (attacker.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(attacker, false);
                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(attacker);
            }
            else
            {
                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(attacker);
            }

            if (opponent.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(opponent, true);
                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(opponent);
            }
            else
            {
                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(opponent);
            }
        }
    }

    /**
     * invocationCard = card that's going to die
     * attacker = the opponent
     */
    private void DealWithDeathEffect(InvocationCard invocationCard, bool isP1Card,
        InvocationCard attackerInvocationCard = null)
    {
        var invocationDeathEffect = invocationCard.GetInvocationDeathEffect();
        var keys = invocationDeathEffect.Keys;
        var values = invocationDeathEffect.Values;

        var cardName = "";
        for (var i = 0; i < keys.Count; i++)
        {
            switch (keys[i])
            {
                case DeathEffect.GetSpecificCard:
                    cardName = values[i];
                    break;
                case DeathEffect.GetCardSource:
                    GetCardSourceDeathEffect(invocationCard, isP1Card, values, i, cardName);
                    break;
                case DeathEffect.ComeBackToHand:
                    ComeBackToHandDeathEffect(invocationCard, isP1Card, values, i);
                    break;
                case DeathEffect.KillAlsoOtherCard:
                    KillAlsoOtherCardDeathEffect(invocationCard, attackerInvocationCard);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void KillAlsoOtherCardDeathEffect(Card invocationCard, Card attackerInvocationCard)
    {
        if (attackerInvocationCard != null)
        {
            if (IsP1Turn)
            {
                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(attackerInvocationCard);
                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(invocationCard);
            }
            else
            {
                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(invocationCard);
                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(attackerInvocationCard);
            }
        }
    }

    private void ComeBackToHandDeathEffect(InvocationCard invocationCard, bool isP1Card, IReadOnlyList<string> values,
        int i)
    {
        var number = int.Parse(values[i]);
        if (number != 0)
        {
            if (invocationCard.GETNumberDeaths() > number)
            {
                if (isP1Card)
                {
                    p1.GetComponent<PlayerCards>().SendCardToYellowTrash(invocationCard);
                }
                else
                {
                    p2.GetComponent<PlayerCards>().SendCardToYellowTrash(invocationCard);
                }
            }
            else
            {
                if (isP1Card)
                {
                    p1.GetComponent<PlayerCards>().SendCardToHand(invocationCard);
                }
                else
                {
                    p2.GetComponent<PlayerCards>().SendCardToHand(invocationCard);
                }
            }
        }
        else
        {
            if (isP1Card)
            {
                p1.GetComponent<PlayerCards>().SendCardToHand(invocationCard);
            }
            else
            {
                p2.GetComponent<PlayerCards>().SendCardToHand(invocationCard);
            }
        }
    }

    private void GetCardSourceDeathEffect(Card invocationCard, bool isP1Card, IReadOnlyList<string> values, int i,
        string cardName)
    {
        Card cardFound = null;
        var source = values[i];
        PlayerCards currentPlayerCard = null;
        currentPlayerCard = isP1Card ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();

        currentPlayerCard.SendCardToYellowTrash(invocationCard);
        switch (source)
        {
            case "deck":
            {
                var deck = currentPlayerCard.deck;
                if (cardName != "")
                {
                    var isFound = false;
                    var j = 0;
                    while (j < deck.Count && !isFound)
                    {
                        if (deck[j].Nom == cardName)
                        {
                            isFound = true;
                            cardFound = deck[j];
                        }

                        j++;
                    }

                    AskUserToAddCardInHand(cardName, cardFound, isFound, currentPlayerCard);
                }

                break;
            }
            case "trash":
            {
                var trash = currentPlayerCard.yellowTrash;
                if (cardName != "")
                {
                    var isFound = false;
                    var j = 0;
                    while (j < trash.Count && !isFound)
                    {
                        if (trash[j].Nom == cardName)
                        {
                            isFound = true;
                            cardFound = trash[j];
                        }

                        j++;
                    }

                    AskUserToAddCardInHand(cardName, cardFound, isFound, currentPlayerCard);
                }

                break;
            }
        }
    }

    private void AskUserToAddCardInHand(string cardName, Card cardFound, bool isFound, PlayerCards currentPlayerCard)
    {
        if (!isFound) return;

        nextPhaseButton.SetActive(false);
        inHandButton.SetActive(false);

        UnityAction positiveAction = () =>
        {
            currentPlayerCard.handCards.Add(cardFound);
            currentPlayerCard.deck.Remove(cardFound);
            nextPhaseButton.SetActive(true);
            inHandButton.SetActive(true);
        };

        UnityAction negativeAction = () =>
        {
            nextPhaseButton.SetActive(true);
            inHandButton.SetActive(true);
        };

        MessageBox.CreateSimpleMessageBox(canvas, "Carte en main",
            "Voulez-vous aussi ajouter " + cardName + " à votre main ?", positiveAction, negativeAction);
        /*var message = Instantiate(messageBox);
        message.GetComponent<MessageBox>().title = "Carte en main";

        message.GetComponent<MessageBox>().description =
            "Voulez-vous aussi ajouter " + cardName + " à votre main ?";
        message.GetComponent<MessageBox>().PositiveAction = () =>
        {
            currentPlayerCard.handCards.Add(cardFound);

            currentPlayerCard.deck.Remove(cardFound);

            Destroy(message);
        };
        message.GetComponent<MessageBox>().NegativeAction = () => { Destroy(message); };*/
    }

    private void DisplayCurrentCard(Card card)
    {
        bigImageCard.SetActive(true);
        bigImageCard.GetComponent<Image>().material = card.MaterialCard;
    }

    private void Draw()
    {
        if (IsP1Turn)
        {
            var p1Cards = p1.GetComponent<PlayerCards>();
            var size = p1Cards.deck.Count;
            if (size > 0)
            {
                var c = p1Cards.deck[size - 1];
                p1Cards.handCards.Add(c);
                p1Cards.deck.RemoveAt(size - 1);
            }
            else
            {
                var p1Pv = p1.GetComponent<PlayerStatus>().GETCurrentPv();
                var p2Pv = p2.GetComponent<PlayerStatus>().GETCurrentPv();
                if (p1Pv > p2Pv)
                {
                    endGameReason = "Joueur 1 n'a plus de cartes. Joueur 1 gagne la partie !";
                }
                else
                {
                    endGameReason = "Joueur 1 n'a plus de cartes. Joueur 2 gagne la partie !";
                }

                phaseId = 5;
            }

            var invocationCards = p1Cards.invocationCards;
            foreach (var invocationCard in invocationCards)
            {
                invocationCard.UnblockAttack();
            }

            numberOfTurn++;
        }
        else
        {
            var p2Cards = p2.GetComponent<PlayerCards>();
            var size = p2Cards.deck.Count;
            if (size > 0)
            {
                var c = p2Cards.deck[size - 1];
                p2Cards.handCards.Add(c);
                p2Cards.deck.RemoveAt(size - 1);
            }
            else
            {
                var p1Pv = p1.GetComponent<PlayerStatus>().GETCurrentPv();
                var p2Pv = p2.GetComponent<PlayerStatus>().GETCurrentPv();
                if (p1Pv > p2Pv)
                {
                    endGameReason = "Joueur 2 n'a plus de cartes. Joueur 1 gagne la partie !";
                }
                else
                {
                    endGameReason = "Joueur 2 n'a plus de cartes. Joueur 2 gagne la partie !";
                }

                phaseId = 5;
            }

            var invocationCards = p2Cards.invocationCards;
            foreach (var invocationCard in invocationCards)
            {
                invocationCard.UnblockAttack();
            }
        }

        phaseId += 1;
        roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pose";
    }

    public void NextRound()
    {
        invocationMenu.SetActive(false);
        if (numberOfTurn == 1 && IsP1Turn)
        {
            phaseId = 3;
        }
        else
        {
            phaseId += 1;
        }

        switch (phaseId)
        {
            case 3:
                inHandButton.SetActive(true);
                roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pioche";
                break;
            case 1:
                inHandButton.SetActive(true);
                roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pose";
                break;
            case 2:
                inHandButton.SetActive(false);
                roundText.GetComponent<TextMeshProUGUI>().text = "Phase d'attaque";
                break;
        }

        if (phaseId != 3) return;
        PlayerCards currentPlayerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        List<EffectCard> effectCards = currentPlayerCard.effectCards;
        foreach(EffectCard effectCard in effectCards) {
            if (effectCard.GetLifeTime() == 1) {
                currentPlayerCard.yellowTrash.Add(effectCard);
                currentPlayerCard.effectCards.Remove(effectCard);
            } else if(effectCard.GetLifeTime() > 1) {
                effectCard.DecrementLifeTime();
            }
        }


        IsP1Turn = !IsP1Turn;
        ChangePlayer.Invoke();
        if (IsP1Turn)
        {
            playerCamera.transform.Rotate(cameraRotation);
            playerText.GetComponent<TextMeshProUGUI>().text = "Joueur 1";
        }
        else
        {
            playerCamera.transform.Rotate(cameraRotation);
            playerText.GetComponent<TextMeshProUGUI>().text = "Joueur 2";
        }

        phaseId = 0;
    }
}