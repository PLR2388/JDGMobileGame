using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
    public static bool isP1Turn;

    public int phaseId;

    [SerializeField] private GameObject playerText;
    [SerializeField] private GameObject roundText;
    [SerializeField] private TextMeshProUGUI healthP1Text;
    [SerializeField] private TextMeshProUGUI healthP2Text;
    
    [SerializeField] private GameObject bigImageCard;
    [SerializeField] private GameObject invocationMenu;
    [SerializeField] private GameObject messageBox;
    [SerializeField] private GameObject P1;
    [SerializeField] private GameObject P2;
    [SerializeField] private Card player;
    [SerializeField] private GameObject inHandButton;
    
    public static UnityEvent ChangePlayer = new UnityEvent();
    
    
    private float ClickDuration = 2;

    private bool stopDetectClicking = false;
    bool clicking = false;
    float totalDownTime = 0;
    private Card cardSelected;
    private InvocationCard attacker;

    // Start is called before the first frame update
    void Start()
    {
        isP1Turn = true;
        PlayerStatus.ChangePvEvent.AddListener(changeHealthText);
    }

    // Update is called once per frame
    void Update()
    {
        switch (phaseId)
        {
            case 0: Draw();
                break;
            case 1:
                ChoosePhase();
                break;
            case 2:
                ChooseAttack();
                break;
        }
    }

    private void ChoosePhase()
    {
        if (isP1Turn)
        {
            invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
            P1.GetComponent<PlayerCards>().resetInvocationCardNewTurn();
        }
        else
        {
            invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
            P2.GetComponent<PlayerCards>().resetInvocationCardNewTurn();
        }
    }

    private void changeHealthText(float pv, bool isP1)
    {
        if (isP1)
        {
            healthP1Text.SetText(pv+"/"+ PlayerStatus.maxPV);
        }
        else
        {
            healthP2Text.SetText(pv+"/"+ PlayerStatus.maxPV);
        }
    }

    private void ChooseAttack()
    {
        if (Input.GetMouseButton(0) && phaseId == 2 && !stopDetectClicking)
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit) 
            {
                HandleClick(hitInfo);
            } else {
                bigImageCard.SetActive(false);
            }
        } 
    }

    private void HandleClick(RaycastHit hitInfo)
    {
        string tag = hitInfo.transform.gameObject.tag;
        string persoTag = isP1Turn ? P1.GetComponent<PlayerCards>().TAG : P2.GetComponent<PlayerCards>().TAG;
        string opponentTag = isP1Turn ? P2.GetComponent<PlayerCards>().TAG : P1.GetComponent<PlayerCards>().TAG;
        GameObject cardObject = hitInfo.transform.gameObject;
        
        if (tag == persoTag)
        {
            cardSelected = cardObject.GetComponent<PhysicalCardDisplay>().card;
            if (Input.GetMouseButtonDown(0))
            {
                totalDownTime = 0;
                clicking = true;
                Vector3 mousePosition = Input.mousePosition;
                if (cardSelected is InvocationCard)
                {
                    attacker = (InvocationCard) cardSelected;
                    invocationMenu.SetActive(true);
                    invocationMenu.transform.position = mousePosition;
                    if (!attacker.hasAttack())
                    {
                        invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
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
        else if (tag == opponentTag)
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
        if (isP1Turn)
        {
            InvocationCard[] opponentCards = P2.GetComponent<PlayerCards>().InvocationCards;
            DisplayCards(opponentCards);
        }
        else
        {
            InvocationCard[] opponentCards = P1.GetComponent<PlayerCards>().InvocationCards;
            DisplayCards(opponentCards);
        }
    }

    private void DisplayCards(InvocationCard[] invocationCards)
    {
        List<Card> notEmptyOpponent = new List<Card>();
        for (int i = 0; i < invocationCards.Length; i++)
        {
            if (invocationCards[i] != null && invocationCards[i].Nom != null)
            {
                notEmptyOpponent.Add(invocationCards[i]);
            }
        }

        if (notEmptyOpponent.Count == 0)
        {
            notEmptyOpponent.Add(player);
        }

        GameObject messageBox = DisplayOpponentMessageBox(notEmptyOpponent);
        stopDetectClicking = true;
        messageBox.GetComponent<MessageBox>().positiveAction = () =>
        {
            InvocationCard invocationCard =
                (InvocationCard) messageBox.GetComponent<MessageBox>().getSelectedCard();

            if (invocationCard != null)
            {
                ComputeAttack(invocationCard);
            }

            stopDetectClicking = false;
            Destroy(messageBox);
        };
        messageBox.GetComponent<MessageBox>().negativeAction = () =>
        {
            Destroy(messageBox);
            stopDetectClicking = false;
        };
    }

    private GameObject DisplayOpponentMessageBox ( List<Card> invocationCards)
    {
        GameObject message = Instantiate(messageBox);
        message.GetComponent<MessageBox>().title = "Choisis ton adversaire :";

        
        message.GetComponent<MessageBox>().displayCardsScript.cardslist = invocationCards;
        message.GetComponent<MessageBox>().displayCards = true;

        return message;
    }

    private void ComputeAttack(InvocationCard opponent)
    {
        float attack = attacker.GetCurrentAttack();
        float defenseOpponent = opponent.GetCurrentDefense();
        
        float diff = defenseOpponent - attack;
        
        attacker.attackTurnDone();
        invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = false;

        if (opponent.Nom == "Player")
        {
            // Directly attack the player
            if (isP1Turn)
            {
                P2.GetComponent<PlayerStatus>().changePV(diff);
            }
            else
            {
                P1.GetComponent<PlayerStatus>().changePV(diff);
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
    }

    /**
     * Attack that kill the opponent
     */
    private void DealWithGoodAttack(InvocationCard opponent, float diff)
    {
        opponent.incrementNumberDeaths();
        if (isP1Turn)
        {
            P2.GetComponent<PlayerStatus>().changePV(diff);

            if (opponent.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffet(opponent, false);
            }
            else
            {
                P2.GetComponent<PlayerCards>().sendCardToYellowTrash(opponent);
            }
        }
        else
        {
            P1.GetComponent<PlayerStatus>().changePV(diff);
            if (opponent.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffet(opponent, true);
            }
            else
            {
                P1.GetComponent<PlayerCards>().sendCardToYellowTrash(opponent);
            }
        }
    }

    /**
     * Attack that kill the attacker
     */
    private void DealWithHurtAttack(float diff)
    {
        attacker.incrementNumberDeaths();
        if (isP1Turn)
        {
            P1.GetComponent<PlayerStatus>().changePV(-diff);
            if (attacker.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffet(attacker, true);
            }
            else
            {
                P1.GetComponent<PlayerCards>().sendCardToYellowTrash(cardSelected);
            }
        }
        else
        {
            P2.GetComponent<PlayerStatus>().changePV(-diff);
            if (attacker.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffet(attacker, false);
            }
            else
            {
                P2.GetComponent<PlayerCards>().sendCardToYellowTrash(cardSelected);
            }
        }
    }

    private void DealWithEqualityAttack(InvocationCard opponent)
    {
        attacker.incrementNumberDeaths();
        opponent.incrementNumberDeaths();
        if (isP1Turn)
        {
            if (attacker.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffet(attacker, true);
            }
            else
            {
                P1.GetComponent<PlayerCards>().sendCardToYellowTrash(attacker);
            }

            if (opponent.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffet(opponent, false);
            }
            else
            {
                P2.GetComponent<PlayerCards>().sendCardToYellowTrash(opponent);
            }
        }
        else
        {
            if (attacker.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffet(attacker, false);
            }
            else
            {
                P2.GetComponent<PlayerCards>().sendCardToYellowTrash(attacker);
            }

            if (opponent.GetInvocationDeathEffect() != null)
            {
                DealWithDeathEffet(opponent, true);
            }
            else
            {
                P1.GetComponent<PlayerCards>().sendCardToYellowTrash(opponent);
            }
        }
    }

    /**
     * invocationCard = card that's going to die
     * attacker = the opponent
     */
    private void DealWithDeathEffet(InvocationCard invocationCard,bool isP1Card, InvocationCard attacker = null)
    {
        InvocationDeathEffect invocationDeathEffect = invocationCard.GetInvocationDeathEffect();
        List<DeathEffect> keys = invocationDeathEffect.Keys;
        List<String> values = invocationDeathEffect.Values;

        string cardName = "";
        for (int i = 0; i < keys.Count; i++)
        {
            switch (keys[i])
            {
                case DeathEffect.GetSpecificCard:
                    cardName = values[i];
                    break;
                case DeathEffect.GetCardSource:
                    GetCardSourceDeathEffect(invocationCard, isP1Card, values, i, cardName);
                    break;
                case DeathEffect.ComeBackToHand:
                    ComeBackToHandDeathEffect(invocationCard, isP1Card, values, i);
                    break;
                case DeathEffect.KillAlsoOtherCard:
                    KillAlsoOtherCardDeathEffect(invocationCard, attacker);
                    break;
            }
        }
    }

    private void KillAlsoOtherCardDeathEffect(InvocationCard invocationCard, InvocationCard attacker)
    {
        if (attacker != null)
        {
            if (isP1Turn)
            {
                P1.GetComponent<PlayerCards>().sendCardToYellowTrash(attacker);
                P2.GetComponent<PlayerCards>().sendCardToYellowTrash(invocationCard);
            }
            else
            {
                P1.GetComponent<PlayerCards>().sendCardToYellowTrash(invocationCard);
                P2.GetComponent<PlayerCards>().sendCardToYellowTrash(attacker);
            }
        }
    }

    private void ComeBackToHandDeathEffect(InvocationCard invocationCard, bool isP1Card, List<string> values, int i)
    {
        int number = int.Parse(values[i]);
        if (number != 0)
        {
            if (invocationCard.getNumberDeaths() > number)
            {
                if (isP1Card)
                {
                    P1.GetComponent<PlayerCards>().sendCardToYellowTrash(invocationCard);
                }
                else
                {
                    P2.GetComponent<PlayerCards>().sendCardToYellowTrash(invocationCard);
                }
            }
            else
            {
                if (isP1Card)
                {
                    P1.GetComponent<PlayerCards>().sendCardToHand(invocationCard);
                }
                else
                {
                    P2.GetComponent<PlayerCards>().sendCardToHand(invocationCard);
                }
            }
        }
        else
        {
            if (isP1Card)
            {
                P1.GetComponent<PlayerCards>().sendCardToHand(invocationCard);
            }
            else
            {
                P2.GetComponent<PlayerCards>().sendCardToHand(invocationCard);
            }
        }
    }

    private void GetCardSourceDeathEffect(InvocationCard invocationCard, bool isP1Card, List<string> values, int i, string cardName)
    {
        Card cardFound = null;
        String source = values[i];
        PlayerCards currentPlayerCard = null;
        if (isP1Card)
        {
            currentPlayerCard = P1.GetComponent<PlayerCards>();
        }
        else
        {
            currentPlayerCard = P2.GetComponent<PlayerCards>();
        }

        currentPlayerCard.sendCardToYellowTrash(invocationCard);
        if (source == "deck")
        {
            List<Card> deck = currentPlayerCard.Deck;
            if (cardName != "")
            {
                bool isFound = false;
                int j = 0;
                while (j < deck.Count && !isFound)
                {
                    if (deck[j].Nom == cardName)
                    {
                        isFound = true;
                        cardFound = deck[j];
                    }

                    j++;
                }

                AskUserToAddCardInHand(cardName, cardFound, isFound, currentPlayerCard);
            }
        }
        else if (source == "trash")
        {
            List<Card> trash = currentPlayerCard.YellowTrash;
            if (cardName != "")
            {
                bool isFound = false;
                int j = 0;
                while (j < trash.Count && !isFound)
                {
                    if (trash[j].Nom == cardName)
                    {
                        isFound = true;
                        cardFound = trash[j];
                    }

                    j++;
                }
                
                AskUserToAddCardInHand(cardName, cardFound, isFound, currentPlayerCard);
            }
        }
    }

    private void AskUserToAddCardInHand(string cardName, Card cardFound, bool isFound, PlayerCards currentPlayerCard)
    {
        if (isFound)
        {
            GameObject message = Instantiate(messageBox);
            message.GetComponent<MessageBox>().title = "Carte en main";

            message.GetComponent<MessageBox>().description =
                "Voulez-vous aussi ajouter " + cardName + " à votre main ?";
            message.GetComponent<MessageBox>().positiveAction = () =>
            {
                currentPlayerCard.handCards.Add(cardFound);

                currentPlayerCard.Deck.Remove(cardFound);

                Destroy(message);
            };
            message.GetComponent<MessageBox>().negativeAction = () => { Destroy(message); };
        }
    }

    private void DisplayCurrentCard(Card card)
    {
        bigImageCard.SetActive(true);
        bigImageCard.GetComponent<Image>().material = card.MaterialCard;
    }

    private void Draw()
    {
        if (isP1Turn)
        {
            PlayerCards P1Cards = P1.GetComponent<PlayerCards>();
            int size = P1Cards.Deck.Count;
            if (size > 0)
            {
                Card c = P1Cards.Deck[size-1];
                P1Cards.handCards.Add(c);
                P1Cards.Deck.RemoveAt(size-1);
            }

        }
        else
        {
            PlayerCards P2Cards = P2.GetComponent<PlayerCards>();
            int size = P2Cards.Deck.Count;
            if (size > 0)
            {
                Card c = P2Cards.Deck[size-1];
                P2Cards.handCards.Add(c);
                P2Cards.Deck.RemoveAt(size-1);
            }      
        }

        phaseId += 1;
        roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pose";
    }

    public void NextRound()
    {
        invocationMenu.SetActive(false);
        phaseId += 1;
        switch (phaseId)
        {
            case 3:
                inHandButton.SetActive(true);
                roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pioche";break;
            case 1:
                inHandButton.SetActive(true);
                roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pose";break;
            case 2:
                inHandButton.SetActive(false);
                roundText.GetComponent<TextMeshProUGUI>().text = "Phase d'attaque";break;
        }
    
        if (phaseId == 3)
        {
            isP1Turn = !isP1Turn;
            ChangePlayer.Invoke();
            if (isP1Turn)
            {
                playerText.GetComponent<TextMeshProUGUI>().text = "Joueur 1";
            }
            else
            {
                playerText.GetComponent<TextMeshProUGUI>().text = "Joueur 2";
            }
            phaseId = 0;
        }
    }
    
}