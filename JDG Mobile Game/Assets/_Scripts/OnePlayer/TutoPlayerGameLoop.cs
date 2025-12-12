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
        protected override void Start()
        {
            // Base class handles EventBus subscriptions and calls Draw()
            base.Start();
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            // Base class handles EventBus cleanup
            base.OnDestroy();
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

            // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
            InGameInvocationCard attackerInvocationCard =
                _cardCollectionService.GetCurrentPlayerCards().InvocationCards.First(card => card.Title == attacker);

            PlayerCards opponentPlayerCards = _cardCollectionService.GetOpponentPlayerCards();

            InGameInvocationCard opponentInvocationCard = defender == CardNameMappings.CardNameMap[CardNames.Player]
                ? opponentPlayerCards.Player as InGameInvocationCard
                : opponentPlayerCards.InvocationCards
                    .First(card => card.Title == defender);

            // Phase 17-18: Use ICombatService instead of CardManager.Instance
            _combatService.Attacker = attackerInvocationCard;
            _combatService.Opponent = opponentInvocationCard;
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

        private void PlaceCard(string putCard)
        {
            // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
            PlayerCards playerCards = _cardCollectionService.GetCurrentPlayerCards();
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
        private void EquipInvocationCard(string putCard, PlayerCards playerCards)
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
                // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
                equipmentCardEquipmentAbility.ApplyEffect(
                    invocationCard,
                    playerCards,
                    _cardCollectionService.GetOpponentPlayerCards()
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
            // Phase 9: Use injected service instead of InvocationMenuManager.Instance
            _invocationMenuService.Hide();
            if (_gameStateService.CurrentPlayer != JDG.Domain.ValueObjects.PlayerId.Player1)
            {
                DialogueUI.TriggerDoneEvent.Invoke(NextDialogueTrigger.NextPhase);
            }
            if (_gameStateService.TurnNumber == 1 && _gameStateService.CurrentPlayer == JDG.Domain.ValueObjects.PlayerId.Player1)
            {
                _gameStateService.SetPhase(JDG.Domain.Phase.End);
            }
            else
            {
                _gameStateService.NextPhase();
            }

            var playerStatus = _playerStatusProvider.GetCurrentPlayerStatus();
            if (_gameStateService.CurrentPhase == JDG.Domain.Phase.Attack && playerStatus.BlockAttack)
            {
                _gameStateService.SetPhase(JDG.Domain.Phase.End);
            }

            // Phase 9: Use injected service instead of RoundDisplayManager.Instance
            _roundDisplayService.AdaptUIToPhaseIdInNextRound(false);

            switch (_gameStateService.CurrentPhase)
            {
                case JDG.Domain.Phase.Attack:
                    PlayAttackMusic();
                    break;
                case JDG.Domain.Phase.End:
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
            // Phase 17-18: Use ICombatService instead of CardManager.Instance
            var notEmptyOpponent = _combatService.BuildValidTargets();
            DisplayOpponentMessageBox(notEmptyOpponent);
            // Phase 19-20: Use injected InputManager from base class instead of .Instance
            _inputManager.DisableDetectionTouch();
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
                    // Phase 17-18: Use ICombatService instead of CardManager.Instance
                    _combatService.Opponent = invocationCard;
                    ComputeAttack();
                    HighLightPlane.Highlight.Invoke(HighlightElement.Tentacules, false);
                    miniCardMenu.SetActive(false);
                    HighLightPlane.Highlight.Invoke(HighlightElement.NextPhaseButton, true);
                }
                // Phase 19-20: Use injected InputManager from base class instead of .Instance
                _inputManager.EnableDetectionTouch();
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
            // Phase 9: Use injected service instead of InvocationMenuManager.Instance
            _invocationMenuService.Enable();
            ChoosePhaseMusic();

            // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
            if (_gameStateService.TurnNumber == 2 && _cardCollectionService.GetCurrentPlayerCards().InvocationCards.Count == 2)
            {
                HighLightPlane.Highlight.Invoke(HighlightElement.NextPhaseButton, true);
            }
        }

        /// <summary>
        /// Handles the touch input by the player during the game.
        /// This method is never called directly - kept for potential future use.
        /// Touch events are handled by the base class OnTouch(TouchStartedEvent) method.
        /// </summary>
        [System.Obsolete("This method is shadowed by base class OnTouch(TouchStartedEvent). Consider removing or renaming.")]
        private void OnTouch()
        {
            var cardTouch = _raycastService.GetTouchedCard();
            if (cardTouch?.Title != CardNameMappings.CardNameMap[CardNames.Tentacules] || _gameStateService.CurrentPhase != JDG.Domain.Phase.Attack) return;
            HandleSingleTouch(cardTouch, CardOwner.Player2, true);
        }
    }
}