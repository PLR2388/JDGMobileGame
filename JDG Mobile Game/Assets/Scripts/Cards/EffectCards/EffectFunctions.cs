using System;
using System.Collections.Generic;
using System.Linq;
using Cards.FieldCards;
using Cards.InvocationCards;
using UnityEngine;
using UnityEngine.Events;

namespace Cards.EffectCards
{
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
                ApplyEffectCard(effectCard, effectCard.GetEffectCardEffect());

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
            var affectOpponent = false;
            var changeField = false;
            var handCard = 0;

            for (var i = 0; i < keys.Count; i++)
            {
                var effect = keys[i];
                var value = values[i];
                switch (effect)
                {
                    case Effect.AffectOpponent:
                    {
                        isValid = CanUseAffectOpponent(value, pvAffected, isValid, out affectOpponent);
                    }
                        break;
                    case Effect.DestroyCards:
                    {
                        isValid = CanUseDestroyCards(value, isValid, affectOpponent);
                    }
                        break;
                    case Effect.SacrificeInvocation:
                    {
                        isValid = CanUseSacrificeInvocation(value, isValid);
                    }
                        break;
                    case Effect.SameFamily:
                    {
                        isValid = CanUseSameFamily(isValid);
                    }
                        break;
                    case Effect.RemoveDeck:
                    {
                        isValid = CanUseRemoveDeck(isValid);
                    }
                        break;
                    case Effect.SpecialInvocation:
                    {
                        isValid = CanUseSpecialInvocation(isValid);
                    }
                        break;
                    case Effect.Duration:
                    {
                        //TODO Do something with it
                    }
                        break;
                    case Effect.Combine:
                    {
                        isValid = CanUseCombine(value, isValid);
                    }
                        break;
                    case Effect.TakeControl:
                    {
                        isValid = CanUseTakeControl(isValid);
                    }
                        break;
                    case Effect.NumberAttacks:
                    {
                        isValid &= currentPlayerCard.invocationCards.Count > 0;
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
                        isValid = CanUseAttackDirectly(value, isValid);
                    }
                        break;
                    case Effect.ProtectAttack:
                    {
                        isValid = CanUseProtectAttack(isValid);
                    }
                        break;
                    case Effect.SkipFieldsEffect:
                    {
                        isValid = CanUseSkipFieldsEffect(isValid);
                    }
                        break;
                    case Effect.ChangeField:
                    {
                        changeField = true;
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
                    case Effect.RemoveHand:
                    {
                        isValid = CanUseRemoveHand(values, i, isValid);
                    }
                        break;
                    case Effect.AffectPv:
                    {
                        CanUseAffectPv(value, out pvAffected);
                    }
                        break;
                    case Effect.Sources:
                    {
                        isValid = CanUseSources(changeField, value, isValid, handCard);
                    }
                        break;
                    case Effect.ChangeHandCards:
                    {
                        isValid = CanUseChangeHandCards(value, isValid, out handCard);
                    }
                        break;
                    case Effect.NumberInvocationCard:
                        break;
                    case Effect.NumberHandCard:
                        break;
                    case Effect.SeeOpponentHand:
                        break;
                    case Effect.RemoveCardOption:
                        break;
                    case Effect.DivideInvocation:
                        break;
                    case Effect.RevertStat:
                        break;
                    case Effect.SkipContre:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return isValid;
        }

        private bool CanUseChangeHandCards(string value, bool isValid, out int handCard)
        {
            handCard = int.Parse(value);
            isValid &= currentPlayerCard.handCards.Count > handCard;
            return isValid;
        }

        private bool CanUseSources(bool changeField, string value, bool isValid, int handCard)
        {
            if (changeField)
            {
                if (value != "deck") return isValid;
                var fieldCardsInDeck =
                    currentPlayerCard.deck.FindAll(card => card.Type == CardType.Field);
                isValid &= fieldCardsInDeck.Count > 0;
            }
            else if (handCard > 0)
            {
                if (value == "deck;yellow")
                {
                    isValid &= (currentPlayerCard.deck.Count + currentPlayerCard.yellowTrash.Count) >=
                               handCard;
                }
            }

            return isValid;
        }

        private static void CanUseAffectPv(string value, out float pvAffected)
        {
            if (float.TryParse(value, out pvAffected)) return;
            if (value == "all")
            {
                pvAffected = 999;
            }
        }

        private bool CanUseRemoveHand(IReadOnlyList<string> values, int i, bool isValid)
        {
            var numberCardToRemove = int.Parse(values[i]);
            // This card + number to remove
            isValid &= currentPlayerCard.handCards.Count > numberCardToRemove;
            return isValid;
        }

        private bool CanUseSkipFieldsEffect(bool isValid)
        {
            isValid &= currentPlayerCard.field != null || opponentPlayerCard.field != null;
            return isValid;
        }

        private bool CanUseProtectAttack(bool isValid)
        {
            isValid &= currentPlayerCard.invocationCards.Count == 0;
            return isValid;
        }

        private bool CanUseAttackDirectly(string value, bool isValid)
        {
            var minValue = float.Parse(value);
            isValid &= opponentPlayerStatus.GetCurrentPv() < minValue;
            return isValid;
        }

        private bool CanUseTakeControl(bool isValid)
        {
            var invocationCardOpponent = opponentPlayerCard.invocationCards.Where(card => card.IsValid())
                .Cast<Card>().ToList();

            isValid &= invocationCardOpponent.Count > 0 && currentPlayerCard.invocationCards.Count < 4;
            return isValid;
        }

        private bool CanUseCombine(string value, bool isValid)
        {
            var numberCombine = int.Parse(value);
            var currentInvocationCards = currentPlayerCard.invocationCards;
            isValid &= currentInvocationCards.Count >= numberCombine;
            return isValid;
        }

        private bool CanUseSpecialInvocation(bool isValid)
        {
            var yellowTrash = currentPlayerCard.yellowTrash;
            var invocationCards = currentPlayerCard.invocationCards;
            var place = invocationCards.Count;

            var invocationFromYellowTrash =
                yellowTrash.Where(card => card.Type == CardType.Invocation).ToList();
            isValid &= place < 4 && invocationFromYellowTrash.Count > 0;
            return isValid;
        }

        private bool CanUseRemoveDeck(bool isValid)
        {
            var size = currentPlayerCard.deck.Count;
            isValid &= size > 0;
            return isValid;
        }

        private bool CanUseSameFamily(bool isValid)
        {
            var fieldCard = currentPlayerCard.field;
            isValid &= fieldCard != null && fieldCard.IsValid();
            return isValid;
        }

        private bool CanUseSacrificeInvocation(string value, bool isValid)
        {
            switch (value)
            {
                case "true":
                {
                    var invocationCards = currentPlayerCard.invocationCards;
                    var invocationCardsValid = invocationCards
                        .Where(invocationCard => invocationCard.IsValid()).Cast<Card>().ToList();

                    isValid &= invocationCardsValid.Count > 0;
                }
                    break;
                case "5":
                {
                    var invocationCards = currentPlayerCard.invocationCards;
                    var invocationCardsValid = invocationCards
                        .Where(invocationCard => invocationCard.IsValid()).Where(invocationCard =>
                            invocationCard.GetCurrentAttack() >= 5 ||
                            invocationCard.GetCurrentDefense() >= 5).Cast<Card>().ToList();
                    isValid &= invocationCardsValid.Count > 0;
                }
                    break;
                case "3":
                {
                    var invocationCards = currentPlayerCard.invocationCards;
                    var invocationCardsValid = invocationCards.Where(invocationCard =>
                            invocationCard.GetCurrentAttack() >= 3 ||
                            invocationCard.GetCurrentDefense() >= 3)
                        .Cast<Card>().ToList();
                    isValid &= invocationCardsValid.Count > 0;
                }
                    break;
            }

            return isValid;
        }

        private bool CanUseDestroyCards(string value, bool isValid, bool affectOpponent)
        {
            switch (value)
            {
                case "field":
                {
                    var fieldCards = new List<Card>();
                    var fieldCard1 = currentPlayerCard.field;
                    var fieldCard2 = opponentPlayerCard.field;
                    if (fieldCard1 != null)
                    {
                        fieldCards.Add(fieldCard1);
                    }

                    if (fieldCard2 != null)
                    {
                        fieldCards.Add(fieldCard2);
                    }

                    isValid &= fieldCards.Count > 0;
                }
                    break;
                case "invocation":
                {
                    if (affectOpponent)
                    {
                        var invocationOpponentValid = opponentPlayerCard.invocationCards
                            .Where(card => card.IsValid()).Cast<Card>().ToList();
                        isValid &= invocationOpponentValid.Count > 0;
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

                    if (fieldCard1 != null)
                    {
                        allCardsOnField.Add(fieldCard1);
                    }

                    if (fieldCard2 != null)
                    {
                        allCardsOnField.Add(fieldCard2);
                    }

                    allCardsOnField.AddRange(effectCards1.Where(card => card.IsValid()));

                    allCardsOnField.AddRange(effectCards2.Where(card => card.IsValid()));

                    allCardsOnField.AddRange(invocationCards1.Where(card => card.IsValid()));

                    allCardsOnField.AddRange(invocationCards2.Where(card => card.IsValid()));

                    isValid &= allCardsOnField.Count > 0;
                }
                    break;
            }

            return isValid;
        }

        private bool CanUseAffectOpponent(string value, float pvAffected, bool isValid, out bool affectOpponent)
        {
            affectOpponent = bool.Parse(value);
            if (pvAffected != 0 && !affectOpponent)
            {
                isValid &= (currentPlayerStatus.GetCurrentPv() + pvAffected) > 0;
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
                    var currentCards = message.GetComponent<MessageBox>().GetMultipleSelectedCards();

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

        private void ApplyEffectCard(EffectCard effectCard, EffectCardEffect effectCardEffect)
        {
            var keys = effectCardEffect.Keys;
            var values = effectCardEffect.Values;

            var pvAffected = 0f;
            var handCardsNumber = 0;
            var affectOpponent = false;
            var changeField = false;
            var cardsToSee = 0;

            for (var i = 0; i < keys.Count; i++)
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
                        if (i < keys.Count - 1)
                        {
                            if (keys[i + 1] == Effect.DestroyCards)
                            {
                                ApplyDestroyCards(values[i + 1], pvAffected, affectOpponent);
                            }
                        }
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
                    case Effect.SacrificeInvocation:
                    {
                        var affected = pvAffected;
                        var opponent = affectOpponent;

                        void DestroyCardAction()
                        {
                            var index = keys.FindIndex(effect1 => effect1 == Effect.DestroyCards);
                            if (index > -1)
                            {
                                ApplyDestroyCards(values[index], affected, opponent);
                            }
                        }

                        ApplySacrificeInvocation(value, pvAffected, DestroyCardAction);
                    }
                        break;
                    case Effect.SameFamily:
                    {
                        ApplySameFamily();
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
                        var sources = ApplySources(value);
                        if (handCardsNumber > 0)
                        {
                            if (sources.Contains("deck") && sources.Contains("yellow"))
                            {
                                List<Card> cards = new List<Card>(currentPlayerCard.deck);
                                cards.AddRange(currentPlayerCard.yellowTrash);
                                var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                    "Choisis 1 carte à rajouter dans ta main", cards);
                                messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    var card = messageBox.GetComponent<MessageBox>().GetSelectedCard();
                                    if (card != null)
                                    {
                                        currentPlayerCard.handCards.Add(card);
                                        if (currentPlayerCard.yellowTrash.Contains(card))
                                        {
                                            currentPlayerCard.yellowTrash.Remove(card);
                                        }
                                        else
                                        {
                                            currentPlayerCard.deck.Remove(card);
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

                                        MessageBox.CreateOkMessageBox(canvas, "Information",
                                            "Tu dois choisir une carte", OkAction);
                                    }
                                };
                                messageBox.GetComponent<MessageBox>().NegativeAction = () =>
                                {
                                    messageBox.SetActive(false);

                                    void OkAction()
                                    {
                                        messageBox.SetActive(true);
                                    }

                                    MessageBox.CreateOkMessageBox(canvas, "Information",
                                        "Tu dois choisir une carte", OkAction);
                                };
                            }
                        }
                        else if (changeField)
                        {
                            if (value == "deck")
                            {
                                List<Card> fieldCardInDeck =
                                    currentPlayerCard.deck.FindAll(card => card.Type == CardType.Field);
                                var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                    "Choix du nouveau terrain", fieldCardInDeck);
                                messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    var fieldCard = (FieldCard)messageBox.GetComponent<MessageBox>().GetSelectedCard();
                                    if (fieldCard != null)
                                    {
                                        if (currentPlayerCard.field != null)
                                        {
                                            currentPlayerCard.SendCardToYellowTrash(currentPlayerCard.field);
                                            currentPlayerCard.field = null;
                                        }

                                        currentPlayerCard.field = fieldCard;
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
                                            "Tu dois chosir un terrain",
                                            OkAction);
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
                                        "Tu dois choisir un terrain",
                                        OkAction);
                                };
                            }
                        }
                    }
                        break;
                    case Effect.HandMax:
                    {
                        ApplyHandMax(handCardsNumber, effectCard);
                    }
                        break;
                    case Effect.SeeOpponentHand:
                    {
                        ApplySeeOpponentHand();
                    }
                        break;
                    case Effect.RemoveCardOption:
                    {
                        ApplyRemoveCardOption();
                    }
                        break;
                    case Effect.RemoveHand:
                    {
                        ApplyRemoveHand(effectCard);
                        if (keys.Count - 1 > i)
                        {
                            if (keys[i + 1] == Effect.DestroyCards)
                            {
                                ApplyDestroyCards(values[i + 1], 0, false);
                            }
                        }
                    }
                        break;
                    case Effect.RemoveDeck:
                    {
                        ApplyRemoveDeck();
                    }
                        break;
                    case Effect.SpecialInvocation:
                    {
                        ApplySpecialInvocation();
                    }
                        break;
                    case Effect.DivideInvocation:
                    {
                        ApplyDivideInvocation();
                    }
                        break;
                    case Effect.Duration:
                        ApplyDuration(effectCard, value);
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
                        ApplyTakeControl();
                    }
                        break;
                    case Effect.NumberAttacks:
                    {
                        var number = int.Parse(value);
                        foreach (var invocationCard in currentPlayerCard.invocationCards)
                        {
                            invocationCard.SetRemainedAttackThisTurn(number);
                        }
                    }
                        break;
                    case Effect.SkipAttack:
                    {
                        if (affectOpponent)
                        {
                            foreach (var opponentInvocationCard in opponentPlayerCard.invocationCards
                                         .Where(card => card.IsAffectedByEffectCard).ToList())
                            {
                                opponentInvocationCard.BlockAttack();
                            }
                        }
                    }
                        break;
                    case Effect.SeeCards:
                    {
                        cardsToSee = int.Parse(value);
                    }
                        break;
                    case Effect.ChangeOrder:
                    {
                        var see = cardsToSee;

                        void PositiveAction()
                        {
                            List<Card> cardsDeckToSee = new List<Card>();
                            if (opponentPlayerCard.deck.Count >= see)
                            {
                                for (var j = opponentPlayerCard.deck.Count - 1;
                                     j > (opponentPlayerCard.deck.Count - 1 - see);
                                     j--)
                                {
                                    cardsDeckToSee.Add(opponentPlayerCard.deck[j]);
                                }
                            }
                            else
                            {
                                cardsDeckToSee.AddRange(opponentPlayerCard.invocationCards);
                            }

                            void PositiveActionCardSelector()
                            {
                                var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                    "Choisis les cartes dans l'ordre que tu veux", cardsDeckToSee,
                                    multipleCardSelection: true, numberCardInSelection: cardsDeckToSee.Count);
                                messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    var selectedCards =
                                        messageBox.GetComponent<MessageBox>().GetMultipleSelectedCards();
                                    if (selectedCards.Count == cardsDeckToSee.Count)
                                    {
                                        foreach (var card in cardsDeckToSee)
                                        {
                                            opponentPlayerCard.deck.Remove(card);
                                        }

                                        selectedCards.Reverse();
                                        opponentPlayerCard.deck.AddRange(selectedCards);
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
                                            "Tu dois choisir l'ordre des cartes", OkAction);
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
                                        "Tu dois choisir l'ordre des cartes", OkAction);
                                };
                            }

                            MessageBox.CreateMessageBoxWithCardSelector(canvas, "Veux-tu changer l'ordre ?",
                                cardsDeckToSee, PositiveActionCardSelector);
                        }

                        var toSee = cardsToSee;

                        void NegativeAction()
                        {
                            var cardsDeckToSee = new List<Card>();
                            if (currentPlayerCard.deck.Count >= toSee)
                            {
                                for (var j = currentPlayerCard.deck.Count - 1;
                                     j > (currentPlayerCard.deck.Count - 1 - toSee);
                                     j--)
                                {
                                    cardsDeckToSee.Add(currentPlayerCard.deck[j]);
                                }
                            }
                            else
                            {
                                cardsDeckToSee.AddRange(currentPlayerCard.invocationCards);
                            }

                            void PositiveActionCardSelector()
                            {
                                var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                    "Choisis les cartes dans l'ordre que tu veux", cardsDeckToSee,
                                    multipleCardSelection: true, numberCardInSelection: cardsDeckToSee.Count);
                                messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    var selectedCards =
                                        messageBox.GetComponent<MessageBox>().GetMultipleSelectedCards();
                                    if (selectedCards.Count == cardsDeckToSee.Count)
                                    {
                                        foreach (var card in cardsDeckToSee)
                                        {
                                            currentPlayerCard.deck.Remove(card);
                                        }

                                        selectedCards.Reverse();
                                        currentPlayerCard.deck.AddRange(selectedCards);
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
                                            "Tu dois choisir l'ordre des cartes", OkAction);
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
                                        "Tu dois choisir l'ordre des cartes", OkAction);
                                };
                            }

                            MessageBox.CreateMessageBoxWithCardSelector(canvas, "Veux-tu changer l'ordre ?",
                                cardsDeckToSee, PositiveActionCardSelector);
                        }

                        MessageBox.CreateSimpleMessageBox(canvas, "Action requise",
                            "Veux-tu regarder dans le deck de l'adversaire ou du tien ?",
                            PositiveAction, NegativeAction);
                    }
                        break;
                    case Effect.ProtectAttack:
                    {
                        var numberShield = int.Parse(value);
                        currentPlayerStatus.SetNumberShield(numberShield);
                    }
                        break;
                    case Effect.SkipFieldsEffect:
                    {
                        currentPlayerCard.DesactivateFieldCardEffect();
                        opponentPlayerCard.DesactivateFieldCardEffect();
                    }
                        break;
                    case Effect.ChangeField:
                    {
                        changeField = true;
                    }
                        break;
                    case Effect.SkipContre:
                    {
                    }
                        break;
                    case Effect.DestroyCards:
                        break;
                    case Effect.AttackDirectly:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //TODO Add effectCard to yellow trash if necessary
        }

        private void ApplyTakeControl()
        {
            var invocationCardOpponent = opponentPlayerCard.invocationCards.Where(card => card.IsAffectedByEffectCard)
                .Cast<Card>().ToList();

            var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Quel carte veux-tu contrôler pendant un tour ?", invocationCardOpponent);

            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var card =
                    (InvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();
                if (card != null)
                {
                    card.ControlCard();
                    card.UnblockAttack();
                    opponentPlayerCard.invocationCards.Remove(card);
                    opponentPlayerCard.SendToSecretHide(card);
                    currentPlayerCard.AddPhysicalCard(card, GameLoop.IsP1Turn ? "P1" : "P2");
                    currentPlayerCard.invocationCards.Add(card);
                    Destroy(message);
                }
                else
                {
                    message.SetActive(false);

                    void OkAction()
                    {
                        message.SetActive(true);
                    }

                    MessageBox.CreateOkMessageBox(canvas, "Action requise", "Tu dois choisir une carte à controller",
                        OkAction);
                }
            };

            message.GetComponent<MessageBox>().NegativeAction = () =>
            {
                message.SetActive(false);

                void OkAction()
                {
                    message.SetActive(true);
                }

                MessageBox.CreateOkMessageBox(canvas, "Action requise", "Tu dois choisir une carte à contrôler",
                    OkAction);
            };
        }

        private void ApplyRevertStat()
        {
            var invocationCards1 = currentPlayerCard.invocationCards;
            var invocationCards2 =
                opponentPlayerCard.invocationCards.Where(card => card.IsAffectedByEffectCard).ToList();

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

        private void ApplyCombine(EffectCard effectCard, string value)
        {
            var numberCombine = int.Parse(value);
            var currentInvocationCards = currentPlayerCard.invocationCards;
            if (currentInvocationCards.Count < numberCombine) return;
            var cards = currentInvocationCards.Cast<Card>().ToList();

            var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Sélectionnez " + numberCombine + " Cartes à fusionner", cards,
                multipleCardSelection: true, numberCardInSelection: numberCombine);
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var currentCards = messageBox.GetComponent<MessageBox>().GetMultipleSelectedCards();

                if (currentCards != null && currentCards.Count > 0)
                {
                    var invocationCards = currentCards.Cast<InvocationCard>().ToList();

                    var superInvocationCard =
                        ScriptableObject.CreateInstance<SuperInvocationCard>();
                    superInvocationCard.Init(invocationCards);

                    var playerName = GameLoop.IsP1Turn ? "P1" : "P2";
                    currentPlayerCard.AddPhysicalCard(superInvocationCard, playerName);
                    foreach (var invocationCard in invocationCards)
                    {
                        var index1 = currentInvocationCards.FindIndex(0, currentInvocationCards.Count,
                            card => card.Nom == invocationCard.Nom);
                        if (index1 <= -1) continue;
                        currentPlayerCard.SendToSecretHide(invocationCard);
                        currentPlayerCard.invocationCards.RemoveAt(index1);
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

        private static void ApplyDuration(EffectCard effectCard, string value)
        {
            if (int.TryParse(value, out var duration))
            {
                effectCard.SetLifeTime(duration);
            }
        }

        private void ApplyDivideInvocation()
        {
            var opponentInvocationCard =
                opponentPlayerCard.invocationCards.Where(card => card.IsAffectedByEffectCard).ToList();
            foreach (var card in opponentInvocationCard)
            {
                var newBonusDefense = card.GetBonusDefense() - card.GetCurrentDefense() / 2;
                card.SetBonusDefense(newBonusDefense);
            }
        }

        private void ApplySpecialInvocation()
        {
            var yellowTrash = currentPlayerCard.yellowTrash;
            var invocationCards = currentPlayerCard.invocationCards;

            var invocationFromYellowTrash =
                yellowTrash.Where(card => card.Type == CardType.Invocation).ToList();

            var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Quel carte veux-tu invoquer spécialement ?", invocationFromYellowTrash);

            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var card =
                    (InvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();
                if (card != null)
                {
                    currentPlayerCard.yellowTrash.Remove(card);
                    card.DeactivateEffect();
                    invocationCards.Add(card);
                    Destroy(message);
                }
                else
                {
                    message.SetActive(false);

                    void OkAction()
                    {
                        message.SetActive(true);
                    }

                    MessageBox.CreateOkMessageBox(canvas, "Action requise", "Tu dois choisir une carte invocation",
                        OkAction);
                }
            };

            message.GetComponent<MessageBox>().NegativeAction = () =>
            {
                message.SetActive(false);

                void OkAction()
                {
                    message.SetActive(true);
                }

                MessageBox.CreateOkMessageBox(canvas, "Action requise", "Tu dois choisir une carte invocation",
                    OkAction);
            };
        }

        private void ApplyRemoveDeck()
        {
            var size = currentPlayerCard.deck.Count;
            if (size <= 0) return;
            var c = currentPlayerCard.deck[size - 1];
            currentPlayerCard.yellowTrash.Add(c);
            currentPlayerCard.deck.RemoveAt(size - 1);
        }

        private void ApplyRemoveHand(Card effectCard)
        {
            var handCardPlayer = new List<Card>(currentPlayerCard.handCards);
            handCardPlayer.Remove(effectCard);
            var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Quel carte veux-tu te défausser?", handCardPlayer);

            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var card = message.GetComponent<MessageBox>().GetSelectedCard();
                if (card != null)
                {
                    currentPlayerCard.yellowTrash.Add(card);
                    currentPlayerCard.handCards.Remove(card);
                    Destroy(message);
                }
                else
                {
                    message.SetActive(false);

                    void OkAction()
                    {
                        message.SetActive(true);
                    }

                    MessageBox.CreateOkMessageBox(canvas, "Action requise", "Tu dois choisir une carte à te défausser",
                        OkAction);
                }
            };

            message.GetComponent<MessageBox>().NegativeAction = () =>
            {
                message.SetActive(false);

                void OkAction()
                {
                    message.SetActive(true);
                }

                MessageBox.CreateOkMessageBox(canvas, "Action requise", "Tu dois choisir une carte à te défausser",
                    OkAction);
            };
        }

        private void ApplyRemoveCardOption()
        {
            var message = MessageBox.CreateSimpleMessageBox(canvas, "Choix",
                "Veux-tu te défausser d'une carte pour en défausser une à l'adversaire ?");


            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                Destroy(message);
                var handCardOpponent = opponentPlayerCard.handCards;
                var message1 = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                    "Quel carte veux-tu enlever à l'adversaire ?", handCardOpponent);
                message1.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    var cardOpponent = message1.GetComponent<MessageBox>().GetSelectedCard();
                    if (!cardOpponent.IsValid()) return;
                    Destroy(message1);
                    var handCardPlayer = currentPlayerCard.handCards;
                    var message2 = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                        "Quel carte veux-tu te défausser?", handCardPlayer);

                    message2.GetComponent<MessageBox>().PositiveAction = () =>
                    {
                        var cardPlayer = message2.GetComponent<MessageBox>().GetSelectedCard();
                        if (cardPlayer.IsValid())
                        {
                            currentPlayerCard.yellowTrash.Add(cardPlayer);
                            opponentPlayerCard.yellowTrash.Add(cardOpponent);
                            currentPlayerCard.handCards.Remove(cardPlayer);
                            opponentPlayerCard.handCards.Remove(cardOpponent);
                        }

                        Destroy(message2);
                    };
                };
                message1.GetComponent<MessageBox>().NegativeAction = () => { Destroy(message1); };
            };

            message.GetComponent<MessageBox>().NegativeAction = () => { Destroy(message); };
        }

        private void ApplySeeOpponentHand()
        {
            var handCardOpponent = opponentPlayerCard.handCards;
            MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Voici les cartes de l'adversaire", handCardOpponent, okButton: true);
        }

        private void ApplyHandMax(int handCardsNumber, EffectCard effectCard)
        {
            var handCard1 = currentPlayerCard.handCards;
            handCard1.Remove(effectCard);
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
                    var currentCards = message.GetComponent<MessageBox>().GetMultipleSelectedCards();

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
            var handCardsNumber = int.Parse(value);
            return handCardsNumber;
        }

        private static void ApplyCheckTurn(EffectCard effectCard)
        {
            effectCard.checkTurn = true;
        }

        private void ApplySameFamily()
        {
            var fieldCard = currentPlayerCard.field;
            if (fieldCard.IsValid())
            {
                var familyField = fieldCard.GetFamily();
                foreach (var card in currentPlayerCard.invocationCards.Where(card => card.IsValid()))
                {
                    card.SetCurrentFamily(familyField);
                }
            }
            else
            {
            }
        }

        private void ApplySacrificeInvocation(string value, float pvAffected, UnityAction completion)
        {
            switch (value)
            {
                case "true":
                {
                    var invocationCards = currentPlayerCard.invocationCards;
                    var invocationCardsValid = invocationCards
                        .Where(invocationCard => invocationCard.IsValid()).Cast<Card>().ToList();
                    GenerateSacrificeInvocationMessageBox(invocationCardsValid, pvAffected, completion);
                }
                    break;
                case "5":
                {
                    var invocationCards = currentPlayerCard.invocationCards;
                    var invocationCardsValid = invocationCards
                        .Where(invocationCard => invocationCard.IsValid()).Where(invocationCard =>
                            invocationCard.GetCurrentAttack() >= 5 ||
                            invocationCard.GetCurrentDefense() >= 5).Cast<Card>().ToList();
                    GenerateSacrificeInvocationMessageBox(invocationCardsValid, pvAffected, completion);
                }
                    break;
                case "3":
                {
                    var invocationCards = currentPlayerCard.invocationCards;
                    var invocationCardsValid = invocationCards.Where(invocationCard =>
                            invocationCard.GetCurrentAttack() >= 3 ||
                            invocationCard.GetCurrentDefense() >= 3)
                        .Cast<Card>().ToList();
                    GenerateSacrificeInvocationMessageBox(invocationCardsValid, pvAffected, completion);
                }
                    break;
            }
        }

        private void ApplyDestroyCards(string value, float pvAffected, bool affectOpponent)
        {
            switch (value)
            {
                case "field":
                {
                    var fieldCards = new List<Card>();
                    var fieldCard1 = currentPlayerCard.field;
                    var fieldCard2 = opponentPlayerCard.field;
                    if (fieldCard1 != null)
                    {
                        fieldCards.Add(fieldCard1);
                    }

                    if (fieldCard2 != null)
                    {
                        fieldCards.Add(fieldCard2);
                    }

                    if (fieldCards.Count > 0)
                    {
                        if (pvAffected < 0)
                        {
                            var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                "Choix du terrain à détruire", fieldCards);
                            var affected = pvAffected;
                            message.GetComponent<MessageBox>().PositiveAction = () =>
                            {
                                var fieldCard =
                                    (FieldCard)message.GetComponent<MessageBox>().GetSelectedCard();
                                if (fieldCard != null)
                                {
                                    if (fieldCard1 != null)
                                    {
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
                                    }
                                    else
                                    {
                                        opponentPlayerCard.yellowTrash.Add(fieldCard);
                                        opponentPlayerCard.field = null;
                                    }

                                    currentPlayerStatus.ChangePv(affected);
                                    Destroy(message);
                                }
                                else
                                {
                                    message.SetActive(false);

                                    void OkAction()
                                    {
                                        message.SetActive(true);
                                    }

                                    MessageBox.CreateOkMessageBox(canvas, "Action requise",
                                        "Tu dois choisir un terrain à détruire", OkAction);
                                }
                            };
                            message.GetComponent<MessageBox>().NegativeAction = () =>
                            {
                                message.SetActive(false);

                                void OkAction()
                                {
                                    message.SetActive(true);
                                }

                                MessageBox.CreateOkMessageBox(canvas, "Action requise",
                                    "Tu dois choisir un terrain à détruire", OkAction);
                            };
                        }
                    }
                }
                    break;
                case "all":
                {
                    for (var j = currentPlayerCard.invocationCards.Count - 1; j >= 0; j--)
                    {
                        var invocationCard = currentPlayerCard.invocationCards[j];
                        if (invocationCard.IsAffectedByEffectCard)
                        {
                            currentPlayerCard.SendInvocationCardToYellowTrash(invocationCard);
                        }
                    }

                    for (var j = currentPlayerCard.effectCards.Count - 1; j >= 0; j--)
                    {
                        var effectCard = currentPlayerCard.effectCards[j];
                        currentPlayerCard.SendCardToYellowTrash(effectCard);
                    }

                    if (currentPlayerCard.field != null)
                    {
                        currentPlayerCard.SendCardToYellowTrash(currentPlayerCard.field);
                    }

                    for (var j = opponentPlayerCard.invocationCards.Count - 1; j >= 0; j--)
                    {
                        var invocationCard = opponentPlayerCard.invocationCards[j];
                        if (invocationCard.IsAffectedByEffectCard)
                        {
                            opponentPlayerCard.SendInvocationCardToYellowTrash(invocationCard);
                        }
                    }

                    for (var j = opponentPlayerCard.effectCards.Count - 1; j >= 0; j--)
                    {
                        var effectCard = opponentPlayerCard.effectCards[j];
                        opponentPlayerCard.SendCardToYellowTrash(effectCard);
                    }

                    if (opponentPlayerCard.field != null)
                    {
                        opponentPlayerCard.SendCardToYellowTrash(opponentPlayerCard.field);
                    }
                }
                    break;
                case "invocation":
                {
                    if (affectOpponent)
                    {
                        var invocationOpponentValid = opponentPlayerCard.invocationCards
                            .Where(card => card.IsAffectedByEffectCard).Cast<Card>().ToList();
                        var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                            "Choix de l'invocation à détruire", invocationOpponentValid);
                        message.GetComponent<MessageBox>().PositiveAction = () =>
                        {
                            var invocationCard =
                                (InvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();
                            if (invocationCard != null)
                            {
                                opponentPlayerCard.SendInvocationCardToYellowTrash(invocationCard);
                                Destroy(message);
                            }
                            else
                            {
                                message.SetActive(false);

                                void OkAction()
                                {
                                    message.SetActive(true);
                                }

                                MessageBox.CreateOkMessageBox(canvas, "Action requise", "Choisis une carte à détruire",
                                    OkAction);
                            }
                        };
                        message.GetComponent<MessageBox>().NegativeAction = () =>
                        {
                            message.SetActive(false);

                            void OkAction()
                            {
                                message.SetActive(true);
                            }

                            MessageBox.CreateOkMessageBox(canvas, "Action requise", "Choisis une carte à détruire",
                                OkAction);
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
                    var invocationCards1 = currentPlayerCard.invocationCards.Where(card => card.IsAffectedByEffectCard)
                        .Cast<Card>().ToList();
                    var invocationCards2 = opponentPlayerCard.invocationCards.Where(card => card.IsAffectedByEffectCard)
                        .Cast<Card>().ToList();

                    var allCardsOnField = new List<Card>();

                    if (fieldCard1 != null)
                    {
                        allCardsOnField.Add(fieldCard1);
                    }

                    if (fieldCard2 != null)
                    {
                        allCardsOnField.Add(fieldCard2);
                    }

                    allCardsOnField.AddRange(effectCards1.Where(card => card.IsValid()));

                    allCardsOnField.AddRange(effectCards2.Where(card => card.IsValid()));

                    allCardsOnField.AddRange(invocationCards1.Where(card => card.IsValid()));

                    allCardsOnField.AddRange(invocationCards2.Where(card => card.IsValid()));

                    var message = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                        "Choix de la carte à détruire", allCardsOnField);

                    message.GetComponent<MessageBox>().PositiveAction = () =>
                    {
                        var card = message.GetComponent<MessageBox>().GetSelectedCard();
                        if (card != null)
                        {
                            switch (card.Type)
                            {
                                case CardType.Invocation:
                                    FindCardInArrayAndSendItToTrash(invocationCards1, invocationCards2, card);
                                    break;
                                case CardType.Effect:
                                    FindCardInArrayAndSendItToTrash(effectCards1, effectCards2, card);
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
                                case CardType.Contre:
                                    break;
                                case CardType.Equipment:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        Destroy(message);
                    };
                    message.GetComponent<MessageBox>().NegativeAction = () => { Destroy(message); };
                }
                    break;
            }
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
            var pvAffected = value == "all" ? 100.0f : float.Parse(value);

            effectCard.affectPv = pvAffected;

            return pvAffected;
        }

        private void GenerateSacrificeInvocationMessageBox(List<Card> invocationCardsValid,
            float pvAffected, UnityAction completion)
        {
            var message =
                MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix de l'invocation à sacrifier",
                    invocationCardsValid);


            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var card = message.GetComponent<MessageBox>().GetSelectedCard();
                if (card != null)
                {
                    currentPlayerCard.SendInvocationCardToYellowTrash(card as InvocationCard);
                    if (pvAffected != 0)
                    {
                        currentPlayerStatus.ChangePv(pvAffected);
                    }

                    completion();
                    Destroy(message);
                }
                else
                {
                    message.SetActive(false);

                    void OkAction()
                    {
                        message.SetActive(true);
                    }

                    MessageBox.CreateOkMessageBox(canvas, "Action requise", "Tu dois choisir une carte à sacrifier",
                        OkAction);
                }
            };
            message.GetComponent<MessageBox>().NegativeAction = () =>
            {
                message.SetActive(false);

                void OkAction()
                {
                    message.SetActive(true);
                }

                MessageBox.CreateOkMessageBox(canvas, "Action requise", "Tu dois choisir une carte à sacrifier",
                    OkAction);
            };
        }


        private void FindCardInArrayAndSendItToTrash(IReadOnlyList<Card> cards1, IReadOnlyList<Card> cards2, Card card)
        {
            var found = false;
            var j = 0;
            while (j < cards2.Count && !found)
            {
                var card2 = cards2[j];
                if (card2 != null)
                {
                    if (card2.Nom == card.Nom)
                    {
                        found = true;
                        switch (card.Type)
                        {
                            case CardType.Invocation:
                            {
                                opponentPlayerCard.SendInvocationCardToYellowTrash(card as InvocationCard);
                            }
                                break;
                            case CardType.Effect:
                            {
                                opponentPlayerCard.effectCards.Remove(card as EffectCard);
                                opponentPlayerCard.yellowTrash.Add(card);
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
                if (card1 != null)
                {
                    if (card1.Nom == card.Nom)
                    {
                        found = true;
                        switch (card.Type)
                        {
                            case CardType.Invocation:
                            {
                                currentPlayerCard.SendInvocationCardToYellowTrash(card as InvocationCard);
                            }
                                break;
                            case CardType.Effect:
                            {
                                currentPlayerCard.yellowTrash.Add(card);
                                currentPlayerCard.effectCards.Remove(card as EffectCard);
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
}