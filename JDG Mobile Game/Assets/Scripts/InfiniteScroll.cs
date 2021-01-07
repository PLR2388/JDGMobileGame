using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    //[SerializeField] private List<Card> AllCards;

    public GameObject GameState;
    // Start is called before the first frame update
    void Start()
    {
        List<Card> AllCards=GameState.GetComponent<GameState>().allCards;
       for (int i = 0; i < AllCards.Count; i++)
            {
                GameObject newCard=Instantiate(prefabCard, Vector3.zero, Quaternion.identity);

                newCard.GetComponent<OnHover>().bIsInGame = false;
                newCard.transform.SetParent(transform,true);
                newCard.GetComponent<CardDisplay>().card = AllCards[i];
            }
    }
}
