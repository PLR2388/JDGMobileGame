using System.Collections.Generic;
using UnityEngine;

public class DisplayCards : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    [SerializeField] public List<Card> cardsList;

    private List<GameObject> createdCards;

    // Start is called before the first frame update
    private void Start()
    {
        createdCards = new List<GameObject>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (createdCards.Count < cardsList.Count)
        {
            foreach (var card in cardsList)
            {
                var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
                newCard.transform.SetParent(transform, true);
                newCard.GetComponent<CardDisplay>().card = card;
                createdCards.Add(newCard);
            }

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(420 * cardsList.Count, rectTransform.sizeDelta.y);
        }
        else if (createdCards.Count > cardsList.Count)
        {
            foreach (var createdCard in createdCards)
            {
                Destroy(createdCard);
            }

            createdCards.Clear();
        }
    }

    private void OnDisable()
    {
        if (createdCards.Count <= 0) return;
        foreach (var createdCard in createdCards)
        {
            Destroy(createdCard);
        }

        createdCards.Clear();
    }
}