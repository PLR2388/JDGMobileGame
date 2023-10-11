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
    /// <summary>
    /// Represents the game loop for the tutorial player.
    /// </summary>
    public class TutoPlayerGameLoop : GameLoop
    {
        [SerializeField] private GameObject tutoImage;
        [SerializeField] private GameObject tutoVideo;

        [SerializeField] private GameObject miniCardMenu;
        [SerializeField] private Transform canvas;
        [SerializeField] private GameObject nextPhaseButtonGameObject;
        private Button nextPhaseButton;

        private ActionScenario[] actionScenarios;

        private readonly Dictionary<Highlight, HighlightElement> highlightMapping = new Dictionary<Highlight, HighlightElement>
        {
            {
                Highlight.space, HighlightElement.Space
            },
            {
                Highlight.deck, HighlightElement.Deck
            },
            {
                Highlight.yellow_trash, HighlightElement.YellowTrash
            },
            {
                Highlight.field, HighlightElement.Field
            },
            {
                Highlight.invocation_cards, HighlightElement.Invocations
            },
            {
                Highlight.effect_cards, HighlightElement.Effect
            },
            {
                Highlight.hand_cards, HighlightElement.InHandButton
            },
            {
                Highlight.next_phase, HighlightElement.NextPhaseButton
            },
            {
                Highlight.tentacules, HighlightElement.Tentacules
            },
            {
                Highlight.life_point, HighlightElement.LifePoints
            }
        };

        private const string EquipSymbol = ">";

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // The opponent is player1 (only the AI attacks the player directly)
            actionScenarios = GetComponent<ScenarioDecoder>().Scenario.ActionScenarios;
            DialogueUI.DialogIndex.AddListener(TriggerScenarioAction);
            nextPhaseButton = nextPhaseButtonGameObject.GetComponent<Button>();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            InputManager.OnLongTouch.AddListener(OnLongTouch);
            InputManager.OnTouch.AddListener(OnTouch);
            InputManager.OnReleaseTouch.AddListener(OnReleaseTouch);
            InputManager.OnBackPressed.AddListener(OnBackPressed);
            Draw();
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        private void OnDestroy()
        {
            InputManager.OnLongTouch.RemoveListener(OnLongTouch);
            InputManager.OnTouch.RemoveListener(OnTouch);
            InputManager.OnReleaseTouch.RemoveListener(OnReleaseTouch);
            InputManager.OnBackPressed.RemoveListener(OnBackPressed);
        }

        /// <summary>
        /// Triggers the scenario action based on the provided index.
        /// </summary>
        /// <param name="index">The index of the action scenario to trigger.</param>
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

                HandleHighlight(highlight);

                tutoImage.SetActive(image != null);
                tutoVideo.SetActive(video != null);

                if (putCard != null)
                {
                    PlaceCard(putCard);
                }

                if (attack != null)
                {
                    HandleAttack(attack);
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
                UnsetHighlight();
                Console.WriteLine(e);
            }
        }
        
        /// <summary>
        /// Handles the attack scenario based on the provided attack configuration.
        /// </summary>
        /// <param name="attack">Array of attack configuration in the form [attacker, defender] or [attacker] if defender is player.</param>
        private void HandleAttack(string[] attack)
        {
            string attacker = attack[0];
            string defender = attack.Length > 1 && !string.IsNullOrEmpty(attack[1]) ? attack[1] : CardNameMappings.CardNameMap[CardNames.Player];

            InGameInvocationCard attackerInvocationCard =
                CardManager.Instance.GetCurrentPlayerCards().InvocationCards.First(card => card.Title == attacker);

            PlayerCards opponentPlayerCards = CardManager.Instance.GetOpponentPlayerCards();

            InGameInvocationCard opponentInvocationCard = defender == CardNameMappings.CardNameMap[CardNames.Player]
                ? opponentPlayerCards.Player as InGameInvocationCard
                : opponentPlayerCards.InvocationCards
                    .First(card => card.Title == defender);

            CardManager.Instance.Attacker = attackerInvocationCard;
            CardManager.Instance.Opponent = opponentInvocationCard;
            ComputeAttack();

            if (defender == CardNameMappings.CardNameMap[CardNames.Player])
            {
                HighLightPlane.Highlight.Invoke(HighlightElement.InHandButton, true);
            }
        }
        
        /// <summary>
        /// Places a card in the game using a scenario
        /// </summary>
        /// <param name="putCard">The name of the card to be placed.</param>

        private static void PlaceCard(string putCard)
        {
            PlayerCards playerCards = CardManager.Instance.GetCurrentPlayerCards();
            if (putCard.Contains(EquipSymbol))
            {
                EquipInvocationCard(putCard, playerCards);
            }
            else
            {
                InvokeInvocationCards(putCard, playerCards);
            }
        }

        /// <summary>
        /// Invokes the specified invocation cards.
        /// </summary>
        /// <param name="putCard">Card name to invoke.</param>
        /// <param name="playerCards">Current player's card details.</param>
        private static void InvokeInvocationCards(string putCard, PlayerCards playerCards)
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
        
        /// <summary>
        /// Equips the specified invocation card with the given equipment.
        /// </summary>
        /// <param name="putCard">Card data for equipment and invocation card.</param>
        /// <param name="playerCards">Current player's card details.</param>
        private static void EquipInvocationCard(string putCard, PlayerCards playerCards)
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
        
        /// <summary>
        /// Handles the highlighting of elements in the game based on the given highlight type.
        /// </summary>
        /// <param name="highlight">Type of highlight to apply.</param>
        private void HandleHighlight(Highlight highlight)
        {
            UnsetHighlight();
            if (highlightMapping.TryGetValue(highlight, out var highlightElement))
            {
                HighLightPlane.Highlight.Invoke(highlightElement, true);
            }
            else if (highlight != Highlight.unknown)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handles the transition to the next round of the game.
        /// </summary>
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

        /// <summary>
        /// Displays available opponents for the current player.
        /// </summary>
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

        /// <summary>
        /// Unsets the highlighted elements in the game.
        /// </summary>
        private void UnsetHighlight()
        {
            foreach (var element in highlightMapping.Values)
            {
                HighLightPlane.Highlight.Invoke(element, false);
            }
        }

        /// <summary>
        /// Chooses the next phase of the game based on game state and conditions.
        /// </summary>
        protected override void ChoosePhase()
        {
            InvocationMenuManager.Instance.Enable();
            ChoosePhaseMusic();

            if (GameStateManager.Instance.NumberOfTurn == 2 && CardManager.Instance.GetCurrentPlayerCards().InvocationCards.Count == 2)
            {
                HighLightPlane.Highlight.Invoke(HighlightElement.NextPhaseButton, true);
            }
        }

        /// <summary>
        /// Handles the touch input by the player during the game.
        /// </summary>
        private void OnTouch()
        {
            var cardTouch = CardRaycastManager.Instance.GetTouchedCard();
            if (cardTouch?.Title != CardNameMappings.CardNameMap[CardNames.Tentacules] || GameStateManager.Instance.Phase != Phase.Attack) return;
            HandleSingleTouch(cardTouch, CardOwner.Player2, true);
        }
    }
}