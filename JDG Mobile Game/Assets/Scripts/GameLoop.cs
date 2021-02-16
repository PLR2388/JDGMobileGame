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
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject bigImageCard;

    [SerializeField] private GameObject P1;
    [SerializeField] private GameObject P2;
    
    public static UnityEvent ChangePlayer = new UnityEvent();
    
    
    public float ClickDuration = 2;

    bool clicking = false;
    float totalDownTime = 0;
    private bool timeToChooseOpponent = false;
    private bool isCardForAttackSelected = false;
    private bool isOpponentSelected = false;
    private Card opponent;
    private Card attacker;

    // Start is called before the first frame update
    void Start()
    {
        isP1Turn = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (phaseId)
        {
            case 0: draw();
                break;
            case 1:
                break;
            case 2:
                chooseAttack();
                break;
        }
    }

    private void chooseAttack()
    {
        if (Input.GetMouseButton(0))
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
                            timeToChooseOpponent = true;
                            if (isCardForAttackSelected)
                            {
                                isCardForAttackSelected = false;
                                gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                            }
                            else
                            {
                                isCardForAttackSelected = true;
                                gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                            }
                
                        }
                    } else if (tag == "card2" && timeToChooseOpponent)
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
                            if (isOpponentSelected)
                            {
                                isOpponentSelected = false;
                                gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                            }
                            else if(!bigImageCard.activeSelf)
                            {
                                isOpponentSelected = true;
                                gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                            }
                        }

                        if (isOpponentSelected)
                        {
                            
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
        phaseId += 1;
        switch (phaseId)
        {
            case 3:
                roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pioche";break;
            case 1:
                roundText.GetComponent<TextMeshProUGUI>().text = "Phase de pose";break;
            case 2:
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
