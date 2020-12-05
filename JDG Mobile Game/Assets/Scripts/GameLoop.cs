using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameLoop : MonoBehaviour
{
    public static bool isP1Turn;

    public int phaseId;

    [SerializeField] private GameObject playerText;
    [SerializeField] private GameObject roundText;
    [SerializeField] private GameObject healthText;

    [SerializeField] private GameObject P1;
    [SerializeField] private GameObject P2;
    
    public static UnityEvent ChangePlayer = new UnityEvent();

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
                break;
        }
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
