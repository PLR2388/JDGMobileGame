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
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
    public static bool IsP1Turn;

    public int phaseId;

    [SerializeField] protected GameObject playerText;
    [SerializeField] protected GameObject roundText;
    [SerializeField] protected TextMeshProUGUI healthP1Text;
    [SerializeField] protected TextMeshProUGUI healthP2Text;

    [SerializeField] protected GameObject bigImageCard;
    [SerializeField] protected GameObject invocationMenu;
    [SerializeField] protected GameObject nextPhaseButton;
    [SerializeField] protected Transform canvas;

    [SerializeField] protected GameObject p1;

    [SerializeField] protected GameObject p2;

    protected InGameCard player;
    [SerializeField] protected InvocationCard playerInvocationCard;
    [SerializeField] protected GameObject inHandButton;

    [SerializeField] private GameObject playerCamera;

    public static readonly UnityEvent ChangePlayer = new UnityEvent();

    protected InvocationFunctions invocationFunctions;

    protected const float ClickDuration = 2;

    protected CardLocation cardLocation;
    protected bool stopDetectClicking;
    protected bool clicking;
    protected float totalDownTime;
    protected InGameCard cardSelected;
    protected InGameInvocationCard attacker;
    protected InGameInvocationCard opponent;
    protected int numberOfTurn;

    private readonly Vector3 cameraRotation = new Vector3(0, 0, 180);

    private void Awake()
    {
        player = InGameCard.CreateInGameCard(playerInvocationCard, CardOwner.Player2);
    }

    // Start is called before the first frame update
    protected void Start()
    {
        invocationFunctions = GetComponent<InvocationFunctions>();
        cardLocation = GetComponent<CardLocation>();
        IsP1Turn = true;
        ChangeHealthText(PlayerStatus.MaxPv, true);
        ChangeHealthText(PlayerStatus.MaxPv, false);
        p1.GetComponent<PlayerStatus>().SetNumberShield(0);
        p2.GetComponent<PlayerStatus>().SetNumberShield(0);
        PlayerStatus.ChangePvEvent.AddListener(ChangeHealthText);
        Draw();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (phaseId)
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

        MessageBox.CreateSimpleMessageBox(canvas, "Pause", "Veux-tu quitter la partie ?", PositiveAction);
    }

    protected void ChoosePhase()
    {
        invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
        ChoosePhaseMusic();
    }

    private void ChoosePhaseMusic()
    {
        var currentFieldCard = IsP1Turn ? p1.GetComponent<PlayerCards>().FieldCard : p2.GetComponent<PlayerCards>().FieldCard;
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

    protected void ChangeHealthText(float pv, bool isP1)
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

    protected void SeeCardAndApplyAction()
    {
        if (!Input.GetMouseButton(0) || phaseId != 1 || stopDetectClicking) return;
#if UNITY_EDITOR
        var position = Input.mousePosition;
#elif UNITY_ANDROID
        var position = Input.GetTouch(0).position;
#endif
        if (Camera.main == null) return;
        var hit = Physics.Raycast(Camera.main.ScreenPointToRay(position), out var hitInfo);
        if (hit)
        {
            HandleClickDuringDrawPhase(hitInfo);
        }
        else
        {
            bigImageCard.SetActive(false);
        }
    }

    protected void ChooseAttack()
    {
        AudioSystem.Instance.PlayMusic(Music.Fight);
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

    protected void BlockCardJustInvokeIfNeeded()
    {
        /*var ownInvocations = IsP1Turn
            ? p1.GetComponent<PlayerCards>().invocationCards
            : p2.GetComponent<PlayerCards>().invocationCards;
        var invocationsOpponent = IsP1Turn
            ? p2.GetComponent<PlayerCards>().invocationCards
            : p1.GetComponent<PlayerCards>().invocationCards;
        foreach (var ownInvocationCard in from invocationCard in invocationsOpponent
                 where invocationCard.EquipmentCard != null
                 select invocationCard.EquipmentCard
                 into equipment
                 where equipment.EquipmentPermEffect != null &&
                       equipment.EquipmentPermEffect.Keys.Contains(PermanentEffect.BlockOpponentDuringInvocation)
                 from ownInvocationCard in ownInvocations.Where(ownInvocationCard =>
                     ownInvocationCard.NumberOfTurnOnField == 0)
                 select ownInvocationCard)
        {
            ownInvocationCard.BlockAttack();
        }*/
    }

    protected void HandleClickDuringDrawPhase(RaycastHit hitInfo)
    {
        var objectTag = hitInfo.transform.gameObject.tag;
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
                if (cardSelected is InGameInvocationCard invocationCard)
                {
                    attacker = invocationCard;
                    var hasAction = attacker.HasAction();
                    invocationMenu.SetActive(true);
                    invocationMenu.transform.GetChild(0).gameObject.SetActive(false);
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
                    attacker = invocationCard;
#if UNITY_EDITOR
                    var mousePosition = Input.mousePosition;
#elif UNITY_ANDROID
                            var mousePosition = Input.GetTouch(0).position;
#endif
                    invocationMenu.SetActive(true);
                    invocationMenu.transform.GetChild(0).gameObject.SetActive(false);
                    invocationMenu.transform.position = mousePosition;
                    if (attacker.HasAction())
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

    protected virtual void HandleClick(RaycastHit hitInfo)
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
                if (cardSelected is InGameInvocationCard invocationCard)
                {
                    attacker = invocationCard;
                    var canAttack = attacker.CanAttack() && ownPlayerCards.ContainsCardInInvocation(attacker);
                    invocationMenu.SetActive(true);
                    invocationMenu.transform.GetChild(0).gameObject.SetActive(true);
                    invocationMenu.transform.GetChild(1).gameObject.SetActive(false);
                    invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = canAttack;
                    invocationMenu.transform.position = mousePosition;
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
                    invocationMenu.SetActive(true);
                    invocationMenu.transform.GetChild(0).gameObject.SetActive(true);
                    invocationMenu.transform.GetChild(1).gameObject.SetActive(false);
                    invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = canAttack;
                    invocationMenu.transform.position = mousePosition;
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

    protected void DisplayAvailableOpponent()
    {
        var opponentCards = IsP1Turn
            ? p2.GetComponent<PlayerCards>().invocationCards
            : p1.GetComponent<PlayerCards>().invocationCards;
        var attackerEffectCards =
            IsP1Turn ? p1.GetComponent<PlayerCards>().effectCards : p2.GetComponent<PlayerCards>().effectCards;
        DisplayCards(opponentCards, attackerEffectCards);
    }

    protected bool IsSpecialActionPossible()
    {
        var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var opponentPlayerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        return attacker.Abilities.TrueForAll(ability =>
            ability.IsActionPossible(attacker, playerCards, opponentPlayerCards));
    }

    protected void UseSpecialAction()
    {
        var abilities = attacker.Abilities;
        var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var opponentPlayerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        foreach (var ability in abilities)
        {
            ability.OnCardActionTouched(canvas, attacker, playerCards, opponentPlayerCards);
        }
    }

    /**
     * Return the list of available opponents
     */
    protected virtual void DisplayCards(ObservableCollection<InGameInvocationCard> invocationCards,
        List<InGameEffectCard> attackPlayerEffectCard)
    {
        var notEmptyOpponent = invocationCards.Where(t => t != null && t.Title != null).Cast<InGameCard>().ToList();

        if (notEmptyOpponent.Count == 0)
        {
            notEmptyOpponent.Add(player);
        }
        /*else if (attacker.EquipmentCard != null && attacker.EquipmentCard.EquipmentInstantEffect != null &&
                 attacker.EquipmentCard.EquipmentInstantEffect.Keys.Contains(InstantEffect.DirectAtk))
        {
            notEmptyOpponent = new List<InGameCard>
            {
                player
            };
        }*/
        else
        {
            var newList = (from card in notEmptyOpponent
                let invocationCard = card as InGameInvocationCard
                where invocationCard?.Aggro == true
                select card).ToList();

            if (newList.Count > 0)
            {
                notEmptyOpponent = newList;
            }
            else
            {
                var hypnoBoobOnly = false;
                for (var j = notEmptyOpponent.Count - 1; j >= 0; j--)
                {
                    var invocationCard = notEmptyOpponent[j] as InGameInvocationCard;
                    var equipmentCard = invocationCard.EquipmentCard;

                    if (invocationCard.CantBeAttack)
                    {
                        notEmptyOpponent.Remove(invocationCard);
                        continue;
                    }

                    /*if (permEffect != null)
                    {
                        var keys = permEffect.Keys;
                        var values = permEffect.Values;

                        List<InGameInvocationCard> moreDefInvocationCards = new List<InGameInvocationCard>();

                        for (var i = 0; i < keys.Count; i++)
                        {
                            switch (keys[i])
                            {
                                case PermEffect.ImpossibleAttackByInvocation:
                                {
                                    if (Enum.TryParse(values[i], out CardFamily family))
                                    {
                                        if ((from cardToCheck in notEmptyOpponent
                                                where cardToCheck.Title != invocationCard.Title
                                                select (InGameInvocationCard)cardToCheck).Any(invocationCardToCheck =>
                                                invocationCardToCheck.Families.Contains(family)))
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
                                                from InGameInvocationCard invocationCardToCheck in notEmptyOpponent
                                                where invocationCardToCheck.Title != invocationCard.Title
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
                                            invocationCardToCheck.Title == nameCard))
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
                    }*/

                    if (equipmentCard != null)
                    {
                        /*var permEquipmentEffect = equipmentCard.EquipmentPermEffect;
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
                        } */
                    }
                }

                if ((notEmptyOpponent.Count == 0 && !hypnoBoobOnly) || attackPlayerEffectCard.Any(card => card.EffectAbilities.Any(ability => ability is DirectAttackEffectAbility)))
                {
                    notEmptyOpponent.Add(player);
                }
                else
                {
                    // Check effect card
                    /*if (attackPlayerEffectCard.Select(effectCard => effectCard.EffectCardEffect)
                        .Where(effectCardEffect => effectCardEffect != null).Any(effectCardEffect =>
                            effectCardEffect.Keys.Contains(Effect.AttackDirectly)))
                    {
                        notEmptyOpponent = new List<InGameCard>
                        {
                            player
                        };
                    }*/
                }
            }
        }

        DisplayOpponentMessageBox(notEmptyOpponent);
        stopDetectClicking = true;
    }

    protected virtual void DisplayOpponentMessageBox(List<InGameCard> invocationCards)
    {
        nextPhaseButton.SetActive(false);

        if (invocationCards.Count > 0)
        {
            var message =
                MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choisis ton adversaire :", invocationCards);
            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var invocationCard =
                    message.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;

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

    protected void ComputeAttack()
    {
        var attack = attacker.GetCurrentAttack();
        var defenseOpponent = opponent.GetCurrentDefense();

        var playerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var opponentPlayerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        var playerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();
        var opponentPlayerStatus = IsP1Turn ? p2.GetComponent<PlayerStatus>() : p1.GetComponent<PlayerStatus>();

        var diff = defenseOpponent - attack;

        attacker.AttackTurnDone();
        invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = attacker.CanAttack();

        var opponentAbilities = opponent.Abilities;
        if (opponentAbilities.Count > 0)
        {
            for (var i = 0; i < opponentAbilities.Count; i++)
            {
                opponentAbilities[i].OnCardAttacked(canvas, opponent, attacker, playerCard, opponentPlayerCard,
                    playerStatus, opponentPlayerStatus);
            }
        }
        else
        {
            if (opponent.Title == "Player")
            {
                // Directly attack the player
                if (opponentPlayerStatus.NumberShield > 0)
                {
                    opponentPlayerStatus.DecrementShield();
                }
                else
                {
                    opponentPlayerStatus.ChangePv(diff);
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
        }


        // Check if one player die


        if (playerStatus.GetCurrentPv() <= 0)
        {
            phaseId = 5;
            GameOver();
        }
        else if (opponentPlayerStatus.GetCurrentPv() <= 0)
        {
            phaseId = 5;
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

    /**
     * Attack that kill the opponent
     */
    protected void DealWithGoodAttack(float diff)
    {
        if (opponent is InGameSuperInvocationCard superOpponent)
        {
            ComputeGoodAttackSuperInvocationCard(diff, superOpponent);
        }
        else if (!IsProtectedByEquipment(opponent, !IsP1Turn))
        {
            opponent.IncrementNumberDeaths();
            ComputeGoodAttack(diff);
        }
    }

    protected void ComputeGoodAttack(float diff)
    {
        var playerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        var playerStatus = IsP1Turn ? p2.GetComponent<PlayerStatus>() : p1.GetComponent<PlayerStatus>();

        playerStatus.ChangePv(diff);

        var abilities = opponent.Abilities;

        if (abilities.Count > 0)
        {
            foreach (var ability in abilities)
            {
                ability.OnCardDeath(canvas, opponent, playerCards);
            }
        }
        else
        {
            playerCards.SendInvocationCardToYellowTrash(opponent);
        }


        /*if (opponent.InvocationDeathEffect != null)
        {
            DealWithDeathEffect(opponent, !IsP1Turn);
            if (!opponent.InvocationDeathEffect.Keys.Contains(DeathEffect.ComeBackToHand))
            {
                playerCards.SendInvocationCardToYellowTrash(opponent);
            }
            else
            {
                opponent.Attack = opponent.baseInvocationCard.BaseInvocationCardStats.Attack;
                opponent.Defense = opponent.baseInvocationCard.BaseInvocationCardStats.Defense;
                opponent.UnblockAttack();
            }
        }
        else
        {
            playerCards.SendInvocationCardToYellowTrash(opponent);
        }*/
    }

    protected void ComputeGoodAttackSuperInvocationCard(float diff, InGameSuperInvocationCard superOpponent)
    {
        var playerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        var playerStatus = IsP1Turn ? p2.GetComponent<PlayerStatus>() : p1.GetComponent<PlayerStatus>();

        playerStatus.ChangePv(diff);
        RemoveCombineEffectCard(playerCards.effectCards,
            playerCards.yellowCards);

        foreach (var combineCard in superOpponent.invocationCards)
        {
            combineCard.IncrementNumberDeaths();
            /*if (combineCard.InvocationDeathEffect != null)
            {
                DealWithDeathEffect(combineCard, !IsP1Turn);
                if (!combineCard.InvocationDeathEffect.Keys.Contains(DeathEffect.ComeBackToHand))
                {
                    playerCards.SendCardToYellowTrash(combineCard);
                }
                else
                {
                    combineCard.Attack = combineCard.baseInvocationCard.BaseInvocationCardStats.Attack;
                    combineCard.Defense = combineCard.baseInvocationCard.BaseInvocationCardStats.Defense;
                    combineCard.ResetNewTurn();
                }
            }
            else
            {
                playerCards.SendCardToYellowTrash(combineCard);
            }*/
        }

        playerCards.RemoveSuperInvocation(superOpponent);
    }

    /**
     * Attack that kill the attacker
     */
    protected void DealWithHurtAttack(float diff)
    {
        if (attacker is InGameSuperInvocationCard superAttacker)
        {
            ComputeHurtAttackSuperInvocationCard(diff, superAttacker);
        }
        else
        {
            ComputeHurtAttack(diff);
        }
    }

    protected bool IsProtectedByEquipment(InGameInvocationCard invocationCard, bool isP1)
    {
        var invocationEquipment = invocationCard.EquipmentCard;
        if (invocationEquipment == null) return false;
        //var instantEffectEquipment = invocationEquipment.EquipmentInstantEffect;
        //if (!instantEffectEquipment.Keys.Contains(InstantEffect.ProtectInvocation)) return false;
        var playerCards = isP1 ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        playerCards.yellowCards.Add(invocationEquipment);
        invocationCard.SetEquipmentCard(null);
        return true;
    }

    protected void ComputeHurtAttack(float diff)
    {
        var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var playerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();

        if (IsProtectedByEquipment(attacker, IsP1Turn)) return;
        attacker.IncrementNumberDeaths();
        playerStatus.ChangePv(-diff);

        var abilities = attacker.Abilities;
        if (abilities.Count > 0)
        {
            foreach (var ability in abilities)
            {
                ability.OnCardDeath(canvas, attacker, playerCards);
            }
        }
        else
        {
            playerCards.SendInvocationCardToYellowTrash(attacker);
        }


        /*if (attacker.InvocationDeathEffect != null)
        {
            DealWithDeathEffect(attacker, IsP1Turn);
            if (!attacker.InvocationDeathEffect.Keys.Contains(DeathEffect.ComeBackToHand))
            {
                playerCards.SendInvocationCardToYellowTrash(cardSelected as InGameInvocationCard);
            }
            else
            {
                attacker.Attack = attacker.baseInvocationCard.BaseInvocationCardStats.Attack;
                attacker.Defense = attacker.baseInvocationCard.BaseInvocationCardStats.Defense;
                attacker.ResetNewTurn();
            }
        }
        else
        {
            playerCards.SendInvocationCardToYellowTrash(cardSelected as InGameInvocationCard);
        }*/
    }

    protected void ComputeHurtAttackSuperInvocationCard(float diff, InGameSuperInvocationCard superAttacker)
    {
        var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var playerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();
        RemoveCombineEffectCard(playerCards.effectCards,
            playerCards.yellowCards);
        playerStatus.ChangePv(-diff);

        /*foreach (var combineCard in superAttacker.invocationCards)
        {
            combineCard.IncrementNumberDeaths();
            if (combineCard.InvocationDeathEffect != null)
            {
                DealWithDeathEffect(combineCard, IsP1Turn);
                if (!combineCard.InvocationDeathEffect.Keys.Contains(DeathEffect.ComeBackToHand))
                {
                    playerCards.SendCardToYellowTrash(combineCard);
                }
                else
                {
                    combineCard.Attack = combineCard.baseInvocationCard.BaseInvocationCardStats.Attack;
                    combineCard.Defense = combineCard.baseInvocationCard.BaseInvocationCardStats.Defense;
                    combineCard.ResetNewTurn();
                }
            }
            else
            {
                playerCards.SendCardToYellowTrash(combineCard);
            }
        }*/

        playerCards.RemoveSuperInvocation(superAttacker);
    }

    protected void DealWithEqualityAttack()
    {
        if (attacker is InGameSuperInvocationCard || opponent is InGameSuperInvocationCard)
        {
            var superAttacker = attacker as InGameSuperInvocationCard;
            var superOpponent = opponent as InGameSuperInvocationCard;
            if (superAttacker != null)
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

            if (superOpponent != null)
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

    protected void ComputeEqualityOpponent()
    {
        var playerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();

        var abilities = opponent.Abilities;
        if (abilities.Count > 0)
        {
            foreach (var ability in abilities)
            {
                ability.OnCardDeath(canvas, opponent, playerCard);
            }
        }
        else
        {
            playerCard.SendInvocationCardToYellowTrash(opponent);
        }

        /*if (opponent.InvocationDeathEffect != null)
        {
            DealWithDeathEffect(opponent, !IsP1Turn);
            if (!opponent.InvocationDeathEffect.Keys.Contains(DeathEffect.ComeBackToHand))
            {
                playerCard.SendInvocationCardToYellowTrash(opponent);
            }
            else
            {
                opponent.Attack = opponent.baseInvocationCard.BaseInvocationCardStats.Attack;
                opponent.Defense = opponent.baseInvocationCard.BaseInvocationCardStats.Defense;
                opponent.ResetNewTurn();
            }
        }
        else
        {
            playerCard.SendInvocationCardToYellowTrash(opponent);
        }*/
    }

    protected void ComputeEqualityAttacker()
    {
        var playerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();

        var abilities = attacker.Abilities;
        if (abilities.Count > 0)
        {
            foreach (var ability in abilities)
            {
                ability.OnCardDeath(canvas, attacker, playerCard);
            }
        }
        else
        {
            playerCard.SendInvocationCardToYellowTrash(attacker);
        }
        /*if (attacker.InvocationDeathEffect != null)
        {
            DealWithDeathEffect(attacker, IsP1Turn);
            if (!attacker.InvocationDeathEffect.Keys.Contains(DeathEffect.ComeBackToHand))
            {
                playerCard.SendInvocationCardToYellowTrash(attacker);
            }
            else
            {
                attacker.Attack = attacker.baseInvocationCard.BaseInvocationCardStats.Attack;
                attacker.Defense = attacker.baseInvocationCard.BaseInvocationCardStats.Defense;
                attacker.ResetNewTurn();
            }
        }
        else
        {
            playerCard.SendInvocationCardToYellowTrash(attacker);
        }*/
    }

    protected void ComputeEqualityAttackSuperAttacker(InGameInvocationCard combineCard)
    {
        var playerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();

        RemoveCombineEffectCard(playerCard.effectCards,
            playerCard.yellowCards);
        /*if (combineCard.InvocationDeathEffect != null)
        {
            DealWithDeathEffect(combineCard, IsP1Turn);
            if (!combineCard.InvocationDeathEffect.Keys.Contains(DeathEffect.ComeBackToHand))
            {
                playerCard.SendCardToYellowTrash(combineCard);
            }
            else
            {
                combineCard.Attack = combineCard.baseInvocationCard.BaseInvocationCardStats.Attack;
                combineCard.Defense = combineCard.baseInvocationCard.BaseInvocationCardStats.Defense;
                combineCard.ResetNewTurn();
            }
        }
        else
        {
            playerCard.SendCardToYellowTrash(combineCard);
        }*/
    }

    protected void ComputeEqualityAttackSuperOpponent(InGameInvocationCard combineCard)
    {
        var playerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();

        RemoveCombineEffectCard(playerCard.effectCards,
            playerCard.yellowCards);
        // TOOD
        /*if (combineCard.InvocationDeathEffect != null)
        {
            DealWithDeathEffect(combineCard, !IsP1Turn);
            if (!combineCard.InvocationDeathEffect.Keys.Contains(DeathEffect.ComeBackToHand))
            {
                playerCard.SendCardToYellowTrash(combineCard);
            }
            else
            {
                combineCard.Attack = combineCard.baseInvocationCard.BaseInvocationCardStats.Attack;
                combineCard.Defense = combineCard.baseInvocationCard.BaseInvocationCardStats.Defense;
                combineCard.ResetNewTurn();
            }
        }
        else
        {
            playerCard.SendCardToYellowTrash(combineCard);
        }*/
    }

    /**
     * invocationCard = card that's going to die
     */
    protected void DealWithDeathEffect(InGameInvocationCard invocationCard, bool isP1Card)
    {
        /*var invocationDeathEffect = invocationCard.InvocationDeathEffect;
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
        }*/
    }

    protected void KillAlsoOtherCardDeathEffect()
    {
        var playerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var opponentPlayerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        playerCard.SendCardToYellowTrash(attacker);
        opponentPlayerCard.SendCardToYellowTrash(opponent);
    }

    protected void ComeBackToHandDeathEffect(InGameInvocationCard invocationCard, bool isP1Card,
        IReadOnlyList<string> values,
        int i)
    {
        var isParsed = int.TryParse(values[i], out var number);
        if (isParsed)
        {
            if (invocationCard.NumberOfDeaths > number)
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

    protected void SendCardToHand(InGameInvocationCard invocationCard, bool isP1Card)
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

    protected void DisplayCurrentCard(InGameCard card)
    {
        bigImageCard.SetActive(true);
        bigImageCard.GetComponent<Image>().material = card.MaterialCard;
    }

    protected void Draw()
    {
        DoDraw();
        numberOfTurn++;
        phaseId++;
        roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pose";
    }

    private void ApplyOnTurnStart(PlayerCards playerCards,
        PlayerCards opponentPlayerCards,
        PlayerStatus playerStatus,
        PlayerStatus opponentPlayerStatus)
    {
        var copyInvocationCards = playerCards.invocationCards.ToList();
        var copyOpponentInvocationCards = opponentPlayerCards.invocationCards.ToList();

        var equipmentCardsAbilities = copyInvocationCards.Where(invocationCard => invocationCard.EquipmentCard != null)
            .SelectMany(invocationCard => invocationCard.EquipmentCard.EquipmentAbilities).ToList();
        var opponentEquipmentCardsAbilities = copyOpponentInvocationCards.Where(invocationCard => invocationCard.EquipmentCard != null)
            .SelectMany(invocationCard => invocationCard.EquipmentCard.EquipmentAbilities).ToList();

        var copyEffectCards = playerCards.effectCards.ToList();
        var copyOpponentEffectCards = opponentPlayerCards.effectCards.ToList();

        foreach (var invocationCard in copyInvocationCards)
        {
            foreach (var invocationCardAbility in invocationCard.Abilities)
            {
                invocationCardAbility.OnTurnStart(canvas, playerCards, opponentPlayerCards);
            }

            if (invocationCard.EquipmentCard != null)
            {
                foreach (var equipmentCardEquipmentAbility in invocationCard.EquipmentCard.EquipmentAbilities)
                {
                    equipmentCardEquipmentAbility.OnTurnStart(invocationCard);
                }
            }
        }
        
        foreach (var invocationCard in copyOpponentInvocationCards)
        {
            foreach (var invocationCardAbility in invocationCard.Abilities)
            {
                invocationCardAbility.OnTurnStart(canvas, opponentPlayerCards, playerCards);
            }

            if (invocationCard.EquipmentCard != null)
            {
                foreach (var equipmentCardEquipmentAbility in invocationCard.EquipmentCard.EquipmentAbilities)
                {
                    equipmentCardEquipmentAbility.OnTurnStart(invocationCard);
                }
            }
        }

        foreach (var effectCardAbility in copyEffectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectCardAbility.OnTurnStart(canvas, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCards);
        }

        foreach (var effectCardAbility in copyOpponentEffectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectCardAbility.OnTurnStart(canvas, opponentPlayerStatus, opponentPlayerCards, opponentPlayerStatus, playerCards);
        }

        if (playerCards.FieldCard != null)
        {
            foreach (var fieldCardFieldAbility in playerCards.FieldCard.FieldAbilities)
            {
               fieldCardFieldAbility.OnTurnStart(canvas, playerCards, playerStatus); 
            }
        }
    }

    private void DoDraw()
    {
        var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var opponentPlayerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();

        var playerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();
        var opponentPlayerStatus = IsP1Turn ? p2.GetComponent<PlayerStatus>() : p1.GetComponent<PlayerStatus>();

        playerCards.ResetInvocationCardNewTurn();

        ApplyOnTurnStart(playerCards, opponentPlayerCards, playerStatus, opponentPlayerStatus);

        DrawPlayerCard(playerCards, playerStatus);

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
                    invocationCard.Families = invocationCard.baseInvocationCard.BaseInvocationCardStats.Families;
                }

                playerCards.yellowCards.Add(effectCard);
                playerCards.effectCards.Remove(effectCard);
            }

            MessageBox.CreateSimpleMessageBox(canvas, "Action requise",
                "Veux-tu prolonger l'effet de " + effectCard.Title + " pour 1 tour pour " + effectCard.affectPv +
                " points de vie ?", PositiveAction, NegativeAction);
        }
    }

    protected void DrawPlayerCard(PlayerCards playerCards, PlayerStatus playerStatus)
    {

        /*if (playerCards.field != null && !playerCards.IsFieldDesactivate)
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
        }*/

        if (playerStatus.SkipCurrentDraw)
        {
            playerStatus.SkipCurrentDraw = false;
        }
        else
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
                GameOver();
            }
        }
    }

    protected void DrawCardFieldEffect(PlayerCards playerCards, string value, ref bool canSkipDraw)
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
            GameOver();
        }
    }

    protected void GetCardFieldEffect(PlayerCards playerCards, string value, ref bool canSkipDraw)
    {
        if (!Enum.TryParse(value, out CardFamily cardFamily)) return;
        var familyCards = (from deckCard in playerCards.deck
            where deckCard.Type == CardType.Invocation
            select (InGameInvocationCard)deckCard
            into invocationCard
            where invocationCard.Families.Contains(cardFamily)
            select invocationCard).Cast<InGameCard>().ToList();
        familyCards.AddRange(from fieldCard in playerCards.yellowCards
            where fieldCard.Type == CardType.Invocation
            select (InGameInvocationCard)fieldCard
            into invocationCard
            where invocationCard.Families.Contains(cardFamily)
            select invocationCard);

        if (familyCards.Count <= 0) return;
        canSkipDraw = true;

        void PositiveAction()
        {
            var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Choix de la carte à récupérer", familyCards);
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var selectedCard = messageBox.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
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
                        playerCards.yellowCards.Remove(selectedCard);
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
                GameOver();
            }
        }

        MessageBox.CreateSimpleMessageBox(canvas, "Proposition",
            "Veux-tu sauter ta phase de pioche pour aller directement chercher une carte de la famille " +
            value + " dans ton deck ou ta poubelle jaune ?", PositiveAction,
            NegativeAction);
    }

    protected virtual void NextRound()
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

        var playerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();
        if (phaseId == 2 && playerStatus.BlockAttack)
        {
            phaseId = 3;
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
                ChoosePhase();
                break;
            case 2:
                inHandButton.SetActive(false);
                BlockCardJustInvokeIfNeeded();
                roundText.GetComponent<TextMeshProUGUI>().text = "Phase d'attaque";
                break;
        }

        if (phaseId != 3) return;
        PlayerCards currentPlayerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();

        var invocationCards = IsP1Turn
            ? p1.GetComponent<PlayerCards>().invocationCards
            : p2.GetComponent<PlayerCards>().invocationCards;

        foreach (var invocationCard in invocationCards)
        {
            invocationCard.UnblockAttack();
        }

        foreach (var invocationCard in currentPlayerCard.invocationCards)
        {
            invocationCard.incrementNumberTurnOnField();
        }

        IsP1Turn = !IsP1Turn;
        ChangePlayer.Invoke();
        playerCamera.transform.Rotate(cameraRotation);
        playerText.GetComponent<TextMeshProUGUI>().text = IsP1Turn ? "Joueur 1" : "Joueur 2";
        phaseId = 0;
        Draw();
    }

    protected static void NumberTurnPermEffect(PlayerCards currentPlayerCard, string value,
        InGameInvocationCard invocationCard)
    {
        var maxTurn = int.Parse(value);
        if (invocationCard.NumberOfTurnOnField >= maxTurn)
        {
            currentPlayerCard.SendInvocationCardToYellowTrash(invocationCard);
        }
    }

    protected static void PreventInvocationCardsPermEffect(PlayerCards currentPlayerCard,
        InGameInvocationCard invocationCard)
    {
        for (var j = currentPlayerCard.invocationCards.Count - 1; j >= 0; j--)
        {
            var checkInvocationCard = currentPlayerCard.invocationCards[j];
            if (checkInvocationCard.Title == invocationCard.Title) continue;
            currentPlayerCard.invocationCards.Remove(checkInvocationCard);
            currentPlayerCard.handCards.Add(checkInvocationCard);
        }
    }

    protected static void SkipFieldsEffect(PlayerCards currentPlayerCard, PlayerCards opponentPlayerCard)
    {
        //currentPlayerCard.ActivateFieldCardEffect();
        //opponentPlayerCard.ActivateFieldCardEffect();
    }

    protected static void ProtectAttackEffect(PlayerStatus playerStatus, List<InGameEffectCard> effectCardsToDelete,
        InGameEffectCard effectCard)
    {
        if (playerStatus.NumberShield == 0)
        {
            effectCardsToDelete.Add(effectCard);
        }
    }

    protected static void DivideInvocationEffect(PlayerCards opponentPlayerCard)
    {
        foreach (var opponentInvocationCard in opponentPlayerCard.invocationCards)
        {
            opponentInvocationCard.Defense *= 2;
        }
    }

    protected static void RevertStatEffect(PlayerCards currentPlayerCard, PlayerCards opponentPlayerCard)
    {
        var invocationCards1 = currentPlayerCard.invocationCards;
        var invocationCards2 = opponentPlayerCard.invocationCards;

        foreach (var card in invocationCards1)
        {
            var currentAttack = card.Attack;
            var currentDefense = card.Defense;
            card.Attack = currentDefense;
            card.Defense = currentAttack;
        }

        foreach (var card in invocationCards2)
        {
            var currentAttack = card.Attack;
            var currentDefense = card.Defense;
            card.Attack = currentDefense;
            card.Defense = currentAttack;
        }
    }
}