using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldFonctions : MonoBehaviour
{
    private PlayerCards currentPlayerCard;
    private GameObject P1;
    private GameObject P2;
    [SerializeField] private GameObject miniCardMenu;
    // Start is called before the first frame update
    void Start()
    {
        GameLoop.ChangePlayer.AddListener(ChangePlayer);
        InGameMenuScript.FieldCardEvent.AddListener(PutFieldCard);
        P1 = GameObject.Find("Player1");
        P2 = GameObject.Find("Player2");
        currentPlayerCard = P1.GetComponent<PlayerCards>();
    }

    private void PutFieldCard(FieldCard fieldCard)
    {
        if (currentPlayerCard.Field == null)
        {
            miniCardMenu.SetActive(false);
            currentPlayerCard.Field = fieldCard;
            currentPlayerCard.handCards.Remove(fieldCard);
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
