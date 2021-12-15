using System;
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
        ChangeHealthText(PlayerStatus.MAXPv, true);
        ChangeHealthText(PlayerStatus.MAXPv, false);
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
        if (components.Length != 0) return;
        UnityAction okAction = () => { SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single); };
        MessageBox.CreateOkMessageBox(canvas, "Fin de partie", endGameReason, okAction);
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
            var attackerEffectCards = p1.GetComponent<PlayerCards>().effectCards;
            DisplayCards(opponentCards, attackerEffectCards);
        }
        else
        {
            List<InvocationCard> opponentCards = p1.GetComponent<PlayerCards>().invocationCards;
            var attackerEffectCards = p2.GetComponent<PlayerCards>().effectCards;
            DisplayCards(opponentCards, attackerEffectCards);
        }
    }

    private bool IsSpecialActionPossible()
    {
        return invocationFunctions.IsSpecialActionPossible(attacker, attacker.InvocationActionEffect);
    }

    public void UseSpecialAction()
    {
        invocationFunctions.AskIfUserWantToUseActionEffect(attacker, attacker.InvocationActionEffect);
    }

    private void DisplayCards(List<InvocationCard> invocationCards, List<EffectCard> attackPlayerEffectCard)
    {
        var notEmptyOpponent = invocationCards.Where(t => t != null && t.Nom != null).Cast<Card>().ToList();

        if (notEmptyOpponent.Count == 0)
        {
            notEmptyOpponent.Add(player);
        } else if (attacker.GETEquipmentCard() != null && attacker.GETEquipmentCard().EquipmentInstantEffect != null &&
                   attacker.GETEquipmentCard().EquipmentInstantEffect.Keys.Contains(InstantEffect.DirectAtk))
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
                for (var j = notEmptyOpponent.Count - 1; j >= 0; j--)
                {
                    var invocationCard = (InvocationCard)notEmptyOpponent[j];
                    var permEffect = invocationCard.InvocationPermEffect;
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
                                            foreach (var card in notEmptyOpponent)
                                            {
                                                var invocationCardToCheck = (InvocationCard)card;
                                                if (invocationCardToCheck.Nom != invocationCard.Nom)
                                                {
                                                    if (invocationCardToCheck.GetCurrentDefense() >
                                                        invocationCard.GetCurrentDefense())
                                                    {
                                                        moreDefInvocationCards.Add(invocationCardToCheck);
                                                    }
                                                }
                                            }
                                        }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                    break;
                                case PermEffect.ProtectBehind:
                                {
                                    var name = values[i];
                                    if (notEmptyOpponent.Any(invocationCardToCheck =>
                                            invocationCardToCheck.Nom == name))
                                    {
                                        notEmptyOpponent.Remove(invocationCard);
                                    }
                                    else if (name == "any")
                                    {
                                        if (moreDefInvocationCards.Count > 0)
                                        {
                                            notEmptyOpponent.Remove(invocationCard);
                                        }
                                    }
                                }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (notEmptyOpponent.Count == 0)
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
    private void DealWithGoodAttack(InvocationCard opponent, float diff)
    {
        if (opponent is SuperInvocationCard)
        {
            var superOpponent = opponent as SuperInvocationCard;

            if (IsP1Turn)
            {
                p2.GetComponent<PlayerStatus>().ChangePv(diff);
                RemoveCombineEffectCard(p2.GetComponent<PlayerCards>().effectCards,
                    p2.GetComponent<PlayerCards>().yellowTrash);

                foreach (var combineCard in superOpponent.invocationCards)
                {
                    combineCard.IncrementNumberDeaths();
                    if (combineCard.GetInvocationDeathEffect() != null)
                    {
                        DealWithDeathEffect(combineCard, true);
                        if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                        {
                            p2.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                        }
                        else
                        {
                            combineCard.SetBonusAttack(0);
                            combineCard.SetBonusDefense(0);
                        }
                    }
                    else
                    {
                        p2.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                    }
                }

                p2.GetComponent<PlayerCards>().RemoveSuperInvocation(superOpponent);
            }
            else
            {
                p1.GetComponent<PlayerStatus>().ChangePv(diff);
                RemoveCombineEffectCard(p1.GetComponent<PlayerCards>().effectCards,
                    p1.GetComponent<PlayerCards>().yellowTrash);

                foreach (var combineCard in superOpponent.invocationCards)
                {
                    combineCard.IncrementNumberDeaths();
                    if (combineCard.GetInvocationDeathEffect() != null)
                    {
                        DealWithDeathEffect(combineCard, true);
                        if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                        {
                            p1.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                        }
                        else
                        {
                            combineCard.SetBonusAttack(0);
                            combineCard.SetBonusDefense(0);
                        }
                    }
                    else
                    {
                        p1.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                    }
                }

                p1.GetComponent<PlayerCards>().RemoveSuperInvocation(superOpponent);
            }
        }
        else
        {
            opponent.IncrementNumberDeaths();
            if (IsP1Turn)
            {
                p2.GetComponent<PlayerStatus>().ChangePv(diff);

                if (opponent.GetInvocationDeathEffect() != null)
                {
                    DealWithDeathEffect(opponent, false);
                    if (!opponent.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                    {
                        p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                    }
                    else
                    {
                        opponent.SetBonusAttack(0);
                        opponent.SetBonusDefense(0);
                    }
                }
                else
                {
                    p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                }
            }
            else
            {
                p1.GetComponent<PlayerStatus>().ChangePv(diff);
                if (opponent.GetInvocationDeathEffect() != null)
                {
                    DealWithDeathEffect(opponent, true);
                    if (!opponent.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                    {
                        p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                    }
                    else
                    {
                        opponent.SetBonusAttack(0);
                        opponent.SetBonusDefense(0);
                    }
                }
                else
                {
                    p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                }
            }
        }
    }

    /**
     * Attack that kill the attacker
     */
    private void DealWithHurtAttack(float diff)
    {
        if (attacker is SuperInvocationCard)
        {
            var superAttacker = attacker as SuperInvocationCard;


            if (IsP1Turn)
            {
                RemoveCombineEffectCard(p1.GetComponent<PlayerCards>().effectCards,
                    p1.GetComponent<PlayerCards>().yellowTrash);
                p1.GetComponent<PlayerStatus>().ChangePv(-diff);

                foreach (var combineCard in superAttacker.invocationCards)
                {
                    combineCard.IncrementNumberDeaths();
                    if (combineCard.GetInvocationDeathEffect() != null)
                    {
                        DealWithDeathEffect(combineCard, true);
                        if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                        {
                            p1.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                        }
                        else
                        {
                            combineCard.SetBonusAttack(0);
                            combineCard.SetBonusDefense(0);
                        }
                    }
                    else
                    {
                        p1.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                    }
                }

                p1.GetComponent<PlayerCards>().RemoveSuperInvocation(superAttacker);
            }
            else
            {
                RemoveCombineEffectCard(p2.GetComponent<PlayerCards>().effectCards,
                    p2.GetComponent<PlayerCards>().yellowTrash);
                p2.GetComponent<PlayerStatus>().ChangePv(-diff);

                foreach (var combineCard in superAttacker.invocationCards)
                {
                    combineCard.IncrementNumberDeaths();
                    if (combineCard.GetInvocationDeathEffect() != null)
                    {
                        DealWithDeathEffect(combineCard, true);
                        if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                        {
                            p2.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                        }
                        else
                        {
                            combineCard.SetBonusAttack(0);
                            combineCard.SetBonusDefense(0);
                        }
                    }
                    else
                    {
                        p2.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                    }
                }

                p2.GetComponent<PlayerCards>().RemoveSuperInvocation(superAttacker);
            }
        }
        else
        {
            attacker.IncrementNumberDeaths();
            if (IsP1Turn)
            {
                p1.GetComponent<PlayerStatus>().ChangePv(-diff);
                if (attacker.GetInvocationDeathEffect() != null)
                {
                    DealWithDeathEffect(attacker, true);
                    if (!attacker.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                    {
                        p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(cardSelected as InvocationCard);
                    }
                    else
                    {
                        attacker.SetBonusAttack(0);
                        attacker.SetBonusDefense(0);
                    }
                }
                else
                {
                    p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(cardSelected as InvocationCard);
                }
            }
            else
            {
                p2.GetComponent<PlayerStatus>().ChangePv(-diff);
                if (attacker.GetInvocationDeathEffect() != null)
                {
                    DealWithDeathEffect(attacker, false);
                    if (!attacker.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                    {
                        p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(cardSelected as InvocationCard);
                    }
                    else
                    {
                        attacker.SetBonusAttack(0);
                        attacker.SetBonusDefense(0);
                    }
                }
                else
                {
                    p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(cardSelected as InvocationCard);
                }
            }
        }
    }

    private void DealWithEqualityAttack(InvocationCard opponent)
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

                    if (IsP1Turn)
                    {
                        RemoveCombineEffectCard(p1.GetComponent<PlayerCards>().effectCards,
                            p1.GetComponent<PlayerCards>().yellowTrash);
                        if (combineCard.GetInvocationDeathEffect() != null)
                        {
                            DealWithDeathEffect(combineCard, true);
                            if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                            {
                                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                            }
                            else
                            {
                                combineCard.SetBonusAttack(0);
                                combineCard.SetBonusDefense(0);
                            }
                        }
                        else
                        {
                            p1.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                        }
                    }
                    else
                    {
                        RemoveCombineEffectCard(p2.GetComponent<PlayerCards>().effectCards,
                            p2.GetComponent<PlayerCards>().yellowTrash);
                        if (combineCard.GetInvocationDeathEffect() != null)
                        {
                            DealWithDeathEffect(combineCard, true);
                            if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                            {
                                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                            }
                            else
                            {
                                combineCard.SetBonusAttack(0);
                                combineCard.SetBonusDefense(0);
                            }
                        }
                        else
                        {
                            p2.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                        }
                    }
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

                if (IsP1Turn)
                {
                    if (attacker.GetInvocationDeathEffect() != null)
                    {
                        DealWithDeathEffect(attacker, true);
                        if (!attacker.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                        {
                            p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(attacker);
                        }
                        else
                        {
                            attacker.SetBonusAttack(0);
                            attacker.SetBonusDefense(0);
                        }
                    }
                    else
                    {
                        p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(attacker);
                    }
                }
                else
                {
                    if (attacker.GetInvocationDeathEffect() != null)
                    {
                        DealWithDeathEffect(attacker, false);
                        if (!attacker.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                        {
                            p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(attacker);
                        }
                        else
                        {
                            attacker.SetBonusAttack(0);
                            attacker.SetBonusDefense(0);
                        }
                    }
                    else
                    {
                        p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(attacker);
                    }
                }
            }

            if (superOpponent)
            {
                foreach (var combineCard in superOpponent.invocationCards)
                {
                    combineCard.IncrementNumberDeaths();

                    if (IsP1Turn)
                    {
                        RemoveCombineEffectCard(p2.GetComponent<PlayerCards>().effectCards,
                            p2.GetComponent<PlayerCards>().yellowTrash);
                        if (combineCard.GetInvocationDeathEffect() != null)
                        {
                            DealWithDeathEffect(combineCard, true);
                            if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                            {
                                p1.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                            }
                            else
                            {
                                combineCard.SetBonusAttack(0);
                                combineCard.SetBonusDefense(0);
                            }
                        }
                        else
                        {
                            p1.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                        }
                    }
                    else
                    {
                        RemoveCombineEffectCard(p1.GetComponent<PlayerCards>().effectCards,
                            p1.GetComponent<PlayerCards>().yellowTrash);
                        if (combineCard.GetInvocationDeathEffect() != null)
                        {
                            DealWithDeathEffect(combineCard, true);
                            if (!combineCard.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                            {
                                p2.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                            }
                            else
                            {
                                combineCard.SetBonusAttack(0);
                                combineCard.SetBonusDefense(0);
                            }
                        }
                        else
                        {
                            p2.GetComponent<PlayerCards>().SendCardToYellowTrash(combineCard);
                        }
                    }
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

                if (IsP1Turn)
                {
                    if (opponent.GetInvocationDeathEffect() != null)
                    {
                        DealWithDeathEffect(opponent, false);
                        if (!opponent.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                        {
                            p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                        }
                        else
                        {
                            opponent.SetBonusAttack(0);
                            opponent.SetBonusDefense(0);
                        }
                    }
                    else
                    {
                        p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                    }
                }
                else
                {
                    if (opponent.GetInvocationDeathEffect() != null)
                    {
                        DealWithDeathEffect(opponent, true);
                        if (!opponent.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                        {
                            p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                        }
                        else
                        {
                            opponent.SetBonusAttack(0);
                            opponent.SetBonusDefense(0);
                        }
                    }
                    else
                    {
                        p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                    }
                }
            }
        }
        else
        {
            attacker.IncrementNumberDeaths();
            opponent.IncrementNumberDeaths();

            if (IsP1Turn)
            {
                if (attacker.GetInvocationDeathEffect() != null)
                {
                    DealWithDeathEffect(attacker, true);
                    if (!attacker.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                    {
                        p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(attacker);
                    }
                    else
                    {
                        attacker.SetBonusAttack(0);
                        attacker.SetBonusDefense(0);
                    }
                }
                else
                {
                    p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(attacker);
                }

                if (opponent.GetInvocationDeathEffect() != null)
                {
                    DealWithDeathEffect(opponent, false);
                    if (!opponent.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                    {
                        p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                    }
                    else
                    {
                        opponent.SetBonusAttack(0);
                        opponent.SetBonusDefense(0);
                    }
                }
                else
                {
                    p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                }
            }
            else
            {
                if (attacker.GetInvocationDeathEffect() != null)
                {
                    DealWithDeathEffect(attacker, false);
                    if (!attacker.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                    {
                        p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(attacker);
                    }
                    else
                    {
                        attacker.SetBonusAttack(0);
                        attacker.SetBonusDefense(0);
                    }
                }
                else
                {
                    p2.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(attacker);
                }

                if (opponent.GetInvocationDeathEffect() != null)
                {
                    DealWithDeathEffect(opponent, true);
                    if (!opponent.GetInvocationDeathEffect().Keys.Contains(DeathEffect.ComeBackToHand))
                    {
                        p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                    }
                    else
                    {
                        opponent.SetBonusAttack(0);
                        opponent.SetBonusDefense(0);
                    }
                }
                else
                {
                    p1.GetComponent<PlayerCards>().sendInvocationCardToYellowTrash(opponent);
                }
            }
        }
    }

    /**
     * invocationCard = card that's going to die
     * attacker = the opponent
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
                    KillAlsoOtherCardDeathEffect(invocationCard, attacker);
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
        var isParsed = int.TryParse(values[i], out var number);
        if (isParsed)
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

            DrawPlayerCard(p1Cards);

            var invocationCards = p1Cards.invocationCards;
            var effectCards = p1Cards.effectCards;

            foreach (var effectCard in effectCards)
            {
                if (effectCard.checkTurn)
                {
                    UnityAction positiveAction = () =>
                    {
                        p1.GetComponent<PlayerStatus>().ChangePv(effectCard.affectPV);
                    };

                    UnityAction negativeAction = () =>
                    {
                        foreach (var invocationCard in invocationCards)
                        {
                            invocationCard.SetCurrentFamily(null);
                        }

                        p1Cards.yellowTrash.Add(effectCard);
                        p1Cards.effectCards.Remove(effectCard);
                    };
                    MessageBox.CreateSimpleMessageBox(canvas, "Action requise",
                        "Veux-tu prolonger l'effet de " + effectCard.Nom + " pour 1 tour pour " + effectCard.affectPV +
                        " points de vie ?", positiveAction, negativeAction);
                }
            }

            numberOfTurn++;
        }
        else
        {
            var p2Cards = p2.GetComponent<PlayerCards>();
            
            DrawPlayerCard(p2Cards);

            var invocationCards = p2Cards.invocationCards;

            var effectCards = p2Cards.effectCards;
            foreach (var effectCard in effectCards)
            {
                if (effectCard.checkTurn)
                {
                    UnityAction positiveAction = () =>
                    {
                        p2.GetComponent<PlayerStatus>().ChangePv(effectCard.affectPV);
                    };

                    UnityAction negativeAction = () =>
                    {
                        foreach (var invocationCard in invocationCards)
                        {
                            invocationCard.SetCurrentFamily(null);
                        }

                        p2Cards.yellowTrash.Add(effectCard);
                        p2Cards.effectCards.Remove(effectCard);
                    };
                    MessageBox.CreateSimpleMessageBox(canvas, "Action requise",
                        "Veux-tu prolonger l'effet de " + effectCard.Nom + " pour 1 tour pour " + effectCard.affectPV +
                        " points de vie ?", positiveAction, negativeAction);
                }
            }
        }

        phaseId += 1;
        roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pose";
    }

    private void DrawPlayerCard(PlayerCards playerCards)
    {
        var canSkipDraw = false;

        if (playerCards.field != null)
        {
            var fieldCardEffect = playerCards.field.FieldCardEffect;

            var keys = fieldCardEffect.Keys;
            var values = fieldCardEffect.Values;

            for (var i = 0; i < keys.Count; i++)
            {
                switch (keys[i])
                {
                    case FieldEffect.GetCard:
                    {
                        if (Enum.TryParse(values[i], out CardFamily cardFamily))
                        {
                            var familyCards = (from deckCard in playerCards.deck
                                where deckCard.Type == CardType.Invocation
                                select (InvocationCard)deckCard
                                into invocationCard
                                where invocationCard.GetFamily().Contains(cardFamily)
                                select invocationCard).Cast<Card>().ToList();
                            familyCards.AddRange((from fieldCard in playerCards.yellowTrash
                                where fieldCard.Type == CardType.Invocation
                                select (InvocationCard)fieldCard
                                into invocationCard
                                where invocationCard.GetFamily().Contains(cardFamily)
                                select invocationCard).Cast<Card>());

                            if (familyCards.Count > 0)
                            {
                                canSkipDraw = true;
                                
                                 UnityAction positiveAction = () =>
                            {
                                var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                    "Choix de la carte à récupérer", familyCards);
                                messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    var cardSelected =
                                        (InvocationCard)messageBox.GetComponent<MessageBox>().GETSelectedCard();
                                    if (cardSelected != null)
                                    {
                                        if (playerCards.deck.Contains(cardSelected))
                                        {
                                            playerCards.handCards.Add(cardSelected);
                                            playerCards.deck.Remove(cardSelected);
                                        }
                                        else
                                        {
                                            playerCards.handCards.Add(cardSelected);
                                            playerCards.yellowTrash.Remove(cardSelected);
                                        }

                                        Destroy(messageBox);
                                    }
                                    else
                                    {
                                        messageBox.SetActive(false);
                                        UnityAction okAction = () => { messageBox.SetActive(true); };
                                        MessageBox.CreateOkMessageBox(canvas, "Action requise",
                                            "Tu dois prendre une carte maintenant", okAction);
                                    }
                                };
                                messageBox.GetComponent<MessageBox>().NegativeAction = () =>
                                {
                                    messageBox.SetActive(false);
                                    UnityAction okAction = () => { messageBox.SetActive(true); };
                                    MessageBox.CreateOkMessageBox(canvas, "Action requise",
                                        "Tu dois prendre une carte maintenant", okAction);
                                };
                            };
                            UnityAction negativeAction = () =>
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
                            };
                            MessageBox.CreateSimpleMessageBox(canvas, "Proposition",
                                "Veux-tu sauter ta phase de pioche pour aller directement chercher une carte de la famille " +
                                values[i] + " dans ton deck ou ta poubelle jaune ?", positiveAction, negativeAction);
                            }
                        }
                    }
                        break;
                    case FieldEffect.DrawCard:
                    {
                        var numberCardToTake = int.Parse(values[i]);
                        canSkipDraw = true;
                        var size = playerCards.deck.Count;
                        if (size >= numberCardToTake)
                        {
                            for (int j = size - 1; j>=0 && j > (size -1 - numberCardToTake); j--)
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
                            var p1Pv = p1.GetComponent<PlayerStatus>().GETCurrentPv();
                            var p2Pv = p2.GetComponent<PlayerStatus>().GETCurrentPv();
                            endGameReason = p1Pv > p2Pv
                                ? "Joueur 1 n'a plus de cartes. Joueur 1 gagne la partie !"
                                : "Joueur 1 n'a plus de cartes. Joueur 2 gagne la partie !";

                            phaseId = 5;
                        }
                    }
                        break;
                    case FieldEffect.Life:
                    {
                        var pvPerAlly = float.Parse(values[i]);
                        var family = playerCards.field.GETFamily();
                        var numberFamilyOnField =
                            playerCards.invocationCards.Count(invocationCard => invocationCard.GetFamily().Contains(family));
                        p1.GetComponent<PlayerStatus>().ChangePv(pvPerAlly * numberFamilyOnField);
                    }
                        break;
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
                var p1Pv = p1.GetComponent<PlayerStatus>().GETCurrentPv();
                var p2Pv = p2.GetComponent<PlayerStatus>().GETCurrentPv();
                endGameReason = p1Pv > p2Pv
                    ? "Joueur 1 n'a plus de cartes. Joueur 1 gagne la partie !"
                    : "Joueur 1 n'a plus de cartes. Joueur 2 gagne la partie !";

                phaseId = 5;
            }
        }
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
        PlayerCards opponentPlayerCard = IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        List<EffectCard> effectCards = currentPlayerCard.effectCards;
        List<EffectCard> effectCardsToDelete = new List<EffectCard>();
        foreach (EffectCard effectCard in effectCards)
        {
            if (effectCard.GetLifeTime() == 1)
            {
                currentPlayerCard.yellowTrash.Add(effectCard);
                effectCardsToDelete.Add(effectCard);
            }
            else if (effectCard.GetLifeTime() > 1)
            {
                effectCard.DecrementLifeTime();
            }
        }

        foreach (var effectCard in effectCardsToDelete)
        {
            effectCards.Remove(effectCard);
        }

        foreach (var invocationCard in currentPlayerCard.invocationCards)
        {
            invocationCard.incrementNumberTurnOnField();
        }

        foreach (var invocationCard in opponentPlayerCard.invocationCards)
        {
            invocationCard.incrementNumberTurnOnField();
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
                    switch (keys[i])
                    {
                        case PermEffect.PreventInvocationCards:
                        {
                            for (int j = currentPlayerCard.invocationCards.Count - 1; j >= 0; j--)
                            {
                                var checkInvocationCard = currentPlayerCard.invocationCards[j];
                                if (checkInvocationCard.Nom != invocationCard.Nom)
                                {
                                    currentPlayerCard.invocationCards.Remove(checkInvocationCard);
                                    currentPlayerCard.handCards.Add(checkInvocationCard);
                                }
                            }

                            for (int j = opponentPlayerCard.invocationCards.Count - 1; j >= 0; j--)
                            {
                                var checkInvocationCard = opponentPlayerCard.invocationCards[j];
                                if (checkInvocationCard.Nom != invocationCard.Nom)
                                {
                                    opponentPlayerCard.invocationCards.Remove(checkInvocationCard);
                                    opponentPlayerCard.handCards.Add(checkInvocationCard);
                                }
                            }
                        }
                            break;
                        case PermEffect.NumberTurn:
                        {
                            var maxTurn = int.Parse(values[i]);
                            if (invocationCard.NumberTurnOnField >= maxTurn)
                            {
                                currentPlayerCard.sendInvocationCardToYellowTrash(invocationCard);
                            }
                        }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        for (int k = opponentPlayerCard.invocationCards.Count - 1; k >= 0; k--)
        {
            var invocationCard = opponentPlayerCard.invocationCards[k];
            var permEffect = invocationCard.InvocationPermEffect;
            if (permEffect != null)
            {
                var keys = permEffect.Keys;
                var values = permEffect.Values;
                for (int i = 0; i < keys.Count; i++)
                {
                    switch (keys[i])
                    {
                        case PermEffect.PreventInvocationCards:
                        {
                            for (int j = currentPlayerCard.invocationCards.Count - 1; j >= 0; j--)
                            {
                                var checkInvocationCard = currentPlayerCard.invocationCards[j];
                                if (checkInvocationCard.Nom != invocationCard.Nom)
                                {
                                    currentPlayerCard.invocationCards.Remove(checkInvocationCard);
                                    currentPlayerCard.handCards.Add(checkInvocationCard);
                                }
                            }

                            for (int j = opponentPlayerCard.invocationCards.Count - 1; j >= 0; j--)
                            {
                                var checkInvocationCard = opponentPlayerCard.invocationCards[j];
                                if (checkInvocationCard.Nom != invocationCard.Nom)
                                {
                                    opponentPlayerCard.invocationCards.Remove(checkInvocationCard);
                                    opponentPlayerCard.handCards.Add(checkInvocationCard);
                                }
                            }
                        }
                            break;
                        case PermEffect.NumberTurn:
                        {
                            var maxTurn = int.Parse(values[i]);
                            if (invocationCard.NumberTurnOnField >= maxTurn)
                            {
                                opponentPlayerCard.sendInvocationCardToYellowTrash(invocationCard);
                            }
                        }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        if (IsP1Turn)
        {
            var invocationCards = p1.GetComponent<PlayerCards>().invocationCards;
            foreach (var invocationCard in invocationCards)
            {
                invocationCard.UnblockAttack();
            }
        }
        else
        {
            var invocationCards = p2.GetComponent<PlayerCards>().invocationCards;
            foreach (var invocationCard in invocationCards)
            {
                invocationCard.UnblockAttack();
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