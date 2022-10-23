using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using Cards.InvocationCards;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCards : MonoBehaviour
{
    [FormerlySerializedAs("Deck")] [SerializeField]
    public List<Card> deck;

    [SerializeField] public List<Card> handCards;

    [FormerlySerializedAs("YellowTrash")] [SerializeField]
    public List<Card> yellowTrash;

    [FormerlySerializedAs("Field")] [SerializeField]
    public FieldCard field;

    [SerializeField] public List<InvocationCard> invocationCards;

    [FormerlySerializedAs("EffectCards")] [SerializeField]
    public List<EffectCard> effectCards;

    [FormerlySerializedAs("IsPlayerOne")] [SerializeField]
    private bool isPlayerOne;
    
    [SerializeField] private GameObject nextPhaseButton;
    [SerializeField] private GameObject inHandButton;
    [SerializeField] private Transform canvas;
    [SerializeField] private CardLocation cardLocation;


    public List<Card> secretCards = new List<Card>(); // Where combine card go


    public string Tag => isPlayerOne ? "card1" : "card2";

    private List<InvocationCard> oldInvocationCards = new List<InvocationCard>();
    private FieldCard oldField;
    private List<Card> oldHandCards = new List<Card>();
    private List<Card> oldYellowTrash = new List<Card>();

    public bool IsFieldDesactivate { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        invocationCards = new List<InvocationCard>(4);
        effectCards = new List<EffectCard>(4);
        var gameStateGameObject = GameObject.Find("GameState");
        var gameState = gameStateGameObject.GetComponent<GameState>();
        deck = isPlayerOne ? gameState.deckP1 : gameState.deckP2;
        cardLocation.InitPhysicalCards(deck, isPlayerOne);

        for (var i = deck.Count - 5; i < deck.Count; i++)
        {
            handCards.Add(deck[i]);
        }
        deck.RemoveRange(deck.Count - 5, 5);
        cardLocation.HideCards(handCards);
    }

    public void DesactivateFieldCardEffect()
    {
        if (field != null)
        {
            OnFieldCardDesactivate(field);
        }

        IsFieldDesactivate = true;
    }

    public void ActivateFieldCardEffect()
    {
        IsFieldDesactivate = false;
        if (field != null)
        {
            FieldFunctions.ApplyFieldCardEffect(field, this);
        }
    }



    public void ResetInvocationCardNewTurn()
    {
        foreach (var invocationCard in invocationCards.Where(invocationCard =>
                     invocationCard != null && invocationCard.Nom != null))
        {
            invocationCard.ResetNewTurn();
        }
    }

    public bool ContainsCardInInvocation(InvocationCard invocationCard)
    {
        return invocationCards.Any(invocation => invocation != null && invocation.Nom == invocationCard.Nom);
    }

    // Update is called once per frame
    private void Update()
    {
        if (invocationCards.Count != oldInvocationCards.Count)
        {
            if (invocationCards.Count > oldInvocationCards.Count)
            {
                OnInvocationCardAdded(invocationCards.Last());
            }
            else
            {
                var removedInvocationCard = oldInvocationCards.Except(invocationCards).First();
                OnInvocationCardsRemoved(removedInvocationCard);
            }

            oldInvocationCards = new List<InvocationCard>(invocationCards);
            OnInvocationCardsChanged();
        }

        if (field != oldField)
        {
            for (var i = effectCards.Count - 1; i >= 0; i--)
            {
                var effectCard = effectCards[i];
                var effectCardEffect = effectCard.GetEffectCardEffect();
                if (effectCardEffect == null) continue;
                if (!effectCardEffect.Keys.Contains(Effect.SameFamily)) continue;
                if (field != null && !IsFieldDesactivate)
                {
                    foreach (var invocationCard in invocationCards)
                    {
                        invocationCard.SetCurrentFamily(field.GetFamily());
                    }
                }
                else
                {
                    effectCards.Remove(effectCard);
                    yellowTrash.Add(effectCard);
                }
            }

            OnFieldCardChanged(oldField);
            oldField = field;
        }

        if (oldHandCards.Count != handCards.Count)
        {
            foreach (var invocationCard in invocationCards)
            {
                if (invocationCard.GetEquipmentCard() == null ||
                    invocationCard.GetEquipmentCard().EquipmentPermEffect == null) continue;
                var permEffect = invocationCard.GetEquipmentCard().EquipmentPermEffect;
                if (permEffect.Keys.Contains(PermanentEffect.AddAtkBaseOnHandCards))
                {
                    var value = float.Parse(permEffect.Values[
                        permEffect.Keys.FindIndex(key => key == PermanentEffect.AddAtkBaseOnHandCards)]);
                    var newBonusAttack = invocationCard.GetBonusAttack() +
                                         (handCards.Count - oldHandCards.Count) * value;
                    invocationCard.SetBonusAttack(newBonusAttack);
                }
                else if (permEffect.Keys.Contains(PermanentEffect.AddDefBaseOnHandCards))
                {
                    var value = float.Parse(permEffect.Values[
                        permEffect.Keys.FindIndex(key => key == PermanentEffect.AddDefBaseOnHandCards)]);
                    var newBonusDefense = invocationCard.GetBonusDefense() +
                                          (handCards.Count - oldHandCards.Count) * value;
                    invocationCard.SetBonusDefense(newBonusDefense);
                }
            }

            oldHandCards = new List<Card>(handCards);
        }

        if (oldYellowTrash.Count != yellowTrash.Count)
        {
            if (yellowTrash.Count > oldYellowTrash.Count)
            {
                OnYellowTrashAdded();
            }

            oldYellowTrash = new List<Card>(yellowTrash);
        }
    }

    public void SendToSecretHide(Card card)
    {
        secretCards.Add(card);
    }

    public void RemoveFromSecretHide(Card card)
    {
        secretCards.Remove(card);
    }

    public void SendCardToYellowTrash(Card card)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i].Nom != card.Nom) continue;
            var invocationCard = card as InvocationCard;
            if (invocationCard == null) continue;
            if (invocationCard is SuperInvocationCard superInvocationCard)
            {
                var invocationListCards = superInvocationCard.invocationCards;
                foreach (var cardFromList in invocationListCards)
                {
                    secretCards.Remove(cardFromList);
                    yellowTrash.Add(cardFromList);
                }

                invocationCards.Remove(superInvocationCard);
            }
            else
            {
                if (invocationCard.GetEquipmentCard() != null)
                {
                    var equipmentCard = invocationCard.GetEquipmentCard();
                    yellowTrash.Add(equipmentCard);
                    invocationCard.SetEquipmentCard(null);
                }

                invocationCards.Remove((InvocationCard)card);
                yellowTrash.Add(card);
            }
        }

        for (var i = 0; i < effectCards.Count; i++)
        {
            if (effectCards[i].Nom == card.Nom)
            {
                effectCards.Remove(card as EffectCard);
                yellowTrash.Add(card);
            }
        }

        if (field != null && field.Nom == card.Nom)
        {
            field = null;
            yellowTrash.Add(card);
        }
    }

    public void SendInvocationCardToYellowTrash(InvocationCard specificCardFound)
    {
        var equipmentCard = specificCardFound.GetEquipmentCard();
        specificCardFound.SetEquipmentCard(null);
        specificCardFound.SetBonusAttack(0);
        specificCardFound.SetBonusDefense(0);
        specificCardFound.SetRemainedAttackThisTurn(1);
        if (specificCardFound.IsControlled)
        {
            var opponentPlayerCards = isPlayerOne
                ? GameObject.Find("Player2").GetComponent<PlayerCards>()
                : GameObject.Find("Player1").GetComponent<PlayerCards>();
            opponentPlayerCards.secretCards.Remove(specificCardFound);
            opponentPlayerCards.yellowTrash.Add(specificCardFound);
            if (equipmentCard != null)
            {
                opponentPlayerCards.yellowTrash.Add(equipmentCard);
            }
            
            invocationCards.Remove(specificCardFound);
            
            cardLocation.RemovePhysicalCard(specificCardFound);

            if (specificCardFound.GetInvocationDeathEffect() == null) return;
            var invocationDeathEffect = specificCardFound.GetInvocationDeathEffect();
            var keys = invocationDeathEffect.Keys;
            var values = invocationDeathEffect.Values;

            var cardName = "";
            for (var i = 0; i < keys.Count; i++)
            {
                var value = values[i];
                switch (keys[i])
                {
                    case DeathEffect.GetSpecificCard:
                        cardName = values[i];
                        break;
                    case DeathEffect.GetCardSource:
                        opponentPlayerCards.GetCardSourceDeathEffect(specificCardFound, value, cardName);
                        break;
                    case DeathEffect.ComeBackToHand:
                        //opponentPlayerCards.ComeBackToHandDeathEffect(specificCardFound, value);
                        break;
                    case DeathEffect.KillAlsoOtherCard:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        else
        {
            invocationCards.Remove(specificCardFound);
            yellowTrash.Add(specificCardFound);
            if (equipmentCard != null)
            {
                yellowTrash.Add(equipmentCard);
            }

            if (specificCardFound.GetInvocationDeathEffect() == null) return;
            var invocationDeathEffect = specificCardFound.GetInvocationDeathEffect();
            var keys = invocationDeathEffect.Keys;
            var values = invocationDeathEffect.Values;

            var cardName = "";
            for (var i = 0; i < keys.Count; i++)
            {
                var value = values[i];
                switch (keys[i])
                {
                    case DeathEffect.GetSpecificCard:
                        cardName = values[i];
                        break;
                    case DeathEffect.GetCardSource:
                        GetCardSourceDeathEffect(specificCardFound, value, cardName);
                        break;
                    case DeathEffect.ComeBackToHand:
                        //ComeBackToHandDeathEffect(specificCardFound, value);
                        break;
                    case DeathEffect.KillAlsoOtherCard:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        specificCardFound.FreeCard();
    }
    
    public void RemoveSuperInvocation(Card superInvocationCard)
    {
        invocationCards.Remove(superInvocationCard as InvocationCard);
        cardLocation.RemovePhysicalCard(superInvocationCard);
    }

    private void ComeBackToHandDeathEffect(InvocationCard invocationCard, string value)
    {
        var isParsed = int.TryParse(value, out var number);
        if (isParsed)
        {
            if (invocationCard.GetNumberDeaths() > number)
            {
                SendInvocationCardToYellowTrash(invocationCard);
            }
            else
            {
                SendCardToHand(invocationCard);
            }
        }
        else
        {
            SendCardToHand(invocationCard);
        }
    }

    private void AskUserToAddCardInHand(string cardName, Card cardFound, bool isFound)
    {
        if (!isFound) return;

        nextPhaseButton.SetActive(false);
        inHandButton.SetActive(false);

        void PositiveAction()
        {
            handCards.Add(cardFound);
            deck.Remove(cardFound);
            nextPhaseButton.SetActive(true);
            inHandButton.SetActive(true);
        }

        void NegativeAction()
        {
            nextPhaseButton.SetActive(true);
            inHandButton.SetActive(true);
        }

        MessageBox.CreateSimpleMessageBox(canvas, "Carte en main",
            "Veux-tu aussi ajouter " + cardName + " à ta main ?", PositiveAction, NegativeAction);
    }

    private void GetCardSourceDeathEffect(Card invocationCard, string source,
        string cardName)
    {
        Card cardFound = null;

        SendCardToYellowTrash(invocationCard);
        switch (source)
        {
            case "deck":
            {
                if (cardName != "")
                {
                    var isFound = false;
                    var j = 0;
                    while (j < deck.Count && !isFound)
                    {
                        if (deck[j].Nom == cardName)
                        {
                            isFound = true;
                            cardFound = deck[j];
                        }

                        j++;
                    }

                    AskUserToAddCardInHand(cardName, cardFound, isFound);
                }

                break;
            }
            case "trash":
            {
                var trash = yellowTrash;
                if (cardName != "")
                {
                    var isFound = false;
                    var j = 0;
                    while (j < trash.Count && !isFound)
                    {
                        if (trash[j].Nom == cardName)
                        {
                            isFound = true;
                            cardFound = trash[j];
                        }

                        j++;
                    }

                    AskUserToAddCardInHand(cardName, cardFound, isFound);
                }

                break;
            }
        }
    }

    public void SendCardToHand(Card card)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i].Nom == card.Nom)
            {
                invocationCards.Remove(card as InvocationCard);
            }
        }

        handCards.Add(card);
    }

    public int GetIndexInvocationCard(string nameCard)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i].Nom == nameCard)
            {
                return i;
            }
        }

        return -1;
    }

    private void OnInvocationCardAdded(InvocationCard newInvocationCard)
    {
        var opponentEffectCards = isPlayerOne
            ? GameObject.Find("Player2").GetComponent<PlayerCards>().effectCards
            : GameObject.Find("Player2").GetComponent<PlayerCards>().effectCards;

        var mustSkipAttack = opponentEffectCards.Select(effectCard => effectCard.GetEffectCardEffect().Keys)
            .Any(keys => keys.Contains(Effect.SkipAttack));


        for (var j = invocationCards.Count - 1; j >= 0; j--)
        {
            var invocationCard = invocationCards[j];

            if (mustSkipAttack)
            {
                invocationCard.BlockAttack();
            }

            var permEffect = invocationCard.InvocationPermEffect;
            if (permEffect == null) continue;
            var keys = permEffect.Keys;
            var values = permEffect.Values;

            var invocationCardsToChange = new List<InvocationCard>();
            var sameFamilyInvocationCards = new List<InvocationCard>();
            var mustHaveMinimumUndef = false;

            for (var i = 0; i < keys.Count; i++)
            {
                var value = values[i];
                switch (keys[i])
                {
                    case PermEffect.GiveStat:
                    {
                        GiveStatPermEffect(newInvocationCard, value, invocationCard, ref invocationCardsToChange);
                    }
                        break;
                    case PermEffect.Family:
                    {
                        FamilyPermEffect(newInvocationCard, value, invocationCard, ref sameFamilyInvocationCards);
                    }
                        break;
                    case PermEffect.Condition:
                    {
                        ConditionPermEffect(value, ref sameFamilyInvocationCards, ref mustHaveMinimumUndef);
                    }
                        break;
                    case PermEffect.IncreaseAtk:
                    {
                        IncreaseAtkPermEffect(invocationCardsToChange, value, sameFamilyInvocationCards,
                            ref invocationCard,
                            mustHaveMinimumUndef);
                    }
                        break;
                    case PermEffect.IncreaseDef:
                    {
                        IncreaseDefPermEffect(invocationCardsToChange, value, sameFamilyInvocationCards,
                            ref invocationCard,
                            mustHaveMinimumUndef);
                    }
                        break;
                    case PermEffect.CanOnlyAttackIt:
                    case PermEffect.PreventInvocationCards:
                    case PermEffect.ProtectBehind:
                    case PermEffect.ImpossibleAttackByInvocation:
                    case PermEffect.ImpossibleToBeAffectedByEffect:
                    case PermEffect.NumberTurn:
                    case PermEffect.checkCardsOnField:
                    default:
                        break;
                }
            }
        }

        for (var i = effectCards.Count - 1; i >= 0; i--)
        {
            var effectCard = effectCards[i];
            var effectCardEffect = effectCard.GetEffectCardEffect();
            if (effectCardEffect != null)
            {
                if (effectCardEffect.Keys.Contains(Effect.SameFamily))
                {
                    if (field != null && !IsFieldDesactivate)
                    {
                        newInvocationCard.SetCurrentFamily(field.GetFamily());
                    }
                }
                else if (effectCardEffect.Keys.Contains(Effect.NumberAttacks))
                {
                    var value = int.Parse(
                        effectCardEffect.Values[
                            effectCardEffect.Keys.FindIndex(effect => effect == Effect.NumberAttacks)]);
                    newInvocationCard.SetRemainedAttackThisTurn(value);
                }
            }
        }

        if (field != null && !IsFieldDesactivate)
        {
            var fieldCardEffect = field.FieldCardEffect;

            var fieldKeys = fieldCardEffect.Keys;
            var fieldValues = fieldCardEffect.Values;

            var family = field.GetFamily();
            for (var i = 0; i < fieldKeys.Count; i++)
            {
                var fieldValue = fieldValues[i];
                switch (fieldKeys[i])
                {
                    case FieldEffect.ATK:
                    {
                        ATKFieldEffect(ref newInvocationCard, fieldValue, family);
                    }
                        break;
                    case FieldEffect.DEF:
                    {
                        DEFFieldEffect(ref newInvocationCard, fieldValue, family);
                    }
                        break;
                    case FieldEffect.Change:
                    {
                        ChangeFieldEffect(ref newInvocationCard, fieldValue, family);
                    }
                        break;
                    case FieldEffect.GetCard:
                        break;
                    case FieldEffect.DrawCard:
                        break;
                    case FieldEffect.Life:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private static void ChangeFieldEffect(ref InvocationCard newInvocationCard, string fieldValue, CardFamily family)
    {
        var names = fieldValue.Split(';');
        if (names.Contains(newInvocationCard.Nom))
        {
            newInvocationCard.SetCurrentFamily(family);
        }
    }

    private static void DEFFieldEffect(ref InvocationCard newInvocationCard, string fieldValue, CardFamily family)
    {
        var def = float.Parse(fieldValue);
        if (newInvocationCard.GetFamily().Contains(family))
        {
            var newBonusDefense = newInvocationCard.GetBonusDefense() + def;
            newInvocationCard.SetBonusDefense(newBonusDefense);
        }
    }

    private static void ATKFieldEffect(ref InvocationCard newInvocationCard, string fieldValue, CardFamily family)
    {
        var atk = float.Parse(fieldValue);
        if (newInvocationCard.GetFamily().Contains(family))
        {
            var newBonusAttack = newInvocationCard.GetBonusAttack() + atk;
            newInvocationCard.SetBonusAttack(newBonusAttack);
        }
    }

    private static void IncreaseDefPermEffect(List<InvocationCard> invocationCardsToChange, string value,
        List<InvocationCard> sameFamilyInvocationCards, ref InvocationCard invocationCard, bool mustHaveMinimumUndef)
    {
        if (invocationCardsToChange.Count > 0)
        {
            foreach (var invocationCardToChange in invocationCardsToChange)
            {
                var newDef = invocationCardToChange.GetBonusDefense() + float.Parse(value);
                invocationCardToChange.SetBonusDefense(newDef);
            }
        }
        else if (sameFamilyInvocationCards.Count > 0)
        {
            var newValue = invocationCard.GetBonusDefense() +
                           float.Parse(value) * sameFamilyInvocationCards.Count;
            invocationCard.SetBonusDefense(newValue);
        }
        else if (mustHaveMinimumUndef)
        {
            var minValue = float.Parse(value);
            if (invocationCard.GetCurrentDefense() < minValue)
            {
                var newDef = minValue - invocationCard.GetCurrentDefense();
                invocationCard.SetBonusDefense(newDef);
            }
        }
    }

    private static void IncreaseAtkPermEffect(List<InvocationCard> invocationCardsToChange, string value,
        List<InvocationCard> sameFamilyInvocationCards, ref InvocationCard invocationCard, bool mustHaveMinimumUndef)
    {
        if (invocationCardsToChange.Count > 0)
        {
            foreach (var invocationCardToChange in invocationCardsToChange)
            {
                var newAtk = invocationCardToChange.GetBonusAttack() + float.Parse(value);
                invocationCardToChange.SetBonusAttack(newAtk);
            }
        }
        else if (sameFamilyInvocationCards.Count > 0)
        {
            var newValue = invocationCard.GetBonusAttack() +
                           float.Parse(value) * sameFamilyInvocationCards.Count;
            invocationCard.SetBonusAttack(newValue);
        }
        else if (mustHaveMinimumUndef)
        {
            var minValue = float.Parse(value);
            if (invocationCard.GetCurrentAttack() < minValue)
            {
                var newAtk = minValue - invocationCard.GetCurrentAttack();
                invocationCard.SetBonusAttack(newAtk);
            }
        }
    }

    private void ConditionPermEffect(string value, ref List<InvocationCard> sameFamilyInvocationCards,
        ref bool mustHaveMinimumUndef)
    {
        switch (value)
        {
            case "2 ATK 2 DEF":
            {
                for (var k = sameFamilyInvocationCards.Count - 1; k >= 0; k--)
                {
                    var invocationCardToCheck = sameFamilyInvocationCards[k];
                    if (invocationCardToCheck.GetAttack() != 2f ||
                        invocationCardToCheck.GetDefense() != 2f)
                    {
                        sameFamilyInvocationCards.Remove(invocationCardToCheck);
                    }
                }
            }
                break;
            case "Benzaie jeune":
            {
                if (invocationCards.Any(invocationCardToCheck =>
                        invocationCardToCheck.Nom == value))
                {
                    mustHaveMinimumUndef = true;
                }
            }
                break;
        }
    }

    private void FamilyPermEffect(InvocationCard newInvocationCard, string value, InvocationCard invocationCard,
        ref List<InvocationCard> sameFamilyInvocationCards)
    {
        if (Enum.TryParse(value, out CardFamily cardFamily))
        {
            if (newInvocationCard.Nom == invocationCard.Nom)
            {
                // Must catch up old cards
                sameFamilyInvocationCards.AddRange(invocationCards
                    .Where(invocationCardToCheck => invocationCardToCheck.Nom != newInvocationCard.Nom)
                    .Where(invocationCardToCheck =>
                        invocationCardToCheck.GetFamily().Contains(cardFamily)));
            }
            else if (newInvocationCard.GetFamily().Contains(cardFamily))
            {
                sameFamilyInvocationCards.Add(newInvocationCard);
            }
        }
    }

    private void GiveStatPermEffect(InvocationCard newInvocationCard, string value, InvocationCard invocationCard,
        ref List<InvocationCard> invocationCardsToChange)
    {
        if (Enum.TryParse(value, out CardFamily cardFamily))
        {
            if (newInvocationCard.Nom == invocationCard.Nom)
            {
                // Must catch up old cards
                invocationCardsToChange.AddRange(invocationCards
                    .Where(invocationCardToCheck => invocationCardToCheck.Nom != newInvocationCard.Nom)
                    .Where(invocationCardToCheck =>
                        invocationCardToCheck.GetFamily().Contains(cardFamily)));
            }
            else if (newInvocationCard.GetFamily().Contains(cardFamily))
            {
                invocationCardsToChange.Add(newInvocationCard);
            }
        }
    }

    private void OnInvocationCardsRemoved(InvocationCard removedInvocationCard)
    {
        for (var j = oldInvocationCards.Count - 1; j >= 0; j--)
        {
            var invocationCard = oldInvocationCards[j];

            invocationCard.UnblockAttack();

            var permEffect = invocationCard.InvocationPermEffect;
            var actionEffect = invocationCard.InvocationActionEffect;
            if (permEffect != null)
            {
                var keys = permEffect.Keys;
                var values = permEffect.Values;

                var invocationCardsToChange = new List<InvocationCard>();
                var sameFamilyInvocationCards = new List<InvocationCard>();

                var mustHaveMiminumAtkdef = false;

                for (var i = 0; i < keys.Count; i++)
                {
                    var value = values[i];
                    switch (keys[i])
                    {
                        case PermEffect.GiveStat:
                        {
                            GiveStatPermEffectRemoved(removedInvocationCard, value, invocationCard,
                                ref invocationCardsToChange);
                        }
                            break;
                        case PermEffect.Family:
                        {
                            FamilyPermEffectRemoved(removedInvocationCard, value, invocationCard,
                                ref sameFamilyInvocationCards);
                        }
                            break;
                        case PermEffect.Condition:
                        {
                            ConditionPermEffectRemoved(removedInvocationCard, value, sameFamilyInvocationCards,
                                invocationCard, ref mustHaveMiminumAtkdef);
                        }
                            break;
                        case PermEffect.IncreaseAtk:
                        {
                            IncreaseAtkPermEffectRemoved(invocationCardsToChange, value, sameFamilyInvocationCards,
                                ref invocationCard, mustHaveMiminumAtkdef);
                        }
                            break;
                        case PermEffect.IncreaseDef:
                        {
                            IncreaseDefPermEffectRemoved(invocationCardsToChange, value, sameFamilyInvocationCards,
                                ref invocationCard, mustHaveMiminumAtkdef);
                        }
                            break;
                        case PermEffect.CanOnlyAttackIt:
                            break;
                        case PermEffect.PreventInvocationCards:
                            break;
                        case PermEffect.ProtectBehind:
                            break;
                        case PermEffect.ImpossibleAttackByInvocation:
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
            else if (actionEffect != null)
            {
                var keys = actionEffect.Keys;
                var values = actionEffect.Values;

                float atk = 0;
                float def = 0;
                List<int> indexToDelete = new List<int>();

                for (var i = 0; i < keys.Count; i++)
                {
                    var value = values[i];
                    switch (keys[i])
                    {
                        case ActionEffect.GiveAtk:
                        {
                            atk = float.Parse(value);
                        }
                            break;
                        case ActionEffect.GiveDef:
                        {
                            def = float.Parse(value);
                        }
                            break;
                        case ActionEffect.Beneficiary:
                        {
                            BeneficiaryActionEffect(removedInvocationCard, invocationCard, value, atk, def,
                                ref indexToDelete, i);
                        }
                            break;
                    }
                }

                if (indexToDelete.Count > 0)
                {
                    indexToDelete.Reverse();
                    foreach (var index in indexToDelete)
                    {
                        actionEffect.Keys.RemoveAt(index);
                        actionEffect.Values.RemoveAt(index);
                    }
                }
            }
        }

        for (var i = effectCards.Count - 1; i >= 0; i--)
        {
            var effectCard = effectCards[i];
            var effectCardEffect = effectCard.GetEffectCardEffect();
            if (effectCardEffect != null)
            {
                if (effectCardEffect.Keys.Contains(Effect.SameFamily))
                {
                    if (field != null)
                    {
                        removedInvocationCard.SetCurrentFamily(null);
                    }
                }
            }
        }

        if (field != null)
        {
            var fieldCardEffect = field.FieldCardEffect;

            var fieldKeys = fieldCardEffect.Keys;
            var fieldValues = fieldCardEffect.Values;

            for (var i = 0; i < fieldKeys.Count; i++)
            {
                switch (fieldKeys[i])
                {
                    case FieldEffect.Change:
                    {
                        ChangeFieldEffect(ref removedInvocationCard, fieldValues[i]);
                    }
                        break;
                    case FieldEffect.ATK:
                        break;
                    case FieldEffect.DEF:
                        break;
                    case FieldEffect.GetCard:
                        break;
                    case FieldEffect.DrawCard:
                        break;
                    case FieldEffect.Life:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private static void ChangeFieldEffect(ref InvocationCard removedInvocationCard, string fieldValue)
    {
        var names = fieldValue.Split(';');
        if (names.Contains(removedInvocationCard.Nom))
        {
            removedInvocationCard.SetCurrentFamily(null);
        }
    }

    private void BeneficiaryActionEffect(InvocationCard removedInvocationCard, InvocationCard invocationCard,
        string value,
        float atk, float def, ref List<int> indexToDelete, int i)
    {
        if (removedInvocationCard.Nom == invocationCard.Nom)
        {
            foreach (var invocationCardToCheck in invocationCards)
            {
                if (invocationCardToCheck.Nom != value) continue;
                var newBonusAttack = invocationCardToCheck.GetBonusAttack() - atk;
                var newBonusDef = invocationCardToCheck.GetBonusDefense() - def;
                invocationCardToCheck.SetBonusAttack(newBonusAttack);
                invocationCardToCheck.SetBonusDefense(newBonusDef);
                indexToDelete.Add(i);
                break;
            }
        }
    }

    private static void IncreaseDefPermEffectRemoved(List<InvocationCard> invocationCardsToChange, string value,
        List<InvocationCard> sameFamilyInvocationCards, ref InvocationCard invocationCard,
        bool mustHaveMiminumAtkdef)
    {
        if (invocationCardsToChange.Count > 0)
        {
            foreach (var invocationCardToChange in invocationCardsToChange)
            {
                var newBonusDefense = invocationCardToChange.GetBonusDefense() - float.Parse(value);
                invocationCardToChange.SetBonusDefense(newBonusDefense);
            }
        }
        else if (sameFamilyInvocationCards.Count > 0)
        {
            var newValue = invocationCard.GetBonusDefense() -
                           float.Parse(value) * sameFamilyInvocationCards.Count;
            invocationCard.SetBonusDefense(newValue);
        }
        else if (mustHaveMiminumAtkdef)
        {
            invocationCard.SetBonusDefense(0);
        }
    }

    private static void IncreaseAtkPermEffectRemoved(List<InvocationCard> invocationCardsToChange, string value,
        List<InvocationCard> sameFamilyInvocationCards, ref InvocationCard invocationCard, bool mustHaveMiminumAtkdef)
    {
        if (invocationCardsToChange.Count > 0)
        {
            foreach (var invocationCardToChange in invocationCardsToChange)
            {
                var newBonusAttack = invocationCardToChange.GetBonusAttack() - float.Parse(value);
                invocationCardToChange.SetBonusAttack(newBonusAttack);
            }
        }
        else if (sameFamilyInvocationCards.Count > 0)
        {
            var newValue = invocationCard.GetBonusAttack() -
                           float.Parse(value) * sameFamilyInvocationCards.Count;
            invocationCard.SetBonusAttack(newValue);
        }
        else if (mustHaveMiminumAtkdef)
        {
            invocationCard.SetBonusAttack(0);
        }
    }

    private static void ConditionPermEffectRemoved(InvocationCard removedInvocationCard, string value,
        List<InvocationCard> sameFamilyInvocationCards, InvocationCard invocationCard, ref bool mustHaveMiminumAtkdef)
    {
        switch (value)
        {
            case "2 ATK 2 DEF":
            {
                for (var k = sameFamilyInvocationCards.Count - 1; k >= 0; k--)
                {
                    var invocationCardToCheck = sameFamilyInvocationCards[k];
                    if (invocationCardToCheck.GetAttack() != 2f ||
                        invocationCardToCheck.GetDefense() != 2f)
                    {
                        sameFamilyInvocationCards.Remove(invocationCardToCheck);
                    }
                }
            }
                break;
            case "Benzaie jeune":
            {
                if (removedInvocationCard.Nom == invocationCard.Nom)
                {
                    mustHaveMiminumAtkdef = true;
                }
            }
                break;
        }
    }

    private void FamilyPermEffectRemoved(InvocationCard removedInvocationCard, string value,
        InvocationCard invocationCard,
        ref List<InvocationCard> sameFamilyInvocationCards)
    {
        if (Enum.TryParse(value, out CardFamily cardFamily))
        {
            if (removedInvocationCard.Nom == invocationCard.Nom)
            {
                // Delete itself so loose all advantage to itself
                foreach (var invocationCardToCheck in invocationCards)
                {
                    if (invocationCardToCheck.Nom != removedInvocationCard.Nom)
                    {
                        if (invocationCardToCheck.GetFamily().Contains(cardFamily))
                        {
                            sameFamilyInvocationCards.Add(invocationCardToCheck);
                        }
                    }
                }
            }
            else if (removedInvocationCard.GetFamily().Contains(cardFamily))
            {
                // Just loose 1 element
                sameFamilyInvocationCards.Add(removedInvocationCard);
            }
        }
    }

    private void GiveStatPermEffectRemoved(InvocationCard removedInvocationCard, string value,
        InvocationCard invocationCard, ref List<InvocationCard> invocationCardsToChange)
    {
        if (Enum.TryParse(value, out CardFamily cardFamily))
        {
            if (removedInvocationCard.Nom == invocationCard.Nom)
            {
                // Delete itself everybody lose advantage
                foreach (var invocationCardToCheck in invocationCards)
                {
                    if (invocationCardToCheck.Nom != removedInvocationCard.Nom)
                    {
                        if (invocationCardToCheck.GetFamily().Contains(cardFamily))
                        {
                            invocationCardsToChange.Add(invocationCardToCheck);
                        }
                    }
                }
            }
        }
    }

    private void OnInvocationCardsChanged()
    {
        for (var j = invocationCards.Count - 1; j >= 0; j--)
        {
            var invocationCard = invocationCards[j];
            var permEffect = invocationCard.InvocationPermEffect;
            if (permEffect == null) continue;
            var keys = permEffect.Keys;
            var values = permEffect.Values;

            for (var i = 0; i < keys.Count; i++)
            {
                switch (keys[i])
                {
                    case PermEffect.checkCardsOnField:
                    {
                        CheckCardOnFieldPermEffect(values[i], invocationCard);
                    }
                        break;
                    case PermEffect.CanOnlyAttackIt:
                    case PermEffect.GiveStat:
                    case PermEffect.IncreaseAtk:
                    case PermEffect.IncreaseDef:
                    case PermEffect.Family:
                    case PermEffect.PreventInvocationCards:
                    case PermEffect.ProtectBehind:
                    case PermEffect.ImpossibleAttackByInvocation:
                    case PermEffect.ImpossibleToBeAffectedByEffect:
                    case PermEffect.Condition:
                    case PermEffect.NumberTurn:
                    default:
                        break;
                }
            }
        }
    }

    private void CheckCardOnFieldPermEffect(string value, InvocationCard invocationCard)
    {
        var isFound = false;
        if (Enum.TryParse(value, out CardFamily cardFamily))
        {
            if (invocationCards
                .Where(otherInvocationCard => otherInvocationCard.Nom != invocationCard.Nom)
                .Any(otherInvocationCard => otherInvocationCard.GetFamily().Contains(cardFamily)))
            {
                isFound = true;
            }
        }
        else
        {
            var cards = value.Split(';');

            if (invocationCards
                .Where(otherInvocationCard => otherInvocationCard.Nom != invocationCard.Nom)
                .Any(otherInvocationCard => cards.Contains(otherInvocationCard.Nom)))
            {
                isFound = true;
            }
        }

        if (!isFound)
        {
            SendInvocationCardToYellowTrash(invocationCard);
        }
    }

    private void OnFieldCardDesactivate(FieldCard oldFieldCard)
    {
        var fieldCardEffect = oldFieldCard.FieldCardEffect;

        var fieldKeys = fieldCardEffect.Keys;
        var fieldValues = fieldCardEffect.Values;

        var family = oldFieldCard.GetFamily();
        for (var i = 0; i < fieldKeys.Count; i++)
        {
            var fieldValue = fieldValues[i];
            switch (fieldKeys[i])
            {
                case FieldEffect.ATK:
                {
                    AtkFieldEffect(fieldValue, family);
                }
                    break;
                case FieldEffect.DEF:
                {
                    DefFieldEffect(fieldValue, family);
                }
                    break;
                case FieldEffect.Change:
                {
                    ChangeFieldEffect(fieldValue);
                }
                    break;
                case FieldEffect.GetCard:
                    break;
                case FieldEffect.DrawCard:
                    break;
                case FieldEffect.Life:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void OnYellowTrashAdded()
    {
        var newYellowTrashCard = yellowTrash.Last();
        var invocationCard = (InvocationCard)newYellowTrashCard;
        if (!invocationCard) return;
        invocationCard.UnblockAttack();
        invocationCard.SetBonusAttack(0);
        invocationCard.SetBonusDefense(0);
        invocationCard.FreeCard();
        invocationCard.ResetNewTurn();
    }

    private void OnFieldCardChanged(FieldCard oldFieldCard)
    {
        if (oldFieldCard == null) return;

        SendInvocationCardToYellowTrashAfterFieldDestruction(oldFieldCard);


        var fieldCardEffect = oldFieldCard.FieldCardEffect;

        var fieldKeys = fieldCardEffect.Keys;
        var fieldValues = fieldCardEffect.Values;

        var family = oldFieldCard.GetFamily();
        for (var i = 0; i < fieldKeys.Count; i++)
        {
            var fieldValue = fieldValues[i];
            switch (fieldKeys[i])
            {
                case FieldEffect.ATK:
                {
                    AtkFieldEffect(fieldValue, family);
                }
                    break;
                case FieldEffect.DEF:
                {
                    DefFieldEffect(fieldValue, family);
                }
                    break;
                case FieldEffect.Change:
                {
                    ChangeFieldEffect(fieldValue);
                }
                    break;
                case FieldEffect.GetCard:
                    break;
                case FieldEffect.DrawCard:
                    break;
                case FieldEffect.Life:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void SendInvocationCardToYellowTrashAfterFieldDestruction(FieldCard oldFieldCard)
    {
        if (oldField.Nom == "Le grenier")
        {
            // Destroy all invocation cards
            for (var i = invocationCards.Count - 1; i >= 0; i--)
            {
                SendInvocationCardToYellowTrash(invocationCards[i]);
            }
        }
        else
        {
            // Picky destruction
            var familyFieldCard = oldFieldCard.GetFamily();
            var familySpecificCard = invocationCards.Where(card => card.GetFamily().Contains(familyFieldCard)).ToList();
            for (var i = familySpecificCard.Count - 1; i >= 0; i--)
            {
                SendInvocationCardToYellowTrash(familySpecificCard[i]);
            }
        }
    }

    private void ChangeFieldEffect(string fieldValue)
    {
        var names = fieldValue.Split(';');
        foreach (var invocationCard in invocationCards.Where(invocationCard =>
                     names.Contains(invocationCard.Nom)))
        {
            invocationCard.SetCurrentFamily(null);
        }
    }

    private void DefFieldEffect(string fieldValue, CardFamily family)
    {
        var def = float.Parse(fieldValue);
        foreach (var invocationCard in invocationCards)
        {
            if (!invocationCard.GetFamily().Contains(family)) continue;
            var newBonusDefense = invocationCard.GetBonusDefense() - def;
            invocationCard.SetBonusDefense(newBonusDefense);
        }
    }

    private void AtkFieldEffect(string fieldValue, CardFamily family)
    {
        var atk = float.Parse(fieldValue);
        foreach (var invocationCard in invocationCards)
        {
            if (!invocationCard.GetFamily().Contains(family)) continue;
            var newBonusAttack = invocationCard.GetBonusAttack() - atk;
            invocationCard.SetBonusAttack(newBonusAttack);
        }
    }
}