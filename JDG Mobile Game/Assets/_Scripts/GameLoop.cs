using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
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
        var currentFieldCard =
            IsP1Turn ? p1.GetComponent<PlayerCards>().FieldCard : p2.GetComponent<PlayerCards>().FieldCard;
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
            ability.IsActionPossible(playerCards)) && !attacker.CancelEffect;
    }

    protected void UseSpecialAction()
    {
        var abilities = attacker.Abilities;
        var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var opponentPlayerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        foreach (var ability in abilities)
        {
            ability.OnCardActionTouched(canvas, playerCards, opponentPlayerCards);
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
                for (var j = notEmptyOpponent.Count - 1; j >= 0; j--)
                {
                    var invocationCard = (InGameInvocationCard) notEmptyOpponent[j];

                    if (invocationCard.CantBeAttack)
                    {
                        notEmptyOpponent.Remove(invocationCard);
                    }
                }

                if (notEmptyOpponent.Count == 0 || attackPlayerEffectCard.Any(card =>
                        card.EffectAbilities.Any(ability => ability is DirectAttackEffectAbility)))
                {
                    notEmptyOpponent.Add(player);
                }
            }
        }

        if (!notEmptyOpponent.Contains(player) && attacker.CanDirectAttack)
        {
            notEmptyOpponent.Add(player);
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
        else
        {
            opponent.IncrementNumberDeaths();
            ComputeGoodAttack(diff);
        }
    }

    protected void ComputeGoodAttack(float diff)
    {
        var playerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        var opponentPlayerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var playerStatus = IsP1Turn ? p2.GetComponent<PlayerStatus>() : p1.GetComponent<PlayerStatus>();

        playerStatus.ChangePv(diff);

        var abilities = opponent.Abilities;

        if (abilities.Count > 0)
        {
            foreach (var ability in abilities)
            {
                ability.OnCardDeath(canvas, opponent, playerCards, opponentPlayerCards);
            }
        }
        else
        {
            playerCards.SendInvocationCardToYellowTrash(opponent);
        }
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

    protected void ComputeHurtAttack(float diff)
    {
        var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var opponentPlayerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        var playerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();

        attacker.IncrementNumberDeaths();
        playerStatus.ChangePv(-diff);

        var abilities = attacker.Abilities;
        if (abilities.Count > 0)
        {
            foreach (var ability in abilities)
            {
                ability.OnCardDeath(canvas, attacker, playerCards, opponentPlayerCards);
            }
        }
        else
        {
            playerCards.SendInvocationCardToYellowTrash(attacker);
        }
    }

    protected void ComputeHurtAttackSuperInvocationCard(float diff, InGameSuperInvocationCard superAttacker)
    {
        var playerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var playerStatus = IsP1Turn ? p1.GetComponent<PlayerStatus>() : p2.GetComponent<PlayerStatus>();
        RemoveCombineEffectCard(playerCards.effectCards,
            playerCards.yellowCards);
        playerStatus.ChangePv(-diff);
        playerCards.RemoveSuperInvocation(superAttacker);
    }

    protected void DealWithEqualityAttack()
    {
        if (attacker is InGameSuperInvocationCard || opponent is InGameSuperInvocationCard)
        {
            var superAttacker = (InGameSuperInvocationCard)attacker;
            var superOpponent = (InGameSuperInvocationCard) opponent;
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
            else
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
            else
            {
                opponent.IncrementNumberDeaths();
                ComputeEqualityOpponent();
            }
        }
        else
        {
            attacker.IncrementNumberDeaths();
            ComputeEqualityAttacker();


            opponent.IncrementNumberDeaths();
            ComputeEqualityOpponent();
        }
    }

    protected void ComputeEqualityOpponent()
    {
        var playerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        var opponentPlayerCards = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();

        var abilities = opponent.Abilities;
        if (abilities.Count > 0)
        {
            foreach (var ability in abilities)
            {
                ability.OnCardDeath(canvas, opponent, playerCard, opponentPlayerCards);
            }
        }
        else
        {
            playerCard.SendInvocationCardToYellowTrash(opponent);
        }
    }

    protected void ComputeEqualityAttacker()
    {
        var playerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        var opponentPlayerCards = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();

        var abilities = attacker.Abilities;
        if (abilities.Count > 0)
        {
            foreach (var ability in abilities)
            {
                ability.OnCardDeath(canvas, attacker, playerCard, opponentPlayerCards);
            }
        }
        else
        {
            playerCard.SendInvocationCardToYellowTrash(attacker);
        }
    }

    protected void ComputeEqualityAttackSuperAttacker(InGameInvocationCard combineCard)
    {
        var playerCard = IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();

        RemoveCombineEffectCard(playerCard.effectCards,
            playerCard.yellowCards);
    }

    protected void ComputeEqualityAttackSuperOpponent(InGameInvocationCard combineCard)
    {
        var playerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();

        RemoveCombineEffectCard(playerCard.effectCards,
            playerCard.yellowCards);
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
        var opponentEquipmentCardsAbilities = copyOpponentInvocationCards
            .Where(invocationCard => invocationCard.EquipmentCard != null)
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
            effectCardAbility.OnTurnStart(canvas, opponentPlayerStatus, opponentPlayerCards, opponentPlayerStatus,
                playerCards);
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
    }

    protected void DrawPlayerCard(PlayerCards playerCards, PlayerStatus playerStatus)
    {
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
}