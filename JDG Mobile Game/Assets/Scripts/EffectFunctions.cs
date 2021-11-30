using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EffectFunctions : MonoBehaviour
{
    [SerializeField] private GameObject miniCardMenu;
    [SerializeField] private Transform canvas;
    private PlayerCards currentPlayerCard;
    private PlayerStatus currentPlayerStatus;
    private PlayerCards opponentPlayerCard;
    private PlayerStatus opponentPlayerStatus;
    private GameObject p1;

    private GameObject p2;

    // Start is called before the first frame update
    private void Start()
    {
        GameLoop.ChangePlayer.AddListener(ChangePlayer);
        InGameMenuScript.EffectCardEvent.AddListener(PutEffectCard);
        p1 = GameObject.Find("Player1");
        p2 = GameObject.Find("Player2");
        currentPlayerCard = p1.GetComponent<PlayerCards>();
        currentPlayerStatus = p1.GetComponent<PlayerStatus>();
        opponentPlayerCard = p2.GetComponent<PlayerCards>();
        opponentPlayerStatus = p2.GetComponent<PlayerStatus>();
    }

    private int FindFirstEmptyEffectLocationCurrentPlayer()
    {
        var effectCards = currentPlayerCard.effectCards;
        return effectCards.Count;
    }

    private void PutEffectCard(EffectCard effectCard)
    {
        var size = FindFirstEmptyEffectLocationCurrentPlayer();

        if (size < 4)
        {
            var cancelled = ApplyEffectCard(effectCard, effectCard.GetEffectCardEffect());

            if (cancelled) return;
            miniCardMenu.SetActive(false);
            currentPlayerCard.handCards.Remove(effectCard);
            currentPlayerCard.effectCards.Add(effectCard);
        }
        else
        {
            MessageBox.CreateOkMessageBox(canvas, "Attention",
                "Tu ne peux pas poser plus de 4 cartes effet durant un tour");
        }
    }

    public bool CanUseEffectCard(EffectCardEffect effectCardEffect)
    {
        var keys = effectCardEffect.Keys;
        var values = effectCardEffect.Values;
        var isValid = true;

        var pvAffected = 0f;
        var handCardsNumber = 0;
        var affectOpponent = false;
        string[] sources = null;
        var canDisplayRemoveOption = false;

        for (var i = 0; i < keys.Count; i++)
        {
            var effect = keys[i];
            var value = values[i];
            switch (effect)
            {
                case Effect.AffectOpponent:
                {
                    affectOpponent = Boolean.Parse(value);
                }
                    break;
                case Effect.DestroyCards:
                {
                    switch (value)
                    {
                        case "field":
                        {
                            var fieldCards = new List<Card>();
                            var fieldCard1 = currentPlayerCard.field;
                            var fieldCard2 = opponentPlayerCard.field;
                            if (fieldCard1.IsValid())
                            {
                                fieldCards.Add(fieldCard1);
                            }

                            if (fieldCard2.IsValid())
                            {
                                fieldCards.Add(fieldCard2);
                            }

                            if (fieldCards.Count == 0)
                            {
                                isValid = false;
                            }
                        }
                            break;
                        case "invocation":
                        {
                            if (affectOpponent)
                            {
                                var invocationOpponentValid = opponentPlayerCard.invocationCards
                                    .Where(card => card.IsValid()).Cast<Card>().ToList();
                                isValid = invocationOpponentValid.Count > 0;
                            }
                        }
                            break;
                        case "1":
                        {
                            var fieldCard1 = currentPlayerCard.field;
                            var fieldCard2 = opponentPlayerCard.field;
                            var effectCards1 = currentPlayerCard.effectCards;
                            var effectCards2 = opponentPlayerCard.effectCards;
                            var invocationCards1 = currentPlayerCard.invocationCards;
                            var invocationCards2 = opponentPlayerCard.invocationCards;

                            var allCardsOnField = new List<Card>();

                            if (fieldCard1.IsValid())
                            {
                                allCardsOnField.Add(fieldCard1);
                            }

                            if (fieldCard2.IsValid())
                            {
                                allCardsOnField.Add(fieldCard2);
                            }

                            allCardsOnField.AddRange(effectCards1.Where(card => card.IsValid()).Cast<Card>());

                            allCardsOnField.AddRange(effectCards2.Where(card => card.IsValid()).Cast<Card>());

                            allCardsOnField.AddRange(invocationCards1.Where(card => card.IsValid()).Cast<Card>());

                            allCardsOnField.AddRange(invocationCards2.Where(card => card.IsValid()).Cast<Card>());

                            isValid = allCardsOnField.Count > 0;
                        }
                            break;
                    }
                }
                    break;
                case Effect.SacrificeInvocation:
                {
                    switch (value)
                    {
                        case "true":
                        {
                            var invocationCards = currentPlayerCard.invocationCards;
                            var invocationCardsValid = invocationCards
                                .Where(invocationCard => invocationCard.IsValid()).Cast<Card>().ToList();

                            isValid = invocationCardsValid.Count > 0;
                        }
                            break;
                        case "5":
                        {
                            var invocationCards = currentPlayerCard.invocationCards;
                            var invocationCardsValid = invocationCards
                                .Where(invocationCard => invocationCard.IsValid()).Where(invocationCard =>
                                    invocationCard.GetCurrentAttack() >= 5 ||
                                    invocationCard.GetCurrentDefense() >= 5).Cast<Card>().ToList();
                            isValid = invocationCardsValid.Count > 0;
                        }
                            break;
                        case "3":
                        {
                            var invocationCards = currentPlayerCard.invocationCards;
                            var invocationCardsValid = invocationCards.Where(invocationCard =>
                                    invocationCard.GetCurrentAttack() >= 3 ||
                                    invocationCard.GetCurrentDefense() >= 3)
                                .Cast<Card>().ToList();
                            isValid = invocationCardsValid.Count > 0;
                        }
                            break;
                    }
                }
                    break;
                case Effect.SameFamily:
                {
                    var fieldCard = currentPlayerCard.field;
                    isValid = fieldCard != null && fieldCard.IsValid();
                }
                    break;
                case Effect.RemoveDeck:
                {
                    var size = currentPlayerCard.deck.Count;
                    isValid = size > 0;
                }
                    break;
                case Effect.SpecialInvocation:
                {
                    var place = FindFirstEmptyInvocationLocationCurrentPlayer();
                    var yellowTrash = currentPlayerCard.yellowTrash;
                    var invocationCards = currentPlayerCard.invocationCards;

                    var invocationFromYellowTrash =
                        yellowTrash.Where(card => card.Type == CardType.Invocation).ToList();
                    isValid = place < 4 && invocationFromYellowTrash.Count > 0;
                }
                    break;
                case Effect.Duration:
                {
                    //TODO Do something with it
                }
                    break;
                case Effect.Combine:
                {
                    var numberCombine = Int32.Parse(value);
                    var currentInvocationCards = currentPlayerCard.invocationCards;
                    isValid = currentInvocationCards.Count >= numberCombine;
                }
                    break;
                case Effect.TakeControl:
                {
                    var invocationCardOpponent = opponentPlayerCard.invocationCards.Where(card => card.IsValid())
                        .Cast<Card>().ToList();

                    isValid = invocationCardOpponent.Count > 0;
                }
                    break;
                case Effect.NumberAttacks:
                {
                }
                    break;
                case Effect.SkipAttack:
                {
                }
                    break;
                case Effect.SeeCards:
                {
                }
                    break;
                case Effect.ChangeOrder:
                {
                }
                    break;
                case Effect.AttackDirectly:
                {
                }
                    break;
                case Effect.ProtectAttack:
                {
                }
                    break;
                case Effect.SkipFieldsEffect:
                {
                }
                    break;
                case Effect.ChangeField:
                {
                }
                    break;
                case Effect.HandMax:
                {
                }
                    break;
                case Effect.CheckTurn:
                {
                }
                    break;
            }
        }

        return isValid;
    }

    private void ReduceHandOpponentPlayer(List<Card> handCard2, int handCardsNumber)
    {
        if (handCard2.Count < handCardsNumber)
        {
            var size = opponentPlayerCard.deck.Count;
            while (size > 0 && opponentPlayerCard.handCards.Count < handCardsNumber)
            {
                var c = opponentPlayerCard.deck[size - 1];
                opponentPlayerCard.handCards.Add(c);
                opponentPlayerCard.deck.RemoveAt(size - 1);
                size--;
            }
        }
        else if (handCard2.Count > handCardsNumber)
        {
            var numberToGet = handCard2.Count - handCardsNumber;
            var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Choisis " + numberToGet + " cartes à enlever",
                handCard2, multipleCardSelection: true, numberCardInSelection: numberToGet);

            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var currentCards = message.GetComponent<MessageBox>().GETMultipleSelectedCards();

                if (currentCards != null && currentCards.Count > 0)
                {
                    if (currentCards.Count == numberToGet)
                    {
                        foreach (var card in currentCards)
                        {
                            opponentPlayerCard.handCards.Remove(card);
                            opponentPlayerCard.yellowTrash.Add(card);
                        }

                        Destroy(message);
                    }
                    else
                    {
                        message.SetActive(false);
                        var messageBox = MessageBox.CreateOkMessageBox(canvas, "Action requise",
                            "Tu dois enlever " + numberToGet + "cartes !");
                        messageBox.GetComponent<MessageBox>().OkAction = () =>
                        {
                            message.SetActive(true);
                            Destroy(messageBox);
                        };
                    }
                }
                else
                {
                    message.SetActive(false);
                    var messageBox = MessageBox.CreateOkMessageBox(canvas, "Action requise",
                        "Tu dois enlever " + numberToGet + "cartes !");
                    messageBox.GetComponent<MessageBox>().OkAction = () =>
                    {
                        message.SetActive(true);
                        Destroy(messageBox);
                    };
                }
            };
            message.GetComponent<MessageBox>().NegativeAction = () =>
            {
                message.SetActive(false);
                var messageBox = MessageBox.CreateOkMessageBox(canvas, "Action requise",
                    "Tu ne peux pas y échapper ...");
                messageBox.GetComponent<MessageBox>().OkAction = () =>
                {
                    message.SetActive(true);
                    Destroy(messageBox);
                };
            };
        }
    }

    private bool ApplyEffectCard(EffectCard effectCard, EffectCardEffect effectCardEffect)
    {
        var keys = effectCardEffect.Keys;
        var values = effectCardEffect.Values;

        var pvAffected = 0f;
        var handCardsNumber = 0;
        var affectOpponent = false;
        var cancelled = false;
        string[] sources = null;
        var canDisplayRemoveOption = false;

        for (var i = 0; i < keys.Count && !cancelled; i++)
        {
            if (!cancelled)
            {
                var effect = keys[i];
                var value = values[i];
                switch (effect)
                {
                    case Effect.AffectPv:
                    {
                        pvAffected = ApplyAffectPv(effectCard, value);
                    }
                        break;
                    case Effect.AffectOpponent:
                    {
                        affectOpponent = ApplyAffectOpponent(value);
                    }
                        break;
                    case Effect.NumberInvocationCard:
                    {
                        ApplyNumberInvocationCard(affectOpponent, pvAffected);
                    }
                        break;
                    case Effect.NumberHandCard:
                    {
                        ApplyNumberHandCard(affectOpponent, pvAffected);
                    }
                        break;
                    case Effect.DestroyCards:
                    {
                        cancelled = ApplyDestroyCards(value, pvAffected, affectOpponent);
                    }
                        break;
                    case Effect.SacrificeInvocation:
                    {
                        cancelled = ApplySacrificeInvocation(value, cancelled, pvAffected);
                    }
                        break;
                    case Effect.SameFamily:
                    {
                        cancelled = ApplySameFamily(cancelled);
                    }
                        break;
                    case Effect.CheckTurn:
                    {
                        ApplyCheckTurn(effectCard);
                    }
                        break;
                    case Effect.ChangeHandCards:
                    {
                        handCardsNumber = ApplyChangeHandCards(value);
                    }
                        break;
                    case Effect.Sources:
                    {
                        sources = ApplySources(value);
                    }
                        break;
                    case Effect.HandMax:
                    {
                        ApplyHandMax(handCardsNumber);
                    }
                        break;
                    case Effect.SeeOpponentHand:
                    {
                        canDisplayRemoveOption = ApplySeeOpponentHand();
                    }
                        break;
                    case Effect.RemoveCardOption:
                    {
                        cancelled = ApplyRemoveCardOption(canDisplayRemoveOption);
                    }
                        break;
                    case Effect.RemoveHand:
                    {
                        cancelled = ApplyRemoveHand();
                    }
                        break;
                    case Effect.RemoveDeck:
                    {
                        cancelled = ApplyRemoveDeck(cancelled);
                    }
                        break;
                    case Effect.SpecialInvocation:
                    {
                        cancelled = ApplySpecialInvocation(cancelled);
                    }
                        break;
                    case Effect.DivideInvocation:
                    {
                        ApplyDivideInvocation();
                    }
                        break;
                    case Effect.Duration:
                        ApplyDuration();
                        break;
                    case Effect.Combine:
                    {
                        ApplyCombine(effectCard, value);
                    }
                        break;
                    case Effect.RevertStat:
                        ApplyRevertStat();
                        break;
                    case Effect.TakeControl:
                    {
                        cancelled = ApplyTakeControl();
                    }
                        break;
                    case Effect.NumberAttacks:
                    {
                    }
                        break;
                    case Effect.SkipAttack:
                    {
                    }
                        break;
                    case Effect.SeeCards:
                    {
                    }
                        break;
                    case Effect.ChangeOrder:
                    {
                    }
                        break;
                    case Effect.AttackDirectly:
                    {
                    }
                        break;
                    case Effect.ProtectAttack:
                    {
                    }
                        break;
                    case Effect.SkipFieldsEffect:
                    {
                    }
                        break;
                    case Effect.ChangeField:
                    {
                    }
                        break;
                    case Effect.SkipContre:
                    {
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //TODO Add effectCard to yellow trash if necessary
        return cancelled;
    }

    private bool ApplyTakeControl()
    {
        bool cancelled = false;
        var invocationCardOpponent = opponentPlayerCard.invocationCards.Where(card => card.IsValid())
            .Cast<Card>().ToList();

        var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
            "Quel carte veux-tu contrôler pendant un tour ?", invocationCardOpponent);

        message.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var card =
                (InvocationCard)message.GetComponent<MessageBox>().GETSelectedCard();
            if (card.IsValid())
            {
            }
            else
            {
                cancelled = true;
            }
        };

        message.GetComponent<MessageBox>().NegativeAction = () =>
        {
            cancelled = true;
            Destroy(message);
        };
        return cancelled;
    }

    private void ApplyRevertStat()
    {
        var invocationCards1 = currentPlayerCard.invocationCards;
        var invocationCards2 = opponentPlayerCard.invocationCards;

        foreach (var card in invocationCards1)
        {
            // TODO Keep previous bonus
            var newBonusAttack = card.GetCurrentDefense() - card.GetAttack();
            var newBonusDefense = card.GetCurrentAttack() - card.GetDefense();
            card.SetBonusDefense(newBonusDefense);
            card.SetBonusAttack(newBonusAttack);
        }
    }

    private void ApplyCombine(EffectCard effectCard, string value)
    {
        var numberCombine = Int32.Parse(value);
        var currentInvocationCards = currentPlayerCard.invocationCards;
        if (currentInvocationCards.Count >= numberCombine)
        {
            List<Card> cards = new List<Card>();
            foreach (Card card in currentInvocationCards)
            {
                cards.Add(card);
            }

            var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Sélectionnez " + numberCombine + " Cartes à fusionner", cards,
                multipleCardSelection: true, numberCardInSelection: numberCombine);
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var currentCards = messageBox.GetComponent<MessageBox>().GETMultipleSelectedCards();

                if (currentCards != null && currentCards.Count > 0)
                {
                    List<InvocationCard> invocationCards = new List<InvocationCard>();
                    foreach (var card in currentCards)
                    {
                        var invocationCard = (InvocationCard)card;
                        invocationCards.Add(invocationCard);
                    }

                    SuperInvocationCard superInvocationCard =
                        ScriptableObject.CreateInstance<SuperInvocationCard>();
                    superInvocationCard.Init(invocationCards);

                    var playerName = GameLoop.IsP1Turn ? "P1" : "P2";
                    currentPlayerCard.AddPhysicalCard(superInvocationCard, playerName);
                    foreach (InvocationCard invocationCard in invocationCards)
                    {
                        var index1 = currentInvocationCards.FindIndex(0, currentInvocationCards.Count,
                            card => card.Nom == invocationCard.Nom);
                        if (index1 > -1)
                        {
                            currentPlayerCard.SendToSecretHide(invocationCard);
                            currentPlayerCard.invocationCards.RemoveAt(index1);
                        }
                    }

                    currentInvocationCards.Add(superInvocationCard);

                    Destroy(messageBox);
                }
                else
                {
                    MessageBox.CreateOkMessageBox(canvas, "Information",
                        "Vous n'avez pas choisi le nombre de carte adéquat");
                    currentPlayerCard.effectCards.Remove(effectCard);
                    currentPlayerCard.handCards.Add(effectCard);
                    Destroy(messageBox);
                }
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                currentPlayerCard.effectCards.Remove(effectCard);
                currentPlayerCard.handCards.Add(effectCard);
                Destroy(messageBox);
            };
        }
    }

    private static void ApplyDuration()
    {
        {
            //TODO Do something with it
        }
    }

    private void ApplyDivideInvocation()
    {
        var opponentInvocationCard = opponentPlayerCard.invocationCards;
        foreach (var card in opponentInvocationCard)
        {
            if (!card.IsValid()) continue;
            // TODO Keep previous bonus
            var newBonusDefense = card.GETBonusDefense() - card.GetCurrentDefense() / 2;
            card.SetBonusDefense(newBonusDefense);
        }
    }

    private bool ApplySpecialInvocation(bool cancelled)
    {
        var place = FindFirstEmptyInvocationLocationCurrentPlayer();
        if (place < 4)
        {
            var yellowTrash = currentPlayerCard.yellowTrash;
            var invocationCards = currentPlayerCard.invocationCards;

            var invocationFromYellowTrash =
                yellowTrash.Where(card => card.Type == CardType.Invocation).ToList();

            if (invocationFromYellowTrash.Count == 0)
            {
                cancelled = true;
                // TODO MessageBox
            }
            else
            {
                var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                    "Quel carte veux-tu invoquer spécialement ?", invocationFromYellowTrash);

                message.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    var card =
                        (InvocationCard)message.GetComponent<MessageBox>().GETSelectedCard();
                    if (card.IsValid())
                    {
                        currentPlayerCard.yellowTrash.Remove(card);
                        currentPlayerCard.invocationCards[place] = card;
                    }
                    else
                    {
                        cancelled = true;
                    }

                    Destroy(message);
                };

                message.GetComponent<MessageBox>().NegativeAction = () =>
                {
                    cancelled = true;
                    Destroy(message);
                };
            }
        }
        else
        {
            cancelled = true;
            // TODO MessageBox
        }

        return cancelled;
    }

    private bool ApplyRemoveDeck(bool cancelled)
    {
        var size = currentPlayerCard.deck.Count;
        if (size > 0)
        {
            Card c = currentPlayerCard.deck[size - 1];
            currentPlayerCard.yellowTrash.Add(c);
            currentPlayerCard.deck.RemoveAt(size - 1);
        }
        else
        {
            cancelled = true;
        }

        return cancelled;
    }

    private bool ApplyRemoveHand()
    {
        bool cancelled = false;
        var handCardPlayer = currentPlayerCard.handCards;
        var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
            "Quel carte veux-tu te défausser?", handCardPlayer);

        message.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var card = message.GetComponent<MessageBox>().GETSelectedCard();
            if (card.IsValid())
            {
                currentPlayerCard.yellowTrash.Add(card);
                currentPlayerCard.handCards.Remove(card);
            }
            else
            {
                cancelled = true;
            }

            Destroy(message);
        };

        message.GetComponent<MessageBox>().NegativeAction = () =>
        {
            cancelled = true;
            Destroy(message);
        };
        return cancelled;
    }

    private bool ApplyRemoveCardOption(bool canDisplayRemoveOption)
    {
        bool cancelled = false;

        var message = MessageBox.CreateSimpleMessageBox(canvas, "Choix",
            "Veux-tu te défausser d'une carte pour en défausser une à l'adversaire ?");


        message.GetComponent<MessageBox>().PositiveAction = () =>
        {
            canDisplayRemoveOption = true;
            var handCardOpponent = opponentPlayerCard.handCards;
            var message1 = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Quel carte veux-tu enlever à l'adversaire ?", handCardOpponent);
            message1.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var cardOpponent = message1.GetComponent<MessageBox>().GETSelectedCard();
                if (cardOpponent.IsValid())
                {
                    Destroy(message1);
                    var handCardPlayer = currentPlayerCard.handCards;
                    var message2 = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                        "Quel carte veux-tu te défausser?", handCardPlayer);

                    message2.GetComponent<MessageBox>().PositiveAction = () =>
                    {
                        var cardPlayer = message2.GetComponent<MessageBox>().GETSelectedCard();
                        if (cardPlayer.IsValid())
                        {
                            currentPlayerCard.yellowTrash.Add(cardPlayer);
                            opponentPlayerCard.yellowTrash.Add(cardOpponent);
                            currentPlayerCard.handCards.Remove(cardPlayer);
                            opponentPlayerCard.handCards.Remove(cardOpponent);
                        }
                        else
                        {
                            cancelled = true;
                        }
                    };

                    message2.GetComponent<MessageBox>().NegativeAction = () => { cancelled = true; };
                }
                else
                {
                    cancelled = true;
                }
            };

            message1.GetComponent<MessageBox>().NegativeAction = () => { cancelled = true; };
        };

        message.GetComponent<MessageBox>().NegativeAction = () => { cancelled = true; };
        return cancelled;
    }

    private bool ApplySeeOpponentHand()
    {
        bool canDisplayRemoveOption = false;
        var handCardOpponent = opponentPlayerCard.handCards;
        var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
            "Voici les cartes de l'adversaire", handCardOpponent, okButton: true);

        message.GetComponent<MessageBox>().OkAction = () =>
        {
            canDisplayRemoveOption = true;
            Destroy(message);
        };
        return canDisplayRemoveOption;
    }

    private void ApplyHandMax(int handCardsNumber)
    {
        var handCard1 = currentPlayerCard.handCards;
        var handCard2 = opponentPlayerCard.handCards;

        if (handCard1.Count < handCardsNumber)
        {
            var size = currentPlayerCard.deck.Count;
            while (size > 0 && currentPlayerCard.handCards.Count < handCardsNumber)
            {
                var c = currentPlayerCard.deck[size - 1];
                currentPlayerCard.handCards.Add(c);
                currentPlayerCard.deck.RemoveAt(size - 1);
                size--;
            }
        }
        else if (handCard1.Count > handCardsNumber)
        {
            var numberToGet = handCard1.Count - handCardsNumber;
            var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Choisis " + numberToGet + " cartes à enlever",
                handCard1, multipleCardSelection: true, numberCardInSelection: numberToGet);

            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var currentCards = message.GetComponent<MessageBox>().GETMultipleSelectedCards();

                if (currentCards != null && currentCards.Count > 0)
                {
                    if (currentCards.Count == numberToGet)
                    {
                        foreach (var card in currentCards)
                        {
                            currentPlayerCard.handCards.Remove(card);
                            currentPlayerCard.yellowTrash.Add(card);
                        }

                        ReduceHandOpponentPlayer(handCard2, handCardsNumber);
                        Destroy(message);
                    }
                    else
                    {
                        message.SetActive(false);
                        var messageBox = MessageBox.CreateOkMessageBox(canvas, "Action requise",
                            "Tu dois enlever " + numberToGet + "cartes !");
                        messageBox.GetComponent<MessageBox>().OkAction = () =>
                        {
                            message.SetActive(true);
                            Destroy(messageBox);
                        };
                    }
                }
                else
                {
                    message.SetActive(false);
                    var messageBox = MessageBox.CreateOkMessageBox(canvas, "Action requise",
                        "Tu dois enlever " + numberToGet + "cartes !");
                    messageBox.GetComponent<MessageBox>().OkAction = () =>
                    {
                        message.SetActive(true);
                        Destroy(messageBox);
                    };
                }
            };
            message.GetComponent<MessageBox>().NegativeAction = () =>
            {
                message.SetActive(false);
                var messageBox = MessageBox.CreateOkMessageBox(canvas, "Action requise",
                    "Tu ne peux pas y échapper ...");
                messageBox.GetComponent<MessageBox>().OkAction = () =>
                {
                    message.SetActive(true);
                    Destroy(messageBox);
                };
            };
        }
    }

    private static string[] ApplySources(string value)
    {
        return value.Split(';');
    }

    private static int ApplyChangeHandCards(string value)
    {
        int handCardsNumber;
        handCardsNumber = int.Parse(value);
        return handCardsNumber;
    }

    private static void ApplyCheckTurn(EffectCard effectCard)
    {
        effectCard.checkTurn = true;
    }

    private bool ApplySameFamily(bool cancelled)
    {
        var fieldCard = currentPlayerCard.field;
        if (fieldCard.IsValid())
        {
            var familyField = fieldCard.GETFamily();
            foreach (var card in currentPlayerCard.invocationCards.Where(card => card.IsValid()))
            {
                card.SetCurrentFamily(familyField);
            }
        }
        else
        {
            cancelled = true;
        }

        return cancelled;
    }

    private bool ApplySacrificeInvocation(string value, bool cancelled, float pvAffected)
    {
        switch (value)
        {
            case "true":
            {
                var invocationCards = currentPlayerCard.invocationCards;
                var invocationCardsValid = invocationCards
                    .Where(invocationCard => invocationCard.IsValid()).Cast<Card>().ToList();

                if (invocationCardsValid.Count == 0)
                {
                    cancelled = true;
                    // TODO MESSAGE BOX
                }
                else
                {
                    cancelled =  GenerateSacrificeInvocationMessageBox(invocationCardsValid,
                        invocationCards, pvAffected);
                }
            }
                break;
            case "5":
            {
                var invocationCards = currentPlayerCard.invocationCards;
                var invocationCardsValid = invocationCards
                    .Where(invocationCard => invocationCard.IsValid()).Where(invocationCard =>
                        invocationCard.GetCurrentAttack() >= 5 ||
                        invocationCard.GetCurrentDefense() >= 5).Cast<Card>().ToList();

                if (invocationCardsValid.Count == 0)
                {
                    cancelled = true;
                    // TODO MESSAGE BOX
                }
                else
                {
                    cancelled = GenerateSacrificeInvocationMessageBox(invocationCardsValid,
                        invocationCards, pvAffected);
                }
            }
                break;
            case "3":
            {
                var invocationCards = currentPlayerCard.invocationCards;
                var invocationCardsValid = invocationCards.Where(invocationCard =>
                        invocationCard.GetCurrentAttack() >= 3 ||
                        invocationCard.GetCurrentDefense() >= 3)
                    .Cast<Card>().ToList();

                if (invocationCardsValid.Count == 0)
                {
                    cancelled = true;
                    // TODO MESSAGE BOX
                }
                else
                {
                    cancelled = GenerateSacrificeInvocationMessageBox(invocationCardsValid,
                        invocationCards, pvAffected);
                }
            }
                break;
        }

        return cancelled;
    }

    private bool ApplyDestroyCards(string value, float pvAffected, bool affectOpponent)
    {
        var cancelled = false;
        switch (value)
        {
            case "field":
            {
                var fieldCards = new List<Card>();
                var fieldCard1 = currentPlayerCard.field;
                var fieldCard2 = opponentPlayerCard.field;
                if (fieldCard1.IsValid())
                {
                    fieldCards.Add(fieldCard1);
                }

                if (fieldCard2.IsValid())
                {
                    fieldCards.Add(fieldCard2);
                }

                if (fieldCards.Count > 0)
                {
                    if (pvAffected < 0)
                    {
                        UnityAction negativeAction = () => { cancelled = true; };
                        var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                            "Choix du terrain à détruire", fieldCards, negativeAction: negativeAction);
                        var affected = pvAffected;
                        message.GetComponent<MessageBox>().PositiveAction = () =>
                        {
                            var fieldCard =
                                (FieldCard)message.GetComponent<MessageBox>().GETSelectedCard();
                            if (fieldCard.Nom == fieldCard1.Nom)
                            {
                                currentPlayerCard.yellowTrash.Add(fieldCard);
                                currentPlayerCard.field = null;
                            }
                            else
                            {
                                opponentPlayerCard.yellowTrash.Add(fieldCard);
                                opponentPlayerCard.field = null;
                            }

                            currentPlayerStatus.ChangePv(affected);
                        };
                    }
                }
            }
                break;
            case "all":
            {
                var fieldCard1 = currentPlayerCard.field;
                var fieldCard2 = opponentPlayerCard.field;
                var effectCards1 = currentPlayerCard.effectCards;
                var effectCards2 = opponentPlayerCard.effectCards;
                var invocationCards1 = currentPlayerCard.invocationCards;
                var invocationCards2 = opponentPlayerCard.invocationCards;
                if (fieldCard1.IsValid())
                {
                    currentPlayerCard.field = null;
                    currentPlayerCard.yellowTrash.Add(fieldCard1);
                }

                if (fieldCard2.IsValid())
                {
                    opponentPlayerCard.field = null;
                    opponentPlayerCard.yellowTrash.Add(fieldCard2);
                }

                for (var j = 0; j < effectCards1.Count; j++)
                {
                    var card = effectCards1[j];
                    if (!card.IsValid()) continue;
                    currentPlayerCard.yellowTrash.Add(card);
                    currentPlayerCard.effectCards[j] = null;
                }

                for (var j = 0; j < effectCards2.Count; j++)
                {
                    var card = effectCards2[j];
                    if (!card.IsValid()) continue;
                    opponentPlayerCard.yellowTrash.Add(card);
                    opponentPlayerCard.effectCards[j] = null;
                }

                for (var j = 0; j < invocationCards1.Count; j++)
                {
                    var card = invocationCards1[j];
                    if (!card.IsValid()) continue;
                    currentPlayerCard.yellowTrash.Add(card);
                    currentPlayerCard.invocationCards[j] = null;
                }

                for (var j = 0; j < invocationCards2.Count; j++)
                {
                    var card = invocationCards2[j];
                    if (!card.IsValid()) continue;
                    opponentPlayerCard.yellowTrash.Add(card);
                    opponentPlayerCard.invocationCards[j] = null;
                }
            }
                break;
            case "invocation":
            {
                if (affectOpponent)
                {
                    var invocationOpponentValid = opponentPlayerCard.invocationCards
                        .Where(card => card.IsValid()).Cast<Card>().ToList();
                    UnityAction negativeAction = () => { cancelled = true; };
                    var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                        "Choix de l'invocation à détruire", invocationOpponentValid,
                        negativeAction: negativeAction);
                    message.GetComponent<MessageBox>().PositiveAction = () =>
                    {
                        var invocationCard =
                            (InvocationCard)message.GetComponent<MessageBox>().GETSelectedCard();
                        if (invocationCard.IsValid())
                        {
                            opponentPlayerCard.yellowTrash.Add(invocationCard);
                            for (var j = 0; j < opponentPlayerCard.invocationCards.Count; j++)
                            {
                                var currentCard = opponentPlayerCard.invocationCards[j];
                                if (!currentCard.IsValid() || currentCard.Nom != invocationCard.Nom)
                                    continue;
                                opponentPlayerCard.invocationCards[j] = null;
                                break;
                            }
                        }
                        else
                        {
                            cancelled = true;
                        }
                    };
                }
            }
                break;
            case "1":
            {
                var fieldCard1 = currentPlayerCard.field;
                var fieldCard2 = opponentPlayerCard.field;
                var effectCards1 = currentPlayerCard.effectCards;
                var effectCards2 = opponentPlayerCard.effectCards;
                var invocationCards1 = currentPlayerCard.invocationCards;
                var invocationCards2 = opponentPlayerCard.invocationCards;

                var allCardsOnField = new List<Card>();

                if (fieldCard1.IsValid())
                {
                    allCardsOnField.Add(fieldCard1);
                }

                if (fieldCard2.IsValid())
                {
                    allCardsOnField.Add(fieldCard2);
                }

                allCardsOnField.AddRange(effectCards1.Where(card => card.IsValid()).Cast<Card>());

                allCardsOnField.AddRange(effectCards2.Where(card => card.IsValid()).Cast<Card>());

                allCardsOnField.AddRange(invocationCards1.Where(card => card.IsValid()).Cast<Card>());

                allCardsOnField.AddRange(invocationCards2.Where(card => card.IsValid()).Cast<Card>());
                UnityAction negativeAction = () => { cancelled = true; };
                var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                    "Choix de la carte à détruire", allCardsOnField, negativeAction: negativeAction);

                message.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    var card = message.GetComponent<MessageBox>().GETSelectedCard();
                    if (card.IsValid())
                    {
                        switch (card.Type)
                        {
                            case CardType.Invocation:
                                //  FindCardInArrayAndSendItToTrash(invocationCards1, invocationCards2, card);
                                break;
                            case CardType.Effect:
                                //   FindCardInArrayAndSendItToTrash(effectCards1, effectCards2, card);
                                break;
                            case CardType.Field when fieldCard1.Nom == card.Nom:
                                currentPlayerCard.yellowTrash.Add(card);
                                currentPlayerCard.field = null;
                                break;
                            case CardType.Field:
                            {
                                if (fieldCard2.Nom == card.Nom)
                                {
                                    opponentPlayerCard.yellowTrash.Add(fieldCard2);
                                    opponentPlayerCard.field = null;
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        cancelled = true;
                    }
                };
            }
                break;
        }

        return cancelled;
    }

    private void ApplyNumberHandCard(bool affectOpponent, float pvAffected)
    {
        if (affectOpponent)
        {
            AffectOpponentNumberHandCard(pvAffected);
        }
        else
        {
            AffectCurrentNumberHandCard(pvAffected);
        }
    }

    private void ApplyNumberInvocationCard(bool affectOpponent, float pvAffected)
    {
        if (affectOpponent)
        {
            AffectOpponentNumberInvocation(pvAffected);
        }
        else
        {
            AffectCurrentNumberInvocation(pvAffected);
        }
    }

    private static bool ApplyAffectOpponent(string value)
    {
        return Boolean.Parse(value);
    }

    private static float ApplyAffectPv(EffectCard effectCard, string value)
    {
        float pvAffected;
        if (value == "ALL")
        {
            pvAffected = 100.0f;
        }
        else
        {
            pvAffected = float.Parse(value, CultureInfo.InvariantCulture);
        }

        effectCard.affectPV = pvAffected;

        return pvAffected;
    }

    private bool GenerateSacrificeInvocationMessageBox(List<Card> invocationCardsValid,
        IReadOnlyList<InvocationCard> invocationCards,
        float pvAffected)
    {
        var isDone = false;
        var cancelled = false;
        var message =
            MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix de l'invocation à sacrifier",
                invocationCardsValid);


        message.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var card = message.GetComponent<MessageBox>().GETSelectedCard();
            if (card.IsValid())
            {
                for (var j = 0; j < invocationCards.Count; j++)
                {
                    var invocationCard = invocationCards[j];
                    if (invocationCard.Nom != card.Nom) continue;
                    currentPlayerCard.yellowTrash.Add(card);
                    currentPlayerCard.invocationCards[j] = null;

                    if (pvAffected > 0)
                    {
                        currentPlayerStatus.ChangePv(pvAffected);
                    }

                    break;
                }
                
            }
            else
            {
                cancelled = true;
            }
            isDone = true;
            Destroy(message);
        };
        message.GetComponent<MessageBox>().NegativeAction = () =>
        {
            isDone = true;
            cancelled = true;
            Destroy(message);
        };
        while (!isDone)
        {
        }

        return cancelled;
    }


    private int FindFirstEmptyInvocationLocationCurrentPlayer()
    {
        var invocationCards = currentPlayerCard.invocationCards;
        var end = false;
        var i = 0;
        while (i < 4 && !end)
        {
            if (invocationCards[i] != null)
            {
                if (invocationCards[i].Nom == null)
                {
                    end = true;
                }
                else
                {
                    i++;
                }
            }
            else
            {
                end = true;
            }
        }

        return i;
    }

    private void FindCardInArrayAndSendItToTrash(IReadOnlyList<Card> cards1, IReadOnlyList<Card> cards2, Card card)
    {
        var found = false;
        var j = 0;
        while (j < cards2.Count && !found)
        {
            var card2 = cards2[j];
            if (card2.IsValid())
            {
                if (card2.Nom == card.Nom)
                {
                    found = true;
                    opponentPlayerCard.yellowTrash.Add(card);
                    switch (card.Type)
                    {
                        case CardType.Invocation:
                        {
                            opponentPlayerCard.invocationCards[j] = null;
                        }
                            break;
                        case CardType.Effect:
                        {
                            opponentPlayerCard.effectCards[j] = null;
                        }
                            break;
                    }
                }
            }

            j++;
        }

        if (found) return;
        j = 0;
        while (j < cards1.Count && !found)
        {
            var card1 = cards1[j];
            if (card1.IsValid())
            {
                if (card1.Nom == card.Nom)
                {
                    found = true;
                    currentPlayerCard.yellowTrash.Add(card);
                    switch (card.Type)
                    {
                        case CardType.Invocation:
                        {
                            currentPlayerCard.invocationCards[j] = null;
                        }
                            break;
                        case CardType.Effect:
                        {
                            currentPlayerCard.effectCards[j] = null;
                        }
                            break;
                    }
                }
            }

            j++;
        }
    }

    private void AffectOpponentNumberInvocation(float pvAffected)
    {
        var invocationCards = opponentPlayerCard.invocationCards;
        var size = invocationCards.Count;
        var damages = size * pvAffected;
        opponentPlayerStatus.ChangePv(damages);
    }

    private void AffectCurrentNumberInvocation(float pvAffected)
    {
        var invocationCards = currentPlayerCard.invocationCards;
        var size = invocationCards.Count;
        var damages = size * pvAffected;
        currentPlayerStatus.ChangePv(damages);
    }

    private void AffectOpponentNumberHandCard(float pvAffected)
    {
        var handCards = opponentPlayerCard.handCards;
        var size = handCards.Count;
        var damages = size * pvAffected;
        opponentPlayerStatus.ChangePv(damages);
    }

    private void AffectCurrentNumberHandCard(float pvAffected)
    {
        var handCards = currentPlayerCard.handCards;
        var size = handCards.Count;
        var damages = size * pvAffected;
        currentPlayerStatus.ChangePv(damages);
    }

    private void ChangePlayer()
    {
        if (GameLoop.IsP1Turn)
        {
            currentPlayerCard = p1.GetComponent<PlayerCards>();
            currentPlayerStatus = p1.GetComponent<PlayerStatus>();
            opponentPlayerCard = p2.GetComponent<PlayerCards>();
            opponentPlayerStatus = p2.GetComponent<PlayerStatus>();
        }
        else
        {
            currentPlayerCard = p2.GetComponent<PlayerCards>();
            currentPlayerStatus = p2.GetComponent<PlayerStatus>();
            opponentPlayerCard = p1.GetComponent<PlayerCards>();
            opponentPlayerStatus = p1.GetComponent<PlayerStatus>();
        }
    }
}