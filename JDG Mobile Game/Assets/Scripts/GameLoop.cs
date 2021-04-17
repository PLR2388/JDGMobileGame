using System;
using System.Collections;
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
            case 0: draw();
                break;
            case 1:
                choosePhase();
                break;
            case 2:
                chooseAttack();
                break;
        }
    }

    private void choosePhase()
    {
        if (isP1Turn)
        {
            invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
            P1.GetComponent<PlayerCards>().resetInvocationCardNewTurn();
        }
        else
        {
            invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
            P1.GetComponent<PlayerCards>().resetInvocationCardNewTurn();
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

    private void chooseAttack()
    {
        if (Input.GetMouseButton(0) && phaseId == 2)
        {
            Debug.Log("Mouse is down");
         
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit) 
            {
                Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                if (isP1Turn)
                {
                    String tag = hitInfo.transform.gameObject.tag;
                    if (tag == "card1")
                    {
                        GameObject cardObject = hitInfo.transform.gameObject;
                        cardSelected = cardObject.GetComponent<PhysicalCardDisplay>().card;
                        if (Input.GetMouseButtonDown(0))
                        {
                            totalDownTime = 0;
                            clicking = true;
                            Vector3 mousePosition = Input.mousePosition;
                            if (cardSelected is InvocationCard)
                            {
                                attacker = (InvocationCard) cardSelected;
                                if (!attacker.hasAttack())
                                {
                                    invocationMenu.SetActive(true);
                                    invocationMenu.transform.position = mousePosition;
                                }
                            }
                        }

                        // If a first click detected, and still clicking,
                        // measure the total click time, and fire an event
                        // if we exceed the duration specified
                        if (clicking && Input.GetMouseButton(0))
                        {
                            totalDownTime += Time.deltaTime;

                            if (totalDownTime >= ClickDuration)
                            {
                                Debug.Log("Long click");
                                clicking = false;
                                DisplayCurrentCard(cardObject.GetComponent<PhysicalCardDisplay>().card);
                            }
                        }

                        // If a first click detected, and we release before the
                        // duraction, do nothing, just cancel the click
                        if (clicking && Input.GetMouseButtonUp(0))
                        {
                            clicking = false;
                        }
                    } else if (tag == "card2")
                    {
                        GameObject cardObject = hitInfo.transform.gameObject;
                        if (Input.GetMouseButtonDown(0))
                        {
                            totalDownTime = 0;
                            clicking = true;
                        }

                        // If a first click detected, and still clicking,
                        // measure the total click time, and fire an event
                        // if we exceed the duration specified
                        if (clicking && Input.GetMouseButton(0))
                        {
                            totalDownTime += Time.deltaTime;

                            if (totalDownTime >= ClickDuration)
                            {
                                Debug.Log("Long click");
                                clicking = false;
                                DisplayCurrentCard(cardObject.GetComponent<PhysicalCardDisplay>().card);
                            }
                        }

                        // If a first click detected, and we release before the
                        // duraction, do nothing, just cancel the click
                        if (clicking && Input.GetMouseButtonUp(0))
                        {
                            clicking = false;
                        }
                    }
                    else {
                        bigImageCard.SetActive(false);
                    }
                }
                else
                {
                    if (hitInfo.transform.gameObject.tag == "card2")
                    {
                        Debug.Log ("It's working!");
                    } else {
                        Debug.Log ("nopz");
                    }
                }
        
            } else {
                bigImageCard.SetActive(false);
                Debug.Log("No hit");
            }
            Debug.Log("Mouse is down");
        } 
    }

    public void displayAvailableOpponent()
    {
        if (isP1Turn)
        {
            InvocationCard[] opponentCards = P2.GetComponent<PlayerCards>().InvocationCards;
            List<Card> notEmptyOpponent = new List<Card>();
            for (int i = 0; i < opponentCards.Length; i++)
            {
                if (opponentCards[i] != null && opponentCards[i].GetNom() != null)
                {
                    notEmptyOpponent.Add(opponentCards[i]);
                }
            }

            if (notEmptyOpponent.Count == 0)
            {
                notEmptyOpponent.Add(player);
            }

            GameObject messageBox = displayOpponentMessageBox(notEmptyOpponent);
            messageBox.GetComponent<MessageBox>().positiveAction = () =>
            {
                InvocationCard invocationCard =
                    (InvocationCard) messageBox.GetComponent<MessageBox>().getSelectedCard();

                if (invocationCard != null)
                {
                    ComputeAttack(invocationCard);
                }
                Destroy(messageBox);
            };
            messageBox.GetComponent<MessageBox>().negativeAction = () =>
            {
                Destroy(messageBox);
            };

        }
        else
        {
            
        }
    }
    
    private GameObject displayOpponentMessageBox ( List<Card> invocationCards)
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

        if (opponent.GetNom() == "Player")
        {
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
                if (isP1Turn)
                {
                    P1.GetComponent<PlayerStatus>().changePV(-diff);
                    P1.GetComponent<PlayerCards>().sendCardToYellowTrash(cardSelected);
                }
                else
                {
                    P2.GetComponent<PlayerStatus>().changePV(-diff);
                    P2.GetComponent<PlayerCards>().sendCardToYellowTrash(cardSelected);
                }
            }
            else if (diff == 0)
            {
                if (isP1Turn)
                {
                    P1.GetComponent<PlayerCards>().sendCardToYellowTrash(cardSelected);
                    P2.GetComponent<PlayerCards>().sendCardToYellowTrash(opponent);
                }
                else
                {
                    P1.GetComponent<PlayerCards>().sendCardToYellowTrash(opponent);
                    P2.GetComponent<PlayerCards>().sendCardToYellowTrash(cardSelected);
                }
            }
            else
            {
                if (isP1Turn)
                {
                    P2.GetComponent<PlayerStatus>().changePV(diff);
                    P2.GetComponent<PlayerCards>().sendCardToYellowTrash(opponent);
                }
                else
                {
                    P1.GetComponent<PlayerStatus>().changePV(diff);
                    P1.GetComponent<PlayerCards>().sendCardToYellowTrash(opponent);
                }
            }
        }


    }

    private void DisplayCurrentCard(Card card)
    {
        bigImageCard.SetActive(true);
        bigImageCard.GetComponent<Image>().material = card.GetMaterialCard();
    }

    private void draw()
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

    public void nextRound()
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
