using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using Cards.InvocationCards;
using OnePlayer.DialogueBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OnePlayer
{
    public class TutoPlayerGameLoop : GameLoop
    {
        [SerializeField] private GameObject tutoImage;
        [SerializeField] private GameObject tutoVideo;

        private ActionScenario[] actionScenarios;

        private void Awake()
        {
            // The opponent is player2 (only the AI attacks the player directly)
            player = InGameCard.CreateInGameCard(playerInvocationCard, CardOwner.Player2);
            actionScenarios = GetComponent<ScenarioDecoder>().Scenario.actionScenarios;
            DialogueUI.DialogIndex.AddListener(TriggerScenarioAction);
        }

        private void TriggerScenarioAction(int index)
        {
            try
            {
                var actionScenario = actionScenarios.First(elt => elt.Index == index);
                var highlight = actionScenario.Highlight;
                var putCard = actionScenario.PutCard;
                var image = actionScenario.Image;
                var video = actionScenario.Video;
                var attack = actionScenario.Attack;
                var action = actionScenario.Action;

                UnsetHighligh();
                switch (highlight)
                {
                    case Highlight.space:
                        HighLightPlane.Highlight.Invoke(HighlightElement.Space, true);
                        break;
                    case Highlight.deck:
                        HighLightPlane.Highlight.Invoke(HighlightElement.Deck, true);
                        break;
                    case Highlight.yellow_trash:
                        HighLightPlane.Highlight.Invoke(HighlightElement.YellowTrash, true);
                        break;
                    case Highlight.field:
                        HighLightPlane.Highlight.Invoke(HighlightElement.Field, true);
                        break;
                    case Highlight.invocation_cards:
                        HighLightPlane.Highlight.Invoke(HighlightElement.Invocations, true);
                        break;
                    case Highlight.effect_cards:
                        HighLightPlane.Highlight.Invoke(HighlightElement.Effect, true);
                        break;
                    case Highlight.hand_cards:
                        HighLightPlane.Highlight.Invoke(HighlightElement.InHandButton, true);
                        break;
                    case Highlight.next_phase:
                        HighLightPlane.Highlight.Invoke(HighlightElement.NextPhaseButton, true);
                        break;
                    case Highlight.tentacules:
                        HighLightPlane.Highlight.Invoke(HighlightElement.Tentacules, true);
                        break;
                    case Highlight.life_point:
                        HighLightPlane.Highlight.Invoke(HighlightElement.LifePoints, true);
                        break;
                    case Highlight.unknown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                tutoImage.SetActive(image != null);
                tutoVideo.SetActive(video != null);

                if (putCard != null)
                {
                    if (putCard.Contains(">"))
                    {
                        var cardNames = putCard.Split('>');
                        PlayerCards playerCards = p1.GetComponent<PlayerCards>();
                        InGameEquipementCard equipmentCard =
                            playerCards.handCards.Find(elt => elt.Title == cardNames[0]) as InGameEquipementCard;
                        InGameInvocationCard invocationCard =
                            playerCards.invocationCards.First(elt => elt.Title == cardNames[1]);

                        if (equipmentCard != null)
                        {
                            var instantEffect = equipmentCard.EquipmentInstantEffect;
                            var permEffect = equipmentCard.EquipmentPermEffect;

                            if (instantEffect != null)
                            {
                                EquipmentFunctions.DealWithInstantEffect(invocationCard, instantEffect);
                            }

                            if (permEffect != null)
                            {
                                EquipmentFunctions.DealWithPermEffect(invocationCard, permEffect);
                            }
                        }

                        invocationCard.SetEquipmentCard(equipmentCard);
                        playerCards.handCards.Remove(equipmentCard);
                    }
                    else
                    {
                        var cardNames = putCard.Split(';');
                        PlayerCards playerCards = p1.GetComponent<PlayerCards>();
                        foreach (var name in cardNames)
                        {
                            InGameCard card = playerCards.handCards.Find(elt => elt.Title == name);
                            if (card is InGameInvocationCard invocationCard)
                            {
                                playerCards.handCards.Remove(card);
                                playerCards.invocationCards.Add(invocationCard);
                            }
                            else if (card is InGameFieldCard fieldCard)
                            {
                                playerCards.field = fieldCard;
                                playerCards.handCards.Remove(fieldCard);
                                FieldFunctions.ApplyFieldCardEffect(fieldCard, playerCards);
                            }
                        }
                    }
                }

                if (attack != null)
                {
                    string attacker = attack[0];
                    string defender = "player";
                    if (attack.Length > 1)
                    {
                        defender = attack[1] == "" ? "player" : attack[1];
                    }

                    InGameInvocationCard attackerInvocationCard =
                        p1.GetComponent<PlayerCards>().invocationCards.First(card => card.Title == attacker);

                    InGameInvocationCard opponentInvocationCard;

                    if (defender == "player")
                    {
                        opponentInvocationCard = player as InGameInvocationCard;
                    }
                    else
                    {
                        opponentInvocationCard = p2.GetComponent<PlayerCards>().invocationCards
                            .First(card => card.Title == defender);
                    }

                    this.attacker = attackerInvocationCard;
                    opponent = opponentInvocationCard;
                    ComputeAttack();

                    if (defender == "player")
                    {
                        HighLightPlane.Highlight.Invoke(HighlightElement.InHandButton, true);
                    }
                }

                switch (action)
                {
                    case Action.next_phase:
                        NextRound();
                        break;
                    case Action.unknown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                UnsetHighligh();
                Console.WriteLine(e);
            }
        }

        private void UnsetHighligh()
        {
            HighLightPlane.Highlight.Invoke(HighlightElement.Space, false);
            HighLightPlane.Highlight.Invoke(HighlightElement.Deck, false);
            HighLightPlane.Highlight.Invoke(HighlightElement.YellowTrash, false);
            HighLightPlane.Highlight.Invoke(HighlightElement.Field, false);
            HighLightPlane.Highlight.Invoke(HighlightElement.Invocations, false);
            HighLightPlane.Highlight.Invoke(HighlightElement.Effect, false);
            HighLightPlane.Highlight.Invoke(HighlightElement.InHandButton, false);
            HighLightPlane.Highlight.Invoke(HighlightElement.NextPhaseButton, false);
            HighLightPlane.Highlight.Invoke(HighlightElement.Tentacules, false);
            HighLightPlane.Highlight.Invoke(HighlightElement.LifePoints, false);
        }

        // Update is called once per frame
        private void Update()
        {
            if (phaseId == 1 && numberOfTurn == 2 && p2.GetComponent<PlayerCards>().invocationCards.Count == 2)
            {
                nextPhaseButton.GetComponent<HighLightButton>().isActivated = true;
            }

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

            // Make sure user is on Android platform
            if (Application.platform != RuntimePlatform.Android) return;
            // Check if Back was pressed this frame
            if (!Input.GetKeyDown(KeyCode.Escape)) return;

            void PositiveAction()
            {
                SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
            }

            MessageBox.CreateSimpleMessageBox(canvas, "Pause", "Veux-tu quitter la partie ?", PositiveAction);
        }

        protected override void HandleClick(RaycastHit hitInfo)
        {
            var objectTag = hitInfo.transform.gameObject.tag;
            var ownPlayerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            var personTag = IsP1Turn ? p1.GetComponent<PlayerCards>().Tag : p2.GetComponent<PlayerCards>().Tag;
            var opponentTag = IsP1Turn ? p2.GetComponent<PlayerCards>().Tag : p1.GetComponent<PlayerCards>().Tag;
            var cardObject = hitInfo.transform.gameObject;

            if (cardObject.GetComponent<PhysicalCardDisplay>() == null) return;
            var card = cardObject.GetComponent<PhysicalCardDisplay>().card;
            if (card.Title != "Tentacules") return;
            if (objectTag == personTag)
            {
                cardSelected = cardObject.GetComponent<PhysicalCardDisplay>().card;
                var isInYellowTrash = ownPlayerCards.yellowCards.Contains(cardSelected);
                if (Input.GetMouseButtonDown(0) && !isInYellowTrash)
                {
                    totalDownTime = 0;
                    clicking = true;
#if UNITY_EDITOR
                    var mousePosition = Input.mousePosition;
#elif UNITY_ANDROID
                var mousePosition = Input.GetTouch(0).position;
#endif
                    if (cardSelected is InGameInvocationCard invocationCard)
                    {
                        attacker = invocationCard;
                        var canAttack = attacker.CanAttack() && ownPlayerCards.ContainsCardInInvocation(attacker);
                        //var hasAction = attacker.InvocationActionEffect != null;
                        var hasAction = false;
                        invocationMenu.SetActive(true);
                        invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = canAttack;
                        invocationMenu.transform.position = mousePosition;
                        if (hasAction)
                        {
                            invocationMenu.transform.GetChild(1).gameObject.SetActive(true);
                            invocationMenu.transform.GetChild(1).GetComponent<Button>().interactable =
                                IsSpecialActionPossible();
                        }
                        else
                        {
                            invocationMenu.transform.GetChild(1).gameObject.SetActive(false);
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
                    var opponentInvocationCard = cardObject.GetComponent<PhysicalCardDisplay>().card;
                    if (opponentInvocationCard is InGameInvocationCard { IsControlled: true } invocationCard)
                    {
#if UNITY_EDITOR
                        var mousePosition = Input.mousePosition;
#elif UNITY_ANDROID
                            var mousePosition = Input.GetTouch(0).position;
#endif
                        attacker = invocationCard;
                        var canAttack = attacker.CanAttack() && ownPlayerCards.ContainsCardInInvocation(attacker);
                        //var hasAction = attacker.InvocationActionEffect != null;
                        var hasAction = false;
                        invocationMenu.SetActive(true);
                        invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = canAttack;
                        invocationMenu.transform.position = mousePosition;
                        if (hasAction)
                        {
                            invocationMenu.transform.GetChild(1).gameObject.SetActive(true);
                            invocationMenu.transform.GetChild(1).GetComponent<Button>().interactable =
                                IsSpecialActionPossible();
                        }
                        else
                        {
                            invocationMenu.transform.GetChild(1).gameObject.SetActive(false);
                        }
                    }
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

        /**
     * Return the list of available opponents
     */
        protected override void DisplayCards(ObservableCollection<InGameInvocationCard> invocationCards,
            List<InGameEffectCard> attackPlayerEffectCard)
        {
            var notEmptyOpponent = invocationCards.Where(t => t != null && t.Title == "Jean-Michel Bruitages")
                .Cast<InGameCard>().ToList();
            DisplayOpponentMessageBox(notEmptyOpponent);
            stopDetectClicking = true;
        }

        protected override void DisplayOpponentMessageBox(List<InGameCard> invocationCards)
        {
            nextPhaseButton.SetActive(false);
            invocationMenu.SetActive(false);

            if (invocationCards.Count > 0)
            {
                var message =
                    MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choisis ton adversaire :", invocationCards);
                message.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    var invocationCard =
                        message.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;

                    if (invocationCard == null)
                    {
                        message.SetActive(false);
                        MessageBox.CreateOkMessageBox(canvas, "Attention",
                            "Tu es obligé d'attaquer Jean-Michel Bruitages", () => { message.SetActive(true); });
                    }
                    else
                    {
                        opponent = invocationCard;
                        ComputeAttack();
                        stopDetectClicking = false;
                        nextPhaseButton.SetActive(true);
                        nextPhaseButton.GetComponent<HighLightButton>().isActivated = true;
                        HighLightPlane.Highlight.Invoke(HighlightElement.Tentacules, false);
                        Destroy(message);
                    }
                };
                message.GetComponent<MessageBox>().NegativeAction = () => { };
            }
            else
            {
                MessageBox.CreateOkMessageBox(canvas, "Attention",
                    "Tu ne peux pas attaquer le joueur ni ses invocations");
            }
        }

        protected override void NextRound()
        {
            if (!IsP1Turn)
            {
                DialogueUI.TriggerDoneEvent.Invoke(NextDialogueTrigger.NextPhase);
            }

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
                    BlockCardJustInvokeIfNeeded();
                    roundText.GetComponent<TextMeshProUGUI>().text = "Phase d'attaque";
                    break;
            }

            if (phaseId != 3) return;
            PlayerCards currentPlayerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            PlayerCards opponentPlayerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
            PlayerStatus currentPlayerStatus =
                IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();
            PlayerStatus opponentPlayerStatus =
                IsP1Turn ? p2.GetComponent<PlayerStatus>() : p1.GetComponent<PlayerStatus>();
            List<InGameEffectCard> effectCards = currentPlayerCard.effectCards;
            List<InGameEffectCard> opponentEffectCards = opponentPlayerCard.effectCards;

            DealWithEndEffect(currentPlayerCard, opponentPlayerCard, currentPlayerStatus, effectCards);
            DealWithEndEffect(opponentPlayerCard, currentPlayerCard, opponentPlayerStatus, opponentEffectCards);

            var invocationCards = IsP1Turn
                ? p1.GetComponent<PlayerCards>().invocationCards
                : p2.GetComponent<PlayerCards>().invocationCards;

            foreach (var invocationCard in invocationCards)
            {
                invocationCard.UnblockAttack();
            }

            IsP1Turn = !IsP1Turn;
            ChangePlayer.Invoke();
            playerText.GetComponent<TextMeshProUGUI>().text = IsP1Turn ? "Joueur 1" : "Joueur 2";
            phaseId = 0;
            nextPhaseButton.GetComponent<Button>().interactable = false;
        }
    }
}