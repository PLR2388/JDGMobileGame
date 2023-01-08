using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using Cards.InvocationCards;
using UnityEngine;

public class PlayerCards : MonoBehaviour
{
    public List<InGameCard> deck = new List<InGameCard>();
    public List<InGameCard> handCards = new List<InGameCard>();
    public List<InGameCard> yellowTrash = new List<InGameCard>();
    public InGameFieldCard field;
    public List<InGameInvocationCard> invocationCards = new List<InGameInvocationCard>();
    public List<InGameEffectCard> effectCards = new List<InGameEffectCard>();

    [SerializeField] private bool isPlayerOne;

    [SerializeField] private GameObject nextPhaseButton;
    [SerializeField] private GameObject inHandButton;
    [SerializeField] private Transform canvas;
    [SerializeField] private CardLocation cardLocation;


    public List<InGameCard> secretCards = new List<InGameCard>(); // Where combine card go


    public string Tag => isPlayerOne ? "card1" : "card2";

    private List<InGameInvocationCard> oldInvocationCards = new List<InGameInvocationCard>();
    private InGameFieldCard oldField;
    private List<InGameCard> oldHandCards = new List<InGameCard>();
    private List<InGameCard> oldYellowTrash = new List<InGameCard>();

    public bool IsFieldDesactivate { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        invocationCards = new List<InGameInvocationCard>(4);
        effectCards = new List<InGameEffectCard>(4);
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
                     invocationCard != null && invocationCard.Title != null))
        {
            invocationCard.ResetNewTurn();
        }
    }

    public bool ContainsCardInInvocation(InGameInvocationCard invocationCard)
    {
        return invocationCards.Any(invocation => invocation != null && invocation.Title == invocationCard.Title);
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

            oldInvocationCards = new List<InGameInvocationCard>(invocationCards);
            OnInvocationCardsChanged();
        }

        if (field != oldField)
        {
            for (var i = effectCards.Count - 1; i >= 0; i--)
            {
                var effectCard = effectCards[i];
                var effectCardEffect = effectCard.EffectCardEffect;
                if (effectCardEffect == null) continue;
                if (!effectCardEffect.Keys.Contains(Effect.SameFamily)) continue;
                if (field != null && !IsFieldDesactivate)
                {
                    foreach (var invocationCard in invocationCards)
                    {
                        invocationCard.Families = new[]
                        {
                            field.GetFamily()
                        };
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
                if (invocationCard.EquipmentCard == null ||
                    invocationCard.EquipmentCard.EquipmentPermEffect == null) continue;
                var permEffect = invocationCard.EquipmentCard.EquipmentPermEffect;
                if (permEffect.Keys.Contains(PermanentEffect.AddAtkBaseOnHandCards))
                {
                    var value = float.Parse(permEffect.Values[
                        permEffect.Keys.FindIndex(key => key == PermanentEffect.AddAtkBaseOnHandCards)]);
                    invocationCard.Attack += (handCards.Count - oldHandCards.Count) * value;
                }
                else if (permEffect.Keys.Contains(PermanentEffect.AddDefBaseOnHandCards))
                {
                    var value = float.Parse(permEffect.Values[
                        permEffect.Keys.FindIndex(key => key == PermanentEffect.AddDefBaseOnHandCards)]);
                    invocationCard.Defense += (handCards.Count - oldHandCards.Count) * value;
                }
            }

            oldHandCards = new List<InGameCard>(handCards);
        }

        if (oldYellowTrash.Count != yellowTrash.Count)
        {
            if (yellowTrash.Count > oldYellowTrash.Count)
            {
                OnYellowTrashAdded();
            }

            oldYellowTrash = new List<InGameCard>(yellowTrash);
        }
    }

    public void SendToSecretHide(InGameCard card)
    {
        secretCards.Add(card);
    }

    public void RemoveFromSecretHide(InGameCard card)
    {
        secretCards.Remove(card);
    }

    public void SendCardToYellowTrash(InGameCard card)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i].Title != card.Title) continue;
            var invocationCard = card as InGameInvocationCard;
            if (invocationCard == null) continue;
            if (invocationCard is InGameSuperInvocationCard superInvocationCard)
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
                if (invocationCard.EquipmentCard != null)
                {
                    var equipmentCard = invocationCard.EquipmentCard;
                    yellowTrash.Add(equipmentCard);
                    invocationCard.SetEquipmentCard(null);
                }

                invocationCards.Remove((InGameInvocationCard)card);
                yellowTrash.Add(card);
            }
        }

        for (var i = 0; i < effectCards.Count; i++)
        {
            if (effectCards[i].Title == card.Title)
            {
                effectCards.Remove(card as InGameEffectCard);
                yellowTrash.Add(card);
            }
        }

        if (field != null && field.Title == card.Title)
        {
            field = null;
            yellowTrash.Add(card);
        }
    }

    public void SendInvocationCardToYellowTrash(InGameInvocationCard specificCardFound)
    {
        var equipmentCard = specificCardFound.EquipmentCard;
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

            if (specificCardFound.InvocationDeathEffect == null) return;
            var invocationDeathEffect = specificCardFound.InvocationDeathEffect;
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

            if (specificCardFound.InvocationDeathEffect == null) return;
            var invocationDeathEffect = specificCardFound.InvocationDeathEffect;
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
        specificCardFound.Reset();
    }

    public void RemoveSuperInvocation(InGameCard superInvocationCard)
    {
        invocationCards.Remove(superInvocationCard as InGameInvocationCard);
        cardLocation.RemovePhysicalCard(superInvocationCard);
    }

    private void ComeBackToHandDeathEffect(InGameInvocationCard invocationCard, string value)
    {
        var isParsed = int.TryParse(value, out var number);
        if (isParsed)
        {
            if (invocationCard.NumberOfDeaths > number)
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

    private void AskUserToAddCardInHand(string cardName, InGameCard cardFound, bool isFound)
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

    private void GetCardSourceDeathEffect(InGameCard invocationCard, string source,
        string cardName)
    {
        InGameCard cardFound = null;

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
                        if (deck[j].Title == cardName)
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
                        if (trash[j].Title == cardName)
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

    public void SendCardToHand(InGameCard card)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i].Title == card.Title)
            {
                invocationCards.Remove(card as InGameInvocationCard);
            }
        }

        handCards.Add(card);
    }

    public int GetIndexInvocationCard(string nameCard)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i].Title == nameCard)
            {
                return i;
            }
        }

        return -1;
    }

    private void OnInvocationCardAdded(InGameInvocationCard newInvocationCard)
    {
        var opponentEffectCards = isPlayerOne
            ? GameObject.Find("Player2").GetComponent<PlayerCards>().effectCards
            : GameObject.Find("Player1").GetComponent<PlayerCards>().effectCards;

        var mustSkipAttack = opponentEffectCards.Select(effectCard => effectCard.EffectCardEffect.Keys)
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

            var invocationCardsToChange = new List<InGameInvocationCard>();
            var sameFamilyInvocationCards = new List<InGameInvocationCard>();
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
            var effectCardEffect = effectCard.EffectCardEffect;
            if (effectCardEffect != null)
            {
                if (effectCardEffect.Keys.Contains(Effect.SameFamily))
                {
                    if (field != null && !IsFieldDesactivate)
                    {
                        newInvocationCard.Families = new[]
                        {
                            field.GetFamily()
                        };
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

    private static void ChangeFieldEffect(ref InGameInvocationCard newInvocationCard, string fieldValue,
        CardFamily family)
    {
        var names = fieldValue.Split(';');
        if (names.Contains(newInvocationCard.Title))
        {
            newInvocationCard.Families = new[]
            {
                family
            };
        }
    }

    private static void DEFFieldEffect(ref InGameInvocationCard newInvocationCard, string fieldValue, CardFamily family)
    {
        var def = float.Parse(fieldValue);
        if (newInvocationCard.Families.Contains(family))
        {
            newInvocationCard.Defense += def;
        }
    }

    private static void ATKFieldEffect(ref InGameInvocationCard newInvocationCard, string fieldValue, CardFamily family)
    {
        var atk = float.Parse(fieldValue);
        if (newInvocationCard.Families.Contains(family))
        {
            newInvocationCard.Attack += atk;
        }
    }

    private static void IncreaseDefPermEffect(List<InGameInvocationCard> invocationCardsToChange, string value,
        List<InGameInvocationCard> sameFamilyInvocationCards, ref InGameInvocationCard invocationCard,
        bool mustHaveMinimumUndef)
    {
        if (invocationCardsToChange.Count > 0)
        {
            foreach (var invocationCardToChange in invocationCardsToChange)
            {
                invocationCardToChange.Defense += float.Parse(value);
            }
        }
        else if (sameFamilyInvocationCards.Count > 0)
        {
            invocationCard.Defense += float.Parse(value) * sameFamilyInvocationCards.Count;
        }
        else if (mustHaveMinimumUndef)
        {
            var minValue = float.Parse(value);
            if (invocationCard.GetCurrentDefense() < minValue)
            {
                invocationCard.Defense = minValue;
            }
        }
    }

    private static void IncreaseAtkPermEffect(List<InGameInvocationCard> invocationCardsToChange, string value,
        List<InGameInvocationCard> sameFamilyInvocationCards, ref InGameInvocationCard invocationCard,
        bool mustHaveMinimumUndef)
    {
        if (invocationCardsToChange.Count > 0)
        {
            foreach (var invocationCardToChange in invocationCardsToChange)
            {
                invocationCardToChange.Attack += float.Parse(value);
            }
        }
        else if (sameFamilyInvocationCards.Count > 0)
        {
            invocationCard.Attack += float.Parse(value) * sameFamilyInvocationCards.Count;
        }
        else if (mustHaveMinimumUndef)
        {
            // TODO : Set same value for this card than benzaie ATK And Def
            var minValue = float.Parse(value);
            if (invocationCard.GetCurrentAttack() < minValue)
            {
                invocationCard.Attack = minValue;
            }
        }
    }

    private void ConditionPermEffect(string value, ref List<InGameInvocationCard> sameFamilyInvocationCards,
        ref bool mustHaveMinimumUndef)
    {
        switch (value)
        {
            case "2 ATK 2 DEF":
            {
                for (var k = sameFamilyInvocationCards.Count - 1; k >= 0; k--)
                {
                    var invocationCardToCheck = sameFamilyInvocationCards[k];
                    if (invocationCardToCheck.Attack != 2f ||
                        invocationCardToCheck.Defense != 2f)
                    {
                        sameFamilyInvocationCards.Remove(invocationCardToCheck);
                    }
                }
            }
                break;
            case "Benzaie jeune":
            {
                if (invocationCards.Any(invocationCardToCheck =>
                        invocationCardToCheck.Title == value))
                {
                    mustHaveMinimumUndef = true;
                }
            }
                break;
        }
    }

    private void FamilyPermEffect(InGameInvocationCard newInvocationCard, string value,
        InGameInvocationCard invocationCard,
        ref List<InGameInvocationCard> sameFamilyInvocationCards)
    {
        if (Enum.TryParse(value, out CardFamily cardFamily))
        {
            if (newInvocationCard.Title == invocationCard.Title)
            {
                // Must catch up old cards
                sameFamilyInvocationCards.AddRange(invocationCards
                    .Where(invocationCardToCheck => invocationCardToCheck.Title != newInvocationCard.Title)
                    .Where(invocationCardToCheck =>
                        invocationCardToCheck.Families.Contains(cardFamily)));
            }
            else if (newInvocationCard.Families.Contains(cardFamily))
            {
                sameFamilyInvocationCards.Add(newInvocationCard);
            }
        }
    }

    private void GiveStatPermEffect(InGameInvocationCard newInvocationCard, string value,
        InGameInvocationCard invocationCard,
        ref List<InGameInvocationCard> invocationCardsToChange)
    {
        if (Enum.TryParse(value, out CardFamily cardFamily))
        {
            if (newInvocationCard.Title == invocationCard.Title)
            {
                // Must catch up old cards
                invocationCardsToChange.AddRange(invocationCards
                    .Where(invocationCardToCheck => invocationCardToCheck.Title != newInvocationCard.Title)
                    .Where(invocationCardToCheck =>
                        invocationCardToCheck.Families.Contains(cardFamily)));
            }
            else if (newInvocationCard.Families.Contains(cardFamily))
            {
                invocationCardsToChange.Add(newInvocationCard);
            }
        }
    }

    private void OnInvocationCardsRemoved(InGameInvocationCard removedInvocationCard)
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

                var invocationCardsToChange = new List<InGameInvocationCard>();
                var sameFamilyInvocationCards = new List<InGameInvocationCard>();

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
            var effectCardEffect = effectCard.EffectCardEffect;
            if (effectCardEffect != null)
            {
                if (effectCardEffect.Keys.Contains(Effect.SameFamily))
                {
                    if (field != null)
                    {
                        removedInvocationCard.Families = removedInvocationCard.baseInvocationCard.Family;
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

    private static void ChangeFieldEffect(ref InGameInvocationCard removedInvocationCard, string fieldValue)
    {
        var names = fieldValue.Split(';');
        if (names.Contains(removedInvocationCard.Title))
        {
            removedInvocationCard.Families = removedInvocationCard.baseInvocationCard.Family;
        }
    }

    private void BeneficiaryActionEffect(InGameInvocationCard removedInvocationCard,
        InGameInvocationCard invocationCard,
        string value,
        float atk, float def, ref List<int> indexToDelete, int i)
    {
        if (removedInvocationCard.Title == invocationCard.Title)
        {
            foreach (var invocationCardToCheck in invocationCards)
            {
                if (invocationCardToCheck.Title != value) continue;
                invocationCardToCheck.Attack -= atk;
                invocationCardToCheck.Defense -= def;
                indexToDelete.Add(i);
                break;
            }
        }
    }

    private static void IncreaseDefPermEffectRemoved(List<InGameInvocationCard> invocationCardsToChange, string value,
        List<InGameInvocationCard> sameFamilyInvocationCards, ref InGameInvocationCard invocationCard,
        bool mustHaveMiminumAtkdef)
    {
        if (invocationCardsToChange.Count > 0)
        {
            foreach (var invocationCardToChange in invocationCardsToChange)
            {
                invocationCardToChange.Defense -= float.Parse(value);
            }
        }
        else if (sameFamilyInvocationCards.Count > 0)
        {
            invocationCard.Defense -= float.Parse(value) * sameFamilyInvocationCards.Count;
        }
        else if (mustHaveMiminumAtkdef)
        {
            invocationCard.Defense = invocationCard.baseInvocationCard.GetDefense();
        }
    }

    private static void IncreaseAtkPermEffectRemoved(List<InGameInvocationCard> invocationCardsToChange, string value,
        List<InGameInvocationCard> sameFamilyInvocationCards, ref InGameInvocationCard invocationCard,
        bool mustHaveMiminumAtkdef)
    {
        if (invocationCardsToChange.Count > 0)
        {
            foreach (var invocationCardToChange in invocationCardsToChange)
            {
                invocationCardToChange.Attack -= float.Parse(value);
            }
        }
        else if (sameFamilyInvocationCards.Count > 0)
        {
            invocationCard.Attack -= float.Parse(value) * sameFamilyInvocationCards.Count;
        }
        else if (mustHaveMiminumAtkdef)
        {
            invocationCard.Attack = invocationCard.baseInvocationCard.GetAttack();
        }
    }

    private static void ConditionPermEffectRemoved(InGameInvocationCard removedInvocationCard, string value,
        List<InGameInvocationCard> sameFamilyInvocationCards, InGameInvocationCard invocationCard,
        ref bool mustHaveMiminumAtkdef)
    {
        switch (value)
        {
            case "2 ATK 2 DEF":
            {
                for (var k = sameFamilyInvocationCards.Count - 1; k >= 0; k--)
                {
                    var invocationCardToCheck = sameFamilyInvocationCards[k];
                    if (invocationCardToCheck.Attack != 2f ||
                        invocationCardToCheck.Defense != 2f)
                    {
                        sameFamilyInvocationCards.Remove(invocationCardToCheck);
                    }
                }
            }
                break;
            case "Benzaie jeune":
            {
                if (removedInvocationCard.Title == invocationCard.Title)
                {
                    mustHaveMiminumAtkdef = true;
                }
            }
                break;
        }
    }

    private void FamilyPermEffectRemoved(InGameInvocationCard removedInvocationCard, string value,
        InGameInvocationCard invocationCard,
        ref List<InGameInvocationCard> sameFamilyInvocationCards)
    {
        if (Enum.TryParse(value, out CardFamily cardFamily))
        {
            if (removedInvocationCard.Title == invocationCard.Title)
            {
                // Delete itself so loose all advantage to itself
                foreach (var invocationCardToCheck in invocationCards)
                {
                    if (invocationCardToCheck.Title != removedInvocationCard.Title)
                    {
                        if (invocationCardToCheck.Families.Contains(cardFamily))
                        {
                            sameFamilyInvocationCards.Add(invocationCardToCheck);
                        }
                    }
                }
            }
            else if (removedInvocationCard.Families.Contains(cardFamily))
            {
                // Just loose 1 element
                sameFamilyInvocationCards.Add(removedInvocationCard);
            }
        }
    }

    private void GiveStatPermEffectRemoved(InGameInvocationCard removedInvocationCard, string value,
        InGameInvocationCard invocationCard, ref List<InGameInvocationCard> invocationCardsToChange)
    {
        if (Enum.TryParse(value, out CardFamily cardFamily))
        {
            if (removedInvocationCard.Title == invocationCard.Title)
            {
                // Delete itself everybody lose advantage
                foreach (var invocationCardToCheck in invocationCards)
                {
                    if (invocationCardToCheck.Title != removedInvocationCard.Title)
                    {
                        if (invocationCardToCheck.Families.Contains(cardFamily))
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

    private void CheckCardOnFieldPermEffect(string value, InGameInvocationCard invocationCard)
    {
        var isFound = false;
        if (Enum.TryParse(value, out CardFamily cardFamily))
        {
            if (invocationCards
                .Where(otherInvocationCard => otherInvocationCard.Title != invocationCard.Title)
                .Any(otherInvocationCard => otherInvocationCard.Families.Contains(cardFamily)))
            {
                isFound = true;
            }
        }
        else
        {
            var cards = value.Split(';');

            if (invocationCards
                .Where(otherInvocationCard => otherInvocationCard.Title != invocationCard.Title)
                .Any(otherInvocationCard => cards.Contains(otherInvocationCard.Title)))
            {
                isFound = true;
            }
        }

        if (!isFound)
        {
            SendInvocationCardToYellowTrash(invocationCard);
        }
    }

    private void OnFieldCardDesactivate(InGameFieldCard oldFieldCard)
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
        var invocationCard = newYellowTrashCard as InGameInvocationCard;
        if (invocationCard != null)
        {
            invocationCard.UnblockAttack();
            invocationCard.Attack = invocationCard.baseInvocationCard.GetAttack();
            invocationCard.Defense = invocationCard.baseInvocationCard.GetDefense();
            invocationCard.FreeCard();
            invocationCard.ResetNewTurn(); 
        }
    }

    private void OnFieldCardChanged(InGameFieldCard oldFieldCard)
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

    private void SendInvocationCardToYellowTrashAfterFieldDestruction(InGameFieldCard oldFieldCard)
    {
        if (oldField.Title == "Le grenier")
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
            var familySpecificCard = invocationCards.Where(card => card.Families.Contains(familyFieldCard)).ToList();
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
                     names.Contains(invocationCard.Title)))
        {
            invocationCard.Families = invocationCard.baseInvocationCard.Family;
        }
    }

    private void DefFieldEffect(string fieldValue, CardFamily family)
    {
        var def = float.Parse(fieldValue);
        foreach (var invocationCard in invocationCards)
        {
            if (!invocationCard.Families.Contains(family)) continue;
            invocationCard.Defense -= def;
        }
    }

    private void AtkFieldEffect(string fieldValue, CardFamily family)
    {
        var atk = float.Parse(fieldValue);
        foreach (var invocationCard in invocationCards)
        {
            if (!invocationCard.Families.Contains(family)) continue;
            invocationCard.Attack -= atk;
        }
    }
}