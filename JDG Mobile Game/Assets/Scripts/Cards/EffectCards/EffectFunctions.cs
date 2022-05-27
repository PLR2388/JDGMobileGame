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

        /// <summary>
        /// Called when user clicks on put for an effect card
        /// Apply effects of the card or refuse to put it if 4 effects cards are on the field
        /// </summary>
        /// <param name="effectCard">The effect card the user put on field</param>
        private void PutEffectCard(EffectCard effectCard)
        {
            var size = currentPlayerCard.effectCards.Count;

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

        /// <summary>
        /// Returns a boolean to indicate if the effect card is usable for the current user
        /// </summary>
        /// <param name="effectCardEffect">The effect of the effect card</param>
        /// <returns>A boolean indicating if effect card is usable</returns>
        public bool CanUseEffectCard(EffectCardEffect effectCardEffect)
        {
            var keys = effectCardEffect.Keys;
            var values = effectCardEffect.Values;

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
                        if (!CanUseAffectOpponent(value, pvAffected, out affectOpponent))
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.DestroyCards:
                    {
                        if (!CanUseDestroyCards(value, affectOpponent))
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.SacrificeInvocation:
                    {
                        if (!CanUseSacrificeInvocation(value))
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.SameFamily:
                    {
                        if (!CanUseSameFamily())
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.RemoveDeck:
                    {
                        if (!CanUseRemoveDeck())
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.SpecialInvocation:
                    {
                        if (!CanUseSpecialInvocation())
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.Combine:
                    {
                        if (!CanUseCombine(value))
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.TakeControl:
                    {
                        if (!CanUseTakeControl())
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.NumberAttacks:
                    {
                        if (!CanUseNumberAttacks())
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.AttackDirectly:
                    {
                        if (!CanUseAttackDirectly(value))
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.ProtectAttack:
                    {
                        if (!CanUseProtectAttack())
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.SkipFieldsEffect:
                    {
                        if (!CanUseSkipFieldsEffect())
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.ChangeField:
                    {
                        changeField = true;
                    }
                        break;
                    case Effect.RemoveHand:
                    {
                        if (!CanUseRemoveHand(value))
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.AffectPv:
                    {
                        CanUseAffectPv(value, out pvAffected);
                    }
                        break;
                    case Effect.Sources:
                    {
                        if (!CanUseSources(changeField, value, handCard))
                        {
                            return false;
                        }
                    }
                        break;
                    case Effect.ChangeHandCards:
                    {
                        handCard = int.Parse(value);
                    }
                        break;
                    case Effect.NumberInvocationCard:
                        break;
                    case Effect.NumberInvocationCardAttacker:
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
                    case Effect.HandMax:
                        break;
                    case Effect.CheckTurn:
                        break;
                    case Effect.SkipAttack:
                        break;
                    case Effect.SeeCards:
                        break;
                    case Effect.ChangeOrder:
                        break;
                    case Effect.Duration:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a boolean to indicate if the AffectOpponent effect can be use.
        /// It is not usable if the effectCard make the user loose more HP than he has.
        /// Cards that use it : Convocation au lycée, Demi-pizza, Incendie, Kebab magique, Maniabilité pourrie, Musique de Mega Drive, Squalala, Torture Ninja, Un délicieux risotto.
        /// </summary>
        /// <param name="value">a string which value is "true" or "false"</param>
        /// <param name="pvAffected">the number of HP the user or the opponent can loose/win</param>
        /// <param name="affectOpponent">a boolean which is true if the effect card impacts the opponent and false if the effect card impacts the user</param>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseAffectOpponent(string value, float pvAffected, out bool affectOpponent)
        {
            affectOpponent = bool.Parse(value);
            if (pvAffected != 0 && !affectOpponent)
            {
                return (currentPlayerStatus.GetCurrentPv() + pvAffected) > 0;
            }

            return true;
        }

        /// <summary>
        /// Returns a boolean to indicate if the DestroyCards effect can be use.
        /// There is different cases depending of value:
        ///  - field : A field card can be destroy if there is at least one on user field or opponent field.
        ///  - invocation :  if it affects the opponent, he must have at least one invocation card on field.
        ///  - 1 : the user can destroy one of the card on field, it must have at least one card on field.
        ///  - equipment : an equipment card can be destroy if there is at least one invocation card equip with an equipment card.
        /// Cards that use it : Croisement des effluves(all), Feuille(1), Incendie(field), Squalala(invocation).
        /// </summary>
        /// <param name="value">a string which value is "field", "invocation" or "1"</param>
        /// <param name="affectOpponent">a boolean which is true if the effect card impacts the opponent and false if the effect card impacts the user</param>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseDestroyCards(string value, bool affectOpponent)
        {
            var isValid = true;
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

                    isValid = fieldCards.Count > 0;
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

                    isValid = allCardsOnField.Count > 0;
                }
                    break;
                case "equipment":
                {
                    var invocationCardsOnField = new List<InvocationCard>(currentPlayerCard.invocationCards);
                    invocationCardsOnField.AddRange(opponentPlayerCard.invocationCards);

                    isValid = invocationCardsOnField.Select(invocationCard => invocationCard.GetEquipmentCard())
                        .Where(equipmentCard => equipmentCard != null).ToList().Count > 0;
                }
                    break;
            }

            return isValid;
        }

        /// <summary>
        /// Returns a boolean to indicate if the SacrificeInvocation effect can be use.
        /// There is different cases depending of value:
        ///  - true : User should have at least one invocation card on field.
        ///  - 5 : User should have an invocation on field with at least 5 ATK Stars or 5 DEF Stars.
        ///  - 3 : User should have an invocation on field with at least 3 ATK Stars or 3 DEF Stars.
        ///  - equipment : an equipment card can be destroy if there is at least one invocation card equip with an equipment card.
        /// Cards that use it : Croisement des effluves(true), Demi-pizza(3), Kebab magique(true), Un délicieux risotto(5).
        /// </summary>
        /// <param name="value">a string which value is "true", "3" or "5"</param>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseSacrificeInvocation(string value)
        {
            var isValid = true;
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

            return isValid;
        }

        /// <summary>
        /// Returns a boolean to indicate if the SameFamily effect can be use.
        /// Apply the same family that the field card.
        /// User must have a field card on field.
        /// Cards that use it : Convocation au lycée.
        /// </summary>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseSameFamily()
        {
            var fieldCard = currentPlayerCard.field;
            return fieldCard != null && fieldCard.IsValid();
        }

        /// <summary>
        /// Returns a boolean to indicate if the RemoveDeck effect can be use.
        /// Remove the first card of the deck.
        /// User must have cards on deck.
        /// Cards that use it : Croisement des effluves.
        /// </summary>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseRemoveDeck()
        {
            var size = currentPlayerCard.deck.Count;
            return size > 0;
        }

        /// <summary>
        /// Returns a boolean to indicate if the SpecialInvocation effect can be use.
        /// Resurect an invocation from yellow trash.
        /// User must have invocations on yellow trash and an invocation place on field.
        /// Cards that use it : Plume de phénix.
        /// </summary>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseSpecialInvocation()
        {
            var yellowTrash = currentPlayerCard.yellowTrash;
            var invocationCards = currentPlayerCard.invocationCards;
            var place = invocationCards.Count;

            var invocationFromYellowTrash =
                yellowTrash.Where(card => card.Type == CardType.Invocation).ToList();
            return place < 4 && invocationFromYellowTrash.Count > 0;
        }

        /// <summary>
        /// Returns a boolean to indicate if the Combine effect can be use.
        /// Merge several invocation cards.
        /// User should have a certain number of invocation cards on field.
        /// Cards that use it : Attaque de la tour Eiffel.
        /// </summary>
        /// <param name="value">a string which value is the number of card to merge</param>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseCombine(string value)
        {
            var numberCombine = int.Parse(value);
            var currentInvocationCards = currentPlayerCard.invocationCards;
            return currentInvocationCards.Count >= numberCombine;
        }

        /// <summary>
        /// Returns a boolean to indicate if the TakeControl effect can be use.
        /// Opponent should have at least one invocation card and user a place for a invocation card on field.
        /// Cards that use it : Youtube Money.
        /// </summary>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseTakeControl()
        {
            var invocationCardOpponent = opponentPlayerCard.invocationCards.Where(card => card.IsValid())
                .Cast<Card>().ToList();

            return invocationCardOpponent.Count > 0 && currentPlayerCard.invocationCards.Count < 4;
        }

        /// <summary>
        /// Returns a boolean to indicate if the NumberAttacks effect can be use.
        /// Give more attack by turn
        /// User should have at least one invocation card.
        /// Cards that use it : Petite portions de " riz ".
        /// </summary>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseNumberAttacks()
        {
            return currentPlayerCard.invocationCards.Count > 0;
        }

        /// <summary>
        /// Returns a boolean to indicate if the AttackDirectly effect can be use.
        /// Check if user can attack directly opponent's HP.
        /// Cards that use it : Fatalité.
        /// </summary>
        /// <param name="value">a string which value is the number of HR opponent should have to use it</param>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseAttackDirectly(string value)
        {
            var minValue = float.Parse(value);
            return opponentPlayerStatus.GetCurrentPv() < minValue;
        }

        /// <summary>
        /// Returns a boolean to indicate if the ProtectAttack effect can be use.
        /// Check if user can be protected from direct attack.
        /// He should not have any invocation cards on field.
        /// Cards that use it : Sceaux magiques.
        /// </summary>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseProtectAttack()
        {
            return currentPlayerCard.invocationCards.Count == 0;
        }

        /// <summary>
        /// Returns a boolean to indicate if the SkipFieldsEffect effect can be use.
        /// Cancel field effect during a turn.
        /// Opponent or player should have at least one field card on field.
        /// Cards that use it : Filtre dégueulasse FMV.
        /// </summary>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseSkipFieldsEffect()
        {
            return currentPlayerCard.field != null || opponentPlayerCard.field != null;
        }

        /// <summary>
        /// Returns a boolean to indicate if the RemoveHand effect can be use.
        /// Check if user has enough cards in his hands to remove some
        /// Cards that use it : Croisement des effluves, Feuille.
        /// </summary>
        /// <param name="value">a string which value is the number of cards to remove from their hands</param>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseRemoveHand(string value)
        {
            var numberCardToRemove = int.Parse(value);
            // This card + number to remove
            return currentPlayerCard.handCards.Count > numberCardToRemove;
        }

        /// <summary>
        /// Save the number of HP affected.
        /// Cards that use it : Bolossage gratuit, Convocation au lycée, Demi-pizza, Musique de Mega Drive, Torture Ninja, Un délicieux risotto.
        /// </summary>
        /// <param name="value">a string which value is the number of HP affected</param>
        /// <param name="pvAffected">a variable in which we store the HP affected in float</param>
        private static void CanUseAffectPv(string value, out float pvAffected)
        {
            if (float.TryParse(value, out pvAffected)) return;
            if (value == "all")
            {
                pvAffected = 999;
            }
        }

        /// <summary>
        /// Returns a boolean to indicate if the Sources effect can be use.
        /// Check if there is enough card in sources.
        /// if changeField is true, one can change our field card if we have one on our deck
        /// if handCard > 0, one should have at least handCard cards in our deck + yellow trash
        /// Cards that use it : Faux raccord, Le mot de passe.
        /// </summary>
        /// <param name="changeField">a boolean</param>
        /// <param name="value">a string which value is "deck" or "deck;yellow"</param>
        /// <param name="handCard">a number of card to take</param>
        /// <returns>A boolean indicating if effect card is usable</returns>
        private bool CanUseSources(bool changeField, string value, int handCard)
        {
            if (changeField)
            {
                if (value != "deck") return true;
                var fieldCardsInDeck =
                    currentPlayerCard.deck.FindAll(card => card.Type == CardType.Field);
                return fieldCardsInDeck.Count > 0;
            }
            else if (handCard > 0)
            {
                if (value == "deck;yellow")
                {
                    return (currentPlayerCard.deck.Count + currentPlayerCard.yellowTrash.Count) >=
                           handCard;
                }
            }

            return true;
        }

        /// <summary>
        /// Apply the effect of an effect card.
        /// Called when user click on put card for an effect card whose button is enabled
        /// </summary>
        /// <param name="effectCard">the effect card</param>
        /// <param name="effectCardEffect">effects of the effect card</param>
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
                    case Effect.NumberInvocationCardAttacker:
                    {
                        ApplyNumberInvocationCardAttacker(pvAffected);
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
                        ApplySources(value, handCardsNumber, changeField);
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
                            var cardsDeckToSee = BuildCardsDeckToSee(see, false);

                            void PositiveActionCardSelector()
                            {
                                DisplayMessageBoxToChangeOrderCards(cardsDeckToSee, false);
                            }

                            MessageBox.CreateMessageBoxWithCardSelector(canvas, "Veux-tu changer l'ordre ?",
                                cardsDeckToSee, PositiveActionCardSelector);
                        }

                        void NegativeAction()
                        {
                            var cardsDeckToSee = BuildCardsDeckToSee(see, true);

                            void PositiveActionCardSelector()
                            {
                                DisplayMessageBoxToChangeOrderCards(cardsDeckToSee, true);
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
                    {
                        if (keys.Count == 1)
                        {
                            ApplyDestroyCards(value);
                        }
                    }
                        break;
                    case Effect.AttackDirectly:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //TODO Add effectCard to yellow trash if necessary
        }


        /// <summary>
        /// Apply the AffectPv effect.
        /// Store affected HP
        /// </summary>
        /// <param name="effectCard">the effect card</param>
        /// <param name="value">value is a string which is the number of affected HP</param>
        /// <returns>the number of affected HP</returns>
        private static float ApplyAffectPv(EffectCard effectCard, string value)
        {
            var pvAffected = value == "all" ? 100.0f : float.Parse(value);

            effectCard.affectPv = pvAffected;

            return pvAffected;
        }

        /// <summary>
        /// Apply the AffectOpponent effect.
        /// Store boolean value
        /// </summary>
        /// <param name="value">value is a string which is a boolean</param>
        /// <returns>a boolean indicating if it affect the user or his opponent</returns>
        private static bool ApplyAffectOpponent(string value)
        {
            return bool.Parse(value);
        }

        /// <summary>
        /// Apply the DestroyCards effect.
        /// Destroy cards depending of value :
        /// - field : remove a field card if there is one on field and it affects HP negatively
        /// - all : send all cards on field to their respective yellow trashs
        /// - invocation : send an opponent invocation card (except those which are not affected by effect cards) to its yellow trash 
        /// - 1 : send one card which is on field to its respective yellow trash
        /// - equipment : send a equipment card from opponent or user to its yellow trash
        /// </summary>
        /// <param name="value">value is a string which is the type of card to destroy or the number</param>
        /// <param name="pvAffected">number of affected HP</param>
        /// <param name="affectOpponent">boolean to indicate if affected HP is for the user or his opponent</param>
        private void ApplyDestroyCards(string value, float pvAffected = 0f, bool affectOpponent = false)
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
                                    // User select a card
                                    // Need to find if it's from user or opponent
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
                                    // User don't select a card
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

                            Destroy(message);
                        }
                        else
                        {
                            message.SetActive(false);
                            UnityAction okAction = () => { message.SetActive(true); };
                            MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte à détruire",
                                okAction);
                        }
                    };
                    message.GetComponent<MessageBox>().NegativeAction = () =>
                    {
                        message.SetActive(false);
                        UnityAction okAction = () => { message.SetActive(true); };
                        MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte à détruire",
                            okAction);
                    };
                }
                    break;

                case "equipment":
                {
                    var currentPlayerEquipmentCard = currentPlayerCard.invocationCards
                        .Select(invocationCard => invocationCard.GetEquipmentCard())
                        .Where(equipmentCard => equipmentCard != null).ToList();
                    var opponentPlayerEquipmentCard = opponentPlayerCard.invocationCards
                        .Select(invocationCard => invocationCard.GetEquipmentCard())
                        .Where(equipmentCard => equipmentCard != null).ToList();

                    var equipmentCards = new List<Card>(currentPlayerEquipmentCard);
                    equipmentCards.AddRange(opponentPlayerEquipmentCard);

                    var message =
                        MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choisis une carte équipement à détruire",
                            equipmentCards);
                    message.GetComponent<MessageBox>().PositiveAction = () =>
                    {
                        var equipmentCardSelected = message.GetComponent<MessageBox>().GetSelectedCard();
                        if (equipmentCardSelected != null)
                        {
                            var isCurrent = currentPlayerEquipmentCard.Any(equipmentCard =>
                                equipmentCard.Nom == equipmentCardSelected.Nom);
                            if (isCurrent)
                            {
                                currentPlayerCard.yellowTrash.Add(equipmentCardSelected);
                                foreach (var invocation in currentPlayerCard.invocationCards.Where(invocation =>
                                             invocation.GetEquipmentCard() != null &&
                                             invocation.GetEquipmentCard().Nom ==
                                             equipmentCardSelected.Nom))
                                {
                                    invocation.SetEquipmentCard(null);
                                }
                            }
                            else
                            {
                                opponentPlayerCard.yellowTrash.Add(equipmentCardSelected);
                                foreach (var invocation in currentPlayerCard.invocationCards.Where(invocation =>
                                             invocation.GetEquipmentCard() != null &&
                                             invocation.GetEquipmentCard().Nom ==
                                             equipmentCardSelected.Nom))
                                {
                                    invocation.SetEquipmentCard(null);
                                }
                            }

                            Destroy(message);
                        }
                        else
                        {
                            message.SetActive(false);
                            UnityAction okAction = () => { message.SetActive(true); };
                            MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte à détruire",
                                okAction);
                        }
                    };
                    message.GetComponent<MessageBox>().NegativeAction = () =>
                    {
                        message.SetActive(false);
                        UnityAction okAction = () => { message.SetActive(true); };
                        MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte à détruire",
                            okAction);
                    };
                }
                    break;
            }
        }

        /// <summary>
        /// Apply the NumberInvocationCard effect.
        /// Change HP on user or opponent depending of affectOpponent bool and the number of invocation cards on field on the correspond player
        /// </summary>
        /// <param name="pvAffected">number of affected HP</param>
        /// <param name="affectOpponent">boolean to indicate if affected HP is for the user or his opponent</param>
        private void ApplyNumberInvocationCard(bool affectOpponent, float pvAffected)
        {
            // TODO: test this refactor
            var invocationCards =
                affectOpponent ? opponentPlayerCard.invocationCards : currentPlayerCard.invocationCards;
            var playerStatus = affectOpponent ? opponentPlayerStatus : currentPlayerStatus;
            var size = invocationCards.Count;
            var damages = size * pvAffected;
            playerStatus.ChangePv(damages);

            /*if (affectOpponent)
            {
                AffectOpponentNumberInvocation(pvAffected);
            }
            else
            {
                AffectCurrentNumberInvocation(pvAffected);
            }*/
        }

        private void AffectOpponentNumberInvocation(float pvAffected)
        {
            var invocationCards = opponentPlayerCard.invocationCards;
        }

        private void AffectCurrentNumberInvocation(float pvAffected)
        {
            var invocationCards = currentPlayerCard.invocationCards;
            var size = invocationCards.Count;
            var damages = size * pvAffected;
            currentPlayerStatus.ChangePv(damages);
        }

        /// <summary>
        /// Apply the NumberInvocationCardAttacker effect.
        /// Change HP on opponent depending of the number of invocation cards the user has on field.
        /// </summary>
        /// <param name="pvAffected">number of affected HP</param>
        private void ApplyNumberInvocationCardAttacker(float pvAffected)
        {
            var invocationCards = currentPlayerCard.invocationCards;
            var size = invocationCards.Count;
            var damages = size * pvAffected;
            opponentPlayerStatus.ChangePv(damages);
        }

        /// <summary>
        /// Apply the NumberHandCard effect.
        /// Change HP on opponent or user depending of the number of Hand cards the user has on field.
        /// </summary>
        /// <param name="affectOpponent">boolean to indicate if affected HP is for the user or his opponent</param>
        /// <param name="pvAffected">number of affected HP</param>
        private void ApplyNumberHandCard(bool affectOpponent, float pvAffected)
        {
            // TODO: test it
            var handCards = affectOpponent ? opponentPlayerCard.handCards : currentPlayerCard.handCards;
            var playerStatus = affectOpponent ? opponentPlayerStatus : currentPlayerStatus;
            var size = handCards.Count;
            var damages = size * pvAffected;
            playerStatus.ChangePv(damages);
            /*if (affectOpponent)
            {
                AffectOpponentNumberHandCard(pvAffected);
            }
            else
            {
                AffectCurrentNumberHandCard(pvAffected);
            }*/
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

        /// <summary>
        /// Apply the SacrificeInvocation effect.
        /// User has to choose invocations to sacrifice.
        /// Depending of value :
        ///  - true : user can sacrifice any invocations he wants
        ///  - 5 : user can sacrifice invocations that has at least 5 ATK Stars or 5 DEF Stars
        ///  - 3 : user can sacrifice invocations that has at least 3 ATK Stars or 3 DEF Stars
        /// </summary>
        /// <param name="value">value is a string which is a bool or a number</param>
        /// <param name="pvAffected">number of HP user can win or lose (can be 0)</param>
        /// <param name="completion">callback call after selecting the invocation to sacrifice</param>
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

        /// <summary>
        /// Display sacrifice messageBox.
        /// User has to choose invocations to sacrifice.
        /// </summary>
        /// <param name="invocationCardsValid">invocation cards list to display</param>
        /// <param name="pvAffected">number of HP user can win or lose (can be 0)</param>
        /// <param name="completion">callback call after selecting the invocation to sacrifice</param>
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

        /// <summary>
        /// Apply the SameFamily effect.
        /// Invocation cards on field has the same family as the field card.
        /// </summary>
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

        /// <summary>
        /// Apply the CheckTurn effect.
        /// Set a boolean on effect card to check effects of this card at every draw (start of turn)
        /// </summary>
        /// <param name="effectCard">effect card to check at each turn</param>
        private static void ApplyCheckTurn(EffectCard effectCard)
        {
            effectCard.checkTurn = true;
        }

        /// <summary>
        /// Apply the ChangeHandCards effect.
        /// Store the number of card to add/remove from handCards
        /// Remove or addition are deduced thanks to other effects on effect card
        /// </summary>
        /// <param name="value">value is a string which is a number</param>
        private static int ApplyChangeHandCards(string value)
        {
            var handCardsNumber = int.Parse(value);
            return handCardsNumber;
        }

        /// <summary>
        /// Apply the Sources effect.
        /// Get a card from value or change field card on field by another from the deck
        /// </summary>
        /// <param name="value">value is a string representing the different sources to get cards to (deck, yellow) separated by ;</param>
        /// <param name="handCardsNumber">number of cards to pick</param>
        /// <param name="changeField">bool to indicate if we're going to change a field card</param>
        private void ApplySources(string value, int handCardsNumber, bool changeField)
        {
            var sources = SplitSources(value);
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
                                "Tu dois choisir un terrain",
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

        /// <summary>
        /// Apply the HandMax effect.
        /// Force user to have a number of cards in their hands
        /// </summary>
        /// <param name="handCardsNumber">number of cards to have in hand</param>
        /// <param name="effectCard">effect card that forces it</param>
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

        private List<Card> BuildCardsDeckToSee(int see, bool isCurrentPlayerCard)
        {
            var playerCard = isCurrentPlayerCard ? currentPlayerCard : opponentPlayerCard;
            List<Card> cardsDeckToSee = new List<Card>();
            if (playerCard.deck.Count > see)
            {
                for (var j = playerCard.deck.Count - 1;
                     j > (playerCard.deck.Count - 1 - see);
                     j--)
                {
                    cardsDeckToSee.Add(playerCard.deck[j]);
                }
            }
            else
            {
                cardsDeckToSee.AddRange(playerCard.invocationCards);
            }

            return cardsDeckToSee;
        }

        private void DisplayMessageBoxToChangeOrderCards(List<Card> cardsDeckToSee, bool isCurrentPlayerCard)
        {
            var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Choisis les cartes dans l'ordre que tu veux", cardsDeckToSee,
                multipleCardSelection: true, numberCardInSelection: cardsDeckToSee.Count, displayOrder: true);
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var selectedCards =
                    messageBox.GetComponent<MessageBox>().GetMultipleSelectedCards();
                if (selectedCards.Count == cardsDeckToSee.Count)
                {
                    var playerCard = isCurrentPlayerCard ? currentPlayerCard : opponentPlayerCard;
                    foreach (var card in cardsDeckToSee)
                    {
                        playerCard.deck.Remove(card);
                    }

                    selectedCards.Reverse();
                    playerCard.deck.AddRange(selectedCards);
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

        private static string[] SplitSources(string value)
        {
            return value.Split(';');
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