using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TutoPlayerGameLoop : MonoBehaviour
    {
        public static bool IsP1Turn;

        public int phaseId;

        private CardLocation cardLocation;
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

        public static readonly UnityEvent ChangePlayer = new UnityEvent();

        private InvocationFunctions invocationFunctions;

        private const float ClickDuration = 2;

        private bool stopDetectClicking;
        private bool clicking;
        private float totalDownTime;
        private Card cardSelected;
        private InvocationCard attacker;
        private InvocationCard opponent;
        private int numberOfTurn;
        
        private ActionScenario[] actionScenarios;
        [SerializeField] 
        private DialogueUI dialogueBox;
        private int currentIndex = 0;
        private bool hasSendCurrent = false;

        // Start is called before the first frame update

        private void Awake()
        {
            actionScenarios = GetComponent<ScenarioDecoder>().Scenario.actionScenarios;
            DialogueUI.DialogIndex.AddListener(TriggerScenarioAction);
        }


        private void Start()
        {
            cardLocation = GetComponent<CardLocation>();
            invocationFunctions = GetComponent<InvocationFunctions>();
            IsP1Turn = true;
            ChangeHealthText(PlayerStatus.MaxPv, true);
            ChangeHealthText(PlayerStatus.MaxPv, false);
            p1.GetComponent<PlayerStatus>().SetNumberShield(0);
            p2.GetComponent<PlayerStatus>().SetNumberShield(0);
            PlayerStatus.ChangePvEvent.AddListener(ChangeHealthText);

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
                    case Highlight.unknown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (putCard != null)
                {
                    var cardNames = putCard.Split(';');
                    PlayerCards playerCards = p1.GetComponent<PlayerCards>();
                    foreach (var name in cardNames)
                    {
                        Card card = playerCards.handCards.Find(elt => elt.Nom == name);
                        if (card is InvocationCard invocationCard)
                        {
                            playerCards.handCards.Remove(card);
                            playerCards.invocationCards.Add(invocationCard);
                        }
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

        private void ApplyScenario()
        {
            if (!hasSendCurrent)
            {
                //dialogueBox.ShowDialogue(dialogueObject);
                hasSendCurrent = true;
            }
        }

        private void ChoosePhase()
        {
            invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
        }

        private static void GameOver()
        {
            SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
        }

        private void ChangeHealthText(float pv, bool isP1)
        {
            if (isP1)
            {
                healthP1Text.SetText(pv + "/" + PlayerStatus.MaxPv);
            }
            else
            {
                healthP2Text.SetText(pv + "/" + PlayerStatus.MaxPv);
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
            if (Camera.main == null) return;
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

        private void BlockCardJustInvokeIfNeeded()
        {
            var ownInvocations = IsP1Turn
                ? p1.GetComponent<PlayerCards>().invocationCards
                : p2.GetComponent<PlayerCards>().invocationCards;
            var invocationsOpponent = IsP1Turn
                ? p2.GetComponent<PlayerCards>().invocationCards
                : p1.GetComponent<PlayerCards>().invocationCards;
            foreach (var ownInvocationCard in from invocationCard in invocationsOpponent
                     where invocationCard.GetEquipmentCard() != null
                     select invocationCard.GetEquipmentCard()
                     into equipment
                     where equipment.EquipmentPermEffect != null &&
                           equipment.EquipmentPermEffect.Keys.Contains(PermanentEffect.BlockOpponentDuringInvocation)
                     from ownInvocationCard in ownInvocations.Where(ownInvocationCard =>
                         ownInvocationCard.NumberTurnOnField == 0)
                     select ownInvocationCard)
            {
                ownInvocationCard.BlockAttack();
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
#if UNITY_EDITOR
                    var mousePosition = Input.mousePosition;
#elif UNITY_ANDROID
                var mousePosition = Input.GetTouch(0).position;
#endif
                    if (cardSelected is InvocationCard invocationCard)
                    {
                        attacker = invocationCard;
                        var canAttack = attacker.CanAttack() && ownPlayerCards.ContainsCardInInvocation(attacker);
                        var hasAction = attacker.InvocationActionEffect != null;
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
                    if (opponentInvocationCard is InvocationCard { IsControlled: true } invocationCard)
                    {
#if UNITY_EDITOR
                        var mousePosition = Input.mousePosition;
#elif UNITY_ANDROID
                            var mousePosition = Input.GetTouch(0).position;
#endif
                        attacker = invocationCard;
                        var canAttack = attacker.CanAttack() && ownPlayerCards.ContainsCardInInvocation(attacker);
                        var hasAction = attacker.InvocationActionEffect != null;
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

        public void DisplayAvailableOpponent()
        {
            var opponentCards = IsP1Turn
                ? p2.GetComponent<PlayerCards>().invocationCards
                : p1.GetComponent<PlayerCards>().invocationCards;
            var attackerEffectCards =
                IsP1Turn ? p1.GetComponent<PlayerCards>().effectCards : p2.GetComponent<PlayerCards>().effectCards;
            DisplayCards(opponentCards, attackerEffectCards);
        }

        private bool IsSpecialActionPossible()
        {
            return invocationFunctions.IsSpecialActionPossible(attacker, attacker.InvocationActionEffect);
        }

        public void UseSpecialAction()
        {
            invocationFunctions.AskIfUserWantToUseActionEffect(attacker, attacker.InvocationActionEffect);
        }

        /**
     * Return the list of available opponents
     */
        private void DisplayCards(List<InvocationCard> invocationCards, List<EffectCard> attackPlayerEffectCard)
        {
            var notEmptyOpponent = invocationCards.Where(t => t != null && t.Nom != null).Cast<Card>().ToList();

            if (notEmptyOpponent.Count == 0)
            {
                notEmptyOpponent.Add(player);
            }
            else if (attacker.GetEquipmentCard() != null && attacker.GetEquipmentCard().EquipmentInstantEffect != null &&
                     attacker.GetEquipmentCard().EquipmentInstantEffect.Keys.Contains(InstantEffect.DirectAtk))
            {
                notEmptyOpponent = new List<Card>
                {
                    player
                };
            }
            else
            {
                var newList = new List<Card>();
                foreach (var card in notEmptyOpponent)
                {
                    var invocationCard = (InvocationCard)card;
                    var permEffect = invocationCard.InvocationPermEffect;
                    if (permEffect != null)
                    {
                        newList.AddRange(from effect in permEffect.Keys
                            where effect == PermEffect.CanOnlyAttackIt
                            select card);
                    }
                }

                if (newList.Count > 0)
                {
                    notEmptyOpponent = newList;
                }
                else
                {
                    var hypnoBoobOnly = false;
                    for (var j = notEmptyOpponent.Count - 1; j >= 0; j--)
                    {
                        var invocationCard = (InvocationCard)notEmptyOpponent[j];
                        var permEffect = invocationCard.InvocationPermEffect;
                        var equipmentCard = invocationCard.GetEquipmentCard();
                        if (permEffect != null)
                        {
                            var keys = permEffect.Keys;
                            var values = permEffect.Values;

                            List<InvocationCard> moreDefInvocationCards = new List<InvocationCard>();

                            for (var i = 0; i < keys.Count; i++)
                            {
                                switch (keys[i])
                                {
                                    case PermEffect.ImpossibleAttackByInvocation:
                                    {
                                        if (Enum.TryParse(values[i], out CardFamily family))
                                        {
                                            if ((from cardToCheck in notEmptyOpponent
                                                    where cardToCheck.Nom != invocationCard.Nom
                                                    select (InvocationCard)cardToCheck).Any(invocationCardToCheck =>
                                                    invocationCardToCheck.GetFamily().Contains(family)))
                                            {
                                                notEmptyOpponent.Remove(invocationCard);
                                            }
                                        }
                                        else if (bool.Parse(values[i]))
                                        {
                                            notEmptyOpponent.Remove(invocationCard);
                                        }
                                    }
                                        break;
                                    case PermEffect.Condition:
                                    {
                                        switch (values[i])
                                        {
                                            case "More DEF":
                                            {
                                                moreDefInvocationCards.AddRange(
                                                    from InvocationCard invocationCardToCheck in notEmptyOpponent
                                                    where invocationCardToCheck.Nom != invocationCard.Nom
                                                    where invocationCardToCheck.GetCurrentDefense() >
                                                          invocationCard.GetCurrentDefense()
                                                    select invocationCardToCheck);
                                            }
                                                break;
                                        }
                                    }
                                        break;
                                    case PermEffect.ProtectBehind:
                                    {
                                        var nameCard = values[i];
                                        if (notEmptyOpponent.Any(invocationCardToCheck =>
                                                invocationCardToCheck.Nom == nameCard))
                                        {
                                            notEmptyOpponent.Remove(invocationCard);
                                        }
                                        else if (nameCard == "any")
                                        {
                                            if (moreDefInvocationCards.Count > 0)
                                            {
                                                notEmptyOpponent.Remove(invocationCard);
                                            }
                                        }
                                    }
                                        break;
                                    case PermEffect.CanOnlyAttackIt:
                                        break;
                                    case PermEffect.GiveStat:
                                        break;
                                    case PermEffect.IncreaseAtk:
                                        break;
                                    case PermEffect.IncreaseDef:
                                        break;
                                    case PermEffect.Family:
                                        break;
                                    case PermEffect.PreventInvocationCards:
                                        break;
                                    case PermEffect.ImpossibleToBeAffectedByEffect:
                                        break;
                                    case PermEffect.NumberTurn:
                                        break;
                                    case PermEffect.checkCardsOnField:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }

                        if (equipmentCard != null)
                        {
                            var permEquipmentEffect = equipmentCard.EquipmentPermEffect;
                            if (permEquipmentEffect != null)
                            {
                                if (permEquipmentEffect.Keys.Contains(PermanentEffect.PreventAttackOnInvocation))
                                {
                                    if (notEmptyOpponent.Contains(invocationCard))
                                    {
                                        notEmptyOpponent.Remove(invocationCard);
                                        hypnoBoobOnly = notEmptyOpponent.Count == 0;
                                    }
                                }
                            }
                        }
                    }

                    if (notEmptyOpponent.Count == 0 && !hypnoBoobOnly)
                    {
                        notEmptyOpponent.Add(player);
                    }
                    else
                    {
                        // Check effect card
                        if (attackPlayerEffectCard.Select(effectCard => effectCard.GetEffectCardEffect())
                            .Where(effectCardEffect => effectCardEffect != null).Any(effectCardEffect =>
                                effectCardEffect.Keys.Contains(Effect.AttackDirectly)))
                        {
                            notEmptyOpponent = new List<Card>
                            {
                                player
                            };
                        }
                    }
                }
            }

            DisplayOpponentMessageBox(notEmptyOpponent);
            stopDetectClicking = true;
        }

        private void DisplayOpponentMessageBox(List<Card> invocationCards)
        {
            nextPhaseButton.SetActive(false);

            if (invocationCards.Count > 0)
            {
                var message =
                    MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choisis ton adversaire :", invocationCards);
                message.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    var invocationCard =
                        (InvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();

                    if (invocationCard != null)
                    {
                        opponent = invocationCard;
                        ComputeAttack();
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
            }
            else
            {
                MessageBox.CreateOkMessageBox(canvas, "Attention",
                    "Tu ne peux pas attaquer le joueur ni ses invocations");
            }
        }

        private void ComputeAttack()
        {
            var attack = attacker.GetCurrentAttack();
            var defenseOpponent = opponent.GetCurrentDefense();

            var diff = defenseOpponent - attack;

            attacker.AttackTurnDone();
            invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = attacker.CanAttack();

            if (opponent.Nom == "Player")
            {
                // Directly attack the player
                if (IsP1Turn)
                {
                    var playerStatus = p2.GetComponent<PlayerStatus>();
                    if (playerStatus.NumberShield > 0)
                    {
                        playerStatus.DecrementShield();
                    }
                    else
                    {
                        playerStatus.ChangePv(diff);
                    }
                }
                else
                {
                    var playerStatus = p1.GetComponent<PlayerStatus>();
                    if (playerStatus.NumberShield > 0)
                    {
                        playerStatus.DecrementShield();
                    }
                    else
                    {
                        playerStatus.ChangePv(diff);
                    }
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
                    DealWithEqualityAttack();
                }
                else
                {
                    DealWithGoodAttack(diff);
                }
            }

            // Check if one player die
            var p1Pv = p1.GetComponent<PlayerStatus>().GetCurrentPv();
            var p2Pv = p2.GetComponent<PlayerStatus>().GetCurrentPv();

            if (p1Pv <= 0)
            {
                phaseId = 5;
            }
            else if (p2Pv <= 0)
            {
                phaseId = 5;
            }
        }

        private void RemoveCombineEffectCard(List<EffectCard> effectCards, List<Card> yellowCards)
        {
            foreach (var effectCard in effectCards.Where(effectCard => effectCard.Nom == "Attaque de la tour Eiffel"))
            {
                effectCards.Remove(effectCard);
                yellowCards.Add(effectCard);
                break;
            }
        }

        /**
     * Attack that kill the opponent
     */
        private void DealWithGoodAttack(float diff)
        {
            if (opponent is SuperInvocationCard superOpponent)
            {
                ComputeGoodAttackSuperInvocationCard(diff, superOpponent);
            }
            else if (!IsProtectedByEquipment(opponent, !IsP1Turn))
            {
                opponent.IncrementNumberDeaths();
                ComputeGoodAttack(diff);
            }
        }

        private void ComputeGoodAttack(float diff)
        {
            var playerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
            var playerStatus = IsP1Turn ? p2.GetComponent<PlayerStatus>() : p1.GetComponent<PlayerStatus>();

            playerStatus.ChangePv(diff);

            if (opponent.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(opponent, !IsP1Turn);
                if (!opponent.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                {
                    playerCards.SendInvocationCardToYellowTrash(opponent);
                }
                else
                {
                    opponent.SetBonusAttack(0);
                    opponent.SetBonusDefense(0);
                    opponent.UnblockAttack();
                }
            }
            else
            {
                playerCards.SendInvocationCardToYellowTrash(opponent);
            }
        }

        private void ComputeGoodAttackSuperInvocationCard(float diff, SuperInvocationCard superOpponent)
        {
            var playerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
            var playerStatus = IsP1Turn ? p2.GetComponent<PlayerStatus>() : p1.GetComponent<PlayerStatus>();

            playerStatus.ChangePv(diff);
            RemoveCombineEffectCard(playerCards.effectCards,
                playerCards.yellowTrash);

            foreach (var combineCard in superOpponent.invocationCards)
            {
                combineCard.IncrementNumberDeaths();
                if (combineCard.GetInvocationDeathEffect() != null)
                {
                    DealWithDeathEffect(combineCard, !IsP1Turn);
                    if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                    {
                        playerCards.SendCardToYellowTrash(combineCard);
                    }
                    else
                    {
                        combineCard.SetBonusAttack(0);
                        combineCard.SetBonusDefense(0);
                        combineCard.ResetNewTurn();
                    }
                }
                else
                {
                    playerCards.SendCardToYellowTrash(combineCard);
                }
            }

            playerCards.RemoveSuperInvocation(superOpponent);
        }

        /**
     * Attack that kill the attacker
     */
        private void DealWithHurtAttack(float diff)
        {
            if (attacker is SuperInvocationCard superAttacker)
            {
                ComputeHurtAttackSuperInvocationCard(diff, superAttacker);
            }
            else
            {
                ComputeHurtAttack(diff);
            }
        }

        private bool IsProtectedByEquipment(InvocationCard invocationCard, bool isP1)
        {
            var invocationEquipment = invocationCard.GetEquipmentCard();
            if (invocationEquipment == null) return false;
            var instantEffectEquipment = invocationEquipment.EquipmentInstantEffect;
            if (!instantEffectEquipment.Keys.Contains(InstantEffect.ProtectInvocation)) return false;
            var playerCards = isP1 ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            playerCards.yellowTrash.Add(invocationEquipment);
            invocationCard.SetEquipmentCard(null);
            return true;
        }

        private void ComputeHurtAttack(float diff)
        {
            var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            var playerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();

            if (IsProtectedByEquipment(attacker, IsP1Turn)) return;
            attacker.IncrementNumberDeaths();
            playerStatus.ChangePv(-diff);
            if (attacker.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(attacker, IsP1Turn);
                if (!attacker.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                {
                    playerCards.SendInvocationCardToYellowTrash(cardSelected as InvocationCard);
                }
                else
                {
                    attacker.SetBonusAttack(0);
                    attacker.SetBonusDefense(0);
                    attacker.ResetNewTurn();
                }
            }
            else
            {
                playerCards.SendInvocationCardToYellowTrash(cardSelected as InvocationCard);
            }
        }

        private void ComputeHurtAttackSuperInvocationCard(float diff, SuperInvocationCard superAttacker)
        {
            var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            var playerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();
            RemoveCombineEffectCard(playerCards.effectCards,
                playerCards.yellowTrash);
            playerStatus.ChangePv(-diff);

            foreach (var combineCard in superAttacker.invocationCards)
            {
                combineCard.IncrementNumberDeaths();
                if (combineCard.GetInvocationDeathEffect() != null)
                {
                    DealWithDeathEffect(combineCard, IsP1Turn);
                    if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                    {
                        playerCards.SendCardToYellowTrash(combineCard);
                    }
                    else
                    {
                        combineCard.SetBonusAttack(0);
                        combineCard.SetBonusDefense(0);
                        combineCard.ResetNewTurn();
                    }
                }
                else
                {
                    playerCards.SendCardToYellowTrash(combineCard);
                }
            }

            playerCards.RemoveSuperInvocation(superAttacker);
        }

        private void DealWithEqualityAttack()
        {
            if (attacker is SuperInvocationCard || opponent is SuperInvocationCard)
            {
                var superAttacker = attacker as SuperInvocationCard;
                var superOpponent = opponent as SuperInvocationCard;
                if (superAttacker)
                {
                    foreach (var combineCard in superAttacker.invocationCards)
                    {
                        combineCard.IncrementNumberDeaths();
                        ComputeEqualityAttackSuperAttacker(combineCard);
                    }

                    if (IsP1Turn)
                    {
                        p1.GetComponent<PlayerCards>().RemoveSuperInvocation(superAttacker);
                    }
                    else
                    {
                        p2.GetComponent<PlayerCards>().RemoveSuperInvocation(superAttacker);
                    }
                }
                else if (!IsProtectedByEquipment(attacker, IsP1Turn))
                {
                    attacker.IncrementNumberDeaths();
                    ComputeEqualityAttacker();
                }

                if (superOpponent)
                {
                    foreach (var combineCard in superOpponent.invocationCards)
                    {
                        combineCard.IncrementNumberDeaths();
                        ComputeEqualityAttackSuperOpponent(combineCard);
                    }

                    if (IsP1Turn)
                    {
                        p2.GetComponent<PlayerCards>().RemoveSuperInvocation(superOpponent);
                    }
                    else
                    {
                        p1.GetComponent<PlayerCards>().RemoveSuperInvocation(superOpponent);
                    }
                }
                else if (!IsProtectedByEquipment(opponent, !IsP1Turn))
                {
                    opponent.IncrementNumberDeaths();
                    ComputeEqualityOpponent();
                }
            }
            else
            {
                if (!IsProtectedByEquipment(attacker, IsP1Turn))
                {
                    attacker.IncrementNumberDeaths();
                    ComputeEqualityAttacker();
                }

                if (!IsProtectedByEquipment(opponent, !IsP1Turn))
                {
                    opponent.IncrementNumberDeaths();
                    ComputeEqualityOpponent();
                }
            }
        }

        private void ComputeEqualityOpponent()
        {
            var playerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
            if (opponent.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(opponent, !IsP1Turn);
                if (!opponent.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                {
                    playerCard.SendInvocationCardToYellowTrash(opponent);
                }
                else
                {
                    opponent.SetBonusAttack(0);
                    opponent.SetBonusDefense(0);
                    opponent.ResetNewTurn();
                }
            }
            else
            {
                playerCard.SendInvocationCardToYellowTrash(opponent);
            }
        }

        private void ComputeEqualityAttacker()
        {
            var playerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            if (attacker.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(attacker, IsP1Turn);
                if (!attacker.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                {
                    playerCard.SendInvocationCardToYellowTrash(attacker);
                }
                else
                {
                    attacker.SetBonusAttack(0);
                    attacker.SetBonusDefense(0);
                    attacker.ResetNewTurn();
                }
            }
            else
            {
                playerCard.SendInvocationCardToYellowTrash(attacker);
            }
        }

        private void ComputeEqualityAttackSuperAttacker(InvocationCard combineCard)
        {
            var playerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();

            RemoveCombineEffectCard(playerCard.effectCards,
                playerCard.yellowTrash);
            if (combineCard.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(combineCard, IsP1Turn);
                if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                {
                    playerCard.SendCardToYellowTrash(combineCard);
                }
                else
                {
                    combineCard.SetBonusAttack(0);
                    combineCard.SetBonusDefense(0);
                    combineCard.ResetNewTurn();
                }
            }
            else
            {
                playerCard.SendCardToYellowTrash(combineCard);
            }
        }

        private void ComputeEqualityAttackSuperOpponent(InvocationCard combineCard)
        {
            var playerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();

            RemoveCombineEffectCard(playerCard.effectCards,
                playerCard.yellowTrash);
            if (combineCard.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffect(combineCard, !IsP1Turn);
                if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                {
                    playerCard.SendCardToYellowTrash(combineCard);
                }
                else
                {
                    combineCard.SetBonusAttack(0);
                    combineCard.SetBonusDefense(0);
                    combineCard.ResetNewTurn();
                }
            }
            else
            {
                playerCard.SendCardToYellowTrash(combineCard);
            }
        }

        /**
     * invocationCard = card that's going to die
     */
        private void DealWithDeathEffect(InvocationCard invocationCard, bool isP1Card)
        {
            var invocationDeathEffect = invocationCard.GetInvocationDeathEffect();
            var keys = invocationDeathEffect.Keys;
            var values = invocationDeathEffect.Values;

            for (var i = 0; i < keys.Count; i++)
            {
                switch (keys[i])
                {
                    case DeathEffect.ComeBackToHand:
                        ComeBackToHandDeathEffect(invocationCard, isP1Card, values, i);
                        break;
                    case DeathEffect.KillAlsoOtherCard:
                        KillAlsoOtherCardDeathEffect();
                        break;
                    case DeathEffect.GetSpecificCard:
                        break;
                    case DeathEffect.GetCardSource:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void KillAlsoOtherCardDeathEffect()
        {
            var playerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            var opponentPlayerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
            playerCard.SendCardToYellowTrash(attacker);
            opponentPlayerCard.SendCardToYellowTrash(opponent);
        }

        private void ComeBackToHandDeathEffect(InvocationCard invocationCard, bool isP1Card, IReadOnlyList<string> values,
            int i)
        {
            var isParsed = int.TryParse(values[i], out var number);
            if (isParsed)
            {
                if (invocationCard.GetNumberDeaths() > number)
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
                    SendCardToHand(invocationCard, isP1Card);
                }
            }
            else
            {
                SendCardToHand(invocationCard, isP1Card);
            }
        }

        private void SendCardToHand(InvocationCard invocationCard, bool isP1Card)
        {
            var playerCards = isP1Card ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            var opponentPlayerCards = isP1Card ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();

            if (invocationCard.IsControlled)
            {
                playerCards.invocationCards.Remove(invocationCard);
                cardLocation.RemovePhysicalCard(invocationCard);
                opponentPlayerCards.SendCardToHand(invocationCard);
            }
            else
            {
                playerCards.SendCardToHand(invocationCard);
            }
        }

        private void DisplayCurrentCard(Card card)
        {
            bigImageCard.SetActive(true);
            bigImageCard.GetComponent<Image>().material = card.MaterialCard;
        }

        private void Draw()
        {
            DoDraw();
            numberOfTurn++;
            phaseId++;
            roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pose";
        }

        private void DoDraw()
        {
            var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            playerCards.ResetInvocationCardNewTurn();

            DrawPlayerCard(playerCards);

            var invocationCards = playerCards.invocationCards;
            var effectCards = playerCards.effectCards;

            foreach (var effectCard in effectCards.Where(effectCard => effectCard.checkTurn))
            {
                void PositiveAction()
                {
                    var playerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();
                    playerStatus.ChangePv(effectCard.affectPv);
                }

                void NegativeAction()
                {
                    foreach (var invocationCard in invocationCards)
                    {
                        invocationCard.SetCurrentFamily(null);
                    }

                    playerCards.yellowTrash.Add(effectCard);
                    playerCards.effectCards.Remove(effectCard);
                }

                if (IsP1Turn)
                {
                    MessageBox.CreateSimpleMessageBox(canvas, "Action requise",
                        "Veux-tu prolonger l'effet de " + effectCard.Nom + " pour 1 tour pour " + effectCard.affectPv +
                        " points de vie ?", PositiveAction, NegativeAction);
                }

            }
        }

        private void DrawPlayerCard(PlayerCards playerCards)
        {
            var canSkipDraw = false;

            if (playerCards.field != null && !playerCards.IsFieldDesactivate)
            {
                var fieldCardEffect = playerCards.field.FieldCardEffect;

                var keys = fieldCardEffect.Keys;
                var values = fieldCardEffect.Values;

                for (var i = 0; i < keys.Count; i++)
                {
                    var value = values[i];
                    switch (keys[i])
                    {
                        case FieldEffect.GetCard:
                        {
                            GetCardFieldEffect(playerCards, value, ref canSkipDraw);
                        }
                            break;
                        case FieldEffect.DrawCard:
                        {
                            DrawCardFieldEffect(playerCards, value, ref canSkipDraw);
                        }
                            break;
                        case FieldEffect.Life:
                        {
                            LifeFieldEffect(playerCards, value);
                        }
                            break;
                        case FieldEffect.ATK:
                            break;
                        case FieldEffect.DEF:
                            break;
                        case FieldEffect.Change:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (canSkipDraw) return;
            {
                var size = playerCards.deck.Count;
                if (size > 0)
                {
                    var c = playerCards.deck[size - 1];
                    playerCards.handCards.Add(c);
                    playerCards.deck.RemoveAt(size - 1);
                }
                else
                {
                    p1.GetComponent<PlayerStatus>().GetCurrentPv();
                    p2.GetComponent<PlayerStatus>().GetCurrentPv();

                    phaseId = 5;
                }
            }
        }

        private void LifeFieldEffect(PlayerCards playerCards, string value)
        {
            var pvPerAlly = float.Parse(value);
            var family = playerCards.field.GetFamily();
            var numberFamilyOnField =
                playerCards.invocationCards.Count(invocationCard =>
                    invocationCard.GetFamily().Contains(family));
            if (IsP1Turn)
            {
                p1.GetComponent<PlayerStatus>().ChangePv(pvPerAlly * numberFamilyOnField);
            }
            else
            {
                p2.GetComponent<PlayerStatus>().ChangePv(pvPerAlly * numberFamilyOnField);
            }
        }

        private void DrawCardFieldEffect(PlayerCards playerCards, string value, ref bool canSkipDraw)
        {
            var numberCardToTake = int.Parse(value);
            canSkipDraw = true;
            var size = playerCards.deck.Count;
            if (size >= numberCardToTake)
            {
                for (int j = size - 1; j >= 0 && j > (size - 1 - numberCardToTake); j--)
                {
                    playerCards.handCards.Add(playerCards.deck[j]);
                    playerCards.deck.RemoveAt(j);
                }
            }
            else if (size > 0)
            {
                var c = playerCards.deck[size - 1];
                playerCards.handCards.Add(c);
                playerCards.deck.RemoveAt(size - 1);
            }
            else
            {
                p1.GetComponent<PlayerStatus>().GetCurrentPv();
                p2.GetComponent<PlayerStatus>().GetCurrentPv();

                phaseId = 5;
            }
        }

        private void GetCardFieldEffect(PlayerCards playerCards, string value, ref bool canSkipDraw)
        {
            if (!Enum.TryParse(value, out CardFamily cardFamily)) return;
            var familyCards = (from deckCard in playerCards.deck
                where deckCard.Type == CardType.Invocation
                select (InvocationCard)deckCard
                into invocationCard
                where invocationCard.GetFamily().Contains(cardFamily)
                select invocationCard).Cast<Card>().ToList();
            familyCards.AddRange(from fieldCard in playerCards.yellowTrash
                where fieldCard.Type == CardType.Invocation
                select (InvocationCard)fieldCard
                into invocationCard
                where invocationCard.GetFamily().Contains(cardFamily)
                select invocationCard);

            if (familyCards.Count <= 0) return;
            canSkipDraw = true;

            void PositiveAction()
            {
                var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                    "Choix de la carte à récupérer", familyCards);
                messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    var selectedCard =
                        (InvocationCard)messageBox.GetComponent<MessageBox>().GetSelectedCard();
                    if (selectedCard != null)
                    {
                        if (playerCards.deck.Contains(selectedCard))
                        {
                            playerCards.handCards.Add(selectedCard);
                            playerCards.deck.Remove(selectedCard);
                        }
                        else
                        {
                            playerCards.handCards.Add(selectedCard);
                            playerCards.yellowTrash.Remove(selectedCard);
                        }

                        Destroy(messageBox);
                    }
                    else
                    {
                        messageBox.SetActive(false);

                        void OkAction()
                        {
                            messageBox.SetActive(true);
                        }

                        MessageBox.CreateOkMessageBox(canvas, "Action requise",
                            "Tu dois prendre une carte maintenant", OkAction);
                    }
                };
                messageBox.GetComponent<MessageBox>().NegativeAction = () =>
                {
                    messageBox.SetActive(false);

                    void OkAction()
                    {
                        messageBox.SetActive(true);
                    }

                    MessageBox.CreateOkMessageBox(canvas, "Action requise",
                        "Tu dois prendre une carte maintenant", OkAction);
                };
            }

            void NegativeAction()
            {
                var size = playerCards.deck.Count;
                if (size > 0)
                {
                    var c = playerCards.deck[size - 1];
                    playerCards.handCards.Add(c);
                    playerCards.deck.RemoveAt(size - 1);
                }
                else
                {
                    var p1Pv = p1.GetComponent<PlayerStatus>().GetCurrentPv();
                    var p2Pv = p2.GetComponent<PlayerStatus>().GetCurrentPv();
                    if (p1Pv > p2Pv)
                    {
                    }
                    else
                    {
                    }

                    phaseId = 5;
                }
            }

            if (IsP1Turn)
            {
                MessageBox.CreateSimpleMessageBox(canvas, "Proposition",
                    "Veux-tu sauter ta phase de pioche pour aller directement chercher une carte de la famille " +
                    value + " dans ton deck ou ta poubelle jaune ?", PositiveAction,
                    NegativeAction);
            }
        
        }

        public void NextRound()
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
            PlayerStatus currentPlayerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();
            PlayerStatus opponentPlayerStatus =
                IsP1Turn ? p2.GetComponent<PlayerStatus>() : p1.GetComponent<PlayerStatus>();
            List<EffectCard> effectCards = currentPlayerCard.effectCards;
            List<EffectCard> opponentEffectCards = opponentPlayerCard.effectCards;

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
        }

        private static void DealWithEndEffect(PlayerCards currentPlayerCard, PlayerCards opponentPlayerCard,
            PlayerStatus playerStatus, List<EffectCard> effectCards)
        {
            List<EffectCard> effectCardsToDelete = new List<EffectCard>();

            foreach (var effectCard in effectCards)
            {
                if (effectCard.GetLifeTime() == 1)
                {
                    effectCard.DecrementLifeTime();
                    effectCardsToDelete.Add(effectCard);

                    var effectCardEffect = effectCard.GetEffectCardEffect();
                    if (effectCardEffect == null) continue;
                    var keys = effectCardEffect.Keys;

                    foreach (var key in keys)
                    {
                        switch (key)
                        {
                            case Effect.RevertStat:
                            {
                                RevertStatEffect(currentPlayerCard, opponentPlayerCard);
                            }
                                break;
                            case Effect.DivideInvocation:
                            {
                                DivideInvocationEffect(opponentPlayerCard);
                            }
                                break;
                            case Effect.SkipFieldsEffect:
                            {
                                SkipFieldsEffect(currentPlayerCard, opponentPlayerCard);
                            }
                                break;
                            case Effect.AffectPv:
                                break;
                            case Effect.AffectOpponent:
                                break;
                            case Effect.NumberInvocationCard:
                                break;
                            case Effect.NumberHandCard:
                                break;
                            case Effect.DestroyCards:
                                break;
                            case Effect.SacrificeInvocation:
                                break;
                            case Effect.SameFamily:
                                break;
                            case Effect.CheckTurn:
                                break;
                            case Effect.ChangeHandCards:
                                break;
                            case Effect.Sources:
                                break;
                            case Effect.HandMax:
                                break;
                            case Effect.SeeOpponentHand:
                                break;
                            case Effect.RemoveCardOption:
                                break;
                            case Effect.RemoveHand:
                                break;
                            case Effect.RemoveDeck:
                                break;
                            case Effect.SpecialInvocation:
                                break;
                            case Effect.Duration:
                                break;
                            case Effect.Combine:
                                break;
                            case Effect.TakeControl:
                                break;
                            case Effect.NumberAttacks:
                                break;
                            case Effect.SkipAttack:
                                break;
                            case Effect.SeeCards:
                                break;
                            case Effect.ChangeOrder:
                                break;
                            case Effect.AttackDirectly:
                                break;
                            case Effect.ProtectAttack:
                                break;
                            case Effect.ChangeField:
                                break;
                            case Effect.SkipContre:
                                break;
                            case Effect.NumberInvocationCardAttacker:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
                else if (effectCard.GetLifeTime() > 1)
                {
                    effectCard.DecrementLifeTime();
                }
                else
                {
                    var effectCardEffect = effectCard.GetEffectCardEffect();
                    if (effectCardEffect == null) continue;
                    var keys = effectCardEffect.Keys;

                    foreach (var key in keys)
                    {
                        switch (key)
                        {
                            case Effect.ProtectAttack:
                            {
                                ProtectAttackEffect(playerStatus, effectCardsToDelete, effectCard);
                            }
                                break;
                            case Effect.AffectPv:
                                break;
                            case Effect.AffectOpponent:
                                break;
                            case Effect.NumberInvocationCard:
                                break;
                            case Effect.NumberHandCard:
                                break;
                            case Effect.DestroyCards:
                                break;
                            case Effect.SacrificeInvocation:
                                break;
                            case Effect.SameFamily:
                                break;
                            case Effect.CheckTurn:
                                break;
                            case Effect.ChangeHandCards:
                                break;
                            case Effect.Sources:
                                break;
                            case Effect.HandMax:
                                break;
                            case Effect.SeeOpponentHand:
                                break;
                            case Effect.RemoveCardOption:
                                break;
                            case Effect.RemoveHand:
                                break;
                            case Effect.RemoveDeck:
                                break;
                            case Effect.SpecialInvocation:
                                break;
                            case Effect.DivideInvocation:
                                break;
                            case Effect.Duration:
                                break;
                            case Effect.Combine:
                                break;
                            case Effect.RevertStat:
                                break;
                            case Effect.TakeControl:
                                break;
                            case Effect.NumberAttacks:
                                break;
                            case Effect.SkipAttack:
                                break;
                            case Effect.SeeCards:
                                break;
                            case Effect.ChangeOrder:
                                break;
                            case Effect.AttackDirectly:
                                break;
                            case Effect.SkipFieldsEffect:
                                break;
                            case Effect.ChangeField:
                                break;
                            case Effect.SkipContre:
                                break;
                            case Effect.NumberInvocationCardAttacker:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }

            foreach (var effectCard in effectCardsToDelete)
            {
                currentPlayerCard.SendCardToYellowTrash(effectCard);
            }

            // Check if preventInvocationCards/NumberTurn is there
            for (int k = currentPlayerCard.invocationCards.Count - 1; k >= 0; k--)
            {
                var invocationCard = currentPlayerCard.invocationCards[k];
                var permEffect = invocationCard.InvocationPermEffect;
                if (permEffect != null)
                {
                    var keys = permEffect.Keys;
                    var values = permEffect.Values;
                    for (int i = 0; i < keys.Count; i++)
                    {
                        var value = values[i];
                        switch (keys[i])
                        {
                            case PermEffect.PreventInvocationCards:
                            {
                                PreventInvocationCardsPermEffect(currentPlayerCard, invocationCard);
                            }
                                break;
                            case PermEffect.NumberTurn:
                            {
                                NumberTurnPermEffect(currentPlayerCard, value, invocationCard);
                            }
                                break;
                            case PermEffect.CanOnlyAttackIt:
                                break;
                            case PermEffect.GiveStat:
                                break;
                            case PermEffect.IncreaseAtk:
                                break;
                            case PermEffect.IncreaseDef:
                                break;
                            case PermEffect.Family:
                                break;
                            case PermEffect.ProtectBehind:
                                break;
                            case PermEffect.ImpossibleAttackByInvocation:
                                break;
                            case PermEffect.ImpossibleToBeAffectedByEffect:
                                break;
                            case PermEffect.Condition:
                                break;
                            case PermEffect.checkCardsOnField:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                if (!invocationCard.IsControlled) continue;
                invocationCard.FreeCard();
                opponentPlayerCard.invocationCards.Add(invocationCard);
                opponentPlayerCard.RemoveFromSecretHide(invocationCard);
                currentPlayerCard.invocationCards.Remove(invocationCard);
                currentPlayerCard.SendToSecretHide(invocationCard);
            }

            foreach (var invocationCard in currentPlayerCard.invocationCards)
            {
                invocationCard.incrementNumberTurnOnField();
            }
        }

        private static void NumberTurnPermEffect(PlayerCards currentPlayerCard, string value,
            InvocationCard invocationCard)
        {
            var maxTurn = int.Parse(value);
            if (invocationCard.NumberTurnOnField >= maxTurn)
            {
                currentPlayerCard.SendInvocationCardToYellowTrash(invocationCard);
            }
        }

        private static void PreventInvocationCardsPermEffect(PlayerCards currentPlayerCard, InvocationCard invocationCard)
        {
            for (var j = currentPlayerCard.invocationCards.Count - 1; j >= 0; j--)
            {
                var checkInvocationCard = currentPlayerCard.invocationCards[j];
                if (checkInvocationCard.Nom == invocationCard.Nom) continue;
                currentPlayerCard.invocationCards.Remove(checkInvocationCard);
                currentPlayerCard.handCards.Add(checkInvocationCard);
            }
        }

        private static void SkipFieldsEffect(PlayerCards currentPlayerCard, PlayerCards opponentPlayerCard)
        {
            currentPlayerCard.ActivateFieldCardEffect();
            opponentPlayerCard.ActivateFieldCardEffect();
        }

        private static void ProtectAttackEffect(PlayerStatus playerStatus, List<EffectCard> effectCardsToDelete,
            EffectCard effectCard)
        {
            if (playerStatus.NumberShield == 0)
            {
                effectCardsToDelete.Add(effectCard);
            }
        }

        private static void DivideInvocationEffect(PlayerCards opponentPlayerCard)
        {
            foreach (var opponentInvocationCard in opponentPlayerCard.invocationCards)
            {
                var newBonusDef = opponentInvocationCard.GetBonusDefense() +
                                  opponentInvocationCard.GetCurrentDefense();
                opponentInvocationCard.SetBonusDefense(newBonusDef);
            }
        }

        private static void RevertStatEffect(PlayerCards currentPlayerCard, PlayerCards opponentPlayerCard)
        {
            var invocationCards1 = currentPlayerCard.invocationCards;
            var invocationCards2 = opponentPlayerCard.invocationCards;

            foreach (var card in invocationCards1)
            {
                var newBonusAttack = card.GetCurrentDefense() - card.GetAttack();
                var newBonusDefense = card.GetCurrentAttack() - card.GetDefense();
                card.SetBonusDefense(newBonusDefense);
                card.SetBonusAttack(newBonusAttack);
            }

            foreach (var card in invocationCards2)
            {
                var newBonusAttack = card.GetCurrentDefense() - card.GetAttack();
                var newBonusDefense = card.GetCurrentAttack() - card.GetDefense();
                card.SetBonusDefense(newBonusDefense);
                card.SetBonusAttack(newBonusAttack);
            }
        }
    }
}