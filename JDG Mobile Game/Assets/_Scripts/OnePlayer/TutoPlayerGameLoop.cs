using System;
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

        // Phase 123: EventBus subscription for dialogue index changes
        private IDisposable _dialogueIndexSubscription;

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
        /// </summary>
        protected override void Start()
        {
            // Base class handles EventBus subscriptions and calls Draw()
            base.Start();
            // Phase 123: Subscribe to DialogueIndexChangedEvent via EventBus
            _dialogueIndexSubscription = _eventBus?.Subscribe<DialogueIndexChangedEvent>(OnDialogueIndexChanged);
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
        /// This function is called when the MonoBehaviour will be destroyed.
        /// Phase 123: Dispose DialogueIndexChangedEvent subscription.
        /// </summary>
        protected override void OnDestroy()
        {
            // Phase 123: Dispose subscription
            _dialogueIndexSubscription?.Dispose();
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

            // Phase 117: Use modern abilities with OnEquip trigger
            var owner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
            var ownerId = JDG.Domain.ValueObjects.PlayerId.FromCardOwner(owner);
            var opponentOwner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player2 : JDG.Domain.CardOwner.Player1;
            var opponentId = JDG.Domain.ValueObjects.PlayerId.FromCardOwner(opponentOwner);
            var context = new JDG.Application.Abilities.AbilityContext(ownerId, opponentId, null, JDG.Domain.AbilityName.Default);

            foreach (var ability in equipmentCard.ModernEquipmentAbilities)
            {
                if (ability.CanActivate(context))
                {
                    ability.Execute(context);
                }
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
                Debug.Log("TutoPlayerGameLoop: Attack blocked - Player 1 cannot attack on Turn 1");
                return;
            }

            Debug.Log("TutoPlayerGameLoop.DisplayAvailableOpponent: Called");
            // Phase 17-18: Use ICombatService instead of CardManager.Instance
            var notEmptyOpponent = _combatService.BuildValidTargets();
            Debug.Log($"TutoPlayerGameLoop.DisplayAvailableOpponent: Found {notEmptyOpponent?.Count ?? 0} valid targets");
            DisplayOpponentMessageBox(notEmptyOpponent);
            // Phase 19-20: Use injected InputManager from base class instead of .Instance
            _inputManager.DisableDetectionTouch();
        }

        /// <summary>
        /// Display the MessageBox with the available opponents
        /// Phase 35: Uses inherited _dialogService instead of CardSelector.Instance.
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
                    // Phase 122: Publish via EventBus
                    _eventBus?.Publish(new HighlightRequestedEvent { Element = (int)HighlightElement.Tentacules, IsActivated = false });
                    miniCardMenu.SetActive(false);
                    _eventBus?.Publish(new HighlightRequestedEvent { Element = (int)HighlightElement.NextPhaseButton, IsActivated = true });
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
            _dialogService.ShowCardSelector(canvas, options);
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