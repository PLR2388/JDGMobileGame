using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayCards : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    public List<Card> cardslist;
    private List<GameObject> createdCards;
    // Start is called before the first frame update
    void Start()
    {
        createdCards = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (createdCards.Count < cardslist.Count)
        {
            for (int i = 0; i < cardslist.Count; i++)
            {
                GameObject newCard=Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
                newCard.transform.SetParent(transform,true);
                newCard.GetComponent<CardDisplay>().card = cardslist[i];
                createdCards.Add(newCard);
            }
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(420 * cardslist.Count,rectTransform.sizeDelta.y);
        }
        else if (createdCards.Count > cardslist.Count)
        {
            for (int i = 0; i < createdCards.Count; i++)
            {
                Destroy(createdCards[i]);
            }
            createdCards.Clear();
        }
    }
    
    private void OnDisable()
    {
        if (createdCards.Count > 0)
        {
            for (int i = 0; i < createdCards.Count; i++)
            {
                Destroy(createdCards[i]);
            }
            createdCards.Clear();
        }
    }
}
