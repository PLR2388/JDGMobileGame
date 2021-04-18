using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFonctions : MonoBehaviour
{
    private PlayerCards currentPlayerCard;
    private GameObject P1;
    private GameObject P2;
    [SerializeField] private GameObject miniCardMenu;
    // Start is called before the first frame update
    void Start()
    {
        GameLoop.ChangePlayer.AddListener(ChangePlayer);
        InGameMenuScript.EffectCardEvent.AddListener(PutEffectCard);
        P1 = GameObject.Find("Player1");
        P2 = GameObject.Find("Player2");
        currentPlayerCard = P1.GetComponent<PlayerCards>();
    }
    
    private int FindFirstEmptyEffectLocationCurrentPlayer()
    {
        EffectCard[] effectCards = currentPlayerCard.EffectCards;
        bool end = false;
        int i = 0;
        while (i < 4 && !end)
        {
            if (effectCards[i] != null)
            {
                if (effectCards[i].Nom == null)
                {
                    end = true;
                }
                else
                {
                    i++;
                }
            }
            else
            {
                end = true;
            }
        }
        return i;
    }
    
    private void PutEffectCard(EffectCard effectCard)
    {
        int size = FindFirstEmptyEffectLocationCurrentPlayer();

        if (size < 4)
        {
            miniCardMenu.SetActive(false);
            currentPlayerCard.EffectCards[size] = effectCard;

            currentPlayerCard.handCards.Remove(effectCard);
        }
    }
    void ChangePlayer()
    {
        if (GameLoop.isP1Turn)
        {
            currentPlayerCard = P1.GetComponent<PlayerCards>();
        }
        else
        {
            currentPlayerCard = P2.GetComponent<PlayerCards>();
        }
    }
}