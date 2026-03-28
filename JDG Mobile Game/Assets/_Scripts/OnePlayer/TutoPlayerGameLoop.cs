using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Application.Services;
using JDG.Domain.Events;
using OnePlayer.DialogueBox;
using UnityEngine;
using UnityEngine.UI;

namespace OnePlayer
{
    /// <summary>
    /// Represents the game loop for the tutorial player.
    /// Phase 122: Uses EventBus for HighlightRequestedEvent instead of static UnityEvent.
    /// </summary>
    public class TutoPlayerGameLoop : GameLoop
    {
        [SerializeField] private GameObject tutoImage;
        [SerializeField] private GameObject tutoVideo;

        [SerializeField] private GameObject miniCardMenu;
        // Phase 136: Removed canvas SerializeField - now uses inherited _canvasProvider
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

        // Phase 123: EventBus subscription for dialogue index changes
        private IDisposable _dialogueIndexSubscription;

        // Phase 143: EventBus subscription for tutorial-specific touch handling
        private IDisposable _touchSubscription;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // The opponent is player1 (only the AI attacks the player directly)
            actionScenarios = GetComponent<ScenarioDecoder>().Scenario.ActionScenarios;
            nextPhaseButton = nextPhaseButtonGameObject.GetComponent<Button>();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// Phase 123: Subscribe to DialogueIndexChangedEvent for tutorial scenario triggers.
        /// Phase 143: Subscribe to TouchStartedEvent for tutorial attack phase handling.
        /// </summary>
        protected override void Start()
        {
            // Base class handles EventBus subscriptions and calls Draw()
            base.Start();
            // Phase 123: Subscribe to DialogueIndexChangedEvent via EventBus
            _dialogueIndexSubscription = _eventBus?.Subscribe<DialogueIndexChangedEvent>(OnDialogueIndexChanged);
            // Phase 143: Subscribe to TouchStartedEvent for tutorial attack phase
            _touchSubscription = _eventBus?.Subscribe<TouchStartedEvent>(OnTutoTouch);
        }

        /// <summary>
        /// Handles DialogueIndexChangedEvent to trigger scenario actions.
        /// Phase 123: Replaces DialogueUI.DialogIndex static event listener.
        /// </summary>
        private void OnDialogueIndexChanged(DialogueIndexChangedEvent evt)
        {
            TriggerScenarioAction(evt.DialogueIndex);
        }

        /// <summary>
        /// Tutorial-specific touch handling for Attack phase.
        /// Phase 143: Multi-step attack flow - highlight Tentacules, then attack button.
        /// Note: Human is Player2 in the tutorial.
        /// </summary>
        private void OnTutoTouch(TouchStartedEvent evt)
        {
            // Only handle during Attack phase
            if (_gameStateService.CurrentPhase != JDG.Domain.Phase.Attack) return;

            var cardTouch = _raycastService.GetTouchedCard();
            if (cardTouch == null) return;

            // Step 2: User (Player2) clicked on Tentacules (their attacking card)
            if (cardTouch.Title == CardNameMappings.CardNameMap[CardNames.Tentacules]
                && cardTouch.CardOwner == CardOwner.Player2)
            {
                // Deactivate Tentacules highlight
                _eventBus?.Publish(new HighlightRequestedEvent
                {
                    Element = (int)HighlightElement.Tentacules,
                    IsActivated = false
                });
                // Base class will show the attack menu - highlight attack button after a frame
                StartCoroutine(HighlightAttackButtonAfterDelay());
            }
        }

        /// <summary>
        /// Highlights the attack button after the menu appears.
        /// Phase 143: Waits one frame for the attack menu to be displayed.
        /// </summary>
        private IEnumerator HighlightAttackButtonAfterDelay()
        {
            yield return null; // Wait for attack menu to appear
            _eventBus?.Publish(new HighlightRequestedEvent
            {
                Element = (int)HighlightElement.AttackButton,
                IsActivated = true
            });
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// Phase 123: Dispose DialogueIndexChangedEvent subscription.
        /// Phase 143: Dispose TouchStartedEvent subscription.
        /// </summary>
        protected override void OnDestroy()
        {
            // Phase 123: Dispose subscription
            _dialogueIndexSubscription?.Dispose();
            // Phase 143: Dispose touch subscription
            _touchSubscription?.Dispose();
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
                var actionScenario = actionScenarios.FirstOrDefault(elt => elt.Index == index);
                if (actionScenario == null) return; // No scenario action defined for this dialogue index
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
                // Phase 158: Use Debug.LogError instead of Console.WriteLine for Unity visibility
                Debug.LogError($"TutoPlayerGameLoop.TriggerScenarioAction: Exception at index {index}: {e}");
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
            // Phase 158: Use FirstOrDefault + null check to prevent InvalidOperationException
            InGameInvocationCard attackerInvocationCard =
                _cardCollectionService.GetCurrentPlayerCards().InvocationCards.FirstOrDefault(card => card.Title == attacker);

            if (attackerInvocationCard == null)
            {
                Debug.LogError($"TutoPlayerGameLoop.HandleAttack: Attacker '{attacker}' not found in current player's invocation cards");
                return;
            }

            PlayerCards opponentPlayerCards = _cardCollectionService.GetOpponentPlayerCards();

            // Phase 158: Use FirstOrDefault + null check to prevent InvalidOperationException
            InGameInvocationCard opponentInvocationCard = defender == CardNameMappings.CardNameMap[CardNames.Player]
                ? opponentPlayerCards.Player as InGameInvocationCard
                : opponentPlayerCards.InvocationCards
                    .FirstOrDefault(card => card.Title == defender);

            // Phase 144: Add null check for the cast result
            if (opponentInvocationCard == null)
            {
                Debug.LogWarning($"TutoPlayerGameLoop: Failed to cast defender '{defender}' to InGameInvocationCard");
                return;
            }

            // Phase 17-18: Use ICombatService instead of CardManager.Instance
            _combatService.Attacker = attackerInvocationCard;
            _combatService.Opponent = opponentInvocationCard;
            ComputeAttack();

            if (defender == CardNameMappings.CardNameMap[CardNames.Player])
            {
                // Phase 122: Publish via EventBus
                _eventBus?.Publish(new HighlightRequestedEvent { Element = (int)HighlightElement.InHandButton, IsActivated = true });
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
        /// Phase 141: Now uses IAbilityExecutor with ICardSyncService for proper stat sync.
        /// Phase 146: Added validation for card type to catch tutorial data issues.
        /// </summary>
        /// <param name="putCard">Card data for equipment and invocation card.</param>
        /// <param name="playerCards">Current player's card details.</param>
        private void EquipInvocationCard(string putCard, PlayerCards playerCards)
        {
            var cardNames = putCard.Split('>');

            // Phase 146: Search for card by title first, then validate type
            var handCardByTitle = playerCards.HandCards.FirstOrDefault(elt => elt.Title == cardNames[0]);
            InGameEquipmentCard equipmentCard = handCardByTitle as InGameEquipmentCard;

            // Phase 146: Log warning if card found by title but wrong type
            if (handCardByTitle != null && equipmentCard == null)
            {
                Debug.LogWarning($"[TutoPlayerGameLoop] Card '{cardNames[0]}' found in hand but is {handCardByTitle.GetType().Name}, not InGameEquipmentCard. Check tutorial data.");
            }

            InGameInvocationCard invocationCard =
                playerCards.InvocationCards.FirstOrDefault(elt => elt.Title == cardNames[1]);

            if (equipmentCard == null || invocationCard == null) return;

            // Phase 141: Execute equipment abilities via IAbilityExecutor
            // This uses ICardSyncService internally to sync domain Card changes back to InGameInvocationCard
            var opponentCards = _cardCollectionService.GetOpponentPlayerCards();
            _abilityExecutor.ExecuteOnEquipmentAttached(equipmentCard, invocationCard, playerCards, opponentCards);

            // Attach equipment and remove from hand
            invocationCard.SetEquipmentCard(equipmentCard);
            playerCards.HandCards.Remove(equipmentCard);
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
                // Phase 122: Publish via EventBus
                _eventBus?.Publish(new HighlightRequestedEvent { Element = (int)highlightElement, IsActivated = true });
            }
            else if (highlight != Highlight.unknown)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handles the transition to the next round of the game.
        /// Note: Attack phase skip for Player 1 on Turn 1 is handled automatically by GameStateService.NextPhase().
        /// </summary>
        protected override void NextRound()
        {
            // Phase 122: Publish via EventBus
            _eventBus?.Publish(new HighlightRequestedEvent { Element = (int)HighlightElement.NextPhaseButton, IsActivated = false });
            // Phase 9: Use injected service instead of InvocationMenuManager.Instance
            _invocationMenuService.Hide();
            if (_gameStateService.CurrentPlayer != JDG.Domain.ValueObjects.PlayerId.Player1)
            {
                // Phase 123: Publish via EventBus instead of static TriggerDoneEvent
                _eventBus?.Publish(new DialogueTriggerCompletedEvent { TriggerType = (int)NextDialogueTrigger.NextPhase });
            }

            // NextPhase() automatically skips Attack phase for Player 1 on Turn 1
            _gameStateService.NextPhase();

            // Check if attack is blocked by card effects
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
        /// Defense-in-depth: Uses GameStateService.ShouldSkipAttackPhase for Turn 1 restriction.
        /// </summary>
        public new void DisplayAvailableOpponent()
        {
            // Defense-in-depth: Block attack if attack phase should be skipped
            if (_gameStateService.ShouldSkipAttackPhase)
            {
#if UNITY_EDITOR
                Debug.Log("TutoPlayerGameLoop: Attack blocked - Player 1 cannot attack on Turn 1");
#endif
                return;
            }

#if UNITY_EDITOR
            Debug.Log("TutoPlayerGameLoop.DisplayAvailableOpponent: Called");
#endif
            // Phase 17-18: Use ICombatService instead of CardManager.Instance
            var notEmptyOpponent = _combatService.BuildValidTargets();
#if UNITY_EDITOR
            Debug.Log($"TutoPlayerGameLoop.DisplayAvailableOpponent: Found {notEmptyOpponent?.Count ?? 0} valid targets");
#endif
            DisplayOpponentMessageBox(notEmptyOpponent);
            // Phase 19-20: Use injected InputManager from base class instead of .Instance
            _inputManager.DisableDetectionTouch();
        }

        /// <summary>
        /// Display the MessageBox with the available opponents
        /// Phase 35: Uses inherited _dialogService instead of CardSelector.Instance.
        /// Phase 143: Multi-step highlight flow - deactivate attack button, highlight JMB.
        /// </summary>
        /// <param name="invocationCards">Available opponents list</param>
        private void DisplayOpponentMessageBox(List<InGameCard> invocationCards)
        {
            // Phase 143: Deactivate attack button highlight when selector opens
            _eventBus?.Publish(new HighlightRequestedEvent
            {
                Element = (int)HighlightElement.AttackButton,
                IsActivated = false
            });

            // Phase 143: Set card to highlight in selector
            DisplayCards.CardToHighlight = CardNameMappings.CardNameMap[CardNames.JeanMichelBruitages];

            void PositiveAction(InGameInvocationCard invocationCard)
            {
                // Phase 143: Clear the card to highlight
                DisplayCards.CardToHighlight = null;

                if (invocationCard?.Title == CardNameMappings.CardNameMap[CardNames.JeanMichelBruitages])
                {
                    // Phase 17-18: Use ICombatService instead of CardManager.Instance
                    _combatService.Opponent = invocationCard;
                    ComputeAttack();
                    miniCardMenu.SetActive(false);
                    // Phase 143: Highlight next phase button after attack
                    _eventBus?.Publish(new HighlightRequestedEvent { Element = (int)HighlightElement.NextPhaseButton, IsActivated = true });
                    // Phase 142: Publish NextPhase trigger to advance dialogue from index 19 (Attack trigger)
                    _eventBus?.Publish(new DialogueTriggerCompletedEvent { TriggerType = (int)NextDialogueTrigger.Attack });
                }
                // Phase 19-20: Use injected InputManager from base class instead of .Instance
                _inputManager.EnableDetectionTouch();
            }

            // Phase 34: Use inherited _localizationService from GameLoop
            // Phase 35: Use inherited _dialogService instead of CardSelector.Instance
            var cardObjects = new List<object>();
            foreach (var card in invocationCards) cardObjects.Add(card);

            var options = new CardSelectorOptions
            {
                Title = _localizationService.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOOSE_OPPONENT),
                Cards = cardObjects,
                ShowOkButton = true,
                OnOkSingle = (card) =>
                {
                    PositiveAction(card as InGameInvocationCard);
                    nextPhaseButtonGameObject.SetActive(true);
                }
            };
            // Phase 136: Use inherited _canvasProvider instead of SerializeField
            var canvasTransform = _canvasProvider.GetGameCanvas() as Transform;
            _dialogService.ShowCardSelector(canvasTransform, options);
        }

        /// <summary>
        /// Unsets the highlighted elements in the game.
        /// </summary>
        private void UnsetHighlight()
        {
            foreach (var element in highlightMapping.Values)
            {
                // Phase 122: Publish via EventBus
                _eventBus?.Publish(new HighlightRequestedEvent { Element = (int)element, IsActivated = false });
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
                // Phase 122: Publish via EventBus
                _eventBus?.Publish(new HighlightRequestedEvent { Element = (int)HighlightElement.NextPhaseButton, IsActivated = true });
            }
        }

    }
}