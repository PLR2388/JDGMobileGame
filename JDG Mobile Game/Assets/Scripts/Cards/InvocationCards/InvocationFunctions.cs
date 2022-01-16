﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cards.FieldCards;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cards.InvocationCards
{
    public class InvocationFunctions : MonoBehaviour
    {
        private PlayerCards currentPlayerCard;
        private PlayerCards opponentPlayerCards;
        private GameObject p1;
        private GameObject p2;
        [SerializeField] private GameObject inHandButton;
        [SerializeField] private Transform canvas;
        [SerializeField] private GameObject invocationMenu;

        private void Start()
        {
            GameLoop.ChangePlayer.AddListener(ChangePlayer);
            InGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
            p1 = GameObject.Find("Player1");
            p2 = GameObject.Find("Player2");
            currentPlayerCard = p1.GetComponent<PlayerCards>();
            opponentPlayerCards = p2.GetComponent<PlayerCards>();
        }

        private void ChangePlayer()
        {
            currentPlayerCard = GameLoop.IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            opponentPlayerCards = GameLoop.IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        }

        private void DealWithStartEffect(InvocationCard currentInvocationCard,
            InvocationStartEffect invocationStartEffect)
        {
            var keys = invocationStartEffect.Keys;
            var values = invocationStartEffect.Values;

            var cardName = "";
            var typeCard = "";
            var familyName = "";
            var invokeCardNames = new List<string>();
            var cardFound = new List<Card>();
            var mustDivideAttack = false;
            var mustDivideDefense = false;
            for (var i = 0; i < keys.Count; i++)
            {
                switch (keys[i])
                {
                    case StartEffect.GetSpecificCard:
                    {
                        cardName = values[i];
                    }
                        break;
                    case StartEffect.GetSpecificTypeCard:
                    {
                        typeCard = values[i];
                    }
                        break;
                    case StartEffect.GetCardFamily:
                    {
                        familyName = values[i];
                    }
                        break;
                    case StartEffect.GetCardSource:
                    {
                        var source = values[i];
                        switch (source)
                        {
                            case "deck":
                            {
                                var deck = currentPlayerCard.deck;
                                if (cardName != "")
                                {
                                    var isFound = false;
                                    var j = 0;
                                    while (j < deck.Count && !isFound)
                                    {
                                        if (deck[j].Nom == cardName)
                                        {
                                            isFound = true;
                                            cardFound.Add(deck[j]);
                                        }

                                        j++;
                                    }

                                    if (isFound)
                                    {
                                        void PositiveAction()
                                        {
                                            currentPlayerCard.handCards.Add(cardFound[0]);
                                            currentPlayerCard.deck.Remove(cardFound[0]);
                                            inHandButton.SetActive(true);
                                        }

                                        void NegativeAction()
                                        {
                                            inHandButton.SetActive(true);
                                        }

                                        CreateMessageBoxSimple("Carte en main",
                                            "Voulez-vous aussi ajouter " + cardName + " à votre main ?",
                                            positiveAction: PositiveAction, NegativeAction);
                                    }
                                }
                                else if (invokeCardNames.Count > 0)
                                {
                                    cardFound.AddRange(from t in deck
                                        from invokeCardName in invokeCardNames
                                        where t.Nom == invokeCardName
                                        select t);

                                    var size = currentPlayerCard.invocationCards.Count;
                                    if (cardFound.Count > 0 && size < 4)
                                    {
                                        if (cardFound.Count == 1)
                                        {
                                            void PositiveAction()
                                            {
                                                var invocationCard = (InvocationCard)cardFound[cardFound.Count - 1];
                                                currentPlayerCard.invocationCards.Add(invocationCard);
                                                currentPlayerCard.deck.Remove(invocationCard);
                                                inHandButton.SetActive(true);
                                            }

                                            void NegativeAction()
                                            {
                                                inHandButton.SetActive(true);
                                            }

                                            CreateMessageBoxSimple("Invocation",
                                                "Voulez-vous aussi invoquer " + cardFound[0].Nom + " ?",
                                                positiveAction: PositiveAction, NegativeAction);
                                        }
                                        else
                                        {
                                            var message =
                                                CreateMessageBoxSelectorCard("Choix de l'invocation", cardFound);
                                            message.GetComponent<MessageBox>().PositiveAction = () =>
                                            {
                                                var invocationCard =
                                                    (InvocationCard)message.GetComponent<MessageBox>()
                                                        .GetSelectedCard();
                                                currentPlayerCard.invocationCards.Add(invocationCard);
                                                currentPlayerCard.deck.Remove(invocationCard);
                                                inHandButton.SetActive(true);
                                                Destroy(message);
                                            };
                                            message.GetComponent<MessageBox>().NegativeAction = () =>
                                            {
                                                inHandButton.SetActive(true);
                                                Destroy(message);
                                            };
                                        }
                                    }
                                }
                                else if (Enum.TryParse(typeCard, out CardType type))
                                {
                                    cardFound.AddRange(deck.Where(t => t.Type == type));

                                    if (cardFound.Count > 0)
                                    {
                                    }
                                }
                                else if (Enum.TryParse(familyName, out CardFamily cardFamily))
                                {
                                    foreach (var card in deck)
                                    {
                                        if (card.Type != CardType.Invocation) continue;
                                        var invocationCard = (InvocationCard)card;

                                        var listFamily = invocationCard.GetFamily();
                                        cardFound.AddRange((from family in listFamily
                                            where family == cardFamily
                                            select invocationCard));
                                    }

                                    if (cardFound.Count > 0)
                                    {
                                        var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                            "Choisis une carte à ajouter à ta main", cardFound);
                                        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                                        {
                                            var card = messageBox.GetComponent<MessageBox>().GetSelectedCard();
                                            if (card != null)
                                            {
                                                currentPlayerCard.deck.Remove(card);
                                                currentPlayerCard.handCards.Add(card);
                                                Destroy(messageBox);
                                            }
                                            else
                                            {
                                                messageBox.SetActive(false);

                                                void OkAction()
                                                {
                                                    messageBox.SetActive(true);
                                                    Destroy(messageBox);
                                                }

                                                MessageBox.CreateOkMessageBox(canvas, "Information",
                                                    "Tu n'as pris aucune carte", OkAction);
                                            }
                                        };
                                        messageBox.GetComponent<MessageBox>().NegativeAction = () =>
                                        {
                                            messageBox.SetActive(false);

                                            void OkAction()
                                            {
                                                messageBox.SetActive(true);
                                                Destroy(messageBox);
                                            }

                                            MessageBox.CreateOkMessageBox(canvas, "Information",
                                                "Tu n'as pris aucune carte", OkAction);
                                        };
                                    }
                                }

                                break;
                            }
                            case "trash":
                            {
                                var trash = currentPlayerCard.yellowTrash;
                                if (cardName != "")
                                {
                                    var isFound = false;
                                    var j = 0;
                                    while (j < trash.Count && !isFound)
                                    {
                                        if (trash[j].Nom == cardName)
                                        {
                                            isFound = true;
                                            cardFound.Add(trash[j]);
                                        }

                                        j++;
                                    }

                                    if (isFound)
                                    {
                                        void PositiveAction()
                                        {
                                            currentPlayerCard.handCards.Add(cardFound[0]);
                                            currentPlayerCard.yellowTrash.Remove(cardFound[0]);
                                            inHandButton.SetActive(true);
                                        }

                                        void NegativeAction()
                                        {
                                            inHandButton.SetActive(true);
                                        }

                                        CreateMessageBoxSimple("Carte en main",
                                            "Voulez-vous aussi ajouter " + cardName + " à votre main ?",
                                            positiveAction: PositiveAction, NegativeAction);
                                    }
                                }
                                else if (invokeCardNames.Count > 0)
                                {
                                    cardFound.AddRange(from t in trash
                                        from invokeCardName in invokeCardNames
                                        where t.Nom == invokeCardName
                                        select t);
                                }
                                else if (Enum.TryParse(typeCard, out CardType type))
                                {
                                    cardFound.AddRange(trash.Where(t => t.Type == type));
                                }
                                else if (Enum.TryParse(familyName, out CardFamily cardFamily))
                                {
                                    foreach (var card in trash)
                                    {
                                        if (card.Type != CardType.Invocation) continue;
                                        var invocationCard = (InvocationCard)card;

                                        var listFamily = invocationCard.GetFamily();
                                        cardFound.AddRange(
                                            (from t in listFamily where t == cardFamily select invocationCard));
                                    }
                                }

                                break;
                            }
                        }
                    }
                        break;
                    case StartEffect.RemoveAllInvocationCards:
                    {
                        var dontRemoveCard = values[i];
                        var p1InvocationCards = this.p1.GetComponent<PlayerCards>().invocationCards;
                        var p2InvocationCards = this.p2.GetComponent<PlayerCards>().invocationCards;


                        for (var j = p1InvocationCards.Count - 1; j >= 0; j--)
                        {
                            if (p1InvocationCards[j].Nom == dontRemoveCard) continue;
                            p1.GetComponent<PlayerCards>().handCards.Add(p1InvocationCards[j]);
                            p1.GetComponent<PlayerCards>().invocationCards
                                .Remove(p1InvocationCards[j]);
                        }

                        for (var j = p2InvocationCards.Count - 1; j >= 0; j--)
                        {
                            if (p2InvocationCards[j].Nom == dontRemoveCard) continue;
                            p2.GetComponent<PlayerCards>().handCards.Add(p2InvocationCards[j]);
                            p2.GetComponent<PlayerCards>().invocationCards
                                .Remove(p2InvocationCards[j]);
                        }
                    }
                        break;
                    case StartEffect.InvokeSpecificCard:
                    {
                        invokeCardNames.Add(values[i]);
                    }
                        break;
                    case StartEffect.PutField:
                    {
                        if (cardFound.Count > 0)
                        {
                            if (currentPlayerCard.field == null)
                            {
                                var message = CreateMessageBoxSelectorCard("Choix du terrain à poser", cardFound);

                                message.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    var fieldCard =
                                        (FieldCard)message.GetComponent<MessageBox>().GetSelectedCard();

                                    if (fieldCard != null)
                                    {
                                        currentPlayerCard.field = fieldCard;
                                        currentPlayerCard.deck.Remove(fieldCard);
                                    }
                                    else
                                    {
                                        CreateMessageBoxNotChoosenCard();
                                    }

                                    inHandButton.SetActive(true);
                                    Destroy(message);
                                };
                                message.GetComponent<MessageBox>().NegativeAction = () =>
                                {
                                    inHandButton.SetActive(true);
                                    Destroy(message);
                                };
                            }
                            else
                            {
                                var message =
                                    CreateMessageBoxSelectorCard("Choix du terrain à prendre en main", cardFound);


                                message.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    var fieldCard =
                                        (FieldCard)message.GetComponent<MessageBox>().GetSelectedCard();

                                    if (fieldCard != null)
                                    {
                                        currentPlayerCard.deck.Add(fieldCard);
                                        currentPlayerCard.deck.Remove(fieldCard);
                                    }
                                    else
                                    {
                                        CreateMessageBoxNotChoosenCard();
                                    }

                                    inHandButton.SetActive(true);
                                    Destroy(message);
                                };
                                message.GetComponent<MessageBox>().NegativeAction = () =>
                                {
                                    inHandButton.SetActive(true);
                                    Destroy(message);
                                };
                            }
                        }
                    }
                        break;
                    case StartEffect.DestroyField:
                    {
                        var fieldCardP1 = p1.GetComponent<PlayerCards>().field;
                        var fieldCardP2 = p2.GetComponent<PlayerCards>().field;

                        if (fieldCardP1 != null)
                        {
                            cardFound.Add(fieldCardP1);
                        }

                        if (fieldCardP2 != null)
                        {
                            cardFound.Add(fieldCardP2);
                        }

                        if (cardFound.Count > 0)
                        {
                            var message = CreateMessageBoxSelectorCard("Choix du terrain à détruire", cardFound);
                            var attack = mustDivideAttack;
                            var defense = mustDivideDefense;
                            message.GetComponent<MessageBox>().PositiveAction = () =>
                            {
                                var fieldCard =
                                    (FieldCard)message.GetComponent<MessageBox>().GetSelectedCard();

                                if (fieldCard != null)
                                {
                                    if (attack)
                                    {
                                        currentInvocationCard.SetBonusAttack(-currentInvocationCard.GetAttack() / 2);
                                    }

                                    if (defense)
                                    {
                                        currentInvocationCard.SetBonusAttack(-currentInvocationCard.GetDefense() / 2);
                                    }

                                    if (fieldCard.Nom == fieldCardP1.Nom)
                                    {
                                        p1.GetComponent<PlayerCards>().field = null;
                                        p1.GetComponent<PlayerCards>().yellowTrash.Add(fieldCard);
                                    }
                                    else
                                    {
                                        p2.GetComponent<PlayerCards>().field = null;
                                        p2.GetComponent<PlayerCards>().yellowTrash.Add(fieldCard);
                                    }
                                }
                                else
                                {
                                    CreateMessageBoxNotChoosenCard();
                                }

                                inHandButton.SetActive(true);
                                Destroy(message);
                            };
                            message.GetComponent<MessageBox>().NegativeAction = () =>
                            {
                                inHandButton.SetActive(true);
                                currentInvocationCard.UnblockAttack();
                                Destroy(message);
                            };
                        }
                    }
                        break;
                    case StartEffect.Divide2Atk:
                    {
                        mustDivideAttack = true;
                    }
                        break;
                    case StartEffect.Divide2Def:
                    {
                        mustDivideDefense = true;
                    }
                        break;
                    case StartEffect.SendToDeath:
                    {
                        PlayerCards opponentPlayerCards;
                        if (GameLoop.IsP1Turn)
                        {
                            opponentPlayerCards = p2.GetComponent<PlayerCards>();
                            var p2InvocationCards = opponentPlayerCards.invocationCards;

                            cardFound.AddRange(p2InvocationCards);
                        }
                        else
                        {
                            opponentPlayerCards = p1.GetComponent<PlayerCards>();
                            var p1InvocationCards = opponentPlayerCards.invocationCards;

                            cardFound.AddRange(p1InvocationCards);
                        }


                        var message = CreateMessageBoxSelectorCard("Choix de la carte à tuer :", cardFound);
                        message.GetComponent<MessageBox>().PositiveAction = () =>
                        {
                            var invocationCardSelected =
                                (InvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();
                            if (invocationCardSelected != null)
                            {
                                var invocationCards = opponentPlayerCards.invocationCards;
                                var k = 0;
                                var found = false;
                                while (!found && k < invocationCards.Count)
                                {
                                    if (invocationCards[k] != null &&
                                        invocationCards[k].Nom == invocationCardSelected.Nom)
                                    {
                                        found = true;
                                    }
                                    else
                                    {
                                        k++;
                                    }
                                }

                                if (found)
                                {
                                    if (GameLoop.IsP1Turn)
                                    {
                                        p2.GetComponent<PlayerCards>().invocationCards.Remove(invocationCardSelected);
                                        p2.GetComponent<PlayerCards>().yellowTrash.Add(invocationCardSelected);
                                    }
                                    else
                                    {
                                        p1.GetComponent<PlayerCards>().invocationCards.Remove(invocationCardSelected);
                                        p1.GetComponent<PlayerCards>().yellowTrash.Add(invocationCardSelected);
                                    }

                                    currentInvocationCard.IncrementNumberDeaths();
                                }
                                else
                                {
                                    Debug.Log("Something went wrong!");
                                }
                            }
                            else
                            {
                                CreateMessageBoxNotChoosenCard();
                            }

                            inHandButton.SetActive(true);
                            Destroy(message);
                        };
                        message.GetComponent<MessageBox>().NegativeAction = () =>
                        {
                            inHandButton.SetActive(true);
                            Destroy(message);
                        };
                    }
                        break;
                    case StartEffect.DrawXCards:
                    {
                        var x = int.Parse(values[i]);
                        var size = currentPlayerCard.deck.Count;
                        if (size > 0)
                        {
                            int maxCardDraw;
                            if ((size - x) >= 0)
                            {
                                maxCardDraw = x;
                            }
                            else
                            {
                                maxCardDraw = size % x;
                            }

                            void PositiveAction()
                            {
                                for (var j = size - 1; j >= size - 1 - maxCardDraw; j--)
                                {
                                    var c = currentPlayerCard.deck[j];
                                    currentPlayerCard.handCards.Add(c);
                                    currentPlayerCard.deck.Remove(c);
                                }

                                inHandButton.SetActive(true);
                            }

                            void NegativeAction()
                            {
                                inHandButton.SetActive(true);
                            }

                            CreateMessageBoxSimple("Action possible",
                                "Voulez-vous piocher " + maxCardDraw + " cartes ?", PositiveAction, NegativeAction);
                        }
                    }
                        break;
                    case StartEffect.Condition:
                    {
                        var condition = values[i];
                        switch (condition)
                        {
                            case "skipAttack":
                            {
                                var m = CreateMessageBoxSimple("Action possible",
                                    "Voulez-vous sauter la phase d'attaque de la carte pour ajouter une carte equipement de votre pioche à votre main ?");
                                m.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    m.GetComponent<MessageBox>().description = "";
                                    List<Card> deck = currentPlayerCard.deck;
                                    var equipmentCards = deck.Where(card => card.Type == CardType.Equipment).ToList();
                                    GameObject messageBox1 =
                                        CreateMessageBoxSelectorCard("Choix de la carte équipement", equipmentCards);
                                    messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                                    {
                                        var selectedCard = messageBox1.GetComponent<MessageBox>().GetSelectedCard();
                                        if (selectedCard != null)
                                        {
                                            currentPlayerCard.handCards.Add(selectedCard);
                                            currentPlayerCard.deck.Remove(selectedCard);
                                            var index = currentPlayerCard.GetIndexInvocationCard(currentInvocationCard
                                                .Nom);
                                            currentPlayerCard.invocationCards[index].BlockAttack();
                                            currentInvocationCard.BlockAttack();
                                        }

                                        inHandButton.SetActive(true);
                                        Destroy(messageBox1);
                                    };
                                    messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                                    {
                                        inHandButton.SetActive(true);
                                        Destroy(messageBox1);
                                    };
                                    Destroy(m);
                                };
                                m.GetComponent<MessageBox>().NegativeAction = () =>
                                {
                                    inHandButton.SetActive(true);
                                    Destroy(m);
                                };
                            }
                                break;
                            case "cantAttack":
                            {
                                currentInvocationCard.BlockAttack();
                            }
                                break;
                        }
                    }
                        break;
                    case StartEffect.SacrificeFieldIncrement:
                    {
                        var elements = values[i].Split(';');
                        var sacrifice = elements[0];
                        var field = elements[1];
                        float incrementStat = float.Parse(elements[2]);

                        var currentField = currentPlayerCard.field;
                        if (currentField != null && field == currentField.Nom)
                        {
                            var invocationCardOnField = currentPlayerCard.invocationCards;
                            var k = 0;
                            var found = false;
                            while (!found && k < invocationCardOnField.Count)
                            {
                                if (invocationCardOnField[k] != null &&
                                    invocationCardOnField[k].Nom == sacrifice)
                                {
                                    found = true;
                                }
                                else
                                {
                                    k++;
                                }
                            }

                            if (found)
                            {
                                var cardToSacrifice = invocationCardOnField[k];

                                void PositiveAction()
                                {
                                    currentPlayerCard.invocationCards.Remove(cardToSacrifice);
                                    currentPlayerCard.yellowTrash.Add(cardToSacrifice);
                                    currentInvocationCard.SetBonusAttack(incrementStat);
                                    currentInvocationCard.SetBonusDefense(incrementStat);
                                }

                                MessageBox.CreateSimpleMessageBox(canvas, "Choix",
                                    "Voulez-vous sacrifier " + sacrifice + " pour gagner " + incrementStat +
                                    " en ATK et DEF ?", PositiveAction);
                            }
                        }
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void PutInvocationCard(InvocationCard invocationCard)
        {
            var size = currentPlayerCard.invocationCards.Count;

            if (size >= 4) return;

            var invocationStartEffect = invocationCard.GetInvocationStartEffect();
            var invocationConditionEffect = invocationCard.InvocationConditions;

            if (invocationConditionEffect != null)
            {
                DealWithConditionInvocation(invocationConditionEffect);
                currentPlayerCard.invocationCards.Add(invocationCard);
                currentPlayerCard.handCards.Remove(invocationCard);
            }
            else
            {
                currentPlayerCard.invocationCards.Add(invocationCard);
                currentPlayerCard.handCards.Remove(invocationCard);
            }

            if (invocationStartEffect != null)
            {
                DealWithStartEffect(invocationCard, invocationStartEffect);
            }
        }

        public bool IsSpecialActionPossible(InvocationCard currentInvocationCard,
            InvocationActionEffect invocationActionEffect)
        {
            var isPossible = false;
            if (invocationActionEffect == null) return false;
            var keys = invocationActionEffect.Keys;
            var values = invocationActionEffect.Values;
            var atk = 0.0f;
            var def = 0.0f;
            for (var i = 0; i < keys.Count; i++)
            {
                switch (keys[i])
                {
                    case ActionEffect.SacrificeInvocation:
                        break;
                    case ActionEffect.BackToLife:
                    {
                        var trash = currentPlayerCard.yellowTrash;

                        var invocationCardDead =
                            trash.Where(card => card is InvocationCard && !card.Collector).ToList();

                        if (invocationCardDead.Count > 0)
                        {
                            isPossible = true;
                        }
                    }
                        break;
                    case ActionEffect.GiveAtk:
                        atk = float.Parse(values[i]);
                        break;
                    case ActionEffect.GiveDef:
                        def = float.Parse(values[i]);
                        break;
                    case ActionEffect.SpecificFamily:
                    {
                        var familyName = values[i];
                        var family = (CardFamily)Enum.Parse(typeof(CardFamily), familyName);

                        var invocationsOnField = currentPlayerCard.invocationCards;

                        var invocationCardSameFamily = new List<Card>();
                        foreach (var card in invocationsOnField.Where(card => card != null))
                        {
                            if (card.Nom == currentInvocationCard.Nom)
                            {
                            }
                            else
                            {
                                var currentFamilies = card.GetFamily();
                                if (currentFamilies.Any(currentFamily => currentFamily == family))
                                {
                                    invocationCardSameFamily.Add(card);
                                }
                            }
                        }

                        if (invocationCardSameFamily.Count > 0)
                        {
                            if (def > 0 && atk > 0)
                            {
                                if (currentInvocationCard.GetBonusDefense() > -def)
                                {
                                    isPossible = true;
                                }

                                if (currentInvocationCard.GETBonusAttack() > -atk)
                                {
                                    isPossible = true;
                                }
                            }
                        }
                    }
                        break;
                    case ActionEffect.SkipOpponentAttack:
                        isPossible = opponentPlayerCards.invocationCards.Count > 0;
                        break;
                    case ActionEffect.Beneficiary:
                    default:
                        break;
                }
            }

            return isPossible;
        }

        public void AskIfUserWantToUseActionEffect(InvocationCard currentInvocationCard,
            InvocationActionEffect invocationActionEffect)
        {
            var keys = invocationActionEffect.Keys;
            var values = invocationActionEffect.Values;
            string cardName = null;
            var atk = 0.0f;
            var def = 0.0f;
            for (var i = 0; i < keys.Count; i++)
            {
                switch (keys[i])
                {
                    case ActionEffect.SacrificeInvocation:
                        cardName = values[i];
                        break;
                    case ActionEffect.BackToLife:
                    {
                        var trash = currentPlayerCard.yellowTrash;

                        var invocationCardDead =
                            trash.Where(card => card is InvocationCard && !card.Collector).ToList();

                        if (invocationCardDead.Count > 0)
                        {
                            var messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Choix",
                                "Voulez-vous sacrifier " + cardName +
                                " pour ramener à la vie une carte invocation de la poubelle jaune ?");
                            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                            {
                                var message =
                                    MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                        "Choix de la carte à ressusciter",
                                        invocationCardDead);
                                var name1 = cardName;
                                message.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    var invocationCards = currentPlayerCard.invocationCards;
                                    var found = false;
                                    var k = 0;
                                    while (!found && k < invocationCards.Count)
                                    {
                                        if (invocationCards[k] != null && invocationCards[k].Nom == name1)
                                        {
                                            found = true;
                                        }
                                        else
                                        {
                                            k++;
                                        }
                                    }


                                    var invocationCardSelected =
                                        (InvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();
                                    if (invocationCardSelected != null)
                                    {
                                        currentPlayerCard.SendInvocationCardToYellowTrash(invocationCards[k]);
                                        currentPlayerCard.yellowTrash.Remove(invocationCardSelected);
                                        currentPlayerCard.invocationCards.Add(invocationCardSelected);
                                    }
                                    else
                                    {
                                        CreateMessageBoxNotChoosenCard();
                                    }

                                    Destroy(message);
                                    Destroy(messageBox);
                                };
                                message.GetComponent<MessageBox>().NegativeAction = () => { Destroy(message); };
                            };
                        }
                    }
                        break;
                    case ActionEffect.GiveAtk:
                        atk = float.Parse(values[i]);
                        break;
                    case ActionEffect.GiveDef:
                        def = float.Parse(values[i]);
                        break;
                    case ActionEffect.SpecificFamily:
                    {
                        var familyName = values[i];
                        CardFamily family = (CardFamily)Enum.Parse(typeof(CardFamily), familyName);

                        var invocationsOnField = currentPlayerCard.invocationCards;
                        var indexCurrent = 0;

                        List<Card> invocationCardSameFamily = new List<Card>();
                        for (int j = 0; j < invocationsOnField.Count; j++)
                        {
                            var card = invocationsOnField[j];
                            if (card != null)
                            {
                                if (card.Nom == currentInvocationCard.Nom)
                                {
                                    indexCurrent = j;
                                }
                                else
                                {
                                    var currentFamilies = card.GetFamily();
                                    if (currentFamilies.Any(t => t == family))
                                    {
                                        invocationCardSameFamily.Add(card);
                                    }
                                }
                            }
                        }

                        if (invocationCardSameFamily.Count > 0)
                        {
                            var messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Choix",
                                "Voulez-vous augmenter les stats d'une carte de la même famille en diminuant les stats de la carte ?");
                            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                            {
                                var message =
                                    MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                        "Choix de la carte à augmenter les stats", invocationCardSameFamily);
                                message.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    var invocationCardSelected =
                                        (InvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();
                                    if (invocationCardSelected != null)
                                    {
                                        for (int j = 0; j < invocationsOnField.Count; j++)
                                        {
                                            var card = invocationsOnField[j];
                                            if (card != null && card.Nom == invocationCardSelected.Nom)
                                            {
                                                var beneficiary = currentPlayerCard.invocationCards[j];
                                                var currentInvocation = currentPlayerCard.invocationCards[indexCurrent];
                                                var newBonusAttack = beneficiary.GETBonusAttack() + atk;
                                                var newBonusDef = beneficiary.GETBonusDefense() + def;

                                                var newMalusAttack = currentInvocation.GETBonusAttack() - atk;
                                                var newMalusDef = currentInvocation.GETBonusDefense() - def;

                                                currentPlayerCard.invocationCards[j].SetBonusAttack(newBonusAttack);
                                                currentPlayerCard.invocationCards[j].SetBonusDefense(newBonusDef);
                                                currentPlayerCard.invocationCards[indexCurrent]
                                                    .SetBonusAttack(newMalusAttack);
                                                currentPlayerCard.invocationCards[indexCurrent]
                                                    .SetBonusDefense(newMalusDef);

                                                invocationMenu.transform.GetChild(1).GetComponent<Button>()
                                                        .interactable =
                                                    false;

                                                keys.Add(ActionEffect.Beneficiary);
                                                values.Add(card.Nom);
                                                invocationActionEffect.Keys = keys;
                                                invocationActionEffect.Values = values;
                                                currentPlayerCard.invocationCards[indexCurrent].InvocationActionEffect =
                                                    invocationActionEffect;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CreateMessageBoxNotChoosenCard();
                                    }

                                    Destroy(message);
                                };
                                message.GetComponent<MessageBox>().NegativeAction = () => { Destroy(message); };
                                Destroy(messageBox);
                            };
                            messageBox.GetComponent<MessageBox>().NegativeAction = () => { Destroy(messageBox); };
                        }
                    }
                        break;
                    case ActionEffect.SkipOpponentAttack:
                    {
                        var currentOpponentCard = GameLoop.IsP1Turn
                            ? p2.GetComponent<PlayerCards>()
                            : p1.GetComponent<PlayerCards>();

                        var invocationCards = currentOpponentCard.invocationCards;

                        if (invocationCards.Count > 0)
                        {
                            List<Card> validList = invocationCards.Cast<Card>().ToList();
                            var message =
                                MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                    "Choix de la carte qui ne pourra pas attaquer le prochain tour", validList);
                            message.GetComponent<MessageBox>().PositiveAction = () =>
                            {
                                var invocationCardSelected =
                                    (InvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();
                                if (invocationCardSelected != null)
                                {
                                    invocationCardSelected.BlockAttack();
                                    invocationMenu.transform.GetChild(1).GetComponent<Button>().interactable = false;
                                }
                                else
                                {
                                    CreateMessageBoxNotChoosenCard();
                                }

                                Destroy(message);
                            };
                            message.GetComponent<MessageBox>().NegativeAction = () => { Destroy(message); };
                        }
                    }


                        break;
                    case ActionEffect.Beneficiary:
                    default:
                        break;
                }
            }
        }

        private static InvocationCard CheckSpecificCardOnField(string cardName, PlayerCards currentPlayerCards)
        {
            var found = false;
            var invocationCards = currentPlayerCards.invocationCards;
            var size = invocationCards.Count;
            InvocationCard invocationCard = null;

            var j = 0;
            while (j < size && !found)
            {
                if (invocationCards[j] != null && invocationCards[j].Nom == cardName)
                {
                    found = true;
                    invocationCard = invocationCards[j];
                }

                j++;
            }

            return invocationCard;
        }

        private static InvocationCard CheckSacrificeSpecificCard(string cardName, PlayerCards currentPlayerCards)
        {
            InvocationCard foundCard = null;
            var found = false;
            var invocationCards = currentPlayerCards.invocationCards;
            var size = invocationCards.Count;
            var j = 0;

            while (j < size && !found)
            {
                if (invocationCards[j] != null && invocationCards[j].Nom == cardName)
                {
                    foundCard = invocationCards[j];
                    found = true;
                }


                j++;
            }

            return foundCard;
        }

        private static bool CheckSpecificEquipmentAttached(InvocationCard invocationCard, string equipmentName)
        {
            var isChecked = false;
            if (!invocationCard) return false;
            if (invocationCard.GETEquipmentCard())
            {
                isChecked = invocationCard.GETEquipmentCard().Nom == equipmentName;
            }

            return isChecked;
        }

        private static bool CheckSpecificField(string fieldName, PlayerCards currentPlayerCards)
        {
            var isCorrectField = false;
            var fieldCard = currentPlayerCards.field;
            if (fieldCard)
            {
                isCorrectField = fieldCard.Nom == fieldName;
            }

            return isCorrectField;
        }

        private static List<InvocationCard> CheckFamily(CardFamily cardFamily, PlayerCards currentPlayerCards)
        {
            var invocationCards = currentPlayerCards.invocationCards;

            return (from t in invocationCards
                where t != null
                let families = t.GetFamily()
                from family in families
                where family == cardFamily
                select t).ToList();
        }

        private static List<InvocationCard> CheckThreshold(bool isAttack, float value, PlayerCards currentPlayerCards)
        {
            var threshold = new List<InvocationCard>();
            var invocationCards = currentPlayerCards.invocationCards;
            threshold.AddRange(isAttack
                ? invocationCards.Where(invocationCard =>
                    invocationCard != null && invocationCard.Nom != null && invocationCard.GetCurrentAttack() >= value)
                : invocationCards.Where(invocationCard => invocationCard.GetCurrentDefense() >= value));

            return threshold;
        }

        private static int CheckNumberInvocationCardInYellowTrash(PlayerCards currentPlayerCards)
        {
            var trashCards = currentPlayerCards.yellowTrash;

            return trashCards.Count(card => card.Type == CardType.Invocation);
        }

        private void DealWithConditionInvocation(InvocationConditions conditions)
        {
            var cardConditions = conditions.Keys;
            var cardExplanation = conditions.Values;

            InvocationCard specificCardFound = null;

            var sacrificedCards = new List<InvocationCard>();

            var invocationCardsOnField = new List<InvocationCard>();

            var i = 0;
            while (i < cardConditions.Count)
            {
                switch (cardConditions[i])
                {
                    case Condition.SpecificCardOnField:
                    {
                        var cardName = cardExplanation[i];
                        var invocationCard = CheckSpecificCardOnField(cardName, currentPlayerCard);
                        if (invocationCard != null)
                        {
                            invocationCardsOnField.Add(invocationCard);
                        }
                    }
                        break;
                    case Condition.SacrificeSpecificCard:
                    {
                        var cardName = cardExplanation[i];
                        if (!specificCardFound)
                        {
                            specificCardFound = CheckSacrificeSpecificCard(cardName, currentPlayerCard);
                            if (i == (cardConditions.Count - 1))
                            {
                            }
                        }
                    }
                        break;
                    case Condition.SpecificEquipmentAttached:
                    {
                        var equipmentName = cardExplanation[i];
                        if (specificCardFound)
                        {
                            var equipmentCard = specificCardFound.GETEquipmentCard();
                            if (equipmentCard != null && equipmentCard.Nom == equipmentName)
                            {
                                currentPlayerCard.SendInvocationCardToYellowTrash(specificCardFound);
                            }
                        }
                    }
                        break;
                    case Condition.SpecificField:
                    {
                        var fieldName = cardExplanation[i];
                        if (specificCardFound)
                        {
                            var fieldCard = currentPlayerCard.field;
                            if (fieldCard != null && fieldCard.Nom == fieldName)
                            {
                                currentPlayerCard.SendInvocationCardToYellowTrash(specificCardFound);
                            }
                        }
                    }
                        break;
                    case Condition.SacrificeFamily:
                    {
                        var familyName = cardExplanation[i];
                        if (Enum.TryParse(familyName, out CardFamily cardFamily))
                        {
                            sacrificedCards = CheckFamily(cardFamily, currentPlayerCard);
                        }
                    }
                        break;
                    case Condition.SpecificFamilyOnField:
                    {
                        var familyName = cardExplanation[i];
                        if (Enum.TryParse(familyName, out CardFamily cardFamily))
                        {
                            invocationCardsOnField = CheckFamily(cardFamily, currentPlayerCard);
                        }
                    }
                        break;
                    case Condition.NumberCard:
                    {
                        var numberCard = int.Parse(cardExplanation[i]);
                        if (invocationCardsOnField.Count > 0)
                        {
                        }
                        else if (sacrificedCards.Count > 0)
                        {
                            if (sacrificedCards.Count == 1 && numberCard == 1)
                            {
                                var invocationCardToKill = sacrificedCards[0];
                                currentPlayerCard.SendInvocationCardToYellowTrash(invocationCardToKill);
                            }
                            else
                            {
                                if (numberCard == 1)
                                {
                                    var listCard = sacrificedCards.Cast<Card>().ToList();
                                    var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                        "Choix de la carte à sacrifier", listCard);
                                    messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                                    {
                                        var cardSelected = messageBox.GetComponent<MessageBox>().GetSelectedCard();
                                        if (cardSelected.IsValid())
                                        {
                                            var invocationCardToKill = (InvocationCard)cardSelected;
                                            currentPlayerCard.SendInvocationCardToYellowTrash(invocationCardToKill);
                                            Destroy(messageBox);
                                        }
                                        else
                                        {
                                            messageBox.SetActive(false);
                                            UnityAction okAction = () => { messageBox.SetActive(true); };
                                            MessageBox.CreateOkMessageBox(canvas, "Information",
                                                "Tu dois sélectionner une carte", okAction);
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
                                            "Tu dois sélectionner une carte", OkAction);
                                    };
                                }
                                else if (sacrificedCards.Count > numberCard)
                                {
                                    var listCard = sacrificedCards.Cast<Card>().ToList();
                                    var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                                        "Choix des " + numberCard + " cartes à sacrifier", listCard,
                                        multipleCardSelection: true, numberCardInSelection: 2);
                                    messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                                    {
                                        var cardSelected = messageBox.GetComponent<MessageBox>()
                                            .GetMultipleSelectedCards();
                                        if (cardSelected.Count == numberCard)
                                        {
                                            foreach (var card in cardSelected)
                                            {
                                                var sacrificedCard = (InvocationCard)card;
                                                currentPlayerCard.SendInvocationCardToYellowTrash(sacrificedCard);
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
                                                "Tu dois sélectionner " + numberCard + " cartes", OkAction);
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
                                            "Tu dois sélectionner " + numberCard + " cartes", OkAction);
                                    };
                                }
                                else
                                {
                                    // sacrifiedCards.Cound == numberCard
                                    foreach (var sacrificedCard in sacrificedCards)
                                    {
                                        currentPlayerCard.SendInvocationCardToYellowTrash(sacrificedCard);
                                    }
                                }
                            }
                        }
                    }
                        break;
                    case Condition.SacrificeThresholdAtk:
                    {
                        var threshold = float.Parse(cardExplanation[i]);
                        if (sacrificedCards.Count > 0)
                        {
                            var validCards = CheckThreshold(true, threshold, currentPlayerCard);
                            foreach (var validCard in validCards.Where(validCard => !sacrificedCards.Contains(validCard)))
                            {
                                sacrificedCards.Add(validCard);
                            }
                        }
                        else
                        {
                            sacrificedCards = CheckThreshold(true, threshold, currentPlayerCard);
                        }
                    }
                        break;
                    case Condition.SacrificeThresholdDef:
                    {
                        var threshold = int.Parse(cardExplanation[i]);
                        if (sacrificedCards.Count > 0)
                        {
                            var validCards = CheckThreshold(false, threshold, currentPlayerCard);
                            foreach (var validCard in validCards.Where(validCard => !sacrificedCards.Contains(validCard)))
                            {
                                sacrificedCards.Add(validCard);
                            }
                        }
                        else
                        {
                            sacrificedCards = CheckThreshold(false, threshold, currentPlayerCard);
                        }
                    }
                        break;
                    case Condition.NumberInvocationCardInYellowTrash:
                    {
                    }
                        break;
                    case Condition.ComeFromYellowTrash:
                    {
                        if (specificCardFound != null)
                        {
                            currentPlayerCard.SendInvocationCardToYellowTrash(specificCardFound);
                        }
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                i++;
            }
        }

        public static bool IsInvocationPossible(InvocationConditions conditions, string playerName)
        {
            var player = GameObject.Find(playerName);
            var currentPlayerCard = player.GetComponent<PlayerCards>();
            if (conditions == null)
            {
                return true;
            }
            else
            {
                var cardConditions = conditions.Keys;
                var cardExplanation = conditions.Values;

                InvocationCard specificCardFound = null;

                var sacrificedCards = new List<InvocationCard>();

                var invocationCardsOnField = new List<InvocationCard>();

                var isInvocationPossible = true;

                var i = 0;
                while (i < cardConditions.Count && isInvocationPossible)
                {
                    switch (cardConditions[i])
                    {
                        case Condition.SpecificCardOnField:
                        {
                            var cardName = cardExplanation[i];
                            var invocationCard = CheckSpecificCardOnField(cardName, currentPlayerCard);
                            if (invocationCard != null)
                            {
                                invocationCardsOnField.Add(invocationCard);
                            }
                        }
                            break;
                        case Condition.SacrificeSpecificCard:
                        {
                            var cardName = cardExplanation[i];
                            if (!specificCardFound)
                            {
                                specificCardFound = CheckSacrificeSpecificCard(cardName, currentPlayerCard);
                                isInvocationPossible = specificCardFound != null;
                            }
                        }
                            break;
                        case Condition.SpecificEquipmentAttached:
                        {
                            var equipmentName = cardExplanation[i];
                            isInvocationPossible = CheckSpecificEquipmentAttached(specificCardFound, equipmentName);
                        }
                            break;
                        case Condition.SpecificField:
                        {
                            var fieldName = cardExplanation[i];
                            isInvocationPossible = CheckSpecificField(fieldName, currentPlayerCard);
                        }
                            break;
                        case Condition.SacrificeFamily:
                        {
                            var familyName = cardExplanation[i];
                            if (Enum.TryParse(familyName, out CardFamily cardFamily))
                            {
                                sacrificedCards = CheckFamily(cardFamily, currentPlayerCard);
                                isInvocationPossible = sacrificedCards.Count > 0;
                            }
                        }
                            break;
                        case Condition.SpecificFamilyOnField:
                        {
                            var familyName = cardExplanation[i];
                            if (Enum.TryParse(familyName, out CardFamily cardFamily))
                            {
                                sacrificedCards = CheckFamily(cardFamily, currentPlayerCard);
                                isInvocationPossible = sacrificedCards.Count > 0;
                            }
                        }
                            break;
                        case Condition.NumberCard:
                        {
                            var numberCard = int.Parse(cardExplanation[i]);
                            if (invocationCardsOnField.Count > 0)
                            {
                                isInvocationPossible = invocationCardsOnField.Count >= numberCard;
                            }
                            else if (sacrificedCards.Count > 0)
                            {
                                isInvocationPossible = sacrificedCards.Count >= numberCard;
                            }
                            else
                            {
                                isInvocationPossible = false;
                            }
                        }
                            break;
                        case Condition.SacrificeThresholdAtk:
                        {
                            var threshold = float.Parse(cardExplanation[i]);
                            if (sacrificedCards.Count > 0)
                            {
                                var validCards = CheckThreshold(true, threshold, currentPlayerCard);
                                foreach (var validCard in validCards)
                                {
                                    if (!sacrificedCards.Contains(validCard))
                                    {
                                        sacrificedCards.Add(validCard);
                                    }
                                }
                            }
                            else
                            {
                                sacrificedCards = CheckThreshold(true, threshold, currentPlayerCard);
                            }
                        }
                            break;
                        case Condition.SacrificeThresholdDef:
                        {
                            var threshold = int.Parse(cardExplanation[i]);
                            if (sacrificedCards.Count > 0)
                            {
                                var validCards = CheckThreshold(false, threshold, currentPlayerCard);
                                foreach (var validCard in validCards)
                                {
                                    if (!sacrificedCards.Contains(validCard))
                                    {
                                        sacrificedCards.Add(validCard);
                                    }
                                }
                            }
                            else
                            {
                                sacrificedCards = CheckThreshold(false, threshold, currentPlayerCard);
                            }
                        }
                            break;
                        case Condition.NumberInvocationCardInYellowTrash:
                        {
                            var numberCardToHave = int.Parse(cardExplanation[i]);
                            var realNumber = CheckNumberInvocationCardInYellowTrash(currentPlayerCard);

                            isInvocationPossible = (realNumber >= numberCardToHave);
                        }
                            break;
                        case Condition.ComeFromYellowTrash:
                        {
                            if (specificCardFound != null)
                            {
                                isInvocationPossible = (specificCardFound.GetNumberDeaths() > 0);
                            }
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    i++;
                }

                return isInvocationPossible;
            }
        }

        private GameObject CreateMessageBoxSelectorCard(string title, List<Card> cards,
            UnityAction positiveAction = null,
            UnityAction negativeAction = null, bool isInfo = false)
        {
            inHandButton.SetActive(false);
            return MessageBox.CreateMessageBoxWithCardSelector(canvas, title, cards, positiveAction, negativeAction,
                isInfo);
        }

        private GameObject CreateMessageBoxSimple(string title, string description, UnityAction positiveAction = null,
            UnityAction negativeAction = null, UnityAction okAction = null, bool isInfo = false)
        {
            inHandButton.SetActive(false);
            return isInfo
                ? MessageBox.CreateOkMessageBox(canvas, title, description, okAction)
                : MessageBox.CreateSimpleMessageBox(canvas, title, description, positiveAction, negativeAction);
        }

        private void CreateMessageBoxNotChoosenCard()
        {
            MessageBox.CreateOkMessageBox(canvas, "Information",
                "Aucune carte n'a été choisie");
        }
    }
}