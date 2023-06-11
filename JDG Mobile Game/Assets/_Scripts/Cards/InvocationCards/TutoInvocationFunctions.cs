using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using OnePlayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cards.InvocationCards
{
    public class TutoInvocationFunctions : MonoBehaviour
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
            TutoPlayerGameLoop.ChangePlayer.AddListener(ChangePlayer);
            TutoInGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
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

        /// <summary>
        /// DealWithStartEffect.
        /// Apply startEffect of an invocation card.
        /// <param name="currentInvocationCard">the current invocation card</param>
        /// <param name="invocationStartEffect">start effect of this invocation card</param>
        /// </summary>
        private void DealWithStartEffect(InGameInvocationCard currentInvocationCard,
            InvocationStartEffect invocationStartEffect)
        {
            var keys = invocationStartEffect.Keys;
            var values = invocationStartEffect.Values;

            var cardName = "";
            var typeCard = "";
            var familyName = "";
            var invokeCardNames = new List<string>();
            var cardFound = new List<InGameCard>();
            var mustDivideAttack = false;
            var mustDivideDefense = false;
            for (var i = 0; i < keys.Count; i++)
            {
                var value = values[i];
                switch (keys[i])
                {
                    case StartEffect.GetSpecificCard:
                    {
                        cardName = value;
                    }
                        break;
                    case StartEffect.GetSpecificTypeCard:
                    {
                        typeCard = value;
                    }
                        break;
                    case StartEffect.GetCardFamily:
                    {
                        familyName = value;
                    }
                        break;
                    case StartEffect.GetCardSource:
                    {
                        StartEffectGetCardSource(value, cardName, ref cardFound, invokeCardNames, typeCard, familyName);
                    }
                        break;
                    case StartEffect.RemoveAllInvocationCards:
                    {
                        StartEffectRemoveAllInvocationCards(value);
                    }
                        break;
                    case StartEffect.InvokeSpecificCard:
                    {
                        invokeCardNames.Add(value);
                    }
                        break;
                    case StartEffect.PutField:
                    {
                        StartEffectPutField(cardFound);
                    }
                        break;
                    case StartEffect.DestroyField:
                    {
                        StartEffectDestroyField(currentInvocationCard, ref cardFound, mustDivideAttack,
                            mustDivideDefense);
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
                        StartEffectSendToDeath(currentInvocationCard, ref cardFound);
                    }
                        break;
                    case StartEffect.DrawXCards:
                    {
                        StartEffectDrawXCards(value);
                    }
                        break;
                    case StartEffect.Condition:
                    {
                        StartEffectCondition(currentInvocationCard, value);
                    }
                        break;
                    case StartEffect.SacrificeFieldIncrement:
                    {
                        StartEffectSacrificeFieldIncrement(currentInvocationCard, value);
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// StartEffectGetCardSource.
        /// Apply GetCardSource startEffect of an invocation card.
        /// <param name="value">string that represent the source</param>
        /// <param name="cardName">name of the card to get</param>
        /// <param name="cardFound">list of cards the user can choose</param>
        /// <param name="invokeCardNames">list of invocation card name to get from source</param>
        /// <param name="typeCard">string that represents the type of card to get in source</param>
        /// <param name="familyName">string that represent the family name of cards to get in source</param>
        /// </summary>
        private void StartEffectGetCardSource(string value, string cardName, ref List<InGameCard> cardFound,
            IReadOnlyCollection<string> invokeCardNames,
            string typeCard, string familyName)
        {
            switch (value)
            {
                case "deck":
                {
                    var deck = new List<InGameCard>(currentPlayerCard.deck);
                    if (cardName != "")
                    {
                        var isFound = false;
                        var j = 0;
                        while (j < deck.Count && !isFound)
                        {
                            if (deck[j].Title == cardName)
                            {
                                isFound = true;
                                cardFound.Add(deck[j]);
                            }

                            j++;
                        }

                        if (isFound)
                        {
                            var cards = new List<InGameCard>(cardFound);

                            void PositiveAction()
                            {
                                currentPlayerCard.handCards.Add(cards[0]);
                                currentPlayerCard.deck.Remove(cards[0]);
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
                        deck.AddRange(currentPlayerCard.handCards);
                        cardFound.AddRange(from t in deck
                            from invokeCardName in invokeCardNames
                            where t.Title == invokeCardName
                            select t);

                        var size = currentPlayerCard.invocationCards.Count;
                        if (cardFound.Count > 0 && size < 4)
                        {
                            if (cardFound.Count == 1)
                            {
                                var cards = new List<InGameCard>(cardFound);

                                void PositiveAction()
                                {
                                    var invocationCard = cards[cards.Count - 1] as InGameInvocationCard;
                                    currentPlayerCard.invocationCards.Add(invocationCard);
                                    currentPlayerCard.handCards.Remove(invocationCard);
                                    currentPlayerCard.deck.Remove(invocationCard);
                                    inHandButton.SetActive(true);
                                }

                                var messageBox = CreateMessageBoxSimple("Invocation",
                                    "Voulez-vous aussi invoquer " + cardFound[0].Title + " ?",
                                    positiveAction: PositiveAction);

                                messageBox.GetComponent<MessageBox>().NegativeAction = () =>
                                {
                                    messageBox.SetActive(false);

                                    var infoMessageBox = MessageBox.CreateOkMessageBox(canvas, "Information",
                                        "Tu n'as pas vraiment le choix");

                                    infoMessageBox.GetComponent<MessageBox>().OkAction = () =>
                                    {
                                        messageBox.SetActive(true);
                                        Destroy(infoMessageBox);
                                    };
                                };
                            }
                            else
                            {
                                var message =
                                    CreateMessageBoxSelectorCard("Choix de l'invocation", cardFound);
                                message.GetComponent<MessageBox>().PositiveAction = () =>
                                {
                                    var invocationCard =
                                        message.GetComponent<MessageBox>()
                                            .GetSelectedCard() as InGameInvocationCard;
                                    currentPlayerCard.invocationCards.Add(invocationCard);
                                    currentPlayerCard.handCards.Remove(invocationCard);
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
                            var invocationCard = card as InGameInvocationCard;

                            var listFamily = invocationCard.Families;
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
                            if (trash[j].Title == cardName)
                            {
                                isFound = true;
                                cardFound.Add(trash[j]);
                            }

                            j++;
                        }

                        if (isFound)
                        {
                            var cards = new List<InGameCard>(cardFound);

                            void PositiveAction()
                            {
                                currentPlayerCard.handCards.Add(cards[0]);
                                currentPlayerCard.yellowTrash.Remove(cards[0]);
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
                            where t.Title == invokeCardName
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
                            var invocationCard = card as InGameInvocationCard;

                            var listFamily = invocationCard.Families;
                            cardFound.AddRange(
                                (from t in listFamily where t == cardFamily select invocationCard));
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// StartEffectRemoveAllInvocationCards.
        /// Apply RemoveAllInvocationCards startEffect of an invocation card.
        /// Remove all invocation cards on field except one
        /// <param name="value">string that represent the name of the card to except</param>
        /// </summary>
        private void StartEffectRemoveAllInvocationCards(string value)
        {
            var p1InvocationCards = p1.GetComponent<PlayerCards>().invocationCards;
            var p2InvocationCards = p2.GetComponent<PlayerCards>().invocationCards;


            for (var j = p1InvocationCards.Count - 1; j >= 0; j--)
            {
                if (p1InvocationCards[j].Title == value) continue;
                p1.GetComponent<PlayerCards>().handCards.Add(p1InvocationCards[j]);
                p1.GetComponent<PlayerCards>().invocationCards
                    .Remove(p1InvocationCards[j]);
            }

            for (var j = p2InvocationCards.Count - 1; j >= 0; j--)
            {
                if (p2InvocationCards[j].Title == value) continue;
                p2.GetComponent<PlayerCards>().handCards.Add(p2InvocationCards[j]);
                p2.GetComponent<PlayerCards>().invocationCards
                    .Remove(p2InvocationCards[j]);
            }
        }

        /// <summary>
        /// StartEffectPutField.
        /// Apply PutField startEffect of an invocation card.
        /// Put or get a field card from the deck depending if user already has a field card on field.
        /// <param name="cardFound">list of field card available to the user</param>
        /// </summary>
        private void StartEffectPutField(List<InGameCard> cardFound)
        {
            if (cardFound.Count <= 0) return;
            if (currentPlayerCard.field == null)
            {
                var message = CreateMessageBoxSelectorCard("Choix du terrain à poser", cardFound);

                message.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    var fieldCard =
                        message.GetComponent<MessageBox>().GetSelectedCard() as InGameFieldCard;

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
                        message.GetComponent<MessageBox>().GetSelectedCard() as InGameFieldCard;

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

        /// <summary>
        /// StartEffectDestroyField.
        /// Apply DestroyField startEffect of an invocation card.
        /// Remove a field card from the field be it user or opponent.
        /// <param name="currentInvocationCard">invocation card that has this property</param>
        /// <param name="cardFound">list of field cards available to destroy</param>
        /// <param name="mustDivideAttack">boolean to know if we have to divide attack by 2 if user agree to destroy a field card</param>
        /// <param name="mustDivideDefense">boolean to know if we have to divide defense by 2 if user agree to destroy a field card</param>
        /// </summary>
        private void StartEffectDestroyField(InGameInvocationCard currentInvocationCard, ref List<InGameCard> cardFound,
            bool mustDivideAttack,
            bool mustDivideDefense)
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

            if (cardFound.Count <= 0) return;
            var message = CreateMessageBoxSelectorCard("Choix du terrain à détruire", cardFound);
            var attack = mustDivideAttack;
            var defense = mustDivideDefense;
            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var fieldCard =
                    message.GetComponent<MessageBox>().GetSelectedCard() as InGameFieldCard;

                if (fieldCard != null)
                {
                    if (attack)
                    {
                        currentInvocationCard.Attack /= 2;
                    }

                    if (defense)
                    {
                        currentInvocationCard.Defense /= 2;
                    }

                    if (fieldCardP1 != null && fieldCard.Title == fieldCardP1.Title)
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

        // TODO: Check if that's the expected behaviour
        /// <summary>
        /// StartEffectSendToDeath.
        /// Apply SendToDeath startEffect of an invocation card.
        /// Send a card from the opponent to the yellow trash.
        /// Increment the number of deck of the invocationCard with this property
        /// <param name="currentInvocationCard">invocation card that has this property</param>
        /// <param name="cardFound">list of invocation cards available to destroy</param>
        /// </summary>
        private void StartEffectSendToDeath(InGameInvocationCard currentInvocationCard, ref List<InGameCard> cardFound)
        {
            if (GameLoop.IsP1Turn)
            {
                var p2InvocationCards = opponentPlayerCards.invocationCards;

                cardFound.AddRange(p2InvocationCards);
            }
            else
            {
                var p1InvocationCards = opponentPlayerCards.invocationCards;

                cardFound.AddRange(p1InvocationCards);
            }


            var message = CreateMessageBoxSelectorCard("Choix de la carte à tuer :", cardFound);
            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var invocationCardSelected =
                    message.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                if (invocationCardSelected != null)
                {
                    var invocationCards = opponentPlayerCards.invocationCards;
                    var k = 0;
                    var found = false;
                    while (!found && k < invocationCards.Count)
                    {
                        if (invocationCards[k] != null &&
                            invocationCards[k].Title == invocationCardSelected.Title)
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


        /// <summary>
        /// StartEffectDrawXCards.
        /// Apply DrawXCards startEffect of an invocation card.
        /// Ask user if he wants to draw value cards
        /// <param name="value">value is a string that is an number representing the max number of cards to draw</param>
        /// </summary>
        private void StartEffectDrawXCards(string value)
        {
            var x = int.Parse(value);
            var size = currentPlayerCard.deck.Count;
            if (size <= 0) return;
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

        /// <summary>
        /// StartEffectCondition.
        /// Apply Condition startEffect of an invocation card.
        /// Apply special affect to invocation card.
        /// Currently there are 2 affects :
        /// - skipAttack : Prevent from attacking this turn to add an equipment card to the hand
        /// - cantAttack : Block user from attacking 
        /// <param name="currentInvocationCard">invocation card with this property</param>
        /// <param name="value">value is a string that reprensent the condition</param>
        /// </summary>
        private void StartEffectCondition(InGameInvocationCard currentInvocationCard, string value)
        {
            switch (value)
            {
                case "skipAttack":
                {
                    var m = CreateMessageBoxSimple("Action possible",
                        "Voulez-vous sauter la phase d'attaque de la carte pour ajouter une carte equipement de votre pioche à votre main ?");
                    m.GetComponent<MessageBox>().PositiveAction = () =>
                    {
                        m.GetComponent<MessageBox>().description = "";
                        var deck = currentPlayerCard.deck;
                        var equipmentCards = deck.Where(card => card.Type == CardType.Equipment).ToList();
                        var messageBox1 =
                            CreateMessageBoxSelectorCard("Choix de la carte équipement", equipmentCards);
                        messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                        {
                            var selectedCard = messageBox1.GetComponent<MessageBox>().GetSelectedCard();
                            if (selectedCard != null)
                            {
                                currentPlayerCard.handCards.Add(selectedCard);
                                currentPlayerCard.deck.Remove(selectedCard);
                                var index = currentPlayerCard.GetIndexInvocationCard(currentInvocationCard
                                    .Title);
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

        /// <summary>
        /// StartEffectSacrificeFieldIncrement.
        /// Apply SacrificeFieldIncrement startEffect of an invocation card.
        /// Earn Def and atk by sacrificing invocation cards
        /// <param name="currentInvocationCard">invocation card with this property</param>
        /// <param name="value">value is a string that represent the invocation card name and field card name</param>
        /// </summary>
        private void StartEffectSacrificeFieldIncrement(InGameInvocationCard currentInvocationCard, string value)
        {
            var elements = value.Split(';');
            var sacrifice = elements[0];
            var field = elements[1];
            var incrementStat = float.Parse(elements[2]);

            var currentField = currentPlayerCard.field;
            if (currentField == null || field != currentField.Title) return;
            var invocationCardOnField = currentPlayerCard.invocationCards;
            var k = 0;
            var found = false;
            while (!found && k < invocationCardOnField.Count)
            {
                if (invocationCardOnField[k] != null &&
                    invocationCardOnField[k].Title == sacrifice)
                {
                    found = true;
                }
                else
                {
                    k++;
                }
            }

            if (!found) return;
            var cardToSacrifice = invocationCardOnField[k];

            void PositiveAction()
            {
                currentPlayerCard.invocationCards.Remove(cardToSacrifice);
                currentPlayerCard.yellowTrash.Add(cardToSacrifice);

                // Example of Mahammad that can earn 2.5 ATK and DEF by sacrificing JDG
                currentInvocationCard.Attack += incrementStat;
                currentInvocationCard.Defense += incrementStat;
            }

            MessageBox.CreateSimpleMessageBox(canvas, "Choix",
                "Voulez-vous sacrifier " + sacrifice + " pour gagner " + incrementStat +
                " en ATK et DEF ?", PositiveAction);
        }

        /// <summary>
        /// PutInvocationCard.
        /// Put an invocation card on field.
        /// Apply StartEffect and ConditionEffect of this card if there is enough place
        /// <param name="invocationCard">invocation card</param>
        /// </summary>
        private void PutInvocationCard(InGameInvocationCard invocationCard)
        {
            var size = currentPlayerCard.invocationCards.Count;

            if (size >= 4) return;

            //  if (invocationConditionEffect != null)
            //{
            //  DealWithConditionInvocation(invocationConditionEffect);
            currentPlayerCard.invocationCards.Add(invocationCard);
            currentPlayerCard.handCards.Remove(invocationCard);
            //}
            // else
            //{
            //    currentPlayerCard.invocationCards.Add(invocationCard);
            //    currentPlayerCard.handCards.Remove(invocationCard);
            //}
        }

        /// <summary>
        /// IsSpecialActionPossible.
        /// Check if a special action is possible for a specific invocation Card.
        /// Activate or desactivate a button when user clicks on a card
        /// <param name="currentInvocationCard">invocation card</param>
        /// <param name="invocationActionEffect">invocation actionEffect</param>
        /// </summary>
        public bool IsSpecialActionPossible(InGameInvocationCard currentInvocationCard,
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
                var value = values[i];
                switch (keys[i])
                {
                    case ActionEffect.SacrificeInvocation:
                        break;
                    case ActionEffect.BackToLife:
                    {
                        isPossible = ActionEffectBackToLifePossible(isPossible);
                    }
                        break;
                    case ActionEffect.GiveAtk:
                        atk = float.Parse(value);
                        break;
                    case ActionEffect.GiveDef:
                        def = float.Parse(value);
                        break;
                    case ActionEffect.SpecificFamily:
                    {
                        isPossible =
                            ActionEffectSpecificFamilyPossible(currentInvocationCard, value, def, atk, isPossible);
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

        /// <summary>
        /// ActionEffectBackToLifePossible.
        /// Check BackToLife ActionEffect can be used.
        /// <param name="isPossible">previous value of isPossible</param>
        /// </summary>
        private bool ActionEffectBackToLifePossible(bool isPossible)
        {
            var trash = currentPlayerCard.yellowTrash;

            var invocationCardDead =
                trash.Where(card => card is InGameInvocationCard && !card.Collector).ToList();

            if (invocationCardDead.Count > 0)
            {
                isPossible = true;
            }

            return isPossible;
        }


        private bool ActionEffectSpecificFamilyPossible(InGameInvocationCard currentInvocationCard, string value,
            float def,
            float atk, bool isPossible)
        {
            var family = (CardFamily)Enum.Parse(typeof(CardFamily), value);

            var invocationsOnField = currentPlayerCard.invocationCards;

            var invocationCardSameFamily = new List<InGameCard>();
            foreach (var card in invocationsOnField.Where(card => card != null))
            {
                if (card.Title == currentInvocationCard.Title)
                {
                }
                else
                {
                    var currentFamilies = card.Families;
                    if (currentFamilies.Any(currentFamily => currentFamily == family))
                    {
                        invocationCardSameFamily.Add(card);
                    }
                }
            }

            if (invocationCardSameFamily.Count <= 0) return isPossible;
            if (!(def > 0) || !(atk > 0)) return isPossible;

            // TODO : Check if Ok
            isPossible = currentInvocationCard.Defense > 0 || currentInvocationCard.Attack > 0;
            /*if (currentInvocationCard.GetBonusDefense() > -def)
            {
                isPossible = true;
            }

            if (currentInvocationCard.GetBonusAttack() > -atk)
            {
                isPossible = true;
            }*/

            return isPossible;
        }

        public void AskIfUserWantToUseActionEffect(InGameInvocationCard currentInvocationCard,
            InvocationActionEffect invocationActionEffect)
        {
            var keys = invocationActionEffect.Keys;
            var values = invocationActionEffect.Values;
            string cardName = null;
            var atk = 0.0f;
            var def = 0.0f;
            for (var i = 0; i < keys.Count; i++)
            {
                var value = values[i];
                switch (keys[i])
                {
                    case ActionEffect.SacrificeInvocation:
                        cardName = value;
                        break;
                    case ActionEffect.BackToLife:
                    {
                        ActionEffectBackToLife(cardName);
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
                        ActionEffectSpecificFamily(currentInvocationCard, invocationActionEffect, values, i, atk, def,
                            keys);
                    }
                        break;
                    case ActionEffect.SkipOpponentAttack:
                    {
                        ActionEffectSkipOpponentAttack();
                    }
                        break;
                    case ActionEffect.Beneficiary:
                    default:
                        break;
                }
            }
        }

        private void ActionEffectSkipOpponentAttack()
        {
            var invocationCards = opponentPlayerCards.invocationCards;

            if (invocationCards.Count <= 0) return;
            var validList = invocationCards.Cast<InGameCard>().ToList();
            var message =
                MessageBox.CreateMessageBoxWithCardSelector(canvas,
                    "Choix de la carte qui ne pourra pas attaquer le prochain tour", validList);
            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var invocationCardSelected =
                    message.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
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

        private void ActionEffectSpecificFamily(InGameInvocationCard currentInvocationCard,
            InvocationActionEffect invocationActionEffect, List<string> values, int i, float atk, float def,
            List<ActionEffect> keys)
        {
            var family = (CardFamily)Enum.Parse(typeof(CardFamily), values[i]);

            var invocationsOnField = currentPlayerCard.invocationCards;
            var indexCurrent = 0;

            var invocationCardSameFamily = new List<InGameCard>();
            for (var j = 0; j < invocationsOnField.Count; j++)
            {
                var card = invocationsOnField[j];
                if (card == null) continue;
                if (card.Title == currentInvocationCard.Title)
                {
                    indexCurrent = j;
                }
                else
                {
                    var currentFamilies = card.Families;
                    if (currentFamilies.Any(t => t == family))
                    {
                        invocationCardSameFamily.Add(card);
                    }
                }
            }

            if (invocationCardSameFamily.Count <= 0) return;
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
                            message.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                        if (invocationCardSelected != null)
                        {
                            for (var j = 0; j < invocationsOnField.Count; j++)
                            {
                                var card = invocationsOnField[j];
                                if (card == null || card.Title != invocationCardSelected.Title) continue;
                                currentPlayerCard.invocationCards[j].Attack += atk;
                                currentPlayerCard.invocationCards[j].Defense += def;
                                currentPlayerCard.invocationCards[indexCurrent].Attack -= atk;
                                currentPlayerCard.invocationCards[indexCurrent].Defense -= def;

                                invocationMenu.transform.GetChild(1).GetComponent<Button>()
                                        .interactable =
                                    false;

                                keys.Add(ActionEffect.Beneficiary);
                                values.Add(card.Title);
                                invocationActionEffect.Keys = keys;
                                invocationActionEffect.Values = values;
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

        private void ActionEffectBackToLife(string cardName)
        {
            var trash = currentPlayerCard.yellowTrash;

            var invocationCardDead =
                trash.Where(card => card is InGameInvocationCard && !card.Collector).ToList();

            if (invocationCardDead.Count <= 0) return;
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
                        if (invocationCards[k] != null && invocationCards[k].Title == name1)
                        {
                            found = true;
                        }
                        else
                        {
                            k++;
                        }
                    }


                    var invocationCardSelected =
                        message.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
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

        private static InGameInvocationCard CheckSpecificCardOnField(string cardName, PlayerCards currentPlayerCards)
        {
            var found = false;
            var invocationCards = currentPlayerCards.invocationCards;
            var size = invocationCards.Count;
            InGameInvocationCard invocationCard = null;

            var j = 0;
            while (j < size && !found)
            {
                if (invocationCards[j] != null && invocationCards[j].Title == cardName)
                {
                    found = true;
                    invocationCard = invocationCards[j];
                }

                j++;
            }

            return invocationCard;
        }

        private static InGameInvocationCard CheckSacrificeSpecificCard(string cardName, PlayerCards currentPlayerCards)
        {
            InGameInvocationCard foundCard = null;
            var found = false;
            var invocationCards = currentPlayerCards.invocationCards;
            var size = invocationCards.Count;
            var j = 0;

            while (j < size && !found)
            {
                if (invocationCards[j] != null && invocationCards[j].Title == cardName)
                {
                    foundCard = invocationCards[j];
                    found = true;
                }


                j++;
            }

            return foundCard;
        }

        private static bool CheckSpecificEquipmentAttached(InGameInvocationCard invocationCard, string equipmentName)
        {
            var isChecked = false;
            if (invocationCard == null) return false;
            if (invocationCard.EquipmentCard != null)
            {
                isChecked = invocationCard.EquipmentCard.Title == equipmentName;
            }

            return isChecked;
        }

        private static bool CheckSpecificField(string fieldName, PlayerCards currentPlayerCards)
        {
            var isCorrectField = false;
            var fieldCard = currentPlayerCards.field;
            if (fieldCard != null)
            {
                isCorrectField = fieldCard.Title == fieldName;
            }

            return isCorrectField;
        }

        private static List<InGameInvocationCard> CheckFamily(CardFamily cardFamily, PlayerCards currentPlayerCards)
        {
            var invocationCards = currentPlayerCards.invocationCards;

            return (from t in invocationCards
                where t != null
                let families = t.Families
                from family in families
                where family == cardFamily
                select t).ToList();
        }

        private static List<InGameInvocationCard> CheckThreshold(bool isAttack, float value,
            PlayerCards currentPlayerCards)
        {
            var threshold = new List<InGameInvocationCard>();
            var invocationCards = currentPlayerCards.invocationCards;
            threshold.AddRange(isAttack
                ? invocationCards.Where(invocationCard =>
                    invocationCard != null && invocationCard.Title != null &&
                    invocationCard.GetCurrentAttack() >= value)
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

            InGameInvocationCard specificCardFound = null;

            var sacrificedCards = new List<InGameInvocationCard>();

            var invocationCardsOnField = new List<InGameInvocationCard>();

            var i = 0;
            while (i < cardConditions.Count)
            {
                var value = cardExplanation[i];
                switch (cardConditions[i])
                {
                    case Condition.SpecificCardOnField:
                    {
                        ConditionSpecificCardOnField(value, currentPlayerCard, ref invocationCardsOnField);
                    }
                        break;
                    case Condition.SacrificeSpecificCard:
                    {
                        ConditionSacrificeSpecificCard(value, ref specificCardFound);
                    }
                        break;
                    case Condition.SpecificEquipmentAttached:
                    {
                        ConditionSpecificEquipmentAttached(value, specificCardFound);
                    }
                        break;
                    case Condition.SpecificField:
                    {
                        ConditionSpecificField(value, specificCardFound);
                    }
                        break;
                    case Condition.SacrificeFamily:
                    {
                        sacrificedCards = ConditionSacrificeFamily(value, sacrificedCards);
                    }
                        break;
                    case Condition.SpecificFamilyOnField:
                    {
                        ConditionSpecificFamilyOnField(value, ref invocationCardsOnField);
                    }
                        break;
                    case Condition.NumberCard:
                    {
                        ConditionNumberCard(value, invocationCardsOnField, sacrificedCards);
                    }
                        break;
                    case Condition.SacrificeThresholdAtk:
                    {
                        sacrificedCards = ConditionSacrificeThresholdAtk(cardExplanation, i, sacrificedCards);
                    }
                        break;
                    case Condition.SacrificeThresholdDef:
                    {
                        ConditionSacrificeThresholdDef(value, sacrificedCards);
                    }
                        break;
                    case Condition.NumberInvocationCardInYellowTrash:
                    {
                    }
                        break;
                    case Condition.ComeFromYellowTrash:
                    {
                        ConditionComeFromYellowTrash(specificCardFound);
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                i++;
            }
        }

        private void ConditionComeFromYellowTrash(InGameInvocationCard specificCardFound)
        {
            if (specificCardFound != null)
            {
                currentPlayerCard.SendInvocationCardToYellowTrash(specificCardFound);
            }
        }

        private void ConditionSacrificeThresholdDef(string value, List<InGameInvocationCard> sacrificedCards)
        {
            var threshold = int.Parse(value);
            if (sacrificedCards.Count > 0)
            {
                var validCards = CheckThreshold(false, threshold, currentPlayerCard);
                foreach (var validCard in validCards.Where(
                             validCard => !sacrificedCards.Contains(validCard)))
                {
                    sacrificedCards.Add(validCard);
                }
            }
            else
            {
                sacrificedCards = CheckThreshold(false, threshold, currentPlayerCard);
            }
        }

        private List<InGameInvocationCard> ConditionSacrificeThresholdAtk(List<string> cardExplanation, int i,
            List<InGameInvocationCard> sacrificedCards)
        {
            var threshold = float.Parse(cardExplanation[i]);
            if (sacrificedCards.Count > 0)
            {
                var validCards = CheckThreshold(true, threshold, currentPlayerCard);
                foreach (var validCard in validCards.Where(
                             validCard => !sacrificedCards.Contains(validCard)))
                {
                    sacrificedCards.Add(validCard);
                }
            }
            else
            {
                sacrificedCards = CheckThreshold(true, threshold, currentPlayerCard);
            }

            return sacrificedCards;
        }

        private void ConditionNumberCard(string value, List<InGameInvocationCard> invocationCardsOnField,
            List<InGameInvocationCard> sacrificedCards)
        {
            var numberCard = int.Parse(value);
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
                        var listCard = sacrificedCards.Cast<InGameCard>().ToList();
                        var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                            "Choix de la carte à sacrifier", listCard);
                        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                        {
                            var cardSelected = messageBox.GetComponent<MessageBox>().GetSelectedCard();
                            if (cardSelected.IsValid())
                            {
                                var invocationCardToKill = cardSelected as InGameInvocationCard;
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
                        var listCard = sacrificedCards.Cast<InGameCard>().ToList();
                        var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                            "Choix des " + numberCard + " cartes à sacrifier", listCard,
                            multipleCardSelection: true, numberCardInSelection: 2);
                        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                        {
                            var cardSelected = messageBox.GetComponent<MessageBox>()
                                .GetMultipleSelectedCards();
                            if (cardSelected.Count == numberCard)
                            {
                                foreach (var sacrificedCard in cardSelected.Cast<InGameInvocationCard>())
                                {
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

        private void ConditionSpecificFamilyOnField(string familyName,
            ref List<InGameInvocationCard> invocationCardsOnField)
        {
            if (Enum.TryParse(familyName, out CardFamily cardFamily))
            {
                invocationCardsOnField = CheckFamily(cardFamily, currentPlayerCard);
            }
        }

        private List<InGameInvocationCard> ConditionSacrificeFamily(string familyName,
            List<InGameInvocationCard> sacrificedCards)
        {
            if (Enum.TryParse(familyName, out CardFamily cardFamily))
            {
                sacrificedCards = CheckFamily(cardFamily, currentPlayerCard);
            }

            return sacrificedCards;
        }

        private void ConditionSpecificField(string value, InGameInvocationCard specificCardFound)
        {
            if (specificCardFound == null) return;
            var fieldCard = currentPlayerCard.field;
            if (fieldCard != null && fieldCard.Title == value)
            {
                currentPlayerCard.SendInvocationCardToYellowTrash(specificCardFound);
            }
        }

        private void ConditionSpecificEquipmentAttached(string value, InGameInvocationCard specificCardFound)
        {
            if (specificCardFound == null) return;
            var equipmentCard = specificCardFound.EquipmentCard;
            if (equipmentCard != null && equipmentCard.Title == value)
            {
                currentPlayerCard.SendInvocationCardToYellowTrash(specificCardFound);
            }
        }

        private void ConditionSacrificeSpecificCard(string cardName, ref InGameInvocationCard specificCardFound)
        {
            if (specificCardFound != null) return;
            specificCardFound = CheckSacrificeSpecificCard(cardName, currentPlayerCard);
        }

        private static void ConditionSpecificCardOnField(string value, PlayerCards playerCards,
            ref List<InGameInvocationCard> invocationCardsOnField)
        {
            var invocationCard = CheckSpecificCardOnField(value, playerCards);
            if (invocationCard != null)
            {
                invocationCardsOnField.Add(invocationCard);
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

            var cardConditions = conditions.Keys;
            var cardExplanation = conditions.Values;

            InGameInvocationCard specificCardFound = null;

            var sacrificedCards = new List<InGameInvocationCard>();

            var invocationCardsOnField = new List<InGameInvocationCard>();

            var isInvocationPossible = true;

            var i = 0;
            while (i < cardConditions.Count && isInvocationPossible)
            {
                var value = cardExplanation[i];
                switch (cardConditions[i])
                {
                    case Condition.SpecificCardOnField:
                    {
                        ConditionSpecificCardOnField(value, currentPlayerCard, ref invocationCardsOnField);
                    }
                        break;
                    case Condition.SacrificeSpecificCard:
                    {
                        isInvocationPossible =
                            ConditionSacrificeSpecificCardIsPossible(value, ref specificCardFound, currentPlayerCard);
                    }
                        break;
                    case Condition.SpecificEquipmentAttached:
                    {
                        isInvocationPossible = CheckSpecificEquipmentAttached(specificCardFound, value);
                    }
                        break;
                    case Condition.SpecificField:
                    {
                        isInvocationPossible = CheckSpecificField(value, currentPlayerCard);
                    }
                        break;
                    case Condition.SacrificeFamily:
                    {
                        isInvocationPossible =
                            ConditionSacrificeFamilyIsPossible(value, currentPlayerCard, ref sacrificedCards);
                    }
                        break;
                    case Condition.SpecificFamilyOnField:
                    {
                        isInvocationPossible =
                            ConditionSpecificFamilyOnFieldIsPossible(value, currentPlayerCard, ref sacrificedCards);
                    }
                        break;
                    case Condition.NumberCard:
                    {
                        isInvocationPossible =
                            ConditionNumberCardIsPossible(value, invocationCardsOnField, sacrificedCards);
                    }
                        break;
                    case Condition.SacrificeThresholdAtk:
                    {
                        ConditionSacrificeThresholdAtkIsPossible(value, ref sacrificedCards, currentPlayerCard);
                    }
                        break;
                    case Condition.SacrificeThresholdDef:
                    {
                        ConditionSacrificeThresholdDefIsPossible(value, ref sacrificedCards, currentPlayerCard);
                    }
                        break;
                    case Condition.NumberInvocationCardInYellowTrash:
                    {
                        isInvocationPossible =
                            ConditionNumberInvocationCardInYellowTrashIsPossible(value, currentPlayerCard);
                    }
                        break;
                    case Condition.ComeFromYellowTrash:
                    {
                        isInvocationPossible = ConditionComeFromYellowTrashIsPossible(specificCardFound);
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                i++;
            }

            return isInvocationPossible;
        }

        private static bool ConditionComeFromYellowTrashIsPossible(InGameInvocationCard specificCardFound)
        {
            if (specificCardFound != null)
            {
                return specificCardFound.NumberOfDeaths > 0;
            }

            return false;
        }

        private static bool ConditionNumberInvocationCardInYellowTrashIsPossible(string value,
            PlayerCards currentPlayerCard)
        {
            var numberCardToHave = int.Parse(value);
            var realNumber = CheckNumberInvocationCardInYellowTrash(currentPlayerCard);

            return realNumber >= numberCardToHave;
        }

        private static void ConditionSacrificeThresholdDefIsPossible(string value,
            ref List<InGameInvocationCard> sacrificedCards,
            PlayerCards currentPlayerCard)
        {
            var threshold = int.Parse(value);
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

        private static void ConditionSacrificeThresholdAtkIsPossible(string value,
            ref List<InGameInvocationCard> sacrificedCards,
            PlayerCards currentPlayerCard)
        {
            var threshold = float.Parse(value);
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

        private static bool ConditionNumberCardIsPossible(string value, ICollection invocationCardsOnField,
            ICollection sacrificedCards)
        {
            bool isInvocationPossible;
            var numberCard = int.Parse(value);
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

            return isInvocationPossible;
        }

        private static bool ConditionSpecificFamilyOnFieldIsPossible(string value, PlayerCards currentPlayerCard,
            ref List<InGameInvocationCard> sacrificedCards)
        {
            if (!Enum.TryParse(value, out CardFamily cardFamily)) return false;
            sacrificedCards = CheckFamily(cardFamily, currentPlayerCard);
            return sacrificedCards.Count > 0;
        }

        private static bool ConditionSacrificeFamilyIsPossible(string value, PlayerCards currentPlayerCard,
            ref List<InGameInvocationCard> sacrificedCards)
        {
            if (!Enum.TryParse(value, out CardFamily cardFamily)) return false;
            sacrificedCards = CheckFamily(cardFamily, currentPlayerCard);
            return sacrificedCards.Count > 0;
        }

        private static bool ConditionSacrificeSpecificCardIsPossible(string cardName,
            ref InGameInvocationCard specificCardFound, PlayerCards currentPlayerCard)
        {
            if (specificCardFound != null) return true;
            specificCardFound = CheckSacrificeSpecificCard(cardName, currentPlayerCard);
            return specificCardFound != null;
        }

        private GameObject CreateMessageBoxSelectorCard(string title, List<InGameCard> cards,
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