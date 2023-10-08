using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using OnePlayer.DialogueBox;
using UnityEngine;
using UnityEngine.UI;

namespace OnePlayer
{
    public class TutoPlayerGameLoop : GameLoop
    {
        [SerializeField] private GameObject tutoImage;
        [SerializeField] private GameObject tutoVideo;

        [SerializeField] private GameObject miniCardMenu;
        [SerializeField] private Transform canvas;
        [SerializeField] private GameObject nextPhaseButtonGameObject;
        private Button nextPhaseButton;

        private ActionScenario[] actionScenarios;

        private void Awake()
        {
            // The opponent is player2 (only the AI attacks the player directly)
            actionScenarios = GetComponent<ScenarioDecoder>().Scenario.actionScenarios;
            DialogueUI.DialogIndex.AddListener(TriggerScenarioAction);
            nextPhaseButton = nextPhaseButtonGameObject.GetComponent<Button>();
        }

        private void Start()
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
                    PlayerCards playerCards = CardManager.Instance.GetCurrentPlayerCards();
                    if (putCard.Contains(">"))
                    {
                        var cardNames = putCard.Split('>');

                        InGameEquipmentCard equipmentCard =
                            playerCards.HandCards.FirstOrDefault(elt => elt.Title == cardNames[0]) as InGameEquipmentCard;
                        InGameInvocationCard invocationCard =
                            playerCards.InvocationCards.FirstOrDefault(elt => elt.Title == cardNames[1]);
                        
                        if (equipmentCard == null) return;

                        invocationCard?.SetEquipmentCard(equipmentCard);
                        playerCards.HandCards.Remove(equipmentCard);
                        foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
                        {
                            equipmentCardEquipmentAbility.ApplyEffect(
                                invocationCard,
                                playerCards,
                                CardManager.Instance.GetOpponentPlayerCards()
                            );
                        }
                    }
                    else
                    {
                        var cardNames = putCard.Split(';');
                        foreach (var cardName in cardNames)
                        {
                            InGameCard card = playerCards.HandCards.First(elt => elt.Title == cardName);
                            if (card is InGameInvocationCard invocationCard)
                            {
                                playerCards.HandCards.Remove(card);
                                playerCards.InvocationCards.Add(invocationCard);
                            }
                            else if (card is InGameFieldCard fieldCard)
                            {
                                playerCards.FieldCard = fieldCard;
                                playerCards.HandCards.Remove(fieldCard);
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
                        CardManager.Instance.GetCurrentPlayerCards().InvocationCards.First(card => card.Title == attacker);

                    PlayerCards opponentPlayerCards = CardManager.Instance.GetOpponentPlayerCards();

                    InGameInvocationCard opponentInvocationCard;

                    if (defender == "player")
                    {
                        opponentInvocationCard = opponentPlayerCards.Player as InGameInvocationCard;
                    }
                    else
                    {
                        opponentInvocationCard = opponentPlayerCards.InvocationCards
                            .First(card => card.Title == defender);
                    }

                    CardManager.Instance.Attacker = attackerInvocationCard;
                    CardManager.Instance.Opponent = opponentInvocationCard;
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

        protected override void NextRound()
        {
            HighLightPlane.Highlight.Invoke(HighlightElement.NextPhaseButton, false);
            InvocationMenuManager.Instance.Hide();
            if (GameStateManager.Instance.IsP1Turn == false)
            {
                DialogueUI.TriggerDoneEvent.Invoke(NextDialogueTrigger.NextPhase);
            }
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

            RoundDisplayManager.Instance.AdaptUIToPhaseIdInNextRound(false);

            switch (GameStateManager.Instance.Phase)
            {
                case Phase.Attack:
                    PlayAttackMusic();
                    break;
                case Phase.End:
                    EndTurnPhase();
                    break;
            }
            nextPhaseButton.interactable = false;
        }

        public new void DisplayAvailableOpponent()
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
                if (invocationCard?.Title == CardNameMappings.CardNameMap[CardNames.JeanMichelBruitages])
                {
                    CardManager.Instance.Opponent = invocationCard;
                    ComputeAttack();
                    HighLightPlane.Highlight.Invoke(HighlightElement.Tentacules, false);
                    miniCardMenu.SetActive(false);
                    HighLightPlane.Highlight.Invoke(HighlightElement.NextPhaseButton, true);
                }
                InputManager.Instance.EnableDetectionTouch();
            }

            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOOSE_OPPONENT),
                invocationCards,
                showOkButton: true,
                okAction: (invocationCard) =>
                {
                    PositiveAction(invocationCard as InGameInvocationCard);
                    nextPhaseButtonGameObject.SetActive(true);
                }
            );
            CardSelector.Instance.CreateCardSelection(
                canvas,
                config
            );
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

        protected override void ChoosePhase()
        {
            InvocationMenuManager.Instance.Enable();
            ChoosePhaseMusic();

            if (GameStateManager.Instance.NumberOfTurn == 2 && CardManager.Instance.GetCurrentPlayerCards().InvocationCards.Count == 2)
            {
                HighLightPlane.Highlight.Invoke(HighlightElement.NextPhaseButton, true);
            }
        }

        private void OnTouch()
        {
            var cardTouch = CardRaycastManager.Instance.GetTouchedCard();
            if (cardTouch?.Title != CardNameMappings.CardNameMap[CardNames.Tentacules] || GameStateManager.Instance.Phase != Phase.Attack) return;
            HandleSingleTouch(cardTouch, CardOwner.Player2, true);
        }
    }
}